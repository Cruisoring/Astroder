using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    [Serializable]
    public class Astrolabe 
    {
        #region Constants and Statics
        public static SearchMode DefaultMode = SearchMode.AroundWorkingDay;

        //static Astrolabe()
        //{
        //    foreach (PlanetId id in Planet.All.Keys)
        //    {
        //        Concerned.Add(id);
        //    }
        //}
        #endregion

        #region Fields and Properties
        //public DateTimeOffset Moment { get; private set; }
        public Period During { get; private set; }

        public SearchMode Mode { get; private set; }

        public SortedDictionary<PlanetId, Position> StarPositions { get; private set; }

        public Position this[PlanetId id]
        {
            get
            {
                if (StarPositions.ContainsKey(id))
                    return StarPositions[id];
                else
                    return null;
            }
        }

        public Dictionary<RelationFlag, Relation> Patterns { get; private set; }

        //public string PetternDescription
        //{
        //    get
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        foreach (KeyValuePair<RelationTypes, Relation> kvp in patterns)
        //        {
        //            sb.AppendFormat("{0}\r\n", kvp.Value);
        //        }

        //        return sb.ToString();
        //    }
        //}

        #endregion

        #region Constructors
        public Astrolabe(DateTimeOffset moment) : this(moment, DefaultMode)
        {
        }

        public Astrolabe(DateTimeOffset moment, SearchMode mode)
        {
            StarPositions = new SortedDictionary<PlanetId, Position>();
            Patterns = new Dictionary<RelationFlag, Relation>();

            Mode = mode;
            During = new Period(moment, Mode);

            Calculate();
        }
        #endregion

        #region
        public void Calculate()
        {
            getPositions();

            getRelations();
        }

        public void ChangeModeTo(SearchMode newMode)
        {
            if (Mode != newMode)
            {
                Mode = newMode;
                getRelations();
            }
        }

        private void getRelations()
        {
            if (Patterns.Count != 0)
                Patterns.Clear();

            During = new Period(During.ReferenceTime, Mode);

            List<PlanetId> planets = Planet.All.Keys.ToList();
            //RelationFinder finder = null;

            for (int i = 9; i >= 1; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    //finder = new RelationFinder(During, planets[i], planets[j]);
                    List<Relation> relations = Relation.RelationsDuring(planets[i], planets[j], During);

                    foreach (Relation relation in relations)
                    {
                        Patterns.Add(relation.Flag, relation);
                    }
                }

            }

        }

        private void getPositions()
        {
            List<Position> positions = Ephemeris.Geocentric[During.ReferenceTime];

            StarPositions.Clear();

            foreach (PlanetId id in Planet.All.Keys)
            {
                foreach (Position pos in positions)
                {
                    if (pos.Owner.Id != id)
                        continue;

                    StarPositions.Add(id, pos);
                    break;
                }

                if (!StarPositions.ContainsKey(id))
                    StarPositions.Add(id, Ephemeris.Geocentric[During.ReferenceTime, id]);
            }

        }

        //public List<Phenomenon> GetNearbyPhenonema()
        //{
        //    List<Phenomenon> phenomena = new List<Phenomenon>();
        //    Position posA, posB;

        //    for (int i = 0; i < starPositions.Count - 1; i++)
        //    {
        //        KeyValuePair<PlanetId, Position> kvp = starPositions.ElementAt(i);
        //        posA = kvp.Value;
        //        for (int j = i + 1; j < starPositions.Count; j++)
        //        {
        //            kvp = starPositions.ElementAt(j);
        //            posB = kvp.Value;

        //            Relation newRelation = new Relation(posA, posB);

        //            RelationFinder helper = new RelationFinder(newRelation.Kind, this.moment);

        //            Phenomenon newPhenom = helper.PhenomenonNearby();

        //            if (newPhenom != null)
        //                phenomena.Add(newPhenom);
        //            //if (Relation.HasRelation(posA, posB))
        //            //{
        //            //    Relation newRelation = new Relation(posA, posB);

        //            //    RelationFinder helper = new RelationFinder(newRelation.Kind, this.moment);

        //            //    Phenomenon newPhenom = helper.PhenomenonNearby();

        //            //    if (newPhenom != null)
        //            //        phenomena.Add(newPhenom);
        //            //}
        //        }
        //    }

        //    SweWrapper.swe_close();

        //    return phenomena;
        //}

        public override string ToString()
        {
            return String.Format("Astrolabe of {0}, {1} relations formed.", During, Patterns.Count);
        }

        #endregion
    }

}
