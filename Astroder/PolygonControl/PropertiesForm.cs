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
    public partial class PropertiesForm : Form
    {
        PolygonControl polygonControl1;

        public PropertiesForm(PolygonControl polygon)
        {
            polygonControl1 = polygon;

            InitializeComponent();

            int selected = -1;
            string name = polygon.Calculator.Shape.Name;
            foreach (KeyValuePair<String, Polygon> kvp in Polygon.All)
            {
                //if (kvp.Value.IsPolygon)
                comboBoxCalculator.Items.Add(kvp.Key);
                if (kvp.Key == name)
                    selected = comboBoxCalculator.Items.Count - 1;
            }
            if (selected != -1)
                comboBoxCalculator.SelectedIndex = selected;

            selected = -1;
            foreach (FirstQuadrantOrientation orientation in Enum.GetValues(typeof(FirstQuadrantOrientation)))
            {
                comboBoxOrientation.Items.Add(orientation.ToString());
                if (orientation == polygon.FirstQuadrant)
                    selected = comboBoxOrientation.Items.Count - 1;
            }
            if (selected != -1)
                comboBoxOrientation.SelectedIndex = selected;

            numericUpDownMaxCycle.Value = polygon.MaxCycle;
            numericUpDownUnitSize.Value = polygon.UnitSize;
        }

        private void buttonCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            String name = (comboBoxCalculator.SelectedItem == null) ? comboBoxCalculator.Items[0].ToString() : comboBoxCalculator.SelectedItem.ToString();
            bool needRedraw = false;

            if (polygonControl1.Calculator.Shape.Name != name)
            {
                needRedraw = true;
                Polygon polygon = Polygon.All[name];
                polygonControl1.Calculator = new PolygonControl.PolygonCalculator(polygonControl1, polygon, (int)numericUpDownMaxCycle.Value);
            }

            if (polygonControl1.MaxCycle != numericUpDownMaxCycle.Value && polygonControl1.Calculator.Shape.IsPolygon)
            {
                needRedraw = true;
                polygonControl1.MaxCycle = (int)numericUpDownMaxCycle.Value;
            }

            if (polygonControl1.UnitSize != numericUpDownUnitSize.Value)
            {
                needRedraw = true;
                polygonControl1.UnitSize = (int)numericUpDownUnitSize.Value;
            }

            FirstQuadrantOrientation orientation = (FirstQuadrantOrientation)(comboBoxOrientation.SelectedIndex);
            if (polygonControl1.FirstQuadrant != orientation)
            {
                needRedraw = true;
                polygonControl1.FirstQuadrant = orientation;
            }

            if (needRedraw)
                polygonControl1.Redraw();
            this.Close();
        }
    }
}
