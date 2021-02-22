using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using NumberHelper.DoubleHelper;

namespace AstroHelper
{
    public enum RectascensionMode
    {
        None,
        Resisted,
        Retrograde,
        Direct,
        Transcend,
        ForwardMost,    //Direct with the highest speed
        BackwardMost    //Retrograde with the highest speed
    }

    public enum DeclinationMode
    {
        None,
        OrbitAsc,
        OrbitDesc,
        Northest,
        Southest,
        UpingMost,      //Northward with the highest speed
        DowningMost     //Southward with the highest speed
    }

    public enum DistanceMode
    {
        None,
        Perigee,
        Apogee,
        ComingMost,     //Getting closer with the highest speed
        DepartingMost   //Leaving with the highest speed
    }

    public class PlanetEvents
    {
        #region Constants
        public static int WatchingDays = 7;
        public static int EffectiveDays = 4;

        public static bool NeglectMoon = true;

        public static Dictionary<RectascensionMode, char> RectascensionSymbols = new Dictionary<RectascensionMode, char> { 
            { RectascensionMode.None, ' '},
            { RectascensionMode.Resisted, '\u21E5'},
            { RectascensionMode.Retrograde, '\u21B5'},
            { RectascensionMode.Direct, '\u21B3'},
            { RectascensionMode.Transcend, '\u21A6'},
            { RectascensionMode.ForwardMost, '\u21a0'},
            { RectascensionMode.BackwardMost, '\u219e'}
        };

        public static Dictionary<DeclinationMode, char> DeclinationSymbols = new Dictionary<DeclinationMode, char> { 
            { DeclinationMode.None, ' '},
            { DeclinationMode.OrbitAsc, '\u21E5'},
            { DeclinationMode.OrbitDesc, '\u21B5'},
            { DeclinationMode.Northest, '\u21B3'},
            { DeclinationMode.Southest, '\u21A6'},
            { DeclinationMode.UpingMost, '\u21a0'},
            { DeclinationMode.DowningMost, '\u219e'}
        };

        public static Dictionary<DistanceMode, char> DistanceSymbols = new Dictionary<DistanceMode, char> { 
            { DistanceMode.None, ' '},
            { DistanceMode.Perigee, '\u260C'},
            { DistanceMode.Apogee, '\u260D'},
            { DistanceMode.ComingMost, '\u25BD'},
            { DistanceMode.DepartingMost, '\u25B3'}
        };

        #endregion

        #region Properties
        public Ephemeris TheEphemeris { get; set; }

        public DateTimeOffset Date { get; set; }

        public Position Current { get; set; }

        List<Relation> RelationWithOthers { get; set; }

        public RectascensionMode LongitudeStatus { get; set; }

        public int DaysToRectascensionMode { get; set; }

        public DeclinationMode LatitudeStatus { get; set; }

        public int DaysToDeclinationMode { get; set; }

        public DistanceMode DistanceStatus { get; set; }

        public int DaysToDistanceMode { get; set; }

        public string Brief
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (LongitudeStatus != RectascensionMode.None)
                {
                    sb.AppendFormat("{0} {1}{2} ", Planet.SymbolOf(Current.Owner), RectascensionSymbols[LongitudeStatus],DaysToRectascensionMode);
                }
                if(LatitudeStatus != DeclinationMode.None)
                {
                    sb.AppendFormat("{0}{1} ", DeclinationSymbols[LatitudeStatus], DaysToDeclinationMode);
                }
                if(DistanceStatus != DistanceMode.None)
                {
                    sb.AppendFormat("{0}{1} ", DistanceSymbols[DistanceStatus], DaysToDistanceMode);
                }
                foreach (Relation rel in RelationWithOthers)
                {
                    if (NeglectMoon && rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                        continue;
                    sb.AppendFormat("{0}{1} ",
                        rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior==Current.Owner ? rel.Superior : rel.Inferior));
                }

