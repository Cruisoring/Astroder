using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EphemerisCalculator;

namespace PolygonControl
{
    public class PlanetSet : AngledItemSet
    {
        #region statics

        public static Color DefaultStarColor = Color.FromArgb(200, Color.Green);
        public static Color DefaultRetroColor = Color.FromArgb(200, Color.Orange);

        public static Dictionary<PlanetId, String> StarSymbols = new Dictionary<PlanetId, String>(){
            { PlanetId.SE_SUN, "s"},
            { PlanetId.SE_MOON, "d"},
            { PlanetId.SE_MERCURY, "f"},
            { PlanetId.SE_VENUS, "g"},
            { PlanetId.SE_MARS, "h"},
            { PlanetId.SE_JUPITER, "j"},
            { PlanetId.SE_SATURN, "S"},
            { PlanetId.SE_URANUS, "G"},
            { PlanetId.SE_NEPTUNE, "F"},
            { PlanetId.SE_PLUTO, "J"},
            //{ PlanetId.SE_EARTH, "O"},
            { PlanetId.SE_NORTHNODE, "k"},
            { PlanetId.SE_CHIRON, "D"}
            //{ PlanetId.SE_SUN, "A"},
            //{ PlanetId.SE_MOON, "B"},
            //{ PlanetId.SE_MERCURY, "C"},
            //{ PlanetId.SE_VENUS, "D"},
            //{ PlanetId.SE_MARS, "E"},
            //{ PlanetId.SE_JUPITER, "F"},
            //{ PlanetId.SE_SATURN, "G"},
            //{ PlanetId.SE_URANUS, "H"},
            //{ PlanetId.SE_NEPTUNE, "I"},
            //{ PlanetId.SE_PLUTO, "J"},
            ////{ PlanetId.SE_EARTH, "O"},
            //{ PlanetId.SE_NORTHNODE, "L"},
            //{ PlanetId.SE_CHIRON, "K"}
        };

        #endregion

        #region Properties

        public Font DirectFont { get; set; }
        public Font RetroFont { get; set; }
        //public Brush StarBrush { get; set; }
        //public Brush RetroBrush { get; set; }
        public Pen StarPen { get; set; }

        private DateTimeOffset date;
        public DateTimeOffset Date 
        { 
            get{ return date; }
            set
            {
                //if (value == date)
                //    return;

                date = value;

                float angle, distance;
                if (Positions == null)
                {
                    Positions = new Dictionary<PlanetId, AngledLabel>();
                    Longitudes = new Dictionary<PlanetId, double>();

                    foreach (KeyValuePair<PlanetId, String> kvp in StarSymbols)
                    {
                        Position starPos = Ephemeris.CurrentEphemeris[Date, kvp.Key];
                        String symbol = kvp.Value;

                        Container.Calculator.AngleDistanceOf(starPos.Longitude, out angle, out distance);

                        SolidBrush starBrush = new SolidBrush(Planet.PlanetsColors[kvp.Key][0]);
                        Toolkit.Add(starBrush);

                        AngledLabel starLable = new AngledLabel(Container, symbol, starPos.LongitudeVelocity >= 0 ? DirectFont : RetroFont,
                            starBrush, distance, angle);
                        Positions.Add(kvp.Key, starLable);
                        Longitudes.Add(kvp.Key, starPos.Longitude);
                    }
                }
                else
                {
                    foreach (KeyValuePair<PlanetId, AngledLabel>kvp in Positions)
                    {
                        Position starPos;

                        if (value.Offset == TimeSpan.Zero && value.TimeOfDay == TimeSpan.Zero)
                            starPos = Ephemeris.CurrentEphemeris[Date, kvp.Key];
                        else
                            starPos = Utilities.GeocentricPositionOf(value, kvp.Key);

                        Container.Calculator.AngleDistanceOf(starPos.Longitude, out angle, out distance);
                        kvp.Value.Distance = distance;
                        kvp.Value.Angle = angle;
                        kvp.Value.LabelFont = starPos.LongitudeVelocity >= 0 ? DirectFont : RetroFont;
                        Longitudes[kvp.Key] = starPos.Longitude;
                    }
                }
            }
        }

