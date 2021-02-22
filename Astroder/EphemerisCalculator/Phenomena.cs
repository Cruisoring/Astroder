using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum PeriodType
    {
        AroundTheDay,
        DayExactly,
        AroundTheWeek,
        AroundTheMonth,
        AroundTheSeason,
        AroundTheYear,
        Customer
    }

    public class Phenomena
    {
        #region Static fields
        public static Dictionary<PeriodType, List<PlanetId>> NegligiblesAspectsOf = new Dictionary<PeriodType, List<PlanetId>>
        {
            { PeriodType.AroundTheDay, new List<PlanetId>{ PlanetId.SE_MOON, PlanetId.SE_NORTHNODE } },
            { PeriodType.AroundTheWeek, new List<PlanetId>{ PlanetId.SE_MOON, PlanetId.SE_NORTHNODE } },
            { PeriodType.AroundTheMonth, new List<PlanetId>{ PlanetId.SE_MOON, PlanetId.SE_NORTHNODE } },
            { PeriodType.AroundTheSeason, new List<PlanetId>{ PlanetId.SE_MOON, PlanetId.SE_MERCURY } },
            { PeriodType.AroundTheYear, new List<PlanetId>{ PlanetId.SE_MOON, PlanetId.SE_MERCURY, PlanetId.SE_VENUS } },
            { PeriodType.DayExactly, new List<PlanetId>{ PlanetId.SE_NORTHNODE } },
            { PeriodType.Customer, new List<PlanetId>{ PlanetId.SE_NORTHNODE } }
        };

        public static Dictionary<PeriodType, string> DateTimeFormats = new Dictionary<PeriodType, string>
        {
            { PeriodType.AroundTheDay, "yyyy-MM-dd"},
            { PeriodType.AroundTheWeek, "MM-dd HH:mm"},
            { PeriodType.AroundTheMonth, "yyyy-MM"},
            { PeriodType.AroundTheSeason, "yyyy-MM"},
            { PeriodType.AroundTheYear, "yyyy"},
            { PeriodType.DayExactly, "yyyy-MM-dd ddd"},
            { PeriodType.Customer, "yyyy-MM-dd ddd"}
        };

        public static Dictionary<PeriodType, AspectImportance> ConcernedAspect = new Dictionary<PeriodType, AspectImportance>
        {
            { PeriodType.AroundTheDay, AspectImportance.Effective},
            { PeriodType.AroundTheWeek, AspectImportance.Effective},
            { PeriodType.AroundTheMonth, AspectImportance.Important},
            { PeriodType.AroundTheSeason, AspectImportance.Important},
            { PeriodType.AroundTheYear, AspectImportance.Important},
            { PeriodType.DayExactly, AspectImportance.Minor},
            { PeriodType.Customer, AspectImportance.Minor}
        };
        #endregion

        #region Variables

        public DateTimeOffset Since { get; private set; }

        public DateTimeOffset Until { get; private set; }

        public PeriodType Kind { get; private set; }

        public string DateTimeFormat 
        {
            get
            {
                return DateTimeFormats[Kind];
            }
        }

        private List<PlanetId> negligiblePlanets { get { return NegligiblesAspectsOf[Kind]; } }

        public AspectImportance Concerned { get; private set; }

        public Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> GeoAspectarian { get; private set; }

        public Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> HelioAspectarian { get; private set; }

        public string Description
        {
             get
             {
                 StringBuilder sb = new StringBuilder();

                 if (GeoAspectarian.Count != 0)
                 {
                     sb.Append("Geo: ");
                     foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in GeoAspectarian)
                     {
                         foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in category.Value)
                         {
                             if (negligiblePlanets.Contains(kvp.Key))
                                 continue;

                             foreach (IPlanetEvent evt in kvp.Value)
                             {
                                 sb.AppendFormat("{0}, ", evt.ShortDescription);
                             }
                         }
                     }
                 }

                 if (HelioAspectarian.Count != 0)
                 {
                     sb.Append("\tHelio: ");
                     foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in HelioAspectarian)
                     {
                         foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in category.Value)
                         {
                             if (negligiblePlanets.Contains(kvp.Key))
                                 continue;

                             foreach (IPlanetEvent evt in kvp.Value)
                             {
                                 sb.AppendFormat("{0}, ", evt.ShortDescription);
                             }
                         }
                     }
                 }

                 return sb.ToString();
             }
        }

        public List<IPlanetEvent> this[SeFlg centric, PlanetEventFlag category]
        {
             get
             {
                 if ((centric != SeFlg.HELIOCENTRIC && centric != SeFlg.GEOCENTRIC) || !PlanetEvent.PlanetEventSymbolToCategory.ContainsValue(category))
                     throw new ArgumentOutOfRangeException();

                 List<IPlanetEvent> result = new List<IPlanetEvent>();

                 if (centric == SeFlg.GEOCENTRIC && GeoAspectarian.ContainsKey(category))
                 {
                     foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in GeoAspectarian[category])
                     {
                         if (kvp.Value.Count != 0)
                             result.AddRange(kvp.Value);
                     }
                 }
                 else if (centric == SeFlg.HELIOCENTRIC && HelioAspectarian.ContainsKey(category))
                 {
                     foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in HelioAspectarian[category])
                     {
                         if (kvp.Value.Count != 0)
                             result.AddRange(kvp.Value);
                     }
                 }

                 return (result.Count == 0) ? null : result;
             }
        }

        public List<IPlanetEvent> this[SeFlg centric, PlanetEventFlag category, PlanetId id]
        {
            get
            {
                if ((centric != SeFlg.HELIOCENTRIC && centric != SeFlg.GEOCENTRIC) || !PlanetEvent.PlanetEventSymbolToCategory.ContainsValue(category))
                    throw new ArgumentOutOfRangeException();

                if (centric == SeFlg.GEOCENTRIC && GeoAspectarian.ContainsKey(category) && GeoAspectarian[category].ContainsKey(id))
                {
                    return GeoAspectarian[category][id];
                }
                else if (centric == SeFlg.HELIOCENTRIC && HelioAspectarian.ContainsKey(category) && HelioAspectarian[category].ContainsKey(id))
                {
                    return HelioAspectarian[category][id];
                }

                return null;
            }
        }

        public List<IPlanetEvent> this[SeFlg centric, PlanetPair pair]
        {
             get
             {
                 Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> aspectarian =
                     (centric == SeFlg.GEOCENTRIC) ? GeoAspectarian : centric == SeFlg.HELIOCENTRIC ? HelioAspectarian : null;

                 if (aspectarian == null || !aspectarian.ContainsKey(PlanetEventFlag.AspectCategory))
                     return null;

                 List<IPlanetEvent> result = new List<IPlanetEvent>();

                 if (pair.Interior == PlanetId.SE_ECL_NUT)
                     throw new NotImplementedException();
                 else if (pair.Exterior == PlanetId.SE_ECL_NUT)
                 {
                     foreach (KeyValuePair<PlanetId, List<IPlanetEvent>>kvp in aspectarian[PlanetEventFlag.AspectCategory])
                     {
                         if (!pair.Contains(kvp.Key))
                             continue;

                         foreach (IPlanetEvent evt in kvp.Value)
                         {
                             ExactAspectEvent aspEvt = evt as ExactAspectEvent;
                             if (aspEvt == null)
                                 throw new Exception();

                             if (pair.Contains(aspEvt.Pair))
                                 result.Add(aspEvt);
                         }
                     }
                 }
                 else
                 {
                     foreach(IPlanetEvent evt in aspectarian[PlanetEventFlag.AspectCategory][pair.Interior])
                     {
                         ExactAspectEvent aspEvt = evt as ExactAspectEvent;
                         if (aspEvt == null)
                             throw new Exception();

                         if (pair.Contains(aspEvt.Pair))
                             result.Add(aspEvt);
                     }
                 }

                 return result.Count == 0 ? null : result;
             }
        }

        public List<IPlanetEvent> this[SeFlg centric, PlanetId star1, PlanetId star2]
        {
            get { return this[centric, new PlanetPair(star1, star2)]; }
        }

        public List<IPlanetEvent> this[SeFlg centric, PlanetId star1]
        {
            get 
            {
                List<IPlanetEvent> result = new List<IPlanetEvent>();

                foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> kvp in centric==SeFlg.GEOCENTRIC ? GeoAspectarian : HelioAspectarian)
                {
                    if (kvp.Value.ContainsKey(star1))
                        result.AddRange(kvp.Value[star1]);
                }
                return result;
            }
        }

        #endregion

        public string GeoEventsSummaryOf(PlanetId id)
        {
            List<IPlanetEvent> events = this[SeFlg.GEOCENTRIC, id];

            if (events == null || events.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (IPlanetEvent evt in events)
            {
                sb.AppendFormat("{0}, ", evt.ShortDescription);
            }

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public string HelioEventsSummaryOf(PlanetId id)
        {
            List<IPlanetEvent> events = this[SeFlg.HELIOCENTRIC, id];

            if (events == null || events.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (IPlanetEvent evt in events)
            {
                sb.AppendFormat("{0}, ", evt.ShortDescription);
            }

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public string GeoEventsDescription()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> kvp in GeoAspectarian)
            {
                sb.AppendLine(kvp.Key.ToString() + ":");
                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> serial in kvp.Value)
                {
                    foreach (IPlanetEvent evt in serial.Value)
                    {
                        sb.AppendLine(evt.ToString());
                    }
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }

        public string HelioEventsDescription()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> kvp in HelioAspectarian)
            {
                sb.AppendLine(kvp.Key.ToString() + ":");
                foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> serial in kvp.Value)
                {
                    foreach (IPlanetEvent evt in serial.Value)
                    {
                        sb.AppendLine(evt.ToString());
                    }
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (GeoAspectarian.Count != 0)
            {
                sb.AppendLine("Geo:");
                foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in GeoAspectarian)
                {
                    foreach (KeyValuePair<PlanetId, List<IPlanetEvent>>kvp in category.Value)
                    {
                        if (negligiblePlanets.Contains(kvp.Key))
                            continue;

                        foreach (IPlanetEvent evt in kvp.Value)
                        {
                            sb.AppendLine(evt.ToString());
                        }
                    }
                }
            }

            if (HelioAspectarian.Count != 0)
            {
                sb.AppendLine("Helio:");
                foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in HelioAspectarian)
                {
                    foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in category.Value)
                    {
                        if (negligiblePlanets.Contains(kvp.Key))
                            continue;

                        foreach (IPlanetEvent evt in kvp.Value)
                        {
                            sb.AppendLine(evt.ToString());
                        }
                    }
                }
            }

            return sb.ToString();

        }


        public Phenomena(DateTimeOffset time, PeriodType kind, AspectImportance concerned)
        {
            Kind = kind;
            Concerned = concerned;

            #region set the eventDate/end of the period

            DateTimeOffset theDate = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, TimeSpan.Zero);
            switch (kind)
            {
                default:
                    throw new ArgumentException("The PeriodType is unexpected");
                case PeriodType.AroundTheDay:
                    Since = workingDayBefore(theDate);
                    Until = workingDayAfter(theDate);
                    break;
                case PeriodType.AroundTheWeek:
                    DateTimeOffset mondayOfTheWeek = theDate.AddDays(DayOfWeek.Monday - theDate.DayOfWeek);
                    Since = workingDayBefore(mondayOfTheWeek);
                    Until = mondayOfTheWeek.AddDays(7);
                    break;
                case PeriodType.AroundTheMonth:
                    Since = workingDayBefore(new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, TimeSpan.Zero));
                    theDate = Since + TimeSpan.FromDays(35);
                    Until = workingDayAfter(new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, TimeSpan.Zero));
                    break;
                case PeriodType.AroundTheSeason:
                    int season = theDate.Month / 3;
                    Since = workingDayBefore(new DateTimeOffset(theDate.Year, season*3 + 1 , 1, 0, 0, 0, TimeSpan.Zero));
                    theDate = Since + TimeSpan.FromDays(95);
                    Until = workingDayAfter(new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, TimeSpan.Zero));
                    break;
                case PeriodType.AroundTheYear:
                    Since = workingDayBefore(new DateTimeOffset(theDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero));
                    Until = workingDayAfter(new DateTimeOffset(theDate.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero));
                    break;
                case PeriodType.DayExactly:
                    Since = theDate;
                    Until = theDate.AddDays(1);
                    break;
            }
            #endregion

            GeoAspectarian = Ephemeris.Geocentric.AspectarianDuring(Since, Until, Concerned);
            HelioAspectarian = Ephemeris.Heliocentric.AspectarianDuring(Since, Until, Concerned);
        }

        public Phenomena(DateTimeOffset start, DateTimeOffset end, AspectImportance concerned)
        {
            Kind = PeriodType.Customer;
            Concerned = concerned;
            Since = (start < end) ? start : end;
            Until = (start < end) ? end : start;

            GeoAspectarian = Ephemeris.Geocentric.AspectarianDuring(Since, Until, Concerned);
            HelioAspectarian = Ephemeris.Heliocentric.AspectarianDuring(Since, Until, Concerned);
            //getAllEvents();
        }

        //private void getAllEvents()
        //{
        //    Occultations = Ephemeris.Geocentric[Since, Until, PlanetEventFlag.EclipseOccultationCategory];

        //    DirectionChanges = classify(Ephemeris.Geocentric, PlanetEventFlag.DirectionalCategory);
        //    GeocentricSignChanges = classify(Ephemeris.Geocentric, PlanetEventFlag.SignChangedCategory);
        //    HeliocentricSignChanges = classify(Ephemeris.Heliocentric, PlanetEventFlag.SignChangedCategory);
        //    VerticalChanges = classify(Ephemeris.Geocentric, PlanetEventFlag.DirectionalCategory);

        //    GeocentricAspects = summarizedAspect(Ephemeris.Geocentric);
        //    HeliocentricAspects = summarizedAspect(Ephemeris.Heliocentric);
        //}

        private SortedDictionary<PlanetPair, List<IPlanetEvent>> summarizedAspect(Ephemeris theEphe)
        {
            List<IPlanetEvent> events = theEphe[Since, Until, Concerned];

            //events.Sort()

            if (events != null && events.Count != 0)
            {
                SortedDictionary<PlanetPair, List<IPlanetEvent>> result = new SortedDictionary<PlanetPair, List<IPlanetEvent>>();

                foreach (ExactAspectEvent aspEvent in events)
                {
                    if (negligiblePlanets != null && (negligiblePlanets.Contains(aspEvent.Interior) || negligiblePlanets.Contains(aspEvent.Exterior)))
                        continue;

                    if (!result.ContainsKey(aspEvent.Pair))
                    {
                        result.Add(aspEvent.Pair, new List<IPlanetEvent>());
                    }

                    result[aspEvent.Pair].Add(aspEvent);
                }

                return result;
            }
            else
                return null;
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> classify(Ephemeris theEphe, PlanetEventFlag category)
        {
            List<IPlanetEvent> events = theEphe[Since, Until, category];

            if (events != null && events.Count != 0)
            {
                Dictionary<PlanetId, List<IPlanetEvent>> result = new Dictionary<PlanetId, List<IPlanetEvent>>();
                foreach (IPlanetEvent evt in events)
                {
                    if (negligiblePlanets != null && (negligiblePlanets.Contains(evt.Who)))
                        continue;

                    if (!result.ContainsKey(evt.Who))
                        result.Add(evt.Who, new List<IPlanetEvent>());

                    result[evt.Who].Add(evt);
                }
                return result;
            }

            return null;
        }

        private DateTimeOffset workingDayBefore(DateTimeOffset time)
        {
            DateTimeOffset result = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, 0, TimeSpan.Zero).AddDays(-1);

            while(result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = result.AddDays(-1);
            }

            return result;
        }

        private DateTimeOffset workingDayAfter(DateTimeOffset time)
        {
            DateTimeOffset result = new DateTimeOffset(time.Year, time.Month, time.Day, 0, 0, 0, 0, TimeSpan.Zero).AddDays(1);

            while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = result.AddDays(1);
            }

            return result;
        }

    }
}
