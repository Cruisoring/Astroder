using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    #region Definitions
    /// <summary>
    /// 	Specifies how a <c>GraphicItem</c> is filled.
    /// </summary>
    //public enum BrushMode
    //{
    //    /// <summary>
    //    /// 	Specifies a solid fill.
    //    /// </summary>
    //    SolidBrush,

    //    TextureBrush,

    //    /// <summary>
    //    /// 	Specifies a hatch fill.
    //    /// </summary>
    //    HatchBrush,

    //    /// <summary>
    //    /// 	Specifies a linear gradient fill.
    //    /// 	The gradient is spread across the client area of the control.
    //    /// </summary>
    //    LinearGradientBrush,

    //    PathGradientBrush
    //}

    #endregion

    public delegate float LogicalAngleConverterDelegate(float logicalAngle);
    //public delegate PointF ToControlPositionDelegate(float distanceLow, float angleLow, float ptShift);
    public delegate float ToPhysicalAngleDelegate(float angle);     

    public abstract class GraphicItem
    {
        #region Helper functions

        public const float RadiansPerDegree = (float)(Math.PI / 180.0);
        public const float DegreesPerRadian = (float)(180 / Math.PI);

        public static Pen DefaultPen = Pens.DarkGray;
        public static Brush DefaultBrush = null;

        // Helper method - calculate cosine of physicalAngle in degrees
        public static float COS(float angle)
        {
            float rad = RadiansPerDegree * angle;
            float cos = (float)Math.Cos(rad);
            return cos;
        }

        // Helper method - calculate sine of physicalAngle in degrees
        public static float SIN(float angle)
        {
            float rad = RadiansPerDegree * angle;
            float sin = (float)Math.Sin(rad);
            return sin;
        }

        public static PointF PositionOf(float distance, float angle)
        {
            return new PointF(distance * COS(angle), distance * SIN(angle));
        }

        public static float Normalized(float a)
        {
            a %= 360.0f;
            if (a < 0.0f)
            {
                a += 360.0f;
            }
            return a;
        }

        public float AdjustRotation(float physicalAngle, float rotation)
        {
            float result = Normalized(physicalAngle + rotation);

            return (result > 180) ?
                 result - 180 : result;
        }

        #endregion

        #region Properties

        public PolygonControl Container { get; set; }

        public bool IsVisible { get; set; }

        public bool IsBordered { get{ return ThePen != null;} }

        public bool IsFilled { get { return TheBrush != null; } }

        public Brush TheBrush { get; set; }

        public Pen ThePen { get; set; }

        #endregion

        #region Constructor
        protected GraphicItem(PolygonControl container, Brush brush, Pen pen)
        {
            Container = container;
            TheBrush = brush;
            ThePen = pen;

            IsVisible = true;
        }

        protected GraphicItem(PolygonControl container)
        {
            Container = container;
            TheBrush = DefaultBrush;
            ThePen = DefaultPen;

            IsVisible = true;
        }
        #endregion

        public void Draw(Graphics g)
        {
            Draw(g, Container.PhysicalAngleOf);
        }

        public abstract void Draw(Graphics g, ToPhysicalAngleDelegate angleOf);

        protected void drawPath(Graphics g, GraphicsPath path)
        {
            if (IsFilled)
                g.FillPath(TheBrush, path);

            if (IsBordered)
                g.DrawPath(ThePen, path);
        }

        //public abstract void Resize(float sizeFactor);
    }
}