                return sb.ToString();
            }
        }

        #endregion

        #region Constructors

        public PlanetEvents(Ephemeris ephem, DateTimeOffset date, PlanetId id)
        {
            List<Position> watchedPositions = new List<Position>();
            List<double> longSpeeds = new List<double>();
            List<double> latSpeeds = new List<double>();
            List<double> latitudes = new List<double>();
            List<double> distSpeeds = new List<double>();
            List<DateTimeOffset> retroDates = null;

            try
            {
                TheEphemeris = ephem;
                Date = date;

                for (int i = -WatchingDays; i <= WatchingDays; i++)
                {
                    Position pos = TheEphemeris[date.AddDays(i), id];
                    watchedPositions.Add(pos);
                    latitudes.Add(pos.Latitude);
                    longSpeeds.Add(pos.LongitudeVelocity);
                    latSpeeds.Add(pos.LatitudeVelocity);
                    distSpeeds.Add(pos.DistanceVelocity);
                }

                Current = watchedPositions[WatchingDays];
                RelationWithOthers = Ephemeris.RelationsWithin(new MatchRules(date, SearchMode.WithinTheDay), id);

                //List<double> reversedVelocities = Ephemeris.TurningsOf(longSpeeds);

                //if (reversedVelocities.Count != 0)
                //{

                //}


                double minLongVel = (from spd in longSpeeds
                                     orderby Math.Abs(spd)
                                     select spd).FirstOrDefault();

                int stationIndex = longSpeeds.IndexOf(minLongVel);
                DateTimeOffset around = Date.AddDays(stationIndex - WatchingDays);
                retroDates = Ephemeris.RetrogradeDateAround(id, around);

                if (retroDates != null)
                {
                    DateTimeOffset nearest = (from keyDay in retroDates
                                              orderby Math.Abs((date - keyDay).TotalDays)
                                              select keyDay).FirstOrDefault();

                    if (!retroDates.Contains(nearest))
                    {
                        LongitudeStatus = RectascensionMode.None;
                        DaysToRectascensionMode = 100;
                    }
                    else
                    {
                        DaysToRectascensionMode = (int)((date - nearest).TotalDays);

                        if (DaysToRectascensionMode > EffectiveDays || DaysToRectascensionMode < -EffectiveDays)
                            LongitudeStatus = RectascensionMode.None;
                        else
                        {
                            switch (retroDates.IndexOf(nearest))
                            {
                                case 0:
                                    LongitudeStatus = RectascensionMode.Resisted;
                                    break;
                                case 1:
                                    LongitudeStatus = RectascensionMode.Retrograde;
                                    break;
                                case 2:
                                    LongitudeStatus = RectascensionMode.Direct;
                                    break;
                                case 3:
                                    LongitudeStatus = RectascensionMode.Transcend;
                                    break;
                                default:
                                    LongitudeStatus = RectascensionMode.None;
                                    break;
                            }
                        }

                    }

                }
                else
                    getRectascensionMode(longSpeeds);

                getDeclinationMode(latSpeeds, latitudes);
                getDistanceMode(distSpeeds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        #endregion


        #region Functions

        private void getRectascensionMode(List<Double> longSpeeds)
        {
            int index = outstandingSpeedIndex(longSpeeds);
            DaysToRectascensionMode = WatchingDays - index;

            if (DaysToRectascensionMode > EffectiveDays || DaysToRectascensionMode < -EffectiveDays)
                LongitudeStatus = RectascensionMode.None;
            else
            {
                Double prev = longSpeeds[(index == 0) ? index : index - 1];
                Double next = longSpeeds[(index == longSpeeds.Count - 1) ? index : index + 1];
                Double temp = prev * next ;

                if (temp < 0)
                    LongitudeStatus = (next > prev) ? RectascensionMode.Direct : RectascensionMode.Retrograde;
                else if (temp > 0)
                    LongitudeStatus = prev > 0 ? RectascensionMode.ForwardMost : RectascensionMode.BackwardMost;
                else
                    LongitudeStatus = RectascensionMode.None;
            }
        }

        private void getDeclinationMode(List<Double> latSpeeds, List<double> latitudes)
        {
            int index = outstandingSpeedIndex(latSpeeds);
            int latIndex = outstandingSpeedIndex(latitudes);

            if (index == -1 && latIndex == -1)
            {
                DaysToDeclinationMode = WatchingDays + 1;
                LatitudeStatus = DeclinationMode.None;
            }
            else if (latIndex != -1)
            {
                Double prev = latitudes[(latIndex == 0) ? latIndex : latIndex - 1];
                Double next = latitudes[(latIndex == latitudes.Count - 1) ? latIndex : latIndex + 1];
                Double temp = prev * next;
                DaysToDeclinationMode = WatchingDays - latIndex;

                if (temp < 0)
                    LatitudeStatus = (next > prev) ? DeclinationMode.OrbitAsc : DeclinationMode.OrbitDesc;
                else if (latIndex != 0 && latIndex != latitudes.Count - 1)
                    LatitudeStatus = (next > 0) ? DeclinationMode.Northest : DeclinationMode.Southest;
                else
                    LatitudeStatus = DeclinationMode.None;
            }
            else if (index != -1)
            {
                Double prev = latSpeeds[(index == 0) ? index : index - 1];
                Double next = latSpeeds[(index == latSpeeds.Count - 1) ? index : index + 1];
                Double temp = prev * next;
                DaysToDeclinationMode = WatchingDays - index;

                if (temp < 0)
                    LatitudeStatus = (next > prev) ? DeclinationMode.Southest : DeclinationMode.Northest;
                else if (temp > 0)
                    LatitudeStatus = (next > 0) ? DeclinationMode.UpingMost : DeclinationMode.DowningMost;
                else
                    LatitudeStatus = DeclinationMode.None;
            }
        }

        private void getDistanceMode(List<Double> distSpeeds)
        {
            int index = outstandingSpeedIndex(distSpeeds);
            DaysToDistanceMode = WatchingDays - index;

            if (DaysToDistanceMode > EffectiveDays || DaysToDistanceMode < -EffectiveDays)
            {
                DistanceStatus = DistanceMode.None;
            }
            else
            {
                Double prev = distSpeeds[(index == 0) ? index : index - 1];
                Double next = distSpeeds[(index == distSpeeds.Count - 1) ? index : index + 1];
                Double temp = prev * next ;

                if (temp < 0)
                    DistanceStatus = (next > prev) ? DistanceMode.Perigee : DistanceMode.Apogee;
                else if (temp > 0)
                    DistanceStatus = prev > 0 ? DistanceMode.DepartingMost : DistanceMode.ComingMost;
                else
                    DistanceStatus = DistanceMode.None;
            }

        }

        private int outstandingSpeedIndex(List<Double> speeds)
        {
            double current = speeds[WatchingDays];

            if (current * speeds[0] < 0 || current * speeds[2 * WatchingDays] < 0)
            {
                var speedQuery = from speed in speeds
                                 orderby Math.Abs(speed)
                                 select speed;

                return speeds.IndexOf(speedQuery.First());
            }
            else if (current != 0)
            {
                var t = from speed in speeds
                        orderby speed
                        select speed;

                int result = speeds.IndexOf(current > 0 ? t.Last() : t.First());

                if (result == 0 || result == speeds.Count - 1)
                    return -1;
                else
                    return result;
            }
            else
                throw new NotImplementedException();
        }

        public string BriefOf(ConcernedEvent focused)
        {
            switch (focused)
            {
                case ConcernedEvent.None:
                    return String.Format("{0}: {1}, H={2:F3}, D={3:F3}",
                        Planet.SymbolOf(Current.Owner), Current.Longitude.AstrologyFormat(), Current.Latitude, Current.Distance);
                case ConcernedEvent.Rectascension:
                    return LongitudeStatus == RectascensionMode.None ? "" :
                        String.Format("{0} {1}", RectascensionSymbols[LongitudeStatus],DaysToRectascensionMode);
                case ConcernedEvent.Declination:
                    return LatitudeStatus == DeclinationMode.None ? "":
                        String.Format("{0} {1}", DeclinationSymbols[LatitudeStatus], DaysToDeclinationMode);
                case ConcernedEvent.Distance:
                    return DistanceStatus == DistanceMode.None ? "" :
                        String.Format("{0}{1}", DistanceSymbols[DistanceStatus], DaysToDistanceMode);
                case ConcernedEvent.Relations:
                    if (RelationWithOthers.Count == 0)
                        return "";

                    StringBuilder sb = new StringBuilder();
                    foreach (Relation rel in RelationWithOthers)
                    {
                        if (NeglectMoon && rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                            continue;
                        sb.AppendFormat("{0}{1} ",
                            rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior == Current.Owner ? rel.Superior : rel.Inferior));
                    }

                    return sb.ToString();
                case ConcernedEvent.All:
                default:
                    return Brief;
            }
        }

        public string StatusOf(ConcernedEvent focused)
        {
            StringBuilder sb = new StringBuilder();
            switch(focused)
            {
                case ConcernedEvent.None:
                    return String.Format("{0} on {1}: Long={2}, Lat={3:F4}, Dist={4:F4}",
                        Planet.SymbolOf(Current.Owner), Date.DayOfWeek, Current.Longitude.AstrologyFormat(), Current.Latitude, Current.Distance);
                case ConcernedEvent.Rectascension:
                    return String.Format("{0} on {1}: Long={2}, Speed={3:F2}, Status={4} {5}",
                        Planet.SymbolOf(Current.Owner), Date.DayOfWeek, Current.Longitude.AstrologyFormat(), Current.LongitudeVelocity, 
                        LongitudeStatus, DaysToRectascensionMode);
                case ConcernedEvent.Declination:
                    return String.Format("{0} on {1}: Lat={2:F3}, Speed={3:F2}, Status={4} {5}",
                        Planet.SymbolOf(Current.Owner), Date.DayOfWeek, Current.Latitude, Current.LatitudeVelocity,
                        LatitudeStatus, DaysToDeclinationMode);
                case ConcernedEvent.Distance:
                    return String.Format("{0} on {1}: Dist={2:F3}, Speed={3:F2}, Status={4} {5}",
                        Planet.SymbolOf(Current.Owner), Date.DayOfWeek, Current.Distance, Current.DistanceVelocity,
                        DistanceStatus, DaysToDistanceMode);
                case ConcernedEvent.Relations:
                    foreach (Relation rel in RelationWithOthers)
                    {
                        if (NeglectMoon && rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                            continue;
                        sb.AppendFormat("{0}º{1}({2}) ",
                            rel.Aspect.Degrees, rel.Inferior == Current.Owner ? rel.Superior : rel.Inferior, rel.Orb.ToString("F3"));
                    }
                    return sb.ToString();
                case ConcernedEvent.All:
                default:
                    sb.AppendFormat("{0}@{1}:", Planet.SymbolOf(Current.Owner), Date.DayOfWeek);
                    if (LongitudeStatus != RectascensionMode.None)
                        sb.AppendFormat(" {0}{1}, Long={2}({3:F3}//day)", 
                            LongitudeStatus, DaysToRectascensionMode, Current.Longitude.AstrologyFormat(), Current.LongitudeVelocity);
                    if (LatitudeStatus != DeclinationMode.None)
                        sb.AppendFormat(" {0}{1}, Lat={2:F3}({3:F3}//day)", 
                            LatitudeStatus, DaysToDeclinationMode, Current.Latitude, Current.LatitudeVelocity);
                    if (DistanceStatus != DistanceMode.None)
                        sb.AppendFormat(" {0}{1}, Long={2:F3}({3:F3}//day)", 
                            DistanceStatus, DaysToDistanceMode, Current.Distance, Current.DistanceVelocity);
                    foreach (Relation rel in RelationWithOthers)
                    {
                        if (NeglectMoon && rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                            continue;
                        sb.AppendFormat("{0}º{1}({2}) ",
                            rel.Aspect.Degrees, rel.Inferior == Current.Owner ? rel.Superior : rel.Inferior, rel.Orb.ToString("F3"));
                    }
                    return sb.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}@{1}: Long={2}", Planet.SymbolOf(Current.Owner), Date.DayOfWeek, Current.Longitude.AstrologyFormat());

            if (LongitudeStatus != RectascensionMode.None)
            {
                sb.AppendFormat(", {0}({1}days)", LongitudeStatus, DaysToRectascensionMode);
            }

            if (LatitudeStatus != DeclinationMode.None)
            {
                sb.AppendFormat("\r\nLat={0}, {1}({2}days)", Current.Latitude.ToString("F4"), LatitudeStatus, DaysToDeclinationMode);
            }
            else
            {
                sb.AppendFormat("\r\nLat={0}", Current.Latitude.ToString("F4"));
            }

            if (DistanceStatus != DistanceMode.None)
            {
                sb.AppendFormat("\r\nDist={0}, {1}({2}days)", Current.Distance.ToString("F4"), DistanceStatus, DaysToDistanceMode);
            }
            else
            {
                sb.AppendFormat("\r\nDist={0}", Current.Distance.ToString("F4"));
            }

            foreach (Relation rel in RelationWithOthers)
            {
                if (NeglectMoon && rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                    continue;

                sb.AppendFormat("\r\n{0}{1}{2}, orb={3}", 
                    Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior), rel.Orb.ToString("F3"));
            }

            return sb.ToString();
        }

        #endregion
    }
}
