namespace AstroTest
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
            this.dateTimeRough = new System.Windows.Forms.DateTimePicker();
            this.comboBoxInterior = new System.Windows.Forms.ComboBox();
            this.comboBoxExterior = new System.Windows.Forms.ComboBox();
            this.comboBoxAspect = new System.Windows.Forms.ComboBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeRough
            // 
            this.dateTimeRough.Location = new System.Drawing.Point(34, 29);
            this.dateTimeRough.Name = "dateTimeRough";
            this.dateTimeRough.Size = new System.Drawing.Size(166, 20);
            this.dateTimeRough.TabIndex = 0;
            this.dateTimeRough.ValueChanged += new System.EventHandler(this.dateTimeRough_ValueChanged);
            // 
            // comboBoxInterior
            // 
            this.comboBoxInterior.FormattingEnabled = true;
            this.comboBoxInterior.Location = new System.Drawing.Point(264, 28);
            this.comboBoxInterior.Name = "comboBoxInterior";
            this.comboBoxInterior.Size = new System.Drawing.Size(121, 21);
            this.comboBoxInterior.TabIndex = 1;
            this.comboBoxInterior.SelectedIndexChanged += new System.EventHandler(this.comboBoxInterior_SelectedIndexChanged);
            // 
            // comboBoxExterior
            // 
            this.comboBoxExterior.FormattingEnabled = true;
            this.comboBoxExterior.Location = new System.Drawing.Point(407, 29);
            this.comboBoxExterior.Name = "comboBoxExterior";
            this.comboBoxExterior.Size = new System.Drawing.Size(110, 21);
            this.comboBoxExterior.TabIndex = 1;
            this.comboBoxExterior.SelectedIndexChanged += new System.EventHandler(this.comboBoxExterior_SelectedIndexChanged);
            // 
            // comboBoxAspect
            // 
            this.comboBoxAspect.FormattingEnabled = true;
            this.comboBoxAspect.Location = new System.Drawing.Point(541, 28);
            this.comboBoxAspect.Name = "comboBoxAspect";
            this.comboBoxAspect.Size = new System.Drawing.Size(95, 21);
            this.comboBoxAspect.TabIndex = 2;
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(675, 26);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 3;
            this.buttonGo.Text = "Go !";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Rough UTC time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Interiror:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(414, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Exterior:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(538, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Aspect:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(37, 84);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(713, 268);
            this.dataGridView1.TabIndex = 5;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(38, 382);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(711, 294);
            this.dataGridView2.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 706);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.comboBoxAspect);
            this.Controls.Add(this.comboBoxExterior);
            this.Controls.Add(this.comboBoxInterior);
            this.Controls.Add(this.dateTimeRough);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeRough;
        private System.Windows.Forms.ComboBox comboBoxInterior;
        private System.Windows.Forms.ComboBox comboBoxExterior;
        private System.Windows.Forms.ComboBox comboBoxAspect;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
    }
}

