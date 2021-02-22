namespace AstroCalendarControl
{
    partial class AstroCalendar
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxPeriodType = new System.Windows.Forms.ComboBox();
            this.treeViewEvents = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxImportance = new System.Windows.Forms.ComboBox();
            this.textBoxDetail = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "yyyy-MM";
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(88, 57);
            this.dateTimePicker.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(134, 21);
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.Leave += new System.EventHandler(this.dateTimePicker_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 63);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Date:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Duration:";
            // 
            // comboBoxPeriodType
            // 
            this.comboBoxPeriodType.FormattingEnabled = true;
            this.comboBoxPeriodType.Location = new System.Drawing.Point(88, 9);
            this.comboBoxPeriodType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxPeriodType.Name = "comboBoxPeriodType";
            this.comboBoxPeriodType.Size = new System.Drawing.Size(134, 20);
            this.comboBoxPeriodType.TabIndex = 3;
            // 
            // treeViewEvents
            // 
            this.treeViewEvents.Font = new System.Drawing.Font("AstroSymbols", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewEvents.Location = new System.Drawing.Point(4, 81);
            this.treeViewEvents.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.treeViewEvents.Name = "treeViewEvents";
            this.treeViewEvents.Size = new System.Drawing.Size(248, 265);
            this.treeViewEvents.TabIndex = 5;
            this.treeViewEvents.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewEvents_AfterSelect);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Level:";
            // 
            // comboBoxImportance
            // 
            this.comboBoxImportance.FormattingEnabled = true;
            this.comboBoxImportance.Location = new System.Drawing.Point(88, 33);
            this.comboBoxImportance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBoxImportance.Name = "comboBoxImportance";
            this.comboBoxImportance.Size = new System.Drawing.Size(134, 20);
            this.comboBoxImportance.TabIndex = 3;
            // 
            // textBoxDetail
            // 
            this.textBoxDetail.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxDetail.Location = new System.Drawing.Point(4, 352);
            this.textBoxDetail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBoxDetail.Multiline = true;
            this.textBoxDetail.Name = "textBoxDetail";
            this.textBoxDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDetail.Size = new System.Drawing.Size(248, 76);
            this.textBoxDetail.TabIndex = 6;
            // 
            // AstroCalendar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxDetail);
            this.Controls.Add(this.treeViewEvents);
            this.Controls.Add(this.comboBoxImportance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxPeriodType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker);
            this.Font = new System.Drawing.Font("AstroSymbols", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AstroCalendar";
            this.Size = new System.Drawing.Size(256, 431);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxPeriodType;
        private System.Windows.Forms.TreeView treeViewEvents;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxImportance;
        private System.Windows.Forms.TextBox textBoxDetail;
    }
}
