using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EphemerisCalculator;

namespace AstroCalendarControl
{
    public partial class AstroCalendar : UserControl
    {
        public const PeriodType DefaultPeriodType = PeriodType.AroundTheMonth;
        public const AspectImportance DefaultAspectImportance = AspectImportance.Important;
        public static Action<AstroCalendar> PeriodParameterChangedEvent { get; set; }

        #region Variables

        PeriodType Kind { get; set; }

        AspectImportance Concerned { get; set; }

        DateTimeOffset RefTime { get; set; }

        EventsSummary EventsCollection;

        public Action<AstroCalendar> PeriodEventsCollectedEvent { get; set; }

        #endregion

        public AstroCalendar()
        {
            InitializeComponent();

            Kind = DefaultPeriodType;
            Concerned = DefaultAspectImportance;
            RefTime = DateTimeOffset.UtcNow;

            int index = 0;

            foreach (PeriodType kind in Enum.GetValues(typeof(PeriodType)))
            {
                if (kind != PeriodType.Customer)
                    comboBoxPeriodType.Items.Add(kind.ToString());

                if (kind == DefaultPeriodType)
                    index = comboBoxPeriodType.Items.Count-1;
            }
            comboBoxPeriodType.SelectedIndex = index;
            this.comboBoxPeriodType.SelectedIndexChanged += new System.EventHandler(this.comboBoxPeriodType_SelectedIndexChanged);

            foreach (AspectImportance importance in Enum.GetValues(typeof(AspectImportance)))
            {
                comboBoxImportance.Items.Add(importance.ToString());

                if (importance == DefaultAspectImportance)
                    index = comboBoxImportance.Items.Count-1;
            }
            comboBoxImportance.SelectedIndex = index;
            this.comboBoxImportance.SelectedIndexChanged += new System.EventHandler(this.comboBoxImportance_SelectedIndexChanged);

            PeriodParameterChangedEvent += new Action<AstroCalendar>(this.SynchPeriodParameters);
        }

