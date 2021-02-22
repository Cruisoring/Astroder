using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public class AspectHelper
    {
        #region Constants
        private static Double Negligible = 0.00001;
        private const int maxTry = 10;
        private const int eventCount = 10;
        #endregion

        static AspectHelper()
        {
            Negligible = Math.Pow(10, -Phenomenon.Decimals-1);
        }

        #region Comparer defintion
        public class JulianUtComparer : IComparer<Double>
        {
            private double timespanResolution = 5.0 / 1440;

            public JulianUtComparer() {}

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

        #region Fields and Properties
        private RelationKind kind;
        private double referencePoint;
        private ForecastingDirection direction = ForecastingDirection.Around;

        private List<Double> concernedAngles;

        private SortedDictionary<double, double> cachedAspects;

        public PlanetId Interior
        {
            get { return kind.Interior; }
        }

        public PlanetId Exterior
        {
            get { return kind.Exterior; }
        }

        public AspectType Pattern
        {
            get { return kind.Pattern; }
        }

        //public Double DegreesExpected
        //{
        //    get { return Aspect.DegreesOf(Pattern); }
        //}

        public DateTime ReferenceTime
        {
            get { return SweWrapper.UtcFromJulianDay(referencePoint); }
        }

        public ForecastingDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        #endregion

        #region Constructors
        public AspectHelper(RelationKind kind, DateTime time)
        {
            this.kind = kind;
            this.referencePoint = SweWrapper.ToJulianDay(time);
            this.cachedAspects = new SortedDictionary<double, double>(new JulianUtComparer());
            this.concernedAngles = new List<double>();
            if (kind.Pattern != AspectType.None)
            {
                this.concernedAngles.Add(Aspect.DegreesOf(kind.Pattern));
            }
            else
            {
                foreach (double degrees in Aspect.Concerned.Keys)
                {
                    this.concernedAngles.Add(degrees);
                }
            }
            initiateCache();
        }

        #endregion

        private void initiateCache()
        {
            this.cachedAspects = new SortedDictionary<double, double>(new JulianUtComparer());
            double inDegrees = SweWrapper.LongitudeDegreeOf(referencePoint, Interior);
            double exDegrees = SweWrapper.LongitudeDegreeOf(referencePoint, Exterior);
            double angle = (inDegrees - exDegrees + 360.0) % 360;
            double expected = similarAspectDegrees(angle);
            double orb = ((angle > 180.0) ? 360.0 - expected : expected) - angle;

            this.cachedAspects.Add(referencePoint, angle);
        }

        private double similarAspectDegrees(Double angles)
        {
            if (concernedAngles.Count == 1)
                return concernedAngles[0];

            double temp = (angles + 360.0) % 360;
            if (temp > 180.0)
                temp = 360.0 - temp;

            double delta = 180;
            double nearAspect = 0;

            foreach (double deg in concernedAngles)
            {
                if (delta > Math.Abs(deg - temp))
                {
                    delta = Math.Abs(deg - temp);
                    nearAspect = deg;
                }
            }
            return nearAspect;
        }

        private DateTime getClosestFromCache(Double expected)
        {
            double closestDate = 0;
            double minOrb = 5.0;
            double orb;

            foreach (KeyValuePair<double, double> kvp in cachedAspects)
            {
                orb = Math.Abs(kvp.Value);
                orb = (orb >= expected) ? orb - expected : expected - orb;
                if (minOrb > orb)
                {
                    closestDate = kvp.Key;
                    minOrb = orb;
                }                
            }

            return SweWrapper.UtcFromJulianDay(closestDate);
        }

        //public DateTime GetEventTime()
        //{
        //    Double expected = DegreesExpected;

        //    double jul = referencePoint;
        //    PlanetPosition interiorPos, exterirorPos;
        //    double angle, orb, speed, step = 100.0, inPos, exPos;

        //    for (int tryCount = 0; tryCount < maxTry; tryCount ++ )
        //    {
        //        interiorPos = SweWrapper.PositionOf(jul, Interior, SeFlg.SEFLG_SPEED);
        //        exterirorPos = SweWrapper.PositionOf(jul, Exterior, SeFlg.SEFLG_SPEED);

        //        inPos = interiorPos.Rectascension.Degrees;
        //        exPos = exterirorPos.Rectascension.Degrees;

        //        angle =( inPos - exPos + 360) % 360.0;

        //        orb = ( (angle > 180.0) ? 360.0-expected : expected) - angle;
        //        speed = interiorPos.LongVelo.Degrees - exterirorPos.LongVelo.Degrees;

        //        if (!cachedAspects.ContainsKey(jul))
        //            cachedAspects.Add(jul, angle);

        //        if (Math.Abs(orb) < Negligible)
        //            return SweWrapper.UtcFromJulianDay(jul);

        //        step = orb / speed;
        //        jul += step;

        //        //step = Math.Min(step / 2.0, Math.Abs(orb / speed));

        //        //if (orb * speed * (inPos-exPos) > 0)
        //        //    jul += step;
        //        //else
        //        //    jul -= step;
        //    }
        //    return getClosestFromCache();
        //}

        public Phenomenon PhenomenonNearby()
        {
            Double expected = similarAspectDegrees(cachedAspects[referencePoint]);

            double jul = referencePoint;
            PlanetPosition interiorPos, exterirorPos;
            double angle, orb, speed, step = 100.0, inPos, exPos;

            for (int tryCount = 0; tryCount < maxTry; tryCount ++ )
            {
                interiorPos = SweWrapper.PositionOf(jul, Interior, SeFlg.SEFLG_SPEED);
                exterirorPos = SweWrapper.PositionOf(jul, Exterior, SeFlg.SEFLG_SPEED);

                inPos = interiorPos.Rectascension.Degrees;
                exPos = exterirorPos.Rectascension.Degrees;

                angle =( inPos - exPos + 360) % 360.0;

                orb = ( (angle > 180.0) ? 360.0-expected : expected) - angle;
                speed = interiorPos.LongVelo.Degrees - exterirorPos.LongVelo.Degrees;

                if (!cachedAspects.ContainsKey(jul))
                    cachedAspects.Add(jul, angle);

                if (Math.Abs(orb) < Negligible)
                {
                    return new Phenomenon(SweWrapper.UtcFromJulianDay(jul),
                        new RelationKind(Interior, Exterior, Aspect.AspectTypeOf(expected)), inPos, exPos);
                    //return SweWrapper.UtcFromJulianDay(jul);
                }

                step = orb / speed;
                jul += step;

                //step = Math.Min(step / 2.0, Math.Abs(orb / speed));

                //if (orb * speed * (inPos-exPos) > 0)
                //    jul += step;
                //else
                //    jul -= step;
            }
            return null;
        }

    }

    public enum ForecastingDirection
    {
        Around,
        Backward,
        Foreward
    }
}
