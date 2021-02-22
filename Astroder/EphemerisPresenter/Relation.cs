using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    //[Flags]
    //public enum Trendings
    //{
    //    ExpandingEntering = 0,
    //    ContractingEntering = 1,
    //    ExpandingExiting = 2,
    //    ContractingExiting = 3
    //    //DirectEntering,
    //    //DirectExiting,
    //    //RetroEntering,
    //    //RetroExiting

    //    //DirectChasing,          //Both SuperiorId and interior is direct, and speed of SuperiorId is larger than the later
    //    //DirectLagging,          //Both SuperiorId and interior is direct, and speed of SuperiorId is lesser than the later
    //    //RetrogradeChasing,      //Both SuperiorId and interior is retrograde, and speed of SuperiorId is larger than the later
    //    //RetrogradeLagging,      //Both SuperiorId and interior is retrograde, and speed of SuperiorId is lesser than the later
    //    //MeetingFaster,          
    //}

    [Serializable]
    public class Relation
    {
        public static Double Negligible = 0.00001;

        public static List<char> TrendChars = new List<char>() { '<', '\u2264', '>', '\u2265' };

        #region Static Functions
        private const int maxTry = 10;
        private const int eventCount = 10;
        public const int MaxStep = 5;

        public static Relation FirstRelationSince(PlanetId SuperiorId, PlanetId InferiorId, Double sinceJulian)
        {
            reorder(ref SuperiorId, ref InferiorId);

            double orbToNext, speed, step = 100.0, time = sinceJulian;
            Position superiorPos, inferiorPos;
            int tries = 0;
            Angle angle;
            Aspects next = null;

            do
            {
                superiorPos = Ephemeris.Geocentric[time, SuperiorId];
                inferiorPos = Ephemeris.Geocentric[time, InferiorId];

                angle = inferiorPos.Longitude - superiorPos.Longitude;
                speed = inferiorPos.LongitudeVelocity - superiorPos.LongitudeVelocity;

                if (next == null)
                {
                    next = Aspects.NextAspect(angle, speed > 0);

                    double temp = next.OrbOf(angle);

                    step = -temp / speed;

                    if (time + step < sinceJulian)
                    {
                        next = Aspects.NextAspect(angle, speed < 0);
                    }
                }

                orbToNext = next.OrbOf(angle);

                if (Math.Abs(orbToNext) < Negligible)
                {
                    return new Relation(Ephemeris.UtcFromJulianDay(time), superiorPos, inferiorPos);
                }

                step = -orbToNext / speed;

                if (step > MaxStep)
                    step = MaxStep;
                else if (step < -MaxStep)
                    step = -MaxStep;

                time += step;

                tries++;

            } while (tries < maxTry);

            return null;
        }

        private static void reorder(ref PlanetId SuperiorId, ref PlanetId InferiorId)
        {
            if (SuperiorId < InferiorId)
            {
                PlanetId temp = SuperiorId;
                SuperiorId = InferiorId;
                InferiorId = temp;
            }
        }

        //public static List<Relation> RelationsDuring(PlanetId superior, PlanetId inferior, MatchRules during)
        //{
        //    reorder(ref superior, ref inferior);

        //    List<Relation> prices = new List<Relation>();
        //    Relation newRelation = null;
        //    Double SolarEclipseAround = during.StartJulianUtc;

        //    do
        //    {
        //        newRelation = FirstRelationSince(superior, inferior, SolarEclipseAround);

        //        if (newRelation == null || newRelation.Moment > during.Until || newRelation.Moment < during.Since)
        //            break;

        //        prices.Add(newRelation);
        //        SolarEclipseAround = Ephemeris.ToJulianDay(newRelation.Moment);

        //    } while (true);

        //    return prices;
        //}

        //public static List<Relation> AllRelationsDuring(MatchRules during)
        //{
        //    List<Relation> allRelations = new List<Relation>();

        //    List<PlanetId> planets = Planet.All.Keys.ToList();

        //    for (int i = 9; i >= 1; i--)
        //    {
        //        for (int j = i - 1; j >= 0; j--)
        //        {
        //            List<Relation> relations = Relation.RelationsDuring(planets[i], planets[j], during);

        //            allRelations.AddRange(relations);
        //        }
        //    }

        //    return allRelations;
        //}

        public static string BriefOf(List<Relation> relations)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Relation rel in relations)
            {
                sb.AppendFormat("{0}{1}{2}, ", Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior));
            }

            return sb.ToString();
        }

        #endregion

        #region Fields and Properties
        public String Description
        {
            get {
                bool superiorFaster = Math.Abs(SuperiorPosition.LongitudeVelocity) > Math.Abs(InferiorPosition.LongitudeVelocity);
                int sIndex = (Flag.IsSuperiorRetrograde ? 0 : 2) + (superiorFaster ? 1 : 0);
                char superiorSpeed = TrendChars[sIndex];
                int iIndex = (Flag.IsInteriorRetrograde ? 0 : 2) + (superiorFaster ? 0 : 1);
                char interiorSpeed = TrendChars[iIndex];
                return String.Format("{0}{1}{2}{3}{4}{5}{6}",
                    Planet.SymbolOf(Superior), superiorSpeed, Flag.IsExpanding?'[': ']', Aspect,
                    Flag.IsExpanding?']': '[', interiorSpeed, Planet.SymbolOf(Inferior));
            }
        }

        public DateTimeOffset Moment { get; private set; }

        public RelationFlag Flag { get; private set; }

        public Position SuperiorPosition { get; private set; }

        public Position InferiorPosition { get; private set; }

        public PlanetId Superior { get { return SuperiorPosition.Owner; } }

        public PlanetId Inferior { get { return InferiorPosition.Owner; } }

        public Aspects Aspect { get; private set; }

        public Angle ReferenceAngle { get; private set; }

        public double Orb { get; private set; }

        public bool IsExactly { get { return Aspect != null && Math.Abs(Orb) < Aspects.DefaultMaxOrb; } }

        #endregion

        #region constructors
        public Relation(DateTimeOffset moment, Position one, Position another)
        {
            Moment = moment;
            if (one.Owner > another.Owner)
            {
                SuperiorPosition = one;
                InferiorPosition = another;
            }
            else
            {
                SuperiorPosition = another;
                InferiorPosition = one;
            }
            Aspect = Aspects.AspectBetween(SuperiorPosition.Longitude, InferiorPosition.Longitude);

            ReferenceAngle = new Rectascension(InferiorPosition.Longitude - SuperiorPosition.Longitude).Reference;

            if (Aspect != null)
            {
                Orb = Aspect.OrbOf(InferiorPosition.Longitude - SuperiorPosition.Longitude);
                Flag = new RelationFlag(this);
            }
        }

        #endregion

        #region Functions
        public override string ToString()
        {
            //return String.Format("{0} : {1}\tAspect={2}, \tOrb={3}", SuperiorPosition, InferiorPosition, Type, Orb);
            string orbString = ", Orb=" + Orb.ToString("F5");
            return string.Format("{0}: {1} {2} {3}{4}", Moment, SuperiorPosition, Aspect, InferiorPosition, IsExactly ? "" : orbString);
        }

        #endregion

    }
}
