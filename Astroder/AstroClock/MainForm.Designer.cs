namespace AstroClock
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOrbitsViewer = new System.Windows.Forms.TabPage();
            this.zedLongTerm = new ZedGraph.ZedGraphControl();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelEvents = new System.Windows.Forms.Panel();
            this.comboConcernedPlanet = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAspects = new System.Windows.Forms.CheckBox();
            this.checkBoxHeight = new System.Windows.Forms.CheckBox();
            this.checkBoxDirection = new System.Windows.Forms.CheckBox();
            this.checkBoxOccultation = new System.Windows.Forms.CheckBox();
            this.checkBoxSignChanges = new System.Windows.Forms.CheckBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonHeliocentric = new System.Windows.Forms.RadioButton();
            this.radioButtonGeocentric = new System.Windows.Forms.RadioButton();
            this.panelAspects = new System.Windows.Forms.Panel();
            this.buttonResetAll = new System.Windows.Forms.Button();
            this.buttonAllAspectsOn = new System.Windows.Forms.Button();
            this.textBoxOffset = new System.Windows.Forms.TextBox();
            this.buttonClearAspects = new System.Windows.Forms.Button();
            this.comboFocusedStar = new System.Windows.Forms.ComboBox();
            this.checkBoxOpposition = new System.Windows.Forms.CheckBox();
            this.checkBoxSquare = new System.Windows.Forms.CheckBox();
            this.checkBoxQuintile = new System.Windows.Forms.CheckBox();
            this.checkBoxSextile = new System.Windows.Forms.CheckBox();
            this.checkBoxTrine = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxReadings = new System.Windows.Forms.CheckBox();
            this.checkBoxNowTimeline = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonAverages = new System.Windows.Forms.RadioButton();
            this.radioButtonOuters = new System.Windows.Forms.RadioButton();
            this.radioButtonInners = new System.Windows.Forms.RadioButton();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            this.panelStars = new System.Windows.Forms.Panel();
            this.checkBoxSun = new System.Windows.Forms.CheckBox();
            this.checkBoxMoon = new System.Windows.Forms.CheckBox();
            this.checkBoxMercury = new System.Windows.Forms.CheckBox();
            this.checkBoxVenus = new System.Windows.Forms.CheckBox();
            this.checkBoxMars = new System.Windows.Forms.CheckBox();
            this.checkBoxJupiter = new System.Windows.Forms.CheckBox();
            this.checkBoxSaturn = new System.Windows.Forms.CheckBox();
            this.checkBoxUranus = new System.Windows.Forms.CheckBox();
            this.checkBoxNeptune = new System.Windows.Forms.CheckBox();
            this.checkBoxPluto = new System.Windows.Forms.CheckBox();
            this.checkBoxFive = new System.Windows.Forms.CheckBox();
            this.checkBoxSixAverage = new System.Windows.Forms.CheckBox();
            this.checkBoxEightAverage = new System.Windows.Forms.CheckBox();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.tabQuoteViewer = new System.Windows.Forms.TabPage();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabOrbitsViewer.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelEvents.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelAspects.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelStars.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOrbitsViewer);
            this.tabControl1.Controls.Add(this.tabQuoteViewer);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1272, 766);
            this.tabControl1.TabIndex = 0;
            // 
            // tabOrbitsViewer
            // 
            this.tabOrbitsViewer.Controls.Add(this.zedLongTerm);
            this.tabOrbitsViewer.Controls.Add(this.panelMain);
            this.tabOrbitsViewer.Location = new System.Drawing.Point(4, 22);
            this.tabOrbitsViewer.Name = "tabOrbitsViewer";
            this.tabOrbitsViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tabOrbitsViewer.Size = new System.Drawing.Size(1264, 740);
            this.tabOrbitsViewer.TabIndex = 0;
            this.tabOrbitsViewer.Text = "Orbits Viewer";
            this.tabOrbitsViewer.UseVisualStyleBackColor = true;
            // 
            // zedLongTerm
            // 
            this.zedLongTerm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.zedLongTerm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedLongTerm.IsEnableSelection = true;
            this.zedLongTerm.IsShowPointValues = true;
            this.zedLongTerm.Location = new System.Drawing.Point(3, 59);
            this.zedLongTerm.Name = "zedLongTerm";
            this.zedLongTerm.ScrollGrace = 0;
            this.zedLongTerm.ScrollMaxX = 0;
            this.zedLongTerm.ScrollMaxY = 0;
            this.zedLongTerm.ScrollMaxY2 = 0;
            this.zedLongTerm.ScrollMinX = 0;
            this.zedLongTerm.ScrollMinY = 0;
            this.zedLongTerm.ScrollMinY2 = 0;
            this.zedLongTerm.Size = new System.Drawing.Size(1258, 678);
            this.zedLongTerm.TabIndex = 1;
            this.zedLongTerm.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedLongTerm_PointValueEvent);
            this.zedLongTerm.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedGeoView_ZoomEvent);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.PowderBlue;
            this.panelMain.Controls.Add(this.panelEvents);
            this.panelMain.Controls.Add(this.dateTimePicker1);
            this.panelMain.Controls.Add(this.label1);
            this.panelMain.Controls.Add(this.groupBox2);
            this.panelMain.Controls.Add(this.panelAspects);
            this.panelMain.Controls.Add(this.panel2);
            this.panelMain.Controls.Add(this.groupBox1);
            this.panelMain.Controls.Add(this.panelStars);
            this.panelMain.Controls.Add(this.dateTimePicker2);
            this.panelMain.Controls.Add(this.label2);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMain.Location = new System.Drawing.Point(3, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1258, 56);
            this.panelMain.TabIndex = 0;
            // 
            // panelEvents
            // 
            this.panelEvents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelEvents.Controls.Add(this.comboConcernedPlanet);
            this.panelEvents.Controls.Add(this.label3);
            this.panelEvents.Controls.Add(this.checkBoxAspects);
            this.panelEvents.Controls.Add(this.checkBoxHeight);
            this.panelEvents.Controls.Add(this.checkBoxDirection);
            this.panelEvents.Controls.Add(this.checkBoxOccultation);
            this.panelEvents.Controls.Add(this.checkBoxSignChanges);
            this.panelEvents.Location = new System.Drawing.Point(991, 2);
            this.panelEvents.Name = "panelEvents";
            this.panelEvents.Size = new System.Drawing.Size(265, 50);
            this.panelEvents.TabIndex = 8;
            // 
            // comboConcernedPlanet
            // 
            this.comboConcernedPlanet.FormattingEnabled = true;
            this.comboConcernedPlanet.Location = new System.Drawing.Point(91, 24);
            this.comboConcernedPlanet.Name = "comboConcernedPlanet";
            this.comboConcernedPlanet.Size = new System.Drawing.Size(86, 21);
            this.comboConcernedPlanet.TabIndex = 3;
            this.comboConcernedPlanet.SelectedIndexChanged += new System.EventHandler(this.comboConcernedPlanet_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "with";
            // 
            // checkBoxAspects
            // 
            this.checkBoxAspects.AutoSize = true;
            this.checkBoxAspects.Location = new System.Drawing.Point(4, 26);
            this.checkBoxAspects.Name = "checkBoxAspects";
            this.checkBoxAspects.Size = new System.Drawing.Size(64, 17);
            this.checkBoxAspects.TabIndex = 1;
            this.checkBoxAspects.Text = "Aspects";
            this.checkBoxAspects.UseVisualStyleBackColor = true;
            this.checkBoxAspects.CheckedChanged += new System.EventHandler(this.checkBoxEventCategory_CheckedChanged);
            // 
            // checkBoxHeight
            // 
            this.checkBoxHeight.AutoSize = true;
            this.checkBoxHeight.Location = new System.Drawing.Point(155, 5);
            this.checkBoxHeight.Name = "checkBoxHeight";
            this.checkBoxHeight.Size = new System.Drawing.Size(57, 17);
            this.checkBoxHeight.TabIndex = 0;
            this.checkBoxHeight.Text = "Height";
            this.checkBoxHeight.UseVisualStyleBackColor = true;
            this.checkBoxHeight.CheckedChanged += new System.EventHandler(this.checkBoxEventCategory_CheckedChanged);
            // 
            // checkBoxDirection
            // 
            this.checkBoxDirection.AutoSize = true;
            this.checkBoxDirection.Location = new System.Drawing.Point(101, 5);
            this.checkBoxDirection.Name = "checkBoxDirection";
            this.checkBoxDirection.Size = new System.Drawing.Size(54, 17);
            this.checkBoxDirection.TabIndex = 0;
            this.checkBoxDirection.Text = "Direct";
            this.checkBoxDirection.UseVisualStyleBackColor = true;
            this.checkBoxDirection.CheckedChanged += new System.EventHandler(this.checkBoxEventCategory_CheckedChanged);
            // 
            // checkBoxOccultation
            // 
            this.checkBoxOccultation.AutoSize = true;
            this.checkBoxOccultation.Location = new System.Drawing.Point(3, 5);
            this.checkBoxOccultation.Name = "checkBoxOccultation";
            this.checkBoxOccultation.Size = new System.Drawing.Size(52, 17);
            this.checkBoxOccultation.TabIndex = 0;
            this.checkBoxOccultation.Text = "Occul";
            this.checkBoxOccultation.UseVisualStyleBackColor = true;
            this.checkBoxOccultation.CheckedChanged += new System.EventHandler(this.checkBoxEventCategory_CheckedChanged);
            // 
            // checkBoxSignChanges
            // 
            this.checkBoxSignChanges.AutoSize = true;
            this.checkBoxSignChanges.Location = new System.Drawing.Point(55, 5);
            this.checkBoxSignChanges.Name = "checkBoxSignChanges";
            this.checkBoxSignChanges.Size = new System.Drawing.Size(46, 17);
            this.checkBoxSignChanges.TabIndex = 0;
            this.checkBoxSignChanges.Text = "Sign";
            this.checkBoxSignChanges.UseVisualStyleBackColor = true;
            this.checkBoxSignChanges.CheckedChanged += new System.EventHandler(this.checkBoxEventCategory_CheckedChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(111, 5);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(86, 20);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Leave += new System.EventHandler(this.dateTimePicker1_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From:";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.radioButtonHeliocentric);
            this.groupBox2.Controls.Add(this.radioButtonGeocentric);
            this.groupBox2.Location = new System.Drawing.Point(3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(83, 42);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonHeliocentric
            // 
            this.radioButtonHeliocentric.AutoSize = true;
            this.radioButtonHeliocentric.Location = new System.Drawing.Point(0, 26);
            this.radioButtonHeliocentric.Name = "radioButtonHeliocentric";
            this.radioButtonHeliocentric.Size = new System.Drawing.Size(80, 17);
            this.radioButtonHeliocentric.TabIndex = 0;
            this.radioButtonHeliocentric.Text = "Heliocentric";
            this.radioButtonHeliocentric.UseVisualStyleBackColor = true;
            this.radioButtonHeliocentric.CheckedChanged += new System.EventHandler(this.radioCentric_CheckedChanged);
            // 
            // radioButtonGeocentric
            // 
            this.radioButtonGeocentric.AutoSize = true;
            this.radioButtonGeocentric.Checked = true;
            this.radioButtonGeocentric.Location = new System.Drawing.Point(0, 2);
            this.radioButtonGeocentric.Name = "radioButtonGeocentric";
            this.radioButtonGeocentric.Size = new System.Drawing.Size(76, 17);
            this.radioButtonGeocentric.TabIndex = 0;
            this.radioButtonGeocentric.TabStop = true;
            this.radioButtonGeocentric.Text = "Geocentric";
            this.radioButtonGeocentric.UseVisualStyleBackColor = true;
            this.radioButtonGeocentric.CheckedChanged += new System.EventHandler(this.radioCentric_CheckedChanged);
            // 
            // panelAspects
            // 
            this.panelAspects.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelAspects.Controls.Add(this.buttonResetAll);
            this.panelAspects.Controls.Add(this.buttonAllAspectsOn);
            this.panelAspects.Controls.Add(this.textBoxOffset);
            this.panelAspects.Controls.Add(this.buttonClearAspects);
            this.panelAspects.Controls.Add(this.comboFocusedStar);
            this.panelAspects.Controls.Add(this.checkBoxOpposition);
            this.panelAspects.Controls.Add(this.checkBoxSquare);
            this.panelAspects.Controls.Add(this.checkBoxQuintile);
            this.panelAspects.Controls.Add(this.checkBoxSextile);
            this.panelAspects.Controls.Add(this.checkBoxTrine);
            this.panelAspects.Location = new System.Drawing.Point(742, 1);
            this.panelAspects.Name = "panelAspects";
            this.panelAspects.Size = new System.Drawing.Size(243, 51);
            this.panelAspects.TabIndex = 5;
            // 
            // buttonResetAll
            // 
            this.buttonResetAll.Location = new System.Drawing.Point(193, 2);
            this.buttonResetAll.Name = "buttonResetAll";
            this.buttonResetAll.Size = new System.Drawing.Size(45, 23);
            this.buttonResetAll.TabIndex = 3;
            this.buttonResetAll.Text = "Reset";
            this.buttonResetAll.UseVisualStyleBackColor = true;
            this.buttonResetAll.Click += new System.EventHandler(this.buttonResetAll_Click);
            // 
            // buttonAllAspectsOn
            // 
            this.buttonAllAspectsOn.Location = new System.Drawing.Point(94, 2);
            this.buttonAllAspectsOn.Name = "buttonAllAspectsOn";
            this.buttonAllAspectsOn.Size = new System.Drawing.Size(45, 23);
            this.buttonAllAspectsOn.TabIndex = 3;
            this.buttonAllAspectsOn.Text = "AllOn";
            this.buttonAllAspectsOn.UseVisualStyleBackColor = true;
            this.buttonAllAspectsOn.Click += new System.EventHandler(this.buttonAllAspectsOn_Click);
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Location = new System.Drawing.Point(200, 27);
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(36, 20);
            this.textBoxOffset.TabIndex = 2;
            this.textBoxOffset.Leave += new System.EventHandler(this.textBoxOffset_Leave);
            // 
            // buttonClearAspects
            // 
            this.buttonClearAspects.Location = new System.Drawing.Point(144, 2);
            this.buttonClearAspects.Name = "buttonClearAspects";
            this.buttonClearAspects.Size = new System.Drawing.Size(44, 23);
            this.buttonClearAspects.TabIndex = 1;
            this.buttonClearAspects.Text = "Clear";
            this.buttonClearAspects.UseVisualStyleBackColor = true;
            this.buttonClearAspects.Click += new System.EventHandler(this.buttonClearAspects_Click);
            // 
            // comboFocusedStar
            // 
            this.comboFocusedStar.FormattingEnabled = true;
            this.comboFocusedStar.Location = new System.Drawing.Point(3, 3);
            this.comboFocusedStar.Name = "comboFocusedStar";
            this.comboFocusedStar.Size = new System.Drawing.Size(85, 21);
            this.comboFocusedStar.TabIndex = 0;
            this.comboFocusedStar.SelectedIndexChanged += new System.EventHandler(this.comboFocusedStar_SelectedIndexChanged);
            // 
            // checkBoxOpposition
            // 
            this.checkBoxOpposition.AutoSize = true;
            this.checkBoxOpposition.Location = new System.Drawing.Point(160, 31);
            this.checkBoxOpposition.Name = "checkBoxOpposition";
            this.checkBoxOpposition.Size = new System.Drawing.Size(44, 17);
            this.checkBoxOpposition.TabIndex = 0;
            this.checkBoxOpposition.Text = "180";
            this.checkBoxOpposition.UseVisualStyleBackColor = true;
            this.checkBoxOpposition.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxSquare
            // 
            this.checkBoxSquare.AutoSize = true;
            this.checkBoxSquare.Location = new System.Drawing.Point(78, 31);
            this.checkBoxSquare.Name = "checkBoxSquare";
            this.checkBoxSquare.Size = new System.Drawing.Size(38, 17);
            this.checkBoxSquare.TabIndex = 0;
            this.checkBoxSquare.Text = "90";
            this.checkBoxSquare.UseVisualStyleBackColor = true;
            this.checkBoxSquare.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxQuintile
            // 
            this.checkBoxQuintile.AutoSize = true;
            this.checkBoxQuintile.Location = new System.Drawing.Point(40, 31);
            this.checkBoxQuintile.Name = "checkBoxQuintile";
            this.checkBoxQuintile.Size = new System.Drawing.Size(38, 17);
            this.checkBoxQuintile.TabIndex = 0;
            this.checkBoxQuintile.Text = "72";
            this.checkBoxQuintile.UseVisualStyleBackColor = true;
            this.checkBoxQuintile.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxSextile
            // 
            this.checkBoxSextile.AutoSize = true;
            this.checkBoxSextile.Location = new System.Drawing.Point(2, 31);
            this.checkBoxSextile.Name = "checkBoxSextile";
            this.checkBoxSextile.Size = new System.Drawing.Size(38, 17);
            this.checkBoxSextile.TabIndex = 0;
            this.checkBoxSextile.Text = "60";
            this.checkBoxSextile.UseVisualStyleBackColor = true;
            this.checkBoxSextile.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxTrine
            // 
            this.checkBoxTrine.AutoSize = true;
            this.checkBoxTrine.Location = new System.Drawing.Point(116, 31);
            this.checkBoxTrine.Name = "checkBoxTrine";
            this.checkBoxTrine.Size = new System.Drawing.Size(44, 17);
            this.checkBoxTrine.TabIndex = 0;
            this.checkBoxTrine.Text = "120";
            this.checkBoxTrine.UseVisualStyleBackColor = true;
            this.checkBoxTrine.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBoxReadings);
            this.panel2.Controls.Add(this.checkBoxNowTimeline);
            this.panel2.Location = new System.Drawing.Point(197, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(81, 48);
            this.panel2.TabIndex = 4;
            // 
            // checkBoxReadings
            // 
            this.checkBoxReadings.AutoSize = true;
            this.checkBoxReadings.Checked = true;
            this.checkBoxReadings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxReadings.Location = new System.Drawing.Point(4, 27);
            this.checkBoxReadings.Name = "checkBoxReadings";
            this.checkBoxReadings.Size = new System.Drawing.Size(65, 17);
            this.checkBoxReadings.TabIndex = 0;
            this.checkBoxReadings.Text = "Reading";
            this.checkBoxReadings.UseVisualStyleBackColor = true;
            this.checkBoxReadings.CheckedChanged += new System.EventHandler(this.checkBoxCurrent_CheckedChanged);
            // 
            // checkBoxNowTimeline
            // 
            this.checkBoxNowTimeline.AutoSize = true;
            this.checkBoxNowTimeline.Checked = true;
            this.checkBoxNowTimeline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNowTimeline.Location = new System.Drawing.Point(4, 5);
            this.checkBoxNowTimeline.Name = "checkBoxNowTimeline";
            this.checkBoxNowTimeline.Size = new System.Drawing.Size(76, 17);
            this.checkBoxNowTimeline.TabIndex = 0;
            this.checkBoxNowTimeline.Text = "Show Now";
            this.checkBoxNowTimeline.UseVisualStyleBackColor = true;
            this.checkBoxNowTimeline.CheckedChanged += new System.EventHandler(this.checkBoxCurrent_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAverages);
            this.groupBox1.Controls.Add(this.radioButtonOuters);
            this.groupBox1.Controls.Add(this.radioButtonInners);
            this.groupBox1.Controls.Add(this.radioButtonAll);
            this.groupBox1.Location = new System.Drawing.Point(277, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(117, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "StarSet Choices:";
            // 
            // radioButtonAverages
            // 
            this.radioButtonAverages.AutoSize = true;
            this.radioButtonAverages.Location = new System.Drawing.Point(59, 13);
            this.radioButtonAverages.Name = "radioButtonAverages";
            this.radioButtonAverages.Size = new System.Drawing.Size(53, 17);
            this.radioButtonAverages.TabIndex = 0;
            this.radioButtonAverages.Text = "Avrgs";
            this.radioButtonAverages.UseVisualStyleBackColor = true;
            this.radioButtonAverages.CheckedChanged += new System.EventHandler(this.starSetRadioButton_CheckedChanged);
            // 
            // radioButtonOuters
            // 
            this.radioButtonOuters.AutoSize = true;
            this.radioButtonOuters.Location = new System.Drawing.Point(59, 29);
            this.radioButtonOuters.Name = "radioButtonOuters";
            this.radioButtonOuters.Size = new System.Drawing.Size(58, 17);
            this.radioButtonOuters.TabIndex = 0;
            this.radioButtonOuters.Text = "Outers";
            this.radioButtonOuters.UseVisualStyleBackColor = true;
            this.radioButtonOuters.CheckedChanged += new System.EventHandler(this.starSetRadioButton_CheckedChanged);
            // 
            // radioButtonInners
            // 
            this.radioButtonInners.AutoSize = true;
            this.radioButtonInners.Location = new System.Drawing.Point(7, 29);
            this.radioButtonInners.Name = "radioButtonInners";
            this.radioButtonInners.Size = new System.Drawing.Size(56, 17);
            this.radioButtonInners.TabIndex = 0;
            this.radioButtonInners.Text = "Inners";
            this.radioButtonInners.UseVisualStyleBackColor = true;
            this.radioButtonInners.CheckedChanged += new System.EventHandler(this.starSetRadioButton_CheckedChanged);
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Checked = true;
            this.radioButtonAll.Location = new System.Drawing.Point(7, 13);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new System.Drawing.Size(36, 17);
            this.radioButtonAll.TabIndex = 0;
            this.radioButtonAll.TabStop = true;
            this.radioButtonAll.Text = "All";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new System.EventHandler(this.starSetRadioButton_CheckedChanged);
            // 
            // panelStars
            // 
            this.panelStars.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStars.Controls.Add(this.checkBoxSun);
            this.panelStars.Controls.Add(this.checkBoxMoon);
            this.panelStars.Controls.Add(this.checkBoxMercury);
            this.panelStars.Controls.Add(this.checkBoxVenus);
            this.panelStars.Controls.Add(this.checkBoxMars);
            this.panelStars.Controls.Add(this.checkBoxJupiter);
            this.panelStars.Controls.Add(this.checkBoxSaturn);
            this.panelStars.Controls.Add(this.checkBoxUranus);
            this.panelStars.Controls.Add(this.checkBoxNeptune);
            this.panelStars.Controls.Add(this.checkBoxPluto);
            this.panelStars.Controls.Add(this.checkBoxFive);
            this.panelStars.Controls.Add(this.checkBoxSixAverage);
            this.panelStars.Controls.Add(this.checkBoxEightAverage);
            this.panelStars.Location = new System.Drawing.Point(400, 2);
            this.panelStars.Name = "panelStars";
            this.panelStars.Size = new System.Drawing.Size(339, 50);
            this.panelStars.TabIndex = 2;
            // 
            // checkBoxSun
            // 
            this.checkBoxSun.AutoSize = true;
            this.checkBoxSun.Checked = true;
            this.checkBoxSun.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSun.Location = new System.Drawing.Point(4, 6);
            this.checkBoxSun.Name = "checkBoxSun";
            this.checkBoxSun.Size = new System.Drawing.Size(44, 17);
            this.checkBoxSun.TabIndex = 0;
            this.checkBoxSun.Text = "Sun";
            this.checkBoxSun.UseVisualStyleBackColor = true;
            this.checkBoxSun.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxMoon
            // 
            this.checkBoxMoon.AutoSize = true;
            this.checkBoxMoon.Location = new System.Drawing.Point(56, 6);
            this.checkBoxMoon.Name = "checkBoxMoon";
            this.checkBoxMoon.Size = new System.Drawing.Size(52, 17);
            this.checkBoxMoon.TabIndex = 0;
            this.checkBoxMoon.Text = "Moon";
            this.checkBoxMoon.UseVisualStyleBackColor = true;
            this.checkBoxMoon.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxMercury
            // 
            this.checkBoxMercury.AutoSize = true;
            this.checkBoxMercury.Checked = true;
            this.checkBoxMercury.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMercury.Location = new System.Drawing.Point(108, 6);
            this.checkBoxMercury.Name = "checkBoxMercury";
            this.checkBoxMercury.Size = new System.Drawing.Size(65, 17);
            this.checkBoxMercury.TabIndex = 0;
            this.checkBoxMercury.Text = "Mercury";
            this.checkBoxMercury.UseVisualStyleBackColor = true;
            this.checkBoxMercury.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxVenus
            // 
            this.checkBoxVenus.AutoSize = true;
            this.checkBoxVenus.Checked = true;
            this.checkBoxVenus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVenus.Location = new System.Drawing.Point(173, 6);
            this.checkBoxVenus.Name = "checkBoxVenus";
            this.checkBoxVenus.Size = new System.Drawing.Size(55, 17);
            this.checkBoxVenus.TabIndex = 0;
            this.checkBoxVenus.Text = "Venus";
            this.checkBoxVenus.UseVisualStyleBackColor = true;
            this.checkBoxVenus.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxMars
            // 
            this.checkBoxMars.AutoSize = true;
            this.checkBoxMars.Checked = true;
            this.checkBoxMars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMars.Location = new System.Drawing.Point(228, 6);
            this.checkBoxMars.Name = "checkBoxMars";
            this.checkBoxMars.Size = new System.Drawing.Size(49, 17);
            this.checkBoxMars.TabIndex = 0;
            this.checkBoxMars.Text = "Mars";
            this.checkBoxMars.UseVisualStyleBackColor = true;
            this.checkBoxMars.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxJupiter
            // 
            this.checkBoxJupiter.AutoSize = true;
            this.checkBoxJupiter.Checked = true;
            this.checkBoxJupiter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxJupiter.Location = new System.Drawing.Point(277, 6);
            this.checkBoxJupiter.Name = "checkBoxJupiter";
            this.checkBoxJupiter.Size = new System.Drawing.Size(59, 17);
            this.checkBoxJupiter.TabIndex = 0;
            this.checkBoxJupiter.Text = "Jupiter";
            this.checkBoxJupiter.UseVisualStyleBackColor = true;
            this.checkBoxJupiter.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxSaturn
            // 
            this.checkBoxSaturn.AutoSize = true;
            this.checkBoxSaturn.Checked = true;
            this.checkBoxSaturn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSaturn.Location = new System.Drawing.Point(4, 28);
            this.checkBoxSaturn.Name = "checkBoxSaturn";
            this.checkBoxSaturn.Size = new System.Drawing.Size(58, 17);
            this.checkBoxSaturn.TabIndex = 0;
            this.checkBoxSaturn.Text = "Saturn";
            this.checkBoxSaturn.UseVisualStyleBackColor = true;
            this.checkBoxSaturn.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxUranus
            // 
            this.checkBoxUranus.AutoSize = true;
            this.checkBoxUranus.Checked = true;
            this.checkBoxUranus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUranus.Location = new System.Drawing.Point(62, 28);
            this.checkBoxUranus.Name = "checkBoxUranus";
            this.checkBoxUranus.Size = new System.Drawing.Size(60, 17);
            this.checkBoxUranus.TabIndex = 0;
            this.checkBoxUranus.Text = "Uranus";
            this.checkBoxUranus.UseVisualStyleBackColor = true;
            this.checkBoxUranus.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxNeptune
            // 
            this.checkBoxNeptune.AutoSize = true;
            this.checkBoxNeptune.Checked = true;
            this.checkBoxNeptune.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNeptune.Location = new System.Drawing.Point(122, 28);
            this.checkBoxNeptune.Name = "checkBoxNeptune";
            this.checkBoxNeptune.Size = new System.Drawing.Size(67, 17);
            this.checkBoxNeptune.TabIndex = 0;
            this.checkBoxNeptune.Text = "Neptune";
            this.checkBoxNeptune.UseVisualStyleBackColor = true;
            this.checkBoxNeptune.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxPluto
            // 
            this.checkBoxPluto.AutoSize = true;
            this.checkBoxPluto.Checked = true;
            this.checkBoxPluto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPluto.Location = new System.Drawing.Point(189, 28);
            this.checkBoxPluto.Name = "checkBoxPluto";
            this.checkBoxPluto.Size = new System.Drawing.Size(50, 17);
            this.checkBoxPluto.TabIndex = 0;
            this.checkBoxPluto.Text = "Pluto";
            this.checkBoxPluto.UseVisualStyleBackColor = true;
            this.checkBoxPluto.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxFive
            // 
            this.checkBoxFive.AutoSize = true;
            this.checkBoxFive.Location = new System.Drawing.Point(239, 28);
            this.checkBoxFive.Name = "checkBoxFive";
            this.checkBoxFive.Size = new System.Drawing.Size(32, 17);
            this.checkBoxFive.TabIndex = 0;
            this.checkBoxFive.Text = "5";
            this.checkBoxFive.UseVisualStyleBackColor = true;
            this.checkBoxFive.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxSixAverage
            // 
            this.checkBoxSixAverage.AutoSize = true;
            this.checkBoxSixAverage.Location = new System.Drawing.Point(271, 28);
            this.checkBoxSixAverage.Name = "checkBoxSixAverage";
            this.checkBoxSixAverage.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSixAverage.TabIndex = 0;
            this.checkBoxSixAverage.Text = "6";
            this.checkBoxSixAverage.UseVisualStyleBackColor = true;
            this.checkBoxSixAverage.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // checkBoxEightAverage
            // 
            this.checkBoxEightAverage.AutoSize = true;
            this.checkBoxEightAverage.Location = new System.Drawing.Point(303, 28);
            this.checkBoxEightAverage.Name = "checkBoxEightAverage";
            this.checkBoxEightAverage.Size = new System.Drawing.Size(32, 17);
            this.checkBoxEightAverage.TabIndex = 0;
            this.checkBoxEightAverage.Text = "8";
            this.checkBoxEightAverage.UseVisualStyleBackColor = true;
            this.checkBoxEightAverage.CheckedChanged += new System.EventHandler(this.checkBoxStar_CheckedChanged);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(111, 29);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(86, 20);
            this.dateTimePicker2.TabIndex = 1;
            this.dateTimePicker2.Value = new System.DateTime(2012, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker2.Leave += new System.EventHandler(this.dateTimePicker2_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(92, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To:";
            // 
            // tabQuoteViewer
            // 
            this.tabQuoteViewer.Location = new System.Drawing.Point(4, 22);
            this.tabQuoteViewer.Name = "tabQuoteViewer";
            this.tabQuoteViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tabQuoteViewer.Size = new System.Drawing.Size(1264, 740);
            this.tabQuoteViewer.TabIndex = 1;
            this.tabQuoteViewer.Text = "QuotesViewer";
            this.tabQuoteViewer.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1272, 766);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "AstroClock";
            this.tabControl1.ResumeLayout(false);
            this.tabOrbitsViewer.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelEvents.ResumeLayout(false);
            this.panelEvents.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelAspects.ResumeLayout(false);
            this.panelAspects.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelStars.ResumeLayout(false);
            this.panelStars.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOrbitsViewer;
        private System.Windows.Forms.TabPage tabQuoteViewer;
        private ZedGraph.ZedGraphControl zedLongTerm;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelStars;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonInners;
        private System.Windows.Forms.RadioButton radioButtonAll;
        private System.Windows.Forms.RadioButton radioButtonAverages;
        private System.Windows.Forms.RadioButton radioButtonOuters;
        private System.Windows.Forms.CheckBox checkBoxSun;
        private System.Windows.Forms.CheckBox checkBoxMars;
        private System.Windows.Forms.CheckBox checkBoxVenus;
        private System.Windows.Forms.CheckBox checkBoxMoon;
        private System.Windows.Forms.CheckBox checkBoxMercury;
        private System.Windows.Forms.CheckBox checkBoxJupiter;
        private System.Windows.Forms.CheckBox checkBoxEightAverage;
        private System.Windows.Forms.CheckBox checkBoxPluto;
        private System.Windows.Forms.CheckBox checkBoxSixAverage;
        private System.Windows.Forms.CheckBox checkBoxNeptune;
        private System.Windows.Forms.CheckBox checkBoxFive;
        private System.Windows.Forms.CheckBox checkBoxUranus;
        private System.Windows.Forms.CheckBox checkBoxSaturn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBoxReadings;
        private System.Windows.Forms.CheckBox checkBoxNowTimeline;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panelAspects;
        private System.Windows.Forms.Button buttonClearAspects;
        private System.Windows.Forms.ComboBox comboFocusedStar;
        private System.Windows.Forms.CheckBox checkBoxSquare;
        private System.Windows.Forms.CheckBox checkBoxSextile;
        private System.Windows.Forms.CheckBox checkBoxTrine;
        private System.Windows.Forms.CheckBox checkBoxOpposition;
        private System.Windows.Forms.CheckBox checkBoxQuintile;
        private System.Windows.Forms.TextBox textBoxOffset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonHeliocentric;
        private System.Windows.Forms.RadioButton radioButtonGeocentric;
        private System.Windows.Forms.Panel panelEvents;
        private System.Windows.Forms.CheckBox checkBoxHeight;
        private System.Windows.Forms.CheckBox checkBoxDirection;
        private System.Windows.Forms.CheckBox checkBoxSignChanges;
        private System.Windows.Forms.CheckBox checkBoxOccultation;
        private System.Windows.Forms.Button buttonResetAll;
        private System.Windows.Forms.Button buttonAllAspectsOn;
        private System.Windows.Forms.CheckBox checkBoxAspects;
        private System.Windows.Forms.ComboBox comboConcernedPlanet;
        private System.Windows.Forms.Label label3;
    }
}