        public bool IsVisible { get; set; }

        public Dictionary<PlanetId, AngledLabel> Positions;

        public Dictionary<PlanetId, double> Longitudes;

        #endregion


        #region Constructors

        public PlanetSet(PolygonControl container) : base(container)
        {
            DirectFont = new Font("StarFont Sans", 16f, FontStyle.Bold);
            RetroFont = new Font("StarFont Sans", 16f, FontStyle.Italic | FontStyle.Underline);
            //StarBrush = new SolidBrush(DefaultStarColor);
            //RetroBrush = new SolidBrush(DefaultRetroColor);
            StarPen = new Pen(DefaultStarColor);

            Toolkit.Add(DirectFont);
            Toolkit.Add(RetroFont);
            //Toolkit.Add(RetroBrush);
            //Toolkit.Add(StarBrush);
            Toolkit.Add(StarPen);

            Date = new DateTimeOffset(DateTime.UtcNow.Date);
            IsVisible = true;

            container.CalculatorChanged += new PolygonControl.CalculatorChangedDelegate(container_CalculatorChanged);

        }

        #endregion

        #region Functions

        void container_CalculatorChanged()
        {
            Date = date;
        }

        public override void Draw(Graphics g)
        {
            if (IsVisible)
            {
                PlanetEvents detail;
                foreach (KeyValuePair<PlanetId, AngledLabel> kvp in Positions)
                {
                    Position starPos = Ephemeris.CurrentEphemeris[Date, kvp.Key];
                    String symbol = StarSymbols[kvp.Key];

                    float angle = (float)(starPos.Longitude);
                    float radius;

                    detail = new PlanetEvents(Ephemeris.CurrentEphemeris, Date, kvp.Key);

                    if (detail.LongitudeStatus != RectascensionMode.None || detail.LatitudeStatus != DeclinationMode.None || detail.DistanceStatus != DistanceMode.None)
                        radius = Ruler.RulerRadius(Container, 1.5f);
                    else
                        radius = Ruler.RulerRadius(Container, 0.5f);

                    AngledLabel outPos = new AngledLabel(Container, symbol, kvp.Value.LabelFont, kvp.Value.FontBrush, radius, angle);
                    outPos.Draw(g);

                    PointF start = Container.RelativeToCenter(Ruler.RulerRadius(Container, 1.5f), angle, 0);
                    PointF end = Container.RelativeToCenter(Ruler.RulerRadius(Container, 0.5f), angle, 0);
                    g.DrawLine(StarPen, start, end);

                    kvp.Value.Draw(g);
                }                
            }

        }

        public PlanetId NearestPlanet(float angle)
        {
            List<KeyValuePair<PlanetId, double>> around =
                (from kvp in Longitudes
                 let dif = Math.Abs(kvp.Value - angle)
                 where dif < 1
                 orderby dif
                 select kvp
                ).ToList();

            if (around.Count == 0)
                return PlanetId.SE_ECL_NUT;
            else
                return around.First().Key;
        }

        public PlanetId NearestPlanet(float angle, float distance)
        {
            //float sectorAngle = 180f / (Container.Calculator.Edges);
            List<KeyValuePair<PlanetId, AngledLabel>> around = (from kvp in Positions
                                        let label = kvp.Value
                                         where Math.Abs(label.Angle - angle) < 5f && Math.Abs(label.Distance - distance) < 0.5
                                         select kvp).ToList();

            if (around.Count == 0)
                return PlanetId.SE_ECL_NUT;

            PlanetId nearest = (from kvp in around
                                     let dist = Container.DistanceBetween(distance, angle, kvp.Value.Distance, kvp.Value.Angle)
                                     orderby dist
                                     where dist <= DirectFont.Height / 2f
                                     select kvp.Key).FirstOrDefault();

            return nearest;
        }

        #endregion
    }
}
