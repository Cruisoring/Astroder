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

namespace HistryDataCollector
{
    public partial class CollectorForm : Form
    {
        GroupedQuoteCollection quoteGroup = null;

        public CollectorForm()
        {
            InitializeComponent();

            int count = CommodityInfomation.CommodityDict.Count ;
        }

        private void buttonLoadPobo_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.da1";
            openFileDialog1.Filter = "Pobo Day Files|*.da1";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            int start = openFileDialog1.FileName.LastIndexOf('\\');

            int upIndex = openFileDialog1.FileName.Substring(0, start).LastIndexOf('\\');

            CommodityInfomation commodity = CommodityInfomation.CommodityOf(openFileDialog1.FileName.Substring(start));

            if (quoteGroup == null)
                throw new Exception("Please import records from text files first.");

            if (commodity == null)
            {
                textBoxDescription.Text = "Failed to get Commodity Information from " + openFileDialog1.FileName;
                return;
            }
            else if (commodity != quoteGroup.Commodity)
            {
                textBoxDescription.Text = "Different commodity: " + openFileDialog1.FileName;
                return;
            }
            else
                textBoxDescription.Text = string.Format("{0}\r\n", commodity);

            foreach (string filename in openFileDialog1.FileNames)
            {
                ContractInfomation theContract = commodity.ContractOf(filename.Substring(start));

                if (theContract.Month == 0 && (openFileDialog1.FileNames.Length > 1))
                    continue;

                List<Quote> items = PoboDataImporter.Import(filename, RecordType.DayRecord);

                if (items == null)
                    continue;

                QuoteCollection quotes = new QuoteCollection(theContract, filename, RecordType.DayRecord, items);

                quoteGroup.Group.Add(theContract, quotes);
            }


        }


        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.txt";
            openFileDialog1.Filter = "Text Files|*.txt";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            int start = openFileDialog1.FileName.LastIndexOf('\\');
            textBoxPath.Text = openFileDialog1.FileName.Substring(0, start+1);
            string fileName = openFileDialog1.FileName.Substring(start);

            int upIndex = openFileDialog1.FileName.Substring(0, start).LastIndexOf('\\');
            textBoxOutput.Text = openFileDialog1.FileName.Substring(0, upIndex + 1);

            CommodityInfomation commodity = CommodityInfomation.CommodityOf(fileName);

            if (commodity == null)
            {
                textBoxDescription.Text = "Failed to get Commodity Information from " + openFileDialog1.FileName;
                return;
            }
            else
                textBoxDescription.Text = string.Format("{0}\r\n", commodity);

            SortedDictionary<ContractInfomation, QuoteCollection> theSortedQuotes = new SortedDictionary<ContractInfomation, QuoteCollection>();

            foreach (string filename in openFileDialog1.FileNames)
            {
                ContractInfomation theContract = commodity.ContractOf(filename.Substring(start));
                List<Quote> items = TextDataImporter.Import(filename);

                if (items == null)
                    continue;

                QuoteCollection quotes = new QuoteCollection(theContract, filename, RecordType.DayRecord, items);

                theSortedQuotes.Add(theContract, quotes);
            }

            quoteGroup = new GroupedQuoteCollection(commodity, theSortedQuotes);

            textBoxDescription.AppendText(String.Format("{0} Contracts: from {1} to {2}", theSortedQuotes.Count, theSortedQuotes.First().Value.Since, theSortedQuotes.Last().Value.Until));

            checkedListBoxMonths.Items.Clear();

            foreach (MonthCodes code in commodity.Months)
            {
                checkedListBoxMonths.Items.Add(code);
                checkedListBoxMonths.SetItemChecked(checkedListBoxMonths.Items.Count - 1, false);
            }

        }

        private void buttonAllOn_Click(object sender, EventArgs e)
        {
            for (int index = 0;  index < checkedListBoxMonths.Items.Count; index ++)
            {
                checkedListBoxMonths.SetItemChecked(index, true);
            }
        }

        private void buttonAllOff_Click(object sender, EventArgs e)
        {
            for(int index = 0;  index < checkedListBoxMonths.Items.Count; index ++)
            {
                checkedListBoxMonths.SetItemChecked(index, false);
            }
        }

        private void checkBoxContinuous_CheckedChanged(object sender, EventArgs e)
        {
            panelContinuous.Enabled = checkBoxContinuous.Checked;
        }

        private List<GroupingFlag> methodFlags
        {
            get
            {
                List<GroupingFlag> flags = new List<GroupingFlag>();

                if (checkBoxAsIs.Checked)
                    flags.Add(GroupingFlag.AsIs);

                if (checkBoxShiftBackward.Checked)
                    flags.Add(GroupingFlag.ForewardCompatible);

                if (checkBoxShiftBackward.Checked)
                    flags.Add(GroupingFlag.BackwardCompatible);

                return flags;
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            string destPath = textBoxOutput.Text;

            int fileCount = 0;

            foreach (object obj in checkedListBoxMonths.CheckedItems)
            {
                MonthCodes code = obj as MonthCodes;

                List<QuoteCollection> monthlyContracts = quoteGroup[code.Month];

                string formatString = "yyyy-MM-dd";

                foreach (QuoteCollection contract in monthlyContracts)
                {
                    string txtname = destPath + contract.Name + ".txt";
                    FileStream stream = new FileStream(txtname, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(stream);
                    sw.AutoFlush = true;

                    sw.WriteLine("Date,Open,High,Low,Close,Volume");
                    for (int i = 0; i < contract.DataCollection.Count; i++)
                    {
                        Quote quote = contract.DataCollection[i];
                        //sw.WriteLine(quote.ToString());
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5}", quote.Time.ToString(formatString),
                            quote.Open, quote.High, quote.Low, quote.Close, quote.Volume));
                    }
                    stream.Close();

                    fileCount++;
                }
            }

            if (panelContinuous.Enabled)
            {
                foreach (GroupingFlag flag in this.methodFlags)
                {
                    bool isVolumeBased = checkBoxVolumeBased.Checked;
                    QuoteCollection continous = quoteGroup[flag, isVolumeBased];

                    string formatString = "yyyy-MM-dd";

                    string txtname = destPath + continous.Name + ".txt";
                    FileStream stream = new FileStream(txtname, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(stream);
                    sw.AutoFlush = true;

                    sw.WriteLine("Date,Open,High,Low,Close,Volume");
                    for (int i = 0; i < continous.DataCollection.Count; i++)
                    {
                        Quote quote = continous.DataCollection[i];
                        //sw.WriteLine(quote.ToString());
                        sw.WriteLine(String.Format("{0},{1},{2},{3},{4},{5}", quote.Time.ToString(formatString),
                            quote.Open, quote.High, quote.Low, quote.Close, quote.Volume));
                    }
                    stream.Close();

                    fileCount++;
                }
            }

            MessageBox.Show(fileCount.ToString() + " files are geneerated.");
        }

    }
}
