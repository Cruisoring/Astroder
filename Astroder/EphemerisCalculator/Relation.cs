using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum RelationType
    {
        Apparent,
        ArithmeticMean,
        GeometricMean,
        HarmonicMean,
        Hypotenuse
    }

    public class RelationBrief
    {
        public RelationType Type { get; set; }
        public Double Degree { get; set; }

        public RelationBrief(RelationType type, Double degree)
        {
            Type = type;
            Degree = degree;
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Type, Degree);
        }
    }

    public class Relation
    {
        public static List<PlanetPair> GeoConcernedSinglePositions = new List<PlanetPair>();

        public static List<PlanetPair> GeoConcernedSingleMovements = new List<PlanetPair>();

        public static List<PlanetPair> GeoConcernedDualPositions = new List<PlanetPair>();

        public static List<PlanetPair> GeoConcernedDualMovements = new List<PlanetPair>();

        public static List<PlanetPair> HelioConcernedSinglePositions = new List<PlanetPair>();

        public static List<PlanetPair> HelioConcernedSingleMovements = new List<PlanetPair>();

        public static List<PlanetPair> HelioConcernedDualPositions = new List<PlanetPair>();

        public static List<PlanetPair> HelioConcernedDualMovements = new List<PlanetPair>();

        static Relation()
        {
            for (int i = 0; i < Ephemeris.GeocentricLuminaries.Count-1; i ++)
            {
                PlanetId interior = Ephemeris.GeocentricLuminaries[i];

                if (interior == PlanetId.SE_MOON)
                    continue;

                if (interior == PlanetId.Five_Average)
                    break;

                GeoConcernedSinglePositions.Add(new PlanetPair(interior, PlanetId.SE_ECL_NUT));
                GeoConcernedSingleMovements.Add(new PlanetPair(interior, PlanetId.SE_ECL_NUT));

                for (int j = i + 1; j < Ephemeris.GeocentricLuminaries.Count - 1; j ++ )
                {
                    PlanetId exterior = Ephemeris.GeocentricLuminaries[j];

                    if (exterior == PlanetId.SE_MOON)
                        continue;
                    else if (interior == PlanetId.SE_PLUTO || exterior == PlanetId.Five_Average)
                        break;

                    PlanetPair pair = new PlanetPair(interior, exterior);

                    GeoConcernedDualPositions.Add(pair);
                    GeoConcernedDualMovements.Add(pair);
                }
            }

            GeoConcernedDualPositions.Sort();
            GeoConcernedDualPositions.Reverse();
            GeoConcernedDualMovements.Sort();
            GeoConcernedDualMovements.Reverse();


            for (int i = 0; i < Ephemeris.HeliocentricLuminaries.Count - 1; i++)
            {
                PlanetId interior = Ephemeris.HeliocentricLuminaries[i];

                if (interior == PlanetId.SE_MOON)
                    continue;

                if (interior == PlanetId.Five_Average)
                    break;

                HelioConcernedSinglePositions.Add(new PlanetPair(interior, PlanetId.SE_ECL_NUT));
                HelioConcernedSingleMovements.Add(new PlanetPair(interior, PlanetId.SE_ECL_NUT));

                for (int j = i + 1; j < Ephemeris.HeliocentricLuminaries.Count - 1; j++)
                {
                    PlanetId exterior = Ephemeris.GeocentricLuminaries[j];

                    if (exterior == PlanetId.SE_MOON)
                        continue;
                    else if (interior == PlanetId.SE_PLUTO || exterior == PlanetId.Five_Average)
                        break;

                    PlanetPair pair = new PlanetPair(interior, exterior);

                    HelioConcernedDualPositions.Add(pair);
                    HelioConcernedDualMovements.Add(pair);
                }
            }

            HelioConcernedDualPositions.Sort();
            HelioConcernedDualPositions.Reverse();
            HelioConcernedDualMovements.Sort();
            HelioConcernedDualMovements.Reverse();

        }

        public bool IsHeliocentric { get; set; }

        public bool IsMovement { get; set; }

        public PlanetPair Pair { get; set; }

        public RelationBrief Brief { get; set; }

        public double Degree1 { get; set; }

        public double Degree2 { get; set; }

        public Relation(PlanetPair pair, RelationBrief brief, double degree1, double degree2)
        {
            IsHeliocentric = false;
            IsMovement = false;
            Pair = pair;
            Brief = brief;
            Degree1 = degree1;
            Degree2 = degree2;
        }

        public Relation(bool isHeliocentric, bool isMovement, PlanetPair pair, RelationBrief brief, double degree1, double degree2)
        {
            IsHeliocentric = isHeliocentric;
            IsMovement = isMovement;
            Pair = pair;
            Brief = brief;
            Degree1 = degree1;
            Degree2 = degree2;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsHeliocentric)
                sb.Append("Helio ");
            sb.AppendFormat("{0} {1}: ", Pair, Brief);

            if(IsMovement)
                sb.AppendFormat("Move1={0:F2}, Move2={1:F2}", Degree1, Degree2);
            else
                sb.AppendFormat("Pos1={0:F2}, Pos2={1:F2}", Degree1, Degree2);

            return sb.ToString();
        }
    }


    public delegate double angleDelegate(List<double> aspects, double orb, double degree1, double degree2);

    public static class RelationDetector
    {
        #region Low level functions
        public static double DistanceOf(double degree1, double degree2)
        {
            return (360 + degree1 - degree2) % 360;
        }

        public static double ArithmeticMeanOf(double degree1, double degree2)
        {
            return (degree1 + degree2) / 2;
        }

        public static double GeometricMeanOf(double degree1, double degree2)
        {
            return Math.Sqrt(degree1 * degree2);
        }

        public static double HarmonicMeanOf(double degree1, double degree2)
        {
            return 2 * degree1 * degree2 / (degree2 + degree1);
        }

        public static double HypotenuseOf(double degree1, double degree2)
        {
            return Math.Sqrt(degree1 * degree1 + degree2 * degree2) % 360;
        }

        private static double aspectAngleOf(List<double> aspects, double orb, double outcome)
        {
            double aspectDegree = (from deg in aspects
                                   orderby Math.Abs(outcome - deg)
                                   select deg).FirstOrDefault();

            if (Math.Abs(aspectDegree - outcome) < orb)
                return aspectDegree;
            else
                return -1;

        }

        #endregion

        #region Middle level functions

        public static double AngleOfApparent(List<double> aspects, double orb, double degree1, double degree2)
        {
            return aspectAngleOf(aspects, orb, DistanceOf(degree1, degree2));
        }

        public static double AngleOfArithmeticMean(List<double> aspects, double orb, double degree1, double degree2)
        {
            return aspectAngleOf(aspects, orb, ArithmeticMeanOf(degree1, degree2));
        }

        public static double AngleOfGeometricMean(List<double> aspects, double orb, double degree1, double degree2)
        {
            return aspectAngleOf(aspects, orb, GeometricMeanOf(degree1, degree2));
        }

        public static double AngleOfHarmonicMean(List<double> aspects, double orb, double degree1, double degree2)
        {
            if (Math.Abs(degree2) < 1 || Math.Abs(degree1) < 1)
                return -1;

            return aspectAngleOf(aspects, orb, HarmonicMeanOf(degree1, degree2));
        }

        public static double AngleOfHypotenuse(List<double> aspects, double orb, double degree1, double degree2)
        {
            return aspectAngleOf(aspects, orb, HypotenuseOf(degree1, degree2));
        }

        #endregion

        public static Dictionary<RelationType, angleDelegate> relationDelegates = new Dictionary<RelationType, angleDelegate>()
        {
            { RelationType.Apparent, AngleOfApparent},
            { RelationType.ArithmeticMean, AngleOfArithmeticMean},
            { RelationType.GeometricMean, AngleOfGeometricMean},
            { RelationType.HarmonicMean, AngleOfHarmonicMean},
            { RelationType.Hypotenuse, AngleOfHypotenuse}
        };

        public static List<angleDelegate> positionComparerDelegates = new List<angleDelegate>()
        {
            AngleOfApparent, AngleOfGeometricMean, AngleOfHarmonicMean, AngleOfHypotenuse
        };

        public static List<angleDelegate> aspectsComparerDelegates = new List<angleDelegate>()
        {
            AngleOfApparent, AngleOfGeometricMean, AngleOfHarmonicMean, AngleOfHypotenuse
        };

        public static List<angleDelegate> movementsComparerDelegates = new List<angleDelegate>()
        {
            AngleOfApparent, AngleOfGeometricMean, AngleOfHarmonicMean, AngleOfHypotenuse
        };


        public static List<RelationBrief> RelationBriefsOf(PlanetPair pair, double degree1, double degree2)
        {
            List<RelationBrief> result = new List<RelationBrief>();

            double orb = pair.Orb;
            List<double> aspects = pair.ConcernedAspects;

            double angle;

            foreach (KeyValuePair<RelationType, angleDelegate> kvp in relationDelegates)
            {
                angle = kvp.Value(aspects, orb, degree1, degree2);
                if (angle != -1)
                {
                    RelationBrief newRelation = new RelationBrief(kvp.Key, angle);
                    result.Add(newRelation);
                }
            }

            return result;

            //double distance, geometric, harmonic, hypotenuse, aspect;

            //distance = DistanceOf(degree1, degree2);
            //aspect = aspectAngleOf(aspects, orb, distance);

            //if (aspect != -1)
            //{
            //    RelationBrief distanceRelation = new RelationBrief(RelationType.Apparent, aspect);
            //    aspects.Add(distanceRelation);
            //}

            //geometric = GeometricMeanOf(degree1, degree2);
            //aspect = aspectAngleOf(aspects, orb, geometric);

            //if (aspect != -1)
            //{
            //    RelationBrief geometricRelation = new RelationBrief(RelationType.GeometricMean, aspect);
            //    aspects.Add(geometricRelation);
            //}

            //harmonic = HarmonicMeanOf(degree1, degree2);
            //aspect = aspectAngleOf(aspects, orb, distance);

            //if (aspect != -1)
            //{
            //    RelationBrief distanceRelation = new RelationBrief(RelationType.Apparent, aspect);
            //    aspects.Add(distanceRelation);
            //}

            //geometric = GeometricMeanOf(degree1, degree2);
            //aspect = aspectAngleOf(aspects, orb, geometric);

            //if (aspect != -1)
            //{
            //    RelationBrief geometricRelation = new RelationBrief(RelationType.GeometricMean, aspect);
            //    aspects.Add(geometricRelation);
            //}
        }


        public static List<Relation> RelationsBetween(PlanetPair pair, DateTimeOffset time1, DateTimeOffset time2)
        {
            double degree1, degree2, movement1, movement2;

            if (pair.Interior == PlanetId.SE_ECL_NUT || time1 == time2)
                return null;

            List<RelationBrief> briefs = null;
            List<Relation> result = new List<Relation>();

            //For single planet
            if (pair.Exterior == PlanetId.SE_ECL_NUT)
            {
                degree1 = Ephemeris.GeocentricPositionOf(time1, pair.Interior).Longitude;
                degree2 = Ephemeris.GeocentricPositionOf(time2, pair.Interior).Longitude;
                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(pair, brief, degree1, degree2));
                }

                degree1 = Ephemeris.HeliocentricPositionOf(time1, pair.Interior).Longitude;
                degree2 = Ephemeris.HeliocentricPositionOf(time2, pair.Interior).Longitude;
                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(true, false, pair, brief, degree1, degree2));
                }
            }
            else
            {
                //Geocentric position relations
                double pos1 = Ephemeris.GeocentricPositionOf(time1, pair.Interior).Longitude;
                double pos2 = Ephemeris.GeocentricPositionOf(time1, pair.Exterior).Longitude;
                degree1 = pos2 - pos1;

                pos1 = Ephemeris.GeocentricPositionOf(time2, pair.Interior).Longitude;
                pos2 = Ephemeris.GeocentricPositionOf(time2, pair.Exterior).Longitude;
                degree2 = pos2 - pos1;
                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(pair, brief, degree1, degree2));
                }

                //Geocentric movements relations
                movement1 = Movements.MovementOf(pair.Interior, false, time1, time2);
                movement2 = Movements.MovementOf(pair.Exterior, false, time1, time2);
                briefs = RelationBriefsOf(pair, movement1, movement2);

                foreach (RelationBrief  brief in briefs)
                {
                    result.Add(new Relation(false, true, pair, brief, movement1, movement2));
                }

                //Heliocentric position relations
                pos1 = Ephemeris.HeliocentricPositionOf(time1, pair.Interior).Longitude;
                pos2 = Ephemeris.HeliocentricPositionOf(time1, pair.Exterior).Longitude;
                degree1 = pos2 - pos1;

                pos1 = Ephemeris.HeliocentricPositionOf(time2, pair.Interior).Longitude;
                pos2 = Ephemeris.HeliocentricPositionOf(time2, pair.Exterior).Longitude;
                degree2 = pos2 - pos1;
                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(true, false, pair, brief, degree1, degree2));
                }

                //Heliocentric movements relations
                movement1 = Movements.MovementOf(pair.Interior, true, time1, time2);
                movement2 = Movements.MovementOf(pair.Exterior, true, time1, time2);
                briefs = RelationBriefsOf(pair, movement1, movement2);

                foreach (RelationBrief  brief in briefs)
                {
                    result.Add(new Relation(true, true, pair, brief, movement1, movement2));
                }

            }


            return result;
        }

        public static Dictionary<PlanetPair, List<Relation>> GeoRelationsOf(DateTimeOffset time1, DateTimeOffset time2)
        {
            Dictionary<PlanetPair, List<Relation>> finalResults = new Dictionary<PlanetPair, List<Relation>>();

            Dictionary<PlanetId, double> positions1 = new Dictionary<PlanetId, double>();
            Dictionary<PlanetId, double> positions2 = new Dictionary<PlanetId, double>();
            Dictionary<PlanetId, double> movements = new Dictionary<PlanetId, double>();

            DateTimeOffset earlier = time1 < time2 ? time1 : time2;
            DateTimeOffset later = time1 < time2 ? time2 : time1;

            foreach (PlanetId id in Ephemeris.GeocentricLuminaries)
            {
                double pos1 = Ephemeris.GeocentricPositionOf(earlier, id).Longitude;
                positions1.Add(id, pos1);

                double pos2 = Ephemeris.GeocentricPositionOf(later, id).Longitude;
                positions2.Add(id, pos2);

                TimeSpan duration = later - earlier;

                double period = duration.TotalDays / Planet.OrbitalPeriods[id];
                double round = period * 360;

                double temp = pos2 - pos1;
                if (temp < 0 && (period > 0.4 || duration.Days > Planet.RetrogradePeriods[id]))
                    temp += 360;
                else if (temp > 300 && period < 0.4)
                    temp -= 360;

                double movement = Math.Round((round - temp) / 360) * 360 + temp;
                movements.Add(id, movement);

                if (id == PlanetId.SE_PLUTO)
                    break;
            }

            double degree1, degree2;
            List<RelationBrief> briefs = null;

            foreach (PlanetPair pair in Relation.GeoConcernedSinglePositions)
            {
                List<Relation> result = new List<Relation>();

                degree1 = positions1[pair.Interior];
                degree2 = positions2[pair.Interior];
                briefs = RelationBriefsOf(pair, degree1, degree2);

                if (briefs.Count != 0)
                {
                    foreach (RelationBrief brief in briefs)
                    {
                        result.Add(new Relation(pair, brief, degree1, degree2));
                    }

                    finalResults.Add(pair, result);
                }

            }

            foreach (PlanetPair pair in Relation.GeoConcernedDualPositions)
            {
                List<Relation> result = new List<Relation>();

                //Geocentric position relations
                degree1 = positions1[pair.Exterior] - positions1[pair.Interior];
                degree2 = positions2[pair.Exterior] - positions2[pair.Interior];

                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(pair, brief, degree1, degree2));
                }

                //Geocentric movements relations
                degree1 = movements[pair.Interior];
                degree2 = movements[pair.Exterior];
                briefs = RelationBriefsOf(pair, degree1, degree2);

                foreach (RelationBrief brief in briefs)
                {
                    result.Add(new Relation(false, true, pair, brief, degree1, degree2));
                }

                if (result.Count != 0)
                {
                    finalResults.Add(pair, result);
                }
            }

            return finalResults;

        }
    }
}
