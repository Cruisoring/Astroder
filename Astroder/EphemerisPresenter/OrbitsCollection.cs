using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    [Serializable]
    public class OrbitsCollection
    {
        public static Double NormalIntervalDays = 1;
        public const Double SparsedInterval = 30;

        #region Field and Properties
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public List<Double> DayByDay { get; set; }
        public List<Double> MonthByMonth { get; set; }

        public Dictionary<PlanetId, Dictionary<OrbitInfoType, List<Double>>> AllOrbits { get; set; }

        #endregion

        public OrbitsCollection(DateTimeOffset since, DateTimeOffset until, Ephemeris ephemeris)
        {
            Start = since;
            End = until;

            //Longitudes = new Dictionary<PlanetId, List<Double>>();
            //Latitudes = new Dictionary<PlanetId, List<double>>();
            //Distances = new Dictionary<PlanetId, List<double>>();
            //LongitudeVelocities = new Dictionary<PlanetId, List<double>>();
            //LatitudeVelocities = new Dictionary<PlanetId, List<double>>();
            //DistanceVelocities = new Dictionary<PlanetId, List<double>>();

            //for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id ++ )
            //{
            //    Longitudes.Add(id, new List<double>());
            //    Latitudes.Add(id, new List<double>());
            //    Distances.Add(id, new List<double>());
            //    LongitudeVelocities.Add(id, new List<double>());
            //    LatitudeVelocities.Add(id, new List<double>());
            //    DistanceVelocities.Add(id, new List<double>());
            //}

            ephemeris.Load(since, until);

            AllOrbits = new Dictionary<PlanetId, Dictionary<OrbitInfoType, List<double>>>();

            for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_CHIRON; id++)
            {
                if (id > PlanetId.SE_PLUTO && id != PlanetId.SE_CHIRON)
                    continue;

                Dictionary<OrbitInfoType, List<Double>> newOrbit = new Dictionary<OrbitInfoType, List<Double>>();
                newOrbit.Add(OrbitInfoType.Longitude, new List<double>());
                newOrbit.Add(OrbitInfoType.Latitude, new List<double>());
                newOrbit.Add(OrbitInfoType.Distance, new List<double>());
                newOrbit.Add(OrbitInfoType.LongitudeVelocities, new List<double>());
                newOrbit.Add(OrbitInfoType.LatitudeVelocities, new List<double>());
                newOrbit.Add(OrbitInfoType.DistanceVelocities, new List<double>());

                AllOrbits.Add(id, newOrbit);
            }

            TimeSpan interval = TimeSpan.FromDays(NormalIntervalDays);

            DayByDay = new List<Double>();
            MonthByMonth = new List<Double>();

            for (DateTimeOffset date = Start; date <= End; date += TimeSpan.FromDays(SparsedInterval))
            {
                MonthByMonth.Add(date.UtcDateTime.ToOADate());
            }

            for (DateTimeOffset date = since; date <= until;  date += interval)
            {
                List<Position> positions = ephemeris[date];
                DayByDay.Add(date.UtcDateTime.ToOADate());
                Position pos;

                for (int i = (int)PlanetId.SE_SUN; i <= (int)PlanetId.SE_PLUTO; i ++ )
                {
                    pos = positions[i];
                    PlanetId id = pos.Owner;

                    AllOrbits[id][OrbitInfoType.Longitude].Add(pos.Longitude);
                    AllOrbits[id][OrbitInfoType.Latitude].Add(pos.Latitude);
                    AllOrbits[id][OrbitInfoType.Distance].Add(pos.Distance);
                    AllOrbits[id][OrbitInfoType.LongitudeVelocities].Add(pos.LongitudeVelocity);
                    AllOrbits[id][OrbitInfoType.LatitudeVelocities].Add(pos.LatitudeVelocity);
                    AllOrbits[id][OrbitInfoType.DistanceVelocities].Add(pos.DistanceVelocity);
                }

                pos = positions.Last();
                if (pos.Owner == PlanetId.SE_CHIRON)
                {
                    Dictionary<OrbitInfoType, List<Double>> values = AllOrbits[PlanetId.SE_CHIRON];
                    values[OrbitInfoType.Longitude].Add(pos.Longitude);
                    values[OrbitInfoType.Latitude].Add(pos.Latitude);
                    values[OrbitInfoType.Distance].Add(pos.Distance);
                    values[OrbitInfoType.LongitudeVelocities].Add(pos.LongitudeVelocity);
                    values[OrbitInfoType.LatitudeVelocities].Add(pos.LatitudeVelocity);
                    values[OrbitInfoType.DistanceVelocities].Add(pos.DistanceVelocity);
                }
            }

            //ascendingOrbits = new Dictionary<PlanetId, List<double>>();
            //descendingOrbits = new Dictionary<PlanetId, List<double>>();
            //perigeeOrbits = new Dictionary<PlanetId, List<double>>();
            //apogeeOrbits = new Dictionary<PlanetId, List<double>>();

            //ascendingHeights = new Dictionary<PlanetId, List<double>>();
            //descendingHeights = new Dictionary<PlanetId, List<double>>();
            //perigeeHeights = new Dictionary<PlanetId, List<double>>();
            //apogeeHeights = new Dictionary<PlanetId, List<double>>();

            //ascendingVelocities = new Dictionary<PlanetId, List<double>>();
            //descendingVelocities = new Dictionary<PlanetId, List<double>>();
            //perigeeVelocities = new Dictionary<PlanetId, List<double>>();
            //apogeeVelocities = new Dictionary<PlanetId, List<double>>();
        }

        #region Functions
        public OrbitSpec this[PlanetId star, OrbitInfoType type]
        {
            get
            {
                List<double> yList, xList;

                if (!AllOrbits.ContainsKey(star))
                    throw new Exception();

                Dictionary<OrbitInfoType, List<double>> planetsOrbits = AllOrbits[star];

                if (type >= OrbitInfoType.Longitude && type <= OrbitInfoType.DistanceVelocities)
                {
                    if (planetsOrbits.ContainsKey(type))
                    {
                        yList = planetsOrbits[type];
                        xList = DayByDay;
                        return new OrbitSpec(star, type, xList, yList);
                    }
                    else
                        throw new Exception();
                }
                else if (type == OrbitInfoType.LongitudeAcceleration)
                {
                    yList = new List<double>();
                    yList.Add(0);

                    List<double> longVelocities = planetsOrbits[OrbitInfoType.LongitudeVelocities];
                    //double lvRange = longVelocities.Max() - longVelocities.Min();

                    for (int i = 1; i < longVelocities.Count; i ++ )
                    {
                        yList.Add(100 * (longVelocities[i] - longVelocities[i-1]));
                    }
                    xList = DayByDay;
                    return new OrbitSpec(star, type, xList, yList);
                }
                else if (type == OrbitInfoType.LongVelocityAndLatitude)
                {
                    yList = new List<double>();
                    List<double> latitudes = planetsOrbits[OrbitInfoType.Latitude];
                    List<double> longVelocities = planetsOrbits[OrbitInfoType.LongitudeVelocities];

                    double latRange = latitudes.Max() - latitudes.Min();
                    double lvRange = longVelocities.Max() - longVelocities.Min();
                    double latFactor = lvRange / latRange;

                    for (int i = 0; i < latitudes.Count; i ++ )
                    {
                        //yList.Add(latitudes[i] + longVelocities[i]);
                        yList.Add(latitudes[i] * latFactor + longVelocities[i]);
                    }
                    xList = DayByDay;
                    return new OrbitSpec(star, type, xList, yList);
                }                else if (type >= OrbitInfoType.Ascending && type <= OrbitInfoType.ApogeeVelocities)
                {
                    if (!planetsOrbits.ContainsKey(type))
                    {
                        loadNodeApsides(star);
                    }

                    yList = planetsOrbits[type];
                    xList = MonthByMonth;

                    if (xList.Count != yList.Count || yList.Count == 0)
                        return null;

                    return new OrbitSpec(star, type, xList, yList);
                }
                else
                    throw new Exception();


                //Dictionary<PlanetId, List<Double>> collection = null;

                //switch(type)
                //{
                //    //case OrbitInfoType.All:
                //    case OrbitInfoType.Longitude: 
                //        collection = Longitudes; 
                //        break;
                //    case OrbitInfoType.Latitude:
                //        collection = Latitudes;
                //        break;
                //    case OrbitInfoType.Distance: 
                //        collection = Distances;
                //        break;
                //    case OrbitInfoType.LongitudeVelocities: 
                //        collection = LongitudeVelocities;
                //        break;
                //    case OrbitInfoType.LatitudeVelocities: 
                //        collection = LatitudeVelocities;
                //        break;
                //    case OrbitInfoType.DistanceVelocities: 
                //        collection = DistanceVelocities;
                //        break;
                //    case OrbitInfoType.Ascending: 
                //        collection = ascendingOrbits;
                //        break;
                //    case OrbitInfoType.Descending: 
                //        collection = descendingOrbits;
                //        break;
                //    case OrbitInfoType.Perigee: 
                //        collection = perigeeOrbits;
                //        break;
                //    case OrbitInfoType.Apogee: 
                //        collection = apogeeOrbits;
                //        break;
                //    case OrbitInfoType.AscendingLatitude: 
                //        collection = ascendingHeights;
                //        break;
                //    case OrbitInfoType.DescendingLatitude: 
                //        collection = descendingHeights;
                //        break;
                //    case OrbitInfoType.PerigeeLatitude: 
                //        collection = perigeeHeights;
                //        break;
                //    case OrbitInfoType.ApogeeLatitude: 
                //        collection = apogeeHeights;
                //        break;
                //    case OrbitInfoType.AscendingVelocities: 
                //        collection = ascendingVelocities;
                //        break;
                //    case OrbitInfoType.DescendingVelocities: 
                //        collection = descendingVelocities;
                //        break;
                //    case OrbitInfoType.PerigeeVelocities: 
                //        collection = perigeeVelocities;
                //        break;
                //    case OrbitInfoType.ApogeeVelocities: 
                //        collection = apogeeVelocities;
                //        break;
                //    default:
                //        throw new NotImplementedException();
                //}

                //if (type >= OrbitInfoType.Ascending && !collection.ContainsKey(star))
                //{
                //    loadNodeApsides(star);
                //}

                //if (collection.ContainsKey(star))
                //    return collection[star];
                //else
                //    throw new Exception();
            }
        }

        public bool ContainsOrbit(PlanetId id)
        {
            return AllOrbits.ContainsKey(id);
        }

        private void loadNodeApsides(PlanetId star)
        {
            double[] xnasc = new double[6];
            double[] xndsc = new double[6];
            double[] xperi = new double[6];
            double[] xaphe = new double[6];
            String errorMsg = "";

            Dictionary<OrbitInfoType, List<Double>> planetOrbits = AllOrbits[star];

            for(OrbitInfoType type = OrbitInfoType.Ascending; type <= OrbitInfoType.ApogeeVelocities; type ++)
            {
                planetOrbits.Add(type, new List<double>());
            }

            double end = Utilities.ToJulianDay(End);

            for (double date = Utilities.ToJulianDay(Start); date <= end; date += SparsedInterval)
            {
                
                if (Utilities.swe_nod_aps_ut(date, star, SeFlg.SEFLG_SPEED | SeFlg.SEFLG_HELCTR, 0, xnasc, xndsc, xperi, xaphe, errorMsg) == SeFlg.ERR)
                    throw new Exception();
                else
                {
                    planetOrbits[OrbitInfoType.Ascending].Add(xnasc[0]);
                    planetOrbits[OrbitInfoType.AscendingLatitude].Add(xnasc[1]);
                    planetOrbits[OrbitInfoType.AscendingVelocities].Add(xnasc[3]);

                    planetOrbits[OrbitInfoType.Descending].Add(xndsc[0]);
                    planetOrbits[OrbitInfoType.DescendingLatitude].Add(xndsc[1]);
                    planetOrbits[OrbitInfoType.DescendingVelocities].Add(xndsc[3]);

                    planetOrbits[OrbitInfoType.Perigee].Add(xperi[0]);
                    planetOrbits[OrbitInfoType.PerigeeLatitude].Add(xperi[1]);
                    planetOrbits[OrbitInfoType.PerigeeVelocities].Add(xperi[3]);

                    planetOrbits[OrbitInfoType.Apogee].Add(xaphe[0]);
                    planetOrbits[OrbitInfoType.ApogeeLatitude].Add(xaphe[1]);
                    planetOrbits[OrbitInfoType.ApogeeVelocities].Add(xaphe[3]);

                }
            }
        }

        #endregion
    }
}
