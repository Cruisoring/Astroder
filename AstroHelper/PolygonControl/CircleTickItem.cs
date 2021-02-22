using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PolygonControl
{
    public class CircleTickItem : AngledItem
    {
        #region Fields

        public float TickLength { get; set; }

        #endregion

        #region Constructors

        public CircleTickItem(float distance, float angle, float length, Pen pen)
            : base(distance, angle, 0, null, pen)
        {
            TickLength = length;
        }

        #endregion

        #region Functions

        public override void Draw(Graphics g, LogicalAngleConverterDelegate angleConverter, float unitSize, float angleOffset)
        {
            if (IsVisible)
            {
                float actualDistance = unitSize * Distance;
                float actualEnd = unitSize * (Distance + TickLength);
                float angle = Normalized(Angle + angleOffset);
                PointF start = PositionOf(actualDistance, angle);
                PointF end = PositionOf(actualEnd, angle);

                g.DrawLine(ThePen, start , end);
            }
        }

        #endregion

    }
}
