using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;

namespace AstroHelper
{
    public class RelationFinder
    {
        #region Constants
        private const int maxTry = 10;
        private const int eventCount = 10;
        public const int MaxStep = 5;
        #endregion

        #region Comparer defintion
        public class JulianUtComparer : IComparer<Double>
        {
            private double timespanResolution = 5.0 / 1440;

            public JulianUtComparer() { }

            public JulianUtComparer(int inMinutes)
            {
                this.timespanResolution = inMinutes / 1440.0;
            }

            #region IComparer<double> 成员

            public int Compare(double x, double y)
            {
                double span = x - y;
                return (int)(span / timespanResolution);
            }

            #endregion
        }
        #endregion

        public class PositionPair
        {
            public Position SuperiorPosition { get; private set; }

            public Position InferiorPosition { get; private set; }

            public PositionPair(Position superiorPos, Position inferiorPos)
            {
                SuperiorPosition = superiorPos;
                InferiorPosition = inferiorPos;
            }

            public override string ToString()
            {
                return String.Format("{0}{1} - {2}{3}", SuperiorPosition.Owner.Symbol, SuperiorPosition.Longitude,
                    InferiorPosition.Owner.Symbol, InferiorPosition.Longitude);
            }
        }

        #region Fields and Properties

        public SortedDictionary<double, PositionPair> CachedPositions;

        public Period During {get; private set; }

        public PlanetId SuperiorId { get; private set; }

        public PlanetId InferiorId { get; private set; }

        #endregion

        #region Constructors
        public RelationFinder(Period during, PlanetId one, PlanetId another)
        {
            During = during;
            SuperiorId = (one < another) ? one : another;
            InferiorId = (one < another) ? another : one;
            CachedPositions = new SortedDictionary<double, PositionPair>(new JulianUtComparer());
        }

        //public RelationFinder(DateTimeOffset time, PlanetId one, PlanetId another, MatchingMode mode)
        //{
        //    SuperiorId = (one < another) ? one : another;
        //    InferiorId = (one < another) ? another : one;

        //    ReferenceTime = time;
        //    DateTime theDate = time.UtcDateTime.Date;
        //    switch(mode)
        //    {
        //        case MatchingMode.AroundTheDay:
        //            {
        //                DateTime previous = theDate - DefaultDayPeriod;
        //                DateTime endOfNext = theDate.AddDays(1) + DefaultDayPeriod;

        //                startJulianUtc = Utilities.ToJulianDay(new DateTimeOffset(previous, TimeSpan.Zero));
        //                endJulianUtc = Utilities.ToJulianDay(new DateTimeOffset(endOfNext, TimeSpan.Zero));

        //                if (previous.DayOfWeek == DayOfWeek.Saturday || previous.DayOfWeek == DayOfWeek.Sunday)
        //                {
        //                    int dif = (previous.DayOfWeek + 7 - DayOfWeek.Friday) % 7;
        //                    startJulianUtc -= dif;
        //                }

