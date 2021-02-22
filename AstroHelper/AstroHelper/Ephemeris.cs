using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NumberHelper;
using NumberHelper.DoubleHelper;

namespace AstroHelper
{
    [Serializable]
    public class Ephemeris 
    {
        public const Double Negligible = 0.001;
        public const Double AverageYearLength = 365.256363004;
        public const Double SunSpeed = 360 / AverageYearLength;

        #region Static region
        private static List<PlanetId> luminous = null;

        public static List<PlanetId> Luminous
        {
            get
            {
                if (luminous == null)
                {
                    luminous = new List<PlanetId>();

                    for (PlanetId id = PlanetId.SE_SUN; id < PlanetId.SE_MEAN_NODE; id++)
                    {
                        luminous.Add(id);
                    }
                    luminous.Add(PlanetId.SE_NORTHNODE);
                    luminous.Add(PlanetId.SE_CHIRON);
                }

                return luminous;
            }
        }

        public static SearchMode DefaultSearchMode = SearchMode.AroundWorkingDay;

        public static List<PlanetId> Masters = new List<PlanetId> { 
            PlanetId.SE_SUN, 
            //PlanetId.SE_MOON,
            PlanetId.SE_MERCURY, PlanetId.SE_VENUS, PlanetId.SE_MARS,
            PlanetId.SE_JUPITER, PlanetId.SE_SATURN, 
            PlanetId.SE_URANUS, PlanetId.SE_NEPTUNE, PlanetId.SE_PLUTO,
            PlanetId.SE_NORTHNODE, PlanetId.SE_CHIRON
        };

        private static DirectoryInfo ephemerisDirectory;

        public static DirectoryInfo EphemerisDirectory
        {
            get 
            {
                if (ephemerisDirectory == null)
                {
                    string dirPath = @"C:\sweph\Ephemeris\";
                    //string dirPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Ephemeris\";

                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    ephemerisDirectory = new DirectoryInfo(dirPath);
                } 
                return ephemerisDirectory; 
            }
        }

        private static Ephemeris currentEphemeris = null;        
        public static Ephemeris CurrentEphemeris
        {
            get
            {
                if (currentEphemeris == null)
                {
                    currentEphemeris = Geocentric;
                }
                return currentEphemeris;
            }
            set
            {
                currentEphemeris = value;
            }
        }

        private static Ephemeris geocentric = new Ephemeris("Geocentric", DateTimeOffset.Now);
        public static Ephemeris Geocentric {
            get
            {
                return geocentric;
            }
        }

        private static Ephemeris heliocentric = 
            new Ephemeris("Heliocentric", new PositionLookupDelegate(Utilities.HeliocentricPositionOfJulian), DateTimeOffset.Now);
        public static Ephemeris Heliocentric
        {
            get
            {
                return heliocentric;
            }
        }

        public static Dictionary<int, DateTimeOffset> Vernals = null;

        //static Ephemeris()
        //{
        //    try
        //    {
        //        luminous = new List<PlanetId>();

        //        for (PlanetId id = PlanetId.SE_SUN; id < PlanetId.SE_MEAN_NODE; id++)
        //        {
        //            luminous.Add(id);
        //        }
        //        luminous.Add(PlanetId.SE_NORTHNODE);
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Something wrong!");
        //    }
        //}

        //public static DateTimeOffset ExactlyTimeOf(PlanetId id, DateTimeOffset refDate, Rectascension destination)
        //{
        //    double jul_ut = Utilities.ToJulianDay(refDate);
        //    double averageSpeed = Planet.AverageSpeedOf(id);
        //    Position posAverage, posDynamic = PositionDelegate(jul_ut, id);

        //    double shiftDynamic, shiftAverage;
        //    double dest = destination.Degrees;

        //    double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);

        //    if (distanceDynamic < 0) distanceDynamic += 360;

        //    double lastDistance = 360, lastShift, distanceAverage = distanceDynamic;

        //    shiftDynamic = distanceDynamic / (posDynamic.LongitudeVelocity < 0 ? averageSpeed : posDynamic.LongitudeVelocity);
        //    shiftAverage = distanceAverage / averageSpeed;

