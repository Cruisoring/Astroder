namespace AstroTeller
{
    partial class QuoteManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param degreeString="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.importQuoteFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelNew = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.panelInformation = new System.Windows.Forms.Panel();
            this.textBoxUntil = new System.Windows.Forms.TextBox();
            this.textBoxSince = new System.Windows.Forms.TextBox();
            this.textBoxLowest = new System.Windows.Forms.TextBox();
            this.textBoxHighest = new System.Windows.Forms.TextBox();
            this.textBoxCount = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelNew.SuspendLayout();
            this.panelInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // importQuoteFileDialog
            // 
            this.importQuoteFileDialog.FileName = "openFileDialog1";
            this.importQuoteFileDialog.Filter = "Text file|*.txt|Pobo file|*.da1|All file|*.*";
            this.importQuoteFileDialog.Multiselect = true;
            this.importQuoteFileDialog.Title = "Import DailyQuote Data ...";
            // 
            // panelNew
            // 
            this.panelNew.Controls.Add(this.label1);
            this.panelNew.Controls.Add(this.textBoxFileName);
            this.panelNew.Controls.Add(this.buttonSave);
            this.panelNew.Controls.Add(this.buttonLoad);
            this.panelNew.Controls.Add(this.buttonImport);
            this.panelNew.Location = new System.Drawing.Point(12, 12);
            this.panelNew.Name = "panelNew";
            this.panelNew.Size = new System.Drawing.Size(276, 108);
            this.panelNew.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "FileName:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileName.Location = new System.Drawing.Point(7, 29);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBoxFileName.Size = new System.Drawing.Size(261, 20);
            this.textBoxFileName.TabIndex = 1;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(199, 66);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(66, 29);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonLoad.AutoSize = true;
            this.buttonLoad.Location = new System.Drawing.Point(115, 66);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(63, 29);
            this.buttonLoad.TabIndex = 0;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonImport.AutoSize = true;
            this.buttonImport.Location = new System.Drawing.Point(7, 66);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(60, 29);
            this.buttonImport.TabIndex = 0;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // panelInformation
            // 
            this.panelInformation.Controls.Add(this.textBoxUntil);
            this.panelInformation.Controls.Add(this.textBoxSince);
            this.panelInformation.Controls.Add(this.textBoxLowest);
            this.panelInformation.Controls.Add(this.textBoxHighest);
            this.panelInformation.Controls.Add(this.textBoxCount);
            this.panelInformation.Controls.Add(this.textBoxDescription);
            this.panelInformation.Controls.Add(this.textBoxName);
            this.panelInformation.Controls.Add(this.label8);
            this.panelInformation.Controls.Add(this.label7);
            this.panelInformation.Controls.Add(this.label6);
            this.panelInformation.Controls.Add(this.label5);
            this.panelInformation.Controls.Add(this.label9);
            this.panelInformation.Controls.Add(this.label4);
            this.panelInformation.Controls.Add(this.label3);
            this.panelInformation.Controls.Add(this.label2);
            this.panelInformation.Location = new System.Drawing.Point(13, 128);
            this.panelInformation.Name = "panelInformation";
            this.panelInformation.Size = new System.Drawing.Size(275, 124);
            this.panelInformation.TabIndex = 1;
            this.panelInformation.Visible = false;
            // 
            // textBoxUntil
            // 
            this.textBoxUntil.Location = new System.Drawing.Point(180, 75);
            this.textBoxUntil.Name = "textBoxUntil";
            this.textBoxUntil.ReadOnly = true;
            this.textBoxUntil.Size = new System.Drawing.Size(76, 20);
            this.textBoxUntil.TabIndex = 2;
            // 
            // textBoxSince
            // 
            this.textBoxSince.Location = new System.Drawing.Point(48, 75);
            this.textBoxSince.Name = "textBoxSince";
            this.textBoxSince.ReadOnly = true;
            this.textBoxSince.Size = new System.Drawing.Size(85, 20);
            this.textBoxSince.TabIndex = 2;
            // 
            // textBoxLowest
            // 
            this.textBoxLowest.Location = new System.Drawing.Point(180, 49);
            this.textBoxLowest.Name = "textBoxLowest";
            this.textBoxLowest.ReadOnly = true;
            this.textBoxLowest.Size = new System.Drawing.Size(76, 20);
            this.textBoxLowest.TabIndex = 2;
            // 
            // textBoxHighest
            // 
            this.textBoxHighest.Location = new System.Drawing.Point(48, 49);
            this.textBoxHighest.Name = "textBoxHighest";
            this.textBoxHighest.ReadOnly = true;
            this.textBoxHighest.Size = new System.Drawing.Size(85, 20);
            this.textBoxHighest.TabIndex = 2;
            // 
            // textBoxCount
            // 
            this.textBoxCount.Location = new System.Drawing.Point(180, 23);
            this.textBoxCount.Name = "textBoxCount";
            this.textBoxCount.ReadOnly = true;
            this.textBoxCount.Size = new System.Drawing.Size(76, 20);
            this.textBoxCount.TabIndex = 2;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(65, 102);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(191, 20);
            this.textBoxDescription.TabIndex = 2;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(48, 23);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(85, 20);
            this.textBoxName.TabIndex = 2;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Until:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Since:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(139, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Zero:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Ceiling:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Description:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(139, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "FutureName:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "DailyQuote Information:";
            // 
            // QuoteManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.ClientSize = new System.Drawing.Size(295, 263);
            this.Controls.Add(this.panelInformation);
            this.Controls.Add(this.panelNew);
            this.Name = "QuoteManagerForm";
            this.Text = "QuoteManagerForm";
            this.panelNew.ResumeLayout(false);
            this.panelNew.PerformLayout();
            this.panelInformation.ResumeLayout(false);
            this.panelInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog importQuoteFileDialog;
        private System.Windows.Forms.Panel panelNew;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Panel panelInformation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxUntil;
        private System.Windows.Forms.TextBox textBoxSince;
        private System.Windows.Forms.TextBox textBoxLowest;
        private System.Windows.Forms.TextBox textBoxHighest;
        private System.Windows.Forms.TextBox textBoxCount;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label9;
    }
}