        //                if (endOfNext.DayOfWeek == DayOfWeek.Sunday || endOfNext.DayOfWeek == DayOfWeek.Monday)
        //                {
        //                    int dif = (8 + DayOfWeek.Monday - endOfNext.DayOfWeek) % 7;
        //                    endJulianUtc += dif;
        //                }
        //                break;
        //            }
        //        case MatchingMode.WithinTheDay:
        //            {
        //                startJulianUtc = Utilities.ToJulianDay(new DateTimeOffset(theDate, TimeSpan.Zero));
        //                endJulianUtc = startJulianUtc + DefaultDayPeriod.TotalDays;
        //                break;
        //            }
        //        case MatchingMode.WithinTheWeek:
        //            {
        //                int daysToLastSaturday = (7 + theDate.DayOfWeek - DayOfWeek.Saturday) % 7;
        //                startJulianUtc = Utilities.ToJulianDay(new DateTimeOffset(theDate, TimeSpan.Zero)) - daysToLastSaturday;
        //                //int daysToNextSaturday = (7 + DayOfWeek.Monday - theDate.DayOfWeek) % 7;
        //                //endJulianUtc = referedJulianUtc + daysToNextSaturday + DefaultDayPeriod;
        //                endJulianUtc = startJulianUtc + 7 + DefaultDayPeriod.TotalDays;
        //                break;
        //            }
        //        case MatchingMode.WithinTheMonth:
        //            {
        //                DateTimeOffset firstOfMonth = new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
        //                startJulianUtc = Utilities.ToJulianDay(firstOfMonth) - DefaultDayPeriod.TotalDays;
        //                DateTimeOffset firstOfNextMonth = new DateTimeOffset(theDate.Year, theDate.Month + 1, 1, 0, 0, 0, TimeSpan.Zero);
        //                endJulianUtc = Utilities.ToJulianDay(firstOfNextMonth) + DefaultDayPeriod.TotalDays;
        //                break;
        //            }
        //        case MatchingMode.WithinTheYear:
        //            {
        //                DateTimeOffset firstOfYear = new DateTimeOffset(theDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
        //                startJulianUtc = Utilities.ToJulianDay(firstOfYear) - DefaultDayPeriod.TotalDays;
        //                DateTimeOffset firstOfNextYear = new DateTimeOffset(theDate.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
        //                endJulianUtc = Utilities.ToJulianDay(firstOfNextYear) + DefaultDayPeriod.TotalDays;
        //                break;
        //            }
        //        case MatchingMode.WithinTheDecade:
        //            {
        //                int year = theDate.Year - theDate.Year % 100;
        //                DateTimeOffset firstOfCenturay = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
        //                startJulianUtc = Utilities.ToJulianDay(firstOfCenturay);
        //                DateTimeOffset firstOfNextCentuary = new DateTimeOffset(year + 100, 1, 1, 0, 0, 0, TimeSpan.Zero);
        //                endJulianUtc = Utilities.ToJulianDay(firstOfNextCentuary);
        //                break;

        //            }
        //        default:
        //            throw new NotImplementedException();
        //    }

        //    Position superiorStart = Utilities.PositionOf(startJulianUtc, SuperiorId, SeFlg.DEFAULT);
        //    Position inferiorStart = Utilities.PositionOf(startJulianUtc, InferiorId, SeFlg.DEFAULT);

        //    CachedPositions = new SortedDictionary<double, PositionPair>(new JulianUtComparer());
        //    CachedPositions.Add(startJulianUtc, new PositionPair(superiorStart, inferiorStart));
        //}

        #endregion

        //private Aspects nextAspect(Angle angle, Double velocity)
        //{
        //    Aspects temp = (velocity > 0) ? Aspects.NextAspectOf(angle) : Aspects.PreviousAspectOf(angle);

        //    if (Math.Abs(temp.OrbOf(angle)) > 0.001)
        //        return temp;
        //    else
        //        return (velocity > 0) ? Aspects.NextAspectOf(temp) : Aspects.PreviousAspectOf(temp);
        //}

        //private Aspects previousAspect(Angle angle, Double velocity)
        //{
        //    Aspects temp = (velocity <= 0) ? Aspects.NextAspectOf(angle) : Aspects.PreviousAspectOf(angle);

        //    if (Math.Abs(temp.OrbOf(angle)) > 0.001)
        //        return temp;
        //    else
        //        return (velocity > 0) ? Aspects.PreviousAspectOf(temp) : Aspects.NextAspectOf(temp);
        //}

