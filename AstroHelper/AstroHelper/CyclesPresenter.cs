using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using NumberHelper;
using NumberHelper.DoubleHelper;
using QuoteHelper;
using ZedGraph;
using Color = System.Drawing.Color;

namespace AstroHelper
{
    [Serializable]
    public class CyclesPresenter
    {
        #region Static ones

        public static PlanetId OwnerOf(char symbol)
        {
            foreach (KeyValuePair<PlanetId, char> kvp in Planet.Symbols)
            {
                if (kvp.Value == symbol)
                    return kvp.Key;
            }

            return PlanetId.SE_ECL_NUT;
        }

        public static PlanetId OwnerOf(CurveItem curve)
        {
            char symbol = curve.Label.Text[0];

            return OwnerOf(symbol);
        }

        private static OrbitInfoType kindOf(string kindStr)
        {
            if (kindStr == null || kindStr.Length == 0)
                return OrbitInfoType.Longitude;
            else
            {
                foreach (KeyValuePair<OrbitInfoType, string> kvp in OrbitSpec.OrbitTypeAbbrv)
                {
                    if (kvp.Value == kindStr)
                        return kvp.Key;
                }

                return OrbitInfoType.Other;
            }
        }

        public static OrbitInfoType KindOf(CurveItem curve)
        {
            return kindOf(curve.Label.Text.Substring(1, curve.Label.Text.Length - 1));
        }


        #endregion

        #region Fields
        public ZedGraphControl Zed { get; set; }
        private GraphPane thePane { get { return Zed.GraphPane; } }

        private OrbitsCollection geoOrbits = null;
        public OrbitsCollection GeocentricOrbits
        {
            get
            {
                if (geoOrbits == null)
                {
                    DateTimeOffset endDate = (DateTimeOffset.Now - History.Until).TotalDays <= 100 ? 
                        new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).AddDays(50) : History.Until.AddDays(30);
                    geoOrbits = new OrbitsCollection(History.Since.AddDays(-30), endDate, 
                            Ephemeris.Geocentric);
                    //geoOrbits = (History.Until.Year != DateTimeOffset.UtcNow.Year) ? new OrbitsCollection(History.Since, History.Until, Ephemeris.Geocentric) :
                    //    new OrbitsCollection(History.Since.AddDays(-30), new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).AddDays(30),
                    //        Ephemeris.Geocentric);
                }
                return geoOrbits;
            }
        }

