using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public abstract class AngledItem : GraphicItem
    {
        public static GraphicsPath Multiply(GraphicsPath original, float unitSize)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < original.PathPoints.Length; i++)
            {
                PointF pt = original.PathPoints[i];
                pt.X *= unitSize;
                pt.Y *= unitSize;
                points.Add(pt);
            }

            return new GraphicsPath(points.ToArray(), original.PathTypes);
        }


        #region Properties & Fields

        /// <summary>
        /// 	Gets or sets the distanceLow from the center of the control to the center of the text.
        /// </summary>
        /// <value>
        /// 	The distanceLow from the center of the control to the center of the text.
        /// </value>
        public float Distance { get; set; }

        /// <summary>
        /// 	Gets or sets the angular location of the center of the text.
        /// </summary>
        /// <value>
        /// 	The angular location of the center of the text (0 - 360).
        /// </value>
        public float Angle { get; set; }

        /// <summary>
        /// 	Gets or sets the physicalAngle at which the text is rotated.
        /// </summary>
        /// <value>
        /// 	The physicalAngle at which the text is rotated.
        /// </value>
        public float Rotation { get; set; }

        #endregion

        #region Constructors

        protected AngledItem(PolygonControl container, float distance, float angle, float rotation) : base(container)
        {
            Distance = distance;
            Angle = angle;
            Rotation = rotation;
        }

        protected AngledItem(PolygonControl container, float distance, float angle, float rotation, Brush brush, Pen pen)
            : base(container, brush, pen)
        {
            Distance = distance;
            Angle = angle;
            Rotation = rotation;
        }
        #endregion
        
        #region Functions

        //public override void Resize(float sizeFactor)
        //{
        //    Distance *= sizeFactor;
        //}

        #endregion
    }
}