        private Relation nextRelation(Double sinceJulian)
        {
            double orbNext, speed, step = 100.0, time;

            //KeyValuePair<double, PositionPair> last = CachedPositions.Last();
            time = sinceJulian;
            Position superiorPos = Utilities.PositionOf(time, SuperiorId, SeFlg.DEFAULT);
            Position inferiorPos = Utilities.PositionOf(time, InferiorId, SeFlg.DEFAULT);

            Angle angle = inferiorPos.Longitude - superiorPos.Longitude;
            speed = inferiorPos.LongitudeVelocity - superiorPos.LongitudeVelocity;

            Aspects next = Aspects.NextAspect(angle, speed>0);

            orbNext = next.OrbOf(angle);

            for (int tryCount = 0; tryCount < maxTry; tryCount++)
            {
                if (Math.Abs(orbNext) < Relation.Negligible)
                {
                    CachedPositions.Add(time, new PositionPair(superiorPos, inferiorPos));
                    //return true;
                    return new Relation(Utilities.UtcFromJulianDay(time), superiorPos, inferiorPos);
                }

                step = -orbNext / speed;

                if (time + step < sinceJulian)
                    return null;

                time += step;

                if (time > During.EndJulianUtc)
                    return null;

                superiorPos = Utilities.PositionOf(time, SuperiorId, SeFlg.SEFLG_SPEED);
                inferiorPos = Utilities.PositionOf(time, InferiorId, SeFlg.SEFLG_SPEED);

                angle = inferiorPos.Longitude - superiorPos.Longitude;
                speed = inferiorPos.LongitudeVelocity - superiorPos.LongitudeVelocity;
                orbNext = next.OrbOf(angle);
            }
            return null;
        }

        public List<Relation> GetAllRelations()
        {
            Relation newRelation = null;
            Double since = During.StartJulianUtc;

            do
            {
                newRelation = nextRelation(since);

            } while (newRelation != null);

            var relationQuery =
                from kvp in CachedPositions
                let relation = new Relation(Utilities.UtcFromJulianDay(kvp.Key), kvp.Value.SuperiorPosition, kvp.Value.InferiorPosition)
                where relation.IsExactly
                orderby relation.Moment
                select relation;

            List<Relation> relations = relationQuery.ToList();

            return relations;
        }

        //private bool getNextRelation()
        //{
        //    double orbToNext, speed, step = 100.0, time;

        //    //KeyValuePair<double, PositionPair> last = CachedPositions.Last();
        //    time = During.StartJulianUtc;
        //    Position superiorPos = Utilities.PositionOf(time, SuperiorId, SeFlg.DEFAULT);
        //    Position inferiorPos = Utilities.PositionOf(time, InferiorId, SeFlg.DEFAULT);

        //    Angle angle = inferiorPos.Longitude - superiorPos.Longitude;
        //    speed = inferiorPos.LongitudeVelocity - superiorPos.LongitudeVelocity;

        //    Aspects next = nextAspect(angle, speed);

        //    orbToNext = next.OrbOf(angle);

        //    for (int tryCount = 0; tryCount < maxTry; tryCount++)
        //    {
        //        if (Math.Abs(orbToNext) < Relation.Negligible)
        //        {
        //            CachedPositions.Add(time, new PositionPair(superiorPos, inferiorPos));
        //            return true;
        //        }

        //        step = -orbToNext / speed;

        //        if (time + step < last.Key)
        //            return false;

        //        time += step;

        //        if (time > During.EndJulianUtc)
        //            return false;

        //        superiorPos = Utilities.PositionOf(time, SuperiorId, SeFlg.SEFLG_SPEED);
        //        inferiorPos = Utilities.PositionOf(time, InferiorId, SeFlg.SEFLG_SPEED);

        //        angle = inferiorPos.Longitude - superiorPos.Longitude;
        //        speed = inferiorPos.LongitudeVelocity - superiorPos.LongitudeVelocity;
        //        orbToNext = next.OrbOf(angle);
        //    }
        //    return false;
        //}

        //public IEnumerable<Relation> GetAllRelations()
        //{
        //    while(true)
        //    {
        //        if (!getNextRelation())
        //            break;
        //    }

        //    var relationQuery =
        //        from kvp in CachedPositions
        //        let relation = new Relation(Utilities.UtcFromJulianDay(kvp.Key), kvp.Value.SuperiorPosition, kvp.Value.InferiorPosition)
        //        where relation.IsExactly
        //        orderby relation.Moment
        //        select relation;

        //    List<Relation> relations = relationQuery.ToList();

        //    return relations;
        //}
    }

}
