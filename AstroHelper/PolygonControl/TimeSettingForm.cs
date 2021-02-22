using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PolygonControl
{
    public partial class TimeSettingForm : Form
    {
        static List<String> timeZones = new List<String> { "China Standard Time", "GMT Standard Time", "Eastern Standard Time" };

        PolygonControl polygon;

        public TimeSettingForm(PolygonControl control)
        {
            polygon = control;

            InitializeComponent();

            this.timeZoneList.DataSource = timeZones;
            timeZoneList.SelectedIndex = 0;

            //System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> tzCollection;
            //tzCollection = TimeZoneInfo.GetSystemTimeZones();
            //this.timeZoneList.DataSource = tzCollection;
            //timeZoneList.SelectedIndex = tzCollection.IndexOf(TimeZoneInfo.Local);

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneList.SelectedItem.ToString());

            DateTime time = dateTimePicker1.Value;

            DateTimeOffset newTime = new DateTimeOffset( time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, zone.BaseUtcOffset);

            polygon.HighlightDate(newTime);

            this.Close();
        }
    }
}
