namespace EphemerisTest
{
    partial class TestForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonImportTxt = new System.Windows.Forms.Button();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonGo = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagePositions = new System.Windows.Forms.TabPage();
            this.dataGridViewPositions = new System.Windows.Forms.DataGridView();
            this.tabPageRelations = new System.Windows.Forms.TabPage();
            this.dataGridViewRelations = new System.Windows.Forms.DataGridView();
            this.tabPageQuotes = new System.Windows.Forms.TabPage();
            this.dataGridViewQuotes = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openTxtFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonTest1 = new System.Windows.Forms.Button();
            this.buttonTest2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPagePositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPositions)).BeginInit();
            this.tabPageRelations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRelations)).BeginInit();
            this.tabPageQuotes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuotes)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonTest2);
            this.panel1.Controls.Add(this.buttonTest1);
            this.panel1.Controls.Add(this.buttonImportTxt);
            this.panel1.Controls.Add(this.comboBoxMode);
            this.panel1.Controls.Add(this.buttonNext);
            this.panel1.Controls.Add(this.buttonPrevious);
            this.panel1.Controls.Add(this.buttonGo);
            this.panel1.Controls.Add(this.dateTimePicker1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(911, 37);
            this.panel1.TabIndex = 0;
            // 
            // buttonImportTxt
            // 
            this.buttonImportTxt.Location = new System.Drawing.Point(570, 4);
            this.buttonImportTxt.Name = "buttonImportTxt";
            this.buttonImportTxt.Size = new System.Drawing.Size(75, 23);
            this.buttonImportTxt.TabIndex = 4;
            this.buttonImportTxt.Text = "Import...";
            this.buttonImportTxt.UseVisualStyleBackColor = true;
            this.buttonImportTxt.Click += new System.EventHandler(this.buttonImportTxt_Click);
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(207, 3);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(146, 21);
            this.comboBoxMode.TabIndex = 3;
            this.comboBoxMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxMode_SelectedIndexChanged);
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonNext.Location = new System.Drawing.Point(464, 4);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(43, 20);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = ">";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonPrevious.Location = new System.Drawing.Point(415, 4);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(43, 20);
            this.buttonPrevious.TabIndex = 2;
            this.buttonPrevious.Text = "<";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(651, 3);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(68, 25);
            this.buttonGo.TabIndex = 1;
            this.buttonGo.Text = "Go !";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(13, 4);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(162, 20);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPagePositions);
            this.tabControl1.Controls.Add(this.tabPageRelations);
            this.tabControl1.Controls.Add(this.tabPageQuotes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 61);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(911, 376);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPagePositions
            // 
            this.tabPagePositions.Controls.Add(this.dataGridViewPositions);
            this.tabPagePositions.Location = new System.Drawing.Point(4, 22);
            this.tabPagePositions.Name = "tabPagePositions";
            this.tabPagePositions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePositions.Size = new System.Drawing.Size(903, 350);
            this.tabPagePositions.TabIndex = 0;
            this.tabPagePositions.Text = "Positions";
            this.tabPagePositions.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPositions
            // 
            this.dataGridViewPositions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewPositions.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPositions.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewPositions.Name = "dataGridViewPositions";
            this.dataGridViewPositions.Size = new System.Drawing.Size(897, 344);
            this.dataGridViewPositions.TabIndex = 0;
            // 
            // tabPageRelations
            // 
            this.tabPageRelations.Controls.Add(this.dataGridViewRelations);
            this.tabPageRelations.Location = new System.Drawing.Point(4, 22);
            this.tabPageRelations.Name = "tabPageRelations";
            this.tabPageRelations.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRelations.Size = new System.Drawing.Size(903, 350);
            this.tabPageRelations.TabIndex = 1;
            this.tabPageRelations.Text = "Relations";
            this.tabPageRelations.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRelations
            // 
            this.dataGridViewRelations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRelations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRelations.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewRelations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRelations.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewRelations.Name = "dataGridViewRelations";
            this.dataGridViewRelations.Size = new System.Drawing.Size(897, 344);
            this.dataGridViewRelations.TabIndex = 0;
            // 
            // tabPageQuotes
            // 
            this.tabPageQuotes.Controls.Add(this.dataGridViewQuotes);
            this.tabPageQuotes.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuotes.Name = "tabPageQuotes";
            this.tabPageQuotes.Size = new System.Drawing.Size(903, 350);
            this.tabPageQuotes.TabIndex = 2;
            this.tabPageQuotes.Text = "Quotes";
            this.tabPageQuotes.UseVisualStyleBackColor = true;
            // 
            // dataGridViewQuotes
            // 
            this.dataGridViewQuotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewQuotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewQuotes.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewQuotes.Name = "dataGridViewQuotes";
            this.dataGridViewQuotes.Size = new System.Drawing.Size(903, 350);
            this.dataGridViewQuotes.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(911, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openTxtFileDialog
            // 
            this.openTxtFileDialog.FileName = "*.txt";
            this.openTxtFileDialog.Filter = "Text file|*.txt|All file|*.*";
            this.openTxtFileDialog.Multiselect = true;
            this.openTxtFileDialog.RestoreDirectory = true;
            this.openTxtFileDialog.ShowReadOnly = true;
            this.openTxtFileDialog.Title = "Import stock quotes ...";
            // 
            // buttonTest1
            // 
            this.buttonTest1.Location = new System.Drawing.Point(752, 4);
            this.buttonTest1.Name = "buttonTest1";
            this.buttonTest1.Size = new System.Drawing.Size(75, 23);
            this.buttonTest1.TabIndex = 5;
            this.buttonTest1.Text = "Test1";
            this.buttonTest1.UseVisualStyleBackColor = true;
            this.buttonTest1.Click += new System.EventHandler(this.buttonTest1_Click);
            // 
            // buttonTest2
            // 
            this.buttonTest2.Location = new System.Drawing.Point(832, 4);
            this.buttonTest2.Name = "buttonTest2";
            this.buttonTest2.Size = new System.Drawing.Size(75, 23);
            this.buttonTest2.TabIndex = 5;
            this.buttonTest2.Text = "Test2";
            this.buttonTest2.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(911, 437);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPagePositions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPositions)).EndInit();
            this.tabPageRelations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRelations)).EndInit();
            this.tabPageQuotes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuotes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagePositions;
        private System.Windows.Forms.TabPage tabPageRelations;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DataGridView dataGridViewPositions;
        private System.Windows.Forms.DataGridView dataGridViewRelations;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.OpenFileDialog openTxtFileDialog;
        private System.Windows.Forms.Button buttonImportTxt;
        private System.Windows.Forms.TabPage tabPageQuotes;
        private System.Windows.Forms.DataGridView dataGridViewQuotes;
        private System.Windows.Forms.Button buttonTest2;
        private System.Windows.Forms.Button buttonTest1;
    }
}

