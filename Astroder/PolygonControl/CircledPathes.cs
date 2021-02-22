using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public delegate bool AngleFilterDelegate(float angle);

    public class CircledPathes : AngledItem
    {
        #region Definitions

        public static AngleFilterDelegate NoFilter = new AngleFilterDelegate(angle => false);

        #endregion

        #region Property

        public GraphicsPath Path { get; private set; }

        public float AngleOffset { get { return Angle; } }

        public int Repetition { get; set; }

        public AngleFilterDelegate BypassPredicate { get; set; }

        public bool IsFixed { get; set; }

        #endregion

        #region Constructors

        public CircledPathes(PolygonControl container, GraphicsPath path, float distance, float angleOffset, float rotation, int repetition, AngleFilterDelegate isBypassed, bool isFixed, Brush brush, Pen pen)
            : base(container, distance, angleOffset, rotation, brush, pen)
        {
            Path = path;
            Repetition = repetition;
            BypassPredicate = isBypassed;
            IsFixed = isFixed;
        }

        public CircledPathes(PolygonControl container, GraphicsPath path, float distance, int repetition, Pen pen) 
            : this(container, path, distance, 0, 0, repetition, NoFilter, false, null, pen) { }

        #endregion

        #region Functions

        //public override void Resize(float sizeFactor)
        //{
        //    //Distance *= sizeFactor;
        //    if (IsFixed)
        //        return;

        //    Path = Multiply(Path, Container.UnitSize);

        //    //List<PointF> points = new List<PointF>();
        //    //for (int i = 0; i < Path.PathPoints.Length; i++)
        //    //{
        //    //    PointF pt = Path.PathPoints[i];
        //    //    pt.X *= sizeFactor;
        //    //    pt.Y *= sizeFactor;
        //    //    points.Add(pt);
        //    //}

        //    //Path = new GraphicsPath(points.ToArray(), Path.PathTypes);
        //}

        //public override void Draw(Graphics g, ToControlPositionDelegate angleOf)
        //{
        //    if (!IsVisible)
        //        return;

        //    if (angleOf == null)
        //        angleOf = Container.RelativeToCenter;

        //    GraphicsPath all = new GraphicsPath();
        //    //float rotated = Container.AngleOffset;

        //    for (float angleLow = 0; angleLow < 360f; angleLow += 360f / Repetition)
        //    {
        //        GraphicsPath path = Multiply(Path, Container.UnitSize);
        //        if (BypassPredicate != null && BypassPredicate(angleLow))
        //            continue;

        //        Matrix matrix = new Matrix();

        //        PointF pos = angleOf(Distance, angleLow, 0);
        //        matrix.Translate(pos.X, pos.Y);

        //        float physicalAngle = Container.AngleOf(pos);
        //        matrix.Rotate(physicalAngle + Rotation);

        //        //angleLow = Container.PhysicalAngleOf(angleLow);
        //        //matrix.Rotate(angleLow);

        //        //matrix.Rotate((Container.IsClockwise ? angleLow : -angleLow) + rotated);

        //        path.Transform(matrix);

        //        all.AddPath(path, false);
        //    }

        //    drawPath(g, all);
        //}

        public override void Draw(Graphics g, ToPhysicalAngleDelegate angleOf)
        {
            if (!IsVisible)
                return;

            if (angleOf == null)
                angleOf = Container.PhysicalAngleOf;

            GraphicsPath all = new GraphicsPath();

            for (float angle = 0; angle < 360f; angle += 360f / Repetition)
            {
                GraphicsPath path = Multiply(Path, Container.UnitSize);
                if (BypassPredicate != null && BypassPredicate(angle))
                    continue;

                Matrix matrix = new Matrix();

                float actualDistance = Distance * Container.UnitSize;
                float actualAngle = angleOf(angle);
                float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
                float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
                PointF pos = new PointF(x, y);

                matrix.Translate(pos.X, pos.Y);

                matrix.Rotate(actualAngle + Rotation);

                path.Transform(matrix);

                all.AddPath(path, false);
            }

            drawPath(g, all);
        }

        #endregion

    }
}
