using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EphemerisCalculator;
using ZedGraph;
using DataImporter;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Astroder
{
    public partial class AstroderForm : Form
    {
        #region Constants and static variables definitions
        private const int defaultSymbolSize = 3;
        private const int defaultMarginInMonth = 3;
        private const int defaultMarginInDegree = 90;

        private static AspectImportance defaultAspectImportance = AspectImportance.Important;
        public const PeriodType DefaultPeriodType = PeriodType.AroundTheDay;

        private static Dictionary<PlanetEventFlag, SymbolType> defaultEventSymbolType = new Dictionary<PlanetEventFlag, SymbolType>{
            { PlanetEventFlag.EclipseOccultationCategory, SymbolType.Diamond},
            { PlanetEventFlag.SignChangedCategory, SymbolType.Plus},
            { PlanetEventFlag.DeclinationCategory, SymbolType.Triangle},
            { PlanetEventFlag.DirectionalCategory, SymbolType.VDash},
            { PlanetEventFlag.AspectCategory, SymbolType.Square}
        };

        #endregion

        #region Variables

        #region Options defining displayed items

        public bool IsLengendShown { get { return showLegendToolStripMenuItem.Checked; } }

        public bool IsTodayShown { get { return showTodayToolStripMenuItem.Checked; } }

        public bool IsKLineShown { get { return showKLineToolStripMenuItem.Checked; } }

        public bool IsOutlineShown { get { return showOutlineToolStripMenuItem.Checked; } }

        public bool IsOrbitReversed { get { return plusOrMinus.Text == "-"; } }

        public bool ShowMousePosition { get { return mousePositionToolStripMenuItem.Checked; } }

        #endregion

        #region Controls

        private Panel planetPanel = new Panel();
        private Panel offsetsPanel = new Panel();
        private Panel eventsPanel = new Panel();
        private ComboBox comboFocused = new ComboBox();
        private ComboBox comboAnother = new ComboBox();
        private CheckBox plusOrMinus = new CheckBox();

        #endregion

        #region Planetary data collections

        public Ephemeris CurrentEphemeris
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? Ephemeris.Geocentric : Ephemeris.Heliocentric; }
        }

        public List<double> DateValues { get; set; }

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

        //Contains the original geocentric planet orbit data, sorted by PositionValueIndex + PlanetId
        private Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> geoOrbitsDict = null;

        //Contains the original heliocentric planet orbit data, sorted by PositionValueIndex + PlanetId
        private Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> helioOrbitsDict = null;

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

        private Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> geoAspectarian = null;

        private Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> helioAspectarian = null;

        public Dictionary<PlanetId, Dictionary<double, List<CurveItem>>> Curves
        {
            get { return (CentricFlag == SeFlg.GEOCENTRIC) ? geoPlanetsCurves : helioPlanetsCurves; }
        }

        //For each Planet orbits, a set of curveItems are stored with offset degrees
        private Dictionary<PlanetId, Dictionary<double, List<CurveItem>>> geoPlanetsCurves = new Dictionary<PlanetId, Dictionary<double, List<CurveItem>>>();

        //For each Planet orbits, a set of curveItems are stored with offset degrees
        private Dictionary<PlanetId, Dictionary<double, List<CurveItem>>> helioPlanetsCurves = new Dictionary<PlanetId, Dictionary<double, List<CurveItem>>>();

        #endregion

        #region Others

        private List<PlanetEventFlag> concernedEventCategories = new List<PlanetEventFlag>();

        private List<RecentFileInfo> recents = new List<RecentFileInfo>();

        public DateTimeOffset Since { get; private set; }
        public DateTimeOffset Until { get; private set; }

        private double priceMin { get; set; }
        private double priceMax { get; set; }

        private PlanetId focusedPlanet = PlanetId.SE_ECL_NUT;
        public PlanetId FocusedPlanet 
        {
            get { return focusedPlanet; }
            set
            {
                if (focusedPlanet != value)
                {
                    focusedPlanet = value;
                    PlanetId first = (value < PlanetId.SE_FICT_OFFSET) ? value : PlanetId.SE_ECL_NUT;
                    PlanetId second = (AnotherPlanet == first) ? PlanetId.SE_ECL_NUT : AnotherPlanet;
                    ConcernedPlanetPair = new PlanetPair(first, second);
                    synchPlanetOffsetStatus();
                }
            }
        }

        private PlanetId anotherPlanet = PlanetId.SE_ECL_NUT;
        public PlanetId AnotherPlanet 
        {
            get { return anotherPlanet; }
            set
            {
                if (anotherPlanet != value)
                {
                    anotherPlanet = value;
                    PlanetId first = (FocusedPlanet < PlanetId.SE_FICT_OFFSET) ? FocusedPlanet : PlanetId.SE_ECL_NUT;
                    PlanetId second = (value == first) ? PlanetId.SE_ECL_NUT : value;
                    ConcernedPlanetPair = new PlanetPair(first, second);
                }
            }
        }

        public AspectImportance ConcernedAspectImportance { get; set; }

        private PlanetPair concernedPlanetPair = new PlanetPair(PlanetId.SE_ECL_NUT, PlanetId.SE_ECL_NUT);
        public PlanetPair ConcernedPlanetPair
        {
            get { return concernedPlanetPair; }
            set
            {
                if (concernedPlanetPair == null || ! concernedPlanetPair.Equals(value))
                {
                    concernedPlanetPair = value;

                    if (concernedEventCategories.Contains(PlanetEventFlag.AspectCategory))
                    {
                        removeEventCurves(PlanetEventFlag.AspectCategory);
                        appendEventCurves(PlanetEventFlag.AspectCategory);
                    }
                }
            }
        }

        public PeriodType TimeWindow { get; set; }             

        private double priceToDegree = 1;

        public double PriceToDegree
        {
            get { return priceToDegree; }
            set 
            {
                priceToDegree = value;

                zedLongTerm.GraphPane.Y2Axis.Scale.Min = zedLongTerm.GraphPane.YAxis.Scale.Min / value;
                zedLongTerm.GraphPane.Y2Axis.Scale.Max = zedLongTerm.GraphPane.YAxis.Scale.Max / value; 
                
                zedLongTerm.AxisChange();

                clearCurves();

                redrawOrbitCurves();

                synchPlanetOffsetStatus();
            }
        }

        public SeFlg CentricFlag  { get; set; }

        public OutlineItem Pivot { get; private set; }

        //public List<string> displayedOrbitSet { get; private set; }

        public List<OrbitSet> concernedOrbitSets { get; private set; }

        private List<OrbitDescription> LockedOrbits { get; set; }

        private GraphObjList currentIndicators = new GraphObjList();

        public RecordType LongTermType { get; set; }

        private Dictionary<RecordType, JapaneseCandleStickItem> allCandles = new Dictionary<RecordType, JapaneseCandleStickItem>();

        private JapaneseCandleStickItem longTermCandle
        {
            get
            {
                if (allCandles.ContainsKey(LongTermType))
                    return allCandles[LongTermType];
                else
                {
                    if(allCandles.ContainsKey(RecordType.DayRecord) && LongTermType > RecordType.DayRecord)
                    {
                        QuoteCollection dayQuotes = allCandles[RecordType.DayRecord].Tag as QuoteCollection;

                        if (dayQuotes == null)
                            return null;

                        QuoteCollection longerQuotes = dayQuotes[LongTermType];
                        allCandles.Add(LongTermType, natualStickOf(longerQuotes));
                        return allCandles[LongTermType];
                    }

                    return null;
                }
            }
        }

        //private Dictionary<RecordType, Dictionary<QuoteCollection, List<CurveItem>>> AllQuotes = new Dictionary<RecordType, Dictionary<QuoteCollection, List<CurveItem>>>();

        //public Dictionary<QuoteCollection, List<CurveItem>> this[RecordType type]
        //{
        //    get
        //    {
        //        return AllQuotes[type];
        //    }
        //}

        //private Dictionary<QuoteCollection, List<CurveItem>> longQuotes
        //{
        //    get
        //    {
        //        if (AllQuotes.ContainsKey(LongTermType))
        //            return this[LongTermType];
        //        else
        //            return null;
        //    }
        //}

        //private Dictionary<QuoteCollection, List<CurveItem>> minuteQuotes
        //{
        //    get 
        //    {
        //        if (AllQuotes.ContainsKey(RecordType.MinuteRecord))
        //            return this[RecordType.MinuteRecord];
        //        else
        //            return null;
        //    }
        //}

        #endregion

        #region Indexor

        public QuoteCollection this[RecordType type]
        {
            get
            {
                if (allCandles.ContainsKey(type))
                    return allCandles[type].Tag as QuoteCollection;
                else
                {
                    if (allCandles.ContainsKey(RecordType.DayRecord) && LongTermType > RecordType.DayRecord)
                    {
                        QuoteCollection dayQuotes = allCandles[RecordType.DayRecord].Tag as QuoteCollection;

                        if (dayQuotes == null)
                            return null;

                        QuoteCollection longerQuotes = dayQuotes[LongTermType];
                        allCandles.Add(LongTermType, natualStickOf(longerQuotes));
                        return longerQuotes;
                    }

                    return null;
                }
            }
        }

        //public List<CurveItem> this[PlanetId id, PlanetEventFlag category]
        //{
        //    get
        //    {
        //        if (!TheAspectarian.ContainsKey(category) || !TheAspectarian[category].ContainsKey(id))
        //            return null;

        //        recentEvents.Clear();
        //        List<CurveItem> result = new List<CurveItem>();

        //        String name = String.Format("{0}:{1}", Planet.Glyphs[id], PlanetEvent.PlanetEventCategorySymbols[category]);
        //        Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

        //        List<double> yValues = new List<double>();
        //        List<double> xValues = new List<double>();

        //        double x, y, since, until;

        //        if (!Curves[id].ContainsKey(0))
        //            Curves[id].Add(0, orbitCurvesOf(CentricFlag, new OrbitSet(id, false, 0)));

        //        foreach (CurveItem curve in Curves[id][0])
        //        {
        //            xValues.Clear();
        //            yValues.Clear();

        //            List<IPlanetEvent> evtCollection = new List<IPlanetEvent>();

        //            since = curve.Points[0].X;
        //            until = curve.Points[curve.Points.Count - 1].X;

        //            foreach (IPlanetEvent evt in TheAspectarian[category][id])
        //            {
        //                if (evt.OADate < since)
        //                    continue;
        //                else if (evt.OADate > until)    //The Events shall be sorted by date, thus no needs to check later ones
        //                    break;
                        
        //                if (category == PlanetEventFlag.AspectCategory)
        //                {
        //                    ExactAspectEvent aspectEvt = evt as ExactAspectEvent;
        //                    if (aspectEvt.TheAspect.Importance < ConcernedAspectImportance)
        //                        continue;

        //                    if (ConcernedPlanetPair.Contains(aspectEvt.Pair))
        //                    {
        //                        evtCollection.Add(aspectEvt);
        //                    }
        //                }
        //                else
        //                {
        //                    evtCollection.Add(evt);
        //                }
        //            }

        //            if (evtCollection.Count == 0)
        //                continue;

        //            recentEvents.AddRange(evtCollection);
        //            foreach (IPlanetEvent evt in evtCollection)
        //            {
        //                x = evt.OADate;
        //                y = curve.Points[(int)(x - since)].Y;
        //                xValues.Add(x);
        //                yValues.Add(y);
        //            }

        //            LineItem evtCurve = new LineItem(name, xValues.ToArray(), yValues.ToArray(), color, defaultEventSymbolType[category], 0);
        //            evtCurve.Tag = evtCollection;
        //            evtCurve.IsSelectable = true;
        //            evtCurve.Symbol.Size = defaultSymbolSize;
        //            evtCurve.IsY2Axis = true;
        //            if (category == PlanetEventFlag.EclipseOccultationCategory)
        //                evtCurve.Symbol.Fill = new Fill(Color.Black);
        //            result.Add(evtCurve);
        //        }

        //        return result;
        //    }
        //}

        public List<CurveItem> this[PlanetId id]
        {
            get
            {
                List<CurveItem> result = new List<CurveItem>();

                foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
                {
                    if (curve.Tag is OrbitDescription)
                    {
                        OrbitDescription desc = curve.Tag as OrbitDescription;

                        if (desc.Owner == id)
                            result.Add(curve);
                    }
                    
                }
                return result;
            }
        }

        public List<CurveItem> this[PlanetEventFlag category]
        {
            get
            {
                SymbolType symbol = defaultEventSymbolType[category];

                List<CurveItem> result = new List<CurveItem>();

                foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
                {
                    if (curve is LineItem && (curve as LineItem).Symbol.Type == symbol)
                        result.Add(curve);
                }
                return result;
            }
        }

        public List<CurveItem> this[OrbitSet orbits]
        {
            get
            {
                List<CurveItem> result = new List<CurveItem>();

                foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
                {
                    if (curve.Tag is OrbitDescription)
                    {
                        OrbitDescription desc = curve.Tag as OrbitDescription;

                        if (!desc.IsLocked && orbits.Contains(desc))
                            result.Add(curve);
                    }
                }
                return result;

                //if (orbits.IsReversed)
                //    return orbitCurvesOf(CentricFlag, orbits);

                //if (orbits.Owner == PlanetId.SE_SUN && CentricFlag == SeFlg.HELIOCENTRIC)
                //    orbits.Owner = PlanetId.SE_EARTH;
                //else if (orbits.Owner == PlanetId.SE_EARTH && CentricFlag == SeFlg.GEOCENTRIC)
                //    orbits.Owner = PlanetId.SE_SUN;

                //PlanetId id = orbits.Owner;

                //if (!Curves[id].ContainsKey(orbits.Offset))
                //{
                //    Curves[id].Add(orbits.Offset, orbitCurvesOf(CentricFlag, orbits));
                //}

                //return Curves[id][orbits.Offset];
            }

        }

        public LineItem this[OrbitDescription description]
        {
            get
            {
                foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
                {
                    if (curve.Tag != null && curve.Tag is OrbitDescription && (curve.Tag as OrbitDescription) == description)
                        return curve as LineItem;
                }

                return null;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public AstroderForm()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            DateValues = new List<double>();

            LockedOrbits = new List<OrbitDescription>();

            //displayedOrbitSet = new List<string>();

            concernedOrbitSets = new List<OrbitSet>();

            ConcernedAspectImportance = defaultAspectImportance;

            InitializeComponent();

            dataGridViewGeoAstrolabe1.AutoGenerateColumns = true;
            dataGridViewGeoAstrolabe2.AutoGenerateColumns = true;
            dataGridViewHelioAstrolabe1.AutoGenerateColumns = true;
            dataGridViewHelioAstrolabe2.AutoGenerateColumns = true;

            dataGridViewGeoAstrolabe1.DefaultCellStyle.Font = this.Font;
            dataGridViewGeoAstrolabe2.DefaultCellStyle.Font = this.Font;
            dataGridViewHelioAstrolabe1.DefaultCellStyle.Font = this.Font;
            dataGridViewHelioAstrolabe2.DefaultCellStyle.Font = this.Font;

            string configFile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.xml";

            if (File.Exists(configFile))
            {
                XmlSerializer serializer = new XmlSerializer(recents.GetType());

                using (FileStream fs = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    recents = (List<RecentFileInfo>)serializer.Deserialize(fs);
                    fs.Close();

                    resetRecentFileMenuItems();
                }
            }

            CentricFlag = heliocentricToolStripMenuItem.Checked ? SeFlg.HELIOCENTRIC : SeFlg.GEOCENTRIC;
            comboIntradayThreshold.SelectedIndex = 4;

            getInterDayToolStripReady();
            getIntraDayToolStripReady();
            getAstrolabeToolStripReady();

            int index = 0;

            foreach (PeriodType kind in Enum.GetValues(typeof(PeriodType)))
            {
                if (kind != PeriodType.Customer)
                    toolStripComboBoxPeriodType.Items.Add(kind);

                if (kind == DefaultPeriodType)
                    index = toolStripComboBoxPeriodType.Items.Count - 1;
            }
            toolStripComboBoxPeriodType.SelectedIndex = index;
            this.toolStripComboBoxPeriodType.SelectedIndexChanged += new EventHandler(toolStripComboBoxPeriodType_SelectedIndexChanged);


            Since = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
            Until = new DateTimeOffset(2011, 8, 1, 0, 0, 0, TimeSpan.Zero);

            initiateLongTermGraph(zedLongTerm);
            initiateShortTermGraph(zedShortTerm);

            loadEphemeris();

            zedLongTerm.Invalidate();

        }

        void toolStripComboBoxPeriodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PeriodType newPeriodType = (PeriodType)(toolStripComboBoxPeriodType.SelectedItem);
            if (newPeriodType != TimeWindow)
                TimeWindow = newPeriodType;
        }

        #endregion

        #region Form Events

        private void AstroderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string configFile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config.xml";

            recents.Sort();

            if (recents.Count > RecentFileInfo.Limit)
                recents.RemoveRange(RecentFileInfo.Limit, recents.Count - RecentFileInfo.Limit);

            XmlSerializer serializer = new XmlSerializer(recents.GetType());

            using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                serializer.Serialize(fs, recents);
                fs.Close();
            }

            saveOrbits();

        }

        private void saveOrbits()
        {
            if (longTermCandle == null)
                return;

            String sourceName, xmlName;
            XmlSerializer serializer = new XmlSerializer(LockedOrbits.GetType());
            FileStream writerFS = null;

            QuoteCollection quotes = this[LongTermType];
            sourceName = quotes.Source;
            if (!File.Exists(sourceName) || LockedOrbits.Count == 0)
                return;

            xmlName = sourceName.Substring(0, sourceName.Length - 3) + "xml";
            using (writerFS = new FileStream(xmlName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                serializer.Serialize(writerFS, LockedOrbits);
                writerFS.Close();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Text.Contains("Intra") && zedShortTerm.GraphPane.CurveList.Count == 0 && allCandles.Count != 0
                    && !allCandles.ContainsKey(RecordType.MinuteRecord) && this[RecordType.DayRecord].Source == recents[0].FullFileName)
                {
                    importPoboMinuteQuote(recents[0]);
                }
                else if (tabControl1.SelectedTab.Text.Contains("Compare") && longTermCandle != null)
                {

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }


        #endregion

        #region MainMenu button events handlers

        private void recenFileMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem && (sender as ToolStripMenuItem).Tag is RecentFileInfo)
            {
                RecentFileInfo recent = (sender as ToolStripMenuItem).Tag as RecentFileInfo;

                if (recent.Source == SourceType.Pobo)
                {
                    recent.LastAccess = DateTime.Now;
                    importPoboDayQuote(recent);
                }
            }
        }

        private void resetRecentFileMenuItems()
        {
            this.recentFilesToolStripMenuItem.DropDownItems.Clear();
            recents.Sort();

            foreach (RecentFileInfo recentFile in recents)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(recentFile.Name);
                item.Tag = recentFile;
                item.ToolTipText = recentFile.ToString();
                item.Click += new EventHandler(recenFileMenuItem_Click);
                this.recentFilesToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void importPoboDayQuote(RecentFileInfo info)
        {
            if (allCandles.Count != 0 && info == recents[0])
                return;

            if (recents.Contains(info))
                recents.Remove(info);

            recents.Add(info);
            recents.Sort();

            resetRecentFileMenuItems();

            allCandles.Clear();
            saveOrbits();

            #region Import the daily dayQuotes
            List<Quote> items = PoboDataImporter.Import(info.FullFileName, RecordType.DayRecord);

            if (items.Last().Time.Date <= DateTime.Today.AddDays(-1) && 
                DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                Quote todayQuote = PoboDataImporter.LastDayQuote(info.FullFileName);
                items.Add(todayQuote);
                //if (todayQuote != null && todayQuote.Time.Date > newQuote.Until.Date)
                //    newQuote.Add(todayQuote);
            }

            QuoteCollection newQuote = new QuoteCollection(info.Name, info.FullFileName, RecordType.DayRecord, items);

            showLongTermQuote(newQuote);

            String xmlName = info.FullFileName.Substring(0, info.FullFileName.Length - 3) + "xml";

            if (File.Exists(xmlName))
            {
                using (FileStream fs = new FileStream(xmlName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<OrbitDescription>));

                    LockedOrbits = (List<OrbitDescription>)serializer.Deserialize(fs);
                    fs.Close();

                    if (LockedOrbits != null)
                    {
                        for (int i = LockedOrbits.Count-1; i >= 0; i --)
                        {
                            OrbitDescription desc = LockedOrbits[i];
                            //if (!isPlanetShown(desc.Owner) || desc.Slope != PriceToDegree)
                            if (!isPlanetShown(desc.Owner))
                                continue;

                            LineItem orbit = orbitCurveOf(desc);

                            if (orbit == null)
                                LockedOrbits.RemoveAt(i);
                            else
                                showOrbit(orbit);
                        }
                    }
                    else
                        LockedOrbits = new List<OrbitDescription>();
                }
            }
            else
                LockedOrbits = new List<OrbitDescription>();

            //synchPlanetOffsetStatus();

            //zedLongTerm.Invalidate();

            #endregion
        }

        private void importPoboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zedLongTerm.ZoomOutAll(zedLongTerm.GraphPane);
            PoboBrowser browser = new PoboBrowser();
            if (browser.ShowDialog() == DialogResult.OK && browser.TheQuote != null)
            {
                importPoboDayQuote(browser.TheQuote);
            }

        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDlg = new OpenFileDialog();
                fileDlg.FileName = "";
                fileDlg.Filter = "Text file|*.txt|All file|*.*";
                fileDlg.Multiselect = false;
                fileDlg.Title = "Import DailyQuote Data from text file ...";

                if (fileDlg.ShowDialog() != DialogResult.OK)
                    return;

                List<Quote> items = TextDataImporter.Import(fileDlg.FileName);

                if (items == null || items.Count == 0)
                    return;

                int pos = fileDlg.FileName.LastIndexOf('.');
                string dirName = fileDlg.FileName.Substring(0, pos);

                pos = dirName.LastIndexOf('\\');
                string name = dirName.Substring(pos + 1, dirName.Length - pos - 1);

                QuoteCollection newQuote = new QuoteCollection(name, fileDlg.FileName, RecordType.DayRecord, items);

                showLongTermQuote(newQuote);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void toolStripButtonCentric_Click(object sender, EventArgs e)
        {
            if (toolStripButtonCentric.Text == "Geo")
            {
                toolStripButtonCentric.Text = "Hel";
                toolStripButtonCentric.ToolTipText = "Switch to Geocentric";
                CentricFlag = SeFlg.HELIOCENTRIC;
                if (FocusedPlanet == PlanetId.SE_SUN)
                    FocusedPlanet = PlanetId.SE_EARTH;

                foreach (OrbitSet orbits in concernedOrbitSets)
                {
                    if (orbits.Owner == PlanetId.SE_SUN)
                        orbits.Owner = PlanetId.SE_EARTH;
                }

                //for (int i = 0; i < displayedOrbitSet.Count - 1; i ++ )
                //{
                //    string setName = displayedOrbitSet[i];
                //    if (setName.Contains(Planet.Glyphs[PlanetId.SE_SUN]))
                //        displayedOrbitSet[i] = setName.Replace(Planet.Glyphs[PlanetId.SE_SUN], Planet.Glyphs[PlanetId.SE_EARTH]);
                //}
            }
            else
            {
                toolStripButtonCentric.Text = "Geo";
                toolStripButtonCentric.ToolTipText = "Switch to Heliocentric";
                CentricFlag = SeFlg.GEOCENTRIC;
                if (FocusedPlanet == PlanetId.SE_EARTH)
                    FocusedPlanet = PlanetId.SE_SUN;

                foreach (OrbitSet orbits in concernedOrbitSets)
                {
                    if (orbits.Owner == PlanetId.SE_EARTH)
                        orbits.Owner = PlanetId.SE_SUN;
                }

                //for (int i = 0; i < displayedOrbitSet.Count - 1; i++)
                //{
                //    string setName = displayedOrbitSet[i];
                //    if (setName.Contains(Planet.Glyphs[PlanetId.SE_EARTH]))
                //        displayedOrbitSet[i] = setName.Replace(Planet.Glyphs[PlanetId.SE_EARTH], Planet.Glyphs[PlanetId.SE_SUN]);
                //}
            }

            updateLongTermToolStrip();

            redrawOrbitCurves();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            QuoteCollection quotes = this[LongTermType];
            saveDialog.Filter = "Quote File|*.quo|Text File(*.txt)|*.txt";
            saveDialog.FileName = quotes.Name + quotes.QuoteType.ToString() + ".quo";
            saveDialog.Title = "Save the Quote as binary or txt file...";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveDialog.FileName.EndsWith("quo"))
                {
                    IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    Stream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, quotes);
                    stream.Close();
                }
                else
                {
                    FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(stream);
                    sw.AutoFlush = true;

                    sw.WriteLine("Time\tOpen\tHigh\tLow\tClose");
                    for (int i = 0; i < quotes.Count; i++)
                    {
                        Quote quote = quotes.DataCollection[i];
                        //sw.WriteLine(quote.ToString());
                        sw.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}", quote.Time.ToString(Quote.DefaultDateTimeFormats[quote.Type]),
                            quote.Open, quote.High, quote.Low, quote.Close));
                    }
                    stream.Close();

                }
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Filter = "Quote File|*.quo";
            loadDialog.Title = "Load the QuoteCollection as binary file ...";

            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(loadDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                QuoteCollection newQuote = (QuoteCollection)formatter.Deserialize(stream);
                stream.Close();

                showLongTermQuote(newQuote);
            }
        }

        private void inputManuallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputForm inputForm = new InputForm();

            if (inputForm.ShowDialog() == DialogResult.OK && inputForm.AllQuotes.Count != 0)
            {
                QuoteCollection newQuote = new QuoteCollection(inputForm.QuoteName, "Manual", inputForm.QuoteType, new List<Quote>(inputForm.AllQuotes));
                showLongTermQuote(newQuote);
            }
        }

        private void setPivotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PivotForm pivotForm = new PivotForm(Pivot);
            if (pivotForm.ShowDialog() == DialogResult.OK)
            {
                Pivot = pivotForm.ThePivot;
                clearCurves();
                redrawOrbitCurves();
            }
        }

        private void showKLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsKLineShown && !zedLongTerm.GraphPane.CurveList.Contains(longTermCandle))
                zedLongTerm.GraphPane.CurveList.Add(longTermCandle);
            else if(!IsKLineShown && zedLongTerm.GraphPane.CurveList.Contains(longTermCandle))
                zedLongTerm.GraphPane.CurveList.Remove(longTermCandle);
            zedLongTerm.Invalidate();

            if (IsKLineShown && !zedShortTerm.GraphPane.CurveList.Contains(theMinStick))
                zedShortTerm.GraphPane.CurveList.Add(theMinStick);
            else if (!IsKLineShown && zedShortTerm.GraphPane.CurveList.Contains(theMinStick))
                zedShortTerm.GraphPane.CurveList.Remove(theMinStick);
            zedShortTerm.Invalidate();
        }

        private void showOutlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOutlineShown && longTermOutline() == null)
            {
                zedLongTerm.GraphPane.CurveList.Add(outlineOf(this[LongTermType]));
            }
            else if (!IsOutlineShown && longTermOutline() != null)
                zedLongTerm.GraphPane.CurveList.Remove(longTermOutline());
            zedLongTerm.Invalidate();

            if (theMinOutline != null && IsOutlineShown && !zedShortTerm.GraphPane.CurveList.Contains(theMinOutline))
                zedShortTerm.GraphPane.CurveList.Add(theMinStick);
            else if (theMinOutline != null && !IsOutlineShown && zedShortTerm.GraphPane.CurveList.Contains(theMinOutline))
                zedShortTerm.GraphPane.CurveList.Remove(theMinStick);
            zedShortTerm.Invalidate();
        }

        private void clearMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (avoidEventTrigger)
                return;

            if (eventsMarkers.Count == 0)
                return;

            foreach (GraphObj obj in eventsMarkers)
            {
                if (zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                    zedLongTerm.GraphPane.GraphObjList.Remove(obj);
            }

            eventsMarkers.Clear();
            zedLongTerm.Invalidate();
        }

        private void showTodayToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (IsTodayShown)
            {
                double x = DateTime.Now.ToOADate();
                LineObj today = new LineObj(Color.Black, x, priceMin, x, priceMax);
                today.Tag = "Today";
                zedLongTerm.GraphPane.GraphObjList.Add(today);

                GraphPane pane = zedLongTerm.GraphPane;
                double xLength = pane.XAxis.Scale.MinorStep;
                double xRect = (x - pane.XAxis.Scale.Min) / (pane.XAxis.Scale.Max - pane.XAxis.Scale.Min);


                DateTime time = DateTime.FromOADate(x);
                TextObj dateIndicator = lableOf(time.ToShortDateString(),
                    xRect, 0, CoordType.ChartFraction, AlignH.Center, AlignV.Bottom);
                dateIndicator.Tag = "Today";
                zedLongTerm.GraphPane.GraphObjList.Add(dateIndicator);

                TextObj timeIndicator = lableOf(time.ToLocalTime().ToShortTimeString(), xRect, 1, CoordType.ChartFraction, AlignH.Center, AlignV.Top);
                timeIndicator.Tag = "Today";
                zedLongTerm.GraphPane.GraphObjList.Add(timeIndicator);


                foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
                {
                    if (curve == null || !(curve.Tag is OrbitDescription) || !curve.IsVisible)
                        continue;

                    appendTodayValue(curve);
                }
            }
            else
            {
                List<GraphObj> toBeRemoved = new List<GraphObj>();
                foreach (GraphObj obj in zedLongTerm.GraphPane.GraphObjList)
                {
                    if ((obj.Tag is String && (string)(obj.Tag) == "Today") || obj.Tag is LineItem)
                        toBeRemoved.Add(obj);
                }

                foreach (GraphObj obj in toBeRemoved)
                {
                    zedLongTerm.GraphPane.GraphObjList.Remove(obj);
                }
            }

            zedLongTerm.Invalidate();
        }

        private void menuYAxisMode_Click(object sender, EventArgs e)
        {
            menuYAxisMode.Text = "YAxis " + (menuYAxisMode.Text.Contains("Auto") ? "Fixed" : "Auto");

            zedLongTerm_ScrollDoneEvent(zedLongTerm, null, null, null);
        }

        #endregion

        #region Astrolabe related event hanlders and functions

        private void getAstrolabeToolStripReady()
        {
            textAstrolabeTime1.Text = "00:00";
            textAstrolabeDay1.Text = astrolabeDate1.Day.ToString();
            textAstrolabeMonth1.Text = astrolabeDate1.Month.ToString();
            textAstrolabeYear1.Text = astrolabeDate1.Year.ToString();

            textAstrolabeDay2.Text = astrolabeDate2.Day.ToString();
            textAstrolabeMonth2.Text = astrolabeDate2.Month.ToString();
            textAstrolabeYear2.Text = astrolabeDate2.Year.ToString();

            foreach (PeriodType type in Enum.GetValues(typeof(PeriodType)))
            {
                comboPeriodType.Items.Add(type);
            }
            comboPeriodType.SelectedIndex = 0;

            foreach (AspectImportance importance in Enum.GetValues(typeof(AspectImportance)))
            {
                comboAspectImportance.Items.Add(importance);
            }
            comboAspectImportance.SelectedIndex = comboAspectImportance.Items.IndexOf(AspectImportance.Important);

            dataGridViewDifference.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            dataGridViewDifference.ColumnHeadersVisible = true;
            dataGridViewDifference.ColumnCount = 3 + 2 * (Ephemeris.GeocentricLuminaries.IndexOf(PlanetId.SE_PLUTO));
            dataGridViewDifference.Columns[0].Name = "Movement";

            int next = 1;
            for (int i = 0; i <= Ephemeris.GeocentricLuminaries.IndexOf(PlanetId.SE_PLUTO); i ++ )
            {
                dataGridViewDifference.Columns[next ++].Name = Planet.SymbolOf(Ephemeris.GeocentricLuminaries[i]).ToString();
            }
            for (int i = 0; i <= Ephemeris.HeliocentricLuminaries.IndexOf(PlanetId.SE_PLUTO); i++)
            {
                dataGridViewDifference.Columns[next++].Name = Planet.SymbolOf(Ephemeris.HeliocentricLuminaries[i]).ToString();
            }
        }

        private PeriodType thePeriodType = PeriodType.AroundTheDay;
        private AspectImportance theAspectImportance = AspectImportance.Important;

        private DateTimeOffset astrolabeDate1 = DateTimeOffset.Now;
        private Phenomena thePhenomena1 = null;

        private DateTimeOffset astrolabeDate2 = DateTimeOffset.Now;
        private Phenomena thePhenomena2 = null;

        private void astrolabeDate1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                int year, month, day, hour, minute;

                if (!int.TryParse(textAstrolabeYear1.Text, out year) || !int.TryParse(textAstrolabeMonth1.Text, out month) || !int.TryParse(textAstrolabeDay1.Text, out day))
                    return;

                string timeString = textAstrolabeTime1.Text;

                int index = timeString.IndexOf(':');

                if (!int.TryParse(timeString.Substring(0, index), out hour))
                    return;
                else if (!int.TryParse(timeString.Substring(index + 1), out minute))
                    return;

                DateTime date = new DateTime(year, month, day, hour, minute, 0);
                if (astrolabeDate1.Date == date.Date && astrolabeDate1.Hour == hour && astrolabeDate1.Minute == minute)
                    return;

                astrolabeDate1 = new DateTimeOffset(date);

                getAstrolabe(1);

                textDate1Details.Text = thePhenomena1.GeoEventsDescription() + "-----------\r\n" + thePhenomena1.HelioEventsDescription();

                getMovements();
            }
        }

        private void astrolabeDate2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int year, month, day, hour, minute;

                if (!int.TryParse(textAstrolabeYear2.Text, out year) || !int.TryParse(textAstrolabeMonth2.Text, out month) || !int.TryParse(textAstrolabeDay2.Text, out day))
                    return;

                string timeString = textAstrolabeTime2.Text;

                int index = timeString.IndexOf(':');

                if (!int.TryParse(timeString.Substring(0, index), out hour))
                    return;
                else if (!int.TryParse(timeString.Substring(index + 1), out minute))
                    return;

                DateTime date = new DateTime(year, month, day, hour, minute, 0);
                if (astrolabeDate2.Date == date.Date && astrolabeDate2.Hour == hour && astrolabeDate2.Minute == minute)
                    return;

                astrolabeDate2 = new DateTimeOffset(date);

                getAstrolabe(2);

                textDate2Details.Text = thePhenomena2.GeoEventsDescription() + "-----------\r\n" + thePhenomena2.HelioEventsDescription();

                getMovements();
            }
        }

        private void getAstrolabe(int index)
        {
            DateTimeOffset time = index % 2 == 1 ? astrolabeDate1 : astrolabeDate2;
            double jul_ut = Ephemeris.ToJulianDay(time);

            List<Position> positions = new List<Position>();

            foreach (PlanetId id in Ephemeris.GeocentricLuminaries)
            {
                if (id > PlanetId.SE_PLUTO)
                    continue;

                positions.Add(Ephemeris.GeocentricPositionOf(jul_ut, id));
            }

            Phenomena thePhenomena = new Phenomena(time, thePeriodType, theAspectImportance);

            if (index % 2 == 1)
                thePhenomena1 = thePhenomena;
            else
                thePhenomena2 = thePhenomena;

            var geoPositionQuery = from position in positions
                                   select new
                                   {
                                       //position.Owner,
                                       Symbol = Planet.SymbolOf(position.Owner),
                                       Longitude = position.Longitude.ToString("F2") + " (" + Rectascension.AstroStringOf(position.Longitude) + ")",
                                       Latitude = position.Latitude.ToString("F5"),
                                       Distance = position.Distance.ToString("F5"),
                                       LongVelo = position.LongitudeVelocity.ToString("F5"),
                                       LatiVelo = position.LatitudeVelocity.ToString("F5"),
                                       DistVelo = position.DistanceVelocity.ToString("F5"),
                                       Events = thePhenomena.GeoEventsSummaryOf(position.Owner)
                                   };

            if (index % 2 == 1)
                bindingSourceGeoAstrolabe1.DataSource = geoPositionQuery;
            else
                bindingSourceGeoAstrolabe2.DataSource = geoPositionQuery;

            positions.Clear();

            foreach (PlanetId id in Ephemeris.HeliocentricLuminaries)
            {
                if (id > PlanetId.SE_PLUTO && id != PlanetId.SE_EARTH)
                    continue;

                positions.Add(Ephemeris.HeliocentricPositionOf(jul_ut, id));
            }

            var helioPositionQuery = from position in positions
                                     select new
                                     {
                                         //position.Owner,
                                         Symbol = Planet.SymbolOf(position.Owner),
                                         Longitude = position.Longitude.ToString("F2") + " (" + Rectascension.AstroStringOf(position.Longitude) + ")",
                                         Latitude = position.Latitude.ToString("F5"),
                                         Distance = position.Distance.ToString("F5"),
                                         LongVelo = position.LongitudeVelocity.ToString("F5"),
                                         LatiVelo = position.LatitudeVelocity.ToString("F5"),
                                         DistVelo = position.DistanceVelocity.ToString("F5"),
                                         Events = thePhenomena.HelioEventsSummaryOf(position.Owner)
                                     };

            if(index % 2 == 1)
                bindingSourceHelioAstrolabe1.DataSource = helioPositionQuery;
            else
                bindingSourceHelioAstrolabe2.DataSource = helioPositionQuery;
        }

        private void getMovements()
        {
            if (astrolabeDate1 != astrolabeDate2)
            {
                dataGridViewDifference.Rows.Clear();

                Movements theMove = new Movements(astrolabeDate1, astrolabeDate2);

                List<string> longDifs = new List<string>() { "Longitude" };
                List<string> latDifs = new List<string>() { "Latitude" };
                List<string> distDifs = new List<string>() { "Apparent" };

                foreach (double dif in theMove.LongitudeChanges)
                {
                    longDifs.Add(dif.ToString("F2"));
                }

                foreach (double dif in theMove.LatitudeChanges)
                {
                    latDifs.Add(dif.ToString("F5"));
                }

                foreach (double dif in theMove.DistanceChanges)
                {
                    distDifs.Add(dif.ToString("F5"));
                }

                object[] rows = new object[] { longDifs, latDifs, distDifs };

                foreach (List<string> row in rows)
                {
                    dataGridViewDifference.Rows.Add(row.ToArray());
                }
                dataGridViewDifference.Invalidate();
            }
        }

        //private Phenomena getAstrolabe(DateTimeOffset time, BindingSource sourceGeo, BindingSource sourceHelio)
        //{
        //    double jul_ut = Ephemeris.ToJulianDay(time);

        //    List<Position> positions = new List<Position>();

        //    foreach (PlanetId id in Ephemeris.GeocentricLuminaries)
        //    {
        //        if (id > PlanetId.SE_PLUTO)
        //            continue;

        //        positions.Add(Ephemeris.GeocentricPositionOf(jul_ut, id));
        //    }

        //    Phenomena thePhenomena = new Phenomena(time, thePeriodType, theAspectImportance);

        //    var geoPositionQuery = from position in positions
        //                           select new
        //                           {
        //                               //position.Owner,
        //                               Symbol = Planet.SymbolOf(position.Owner),
        //                               Longitude = position.Longitude.ToString("F2") + " (" + Rectascension.AstroStringOf(position.Longitude) + ")",
        //                               Latitude = position.Latitude.ToString("F5"),
        //                               Apparent = position.Apparent.ToString("F5"),
        //                               LongVelo = position.LongitudeVelocity.ToString("F5"),
        //                               LatiVelo = position.LatitudeVelocity.ToString("F5"),
        //                               DistVelo = position.DistanceVelocity.ToString("F5"),
        //                               Events = thePhenomena.GeoEventsSummaryOf(position.Owner)
        //                           };

        //    sourceGeo.DataSource = geoPositionQuery;

        //    positions.Clear();

        //    foreach (PlanetId id in Ephemeris.HeliocentricLuminaries)
        //    {
        //        if (id > PlanetId.SE_PLUTO && id != PlanetId.SE_EARTH)
        //            continue;

        //        positions.Add(Ephemeris.HeliocentricPositionOf(jul_ut, id));
        //    }

        //    var helioPositionQuery = from position in positions
        //                             select new
        //                             {
        //                                 //position.Owner,
        //                                 Symbol = Planet.SymbolOf(position.Owner),
        //                                 Longitude = position.Longitude.ToString("F2") + " (" + Rectascension.AstroStringOf(position.Longitude) + ")",
        //                                 Latitude = position.Latitude.ToString("F5"),
        //                                 Apparent = position.Apparent.ToString("F5"),
        //                                 LongVelo = position.LongitudeVelocity.ToString("F5"),
        //                                 LatiVelo = position.LatitudeVelocity.ToString("F5"),
        //                                 DistVelo = position.DistanceVelocity.ToString("F5"),
        //                                 Events = thePhenomena.HelioEventsSummaryOf(position.Owner)
        //                             };

        //    sourceHelio.DataSource = helioPositionQuery;

        //    return thePhenomena;
        //}

        private void comboPeriodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PeriodType newPeriodType = (PeriodType)(comboPeriodType.SelectedItem);

            if (thePeriodType != newPeriodType)
            {
                thePeriodType = newPeriodType;

                getAstrolabe(1);
                getAstrolabe(2);
            }
        }

        private void comboAspectImportance_SelectedIndexChanged(object sender, EventArgs e)
        {
            AspectImportance newAspectImportance = (AspectImportance)(comboAspectImportance.SelectedItem);

            if (theAspectImportance != newAspectImportance)
            {
                theAspectImportance = newAspectImportance;
                getAstrolabe(1);
                getAstrolabe(2);
                //thePhenomena1 = getAstrolabe(astrolabeDate1, bindingSourceGeoAstrolabe1, bindingSourceHelioAstrolabe1);
                //thePhenomena2 = getAstrolabe(astrolabeDate2, bindingSourceGeoAstrolabe2, bindingSourceHelioAstrolabe2);
            }
        }

        private void dataGridViewGeoAstrolabe_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (dataGridViewGeoAstrolabe1.CurrentRow == null || dataGridViewGeoAstrolabe1.CurrentRow.Index == -1)
            //    return;

            if (e.RowIndex == -1)
                return;

            //int rowIndex = dataGridViewGeoAstrolabe1.CurrentRow.Index;
            //if (dataGridViewGeoAstrolabe1.ColumnHeadersVisible)
            //    rowIndex--;

            //string symbol = dataGridViewGeoAstrolabe1.CurrentRow.Cells[0].Value.ToString();

            //PlanetId id = Planet.GlyphToPlanetId[symbol[0]];

            PlanetId id = Ephemeris.GeocentricLuminaries[e.RowIndex];

            StringBuilder sb = new StringBuilder();

            Position pos = Ephemeris.GeocentricPositionOf(astrolabeDate1, id);

            sb.AppendLine(pos.ToString());

            List<IPlanetEvent> planetEvent = thePhenomena1[SeFlg.GEOCENTRIC, id];

            foreach (IPlanetEvent evt in planetEvent)
            {
                sb.AppendLine(evt.ToString());
            }

            textDate1Details.Text = sb.ToString();
        }

        private void dataGridViewHelioAstrolabe_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            PlanetId id = Ephemeris.HeliocentricLuminaries[e.RowIndex];

            StringBuilder sb = new StringBuilder();

            Position pos = Ephemeris.HeliocentricPositionOf(astrolabeDate1, id);

            sb.AppendLine(pos.ToString());

            List<IPlanetEvent> planetEvent = thePhenomena1[SeFlg.HELIOCENTRIC, id];

            foreach (IPlanetEvent evt in planetEvent)
            {
                sb.AppendLine(evt.ToString());
            }

            textDate2Details.Text = sb.ToString();

        }

        private void buttonLastDay_Click(object sender, EventArgs e)
        {
            DateTimeOffset newDate = astrolabeDate1.AddDays(-1);

            textAstrolabeYear1.Text = "";
            textAstrolabeDay1.Text = newDate.Day.ToString();
            if (textAstrolabeMonth1.Text != newDate.Month.ToString())
                textAstrolabeMonth1.Text = newDate.Month.ToString();
            textAstrolabeYear1.Text = newDate.Year.ToString();
        }

        private void buttonLastMonth_Click(object sender, EventArgs e)
        {
            DateTimeOffset newDate = astrolabeDate1.AddMonths(-1);

            textAstrolabeYear1.Text = "";
            textAstrolabeMonth1.Text = newDate.Month.ToString();
            textAstrolabeYear1.Text = newDate.Year.ToString();
        }

        private void buttonNextDay_Click(object sender, EventArgs e)
        {
            DateTimeOffset newDate = astrolabeDate1.AddDays(1);

            textAstrolabeYear1.Text = "";
            textAstrolabeDay1.Text = newDate.Day.ToString();
            if (textAstrolabeMonth1.Text != newDate.Month.ToString())
                textAstrolabeMonth1.Text = newDate.Month.ToString();
            textAstrolabeYear1.Text = newDate.Year.ToString();

        }

        private void buttonNextMonth_Click(object sender, EventArgs e)
        {
            DateTimeOffset newDate = astrolabeDate1.AddMonths(1);

            textAstrolabeYear1.Text = "";
            textAstrolabeMonth1.Text = newDate.Month.ToString();
            textAstrolabeYear1.Text = newDate.Year.ToString();
        }

        #endregion

        #region Chart InterDay related event handlers and functions

        #region zedLongTerm Control event handlers

        private void zedLongTerm_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            if (this[LongTermType] != null && menuYAxisMode.Text.Contains("Auto"))
                fitYAxis(sender, this[LongTermType]);
            else
                keepYAxisUnchange(sender);
        }

        private void zedLongTerm_ScrollDoneEvent(ZedGraphControl sender, ScrollBar scrollBar, ZoomState oldState, ZoomState newState)
        {
            if (this[LongTermType] != null && menuYAxisMode.Text.Contains("Auto"))
                fitYAxis(sender, this[LongTermType]);
            else
                keepYAxisUnchange(sender);
        }

        private List<GraphObj> mouseIndicators = new List<GraphObj>();

        private void zedLongTerm_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                GraphPane pane = (sender as ZedGraphControl).GraphPane;
                clearMouseIndicators();

                double x, x2, y, y2;
                pane.ReverseTransform(new PointF(e.X, e.Y), out x, out x2, out y, out y2);
                PointF mousePt = new PointF((float)x, (float)y2);

                if (pane.XAxis.Scale.Min > x || pane.XAxis.Scale.Max < x)
                    return;

                object nearestObj;
                int iPt;

                using (Graphics g = (sender as ZedGraphControl).CreateGraphics())
                {
                    //if ((sender as ZedGraphControl).MasterPane.FindNearestPaneObject(mousePt, g, out pane, out nearestObj, out iPt))
                    if (pane.FindNearestObject(new PointF(e.X, e.Y), g, out nearestObj, out iPt))
                    {
                        if (nearestObj is LineObj)
                        {
                            IPlanetEvent evt = (nearestObj as LineObj).Tag as IPlanetEvent;

                            if (evt == null)
                                theToolTip.Active = false;
                            else
                            {
                                string label = evt.ToString();
                                theToolTip.SetToolTip(sender as ZedGraphControl, label);
                                theToolTip.Active = true;
                            }
                        }
                        else
                            theToolTip.Active = false;
                    }
                }

                DateTime time = DateTime.FromOADate(x);
                DateTimeOffset theDay = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, TimeSpan.Zero);
                StringBuilder sb = new StringBuilder();
                foreach (PlanetId id in CurrentEphemeris.Luminaries)
                {
                    sb.AppendFormat("{0}:{1:F1} ", Planet.SymbolOf(id), CurrentEphemeris[theDay, id].Longitude);
                }
                longitudeStatus.Text = sb.ToString();

                Phenomena events = new Phenomena(theDay, PeriodType.AroundTheDay, defaultAspectImportance);
                eventsStatus.Text = events.Description;

                if (ShowMousePosition)
                {
                    pane = (sender as ZedGraphControl).GraphPane;
                    if (pane != null && pane.Chart.Rect.Contains(e.Location))
                    {

                        double xLength = pane.XAxis.Scale.MinorStep;
                        double yLength = pane.YAxis.Scale.MinorStep;


                        double xRect = (x - pane.XAxis.Scale.Min) / (pane.XAxis.Scale.Max - pane.XAxis.Scale.Min);
                        double yRect = (pane.YAxis.Scale.Max - y) / (pane.YAxis.Scale.Max - pane.YAxis.Scale.Min);

                        int dayDif = Pivot == null ? (int)(x - pane.XAxis.Scale.Min) : (int)(x - Pivot.DateValue);

                        TextObj dateIndicator = lableOf(String.Format("{0}({1})", time.ToShortDateString(), dayDif),
                            xRect, 0, CoordType.ChartFraction, AlignH.Center, AlignV.Bottom);
                        mouseIndicators.Add(dateIndicator);

                        //TextObj timeIndicator = lableOf(time.ToShortTimeString(),xRect, 1, CoordType.ChartFraction, AlignH.Center, AlignV.Top);
                        //mouseIndicators.Add(timeIndicator);


                        TextObj priceIndicator = lableOf(y.ToString("F2"), 0, yRect, CoordType.ChartFraction, AlignH.Right, AlignV.Center);
                        TextObj degreeIndicator = lableOf((y2).ToString("F1"), 1, yRect, CoordType.ChartFraction, AlignH.Left, AlignV.Center);
                        mouseIndicators.Add(priceIndicator);
                        mouseIndicators.Add(degreeIndicator);

                        foreach (GraphObj obj in mouseIndicators)
                        {
                            zedLongTerm.GraphPane.GraphObjList.Add(obj);
                        }

                        zedLongTerm.Invalidate();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void clearMouseIndicators()
        {
            if (mouseIndicators.Count != 0)
            {
                foreach (GraphObj obj in mouseIndicators)
                {
                    if (zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                        zedLongTerm.GraphPane.GraphObjList.Remove(obj);
                }
                mouseIndicators.Clear();
            }
        }

        private void zedLongTerm_MouseLeave(object sender, EventArgs e)
        {
            clearMouseIndicators();
        }

        private string zedLongTerm_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            if (curve.Tag != null && curve.Tag is QuoteCollection)
            {
                QuoteCollection quotes = curve.Tag as QuoteCollection;

                Quote theQuote = quotes.DataCollection[iPt];
                return String.Format("{0}: H={1}, L={2}, O={3}, C={4}", theQuote.Time.ToString("MM-dd, ddd"),
                    theQuote.High, theQuote.Low, theQuote.Open, theQuote.Close);
            }
            else if (curve.Tag != null && curve.Tag is Outline)
            {
                double x = curve.Points[iPt].X;
                OutlineItem pivot = (curve.Tag as Outline)[x];

                if (pivot == null)
                    return "";

                Phenomena events = new Phenomena(pivot.Time, TimeWindow, defaultAspectImportance);

                return String.Format("{0}\r\n{1}", pivot, events);

            }
            else if (curve.Tag is OrbitDescription)
            {
                OrbitDescription desc = curve.Tag as OrbitDescription;

                PointF pos = sender.PointToClient(Cursor.Position);

                double x, y, x2, y2;
                sender.GraphPane.ReverseTransform(pos, out x, out x2, out y, out y2);

                double degree = desc.DegreeOn(x);
                if (Math.Abs(degree - y2) > 300)
                    degree = Math.Round((y2 - degree) / 360) * 360 + degree;

                DateTime time = DateTime.FromOADate(x);

                return string.Format("{0}@{1:F2}({2:F2}) on {3}", desc, degree * PriceToDegree, degree % 360, time);
            }
            else if (curve.Tag is List<IPlanetEvent>)
            {
                List<IPlanetEvent> events = curve.Tag as List<IPlanetEvent>;
                IPlanetEvent theEvent = events[iPt];
                return theEvent.ToString();
            }
            else
            {
                string name = curve.Label.Text;
                double degree = curve[iPt].Y;
                DateTime time = DateTime.FromOADate(curve[iPt].X);
                return string.Format("{0}@{1:F1} on {2}", name, degree % 360, time.ToShortDateString());
            }
        }

        private bool zedLongTerm_DoubleClickEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            PointF mousePt = new PointF(e.X, e.Y);

            CurveList eventsCurves = new CurveList();
            CurveList outlineCurves = new CurveList();
            CurveList orbitsCurves = new CurveList();
            CurveItem nearestCurve;
            int nearestPt;

            foreach (CurveItem curve in sender.GraphPane.CurveList)
            {
                if (curve.Tag is List<IPlanetEvent>)
                    eventsCurves.Add(curve);
                else if (curve.Tag is Outline)
                    outlineCurves.Add(curve);
                else if (curve.Tag is OrbitDescription)
                    orbitsCurves.Add(curve);
            }

            if (eventsCurves.Count != 0 && sender.GraphPane.FindNearestPoint(mousePt, eventsCurves, out nearestCurve, out nearestPt))
            {
                LineItem line = nearestCurve as LineItem;
                
                if (line.Tag != null && line.Tag is List<IPlanetEvent>)
                {
                    List<IPlanetEvent> eventList = line.Tag as List<IPlanetEvent>;
                    double x = line.Points[nearestPt].X;

                    IPlanetEvent theEvent = eventList.Find(evt => evt.OADate == x);
                    return markSimilarEvents(theEvent);
                }

                return false;
            }
            else if (outlineCurves.Count != 0 && sender.GraphPane.FindNearestPoint(mousePt, outlineCurves, out nearestCurve, out nearestPt))
            {
                if (nearestCurve is LineItem && nearestCurve.Tag is Outline)
                {
                    Outline theOutline = nearestCurve.Tag as Outline;
                    OutlineItem thePivot = theOutline.Pivots[nearestPt];

                    if (FocusedPlanet != PlanetId.SE_ECL_NUT&& AnotherPlanet != PlanetId.SE_ECL_NUT)
                    {
                        if(FocusedPlanet == AnotherPlanet)
                        {
                            markTranscension(Ephemeris.PositionOf(thePivot.Time, focusedPlanet, CentricFlag));
                            return true;
                        }
                        else
                        {
                            Position interiorPos = Ephemeris.PositionOf(thePivot.Time, ConcernedPlanetPair.Interior, CentricFlag);
                            Position exteriorPos = Ephemeris.PositionOf(thePivot.Time, ConcernedPlanetPair.Exterior, CentricFlag);
                            double aspectDegree = interiorPos.Longitude-exteriorPos.Longitude;
                            //double aspectDegree = Math.Abs(Angle.BeelineOf(interiorPos.Longitude, exteriorPos.Longitude));
                            markAspectRecurrence(ConcernedPlanetPair, aspectDegree);
                            return true;
                        }
                    }
                    else if (Pivot == null || Pivot.DateValue != thePivot.DateValue)
                        Pivot = thePivot;
                    else
                    {
                        List<double> extended = new List<double>() { thePivot.Price, 0, theOutline.Min, theOutline.Max };
                        if (thePivot.Price == theOutline.Max)
                            extended.RemoveAt(3);
                        else if (thePivot.Price == theOutline.Min)
                            extended.RemoveAt(2);

                        int nextIndex = (extended.IndexOf(Pivot.Price) + 1) % extended.Count;
                        Pivot = new OutlineItem(thePivot, extended[nextIndex]);
                    }

                    clearCurves();
                    redrawOrbitCurves();
                    return true;
                }
            }
            else if (orbitsCurves.Count != 0 && sender.GraphPane.FindNearestPoint(mousePt, orbitsCurves, out nearestCurve, out nearestPt))
            {
                if (nearestCurve.Tag is OrbitDescription)
                {
                    OrbitDescription description = nearestCurve.Tag as OrbitDescription;

                    if (LockedOrbits.Contains(description))
                    {
                        //nearestCurve.Label.Text = nearestCurve.Label.Text.Substring(1);
                        description.IsLocked = false;
                        LockedOrbits.Remove(description);
                        hideOrbit(nearestCurve);
                        //zedLongTerm.GraphPane.CurveList.Remove(nearestCurve);
                        zedLongTerm.Invalidate();
                    }
                    else
                    {
                        //nearestCurve.Label.Text = "#" + nearestCurve.Label.Text;
                        description.IsLocked = true;
                        LockedOrbits.Add(description);
                    }
                }
                return true;
            }
            else if (Pivot != null)
            {
                Pivot = null;
                clearCurves();
                redrawOrbitCurves();
                return true;
            }

            return false;
        }

        #endregion

        #region Toolbar related event handlers

        private bool avoidEventTrigger = false;

        private void updateLongTermToolStrip()
        {
            PlanetId id = CurrentEphemeris.Luminaries[0];

            CheckBox cb = planetPanel.Controls[0] as CheckBox;
            cb.Tag = id;
            cb.Text = Planet.Glyphs[id].ToString();
            cb.ForeColor = Planet.PlanetsColors[id][0];

            comboFocused.Items.RemoveAt(0);
            comboFocused.Items.Insert(0, Planet.All[id]);

            comboAnother.Items.RemoveAt(0);
            comboAnother.Items.Insert(0, Planet.All[id]);

            int selectedIndex = comboFocused.SelectedIndex;

            synchPlanetOffsetStatus();

            //foreach (Control cnt in offsetsPanel.Controls)
            //{
            //    CheckBox ck = cnt as CheckBox;
            //    if (ck != null && ck.Checked)
            //        ck.Checked = false;
            //}

            comboFocused.SelectedIndex = selectedIndex;

            //reloadAllOrbits();
        }

        private void heliocentricToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            CentricFlag = heliocentricToolStripMenuItem.Checked ? SeFlg.HELIOCENTRIC : SeFlg.GEOCENTRIC;
            updateLongTermToolStrip();
            redrawOrbitCurves();
        }

        void planetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (avoidEventTrigger)
                return;

            CheckBox cb = sender as CheckBox;

            if (cb == null)
                return;

            PlanetId id = (PlanetId)cb.Tag;

            char symbol = Planet.All[id].Symbol;

            if (!cb.Checked)
            {
                foreach (OrbitSet orbits in concernedOrbitSets)
                {
                    if (orbits.Owner == id)
                        showOrbitSet(orbits);
                }

                if (btnShowLocked.Checked)
                {
                    foreach (OrbitDescription desc in LockedOrbits)
                    {
                        //if (desc.Owner == id && desc.Slope == PriceToDegree)
                        if (desc.Owner == id)
                        {
                            LineItem orbit = orbitCurveOf(desc);
                            if (orbit != null)
                                showOrbit(orbit);
                        }
                    }
                }
            }
            else
            {
                for (int i = zedLongTerm.GraphPane.CurveList.Count - 1; i >= 0; i--)
                {
                    CurveItem curve = zedLongTerm.GraphPane.CurveList[i];

                    if (curve.Tag is OrbitDescription)
                    {
                        OrbitDescription desc = curve.Tag as OrbitDescription;

                        if (desc.Owner == id)
                            hideOrbit(curve);
                        else if (id == PlanetId.SE_SUN || id == PlanetId.SE_EARTH)
                        {
                            if ((desc.Owner == PlanetId.SE_EARTH || desc.Owner == PlanetId.SE_SUN) && isShown(desc))
                                hideOrbit(curve);
                        }
                    }
                    else if (curve.Tag is IPlanetEvent)
                    {
                        IPlanetEvent evt = curve.Tag as IPlanetEvent;

                        if (evt.Who == id)
                            zedLongTerm.GraphPane.CurveList.Remove(curve);
                    }
                }

            }

            zedLongTerm.Invalidate();

        }

        void allPlanet_CheckedChanged(object sender, EventArgs e)
        {
            if (avoidEventTrigger)
                return;

            CheckBox cb = sender as CheckBox;

            if (cb.Checked && concernedOrbitSets.Count == 0)
            {
                for (int i = 0; i < CurrentEphemeris.Luminaries.Count - 3; i++)
                {
                    if (i == 1)
                        continue;

                    PlanetId id = CurrentEphemeris.Luminaries[i];

                    OrbitSet theOrbits = new OrbitSet(id, false, 0);

                    if (!concernedOrbitSets.Contains(theOrbits))
                        concernedOrbitSets.Add(theOrbits);

                    OrbitSet reversedOrbitSet = new OrbitSet(id, true, 0);

                    if (!concernedOrbitSets.Contains(reversedOrbitSet))
                        concernedOrbitSets.Add(reversedOrbitSet);
                }
            }
            else
                concernedOrbitSets.Clear();

            //if (btn.Checked && displayedOrbitSet.Count == 0)
            //{
            //    for (int i = 0; i < CurrentEphemeris.Luminaries.Count - 3; i++)
            //    {
            //        if (i == 1)
            //            continue;

            //        PlanetId id = CurrentEphemeris.Luminaries[i];
            //        char symbol = Planet.Glyphs[id];

            //        string setName = string.Format("{0}+0", symbol);
            //        string reverseName = string.Format("-{0}+0", symbol);

            //        if (!displayedOrbitSet.Contains(setName))
            //            displayedOrbitSet.Add(setName);

            //        if (!displayedOrbitSet.Contains(reverseName))
            //            displayedOrbitSet.Add(reverseName);
            //    }
            //}
            //else if (!btn.Checked && displayedOrbitSet.Count != 0)
            //    displayedOrbitSet.Clear();

            redrawOrbitCurves();

            //bool checkAll = (sender as CheckBox).Checked;
            //for (int i = 0; i < planetPanel.Controls.Count - 4; i++)
            //{
            //    if (i == 1)
            //        continue;

            //    (planetPanel.Controls[i] as CheckBox).Checked = checkAll;
            //}
        }

        private void comboRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox combo = sender as ToolStripComboBox;
            if (avoidEventTrigger || combo == null || combo.SelectedIndex == -1)
                return;

            RecordType type = (RecordType)combo.SelectedItem;

            if (type != LongTermType)
            {
                if (zedLongTerm.GraphPane.CurveList.Contains(longTermCandle))
                    zedLongTerm.GraphPane.CurveList.Remove(longTermCandle);

                if (zedLongTerm.GraphPane.CurveList.Contains(longTermOutline()))
                    zedLongTerm.GraphPane.CurveList.Remove(longTermOutline());

                LongTermType = type;

                if (IsKLineShown)
                    zedLongTerm.GraphPane.CurveList.Add(longTermCandle);

                if (IsOutlineShown)
                    zedLongTerm.GraphPane.CurveList.Add(outlineOf(this[LongTermType]));

                zedLongTerm.Invalidate();
            }
        }

        void comboFocused_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
                return;

            if (combo.SelectedIndex != -1)
            {
                PlanetId id = (combo.SelectedItem as Planet).Id;

                if (id != FocusedPlanet)
                    FocusedPlanet = id;

                if (id <= PlanetId.SE_PLUTO && id != PlanetId.SE_MOON)
                {
                    CheckBox cb = offsetsPanel.Controls[2] as CheckBox;
                    if (!cb.Checked)
                        cb.Checked = true;
                }
            }
            else
                FocusedPlanet = PlanetId.SE_ECL_NUT;

            if (FocusedPlanet == PlanetId.SE_ECL_NUT)
            {
                //displayedOrbitSet.Clear();
                concernedOrbitSets.Clear();
                redrawOrbitCurves();
            }
        }

        private void comboPriceToDegree_SelectedIndexChanged(object sender, EventArgs e)
        {
            double newRatio = double.Parse(comboPriceToDegree.SelectedItem.ToString());

            PriceToDegree = newRatio;
        }

        private void comboPriceToDegree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double keyRatio = -1;

                if (double.TryParse(comboPriceToDegree.Text, out keyRatio))
                {
                    setPriceToDegree(keyRatio);
                }
            }
        }

        void comboAnotherPlanet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
                return;

            if (combo.SelectedIndex != -1)
            {
                PlanetId id = (combo.SelectedItem as Planet).Id;

                if (id != AnotherPlanet)
                    AnotherPlanet = id;
            }
            else
                AnotherPlanet = PlanetId.SE_ECL_NUT;
        }

        void offsetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox && FocusedPlanet != PlanetId.SE_ECL_NUT && !avoidEventTrigger)
            {
                CheckBox cb = sender as CheckBox;
                int degree = (int)cb.Tag;

                OrbitSet orbits = new OrbitSet(FocusedPlanet, IsOrbitReversed, degree);

                OrbitSet mirrorOrbits = new OrbitSet(FocusedPlanet, IsOrbitReversed, 360 - degree);

                if (cb.Checked)
                {
                    if (!concernedOrbitSets.Contains(orbits))
                    {
                        concernedOrbitSets.Add(orbits);
                        showPlanetCurves(orbits);
                    }

                    if (degree % 180 != 0)
                    {
                        if (!concernedOrbitSets.Contains(mirrorOrbits))
                        {
                            concernedOrbitSets.Add(mirrorOrbits);
                            showPlanetCurves(mirrorOrbits);
                        }
                    }
                }
                else
                {
                    if (concernedOrbitSets.Contains(orbits))
                    {
                        concernedOrbitSets.Remove(orbits);
                        hidePlanetCurves(orbits);
                    }

                    if (concernedOrbitSets.Contains(mirrorOrbits))
                    {
                        concernedOrbitSets.Remove(mirrorOrbits);
                        hidePlanetCurves(mirrorOrbits);
                    }
                }

                //char symbol = Planet.All[FocusedPlanet].Symbol;

                //string setName = string.Format("{0}{1}+{2}", IsOrbitReversed ? "-" : "", symbol, degree);
                //string mirrorName = string.Format("{0}{1}+{2}", IsOrbitReversed ? "-" : "", symbol, (360 - degree)%360);

                //if (btn.Checked)
                //{
                //    if (!displayedOrbitSet.Contains(setName))
                //        displayedOrbitSet.Add(setName);

                //    if (degree % 180 != 0 && !displayedOrbitSet.Contains(mirrorName))
                //        displayedOrbitSet.Add(mirrorName);

                //    showPlanetCurves(new OrbitSet(FocusedPlanet, IsOrbitReversed, degree));
                //    if (degree % 180 != 0)
                //        showPlanetCurves(new OrbitSet(FocusedPlanet, IsOrbitReversed, 360-degree));              
                //}
                //else
                //{
                //    if (displayedOrbitSet.Contains(setName))
                //        displayedOrbitSet.Remove(setName);

                //    if (displayedOrbitSet.Contains(mirrorName))
                //        displayedOrbitSet.Remove(mirrorName);

                //    hidePlanetCurves(FocusedPlanet, degree, IsOrbitReversed);
                //    if (degree % 180 != 0)
                //        hidePlanetCurves(FocusedPlanet, 360 - degree, IsOrbitReversed);
                //}
                zedLongTerm.Invalidate();
            }
            else if (FocusedPlanet == PlanetId.SE_ECL_NUT)
            {
                avoidEventTrigger = true;

                for (int i = 2; i < offsetsPanel.Controls.Count; i++)
                {
                    if (offsetsPanel.Controls[i] is CheckBox)
                    {
                        CheckBox cb = offsetsPanel.Controls[i] as CheckBox;
                        cb.Checked = false;
                    }
                }

                avoidEventTrigger = false;
            }
        }

        void eventCategory_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox cb = sender as CheckBox;
                PlanetEventFlag category = (PlanetEventFlag)cb.Tag;

                if (cb.Checked && !concernedEventCategories.Contains(category))
                {
                    concernedEventCategories.Add(category);
                    appendEventCurves(category);
                }
                else if (!cb.Checked && concernedEventCategories.Contains(category))
                {
                    concernedEventCategories.Remove(category);
                    removeEventCurves(category);
                }
                zedLongTerm.Invalidate();
            }
        }

        void plusOrMinus_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Text == "+")
            {
                cb.Text = "-";
            }
            else
            {
                cb.Text = "+";
            }
            synchPlanetOffsetStatus();
        }

        void allOffsets_CheckedChanged(object sender, EventArgs e)
        {
            if (avoidEventTrigger)
                return;

            if (FocusedPlanet != PlanetId.SE_ECL_NUT)
            {
                bool checkAll = (sender as CheckBox).Checked;
                for (int i = 2; i < offsetsPanel.Controls.Count - 1; i++)
                {
                    if (offsetsPanel.Controls[i] is CheckBox)
                        (offsetsPanel.Controls[i] as CheckBox).Checked = checkAll;
                }
            }
            else if (!((sender as CheckBox).Checked))
            {
                hideAllOrbits();
                //displayedOrbitSet.Clear();
                concernedOrbitSets.Clear();
            }
        }

        private void comboOutlineThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newThreshold = int.Parse(comboOutlineThreshold.SelectedItem.ToString());

            if (IsOutlineShown && Outline.DefaultThreshold != newThreshold && longTermOutline() != null)
            {
                Outline.DefaultThreshold = newThreshold;

                zedLongTerm.GraphPane.CurveList.Remove(longTermOutline());

                zedLongTerm.GraphPane.CurveList.Add(outlineOf(this[LongTermType]));

                zedLongTerm.Invalidate();

                //foreach (KeyValuePair<QuoteCollection, List<CurveItem>> kvp in longQuotes)
                //{
                //    CurveItem oldOutline = kvp.Value[1];
                //    kvp.Value.Remove(oldOutline);
                //    kvp.Value.Add(outlineOf(kvp.Key.Name, kvp.Key.BaseOutline));

                //    if (zedLongTerm.GraphPane.CurveList.Contains(oldOutline))
                //    {
                //        zedLongTerm.GraphPane.CurveList.Remove(oldOutline);
                //        zedLongTerm.GraphPane.CurveList.Add(kvp.Value[1]);
                //        zedLongTerm.Invalidate();
                //    }
                //}
            }
        }

        private void btnHighlightEvents_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (recentEvents != null && recentEvents.Count != 0)
                {
                    foreach (IPlanetEvent evt in recentEvents)
                    {
                        Color color = (evt is ExactAspectEvent) ? (Planet.PlanetsColors[(evt as ExactAspectEvent).Exterior][1]) : Planet.PlanetsColors[evt.Who][1];
                        LineObj line = new LineObj(color, evt.OADate, priceMin, evt.OADate, priceMax);
                        line.Tag = evt;
                        switch (evt.Category)
                        {
                            case PlanetEventFlag.AspectCategory:
                                line.Line.Style = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                                break;
                            case PlanetEventFlag.EclipseOccultationCategory:
                                line.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
                                break;
                            case PlanetEventFlag.SignChangedCategory:
                                line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
                                break;
                            case PlanetEventFlag.DeclinationCategory:
                                line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
                                break;
                            case PlanetEventFlag.DirectionalCategory:
                                line.Line.Style = System.Drawing.Drawing2D.DashStyle.DashDot;
                                break;
                            default:
                                break;
                        }
                        line.IsClippedToChartRect = true;
                        eventsMarkers.Add(line);

                        GraphPane pane = zedLongTerm.GraphPane;

                        TextObj text = new TextObj(evt.ShortDescription, evt.OADate, 1, CoordType.XScaleYChartFraction, AlignH.Right, AlignV.Center);

                        text.Tag = evt;
                        text.FontSpec.Angle = 90;
                        text.FontSpec.Size = 6;
                        text.FontSpec.Family = "AstroSymbols";
                        text.FontSpec.FontColor = Planet.PlanetsColors[evt.Who][0];
                        text.FontSpec.Border.IsVisible = false;
                        text.FontSpec.Fill = new ZedGraph.Fill(Color.Transparent);
                        eventsMarkers.Add(text);
                    }

                    foreach (GraphObj obj in eventsMarkers)
                    {
                        if (!zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                            zedLongTerm.GraphPane.GraphObjList.Add(obj);
                    }

                    zedLongTerm.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (eventsMarkers.Count != 0)
                {
                    foreach (GraphObj obj in eventsMarkers)
                    {
                        if (zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                            zedLongTerm.GraphPane.GraphObjList.Remove(obj);
                    }

                    eventsMarkers.Clear();
                    zedLongTerm.Invalidate();
                }

            }
        }

        private bool markSimilarEvents(IPlanetEvent theEvent)
        {
            switch (theEvent.Category)
            {
                case PlanetEventFlag.EclipseOccultationCategory:
                    //if (theEvent is SolarEclipse || theEvent is LunarEclipse)
                    //    markTranscension(theEvent.Who, theEvent.Where.Longitude);
                    //else
                    //    markTranscension(theEvent.Who, theEvent.Where.Longitude);
                    if (FocusedPlanet == PlanetId.SE_ECL_NUT)
                        markTranscension(theEvent.Who, theEvent.Where.Longitude);
                    else
                    {
                        Position pos = CurrentEphemeris[theEvent.When, FocusedPlanet];
                        markTranscension(FocusedPlanet, pos.Longitude);
                    }
                    return true;
                case PlanetEventFlag.AspectCategory:
                    break;
                case PlanetEventFlag.DeclinationCategory:
                    break;
                case PlanetEventFlag.DirectionalCategory:
                    break;
                case PlanetEventFlag.SignChangedCategory:
                    break;
                default:
                    return false;
            }

            return false;
        }

        private void markTranscension(Position pos)
        {
            markTranscension(pos.Owner, pos.Longitude);
        }

        private void markTranscension(PlanetId id, double degree)
        {
            List<IPlanetEvent> transcensions = CurrentEphemeris.TranscensionEventOf(id, degree, Since, Until);

            foreach (IPlanetEvent evt in transcensions)
            {
                Color color = Planet.PlanetsColors[id][2];
                LineObj line = new LineObj(color, evt.OADate, priceMin, evt.OADate, priceMax);
                line.Line.DashOn = 10;
                line.Line.DashOff = 15;
                line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
                line.Tag = evt;

                eventsMarkers.Add(line);

                GraphPane pane = zedLongTerm.GraphPane;

                TextObj text = new TextObj(evt.ShortDescription, evt.OADate, 1, CoordType.XScaleYChartFraction, AlignH.Right, AlignV.Center);

                text.Tag = evt;
                text.FontSpec.Angle = 90;
                text.FontSpec.Size = 6;
                text.FontSpec.Family = "AstroSymbols";
                text.FontSpec.FontColor = Planet.PlanetsColors[evt.Who][0];
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill = new ZedGraph.Fill(Color.Transparent);
                eventsMarkers.Add(text);
            }

            foreach (GraphObj obj in eventsMarkers)
            {
                if (!zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                    zedLongTerm.GraphPane.GraphObjList.Add(obj);
            }

            zedLongTerm.Invalidate();
        }

        private void markAspectRecurrence(PlanetPair pair, double degree)
        {
            List<double> aspects = pair.ConcernedAspects;

            List<IPlanetEvent> recurrences = CurrentEphemeris.AspectRecurredEventOf(ConcernedPlanetPair, degree, Since, Until, aspects);

            //List<IPlanetEvent> mirrored = CurrentEphemeris.AspectRecurredEventOf(ConcernedPlanetPair, -degree, Since, Until, aspects);

            //foreach (IPlanetEvent evt in mirrored)
            //{
            //    if (!recurrences.Contains(evt))
            //        recurrences.Add(evt);
            //}

            foreach (IPlanetEvent evt in recurrences)
            {
                AspectRecurrenceEvent aspEvt = evt as AspectRecurrenceEvent;

                Color color = aspEvt.RefAspectDegree > 0 ? Planet.PlanetsColors[pair.Interior][1] : Planet.PlanetsColors[pair.Interior][2];
                LineObj line = new LineObj(color, evt.OADate, priceMin, evt.OADate, priceMax);
                line.Line.DashOn = 10;
                line.Line.DashOff = 15;
                line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
                line.Tag = evt;

                eventsMarkers.Add(line);

                GraphPane pane = zedLongTerm.GraphPane;

                TextObj text = new TextObj(evt.ShortDescription, evt.OADate, 1, CoordType.XScaleYChartFraction, AlignH.Right, AlignV.Center);

                text.Tag = evt;
                text.FontSpec.Angle = 90;
                text.FontSpec.Size = 6;
                text.FontSpec.Family = "AstroSymbols";
                text.FontSpec.FontColor = Planet.PlanetsColors[evt.Who][0];
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill = new ZedGraph.Fill(Color.Transparent);
                eventsMarkers.Add(text);
            }

            foreach (GraphObj obj in eventsMarkers)
            {
                if (!zedLongTerm.GraphPane.GraphObjList.Contains(obj))
                    zedLongTerm.GraphPane.GraphObjList.Add(obj);
            }

            zedLongTerm.Invalidate();
        }

        private void btnShowLocked_CheckedChanged(object sender, EventArgs e)
        {
            if (btnShowLocked.Checked)
            {
                foreach (OrbitDescription desc in LockedOrbits)
                {
                    if (isPlanetShown(desc.Owner))
                    {
                        LineItem orbit = orbitCurveOf(desc);
                        if (orbit != null)
                            showOrbit(orbit);
                    }
                }
            }
            else
            {
                for (int i = zedLongTerm.GraphPane.CurveList.Count - 1; i >= 0; i--)
                {
                    CurveItem curve = zedLongTerm.GraphPane.CurveList[i];
                    if (curve.Tag is OrbitDescription && (curve.Tag as OrbitDescription).IsLocked)
                    {
                        hideOrbit(curve);
                    }
                }
            }
            zedLongTerm.Invalidate();
        }


        #endregion

        #region Functions

        private void showLongTermQuote(QuoteCollection newQuote)
        {
            if (newQuote != null && newQuote.Count != 0)
            {
                comboRecordType.SelectedIndex = comboRecordType.Items.IndexOf(newQuote.QuoteType);

                allCandles.Clear();

                Since = new DateTimeOffset(newQuote.Since.Year, newQuote.Since.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-defaultMarginInMonth);
                Until = new DateTimeOffset(newQuote.Until.Year, newQuote.Until.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(1 + defaultMarginInMonth);
                loadEphemeris();

                allCandles.Add(newQuote.QuoteType, natualStickOf(newQuote));

                zedLongTerm.GraphPane.CurveList.Clear();
                zedLongTerm.GraphPane.GraphObjList.Clear();

                if (newQuote.QuoteType != RecordType.UserDefined && IsKLineShown)
                    zedLongTerm.GraphPane.CurveList.Add(longTermCandle);

                if (IsOutlineShown)
                    zedLongTerm.GraphPane.CurveList.Add(outlineOf(newQuote));

                initiateAxisY(zedLongTerm, newQuote);

                pageInterday.Text = newQuote.Name;

                synchPlanetOffsetStatus();

                zedLongTerm.Invalidate();
            }
        }

        private void keepYAxisUnchange(ZedGraphControl sender)
        {
            sender.GraphPane.YAxis.Scale.Max = priceMax;
            sender.GraphPane.YAxis.Scale.Min = priceMin;
            sender.GraphPane.Y2Axis.Scale.Min = priceMin / PriceToDegree;
            sender.GraphPane.Y2Axis.Scale.Max = priceMax / PriceToDegree;

            sender.AxisChange();
        }

        private void getInterDayToolStripReady()
        {
            #region Fill with RecordTypes
            for (RecordType type = RecordType.DayRecord; type <= RecordType.UserDefined; type++)
            {
                comboRecordType.Items.Add(type);
                //AllQuotes.Add(type, new Dictionary<QuoteCollection, List<CurveItem>>());
            }
            comboRecordType.SelectedIndex = 0;
            //AllQuotes.Add(RecordType.MinuteRecord, new Dictionary<QuoteCollection, List<CurveItem>>());
            #endregion

            int width = 0;
            comboFocused.Size = new System.Drawing.Size(80, 24);
            comboAnother.Size = new System.Drawing.Size(80, 24);
            List<ToolStripItem> items = new List<ToolStripItem>();

            //foreach (PlanetId id in CurrentEphemeris.Luminaries)
            //{
            //    ToolStripButton btn = new ToolStripButton();
            //    btn.AutoSize = false;
            //    btn.CheckOnClick = true;
            //    btn.Size = new System.Drawing.Size(24, 24);
            //    btn.Tag = id;
            //    btn.Text = Planet.Glyphs[id].ToString();
            //    btn.ForeColor = Planet.PlanetsColors[id][0];
            //    //btn.Location = new Point(width, 0);
            //    //width += btn.Size.Width;
            //    items.Add(btn);
            //    btn.CheckedChanged += new EventHandler(planetCheckBox_CheckedChanged);
            //    //planetPanel.Controls.Add(btn);
            //    //comboFocused.Items.Add(Planet.All[id]);
            //    comboFocused.Items.Add(Planet.All[id]);
            //    if (id <= PlanetId.SE_FICT_OFFSET)
            //        comboAnother.Items.Add(Planet.All[id]);
            //}

            foreach (PlanetId id in CurrentEphemeris.Luminaries)
            {
                CheckBox btn = new CheckBox();
                btn.Appearance = Appearance.Button;
                btn.AutoSize = false;
                btn.Size = new System.Drawing.Size(24, 24);
                btn.Tag = id;
                btn.Text = Planet.Glyphs[id].ToString();
                btn.ForeColor = Planet.PlanetsColors[id][0];
                btn.Location = new Point(width, 0);
                width += btn.Size.Width;
                btn.CheckedChanged += new EventHandler(planetCheckBox_CheckedChanged);
                planetPanel.Controls.Add(btn);
                comboFocused.Items.Add(Planet.All[id]);
                if (id <= PlanetId.SE_FICT_OFFSET)
                    comboAnother.Items.Add(Planet.All[id]);
            }
            comboAnother.Items.Add(Planet.All[PlanetId.SE_ECL_NUT]);
            comboFocused.Items.Add(Planet.All[PlanetId.SE_ECL_NUT]);

            CheckBox allPlanet = new CheckBox();
            allPlanet.Appearance = Appearance.Button;
            allPlanet.AutoSize = false;
            allPlanet.Size = new System.Drawing.Size(40, 24);
            allPlanet.Text = "All";
            allPlanet.Location = new Point(width, 0);
            width += allPlanet.Size.Width;
            allPlanet.CheckedChanged += new EventHandler(allPlanet_CheckedChanged);
            planetPanel.Controls.Add(allPlanet);

            planetPanel.Size = new System.Drawing.Size(width, 24);
            planetPanel.BackColor = Color.Transparent;

            ToolStripControlHost planetHost = new ToolStripControlHost(planetPanel);

            plusOrMinus.Appearance = Appearance.Button;
            plusOrMinus.AutoSize = false;
            plusOrMinus.Size = new System.Drawing.Size(24, 24);
            plusOrMinus.Text = "+";
            plusOrMinus.ForeColor = Color.White;
            plusOrMinus.BackColor = Color.Black;
            plusOrMinus.Location = new Point(0, 0);
            plusOrMinus.CheckedChanged += new EventHandler(plusOrMinus_CheckedChanged);
            offsetsPanel.Controls.Add(plusOrMinus);
            width = plusOrMinus.Size.Width;
            comboFocused.SelectedIndexChanged += new EventHandler(comboFocused_SelectedIndexChanged);
            comboFocused.Location = new Point(width, 0);
            width += comboFocused.Size.Width;
            offsetsPanel.Controls.Add(comboFocused);

            foreach (double degree in Aspect.EffectiveAspectDegrees)
            {
                if (degree > 180)
                    break;

                CheckBox cb = new CheckBox();
                cb.Appearance = Appearance.Button;
                cb.AutoSize = false;
                cb.Size = new System.Drawing.Size(24, 24);
                cb.Tag = (int)degree;
                cb.Text = Aspect.All[degree].Symbol.ToString();
                cb.ForeColor = Color.Navy;
                cb.Location = new Point(width, 0);
                cb.CheckedChanged += new EventHandler(offsetCheckBox_CheckedChanged);
                width += cb.Size.Width;
                offsetsPanel.Controls.Add(cb);
            }
            CheckBox allOffsets = new CheckBox();
            allOffsets.Appearance = Appearance.Button;
            allOffsets.AutoSize = false;
            allOffsets.Size = new System.Drawing.Size(40, 24);
            allOffsets.Text = "All";
            allOffsets.ForeColor = Color.Navy;
            allOffsets.Location = new Point(width, 0);
            width += allOffsets.Size.Width;
            allOffsets.CheckedChanged += new EventHandler(allOffsets_CheckedChanged);
            offsetsPanel.Controls.Add(allOffsets);

            offsetsPanel.Size = new System.Drawing.Size(width, 24);
            offsetsPanel.BackColor = Color.Transparent;

            ToolStripControlHost offsetsHost = new ToolStripControlHost(offsetsPanel);

            width = 0;
            foreach (KeyValuePair<PlanetEventFlag, char> kvp in PlanetEvent.PlanetEventCategorySymbols)
            {
                CheckBox cb = new CheckBox();
                cb.Appearance = Appearance.Button;
                cb.AutoSize = false;
                cb.Size = new System.Drawing.Size(24, 24);
                cb.Tag = kvp.Key;
                cb.Text = kvp.Value.ToString();
                cb.Location = new Point(width, 0);
                width += cb.Size.Width;
                cb.CheckedChanged += new EventHandler(eventCategory_CheckedChanged);
                eventsPanel.Controls.Add(cb);
            }
            comboAnother.SelectedIndexChanged += new EventHandler(comboAnotherPlanet_SelectedIndexChanged);
            comboAnother.Location = new Point(width, 0);
            eventsPanel.Controls.Add(comboAnother);

            width += comboFocused.Size.Width;

            eventsPanel.Size = new System.Drawing.Size(width, 24);
            eventsPanel.BackColor = Color.Transparent;

            ToolStripControlHost eventsHost = new ToolStripControlHost(eventsPanel);

            toolStrip1.SuspendLayout();
            toolStrip1.Items.Add(planetHost);
            toolStrip1.Items.Add(new ToolStripSeparator());
            toolStrip1.Items.Add(offsetsHost);
            toolStrip1.Items.Add(new ToolStripSeparator());
            toolStrip1.Items.Add(eventsHost);
            toolStrip1.Items.Add(new ToolStripSeparator());
            toolStrip1.ResumeLayout();
        }

        private void initiateLongTermGraph(ZedGraphControl zed)
        {
            GraphPane graph = zed.GraphPane;

            #region set the graphGeocentric display characters
            zed.IsShowVScrollBar = false;
            zed.IsShowHScrollBar = true;
            zed.IsAutoScrollRange = true;

            // Disable the Title and Legend
            graph.Title.IsVisible = false;
            graph.Legend.IsVisible = false;

            graph.XAxis.Title.IsVisible = false;
            graph.XAxis.Scale.MinAuto = false;
            graph.XAxis.Scale.MaxAuto = false;
            graph.XAxis.Type = AxisType.Date;
            graph.XAxis.Scale.FormatAuto = true;
            graph.XAxis.Scale.FontSpec.Size = 6;
            graph.XAxis.MajorGrid.IsVisible = true;
            graph.XAxis.MajorGrid.Color = Color.LightGray;
            zedLongTerm.GraphPane.XAxis.Scale.Min = Since.DateTime.ToOADate();
            zedLongTerm.GraphPane.XAxis.Scale.Max = Until.DateTime.ToOADate();

            graph.YAxis.Title.IsVisible = false;
            graph.YAxis.Scale.Align = AlignP.Inside;
            graph.YAxis.MajorTic.IsOpposite = false;
            graph.YAxis.MinorTic.IsOpposite = false;
            graph.YAxis.Scale.FontSpec.Size = 6;
            graph.YAxis.Scale.MagAuto = false;
            graph.YAxis.MajorGrid.IsVisible = true;
            graph.YAxis.MajorGrid.Color = Color.LightGray;
            graph.YAxis.MajorGrid.IsZeroLine = false;
            graph.YAxis.Scale.MinAuto = true;
            graph.YAxis.Scale.MaxAuto = true;
            graph.YAxis.Scale.Min = 0;
            graph.YAxis.Scale.Max = 360;
            priceMax = 360;
            priceMin = 0;
            graph.YAxis.Scale.MajorStep = 30;
            graph.YAxis.Scale.MinorStep = 5;
            graph.YAxis.Scale.MinorStepAuto = true;
            graph.YAxis.Scale.MajorStepAuto = true;
            graph.YAxis.Scale.FormatAuto = true;

            graph.Y2Axis.IsVisible = true;
            graph.Y2Axis.MajorGrid.IsZeroLine = false;
            graph.Y2Axis.Title.IsVisible = false;
            graph.Y2Axis.Scale.FontSpec.Size = 6;
            graph.Y2Axis.Scale.FontSpec.FontColor = Color.DarkGray;
            graph.Y2Axis.MajorTic.IsOpposite = false;
            graph.Y2Axis.MinorTic.IsOpposite = false;
            graph.Y2Axis.Scale.Align = AlignP.Outside;
            graph.Y2Axis.Scale.Min = 0;
            graph.Y2Axis.Scale.Max = 360;
            graph.Y2Axis.Scale.MajorStep = 30;
            graph.Y2Axis.Scale.MinorStep = 10;
            graph.Y2Axis.Scale.MinorStepAuto = false;
            graph.Y2Axis.Scale.MajorStepAuto = false;

            graph.AxisChange();
            #endregion
        }

        private void hideAllOrbits()
        {
            for (int i = zedLongTerm.GraphPane.CurveList.Count - 1; i >= 0; i--)
            {
                CurveItem curve = zedLongTerm.GraphPane.CurveList[i];
                if (curve is LineItem && curve.IsY2Axis && !(curve.Tag is OrbitDescription && (curve.Tag as OrbitDescription).IsLocked))
                    hideOrbit(curve);
            }
        }

        private void loadEphemeris()
        {
            double min = Since.DateTime.ToOADate();
            double max = Until.DateTime.ToOADate();
            zedLongTerm.GraphPane.XAxis.Scale.Min = min;
            zedLongTerm.GraphPane.XAxis.Scale.Max = max;

            DateValues.Clear();

            for (double date = min; date <= max; date++)
            {
                DateValues.Add(date);
            }

            clearCurves();

            foreach (SeFlg flag in new List<SeFlg> { SeFlg.HELIOCENTRIC, SeFlg.GEOCENTRIC })
            {
                CentricFlag = flag;

                OrbitsDict = CurrentEphemeris.AllOrbitsCollectionDuring(Since, Until);
                TheAspectarian = CurrentEphemeris.AspectarianDuring(Since, Until, AspectImportance.Minor);
                populateAspectEvents(TheAspectarian);
            }

            //zedLongTerm.AxisChange();
        }

        private bool isShown(OrbitDescription description)
        {
            //if (!btnShowLocked.Checked)
            //    return false;

            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (curve.Tag != null && curve.Tag is OrbitDescription)
                {
                    if (curve.Tag == description)
                        return true;
                }
            }
            return false;
        }

        private void synchPlanetOffsetStatus()
        {
            //if (FocusedPlanet == PlanetId.SE_ECL_NUT)
            //    return;

            avoidEventTrigger = true;

            for (int i = 2; i < offsetsPanel.Controls.Count - 1; i++)
            {
                if (offsetsPanel.Controls[i] is CheckBox)
                {
                    CheckBox cb = offsetsPanel.Controls[i] as CheckBox;
                    int degree = (int)cb.Tag;

                    OrbitSet orbits = new OrbitSet(FocusedPlanet, IsOrbitReversed, degree);
                    cb.Checked = (this[orbits].Count != 0);
                }
            }

            bool isAllSame = true;
            bool isChecked = (offsetsPanel.Controls[2] as CheckBox).Checked;
            for (int i = 3; i < offsetsPanel.Controls.Count - 1; i++)
            {
                if (offsetsPanel.Controls[i] is CheckBox)
                {
                    CheckBox cb = offsetsPanel.Controls[i] as CheckBox;
                    if (isChecked != cb.Checked)
                    {
                        isAllSame = false;
                        break;
                    }
                }
            }

            if (isAllSame)
                (offsetsPanel.Controls[offsetsPanel.Controls.Count - 1] as CheckBox).Checked = isChecked;

            //for (int i = 0; i < planetPanel.Controls.Count-2; i ++ )
            //{
            //    if (planetPanel.Controls[i] is CheckBox)
            //    {
            //        CheckBox btn = planetPanel.Controls[i] as CheckBox;
            //        PlanetId id = (PlanetId)btn.Tag;

            //        char symbol = Planet.All[id].Symbol;

            //        string setName = string.Format("{0}{1}+0", IsOrbitReversed ? "-" : "", symbol);
            //        btn.Checked = displayedOrbitSet.Contains(setName);
            //    }
            //}

            (planetPanel.Controls[planetPanel.Controls.Count - 1] as CheckBox).Checked = false;

            avoidEventTrigger = false;
        }

        private LineItem longTermOutline()
        {
            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (!curve.IsY2Axis && curve is LineItem && curve.Tag is Outline)
                {
                    return curve as LineItem;
                }
            }

            return null;
        }

        private void populateAspectEvents(Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> aspectarian)
        {
            Dictionary<PlanetId, List<IPlanetEvent>> mirrored = new Dictionary<PlanetId, List<IPlanetEvent>>();

            foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in aspectarian[PlanetEventFlag.AspectCategory])
            {
                foreach (IPlanetEvent evt in kvp.Value)
                {
                    ExactAspectEvent aspect = evt as ExactAspectEvent;

                    if (aspect == null)
                        throw new Exception("Unexpected IPlanetEvent which is expected to be ExactAspectEvent: " + evt.ToString());

                    if (!mirrored.ContainsKey(aspect.Exterior))
                        mirrored.Add(aspect.Exterior, new List<IPlanetEvent>());

                    mirrored[aspect.Exterior].Add(aspect);
                }
            }

            foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in mirrored)
            {
                if (!aspectarian[PlanetEventFlag.AspectCategory].ContainsKey(kvp.Key))
                {
                    aspectarian[PlanetEventFlag.AspectCategory].Add(kvp.Key, mirrored[kvp.Key]);
                }
                else
                {
                    aspectarian[PlanetEventFlag.AspectCategory][kvp.Key].AddRange(mirrored[kvp.Key]);
                }
                aspectarian[PlanetEventFlag.AspectCategory][kvp.Key].Sort();
            }
        }

        protected double calcStepSize(double range, double targetSteps)
        {
            // Calculate an initial guess at pricePerDegree size

            double tempStep = range / targetSteps;

            // Get the magnitude of the pricePerDegree size

            double mag = Math.Floor(Math.Log10(tempStep));
            double magPow = Math.Pow((double)10.0, mag);

            // Calculate most significant digit of the new pricePerDegree size

            double magMsd = ((int)(tempStep / magPow + .5));

            // promote the MSD to either 1, 2, or 5

            if (magMsd > 5.0)
                magMsd = 10.0;
            else if (magMsd > 2.0)
                magMsd = 5.0;
            else if (magMsd > 1.0)
                magMsd = 2.0;

            return magMsd * magPow;
        }

        private void fitYAxis(ZedGraphControl sender, QuoteCollection quotes)
        {
            GraphPane thePane = sender.GraphPane;

            double floor, ceiling;

            double xMin = thePane.XAxis.Scale.Min;
            double xMax = thePane.XAxis.Scale.Max;

            if (quotes.GetRangeOf(xMin, xMax, out ceiling, out floor))
            {
                double step = calcStepSize(ceiling - floor, 10);
                thePane.YAxis.Scale.Min = Math.Truncate(floor / step - 1) * step;
                thePane.YAxis.Scale.Max = Math.Truncate(ceiling / step + 2) * step;
                thePane.Y2Axis.Scale.Min = thePane.YAxis.Scale.Min / PriceToDegree;
                thePane.Y2Axis.Scale.Max = thePane.YAxis.Scale.Max / PriceToDegree;
            }

            sender.AxisChange();
        }

        private List<double> ratios = new List<double> { 0.02, 1 / 32.0, 1 / 30.0, 0.05, 1 / 16.0, 1 / 12.0, 0.1, 0.125, 0.2, 0.25, 1 / 3.0, 0.5, 1, 2, 3, 4, 5, 8, 10, 12, 16, 20, 30, 32, 50 };
        private void initiateAxisY(ZedGraphControl zed, QuoteCollection quotes)
        {
            fitYAxis(zed, quotes);

            priceMin = zed.GraphPane.YAxis.Scale.Min;
            priceMax = zed.GraphPane.YAxis.Scale.Max;

            //double keyRatio = calcStepSize(range, 360);
            double keyRatio = calcStepSize(priceMax - priceMin, 500);

            setPriceToDegree(keyRatio);

            //double mag = Math.Floor(Math.Log10(range) - 2);

            //Double keyRatio = Math.Pow(10, mag);
            //if ((max-min) / keyRatio < 300)
            //    keyRatio /= 2;

            //if (max/keyRatio > 3000)
            //{
            //    keyRatio *= 10;
            //}

            //for (int i = -4; i <= 4; i++)
            //{
            //    double val = keyRatio * Math.Pow(2, i);
            //    comboPriceToDegree.Items.Add(val);
            //}
            //comboPriceToDegree.SelectedIndex = 4;

            //getMarginLines(dayQuotes, keyRatio);
        }

        private void setPriceToDegree(double keyRatio)
        {
            comboPriceToDegree.Items.Clear();
            comboY2Ratio.Items.Clear();
            foreach (double vector in ratios)
            {
                comboPriceToDegree.Items.Add(keyRatio * vector);
                comboY2Ratio.Items.Add(keyRatio * vector);
            }
            comboPriceToDegree.SelectedIndex = ratios.IndexOf(1);
            comboY2Ratio.SelectedIndex = comboPriceToDegree.SelectedIndex;
        }

        private bool isPlanetShown(PlanetId id)
        {
            int pos = CurrentEphemeris.Luminaries.IndexOf(id);

            if (pos == -1)
                return false;

            CheckBox cb = planetPanel.Controls[pos] as CheckBox;

            if (cb == null || !(cb.Tag is PlanetId) || (PlanetId)(cb.Tag) != id)
                return false;

            return !cb.Checked;
        }

        private void redrawOrbitCurves()
        {
            for (int i = zedLongTerm.GraphPane.CurveList.Count - 1; i >= 0; i--)
            {
                CurveItem curve = zedLongTerm.GraphPane.CurveList[i];

                if (curve.IsY2Axis)
                    hideOrbit(curve);
            }

            foreach (OrbitSet orbits in concernedOrbitSets)
            {
                if (isPlanetShown(orbits.Owner))
                    showOrbitSet(orbits);
            }

            if (btnShowLocked.Checked)
            {
                foreach (OrbitDescription desc in LockedOrbits)
                {
                    //if (desc.Slope == PriceToDegree)
                    if (isPlanetShown(desc.Owner) && desc.Slope == PriceToDegree)
                    {
                        LineItem orbit = orbitCurveOf(desc);
                        if (orbit != null)
                            showOrbit(orbit);
                    }
                }
            }

            zedLongTerm.Invalidate();
        }

        private void showOrbitSet(OrbitSet orbits)
        {
            List<CurveItem> curves = orbitCurvesOf(CentricFlag, orbits);

            if (curves == null || curves.Count == 0)
                return;

            foreach (CurveItem curve in curves)
            {
                showOrbit(curve);
            }
        }


        //private void showOrbitSet(string curveSetName)
        //{
        //    List<CurveItem> curves = this[curveSetName];

        //    if (curves == null || curves.Count == 0)
        //        return;

        //    foreach (CurveItem curve in curves)
        //    {
        //        showOrbit(curve);
        //    }
        //}

        private void hideTodayValue(CurveItem curve)
        {
            for (int i = 0; i < zedLongTerm.GraphPane.GraphObjList.Count; i++)
            {
                GraphObj obj = zedLongTerm.GraphPane.GraphObjList[i];
                if (!(obj is TextObj) || obj.Tag != curve)
                    continue;

                zedLongTerm.GraphPane.GraphObjList.Remove(obj);
                //break;
            }
        }

        private TextObj lableOf(string name, double x, double y, CoordType type, AlignH alignH, AlignV alignV)
        {
            TextObj label = null;

            label = new TextObj(name, x, y, type, alignH, alignV);

            if (Planet.GlyphToPlanetId.ContainsKey(name[0]))
            {
                PlanetId id = Planet.GlyphToPlanetId[name[0]];
                label.FontSpec.FontColor = Planet.PlanetsColors[id][0];
            }
            else if (name.Length >= 2 && Planet.GlyphToPlanetId.ContainsKey(name[1]))
            {
                PlanetId id = Planet.GlyphToPlanetId[name[1]];
                label.FontSpec.FontColor = Planet.PlanetsColors[id][0];
                label.FontSpec.IsItalic = true;
                label.FontSpec.IsUnderline = true;
            }
            else
                label.FontSpec.FontColor = Color.Gray;

            label.FontSpec.Fill.IsVisible = false;
            label.FontSpec.Border.IsVisible = false;
            label.FontSpec.Size = 6f;
            return label;
        }

        List<GraphObj> eventsMarkers = new List<GraphObj>();
        private List<IPlanetEvent> recentEvents = new List<IPlanetEvent>();
        List<GraphObj> legends = new List<GraphObj>();

        private void showOrbit(CurveItem curve)
        {
            if (curve == null)
                return;

            LineItem line = (LineItem)curve;

            curve.IsVisible = true;

            GraphPane pane = zedLongTerm.GraphPane;

            if (!pane.CurveList.Contains(line))
            {
                pane.CurveList.Add(line);

                PointPair last = line.Points[line.Points.Count - 1];

                if (IsLengendShown && line.Symbol.Type == SymbolType.None && last.X == pane.XAxis.Scale.Max)
                {
                    double y = last.Y;

                    if (y < pane.Y2Axis.Scale.Max && y > pane.Y2Axis.Scale.Min)
                    {
                        //TextObj label = lableOf(evtCurve.Label.Text, 1, yRect, CoordType.ChartFraction, AlignH.Left, AlignV.Center);
                        TextObj label = lableOf(line.Label.Text, last.X, y, CoordType.AxisXY2Scale, AlignH.Left, AlignV.Center);
                        label.Tag = curve;
                        label.FontSpec.Size = 7;

                        legends.Add(label);
                        pane.GraphObjList.Add(label);
                    }
                }

                if (IsTodayShown && line.Tag is OrbitDescription)
                    appendTodayValue(line);
            }

            //zedLongTerm.Invalidate();
        }

        private void hideOrbit(CurveItem curve)
        {
            if (curve == null || !(curve.Tag is OrbitDescription))
                return;

            OrbitDescription description = curve.Tag as OrbitDescription;

            if (zedLongTerm.GraphPane.CurveList.Contains(curve))
                zedLongTerm.GraphPane.CurveList.Remove(curve);

            for (int i = legends.Count - 1; i >= 0; i--)
            {
                if (legends[i].Tag != null && legends[i].Tag == curve)
                {
                    zedLongTerm.GraphPane.GraphObjList.Remove(legends[i]);
                    legends.RemoveAt(i);
                }
            }

            hideTodayValue(curve);
        }

        private List<CurveItem> displayedOrbitOf(PlanetId id)
        {
            List<CurveItem> result = new List<CurveItem>();

            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (!(curve.Tag is OrbitDescription))
                    continue;

                OrbitDescription desc = curve.Tag as OrbitDescription;

                if (desc.Owner == id)
                    result.Add(curve);
            }
            return result;
        }


        //private List<CurveItem> displayedOrbitOf(PlanetId id, int offset, bool isReversed)
        //{
        //    string curveName = String.Format("{0}{1}+{2}", isReversed ? "-" : "", Planet.Glyphs[id], offset);
        //    List<CurveItem> result = new List<CurveItem>();

        //    foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
        //    {
        //        if (curve.Label.Text == curveName)
        //            result.Add(curve);
        //    }
        //    return result;
        //}

        private List<CurveItem> displayedEventCurvesOf(PlanetId id)
        {
            String name = String.Format("{0}:", Planet.Glyphs[id]);

            List<CurveItem> result = new List<CurveItem>();

            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (curve.Label.Text.StartsWith(name))
                    result.Add(curve);
            }
            return result;
        }

        private List<CurveItem> displayedEventCurvesOf(PlanetEventFlag category)
        {
            char end = PlanetEvent.PlanetEventCategorySymbols[category];

            List<CurveItem> result = new List<CurveItem>();

            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (curve.Label.Text[curve.Label.Text.Length - 1] == end)
                    result.Add(curve);
            }
            return result;

        }

        private void hidePlanetCurves(PlanetId id)
        {
            List<CurveItem> curves = displayedOrbitOf(id);
            curves.AddRange(this[id]);
            //curves.AddRange(displayedEventCurvesOf(id));

            for (int i = curves.Count - 1; i >= 0; i--)
            {
                CurveItem curve = curves[i];
                hideOrbit(curve);
            }
        }

        private void hidePlanetCurves(OrbitSet orbits)
        {
            List<CurveItem> curves = this[orbits];

            if (orbits.Offset == 0 && !orbits.IsReversed)
                curves.AddRange(this[orbits.Owner]);
            //curves.AddRange(displayedEventCurvesOf(orbits.Owner));

            for (int i = curves.Count - 1; i >= 0; i--)
            {
                CurveItem curve = curves[i];
                hideOrbit(curve);
            }

        }

        //private void hidePlanetCurves(PlanetId id, int offset, bool isReversed)
        //{
        //    List<CurveItem> curves = displayedOrbitOf(id, offset, isReversed);

        //    if (offset == 0 && !isReversed)
        //        curves.AddRange(displayedEventCurvesOf(id));

        //    if (curves.Count != 0)
        //    {
        //        foreach (CurveItem curve in curves)
        //        {
        //            hideOrbit(curve);
        //        }
        //    }
        //}

        private void showPlanetCurves(OrbitSet orbits)
        {
            double key = orbits.IsReversed ? (orbits.Offset == 0 ? 360 : -orbits.Offset) : orbits.Offset;

            PlanetId id = orbits.Owner;

            if (!Curves[id].ContainsKey(key))
                Curves[id].Add(key, orbitCurvesOf(CentricFlag, orbits));

            foreach (CurveItem curve in Curves[id][key])
            {
                showOrbit(curve);
            }

            if (orbits.Offset == 0 && !orbits.IsReversed)
            {
                foreach (PlanetEventFlag category in concernedEventCategories)
                {
                    showPlanetEventsOf(id, category);
                }
            }
        }

        //private void showPlanetCurves(PlanetId id, int offset, bool isReversed)
        //{
        //    int key = isReversed ? (offset == 0 ? 360 : -offset) : offset;

        //    if (!Curves[id].ContainsKey(key))
        //        Curves[id].Add(key, ordinalOrbitCurvesOf(CentricFlag, id, offset, isReversed));

        //    foreach (CurveItem curve in Curves[id][key])
        //    {
        //        showOrbit(curve);
        //    }

        //    if (offset == 0 && !isReversed)
        //    {
        //        foreach (PlanetEventFlag category in concernedEventCategories)
        //        {
        //            showPlanetEventsOf(id, category);
        //        }
        //    }
        //}

        //private List<CurveItem> eventCurvesOf(PlanetId id, PlanetEventFlag category)
        //{
        //    recentEvents.Clear();
        //    List<CurveItem> result = new List<CurveItem>();

        //    String name = String.Format("{0}:{1}", Planet.Glyphs[id], PlanetEvent.PlanetEventCategorySymbols[category]);
        //    Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

        //    List<double> yValues = new List<double>();
        //    List<double> xValues = new List<double>();

        //    double x, y, since, until;

        //    if (!Curves[id].ContainsKey(0))
        //        Curves[id].Add(0, orbitCurvesOf(CentricFlag, new OrbitSet(id, false, 0)));

        //    foreach (CurveItem curve in Curves[id][0])
        //    {
        //        xValues.Clear();
        //        yValues.Clear();

        //        List<IPlanetEvent> evtCollection = new List<IPlanetEvent>();

        //        since = curve.Points[0].X;
        //        until = curve.Points[curve.Points.Count - 1].X;

        //        foreach (IPlanetEvent evt in TheAspectarian[category][id])
        //        {
        //            if (evt.OADate < since)
        //                continue;
        //            else if (evt.OADate > until)    //The Events shall be sorted by date, thus no needs to check later ones
        //                break;

        //            if (category == PlanetEventFlag.AspectCategory)
        //            {
        //                ExactAspectEvent aspectEvt = evt as ExactAspectEvent;
        //                if (aspectEvt.TheAspect.Importance < ConcernedAspectImportance)
        //                    continue;

        //                if (ConcernedPlanetPair.Contains(aspectEvt.Pair))
        //                {
        //                    evtCollection.Add(aspectEvt);
        //                }
        //            }
        //            else
        //            {
        //                evtCollection.Add(evt);
        //            }
        //        }

        //        if (evtCollection.Count == 0)
        //            continue;

        //        recentEvents.AddRange(evtCollection);
        //        foreach (IPlanetEvent evt in evtCollection)
        //        {
        //            x = evt.OADate;
        //            y = curve.Points[(int)(x - since)].Y;
        //            xValues.Add(x);
        //            yValues.Add(y);
        //        }

        //        LineItem orbit = new LineItem(name, xValues.ToArray(), yValues.ToArray(), color, defaultEventSymbolType[category], 0);
        //        orbit.Tag = evtCollection;
        //        orbit.IsSelectable = true;
        //        orbit.Symbol.Size = defaultSymbolSize;
        //        orbit.IsY2Axis = true;
        //        if (category == PlanetEventFlag.EclipseOccultationCategory)
        //            orbit.Symbol.Fill = new Fill(Color.Black);
        //        result.Add(orbit);
        //    }

        //    return result;
        //}

        private List<IPlanetEvent> showPlanetEventsOf(PlanetId id, PlanetEventFlag category)
        {
            if (!TheAspectarian.ContainsKey(category) || !TheAspectarian[category].ContainsKey(id) || !concernedEventCategories.Contains(category))
                return null;
            else if (category == PlanetEventFlag.AspectCategory && !ConcernedPlanetPair.Contains(id))
                return null;

            List<CurveItem> orbitCurves = this[id];

            orbitCurves.Sort((a, b) => { return b.NPts.CompareTo(a.NPts); });

            List<IPlanetEvent> evtCollection = TheAspectarian[category][id];

            if (category == PlanetEventFlag.AspectCategory)
            {
                evtCollection = new List<IPlanetEvent>();

                foreach (IPlanetEvent evt in TheAspectarian[category][id])
                {
                    ExactAspectEvent aspectEvt = evt as ExactAspectEvent;
                    //if (aspectEvt.TheAspect.Importance < ConcernedAspectImportance)
                    //    continue;


                    if (ConcernedPlanetPair.Contains(aspectEvt.Pair))
                    {
                        if (!ConcernedPlanetPair.ConcernedAspects.Contains(aspectEvt.TheAspect.Degrees))
                            continue;
                        evtCollection.Add(aspectEvt);
                    }
                }
            }

            List<double> yValues = new List<double>();
            List<double> xValues = new List<double>();

            double x, y, since, until;

            foreach (IPlanetEvent evt in evtCollection)
            {
                if (evt is ExactAspectEvent && (evt as ExactAspectEvent).TheAspect.Importance < ConcernedAspectImportance)
                    continue;

                bool isAdded = false;

                foreach (CurveItem curve in orbitCurves)
                {
                    since = curve.Points[0].X;
                    until = curve.Points[curve.Points.Count - 1].X;

                    if (evt.OADate < since || evt.OADate > until)
                        continue;

                    x = evt.OADate;
                    y = curve.Points[(int)(x - since)].Y;
                    xValues.Add(x);
                    yValues.Add(y);
                    isAdded = true;
                    break;
                }

                if (!isAdded)
                {
                    x = evt.OADate;
                    y = yValues.Count == 0 ? (zedLongTerm.GraphPane.Y2Axis.Scale.Min + 10) : yValues.Last();
                    xValues.Add(x);
                    yValues.Add(y);
                }
            }

            String name = String.Format("{0}:{1}", Planet.Glyphs[id], PlanetEvent.PlanetEventCategorySymbols[category]);
            Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

            LineItem evtCurve = new LineItem(name, xValues.ToArray(), yValues.ToArray(), color, defaultEventSymbolType[category], 0);
            evtCurve.Tag = evtCollection;
            //evtCurve.IsSelectable = true;
            evtCurve.Symbol.Size = defaultSymbolSize;
            evtCurve.IsY2Axis = true;
            if (category == PlanetEventFlag.EclipseOccultationCategory)
                evtCurve.Symbol.Fill = new Fill(Color.Black);

            showOrbit(evtCurve);

            return evtCollection;

            //if (concernedEventCategories.Contains(category))
            //{
            //    List<CurveItem> eventCurves = eventCurvesOf(id, category);

            //    if (eventCurves == null || eventCurves.Count == 0)
            //        return;

            //    foreach (CurveItem curve in eventCurves)
            //    {
            //        showOrbit(curve);
            //    }
            //}

        }

        private void removeEventCurves(PlanetEventFlag category)
        {
            //List<CurveItem> curves = displayedEventCurvesOf(category);
            List<CurveItem> curves = this[category];

            for (int i = curves.Count - 1; i >= 0; i--)
            {
                CurveItem curve = curves[i];
                if (zedLongTerm.GraphPane.CurveList.Contains(curve))
                    zedLongTerm.GraphPane.CurveList.Remove(curve);
            }

        }

        private void appendEventCurves(PlanetEventFlag category)
        {
            List<PlanetId> affected = new List<PlanetId>();

            foreach (CurveItem curve in zedLongTerm.GraphPane.CurveList)
            {
                if (curve.Tag is OrbitDescription)
                {
                    OrbitDescription desc = curve.Tag as OrbitDescription;

                    if (!affected.Contains(desc.Owner))
                        //if (desc.Offset % 360 == 0 && !affected.Contains(desc.Owner))
                    {
                        affected.Add(desc.Owner);
                    }

                }
            }

            recentEvents.Clear();
            foreach (PlanetId id in affected)
            {
                List<IPlanetEvent> evts = showPlanetEventsOf(id, category);
                if (evts != null)
                    recentEvents.AddRange(evts);
            }
        }

        private JapaneseCandleStickItem natualStickOf(QuoteCollection newQuote)
        {
            StockPointList spl = new StockPointList();

            for (int i = 0; i < newQuote.Count; i++)
            {
                Quote item = newQuote.DataCollection[i];
                double date = newQuote.QuoteOADates[i];
                StockPt pt = new StockPt(date, item.High, item.Low, item.Open, item.Close, 0);
                spl.Add(pt);
            }

            JapaneseCandleStickItem kLine = new JapaneseCandleStickItem(newQuote.Name, spl);
            kLine.Stick.IsAutoSize = true;
            kLine.Stick.Color = Color.Blue;
            kLine.Tag = newQuote;

            return kLine;
        }

        private LineItem outlineOf(QuoteCollection quotes)
        {
            if (quotes == null || quotes.BaseOutline == null)
                return null;

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            foreach (OutlineItem item in quotes.BaseOutline.Turnings)
            {
                xValues.Add(item.DateValue);
                yValues.Add(item.Price);
            }
            LineItem line = new LineItem(quotes.Name, xValues.ToArray(), yValues.ToArray(), Color.Blue, SymbolType.Circle);
            line.Tag = quotes.BaseOutline;

            line.Symbol.Fill = new Fill(Color.CadetBlue);
            line.Symbol.Size = 3;
            line.Symbol.Border.IsVisible = false;
            return line;
        }

        private bool quotesValueRange(ZedGraphControl zed, Dictionary<QuoteCollection, List<CurveItem>> quotes, double xMin, double xMax, out double ceiling, out double floor)
        {
            ceiling = double.MinValue;
            floor = double.MaxValue;

            double low, high;

            foreach (KeyValuePair<QuoteCollection, List<CurveItem>> kvp in quotes)
            {
                if ((zed.GraphPane.CurveList.Contains(kvp.Value[0]) || zed.GraphPane.CurveList.Contains(kvp.Value[1]))
                    && kvp.Key.GetRangeOf(xMin, xMax, out high, out low))
                {
                    ceiling = Math.Max(ceiling, high);
                    floor = Math.Min(floor, low);
                }
            }

            return ceiling > floor;
        }

        private List<double> jumpSteps = new List<double> { 360, 180, 120, 90, 72, 60, 360.0 / 7, 45, -45, -360.0 / 7, -60, -72, -90, -120, -180, -360 };

        private List<Double> adjustedOf(List<double> original, double jumpSize)
        {
            double last = original[0];
            double dif, next, step;
            List<Double> adjusted = new List<Double>(original);

            for (int i = 1; i < original.Count; i++)
            {
                next = original[i];
                dif = last - next;

                if (jumpSize < 0)
                {
                    double temp1 = Math.Round(last/360) * 360;
                    double temp2 = Math.Round(next / 360) * 360;

                    if (temp1 != temp2 && Math.Abs(dif) > 180)
                    {
                        step = temp1 - temp2;

                        for (int j = i; j < original.Count; j++)
                        {
                            adjusted[j] += step;
                        }
                    }
                }
                else if (Math.Abs(dif) > jumpSize)
                {
                    step = (from jump in jumpSteps
                            orderby Math.Abs(jump - dif)
                            select jump).First();

                    for (int j = i; j < original.Count; j++)
                    {
                        adjusted[j] += step;
                    }
                }

                last = next;
            }

            return adjusted;
        }

        private List<Double> smoothingOf(List<double> original, bool isIncreasing)
        {
            List<Double> adjusted = new List<Double>(original);

            for (int i = 1; i < original.Count; i++)
            {
                if (isIncreasing && original[i] < original[i-1] )
                {
                    for (int j = i; j < original.Count; j++)
                    {
                        adjusted[j] += 360;
                    }
                }
                else if (!isIncreasing && original[i] > original[i-1])
                {
                    for (int j = i; j < original.Count; j++)
                    {
                        adjusted[j] -= 360;
                    }
                }
            }

            return adjusted;
        }

        private List<Double> shiftOf(List<double> original, double offset)
        {
            List<double> result = new List<double>();

            foreach (double y in original)
            {
                result.Add(offset + y);
            }
            return result;
        }

        private List<double> reversedOf(List<double> original)
        {
            List<double> reversed = new List<double>();

            foreach (double val in original)
            {
                reversed.Add(360 - val);
            }
            return reversed;
        }

        public LineItem orbitCurveOf(OrbitDescription description, List<double> longitudes0)
        {
            LineItem existed = this[description];
            if (existed != null)
                return existed;

            PlanetId id = description.Owner;
            string name = description.ToString();
            Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

            List<double> yValues = shiftOf(longitudes0, description.Offset + description.PivotShift);

            double floor = priceMin / description.Slope;
            double ceiling = priceMax / description.Slope;

            int startIndex = yValues.FindIndex(0, y => y >= floor);
            int endIndex = yValues.Last() <= ceiling ? yValues.Count - 1 : yValues.FindLastIndex(y => y <= ceiling);

            List<double> xValues = null;
            if (startIndex < 0 || endIndex < 0)
                return null;
            if (startIndex != 0 || endIndex != yValues.Count - 1)
            {
                yValues = yValues.GetRange(startIndex, endIndex - startIndex + 1);
                xValues = DateValues.GetRange(startIndex, endIndex - startIndex + 1);
            }
            else
                xValues = DateValues;

            if (description.IsLocked)
            {
                for (int i = 0; i < yValues.Count; i ++ )
                {
                    yValues[i] *= description.Slope;
                }
            }

            LineItem line = new LineItem(name, xValues.ToArray(), yValues.ToArray(), color, SymbolType.None);
            line.Tag = description;
            line.IsY2Axis = ! description.IsLocked;
            return line;
        }


        public LineItem orbitCurveOf(OrbitDescription description)
        {
            LineItem existed = this[description];
            if (existed != null)
                return existed;

            PlanetId id = description.Owner;
            List<double> original = description.Centric == SeFlg.GEOCENTRIC ?
                geoOrbitsDict[PositionValueIndex.Longitude][id]
                : helioOrbitsDict[PositionValueIndex.Longitude][id];

            if (description.IsReversed)
                original = reversedOf(original);
            
            List<double> longitudes0 = null;

            if (description.Centric == SeFlg.HELIOCENTRIC || id == PlanetId.Earth_Rotation || id < PlanetId.SE_MERCURY)
                longitudes0 = smoothingOf(original, !description.IsReversed);
            else
            {
                double jump = id >= PlanetId.Five_Average ? 30 : 300;

                longitudes0 = adjustedOf(original, jump);
            }

            return orbitCurveOf(description, longitudes0);
        }

        private List<CurveItem> orbitCurvesOf(SeFlg centric, OrbitSet orbits)
        {
            List<CurveItem> result = new List<CurveItem>();

            if (orbits.Owner == PlanetId.SE_SUN && CentricFlag == SeFlg.HELIOCENTRIC)
                orbits.Owner = PlanetId.SE_EARTH;
            else if (orbits.Owner == PlanetId.SE_EARTH && centric == SeFlg.GEOCENTRIC)
                orbits.Owner = PlanetId.SE_SUN;

            PlanetId id = orbits.Owner;

            List<double> original = (centric == SeFlg.GEOCENTRIC) ? geoOrbitsDict[PositionValueIndex.Longitude][id]
                : helioOrbitsDict[PositionValueIndex.Longitude][id];

            if (orbits.IsReversed)
                original = reversedOf(original);

            List<double> longitudes0 = null;

            if (centric == SeFlg.HELIOCENTRIC || id == PlanetId.Earth_Rotation || id < PlanetId.SE_MERCURY)
                longitudes0 = smoothingOf(original, !orbits.IsReversed);
            else
            {
                double jump = id >= PlanetId.Five_Average ? 30 : 300;

                longitudes0 = adjustedOf(original, jump);
            }

            double floor = priceMin / PriceToDegree;
            double ceiling = priceMax / PriceToDegree;

            double pivotOffset = 0;

            if (Pivot != null)
            {
                Position pos = Ephemeris.PositionOf(Pivot.Time, id, centric);
                double orig = orbits.IsReversed ? 360 - pos.Longitude : pos.Longitude;
                pivotOffset = (Pivot.Price / PriceToDegree - orig);
            }

            double longMin = longitudes0.Min();
            double longMax = longitudes0.Max();
            int maxRound = (int)Math.Floor((ceiling - orbits.Offset - pivotOffset - longMin) / 360);
            int minRound = (int)Math.Ceiling((floor - orbits.Offset - pivotOffset - longMax) / 360);

            if (maxRound - minRound > 100)
                throw new Exception();

            for (int round = minRound; round <= maxRound; round++)
            {
                OrbitDescription desc = new OrbitDescription(false, centric, orbits, round, Pivot, PriceToDegree);
                    //new OrbitDescription(false, centric, orbits, round, pivot, slope);

                double shift = desc.Offset + desc.PivotShift;

                if (shift + longMax < floor || shift + longMin > ceiling)
                    continue;

                LineItem line = orbitCurveOf(desc, longitudes0);

                if (line !=  null)
                    result.Add(line);
            }

            return result;
        }

        private bool curtailOf(ref List<double> yValues, ref List<double> xValues)
        {
            double floor = priceMin / PriceToDegree;
            double ceiling = priceMax / PriceToDegree;

            if (yValues.Max() < floor || yValues.Min() > ceiling)
                return false;

            int startIndex = yValues.FindIndex(0, y => y >= floor);
            int endIndex = yValues.Last() <= ceiling ? yValues.Count - 1 : yValues.FindLastIndex(y => y <= ceiling);

            if (startIndex != 0 || endIndex != yValues.Count - 1)
            {
                yValues = yValues.GetRange(startIndex, endIndex - startIndex + 1);
                xValues = DateValues.GetRange(startIndex, endIndex - startIndex + 1);
            }
            else
                xValues = DateValues;

            if (yValues.Count != 0)
                return true;
            else
                return false;
        }

        private void clearCurves()
        {
            SeFlg oldFlag = CentricFlag;
            foreach (SeFlg flag in new List<SeFlg> { SeFlg.HELIOCENTRIC, SeFlg.GEOCENTRIC })
            {
                CentricFlag = flag;

                //Clear the existing curves
                foreach (PlanetId id in CurrentEphemeris.Luminaries)
                {
                    if (!Curves.ContainsKey(id))
                        Curves.Add(id, new Dictionary<double, List<CurveItem>>());

                    Curves[id].Clear();
                }
            }
            CentricFlag = oldFlag;
        }

        private void appendTodayValue(CurveItem curve)
        {
            hideTodayValue(curve);
            double x = DateTime.UtcNow.ToOADate();

            if (curve[0].X > x || curve[curve.NPts - 1].X < x || !(curve.Tag is OrbitDescription))
                return;

            int index = (int)(x - curve[0].X);
            OrbitDescription desc = curve.Tag as OrbitDescription;
            double about = curve.IsY2Axis ? curve[index].Y : curve[index].Y/desc.Slope;

            double degree = desc.DegreeOn(x);

            if (Math.Abs(degree - about) > 300)
                degree += Math.Round((about - degree) / 360) * 360;

            double y = degree * desc.Slope;

            TextObj todayDegree = new TextObj(y.ToString("F2"), x, y, CoordType.AxisXYScale, AlignH.Left, AlignV.Bottom);
            todayDegree.Tag = curve;
            todayDegree.FontSpec.Size = 6;
            todayDegree.FontSpec.FontColor = curve.Color;
            todayDegree.FontSpec.Border.IsVisible = false;
            todayDegree.FontSpec.Fill = new Fill(Color.Transparent);
            zedLongTerm.GraphPane.GraphObjList.Add(todayDegree);
        }

        #endregion

        #endregion

        #region Chart IntraDay related event handlers and functions

        private void initiateShortTermGraph(ZedGraphControl zed)
        {
            GraphPane graph = zed.GraphPane;

            #region set the zedShortTerm Control display characters
            zed.IsShowVScrollBar = false;
            zed.IsShowHScrollBar = true;
            zed.IsAutoScrollRange = true;

            // Disable the Title and Legend
            graph.Title.IsVisible = false;
            graph.Legend.IsVisible = false;

            graph.XAxis.Title.IsVisible = false;
            graph.XAxis.Scale.MinAuto = true;
            graph.XAxis.Scale.MaxAuto = true;
            graph.XAxis.Type = AxisType.Text;
            graph.XAxis.Scale.FormatAuto = true;
            graph.XAxis.Scale.MajorStepAuto = true;
            graph.XAxis.Scale.MinorStepAuto = true;
            graph.XAxis.Scale.FontSpec.Size = 6;
            graph.XAxis.MajorGrid.IsVisible = true;
            graph.XAxis.MajorGrid.Color = Color.LightGray;

            graph.YAxis.Title.IsVisible = false;
            graph.YAxis.Scale.Align = AlignP.Inside;
            graph.YAxis.MajorTic.IsOpposite = false;
            graph.YAxis.MinorTic.IsOpposite = false;
            graph.YAxis.Scale.FontSpec.Size = 6;
            graph.YAxis.Scale.MagAuto = false;
            graph.YAxis.MajorGrid.IsVisible = true;
            graph.YAxis.MajorGrid.Color = Color.LightGray;
            graph.YAxis.MajorGrid.IsZeroLine = false;
            graph.YAxis.Scale.MinAuto = true;
            graph.YAxis.Scale.MaxAuto = true;
            graph.YAxis.Scale.Min = 0;
            graph.YAxis.Scale.Max = 360;
            priceMax = 360;
            priceMin = 0;
            graph.YAxis.Scale.MajorStep = 30;
            graph.YAxis.Scale.MinorStep = 5;
            graph.YAxis.Scale.MinorStepAuto = true;
            graph.YAxis.Scale.MajorStepAuto = true;
            graph.YAxis.Scale.FormatAuto = true;

            graph.Y2Axis.IsVisible = true;
            graph.Y2Axis.MajorGrid.IsZeroLine = false;
            graph.Y2Axis.Title.IsVisible = false;
            graph.Y2Axis.Scale.FontSpec.Size = 6;
            graph.Y2Axis.Scale.FontSpec.FontColor = Color.DarkGray;
            graph.Y2Axis.MajorTic.IsOpposite = false;
            graph.Y2Axis.MinorTic.IsOpposite = false;
            graph.Y2Axis.Scale.Align = AlignP.Outside;
            graph.Y2Axis.Scale.Min = 0;
            graph.Y2Axis.Scale.Max = 360;
            graph.Y2Axis.Scale.MajorStep = 30;
            graph.Y2Axis.Scale.MinorStep = 5;
            graph.Y2Axis.Scale.MinorStepAuto = false;
            graph.Y2Axis.Scale.MajorStepAuto = false;

            Y2Axis yAxis3 = new Y2Axis("Rotation");
            yAxis3.Scale.FontSpec.FontColor = Color.Blue;
            yAxis3.Title.FontSpec.FontColor = Color.Blue;
            yAxis3.IsVisible = false;
            yAxis3.Title.IsVisible = false;
            yAxis3.Scale.FontSpec.Size = 6;
            graph.Y2AxisList.Add(yAxis3);
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            yAxis3.MajorTic.IsInside = false;
            yAxis3.MinorTic.IsInside = false;
            // Align the Y3 axis labels so they are flush to the axis
            yAxis3.Scale.Align = AlignP.Outside;
            yAxis3.Scale.MinorStepAuto = true;
            yAxis3.Scale.MajorStepAuto = true;

            graph.AxisChange();
            #endregion
        }

        Panel starPanel = new Panel();
        private void getIntraDayToolStripReady()
        {
            SeFlg centric = buttonCentric.Text.StartsWith("Geo") ? SeFlg.GEOCENTRIC : SeFlg.HELIOCENTRIC;

            ToolStripControlHost starHost = new ToolStripControlHost(starPanel);

            int width = 0;

            List<PlanetId> starList = new List<PlanetId>(Ephemeris.GeocentricLuminaries);

            int startIndex = Ephemeris.GeocentricLuminaries.IndexOf(PlanetId.Five_Average);
            starList.RemoveRange(startIndex, Ephemeris.GeocentricLuminaries.Count-startIndex);
            starList.Add(PlanetId.Earth_Rotation);

            foreach (PlanetId id in starList )
            {
                comboPlanet.Items.Add(id);
                CheckBox cb = new CheckBox();
                cb.Appearance = Appearance.Button;
                cb.AutoSize = false;
                cb.Size = new System.Drawing.Size(24, 24);
                cb.Tag = id;
                cb.Text = Planet.Glyphs[id].ToString();
                cb.ForeColor = Planet.PlanetsColors[id][0];
                cb.Location = new Point(width, 0);
                width += cb.Size.Width;
                cb.CheckedChanged += new EventHandler(star_CheckedChanged);
                starPanel.Controls.Add(cb);
            }
            


            starPanel.Size = new System.Drawing.Size(width, 24);
            starPanel.BackColor = Color.Transparent;

            //plusOrMinus.Appearance = Appearance.Button;
            //plusOrMinus.AutoSize = false;
            //plusOrMinus.Size = new System.Drawing.Size(24, 24);
            //plusOrMinus.Text = "+";
            //plusOrMinus.ForeColor = Color.White;
            //plusOrMinus.BackColor = Color.Black;
            //plusOrMinus.Location = new Point(0, 0);
            //plusOrMinus.CheckedChanged += new EventHandler(plusOrMinus_CheckedChanged);
            //offsetsPanel.Controls.Add(plusOrMinus);
            //width = plusOrMinus.Size.Width;
            //comboFocused.SelectedIndexChanged += new EventHandler(comboFocused_SelectedIndexChanged);
            //comboFocused.Location = new Point(width, 0);
            //width += comboFocused.Size.Width;
            //offsetsPanel.Controls.Add(comboFocused);

            //foreach (double degree in Aspect.EffectiveAspectDegrees)
            //{
            //    if (degree > 180)
            //        break;

            //    CheckBox btn = new CheckBox();
            //    btn.Appearance = Appearance.Button;
            //    btn.AutoSize = false;
            //    btn.Size = new System.Drawing.Size(24, 24);
            //    btn.Tag = (int)degree;
            //    btn.Text = Aspect.All[degree].Symbol.ToString();
            //    btn.ForeColor = Color.Navy;
            //    btn.Location = new Point(width, 0);
            //    btn.CheckedChanged += new EventHandler(offsetCheckBox_CheckedChanged);
            //    width += btn.Size.Width;
            //    offsetsPanel.Controls.Add(btn);
            //}
            //CheckBox allOffsets = new CheckBox();
            //allOffsets.Appearance = Appearance.Button;
            //allOffsets.AutoSize = false;
            //allOffsets.Size = new System.Drawing.Size(40, 24);
            //allOffsets.Text = "All";
            //allOffsets.ForeColor = Color.Navy;
            //allOffsets.Location = new Point(width, 0);
            //width += allOffsets.Size.Width;
            //allOffsets.CheckedChanged += new EventHandler(allOffsets_CheckedChanged);
            //offsetsPanel.Controls.Add(allOffsets);

            //offsetsPanel.Size = new System.Drawing.Size(width, 24);
            //offsetsPanel.BackColor = Color.Transparent;

            //ToolStripControlHost offsetsHost = new ToolStripControlHost(offsetsPanel);

            //width = 0;
            //foreach (KeyValuePair<PlanetEventFlag, char> kvp in PlanetEvent.PlanetEventCategorySymbols)
            //{
            //    CheckBox btn = new CheckBox();
            //    btn.Appearance = Appearance.Button;
            //    btn.AutoSize = false;
            //    btn.Size = new System.Drawing.Size(24, 24);
            //    btn.Tag = kvp.Key;
            //    btn.Text = kvp.Value.ToString();
            //    btn.Location = new Point(width, 0);
            //    width += btn.Size.Width;
            //    btn.CheckedChanged += new EventHandler(eventCategory_CheckedChanged);
            //    eventsPanel.Controls.Add(btn);
            //}
            //comboAnother.SelectedIndexChanged += new EventHandler(comboAnotherPlanet_SelectedIndexChanged);
            //comboAnother.Location = new Point(width, 0);
            //eventsPanel.Controls.Add(comboAnother);

            //width += comboFocused.Size.Width;

            //eventsPanel.Size = new System.Drawing.Size(width, 24);
            //eventsPanel.BackColor = Color.Transparent;

            //ToolStripControlHost eventsHost = new ToolStripControlHost(eventsPanel);

            toolStrip2.SuspendLayout();
            toolStrip2.Items.Add(starHost);
            toolStrip2.Items.Add(new ToolStripSeparator());
            //toolStrip2.Items.Add(offsetsHost);
            //toolStrip2.Items.Add(new ToolStripSeparator());
            //toolStrip2.Items.Add(eventsHost);
            //toolStrip2.Items.Add(new ToolStripSeparator());
            toolStrip2.ResumeLayout();
        }

        //private List<CurveItem> ordinalOrbitCurvesOf(SeFlg centric, OrbitSet orbits, OutlineItem pivot, double slope)
        //{
        //    List<CurveItem> result = new List<CurveItem>();

        //    PlanetId id = orbits.Owner;

        //    List<double> original = (centric == SeFlg.GEOCENTRIC) ? geoEndingPos[id] : helioEndingPos[id];

        //    double jump = id >= PlanetId.Five_Average ? 30 : 240;

        //    List<double> longitudes0 = adjustedOf(orbits.IsReversed ? reversedOf(original) : original, -1);

        //    double floor = zedShortTerm.GraphPane.Y2Axis.Scale.Min;
        //    double ceiling = zedShortTerm.GraphPane.Y2Axis.Scale.Max;

        //    double pivotOffset = 0;

        //    if (pivot != null)
        //    {
        //        Position pos = Ephemeris.PositionOf(pivot.Time, id, centric);
        //        double orig = orbits.IsReversed ? 360 - pos.Longitude : pos.Longitude;
        //        pivotOffset = (pivot.Price / y2Ratio - orig) % 360;
        //    }

        //    int maxRound = (int)Math.Floor((ceiling - orbits.Offset - pivotOffset - longitudes0.Min()) / 360);
        //    int minRound = (int)Math.Ceiling((floor - orbits.Offset - pivotOffset - longitudes0.Max()) / 360);


        //    for (int round = minRound; round <= maxRound; round++)
        //    {
        //        OrbitDescription desc = new OrbitDescription(false, centric, orbits, round, pivot, slope);

        //        result.Add(ordinalCurveOf(desc, longitudes0));
        //    }

        //    return result;
        //}

        JapaneseCandleStickItem theMinStick = null;
        LineItem theMinOutline = null;
        QuoteCollection minQuote = null;
        int itemsPerDay = 0;
        Dictionary<int, TimeSpan> tr2TimeIndex = new Dictionary<int, TimeSpan>();
        Dictionary<int, TimeSpan> fiveMinTimeIndex = new Dictionary<int, TimeSpan>();
        Dictionary<double, double> endingsDict = new Dictionary<double, double>();

        OutlineItem recentPivot = null;

        Dictionary<PlanetId, List<double>> geoEndingPos = new Dictionary<PlanetId, List<double>>();

        Dictionary<PlanetId, List<double>> helioEndingPos = new Dictionary<PlanetId, List<double>>();

        List<OrbitSet> currentOrbitSets = new List<OrbitSet>();

        private void getEndingDict()
        {
            double last = 0;
            TimeSpan interval = TimeSpan.FromMinutes(10);

            endingsDict.Clear();
            DateTimeOffset since = minQuote.Since.Date.AddDays(-1);

            if (since.DayOfWeek == DayOfWeek.Saturday)
                since += TimeSpan.FromDays(-1);
            else if (since.DayOfWeek == DayOfWeek.Sunday)
                since += TimeSpan.FromDays(-2);

            Dictionary<int, TimeSpan> indexes = (minQuote.QuoteType == RecordType.FiveMinuteRecord) ? fiveMinTimeIndex : tr2TimeIndex;

            for (int i = 0; i < indexes.Count; i++)
            {
                DateTimeOffset time = since.Add(indexes[i]);
                double val = Ephemeris.ToJulianDay(time);
                endingsDict.Add(i-indexes.Count, val);
            }

            endingsDict.Add(0, minQuote.DataCollection[0].JulianDay);
            for (int j = 1; j < minQuote.Count; j++)
            {
                if (minQuote.DataCollection[j].Time.Date == DateTime.Today)
                {
                    last = j - 1;
                    if (!endingsDict.Keys.Contains(j - 1))
                    {
                        endingsDict.Add(j - 1, minQuote.DataCollection[j - 1].JulianDay);
                    }
                    break;
                }

                if (j-last >= 10)
                {
                    endingsDict.Add(j, minQuote.DataCollection[j].JulianDay);
                    last = j;
                }
                else if (minQuote.DataCollection[j].Time - minQuote.DataCollection[j - 1].Time > interval)
                {
                    if (!endingsDict.Keys.Contains(j-1))
                        endingsDict.Add(j-1, minQuote.DataCollection[j-1].JulianDay);

                    endingsDict.Add(j, minQuote.DataCollection[j].JulianDay);
                    last = j;
                }
            }


            for(int d = 1; d <= 7; d ++)
            {                
                DateTimeOffset nextDay = new DateTimeOffset(minQuote.Until.Date).AddDays(d);
                if (nextDay.DayOfWeek == DayOfWeek.Saturday || nextDay.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                last = endingsDict.Keys.Last() + 1;

                for (int i = 0; i < indexes.Count; i++)
                {
                    DateTimeOffset time = nextDay.Add(indexes[i]);
                    double val = Ephemeris.ToJulianDay(time);
                    endingsDict.Add(i + last, val);
                }
            }

            foreach (PlanetId id in Ephemeris.GeocentricLuminaries)
            {
                List<double> orbit = new List<double>();
                foreach (KeyValuePair<double, double> kvp in endingsDict)
                {
                    Position pos = Ephemeris.GeocentricPositionOf(kvp.Value, id);
                    orbit.Add(pos.Longitude);
                }
                geoEndingPos.Add(id, orbit);
            }

            foreach (PlanetId id in Ephemeris.HeliocentricLuminaries)
            {
                if (id == PlanetId.SE_MOON)
                {
                    helioEndingPos.Add(id, geoEndingPos[id]);
                }
                else
                {
                    List<double> orbit = new List<double>();
                    foreach (KeyValuePair<double, double> kvp in endingsDict)
                    {
                        Position pos = Ephemeris.HeliocentricPositionOf(kvp.Value, id);
                        orbit.Add(pos.Longitude);
                    }
                    helioEndingPos.Add(id, orbit);
                }
            }
        }

        private List<Quote> getTr2Record(RecentFileInfo info)
        {
            string minFileName = PoboDataImporter.FullNameOf(info.FullFileName, RecordType.MinuteRecord);

            int lastDash = minFileName.LastIndexOf(@"\");

            string indexFileName = minFileName.Substring(0, lastDash) + "index.hex";

            if (!File.Exists(indexFileName))
            {
                List<Quote> minuteItems = PoboDataImporter.Import(minFileName, RecordType.MinuteRecord);

                if (minuteItems == null || minuteItems.Count == 0)
                    return null;

                minQuote = new QuoteCollection(info.Name, minFileName, RecordType.MinuteRecord, minuteItems);

                int lastIndex = minQuote.Count-1, nextIndex;

                DateTimeOffset lastDate = minQuote.Until.Date;

                for (int i = 0; i < minQuote.Count; i++)
                {
                    Quote quote = minQuote.DataCollection[i];
                    if (quote.Time.Date != lastDate)
                        continue;

                    nextIndex = i;
                    //if (itemsPerDay == lastIndex - nextIndex)
                    {
                        tr2TimeIndex.Clear();
                        for (int k = nextIndex; k < lastIndex; k++)
                        {
                            tr2TimeIndex.Add(k - nextIndex, minQuote.DataCollection[k].Time.TimeOfDay);
                        }
                        break;
                    }

                    //itemsPerDay =  lastIndex - nextIndex;
                    //lastIndex = nextIndex;
                }

                //DateTimeOffset lastDate = minQuote.Since.Date;

                //for (int i = 1; i < minQuote.Count; i++)
                //{
                //    Quote quote = minQuote.DataCollection[i];
                //    if (quote.Time.Date == lastDate)
                //        continue;

                //    nextIndex = i;
                //    lastDate = quote.Time.Date;
                //    if (itemsPerDay == nextIndex - lastIndex)
                //    {
                //        tr2TimeIndex.Clear();
                //        for (int k = lastIndex; k < nextIndex; k++)
                //        {
                //            tr2TimeIndex.Add(k - lastIndex, minQuote.DataCollection[k].Time.TimeOfDay);
                //        }
                //        break;
                //    }

                //    itemsPerDay = nextIndex - lastIndex;
                //    lastIndex = nextIndex;
                //}
                
                if (tr2TimeIndex != null && tr2TimeIndex.Count != 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using(FileStream fs = new FileStream(indexFileName, FileMode.Create))
                    {
                        formatter.Serialize(fs, tr2TimeIndex);
                        fs.Close();
                    }
                }
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(indexFileName, FileMode.Open))
                {
                    tr2TimeIndex = (Dictionary<int, TimeSpan>)formatter.Deserialize(fs);
                    fs.Close();
                }
            }

            if (tr2TimeIndex == null || tr2TimeIndex.Count == 0)
                return null;

            string tr2FileName = info.FullFileName.Replace("da1", "tr3").Replace("Day", "Tick");

            if (File.Exists(tr2FileName))
            {
                List<Quote> lastdayItems = PoboDataImporter.MinQuotesFromTr3(tr2FileName, tr2TimeIndex);

                return lastdayItems;
            }
            else
                return null;
        }

        private QuoteCollection getFiveMinQuotes(RecentFileInfo info)
        {
            string minFileName = PoboDataImporter.FullNameOf(info.FullFileName, RecordType.FiveMinuteRecord);

            if (!File.Exists(minFileName))
                return null;

            List<Quote> minuteItems = PoboDataImporter.Import(minFileName, RecordType.FiveMinuteRecord);

            if (minuteItems == null || minuteItems.Count == 0)
                return null;

            minQuote = new QuoteCollection(info.Name, minFileName, RecordType.FiveMinuteRecord, minuteItems);

            int lastDash = minFileName.LastIndexOf(@"\");

            string indexFileName = minFileName.Substring(0, lastDash) + "index.hex";

            if (!File.Exists(indexFileName))
            {
                int lastIndex = 0, nextIndex;

                DateTimeOffset lastDate = minQuote.Since.Date;

                for (int i = 1; i < minQuote.Count; i++)
                {
                    Quote quote = minQuote.DataCollection[i];
                    if (quote.Time.Date == lastDate)
                        continue;

                    nextIndex = i;
                    lastDate = quote.Time.Date;
                    if (itemsPerDay == nextIndex - lastIndex)
                    {
                        fiveMinTimeIndex.Clear();
                        for (int k = lastIndex; k < nextIndex; k++)
                        {
                            fiveMinTimeIndex.Add(k - lastIndex, minQuote.DataCollection[k].Time.TimeOfDay);
                        }
                        break;
                    }

                    itemsPerDay = nextIndex - lastIndex;
                    lastIndex = nextIndex;
                }

                if (fiveMinTimeIndex != null && fiveMinTimeIndex.Count != 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(indexFileName, FileMode.Create))
                    {
                        formatter.Serialize(fs, fiveMinTimeIndex);
                        fs.Close();
                    }
                }
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream(indexFileName, FileMode.Open))
                {
                    fiveMinTimeIndex = (Dictionary<int, TimeSpan>)formatter.Deserialize(fs);
                    itemsPerDay = fiveMinTimeIndex.Count;
                    fs.Close();
                }
            }

            return minQuote;
        }

        private void importPoboMinuteQuote(RecentFileInfo info)
        {
            #region Import the 5-minute dayQuotes

            minQuote = getFiveMinQuotes(info);

            List<Quote> lastdayItems = getTr2Record(info);

            if (lastdayItems != null && lastdayItems[0].Time > minQuote.Until)
            {
                List<Quote> extraFiveMinutesItem = QuoteCollection.FromShortTerms(lastdayItems, RecordType.FiveMinuteRecord);
                if (extraFiveMinutesItem != null && extraFiveMinutesItem.Count != 0)
                    minQuote.AddRange(extraFiveMinutesItem);
            }

            getEndingDict();

            theMinStick = ordinalStickOf(minQuote);
            allCandles.Add(minQuote.QuoteType, theMinStick);

            theMinOutline = ordinalOutlineOf(minQuote);

            zedShortTerm.GraphPane.CurveList.Clear();
            zedShortTerm.GraphPane.GraphObjList.Clear();

            double min = minQuote.Floor;
            double max = minQuote.Ceiling;

            GraphPane myPane = zedShortTerm.GraphPane;

            myPane.XAxis.Scale.Min = endingsDict.Keys.First();
            myPane.XAxis.Scale.Max = endingsDict.Keys.Last();
            //myPane.XAxis.Scale.Min = -itemsPerDay;
            //myPane.XAxis.Scale.Max = minQuote.Count + 2 * itemsPerDay;
            myPane.XAxis.Scale.MinorStep = itemsPerDay;
            myPane.XAxis.Scale.MajorStep = itemsPerDay * 10;

            fitOrdinalYAxis(zedShortTerm, minQuote);

            if (IsKLineShown)
                myPane.CurveList.Add(theMinStick);

            if (IsOutlineShown)
                myPane.CurveList.Add(theMinOutline);

            zedShortTerm.Invalidate();

            #endregion
        }

        private LineItem ordinalOutlineOf(QuoteCollection quotes)
        {
            if (quotes == null || quotes.BaseOutline == null)
                return null;

            int minThreshold = int.Parse(comboIntradayThreshold.SelectedItem.ToString());
            Outline theOutline = OutlineHelper.OutlineFromQuotes(quotes, minThreshold);
            
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            foreach (OutlineItem item in theOutline.Turnings)
            {
                xValues.Add(item.RecordIndex);
                yValues.Add(item.Price);
            }
            LineItem line = new LineItem(quotes.Name, xValues.ToArray(), yValues.ToArray(), Color.Blue, SymbolType.Circle);
            line.Tag = theOutline;

            line.Symbol.Fill = new Fill(Color.CadetBlue);
            line.Symbol.Size = 3;
            line.IsOverrideOrdinal = true;
            line.Symbol.Border.IsVisible = false;
            return line;
        }

        private JapaneseCandleStickItem ordinalStickOf(QuoteCollection newQuote)
        {
            StockPointList spl = new StockPointList();

            for (int i = 0; i < newQuote.Count; i++)
            {
                Quote item = newQuote.DataCollection[i];
                StockPt pt = new StockPt(i, item.High, item.Low, item.Open, item.Close, 0);
                spl.Add(pt);
            }

            JapaneseCandleStickItem kLine = new JapaneseCandleStickItem(newQuote.Name, spl);
            //kLine.Stick.IsAutoSize = true;
            kLine.Stick.Color = Color.Blue;
            kLine.IsOverrideOrdinal = true;
            kLine.Tag = newQuote;

            return kLine;
        }

        private void fitOrdinalYAxis(ZedGraphControl sender, QuoteCollection quotes)
        {
            if (quotes == null)
                return;

            GraphPane thePane = sender.GraphPane;

            double floor, ceiling;

            int minIndex = (int)thePane.XAxis.Scale.Min;
            int maxIndex = (int)thePane.XAxis.Scale.Max;

            double minorStep = thePane.YAxis.Scale.MinorStep;
            
            quotes.ValueRangeOf(minIndex, maxIndex, out ceiling, out floor);
            double step = calcStepSize(ceiling - floor, 10);
            thePane.YAxis.Scale.Min = Math.Truncate(floor / step - 1) * step;
            thePane.YAxis.Scale.Max = Math.Truncate(ceiling / step + 2) * step;

            double ratio = y2Ratio;
            thePane.Y2Axis.Scale.Min = thePane.YAxis.Scale.Min / ratio;
            thePane.Y2Axis.Scale.Max = thePane.YAxis.Scale.Max / ratio;

            fitY3Scale();

            sender.AxisChange();
        }

        private void zedShortTerm_ScrollDoneEvent(ZedGraphControl sender, ScrollBar scrollBar, ZoomState oldState, ZoomState newState)
        {
            fitOrdinalYAxis(zedShortTerm, this[RecordType.FiveMinuteRecord]);
        }

        private void zedShortTerm_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            fitOrdinalYAxis(zedShortTerm, this[RecordType.FiveMinuteRecord]);

        }

        //private void fitShortTermGraph(ZedGraphControl zed, QuoteCollection quotes)
        //{
        //    GraphPane myPane = zed.GraphPane;

        //    double min = quotes.Floor;
        //    double max = quotes.Ceiling;

        //    myPane.XAxis.Scale.Min = quotes.Since.DateTime.AddHours(-1).ToOADate();
        //    myPane.XAxis.Scale.Max = DateTime.Now.AddHours(2).ToOADate();

        //    myPane.YAxis.Scale.MinorStep = calcStepSize(max - min, 20);
        //    double majorStep = myPane.YAxis.Scale.MajorStep = calcStepSize(max - min, 5);

        //    myPane.YAxis.Scale.Min = Math.Truncate(min / majorStep - 0.2) * majorStep;
        //    myPane.YAxis.Scale.Max = Math.Truncate(max / majorStep + 1.2) * majorStep;

        //    double mag = Math.Floor(Math.Log10(max) - 2);
        //    Double step = Math.Pow(10, mag);
        //    if (max / step < 300)
        //        step /= 2;

        //    zed.AxisChange();
        //}

        private LineItem ordinalCurveOf(OrbitDescription description)
        {
            PlanetId id = description.Owner;

            SeFlg centric = description.Centric;

            List<double> original = centric == SeFlg.GEOCENTRIC ? geoEndingPos[id] : helioEndingPos[id];

            if (description.IsReversed)
                original = reversedOf(original);

            List<double> longitudes0 = null;

            if (description.Centric == SeFlg.HELIOCENTRIC || id == PlanetId.Earth_Rotation || id < PlanetId.SE_MERCURY)
                longitudes0 = smoothingOf(original, !description.IsReversed);
            else
            {
                double jump = id >= PlanetId.Five_Average ? 30 : 300;

                longitudes0 = adjustedOf(original, jump);
            }

            return ordinalCurveOf(description, longitudes0);
        }

        private LineItem ordinalCurveOf(OrbitDescription description, List<double> longitudes0)
        {
            PlanetId id = description.Owner;
            String name = description.ToString();
            Color color = Planet.PlanetsColors.ContainsKey(id) ? Planet.PlanetsColors[id].First() : Color.Gray;

            if (id == PlanetId.Earth_Rotation)
            {
                List<double> yValues = shiftOf(longitudes0, description.Offset);
                LineItem line = new LineItem(name, endingsDict.Keys.ToArray(), yValues.ToArray(), color, SymbolType.None);
                line.Tag = description;
                line.IsY2Axis = true;
                line.YAxisIndex = 1;
                line.IsOverrideOrdinal = true;
                return line;
            }
            else
            {
                List<double> yValues = shiftOf(longitudes0, description.Offset + description.PivotShift);
                LineItem line = new LineItem(name, endingsDict.Keys.ToArray(), yValues.ToArray(), color, SymbolType.None);
                line.Tag = description;
                line.IsY2Axis = true;
                line.IsOverrideOrdinal = true;
                return line;
            }
        }

        private List<CurveItem> ordinalOrbitCurvesOf(OrbitSet orbits)
        {
            PlanetId id = orbits.Owner;

            List<CurveItem> result = new List<CurveItem>();

            if (id == PlanetId.Earth_Rotation)
            {
                double slope = y3Ratio;
                if (recentPivot == null || slope <= 0)
                    return null;

                double pivotTimeValue = Ephemeris.ToJulianDay(recentPivot.Time);

                List<double> degrees = new List<double>();
                double degree = 0;

                foreach (KeyValuePair<double, double> kvp in endingsDict)
                {
                    degree = 360 * (orbits.IsReversed ? (pivotTimeValue - kvp.Value) : (kvp.Value - pivotTimeValue));
                    degrees.Add(degree);
                }

                double floor = zedShortTerm.GraphPane.Y2AxisList[1].Scale.Min;
                double ceiling = zedShortTerm.GraphPane.Y2AxisList[1].Scale.Max;

                OrbitDescription desc = new OrbitDescription(false, SeFlg.GEOCENTRIC, orbits, 0, recentPivot, slope);

                int maxRound = (int)Math.Floor((ceiling - orbits.Offset - degrees.Min()) / 36000);
                int minRound = (int)Math.Ceiling((floor - orbits.Offset - degrees.Max()) / 36000);

                result.Add(ordinalCurveOf(desc, degrees));

                //for (int round = minRound; round <= maxRound; round++)
                //{
                //    OrbitDescription desc = new OrbitDescription(false, SeFlg.GEOCENTRIC, orbits, round, recentPivot, slope);

                //    result.Add(ordinalCurveOf(desc, degrees));
                //}

            }
            else
            {
                SeFlg centric = (buttonCentric.Text == "Geocentric") ? SeFlg.GEOCENTRIC : SeFlg.HELIOCENTRIC;
                double slope = y2Ratio;
                List<double> original = (centric == SeFlg.GEOCENTRIC) ? geoEndingPos[id] : helioEndingPos[id];

                if (orbits.IsReversed)
                    original = reversedOf(original);

                List<double> longitudes0 = null;

                if (centric == SeFlg.HELIOCENTRIC || id == PlanetId.Earth_Rotation || id < PlanetId.SE_MERCURY)
                    longitudes0 = smoothingOf(original, !orbits.IsReversed);
                else
                {
                    double jump = id >= PlanetId.Five_Average ? 30 : 300;

                    longitudes0 = adjustedOf(original, jump);
                }

                double floor = zedShortTerm.GraphPane.Y2Axis.Scale.Min;
                double ceiling = zedShortTerm.GraphPane.Y2Axis.Scale.Max;

                double pivotOffset = 0;

                if (recentPivot != null)
                {
                    Position pos = Ephemeris.PositionOf(recentPivot.Time, id, centric);
                    double orig = orbits.IsReversed ? 360 - pos.Longitude : pos.Longitude;
                    pivotOffset = (recentPivot.Price / slope - orig);
                }

                int maxRound = (int)Math.Floor((ceiling - orbits.Offset - pivotOffset - longitudes0.Min()) / 360);
                int minRound = (int)Math.Ceiling((floor - orbits.Offset - pivotOffset - longitudes0.Max()) / 360);

                for (int round = minRound; round <= maxRound; round++)
                {
                    OrbitDescription desc = new OrbitDescription(false, centric, orbits, round, recentPivot, slope);

                    result.Add(ordinalCurveOf(desc, longitudes0));
                }
            }

            return result;
        }

        private void comboIntradayThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            int minThreshold = int.Parse(comboIntradayThreshold.SelectedItem.ToString());

            if (minQuote == null)
                return;

            if (theMinOutline == null || (theMinOutline.Tag as Outline).Threshold != minThreshold)
            {
                if (zedShortTerm.GraphPane.CurveList.Contains(theMinOutline))
                {
                    zedShortTerm.GraphPane.CurveList.Remove(theMinOutline);
                    theMinOutline = ordinalOutlineOf(minQuote);
                    zedShortTerm.GraphPane.CurveList.Add(theMinOutline);
                    zedShortTerm.Invalidate();
                }
                else
                    theMinOutline  = ordinalOutlineOf(minQuote);
            }
        }

        private string zedShortTerm_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            if (curve.Tag != null && curve.Tag is QuoteCollection)
            {
                QuoteCollection quotes = curve.Tag as QuoteCollection;

                Quote theQuote = quotes.DataCollection[iPt];
                return String.Format("{0}: H={1}, L={2}, O={3}, C={4}", theQuote.Time.ToString("MM-dd, ddd"),
                    theQuote.High, theQuote.Low, theQuote.Open, theQuote.Close);
            }
            else if (curve.Tag != null && curve.Tag is Outline)
            {
                double x = curve.Points[iPt].X;
                OutlineItem pivot = (curve.Tag as Outline)[x];

                if (pivot == null)
                    return "";

                Phenomena events = new Phenomena(pivot.Time, TimeWindow, defaultAspectImportance);

                return String.Format("{0}\r\n{1}", pivot, events);
            }
            else if (curve.Tag is OrbitDescription && curve.YAxisIndex == 0)
            {
                OrbitDescription desc = curve.Tag as OrbitDescription;

                double x, x2, y, y2;
                PointF clientPos = sender.PointToClient(Cursor.Position);
                sender.GraphPane.ReverseTransform(clientPos, out x, out x2, out y, out y2);

                double around = Math.Round(x);
                if (pane.XAxis.Scale.Min > around || pane.XAxis.Scale.Max < around)
                    return "";

                DateTimeOffset time;
                if (endingsDict.ContainsKey(around))
                {
                    time = Ephemeris.UtcFromJulianDay(endingsDict[around]).ToLocalTime();
                }
                else
                {
                    double after = (from index in endingsDict.Keys
                                    where index < around
                                    select index).Last();
                    time = Ephemeris.UtcFromJulianDay(endingsDict[after]).ToLocalTime().AddMinutes(around - after);
                }

                double degree = desc.DegreeOn(time);
                if (Math.Abs(degree - y2) > 300)
                    degree = Math.Round((y2 - degree) / 360) * 360 + degree;

                return string.Format("{0}@{1:F2}({2:F2}) on {3}", desc, degree * desc.Slope, degree % 360, time);
            }
            else if (curve.Tag is OrbitDescription && curve.YAxisIndex == 1)
            {
                OrbitDescription desc = curve.Tag as OrbitDescription;

                double x, y;
                PointF clientPos = sender.PointToClient(Cursor.Position);
                sender.GraphPane.ReverseTransform(clientPos, out x, out y);

                DateTimeOffset time;
                double around = Math.Round(x);
                if (endingsDict.ContainsKey(around))
                {
                    time = Ephemeris.UtcFromJulianDay(endingsDict[around]).ToLocalTime();
                }
                else
                {
                    double after = (from index in endingsDict.Keys
                                    where index < around
                                    select index).Last();
                    time = Ephemeris.UtcFromJulianDay(endingsDict[after]).ToLocalTime().AddMinutes(around - after);
                }

                double degree = desc.DegreeOn(time);

                TimeSpan span = TimeSpan.FromMinutes(degree * 4);

                return string.Format("{0}@{1:F2}({2:F2}) on {3}", desc, degree * y3Ratio, degree % 360, time);
            } 
            else
            {
                string name = curve.Label.Text;
                double degree = curve[iPt].Y;
                DateTime time = DateTime.FromOADate(curve[iPt].X);
                return string.Format("{0}@{1:F1} on {2}", name, degree % 360, time.ToShortTimeString());
            }
        }

        List<GraphObj> shortTermMouseIndicators = new List<GraphObj>();

        private void zedShortTerm_MouseMove(object sender, MouseEventArgs e)
        {
            if (minQuote == null)
                return;

            try
            {
                GraphPane pane = (sender as ZedGraphControl).GraphPane;

                PointF mousePt = new PointF(e.X, e.Y);

                if (shortTermMouseIndicators.Count != 0)
                {
                    foreach (GraphObj obj in shortTermMouseIndicators)
                    {
                        if (pane.GraphObjList.Contains(obj))
                            pane.GraphObjList.Remove(obj);
                    }
                    shortTermMouseIndicators.Clear();
                }

                double x, x2, y, y2;

                pane.ReverseTransform(mousePt, out x, out x2, out y, out y2);

                if (pane.XAxis.Scale.Min > x || pane.XAxis.Scale.Max < x)
                    return;

                int index = (int)Math.Round(x);

                DateTimeOffset time;

                if (index >= 0 && index <= minQuote.Count - 1)
                {
                    Quote quote = minQuote.DataCollection[index];
                    time = quote.Time;
                }
                else if (endingsDict.ContainsKey(index))
                {
                    time = Ephemeris.UtcFromJulianDay(endingsDict[index]).ToLocalTime();
                }
                else
                    return;

                double xRect = (x - pane.XAxis.Scale.Min) / (pane.XAxis.Scale.Max - pane.XAxis.Scale.Min);
                double yRect = (pane.YAxis.Scale.Max - y) / (pane.YAxis.Scale.Max - pane.YAxis.Scale.Min);

                //string posString = string.Format("{0}: x={1}, index={2}, quote.time={3}", time.ToString("yy-MM-dd"), x, index, quote.Time);
                //TextObj dateIndicator = lableOf(posString, xRect, 1, CoordType.ChartFraction, AlignH.Center, AlignV.Top);
                TextObj dateIndicator = lableOf(time.ToString("yy-MM-dd"), xRect, 1, CoordType.ChartFraction, AlignH.Center, AlignV.Top);
                shortTermMouseIndicators.Add(dateIndicator);

                TextObj timeIndicator = lableOf(time.ToString("hh:mm"), xRect, 0, CoordType.ChartFraction, AlignH.Center, AlignV.Bottom);
                shortTermMouseIndicators.Add(timeIndicator);

                TextObj priceIndicator = lableOf(y.ToString("F2"), 0, yRect, CoordType.ChartFraction, AlignH.Right, AlignV.Center);
                //TextObj degreeIndicator = lableOf((y2).ToString("F1"), 1, yRect, CoordType.ChartFraction, AlignH.Left, AlignV.Center);
                shortTermMouseIndicators.Add(priceIndicator);
                //mouseIndicators.Add(degreeIndicator);

                foreach (GraphObj obj in shortTermMouseIndicators)
                {
                    pane.GraphObjList.Add(obj);
                }
                (sender as ZedGraphControl).Invalidate();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void buttonCentric_Click(object sender, EventArgs e)
        {
            if (buttonCentric.Text == "Geocentric")
            {
                buttonCentric.Text = "Heliocentric";
                buttonCentric.ToolTipText = "Switch to Geocentric";

                foreach (OrbitSet orbits in currentOrbitSets)
                {
                    if (orbits.Owner == PlanetId.SE_SUN)
                        orbits.Owner = PlanetId.SE_EARTH;
                }

                if(starPanel.Controls[0].Text == Planet.Glyphs[PlanetId.SE_SUN].ToString())
                {
                    starPanel.Controls[0].Tag = PlanetId.SE_EARTH;
                    starPanel.Controls[0].Text = Planet.Glyphs[PlanetId.SE_EARTH].ToString();
                }
            }
            else
            {
                buttonCentric.Text = "Geocentric";
                buttonCentric.ToolTipText = "Switch to Heliocentric";

                foreach (OrbitSet orbits in currentOrbitSets)
                {
                    if (orbits.Owner == PlanetId.SE_EARTH)
                        orbits.Owner = PlanetId.SE_SUN;
                }
                if (starPanel.Controls[0].Text == Planet.Glyphs[PlanetId.SE_EARTH].ToString())
                {
                    starPanel.Controls[0].Tag = PlanetId.SE_SUN;
                    starPanel.Controls[0].Text = Planet.Glyphs[PlanetId.SE_SUN].ToString();
                }
            }
            redrawOrdinalOrbits();
        }

        private void textOffset_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double offset;

                if (!double.TryParse(textOffset.Text, out offset))
                {
                    offset = 0;
                }

                PlanetId id = (PlanetId)(comboPlanet.SelectedItem);

                OrbitSet orbits = new OrbitSet(id, false, offset);

                if (!currentOrbitSets.Contains(orbits))
                {
                    currentOrbitSets.Add(orbits);
                    showOrdinalOrbitSet(orbits);
                    zedShortTerm.Invalidate();
                }
                
                orbits = new OrbitSet(id, true, offset);

                if (!currentOrbitSets.Contains(orbits))
                {
                    currentOrbitSets.Add(orbits);
                    showOrdinalOrbitSet(orbits);
                    zedShortTerm.Invalidate();
                }
            }
        }

        private void showOrdinalOrbitSet(OrbitSet orbits)
        {
            List<CurveItem> curves = ordinalOrbitCurvesOf(orbits);

            if (curves == null)
                return;

            foreach (CurveItem curve in curves)
            {
                showOrdinalOrbit(curve);
            }
        }

        private void showOrdinalOrbit(CurveItem curve)
        {
            OrbitDescription desc = curve.Tag as OrbitDescription;

            foreach (CurveItem orbit in zedShortTerm.GraphPane.CurveList)
            {
                if (orbit.Tag is OrbitDescription && (orbit.Tag as OrbitDescription) == desc)
                    return;
            }
            
            zedShortTerm.GraphPane.CurveList.Add(curve);
        }

        private void hideOrdinalOrbitSet(OrbitSet orbits)
        {

            for (int i = zedShortTerm.GraphPane.CurveList.Count-1; i >= 0; i--)
            {
                CurveItem orbit = zedShortTerm.GraphPane.CurveList[i];
                if (orbit.Tag is OrbitDescription && orbits.Contains(orbit.Tag as OrbitDescription))
                    zedShortTerm.GraphPane.CurveList.Remove(orbit);
            }
        }

        private double y2Ratio
        {
             get
             {
                 double ratio;
                     
                 if (comboY2Ratio.SelectedIndex == -1)
                 {
                     if (double.TryParse(comboY2Ratio.Text, out ratio))
                         return ratio;
                     else
                        return 1;
                 }
                 else
                 {
                     if (double.TryParse(comboY2Ratio.SelectedItem.ToString(), out ratio))
                         return ratio;
                     else
                         return -1;
                 }
             }
            set
            {
                fitOrdinalYAxis(zedShortTerm, minQuote);

                foreach (OrbitSet orbits in currentOrbitSets)
                {
                    hideOrdinalOrbitSet(orbits);
                    showOrdinalOrbitSet(orbits);
                }

                zedShortTerm.Invalidate();
            }
        }

        private double y3Ratio
        {
            get
            {
                double ratio;

                if (textY3Ratio.Text == "" || !double.TryParse(textY3Ratio.Text, out ratio))
                    return 1;
                else
                    return ratio;
            }
        }

        private void comboY2Ratio_SelectedIndexChanged(object sender, EventArgs e)
        {
            double ratio;

            if (double.TryParse(comboY2Ratio.SelectedItem.ToString(), out ratio))
                y2Ratio = ratio;
        }


        private void comboY2Ratio_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double ratio;

                if (double.TryParse(comboY2Ratio.Text, out ratio))
                    y2Ratio = ratio;
            }
        }

        private void redrawOrdinalOrbits()
        {
            foreach (OrbitSet orbits in currentOrbitSets)
            {
                hideOrdinalOrbitSet(orbits);
                showOrdinalOrbitSet(orbits);
            }

            zedShortTerm.Invalidate();
        }

        private bool zedShortTerm_DoubleClickEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            PointF mousePt = new PointF(e.X, e.Y);

            CurveList eventsCurves = new CurveList();
            CurveList outlineCurves = new CurveList();
            CurveList orbitsCurves = new CurveList();
            CurveItem nearestCurve;
            int nearestPt;

            foreach (CurveItem curve in sender.GraphPane.CurveList)
            {
                if (curve.Tag is List<IPlanetEvent>)
                    eventsCurves.Add(curve);
                else if (curve.Tag is Outline)
                    outlineCurves.Add(curve);
                else if (curve.Tag is OrbitDescription)
                    orbitsCurves.Add(curve);
            }

            if (outlineCurves.Count != 0 && sender.GraphPane.FindNearestPoint(mousePt, outlineCurves, out nearestCurve, out nearestPt))
            {
                if (nearestCurve is LineItem && nearestCurve.Tag is Outline)
                {
                    Outline theOutline = nearestCurve.Tag as Outline;
                    recentPivot = theOutline.Pivots[nearestPt];
                    fitY3Scale();
                }

                redrawOrdinalOrbits();
                return true;
            }
            else if (recentPivot != null)
            {
                recentPivot = null;
                fitY3Scale();
                redrawOrdinalOrbits();
                return true;
            }

            return false;
        }

        private void fitY3Scale()
        {
            double slope = y3Ratio;

            GraphPane thePane = zedShortTerm.GraphPane;

            if (recentPivot == null || slope <= 0)
            {
                thePane.Y2AxisList[1].Scale.Min = thePane.Y2Axis.Scale.Min;
                thePane.Y2AxisList[1].Scale.Max = thePane.Y2Axis.Scale.Max;

                if (thePane.Y2AxisList[1].IsVisible)
                {
                    thePane.Y2AxisList[1].IsVisible = false;
                }
            }
            else
            {
                double pivotPrice = recentPivot.Price;
                thePane.Y2AxisList[1].Scale.Min = (thePane.YAxis.Scale.Min - pivotPrice) / slope;
                thePane.Y2AxisList[1].Scale.Max = (thePane.YAxis.Scale.Max - pivotPrice) / slope;

                if (!thePane.Y2AxisList[1].IsVisible)
                {
                    thePane.Y2AxisList[1].IsVisible = true;
                }
            }
            zedShortTerm.AxisChange();

        }

        private void buttonClearOrbitSets_Click(object sender, EventArgs e)
        {
            currentOrbitSets.Clear();

            for (int i = zedShortTerm.GraphPane.CurveList.Count - 1; i >= 0; i -- )
            {
                CurveItem curve = zedShortTerm.GraphPane.CurveList[i];

                if (curve.Tag is OrbitDescription && !(curve.Tag as OrbitDescription).IsLocked)
                    zedShortTerm.GraphPane.CurveList.RemoveAt(i);
            }
            zedShortTerm.Invalidate();
        }

        private void textY3Ratio_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                double slope = y3Ratio;
                if (e.KeyCode == Keys.Up)
                    textY3Ratio.Text = (slope * 2).ToString();
                else if (e.KeyCode == Keys.Down)
                    textY3Ratio.Text = (slope / 2).ToString();

                fitY3Scale();

                if (recentPivot != null)
                {
                    OrbitSet orbits = new OrbitSet(PlanetId.Earth_Rotation, false, 0);

                    if (!currentOrbitSets.Contains(orbits))
                    {
                        currentOrbitSets.Add(orbits);
                        showOrdinalOrbitSet(orbits);
                    }
                    //else
                    //    hideOrdinalOrbitSet(orbits);

                    
                    orbits = new OrbitSet(PlanetId.Earth_Rotation, true, 0);

                    if (!currentOrbitSets.Contains(orbits))
                    {
                        currentOrbitSets.Add(orbits);
                        showOrdinalOrbitSet(orbits);
                    }
                    //else
                    //    hideOrdinalOrbitSet(orbits);

                }

                zedShortTerm.Invalidate();
            }
        }

        void star_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            PlanetId id = (PlanetId)cb.Tag;

            if (cb.Checked)
            {
                double offset;

                if (!double.TryParse(textOffset.Text, out offset))
                {
                    offset = 0;
                }

                OrbitSet orbits = new OrbitSet(id, false, offset);

                if (!currentOrbitSets.Contains(orbits))
                {
                    currentOrbitSets.Add(orbits);
                    showOrdinalOrbitSet(orbits);
                }

                orbits = new OrbitSet(id, true, offset);

                if (!currentOrbitSets.Contains(orbits))
                {
                    currentOrbitSets.Add(orbits);
                    showOrdinalOrbitSet(orbits);
                }
            }
            else
            {
                for (int j = currentOrbitSets.Count - 1; j >= 0; j--)
                {
                    if (currentOrbitSets[j].Owner == id)
                        currentOrbitSets.RemoveAt(j);
                }

                for (int i = zedShortTerm.GraphPane.CurveList.Count - 1; i >= 0; i--)
                {
                    CurveItem curve = zedShortTerm.GraphPane.CurveList[i];

                    if (!(curve.Tag is OrbitDescription))
                        continue;

                    OrbitDescription desc = curve.Tag as OrbitDescription;

                    if (desc.Owner == id)
                        zedShortTerm.GraphPane.CurveList.RemoveAt(i);
                }
            }

            zedShortTerm.Invalidate();
        }


        #endregion


    }
}
