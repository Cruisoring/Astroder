using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public class Astrolabe // : INotifyPropertyChanged
    {
        #region Constants and Statics
        public static readonly List<PlanetId> Concerned = new List<PlanetId>();

        static Astrolabe()
        {
            foreach(PlanetId id in Planet.All.Keys)
            {
                Concerned.Add(id);
            }
        }
        #endregion

        #region Fields and Properties
        private readonly SortedDictionary<PlanetId, PlanetPosition> starPositions;

        public SortedDictionary<PlanetId, PlanetPosition> StarPositions
        {
            get { return starPositions; }
        } 

        public PlanetPosition this[PlanetId id]
        {
            get
            {
                if (starPositions.ContainsKey(id))
                    return starPositions[id];
                else
                    return null;
            }
        }

        private readonly Dictionary<RelationKind, Relation> patterns;

        public Dictionary<RelationKind, Relation> Patterns
        {
            get { return patterns; }
        } 

        public string PetternDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<RelationKind, Relation> kvp in patterns)
                {
                    sb.AppendFormat("{0}\r\n", kvp.Value);
                }

                return sb.ToString();
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
        }

        private DateTime moment;

        public DateTime Moment
        {
            get { return moment; }
            set {  moment = value;  // NotifyPropertyChanged("Moment"); 
            }
        } 


        #endregion

        #region INotifyPropertyChanged 成员

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(string propName)
        //{
        //    //if (PropertyChanged != null)
        //    //{
        //    //    PropertyChanged(this, new PropertyChangedEventArgs(propName));
        //    //}
        //}

        #endregion

        #region Constructors
        public Astrolabe(string name)
        {
            this.starPositions = new SortedDictionary<PlanetId, PlanetPosition>();
            this.patterns = new Dictionary<RelationKind, Relation>();

            this.name = name;
        }

        public Astrolabe(string name, DateTime moment) : this(name)
        {
            this.moment = moment;
        }
        #endregion

        #region 
        public void Calculate()
        {
            double jul_ut = SweWrapper.ToJulianDay(this.moment);

            PlanetPosition pos = null;

            foreach (PlanetId id in Astrolabe.Concerned)
            {
                pos = SweWrapper.PositionOf(jul_ut, id, SeFlg.SEFLG_SPEED);

                if (this.starPositions.ContainsKey(id))
                    this.starPositions[id] = pos;
                else
                    this.starPositions.Add(id, pos);

                //this.starPositions.Add(id, pos);

                //pos2 = PositionOf(jul_et, id);
            }

            SweWrapper.swe_close();

            PlanetPosition posA, posB;

            patterns.Clear();

            for(int i = 0; i < starPositions.Count - 1; i ++)
            {
                KeyValuePair<PlanetId, PlanetPosition> kvp = starPositions.ElementAt(i);
                posA = kvp.Value;
                for (int j = i + 1; j < starPositions.Count; j ++ )
                {
                    kvp = starPositions.ElementAt(j);
                    posB = kvp.Value;

                    if (Relation.HasRelation(posA, posB))
                    {
                        Relation newRelation = new Relation(posA, posB);

                        patterns.Add(newRelation.Kind, newRelation);
                    }
                }
            }
        }

        public List<Phenomenon> GetNearbyPhenonema()
        {
            List<Phenomenon> phenomena = new List<Phenomenon>();
            PlanetPosition posA, posB;

            for (int i = 0; i < starPositions.Count - 1; i++)
            {
                KeyValuePair<PlanetId, PlanetPosition> kvp = starPositions.ElementAt(i);
                posA = kvp.Value;
                for (int j = i + 1; j < starPositions.Count; j++)
                {
                    kvp = starPositions.ElementAt(j);
                    posB = kvp.Value;

                    Relation newRelation = new Relation(posA, posB);

                    AspectHelper helper = new AspectHelper(newRelation.Kind, this.moment);

                    Phenomenon newPhenom = helper.PhenomenonNearby();

                    if (newPhenom != null)
                        phenomena.Add(newPhenom);
                    //if (Relation.HasRelation(posA, posB))
                    //{
                    //    Relation newRelation = new Relation(posA, posB);

                    //    AspectHelper helper = new AspectHelper(newRelation.Kind, this.moment);

                    //    Phenomenon newPhenom = helper.PhenomenonNearby();

                    //    if (newPhenom != null)
                    //        phenomena.Add(newPhenom);
                    //}
                }
            }

            SweWrapper.swe_close();

            return phenomena;
        }

        #endregion
    }
}
