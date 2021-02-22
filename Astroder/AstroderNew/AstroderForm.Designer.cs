namespace Astroder
{
    partial class AstroderForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param fileName="disposing">如果应释放托管资源，为 true；否则为 false。</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AstroderForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPoboToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputManuallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showKLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mousePositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTodayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setPivotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heliocentricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuYAxisMode = new System.Windows.Forms.ToolStripMenuItem();
            this.periodTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxPeriodType = new System.Windows.Forms.ToolStripComboBox();
            this.theToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.longitudeStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.eventsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageInterday = new System.Windows.Forms.TabPage();
            this.zedLongTerm = new ZedGraph.ZedGraphControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCentric = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHighlightEvents = new System.Windows.Forms.ToolStripButton();
            this.btnShowLocked = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.comboOutlineThreshold = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.comboPriceToDegree = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.pageIntraday = new System.Windows.Forms.TabPage();
            this.zedShortTerm = new ZedGraph.ZedGraphControl();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.comboIntradayThreshold = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClearOrbitSets = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.comboY2Ratio = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.textY3Ratio = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonCentric = new System.Windows.Forms.ToolStripButton();
            this.comboPlanet = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.textOffset = new System.Windows.Forms.ToolStripTextBox();
            this.pageAstrolabe = new System.Windows.Forms.TabPage();
            this.dataGridViewDifference = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewHelioAstrolabe1 = new System.Windows.Forms.DataGridView();
            this.bindingSourceHelioAstrolabe1 = new System.Windows.Forms.BindingSource(this.components);
            this.textDate1Details = new System.Windows.Forms.TextBox();
            this.dataGridViewGeoAstrolabe1 = new System.Windows.Forms.DataGridView();
            this.bindingSourceGeoAstrolabe1 = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewHelioAstrolabe2 = new System.Windows.Forms.DataGridView();
            this.bindingSourceHelioAstrolabe2 = new System.Windows.Forms.BindingSource(this.components);
            this.textDate2Details = new System.Windows.Forms.TextBox();
            this.dataGridViewGeoAstrolabe2 = new System.Windows.Forms.DataGridView();
            this.bindingSourceGeoAstrolabe2 = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.buttonLastMonth = new System.Windows.Forms.ToolStripButton();
            this.buttonLastDay = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeYear1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeMonth1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeDay1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeTime1 = new System.Windows.Forms.ToolStripTextBox();
            this.buttonNextDay = new System.Windows.Forms.ToolStripButton();
            this.buttonNextMonth = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.comboPeriodType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.comboAspectImportance = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeYear2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeMonth2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeDay2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel12 = new System.Windows.Forms.ToolStripLabel();
            this.textAstrolabeTime2 = new System.Windows.Forms.ToolStripTextBox();
            this.pageCompare = new System.Windows.Forms.TabPage();
            this.textClues = new System.Windows.Forms.TextBox();
            this.comboRecordType = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.pageInterday.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pageIntraday.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.pageAstrolabe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDifference)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHelioAstrolabe1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHelioAstrolabe1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeoAstrolabe1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGeoAstrolabe1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHelioAstrolabe2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHelioAstrolabe2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeoAstrolabe2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGeoAstrolabe2)).BeginInit();
            this.toolStrip3.SuspendLayout();
            this.pageCompare.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.commandToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1270, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recentFilesToolStripMenuItem,
            this.importPoboToolStripMenuItem,
            this.openToolStripMenuItem,
            this.inputManuallyToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.recentFilesToolStripMenuItem.Text = "&Recent Files";
            // 
            // importPoboToolStripMenuItem
            // 
            this.importPoboToolStripMenuItem.Name = "importPoboToolStripMenuItem";
            this.importPoboToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.importPoboToolStripMenuItem.Text = "Import &Pobo";
            this.importPoboToolStripMenuItem.Click += new System.EventHandler(this.importPoboToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.openToolStripMenuItem.Text = "Import &Text";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // inputManuallyToolStripMenuItem
            // 
            this.inputManuallyToolStripMenuItem.Name = "inputManuallyToolStripMenuItem";
            this.inputManuallyToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.inputManuallyToolStripMenuItem.Text = "Input manually";
            this.inputManuallyToolStripMenuItem.Click += new System.EventHandler(this.inputManuallyToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(145, 22);
            this.openToolStripMenuItem1.Text = "&Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Checked = true;
            this.viewToolStripMenuItem.CheckOnClick = true;
            this.viewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLegendToolStripMenuItem,
            this.showKLineToolStripMenuItem,
            this.showOutlineToolStripMenuItem,
            this.mousePositionToolStripMenuItem,
            this.showTodayToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // showLegendToolStripMenuItem
            // 
            this.showLegendToolStripMenuItem.Checked = true;
            this.showLegendToolStripMenuItem.CheckOnClick = true;
            this.showLegendToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLegendToolStripMenuItem.Name = "showLegendToolStripMenuItem";
            this.showLegendToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.showLegendToolStripMenuItem.Text = "Show Legend";
            // 
            // showKLineToolStripMenuItem
            // 
            this.showKLineToolStripMenuItem.CheckOnClick = true;
            this.showKLineToolStripMenuItem.Name = "showKLineToolStripMenuItem";
            this.showKLineToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.showKLineToolStripMenuItem.Text = "Show K-Line";
            this.showKLineToolStripMenuItem.Click += new System.EventHandler(this.showKLineToolStripMenuItem_Click);
            // 
            // showOutlineToolStripMenuItem
            // 
            this.showOutlineToolStripMenuItem.Checked = true;
            this.showOutlineToolStripMenuItem.CheckOnClick = true;
            this.showOutlineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOutlineToolStripMenuItem.Name = "showOutlineToolStripMenuItem";
            this.showOutlineToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.showOutlineToolStripMenuItem.Text = "Show Outline";
            this.showOutlineToolStripMenuItem.Click += new System.EventHandler(this.showOutlineToolStripMenuItem_Click);
            // 
            // mousePositionToolStripMenuItem
            // 
            this.mousePositionToolStripMenuItem.Checked = true;
            this.mousePositionToolStripMenuItem.CheckOnClick = true;
            this.mousePositionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mousePositionToolStripMenuItem.Name = "mousePositionToolStripMenuItem";
            this.mousePositionToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.mousePositionToolStripMenuItem.Text = "Mouse Position";
            // 
            // showTodayToolStripMenuItem
            // 
            this.showTodayToolStripMenuItem.CheckOnClick = true;
            this.showTodayToolStripMenuItem.Name = "showTodayToolStripMenuItem";
            this.showTodayToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.showTodayToolStripMenuItem.Text = "Show Today";
            this.showTodayToolStripMenuItem.ToolTipText = "Show/hide mark of today";
            this.showTodayToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showTodayToolStripMenuItem_CheckedChanged);
            // 
            // commandToolStripMenuItem
            // 
            this.commandToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearMarksToolStripMenuItem,
            this.setPivotToolStripMenuItem});
            this.commandToolStripMenuItem.Name = "commandToolStripMenuItem";
            this.commandToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.commandToolStripMenuItem.Text = "Command";
            // 
            // clearMarksToolStripMenuItem
            // 
            this.clearMarksToolStripMenuItem.Name = "clearMarksToolStripMenuItem";
            this.clearMarksToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.clearMarksToolStripMenuItem.Text = "Clear Marks";
            this.clearMarksToolStripMenuItem.Click += new System.EventHandler(this.clearMarksToolStripMenuItem_Click);
            // 
            // setPivotToolStripMenuItem
            // 
            this.setPivotToolStripMenuItem.Name = "setPivotToolStripMenuItem";
            this.setPivotToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.setPivotToolStripMenuItem.Text = "Set Pivot";
            this.setPivotToolStripMenuItem.Click += new System.EventHandler(this.setPivotToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.heliocentricToolStripMenuItem,
            this.menuYAxisMode,
            this.periodTypeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // heliocentricToolStripMenuItem
            // 
            this.heliocentricToolStripMenuItem.CheckOnClick = true;
            this.heliocentricToolStripMenuItem.Name = "heliocentricToolStripMenuItem";
            this.heliocentricToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.heliocentricToolStripMenuItem.Text = "Heliocentric";
            this.heliocentricToolStripMenuItem.CheckedChanged += new System.EventHandler(this.heliocentricToolStripMenuItem_CheckedChanged);
            // 
            // menuYAxisMode
            // 
            this.menuYAxisMode.Name = "menuYAxisMode";
            this.menuYAxisMode.Size = new System.Drawing.Size(161, 22);
            this.menuYAxisMode.Text = "YAxis Auto";
            this.menuYAxisMode.Click += new System.EventHandler(this.menuYAxisMode_Click);
            // 
            // periodTypeToolStripMenuItem
            // 
            this.periodTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxPeriodType});
            this.periodTypeToolStripMenuItem.Name = "periodTypeToolStripMenuItem";
            this.periodTypeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.periodTypeToolStripMenuItem.Text = "Period QuoteType";
            // 
            // toolStripComboBoxPeriodType
            // 
            this.toolStripComboBoxPeriodType.Name = "toolStripComboBoxPeriodType";
            this.toolStripComboBoxPeriodType.Size = new System.Drawing.Size(121, 21);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.longitudeStatus,
            this.eventsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 722);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1270, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // longitudeStatus
            // 
            this.longitudeStatus.AutoSize = false;
            this.longitudeStatus.Font = new System.Drawing.Font("AstroSymbols", 9F);
            this.longitudeStatus.Name = "longitudeStatus";
            this.longitudeStatus.Size = new System.Drawing.Size(500, 17);
            this.longitudeStatus.Text = "longDif";
            this.longitudeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.longitudeStatus.ToolTipText = "Longitudes of Luminaries";
            // 
            // eventsStatus
            // 
            this.eventsStatus.AutoSize = false;
            this.eventsStatus.Font = new System.Drawing.Font("AstroSymbols", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventsStatus.Name = "eventsStatus";
            this.eventsStatus.Size = new System.Drawing.Size(750, 17);
            this.eventsStatus.Text = "events";
            this.eventsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.eventsStatus.ToolTipText = "Events List";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pageInterday);
            this.tabControl1.Controls.Add(this.pageIntraday);
            this.tabControl1.Controls.Add(this.pageAstrolabe);
            this.tabControl1.Controls.Add(this.pageCompare);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1270, 698);
            this.tabControl1.TabIndex = 5;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // pageInterday
            // 
            this.pageInterday.Controls.Add(this.zedLongTerm);
            this.pageInterday.Controls.Add(this.toolStrip1);
            this.pageInterday.Location = new System.Drawing.Point(4, 22);
            this.pageInterday.Name = "pageInterday";
            this.pageInterday.Padding = new System.Windows.Forms.Padding(3);
            this.pageInterday.Size = new System.Drawing.Size(1262, 672);
            this.pageInterday.TabIndex = 0;
            this.pageInterday.Text = "InterDay";
            this.pageInterday.UseVisualStyleBackColor = true;
            // 
            // zedLongTerm
            // 
            this.zedLongTerm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.zedLongTerm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedLongTerm.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zedLongTerm.IsEnableSelection = true;
            this.zedLongTerm.IsShowPointValues = true;
            this.zedLongTerm.Location = new System.Drawing.Point(3, 28);
            this.zedLongTerm.Name = "zedLongTerm";
            this.zedLongTerm.ScrollGrace = 0;
            this.zedLongTerm.ScrollMaxX = 0;
            this.zedLongTerm.ScrollMaxY = 0;
            this.zedLongTerm.ScrollMaxY2 = 0;
            this.zedLongTerm.ScrollMinX = 0;
            this.zedLongTerm.ScrollMinY = 0;
            this.zedLongTerm.ScrollMinY2 = 0;
            this.zedLongTerm.Size = new System.Drawing.Size(1256, 641);
            this.zedLongTerm.TabIndex = 3;
            this.zedLongTerm.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedLongTerm_PointValueEvent);
            this.zedLongTerm.DoubleClickEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedLongTerm_DoubleClickEvent);
            this.zedLongTerm.ScrollDoneEvent += new ZedGraph.ZedGraphControl.ScrollDoneHandler(this.zedLongTerm_ScrollDoneEvent);
            this.zedLongTerm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.zedLongTerm_MouseMove);
            this.zedLongTerm.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedLongTerm_ZoomEvent);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Silver;
            this.toolStrip1.Font = new System.Drawing.Font("AstroSymbols", 12F, System.Drawing.FontStyle.Bold);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.comboRecordType,
            this.toolStripButtonCentric,
            this.toolStripSeparator6,
            this.btnHighlightEvents,
            this.btnShowLocked,
            this.toolStripSeparator10,
            this.comboOutlineThreshold,
            this.toolStripSeparator7,
            this.comboPriceToDegree,
            this.toolStripSeparator8});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1256, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip3";
            // 
            // toolStripButtonCentric
            // 
            this.toolStripButtonCentric.CheckOnClick = true;
            this.toolStripButtonCentric.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonCentric.Font = new System.Drawing.Font("AstroSymbols", 9F);
            this.toolStripButtonCentric.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCentric.Name = "toolStripButtonCentric";
            this.toolStripButtonCentric.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonCentric.Text = "Geo";
            this.toolStripButtonCentric.ToolTipText = "Click to Heliocentric";
            this.toolStripButtonCentric.Click += new System.EventHandler(this.toolStripButtonCentric_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // btnHighlightEvents
            // 
            this.btnHighlightEvents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHighlightEvents.Font = new System.Drawing.Font("AstroSymbols", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHighlightEvents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHighlightEvents.Name = "btnHighlightEvents";
            this.btnHighlightEvents.Size = new System.Drawing.Size(36, 22);
            this.btnHighlightEvents.Text = "Mark";
            this.btnHighlightEvents.ToolTipText = "Click LEFT to highlight recent concerned events, click RIGHT to clear them all";
            this.btnHighlightEvents.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnHighlightEvents_MouseUp);
            // 
            // btnShowLocked
            // 
            this.btnShowLocked.Checked = true;
            this.btnShowLocked.CheckOnClick = true;
            this.btnShowLocked.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnShowLocked.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowLocked.Font = new System.Drawing.Font("AstroSymbols", 9F);
            this.btnShowLocked.Image = ((System.Drawing.Image)(resources.GetObject("btnShowLocked.Image")));
            this.btnShowLocked.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowLocked.Name = "btnShowLocked";
            this.btnShowLocked.Size = new System.Drawing.Size(23, 22);
            this.btnShowLocked.Text = "#";
            this.btnShowLocked.ToolTipText = "Display/hide selected orbits";
            this.btnShowLocked.Click += new System.EventHandler(this.btnShowLocked_CheckedChanged);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // comboOutlineThreshold
            // 
            this.comboOutlineThreshold.AutoSize = false;
            this.comboOutlineThreshold.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.comboOutlineThreshold.Name = "comboOutlineThreshold";
            this.comboOutlineThreshold.Size = new System.Drawing.Size(30, 21);
            this.comboOutlineThreshold.ToolTipText = "Outline Threashold";
            this.comboOutlineThreshold.SelectedIndexChanged += new System.EventHandler(this.comboOutlineThreshold_SelectedIndexChanged);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // comboPriceToDegree
            // 
            this.comboPriceToDegree.AutoSize = false;
            this.comboPriceToDegree.AutoToolTip = true;
            this.comboPriceToDegree.Name = "comboPriceToDegree";
            this.comboPriceToDegree.Size = new System.Drawing.Size(70, 21);
            this.comboPriceToDegree.ToolTipText = "Price Unit per Degree";
            this.comboPriceToDegree.SelectedIndexChanged += new System.EventHandler(this.comboPriceToDegree_SelectedIndexChanged);
            this.comboPriceToDegree.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboPriceToDegree_KeyUp);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // pageIntraday
            // 
            this.pageIntraday.Controls.Add(this.zedShortTerm);
            this.pageIntraday.Controls.Add(this.toolStrip2);
            this.pageIntraday.Location = new System.Drawing.Point(4, 22);
            this.pageIntraday.Name = "pageIntraday";
            this.pageIntraday.Padding = new System.Windows.Forms.Padding(3);
            this.pageIntraday.Size = new System.Drawing.Size(1262, 672);
            this.pageIntraday.TabIndex = 1;
            this.pageIntraday.Text = "IntraDay";
            this.pageIntraday.UseVisualStyleBackColor = true;
            // 
            // zedShortTerm
            // 
            this.zedShortTerm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedShortTerm.IsShowPointValues = true;
            this.zedShortTerm.Location = new System.Drawing.Point(3, 3);
            this.zedShortTerm.Name = "zedShortTerm";
            this.zedShortTerm.ScrollGrace = 0;
            this.zedShortTerm.ScrollMaxX = 0;
            this.zedShortTerm.ScrollMaxY = 0;
            this.zedShortTerm.ScrollMaxY2 = 0;
            this.zedShortTerm.ScrollMinX = 0;
            this.zedShortTerm.ScrollMinY = 0;
            this.zedShortTerm.ScrollMinY2 = 0;
            this.zedShortTerm.Size = new System.Drawing.Size(1256, 666);
            this.zedShortTerm.TabIndex = 1;
            this.zedShortTerm.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedShortTerm_PointValueEvent);
            this.zedShortTerm.DoubleClickEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedShortTerm_DoubleClickEvent);
            this.zedShortTerm.ScrollDoneEvent += new ZedGraph.ZedGraphControl.ScrollDoneHandler(this.zedShortTerm_ScrollDoneEvent);
            this.zedShortTerm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.zedShortTerm_MouseMove);
            this.zedShortTerm.ZoomEvent += new ZedGraph.ZedGraphControl.ZoomEventHandler(this.zedShortTerm_ZoomEvent);
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.Color.Silver;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.comboIntradayThreshold,
            this.toolStripSeparator1,
            this.buttonClearOrbitSets,
            this.toolStripLabel2,
            this.comboY2Ratio,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.textY3Ratio,
            this.toolStripSeparator5,
            this.buttonCentric,
            this.comboPlanet,
            this.toolStripLabel8,
            this.textOffset});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1256, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 22);
            this.toolStripLabel1.Text = "Outline:";
            // 
            // comboIntradayThreshold
            // 
            this.comboIntradayThreshold.AutoSize = false;
            this.comboIntradayThreshold.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "15",
            "20"});
            this.comboIntradayThreshold.Name = "comboIntradayThreshold";
            this.comboIntradayThreshold.Size = new System.Drawing.Size(40, 21);
            this.comboIntradayThreshold.ToolTipText = "Set the threshold of outline";
            this.comboIntradayThreshold.SelectedIndexChanged += new System.EventHandler(this.comboIntradayThreshold_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonClearOrbitSets
            // 
            this.buttonClearOrbitSets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonClearOrbitSets.Image = ((System.Drawing.Image)(resources.GetObject("buttonClearOrbitSets.Image")));
            this.buttonClearOrbitSets.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClearOrbitSets.Name = "buttonClearOrbitSets";
            this.buttonClearOrbitSets.Size = new System.Drawing.Size(36, 22);
            this.buttonClearOrbitSets.Text = "Clear";
            this.buttonClearOrbitSets.ToolTipText = "Clear orbit sets";
            this.buttonClearOrbitSets.Click += new System.EventHandler(this.buttonClearOrbitSets_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel2.Text = "Y2 Ratio:";
            // 
            // comboY2Ratio
            // 
            this.comboY2Ratio.AutoSize = false;
            this.comboY2Ratio.Name = "comboY2Ratio";
            this.comboY2Ratio.Size = new System.Drawing.Size(60, 21);
            this.comboY2Ratio.ToolTipText = "Y2 Price to Degree Ratio";
            this.comboY2Ratio.SelectedIndexChanged += new System.EventHandler(this.comboY2Ratio_SelectedIndexChanged);
            this.comboY2Ratio.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboY2Ratio_KeyUp);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(51, 22);
            this.toolStripLabel3.Text = "Y3 Ratio:";
            // 
            // textY3Ratio
            // 
            this.textY3Ratio.AutoSize = false;
            this.textY3Ratio.Name = "textY3Ratio";
            this.textY3Ratio.Size = new System.Drawing.Size(50, 25);
            this.textY3Ratio.ToolTipText = "1/100 of Earth Rotation Ratio";
            this.textY3Ratio.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textY3Ratio_KeyUp);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonCentric
            // 
            this.buttonCentric.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCentric.Image = ((System.Drawing.Image)(resources.GetObject("buttonCentric.Image")));
            this.buttonCentric.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCentric.Name = "buttonCentric";
            this.buttonCentric.Size = new System.Drawing.Size(62, 22);
            this.buttonCentric.Text = "Geocentric";
            this.buttonCentric.ToolTipText = "To Heliocentric or Geocentric";
            this.buttonCentric.Click += new System.EventHandler(this.buttonCentric_Click);
            // 
            // comboPlanet
            // 
            this.comboPlanet.AutoSize = false;
            this.comboPlanet.Name = "comboPlanet";
            this.comboPlanet.Size = new System.Drawing.Size(80, 21);
            this.comboPlanet.ToolTipText = "Select the concerned planet";
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(42, 22);
            this.toolStripLabel8.Text = "Offset:";
            // 
            // textOffset
            // 
            this.textOffset.AutoSize = false;
            this.textOffset.Name = "textOffset";
            this.textOffset.Size = new System.Drawing.Size(40, 25);
            this.textOffset.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textOffset_KeyUp);
            // 
            // pageAstrolabe
            // 
            this.pageAstrolabe.Controls.Add(this.dataGridViewDifference);
            this.pageAstrolabe.Controls.Add(this.splitContainer1);
            this.pageAstrolabe.Controls.Add(this.toolStrip3);
            this.pageAstrolabe.Location = new System.Drawing.Point(4, 22);
            this.pageAstrolabe.Name = "pageAstrolabe";
            this.pageAstrolabe.Size = new System.Drawing.Size(1262, 672);
            this.pageAstrolabe.TabIndex = 2;
            this.pageAstrolabe.Text = "Astrolabe";
            this.pageAstrolabe.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDifference
            // 
            this.dataGridViewDifference.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDifference.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDifference.Location = new System.Drawing.Point(0, 598);
            this.dataGridViewDifference.Name = "dataGridViewDifference";
            this.dataGridViewDifference.Size = new System.Drawing.Size(1262, 74);
            this.dataGridViewDifference.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewHelioAstrolabe1);
            this.splitContainer1.Panel1.Controls.Add(this.textDate1Details);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewGeoAstrolabe1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewHelioAstrolabe2);
            this.splitContainer1.Panel2.Controls.Add(this.textDate2Details);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewGeoAstrolabe2);
            this.splitContainer1.Size = new System.Drawing.Size(1262, 573);
            this.splitContainer1.SplitterDistance = 648;
            this.splitContainer1.TabIndex = 2;
            // 
            // dataGridViewHelioAstrolabe1
            // 
            this.dataGridViewHelioAstrolabe1.AutoGenerateColumns = false;
            this.dataGridViewHelioAstrolabe1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.dataGridViewHelioAstrolabe1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHelioAstrolabe1.DataSource = this.bindingSourceHelioAstrolabe1;
            this.dataGridViewHelioAstrolabe1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewHelioAstrolabe1.Location = new System.Drawing.Point(0, 248);
            this.dataGridViewHelioAstrolabe1.Name = "dataGridViewHelioAstrolabe1";
            this.dataGridViewHelioAstrolabe1.Size = new System.Drawing.Size(648, 245);
            this.dataGridViewHelioAstrolabe1.TabIndex = 2;
            // 
            // textDate1Details
            // 
            this.textDate1Details.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textDate1Details.Location = new System.Drawing.Point(0, 497);
            this.textDate1Details.Multiline = true;
            this.textDate1Details.Name = "textDate1Details";
            this.textDate1Details.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDate1Details.Size = new System.Drawing.Size(648, 76);
            this.textDate1Details.TabIndex = 1;
            // 
            // dataGridViewGeoAstrolabe1
            // 
            this.dataGridViewGeoAstrolabe1.AutoGenerateColumns = false;
            this.dataGridViewGeoAstrolabe1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridViewGeoAstrolabe1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGeoAstrolabe1.DataSource = this.bindingSourceGeoAstrolabe1;
            this.dataGridViewGeoAstrolabe1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewGeoAstrolabe1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGeoAstrolabe1.Name = "dataGridViewGeoAstrolabe1";
            this.dataGridViewGeoAstrolabe1.Size = new System.Drawing.Size(648, 248);
            this.dataGridViewGeoAstrolabe1.TabIndex = 0;
            this.dataGridViewGeoAstrolabe1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewGeoAstrolabe_RowEnter);
            // 
            // dataGridViewHelioAstrolabe2
            // 
            this.dataGridViewHelioAstrolabe2.AutoGenerateColumns = false;
            this.dataGridViewHelioAstrolabe2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.dataGridViewHelioAstrolabe2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHelioAstrolabe2.DataSource = this.bindingSourceHelioAstrolabe2;
            this.dataGridViewHelioAstrolabe2.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewHelioAstrolabe2.Location = new System.Drawing.Point(0, 248);
            this.dataGridViewHelioAstrolabe2.Name = "dataGridViewHelioAstrolabe2";
            this.dataGridViewHelioAstrolabe2.Size = new System.Drawing.Size(610, 245);
            this.dataGridViewHelioAstrolabe2.TabIndex = 3;
            // 
            // textDate2Details
            // 
            this.textDate2Details.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textDate2Details.Location = new System.Drawing.Point(0, 497);
            this.textDate2Details.Multiline = true;
            this.textDate2Details.Name = "textDate2Details";
            this.textDate2Details.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDate2Details.Size = new System.Drawing.Size(610, 76);
            this.textDate2Details.TabIndex = 2;
            // 
            // dataGridViewGeoAstrolabe2
            // 
            this.dataGridViewGeoAstrolabe2.AutoGenerateColumns = false;
            this.dataGridViewGeoAstrolabe2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.dataGridViewGeoAstrolabe2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGeoAstrolabe2.DataSource = this.bindingSourceGeoAstrolabe2;
            this.dataGridViewGeoAstrolabe2.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewGeoAstrolabe2.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGeoAstrolabe2.Name = "dataGridViewGeoAstrolabe2";
            this.dataGridViewGeoAstrolabe2.Size = new System.Drawing.Size(610, 248);
            this.dataGridViewGeoAstrolabe2.TabIndex = 0;
            this.dataGridViewGeoAstrolabe2.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewHelioAstrolabe_RowEnter);
            // 
            // toolStrip3
            // 
            this.toolStrip3.BackColor = System.Drawing.Color.Silver;
            this.toolStrip3.CanOverflow = false;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonLastMonth,
            this.buttonLastDay,
            this.toolStripLabel4,
            this.textAstrolabeYear1,
            this.toolStripLabel5,
            this.textAstrolabeMonth1,
            this.toolStripLabel6,
            this.textAstrolabeDay1,
            this.toolStripLabel7,
            this.textAstrolabeTime1,
            this.buttonNextDay,
            this.buttonNextMonth,
            this.toolStripSeparator3,
            this.comboPeriodType,
            this.toolStripSeparator4,
            this.comboAspectImportance,
            this.toolStripSeparator9,
            this.toolStripSeparator11,
            this.toolStripSeparator12,
            this.toolStripSeparator13,
            this.toolStripLabel9,
            this.textAstrolabeYear2,
            this.toolStripLabel10,
            this.textAstrolabeMonth2,
            this.toolStripLabel11,
            this.textAstrolabeDay2,
            this.toolStripLabel12,
            this.textAstrolabeTime2});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(1262, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // buttonLastMonth
            // 
            this.buttonLastMonth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLastMonth.Image = global::Astroder.Properties.Resources.BuilderDialog_RemoveAll;
            this.buttonLastMonth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLastMonth.Name = "buttonLastMonth";
            this.buttonLastMonth.Size = new System.Drawing.Size(23, 22);
            this.buttonLastMonth.Text = "toolStripButton3";
            this.buttonLastMonth.ToolTipText = "1 month before";
            this.buttonLastMonth.Click += new System.EventHandler(this.buttonLastMonth_Click);
            // 
            // buttonLastDay
            // 
            this.buttonLastDay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLastDay.Image = global::Astroder.Properties.Resources.BuilderDialog_remove;
            this.buttonLastDay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLastDay.Name = "buttonLastDay";
            this.buttonLastDay.Size = new System.Drawing.Size(23, 22);
            this.buttonLastDay.Text = "toolStripButton1";
            this.buttonLastDay.ToolTipText = "1 day before";
            this.buttonLastDay.Click += new System.EventHandler(this.buttonLastDay_Click);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel4.Text = "Year:";
            // 
            // textAstrolabeYear1
            // 
            this.textAstrolabeYear1.Name = "textAstrolabeYear1";
            this.textAstrolabeYear1.Size = new System.Drawing.Size(50, 25);
            this.textAstrolabeYear1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate1_KeyUp);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(41, 22);
            this.toolStripLabel5.Text = "Month:";
            // 
            // textAstrolabeMonth1
            // 
            this.textAstrolabeMonth1.Name = "textAstrolabeMonth1";
            this.textAstrolabeMonth1.Size = new System.Drawing.Size(30, 25);
            this.textAstrolabeMonth1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate1_KeyUp);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabel6.Text = "Day:";
            // 
            // textAstrolabeDay1
            // 
            this.textAstrolabeDay1.Name = "textAstrolabeDay1";
            this.textAstrolabeDay1.Size = new System.Drawing.Size(30, 25);
            this.textAstrolabeDay1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate1_KeyUp);
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel7.Text = "Time:";
            // 
            // textAstrolabeTime1
            // 
            this.textAstrolabeTime1.Name = "textAstrolabeTime1";
            this.textAstrolabeTime1.Size = new System.Drawing.Size(60, 25);
            this.textAstrolabeTime1.Text = "00:00";
            this.textAstrolabeTime1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate1_KeyUp);
            // 
            // buttonNextDay
            // 
            this.buttonNextDay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextDay.Image = global::Astroder.Properties.Resources.BuilderDialog_add;
            this.buttonNextDay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextDay.Name = "buttonNextDay";
            this.buttonNextDay.Size = new System.Drawing.Size(23, 22);
            this.buttonNextDay.Text = "1 day later";
            this.buttonNextDay.Click += new System.EventHandler(this.buttonNextDay_Click);
            // 
            // buttonNextMonth
            // 
            this.buttonNextMonth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNextMonth.Image = global::Astroder.Properties.Resources.BuilderDialog_AddAll;
            this.buttonNextMonth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNextMonth.Name = "buttonNextMonth";
            this.buttonNextMonth.Size = new System.Drawing.Size(23, 22);
            this.buttonNextMonth.Text = "1 month later";
            this.buttonNextMonth.Click += new System.EventHandler(this.buttonNextMonth_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // comboPeriodType
            // 
            this.comboPeriodType.Name = "comboPeriodType";
            this.comboPeriodType.Size = new System.Drawing.Size(100, 25);
            this.comboPeriodType.SelectedIndexChanged += new System.EventHandler(this.comboPeriodType_SelectedIndexChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // comboAspectImportance
            // 
            this.comboAspectImportance.Name = "comboAspectImportance";
            this.comboAspectImportance.Size = new System.Drawing.Size(100, 25);
            this.comboAspectImportance.SelectedIndexChanged += new System.EventHandler(this.comboAspectImportance_SelectedIndexChanged);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel9.Text = "Year:";
            // 
            // textAstrolabeYear2
            // 
            this.textAstrolabeYear2.Name = "textAstrolabeYear2";
            this.textAstrolabeYear2.Size = new System.Drawing.Size(50, 25);
            this.textAstrolabeYear2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate2_KeyUp);
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(41, 22);
            this.toolStripLabel10.Text = "Month:";
            // 
            // textAstrolabeMonth2
            // 
            this.textAstrolabeMonth2.Name = "textAstrolabeMonth2";
            this.textAstrolabeMonth2.Size = new System.Drawing.Size(30, 25);
            this.textAstrolabeMonth2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate2_KeyUp);
            // 
            // toolStripLabel11
            // 
            this.toolStripLabel11.Name = "toolStripLabel11";
            this.toolStripLabel11.Size = new System.Drawing.Size(30, 22);
            this.toolStripLabel11.Text = "Day:";
            // 
            // textAstrolabeDay2
            // 
            this.textAstrolabeDay2.Name = "textAstrolabeDay2";
            this.textAstrolabeDay2.Size = new System.Drawing.Size(30, 25);
            this.textAstrolabeDay2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate2_KeyUp);
            // 
            // toolStripLabel12
            // 
            this.toolStripLabel12.Name = "toolStripLabel12";
            this.toolStripLabel12.Size = new System.Drawing.Size(33, 22);
            this.toolStripLabel12.Text = "Time:";
            // 
            // textAstrolabeTime2
            // 
            this.textAstrolabeTime2.Name = "textAstrolabeTime2";
            this.textAstrolabeTime2.Size = new System.Drawing.Size(60, 25);
            this.textAstrolabeTime2.Text = "00:00";
            this.textAstrolabeTime2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.astrolabeDate2_KeyUp);
            // 
            // pageCompare
            // 
            this.pageCompare.Controls.Add(this.textClues);
            this.pageCompare.Location = new System.Drawing.Point(4, 22);
            this.pageCompare.Name = "pageCompare";
            this.pageCompare.Size = new System.Drawing.Size(1262, 672);
            this.pageCompare.TabIndex = 3;
            this.pageCompare.Text = "Compare";
            this.pageCompare.UseVisualStyleBackColor = true;
            // 
            // textClues
            // 
            this.textClues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textClues.Location = new System.Drawing.Point(0, 0);
            this.textClues.Multiline = true;
            this.textClues.Name = "textClues";
            this.textClues.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textClues.Size = new System.Drawing.Size(1262, 672);
            this.textClues.TabIndex = 0;
            // 
            // comboRecordType
            // 
            this.comboRecordType.AutoSize = false;
            this.comboRecordType.Name = "comboRecordType";
            this.comboRecordType.Size = new System.Drawing.Size(50, 21);
            this.comboRecordType.SelectedIndexChanged += new System.EventHandler(this.comboRecordType_SelectedIndexChanged);
            // 
            // AstroderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1270, 744);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AstroderForm";
            this.Text = "Astroder";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AstroderForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.pageInterday.ResumeLayout(false);
            this.pageInterday.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pageIntraday.ResumeLayout(false);
            this.pageIntraday.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.pageAstrolabe.ResumeLayout(false);
            this.pageAstrolabe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDifference)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHelioAstrolabe1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHelioAstrolabe1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeoAstrolabe1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGeoAstrolabe1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHelioAstrolabe2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceHelioAstrolabe2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeoAstrolabe2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceGeoAstrolabe2)).EndInit();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.pageCompare.ResumeLayout(false);
            this.pageCompare.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPoboToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heliocentricToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLegendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showKLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOutlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mousePositionToolStripMenuItem;
        private System.Windows.Forms.ToolTip theToolTip;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel longitudeStatus;
        private System.Windows.Forms.ToolStripStatusLabel eventsStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageInterday;
        private ZedGraph.ZedGraphControl zedLongTerm;
        private System.Windows.Forms.TabPage pageIntraday;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox comboOutlineThreshold;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButtonCentric;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripComboBox comboPriceToDegree;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton btnHighlightEvents;
        private System.Windows.Forms.ToolStripButton btnShowLocked;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripMenuItem menuYAxisMode;
        private System.Windows.Forms.ToolStripMenuItem commandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMarksToolStripMenuItem;
        private ZedGraph.ZedGraphControl zedShortTerm;
        private System.Windows.Forms.ToolStripComboBox comboIntradayThreshold;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showTodayToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox comboY2Ratio;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripMenuItem periodTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxPeriodType;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem inputManuallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setPivotToolStripMenuItem;
        private System.Windows.Forms.TabPage pageAstrolabe;
        private System.Windows.Forms.BindingSource bindingSourceGeoAstrolabe1;
        private System.Windows.Forms.BindingSource bindingSourceHelioAstrolabe1;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeYear1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeMonth1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeDay1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeTime1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewGeoAstrolabe1;
        private System.Windows.Forms.DataGridView dataGridViewGeoAstrolabe2;
        private System.Windows.Forms.ToolStripComboBox comboPeriodType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripComboBox comboAspectImportance;
        private System.Windows.Forms.TextBox textDate1Details;
        private System.Windows.Forms.TextBox textDate2Details;
        private System.Windows.Forms.ToolStripButton buttonLastDay;
        private System.Windows.Forms.ToolStripButton buttonLastMonth;
        private System.Windows.Forms.ToolStripButton buttonNextDay;
        private System.Windows.Forms.ToolStripButton buttonNextMonth;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripComboBox comboPlanet;
        private System.Windows.Forms.ToolStripButton buttonCentric;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripTextBox textOffset;
        private System.Windows.Forms.ToolStripButton buttonClearOrbitSets;
        private System.Windows.Forms.ToolStripTextBox textY3Ratio;
        private System.Windows.Forms.DataGridView dataGridViewHelioAstrolabe1;
        private System.Windows.Forms.DataGridView dataGridViewHelioAstrolabe2;
        private System.Windows.Forms.DataGridView dataGridViewDifference;
        private System.Windows.Forms.BindingSource bindingSourceHelioAstrolabe2;
        private System.Windows.Forms.BindingSource bindingSourceGeoAstrolabe2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeYear2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeMonth2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel11;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeDay2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel12;
        private System.Windows.Forms.ToolStripTextBox textAstrolabeTime2;
        private System.Windows.Forms.TabPage pageCompare;
        private System.Windows.Forms.TextBox textClues;
        private System.Windows.Forms.ToolStripComboBox comboRecordType;
    }
}

