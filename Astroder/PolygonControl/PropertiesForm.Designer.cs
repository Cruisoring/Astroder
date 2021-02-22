namespace PolygonControl
{
    partial class PropertiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCalculator = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownMaxCycle = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownUnitSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxOrientation = new System.Windows.Forms.ComboBox();
            this.buttonCancle = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxCycle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUnitSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Shape:";
            // 
            // comboBoxCalculator
            // 
            this.comboBoxCalculator.FormattingEnabled = true;
            this.comboBoxCalculator.Location = new System.Drawing.Point(75, 29);
            this.comboBoxCalculator.Name = "comboBoxCalculator";
            this.comboBoxCalculator.Size = new System.Drawing.Size(144, 21);
            this.comboBoxCalculator.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MaxCycle:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(152, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "UnitSize:";
            // 
            // numericUpDownMaxCycle
            // 
            this.numericUpDownMaxCycle.Location = new System.Drawing.Point(75, 66);
            this.numericUpDownMaxCycle.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxCycle.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownMaxCycle.Name = "numericUpDownMaxCycle";
            this.numericUpDownMaxCycle.Size = new System.Drawing.Size(70, 20);
            this.numericUpDownMaxCycle.TabIndex = 3;
            this.numericUpDownMaxCycle.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericUpDownUnitSize
            // 
            this.numericUpDownUnitSize.Location = new System.Drawing.Point(207, 66);
            this.numericUpDownUnitSize.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownUnitSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownUnitSize.Name = "numericUpDownUnitSize";
            this.numericUpDownUnitSize.Size = new System.Drawing.Size(70, 20);
            this.numericUpDownUnitSize.TabIndex = 3;
            this.numericUpDownUnitSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Orientation:";
            // 
            // comboBoxOrientation
            // 
            this.comboBoxOrientation.FormattingEnabled = true;
            this.comboBoxOrientation.Location = new System.Drawing.Point(327, 29);
            this.comboBoxOrientation.Name = "comboBoxOrientation";
            this.comboBoxOrientation.Size = new System.Drawing.Size(121, 21);
            this.comboBoxOrientation.TabIndex = 4;
            // 
            // buttonCancle
            // 
            this.buttonCancle.Location = new System.Drawing.Point(111, 148);
            this.buttonCancle.Name = "buttonCancle";
            this.buttonCancle.Size = new System.Drawing.Size(75, 23);
            this.buttonCancle.TabIndex = 5;
            this.buttonCancle.Text = "Cancel";
            this.buttonCancle.UseVisualStyleBackColor = true;
            this.buttonCancle.Click += new System.EventHandler(this.buttonCancle_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(342, 148);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 215);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancle);
            this.Controls.Add(this.comboBoxOrientation);
            this.Controls.Add(this.numericUpDownUnitSize);
            this.Controls.Add(this.numericUpDownMaxCycle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxCalculator);
            this.Controls.Add(this.label1);
            this.Name = "PropertiesForm";
            this.Text = "Properties";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxCycle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUnitSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCalculator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxCycle;
        private System.Windows.Forms.NumericUpDown numericUpDownUnitSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxOrientation;
        private System.Windows.Forms.Button buttonCancle;
        private System.Windows.Forms.Button buttonOk;
    }
}