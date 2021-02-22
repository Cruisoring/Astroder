namespace AstroClock
{
    partial class FormClock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClock));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.polygon = new PolygonControl.PolygonControl();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // polygon
            // 
            this.polygon.Adapter = null;
            this.polygon.BackColor = System.Drawing.SystemColors.Info;
            this.polygon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("polygon.BackgroundImage")));
            this.polygon.Date = new System.DateTimeOffset(1, 1, 1, 0, 0, 0, 0, System.TimeSpan.Parse("00:00:00"));
            this.polygon.DisplayQuote = true;
            this.polygon.FirstQuadrant = PolygonControl.FirstQuadrantOrientation.RightUp;
            this.polygon.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.polygon.ForeColor = System.Drawing.Color.Transparent;
            this.polygon.History = null;
            this.polygon.Location = new System.Drawing.Point(0, 4);
            this.polygon.Margin = new System.Windows.Forms.Padding(4);
            this.polygon.Name = "polygon";
            this.polygon.QuoteRangs = ((System.Collections.Generic.Dictionary<System.DateTimeOffset, System.Drawing.Drawing2D.GraphicsPath>)(resources.GetObject("polygon.QuoteRangs")));
            this.polygon.Size = new System.Drawing.Size(1034, 1034);
            this.polygon.TabIndex = 0;
            this.polygon.UnitSize = 20;
            // 
            // FormClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 634);
            this.Controls.Add(this.polygon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormClock";
            this.Text = "AstroClock";
            this.ResumeLayout(false);

        }

        #endregion

        private PolygonControl.PolygonControl polygon;
        private System.Windows.Forms.Timer timer1;
    }
}

