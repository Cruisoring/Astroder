using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuoteHelper;
using NumberHelper;
using AstroHelper;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AstroTeller
{
    public partial class QuoteManagerForm : Form
    {
        public QuoteCollection History = null;

        public QuoteManagerForm()
        {
            InitializeComponent();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (importQuoteFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                List<Quote> items = null;
                int pos = importQuoteFileDialog.FileName.LastIndexOf('.');
                string dirName = importQuoteFileDialog.FileName.Substring(0, pos);

                pos = dirName.LastIndexOf('\\');
                string name = dirName.Substring(pos + 1, dirName.Length - pos - 1);

                if (importQuoteFileDialog.FileName.EndsWith("txt"))
                {
                    textBoxFileName.Text = "";

                    items = TextImporter.Import(importQuoteFileDialog.FileName);

                    //foreach (string filename in importQuoteFileDialog.FileNames)
                    //{
                    //    textBoxFileName.Text += filename + " ";
                    //}

                    //int pos = importQuoteFileDialog.FileNames[0].LastIndexOf(importQuoteFileDialog.FileNames.Count() == 1 ? '.' : '\\');
                    //string dirName = importQuoteFileDialog.FileNames[0].Substring(0, pos);

                    //foreach (string fileName in importQuoteFileDialog.FileNames)
                    //{
                    //    List<Quote> newItems = TextImporter.Import(fileName);
                    //    items.AddRange(newItems);
                    //}
                }
                else if (importQuoteFileDialog.FileName.EndsWith(PoboImporter.FilePostfix))
                {
                    items = PoboImporter.Import(importQuoteFileDialog.FileName);
                }

                History = new QuoteCollection(name, items);

                if (History.Count != 0)
                {
                    loadQuoteInformation();
                    Ephemeris.CurrentEphemeris.Load(History.Since, History.Until);
                }

                launchDetailForm();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void launchDetailForm()
        {
            if (this.Tag == null)
            {
                MainForm main = (this.ParentForm is MainForm) ? (this.ParentForm as MainForm) : null;

                if (main != null)
                {
                    main.MaximizePanel2();
                }

                DetailForm newStudy = new DetailForm(History);
                newStudy.Tag = this;
                newStudy.Text = History.Name;

                newStudy.MdiParent = GlobalItems.MdiWindow;
                //newStudy.Parent = this;

                this.FormClosed += new FormClosedEventHandler(newStudy.OnQuoteManagerForm_FormClosed);

                GlobalItems.MdiWindow.Panel2.Controls.Add(newStudy);
                newStudy.Parent = GlobalItems.MdiWindow.Panel2;
                newStudy.Dock = DockStyle.Fill;

                newStudy.Show();

                this.Tag = newStudy;
            }
            else
            {
                if (this.Tag is DetailForm)
                {
                    DetailForm detail = (DetailForm)(this.Tag);

                    detail.ReloadHistory(History);

                    detail.Display();
                }
            }
        }

        private void loadQuoteInformation()
        {
            textBoxName.Text = History.Name;
            textBoxCount.Text = History.Count.ToString();
            textBoxHighest.Text = History.DataCollection[History.CelingIndex].Date.ToString();
            textBoxLowest.Text = History.DataCollection[History.FloorIndex].Date.ToString();
            textBoxSince.Text = History.Since.ToString(History.DateTimeFormat);
            textBoxUntil.Text = History.Until.ToString(History.DateTimeFormat);
            textBoxDescription.Text = History.Description;
            panelInformation.Visible = true;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            History.Name = textBoxName.Text;
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            History.Description = textBoxDescription.Text;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Quote File|*.quo";
            saveDialog.FileName = History.Name + ".quo";
            saveDialog.Title = "Save the Quote as in binary format...";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, History);
                stream.Close();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Filter = "Quote File|*.quo";
            loadDialog.Title = "Load the QuoteCollection now binary file ...";

            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(loadDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                History = (QuoteCollection)formatter.Deserialize(stream);
                stream.Close();

                if (History != null && History.Count != 0)
                {
                    loadQuoteInformation();
                    Ephemeris.CurrentEphemeris.Load(History.Since, History.Until);

                    launchDetailForm();
                }
            }

            //if (this.FormClosed != null)
            //{

            //}
        }

    }
}