        private void comboBoxPeriodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PeriodType newPeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), comboBoxPeriodType.SelectedItem.ToString());
            if (Kind != newPeriodType)
            {
                Kind = newPeriodType;
                AspectImportance importance = EventsSummary.ConcernedAspect[Kind];

                dateTimePicker.Format = DateTimePickerFormat.Custom;
                dateTimePicker.CustomFormat = EventsSummary.DateTimeFormats[Kind];
                comboBoxImportance.SelectedIndex = (int)importance;

                if (PeriodParameterChangedEvent != null)
                    PeriodParameterChangedEvent(this);
            }
        }

        private void comboBoxImportance_SelectedIndexChanged(object sender, EventArgs e)
        {
            AspectImportance newImportance = (AspectImportance)comboBoxImportance.SelectedIndex;
            if (Concerned != newImportance)
            {
                Concerned = newImportance;
                if (PeriodParameterChangedEvent != null)
                    PeriodParameterChangedEvent(this);
            }
        }

        private void dateTimePicker_Leave(object sender, EventArgs e)
        {
            DateTimeOffset newDate = dateTimePicker.Value.Date;
            if (EventsCollection == null || newDate != RefTime.Date)
            {
                RefTime = new DateTimeOffset(newDate.Year, newDate.Month, newDate.Day, 0, 0, 0, TimeSpan.Zero);

                EventsCollection = new EventsSummary(RefTime, Kind, Concerned);

                displayEvents();

                if (PeriodEventsCollectedEvent != null)
                    PeriodEventsCollectedEvent(this);

            }
        }

        private void displayEvents()
        {
            treeViewEvents.BeginUpdate();
            treeViewEvents.Nodes.Clear();

            TreeNode parent = new TreeNode(String.Format("{0} {1}: {2}.", 
                EventsCollection.Since.ToString(EventsCollection.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
                EventsCollection.Until.ToString(EventsCollection.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture),
                EventsCollection.EventListNum));
            parent.Tag = EventsCollection;

            TreeNode temp = null;

            #region display occultations
            if (EventsCollection.Occultations != null && EventsCollection.Occultations.Count != 0)
            {
                TreeNode occuNodes = new TreeNode(EventsCollection.Occultations.Count.ToString() + " eclipses/occultations.");

                foreach (IPlanetEvent evt in EventsCollection.Occultations)
                {
                    temp = new TreeNode(evt.ShortDescription);
                    temp.Tag = evt;
                    occuNodes.Nodes.Add(temp);
                }
                parent.Nodes.Add(occuNodes);
            }
            #endregion

            #region display Geocentric Events
            if (EventsCollection.GeocentricAspects != null && EventsCollection.GeocentricAspects.Count != 0)
            {
                TreeNode aspNodes = new TreeNode("GeoAspect: " + EventsCollection.GeocentricAspects.Count.ToString() + " pairs.");

                foreach (KeyValuePair<PlanetPair, List<IPlanetEvent>> kvp in EventsCollection.GeocentricAspects)
                {
                    TreeNode pairNode = null;
                    if (kvp.Value.Count > 1)
                    {
                        pairNode = new TreeNode(string.Format("{0}: {1} events.", kvp.Key.ToString(), kvp.Value.Count));
                        foreach (ExactAspectEvent evt in kvp.Value)
                        {
                            temp = new TreeNode(evt.ShortDescription);
                            temp.Tag = evt;
                            pairNode.Nodes.Add(temp);
                        }
                    }
                    else if (kvp.Value.Count == 1)
                    {
                        pairNode = new TreeNode(kvp.Value[0].ShortDescription);
                        pairNode.Tag = kvp.Value[0];
                    }
                    else
                        throw new Exception("The GeocentricAspects contains no events for " + kvp.Key.ToString());

                    aspNodes.Nodes.Add(pairNode);
                }
                parent.Nodes.Add(aspNodes);
            }

            if (EventsCollection.GeocentricSignChanges != null && EventsCollection.GeocentricSignChanges.Count != 0)
            {
                TreeNode signEventsNodes = new TreeNode("GeoSigns: " + EventsCollection.GeocentricSignChanges.Count.ToString() + " changes.");

                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in EventsCollection.GeocentricSignChanges)
                {
                    if (kvp.Value.Count == 1)
                    {
                        temp = new TreeNode(kvp.Value[0].ShortDescription);
                        temp.Tag = kvp.Value[0];
                    }
                    else
                    {
                        temp = new TreeNode(String.Format("{0}: {1} times", Planet.Glyphs[kvp.Key], kvp.Value.Count));
                        foreach (IPlanetEvent evt in kvp.Value)
                        {
                            TreeNode detailNode = new TreeNode(evt.ShortDescription);
                            detailNode.Tag = evt;
                            temp.Nodes.Add(detailNode);
                        }
                    }

                    signEventsNodes.Nodes.Add(temp);
                }
                parent.Nodes.Add(signEventsNodes);
            }

            if (EventsCollection.DirectionChanges != null && EventsCollection.DirectionChanges.Count != 0)
            {
                TreeNode directionalNodes = new TreeNode("GeoDirection: " + EventsCollection.DirectionChanges.Count.ToString() + " changes.");

                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in EventsCollection.DirectionChanges)
                {
                    if (kvp.Value.Count == 1)
                    {
                        temp = new TreeNode(kvp.Value[0].ShortDescription);
                        temp.Tag = kvp.Value[0];
                    }
                    else
                    {
                        temp = new TreeNode(String.Format("{0}: {1} times", Planet.Glyphs[kvp.Key], kvp.Value.Count));
                        foreach (IPlanetEvent evt in kvp.Value)
                        {
                            TreeNode detailNode = new TreeNode(evt.ShortDescription);
                            detailNode.Tag = evt;
                            temp.Nodes.Add(detailNode);
                        }
                    }

                    directionalNodes.Nodes.Add(temp);
                }
                parent.Nodes.Add(directionalNodes);

            }

            #endregion

            #region display Heliocentric Events

            if (EventsCollection.HeliocentricAspects != null && EventsCollection.HeliocentricAspects.Count != 0)
            {
                TreeNode aspNodes = new TreeNode("HelioAspects: " + EventsCollection.HeliocentricAspects.Count.ToString() + " pairs.");

                foreach (KeyValuePair<PlanetPair, List<IPlanetEvent>> kvp in EventsCollection.HeliocentricAspects)
                {
                    TreeNode pairNode = null;
                    if (kvp.Value.Count > 1)
                    {
                        pairNode = new TreeNode(string.Format("{0}: {1} events.", kvp.Key, kvp.Value.Count));
                        foreach (ExactAspectEvent evt in kvp.Value)
                        {
                            temp = new TreeNode(evt.ShortDescription);
                            temp.Tag = evt;
                            pairNode.Nodes.Add(temp);
                        }
                    }
                    else if (kvp.Value.Count == 1)
                    {
                        pairNode = new TreeNode(kvp.Value[0].ShortDescription);
                        pairNode.Tag = kvp.Value[0];
                    }
                    else
                        throw new Exception("The HeliocentricAspects contains no events for " + kvp.Key.ToString());

                    aspNodes.Nodes.Add(pairNode);
                }
                parent.Nodes.Add(aspNodes);
            }

            if (EventsCollection.HeliocentricSignChanges != null && EventsCollection.HeliocentricSignChanges.Count != 0)
            {
                TreeNode signEventsNodes = new TreeNode("HelioSigns: " + EventsCollection.HeliocentricSignChanges.Count.ToString() + " changes.");

                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in EventsCollection.HeliocentricSignChanges)
                {
                    if (kvp.Value.Count == 1)
                    {
                        temp = new TreeNode(kvp.Value[0].ShortDescription);
                        temp.Tag = kvp.Value[0];
                    }
                    else
                    {
                        temp = new TreeNode(String.Format("{0}: {1} times", Planet.Glyphs[kvp.Key], kvp.Value.Count));
                        foreach (IPlanetEvent evt in kvp.Value)
                        {
                            TreeNode detailNode = new TreeNode(evt.ShortDescription);
                            detailNode.Tag = evt;
                            temp.Nodes.Add(detailNode);
                        }
                    }

                    signEventsNodes.Nodes.Add(temp);
                }
                parent.Nodes.Add(signEventsNodes);
            }

            #endregion

            treeViewEvents.Nodes.Add(parent);

            treeViewEvents.ExpandAll();

            treeViewEvents.EndUpdate();
        }

        private void treeViewEvents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewEvents.SelectedNode.Tag != null && treeViewEvents.SelectedNode.Tag is IPlanetEvent)
            {
                textBoxDetail.Text = (treeViewEvents.SelectedNode.Tag).ToString();
            }
            else
                textBoxDetail.Text = "";
        }

        public void SynchPeriodParameters(AstroCalendar another)
        {
            if (another == this)
                return;

            List<string> valueStrings = new List<string>();

            if (this.Kind != another.Kind)
            {
                this.Kind = another.Kind;

                dateTimePicker.CustomFormat = EventsSummary.DateTimeFormats[Kind];
                
                foreach (object item in comboBoxPeriodType.Items)
                {
                    valueStrings.Add(item.ToString());
                }

                comboBoxPeriodType.SelectedIndex = valueStrings.IndexOf(Kind.ToString());

                EventsCollection = null;

                treeViewEvents.Nodes.Clear();
            }
            else if (this.Concerned != another.Concerned)
            {
                this.Concerned = another.Concerned;

                valueStrings.Clear();

                foreach (object item in comboBoxImportance.Items)
                {
                    valueStrings.Add(item.ToString());
                }

                comboBoxImportance.SelectedIndex = valueStrings.IndexOf(Concerned.ToString());

                EventsCollection = null;

                treeViewEvents.Nodes.Clear();
            }
        }

        public void EnableOrDisable(AstroCalendar another)
        {
            this.Enabled = (another.EventsCollection != null);
        }
    }
}