        private OrbitsCollection helioOrbits = null;
        public OrbitsCollection HeliocentricOrbits
        {
            get
            {
                if (helioOrbits == null)
                {
                    helioOrbits = new OrbitsCollection(History.Since.AddDays(-30), new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).AddDays(50),
                            Ephemeris.Heliocentric);
                    //helioOrbits = (History.Until.Year != DateTimeOffset.UtcNow.Year) ? new OrbitsCollection(History.Since, History.Until, Ephemeris.Heliocentric) :
                    //    new OrbitsCollection(History.Since.AddDays(-30), new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).AddDays(30),
                    //        Ephemeris.Heliocentric);
                }
                return helioOrbits;
            }
        }

        public OrbitsCollection Orbits { get; set; }

        public QuoteCollection History { get; set; }

        public AngleFrame Frame { get; set; }

        public TimeAngleConverter Clock { get; set; }

        public Dictionary<String, TimeAngleConverter> ClocksDict { get; private set; }

        public Dictionary<String, PriceAdapter> PriceDict { get; private set; }

        public PriceAdapter PriceTranslator { get; set; }

        public Polygon CycleMapper { get; set; }

        //public Dictionary<PlanetId, Dictionary<OrbitInfoType, OrbitSpec>> Curves = null;

        public List<OrbitSpec> PlusOrbits { get; set; }

        public List<OrbitSpec> MinusOrbits { get; set; }

        public Angle Shift {get; set; }
        private List<double> timeScope
        {
            get
            {
                return new List<double>() { thePane.XAxis.Scale.Min, thePane.XAxis.Scale.Max };
            }
        }

        #endregion

        #region Constructor

        public CyclesPresenter(ZedGraphControl control, QuoteCollection history)
        {
            Zed = control;
            History = history;
            Orbits = GeocentricOrbits;

            Frame = AngleFrame.Natural;
            Clock = TimeAngleConverter.SolarClock;

            ClocksDict = new Dictionary<string, TimeAngleConverter>();

            DateTimeOffset floorDate = History.DataCollection[History.FloorIndex].Date;
            DateTimeOffset ceilingDate = History.DataCollection[History.CelingIndex].Date;

            ClocksDict.Add("FloorCeiling", new TimeAngleConverter(floorDate, Math.Abs((floorDate-ceilingDate).TotalDays)) );
            ClocksDict.Add("FirstNatural", new TimeAngleConverter(History.Since));
            ClocksDict.Add("First360", new TimeAngleConverter(History.Since, 360));
            ClocksDict.Add("LowestNatural", new TimeAngleConverter(floorDate));
            ClocksDict.Add("Lowest360", new TimeAngleConverter(floorDate, 360));
            ClocksDict.Add("HighestNatural", new TimeAngleConverter(ceilingDate));
            ClocksDict.Add("Highest360", new TimeAngleConverter(ceilingDate, 360));

            PriceDict = new Dictionary<string, PriceAdapter>();
            double floor = History.Floor;
            double ceiling = History.Ceiling;

            for (PriceMappingRules rule = PriceMappingRules.Filled; rule <= PriceMappingRules.FloorStep; rule ++)
            {
                PriceDict.Add(rule.ToString(), new PriceAdapter(rule, floor, ceiling));
            }

            //Curves = new Dictionary<PlanetId, Dictionary<OrbitInfoType, OrbitSpec>>();

            //for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
            //{
            //    Curves.Add(id, new Dictionary<OrbitInfoType, OrbitSpec>());
            //}

            InitiateGraphControl();
            Zed.ZoomEvent += new ZedGraphControl.ZoomEventHandler(Control_ZoomEvent);

            PlusOrbits = new List<OrbitSpec>();
            MinusOrbits = new List<OrbitSpec>();
        }

        #endregion

        #region Functions

        public void InitiateGraphControl()
        {
            Zed.IsShowVScrollBar = false;
            Zed.IsShowHScrollBar = true;
            Zed.IsAutoScrollRange = true;

            thePane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 210, 255), -45F);
            // Disable the Titles
            thePane.Title.IsVisible = false;
            thePane.Legend.FontSpec.Size = 8;

            thePane.XAxis.Type = AxisType.Date;
            thePane.XAxis.Title.IsVisible = false;
            thePane.XAxis.Scale.Min = Orbits.Start.UtcDateTime.ToOADate();
            thePane.XAxis.Scale.Max = Orbits.End.UtcDateTime.ToOADate();
            thePane.XAxis.Scale.FontSpec.Size = 8;
            thePane.XAxis.MajorGrid.IsVisible = true;
            thePane.XAxis.MajorGrid.Color = Color.LightGray;

            thePane.XAxis.Scale.Format = "yyyy-MMdd";

            //thePane.XAxis.Scale.Format = History.ActivedDateFormat;
            thePane.XAxis.Scale.MinAuto = false;
            thePane.XAxis.Scale.MaxAuto = false;
            

            thePane.YAxis.Title.IsVisible = false;
            //thePane.YAxis.Title.Text = "Quadranted Degrees";
            //thePane.YAxis.Title.FontSpec.FontColor = Color.Green;
            thePane.YAxis.Title.FontSpec.Size = 8;
            thePane.YAxis.Scale.FontSpec.Size = 8;
            thePane.YAxis.Scale.FontSpec.FontColor = Color.Green;
            thePane.YAxis.Scale.Align = AlignP.Inside;
            thePane.YAxis.MajorTic.IsOpposite = false;
            thePane.YAxis.MinorTic.IsOpposite = false;
            thePane.YAxis.Scale.MagAuto = false;
            thePane.YAxis.MajorGrid.IsVisible = true;
            thePane.YAxis.MajorGrid.Color = Color.LightGray;
            thePane.YAxis.MajorGrid.IsZeroLine = false;
            thePane.YAxis.Scale.MinAuto = false;
            thePane.YAxis.Scale.MaxAuto = false;
            thePane.YAxis.Scale.FormatAuto = true;
            ResetYScale();

            thePane.Y2Axis.IsVisible = false;
            thePane.Y2Axis.MajorGrid.IsZeroLine = false;
            thePane.Y2Axis.Title.Text = "Latitude";
            thePane.Y2Axis.Title.FontSpec.Size = 8;
            thePane.Y2Axis.Title.FontSpec.FontColor = Color.DarkGray;
            thePane.Y2Axis.Scale.FontSpec.Size = 8;
            thePane.Y2Axis.Scale.FontSpec.FontColor = Color.DarkGray;
            thePane.Y2Axis.MajorTic.IsOpposite = false;
            thePane.Y2Axis.MinorTic.IsOpposite = false;
            thePane.Y2Axis.Scale.Align = AlignP.Inside;
            thePane.Y2Axis.MajorGrid.IsZeroLine = true;
            thePane.Y2Axis.Scale.MinAuto = false;
            thePane.Y2Axis.Scale.MaxAuto = false;
            thePane.Y2Axis.Scale.FormatAuto = true;
            thePane.Y2Axis.Scale.Max = 0.00001;
            thePane.Y2Axis.Scale.Min = -0.00001;
            thePane.Y2Axis.Scale.MajorStepAuto = false;
            thePane.Y2Axis.Scale.MinorStepAuto = false;
            thePane.Y2Axis.Scale.FormatAuto = true;
            thePane.Y2Axis.Scale.MagAuto = false;

            Y2Axis yAxis3 = new Y2Axis("Speed/Distance");
            yAxis3.IsVisible = false;
            thePane.Y2AxisList.Add(yAxis3);
            yAxis3.MajorGrid.IsZeroLine = false;
            yAxis3.Title.FontSpec.Size = 8;
            yAxis3.Title.FontSpec.FontColor = Color.DarkCyan;
            yAxis3.Scale.FontSpec.Size = 8;
            yAxis3.Scale.FontSpec.FontColor = Color.DarkCyan;
            yAxis3.MajorTic.IsOpposite = false;
            yAxis3.MinorTic.IsOpposite = false;
            yAxis3.Scale.Align = AlignP.Inside;
            yAxis3.MajorGrid.IsZeroLine = true;
            yAxis3.Scale.MinAuto = false;
            yAxis3.Scale.MaxAuto = false;
            yAxis3.Scale.FormatAuto = true;
            yAxis3.Scale.Max = 0.00001;
            yAxis3.Scale.Min = -0.00001;
            //yAxis3.Scale.MajorStep = 2;
            yAxis3.Scale.MajorStepAuto = false;
            yAxis3.Scale.MinorStepAuto = false;
            yAxis3.Scale.MagAuto = true;

            thePane.AxisChange();
        }

        public void ResetYScale()
        {
            thePane.YAxis.Scale.Max = Frame.Max;
            thePane.YAxis.Scale.Min = Frame.Min;
            thePane.YAxis.Scale.MajorStep = Frame.Step;
            thePane.YAxis.Scale.MinorStep = (Frame.Step%4==0) ? Frame.Step/4 : (Frame.Step % 3 == 0 ? Frame.Step/3 : 5);
            thePane.AxisChange();
        }

        public void SetY2Scale()
        {
            Y2Axis yAxis2 = thePane.Y2Axis;

            var y2CurveQuery = from curve in thePane.CurveList
                               where curve.IsY2Axis && curve.GetYAxisIndex(thePane) == 0
                               select curve;
            List<CurveItem> y2Curves = y2CurveQuery.ToList();

            if (y2Curves.Count == 0)
            {
                yAxis2.IsVisible = false;
                yAxis2.Scale.Max = 0.00001;
                yAxis2.Scale.Min = -0.00001;
            }
            else
            {
                yAxis2.IsVisible = true;
                //yAxis2.Scale.IsVisible = true;
                //yAxis2.Title.IsVisible = true;

                double max = 0;
                double xMin, xMax, yMin, yMax;

                foreach (CurveItem curve in y2Curves)
                {
                    curve.GetRange(out xMin, out xMax, out yMin, out yMax, false, false, thePane);
                    max = Math.Max(max, Math.Abs(yMin));
                    max = Math.Max(max, Math.Abs(yMax));
                }

                SetY2Scale(max);
            }
        }

        public void SetY2Scale(double max)
        {
            Y2Axis yAxis2 = thePane.Y2Axis;

            if (max == 0)
            {
                yAxis2.IsVisible = false;
                yAxis2.Scale.Max = 0.00001;
                yAxis2.Scale.Min = -0.00001;
                return;
            }

            yAxis2.Scale.Max = gracefulOf(Math.Abs(max), false);
            yAxis2.Scale.Min = -yAxis2.Scale.Max;

            double mag = Math.Floor(Math.Log10(yAxis2.Scale.Max) - 1);
            Double step = Math.Pow(10, mag);
            Double multiples = yAxis2.Scale.Max / step;

            if (multiples >= 50)
            {
                yAxis2.Scale.MinorStep = 5 * step;
                yAxis2.Scale.MajorStep = Math.Floor(multiples / 20) * 10 * step;
            }
            else if (multiples >= 20)
            {
                yAxis2.Scale.MinorStep = 2 * step;
                yAxis2.Scale.MajorStep = Math.Floor(multiples / 10) * 5 * step;
            }
            else
            {
                yAxis2.Scale.MajorStep = step;
                yAxis2.Scale.MinorStep = Math.Floor(multiples / 10) * 5 * step;
            }
        }

        public void SetY3Scale()
        {
            Y2Axis yAxis3 = thePane.Y2AxisList[1];

            var y3CurveQuery = from curve in thePane.CurveList
                               where curve.IsY2Axis && curve.GetYAxisIndex(thePane) == 1
                               select curve;
            List<CurveItem> y3Curves = y3CurveQuery.ToList();

            if (y3Curves.Count == 0)
            {
                yAxis3.IsVisible = false;
                yAxis3.Scale.Max = 0.00001;
                yAxis3.Scale.Min = -0.00001;
            }
            else 
            {
                yAxis3.IsVisible = true;
                //yAxis3.Scale.IsVisible = true;
                //yAxis3.Title.IsVisible = true;

                double max = 0;
                double xMin, xMax, yMin, yMax;

                foreach (CurveItem curve in y3Curves)
                {
                    curve.GetRange(out xMin, out xMax, out yMin, out yMax, false, false, thePane);
                    max = Math.Max(max, Math.Abs(yMin));
                    max = Math.Max(max, Math.Abs(yMax));
                }

                SetY3Scale(max);
            }
        }

        public void SetY3Scale( double max)
        {
            Y2Axis yAxis3 = thePane.Y2AxisList[1];

            if (max == 0)
            {
                yAxis3.IsVisible = false;
                yAxis3.Scale.Max = 0.00001;
                yAxis3.Scale.Min = -0.00001;
                return;
            }

            yAxis3.Scale.Max = gracefulOf(Math.Abs(max), false);
            yAxis3.Scale.Min = -yAxis3.Scale.Max;

            double mag = Math.Floor(Math.Log10(yAxis3.Scale.Max) - 1);
            Double step = Math.Pow(10, mag);
            Double multiples = yAxis3.Scale.Max / step;

            if (multiples >= 50)
            {
                yAxis3.Scale.MinorStep = 5 * step;
                yAxis3.Scale.MajorStep = Math.Floor(multiples / 20) * 10 * step;
            }
            else if (multiples > 20)
            {
                yAxis3.Scale.MinorStep = 2 * step;
                yAxis3.Scale.MajorStep = Math.Floor(multiples / 10) * 5 * step;
            }
            else
            {
                yAxis3.Scale.MinorStep = step;
                yAxis3.Scale.MajorStep = Math.Floor(multiples / 10) * 5 * step;
            }
        }

        public OrbitSpec this[PlanetId id, OrbitInfoType kind]
        {
            get
            {
                return Orbits[id, kind];
            }
        }

        //public OrbitSpec this[PlanetId id, OrbitInfoType kind]
        //{
        //    get
        //    {
        //        if (!Curves.ContainsKey(id))
        //            throw new Exception();

        //        if (!Curves[id].ContainsKey(kind))
        //        {
        //            Curves[id].Add(kind, new OrbitSpec(Orbits, id, kind));
        //        }

        //        return Curves[id][kind];
        //    }
        //}
        public void Control_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            sender.GraphPane.YAxis.Scale.Max = Frame.Max;
            sender.GraphPane.YAxis.Scale.Min = Frame.Min;
            //sender.GraphPane.Y2Axis.Scale.Max = 15;
            //sender.GraphPane.Y2Axis.Scale.Min = -15;
            SetY2Scale();
            SetY3Scale();
            sender.AxisChange();
            sender.Invalidate();
        }

        public bool IsCurveAdded(string label)
        {
            foreach (CurveItem curve in thePane.CurveList)
            {
                if (curve.Label.Text == label)
                    return true;
            }

            return false;
        }

        public bool IsCurveAdded(PlanetId id, OrbitInfoType kind)
        {
            String label = OrbitSpec.LableOf(id, kind);
            return IsCurveAdded(label);
        }

        private CurveItem combinedCurveOf(List<double> xList)
        {
            if (MinusOrbits.Count == 0 && PlusOrbits.Count == 0)
            {
                return null;
            }

            List<double> yList = new List<double>();
            double total;

            foreach (double x in xList)
            {
                total = Shift.Degrees;
                foreach (OrbitSpec orbit in PlusOrbits)
                {
                    total += orbit[x];
                }
                foreach (IOrbitable orbit in MinusOrbits)
                {
                    total -= orbit[x];
                }
                total = total.Normalize();
                yList.Add(total);
            }

            return Frame.CurveOf("*", xList, yList, Color.Orange, SymbolType.None);
        }

        private List<double> getCombinedDegrees(List<double> xList)
        {
            List<double> result = new List<double>();
            double total = 0;

            foreach (double x in xList)
            {
                total = 0;
                foreach (OrbitSpec orbit in PlusOrbits)
                {
                    total += orbit[x];
                }
                foreach(IOrbitable orbit in MinusOrbits)
                {
                    total -= orbit[x];
                }
                total = total.Normalize();
                result.Add(total);
            }

            return result;
        }

        private List<double> getCombinedDateValues()
        {
            if (MinusOrbits.Count == 0 && PlusOrbits.Count == 0)
                return new List<double>() { thePane.XAxis.Scale.Min, thePane.XAxis.Scale.Max };
            else if (MinusOrbits.Count != 0)
                return MinusOrbits[0].XList;
            else
                return PlusOrbits[0].XList;
        }

        public void AddCurve(String name)
        {
            if (IsCurveAdded(name))
                RemoveCurveOf(name);

            switch (name[0])
            {
                case 'A':
                    if (Clock != null)
                    {
                        LineItem clockLine = Frame.CurveOf(Clock, Orbits.Start, Orbits.End);

                        if (clockLine != null)
                            AddCurve(clockLine);
                    }
                    break;
                case '$':
                    if (PriceTranslator != null)
                    {
                        LineItem priceLine;

                        if (PriceTranslator.Rule != PriceMappingRules.Filled)
                            priceLine = Frame.CurveOf(PriceTranslator, History.CurrentOutline.PivotDates, History.CurrentOutline.PivotValues, CycleMapper, true);
                        else
                            priceLine = Frame.CurveOf(PriceTranslator, History.Dates, History.OutlineValues, CycleMapper, true);

                        if (priceLine != null)
                            AddCurve(priceLine);
                    }
                    break;
                case '+':
                    if (Shift != null)
                    {
                        LineItem shiftLine = Frame.CurveOf("+", timeScope, new List<double> { Shift.Degrees, Shift.Degrees }, Color.DarkCyan, SymbolType.None);
                        AddCurve(shiftLine);
                    }
                    break;
                case '*':
                    if (Shift == null)
                        break;
                    else if (MinusOrbits.Count != 0 || PlusOrbits.Count != 0)
                    {
                        List<Double> xList = getCombinedDateValues();

                        if (xList != null)
                        {
                            CurveItem curve = combinedCurveOf(xList);
                            AddCurve(curve);
                        }
                        else
                            throw new Exception();
                    }
                    break;
                default:
                    PlanetId id = OwnerOf(name[0]);
                    OrbitInfoType kind = kindOf(name.Substring(1, name.Length - 1));
                    AddCurve(id, kind);
                    break;
            }
            Zed.Invalidate();
        }

        public void AddCurve(CurveItem curve)
        {
            if (!IsCurveAdded(curve.Label.Text))
            {
                PlanetId id = OwnerOf(curve.Label.Text[0]);
                int i = 0;
                for (; i < thePane.CurveList.Count; i++)
                {
                    CurveItem existed = thePane.CurveList[i];
                    PlanetId starId = OwnerOf(existed);
                    OrbitInfoType starKind = KindOf(existed);
                    if (starId > id)
                        continue;
                    else if (starId < id)
                        break;
                    //else if (starKind < kind)
                    //    continue;
                    //else if (starKind > kind)
                    //    break;
                }
                thePane.CurveList.Insert(i, curve);
                Zed.Invalidate();
            }
        }

        public void AddCurve(PlanetId id, OrbitInfoType kind)
        {
            if (!IsCurveAdded(id, kind))
            {
                OrbitSpec spec = this[id, kind];

                CurveItem curve = null;

                if (kind == OrbitInfoType.Longitude || (kind >= OrbitInfoType.Ascending && kind <= OrbitInfoType.Apogee))
                    curve = Frame.CurveOf(spec);
                else if (kind.ToString().EndsWith("Latitude"))
                {
                    curve = spec.OriginalCurve;
                    curve.IsY2Axis = true;
                    thePane.Y2Axis.IsVisible = true;

                    Double max = Math.Max(Math.Abs(spec.MaxY), Math.Abs(spec.MinY));
                    if (max > thePane.Y2Axis.Scale.Max)
                        SetY2Scale(max);
                }
                else
                {
                    thePane.Y2AxisList[1].IsVisible = true;

                    curve = spec.OriginalCurve;
                    curve.IsY2Axis = true;
                    curve.YAxisIndex = 1;

                    Double max = Math.Max(Math.Abs(spec.MaxY), Math.Abs(spec.MinY));
                    if (max > thePane.Y2AxisList[1].Scale.Max)
                        SetY3Scale(max);
                }

                AddCurve(curve);
            }
        }

        public void AddFilledCurve(String name, List<double> xItems, List<double> yItems, Color color)
        {
            if (yItems.Count != xItems.Count)
                throw new Exception();

            Double max = yItems.Max();
            Double min = yItems.Min();

            max = gracefulOf(max, false);
            min = gracefulOf(min, true);
            double temp, range = max - min;
            List<double> converted = new List<double>();

            for (int i = 0; i < yItems.Count; i ++ )
            {
                temp = (yItems[i] - min) * Frame.Range / range + Frame.Min;
                converted.Add(temp);
            }

            CurveItem curve = thePane.AddCurve(name, xItems.ToArray(), converted.ToArray(), color, SymbolType.None);
            Zed.Invalidate();
        }

        private Double gracefulOf(double price, bool smaller)
        {
            double mag = Math.Floor(Math.Log10(price) - 1);
            double step = Math.Pow(10, mag);

            return Math.Round(smaller ? Math.Floor(price / step) : Math.Ceiling(price / step), 1) * step;
        }

        public bool RemoveCurveOf(PlanetId id, OrbitInfoType kind)
        {
            String label = OrbitSpec.LableOf(id, kind);

            return RemoveCurveOf(label);
        }

        public bool RemoveCurveOf(String label)
        {
            CurveItem toBeDropped = null;
            foreach (CurveItem curve in thePane.CurveList)
            {
                if (curve.Label.Text != label)
                    continue;

                toBeDropped = curve;
                break;
            }

            if (toBeDropped == null)
                return false;
            else
            {
                thePane.CurveList.Remove(toBeDropped);

                if (toBeDropped.IsY2Axis)
                {
                    int index = toBeDropped.GetYAxisIndex(thePane);
                    if (index == 0)
                    {
                        SetY2Scale();
                    }
                    else if (index == 1)
                    {
                        SetY3Scale();
                    }
                }

                Zed.Invalidate();
                return true;
            }
        }

        public void RedrawYValues()
        {
            ResetYScale();

            List<String> toBeRedrawed = new List<String>();

            foreach (CurveItem curve in thePane.CurveList)
            {
                if (curve.GetYAxis(thePane) == thePane.YAxis)
                    toBeRedrawed.Add(curve.Label.Text);
            }

            foreach (string s in toBeRedrawed)
            {
                AddCurve(s);
            }
            //List<int> longCurves = new List<int>();

            //for(int i = 0; i < thePane.CurveList.Count; i ++)
            //{
            //    CurveItem curve = thePane.CurveList[i];
            //    OrbitInfoType kind = KindOf(curve);
            //    if (kind == OrbitInfoType.Longitude)
            //        longCurves.Add(i);
            //}

            //foreach(int i in longCurves)
            //{
            //    PlanetId id = OwnerOf(thePane.CurveList[i]);
            //    OrbitInfoType kind = KindOf(thePane.CurveList[i]);

            //    if (id >= PlanetId.SE_SUN && id <= PlanetId.SE_PLUTO)
            //    {
            //        thePane.CurveList.RemoveAt(i);
            //        AddFilledCurve(id, kind);
            //    }
            //}

            //if (IsCurveAdded("O"))
            //{
            //    RemoveCurveOf("O");
            //    AddFilledCurve("O");
            //}

            //if (IsCurveAdded("T"))
            //{
            //    RemoveCurveOf("T");
            //    AddFilledCurve("T");
            //}

            //if (IsCurveAdded("$"))
            //{
            //    RemoveCurveOf("$");
            //    AddFilledCurve("$");
            //}
        }

        #endregion
    }
}
