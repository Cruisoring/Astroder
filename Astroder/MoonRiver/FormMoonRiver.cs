using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using EphemerisCalculator;
using AstroCalendarControl;

namespace MoonRiver
{
    public partial class FormMoonRiver : Form
    {
        public static TimeSpan SamplingInterval = TimeSpan.FromMinutes(1);

        DateTimeOffset intradaySince, intradayUntil, interdaySince, interdayUntil;

        private Dictionary<PlanetId, List<double>> orbits = new Dictionary<PlanetId, List<double>>();
        private Dictionary<PlanetId, List<double>> expandedOrbits = new Dictionary<PlanetId, List<double>>();
        private Dictionary<CurveItem, PlanetId> curvesDict = new Dictionary<CurveItem, PlanetId>();

        List<double> xValues = new List<double>();

        ArrowObj nowArrow = null;

        public FormMoonRiver()
        {
            InitializeComponent();

            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset thisHour = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Offset);
            intradaySince = thisHour.AddHours(-24);
            intradayUntil = thisHour.AddHours(24);
            interdaySince = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero).AddMonths(-1);
            interdayUntil = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero).AddMonths(1);


            InitiateGraphControl(geoIntraDay, intradaySince, intradayUntil);
            InitiateGraphControl(geoInterday, interdaySince, interdayUntil);


            addIntraDayCurves();

            markPlanetEvent();

            initializeEventsTab();
        }

        private void initializeEventsTab()
        {
            astroCalendar1.PeriodEventsCollectedEvent += new Action<AstroCalendar>(astroCalendar2.EnableOrDisable);
            astroCalendar2.PeriodEventsCollectedEvent += new Action<AstroCalendar>(astroCalendar3.EnableOrDisable);
            astroCalendar3.PeriodEventsCollectedEvent += new Action<AstroCalendar>(astroCalendar4.EnableOrDisable);

            timer1_Tick(this, null);
        }

        private void markPlanetEvent()
        {
            List<IPlanetEvent> events = Ephemeris.Geocentric[intradaySince, intradayUntil];

            IPlanetEvent lunarAspect = null;
            double x;

            foreach (PlanetId id in Ephemeris.Geocentric.Luminaries)
            {
                if (id == PlanetId.SE_MOON || id > PlanetId.SE_FICT_OFFSET)
                    continue;

                lunarAspect = Ephemeris.Geocentric.ExactAspectEventOf(PlanetId.SE_MOON, id, intradaySince);

                if (lunarAspect != null && lunarAspect.When >= intradaySince && lunarAspect.When <= intradayUntil)
                    events.Add(lunarAspect);
            }

            events.Sort();

            foreach (IPlanetEvent evt in events)
            {
                x = evt.When.LocalDateTime.ToOADate();
                ArrowObj newArrow = new ArrowObj(x, 0, x, geoIntraDay.GraphPane.YAxis.Scale.Max / 10);
                newArrow.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
                newArrow.IsArrowHead = false;
                geoIntraDay.GraphPane.GraphObjList.Add(newArrow);

                string description = evt.ShortDescription;

                TextObj text = new TextObj(description, x, 0);
                text.FontSpec.Size = 6;
                text.FontSpec.Angle = 45;
                text.FontSpec.Family = "AstroSymbols";
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill.IsVisible = false;
                text.Location.AlignH = AlignH.Right;
                text.Location.AlignV = AlignV.Center;
                geoIntraDay.GraphPane.GraphObjList.Add(text);
            }
        }

        private void addIntraDayCurves()
        {
            geoIntraDay.GraphPane.CurveList.Clear();
            curvesDict.Clear();

            getOrbits();

            foreach (KeyValuePair<PlanetId, List<double>> kvp in orbits)
            {
                String name = Planet.PlanetOf(kvp.Key).Name;
                Color color = Planet.PlanetsColors.ContainsKey(kvp.Key) ? Planet.PlanetsColors[kvp.Key].First() : Color.Gray;

                LineItem line = new LineItem(name, xValues.ToArray(), expandedOrbits[kvp.Key].ToArray(), color, SymbolType.None);
                geoIntraDay.GraphPane.CurveList.Add(line);
                curvesDict.Add(line, kvp.Key);
            }

            geoIntraDay.Invalidate();
        }

        private void getOrbits()
        {
            List<DateTimeOffset> timeValues = new List<DateTimeOffset>();

            DateTimeOffset time = intradaySince;
            xValues.Clear();

            do 
            {
                timeValues.Add(time);
                time += SamplingInterval;
                xValues.Add(time.DateTime.ToOADate());
            } while (time <= intradayUntil);

            orbits.Clear();

            List<double> orbit = null;
            List<double> expanded = null;
            Position pos = null;

            foreach (PlanetId id in Ephemeris.Geocentric.Luminaries)
            {
                orbit = new List<double>();
                expanded = new List<double>();

                foreach(DateTimeOffset moment in timeValues)
                {
                    pos = Ephemeris.GeocentricPositionOf(moment, id);
                    orbit.Add(pos.Longitude);
                    expanded.Add((pos.Longitude % 30) * 12);
                }

                orbits.Add(id, orbit);
                expandedOrbits.Add(id, expanded);
            }
        }

        public void InitiateGraphControl(ZedGraphControl zed, DateTimeOffset since, DateTimeOffset until)
        {
            #region set the graphGeocentric display characters
            zed.IsShowVScrollBar = false;
            zed.IsShowHScrollBar = true;
            zed.IsAutoScrollRange = true;

            zed.GraphPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 210, 255), -45F);

            // Disable the Title and Legend
            zed.GraphPane.Title.IsVisible = false;
            zed.GraphPane.Legend.IsVisible = false;

            //graphGeocentric.GraphPane.XAxis.Scale.Format = History.ActivedDateFormat;
            zed.GraphPane.XAxis.Scale.MinAuto = false;  
            zed.GraphPane.XAxis.Scale.MaxAuto = false;

            zed.GraphPane.XAxis.Title.IsVisible = false;
            zed.GraphPane.XAxis.Type = AxisType.Date;
            zed.GraphPane.XAxis.Scale.FontSpec.Size = 8;
            zed.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zed.GraphPane.XAxis.MajorGrid.Color = Color.LightGray;
            zed.GraphPane.XAxis.Scale.Min = since.DateTime.ToOADate();
            zed.GraphPane.XAxis.Scale.Max = until.DateTime.ToOADate();
            zed.GraphPane.XAxis.Scale.Format = "HH:mm";

            zed.GraphPane.YAxis.Title.IsVisible = false;
            //graphGeocentric.GraphPane.YAxis.Title.Text = "Quadranted Degrees";
            //graphGeocentric.GraphPane.YAxis.Scale.FontSpec.FontColor = Color.Green;
            zed.GraphPane.YAxis.Scale.Align = AlignP.Inside;
            zed.GraphPane.YAxis.MajorTic.IsOpposite = false;
            zed.GraphPane.YAxis.MinorTic.IsOpposite = false;
            zed.GraphPane.YAxis.Scale.FontSpec.Size = 6;
            zed.GraphPane.YAxis.Scale.MagAuto = false;
            zed.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zed.GraphPane.YAxis.MajorGrid.Color = Color.LightGray;
            zed.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            zed.GraphPane.YAxis.Scale.MinAuto = false;
            zed.GraphPane.YAxis.Scale.MaxAuto = false;
            zed.GraphPane.YAxis.Scale.Min = 0;
            zed.GraphPane.YAxis.Scale.Max = 360;
            zed.GraphPane.YAxis.Scale.MajorStep = 30;
            zed.GraphPane.YAxis.Scale.MinorStep = 5;
            zed.GraphPane.YAxis.Scale.FormatAuto = true;

            zed.GraphPane.Y2Axis.IsVisible = true;
            zed.GraphPane.Y2Axis.MajorGrid.IsZeroLine = false;
            zed.GraphPane.Y2Axis.Title.IsVisible = false;
            zed.GraphPane.Y2Axis.Scale.FontSpec.Size = 6;
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.Family = "AstroSymbols";
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.FontColor = Color.DarkGray;
            //graphGeocentric.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
            //graphGeocentric.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
            //graphGeocentric.GraphPane.Y2Axis.Scale.Align = AlignP.Inside;
            zed.GraphPane.Y2Axis.Type = AxisType.Text;
            zed.GraphPane.Y2Axis.Scale.TextLabels = (from sign in Sign.All.Values
                                                                     select sign.Symbol.ToString()).ToArray();
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.Angle = 40;
            zed.GraphPane.Y2Axis.MajorTic.IsBetweenLabels = true;

            zed.GraphPane.AxisChange();
            #endregion
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double x = DateTime.Now.ToOADate();

            if (nowArrow != null && geoIntraDay.GraphPane.GraphObjList.Contains(nowArrow))
            {
                geoIntraDay.GraphPane.GraphObjList.Remove(nowArrow);
            }

            nowArrow = new ArrowObj(x, geoIntraDay.GraphPane.YAxis.Scale.Min, x, geoIntraDay.GraphPane.YAxis.Scale.Max);
            nowArrow.IsArrowHead = false;
            nowArrow.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;

            geoIntraDay.GraphPane.GraphObjList.Add(nowArrow);

            geoIntraDay.Invalidate();
        }

        private string graphGeocentric_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PlanetId id = curvesDict[curve];

            double longitude = orbits[id][iPt];
            DateTimeOffset time = new DateTimeOffset(DateTime.FromOADate(xValues[iPt]));

            return String.Format("{0}: {1}@{2:F2}", time, Planet.PlanetOf(id), longitude);
        }


    }
}
