using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum SearchMode
    {
        AroundWorkingDay,
        WithinTheDay,
        WithinTheWeek,
        WithinTheMonth,
        WithinTheYear //,
        //WithinTheDecade
    }

    [SerializableAttribute]
    public class MatchRules// : IComparable<MatchRules>
    {
        public static TimeSpan DefaultDayPeriod = TimeSpan.FromDays(1);

        public SearchMode Mode { get; private set; }

        private double referedJulianUtc;

        public double ReferedJulianUtc
        {
            get { return referedJulianUtc; }
        }

        private double startJulianUtc;

        public double StartJulianUtc
        {
            get { return startJulianUtc; }
        }

        private double endJulianUtc;

        public double EndJulianUtc
        {
            get { return endJulianUtc; }
        }

        public DateTimeOffset ReferenceTime
        {
            get { return Ephemeris.UtcFromJulianDay(referedJulianUtc); }
            set { referedJulianUtc = Ephemeris.ToJulianDay(value); }
        }

        public DateTimeOffset Since
        {
            get { return Ephemeris.UtcFromJulianDay(startJulianUtc); }
        }

        public DateTimeOffset Until
        {
            get { return Ephemeris.UtcFromJulianDay(endJulianUtc); }
        }

        public TimeSpan Length
        {
            get { return Until - Since; }
        }

        public MatchRules(DateTimeOffset time, SearchMode mode)
        {
            Mode = mode;
            ReferenceTime = time;
            DateTime theDate = time.UtcDateTime.Date;
            switch (mode)
            {
                case SearchMode.AroundWorkingDay:
                    {
                        DateTime previous = theDate - DefaultDayPeriod;
                        DateTime endOfNext = theDate.AddDays(1) + DefaultDayPeriod;

                        startJulianUtc = Ephemeris.ToJulianDay(new DateTimeOffset(previous, TimeSpan.Zero));
                        endJulianUtc = Ephemeris.ToJulianDay(new DateTimeOffset(endOfNext, TimeSpan.Zero));

                        if (previous.DayOfWeek == DayOfWeek.Saturday || previous.DayOfWeek == DayOfWeek.Sunday)
                        {
                            int dif = (previous.DayOfWeek + 7 - DayOfWeek.Friday) % 7;
                            startJulianUtc -= dif;
                        }

                        if (endOfNext.DayOfWeek == DayOfWeek.Sunday || endOfNext.DayOfWeek == DayOfWeek.Monday)
                        {
                            int dif = (8 + DayOfWeek.Monday - endOfNext.DayOfWeek) % 7;
                            endJulianUtc += dif;
                        }
                        break;
                    }
                case SearchMode.WithinTheDay:
                    {
                        startJulianUtc = Ephemeris.ToJulianDay(new DateTimeOffset(theDate, TimeSpan.Zero));
                        endJulianUtc = startJulianUtc + DefaultDayPeriod.TotalDays;
                        break;
                    }
                case SearchMode.WithinTheWeek:
                    {
                        int daysToLastSaturday = (7 + theDate.DayOfWeek - DayOfWeek.Saturday) % 7;
                        startJulianUtc = Ephemeris.ToJulianDay(new DateTimeOffset(theDate, TimeSpan.Zero)) - daysToLastSaturday;
                        endJulianUtc = startJulianUtc + 8 + DefaultDayPeriod.TotalDays;
                        break;
                    }
                case SearchMode.WithinTheMonth:
                    {
                        DateTimeOffset firstOfMonth = new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
                        startJulianUtc = Ephemeris.ToJulianDay(firstOfMonth) - DefaultDayPeriod.TotalDays;
                        firstOfMonth += TimeSpan.FromDays(31);
                        firstOfMonth -= TimeSpan.FromDays(firstOfMonth.Day - 1);
                        endJulianUtc = Ephemeris.ToJulianDay(firstOfMonth) + DefaultDayPeriod.TotalDays;
                        break;
                    }
                case SearchMode.WithinTheYear:
                    {
                        DateTimeOffset firstOfYear = new DateTimeOffset(theDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                        startJulianUtc = Ephemeris.ToJulianDay(firstOfYear) - DefaultDayPeriod.TotalDays;
                        firstOfYear += TimeSpan.FromDays(366);
                        firstOfYear -= TimeSpan.FromDays(firstOfYear.Day - 1);
                        endJulianUtc = Ephemeris.ToJulianDay(firstOfYear) + DefaultDayPeriod.TotalDays;
                        break;
                    }
                //case SearchMode.WithinTheDecade:
                //    {
                //        int year = theDate.Year - theDate.Year % 10;
                //        DateTimeOffset firstOfCenturay = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                //        startJulianUtc = Ephemeris.ToJulianDay(firstOfCenturay);
                //        DateTimeOffset firstOfNextCentuary = new DateTimeOffset(year + 10, 1, 1, 0, 0, 0, TimeSpan.Zero);
                //        endJulianUtc = Ephemeris.ToJulianDay(firstOfNextCentuary);
                //        break;
                //    }
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            switch(Mode)
            {
                case SearchMode.AroundWorkingDay:
                    {
                        sb.AppendFormat("Around the day {0}({1}): now {2} destination {3}", ReferenceTime.Date, ReferenceTime.DayOfWeek, 
                            Since.ToString("MM-dd"), Until.ToString("MM-dd"));
                        break;
                    }
                case SearchMode.WithinTheDay:
                    {
                        sb.AppendFormat("Within the day {0}({1}): now {2} destination {3}", ReferenceTime.Date, ReferenceTime.DayOfWeek,
                            Since.ToString("MM-dd"), Until.ToString("MM-dd"));
                        break;
                    }
                case SearchMode.WithinTheWeek:
                    {
                        sb.AppendFormat("Within the week {0}({1}): now {2} destination {3}", ReferenceTime.Date, ReferenceTime.DayOfWeek,
                            Since.ToString("MM-dd"), Until.ToString("MM-dd"));
                        break;
                    }
                case SearchMode.WithinTheMonth:
                    {
                        sb.AppendFormat("Within the month of {0}: now {1} destination {2}", ReferenceTime.Date,
                            Since.ToString("yyyy-MM-dd"), Until.ToString("yyyy-MM-dd"));
                        break;
                    }
                case SearchMode.WithinTheYear:
                    {
                        sb.AppendFormat("Within the year of {0}: now {1} destination {2}", ReferenceTime.Date, ReferenceTime.DayOfWeek,
                            Since.ToString("yyyy-MM-dd"), Until.ToString("yyyy-MM-dd"));
                        break;
                    }
                //case SearchMode.WithinTheDecade:
                //    {
                //        sb.AppendFormat("Within the decade of {0}: now {1} destination {2}", ReferenceTime.Date, ReferenceTime.DayOfWeek,
                //            Since.AstroStringOf("yyyy-MM-dd"), Until.AstroStringOf("yyyy-MM-dd"));
                //        break;
                //    }
                default:
                    throw new NotImplementedException();
            }

            return sb.ToString();
        }
    }

}
