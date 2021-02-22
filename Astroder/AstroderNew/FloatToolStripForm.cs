using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EphemerisCalculator;

namespace Astroder
{
    public partial class FloatToolStripForm : Form
    {
        public event Action<DateTimeOffset, DateTimeOffset> DurationChanged = null;

        public event Action<SeFlg> CentricChanged = null;

        public event Action<List<PlanetId>> SelectedPlanetChanged = null;

        public event Action<PlanetId, List<int>, bool> PlanetOffsetSelected = null;

        public event Action<PlanetEventFlag, bool> ConcernedEventsChanged = null;

        public event Action<bool, PlanetId, PlanetId, AspectImportance> AspectsConcernedChanged = null;

        private AstroderForm theParent = null;

        private bool triggerEvent = true;

        public FloatToolStripForm(AstroderForm parent)
        {
            InitializeComponent();

            theParent = parent;

            radioButtonGeocentric.Checked = (theParent.CentricFlag == SeFlg.GEOCENTRIC);

            foreach (AspectImportance importance in Enum.GetValues(typeof(AspectImportance)))
            {
                comboBoxImportance.Items.Add(importance.ToString());
                if (importance == AspectImportance.Important)
                    comboBoxImportance.SelectedIndex = comboBoxImportance.Items.Count - 1;
            }

            setText();

            parent.DataReload += new Action<AstroderForm>(parent_BackgroundChanged);
        }

        void setText()
        {
            this.radioButtonGeocentric.Checked = theParent.CentricFlag == SeFlg.GEOCENTRIC;
            dateTimePicker1.Value = theParent.Since.UtcDateTime;
            dateTimePicker2.Value = theParent.Until.UtcDateTime;

            checkBoxAspect.Checked = false;

            for (int i = 0; i < theParent.CurrentEphemeris.Luminaries.Count; i ++ )
            {
                Planet star = Planet.PlanetOf(theParent.CurrentEphemeris.Luminaries[i]);
                CheckBox cb = panelStars.Controls[i] as CheckBox;

                cb.Text = star.Symbol.ToString();
                cb.BackColor = Planet.PlanetsColors[theParent.CurrentEphemeris.Luminaries[i]][0];

                comboBoxFocused.Items.Add(star.Name);

                if (theParent.CurrentEphemeris.Luminaries[i] <= PlanetId.SE_PLUTO)
                {
                    comboBoxConcernedStar.Items.Add(star.Name);
                    comboBoxAnother.Items.Add(star.Name);
                }
            }
            comboBoxAnother.Items.Add("Any");
            comboBoxConcernedStar.Items.Add("Any");
            comboBoxAnother.SelectedIndex = comboBoxAnother.Items.Count - 1;
            comboBoxConcernedStar.SelectedIndex = comboBoxConcernedStar.Items.Count - 1;
        }

        void parent_BackgroundChanged(AstroderForm parent)
        {
            triggerEvent = false;

            this.radioButtonGeocentric.Checked = theParent.CentricFlag == SeFlg.GEOCENTRIC;
            dateTimePicker1.Value = theParent.Since.UtcDateTime;
            dateTimePicker2.Value = theParent.Until.UtcDateTime;

            foreach (Control container in this.Controls)
            {
                if (container is Panel)
                {
                    foreach (Control item in container.Controls)
                    {
                        if (item is CheckBox)
                            (item as CheckBox).Checked = false;
                    }
                }
            }

            triggerEvent = true;
        }

        #region Events Handlers

        private void radioButtonGeocentric_CheckedChanged(object sender, EventArgs e)
        {
            if (triggerEvent && CentricChanged != null && radioButtonGeocentric.Checked != (theParent.CentricFlag == SeFlg.GEOCENTRIC))
            {
                CentricChanged(radioButtonGeocentric.Checked ? SeFlg.GEOCENTRIC : SeFlg.HELIOCENTRIC);
            }
        }

        private void dateTimePicker1_Leave(object sender, EventArgs e)
        {
            if (triggerEvent && DurationChanged != null &&(ActiveControl != dateTimePicker1 && ActiveControl != dateTimePicker2))
            {
                DateTime temp = dateTimePicker1.Value;
                DateTimeOffset start = new DateTimeOffset(temp.Year, temp.Month, 1, 0, 0, 0, TimeSpan.Zero);
                temp = dateTimePicker2.Value;
                DateTimeOffset end = new DateTimeOffset(temp.Year, temp.Month, 1, 0, 0, 0, TimeSpan.Zero);

                if (start != theParent.Since || end != theParent.Until)
                    DurationChanged(start, end);
            }
        }

        private void starCheckChanged(object sender, EventArgs e)
        {
            if (triggerEvent && SelectedPlanetChanged != null)
            {
                List<PlanetId> selected = new List<PlanetId>();

                for (int i = 0; i < theParent.CurrentEphemeris.Luminaries.Count; i++)
                {
                    CheckBox cb = panelStars.Controls[i] as CheckBox;

                    if (cb != null && cb.Checked)
                        selected.Add(theParent.CurrentEphemeris.Luminaries[i]);
                }

                SelectedPlanetChanged(selected);
            }
        }

        private void comboBoxFocused_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlanetId id = theParent.CurrentEphemeris.Luminaries[comboBoxFocused.SelectedIndex];