        //    for (int i = 0; i < 10; i++)
        //    {
        //        posDynamic = PositionDelegate(jul_ut + shiftDynamic, id);
        //        posAverage = PositionDelegate(jul_ut + shiftAverage, id);

        //        distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);
        //        distanceAverage = Angle.BeelineOf(posAverage.Longitude, dest);


        //        if (Math.Abs(distanceAverage) > Math.Abs(distanceDynamic))
        //        {
        //            if (Math.Abs(distanceDynamic) < Negligible)
        //                return Utilities.UtcFromJulianDay(jul_ut);

        //            jul_ut += shiftDynamic;
        //            lastShift = shiftDynamic;
        //            distanceAverage = distanceDynamic;
        //            posAverage = posDynamic;
        //            lastDistance = distanceDynamic;
        //        }
        //        else
        //        {
        //            if (Math.Abs(distanceAverage) < Negligible)
        //                return Utilities.UtcFromJulianDay(jul_ut);

        //            jul_ut += shiftAverage;
        //            lastShift = shiftAverage;
        //            distanceDynamic = distanceAverage;
        //            posDynamic = posAverage;
        //            lastDistance = distanceAverage;

        //        }

        //        shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //        shiftAverage = distanceAverage / averageSpeed;

        //    }

        //    throw new Exception("Failed destination get the exact time!");
        //}

        //public static Angle AngleOfTime(DateTimeOffset time)
        //{
        //    DateTimeOffset equinox = VernalEquinoxTimeOf(time.Year);

        //    Double daysPassed = (time - equinox).TotalDays;

        //    if (daysPassed < 0)
        //        daysPassed += AverageYearLength;

        //    return new Angle( (daysPassed / AverageYearLength) * 360 );
        //}

        public static DateTimeOffset DateOfPlanetPosition(PlanetId star, DateTimeOffset since, Rectascension destination)
        {
            DateTimeOffset date;
            Position posPrev, posNext;
            Double difPrev, difNext;

            posPrev = CurrentEphemeris[since, star];
            difPrev = Angle.DistanceBetween(posPrev.Longitude, destination.Degrees);

            if (difPrev > 60)
                throw new ArgumentOutOfRangeException("The date of starting search is not set properly.");
            else if (difPrev < Negligible)
                return since;

            for (int i = 1; i < 200; i ++ )
            {
                date = since.AddDays(i);
                posNext = CurrentEphemeris[date, star];

                difNext = Angle.DistanceBetween(posNext.Longitude, destination.Degrees);

                if (difPrev <= difNext)
                {
                    return date.AddDays(-1);
                }
                else if (difPrev < Angle.DistanceBetween(posNext.Longitude, posPrev.Longitude))
                {
                    return date.AddDays(-1);
                }

                posPrev = posNext;
                difPrev = difNext;
            }

            return DateTimeOffset.MinValue;

            //Double cusp = destination.Within.Cusp;
            //Sign destSign = destination.Within;
            //Position pos1, pos2;

            //DateTimeOffset date1, date2, enter, exit;

            //enter = entryDateAround(star, destSign, since);
            //exit = entryDateAround(star, destSign + 1, enter);

            //for (date1 = enter.AddDays(-1); date1 < exit; date1 += TimeSpan.FromDays(1) )
            //{
            //    date2 = date1.AddDays(1);
            //    pos1 = Geocentric[date1, star];
            //    pos2 = Geocentric[date2, star];

            //    Double b2 = Angle.BeelineOf(pos2.Longitude, destination.Degrees);
            //    Double b1 = Angle.BeelineOf(pos1.Longitude, destination.Degrees);

            //    if (b2 > 0 || b1 < 0)
            //        continue;

            //    return date2;
            //}

            throw new Exception();
        }

        private static List<double> TurningsOf(List<double> velocities)
        {
            List<Double> result = new List<Double>();

            double last = velocities[0];
            for (int i = 1; i < velocities.Count; i ++ )
            {
                if (last * velocities[i] < 0)
                {
                    result.Add(velocities[i-1]);
                    last = velocities[i];
                }
            }

            return result;
        }

