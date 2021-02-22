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

namespace AstroClock
{
    //public delegate void PlanetEventDisplayChanged(PlanetEventFlag category, bool display);

    public partial class MainForm : Form
    {
        #region Constants and static variables definitions
        private const int defaultSymbolSize = 3;
        private static AspectImportance defaultAspectImportance = AspectImportance.Important;

        private static SeFlg CentricFlag = SeFlg.GEOCENTRIC;

        private static Dictionary<PlanetEventFlag, SymbolType> defaultEventSymbolType = new Dictionary<PlanetEventFlag, SymbolType>{
            { PlanetEventFlag.EclipseOccultationCategory, SymbolType.Diamond},
            { PlanetEventFlag.SignChangedCategory, SymbolType.Plus},
            { PlanetEventFlag.DeclinationCategory, SymbolType.Triangle},
            { PlanetEventFlag.DirectionalCategory, SymbolType.VDash},
            { PlanetEventFlag.AspectCategory, SymbolType.Square}
        };

        private static List<PlanetEventFlag> concernedEventCategories = new List<PlanetEventFlag>();

        #endregion

        #region Variables

        private DateTimeOffset since = new DateTimeOffset(2010, 10, 1, 0, 0, 0, TimeSpan.Zero);
        private DateTimeOffset until = new DateTimeOffset(2012, 5, 1, 0, 0, 0, TimeSpan.Zero);

        private PlanetId concernedPlanet = PlanetId.SE_ECL_NUT;

        public PlanetId ConcernedPlanet
        {
            get { return concernedPlanet; }
            set 
            {
                if(ConcernedPlanet != value)
                {
                    concernedPlanet = value;

                    foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> planetKvp in TheAspectarian[PlanetEventFlag.AspectCategory])
                    {
                        CurveItem old = this[planetKvp.Key, PlanetEventFlag.AspectCategory];
                        if (zedLongTerm.GraphPane.CurveList.Contains(old))
                            zedLongTerm.GraphPane.CurveList.Remove(old);

                        this[planetKvp.Key, PlanetEventFlag.AspectCategory] = eventCurveOf(planetKvp.Key, PlanetEventFlag.AspectCategory, planetKvp.Value);
                    }

                    displayCurves();
                }
            }
        }

        private PlanetId FocusedPlanet = PlanetId.SE_ECL_NUT;

        //Contains the original geocentric planet orbit data, sorted by PositionValueIndex + PlanetId
        private Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> geoOrbitsDict = null;

        //Contains the original heliocentric planet orbit data, sorted by PositionValueIndex + PlanetId
        private Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> helioOrbitsDict = null;

        private Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> geoAspectarian = null;

        private Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> helioAspectarian = null;        


        //For each Planet orbits, a set of curveItems are stored with offset degrees
        private Dictionary<PlanetId, Dictionary<int, CurveItem>> geoPlanetsCurves = new Dictionary<PlanetId, Dictionary<int, CurveItem>>(); 

        //For each Planet orbits, a set of curveItems are stored with offset degrees
        private Dictionary<PlanetId, Dictionary<int, CurveItem>> helioPlanetsCurves = new Dictionary<PlanetId, Dictionary<int, CurveItem>>();

        #endregion

        #region Properties

