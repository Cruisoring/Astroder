using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using QuoteHelper;
using AstroHelper;

namespace PolygonControl
{
    public class PivotSet : GraphicItemSet<PivotWrapper>
    {
        #region statics
        //public static PointF[] DefaultBottomShape = new PointF[] { 
        //    new PointF(-0.25f, 0f), new PointF(0, 0.25f), 
        //    new PointF( 0.25f, 0f), new PointF( 0.1f, 0.1f), 
        //    new PointF(0.1f, -0.25f), new PointF(-0.1f, -0.25f),
        //    new PointF(-0.1f, 0.1f), new PointF( -0.25f, 0f) };
        //public static PointF[] DefaultTopShape = new PointF[] { 
        //    new PointF(-0.25f, 0f), new PointF(0, -0.25f), 
        //    new PointF( 0.25f, 0f), new PointF( 0.1f, -0.1f), 
        //    new PointF(0.1f, 0.25f), new PointF(-0.1f, 0.25f),
        //    new PointF(-0.1f, -0.1f), new PointF( -0.25f, 0f) };

        public static Color HighlightColor = Color.FromArgb(200, Color.Yellow);

        public static Color DefaultTopFillColor = Color.FromArgb(100, Color.Maroon);
        public static Color DefaultTopBorderColor = Color.FromArgb(200, Color.Red);
        public static Color DefaultTopLeadingColor = Color.FromArgb(200, Color.Firebrick);

        public static Color DefaultBottomFillColor = Color.FromArgb(100, Color.Teal);
        public static Color DefaultBottomBorderColor = Color.FromArgb(200, Color.Green);
        public static Color DefaultBottomLeadingColor = Color.FromArgb(200, Color.Lime);

        public static PointF[] DefaultShapePoints = new PointF[] { 
            new PointF(-1f, 0f), new PointF(0, 1f), 
            new PointF( 1f, 0f), new PointF( 0f, -1f)
        };

        public static int NeighborHood = 5;
        #endregion

        #region Property

        public GraphicsPath DefaultSymbol { get; set; }

        public Brush TopBrush { get; set; }
        public Brush BottomBrush { get; set; }
        public Brush HightlightBrush { get; set; }

        public Pen TopBorderPen { get; set; }
        public Pen BottomBorderPen { get; set; }
        public Pen HightlightBorderPen { get; set; }

        public Pen TopLeadingPen { get; set; }
        public Pen BottomeLeadingPen { get; set; }
        public Pen HighlightLeadingPen { get; set; }

        public bool IsVisible { get; set; }

        public PlanetId PlanetForStudy { get; set; }

        public DateTimeOffset HoroscopeDate { get; set; }

        public PivotWrapper Ceiling { get; private set; }

        public PivotWrapper Floor { get; private set; }

        public PivotWrapper Focused { get; set; }

        #endregion

        #region Constructors

        public PivotSet(PolygonControl container)
            : base(container)
        {
            load();

            IsVisible = false;

            PlanetForStudy = PlanetId.SE_ECL_NUT;

            container.CalculatorChanged += new PolygonControl.CalculatorChangedDelegate(container_CalculatorChanged);
            container.AdapterChanged += new PolygonControl.AdapterChangedDelegate(container_AdapterChanged);
        }


        public PivotSet(PolygonControl container, Outline outline)
            : this(container)
        {
            AddPivots(outline);
        }

        #endregion

        private void load()
        {
            DefaultSymbol = new GraphicsPath();
            DefaultSymbol.AddPolygon(DefaultShapePoints);

            TopBrush = new SolidBrush(DefaultTopFillColor);
            BottomBrush = new SolidBrush(DefaultBottomFillColor);
            HightlightBrush = new SolidBrush(Color.FromArgb(100, HighlightColor));

            TopBorderPen = new Pen(Color.Red); //new Pen(DefaultTopBorderColor);
            BottomBorderPen = new Pen(DefaultBottomBorderColor);
            HightlightBorderPen = new Pen(HighlightColor);

            TopLeadingPen = new Pen(DefaultTopLeadingColor, 1);
            TopLeadingPen.DashStyle = DashStyle.Dash;
            BottomeLeadingPen = new Pen(DefaultBottomLeadingColor, 1);
            BottomeLeadingPen.DashStyle = DashStyle.Dash;
            HighlightLeadingPen = new Pen(HighlightColor, 2);
            HighlightLeadingPen.DashStyle = DashStyle.DashDotDot;

            Toolkit.Add(TopBorderPen);
            Toolkit.Add(TopBrush);
            Toolkit.Add(TopLeadingPen);
            Toolkit.Add(BottomBorderPen);
            Toolkit.Add(BottomBrush);
            Toolkit.Add(BottomeLeadingPen);
            Toolkit.Add(HighlightLeadingPen);
            Toolkit.Add(HightlightBorderPen);
            Toolkit.Add(HightlightBrush);

            //DirectFont = new Font("AstroGadget", 18f, FontStyle.Bold);
            //StarBrush = new SolidBrush(DefaultStarColor);
            //RetroBrush = new SolidBrush(DefaultRetroColor);
            //StarPen = new Pen(DefaultStarColor);
            //Toolkit.Add(DirectFont);
            //Toolkit.Add(RetroBrush);
            //Toolkit.Add(StarBrush);
            //Toolkit.Add(StarPen);
        }

