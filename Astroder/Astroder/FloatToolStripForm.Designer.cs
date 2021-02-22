namespace Astroder
{
    partial class FloatToolStripForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonHeliocentric = new System.Windows.Forms.RadioButton();
            this.radioButtonGeocentric = new System.Windows.Forms.RadioButton();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.panelStars = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.checkBox12 = new System.Windows.Forms.CheckBox();
            this.checkBox13 = new System.Windows.Forms.CheckBox();
            this.panelAspects = new System.Windows.Forms.Panel();
            this.textBoxOffset = new System.Windows.Forms.TextBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonSet = new System.Windows.Forms.Button();
            this.checkBoxQuintile = new System.Windows.Forms.CheckBox();
            this.checkBoxOpposite = new System.Windows.Forms.CheckBox();
            this.checkBoxTrine = new System.Windows.Forms.CheckBox();
            this.checkBoxSquare = new System.Windows.Forms.CheckBox();
            this.checkBoxSextile = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.comboBoxFocused = new System.Windows.Forms.ComboBox();
            this.panelEvents = new System.Windows.Forms.Panel();
            this.comboBoxAnother = new System.Windows.Forms.ComboBox();
            this.comboBoxConcernedStar = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxImportance = new System.Windows.Forms.ComboBox();
            this.checkBoxAspect = new System.Windows.Forms.CheckBox();
            this.checkBoxDeclination = new System.Windows.Forms.CheckBox();
            this.checkBoxSigns = new System.Windows.Forms.CheckBox();
            this.checkBoxDirection = new System.Windows.Forms.CheckBox();
            this.checkBoxOccultations = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.panelStars.SuspendLayout();
            this.panelAspects.SuspendLayout();
            this.panelEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.radioButtonHeliocentric);
            this.groupBox2.Controls.Add(this.radioButtonGeocentric);
            this.groupBox2.Location = new System.Drawing.Point(1, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(163, 31);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonHeliocentric
            // 
            this.radioButtonHeliocentric.AutoSize = true;
            this.radioButtonHeliocentric.Location = new System.Drawing.Point(85, 10);
            this.radioButtonHeliocentric.Name = "radioButtonHeliocentric";
            this.radioButtonHeliocentric.Size = new System.Drawing.Size(80, 17);
            this.radioButtonHeliocentric.TabIndex = 0;
            this.radioButtonHeliocentric.Text = "Heliocentric";
            this.radioButtonHeliocentric.UseVisualStyleBackColor = true;
            // 
            // radioButtonGeocentric
            // 
            this.radioButtonGeocentric.AutoSize = true;
            this.radioButtonGeocentric.Checked = true;
            this.radioButtonGeocentric.Location = new System.Drawing.Point(2, 10);
            this.radioButtonGeocentric.Name = "radioButtonGeocentric";
            this.radioButtonGeocentric.Size = new System.Drawing.Size(76, 17);
            this.radioButtonGeocentric.TabIndex = 0;
            this.radioButtonGeocentric.TabStop = true;
            this.radioButtonGeocentric.Text = "Geocentric";
            this.radioButtonGeocentric.UseVisualStyleBackColor = true;
            this.radioButtonGeocentric.CheckedChanged += new System.EventHandler(this.radioButtonGeocentric_CheckedChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(3, 34);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(63, 20);
            this.dateTimePicker1.TabIndex = 9;
            this.dateTimePicker1.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Leave += new System.EventHandler(this.dateTimePicker1_Leave);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(99, 34);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(64, 20);
            this.dateTimePicker2.TabIndex = 8;
            this.dateTimePicker2.Value = new System.DateTime(2011, 5, 1, 0, 0, 0, 0);
            this.dateTimePicker2.Leave += new System.EventHandler(this.dateTimePicker1_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "To:";
            // 
            // panelStars
            // 
            this.panelStars.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStars.Controls.Add(this.checkBox1);
            this.panelStars.Controls.Add(this.checkBox2);
            this.panelStars.Controls.Add(this.checkBox3);
            this.panelStars.Controls.Add(this.checkBox4);
            this.panelStars.Controls.Add(this.checkBox5);
            this.panelStars.Controls.Add(this.checkBox6);
            this.panelStars.Controls.Add(this.checkBox7);
            this.panelStars.Controls.Add(this.checkBox8);
            this.panelStars.Controls.Add(this.checkBox9);
            this.panelStars.Controls.Add(this.checkBox10);
            this.panelStars.Controls.Add(this.checkBox11);
            this.panelStars.Controls.Add(this.checkBox12);
            this.panelStars.Controls.Add(this.checkBox13);
            this.panelStars.Location = new System.Drawing.Point(2, 57);
            this.panelStars.Name = "panelStars";
            this.panelStars.Size = new System.Drawing.Size(162, 54);
            this.panelStars.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(0, 0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(23, 23);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "X";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(23, 0);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(23, 23);
            this.checkBox2.TabIndex = 0;
            this.checkBox2.Text = "X";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(46, 0);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(23, 23);
            this.checkBox3.TabIndex = 0;
            this.checkBox3.Text = "X";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(68, 0);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(23, 23);
            this.checkBox4.TabIndex = 0;
            this.checkBox4.Text = "X";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(92, 0);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(23, 23);
            this.checkBox5.TabIndex = 0;
            this.checkBox5.Text = "X";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(114, 0);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(23, 23);
            this.checkBox6.TabIndex = 0;
            this.checkBox6.Text = "X";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox7
            // 
            this.checkBox7.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(136, 0);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(23, 23);
            this.checkBox7.TabIndex = 4;
            this.checkBox7.Text = "X";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox8
            // 
            this.checkBox8.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(0, 25);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(23, 23);
            this.checkBox8.TabIndex = 5;
            this.checkBox8.Text = "X";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox9
            // 
            this.checkBox9.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox9.AutoSize = true;
            this.checkBox9.Location = new System.Drawing.Point(24, 25);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(23, 23);
            this.checkBox9.TabIndex = 6;
            this.checkBox9.Text = "X";
            this.checkBox9.UseVisualStyleBackColor = true;
            this.checkBox9.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox10
            // 
            this.checkBox10.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(48, 25);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(23, 23);
            this.checkBox10.TabIndex = 1;
            this.checkBox10.Text = "X";
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox10.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox11
            // 
            this.checkBox11.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(92, 24);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(23, 23);
            this.checkBox11.TabIndex = 2;
            this.checkBox11.Text = "X";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox12
            // 
            this.checkBox12.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox12.AutoSize = true;
            this.checkBox12.Location = new System.Drawing.Point(114, 24);
            this.checkBox12.Name = "checkBox12";
            this.checkBox12.Size = new System.Drawing.Size(23, 23);
            this.checkBox12.TabIndex = 3;
            this.checkBox12.Text = "X";
            this.checkBox12.UseVisualStyleBackColor = true;
            this.checkBox12.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // checkBox13
            // 
            this.checkBox13.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox13.AutoSize = true;
            this.checkBox13.Location = new System.Drawing.Point(136, 24);
            this.checkBox13.Name = "checkBox13";
            this.checkBox13.Size = new System.Drawing.Size(23, 23);
            this.checkBox13.TabIndex = 3;
            this.checkBox13.Text = "X";
            this.checkBox13.UseVisualStyleBackColor = true;
            this.checkBox13.CheckedChanged += new System.EventHandler(this.starCheckChanged);
            // 
            // panelAspects
            // 
            this.panelAspects.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelAspects.Controls.Add(this.textBoxOffset);
            this.panelAspects.Controls.Add(this.buttonReset);
            this.panelAspects.Controls.Add(this.buttonSet);
            this.panelAspects.Controls.Add(this.checkBoxQuintile);
            this.panelAspects.Controls.Add(this.checkBoxOpposite);
            this.panelAspects.Controls.Add(this.checkBoxTrine);
            this.panelAspects.Controls.Add(this.checkBoxSquare);
            this.panelAspects.Controls.Add(this.checkBoxSextile);
            this.panelAspects.Controls.Add(this.buttonClear);
            this.panelAspects.Controls.Add(this.comboBoxFocused);
            this.panelAspects.Location = new System.Drawing.Point(2, 113);
            this.panelAspects.Name = "panelAspects";
            this.panelAspects.Size = new System.Drawing.Size(162, 78);
            this.panelAspects.TabIndex = 12;
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Location = new System.Drawing.Point(1, 52);
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(78, 20);
            this.textBoxOffset.TabIndex = 8;
            this.textBoxOffset.Leave += new System.EventHandler(this.textBoxOffset_Leave);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(75, 2);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(46, 23);
            this.buttonReset.TabIndex = 7;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonSet
            // 
            this.buttonSet.Location = new System.Drawing.Point(121, 2);
            this.buttonSet.Name = "buttonSet";
            this.buttonSet.Size = new System.Drawing.Size(36, 23);
            this.buttonSet.TabIndex = 7;
            this.buttonSet.Text = "Set";
            this.buttonSet.UseVisualStyleBackColor = true;
            this.buttonSet.Click += new System.EventHandler(this.buttonSet_Click);
            // 
            // checkBoxQuintile
            // 
            this.checkBoxQuintile.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxQuintile.AutoSize = true;
            this.checkBoxQuintile.Location = new System.Drawing.Point(28, 27);
            this.checkBoxQuintile.Name = "checkBoxQuintile";
            this.checkBoxQuintile.Size = new System.Drawing.Size(29, 23);
            this.checkBoxQuintile.TabIndex = 6;
            this.checkBoxQuintile.Text = "72";
            this.checkBoxQuintile.UseVisualStyleBackColor = true;
            this.checkBoxQuintile.CheckedChanged += new System.EventHandler(this.aspectCheckedChanged);
            // 
            // checkBoxOpposite
            // 
            this.checkBoxOpposite.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxOpposite.AutoSize = true;
            this.checkBoxOpposite.Location = new System.Drawing.Point(121, 27);
            this.checkBoxOpposite.Name = "checkBoxOpposite";
            this.checkBoxOpposite.Size = new System.Drawing.Size(35, 23);
            this.checkBoxOpposite.TabIndex = 5;
            this.checkBoxOpposite.Text = "180";
            this.checkBoxOpposite.UseVisualStyleBackColor = true;
            this.checkBoxOpposite.CheckedChanged += new System.EventHandler(this.aspectCheckedChanged);
            // 
            // checkBoxTrine
            // 
            this.checkBoxTrine.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxTrine.AutoSize = true;
            this.checkBoxTrine.Location = new System.Drawing.Point(86, 27);
            this.checkBoxTrine.Name = "checkBoxTrine";
            this.checkBoxTrine.Size = new System.Drawing.Size(35, 23);
            this.checkBoxTrine.TabIndex = 4;
            this.checkBoxTrine.Text = "120";
            this.checkBoxTrine.UseVisualStyleBackColor = true;
            this.checkBoxTrine.CheckedChanged += new System.EventHandler(this.aspectCheckedChanged);
            // 
            // checkBoxSquare
            // 
            this.checkBoxSquare.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxSquare.AutoSize = true;
            this.checkBoxSquare.Location = new System.Drawing.Point(57, 27);
            this.checkBoxSquare.Name = "checkBoxSquare";
            this.checkBoxSquare.Size = new System.Drawing.Size(29, 23);
            this.checkBoxSquare.TabIndex = 3;
            this.checkBoxSquare.Text = "90";
            this.checkBoxSquare.UseVisualStyleBackColor = true;
            this.checkBoxSquare.CheckedChanged += new System.EventHandler(this.aspectCheckedChanged);
            // 
            // checkBoxSextile
            // 
            this.checkBoxSextile.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxSextile.AutoSize = true;
            this.checkBoxSextile.Location = new System.Drawing.Point(-1, 27);
            this.checkBoxSextile.Name = "checkBoxSextile";
            this.checkBoxSextile.Size = new System.Drawing.Size(29, 23);
            this.checkBoxSextile.TabIndex = 2;
            this.checkBoxSextile.Text = "60";
            this.checkBoxSextile.UseVisualStyleBackColor = true;
            this.checkBoxSextile.CheckedChanged += new System.EventHandler(this.aspectCheckedChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(85, 51);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(71, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "Clear All";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // comboBoxFocused
            // 
            this.comboBoxFocused.FormattingEnabled = true;
            this.comboBoxFocused.Location = new System.Drawing.Point(2, 2);
            this.comboBoxFocused.Name = "comboBoxFocused";
            this.comboBoxFocused.Size = new System.Drawing.Size(71, 21);
            this.comboBoxFocused.TabIndex = 0;
            this.comboBoxFocused.SelectedIndexChanged += new System.EventHandler(this.comboBoxFocused_SelectedIndexChanged);
            // 
            // panelEvents
            // 
            this.panelEvents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelEvents.Controls.Add(this.comboBoxAnother);
            this.panelEvents.Controls.Add(this.comboBoxConcernedStar);
            this.panelEvents.Controls.Add(this.label1);
            this.panelEvents.Controls.Add(this.comboBoxImportance);
            this.panelEvents.Controls.Add(this.checkBoxAspect);
            this.panelEvents.Controls.Add(this.checkBoxDeclination);
            this.panelEvents.Controls.Add(this.checkBoxSigns);
            this.panelEvents.Controls.Add(this.checkBoxDirection);
            this.panelEvents.Controls.Add(this.checkBoxOccultations);
            this.panelEvents.Location = new System.Drawing.Point(2, 194);
            this.panelEvents.Name = "panelEvents";
            this.panelEvents.Size = new System.Drawing.Size(162, 89);
            this.panelEvents.TabIndex = 13;
            // 
            // comboBoxAnother
            // 
            this.comboBoxAnother.Enabled = false;
            this.comboBoxAnother.FormattingEnabled = true;
            this.comboBoxAnother.Location = new System.Drawing.Point(87, 59);
            this.comboBoxAnother.Name = "comboBoxAnother";
            this.comboBoxAnother.Size = new System.Drawing.Size(71, 21);
            this.comboBoxAnother.TabIndex = 5;
            // 
            // comboBoxConcernedStar
            // 
            this.comboBoxConcernedStar.Enabled = false;
            this.comboBoxConcernedStar.FormattingEnabled = true;
            this.comboBoxConcernedStar.Location = new System.Drawing.Point(15, 59);
            this.comboBoxConcernedStar.Name = "comboBoxConcernedStar";
            this.comboBoxConcernedStar.Size = new System.Drawing.Size(71, 21);
            this.comboBoxConcernedStar.TabIndex = 5;
            this.comboBoxConcernedStar.SelectedIndexChanged += new System.EventHandler(this.comboBoxConcernedStar_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "of";
            // 
            // comboBoxImportance
            // 
            this.comboBoxImportance.Enabled = false;
            this.comboBoxImportance.FormattingEnabled = true;
            this.comboBoxImportance.Location = new System.Drawing.Point(69, 35);
            this.comboBoxImportance.Name = "comboBoxImportance";
            this.comboBoxImportance.Size = new System.Drawing.Size(85, 21);
            this.comboBoxImportance.TabIndex = 3;
            // 
            // checkBoxAspect
            // 
            this.checkBoxAspect.AutoSize = true;
            this.checkBoxAspect.Location = new System.Drawing.Point(3, 36);
            this.checkBoxAspect.Name = "checkBoxAspect";
            this.checkBoxAspect.Size = new System.Drawing.Size(64, 17);
            this.checkBoxAspect.TabIndex = 2;
            this.checkBoxAspect.Text = "Aspects";
            this.checkBoxAspect.UseVisualStyleBackColor = true;
            this.checkBoxAspect.CheckedChanged += new System.EventHandler(this.checkBoxAspect_CheckedChanged);
            // 
            // checkBoxDeclination
            // 
            this.checkBoxDeclination.AutoSize = true;
            this.checkBoxDeclination.Location = new System.Drawing.Point(79, 20);
            this.checkBoxDeclination.Name = "checkBoxDeclination";
            this.checkBoxDeclination.Size = new System.Drawing.Size(57, 17);
            this.checkBoxDeclination.TabIndex = 2;
            this.checkBoxDeclination.Text = "Height";
            this.checkBoxDeclination.UseVisualStyleBackColor = true;
            this.checkBoxDeclination.CheckedChanged += new System.EventHandler(this.eventCategorySelected);
            // 
            // checkBoxSigns
            // 
            this.checkBoxSigns.AutoSize = true;
            this.checkBoxSigns.Location = new System.Drawing.Point(3, 20);
            this.checkBoxSigns.Name = "checkBoxSigns";
            this.checkBoxSigns.Size = new System.Drawing.Size(46, 17);
            this.checkBoxSigns.TabIndex = 1;
            this.checkBoxSigns.Text = "Sign";
            this.checkBoxSigns.UseVisualStyleBackColor = true;
            this.checkBoxSigns.CheckedChanged += new System.EventHandler(this.eventCategorySelected);
            // 
            // checkBoxDirection
            // 
            this.checkBoxDirection.AutoSize = true;
            this.checkBoxDirection.Location = new System.Drawing.Point(79, 3);
            this.checkBoxDirection.Name = "checkBoxDirection";
            this.checkBoxDirection.Size = new System.Drawing.Size(68, 17);
            this.checkBoxDirection.TabIndex = 0;
            this.checkBoxDirection.Text = "Direction";
            this.checkBoxDirection.UseVisualStyleBackColor = true;
            this.checkBoxDirection.CheckedChanged += new System.EventHandler(this.eventCategorySelected);
            // 
            // checkBoxOccultations
            // 
            this.checkBoxOccultations.AutoSize = true;
            this.checkBoxOccultations.Location = new System.Drawing.Point(3, 3);
            this.checkBoxOccultations.Name = "checkBoxOccultations";
            this.checkBoxOccultations.Size = new System.Drawing.Size(80, 17);
            this.checkBoxOccultations.TabIndex = 0;
            this.checkBoxOccultations.Text = "Occultation";
            this.checkBoxOccultations.UseVisualStyleBackColor = true;
            this.checkBoxOccultations.CheckedChanged += new System.EventHandler(this.eventCategorySelected);
            // 
            // FloatToolStripForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(168, 288);
            this.Controls.Add(this.panelEvents);
            this.Controls.Add(this.panelAspects);
            this.Controls.Add(this.panelStars);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("AstroSymbols", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FloatToolStripForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Toolbox";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FloatToolStripForm_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelStars.ResumeLayout(false);
            this.panelStars.PerformLayout();
            this.panelAspects.ResumeLayout(false);
            this.panelAspects.PerformLayout();
            this.panelEvents.ResumeLayout(false);
            this.panelEvents.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonHeliocentric;
        private System.Windows.Forms.RadioButton radioButtonGeocentric;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelStars;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox checkBox11;
        private System.Windows.Forms.CheckBox checkBox12;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Panel panelAspects;
        private System.Windows.Forms.ComboBox comboBoxFocused;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.CheckBox checkBoxQuintile;
        private System.Windows.Forms.CheckBox checkBoxOpposite;
        private System.Windows.Forms.CheckBox checkBoxTrine;
        private System.Windows.Forms.CheckBox checkBoxSquare;
        private System.Windows.Forms.CheckBox checkBoxSextile;
        private System.Windows.Forms.TextBox textBoxOffset;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonSet;
        private System.Windows.Forms.Panel panelEvents;
        private System.Windows.Forms.CheckBox checkBoxAspect;
        private System.Windows.Forms.CheckBox checkBoxDeclination;
        private System.Windows.Forms.CheckBox checkBoxSigns;
        private System.Windows.Forms.CheckBox checkBoxDirection;
        private System.Windows.Forms.CheckBox checkBoxOccultations;
        private System.Windows.Forms.ComboBox comboBoxAnother;
        private System.Windows.Forms.ComboBox comboBoxConcernedStar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxImportance;
        private System.Windows.Forms.CheckBox checkBox13;
    }
}