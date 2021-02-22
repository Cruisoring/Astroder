using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataImporter;

namespace Astroder
{
    public partial class PoboBrowser : Form
    {
        public RecentFileInfo TheQuote { get; set; }

        //public string SelectedFileName = "";
        //public string Identification = "";

        public PoboBrowser()
        {
            InitializeComponent();

            tabControlMarkets.TabPages.Clear();

            foreach (KeyValuePair<string, string> kvp in PoboDataImporter.MarketsTable)
            {
                TabPage newPage = new TabPage(kvp.Value);
                newPage.Tag = kvp.Key;
                tabControlMarkets.TabPages.Add(newPage);
            }

            tabControlMarkets.SelectedIndex = 1;
        }

        private void tabControlMarkets_SelectedIndexChanged(object sender, EventArgs e)
        {
            appendGoodsSummary();
        }

        private void appendGoodsSummary()
        {
            if (tabControlMarkets.SelectedTab.Controls.Count != 0)
                return;

            DataGridView allGoodsView = new DataGridView();
            BindingSource summaryBindings = new BindingSource();
            allGoodsView.Dock = DockStyle.Fill;
            allGoodsView.ScrollBars = ScrollBars.Both;
            allGoodsView.MultiSelect = false;
            allGoodsView.AutoGenerateColumns = true;
            allGoodsView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabControlMarkets.SelectedTab.Controls.Add(allGoodsView);

            Dictionary<string, string> allGoods = PoboDataImporter.AllGoodsOf(tabControlMarkets.SelectedTab.Tag as string);

            allGoodsView.Tag = allGoods;

            List<PoboDayFileSummary> summaries = new List<PoboDayFileSummary>();

            foreach (KeyValuePair<string, string> kvp in allGoods)
            {
                PoboDayFileSummary newSummary = new PoboDayFileSummary(kvp.Key, kvp.Value);
                if (newSummary == null || newSummary.ItemsCount <= 1)
                    continue;

                summaries.Add(newSummary);
            }

            summaryBindings.DataSource = summaries;
            allGoodsView.DataSource = summaryBindings;
            allGoodsView.SelectionChanged += new EventHandler(allGoodsView_SelectionChanged);
            allGoodsView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //allGoodsView.Invalidate();

        }

        void allGoodsView_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView theView = sender as DataGridView;
            Dictionary<string, string> allGoods = theView.Tag as Dictionary<string, string>;

            if (theView == null || allGoods == null)
                throw new Exception("The DataGridView is not ready.");

            BindingSource summaryBindings = theView.DataSource as BindingSource;

            if (summaryBindings == null)
                throw new Exception("Null BindingSource!");

            List<PoboDayFileSummary> summaries = summaryBindings.DataSource as List<PoboDayFileSummary>;

            if (summaries == null)
                throw new Exception("The name list is not available!");

            string name = summaries[theView.CurrentRow.Index].Name;

            TheQuote = new RecentFileInfo(name, allGoods[name], SourceType.Pobo);
            //SelectedFileName = allGoods[name];
            //Identification = name;
        }
    }
}
