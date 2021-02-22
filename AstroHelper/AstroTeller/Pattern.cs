using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteHelper;
using AstroHelper;

namespace AstroTeller
{
    public class Pattern
    {
        #region Static definitions
        public static Dictionary<RecordType, PeriodMode> DefaultModes;

        static Pattern()
        {
            DefaultModes = new Dictionary<RecordType, PeriodMode>
            {
                {RecordType.MinuteRecord, PeriodMode.WithinTheDay},
                {RecordType.FiveMinuteRecord, PeriodMode.WithinTheDay},
                {RecordType.ThirteenMinuteRecord, PeriodMode.WithinTheDay},
                {RecordType.HalfHourRecord, PeriodMode.WithinTheDay},
                {RecordType.HourRecord, PeriodMode.WithinTheDay},
                {RecordType.TwoHourRecord, PeriodMode.WithinTheDay},
                {RecordType.DayRecord, PeriodMode.AroundWorkingDay},
                {RecordType.WeekRecord, PeriodMode.WithinTheWeek},
                {RecordType.MonthRecord, PeriodMode.WithinTheMonth},
                {RecordType.QuarterRecord, PeriodMode.WithinTheYear},
                {RecordType.Unknown, PeriodMode.AroundWorkingDay}
            };

        }

        #endregion

        public PeriodMode Mode { get; set; }
        public List<TurningPoint<T>> Majors { get; set; }
        public Dictionary<DateTime, List<Relation>> Phenomena { get; set; }

        public Pattern(List<TurningPoint<T>> majors)
        {
            T sample = majors[0].Record;
            Mode = DefaultModes[sample.TheRecordType];

            Majors = majors;
            Phenomena = new Dictionary<DateTime, List<Relation>>();
        }

        public void Search()
        {
            foreach (TurningPoint<T> major in Majors)
            {
                Period duration = new Period(major.Date, Mode);

                List<Relation> allRelations = Relation.AllRelationsDuring(duration);

                Phenomena.Add(major.Date, allRelations);
            }
        }
    }
}
