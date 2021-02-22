using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataImporter;
using System.IO;
using System.Xml.Serialization;

namespace Astroder
{
    public partial class InputForm : Form
    {
        public BindingList<Quote> AllQuotes { get; set; }

        public Quote Current
        {
            get
            {
                if (dataGridViewQuotes.CurrentRow == null || dataGridViewQuotes.CurrentRow.Index < 0 || dataGridViewQuotes.CurrentRow.Index > AllQuotes.Count)
                    return null;

                return AllQuotes[dataGridViewQuotes.CurrentRow.Index];
            }
        }

        public string QuoteName { get { return textBoxName.Text; } }

        public RecordType QuoteType { get { return theType; } }

        private RecordType theType = RecordType.UserDefined;

        public InputForm()
        {
            InitializeComponent();

            AllQuotes = new BindingList<Quote>();

            for (RecordType type = RecordType.DayRecord; type <= RecordType.UserDefined; type ++ )
            {
                comboBoxQuoteType.Items.Add(type);
            }
            comboBoxQuoteType.SelectedIndex = comboBoxQuoteType.Items.Count - 1;

            dataGridViewQuotes.AutoGenerateColumns = true;
            dataGridViewQuotes.AllowUserToAddRows = true;
            dataGridViewQuotes.AllowUserToDeleteRows = true;

            dataGridViewQuotes.DataSource = AllQuotes;

            buttonRemove.Enabled = false;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int year, month, day;
            double open=-1, high=-1, low=-1, close=-1;

            double.TryParse(textBoxOpen.Text, out open);
            double.TryParse(textBoxHigh.Text, out high);
            double.TryParse(textBoxLow.Text, out low);
            double.TryParse(textBoxClose.Text, out close);

            if (open == -1 && high == -1 && low == -1 && close == -1)
                return;

            if (!int.TryParse(textBoxYear.Text, out year) || !int.TryParse(textBoxMonth.Text, out month) || !int.TryParse(textBoxDay.Text, out day))
                return;

            DateTimeOffset time = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);

            open = (open > 0) ? open : (high > 0 ? high : low);

            Quote newQuote = new Quote(theType, time, open, high, low, close);

            if (Current == null || Current.Time != newQuote.Time)
                AllQuotes.Add(newQuote);
            else
            {
                int index = dataGridViewQuotes.CurrentRow.Index;
                AllQuotes.RemoveAt(index);
                AllQuotes.Insert(index, newQuote);
            }

            if (theType == RecordType.WeekRecord)
                time = time.AddDays(7);
            else if (theType == RecordType.MonthRecord)
                time = time.AddMonths(1);
            else
                time = time.AddDays(1);

            textBoxYear.Text = time.Year.ToString();
            textBoxMonth.Text = time.Month.ToString();
            textBoxDay.Text = time.Day.ToString();

            textBoxOpen.Focus();
            textBoxOpen.SelectAll();
        }

        private void textBoxOpen_Leave(object sender, EventArgs e)
        {
            if (textBoxOpen.Text != "")
            {
                textBoxHigh.Text = textBoxLow.Text = textBoxClose.Text = textBoxOpen.Text;
            }
        }

        private void textBoxHigh_Leave(object sender, EventArgs e)
        {
            if (textBoxHigh.Text != "")
            {
                textBoxLow.Text = textBoxClose.Text = textBoxHigh.Text;
            }
        }

        private void textBoxLow_Leave(object sender, EventArgs e)
        {
            if (textBoxLow.Text != "")
                textBoxClose.Text = textBoxLow.Text;
        }

        private void comboBoxQuoteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            theType = (RecordType)comboBoxQuoteType.SelectedItem;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            AllQuotes.Remove(Current);
            buttonRemove.Enabled = false;
            buttonAdd.Text = "Add";
        }

        private void dataGridViewQuotes_DoubleClick(object sender, EventArgs e)
        {
            textBoxYear.Text = Current.Time.Year.ToString();
            textBoxMonth.Text = Current.Time.Month.ToString();
            textBoxDay.Text = Current.Time.Day.ToString();
            textBoxOpen.Text = Current.Open.ToString();
            textBoxHigh.Text = Current.High.ToString();
            textBoxLow.Text = Current.Low.ToString();
            textBoxClose.Text = Current.Close.ToString();
            buttonRemove.Enabled = true;
            buttonAdd.Text = "Modify";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == "")
            {
                MessageBox.Show("Please specify the name first!");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            List<Quote> quoteList = new List<Quote>(AllQuotes);
            quoteList.Sort();
            saveDialog.Filter = "QuoteList as XML|*.his";
            saveDialog.FileName = textBoxName.Text + theType.ToString() + ".his";
            saveDialog.Title = "Save the Quote List as his file...";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(quoteList.GetType());

                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    serializer.Serialize(fs, quoteList);
                    fs.Close();
                }
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Filter = "QuoteList as XML|*.his";
            loadDialog.Title = "Load the Quote List from xml file...";

            List<Quote> quoteList = null;

            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Quote>));

                using (FileStream fs = new FileStream(loadDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    quoteList = (List<Quote>)serializer.Deserialize(fs);
                    AllQuotes = new BindingList<Quote>(quoteList);
                    dataGridViewQuotes.DataSource = AllQuotes;

                    comboBoxQuoteType.SelectedIndex = comboBoxQuoteType.Items.IndexOf(quoteList[0].Type);
                    string fileName = loadDialog.FileName.Substring(loadDialog.FileName.LastIndexOf(@"\")+1);
                    if (fileName.Contains(theType.ToString()))
                        textBoxName.Text = fileName.Substring(0, fileName.IndexOf(theType.ToString()));
                    else
                        textBoxName.Text = fileName.Substring(0, fileName.Length - 4);

                    fs.Close();
                }
            }

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
