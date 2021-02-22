using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolygonControl
{
    public class IndexedPosition
    {
        #region Property

        public double IndexValue { get; set; }

        public float Radius { get; set; }

        public float Angle { get; set; }

        public int Round { get; set; }

        #endregion

        public IndexedPosition(double indexValue, float radius, float angle, int round)
        {
            IndexValue = indexValue;
            Radius = radius;
            Angle = angle;
            Round = round;
        }

        public override string ToString()
        {
            if (Radius > 0)
                return String.Format("Index={0}: round {1}, angleLow={2}, radius={3}", IndexValue, Round, Angle, Radius);
            else
                return String.Format("Rotate {0:F1}º, after {1} rounds, Totally {2:F1}º", Angle, Round, Angle+Round*360);
        }

        public static IndexedPosition operator -(IndexedPosition lhs, IndexedPosition rhs)
        {
            double indexDif;
            int round;
            float angle;

            if (lhs.IndexValue >= rhs.IndexValue)
            {
                indexDif = lhs.IndexValue - rhs.IndexValue;
                round = lhs.Round - rhs.Round;
            }
            else
            {
                indexDif = rhs.IndexValue - lhs.IndexValue;
                round = rhs.Round - lhs.Round;
            }
            angle = (lhs.Angle - rhs.Angle + 360)%360;
            if (angle < 0)
                angle += 360;

            return new IndexedPosition(indexDif, 0, angle, round);
        }

        public static IndexedPosition operator +(IndexedPosition lhs, IndexedPosition rhs)
        {
            double indexSum = lhs.IndexValue + rhs.IndexValue;
            int round = lhs.Round + rhs.Round;
            float angle = (lhs.Angle + rhs.Angle + 360)%360;

            return new IndexedPosition(indexSum, 0, angle, round);
        }

    }
}
