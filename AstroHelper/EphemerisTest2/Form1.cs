using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstroHelper;

namespace EphemerisTest2
{
    public partial class Form1 : Form
    {
        private DateTimeOffset start;
        private bool isBackward = false;
        private PlanetId occulted = PlanetId.SE_SUN;
        private int defaultRangeInDays = 365;

        public Form1()
        {
            InitializeComponent();
            DateTime date = dateTimePicker1.Value.Date;
            start = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);

            for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id ++ )
            {
                if (id == PlanetId.SE_MOON)
                    continue;

                comboBoxPlanets.Items.Add(id.ToString());
            }
            comboBoxPlanets.SelectedIndex = 0;
        }

        private void ButtonTest1_Click(object sender, EventArgs e)
        {
            DateTimeOffset since = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);
            int count = 0;
            double longitude = 0;
            DateTime start = DateTime.Now;

            for (int i = 0; i < 360; i++)
            {
                DateTimeOffset date = since.AddDays(i);
                for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
                {
                    longitude = Ephemeris.Geocentric[date, id].Longitude;
                    count++;
                }
            }

            MessageBox.Show(String.Format("{0}ms elapsed for {1} geoPos calculation.", (DateTime.Now - start).TotalMilliseconds, count));
        }

        private void buttonTest2_Click(object sender, EventArgs e)
        {
            DateTimeOffset since = new DateTimeOffset(2001, 1, 1, 12, 0, 0, TimeSpan.Zero); 
            int count = 0;
            double longitude = 0;
            DateTime start = DateTime.Now;

            for (int i = 0; i < 360; i++)
            {
                DateTimeOffset date = since.AddDays(i);
                for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
                {
                    longitude = Ephemeris.Geocentric[date, id].Longitude;
                    count++;
                }
            }

            MessageBox.Show(String.Format("{0}ms elapsed for {1} geoPos calculation.", (DateTime.Now - start).TotalMilliseconds, count));
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime date = dateTimePicker1.Value.Date;
            start = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);
        }

        private void checkBoxDirection_CheckedChanged(object sender, EventArgs e)
        {
            isBackward = checkBoxDirection.Checked;
        }

        private void buttonSolarEclipse_Click(object sender, EventArgs e)
        {
            SolarEclipse solEclipse = Utilities.SolarEclipseAround(start, isBackward);

            MessageBox.Show(solEclipse.ToString());
            
        }

        private void buttonLunarEclipse_Click(object sender, EventArgs e)
        {
            LunarEclipse lunEclipse = Utilities.LunarEclipseAround(start, isBackward);
            MessageBox.Show(lunEclipse.ToString());
        }

        private void buttonOccultation_Click(object sender, EventArgs e)
        {
            LunarOccultation lunOccultation = Utilities.LunarOccultationAround(start, occulted, isBackward);
            MessageBox.Show(lunOccultation.ToString());
        }

        private void comboBoxPlanets_SelectedIndexChanged(object sender, EventArgs e)
        {
            occulted = (PlanetId)Enum.Parse(typeof(PlanetId), comboBoxPlanets.SelectedItem.ToString());
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            DateTimeOffset end = start.AddDays(isBackward ? -defaultRangeInDays : defaultRangeInDays);

            List<EclipseOccultation> lunarEvents = isBackward ? Utilities.LunarEventsDuring(end, start) : Utilities.LunarEventsDuring(start, end);

            StringBuilder sb = new StringBuilder();
            foreach (EclipseOccultation evt in lunarEvents)
            {
                sb.AppendLine(evt.ToString());
            }

            MessageBox.Show(sb.ToString(), String.Format("Lunar events between {0} and {1}", start, end));
        }


    }
}