        public Ephemeris CurrentEphemeris  
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? Ephemeris.Geocentric : Ephemeris.Heliocentric; } 
        }

        private GraphObjList currentIndicators = new GraphObjList();

        public Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> OrbitsDict 
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? geoOrbitsDict : helioOrbitsDict; }
            set
            {
                if (CentricFlag == SeFlg.GEOCENTRIC)
                    geoOrbitsDict = value;
                else
                    helioOrbitsDict = value;
            }
        }

        public Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> TheAspectarian
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? geoAspectarian : helioAspectarian; }
            set
            {
                if (CentricFlag == SeFlg.GEOCENTRIC)
                    geoAspectarian = value;
                else
                    helioAspectarian = value;
            }
        }

        public Dictionary<PlanetId, Dictionary<int, CurveItem>> Curves 
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? geoPlanetsCurves : helioPlanetsCurves; }
        }

        public CurveItem EclipsesCurve
        {
            get
            {
                return Curves.ContainsKey(PlanetId.SE_SUN) && Curves[PlanetId.SE_SUN].ContainsKey((int)PlanetEventFlag.EclipseOccultationCategory)
                    ? Curves[PlanetId.SE_SUN][(int)PlanetEventFlag.EclipseOccultationCategory] : null;
            }
            set
            {
                if (!Curves.ContainsKey(PlanetId.SE_SUN))
                    Curves.Add(PlanetId.SE_SUN, new Dictionary<int, CurveItem>());

                if (!Curves[PlanetId.SE_SUN].ContainsKey((int)PlanetEventFlag.EclipseOccultationCategory))
                    Curves[PlanetId.SE_SUN].Add((int)PlanetEventFlag.EclipseOccultationCategory, null);

                Curves[PlanetId.SE_SUN][(int)PlanetEventFlag.EclipseOccultationCategory] = value;
            }
        }

        public CurveItem this[PlanetId id, PlanetEventFlag category]
        {
            get 
            {
                if (!Curves.ContainsKey(id) || !Curves[id].ContainsKey((int)category))
                    return null;
                return Curves[id][(int)category];
            }
            set
            {
                if (!Curves.ContainsKey(id))
                    Curves.Add(id, new Dictionary<int, CurveItem>());

                if (!Curves[id].ContainsKey((int)category))
                    Curves[id].Add((int)category, null);

                Curves[id][(int)category] = value;
            }
        }

        #endregion

        #region Constructor

        public MainForm()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            InitializeComponent();

            DateTimeOffset time = dateTimePicker1.Value;
            since = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, TimeSpan.Zero);
            time = dateTimePicker2.Value;
            until = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, TimeSpan.Zero);

            initiateOrbitsGraph(zedLongTerm);

            foreach (PlanetId id in CurrentEphemeris.Luminaries)
            {
                comboFocusedStar.Items.Add(Planet.PlanetOf(id).Name);
            }

            //Set the x scale to concerned period
            setDuration(zedLongTerm, since, until);

            CentricFlag = (radioButtonGeocentric.Checked) ? SeFlg.GEOCENTRIC : SeFlg.HELIOCENTRIC;
            checkBoxSun.Text = CentricFlag == SeFlg.GEOCENTRIC ? "Sun" : "Earth";

            comboFocusedStar.Items.Clear();
            comboConcernedPlanet.Items.Clear();
            FocusedPlanet = PlanetId.SE_ECL_NUT;
            foreach (PlanetId id in CurrentEphemeris.Luminaries)
            {
                comboFocusedStar.Items.Add(Planet.PlanetOf(id).Name);

                if (id <= PlanetId.SE_PLUTO)
                    comboConcernedPlanet.Items.Add(Planet.PlanetOf(id).Name);
            }
            comboFocusedStar.SelectedIndex = -1;
            comboConcernedPlanet.Items.Add("Any");

            displayCurves();

            if (checkBoxNowTimeline.Checked)
            {
                timer1.Enabled = true;
                timer1_Tick(null, null);
            }

            zedLongTerm.AxisChange();
        }

        #endregion

        #region Curve related functions

        private bool addAspectedOrbits(PlanetId id, int offset)
        {
            if (Curves[id].ContainsKey(offset))
                return false;

            List<double> original = OrbitsDict[PositionValueIndex.Longitude][id];
            List<double> shifted = new List<double>();
            List<double> xValues = new List<double>();

            String name = String.Format("{0}+{1}", Planet.Glyphs[id], offset) ;
            Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].Last() : Color.Gray;
            double x = since.DateTime.ToOADate();
            foreach (double y0 in original)
            {
                xValues.Add(x++);
                shifted.Add((y0 + offset) % 360);
            }

            LineItem line = new LineItem(name, xValues.ToArray(), shifted.ToArray(), color, SymbolType.None);

            Curves[id].Add(offset, line);
            return true;
        }

        private bool removeAspectedOrbits(ZedGraphControl zed, PlanetId id, int offset)
        {
            if (!Curves[id].ContainsKey(offset))
                return false;

            String name = String.Format("{0}+{1}", Planet.Glyphs[id], offset);

            if (zed.GraphPane.CurveList.Contains(Curves[id][offset]))
            {
                zed.GraphPane.CurveList.Remove(Curves[id][offset]);
            }

            Curves[id].Remove(offset);
            for (int i = 0; i < zed.GraphPane.GraphObjList.Count; i ++ )
            {
                TextObj text = zed.GraphPane.GraphObjList[i] as TextObj;
                if (text == null || text.Text != name)
                    continue;

                zed.GraphPane.GraphObjList.Remove(text);
                break;
            }
            return true;
        }

        private TextObj lableOf(string name, double x, double y)
        {
            TextObj label = null;

            label = new TextObj(name, x, y);
            label.Location.AlignV = AlignV.Center;

            if (Planet.GlyphToPlanetId.ContainsKey(name[0]))
            {
                PlanetId id = Planet.GlyphToPlanetId[name[0]];
                label.FontSpec.FontColor = Planet.PlanetsColors[id][0];
            }
            else
                label.FontSpec.FontColor = Color.Gray;

            label.Location.AlignH = AlignH.Left;
            label.FontSpec.Fill.IsVisible = false;
            label.FontSpec.Border.IsVisible = false;
            label.FontSpec.Size = 6f;
            return label;
        }

        private void showCurve(ZedGraphControl zed, CurveItem curve, bool withGlyph)
        {
            zed.GraphPane.CurveList.Add(curve);

            LineItem line = (LineItem)curve;
            if (withGlyph && line != null)
            {
                double x = zed.GraphPane.XAxis.Scale.Max; 

                int closestIndex = Math.Max(0, (int)(x - since.DateTime.ToOADate()));

                for (int i = closestIndex; i <= line.NPts; i++)
                {
                    if (line[i].X < x)
                        continue;

                    closestIndex = i;
                    break;
                }
                double y = line[closestIndex].Y;
                TextObj label = lableOf(curve.Label.Text, x, y);
                label.FontSpec.Size = 7;
                zed.GraphPane.GraphObjList.Add(label);
            }
        }

        private void removeCurve(ZedGraphControl zed, CurveItem curve)
        {
            zed.GraphPane.CurveList.Remove(curve);

            List<TextObj> toBeRemoved = new List<TextObj>();
            
            foreach (GraphObj obj in zed.GraphPane.GraphObjList)
            {
                if (obj is TextObj)
                {
                    TextObj text = obj as TextObj;
                    if (text.Text[0] == curve.Label.Text[0])
                        toBeRemoved.Add(obj as TextObj);
                }
            }

            foreach (TextObj text in toBeRemoved)
            {
                    zed.GraphPane.GraphObjList.Remove(text);
            }
        }

        private void hidePlanetCurves(PlanetId id)
        {
            foreach (KeyValuePair<int, CurveItem> kvp in Curves[id])
            {
                if (zedLongTerm.GraphPane.CurveList.Contains(kvp.Value))
                    removeCurve(zedLongTerm, kvp.Value);
            }
        }

        private void showPlanetCurves(PlanetId id)
        {
            foreach (KeyValuePair<int, CurveItem> kvp in Curves[id])
            {
                LineItem line = kvp.Value as LineItem;

                if (line == null)
                    continue;

                if (line.Symbol.Type == SymbolType.None && !zedLongTerm.GraphPane.CurveList.Contains(line))
                    showCurve(zedLongTerm, line, true);
                else if (line.Symbol.Type != SymbolType.None)
                {
                    PlanetEventFlag category = (PlanetEventFlag)kvp.Key;

                    if (concernedEventCategories.Contains(category) && !zedLongTerm.GraphPane.CurveList.Contains(line))
                        showCurve(zedLongTerm, line, false);
                    else if (!concernedEventCategories.Contains(category) && zedLongTerm.GraphPane.CurveList.Contains(line))
                        removeCurve(zedLongTerm, line);
                }
            }
        }

        /// <summary>
        /// Display the selected curves.
        /// </summary>
        private void displayCurves()
        {
            foreach (Control starControl in panelStars.Controls)
            {
                if (starControl is CheckBox)
                {
                    CheckBox cb = starControl as CheckBox;
                    int index = panelStars.Controls.IndexOf(starControl);
                    PlanetId id = CurrentEphemeris.Luminaries[index];
                    char symbol = Planet.Glyphs[id];

                    if (cb.Checked)
                    {
                        showPlanetCurves(id);
                    }
                    else
                    {
                        hidePlanetCurves(id);
                    }
                }
            }
            zedLongTerm.Invalidate();
        }

        private CurveItem eventCurveOf(PlanetId id, PlanetEventFlag category, List<IPlanetEvent> events)
        {
            List<double> yValues = new List<double>();
            List<double> xValues = new List<double>();

            String name = String.Format("{0}-{1}", Planet.Glyphs[id], category);
            Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

            foreach (IPlanetEvent evt in events)
            {
                if (category == PlanetEventFlag.AspectCategory)
                {
                    ExactAspectEvent aspEvent = evt as ExactAspectEvent;
                    if (aspEvent != null && aspEvent.TheAspect.Importance < defaultAspectImportance)
                        continue;
                    else if (concernedPlanet != PlanetId.SE_ECL_NUT && aspEvent.Interior != concernedPlanet && aspEvent.Exterior != concernedPlanet)
                        continue;
                }
                yValues.Add(evt.Where.Longitude);
                xValues.Add(evt.When.DateTime.ToOADate());
            }

            LineItem line = new LineItem(name, xValues.ToArray(), yValues.ToArray(), color, defaultEventSymbolType[category], 0);
            line.Symbol.Size = defaultSymbolSize;
            if (category == PlanetEventFlag.EclipseOccultationCategory)
                line.Symbol.Fill = new Fill(Color.Black);
            return line;
        }

        private void getEventCurves()
        {
            foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> kvp in TheAspectarian)
            {
                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> planetKvp in kvp.Value)
                {
                    this[planetKvp.Key, kvp.Key] = eventCurveOf(planetKvp.Key, kvp.Key, planetKvp.Value);
                }
            }
        }

        #endregion

        #region Initialize functions

        /// <summary>
        /// Set the ZedGraphControl's x and y scale.
        /// </summary>
        /// <param name="zed">the ZedGraphControl</param>
        private void initiateOrbitsGraph(ZedGraphControl zed)
        {
            #region set the graphGeocentric display characters
            zed.IsShowVScrollBar = false;
            zed.IsShowHScrollBar = true;
            zed.IsAutoScrollRange = true;

            // Disable the Title and Legend
            zed.GraphPane.Title.IsVisible = false;
            zed.GraphPane.Legend.IsVisible = false;

            //graphGeocentric.GraphPane.XAxis.Scale.Format = History.ActivedDateFormat;
            zed.GraphPane.XAxis.Scale.MinAuto = false;
            zed.GraphPane.XAxis.Scale.MaxAuto = false;
            zed.GraphPane.XAxis.Title.IsVisible = false;
            zed.GraphPane.XAxis.Type = AxisType.Date;
            //zed.GraphPane.XAxis.Scale.Format = "yyyy/MM/dd";
            zed.GraphPane.XAxis.Scale.FormatAuto = true;
            zed.GraphPane.XAxis.Scale.FontSpec.Size = 6;
            zed.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zed.GraphPane.XAxis.MajorGrid.Color = Color.LightGray;

            zed.GraphPane.YAxis.Title.IsVisible = false;
            zed.GraphPane.YAxis.Scale.Align = AlignP.Inside;
            zed.GraphPane.YAxis.MajorTic.IsOpposite = false;
            zed.GraphPane.YAxis.MinorTic.IsOpposite = false;
            zed.GraphPane.YAxis.Scale.FontSpec.Size = 6;
            zed.GraphPane.YAxis.Scale.MagAuto = false;
            zed.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zed.GraphPane.YAxis.MajorGrid.Color = Color.LightGray;
            zed.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            zed.GraphPane.YAxis.Scale.MinAuto = true;
            zed.GraphPane.YAxis.Scale.MaxAuto = true;
            zed.GraphPane.YAxis.Scale.Min = 0;
            zed.GraphPane.YAxis.Scale.Max = 360;
            zed.GraphPane.YAxis.Scale.MajorStep = 30;
            zed.GraphPane.YAxis.Scale.MinorStep = 5;
            zed.GraphPane.YAxis.Scale.FormatAuto = true;

            //zed.GraphPane.Y2Axis.IsVisible = true;
            //zed.GraphPane.Y2Axis.MajorGrid.IsZeroLine = false;
            //zed.GraphPane.Y2Axis.Title.IsVisible = false;
            //zed.GraphPane.Y2Axis.Scale.FontSpec.Size = 6;
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.Family = "AstroSymbols";
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.FontColor = Color.DarkGray;
            //graphGeocentric.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
            //graphGeocentric.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
            //graphGeocentric.GraphPane.Y2Axis.Scale.Align = AlignP.Inside;
            //zed.GraphPane.Y2Axis.Type = AxisType.Text;
            //zed.GraphPane.Y2Axis.Scale.TextLabels = (from sign in Sign.All.Values
            //                                         select sign.Symbol.ToString()).ToArray();
            //graphGeocentric.GraphPane.Y2Axis.Scale.FontSpec.Angle = 40;
            //zed.GraphPane.Y2Axis.MajorTic.IsBetweenLabels = true;

            zed.GraphPane.AxisChange();
            #endregion
        }

        //private List<double> smoothingOf(List<double> original, int offset)
        //{
        //    List<double> result = new List<double>();
        //    result.Add(original[0]);
        //    double next, last, dif;

        //    for (int i = 1; i < original.Count; i ++ )
        //    {
        //        last = original[i - 1];
        //        next = original[i];
        //        dif = next - last;

        //        if (dif > 30 || dif < -30)
        //        {
        //            dif = Math.Round(dif);
        //            next = (next - dif + 360) % 360;
        //        }

        //        result.Add(next);
        //    }

        //    return result;
        //}

        private void populateAspectarian(Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> aspectarian)
        {
            foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in aspectarian)
            {
                if (category.Key == PlanetEventFlag.EclipseOccultationCategory)
                {
                    if (!category.Value.ContainsKey(PlanetId.SE_MOON))
                        category.Value.Add(PlanetId.SE_MOON, new List<IPlanetEvent>());

                    if (!category.Value.ContainsKey(PlanetId.SE_SUN))
                        category.Value.Add(PlanetId.SE_SUN, new List<IPlanetEvent>());

                    List<IPlanetEvent> lunarEclipse = new List<IPlanetEvent>(category.Value[PlanetId.SE_MOON]);

                    category.Value[PlanetId.SE_MOON].AddRange(category.Value[PlanetId.SE_SUN]);
                    category.Value[PlanetId.SE_SUN].AddRange(lunarEclipse);
                    category.Value[PlanetId.SE_MOON].Sort();
                    category.Value[PlanetId.SE_SUN].Sort();


                    if (category.Value[PlanetId.SE_MOON].Count == 0)
                        category.Value.Remove(PlanetId.SE_MOON);

                    if (category.Value[PlanetId.SE_SUN].Count == 0)
                        category.Value.Remove(PlanetId.SE_SUN);

                }
                //else if (category.Key == PlanetEventFlag.AspectCategory)
                //{
                //    Dictionary<PlanetId, List<IPlanetEvent>> mirrored = new Dictionary<PlanetId, List<IPlanetEvent>>();

                //    foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in category.Value )
                //    {
                //        foreach (IPlanetEvent evt in kvp.Value)
                //        {
                //            ExactAspectEvent aspect = evt as ExactAspectEvent;

                //            if (aspect == null)
                //                throw new Exception("Unexpected IPlanetEvent which is expected to be ExactAspectEvent: " + evt.ToString());

                //            if (!mirrored.ContainsKey(aspect.Exterior))
                //                mirrored.Add(aspect.Exterior, new List<IPlanetEvent>());

                //            mirrored[aspect.Exterior].Add(aspect);
                //        }
                //    }

                //    foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in mirrored )
                //    {
                //        if (!category.Value.ContainsKey(kvp.Key))
                //        {
                //            category.Value.Add(kvp.Key, mirrored[kvp.Key]);
                //        }
                //        else
                //        {
                //            category.Value[kvp.Key].AddRange(mirrored[kvp.Key]);
                //        }
                //        category.Value[kvp.Key].Sort();
                //    }
                //}
            }
        }

        private void setDuration(ZedGraphControl zed, DateTimeOffset since, DateTimeOffset until)
        {
            double min = zed.GraphPane.XAxis.Scale.Min = since.DateTime.ToOADate();
            double max = zed.GraphPane.XAxis.Scale.Max = until.DateTime.ToOADate();

            List<SeFlg> centricFlags = new List<SeFlg> { SeFlg.GEOCENTRIC, SeFlg.HELIOCENTRIC };
            List<double> xValues = new List<double>();

            for (double x = min; x <= max; x++)
            {
                xValues.Add(x);
            }

            for( int i = zed.GraphPane.CurveList.Count-1; i >= 0; i--)
            {
                CurveItem curve = zed.GraphPane.CurveList[i];
                removeCurve(zedLongTerm, curve);
            }

            foreach (SeFlg flag in centricFlags)
            {
                CentricFlag = flag;

                #region Get the longitude curves
                OrbitsDict = CurrentEphemeris.AllOrbitsCollectionDuring(since, until);
                TheAspectarian = CurrentEphemeris.AspectarianDuring(since, until, defaultAspectImportance);
                //populateAspectarian(TheAspectarian);

                //Special treatment of average longDif by replacing them with the smoothed version without gap generated when a planet enters Aries
                for (int i = CurrentEphemeris.Luminaries.IndexOf(PlanetId.Five_Average); i < CurrentEphemeris.Luminaries.Count; i++)
                {
                    PlanetId id = CurrentEphemeris.Luminaries[i];
                    List<double> beforeShift = OrbitsDict[PositionValueIndex.Longitude][id];
                    List<double> afterShift = Ephemeris.SmoothingOfAverage(beforeShift);
                    OrbitsDict[PositionValueIndex.Longitude].Remove(id);
                    OrbitsDict[PositionValueIndex.Longitude].Add(id, afterShift);
                }
                #endregion

                Curves.Clear();

                foreach (KeyValuePair<PlanetId, List<double>> kvp in OrbitsDict[PositionValueIndex.Longitude])
                {
                    String name = Planet.Glyphs[kvp.Key].ToString();
                    Color color = Planet.PlanetsColors.ContainsKey(kvp.Key) ? Planet.PlanetsColors[kvp.Key].First() : Color.Gray;
                    LineItem line = null;

                    List<IPlanetEvent> signChanges = CurrentEphemeris[since, until, PlanetEventFlag.SignChangedCategory, kvp.Key];

                    if (signChanges != null && signChanges.Count != 0)
                    {
                        List<double> finalYs = new List<double>(kvp.Value);
                        List<double> finalXs = new List<double>(xValues);

                        for (int i = signChanges.Count - 1; i >= 0; i--)
                        {
                            SignEntrance change = signChanges[i] as SignEntrance;

                            double x = (change.When - since).TotalDays;
                            double y = Math.Round(change.Where.Longitude);

                            int insertPos = (int)x + 1;

                            if (y != 0 && y != 360)
                            {
                                finalXs.Insert(insertPos, x + min);
                                finalYs.Insert(insertPos, y);
                            }
                            else
                            {
                                finalXs.Insert(insertPos, x + min);
                                finalXs.Insert(insertPos, x + min);
                                finalYs.Insert(insertPos, change.IsRetrograde ? 360 : 0);
                                finalYs.Insert(insertPos, change.IsRetrograde ? 0 : 360);
                            }

                        }

                        line = new LineItem(name, finalXs.ToArray(), finalYs.ToArray(), color, SymbolType.None);
                    }
                    else
                    {
                        line = new LineItem(name, xValues.ToArray(), kvp.Value.ToArray(), color, SymbolType.None);
                    }

                    Curves.Add(kvp.Key, new Dictionary<int, CurveItem> { { 0, line } });
                }
                getEventCurves();
            }

        }

        #endregion

        #region Current Highlight functions

        /// <summary>
        /// Highlight the interiorPos position of selected planets or averages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentIndicators.Count != 0)
            {
                foreach(GraphObj obj in currentIndicators)
                {
                    zedLongTerm.GraphPane.GraphObjList.Remove(obj);                    
                }
                currentIndicators.Clear();
            }

            #region Draw the line to highlight interiorPos planet positions
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string timeString = DateTimeOffset.Now.ToString("MM-dd HH:mm");

            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, TimeSpan.Zero);

            double x = now.DateTime.ToOADate();

            LineObj nowIndicator = new LineObj(x, zedLongTerm.GraphPane.YAxis.Scale.Min - 10, x, zedLongTerm.GraphPane.YAxis.Scale.Max + 10);
            nowIndicator.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            nowIndicator.Line.Color = Color.Gray;
            currentIndicators.Add(nowIndicator);

            #region With the lable of local time under the line

            TextObj timeIndicator = new TextObj(timeString, x, 0, CoordType.AxisXYScale);
            timeIndicator.Location.AlignH = AlignH.Center;
            timeIndicator.Location.AlignV = AlignV.Top;
            timeIndicator.FontSpec.Fill.Color = Color.Yellow;
            timeIndicator.FontSpec.Fill.IsVisible = true;
            timeIndicator.FontSpec.Border.IsVisible = true;
            timeIndicator.FontSpec.Size = 6f;
            currentIndicators.Add(timeIndicator);

            #endregion


            if (checkBoxReadings.Checked)
            {
                for (int i = 0; i < CurrentEphemeris.Luminaries.Count; i++)
                {
                    CheckBox cb = panelStars.Controls[i] as CheckBox;
                    Position pos = null;

                    if (cb != null && cb.Checked)
                    {
                        PlanetId id = CurrentEphemeris.Luminaries[i];
                        string label = null;
                        double y = 180;

                        if (id < PlanetId.SE_FICT_OFFSET)
                        {
                            pos = CurrentEphemeris[now, id];
                            y = pos.Longitude;
                            label = string.Format("{0}: {1} ({2})", Planet.Glyphs[id], pos.Longitude.ToString("F1"), Rectascension.AstroStringOf(pos.Longitude));
                        }
                        else if (OrbitsDict[PositionValueIndex.Longitude].ContainsKey(id))
                        {
                            int todayIndex = (int)Math.Floor(x - since.DateTime.ToOADate());
                            double y1 = OrbitsDict[PositionValueIndex.Longitude][id][todayIndex];
                            double y2 = OrbitsDict[PositionValueIndex.Longitude][id][todayIndex + 1];
                            y = y1 + (y2 - y1) / (x - since.DateTime.ToOADate());

                            label = string.Format("{0}: {1:F1} ({2})", Planet.Glyphs[id], y, Rectascension.AstroStringOf(y));
                        }

                        TextObj posText = lableOf(label, x, y);
                        posText.Location.AlignV = y < 180 ? AlignV.Bottom : AlignV.Top;
                        currentIndicators.Add(posText);
                    }
                }
            }

            #endregion

            foreach (GraphObj obj in currentIndicators)
            {
                zedLongTerm.GraphPane.GraphObjList.Add(obj);
            }

            zedLongTerm.Invalidate();

        }

        #endregion

        #region Events handlers

        #region Centric & Duration & Time settings

        private void radioCentric_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGeocentric.Checked)
            {
                CentricFlag = SeFlg.GEOCENTRIC;
                if (checkBoxSun.Text == "Earth")
                    checkBoxSun.Text = "Sun";
            }
            else
            {
                CentricFlag = SeFlg.HELIOCENTRIC;

                if (checkBoxSun.Text == "Sun")
                    checkBoxSun.Text = "Earth";
            }

            comboFocusedStar.Items.Clear();
            comboConcernedPlanet.Items.Clear();
            FocusedPlanet = PlanetId.SE_ECL_NUT;
            foreach (PlanetId id in CurrentEphemeris.Luminaries)
            {
                comboFocusedStar.Items.Add(Planet.PlanetOf(id).Name);
                if (id <= PlanetId.SE_PLUTO)
                    comboConcernedPlanet.Items.Add(Planet.PlanetOf(id).Name);
            }
            comboFocusedStar.SelectedIndex = -1;
            comboConcernedPlanet.Items.Add("Any");
            comboConcernedPlanet.SelectedIndex = comboConcernedPlanet.Items.Count - 1;

            foreach (Control control in panelAspects.Controls)
            {
                if (control is CheckBox)
                {
                    CheckBox cb = control as CheckBox;
                    cb.Checked = false;
                }
            }

            zedLongTerm.GraphPane.CurveList.Clear();
            zedLongTerm.GraphPane.GraphObjList.Clear();
            displayCurves();

            timer1_Tick(sender, e);
        }

        //Change of Since trigger the re-calculation
        private void dateTimePicker1_Leave(object sender, EventArgs e)
        {
            if (this.ActiveControl != dateTimePicker2)
            {
                DateTimeOffset newDate = new DateTimeOffset(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day,
                    0, 0, 0, TimeSpan.Zero);
                if (since.Date != newDate.Date)
                {
                    since = newDate;
                    setDuration(zedLongTerm, since, until);
                    displayCurves();
                }
            }
        }

        //Change of Until trigger the re-calculation
        private void dateTimePicker2_Leave(object sender, EventArgs e)
        {
            if (this.ActiveControl != dateTimePicker1)
            {
                DateTimeOffset newDate = new DateTimeOffset(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day,
                    0, 0, 0, TimeSpan.Zero);
                if (until.Date != newDate.Date)
                {
                    until = newDate;
                    setDuration(zedLongTerm, since, until);
                    displayCurves();
                }
            }
        }

        private void checkBoxCurrent_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNowTimeline.Checked)
            {
                timer1.Enabled = true;
                timer1_Tick(sender, e);
            }
            else
            {
                timer1.Enabled = false;
                foreach (GraphObj obj in currentIndicators)
                {
                    zedLongTerm.GraphPane.GraphObjList.Remove(obj);
                }

                zedLongTerm.Invalidate();
            }
        }

        #endregion

        #region Planet Curves display controler

        /// <summary>
        /// Check a set of CheckBox for efficiency
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">event param</param>
        private void starSetRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked)
            {
                foreach (Control cnt in panelStars.Controls)
                {
                    if (cnt is CheckBox)
                    {
                        CheckBox cb = cnt as CheckBox;
                        cb.Checked = true;
                    }
                }
            }
            else if (radioButtonInners.Checked)
            {
                for(int i = 0; i < panelStars.Controls.Count; i ++)
                {
                    if (panelStars.Controls[i] is CheckBox)   
                    {
                        CheckBox cb = panelStars.Controls[i] as CheckBox;
                        cb.Checked = i < 4 && i != 1;       //Skip Moon
                    }
                }
            }
            else if (radioButtonOuters.Checked)
            {
                for (int i = 0; i < panelStars.Controls.Count; i++)
                {
                    if (panelStars.Controls[i] is CheckBox)
                    {
                        CheckBox cb = panelStars.Controls[i] as CheckBox;
                        cb.Checked = (i > 3 && i < 10);
                    }
                }
            }
            else if (radioButtonAverages.Checked)
            {
                for (int i = 0; i < panelStars.Controls.Count; i++)
                {
                    if (panelStars.Controls[i] is CheckBox)
                    {
                        CheckBox cb = panelStars.Controls[i] as CheckBox;
                        cb.Checked = (i > 9);
                    }
                }
            }
        }

        /// <summary>
        /// Turn on/off the line related to the specific star
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">param</param>
        private void checkBoxStar_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb == null)
                return;

            int index = panelStars.Controls.IndexOf(cb);
            PlanetId id = CurrentEphemeris.Luminaries[index];

            if (cb.Checked)
            {
                showPlanetCurves(id);
            }
            else
            {
                hidePlanetCurves(id);
            }

            zedLongTerm.Invalidate();
        }

        #endregion

        #region Aspected Orbits settings

        private void comboFocusedStar_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = comboFocusedStar.SelectedItem.ToString();
            foreach (KeyValuePair<PlanetId, Planet> kvp in Planet.All)
            {
                if (kvp.Value.Name != name)
                    continue;

                FocusedPlanet = kvp.Key;

                synchFocusedAspect();
                break;
            }           
        }

        private void synchFocusedAspect()
        {
            foreach (Control cnt in panelAspects.Controls)
            {
                CheckBox cb = cnt as CheckBox;
                if (cb == null)
                    continue;

                int aspectDegree = int.Parse(cb.Text);

                cb.Checked = Curves[FocusedPlanet].ContainsKey(aspectDegree) || Curves[FocusedPlanet].ContainsKey(360 - aspectDegree);
            }
        }

        private void checkBoxAspect_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb == null || FocusedPlanet == PlanetId.SE_ECL_NUT)
                return;

            int offset = int.Parse(cb.Text);

            if (cb.Checked)
            {
                addAspectedOrbits(FocusedPlanet, offset);
                addAspectedOrbits(FocusedPlanet, 360 - offset);
            }
            else
            {
                removeAspectedOrbits(zedLongTerm, FocusedPlanet, offset);
                removeAspectedOrbits(zedLongTerm, FocusedPlanet, 360 - offset);
            }
            displayCurves();
        }

        private void buttonClearAspects_Click(object sender, EventArgs e)
        {
            if (FocusedPlanet == PlanetId.SE_ECL_NUT)
                return;

            List<int> existedAspect = (from kvp in Curves[FocusedPlanet]
                                       where kvp.Key != 0
                                       select kvp.Key).ToList();

            foreach (int degree in existedAspect)
            {
                removeAspectedOrbits(zedLongTerm, FocusedPlanet, degree);
            }

            synchFocusedAspect();
            zedLongTerm.Invalidate();
        }
        
        private void buttonAllAspectsOn_Click(object sender, EventArgs e)
        {
            if (FocusedPlanet == PlanetId.SE_ECL_NUT)
                return;

            foreach (Control cn in panelAspects.Controls)
            {
                if (cn is CheckBox)
                {
                    CheckBox cb = cn as CheckBox;
                    cb.Checked = true;
                }
            }
        }

        private void buttonResetAll_Click(object sender, EventArgs e)
        {
            foreach(PlanetId id in Curves.Keys)
            {
                List<int> existedAspect = (from kvp in Curves[id]
                                           where kvp.Key != 0
                                           select kvp.Key).ToList();

                foreach (int degree in existedAspect)
                {
                    removeAspectedOrbits(zedLongTerm, id, degree);
                }
            }

            synchFocusedAspect();
            zedLongTerm.Invalidate();

        }
        
        private void textBoxOffset_Leave(object sender, EventArgs e)
        {
            int degree = 0;

            if (FocusedPlanet == PlanetId.SE_ECL_NUT || textBoxOffset.Text == "" || !int.TryParse(textBoxOffset.Text, out degree))
            {
                textBoxOffset.Text = "";
                return;
            }

            if (Curves[FocusedPlanet].ContainsKey(degree))
                removeAspectedOrbits(zedLongTerm, FocusedPlanet, degree);
            else
                addAspectedOrbits(FocusedPlanet, degree);

            textBoxOffset.Text = "";

            displayCurves();
        }

        #endregion

        #region EventCategory settings

        private void checkBoxEventCategory_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (cb == null)
                return;

            String text = cb.Text;

            if (text.Contains("Occ"))
            {
                if (!cb.Checked && concernedEventCategories.Contains(PlanetEventFlag.EclipseOccultationCategory))
                    concernedEventCategories.Remove(PlanetEventFlag.EclipseOccultationCategory);
                else if (cb.Checked && !concernedEventCategories.Contains(PlanetEventFlag.EclipseOccultationCategory))
                    concernedEventCategories.Add(PlanetEventFlag.EclipseOccultationCategory);
            }
            else if (text.Contains("Sign"))
            {
                if (!cb.Checked && concernedEventCategories.Contains(PlanetEventFlag.SignChangedCategory))
                    concernedEventCategories.Remove(PlanetEventFlag.SignChangedCategory);
                else if (cb.Checked && !concernedEventCategories.Contains(PlanetEventFlag.SignChangedCategory))
                    concernedEventCategories.Add(PlanetEventFlag.SignChangedCategory);
            }
            else if (text.Contains("Dire"))
            {
                if (!cb.Checked && concernedEventCategories.Contains(PlanetEventFlag.DirectionalCategory))
                    concernedEventCategories.Remove(PlanetEventFlag.DirectionalCategory);
                else if (cb.Checked && !concernedEventCategories.Contains(PlanetEventFlag.DirectionalCategory))
                    concernedEventCategories.Add(PlanetEventFlag.DirectionalCategory);
            }
            else if (text.Contains("Heig"))
            {
                if (!cb.Checked && concernedEventCategories.Contains(PlanetEventFlag.DeclinationCategory))
                    concernedEventCategories.Remove(PlanetEventFlag.DeclinationCategory);
                else if (cb.Checked && !concernedEventCategories.Contains(PlanetEventFlag.DeclinationCategory))
                    concernedEventCategories.Add(PlanetEventFlag.DeclinationCategory);
            }
            else if (text.Contains("Asp"))
            {
                if (!cb.Checked && concernedEventCategories.Contains(PlanetEventFlag.AspectCategory))
                    concernedEventCategories.Remove(PlanetEventFlag.AspectCategory);
                else if (cb.Checked && !concernedEventCategories.Contains(PlanetEventFlag.AspectCategory))
                    concernedEventCategories.Add(PlanetEventFlag.AspectCategory);

                comboConcernedPlanet.Enabled = cb.Checked;
            }

            displayCurves();
        }

        private void comboConcernedPlanet_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = comboConcernedPlanet.SelectedItem.ToString();
            foreach (KeyValuePair<PlanetId, Planet> kvp in Planet.All)
            {
                if (kvp.Value.Name != name)
                    continue;

                ConcernedPlanet = kvp.Key;
                return;
            }

            ConcernedPlanet = PlanetId.SE_ECL_NUT;
        }

        #endregion

        #region ZedGraphControl Event Handlers

        /// <summary>
        /// Assure the Y value range from 0 to 360.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        private void zedGeoView_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            sender.GraphPane.YAxis.Scale.Min = 0;
            sender.GraphPane.YAxis.Scale.Max = 360;

            double xMax = sender.GraphPane.XAxis.Scale.Max;

            foreach (GraphObj obj in sender.GraphPane.GraphObjList)
            {
                if (!(obj is TextObj))
                    continue;

                TextObj text = obj as TextObj;
                LineItem line = (LineItem)curveOf(sender, text.Text);
                if (line != null && line.Symbol.Type == SymbolType.None)
                {
                    obj.Location.X = xMax;
                    int closestIndex = Math.Max(0, (int)(xMax - since.DateTime.ToOADate()));

                    for(int i = closestIndex; i <= line.NPts; i ++)
                    {
                        if (line[i].X < xMax)
                            continue;

                        closestIndex = i;
                        break;
                    }
                    obj.Location.Y = line[closestIndex].Y;
                }
            }
        }

        private CurveItem curveOf(ZedGraphControl zed, String name)
        {
            foreach (CurveItem curve in zed.GraphPane.CurveList)
            {
                if (curve.Label.Text == name)
                    return curve;
            }
            return null;
        }

        private string zedLongTerm_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            LineItem line = curve as LineItem;

            if (line == null)
                return "";

            string name = curve.Label.Text;

            PlanetId id = Planet.GlyphToPlanetId[name[0]];

            if (line.Symbol.Type == SymbolType.None)
            {
                PointPair pt = curve.Points[iPt];
                DateTimeOffset time = new DateTimeOffset(DateTime.FromOADate(pt.X), TimeSpan.Zero);

                return string.Format("{0}: {1} @ {2:F1}", time.ToString("MM-dd"), name, pt.Y);
            }
            else
            {
                PlanetEventFlag category = (from kvp in defaultEventSymbolType
                                            where kvp.Value == line.Symbol.Type
                                            select kvp.Key).FirstOrDefault();

                if (category != PlanetEventFlag.AspectCategory)
                {
                    IPlanetEvent evt = TheAspectarian[category][id][iPt];

                    return evt.ToString();
                }
                else
                {
                    PointPair pt = curve.Points[iPt];
                    DateTimeOffset time = new DateTimeOffset(DateTime.FromOADate(pt.X), TimeSpan.Zero);

                    IPlanetEvent closestEvt = (from one in TheAspectarian[category][id]
                                               orderby Math.Abs((one.When - time).TotalDays)
                                               select one
                                                   ).First();
                    return closestEvt.ToString();
                }
            }
        }

        #endregion

        #endregion
    }
}
