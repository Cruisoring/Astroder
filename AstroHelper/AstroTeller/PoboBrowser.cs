using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuoteHelper;

namespace AstroTeller
{
    public partial class PoboBrowser : Form
    {
        public string SelectedFileName = "";
        public string FutureName = "";

        public PoboBrowser()
        {
            InitializeComponent();

            tabControlMarkets.TabPages.Clear();

            foreach (KeyValuePair<string, string> kvp in PoboImporter.MarketsTable)
            {
                TabPage newPage = new TabPage(kvp.Value);
                newPage.Tag = kvp.Key;
                tabControlMarkets.TabPages.Add(newPage);
            }

            tabControlMarkets.SelectedIndex = 1;
        }

        private void tabControlMarkets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMarkets.SelectedTab.Controls.Count != 0)
                return;
            else
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

            Dictionary<string, string> allGoods = PoboImporter.AllGoodsOf(tabControlMarkets.SelectedTab.Tag as string);

            allGoodsView.Tag = allGoods;

            List<PoboDayFileSummary> summaries = new List<PoboDayFileSummary>();

            foreach (KeyValuePair<string, string> kvp in allGoods)
            {
                PoboDayFileSummary newSummary = new PoboDayFileSummary(kvp.Key, kvp.Value);
                if (newSummary == null)
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

            BindingSource summaryBindings = theView.DataSource as  BindingSource;

            if (summaryBindings == null)
                throw new Exception("Null BindingSource!");

            List<PoboDayFileSummary> summaries = summaryBindings.DataSource as List<PoboDayFileSummary>;

            if (summaries == null)
                throw new Exception("The name list is not available!");

            string name = summaries[theView.CurrentRow.Index].Name;

            SelectedFileName = allGoods[name];
            FutureName = name;
        }
    }
}
