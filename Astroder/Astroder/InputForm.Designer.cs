namespace Astroder
{
    partial class InputForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxYear = new System.Windows.Forms.TextBox();
            this.textBoxMonth = new System.Windows.Forms.TextBox();
            this.textBoxDay = new System.Windows.Forms.TextBox();
            this.textBoxOpen = new System.Windows.Forms.TextBox();
            this.textBoxHigh = new System.Windows.Forms.TextBox();
            this.textBoxLow = new System.Windows.Forms.TextBox();
            this.textBoxClose = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.comboBoxQuoteType = new System.Windows.Forms.ComboBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.dataGridViewQuotes = new System.Windows.Forms.DataGridView();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuotes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(284, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "QuoteType:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Time: (yyyy MM dd)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Open:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "High:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(251, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Low:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(299, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Close:";
            // 
            // textBoxYear
            // 
            this.textBoxYear.Location = new System.Drawing.Point(27, 75);
            this.textBoxYear.Name = "textBoxYear";
            this.textBoxYear.Size = new System.Drawing.Size(42, 20);
            this.textBoxYear.TabIndex = 1;
            // 
            // textBoxMonth
            // 
            this.textBoxMonth.Location = new System.Drawing.Point(75, 75);
            this.textBoxMonth.Name = "textBoxMonth";
            this.textBoxMonth.Size = new System.Drawing.Size(24, 20);
            this.textBoxMonth.TabIndex = 2;
            // 
            // textBoxDay
            // 
            this.textBoxDay.Location = new System.Drawing.Point(105, 75);
            this.textBoxDay.Name = "textBoxDay";
            this.textBoxDay.Size = new System.Drawing.Size(24, 20);
            this.textBoxDay.TabIndex = 3;
            // 
            // textBoxOpen
            // 
            this.textBoxOpen.Location = new System.Drawing.Point(145, 75);
            this.textBoxOpen.Name = "textBoxOpen";
            this.textBoxOpen.Size = new System.Drawing.Size(42, 20);
            this.textBoxOpen.TabIndex = 4;
            this.textBoxOpen.Leave += new System.EventHandler(this.textBoxOpen_Leave);
            // 
            // textBoxHigh
            // 
            this.textBoxHigh.Location = new System.Drawing.Point(194, 75);
            this.textBoxHigh.Name = "textBoxHigh";
            this.textBoxHigh.Size = new System.Drawing.Size(42, 20);
            this.textBoxHigh.TabIndex = 5;
            this.textBoxHigh.Leave += new System.EventHandler(this.textBoxHigh_Leave);
            // 
            // textBoxLow
            // 
            this.textBoxLow.Location = new System.Drawing.Point(244, 75);
            this.textBoxLow.Name = "textBoxLow";
            this.textBoxLow.Size = new System.Drawing.Size(42, 20);
            this.textBoxLow.TabIndex = 6;
            this.textBoxLow.Leave += new System.EventHandler(this.textBoxLow_Leave);
            // 
            // textBoxClose
            // 
            this.textBoxClose.Location = new System.Drawing.Point(296, 75);
            this.textBoxClose.Name = "textBoxClose";
            this.textBoxClose.Size = new System.Drawing.Size(42, 20);
            this.textBoxClose.TabIndex = 7;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(75, 28);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(156, 20);
            this.textBoxName.TabIndex = 100;
            // 
            // comboBoxQuoteType
            // 
            this.comboBoxQuoteType.FormattingEnabled = true;
            this.comboBoxQuoteType.Location = new System.Drawing.Point(353, 27);
            this.comboBoxQuoteType.Name = "comboBoxQuoteType";
            this.comboBoxQuoteType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxQuoteType.TabIndex = 101;
            this.comboBoxQuoteType.SelectedIndexChanged += new System.EventHandler(this.comboBoxQuoteType_SelectedIndexChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(353, 72);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(60, 23);
            this.buttonAdd.TabIndex = 8;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(419, 72);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(55, 23);
            this.buttonRemove.TabIndex = 9;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // dataGridViewQuotes
            // 
            this.dataGridViewQuotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewQuotes.Location = new System.Drawing.Point(27, 116);
            this.dataGridViewQuotes.Name = "dataGridViewQuotes";
            this.dataGridViewQuotes.Size = new System.Drawing.Size(447, 217);
            this.dataGridViewQuotes.TabIndex = 102;
            this.dataGridViewQuotes.DoubleClick += new System.EventHandler(this.dataGridViewQuotes_DoubleClick);
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(287, 339);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 200;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(399, 339);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 202;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(135, 339);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 201;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(27, 339);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 201;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 374);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.dataGridViewQuotes);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.comboBoxQuoteType);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxClose);
            this.Controls.Add(this.textBoxLow);
            this.Controls.Add(this.textBoxHigh);
            this.Controls.Add(this.textBoxOpen);
            this.Controls.Add(this.textBoxDay);
            this.Controls.Add(this.textBoxMonth);
            this.Controls.Add(this.textBoxYear);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "InputForm";
            this.Text = "InputForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuotes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxYear;
        private System.Windows.Forms.TextBox textBoxMonth;
        private System.Windows.Forms.TextBox textBoxDay;
        private System.Windows.Forms.TextBox textBoxOpen;
        private System.Windows.Forms.TextBox textBoxHigh;
        private System.Windows.Forms.TextBox textBoxLow;
        private System.Windows.Forms.TextBox textBoxClose;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.ComboBox comboBoxQuoteType;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.DataGridView dataGridViewQuotes;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
    }
}