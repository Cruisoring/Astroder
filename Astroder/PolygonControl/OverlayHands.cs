using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class OverlayHands : AngledItemSet
    {
        #region Definition

        private static float emptyDistance = 3;

        public static float HandleSize = 1.5f;

        //private static PointF[] buttonShape = new PointF[] { 
        //    new PointF(-0.5f, 0f), new PointF(0, 0.5f), 
        //    new PointF( 0.5f, 0f), new PointF( 0f, -0.5f), new PointF(-0.5f, 0f)
        //};

        #endregion

        #region Property

        public bool IsVisble { get; set; }

        public CircledPathes Hands { get; set; }
        //public List<AngledPath> Hands { get; set; }

        public AngledPath Indicator { get; set; }

        public AngledPath Handle { get; set; }

        public OverlayScheme Scheme { get; set; }

        #endregion

        #region Constructors

        public OverlayHands(PolygonControl container, int divisions) 
            : base(container)
        {
            Scheme = OverlayScheme.SchemeOf(divisions);
            IsVisble = true;
            float step = 360f / divisions;
            float radius = Ruler.RulerRadius(Container, 1);

            PointF[] vertices = PolygonControl.VerticesPointsOf(divisions);
            GraphicsPath buttonPath = new GraphicsPath();
            buttonPath.AddPolygon(vertices);
            buttonPath = AngledItem.Multiply(buttonPath, HandleSize);
            Indicator = AddPath(buttonPath, radius, step, 0, false, 
                new SolidBrush(Color.FromArgb(180, Scheme.BaseColor)), new Pen(Color.FromArgb(127, Scheme.BaseColor)));

            buttonPath = new GraphicsPath();
            buttonPath.AddEllipse(-HandleSize, -HandleSize, 2*HandleSize, 2*HandleSize);
            Handle = AddPath(buttonPath, radius, 0, 0, false, new SolidBrush(Color.FromArgb(200, Color.White)), new Pen(Color.FromArgb(127, Color.Yellow)));

            Pen pen = new Pen(Color.FromArgb(127, Scheme.DarkColor), 0.5f);
            pen.DashStyle = DashStyle.Dash;
            SolidBrush brush = new SolidBrush(Color.FromArgb(127, Scheme.LightColor));

            GraphicsPath handPath = new GraphicsPath();
            RectangleF outRect = new RectangleF(-radius, -radius, 2 * radius, 2 * radius);
            RectangleF inRect = new RectangleF(-emptyDistance, -emptyDistance, 2 * emptyDistance, 2 * emptyDistance);

            float sin = PolygonControl.SIN(Scheme.Orb);
            float cos = PolygonControl.COS(Scheme.Orb);
            handPath.AddLine(radius * cos, radius * sin, emptyDistance * cos, sin * emptyDistance);
            handPath.AddArc(inRect, Scheme.Orb, -2 * Scheme.Orb);
            handPath.AddLine(cos * emptyDistance, -sin * emptyDistance, radius * cos, -radius * sin);
            handPath.AddArc(outRect, 360f - Scheme.Orb, 2 * Scheme.Orb);
            //handPath.AddLine(new PointF(0, 0), new PointF(radius, 0));
            //handPath.CloseAllFigures();

            Hands = AddPathSet(handPath, 0, 0, divisions, false, brush, pen);
            //Hands = new List<AngledPath>();

            //for (float angleLow = step; angleLow <= 360; angleLow += step )
            //{
            //    Hands.Add(AddPath(handPath, 0, 0, angleLow, false, brush, pen));
            //}
        }

        public OverlayHands(OverlayHands old)
            : this(old.Container, old.Hands.Repetition)
        {
            IsVisble = old.IsVisble;
            AngleOffset = old.AngleOffset;
        }


        #endregion

        #region Functions


        //public void Resize(float sizeFactor)
        //{
        //    Indicator.Resize(sizeFactor);
        //    Hands.Resize(sizeFactor);
        //}

        //public void SetHands()
        //{
        //    Color color1 = Color.FromArgb(127, Scheme.LightColor);
        //    Color color2 = Color.FromArgb(127, Scheme.DarkColor);
        //    float sin = PolygonControl.SIN(Scheme.Orb);
        //    float cos = PolygonControl.COS(Scheme.Orb);
        //    float actualRadius = radius * UnitSize;
        //    float innerRadius = emptyDistance * UnitSize;

        //    Pen pen = new Pen(color2, 0.5f);
        //    pen.DashStyle = DashStyle.Dash;
        //    GraphicsPath path = new GraphicsPath();
        //    RectangleF outRect = new RectangleF(-actualRadius, -actualRadius, 2 * actualRadius, 2 * actualRadius);
        //    RectangleF inRect = new RectangleF(- innerRadius, -innerRadius, 2 * innerRadius, 2* innerRadius);

        //    path.AddLine(actualRadius * cos, actualRadius * sin, innerRadius * cos, sin * innerRadius);
        //    path.AddArc(inRect, Scheme.Orb, -2 * Scheme.Orb);
        //    path.AddLine(cos * innerRadius, -sin * innerRadius, actualRadius * cos, -actualRadius * sin);
        //    path.AddArc(outRect, 360f - Scheme.Orb, 2 * Scheme.Orb);

        //    SolidBrush brush = new SolidBrush(color1);
        //    //LinearGradientBrush brush = new LinearGradientBrush(new PointF(innerRadius, 0), new PointF(theRadius, 0), color2, color1);

        //    //for (int i = 0; i < HandsNumber; i++)
        //    //{
        //    //    float angleLow = Container.PhysicalAngleOf(AngleOffset + (i+1) * Scheme.SweepAngle);
        //    //    this.AddPath(handPath, 0, angleLow, angleLow, brush, null);
        //    //    this.AddPath(handPath, 0, angleLow, angleLow, null, pen);
        //    //    this.AddRadiateLine(1f, angleLow, radius - 1, new Pen(WeightedColor(Scheme.BaseColor)));
        //    //    if (i == 0)
        //    //    {
        //    //        this.AddPath(ButtonPath, radius-0.5f, angleLow, angleLow, brush, pen);
        //    //    }
        //    //}

        //}

        public Color WeightedColor(Color baseColor, float percentage, bool isTransparent)
        {
            int a = isTransparent ? (int)(255 * percentage) : baseColor.A;
            int r = (int)(baseColor.R * percentage);
            int g = (int)(baseColor.G * percentage);
            int b = (int)(baseColor.B * percentage);
            return Color.FromArgb(a, r, g, b);
        }

        public Color WeightedColor(Color baseColor, float percentage)
        {
            return WeightedColor(baseColor, percentage, true);
        }

        public Color WeightedColor(Color baseColor)
        {
            return WeightedColor(baseColor, 1, true);
        }

        #endregion
    }
}
