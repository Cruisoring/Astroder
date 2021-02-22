using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AstroClock
{
    public partial class FormClock : Form
    {
        static int timerInterval = 30000;
        public FormClock()
        {
            InitializeComponent();
            polygon.SizeChanged += new EventHandler(polygon_SizeChanged);
            
            polygon.Calculator = new PolygonControl.PolygonControl.PolygonCalculator(polygon, NumberHelper.Polygon.Circle24);
            polygon.PlanetHolder.Date = DateTimeOffset.Now;
            polygon.UnitSize = 16;

            this.Text = "AstroClock " + DateTimeOffset.Now.ToString();

            int waiting = timerInterval - ((int)DateTimeOffset.Now.TimeOfDay.TotalMilliseconds) % timerInterval;
            timer1.Interval = waiting;
            timer1.Tick += new EventHandler(timer1_Adjust);
            timer1.Start();
        }

        void polygon_SizeChanged(object sender, EventArgs e)
        {
            this.ClientSize = polygon.Size;
        }

        private void timer1_Adjust(object sender, EventArgs e)
        {
            if (timer1.Interval != timerInterval)
                timer1.Interval = timerInterval;

            timer1.Tick -= new EventHandler(timer1_Adjust);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "AstroClock " + DateTimeOffset.Now.ToString();
            polygon.PlanetHolder.Date = DateTimeOffset.Now;
            polygon.HighlightDate(DateTimeOffset.Now);
        }
    }
}
