using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class CircleItem : GraphicItem
    {
        #region Properties
        public PointF Center { get; set; }

        public float Radius { get; set; }

        public bool IsFixed { get; set; }

        #endregion

        #region Constructors

        public CircleItem(PolygonControl container, PointF center, float radius, bool isFixed, Brush brush, Pen pen) : base(container, brush, pen)
        {
            Center = center;
            Radius = radius;
            IsFixed = isFixed;
        }

        public CircleItem(PolygonControl container, float radius, Brush brush, Pen pen) : base (container, brush, pen)       
        {
            Radius = radius;
            Center = new PointF(0, 0);
            IsFixed = false;
        }

        public CircleItem(PolygonControl container, float radius)  : this(container, radius, DefaultBrush, DefaultPen) {}
        #endregion

        #region functions

        //public override void Resize(float sizeFactor)
        //{
        //    if (!IsFixed)
        //        Radius *= sizeFactor;
        //}

        public override void Draw(Graphics g, ToPhysicalAngleDelegate angleOf)
        {
            if (!IsVisible)
                return;

            if (angleOf == null)
                angleOf = Container.PhysicalAngleOf;

            float actualDistance = Radius * Container.UnitSize;
            float actualAngle = angleOf(-45);
            float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
            float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);

            PointF corner1 = new PointF(x, y);
            PointF corner2 = new PointF(-x, -y);

            x = Math.Min(corner1.X, corner2.X);
            y = Math.Min(corner1.Y, corner2.Y);
            float width = Math.Max(corner1.X, corner2.X) - x;
            float height = Math.Max(corner1.Y, corner2.Y) - y;

            RectangleF rect = new RectangleF(x, y, width, height);

            if (IsFilled && TheBrush != Brushes.Transparent)
                g.FillEllipse(TheBrush, rect);

            if (IsBordered)
                g.DrawEllipse(ThePen, rect);
        }

        //public void Draw(Graphics g, ToControlPositionDelegate angleOf, bool hideBorder)
        //{
        //    if (!IsVisible)
        //        return;

        //    if (Center != new PointF(0, 0))
        //    {
        //        throw new NotImplementedException();
        //    }

        //    PointF border = angleOf(radius, 0, 0);
        //    float actualRadius = (float)Math.Sqrt(border.X * border.X + border.Y * border.Y);

        //    RectangleF rect = new RectangleF(- actualRadius, - actualRadius, actualRadius * 2, actualRadius * 2);

        //    if (IsFilled && TheBrush != Brushes.Transparent)
        //        g.FillEllipse(TheBrush, rect);

        //    if (IsBordered && !hideBorder)
        //        g.DrawEllipse(ThePen, rect);
        //}

        //public void Draw(Graphics g, ToControlPositionDelegate angleOf, bool hideBorder, float innerRadius)
        //{
        //    if (!IsVisible)
        //        return;

        //    PointF border = angleOf(radius, 0, 0);
        //    float actualRadius = (float)Math.Sqrt(border.X * border.X + border.Y * border.Y);
        //    int unitSize = (int)(actualRadius / radius);
        //    float xShift = Center.X * unitSize;
        //    float yShift = Center.Y * unitSize;
        //    RectangleF outRect = new RectangleF(xShift - actualRadius, yShift - actualRadius, actualRadius * 2, actualRadius * 2);

        //    if (IsFilled && TheBrush != Brushes.Transparent)
        //    {
        //        innerRadius = unitSize * innerRadius;
        //        RectangleF innerRect = new RectangleF(xShift - innerRadius, yShift - innerRadius, innerRadius * 2, innerRadius * 2);

        //        GraphicsPath handPath = new GraphicsPath();

        //        handPath.AddArc(outRect, 0, 180);
        //        handPath.AddLine(actualRadius, 0, innerRadius, 0);
        //        handPath.AddArc(innerRect, 0, 180);
        //        handPath.AddLine(-innerRadius, 0, -actualRadius, 0);
        //        g.FillPath(TheBrush, handPath);

        //        handPath.Initialize();

        //        handPath.AddArc(outRect, 180, 180);
        //        handPath.AddLine(actualRadius, 0, innerRadius, 0);
        //        handPath.AddArc(innerRect, 180, 180);
        //        handPath.AddLine(-innerRadius, 0, -actualRadius, 0);
        //        g.FillPath(TheBrush, handPath);
        //    }

        //    if (IsBordered && !hideBorder)
        //        g.DrawEllipse(ThePen, outRect);
        //}

        #endregion
    }
}
