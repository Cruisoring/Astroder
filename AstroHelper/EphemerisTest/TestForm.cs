using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstroHelper;
using NumberHelper;
using QuoteHelper;

namespace EphemerisTest
{
    public partial class TestForm : Form
    {
        private const string timeFormat = "yyyy-MM-dd";

        private DateTimeOffset theMoment = DateTimeOffset.UtcNow;
        //private Astrolabe theAstrolabe = null;
        private BindingSource positionsBindingSource = new BindingSource();
        private BindingSource relationBindingSource = new BindingSource();
        private BindingSource quoteBindingSource = new BindingSource();

        public TestForm()
        {
            InitializeComponent();
            
            //foreach (PeriodMode mode in (PeriodMode[])System.Enum.GetValues(typeof(PeriodMode)))
            //{
            //    comboBoxMode.Items.Add(mode.ToString());
            //}
            //comboBoxMode.SelectedIndex = 0;

            dateTimePicker1.CustomFormat = timeFormat;
            dataGridViewPositions.AutoGenerateColumns = true;
            dataGridViewPositions.DataSource = positionsBindingSource;
            dataGridViewRelations.DataSource = relationBindingSource;
            dataGridViewQuotes.DataSource = quoteBindingSource;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime time = dateTimePicker1.Value.Date;
            theMoment = new DateTimeOffset(new DateTime(time.Year, time.Month, time.Day), TimeSpan.Zero);
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            //if (theAstrolabe == null || theAstrolabe.During.Mode != Astrolabe.DefaultMode || theAstrolabe.During.ReferenceTime != theMoment)
            //{
            //    theAstrolabe = new Astrolabe(theMoment);
            //}

            //var posQuery =
            //    from pos in theAstrolabe.StarPositions.Values
            //    orderby pos.Owner.Id
            //    select new 
            //    { 
            //        Star = pos.Owner.Name, 
            //        pos.Longitude, 
            //        pos.Latitude, 
            //        pos.LongitudeVelocity, 
            //        pos.LatitudeVelocity
            //    };
            

            //positionsBindingSource.DataSource = posQuery.ToList();
            ////dataGridViewPositions.DataSource = positionsBindingSource;

            //var relationQuery =
            //    from relation in theAstrolabe.Patterns.Values
            //    orderby relation.Flag.SupperiorId descending, relation.Flag.InferiorId descending, relation.Moment
            //    select new
            //    {
            //        relation.Moment,
            //        //detail.Superior,
            //        //detail.Aspect,
            //        //detail.Inferior,
            //        relation.Description,
            //        //SuperiorRect = detail.SuperiorPosition.Longitude,
            //        //InferiorRect = detail.InferiorPosition.Longitude,
            //        Angle = relation.InferiorPosition.Longitude - relation.SuperiorPosition.Longitude,
            //        Orb = Math.Round(relation.Orb, 4),
            //        relation.Flag.IsSuperiorRetrograde,
            //        relation.Flag.IsInteriorRetrograde,
            //        relation.Flag.IsSameDirection,
            //        relation.Flag.IsExpanding,
            //        SuperiorVelo = Math.Round(relation.SuperiorPosition.LongitudeVelocity, 5),
            //        InferiorVelo = Math.Round(relation.InferiorPosition.LongitudeVelocity, 5)
            //    };

            //relationBindingSource.DataSource = relationQuery.ToList();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            //DateTime date = dateTimePicker1.Value.Date;
            //switch(Astrolabe.DefaultMode)
            //{
            //    case PeriodMode.AroundWorkingDay:
            //    case PeriodMode.WithinTheDay:
            //        dateTimePicker1.Value = dateTimePicker1.Value.AddDays(-1);
            //        break;
            //    case PeriodMode.WithinTheWeek:
            //        dateTimePicker1.Value = date.AddDays(-7);
            //        break;
            //    case PeriodMode.WithinTheMonth:
            //        dateTimePicker1.Value = date.AddMonths(-1);
            //        break;
            //    case PeriodMode.WithinTheYear:
            //        dateTimePicker1.Value = date.AddYears(-1);
            //        break;
            //    default:
            //        throw new NotImplementedException();
            //}
            //buttonGo_Click(this, null);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //DateTime date = dateTimePicker1.Value.Date;
            //switch (Astrolabe.DefaultMode)
            //{
            //    case PeriodMode.AroundWorkingDay:
            //    case PeriodMode.WithinTheDay:
            //        dateTimePicker1.Value = dateTimePicker1.Value.AddDays(1);
            //        break;
            //    case PeriodMode.WithinTheWeek:
            //        dateTimePicker1.Value = date.AddDays(7);
            //        break;
            //    case PeriodMode.WithinTheMonth:
            //        dateTimePicker1.Value = date.AddMonths(1);
            //        break;
            //    case PeriodMode.WithinTheYear:
            //        dateTimePicker1.Value = date.AddYears(1);
            //        break;
            //    default:
            //        throw new NotImplementedException();
            //}
            //buttonGo_Click(this, null);
        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Astrolabe.DefaultMode = (PeriodMode)Enum.Parse(typeof(PeriodMode), comboBoxMode.SelectedItem as string);
        }

        private void buttonImportTxt_Click(object sender, EventArgs e)
        {
            if(openTxtFileDialog.ShowDialog() != DialogResult.OK)
                return;

            int pos = openTxtFileDialog.FileNames[0].LastIndexOf('\\');
            string dirName = openTxtFileDialog.FileNames[0].Substring(0, pos);

            pos = dirName.LastIndexOf('\\');
            string name = dirName.Substring(pos + 1, dirName.Length - pos - 1);
            List<DayItem> items = new List<DayItem>();

            foreach (string fileName in openTxtFileDialog.FileNames)
            {
                List<DayItem> newItems = TextImporter.Import(fileName);
                items.AddRange(newItems);
            }

            DailyQuote theQuote = new DailyQuote(name, items);

            if (theQuote.DailyData == null || theQuote.DailyData.Count == 0)
                return;

            var quoteQuery =
                from quote in theQuote.DailyData
                select new { Time = quote.Date, quote.Open, quote.High, quote.Low, quote.Close };

            quoteBindingSource.DataSource = quoteQuery.ToList();

            TrendParser<DayItem> dailyParser = new TrendParser<DayItem>(10);

            List<TurningPoint<DayItem>> majors = dailyParser.PivotsOf(theQuote.DailyData);

            relationBindingSource.DataSource = majors;

            tabControl1.SelectedIndex = 2;
        }

        private void buttonTest1_Click(object sender, EventArgs e)
        {
            DateTimeOffset since = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);
            int count = 0;
            double longitude = 0;
            DateTime start = DateTime.Now;

            for (int i = 0; i < 360; i ++ )
            {
                DateTimeOffset date = since.AddDays(i);
                for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id ++ )
                {
                    longitude = Ephemeris.Geocentric[date, id].Longitude;
                    count++;
                }
            }

            MessageBox.Show(String.Format("{0}ms elapsed for {1} position calculation.", (DateTime.Now - start).TotalMilliseconds, count));
        }
    }
}
