using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class CircleSet : GraphicItemSet<CircleItem>
    {
        #region Definition

        public static Pen DefaultPen = Pens.DarkGray;
        public static Brush DefaultBrush = Brushes.Transparent;

        #endregion

        #region Properties

        public bool AllHideBorder { get; set; }

        public PointF Center { get; set; }

        #endregion

        #region Constructors

        public CircleSet(PolygonControl container) : this(container, new PointF(0, 0)){}

        public CircleSet(PolygonControl container, PointF center) : base(container)
        {
            AllHideBorder = false;
            Center = center;
        }
        
        #endregion

        #region functions

        protected override void Add(CircleItem t)
        {
            if (t.Center != Center)
                throw new Exception();

            int index = Count;

            for (int i = 0; i < Count; i ++ )
            {
                if (Items[i].Radius > t.Radius)
                    continue;

                index = i;
                break;
            }
            
            Items.Insert(index, t);
        }

        public void Add(PointF center, float radius, bool isFixed, Brush brush, Pen pen)
        {
            if (pen != null && !Toolkit.Contains(pen) && pen != DefaultPen)
                Toolkit.Add(pen);

            if (brush != null && !Toolkit.Contains(brush) && brush != DefaultBrush)
                Toolkit.Add(brush);

            Add(new CircleItem(Container, center, radius, isFixed, brush, pen));
        }

        public void Add(float radius) 
        {
            this.Add(new PointF(0,0), radius, false, DefaultBrush, DefaultPen);
        }

        public void Draw(Graphics g)
        {
            int unitSize = Container.UnitSize;
            float xShift = Center.X * unitSize;
            float yShift = Center.Y * unitSize;
            
            for (int i = 0; i < Count; i++)
            {
                CircleItem circle = Items[i];
                if (!circle.IsVisible) continue;

                float theRadius = circle.Radius * unitSize;
                RectangleF outRect = new RectangleF(xShift - theRadius, yShift - theRadius, theRadius * 2, theRadius * 2);

                if (i != Count - 1 && circle.IsFilled)
                {
                    float insideRadius = Items[i + 1].Radius * unitSize;
                    RectangleF inRect = new RectangleF(xShift - insideRadius, yShift - insideRadius, insideRadius * 2, insideRadius * 2);

                    GraphicsPath path = new GraphicsPath();

                    path.AddArc(outRect, 0, 180);
                    path.AddLine(theRadius, 0, insideRadius, 0);
                    path.AddArc(inRect, 0, 180);
                    path.AddLine(-insideRadius, 0, -theRadius, 0);
                    g.FillPath(circle.TheBrush, path);

                    path.Reset();

                    path.AddArc(outRect, 180, 180);
                    path.AddLine(theRadius, 0, insideRadius, 0);
                    path.AddArc(inRect, 180, 180);
                    path.AddLine(-insideRadius, 0, -theRadius, 0);
                    g.FillPath(circle.TheBrush, path);
                }
                else if (i == Count - 1 && circle.IsFilled)
                    g.FillEllipse(circle.TheBrush, outRect);

                if (circle.IsBordered && !AllHideBorder)
                    g.DrawEllipse(circle.ThePen, outRect);
            }

        }

        //public void Resize(float sizeFactor)
        //{
        //    foreach(CircleItem item in Items)
        //    {
        //        item.Resize(sizeFactor);
        //    }
        //}

        #endregion
    }
}
