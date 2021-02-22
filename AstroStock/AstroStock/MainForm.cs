using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using EpheWrapper;

namespace AstroStock
{
    public partial class MainForm : Form
    {
        private const string timeFormat = "yyyy-MM-dd HH:mm";
        private TimeZoneInfo theTimeZone;
        private TimeSpan baseAdjust;
        private TimeZoneInfo.AdjustmentRule[] adjustments = null;
        private Astrolabe theAstrolobe = null;

        DateTime utcMoment = DateTime.UtcNow;

        public MainForm()
        {
            InitializeComponent();
            this.dateTimePicker1.CustomFormat = timeFormat;
            //this.dateTimePicker1.Value = DateTime.Now;
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo zone in timeZones)
            {
                comboBoxTimeZones.DataSource = new BindingSource(timeZones, "");
            }
            comboBoxTimeZones.SelectedIndex = 1;
            //comboBoxTimeZones.SelectedIndex = timeZones.IndexOf(TimeZoneInfo.Local);

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            dataGridView1.ColumnHeadersVisible = true;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSize = true;

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (theTimeZone == null)
                return;

            DateTime temp = dateTimePicker1.Value - theTimeZone.BaseUtcOffset;

            if (checkBoxDST.Checked && adjustments.Length != 0 )//&& theTimeZone.IsAmbiguousTime(temp.Date) )
            {
                foreach (TimeZoneInfo.AdjustmentRule rule in adjustments)
                {
                    if (rule.DateEnd < temp.Date || rule.DateStart > temp.Date)
                        continue;
                    else
                    {
                        temp -= rule.DaylightDelta;
                        break;
                    }
                }
            }


            utcMoment = new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, 0, DateTimeKind.Utc);
            textBox2.Text = utcMoment.ToString(timeFormat);
            double julDay = SweWrapper.ToJulianDay(utcMoment);
            textBox3.Text = julDay.ToString();
        }


        private void buttonCalc_Click(object sender, EventArgs e)
        {
            if (theAstrolobe == null)
                theAstrolobe = new Astrolabe("Unknown", utcMoment);
            else if (theAstrolobe.Moment != utcMoment)
            {
                theAstrolobe.Moment = utcMoment;
            }

            theAstrolobe.Calculate();

            if (astrolabeBindingSource.DataSource == null)
                astrolabeBindingSource.DataSource = theAstrolobe.StarPositions.Values;
            else
            {
                astrolabeBindingSource.DataSource = null;
                astrolabeBindingSource.DataSource = theAstrolobe.StarPositions.Values;
                //astrolabeBindingSource.ResetBindings(false);
            }

            this.textBoxRelations.Text = theAstrolobe.PetternDescription;
        }

        private void comboBoxTimeZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            theTimeZone = (TimeZoneInfo)comboBoxTimeZones.SelectedItem;
            //checkBoxDST.Checked = theTimeZone.IsDaylightSavingTime(dateTimePicker1.Value);
            adjustments = theTimeZone.GetAdjustmentRules();
            baseAdjust = TimeZoneInfo.Local.BaseUtcOffset - theTimeZone.BaseUtcOffset;
            dateTimePicker1_ValueChanged(null, null);
        }

        private void checkBoxDST_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1_ValueChanged(null, null);
        }

    }
}