        public void AddPivots(Outline outline)
        {
            Items.Clear(); 
            
            PivotWrapper newPivot = null;

            foreach(OutlineItem pivot in outline.Pivots)
            {
                if (pivot.Type == PivotType.Bottom)
                    newPivot = new PivotWrapper(Container, DefaultSymbol, BottomBrush, BottomBorderPen, BottomeLeadingPen, pivot);
                else
                    newPivot = new PivotWrapper(Container, DefaultSymbol, TopBrush, TopBorderPen, TopLeadingPen, pivot);

                Add(newPivot);
            }

            Ceiling = Floor = Items[0];

            for (int i = 1; i < Count; i ++ )
            {
                if (Ceiling.Pivot.Price < Items[i].Pivot.Price)
                    Ceiling = Items[i];

                if (Floor.Pivot.Price > Items[i].Pivot.Price)
                    Floor = Items[i];
            }

            HoroscopeDate = Items[0].Pivot.Date;
        }

        public void Draw(Graphics g)
        {
            if (PlanetForStudy != PlanetId.SE_ECL_NUT)
            {
                foreach (PivotWrapper pivot in Items)
                {
                    if (pivot.Pivot.Date == HoroscopeDate)
                        continue;

                    Position starPos = Ephemeris.CurrentEphemeris[pivot.Pivot.Date, PlanetForStudy];
                    float angle = (float)(starPos.Longitude);
                    PointF start = Container.RelativeToCenter(3, angle, 0);
                    PointF end = Container.RelativeToCenter(Ruler.RulerRadius(Container, 1), angle, 0);

                    g.DrawLine(pivot.LeadingPen, start, end);
                }
            }
            else if (IsVisible)
            {
                foreach (PivotWrapper pivot in Items)
                {
                    pivot.Draw(g);
                }
            }

            //if (DisplayHoroscope)
            //{
            //    PlanetEvents detail;
            //    foreach (KeyValuePair<PlanetId, String> kvp in StarSymbols)
            //    {
            //        Position starPos = Ephemeris.CurrentEphemeris[Date, kvp.Key];
            //        String symbol = kvp.Value;
            //        SizeF size = g.MeasureString(symbol, DirectFont);
            //        float angleLow = (float)(starPos.Longitude);
            //        float radius;

            //        detail = new PlanetEvents(Ephemeris.CurrentEphemeris, Date, kvp.Key);

            //        if (detail.LongitudeStatus != RectascensionMode.None || detail.LatitudeStatus != DeclinationMode.None || detail.DistanceStatus != DistanceMode.None)
            //            radius = Ruler.RulerRadius(Container, 1.5f);
            //        else
            //            radius = Ruler.RulerRadius(Container, 0.5f);

            //        PointF pos = Container.RelativeToCenter(radius, angleLow, 0);

            //        pos = new PointF(pos.X - size.Width / 2, pos.Y - size.Height / 2);
            //        g.DrawString(symbol, DirectFont, starPos.LongitudeVelocity >= 0 ? StarBrush : RetroBrush, pos);

            //        PointF start = Container.RelativeToCenter(Ruler.RulerRadius(Container, 1), angleLow, 0);
            //        PointF end = Container.RelativeToCenter(Ruler.RulerRadius(Container, 0), angleLow, 0);
            //        g.DrawLine(StarPen, start, end);
            //    }
            //}
        }

        void container_AdapterChanged()
        {
            if (Count != 0)
                Calculate();
        }

        void container_CalculatorChanged()
        {
            if(Count != 0)
                Calculate();
        }

        public PivotWrapper NearestPivot(float angle, float distance)
        {
            //float sectorAngle = 180f / (Container.Calculator.Edges);
            List<PivotWrapper> around = (from pivot in Items
                                         where Math.Abs(pivot.Angle - angle) < 45f && Math.Abs(pivot.Distance - distance) < 0.5
                                         select pivot).ToList();

            PivotWrapper nearest = null;
            float minPow = float.MaxValue, angleDif, difPow, cos;

            float distancePow = distance * distance;

            foreach (PivotWrapper pivot in around)
            {
                angleDif = angle - pivot.Angle;
                cos = PolygonControl.COS(angleDif);
                difPow = distancePow + pivot.Distance * pivot.Distance - 2 * distance * pivot.Distance * cos;
                if (difPow < minPow)
                {
                    minPow = difPow;
                    nearest = pivot;
                }
            }

            if (Math.Sqrt(minPow) < PivotWrapper.DefaultSymbolSize)
                return nearest;
            else
                return null;
        }


        public void Calculate()
        {
            foreach (PivotWrapper item in Items)
            {
                item.ReCalculate();
            }
        }
    }
}
