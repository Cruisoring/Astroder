namespace HistryDataCollector
{
    partial class CollectorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.checkedListBoxMonths = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAllOn = new System.Windows.Forms.Button();
            this.buttonAllOff = new System.Windows.Forms.Button();
            this.checkBoxContinuous = new System.Windows.Forms.CheckBox();
            this.panelContinuous = new System.Windows.Forms.Panel();
            this.checkBoxShiftForward = new System.Windows.Forms.CheckBox();
            this.checkBoxShiftBackward = new System.Windows.Forms.CheckBox();
            this.checkBoxAsIs = new System.Windows.Forms.CheckBox();
            this.checkBoxSplice = new System.Windows.Forms.CheckBox();
            this.textBoxDate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonGo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxVolumeBased = new System.Windows.Forms.CheckBox();
            this.buttonLoadPobo = new System.Windows.Forms.Button();
            this.panelContinuous.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "*.txt";
            this.openFileDialog1.Filter = "Text file|*.txt|All File|*.*";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Select the raw text files...";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(105, 23);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(396, 20);
            this.textBoxPath.TabIndex = 1;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(512, 21);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Description:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(105, 66);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(532, 103);
            this.textBoxDescription.TabIndex = 4;
            // 
            // checkedListBoxMonths
            // 
            this.checkedListBoxMonths.FormattingEnabled = true;
            this.checkedListBoxMonths.Location = new System.Drawing.Point(104, 196);
            this.checkedListBoxMonths.Name = "checkedListBoxMonths";
            this.checkedListBoxMonths.ScrollAlwaysVisible = true;
            this.checkedListBoxMonths.Size = new System.Drawing.Size(179, 154);
            this.checkedListBoxMonths.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 196);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Months:";
            // 
            // buttonAllOn
            // 
            this.buttonAllOn.Location = new System.Drawing.Point(305, 196);
            this.buttonAllOn.Name = "buttonAllOn";
            this.buttonAllOn.Size = new System.Drawing.Size(56, 23);
            this.buttonAllOn.TabIndex = 7;
            this.buttonAllOn.Text = "All On";
            this.buttonAllOn.UseVisualStyleBackColor = true;
            this.buttonAllOn.Click += new System.EventHandler(this.buttonAllOn_Click);
            // 
            // buttonAllOff
            // 
            this.buttonAllOff.Location = new System.Drawing.Point(305, 236);
            this.buttonAllOff.Name = "buttonAllOff";
            this.buttonAllOff.Size = new System.Drawing.Size(56, 23);
            this.buttonAllOff.TabIndex = 7;
            this.buttonAllOff.Text = "All Off";
            this.buttonAllOff.UseVisualStyleBackColor = true;
            this.buttonAllOff.Click += new System.EventHandler(this.buttonAllOff_Click);
            // 
            // checkBoxContinuous
            // 
            this.checkBoxContinuous.AutoSize = true;
            this.checkBoxContinuous.Checked = true;
            this.checkBoxContinuous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxContinuous.Location = new System.Drawing.Point(403, 196);
            this.checkBoxContinuous.Name = "checkBoxContinuous";
            this.checkBoxContinuous.Size = new System.Drawing.Size(135, 17);
            this.checkBoxContinuous.TabIndex = 8;
            this.checkBoxContinuous.Text = "Continuous Description";
            this.checkBoxContinuous.UseVisualStyleBackColor = true;
            this.checkBoxContinuous.CheckedChanged += new System.EventHandler(this.checkBoxContinuous_CheckedChanged);
            // 
            // panelContinuous
            // 
            this.panelContinuous.Controls.Add(this.groupBox1);
            this.panelContinuous.Controls.Add(this.checkBoxShiftForward);
            this.panelContinuous.Controls.Add(this.checkBoxShiftBackward);
            this.panelContinuous.Controls.Add(this.checkBoxAsIs);
            this.panelContinuous.Location = new System.Drawing.Point(386, 220);
            this.panelContinuous.Name = "panelContinuous";
            this.panelContinuous.Size = new System.Drawing.Size(251, 124);
            this.panelContinuous.TabIndex = 9;
            // 
            // checkBoxShiftForward
            // 
            this.checkBoxShiftForward.AutoSize = true;
            this.checkBoxShiftForward.Checked = true;
            this.checkBoxShiftForward.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShiftForward.Location = new System.Drawing.Point(103, 104);
            this.checkBoxShiftForward.Name = "checkBoxShiftForward";
            this.checkBoxShiftForward.Size = new System.Drawing.Size(88, 17);
            this.checkBoxShiftForward.TabIndex = 3;
            this.checkBoxShiftForward.Text = "Shift Forward";
            this.checkBoxShiftForward.UseVisualStyleBackColor = true;
            // 
            // checkBoxShiftBackward
            // 
            this.checkBoxShiftBackward.AutoSize = true;
            this.checkBoxShiftBackward.Checked = true;
            this.checkBoxShiftBackward.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShiftBackward.Location = new System.Drawing.Point(103, 81);
            this.checkBoxShiftBackward.Name = "checkBoxShiftBackward";
            this.checkBoxShiftBackward.Size = new System.Drawing.Size(98, 17);
            this.checkBoxShiftBackward.TabIndex = 3;
            this.checkBoxShiftBackward.Text = "Shift Backward";
            this.checkBoxShiftBackward.UseVisualStyleBackColor = true;
            // 
            // checkBoxAsIs
            // 
            this.checkBoxAsIs.AutoSize = true;
            this.checkBoxAsIs.Location = new System.Drawing.Point(21, 81);
            this.checkBoxAsIs.Name = "checkBoxAsIs";
            this.checkBoxAsIs.Size = new System.Drawing.Size(49, 17);
            this.checkBoxAsIs.TabIndex = 3;
            this.checkBoxAsIs.Text = "As Is";
            this.checkBoxAsIs.UseVisualStyleBackColor = true;
            // 
            // checkBoxSplice
            // 
            this.checkBoxSplice.AutoSize = true;
            this.checkBoxSplice.Location = new System.Drawing.Point(18, 19);
            this.checkBoxSplice.Name = "checkBoxSplice";
            this.checkBoxSplice.Size = new System.Drawing.Size(83, 17);
            this.checkBoxSplice.TabIndex = 2;
            this.checkBoxSplice.Text = "Rollover on:";
            this.checkBoxSplice.UseVisualStyleBackColor = true;
            // 
            // textBoxDate
            // 
            this.textBoxDate.Location = new System.Drawing.Point(104, 16);
            this.textBoxDate.Name = "textBoxDate";
            this.textBoxDate.Size = new System.Drawing.Size(25, 20);
            this.textBoxDate.TabIndex = 1;
            this.textBoxDate.Text = "30";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "days in advance";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Location = new System.Drawing.Point(104, 377);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(431, 20);
            this.textBoxOutput.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 377);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Destination:";
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(562, 377);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 12;
            this.buttonGo.Text = "Go!";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxVolumeBased);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxDate);
            this.groupBox1.Controls.Add(this.checkBoxSplice);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 72);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select quotes by:";
            // 
            // checkBoxVolumeBased
            // 
            this.checkBoxVolumeBased.AutoSize = true;
            this.checkBoxVolumeBased.Checked = true;
            this.checkBoxVolumeBased.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVolumeBased.Location = new System.Drawing.Point(18, 43);
            this.checkBoxVolumeBased.Name = "checkBoxVolumeBased";
            this.checkBoxVolumeBased.Size = new System.Drawing.Size(94, 17);
            this.checkBoxVolumeBased.TabIndex = 3;
            this.checkBoxVolumeBased.Text = "Volume Based";
            this.checkBoxVolumeBased.UseVisualStyleBackColor = true;
            // 
            // buttonLoadPobo
            // 
            this.buttonLoadPobo.Location = new System.Drawing.Point(593, 21);
            this.buttonLoadPobo.Name = "buttonLoadPobo";
            this.buttonLoadPobo.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadPobo.TabIndex = 2;
            this.buttonLoadPobo.Text = "Pobo";
            this.buttonLoadPobo.UseVisualStyleBackColor = true;
            this.buttonLoadPobo.Click += new System.EventHandler(this.buttonLoadPobo_Click);
            // 
            // CollectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 430);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.panelContinuous);
            this.Controls.Add(this.checkBoxContinuous);
            this.Controls.Add(this.buttonAllOff);
            this.Controls.Add(this.buttonAllOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkedListBoxMonths);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonLoadPobo);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.label1);
            this.Name = "CollectorForm";
            this.Text = "Data Processor";
            this.panelContinuous.ResumeLayout(false);
            this.panelContinuous.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.CheckedListBox checkedListBoxMonths;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAllOn;
        private System.Windows.Forms.Button buttonAllOff;
        private System.Windows.Forms.CheckBox checkBoxContinuous;
        private System.Windows.Forms.Panel panelContinuous;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.CheckBox checkBoxSplice;
        private System.Windows.Forms.TextBox textBoxDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxShiftForward;
        private System.Windows.Forms.CheckBox checkBoxShiftBackward;
        private System.Windows.Forms.CheckBox checkBoxAsIs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxVolumeBased;
        private System.Windows.Forms.Button buttonLoadPobo;
    }
}

