using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuoteHelper;
using NumberHelper;
using NumberHelper.DoubleHelper;
using AstroHelper;
using ZedGraph;
using System.IO;

namespace AstroTeller
{
    public delegate void OutlineChangedDelegate();

    public partial class DetailForm : Form
    {
        #region Fields
        public QuoteCollection History { get; private set; }

        public event OutlineChangedDelegate OutlineChanged;

        private RecordType quoteType;
        public RecordType QuoteType
        {
            get { return quoteType; }
            private set
            {
                RecordType old = quoteType;

                if (value <= History.QuoteType)
                    quoteType = History.QuoteType;
                else if (value > RecordType.YearRecord)
                    quoteType = RecordType.YearRecord;
                else
                    quoteType = value;

                if (old != quoteType && OutlineChanged != null)
                    OutlineChanged();

                toolStripButtonDown.Enabled = quoteType > History.QuoteType ? true : false;
                toolStripButtonUp.Enabled = quoteType < RecordType.YearRecord ? true : false;
            }
        }

        public SearchMode Mode { get; private set; }

        private int threshold;
        public int Threshold 
        { 
            get { return threshold; }
            private set 
            {
                if (value < 0)
                    throw new Exception();
                else if (value == threshold)
                    return;

                threshold = value;
                if (OutlineChanged != null)
                    OutlineChanged();
            }
        }

        public QuoteCollection Active { get { return History[QuoteType]; } }

        List<Quote> ActivedQuotes
        {
            get
            {
                return History[QuoteType].DataCollection;
            }
        }

        public Outline CurrentOutline { get { return Active.OutlineOf(Threshold == 0 ? TrendMarker.DefaultThreshold : Threshold); } }

        public string ActivedDateFormat { get { return QuoteCollection.DefaultDateTimeFormats[QuoteType]; } }

        CyclesPresenter helper = null;

        Vibration wave = null;

        public ConcernedEvent FocusedEvent
        {
            get { return (ConcernedEvent)Enum.Parse(typeof(ConcernedEvent), comboStarEvents.SelectedItem.ToString()); }
        }

        #region Extra ToolStripItems

        ToolStripItem[] quotesItems, astrolabeItems, chartsItems, patternItems, cycleItems, polygonsItems;

        DateTimePicker calendar = new DateTimePicker();
        ToolStripControlHost calendarHost = null;

        NumericUpDown numThreshold = new NumericUpDown();
        ToolStripControlHost thresholdHost = null;

        CheckedListBox planetsCheck = new CheckedListBox();
        ToolStripControlHost planetsHost = null;

        CheckedListBox orbitsCheck = new CheckedListBox();
        ToolStripControlHost orbitsHost = null;

        ToolStripTextBox textShift = new ToolStripTextBox();

        CheckedListBox plusCheck = new CheckedListBox();
        ToolStripControlHost plusHost = null;

        //ToolStripComboBox comboStudyObjects = new ToolStripComboBox();

        CheckedListBox minusCheck = new CheckedListBox();
        ToolStripControlHost minusHost = null;

        ToolStripComboBox comboOrientation = new ToolStripComboBox();

        #endregion

        #endregion

        #region Constructor

        public DetailForm(QuoteCollection obj)
        {
            History = obj;
            this.Name = History.Name;
            InitializeComponent();
            //this.polygonControl1.Reset();

            QuoteType = History.QuoteType;

            polygonControl1.History = History;

            helper = new CyclesPresenter(cycleControl, History);

            initializeToolStripItems();

            initializeChartControl();

            dataGridViewQuotes.AutoGenerateColumns = true;
            dataGridViewAstrolabe.AutoGenerateColumns = true;
            dataGridViewRelations.AutoGenerateColumns = true;
            dataGridViewPattern.AutoGenerateColumns = true;

            foreach (SearchMode mode in (SearchMode[])System.Enum.GetValues(typeof(SearchMode)))
            {
                comboBoxMatchingMode.Items.Add(mode.ToString());
            }
            comboBoxMatchingMode.SelectedIndex = (int)Ephemeris.DefaultSearchMode;

            this.Text = Active.Description;

            displayQuotes();

            displayPattern();

            Display();

            this.polygonControl1.Size = this.polygonControl1.FittedSize();

            tabControl1.SelectedIndex = 0;

            this.OutlineChanged += new OutlineChangedDelegate(DetailForm_OutlineChanged);
        }


        #endregion
        
        #region General functions

        void DetailForm_OutlineChanged()
        {
            if (polygonControl1 != null)// && polygonControl1.PivotsHolder != null)
            {
                //polygonControl1.PivotsHolder.AddPivots(CurrentOutline);
                polygonControl1.Redraw();
            }

            if (History != null)
            {
                History.CurrentOutline = CurrentOutline;
            }

            if (comboPrice.SelectedItem.ToString() == PriceMappingRules.FilledPivots.ToString())
            {
                helper.AddCurve("$");
            }

            displayPattern();
        }

        public void Display()
        {
            displayKLine();

            displayOutline();

            setRange(chartControl);
        }

        private void initializeToolStripItems()
        {
            Threshold = 6;

            thresholdHost = new ToolStripControlHost(numThreshold);
            initializeCycleToolItems();

            calendar.Value = History.Since.Date;
            calendar.Size = new System.Drawing.Size(150, 20);
            calendar.Format = DateTimePickerFormat.Custom;
            calendar.CustomFormat = "yyyy-MM-dd HH:mm";
            calendar.ValueChanged += new EventHandler(calendar_ValueChanged);
            calendarHost = new ToolStripControlHost(calendar);

            //thresholdHost.Size = new System.Drawing.Size(200, 30);
            numThreshold.Maximum = 30;
            numThreshold.Minimum = 1;
            numThreshold.Value = Threshold;
            numThreshold.ValueChanged += new EventHandler(numThreshold_ValueChanged);

            foreach (KeyValuePair<String, String> kvp in Rectascension.PredefinedFormats)
            {
                comboAngleFormats.Items.Add(kvp.Key);
            }
            comboAngleFormats.SelectedIndex = 0;

            initiatePolygonsPage();

            foreach (KeyValuePair<String, GetTrendDelegate>kvp in GannTrend.Methods)
            {
                comboTrending.Items.Add(kvp.Key);
            }
            comboTrending.SelectedIndex = 0;

            comboEphemeris.SelectedIndex = 0;

            for (ConcernedEvent concerned = ConcernedEvent.None; concerned <= ConcernedEvent.All; concerned ++ )
            {
                comboStarEvents.Items.Add(concerned);
            }
            comboStarEvents.SelectedIndex = comboStarEvents.Items.Count-1;
            comboStarEvents.SelectedIndexChanged += new EventHandler(comboStarEvents_SelectedIndexChanged);

            quotesItems = new ToolStripItem[] { //comboAngleFormats, toolStripSeparator4,
                calendarHost, toolStripSeparator1, toolStripLabelThreshold, thresholdHost };
            astrolabeItems = new ToolStripItem[] { comboEphemeris, comboAngleFormats, toolStripSeparator4,
                calendarHost, toolStripSeparator1, toolStripLabelPeriod, comboBoxMatchingMode,
                toolStripButtonBack, toolStripButtonForward, toolStripSeparator2, 
                toolStripButtonLastBefore, toolStripButtonLast, toolStripButtonNext, toolStripButtonNextAfter};
            patternItems = new ToolStripItem[] { toolStripLabelThreshold, thresholdHost, 
                toolStripSeparator2, comboStarEvents };
            chartsItems = new ToolStripItem[] { calendarHost, toolStripSeparator1, toolStripLabelThreshold, thresholdHost,  
                toolStripButtonUp, toolStripButtonDown, toolStripSeparator2, buttonKLine, toolStripSeparator3, /* buttonSpectrum, */
                buttonSaveTypical, comboTrending};
        }


        public void ReloadHistory(QuoteCollection newHistory)
        {
            if (History != newHistory)
            {
                History = newHistory;
                helper = new CyclesPresenter(cycleControl, History);
            }
        }

        public void OnQuoteManagerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
        
        void numThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (Threshold != numThreshold.Value)
            {
                Threshold = (int)numThreshold.Value;
                displayOutline();
            }
        }

        //private void comboThreshold_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int newThreshold;
        //    if (int.TryParse(comboThreshold.SelectedItem as String, out newThreshold) && newThreshold != Threshold && newThreshold >= 0)
        //    {
        //        Threshold = newThreshold;
        //        displayOutline();
        //    }
        //}

        private void loadToolStripItems(ToolStripItem[] items)
        {
            toolStrip1.Items.Clear();
            toolStrip1.Items.AddRange(items);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage thePage = tabControl1.SelectedTab;
            switch(thePage.Text)
            {
                case "Quotes":
                    statusLable.Text = History.ToString();
                    loadToolStripItems(quotesItems);
                    break;
                case "Patterns":
                    loadToolStripItems(patternItems);
                    break;
                case "Astrolabe":
                    loadToolStripItems(astrolabeItems);
                    break;
                case "Charts":
                    loadToolStripItems(chartsItems);
                    break;
                case "Cycles":
                    loadToolStripItems(cycleItems);
                    break;
                case "Polygons":
                    loadToolStripItems(polygonsItems);
                    break;
                default:
                    break;
            }
        }

        private void comboAngleFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rectascension.DefaultRectascensionFormat  = Rectascension.PredefinedFormats[comboAngleFormats.SelectedItem as String];