            foreach (Control cnt in panelAspects.Controls)
            {
                CheckBox cb = cnt as CheckBox;
                if (cb == null)
                    continue;

                int aspectDegree = int.Parse(cb.Text);

                if (theParent.Curves.ContainsKey(id))
                    cb.Checked = theParent.Curves[id].ContainsKey(aspectDegree) || theParent.Curves[id].ContainsKey(360 - aspectDegree);
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (comboBoxFocused.SelectedIndex == -1)
                return;

            PlanetId id = theParent.CurrentEphemeris.Luminaries[comboBoxFocused.SelectedIndex];

            foreach (Control cn in panelAspects.Controls)
            {
                if (cn is CheckBox)
                {
                    CheckBox cb = cn as CheckBox;
                    cb.Checked = true;
                }
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            if (comboBoxFocused.SelectedIndex == -1)
                return;

            PlanetId id = theParent.CurrentEphemeris.Luminaries[comboBoxFocused.SelectedIndex];

            foreach (Control cn in panelAspects.Controls)
            {
                if (cn is CheckBox)
                {
                    CheckBox cb = cn as CheckBox;
                    cb.Checked = false;
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (triggerEvent && PlanetOffsetSelected != null)
                PlanetOffsetSelected(PlanetId.SE_ECL_NUT, null, false);
        }

        private void aspectCheckedChanged(object sender, EventArgs e)
        {
            if (triggerEvent && PlanetOffsetSelected != null && comboBoxFocused.SelectedIndex != -1 && sender is CheckBox)
            {
                PlanetId id = theParent.CurrentEphemeris.Luminaries[comboBoxFocused.SelectedIndex];

                CheckBox cb = sender as CheckBox;

                int offset = int.Parse(cb.Text);

                List<int> aspects = new List<int>();

                aspects.Add(offset);

                if (!aspects.Contains(360 - offset))
                    aspects.Add(360 - offset);

                PlanetOffsetSelected(id, aspects, cb.Checked);
            }
        }

        private void textBoxOffset_Leave(object sender, EventArgs e)
        {
            if (triggerEvent && PlanetOffsetSelected != null && comboBoxFocused.SelectedIndex != -1)
            {
                PlanetId id = theParent.CurrentEphemeris.Luminaries[comboBoxFocused.SelectedIndex];
                int degree = 0;

                if (int.TryParse(textBoxOffset.Text, out degree))
                {
                    PlanetOffsetSelected(id, new List<int> { degree }, true);
                }
            }

            textBoxOffset.Text = "";
        }

        private void eventCategorySelected(object sender, EventArgs e)
        {
            if (triggerEvent && ConcernedEventsChanged != null && sender is CheckBox)
            {
                CheckBox cb = sender as CheckBox;

                if (cb.Text.Contains("Occu"))
                    ConcernedEventsChanged(PlanetEventFlag.EclipseOccultationCategory, cb.Checked);
                else if (cb.Text.Contains("Sign"))
                    ConcernedEventsChanged(PlanetEventFlag.SignChangedCategory, cb.Checked);
                else if (cb.Text.Contains("Dire"))
                    ConcernedEventsChanged(PlanetEventFlag.DirectionalCategory, cb.Checked);
                else if (cb.Text.Contains("Hei"))
                    ConcernedEventsChanged(PlanetEventFlag.DeclinationCategory, cb.Checked);
            }
        }

        private void checkBoxAspect_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxImportance.Enabled = comboBoxConcernedStar.Enabled = checkBoxAspect.Checked;

            if (triggerEvent && AspectsConcernedChanged != null)
            {
                AspectImportance importance = (AspectImportance)Enum.Parse(typeof(AspectImportance), comboBoxImportance.SelectedItem.ToString());

                PlanetId star1 = (comboBoxConcernedStar.SelectedIndex == comboBoxConcernedStar.Items.Count - 1) ?
                    PlanetId.SE_ECL_NUT : theParent.CurrentEphemeris.Luminaries[comboBoxConcernedStar.SelectedIndex];

                PlanetId star2 = (comboBoxAnother.SelectedIndex == comboBoxAnother.Items.Count - 1) ?
                    PlanetId.SE_ECL_NUT : theParent.CurrentEphemeris.Luminaries[comboBoxAnother.SelectedIndex];

                AspectsConcernedChanged(checkBoxAspect.Checked, star1, star2, importance);
            }
        }

        private void comboBoxConcernedStar_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxAnother.Enabled = comboBoxConcernedStar.Enabled &&
                comboBoxConcernedStar.SelectedIndex != -1 && (comboBoxConcernedStar.SelectedIndex != comboBoxConcernedStar.Items.Count - 1);

            if (triggerEvent && AspectsConcernedChanged != null)
            {
                AspectImportance importance = (AspectImportance)Enum.Parse(typeof(AspectImportance), comboBoxImportance.SelectedItem.ToString());

                PlanetId star1 = (comboBoxConcernedStar.SelectedIndex == comboBoxConcernedStar.Items.Count - 1) ?
                    PlanetId.SE_ECL_NUT : theParent.CurrentEphemeris.Luminaries[comboBoxConcernedStar.SelectedIndex];

                PlanetId star2 = (comboBoxAnother.SelectedIndex == comboBoxAnother.Items.Count - 1) ?
                    PlanetId.SE_ECL_NUT : theParent.CurrentEphemeris.Luminaries[comboBoxAnother.SelectedIndex];

                AspectsConcernedChanged(true, star1, star2, importance);
            }

        }

        private void FloatToolStripForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            theParent.DataReload -= new Action<AstroderForm>(parent_BackgroundChanged);
        }

        #endregion
    }
}
