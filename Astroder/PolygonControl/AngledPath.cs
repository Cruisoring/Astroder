using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class AngledPath : AngledItem
    {
        #region Fields

        /// <summary>
        /// 	Gets or sets the GraphicsPath to drawPath.
        /// </summary>
        /// <value>
        /// 	The GraphicsPath to drawPath.
        /// </value>
        public GraphicsPath Path { get; private set; }

        //public float Rotation { get; set; }

        public bool IsFixed { get; set; }

        #endregion

        #region Constructors

        public AngledPath(PolygonControl container, GraphicsPath path, float distance, float angle, float rotation, bool isFixed, Brush brush, Pen pen)
            : base(container, distance, angle, rotation, brush, pen)
        {
            Path = path;
            IsFixed = isFixed;
        }

        public AngledPath(PolygonControl container, GraphicsPath path, float distance, float angle) 
            : this(container, path, distance, angle, 0, false, DefaultBrush, DefaultPen) {}

        #endregion

        #region Functions

        public override void Draw(Graphics g, ToPhysicalAngleDelegate angleOf)
        {
            if (!IsVisible)
                return;

            if (angleOf == null)
                angleOf = Container.PhysicalAngleOf;

            GraphicsPath path = Multiply(Path, Container.UnitSize);

            if (Distance != 0 || Angle != 0)
            {
                float actualDistance = Distance * Container.UnitSize;
                float actualAngle = angleOf(Angle);
                float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
                float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
                PointF pos = new PointF(x, y);

                Matrix translateMatrix = new Matrix();
                translateMatrix.Translate(pos.X, pos.Y);

                translateMatrix.Rotate(actualAngle + Rotation);

                //if (Rotation != 0)
                //    translateMatrix.Rotate(Container.AngleOffset + (Container.IsClockwise ? Rotation : -Rotation));
                path.Transform(translateMatrix);
                translateMatrix.Dispose();
            }

            drawPath(g, path);

            path.Dispose();
        }

        //public override void Resize(float sizeFactor)
        //{
        //    Distance *= sizeFactor;
        //    if (IsFixed) return;

        //    //List<PointF> points = new List<PointF>();
        //    //for (int i = 0; i < Path.PathPoints.Length; i++)
        //    //{
        //    //    PointF pt = Path.PathPoints[i];
        //    //    pt.X *= sizeFactor;
        //    //    pt.Y *= sizeFactor;
        //    //    points.Add(pt);
        //    //}

        //    Path = Multiply(Path, Container.UnitSize);
        //}

        //public override void Draw(Graphics g, ToControlPositionDelegate angleOf)
        //{
        //    if (!IsVisible)
        //        return;

        //    if (angleOf == null)
        //        angleOf = Container.RelativeToCenter;

        //    GraphicsPath path = Multiply(Path, Container.UnitSize);

        //    if (Distance != 0 || Angle != 0)
        //    {
        //        PointF pos = angleOf(Distance, Angle, 0);
        //        Matrix translateMatrix = new Matrix();
        //        translateMatrix.Translate(pos.X, pos.Y);

        //        float physicalAngle = Container.PhysicalAngleOf(Angle);
        //        translateMatrix.Rotate(physicalAngle + Rotation);

        //        //if (Rotation != 0)
        //        //    translateMatrix.Rotate(Container.AngleOffset + (Container.IsClockwise ? Rotation : -Rotation));
        //        path.Transform(translateMatrix);
        //        translateMatrix.Dispose();
        //    }

        //    drawPath(g, path);

        //    path.Dispose();
        //}

        //public override void Draw(Graphics g, LogicalAngleConverterDelegate angleConverter, float unitSize, float angleOffset)
        //{
        //    Draw(g, unitSize, angleOffset, null);

        //}

        //public virtual void Draw(Graphics g, LogicalAngleConverterDelegate angleConverter, float unitSize, float angleOffset, Pen leadingPen)
        //{
        //    if (!IsVisible)
        //        return;

        //    GraphicsPath handPath = Path.Clone() as GraphicsPath;
        //    Matrix translateMatrix = new Matrix();

        //    float angleLow = Normalized(Angle + angleOffset);
        //    float cos = COS(angleLow);
        //    float sin = SIN(angleLow);
        //    float actualDistance = unitSize * Distance;
        //    PointF pos = new PointF(actualDistance * cos, actualDistance * sin);

        //    if (actualDistance != 0)
        //        translateMatrix.Translate(pos.X, pos.Y);

        //    float rotated = Normalized(Rotation + angleOffset);
        //    if (rotated != 0f)
        //        translateMatrix.Rotate(rotated);

        //    handPath.Transform(translateMatrix);
        //    translateMatrix.Dispose();

        //    if (IsFilled && TheBrush != null)
        //        g.FillPath(TheBrush, handPath);

        //    if (IsBordered && ThePen != null)
        //        g.DrawPath(ThePen, handPath);

        //    if (leadingPen != null)
        //        g.DrawLine(leadingPen, new PointF(3*unitSize*cos, 3*unitSize*sin), pos);

        //    handPath.Dispose();
        //}

        #endregion

    }
}