        //private static DateTimeOffset RetroOrDirectDate(PlanetId star, DateTimeOffset around, int period)
        //{
        //    if (!Planet.RetrogradePeriods.ContainsKey(star))
        //        return DateTimeOffset.MinValue;

        //    DateTimeOffset date = around;

        //    Position pos = Geocentric[around, star];

        //    if (pos.LongitudeVelocity < 0)
        //        date -= TimeSpan.FromDays((int)(period));

        //    List<DateTimeOffset> watched = new List<DateTimeOffset>();

        //    for (int i = -period; i <= period; i++)
        //    {
        //        watched.Add(date.AddDays(i));
        //    }

        //    List<double> longVelocities = new List<double>();
        //    List<double> longitudes = new List<double>();

        //    foreach (DateTimeOffset day in watched)
        //    {
        //        pos = Geocentric[day, star];
        //        longVelocities.Add(pos.LongitudeVelocity);
        //        longitudes.Add(pos.Longitude);
        //    }

        //    List<double> reversedVelocities = TurningsOf(longVelocities);

        //    if (reversedVelocities.Count == 0)
        //        return DateTimeOffset.MinValue;
        //    else
        //        return watched[longVelocities.IndexOf(reversedVelocities[0])];
        //}

        public static List<DateTimeOffset> RetrogradeDateAround(PlanetId star, DateTimeOffset around)
        {
            if (!Planet.RetrogradePeriods.ContainsKey(star))
            {
                return null;
            }

            int period = (int)Planet.RetrogradePeriods[star];
            DateTimeOffset date = around;

            List<DateTimeOffset> results = new List<DateTimeOffset>();
            Double longRetro = -1, longDirect = -1;

            Position pos =  Geocentric[around, star];
            
            if (pos.LongitudeVelocity < 0)
                date -= TimeSpan.FromDays((int)(period/2));

            List<DateTimeOffset> watched = new List<DateTimeOffset>();

            for (int i = -period; i <= period; i ++)
            {
                watched.Add(date.AddDays(i));
            }

            //List<double> longVelocities = new List<double>();
            //List<double> longitudes = new List<double>();

            //foreach (DateTimeOffset day in watched)
            //{
            //    pos =  Geocentric[day, star];
            //    longVelocities.Add(pos.LongitudeVelocity);
            //    longitudes.Add(pos.Longitude);
            //}

            //List<double> reversedVelocities = TurningsOf(longVelocities);

            //if (reversedVelocities.Count == 2)
            //{
            //    int index = longVelocities.IndexOf(reversedVelocities[0]);
            //    pos = Geocentric[watched[index], star];
            //    if (reversedVelocities[0] > 0)
            //        longRetro = pos.Longitude;
            //    else
            //        longDirect = pos.Longitude;

            //    index = longVelocities.IndexOf(reversedVelocities[1]);
            //    pos = Geocentric[watched[index], star];
            //    if (reversedVelocities[1] > 0)
            //        longRetro = pos.Longitude;
            //    else
            //        longDirect = pos.Longitude;
            //}
            //else
            //    return null;


            if (pos.LongitudeVelocity >= 0)
            {
                for (int i = -2; i <= 2; i++)
                {
                    date = around.AddDays((int)(i * period / 2));
                    pos = Geocentric[date, star];

                    if (pos.LongitudeVelocity < 0)
                        break;
                }

                if (pos.LongitudeVelocity < 0)
                    around = date;
                else
                    return null;
            }

            for (int j = 1; j <= period + 5; j++)
            {
                date = around.AddDays(-j);
                pos = Geocentric[date, star];
                if (pos.LongitudeVelocity < 0)
                    continue;

                results.Add(date);
                longRetro = pos.Longitude;
                break;
            }

            for (int k = 1; k <= period + 5; k++)
            {
                date = around.AddDays(k);

                pos = Geocentric[date, star];
                if (pos.LongitudeVelocity < 0)
                    continue;

                results.Add(date.AddDays(-1));
                longDirect = pos.Longitude;
                break;
            }

            if (longDirect == -1 || longRetro == -1)
            {
                return null;
                //throw new Exception("Failed to get either direct or retrograde around, the retrograde period may not be proper?");
            }
            else
            {
                date = results[0].AddDays(-period - 10);
                DateTimeOffset temp = DateOfPlanetPosition(star, date, new Rectascension(longDirect));

                if (temp != DateTimeOffset.MinValue)
                    results.Add(temp);

                temp = DateOfPlanetPosition(star, results[1], new Rectascension(longRetro));

                if (temp != DateTimeOffset.MinValue)
                    results.Add(temp);
            }

            if (results.Count == 4)
            {
                results.Sort();
                return results;
            }
            else
            {
                return null;
                //throw new Exception("Something is wrong to the the retrograde related events?");
            }

        }