            if (bindingSourceAstrolabe.DataSource != null)
            {
                dataGridViewAstrolabe.Invalidate();
                dataGridViewRelations.Invalidate();
            }

        }

        private void DetailForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Tag != null && this.Tag is QuoteManagerForm)
            {
                ((QuoteManagerForm)(this.Tag)).Tag = null;
            }
        }

        #endregion

        #region Quotes Handling

        private void displayQuotes()
        {
            //Outline outline = CurrentOutline == null ? History.CurrentOutline : CurrentOutline;

            var quoteQuery =
                from quote in ActivedQuotes
                let quoteDate = quote.Date
                select new
                {
                    ItemNum = ActivedQuotes.IndexOf(quote),
                    Span = (quoteDate - History.Since).TotalDays,
                    //Kind = (from around in outline
                    //        where around == quoteDate
                    //        select outline.KindOf(around).ToString()).FirstOrDefault(),
                    Date = quote.Date.ToString(ActivedDateFormat),
                    quote.Open,
                    quote.High,
                    quote.Low,
                    quote.Close
                };

            bindingSourceQuotes.DataSource = quoteQuery.ToList();

            DateTimeOffset startDate = ActivedQuotes[0].Date;
            calendar.Value = new DateTime(startDate.Year, startDate.Month, startDate.Day);
        }

        private void dataGridViewQuotes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.RowIndex >= ActivedQuotes.Count)
                return;

            int rowIndex = e.RowIndex;

            DateTimeOffset utcMoment = ActivedQuotes[rowIndex].Date;

            calendar.Value = utcMoment.DateTime;

            tabControl1.SelectedIndex = 1;
        }

        #endregion

        #region Astrolabe Handling

        private void comboEphemeris_SelectedIndexChanged(object sender, EventArgs e)
        {
            String text = comboEphemeris.SelectedItem.ToString();

            if (text == "Geocentric")
            {
                Ephemeris.CurrentEphemeris = Ephemeris.Geocentric;
                helper.Orbits = helper.GeocentricOrbits;
            }
            else if (text == "Heliocentric")
            {
                Ephemeris.CurrentEphemeris = Ephemeris.Heliocentric;
                helper.Orbits = helper.HeliocentricOrbits;
            }
            calendar_ValueChanged(this, null);
            helper.RedrawYValues();
        }

        void calendar_ValueChanged(object sender, EventArgs e)
        {
            DateTimeOffset time = new DateTimeOffset(calendar.Value, TimeSpan.Zero);

            List<Position> positions = Ephemeris.CurrentEphemeris[time];

            MatchRules during = new MatchRules(time, Mode);

            List<Relation> relations = Ephemeris.RelationsWithin(during);

            var positionQuery = from position in positions
                                select new { position.Owner, Symbol = Planet.SymbolOf(position.Owner),
                                    Longitude = position.Longitude.AngleFormatOf(Rectascension.DefaultRectascensionFormat), 
                                    Latitude = position.Latitude.ToString("F5"),
                                    Distance = position.Distance.ToString("F6"), 
                                    LongVelo = position.LongitudeVelocity.ToString("F4"), 
                                    LatiVelo = position.LatitudeVelocity.ToString("F5"), 
                                    DistVelo = position.DistanceVelocity.ToString("F6")
                                };

            bindingSourceAstrolabe.DataSource = positionQuery;

            bindingSourceRelations.DataSource = relations;

            if (CurrentOutline != null)
                statusLable.Text = (CurrentOutline.Sequences.ContainsKey(time)) ?  CurrentOutline[time].ToString() : "";

            polygonControl1.Date = time;
            //showQuotes(time);
        }

        //private void showQuotes(DateTimeOffset time)
        //{
        //    if (polygonControl1 != null && time >= History.Since)
        //    {
        //        Quote quote = History[time];
        //        if (quote == null)
        //        {
        //            Double dateValue = time.UtcDateTime.ToOADate();
        //            double nearest =
        //                (from dt in History.Dates
        //                 where dt - dateValue < 7 && dt - dateValue > -7
        //                 orderby Math.Abs(dt - dateValue)
        //                 select dt).FirstOrDefault();

        //            if (History.Dates.Contains(nearest) && nearest - dateValue <= 7)
        //            {
        //                quote = History.DataCollection[History.Dates.IndexOf(nearest)];
        //            }
        //        }

        //        if (quote != null)
        //        {
        //            statusLable.Text = quote.ToString(Active.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
        //        } 

        //    }
        //}

        //private void showRelations()
        //{
        //    var relationQuery =
        //        now relation in theHoroscope.Patterns.OutlineValues
        //        orderby relation.Flag.SupperiorId descending, relation.Flag.InferiorId descending, relation.Moment
        //        select new
        //        {
        //            relation.Moment,
        //            //detail.Superior,
        //            //detail.Type,
        //            //detail.Inferior,
        //            relation.Description,
        //            //SuperiorRect = detail.SuperiorPosition.Longitude,
        //            //InferiorRect = detail.InferiorPosition.Longitude,
        //            Angle = relation.InferiorPosition.Longitude - relation.SuperiorPosition.Longitude,
        //            Orb = Math.Round(relation.Orb, 4),
        //            relation.Flag.IsSuperiorRetrograde,
        //            relation.Flag.IsInteriorRetrograde,
        //            relation.Flag.IsSameDirection,
        //            relation.Flag.IsExpanding,
        //            SuperiorVelo = Math.Round(relation.SuperiorPosition.LongitudeVelocity, 5),
        //            InferiorVelo = Math.Round(relation.InferiorPosition.LongitudeVelocity, 5)
        //        };

        //    bindingSourceRelations.DataSource = relationQuery.ToList();

        //}

        private void comboBoxMatchingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Mode = (SearchMode)Enum.Parse(typeof(SearchMode), comboBoxMatchingMode.SelectedItem.ToString());
            calendar_ValueChanged(sender, e);
        }

        private void toolStripButtonBack_Click(object sender, EventArgs e)
        {
            DateTimeOffset date = new DateTimeOffset(calendar.Value, TimeSpan.Zero);
            double dateValue = date.UtcDateTime.ToOADate();
            OutlineItem topOrBottom = null;

            if (CurrentOutline.Pivots.First().DateValue >= dateValue)
            {
                topOrBottom = CurrentOutline.Pivots.First();
            }
            else
            {
                topOrBottom = (from OutlineItem item in CurrentOutline.Pivots
                        where item.DateValue < dateValue
                        select item).Last();
            }
            date = topOrBottom.Date;

            statusLable.Text = topOrBottom.ToString();
            calendar.Value = new DateTime(date.Year, date.Month, date.Day);
        }

        private void toolStripButtonForward_Click(object sender, EventArgs e)
        {
            DateTimeOffset date = new DateTimeOffset(calendar.Value, TimeSpan.Zero);
            double dateValue = date.UtcDateTime.ToOADate();
            OutlineItem topOrBottom = null;

            if (CurrentOutline.Pivots.Last().DateValue <= dateValue)
            {
                topOrBottom = CurrentOutline.Pivots.Last();
            }
            else
            {
                topOrBottom = (from OutlineItem item in CurrentOutline.Pivots
                        where item.DateValue > dateValue
                        select item).First();
            }
            date = topOrBottom.Date;
            statusLable.Text = topOrBottom.ToString();
            calendar.Value = new DateTime(date.Year, date.Month, date.Day);

        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            switch(Mode)
            {
                default:
                case SearchMode.AroundWorkingDay:
                case SearchMode.WithinTheDay:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(-1));
                    break;
                case SearchMode.WithinTheWeek:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(-7));
                    break;
                case SearchMode.WithinTheMonth:
                case SearchMode.WithinTheYear:
                    calendar.Value = calendar.Value.Date.AddDays(-30);
                    break;                    
            }
        }

        private void toolStripButtonLastBefore_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                default:
                case SearchMode.AroundWorkingDay:
                case SearchMode.WithinTheDay:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(-7));
                    break;
                case SearchMode.WithinTheWeek:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(-30));
                    break;
                case SearchMode.WithinTheMonth:
                case SearchMode.WithinTheYear:
                    calendar.Value = calendar.Value.Date.AddDays(-90);
                    break;
            }
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                default:
                case SearchMode.AroundWorkingDay:
                case SearchMode.WithinTheDay:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(1));
                    break;
                case SearchMode.WithinTheWeek:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(7));
                    break;
                case SearchMode.WithinTheMonth:
                case SearchMode.WithinTheYear:
                    calendar.Value = calendar.Value.Date.AddDays(30);
                    break;
            }
        }

        private void toolStripButtonNextAfter_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                default:
                case SearchMode.AroundWorkingDay:
                case SearchMode.WithinTheDay:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(7));
                    break;
                case SearchMode.WithinTheWeek:
                    calendar.Value = calendar.Value.Date.Add(TimeSpan.FromDays(28));
                    break;
                case SearchMode.WithinTheMonth:
                case SearchMode.WithinTheYear:
                    calendar.Value = calendar.Value.Date.AddDays(90);
                    break;
            }
        }

        #endregion

        #region Patterns Handling

        private void displayPattern()
        {
            //Outline outline = CurrentOutline;

            //Dictionary<OutlineItem, DateEvents> topBottoms = new Dictionary<OutlineItem, DateEvents>();

            //foreach (OutlineItem pivot in outline.Pivots)
            //{
            //    if (pivot.Type == PivotType.Bottom|| pivot.Type == PivotType.Top)
            //        topBottoms.Add(pivot, new DateEvents(pivot.Date));
            //}

            //var pivotQuery =
            //    from pivot in topBottoms
            //    select new
            //    {
            //        Date = pivot.Key.Date,
            //        Type = pivot.Key.Type,
            //        Sun = pivot.Value[PlanetId.SE_SUN].Brief,
            //        Moon = pivot.Value[PlanetId.SE_MOON].Brief,
            //        Mercury = pivot.Value[PlanetId.SE_MERCURY].Brief,
            //        Venus = pivot.Value[PlanetId.SE_VENUS].Brief,
            //        Mars = pivot.Value[PlanetId.SE_MARS].Brief,
            //        Jupiter = pivot.Value[PlanetId.SE_JUPITER].Brief,
            //        Saturn = pivot.Value[PlanetId.SE_SATURN].Brief,
            //        Uranus = pivot.Value[PlanetId.SE_URANUS].Brief,
            //        Neptune = pivot.Value[PlanetId.SE_NEPTUNE].Brief,
            //        Pluto = pivot.Value[PlanetId.SE_PLUTO].Brief,
            //        NorthNode = pivot.Value[PlanetId.SE_NORTHNODE].Brief
            //    };

            var pivotQuery =
                from pivot in CurrentOutline.Pivots
                let evt = new DateEvents(pivot.Date)
                select new
                {
                    Date = pivot.Date,
                    Type = pivot.Type,
                    Sun = evt[PlanetId.SE_SUN].BriefOf(FocusedEvent),
                    //Moon = evt[PlanetId.SE_MOON].BriefOf(FocusedEvent),
                    Mercury = evt[PlanetId.SE_MERCURY].BriefOf(FocusedEvent),
                    Venus = evt[PlanetId.SE_VENUS].BriefOf(FocusedEvent),
                    Mars = evt[PlanetId.SE_MARS].BriefOf(FocusedEvent),
                    Jupiter = evt[PlanetId.SE_JUPITER].BriefOf(FocusedEvent),
                    Saturn = evt[PlanetId.SE_SATURN].BriefOf(FocusedEvent),
                    Uranus = evt[PlanetId.SE_URANUS].BriefOf(FocusedEvent),
                    Neptune = evt[PlanetId.SE_NEPTUNE].BriefOf(FocusedEvent),
                    Pluto = evt[PlanetId.SE_PLUTO].BriefOf(FocusedEvent),
                    NorthNode = evt[PlanetId.SE_NORTHNODE].BriefOf(FocusedEvent),
                    Chiron = evt[PlanetId.SE_CHIRON].BriefOf(FocusedEvent)
                };

            var dates = pivotQuery.ToList();

            bindingSourcePatterns.DataSource = dates;

            //dataGridViewPattern.DataSource = dates;
        }

        private void dataGridViewPattern_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 2|| e.RowIndex < 0)
            {
                return;
            }

            DateTimeOffset date = (DateTimeOffset)dataGridViewPattern.Rows[e.RowIndex].Cells[0].Value;

            String starName = "SE_" + dataGridViewPattern.Columns[e.ColumnIndex].DataPropertyName.ToUpper();
            PlanetId id = (PlanetId)Enum.Parse(typeof(PlanetId), starName);

            DateEvents evt = new DateEvents(date);
            PlanetEvents pl = evt[id];

            statusLable.Text = pl.StatusOf(FocusedEvent).Replace("\r\n", "     ");
        }

        void comboStarEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayPattern();
        }




        //private int countOfList(List<Relation> relations, PlanetId id)
        //{
        //    int count = 0;
        //    foreach (Relation rel in relations)
        //    {
        //        if (rel.Superior == id || rel.Inferior == id)
        //            count++;
        //    }
        //    return count;
        //}


        //private void toolStripButtonMatch_Click(object sender, EventArgs e)
        //{
        //    //MajorCollection topBottoms = History.MajorsOf(Threshold);

        //    //var rulesQuery =
        //    //    from mj in topBottoms
        //    //    where mj.Kind == PivotType.Top || mj.Kind == PivotType.Bottom
        //    //    let physicalAngle = Polygon.Circle360.AngleOf(mj.Price * 10)
        //    //    let rule = new Rulership(physicalAngle)
        //    //    select new { mj.Kind, mj.Date, Value = mj.Price, rule.Ruler, rule.Remains, rule.House, rule.Ruling, rule.IsExactly };

        //    //bindingSourcePatterns.DataSource = rulesQuery.ToList();

        //    //List<Rulership> rules = new List<Rulership>();

        //    //foreach (Major mj in topBottoms)
        //    //{
        //    //    Angle physicalAngle = Polygon.Circle360.AngleOf(mj.Price);
        //    //    Rulership newRule = new Rulership(physicalAngle);
        //    //    rules.Add(newRule);
        //    //}

        //    //bindingSourcePatterns.DataSource = rules;
        //}

        #endregion

        #region Shape related functions

        private void initializeChartControl()
        {
            // get a reference destination the GraphPane
            GraphPane myPane = chartControl.GraphPane;

            // Disable the Titles
            myPane.Title.IsVisible = false;
            myPane.XAxis.Title.IsVisible = false;
            myPane.YAxis.Title.IsVisible = false;

            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = Color.LightGray;
            myPane.YAxis.MajorGrid.Color = Color.LightGray;
            //myPane.Shape.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 210), -45F);

            myPane.XAxis.Type = AxisType.Date;

            //thePane.XAxis.Scale.Format = History.ActivedDateFormat;
            myPane.XAxis.Scale.MinAuto = false;
            myPane.XAxis.Scale.MaxAuto = false;
            myPane.YAxis.Scale.MinAuto = false;
            myPane.YAxis.Scale.MaxAuto = false;
            myPane.YAxis.Scale.Format = "f";

            myPane.Y2Axis.Scale.Align = AlignP.Inside;
            myPane.Y2Axis.MajorTic.IsOpposite = false;
            myPane.Y2Axis.MinorTic.IsOpposite = false;
            myPane.Y2Axis.Scale.MagAuto = false;
            myPane.Y2Axis.MajorGrid.IsVisible = true;
            myPane.Y2Axis.MajorGrid.Color = Color.LightGray;
            myPane.Y2Axis.MajorGrid.IsZeroLine = false;
            myPane.Y2Axis.Scale.MinAuto = true;
            myPane.Y2Axis.Scale.MaxAuto = true;

            myPane.X2Axis.MajorTic.IsOpposite = false;
            myPane.X2Axis.MinorTic.IsOpposite = false;
            myPane.X2Axis.Scale.MagAuto = false;
            myPane.X2Axis.MajorGrid.IsVisible = true;
            myPane.X2Axis.MajorGrid.Color = Color.LightGray;
            myPane.X2Axis.MajorGrid.IsZeroLine = false;
            myPane.X2Axis.Scale.MinAuto = true;
            myPane.X2Axis.Scale.MaxAuto = true;

            //chartControl.IsShowVScrollBar = true;
            //chartControl.IsAutoScrollRange = true;
            //thePane.IsBoundedRanges = true;
            chartControl.IsShowHScrollBar = true;
            chartControl.ScrollGrace = 0.1;
        }

        private string chartControl_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair pt = curve.Points[iPt];
            DateTimeOffset time = new DateTimeOffset( DateTime.FromOADate(pt.X), TimeSpan.Zero);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} @ {1}", time.ToString("yyyy-MM-dd ddd"), pt.Y);

            Quote qt = History[time];
            if (qt != null)
            {
                sb.AppendFormat(" High={0}, Low={1}\r\n", qt.High, qt.Low);

                MatchRules during = new MatchRules(time, Mode);
                List<Relation> relations = Ephemeris.RelationsWithin(during);

                foreach (Relation rel in relations)
                {
                    if (rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                        continue;

                    sb.AppendFormat(", {0}{1}{2}(Orb={3})", 
                        Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior), rel.Orb.ToString("F3"));
                }
            }

            return sb.ToString();
        }


        private void chartControl_DoubleClick(object sender, EventArgs e)
        {
            // Determine object state
            Point mousePt = chartControl.PointToClient(Control.MousePosition);
            int iPt;
            GraphPane pane;
            object nearestObj;

            using (Graphics g = chartControl.CreateGraphics())
            {
                if (chartControl.MasterPane.FindNearestPaneObject(mousePt, g, out pane, out nearestObj, out iPt) && nearestObj is CurveItem && iPt >= 0)
                {
                    CurveItem curve = (nearestObj) as CurveItem;
                    PointPair pt = curve[iPt];

                    if (pt != null)
                    {
                        DateTime date = DateTime.FromOADate(pt.X);
                        calendar.Value = date;
                    }
                }
            }

            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
        }

        private List<CurveItem> curveInGraphPane(GraphPane pane, string label)
        {
            List<CurveItem> curves = new List<CurveItem>();
  
            foreach (CurveItem curve in pane.CurveList)
            {
                if (curve.Label.Text.ToLower().StartsWith(label.ToLower()) )
                    curves.Add(curve);
            }
            return curves;
        }

        private void clearCurveOf(ZedGraphControl zed, string label)
        {
            GraphPane pane = zed.GraphPane;
            List<CurveItem> curves = curveInGraphPane(pane, label);
            foreach (CurveItem curve in curves)
            {
                pane.CurveList.Remove(curve);
            }

            zed.Invalidate();
        }

        // Build the Shape
        private void displayKLine()
        {
            // get a reference destination the GraphPane
            GraphPane myPane = chartControl.GraphPane;

            myPane.CurveList.Clear();

            StockPointList spl = new StockPointList();

            for (int i = 0; i < ActivedQuotes.Count; i++)
            {
                Quote item = ActivedQuotes[i];
                double date = Active.Dates[i];
                StockPt pt = new StockPt(date, item.High, item.Low, item.Open, item.Close, 0);
                spl.Add(pt);
            }

            JapaneseCandleStickItem kLine = myPane.AddJapaneseCandleStick(this.Text, spl);
            kLine.Stick.IsAutoSize = true;
            kLine.Stick.Color = Color.Blue;
            
            PointPairList list = new PointPairList();

            setAxisX(chartControl, new XDate(spl.First().X), new XDate(spl.Last().X));

            setAxisY(chartControl, History.Ceiling, History.Floor);

            // Tell ZedGraph destination refigure the
            // axes around the data have changed
            chartControl.AxisChange();

            chartControl.Invalidate();
        }

        private void displayOutline()
        {
            clearCurveOf(chartControl, "Outline");

            if (Threshold != 0)
            {
                LineItem trendLine = chartControl.GraphPane.AddCurve("Outline", CurrentOutline.Dates.ToArray(), CurrentOutline.Values.ToArray(), 
                    Color.CadetBlue, SymbolType.Circle);
                //trendLine.Line.IsVisible = true;
                trendLine.Symbol.Fill = new Fill(Color.CadetBlue);
                trendLine.Symbol.Size = 3;
                trendLine.Symbol.Border.IsVisible = false;
            }

            chartControl.Invalidate();
        }

        private GetTrendDelegate getTrend = null;

        private void comboTrending_SelectedIndexChanged(object sender, EventArgs e)
        {
            getTrend = GannTrend.Methods[comboTrending.SelectedItem.ToString()];

            clearCurveOf(chartControl, "Trend");
            //chartControl.GraphPane.Legend.IsVisible = true;

            GannTrend trend = null;
            Outline outline = CurrentOutline;
            List<List<Double>> coordinates = null;
            List<List<Double>> all = null;

            if (getTrend != null && outline != null && Threshold >= 5)
            {
                OutlineItem start = null, end = null;
                Double cornerX = History.Until.UtcDateTime.ToOADate() + 30, cornerY;

                Double mag = Math.Floor(Math.Log10(History.Ceiling)) - 1;
                Double step = Math.Pow(10, mag);
                Double ceiling = (Math.Ceiling(History.Ceiling / step) + 2) * step;
                Double floor = (Math.Max(0, Math.Floor(History.Floor / step) - 2) * step);



                start = outline.Pivots[0];
                end = outline.Pivots[1];
                trend = getTrend(start, end);
                cornerY = trend.Rate > 0 ? ceiling : floor;

                all = trend.CoordinatesWithin(cornerX,cornerY);

                for (int i = 1; i < outline.Pivots.Count - 1; i ++ )
                {
                    start = outline.Pivots[i];
                    end = outline.Pivots[i + 1];
                    trend = getTrend(start, end);
                    cornerY = trend.Rate > 0 ? ceiling : floor;

                    coordinates = trend.CoordinatesWithin(cornerX, cornerY);

                    if (all[0].Last() != coordinates[0][0])
                    {
                        all[0].Add(cornerX);
                        all[1].Add(all[1].Last());
                    }

                    if (coordinates[0][0] != cornerX)
                    {
                        all[0].Add(cornerX);
                        all[1].Add(coordinates[1].Last());
                    }

                    all[0].AddRange(coordinates[0]);
                    all[1].AddRange(coordinates[1]);
                }

                LineItem trendLine = chartControl.GraphPane.AddCurve("Trend", all[0].ToArray(), all[1].ToArray(), Color.DarkGreen, SymbolType.None);

            }
            chartControl.Invalidate();
        }

        private bool zedGraphControl1_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            PointF moustPt = new PointF(e.X, e.Y);

            GraphPane pane = sender.MasterPane.FindChartRect(moustPt);

            if (pane != null)
            {
                double x, y;

                pane.ReverseTransform(moustPt, out x, out y);

                XDate date = new XDate(x);

                //DateTimeOffset dt = new DateTimeOffset(around.DateTime, TimeSpan.Zero);
                DateTimeOffset dt = Active.NearestOf(x);

                Quote quote = Active[dt];

                if (quote != null)
                    statusLable.Text = quote.ToString(Active.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
            }

            return false;
        }

        private void toolStripButtonUp_Click(object sender, EventArgs e)
        {
            QuoteType++;
            Display();
        }

        private void toolStripButtonDown_Click(object sender, EventArgs e)
        {
            QuoteType--;
            Display();
        }

        private void zedGraphControl1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            setRange(sender);
        }

        private void setRange(ZedGraphControl sender)
        {
            GraphPane myPane = sender.GraphPane;

            Scale xScale = myPane.XAxis.Scale;

            XDate startDate = new XDate(xScale.Min);
            XDate endDate = new XDate(xScale.Max);

            if (startDate.DateTime > History.Until.AddDays(30) || endDate.DateTime < History.Since.AddDays(-30))
                return;

            setAxisX(sender, startDate, endDate);

            double ceiling, bottom;

            if(Active.GetRangeOf( myPane.XAxis.Scale.Min, myPane.XAxis.Scale.Max, out ceiling, out bottom))
            {
                Double mag = Math.Floor(Math.Log10(ceiling)) - 1;
                Double step = Math.Pow(10, mag);
                ceiling = Math.Ceiling(ceiling / step) * step;
                bottom = Math.Max(0, Math.Floor(bottom / step) * step);

                setAxisY(sender, ceiling, bottom);
            }
        }


        private void buttonKLine_Click(object sender, EventArgs e)
        {
            if (chartControl.GraphPane.CurveList[0].IsVisible)
            {
                chartControl.GraphPane.CurveList[0].IsVisible = false;
                buttonKLine.Text = "K On";
            }
            else
            {
                chartControl.GraphPane.CurveList[0].IsVisible = true;
                buttonKLine.Text = "K Off";
            }
            chartControl.Invalidate();
        }


        private void buttonSaveTypical_Click(object sender, EventArgs e)
        {
            //List<Double> result = new List<Double>();
            //List<Double> dates = History.Dates;
            //List<Double> prices = History.OutlineValues;

            //Double lastDateValue = dates[0]-1, nowDateValue, lastPrice = 0, curPrice, interval, step;

            //for (int i = 0; i < History.Count; i ++ )
            //{
            //    nowDateValue = dates[i];
            //    curPrice = prices[i];
            //    interval =nowDateValue - lastDateValue;
            //    if (interval == 1)
            //    {
            //        result.Add(curPrice);
            //    }
            //    else
            //    {
            //        step = (lastPrice - curPrice) / interval;

            //        for (int times = 0; times < interval; times ++ )
            //        {
            //            lastPrice -= step;
            //            result.Add(Math.Round(lastPrice, 2));
            //        };
            //    }
            //    lastDateValue = nowDateValue;
            //    lastPrice = curPrice;
            //    if (lastDateValue - dates[0] != result.Count - 1)
            //    {
            //        ;
            //    }
            //}

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Typical quote as text|*.txt";
            saveDialog.FileName = "+" + History.Name + ".txt";
            saveDialog.Title = "Save the typical Quote as in txt format...";
            saveDialog.Filter = "Text File(*.txt)|*.txt";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);
                sw.AutoFlush = true;

                for (int i = 0; i < History.Count; i ++)
                {
                    Double price = History.OutlineValues[i];
                    sw.WriteLine(price.ToString());
                }
                stream.Close();
            }
        }

        private void showSpectrum(SortedDictionary<Double, Double> spectrum, string name, Color color)
        {
            CurveList curves = chartControl.GraphPane.CurveList;
            curves.RemoveAll(curve => curve.IsX2Axis == true);

            if(spectrum != null)
            {
                List<Double> xList = spectrum.Keys.ToList();
                List<Double> yList = new List<Double>();
                List<String> lables = new List<String>();

                foreach (Double x in xList)
                {
                    yList.Add(spectrum[x]);
                    lables.Add(x.ToString());
                }

                CurveItem bar = chartControl.GraphPane.AddCurve(name, xList.ToArray(), yList.ToArray(), color, SymbolType.Circle);

                bar.IsX2Axis = true;
                bar.IsY2Axis = true;
                BarItem.CreateBarLabels(chartControl.GraphPane, false, "f0");

                chartControl.GraphPane.X2Axis.Type = AxisType.Linear;
                //chartControl.GraphPane.X2Axis.Scale.TextLabels = lables.ToArray();
                chartControl.GraphPane.X2Axis.Scale.Min = xList.Min() - 1;
                chartControl.GraphPane.X2Axis.Scale.Max = xList.Max() + 1;
                chartControl.GraphPane.Y2Axis.Scale.Min = yList.Min() - 1;
                chartControl.GraphPane.Y2Axis.Scale.Max = yList.Max() + 1;

                chartControl.GraphPane.X2Axis.IsVisible = true;
                chartControl.GraphPane.Y2Axis.IsVisible = true;

                chartControl.GraphPane.X2Axis.MajorTic.IsOpposite = false;
                chartControl.GraphPane.X2Axis.MinorTic.IsOpposite = false;
                chartControl.GraphPane.Y2Axis.MajorTic.IsOpposite = false;
                chartControl.GraphPane.Y2Axis.MinorTic.IsOpposite = false;
                bar.IsVisible = true;
            }
            else
            {
                chartControl.GraphPane.X2Axis.IsVisible = false;
                chartControl.GraphPane.Y2Axis.IsVisible = false;
                chartControl.GraphPane.XAxis.MajorTic.IsOpposite = true;
                chartControl.GraphPane.XAxis.MinorTic.IsOpposite = true;
                chartControl.GraphPane.YAxis.MajorTic.IsOpposite = true;
                chartControl.GraphPane.YAxis.MinorTic.IsOpposite = true;

            }

            chartControl.GraphPane.AxisChange();
            chartControl.Invalidate();
        }

        private void buttonSpectrum_Click(object sender, EventArgs e)
        {
            wave = new Vibration(CurrentOutline!=null ? CurrentOutline : History.ShortTermOutline, ReferenceType.Nadia);


            switch(buttonSpectrum.Text)
            {
                case "TopsSpectrum":
                    buttonSpectrum.Text = "BottomsSpectrum";
                    showSpectrum(wave.TopsSpectrum, "Tops", Color.Red);
                    break;
                case "BottomsSpectrum":
                    buttonSpectrum.Text = "SpectrumAll";
                    showSpectrum(wave.BottomsSpectrum, "Bottoms", Color.Green);
                    break;
                case "SpectrumAll":
                    buttonSpectrum.Text = "SpectrumOff";
                    showSpectrum(wave.Spectrum, "All", Color.Blue);
                    break;
                case "SpectrumOff":
                    buttonSpectrum.Text = "TopsSpectrum";
                    showSpectrum(null, "Off", Color.Blue);
                    break;
                default:
                    break;
            }
        }

        private void setAxisX(ZedGraphControl sender, XDate startX, XDate endX)
        {
            GraphPane myPane = sender.GraphPane;

            double minorStep;
            DateTime beforeDate = startX.DateTime;
            DateTime endDate = endX.DateTime;

            switch(QuoteType)
            {
                case RecordType.YearRecord:
                    minorStep = 365;
                    beforeDate = new DateTime(beforeDate.Year-1, 12, 31);
                    endDate = new DateTime(endDate.Year, 12, 31);
                    break;
                case RecordType.QuarterRecord:
                    minorStep = 365.0 / 4;
                    beforeDate = new DateTime(beforeDate.Year, (beforeDate.Month == 12) ? 10 : (beforeDate.Month / 3) * 3 + 1, 1);
                    endDate += TimeSpan.FromDays(minorStep);
                    endDate = new DateTime(endDate.Year, endDate.Month == 12 ? 10 : (endDate.Month / 3) * 3 + 1, 1).AddDays(-1);
                    break;
                case RecordType.MonthRecord:
                    minorStep = 30;
                    beforeDate = new DateTime(beforeDate.Year, beforeDate.Month, 1);
                    endDate = new DateTime(endDate.Year, endDate.Month, 1).AddDays(31);
                    endDate = new DateTime(endDate.Year, endDate.Month, 1).AddDays(-1);
                    break;
                case RecordType.WeekRecord:
                    minorStep = 7;
                    beforeDate = new DateTime(beforeDate.Year, beforeDate.Month, beforeDate.Day).AddDays( 0 - (int)beforeDate.DayOfWeek);
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day).AddDays(8 - (int)endDate.DayOfWeek);
                    break;
                case RecordType.DayRecord:
                    minorStep = 1.0;
                    beforeDate = new DateTime(beforeDate.Year, beforeDate.Month, beforeDate.Day);
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day).AddDays(1);
                    break;
                case RecordType.TwoHourRecord:
                case RecordType.HourRecord:
                    minorStep = 1/ 24.0;
                    beforeDate = new DateTime(beforeDate.Year, beforeDate.Month, beforeDate.Day, beforeDate.Hour, 0, 0);
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, 0, 0).AddHours(1);
                    break;
                default:
                    throw new NotImplementedException();
            }

            myPane.XAxis.Scale.Min = XDate.DateTimeToXLDate(beforeDate);
            if (QuoteType <= RecordType.DayRecord)
                myPane.XAxis.Scale.Min -= minorStep / 2;

            myPane.XAxis.Scale.Max = XDate.DateTimeToXLDate(endDate) + minorStep/2 ;

        }

        private void setAxisY(ZedGraphControl sender, double ceiling, double bottom)
        {
            GraphPane myPane = sender.GraphPane;

            double minorStep = myPane.YAxis.Scale.MinorStep;

            myPane.YAxis.Scale.Min = Math.Truncate( bottom/minorStep) * minorStep;
            myPane.YAxis.Scale.Max = Math.Truncate((ceiling + minorStep) / minorStep) * minorStep;
        }

        private void chartControl_ScrollDoneEvent(ZedGraphControl sender, ScrollBar scrollBar, ZoomState oldState, ZoomState newState)
        {
            setRange(sender);

        }

        #endregion

        #region Cycles related functions

        private void initializeCycleToolItems()
        {
            #region Frame preparation

            foreach (KeyValuePair<String, AngleFrame> kvp in AngleFrame.AllPresenters)
            {
                comboFrame.Items.Add(kvp.Key);
            }
            comboFrame.SelectedIndexChanged += new EventHandler(comboPresenter_SelectedIndexChanged);
            comboFrame.SelectedIndex = 0;

            #endregion

            #region Price preparation

            foreach (KeyValuePair<String, PriceAdapter> kvp in PriceAdapter.All)
            {
                comboPrice.Items.Add(kvp.Key);
            }

            foreach (KeyValuePair<String, PriceAdapter> kvp in helper.PriceDict)
            {
                comboPrice.Items.Add(kvp.Key);
            }
            comboPrice.SelectedIndexChanged += new EventHandler(comboPrice_SelectedIndexChanged);
            comboPrice.SelectedIndex = 0;

            #endregion

            #region Clock preparation

            foreach (KeyValuePair<String, TimeAngleConverter> kvp in TimeAngleConverter.All)
            {
                comboTime.Items.Add(kvp.Key);
            }

            foreach (KeyValuePair<String, TimeAngleConverter> kvp in helper.ClocksDict)
            {
                comboTime.Items.Add(kvp.Key);
            }
            
            comboTime.SelectedIndexChanged += new EventHandler(comboTime_SelectedIndexChanged);
            comboTime.SelectedIndex = 0;

            #endregion

            #region Planets CheckList preparation
            List<string> stars = new List<string>();
            for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
            {
                //planetsCheck.Items.Add(id, false);
                stars.Add(id.ToString());
            }
            stars.Add(PlanetId.SE_CHIRON.ToString());

            planetsCheck.Items.AddRange(stars.ToArray());
            planetsCheck.Text = "Select planets ...";
            planetsCheck.MaximumSize = new System.Drawing.Size(200, 30);
            planetsCheck.CheckOnClick = true;
            planetsCheck.ItemCheck += new ItemCheckEventHandler(planetsCheck_ItemCheck);
            //planetsCheck.Leave += new EventHandler(planetsCheck_Leave);
            planetsHost = new ToolStripControlHost(planetsCheck);
            planetsHost.Size = new System.Drawing.Size(200, 30);

            for (OrbitInfoType kind = OrbitInfoType.Longitude; kind <= OrbitInfoType.ApogeeVelocities; kind++)
            {
                orbitsCheck.Items.Add(kind, false);
            }
            orbitsCheck.Text = "Select orbits type ...";
            orbitsCheck.MaximumSize = new Size(200, 30);
            orbitsCheck.CheckOnClick = true;
            orbitsCheck.ItemCheck += new ItemCheckEventHandler(orbitsCheck_ItemCheck);
            orbitsHost = new ToolStripControlHost(orbitsCheck);
            orbitsHost.Size = new Size(200, 30);

            #endregion

            #region Plus and Minus preparation

            for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
            {
                plusCheck.Items.Add(id.ToString(), false);
                minusCheck.Items.Add(id.ToString(), false);
            }
            plusCheck.Items.Add(PlanetId.SE_CHIRON.ToString(), false);
            minusCheck.Items.Add(PlanetId.SE_CHIRON.ToString(), false);
            
            plusCheck.MaximumSize = new System.Drawing.Size(200, 30);
            plusCheck.CheckOnClick = true;
            //plusCheck.Leave += new EventHandler(plusCheck_Leave);
            plusCheck.ItemCheck += new ItemCheckEventHandler(plusCheck_ItemCheck);
            //plusCheck.LostFocus += new EventHandler(plusCheck_LostFocus);
            plusHost = new ToolStripControlHost(plusCheck);
            plusHost.Size = new System.Drawing.Size(200, 30);

            minusCheck.MaximumSize = new Size(200, 30);
            minusCheck.CheckOnClick = true;
            //minusCheck.Leave += new EventHandler(minusCheck_Leave);
            minusCheck.ItemCheck += new ItemCheckEventHandler(minusCheck_ItemCheck);
            //minusCheck.LostFocus += new EventHandler(minusCheck_LostFocus);
            minusHost = new ToolStripControlHost(minusCheck);
            minusHost.Size = new Size(200, 30);

            textShift.Size = new System.Drawing.Size(40, 21);
            textShift.Leave += new EventHandler(textShift_Leave);

            #endregion

            cycleItems = new ToolStripItem[] { comboEphemeris, comboFrame, 
                toolStripSeparator3, thresholdHost, comboPrice, comboPolygons, toolStripSeparator1, comboTime, 
                toolStripSeparator2, planetsHost, orbitsHost, buttonAll, toolStripSeparator4, 
                textShift, toolStripLabelPlus, plusHost, toolStripLabelSubtract, minusHost, buttonClear };

        }

        private string cycleControl_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            StringBuilder sb = new StringBuilder();
            PointPair pt = curve.Points[iPt];
            DateTimeOffset time = new DateTimeOffset(DateTime.FromOADate(pt.X), TimeSpan.Zero);
            MatchRules during = new MatchRules(time, Mode);
            List<Relation> relations = null;

            string label = curve.Label.Text;

            PlanetId id = (label[0] == 'A') ? PlanetId.SE_SUN : CyclesPresenter.OwnerOf(curve);

            Double degree;
            
            if (id < PlanetId.SE_SUN || id > PlanetId.SE_CHIRON)
                degree = pt.Y;
            else
            {
                Position pos = Ephemeris.CurrentEphemeris[time, id];
                OrbitInfoType kind = CyclesPresenter.KindOf(curve);

                switch (kind)
                {
                    case OrbitInfoType.Longitude:
                        degree = pos.Longitude;
                        break;
                    case OrbitInfoType.Latitude:
                        degree = pos.Latitude;
                        break;
                    case OrbitInfoType.Distance:
                        degree = pos.Distance;
                        break;
                    case OrbitInfoType.LongitudeVelocities:
                        degree = pos.LongitudeVelocity;
                        break;
                    case OrbitInfoType.LatitudeVelocities:
                        degree = pos.LatitudeVelocity;
                        break;
                    case OrbitInfoType.DistanceVelocities:
                        degree = pos.DistanceVelocity;
                        break;
                    default:
                        degree = pt.Y;
                        break;
                }
            }

            sb.AppendFormat("{0}: {1} @ {2}({3})", label, time.UtcDateTime.ToString("yyyy-MM-dd ddd"),
                Angle.AngleFormatOf(degree, ""), Rectascension.AngleFormatOf(degree, "Astro"));

            switch (label[0])
            {
                case '$':
                    Quote qt = History[time];
                    if (qt != null)
                    {
                        sb.AppendFormat(" High={0}, Low={1}\r\n", qt.High, qt.Low);

                        relations = Ephemeris.RelationsWithin(during);

                        foreach (Relation rel in relations)
                        {
                            if (rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                                continue;

                            sb.AppendFormat(", {0}{1}{2}", Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior));
                        }

                        PlanetEvents posAttr = null;
                        for (PlanetId star = PlanetId.SE_SUN; star <= PlanetId.SE_PLUTO; star++)
                        {
                            posAttr = new PlanetEvents(Ephemeris.CurrentEphemeris, time, star);

                            if (posAttr.LongitudeStatus == RectascensionMode.None && posAttr.LatitudeStatus == DeclinationMode.None
                                && posAttr.DistanceStatus == DistanceMode.None)
                                continue;
                            else
                                sb.AppendFormat("\r\n{0}: ", Planet.SymbolOf(star));

                            if (posAttr.LongitudeStatus != RectascensionMode.None)
                            {
                                sb.AppendFormat("{0}({1}days). ", posAttr.LongitudeStatus, posAttr.DaysToRectascensionMode);
                            }

                            if (posAttr.LatitudeStatus != DeclinationMode.None)
                            {
                                sb.AppendFormat("{0}({1}days). ", posAttr.LatitudeStatus, posAttr.DaysToDeclinationMode);
                            }

                            if (posAttr.DistanceStatus != DistanceMode.None)
                            {
                                sb.AppendFormat("{0}({1}days)", posAttr.DistanceStatus, posAttr.DaysToDistanceMode);
                            }
                        }

                    }
                    break;
                case '+':
                case '*':
                    break;
                case 'A':
                default:
                    relations = Ephemeris.RelationsWithin(during, id);

                    foreach (Relation rel in relations)
                    {
                        if (rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                            continue;

                        sb.AppendFormat(", {0}{1}{2}(Orb={3:F2})", Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol,
                            Planet.SymbolOf(rel.Inferior), rel.Orb);
                    }

                    PlanetEvents detail = new PlanetEvents(Ephemeris.CurrentEphemeris, time, id);

                    sb.AppendFormat("\r\n{0}", detail);

                    break;

            }

            return sb.ToString();
        }

        private void cycleControl_DoubleClick(object sender, EventArgs e)
        {
            // Determine object state
            Point mousePt = cycleControl.PointToClient(Control.MousePosition);
            int iPt;
            GraphPane pane;
            object nearestObj;

            using (Graphics g = cycleControl.CreateGraphics())
            {
                if (cycleControl.MasterPane.FindNearestPaneObject(mousePt, g, out pane, out nearestObj, out iPt) && nearestObj is CurveItem && iPt >= 0)
                {
                    CurveItem curve = (nearestObj) as CurveItem;
                    PointPair pt = curve[iPt];

                    if (pt != null)
                    {
                        DateTime date = DateTime.FromOADate(pt.X);
                        calendar.Value = date;
                    }
                }
            }

            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;

        }

        #region Functions for combined analysis

        //private SpecialAngle shiftFromString(string s)
        //{
        //    Double degrees;

        //    if (Double.TryParse(s, out degrees) && SpecialAngle.Famous.ContainsKey(degrees))
        //    {
        //        return SpecialAngle.Famous[degrees];
        //    }
        //    else
        //        throw new Exception();
        //}


        void textShift_Leave(object sender, EventArgs e)
        {
            if (helper != null)
            {
                double shift;

                if (!double.TryParse(textShift.Text, out shift))
                {
                    //plusCheck.Enabled = false;
                    //minusCheck.Enabled = false;
                    helper.Shift = null;
                }
                else
                {
                    helper.Shift = new Angle(shift);
                    //plusCheck.Enabled = true;
                    //minusCheck.Enabled = true;
                }

                helper.AddCurve("+");
                helper.AddCurve("*");
                AngleFrame_TransitionsChanged();
            }
            
        }

        void minusCheck_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string starName = minusCheck.Items[e.Index].ToString();

            OrbitSpec orbit = null;
            PlanetId id;

            if (!Planet.TryParseId(starName, out id))
                throw new Exception("PlanetId is not recognized!");

            if (e.NewValue == CheckState.Checked)
            {
                foreach (OrbitSpec ob in helper.MinusOrbits)
                {
                    if (ob.Id == id)
                        throw new Exception(id.ToString() + " is already minus.");
                }

                orbit = helper.Orbits[id, OrbitInfoType.Longitude];
                if (orbit != null)
                    helper.MinusOrbits.Add(orbit);

                removePlanet(plusCheck, id);
            }
            else
            {
                int pos = -1;
                for (int i = 0; i < helper.MinusOrbits.Count; i++)
                {
                    if (helper.MinusOrbits[i].Id != id)
                        continue;

                    pos = i;
                    break;
                }

                if (pos == -1)
                    throw new Exception(id.ToString() + " is not added to the MinusOrbits.");
                helper.MinusOrbits.RemoveAt(pos);

                addPlanet(plusCheck, id);
            }

            helper.AddCurve("*");
            AngleFrame_TransitionsChanged();
        }

        void plusCheck_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string starName = plusCheck.Items[e.Index].ToString();

            OrbitSpec orbit = null;
            PlanetId id;

            if (!Planet.TryParseId(starName, out id))
                throw new Exception("PlanetId is not recognized!");

            if (e.NewValue == CheckState.Checked)
            {
                foreach(OrbitSpec ob in helper.PlusOrbits)
                {
                    if (ob.Id == id)
                        throw new Exception(id.ToString() + " is already added.");
                }

                orbit = helper.Orbits[id, OrbitInfoType.Longitude];
                if (orbit != null)
                    helper.PlusOrbits.Add(orbit);

                removePlanet(minusCheck, id);
            }
            else
            {
                int pos = -1;
                for (int i = 0; i < helper.PlusOrbits.Count; i++)
                {
                    if (helper.PlusOrbits[i].Id != id)
                        continue;

                    pos = i;
                    break;
                }

                if (pos == -1)
                    throw new Exception(id.ToString() + " is not added to the PlusOrbits.");
                helper.PlusOrbits.RemoveAt(pos);

                addPlanet(minusCheck, id);
            }

            helper.AddCurve("*");
            AngleFrame_TransitionsChanged();
        }

        void addPlanet(CheckedListBox box, PlanetId idAdd)
        {
            string starName = idAdd.ToString();

            if (box.Items.Contains(starName))
                throw new Exception();

            int insertPos = 0;

            for (PlanetId id = idAdd-1; id >= PlanetId.SE_MOON; id--)
            {
                string name = id.ToString();
                if (!box.Items.Contains(name))
                    continue;

                insertPos = box.Items.IndexOf(name) + 1;
                break;
            }

            box.Items.Insert(insertPos, starName);
        }

        void removePlanet(CheckedListBox box, PlanetId id)
        {
            string starName = id.ToString();

            if (box.Items.Contains(starName))
            {
                box.Items.Remove(starName);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            foreach (int index in plusCheck.CheckedIndices)
            {
                plusCheck.SetItemChecked(index, false);
            }

            foreach (int index in minusCheck.CheckedIndices)
            {
                minusCheck.SetItemChecked(index, false);
            }

            helper.PlusOrbits.Clear();
            helper.MinusOrbits.Clear();
            helper.AddCurve("*");

            AngleFrame.Transitions.Clear();
            AngleFrame_TransitionsChanged();
        }

        #endregion

        #region Orbits related functions

        void planetsCheck_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            String planetIdString = planetsCheck.Items[e.Index].ToString();
            planetsCheck.Text = planetIdString;
            PlanetId id = (PlanetId)Enum.Parse(typeof(PlanetId), planetIdString);

            if (e.NewValue == CheckState.Checked)
            {
                int top = orbitsCheck.TopIndex;

                for (int i = 0; i < 6; i ++ )
                {
                    if (!orbitsCheck.GetItemChecked(i))
                        orbitsCheck.SetItemChecked(i, true);
                }

                orbitsCheck.TopIndex = top;
                //if (!helper.IsCurveAdded(id, OrbitInfoType.Longitude))
                //{
                //    helper.AddCurve(id, OrbitInfoType.Longitude);
                //    orbitsCheck.Tag = OrbitInfoType.Longitude;
                //    planetsCheck.Tag = id;
                //    refreshOrbitsStatus(id);
                //}
            }
            else
            {
                int top = orbitsCheck.TopIndex;

                for (int i = 0; i < orbitsCheck.Items.Count; i++)
                {
                    if (orbitsCheck.GetItemChecked(i))
                        orbitsCheck.SetItemChecked(i, false);
                }

                orbitsCheck.TopIndex = top;
                //if (helper.IsCurveAdded(id, OrbitInfoType.Longitude))
                //{
                //    helper.RemoveCurveOf(id, OrbitInfoType.Longitude);
                //    orbitsCheck.Tag = OrbitInfoType.Longitude;
                //    planetsCheck.Tag = id;
                //    refreshOrbitsStatus(id);
                //}
                
            }
        }

        void orbitsCheck_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string planetName = planetsCheck.Items[planetsCheck.TopIndex].ToString();

            int index = e.Index;
            OrbitInfoType kind = (OrbitInfoType)Enum.Parse(typeof(OrbitInfoType), orbitsCheck.Items[index].ToString());
            PlanetId id;

            if (!Planet.TryParseId(planetName, out id))
                throw new Exception();

            if (e.NewValue == CheckState.Checked && !helper.IsCurveAdded(id, kind))
            {
                helper.AddCurve(id, kind);
            }
            else if (e.NewValue == CheckState.Unchecked && helper.IsCurveAdded(id, kind))
            {
                helper.RemoveCurveOf(id, kind);
                //refreshPlanetStatus(id);
            }
        }

        void orbitsCheck_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = orbitsCheck.SelectedIndex;
            string curItem = orbitsCheck.Items[index].ToString();
            bool isChecked = orbitsCheck.GetItemChecked(index);
            orbitsCheck.SetItemChecked(index, !isChecked);
        }

        private void refreshPlanetStatus(PlanetId id)
        {
            planetsCheck.Text = id.ToString();

            if (helper.IsCurveAdded(id, OrbitInfoType.Longitude))
                planetsCheck.SetItemChecked(planetsCheck.SelectedIndex, true);
            else
                planetsCheck.SetItemChecked(planetsCheck.SelectedIndex, false);
        }

        private void refreshOrbitsStatus(PlanetId id)
        {
            orbitsCheck.Text = orbitsCheck.Tag.ToString();
            for (int i = 0; i < orbitsCheck.Items.Count; i++)
            {
                String orbitTypeStr = orbitsCheck.Items[i].ToString();
                OrbitInfoType kind = (OrbitInfoType)Enum.Parse(typeof(OrbitInfoType), orbitTypeStr);
                if (helper.IsCurveAdded(id, kind))
                    orbitsCheck.SetItemChecked(i, true);
                else
                    orbitsCheck.SetItemChecked(i, false);
            }
        }

        #endregion

        void comboPresenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (helper != null)
            {
                string name = comboFrame.SelectedItem.ToString();

                helper.Frame = AngleFrame.AllPresenters[name];

                helper.RedrawYValues();
            }
        }

        void AngleFrame_TransitionsChanged()
        {
            CurveItem toBeDropped = null;

            foreach (CurveItem curve in chartControl.GraphPane.CurveList)
            {
                if (curve.Label.Text != "Transition")
                    continue;

                toBeDropped = curve;
                break;
            }

            if (toBeDropped != null)
            {
                chartControl.GraphPane.CurveList.Remove(toBeDropped);
                Console.WriteLine("One transition curve is removed.");
            }

            if (AngleFrame.Transitions.Count == 0)
            {
                polygonControl1.QuoteRangs.Clear();
                return;
            }

            double max = History.Ceiling * 1.2;
            double[] yValues =
                (from date in AngleFrame.Transitions
                 select max).ToArray();

            if (yValues != null && yValues.Length != 0)
            {
                StickItem breaks = chartControl.GraphPane.AddStick("Transition", AngleFrame.Transitions.ToArray(), yValues, Color.Gray);

                Console.WriteLine("StickItem is added.");

                polygonControl1.GetPathes(AngleFrame.Transitions);

                Console.WriteLine("Quote Pathes are inserted.");
            }

        }

        void comboTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (helper != null)
            {
                string name = (comboTime.SelectedItem== null) ? comboTime.Items[0].ToString() : comboTime.SelectedItem.ToString();
                if (TimeAngleConverter.All.ContainsKey(name))
                    helper.Clock = TimeAngleConverter.All[name];
                else if (helper.ClocksDict != null && helper.ClocksDict.ContainsKey(name))
                    helper.Clock = helper.ClocksDict[name];
                else
                    throw new Exception();

                helper.AddCurve("A");
            }
        }

        void comboPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (helper != null)
            {
                string name = (comboPrice.SelectedItem == null) ? comboPrice.Items[0].ToString() : comboPrice.SelectedItem.ToString();
                if (PriceAdapter.All.ContainsKey(name))
                    helper.PriceTranslator = PriceAdapter.All[name];
                else if (helper.PriceDict != null && helper.PriceDict.ContainsKey(name))
                    helper.PriceTranslator = helper.PriceDict[name];
                else
                    throw new Exception();

                helper.AddCurve("$");
            }
        }


        private void cycleControl_ScrollDoneEvent(ZedGraphControl sender, ScrollBar scrollBar, ZoomState oldState, ZoomState newState)
        {
            if (helper.IsCurveAdded("$") && (helper.PriceTranslator.Rule == PriceMappingRules.Filled || helper.PriceTranslator.Rule == PriceMappingRules.FilledPivots))
            {
                fillPrice(sender);
            }

        }

        private void cycleControl_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            if (helper.IsCurveAdded("$") && (helper.PriceTranslator.Rule == PriceMappingRules.Filled || helper.PriceTranslator.Rule == PriceMappingRules.FilledPivots))
            {
                fillPrice(sender);
            }
        }

        private void fillPrice(ZedGraphControl sender)
        {
            GraphPane myPane = sender.GraphPane;

            Scale xScale = myPane.XAxis.Scale;
            xScale.Min = Math.Floor(xScale.Min);
            xScale.Max = Math.Ceiling(xScale.Max);

            XDate startDate = new XDate(xScale.Min);
            XDate endDate = new XDate(xScale.Max);

            int startIndex, endIndex;

            startIndex = (from date in History.Dates
                          where date >= xScale.Min
                          select History.Dates.IndexOf(date)).FirstOrDefault();

            endIndex = (from date in History.Dates
                        where date <= xScale.Max
                        select History.Dates.IndexOf(date)).LastOrDefault();

            List<double> dates = new List<double>();
            List<Double> prices = new List<Double>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                dates.Add(History.Dates[i]);
                prices.Add(History.OutlineValues[i]);
            }

            PriceAdapter newAdapter = new PriceAdapter(PriceMappingRules.Filled, prices.Min(), prices.Max());
            //LineItem priceLine = helper.Frame.CurveOf(newAdapter, dates, prices, helper.CycleMapper, true);

            LineItem priceLine;

            if (helper.PriceTranslator.Rule == PriceMappingRules.Filled)
                priceLine = helper.Frame.CurveOf(newAdapter, History.Dates, History.OutlineValues, helper.CycleMapper, true);
            else
                priceLine = helper.Frame.CurveOf(newAdapter, History.CurrentOutline.PivotDates, History.CurrentOutline.PivotValues, helper.CycleMapper, true);

            if (priceLine != null)
            {
                helper.RemoveCurveOf("$");
                helper.AddCurve(priceLine);
            }
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            if (buttonAll.Text.Contains("Off"))
            {
                buttonAll.Text = "AllOn";

                List<int> removed = new List<int>();
                for (int i = 0; i < helper.Zed.GraphPane.CurveList.Count; i ++)
                {
                    CurveItem curve = helper.Zed.GraphPane.CurveList[i];
                    PlanetId owner = CyclesPresenter.OwnerOf(curve);
                    if (owner <= PlanetId.SE_CHIRON && owner >= PlanetId.SE_MOON)
                        removed.Add(i);
                }

                for (int i = removed.Count - 1; i >= 0; i -- )
                {
                    helper.Zed.GraphPane.CurveList.RemoveAt(removed[i]);
                }

                orbitsCheck.ItemCheck -= new ItemCheckEventHandler(orbitsCheck_ItemCheck);

                for (int i = 0; i < planetsCheck.Items.Count; i ++ )
                {
                    planetsCheck.SetItemChecked(i, false);
                }

                for (int i = 0; i < orbitsCheck.Items.Count; i++)
                {
                    orbitsCheck.SetItemChecked(i, false);
                }                

                orbitsCheck.ItemCheck += new ItemCheckEventHandler(orbitsCheck_ItemCheck);

                helper.SetY2Scale(0);
                helper.SetY3Scale(0);

                helper.Zed.Invalidate();
            }
            else
            {
                buttonAll.Text = "AllOff";

                String orbitString = orbitsCheck.Items[orbitsCheck.TopIndex].ToString();
                OrbitInfoType kind = (OrbitInfoType)Enum.Parse(typeof(OrbitInfoType), orbitString);

                orbitsCheck.ItemCheck -= new ItemCheckEventHandler(orbitsCheck_ItemCheck);
                orbitsCheck.SetItemChecked(orbitsCheck.TopIndex, true);
                orbitsCheck.ItemCheck += new ItemCheckEventHandler(orbitsCheck_ItemCheck);

                planetsCheck.ItemCheck -= new ItemCheckEventHandler(planetsCheck_ItemCheck);
                for (int i = (int)PlanetId.SE_MERCURY; i < planetsCheck.Items.Count; i++)
                {
                    if (!planetsCheck.GetItemChecked(i))
                        planetsCheck.SetItemChecked(i, true);

                    String planetIdString = planetsCheck.Items[i].ToString();
                    PlanetId id = (PlanetId)Enum.Parse(typeof(PlanetId), planetIdString);

                    helper.AddCurve(id, kind);
                }
                planetsCheck.ItemCheck += new ItemCheckEventHandler(planetsCheck_ItemCheck);
            }
        }

        #endregion

        #region Geometrical functions

        private void initiatePolygonsPage()
        {
            foreach (KeyValuePair<String, Polygon> kvp in Polygon.All)
            {
                //if (kvp.Value.IsPolygon)
                    comboPolygons.Items.Add(kvp.Key);
            }
            comboPolygons.SelectedIndexChanged += new EventHandler(comboPolygons_SelectedIndexChanged);
            comboPolygons.SelectedIndex = 0;

            //comboStudyObjects.Size = new System.Drawing.Size(80, 21);
            //comboStudyObjects.Text = "Object";

            //comboStudyObjects.Items.Add("Off");
            //for (PlanetId id = PlanetId.SE_ECL_NUT; id <= PlanetId.SE_PLUTO; id++)
            //{
            //    comboStudyObjects.Items.Add(id.ToString());
            //}
            //comboStudyObjects.Items.Add(PlanetId.SE_NORTHNODE.ToString());
            //comboStudyObjects.SelectedIndex = 0;
            //comboStudyObjects.SelectedIndexChanged += new EventHandler(comboStudyObjects_SelectedIndexChanged);

            foreach (PolygonControl.FirstQuadrantOrientation orientation in Enum.GetValues(typeof(PolygonControl.FirstQuadrantOrientation)))
            {
                comboOrientation.Items.Add(orientation.ToString());
            }
            comboOrientation.SelectedIndex = 0;
            comboOrientation.SelectedIndexChanged += new EventHandler(comboOrientation_SelectedIndexChanged);

            comboOverlays.Items.Add("0");
            foreach (int i in PolygonControl.OverlayScheme.DefaultSetting.Keys)
            {
                comboOverlays.Items.Add(i.ToString());
            }
            comboOverlays.Items.Add("-1");
            comboOverlays.SelectedIndexChanged += new EventHandler(comboOverlays_SelectedIndexChanged);
            //comboOverlays.SelectedIndex = comboOverlays.Items.Count - 1;

            //textNeighbors.Text = PolygonControl.PivotSet.NeighborHood.ToString();

            polygonsItems = new ToolStripItem[] {calendarHost, toolStripButtonBack, toolStripButtonForward,
                toolStripButtonLast,toolStripButtonNext, toolStripSeparator1, thresholdHost, //comboStudyObjects,
                toolStripSeparator2, comboPolygons, toolStripSeparator3, comboOrientation,
                toolStripSeparator4, new ToolStripLabel("MaxCycles:"), textMaxCycle, 
                new ToolStripLabel("UnitSize:"), textUnitSize, 
                new ToolStripSeparator(), new ToolStripLabel("Overlay:"),comboOverlays, 
                new ToolStripSeparator(), new ToolStripLabel("StepSize:"), textStepSize
            };
            
            polygonControl1.Adapter = new PriceAdapter(PriceMappingRules.Natural, CurrentOutline);
            textStepSize.Text = polygonControl1.Adapter.Step.ToString();

            //polygonControl1.PivotsHolder.AddPivots(CurrentOutline);
            polygonControl1.HighlightChanged += new PolygonControl.PolygonControl.HighlightChangedDelegate(polygonControl1_HighlightChanged);
        }

        void polygonControl1_HighlightChanged()
        {
            if (polygonControl1.QuoteRangs.Count != 0)
            {
                statusLable.Text = History[polygonControl1.QuoteRangs.First().Key].ToString();
            }
        }

        void comboOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            polygonControl1.FirstQuadrant = (PolygonControl.FirstQuadrantOrientation)(comboOrientation.SelectedIndex);
        }

        //void comboStudyObjects_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //if (polygonControl1.PivotsHolder != null)
        //    //{
        //    //    string name = (comboStudyObjects.SelectedItem == null) ? comboStudyObjects.Items[0].ToString() : comboStudyObjects.SelectedItem.ToString();

        //    //    if (name == "Off")
        //    //    {
        //    //        polygonControl1.PivotsHolder.IsVisible = false;
        //    //        polygonControl1.PivotsHolder.PlanetForStudy = PlanetId.SE_ECL_NUT;
        //    //    }
        //    //    else
        //    //    {
        //    //        polygonControl1.PivotsHolder.IsVisible = true;

        //    //        PlanetId id = (PlanetId)(Enum.Parse(typeof(PlanetId), name));
        //    //        polygonControl1.PivotsHolder.PlanetForStudy = id;
        //    //    }

        //    //    polygonControl1.Redraw();
        //    //}
        //}

        void comboOverlays_SelectedIndexChanged(object sender, EventArgs e)
        {
            int divisions;
            if (int.TryParse(comboOverlays.SelectedItem as String, out divisions))
            {
                if (divisions == -1)
                {
                    if (polygonControl1.TheOverlay.IsVisible)
                    {
                        polygonControl1.TheOverlay.IsVisible = false;
                    }
                    else
                    {
                        polygonControl1.TheOverlay.IsVisible = true;
                    }
                }
                else if (divisions == 0)
                    polygonControl1.TheOverlay.Ring.IsVisible = polygonControl1.TheOverlay.Ring.IsVisible ? false : true;
                else if (!polygonControl1.TheOverlay.HandSets.ContainsKey(divisions))
                    polygonControl1.TheOverlay.AddOrShowHands(divisions, 1f);
                else
                    polygonControl1.TheOverlay.HandSets[divisions].IsVisble = polygonControl1.TheOverlay.HandSets[divisions].IsVisble ? false : true;

                polygonControl1.TheOverlay.IsOverlayChanged = true;
                polygonControl1.OverlayRedraw();
            }
        }


        private void textMaxCycle_Leave(object sender, EventArgs e)
        {
            int newSize;
            if (int.TryParse(textMaxCycle.Text, out newSize) && newSize != polygonControl1.MaxCycle)
            {
                polygonControl1.MaxCycle = newSize;
            }

        }

        private void textUnitSize_Leave(object sender, EventArgs e)
        {
            int newSize;
            if (int.TryParse(textUnitSize.Text, out newSize) && newSize != polygonControl1.UnitSize)
            {
                polygonControl1.UnitSize = newSize;
            }
        }

        private void polygonControl1_SizeChanged(object sender, EventArgs e)
        {
            this.panel1.Size = polygonControl1.Size;
        }

        void comboPolygons_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = (comboPolygons.SelectedItem == null) ? comboPolygons.Items[0].ToString() : comboPolygons.SelectedItem.ToString();
            if (helper != null)
            {
                if (Polygon.All.ContainsKey(name))
                    helper.CycleMapper = Polygon.All[name];
                else
                    throw new Exception();
            }

            if (polygonControl1.Calculator.Shape.Name != name)
            {
                Polygon polygon = Polygon.All[name];
                polygonControl1.Calculator = 
                    new PolygonControl.PolygonControl.PolygonCalculator(polygonControl1, polygon, polygonControl1.MaxCycle);

                //polygonControl1.Redraw();
            }
        }

        private void textStepSize_Leave(object sender, EventArgs e)
        {
            double newSize;
            if (double.TryParse(textStepSize.Text, out newSize) && newSize != polygonControl1.Adapter.Step)
            {
                polygonControl1.Adapter = new PriceAdapter(newSize);
            }
        }


        #endregion

    }
}
