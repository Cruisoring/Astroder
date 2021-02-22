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
    public partial class PivotForm : Form
    {
        public OutlineItem ThePivot;

        public PivotForm(OutlineItem pivot)
        {
            InitializeComponent();

            foreach (PivotType type in Enum.GetValues(typeof(PivotType)))
            {
                comboBoxPivotType.Items.Add(type);
            }
            comboBoxPivotType.SelectedIndex = comboBoxPivotType.Items.IndexOf(PivotType.Unknown);

            if (pivot == null)
            {
                textBoxYear.Focus();
                textBoxIndex.Text = "0";
                textBoxPrice.Text = "0";
            }
            else
            {
                textBoxYear.Text = pivot.Time.Year.ToString();
                textBoxMonth.Text = pivot.Time.Month.ToString();
                textBoxDay.Text = pivot.Time.Day.ToString();
                textBoxPrice.Text = pivot.Price.ToString();
                //comboBoxPivotType.SelectedIndex = comboBoxPivotType.Items.IndexOf(pivot.Type);
                textBoxIndex.Text = pivot.RecordIndex.ToString();
                textBoxPrice.Focus();
                textBoxPrice.SelectAll();
            }

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            int year, month, day, index;

            double price;

            if (!int.TryParse(textBoxYear.Text, out year) || !int.TryParse(textBoxMonth.Text, out month) || !int.TryParse(textBoxDay.Text, out day))
            {
                MessageBox.Show("Please specify the Date!");
                return;
            }

            DateTimeOffset time = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);

            if (!double.TryParse(textBoxPrice.Text, out price))
            {
                MessageBox.Show("Please specify the Price!");
                return;
            }

            if (!int.TryParse(textBoxIndex.Text, out index))
            {
                MessageBox.Show("Please specify the Index!");
                return;
            }

            ThePivot = new OutlineItem(time, price, index, (PivotType)(comboBoxPivotType.SelectedItem));

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