        private static DateTimeOffset entryDateAround(PlanetId star, Sign destSign, DateTimeOffset around)
        {
            Double speed = Planet.AverageSpeedOf(star);
            int dayAdjustment = 0;
            DateTimeOffset date = around.UtcDateTime.Date;
            Position pos = CurrentEphemeris[date, star];

            int dif = destSign.Order - Sign.SignOf(pos.Longitude).Order;
            dayAdjustment = dif > 0 ? (int)((destSign.Order - Sign.SignOf(pos.Longitude).Order - 1) * 30 / speed)
                : (int)((11 + destSign.Order - Sign.SignOf(pos.Longitude).Order) * 30 / speed);

            do
            {
                date = date.AddDays(dayAdjustment);
                pos = CurrentEphemeris[date, star];
                dayAdjustment--;
            } while (Sign.SignOf(pos.Longitude) != destSign.Previous);

            if (Sign.SignOf(pos.Longitude) != destSign.Previous)
                throw new Exception();

            do 
            {
                date = date.AddDays(1);
                pos = CurrentEphemeris[date, star];
                if (Sign.SignOf(pos.Longitude) == destSign)
                    return date;
            } while (true);

            throw new Exception();
        }

        public static DateTimeOffset VernalEquinoxTimeOf(int year)
        {
            if (Vernals == null)
                Vernals = new Dictionary<int, DateTimeOffset>();

            if (!Vernals.ContainsKey(year))
            {
                double jul_ut = Utilities.ToJulianDay(new DateTimeOffset(year, 3, 15, 12, 0, 0, TimeSpan.Zero));
                double averageSpeed = 360.0 / AverageYearLength;
                Position posDynamic = Geocentric.PositionDelegate(jul_ut, PlanetId.SE_SUN);

                double shiftDynamic;

                double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, 360);

                shiftDynamic = distanceDynamic / averageSpeed;

                for (int i = 0; i < 10; i++)
                {
                    posDynamic = Geocentric.PositionDelegate(jul_ut + shiftDynamic, PlanetId.SE_SUN);

                    distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, 360);

                    if (Math.Abs(distanceDynamic) < Negligible)
                        break;

                    jul_ut += shiftDynamic;

                    shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
                }

                Vernals.Add(year, Utilities.UtcFromJulianDay(jul_ut));
            }

