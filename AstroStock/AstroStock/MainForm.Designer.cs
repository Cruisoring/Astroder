namespace AstroStock
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.buttonCalc = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.comboBoxTimeZones = new System.Windows.Forms.ComboBox();
            this.checkBoxDST = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.astrolabeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPositions = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabRelations = new System.Windows.Forms.TabPage();
            this.textBoxRelations = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.astrolabeBindingSource)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabRelations.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(39, 42);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // buttonCalc
            // 
            this.buttonCalc.Location = new System.Drawing.Point(644, 19);
            this.buttonCalc.Name = "buttonCalc";
            this.buttonCalc.Size = new System.Drawing.Size(75, 23);
            this.buttonCalc.TabIndex = 1;
            this.buttonCalc.Text = "Calculate";
            this.buttonCalc.UseVisualStyleBackColor = true;
            this.buttonCalc.Click += new System.EventHandler(this.buttonCalc_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(460, 15);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(146, 20);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(460, 41);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(107, 20);
            this.textBox3.TabIndex = 2;
            // 
            // comboBoxTimeZones
            // 
            this.comboBoxTimeZones.FormattingEnabled = true;
            this.comboBoxTimeZones.Location = new System.Drawing.Point(39, 15);
            this.comboBoxTimeZones.Name = "comboBoxTimeZones";
            this.comboBoxTimeZones.Size = new System.Drawing.Size(344, 21);
            this.comboBoxTimeZones.TabIndex = 3;
            this.comboBoxTimeZones.SelectedIndexChanged += new System.EventHandler(this.comboBoxTimeZones_SelectedIndexChanged);
            // 
            // checkBoxDST
            // 
            this.checkBoxDST.AutoSize = true;
            this.checkBoxDST.Location = new System.Drawing.Point(268, 44);
            this.checkBoxDST.Name = "checkBoxDST";
            this.checkBoxDST.Size = new System.Drawing.Size(107, 17);
            this.checkBoxDST.TabIndex = 4;
            this.checkBoxDST.Text = "Day Time Saving";
            this.checkBoxDST.UseVisualStyleBackColor = true;
            this.checkBoxDST.CheckedChanged += new System.EventHandler(this.checkBoxDST_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(422, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "UTC:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Julian:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPositions);
            this.tabControl1.Controls.Add(this.tabRelations);
            this.tabControl1.Location = new System.Drawing.Point(28, 69);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(676, 403);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPositions
            // 
            this.tabPositions.Controls.Add(this.dataGridView1);
            this.tabPositions.Location = new System.Drawing.Point(4, 22);
            this.tabPositions.Name = "tabPositions";
            this.tabPositions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPositions.Size = new System.Drawing.Size(668, 377);
            this.tabPositions.TabIndex = 0;
            this.tabPositions.Text = "Positions";
            this.tabPositions.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.DataSource = this.astrolabeBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(665, 374);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabRelations
            // 
            this.tabRelations.Controls.Add(this.textBoxRelations);
            this.tabRelations.Location = new System.Drawing.Point(4, 22);
            this.tabRelations.Name = "tabRelations";
            this.tabRelations.Padding = new System.Windows.Forms.Padding(3);
            this.tabRelations.Size = new System.Drawing.Size(668, 377);
            this.tabRelations.TabIndex = 1;
            this.tabRelations.Text = "Relations";
            this.tabRelations.UseVisualStyleBackColor = true;
            // 
            // textBoxRelations
            // 
            this.textBoxRelations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRelations.Font = new System.Drawing.Font("StarFont Sans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxRelations.Location = new System.Drawing.Point(3, 3);
            this.textBoxRelations.Multiline = true;
            this.textBoxRelations.Name = "textBoxRelations";
            this.textBoxRelations.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxRelations.Size = new System.Drawing.Size(662, 371);
            this.textBoxRelations.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 498);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxDST);
            this.Controls.Add(this.comboBoxTimeZones);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.buttonCalc);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "MainForm";
            this.Text = "AstroStock";
            ((System.ComponentModel.ISupportInitialize)(this.astrolabeBindingSource)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPositions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabRelations.ResumeLayout(false);
            this.tabRelations.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button buttonCalc;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ComboBox comboBoxTimeZones;
        private System.Windows.Forms.CheckBox checkBoxDST;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn starPositionsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn momentDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource astrolabeBindingSource;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPositions;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabRelations;
        private System.Windows.Forms.TextBox textBoxRelations;
    }
}

