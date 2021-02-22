using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class Ruler : IDisposable
    {
        #region Constants
        public const float RulerMargin = 0.1f;

        public static float RulerRadius(PolygonControl Container, float index)
        {
            return Container.Calculator.Radius + index * (Ruler.DefaultLoopWidthInPoints / Container.UnitSize + RulerMargin);
        }            

        public static Brush DefaultOutsideBrush = Brushes.LightGray;
        public static Brush DefaulInsideBrush = Brushes.LightCyan;
        public static Pen DefaultBorderPen = Pens.Black;

        public static Color DefaultScaleColor = Color.DarkBlue;
        public static Color DefaultLabelColor = Color.Blue;

        public static Color DefaultLoopColor = Color.LightGray;
        public static Color DefaultFaceColor = Color.LightCyan;
        public static Color DefaultBorderColor = Color.Black;

        public const float DefaultLoopWidthInPoints = 40f;
        public const float DefaultBorderWidth = 1.2f;

        public const int DefaultMajorTickStep = 15;
        public const int DefaultMiddleTickStep = 5;
        public const int DefaultMinorTickStep = 1;

        #endregion

        #region Property

        public PolygonControl Container { get; set; }

        public bool IsVisible { get; set; }

        public CircleSet Circles { get; set; }

        public AngledItemSet TicksAndLables { get; set; }

        public float AngleOffset { get; set; }

        #region Appearence settings

        public bool IsTransparent { get; set; }

        public float OutsideRadius { get; set; }

        public float InsideRadius { get; set; }

        public float BorderWidth { get; set; }

        private Color borderColor = DefaultBorderColor;
        public Color BorderColor 
        { 
            get { return IsTransparent ? Color.FromArgb(127, borderColor) : borderColor; } 
            set { borderColor = value; }
        }

        private Color faceColor = DefaultFaceColor;
        public Color FaceColor
        {
            get { return IsTransparent ? Color.FromArgb(127, faceColor) : faceColor; }
            set { faceColor = value; }
        }

        private Color loopColor = DefaultLoopColor;
        public Color LoopColor
        {
            get { return IsTransparent ? Color.FromArgb(127, loopColor) : loopColor; }
            set { loopColor = value; }
        }

        private Color scaleColor = DefaultScaleColor;
        public Color ScaleColor
        {
            get { return IsTransparent ? Color.FromArgb(127, scaleColor) : scaleColor; }
            set { scaleColor = value; }
        }

        private Color lableColor = DefaultLabelColor;
        public Color LabelColor
        {
            get { return IsTransparent ? Color.FromArgb(127, lableColor) : lableColor; }
            set { lableColor = value; }
        }

        #endregion

        #region Scale settings

        public int MajorTickStep { get; set; }

        public int MiddleTickStep { get; set; }

        public int MinorTickStep { get; set; }

        public int LabelStep { get; set; }

        public int SignOffset { get; set; }

        #endregion

        #endregion

        #region Constructors

        public Ruler(PolygonControl container, 
            bool isTransparent, 
            float insideRadius,
            int signOffset,
            int majorTick,
            int middleTick,
            int minorTick,
            int labelTick,
            float borderWidth, 
            Color borderColor, 
            Color faceColor, 
            Color loopColor, 
            Color scaleColor, 
            Color labelColor
            )
        {
            Container = container;
            IsVisible = true;
            Circles = new CircleSet(container);
            TicksAndLables = new AngledItemSet(container);

            IsTransparent = isTransparent;

            InsideRadius = insideRadius;
            OutsideRadius = insideRadius + DefaultLoopWidthInPoints / Container.UnitSize;
            BorderWidth = borderWidth;

            BorderColor = borderColor;
            FaceColor = faceColor;
            LoopColor = loopColor;
            ScaleColor = scaleColor;
            LabelColor = labelColor;

            MajorTickStep = majorTick;
            MiddleTickStep = middleTick;
            MinorTickStep = minorTick;
            LabelStep = labelTick;
            SignOffset = signOffset;

            AngleOffset = 0;

            Initialize();

            //Container.MaxCycleChanged += new PolygonControl.MaxCycleChangedDelegate(Container_MaxCycleChanged);
            //Container.UnitSizeChanged += new PolygonControl.UnitSizeChangedDelegate(Container_UnitSizeChanged);
            //Container.CalculatorChanged += new PolygonControl.CalculatorChangedDelegate(container_CalculatorChanged);
        }

        public Ruler(PolygonControl container, bool isTransparent, int index)
            : this(container, isTransparent, RulerRadius(container, index-1)){}

        public Ruler(PolygonControl container, bool isTransparent, float insideRadius)
            : this(container, 
            isTransparent, 
            insideRadius,
            DefaultMajorTickStep,
            DefaultMajorTickStep,
            DefaultMiddleTickStep,
            DefaultMinorTickStep,
            DefaultMajorTickStep,
            DefaultBorderWidth, 
            DefaultBorderColor, 
            isTransparent ? Color.Transparent : DefaultFaceColor, 
            DefaultLoopColor, 
            DefaultScaleColor, 
            DefaultLabelColor) {}

        public Ruler(PolygonControl container, bool isTransparent, int index, Color scaleColor) 
            : this(container, isTransparent, index)
        {
            ScaleColor = scaleColor;
        }

        public Ruler(PolygonControl container, bool isTransparent, float innerSize, 
            int signOffset,
            int majorTick,
            int middleTick,
            int minorTick,
            int labelTick
            )
            : 
            this(container,
            isTransparent,
            innerSize,
            signOffset,
            majorTick,
            middleTick,
            minorTick,
            labelTick,
            DefaultBorderWidth,
            DefaultBorderColor,
            DefaultFaceColor,
            DefaultLoopColor,
            DefaultScaleColor,
            DefaultLabelColor) { }

        #endregion

        //public PointF ToControlPosition(float distanceLow, float angleLow, float ptShift)
        //{
        //    angleLow += AngleOffset;
        //    return Container.RelativeToCenter(distanceLow, angleLow, ptShift);
        //}

        public float ToPhysicalAngle(float angle)
        {
            angle += AngleOffset;
            return Container.PhysicalAngleOf(angle);
        }

        public void Clear()
        {
            Circles.Clear();
            TicksAndLables.Clear();
        }

        public void Initialize()
        {
            Circles.Mapper = this.ToPhysicalAngle;
            TicksAndLables.Mapper = this.ToPhysicalAngle;

            Circles.Add(new PointF(), OutsideRadius, false, new SolidBrush(LoopColor), new Pen(BorderColor, BorderWidth));
            Circles.Add(new PointF(), InsideRadius, false, IsTransparent ? new SolidBrush(Color.Transparent) : new SolidBrush(FaceColor), new Pen(BorderColor, BorderWidth));

            #region Add Ticks

            if (MajorTickStep > 0)
            {
                Pen majorPen = new Pen(ScaleColor, 3);
                TicksAndLables.AddLines(InsideRadius, 0, -0.4f, MajorTickStep, majorPen, null, false);
                TicksAndLables.AddLines(OutsideRadius-0.2f, 0, -0.4f, MajorTickStep, majorPen, null, false);
            }

            if (MiddleTickStep > 0)
            {
                Pen middlePen = new Pen(ScaleColor, 2);
                AngleFilterDelegate filter = new AngleFilterDelegate(x => x % MajorTickStep == 0);
                TicksAndLables.AddLines(InsideRadius, 0, -0.2f, MiddleTickStep, middlePen, filter, false);
                TicksAndLables.AddLines(OutsideRadius-0.1f, 0, -0.2f, MiddleTickStep, middlePen, filter, false);
            }

            if (MinorTickStep > 0)
            {
                Pen minorPen = new Pen(ScaleColor, 1);
                //AngleFilterDelegate filter = new AngleFilterDelegate(x => x % MiddleTickStep == 0);
                TicksAndLables.AddLines(InsideRadius, 0, -0.2f, MinorTickStep, minorPen, (x => x % MiddleTickStep == 0), true);
                TicksAndLables.AddLines(OutsideRadius - 0.1f, 0, -0.2f, MinorTickStep, minorPen, (x => x % MiddleTickStep == 0), true);
            }

            #endregion

            float middle = (InsideRadius + OutsideRadius) / 2f;

            if (LabelStep > 0)
                TicksAndLables.AddLables(new Font("AstroGadget", 9f, FontStyle.Regular), new SolidBrush(LabelColor),
                    middle, 0, 0, 12, null, null);

            if (SignOffset > 0)
            {
                List<string> signs = new List<string>();
                for (char c = 'a'; c < 'm'; c++)
                {
                    signs.Add(c.ToString());
                }

                TicksAndLables.AddLables(signs, new Font("AstroGadget", 24f, FontStyle.Bold), 
                    new SolidBrush(IsTransparent ? Color.FromArgb(127, Color.Crimson) : Color.Crimson), 
                    middle, SignOffset, -90f, null, null);
            }

        }

        //void container_CalculatorChanged()
        //{
        //    InsideRadius = Container.Calculator.radius;
        //    OutsideRadius = InsideRadius + DefaultLoopWidthInPoints / Container.UnitSize;

        //    Clear();
        //    Initialize();
        //}

        //void Container_UnitSizeChanged(float oldSize, float newSize)
        //{
        //    OutsideRadius = InsideRadius + DefaultLoopWidthInPoints / newSize;

        //    Clear();
        //    Initialize();
        //}

        //void Container_MaxCycleChanged(int oldMax, int newMax)
        //{
        //    InsideRadius = Container.Calculator.radius;
        //    OutsideRadius = InsideRadius + DefaultLoopWidthInPoints / Container.UnitSize;

        //    Clear();
        //    Initialize();
        //}

        public void Draw(Graphics g)
        {
            if (IsVisible)
            {
                Circles.Draw(g);
                TicksAndLables.Draw(g);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Circles.Dispose();
            TicksAndLables.Dispose();
        }

        #endregion
    }
}
