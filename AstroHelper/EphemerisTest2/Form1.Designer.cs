namespace EphemerisTest2
{
    partial class Form1
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
            this.ButtonTest1 = new System.Windows.Forms.Button();
            this.buttonTest2 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.checkBoxDirection = new System.Windows.Forms.CheckBox();
            this.buttonSolarEclipse = new System.Windows.Forms.Button();
            this.buttonLunarEclipse = new System.Windows.Forms.Button();
            this.buttonOccultation = new System.Windows.Forms.Button();
            this.comboBoxPlanets = new System.Windows.Forms.ComboBox();
            this.buttonAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonTest1
            // 
            this.ButtonTest1.Location = new System.Drawing.Point(77, 61);
            this.ButtonTest1.Name = "ButtonTest1";
            this.ButtonTest1.Size = new System.Drawing.Size(75, 23);
            this.ButtonTest1.TabIndex = 0;
            this.ButtonTest1.Text = "Test1";
            this.ButtonTest1.UseVisualStyleBackColor = true;
            this.ButtonTest1.Click += new System.EventHandler(this.ButtonTest1_Click);
            // 
            // buttonTest2
            // 
            this.buttonTest2.Location = new System.Drawing.Point(189, 61);
            this.buttonTest2.Name = "buttonTest2";
            this.buttonTest2.Size = new System.Drawing.Size(75, 23);
            this.buttonTest2.TabIndex = 0;
            this.buttonTest2.Text = "Test1";
            this.buttonTest2.UseVisualStyleBackColor = true;
            this.buttonTest2.Click += new System.EventHandler(this.buttonTest2_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(53, 114);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(128, 20);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // checkBoxDirection
            // 
            this.checkBoxDirection.AutoSize = true;
            this.checkBoxDirection.Location = new System.Drawing.Point(219, 116);
            this.checkBoxDirection.Name = "checkBoxDirection";
            this.checkBoxDirection.Size = new System.Drawing.Size(81, 17);
            this.checkBoxDirection.TabIndex = 2;
            this.checkBoxDirection.Text = "isBackward";
            this.checkBoxDirection.UseVisualStyleBackColor = true;
            this.checkBoxDirection.CheckedChanged += new System.EventHandler(this.checkBoxDirection_CheckedChanged);
            // 
            // buttonSolarEclipse
            // 
            this.buttonSolarEclipse.Location = new System.Drawing.Point(34, 224);
            this.buttonSolarEclipse.Name = "buttonSolarEclipse";
            this.buttonSolarEclipse.Size = new System.Drawing.Size(75, 23);
            this.buttonSolarEclipse.TabIndex = 3;
            this.buttonSolarEclipse.Text = "SolarEclipse";
            this.buttonSolarEclipse.UseVisualStyleBackColor = true;
            this.buttonSolarEclipse.Click += new System.EventHandler(this.buttonSolarEclipse_Click);
            // 
            // buttonLunarEclipse
            // 
            this.buttonLunarEclipse.Location = new System.Drawing.Point(147, 224);
            this.buttonLunarEclipse.Name = "buttonLunarEclipse";
            this.buttonLunarEclipse.Size = new System.Drawing.Size(75, 23);
            this.buttonLunarEclipse.TabIndex = 3;
            this.buttonLunarEclipse.Text = "LunarEclipse";
            this.buttonLunarEclipse.UseVisualStyleBackColor = true;
            this.buttonLunarEclipse.Click += new System.EventHandler(this.buttonLunarEclipse_Click);
            // 
            // buttonOccultation
            // 
            this.buttonOccultation.Location = new System.Drawing.Point(219, 172);
            this.buttonOccultation.Name = "buttonOccultation";
            this.buttonOccultation.Size = new System.Drawing.Size(75, 23);
            this.buttonOccultation.TabIndex = 3;
            this.buttonOccultation.Text = "Occultation";
            this.buttonOccultation.UseVisualStyleBackColor = true;
            this.buttonOccultation.Click += new System.EventHandler(this.buttonOccultation_Click);
            // 
            // comboBoxPlanets
            // 
            this.comboBoxPlanets.FormattingEnabled = true;
            this.comboBoxPlanets.Location = new System.Drawing.Point(53, 174);
            this.comboBoxPlanets.Name = "comboBoxPlanets";
            this.comboBoxPlanets.Size = new System.Drawing.Size(128, 21);
            this.comboBoxPlanets.TabIndex = 4;
            this.comboBoxPlanets.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlanets_SelectedIndexChanged);
            // 
            // buttonAll
            // 
            this.buttonAll.Location = new System.Drawing.Point(257, 224);
            this.buttonAll.Name = "buttonAll";
            this.buttonAll.Size = new System.Drawing.Size(75, 23);
            this.buttonAll.TabIndex = 3;
            this.buttonAll.Text = "All";
            this.buttonAll.UseVisualStyleBackColor = true;
            this.buttonAll.Click += new System.EventHandler(this.buttonAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 298);
            this.Controls.Add(this.comboBoxPlanets);
            this.Controls.Add(this.buttonOccultation);
            this.Controls.Add(this.buttonAll);
            this.Controls.Add(this.buttonLunarEclipse);
            this.Controls.Add(this.buttonSolarEclipse);
            this.Controls.Add(this.checkBoxDirection);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.buttonTest2);
            this.Controls.Add(this.ButtonTest1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonTest1;
        private System.Windows.Forms.Button buttonTest2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkBoxDirection;
        private System.Windows.Forms.Button buttonSolarEclipse;
        private System.Windows.Forms.Button buttonLunarEclipse;
        private System.Windows.Forms.Button buttonOccultation;
        private System.Windows.Forms.ComboBox comboBoxPlanets;
        private System.Windows.Forms.Button buttonAll;
    }
}

