using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EpheWrapper;
using DataTableGenerator;

namespace AstroTest
{
    public partial class Form1 : Form
    {
        DateTime utcMoment;
        PlanetId interior = PlanetId.SE_ECL_NUT;
        PlanetId exterior = PlanetId.SE_ECL_NUT;

        DataTable phenomenonTable, patternTable;
        Astrolabe theAstrolobe = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<PlanetId, Planet> kvp in Planet.All)
            {
                comboBoxInterior.Items.Add(kvp.Value);
            }

            foreach(Aspect aspct in Aspect.Major.Values)
            {
                comboBoxAspect.Items.Add(aspct);
            }

            utcMoment = DateTime.UtcNow.Date + TimeSpan.FromHours(1);
        }

        private void comboBoxInterior_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxExterior.Items.Clear();

            interior = (comboBoxInterior.SelectedItem as Planet).Id;

            foreach (KeyValuePair<PlanetId, Planet> kvp in Planet.All)
            {
                if (kvp.Key <= interior)
                    continue;

                comboBoxExterior.Items.Add(kvp.Value);
            }

            comboBoxExterior.SelectedIndex = 0;
        }

        private void comboBoxExterior_SelectedIndexChanged(object sender, EventArgs e)
        {
            exterior = (comboBoxExterior.SelectedItem as Planet).Id;
        }

        private void dateTimeRough_ValueChanged(object sender, EventArgs e)
        {
            DateTime temp = dateTimeRough.Value;
            utcMoment = new DateTime(temp.Year, temp.Month, temp.Day, 12, 0, 0, DateTimeKind.Utc);
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            //List<PlanetPairAspect> pairs = new List<PlanetPairAspect>();

            //PlanetPairAspect pairAspect = new PlanetPairAspect(utcMoment, interior, exterior);

            //pairs.Add(pairAspect);

            //if (pairAspect.Pattern != AspectType.None)
            //{
            //    AspectHelper foreteller = new AspectHelper(pairAspect.Kind, utcMoment);
            //    DateTime exactMoment = foreteller.GetEventTime();
            //    pairAspect = new PlanetPairAspect(exactMoment, interior, exterior);
            //    pairs.Add(pairAspect);
            //}

            //phenomenonTable = DataTableGenerator.DataTableHelper<PlanetPairAspect>.DataTableOf(pairs);

            if (theAstrolobe == null)
                theAstrolobe = new Astrolabe("Unknown", utcMoment);
            else if (theAstrolobe.Moment != utcMoment)
            {
                theAstrolobe.Moment = utcMoment;
            }

            theAstrolobe.Calculate();

            patternTable = DataTableHelper<Relation>.DataTableOf(theAstrolobe.Patterns.Values);

            dataGridView1.DataSource = patternTable;

            List<Phenomenon> phenomena = theAstrolobe.GetNearbyPhenonema();

            phenomenonTable = DataTableGenerator.DataTableHelper<Phenomenon>.DataTableOf(phenomena);

            dataGridView2.DataSource = phenomenonTable;


        }
    }
}