            return Vernals[year];
        }

        public static List<Relation> RelationsOn(DateTimeOffset date)
        {
            List<Position> positions = CurrentEphemeris[date];

            List<Relation> allRelations = new List<Relation>();

            Position superiorPos, inferiorPos;
            Angle distance;
            Aspects aspect;

            for (int i = Masters.Count - 1; i > 0; i--)
            {
                PlanetId superiorId = Masters[i];
                superiorPos = CurrentEphemeris.PositionOf(positions, superiorId);

                for (int j = i - 1; j >= 0; j--)
                {
                    PlanetId inferiorId = Masters[j];
                    inferiorPos = CurrentEphemeris.PositionOf(positions, inferiorId);
                    distance = inferiorPos.Longitude - superiorPos.Longitude;
                    aspect = Aspects.CurrentAspectOf(distance);

                    if (aspect != null)
                        allRelations.Add(new Relation(date, superiorPos, inferiorPos));
                }
            }

            return allRelations;
        }

        public static List<Relation> RelationsOn(DateTimeOffset date, PlanetId refStar)
        {
            List<Relation> allRelations = new List<Relation>();

            Position refPos = CurrentEphemeris[date, refStar];

            Position anotherPos;
            Angle distance;
            Aspects aspect;

            for (PlanetId anotherId = PlanetId.SE_SUN; anotherId <= PlanetId.SE_CHIRON; anotherId ++ )
            {
                if (anotherId == refStar || !Masters.Contains(anotherId))
                    continue;

                anotherPos = CurrentEphemeris[date, anotherId];
                distance = refPos.Longitude - anotherPos.Longitude;

                aspect = Aspects.CurrentAspectOf(distance);

                if (aspect != null)
                    allRelations.Add(new Relation(date, anotherPos, refPos));
            }

            return allRelations;
            
            //List<Relation> allRelations = RelationsOn(around);
            //return (from relation in allRelations
            //        where relation.Inferior == refStar || relation.Superior == refStar
            //        select relation).ToList();
        }

        public static List<Relation> RelationsWithin(MatchRules period)
        {
            DateTimeOffset startDate = period.Since.UtcDateTime.Date;
            DateTimeOffset endDate = period.Until.UtcDateTime.Date;

            List<Relation> allRelations = new List<Relation>();

            for (DateTimeOffset date = startDate; date < endDate; date += TimeSpan.FromDays(1))
            {
                allRelations.AddRange(RelationsOn(date));
            }

            if (endDate - startDate <= TimeSpan.FromDays(1))
                return allRelations;

            allRelations = Optimize(allRelations);

            return allRelations;
        }

        public static List<Relation> RelationsWithin(MatchRules period, PlanetId oneParty)
        {
            DateTimeOffset startDate = period.Since.UtcDateTime.Date;
            DateTimeOffset endDate = period.Until.UtcDateTime.Date;

            List<Relation> allRelations = new List<Relation>();

            for (DateTimeOffset date = startDate; date < endDate; date += TimeSpan.FromDays(1))
            {
                allRelations.AddRange(RelationsOn(date, oneParty));
            }

            if (endDate - startDate <= TimeSpan.FromDays(1))
                return allRelations;

            allRelations = Optimize(allRelations);

            return allRelations;
        }

        public static List<Relation> Optimize(List<Relation> allRelations)
        {
            List<Relation> result = new List<Relation>();
            Relation newRelation = null;

            var relationClassifier =
                from relation in allRelations
                group relation by new { relation.Superior, relation.Inferior, relation.Flag.Around } into relationGroup
                orderby relationGroup.Key.Superior descending, relationGroup.Key.Inferior descending
                select relationGroup;


            foreach (var relations in relationClassifier)
            {
                if (relations.Count() == 0)
                    throw new Exception();
                else if (relations.Count() == 1)
                    result.Add(relations.First());
                else
                {
                    newRelation = null;
                    double orb = 100;
                    foreach (Relation relation in relations)
                    {
                        if (Math.Abs(relation.Orb) < orb)
                        {
                            orb = Math.Abs(relation.Orb);
                            newRelation = relation;
                        }
                    }
                    result.Add(newRelation);
                }
            }

            return result;
        }

        //public static void LoadEphemeris()
        //{
        //    string fileName = null;

        //    foreach (FileInfo fi in dir.GetFiles())
        //    {
        //        if (fi.Name.ToLower() != "geocentric.eph")
        //            continue;

        //        fileName = fi.FullName;
        //    }

        //    if (fileName != null)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        Geocentric = (Ephemeris)formatter.Deserialize(stream);
        //        stream.Close();
        //    }

        //    if (Geocentric == null)
        //    {
        //        Geocentric = new Ephemeris("Geocentric");
        //    }
        //}

        //public static DateTimeOffset ExactlyTimeOf(PlanetId id, DateTimeOffset refDate, Rectascension destination)
        //{
        //    double around = Utilities.ToJulianDay(refDate);
        //    double averageSpeed = AverageSpeedOf(id);
        //    Position posAverage, posDynamic = Utilities.GeocentricPositionOf(around, id, SeFlg.SEFLG_SPEED);

        //    double shiftDynamic, shiftAverage;
        //    double dest = destination.Degrees;

        //    double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);

        //    if (distanceDynamic < 0) distanceDynamic += 360;

        //    double lastDistance = 360, lastShift, minDistance, distanceAverage = distanceDynamic;

        //    shiftDynamic = distanceDynamic / (posDynamic.LongitudeVelocity < 0 ? averageSpeed : posDynamic.LongitudeVelocity);
        //    shiftAverage = Math.Abs(distanceAverage / averageSpeed);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        posDynamic = Utilities.GeocentricPositionOf(around + shiftDynamic, id, SeFlg.SEFLG_SPEED);
        //        posAverage = Utilities.GeocentricPositionOf(around + shiftAverage, id, SeFlg.GEOCENTRIC);

        //        distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);
        //        distanceAverage = Angle.BeelineOf(posAverage.Longitude.Degrees, dest);

        //        minDistance = (Math.Abs(distanceAverage) < Math.Abs(distanceDynamic)) ? Math.Abs(distanceAverage) : Math.Abs(distanceDynamic);

        //        if (minDistance > Math.Abs(lastDistance) && minDistance < 8)
        //        {
        //            around += 4 * majorStep;
        //            i++;

        //            posDynamic = Utilities.GeocentricPositionOf(around, id, SeFlg.SEFLG_SPEED);
        //            distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);

        //            shiftDynamic = Math.Abs(distanceDynamic / posDynamic.LongitudeVelocity);
        //            shiftAverage = Math.Abs(distanceDynamic / posDynamic.LongitudeVelocity);

        //            posDynamic = Utilities.GeocentricPositionOf(around + shiftDynamic, id, SeFlg.SEFLG_SPEED);
        //            posAverage = Utilities.GeocentricPositionOf(around + shiftAverage, id, SeFlg.GEOCENTRIC);

        //            distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);
        //            distanceAverage = Angle.BeelineOf(posAverage.Longitude.Degrees, dest);

        //            if (Math.Abs(distanceAverage) < Math.Abs(distanceDynamic))
        //            {
        //                lastDistance = distanceAverage;
        //                lastShift = shiftAverage;
        //                shiftDynamic = distanceAverage / posAverage.LongitudeVelocity;
        //                shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //            }
        //            else
        //            {
        //                lastDistance = distanceDynamic;
        //                lastShift = shiftDynamic;
        //                shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //                shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //            }
        //        }
        //        else
        //        {
        //            if (Math.Abs(distanceAverage) > Math.Abs(distanceDynamic))
        //            {
        //                if (Math.Abs(distanceDynamic) < Negligible)
        //                    return Utilities.UtcFromJulianDay(around);

        //                around += shiftDynamic;
        //                lastShift = shiftDynamic;
        //                distanceAverage = distanceDynamic;
        //                posAverage = posDynamic;
        //                lastDistance = distanceDynamic;
        //            }
        //            else
        //            {
        //                if (Math.Abs(distanceAverage) < Negligible)
        //                    return Utilities.UtcFromJulianDay(around);

        //                around += shiftAverage;
        //                lastShift = shiftAverage;
        //                distanceDynamic = distanceAverage;
        //                posDynamic = posAverage;
        //                lastDistance = distanceAverage;

        //            }

        //            shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //            shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //        }

        //    }

        //    throw new Exception("Failed destination get the exact time!");
        //}

        #endregion

        #region Properties

        public string Name { get; private set; }

        public PositionLookupDelegate PositionDelegate { get; private set; }

        public DateTimeOffset Since { get { return Buffer.Keys.FirstOrDefault(); } }

        public DateTimeOffset Until { get { return Buffer.Keys.LastOrDefault(); } }

        public int Count { get { return Buffer.Count; } }

        public SortedDictionary<DateTimeOffset, List<Position>> Buffer { get; private set; }

        public List<Position> this[DateTimeOffset date]
        {
            get
            {
                if (date.TimeOfDay != TimeSpan.Zero || date.Offset != TimeSpan.Zero)
                {
                    return lookFor(Utilities.ToJulianDay(date));
                }

                if (!Buffer.ContainsKey(date))
                {
                    Ephemeris decade = decadeEphemerisOf(date);
                    this.merge(decade);
                }

                return Buffer[date];
          
            }
        }

        public List<Position> this[Double julianDay]
        {
            get
            {
                return this[Utilities.UtcFromJulianDay(julianDay)];
            }
        }

        public Position this[DateTimeOffset date, PlanetId id]
        {
             get
             {
                 if (date.TimeOfDay != TimeSpan.Zero)
                 {
                     double jul_ut = Utilities.ToJulianDay(date);
                     return PositionDelegate(jul_ut, id);
                 }

                 if (!Buffer.ContainsKey(date))
                 {
                     Ephemeris decade = decadeEphemerisOf(date);
                     this.merge(decade);
                 }

                 Position result = PositionOf(Buffer[date], id);

                 if (result == null)
                 {
                     double jul_ut = Utilities.ToJulianDay(date);
                     result = PositionDelegate(jul_ut, id);
                     Buffer[date].Add(result);
                 }

                 return result;
             }
        }

        public Position this[Double julianDay, PlanetId id]
        {
            get
            {
                return this[Utilities.UtcFromJulianDay(julianDay), id];
            }
        }

        #endregion

        #region Constructors

        private Ephemeris(string name, PositionLookupDelegate positionDel) 
        {
            Buffer = new SortedDictionary<DateTimeOffset, List<Position>>();
            Name = name;
            PositionDelegate = positionDel;
        }

        private Ephemeris(string name) : this (name, new PositionLookupDelegate(Utilities.GeocentricPositionOfJulian))
        { }

        private Ephemeris(string name, DateTimeOffset date) : this(name)
        {
            Ephemeris decade = decadeEphemerisOf(date);
            this.merge(decade);
        }

        private Ephemeris(string name, PositionLookupDelegate positionDel, DateTimeOffset date)
            : this(name, positionDel)
        {
            Ephemeris decade = decadeEphemerisOf(date);
            this.merge(decade);
        }

        #endregion

        #region functions

        public DateTimeOffset ExactlyTimeOf(PlanetId id, DateTimeOffset refDate, Rectascension destination)
        {
            double jul_ut = Utilities.ToJulianDay(refDate);
            double averageSpeed = Planet.AverageSpeedOf(id);
            Position posAverage, posDynamic = PositionDelegate(jul_ut, id);

            double shiftDynamic, shiftAverage;
            double dest = destination.Degrees;

            double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);

            if (distanceDynamic < 0) distanceDynamic += 360;

            double lastDistance = 360, lastShift, distanceAverage = distanceDynamic;

            shiftDynamic = distanceDynamic / (posDynamic.LongitudeVelocity < 0 ? averageSpeed : posDynamic.LongitudeVelocity);
            shiftAverage = distanceAverage / averageSpeed;

            for (int i = 0; i < 10; i++)
            {
                posDynamic = PositionDelegate(jul_ut + shiftDynamic, id);
                posAverage = PositionDelegate(jul_ut + shiftAverage, id);

                distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);
                distanceAverage = Angle.BeelineOf(posAverage.Longitude, dest);


                if (Math.Abs(distanceAverage) > Math.Abs(distanceDynamic))
                {
                    if (Math.Abs(distanceDynamic) < Negligible)
                        return Utilities.UtcFromJulianDay(jul_ut);

                    jul_ut += shiftDynamic;
                    lastShift = shiftDynamic;
                    distanceAverage = distanceDynamic;
                    posAverage = posDynamic;
                    lastDistance = distanceDynamic;
                }
                else
                {
                    if (Math.Abs(distanceAverage) < Negligible)
                        return Utilities.UtcFromJulianDay(jul_ut);

                    jul_ut += shiftAverage;
                    lastShift = shiftAverage;
                    distanceDynamic = distanceAverage;
                    posDynamic = posAverage;
                    lastDistance = distanceAverage;

                }

                shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
                shiftAverage = distanceAverage / averageSpeed;

            }

            throw new Exception("Failed destination get the exact time!");
        }

        public void Load(DateTimeOffset start, DateTimeOffset end)
        {
            int startYear = 10 * (start.Year / 10);
            int endYear = 10 * (end.Year / 10);

            for (; startYear <= endYear; startYear += 10)
            {
                DateTimeOffset date = new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero);

                if (!Buffer.ContainsKey(date))
                {
                    Ephemeris decade = decadeEphemerisOf(date);
                    this.merge(decade);
                    date = decade.Until.AddDays(1);
                };
            }
        }

        public Position PositionOf(List<Position> positions, PlanetId id)
        {
            if (Luminous.Contains(id))
            {
                int index = (int)id;
                Position result = null;

                if (index < positions.Count)
                    result = positions[index];
                else
                {
                    index = Luminous.IndexOf(id);
                    result = positions[index];
                }

                if (result.Owner == id)
                    return result;
                else
                    throw new Exception();
            }
            else
            {
                for (int i = Luminous.Count; i < positions.Count; i++)
                {
                    Position pos = positions[i];
                    if (pos.Owner == id)
                        return pos;
                }
            }

            return null;
        }

        private Ephemeris decadeEphemerisOf(DateTimeOffset date)
        {
            int startYear = 10 * (date.Year / 10);
            string fileName = String.Format("{0}{1}.eph", Name, startYear);

            FileInfo[] files = EphemerisDirectory.GetFiles(fileName, SearchOption.AllDirectories);
            Ephemeris decadeEphe = null;

            if (files.Length != 0)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(EphemerisDirectory + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                decadeEphe = (Ephemeris)formatter.Deserialize(stream);
                stream.Close();
            }

            if (decadeEphe == null)
            {
                decadeEphe = new Ephemeris(Name + startYear.ToString(), PositionDelegate);
                DateTimeOffset start = new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset end = new DateTimeOffset(startYear + 10, 1, 1, 0, 0, 0, TimeSpan.Zero);
                decadeEphe.calculate(start, end);

                decadeEphe.save();
            }

            return decadeEphe;
        }

        private List<Position> lookFor(Double jul_ut)
        {
            List<Position> positions = new List<Position>();

            Position pos = null;

            foreach (PlanetId id in Luminous)
            {
                pos = PositionDelegate(jul_ut, id);

                if (pos != null)
                    positions.Add(pos);
            }

            Utilities.swe_close();

            return positions;
        }

        private bool merge(Ephemeris other)
        {
            if (!other.Name.StartsWith(this.Name))
                throw new Exception();

            if (Buffer.Count == 0)
            {
                Buffer = other.Buffer;
            }
            else if (Buffer.ContainsKey(other.Buffer.First().Key))
            {
                throw new Exception("No need of merge at all ! ?");
            }
            else
            {
                foreach (KeyValuePair<DateTimeOffset, List<Position>> kvp in other.Buffer)
                {
                    Buffer.Add(kvp.Key, kvp.Value);
                }

            }
            
            return true;
        }

        private void calculate(DateTimeOffset start, DateTimeOffset end)
        {
            DateTimeOffset firstDate = new DateTimeOffset(start.UtcDateTime.Date, TimeSpan.Zero);
            DateTimeOffset lastDate = new DateTimeOffset(end.UtcDateTime.Date, TimeSpan.Zero);
            TimeSpan step = TimeSpan.FromDays(1);

            Double dateVal = Utilities.ToJulianDay(firstDate);

            for (DateTimeOffset date = firstDate; date < lastDate; date +=  step, dateVal++)
            {                
                List<Position> positions = lookFor(dateVal);
                Buffer[date] = positions;
            }
        }

        private void save()
        {
            string fileName = Name + ".eph";

            FileInfo[] files = EphemerisDirectory.GetFiles(fileName, SearchOption.AllDirectories);

            if (files.Length != 0)
                throw new Exception("Ephemeris already existed: " + fileName + " in " + EphemerisDirectory.Name);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(EphemerisDirectory + fileName, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, this);
            stream.Close();

        }

        public override string ToString()
        {
            return String.Format("{0}-{1}: {2} records.", Since.ToString("yyyy"), Until.ToString("yyyy"), Count);
        }

        #endregion
    }
}
