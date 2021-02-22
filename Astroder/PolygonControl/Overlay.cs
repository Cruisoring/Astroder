using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class Overlay : IDisposable
    {
        //public delegate void OverlayChangedDelegate();

        #region Property

        public bool IsOverlayChanged { get; set; }

        public PolygonControl Container { get; set; }

        public float AngleOffset
        {
            get { return Ring.AngleOffset; }
            set 
            {
                if (Ring.AngleOffset != value)
                {
                    Ring.AngleOffset = GraphicItem.Normalized(value);
                    IsOverlayChanged = true;
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                if (Ring.IsVisible)
                    return true;
                else
                {
                    foreach (OverlayHands hands in HandSets.Values)
                    {
                        if (hands.IsVisble)
                            return true;
                    }
                    return false;
                }
            }
            set
            {
                if (IsVisible == value)
                    return;

                if (value)
                {
                    Ring.IsVisible = true;

                    AddOrShowHands(8, 1f);
                    AddOrShowHands(6, 1f);
                    AddOrShowHands(4, 1f);
                    AddOrShowHands(3, 1f);
                }
                else
                {
                    foreach (OverlayHands hands in HandSets.Values)
                    {
                        if (hands.IsVisble)
                        {
                            hands.IsVisble = false;
                        }
                    }
                    Ring.IsVisible = false;
                }
                IsOverlayChanged = true; 
            }
        }

        public Ruler Ring { get; set; }

        public Dictionary<int, OverlayHands> HandSets { get; set; }

        //public bool ShowScale
        //{
        //    get { return Scale.IsVisible; }
        //    set { Scale.IsVisible = value; }
        //}

        #endregion

        #region Constructors

        public Overlay(PolygonControl container, Color scaleColor)
        {
            Container = container;

            HandSets = new Dictionary<int, OverlayHands>();

            Ring = new Ruler(container, true, 2, scaleColor);
            Ring.IsVisible = false;

            IsOverlayChanged = true;

        }

        public Overlay(PolygonControl container, int division, Color scaleColor) : this (container, scaleColor)
        {
            OverlayHands newHands = new OverlayHands(container, division);
            HandSets.Add(division, newHands);

            //ScaleColor = scaleColor;
            //Circles = new CircleSet(Container);
            //Circles.Add(container.radius - overlayMargin);

            //Circles.Add(container.radius - overlayMargin, new SolidBrush(WeightedColor(ScaleColor, 0.2f)), new Pen(WeightedColor(ScaleColor), 2));
            //Circles.Add(3f, null, new Pen(WeightedColor(ScaleColor, 0.5f), 2));

            //TicksAndLables = new AngledItemSet(container);
            //SetScale();

            //Scale = new Dialer(container, true, container.radius -3, 15, 15, 5, -1, 30, 1f, Color.FromArgb(0, Color.Yellow), Color.Transparent,
            //    Color.FromArgb(0, Color.Turquoise), Color.FromArgb(127, Color.LightSkyBlue), Color.FromArgb(127, Color.Orange));
            //ShowScale = true;
            //Scale.Circles.Add(1f, null, new Pen(WeightedColor(ScaleColor, 0.5f), 2));

            //Scale = new Dialer(container, true, radius - Dialer.DefaultLoopWidthInPoints, radius, 1,
            //    WeightedColor(ScaleColor, 1f), Color.Transparent, WeightedColor(ScaleColor, 0.8f),
            //    WeightedColor(Dialer.DefaultLoopColor, 0.5f), WeightedColor(Dialer.DefaultLabelColor));

            //AddOrShowHands(division, 1f);
        }

        public Overlay(Overlay existing)
        {
            Container = existing.Container;
            Ring = new Ruler(Container, existing.Ring.IsTransparent, 2, existing.Ring.ScaleColor);

            HandSets = new Dictionary<int, OverlayHands>();

            foreach (KeyValuePair<int, OverlayHands>kvp in existing.HandSets)
            {
                HandSets.Add(kvp.Key, new OverlayHands(kvp.Value));
            }

            IsOverlayChanged = true;
        }

        #endregion

        #region Functions

        public float ToPhysicalAngle(float angle)
        {
            angle += AngleOffset;
            return Container.PhysicalAngleOf(angle);
        }

        public bool IsOnHandle(float angle, float distance)
        {
            if (HandSets.Count == 0)
                return false;

            float radius = Ruler.RulerRadius(Container, 1);
            float angleDif = angle > AngleOffset ? angle - AngleOffset : AngleOffset - angle;
            float cos = PolygonControl.COS(angleDif);
            double distanceToHandle = Math.Sqrt(distance * distance + radius * radius - 2 * distance * radius * cos);

            return distanceToHandle <= OverlayHands.HandleSize;
        }

        public void Draw(Graphics g)
        {
            if (IsVisible)
            {
                if (Ring.IsVisible)
                    Ring.Draw(g);

                foreach (OverlayHands hands in HandSets.Values)
                {
                    if (hands.IsVisble)
                        hands.Draw(g, ToPhysicalAngle);
                }
            }

        }
        //public void Resize(float factor)
        //{
        //    foreach (CircleItem circle in Circles)
        //    {
        //        if (circle.radius != Math.Round(circle.radius))
        //            circle.radius *= factor;
        //    }

        //    foreach (AngledItem item in TicksAndLables)
        //    {
        //        item.Distance *= factor;
        //    }
        //}

        //public void SetScale()
        //{
        //    float scaleDistance = Container.radius - overlayMargin;
            //Circles.Add(InsideRadius, Brushes.AliceBlue, Pens.White);

            //TicksAndLables.AddTicks(InsideRadius, -0.4f, new Pen(ScaleColor, 3), 15);
            //TicksAndLables.AddTicks(InsideRadius, -0.2f, new Pen(ScaleColor, 2), 5, 15);
            //TicksAndLables.AddTicks(InsideRadius, -0.2f, new Pen(ScaleColor, 1), 2, 5);

            //List<string> signs = new List<string>();
            //for (char c = 'a'; c < 'm'; c++)
            //{
            //    signs.Add(c.ToString());
            //}

            //TicksAndLables.AddLables(signs, new Font("AstroGadget", 24f, FontStyle.Bold), new SolidBrush(IsTransparent ? Color.FromArgb(0, Color.Crimson) : Color.Crimson),
            //    InsideRadius + 0.1f, SignOffset, 90f, null, null);
            //TicksAndLables.AddLables(new Font("AstroGadget", 9f, FontStyle.Regular), new SolidBrush(WeightedColor(ScaleColor, 0.5f)),
            //    InsideRadius - 0.9f, 15f, null, null);
        //}

        public void AddOrShowHands(int divisions, float orb)
        {
            if (!HandSets.ContainsKey(divisions))
            {
                OverlayHands hands = new OverlayHands(Container, divisions);
                HandSets.Add(divisions, hands);
            }
            else
            {
                if (!HandSets[divisions].IsVisble)
                    HandSets[divisions].IsVisble = true;
            }

            HandSets[divisions].Scheme.Orb = orb;
        }

        //public void AddHands(int times, float orb, float offset, Color color1, Color color2)
        //{
        //    color1 = Color.FromArgb(0, color1);
        //    color2 = Color.FromArgb(128, color2);
        //    float sin = PolygonControl.SIN(orb);
        //    float cos = PolygonControl.COS(orb);

        //    float angleLow = 0;
        //    int unitSize = Container.UnitSize;
        //    float theRadius = radius * unitSize;

        //    Pen pen = new Pen(color2, 0.5f);
        //    pen.DashStyle = DashStyle.Dash;
        //    GraphicsPath handPath = new GraphicsPath();
        //    RectangleF outRect = new RectangleF(-theRadius, -theRadius, 2 * theRadius, 2 * theRadius);
        //    RectangleF inRect = new RectangleF(-3 * unitSize, -3 * unitSize, 6 * unitSize, 6 * unitSize);

        //    //handPath.AddRadiateLine(new PointF(0, 0), new PointF(theRadius, 0));
        //    handPath.AddLine(theRadius * cos, theRadius * sin, 3 * cos * unitSize, 3 * sin * unitSize);
        //    handPath.AddArc(inRect, orb, -2 * orb);
        //    handPath.AddLine(3 * cos * unitSize, -3 * sin * unitSize, theRadius * cos, -theRadius * sin);
        //    handPath.AddArc(outRect, 360f-orb, 2*orb);

        //    LinearGradientBrush brush = new LinearGradientBrush(new PointF(0, 0), new PointF(radius * unitSize, 0), color2, color1);

        //    for (int i = 0; i < times; i++)
        //    {
        //        angleLow = HandSets.PhysicalAngleOf(offset + i * 360f / times);
        //        HandSets.AddPath(handPath, 0, angleLow, 0, brush, null);
        //        HandSets.AddPath(handPath, 0, angleLow, 0, null, pen);
        //        HandSets.AddRadiateLine(1f, angleLow, radius-1, new Pen(WeightedColor(ScaleColor)));
        //    }

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

        #region IDisposable 成员

        public void Dispose()
        {
            if (Ring != null)
                Ring.Dispose();

            foreach (OverlayHands hands in HandSets.Values)
            {
                hands.Dispose();
            }
        }

        #endregion
    }
}
