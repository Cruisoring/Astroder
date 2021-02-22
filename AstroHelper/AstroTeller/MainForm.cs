using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuoteHelper;
using AstroHelper;

namespace AstroTeller
{
    public partial class MainForm : Form
    {
        public SplitContainer Splitter { get { return (SplitContainer)splitContainer1; } }

        public SplitterPanel Panel1 { get { return Splitter.Panel1; } }
        public SplitterPanel Panel2 { get { return Splitter.Panel2; } }

        public MainForm()
        {            
            InitializeComponent();
            //int count = AstroHelper.Ephemeris.CurrentEphemeris.Count;
            newToolStripMenuItem_Click(this, null);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuoteManagerForm quoteForm = new QuoteManagerForm();
            quoteForm.MdiParent = this;
            Panel1.Controls.Add(quoteForm);
            quoteForm.Show();
            quoteForm.BringToFront();
            if (Panel1.Controls.Count == 1)
                quoteForm.Dock = DockStyle.Fill;
            else
            {
                int count = Panel1.Controls.Count;
                int height = Panel1.Height / count;
                int width = Panel1.Width;

                for (int i = 0; i < count; i++)
                {
                    Panel1.Controls[i].Dock = DockStyle.None;
                    Panel1.Controls[i].Location = new Point(0, i * height);
                    Panel1.Controls[i].Height = height;
                    Panel1.Controls[i].Width = width;
                }
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ActiveMdiChild.Name == "StarsViewerForm")
                {
                    DetailForm starsViewer = (DetailForm)this.ActiveMdiChild;
                    starsViewer.Close();
                }
                else
                {
                    QuoteManagerForm objfrmSChild = (QuoteManagerForm)this.ActiveMdiChild;
                    objfrmSChild.Close();
                }
            }
            catch
            {
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !(splitContainer1.Panel1Collapsed);
        }

        private void splitContainer1_Panel2_DoubleClick(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !(splitContainer1.Panel1Collapsed);
        }

        private void splitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            if(splitContainer1.Panel2.Controls.Count != 0 && splitContainer1.Panel2.Controls[0] is DetailForm)
            {
                DetailForm first = splitContainer1.Panel2.Controls[0] as DetailForm;
                first.WindowState = FormWindowState.Normal;
                first.WindowState = FormWindowState.Maximized;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //AstroHelper.Ephemeris.CurrentEphemeris.Dispose();
        }

        public void MaximizePanel2()
        {
            splitContainer1.Panel1Collapsed = true;
        }

        private void launchDetailForm()
        {
            MaximizePanel2();

            DetailForm newStudy = new DetailForm(History);
            newStudy.Tag = this;
            newStudy.Text = History.Name;

            newStudy.MdiParent = GlobalItems.MdiWindow;
            //newStudy.Parent = this;

            Panel2.Controls.Add(newStudy);
            newStudy.Parent = Panel2;
            newStudy.Dock = DockStyle.Fill;

            newStudy.Show();
        }

        public QuoteCollection History = null;

        private void browsePoboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PoboBrowser browser = new PoboBrowser();
            if (browser.ShowDialog() == DialogResult.OK && browser.SelectedFileName != "")
            {
                List<Quote> items = PoboImporter.Import(browser.SelectedFileName);

                History = new QuoteCollection(browser.FutureName, items);

                if (History.Count != 0)
                {
                    Ephemeris.CurrentEphemeris.Load(History.Since, History.Until);
                }

                launchDetailForm();
            }
        }
    }
}
