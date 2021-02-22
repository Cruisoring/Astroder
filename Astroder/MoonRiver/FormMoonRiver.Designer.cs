namespace MoonRiver
{
    partial class FormMoonRiver
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMoonRiver));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeocentric = new System.Windows.Forms.TabPage();
            this.geoInterday = new ZedGraph.ZedGraphControl();
            this.tabGeoIntraday = new System.Windows.Forms.TabPage();
            this.geoIntraDay = new ZedGraph.ZedGraphControl();
            this.tabPageEvents = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabHeliocentric = new System.Windows.Forms.TabPage();
            this.tabHelioIntraday = new System.Windows.Forms.TabPage();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.astroCalendar4 = new AstroCalendarControl.AstroCalendar();
            this.astroCalendar3 = new AstroCalendarControl.AstroCalendar();
            this.astroCalendar2 = new AstroCalendarControl.AstroCalendar();
            this.astroCalendar1 = new AstroCalendarControl.AstroCalendar();
            this.tabControl.SuspendLayout();
            this.tabGeocentric.SuspendLayout();
            this.tabGeoIntraday.SuspendLayout();
            this.tabPageEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabGeocentric);
            this.tabControl.Controls.Add(this.tabGeoIntraday);
            this.tabControl.Controls.Add(this.tabPageEvents);
            this.tabControl.Controls.Add(this.tabHeliocentric);
            this.tabControl.Controls.Add(this.tabHelioIntraday);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1227, 778);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeocentric
            // 
            this.tabGeocentric.Controls.Add(this.geoInterday);
            this.tabGeocentric.Location = new System.Drawing.Point(4, 22);
            this.tabGeocentric.Name = "tabGeocentric";
            this.tabGeocentric.Size = new System.Drawing.Size(1219, 752);
            this.tabGeocentric.TabIndex = 3;
            this.tabGeocentric.Text = "Geocentric Calendar";
            this.tabGeocentric.UseVisualStyleBackColor = true;
            // 
            // geoInterday
            // 
            this.geoInterday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geoInterday.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.geoInterday.Location = new System.Drawing.Point(0, 0);
            this.geoInterday.Name = "geoInterday";
            this.geoInterday.ScrollGrace = 0;
            this.geoInterday.ScrollMaxX = 0;
            this.geoInterday.ScrollMaxY = 0;
            this.geoInterday.ScrollMaxY2 = 0;
            this.geoInterday.ScrollMinX = 0;
            this.geoInterday.ScrollMinY = 0;
            this.geoInterday.ScrollMinY2 = 0;
            this.geoInterday.Size = new System.Drawing.Size(1219, 752);
            this.geoInterday.TabIndex = 0;
            // 
            // tabGeoIntraday
            // 
            this.tabGeoIntraday.Controls.Add(this.geoIntraDay);
            this.tabGeoIntraday.Location = new System.Drawing.Point(4, 22);
            this.tabGeoIntraday.Name = "tabGeoIntraday";
            this.tabGeoIntraday.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeoIntraday.Size = new System.Drawing.Size(1219, 752);
            this.tabGeoIntraday.TabIndex = 0;
            this.tabGeoIntraday.Text = "Geocentric Intraday";
            this.tabGeoIntraday.UseVisualStyleBackColor = true;
            // 
            // geoIntraDay
            // 
            this.geoIntraDay.AutoSize = true;
            this.geoIntraDay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.geoIntraDay.Cursor = System.Windows.Forms.Cursors.Cross;
            this.geoIntraDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geoIntraDay.Font = new System.Drawing.Font("AstroSymbols", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.geoIntraDay.IsAutoScrollRange = true;
            this.geoIntraDay.IsShowPointValues = true;
            this.geoIntraDay.Location = new System.Drawing.Point(3, 3);
            this.geoIntraDay.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.geoIntraDay.Name = "geoIntraDay";
            this.geoIntraDay.ScrollGrace = 0;
            this.geoIntraDay.ScrollMaxX = 0;
            this.geoIntraDay.ScrollMaxY = 0;
            this.geoIntraDay.ScrollMaxY2 = 0;
            this.geoIntraDay.ScrollMinX = 0;
            this.geoIntraDay.ScrollMinY = 0;
            this.geoIntraDay.ScrollMinY2 = 0;
            this.geoIntraDay.Size = new System.Drawing.Size(1213, 746);
            this.geoIntraDay.TabIndex = 0;
            this.geoIntraDay.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.graphGeocentric_PointValueEvent);
            // 
            // tabPageEvents
            // 
            this.tabPageEvents.Controls.Add(this.label5);
            this.tabPageEvents.Controls.Add(this.label3);
            this.tabPageEvents.Controls.Add(this.label4);
            this.tabPageEvents.Controls.Add(this.label2);
            this.tabPageEvents.Controls.Add(this.label1);
            this.tabPageEvents.Controls.Add(this.astroCalendar4);
            this.tabPageEvents.Controls.Add(this.astroCalendar3);
            this.tabPageEvents.Controls.Add(this.astroCalendar2);
            this.tabPageEvents.Controls.Add(this.astroCalendar1);
            this.tabPageEvents.Location = new System.Drawing.Point(4, 22);
            this.tabPageEvents.Name = "tabPageEvents";
            this.tabPageEvents.Size = new System.Drawing.Size(1219, 752);
            this.tabPageEvents.TabIndex = 2;
            this.tabPageEvents.Text = "Events";
            this.tabPageEvents.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(676, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Fourth:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(451, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Third:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(226, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Second:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "First:";
            // 
            // tabHeliocentric
            // 
            this.tabHeliocentric.Location = new System.Drawing.Point(4, 22);
            this.tabHeliocentric.Name = "tabHeliocentric";
            this.tabHeliocentric.Size = new System.Drawing.Size(1219, 752);
            this.tabHeliocentric.TabIndex = 4;
            this.tabHeliocentric.Text = "Heliocentric Calendar";
            this.tabHeliocentric.UseVisualStyleBackColor = true;
            // 
            // tabHelioIntraday
            // 
            this.tabHelioIntraday.Location = new System.Drawing.Point(4, 22);
            this.tabHelioIntraday.Name = "tabHelioIntraday";
            this.tabHelioIntraday.Padding = new System.Windows.Forms.Padding(3);
            this.tabHelioIntraday.Size = new System.Drawing.Size(1219, 752);
            this.tabHelioIntraday.TabIndex = 1;
            this.tabHelioIntraday.Text = "Heliocentric Intraday";
            this.tabHelioIntraday.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // astroCalendar4
            // 
            this.astroCalendar4.Enabled = false;
            this.astroCalendar4.Location = new System.Drawing.Point(679, 31);
            this.astroCalendar4.Name = "astroCalendar4";
            this.astroCalendar4.PeriodEventsCollectedEvent = null;
            this.astroCalendar4.Size = new System.Drawing.Size(219, 848);
            this.astroCalendar4.TabIndex = 0;
            // 
            // astroCalendar3
            // 
            this.astroCalendar3.Enabled = false;
            this.astroCalendar3.Location = new System.Drawing.Point(454, 31);
            this.astroCalendar3.Name = "astroCalendar3";
            this.astroCalendar3.PeriodEventsCollectedEvent = null;
            this.astroCalendar3.Size = new System.Drawing.Size(219, 735);
            this.astroCalendar3.TabIndex = 0;
            // 
            // astroCalendar2
            // 
            this.astroCalendar2.Enabled = false;
            this.astroCalendar2.Location = new System.Drawing.Point(229, 31);
            this.astroCalendar2.Name = "astroCalendar2";
            this.astroCalendar2.PeriodEventsCollectedEvent = null;
            this.astroCalendar2.Size = new System.Drawing.Size(219, 637);
            this.astroCalendar2.TabIndex = 0;
            // 
            // astroCalendar1
            // 
            this.astroCalendar1.Location = new System.Drawing.Point(4, 31);
            this.astroCalendar1.Name = "astroCalendar1";
            this.astroCalendar1.PeriodEventsCollectedEvent = null;
            this.astroCalendar1.Size = new System.Drawing.Size(219, 552);
            this.astroCalendar1.TabIndex = 0;
            // 
            // FormMoonRiver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 778);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMoonRiver";
            this.Text = "Moon River";
            this.tabControl.ResumeLayout(false);
            this.tabGeocentric.ResumeLayout(false);
            this.tabGeoIntraday.ResumeLayout(false);
            this.tabGeoIntraday.PerformLayout();
            this.tabPageEvents.ResumeLayout(false);
            this.tabPageEvents.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeoIntraday;
        private System.Windows.Forms.TabPage tabHelioIntraday;
        private ZedGraph.ZedGraphControl geoIntraDay;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabPageEvents;
        private AstroCalendarControl.AstroCalendar astroCalendar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private AstroCalendarControl.AstroCalendar astroCalendar3;
        private AstroCalendarControl.AstroCalendar astroCalendar2;
        private System.Windows.Forms.Label label5;
        private AstroCalendarControl.AstroCalendar astroCalendar4;
        private System.Windows.Forms.TabPage tabGeocentric;
        private System.Windows.Forms.TabPage tabHeliocentric;
        private ZedGraph.ZedGraphControl geoInterday;
    }
}

