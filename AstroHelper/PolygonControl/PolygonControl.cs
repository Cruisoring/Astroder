using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using NumberHelper;
using QuoteHelper;
using AstroHelper;

namespace PolygonControl
{
    public enum FirstQuadrantOrientation
    {
        LeftDown,
        LeftUp,
        RightDown,
        RightUp
    }

    [ToolboxBitmap(typeof(System.Windows.Forms.Timer))]
    public partial class PolygonControl : UserControl
    {
        #region Helper functions, classes and delegate definitions

        public const char OneBefore = '\u2460';
        public const char CeilingSymbol = '\u2227';
        public const char FloorSymbol = '\u2228';
        public const char FocusedSymbol = '\u272f';

        public static SearchMode DefaultSearchMode = SearchMode.AroundWorkingDay;

        public static Pen UpPathPen = new Pen(Color.FromArgb(128, Color.Red), 3);

        public static Pen DownPathPen = new Pen(Color.FromArgb(128, Color.Green), 3);

        public static Pen AroundPen = new Pen(Color.FromArgb(128, Color.Gray), 3);

        // Painting radials

        private const float radiansPerDegree = (float)(Math.PI / 180.0);
        private const float degreesPerRadian = (float)(180 / Math.PI);

        // Helper method - calculate cosine of physicalAngle in degrees
        public static float COS(float angle)
        {
            float rad = radiansPerDegree * angle;
            float cos = (float)Math.Cos(rad);
            return cos;
        }

        // Helper method - calculate sine of physicalAngle in degrees
        public static float SIN(float angle)
        {
            float rad = radiansPerDegree * angle;
            float sin = (float)Math.Sin(rad);
            return sin;
        }

        public static PointF[] VerticesPointsOf(int edges)
        {
            float sinValue = SIN(180/edges);

            float bevelLength = 0.5f;

            PointF[] vertices = new PointF[edges + 1];

            for (int i = 0; i < edges; i++)
            {
                float angle = 360*i/edges + (edges == 4 ? 45f : 0f);
                vertices[i].X = (float)Math.Round(bevelLength * COS(angle), 5);
                vertices[i].Y = (float)Math.Round(bevelLength * SIN(angle), 5);
            }

            vertices[edges] = vertices[0];

            return vertices;
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

        public delegate void AdapterChangedDelegate();

        public delegate void CalculatorChangedDelegate();

        public delegate void OrientationChangedDelegate();

        public delegate void HighlightChangedDelegate();

        
        #endregion

        #region Fields and properties

        private ToolTip theToolTip = new ToolTip();

        public event CalculatorChangedDelegate CalculatorChanged;

        public event OrientationChangedDelegate OrientationChanged;

        public event AdapterChangedDelegate AdapterChanged;

        public event HighlightChangedDelegate HighlightChanged;

        private PolygonCalculator calculator = null;
        public PolygonCalculator Calculator {
            [DebuggerStepThrough]
            get{ return calculator;} 
            set
            {
                if (value != calculator)
                {
                    calculator = value;

                    if (CalculatorChanged != null)
                    {
                        CalculatorChanged();
                    }

                    QuoteRangs.Clear();

                    Redraw();
                }
            }
        }

        [Browsable(true), Category("Appearance"), DefaultValue(15)]
        public int MaxCycle 
        {
            get { return Calculator.MaxCycle; }
            set
            {
                if (value != calculator.MaxCycle)
                {
                    Calculator.MaxCycle = value;
                    Redraw();
                }
            }
        }

        private int unitSize = 20;
        [Browsable(true), Category("Appearance"), DefaultValue(30)]
        public int UnitSize {
            [DebuggerStepThrough]
            get { return unitSize; }
            set 
            {
                if (value != unitSize)
                {
                    unitSize = value;
                    Redraw();
                }
            }
        }

        int xUnit { get { return IsXReversed ? -unitSize : unitSize; } }

        int yUnit { get { return IsYReversed ? -unitSize : unitSize; } }

        private FirstQuadrantOrientation firstQuadrant = FirstQuadrantOrientation.LeftDown;
        public FirstQuadrantOrientation FirstQuadrant 
        {
            [DebuggerStepThrough]
            get { return firstQuadrant; }
            //[DebuggerStepThrough]
            set
            {
                if (value != firstQuadrant && OrientationChanged != null)
                {
                    OrientationChanged();
                }
                firstQuadrant = value;
                Redraw();
            }
        }

        public bool IsXReversed {
            get { return firstQuadrant == FirstQuadrantOrientation.LeftDown || firstQuadrant == FirstQuadrantOrientation.LeftUp; }
        }

        public bool IsYReversed
        {
            [DebuggerStepThrough]
            get { return firstQuadrant == FirstQuadrantOrientation.LeftUp || firstQuadrant == FirstQuadrantOrientation.RightUp; }
        }

        public bool IsClockwise { get { return FirstQuadrant == FirstQuadrantOrientation.RightDown || firstQuadrant == FirstQuadrantOrientation.LeftUp; } }

        public float AngleOffset { get { return (IsXReversed ? 180 : 0) ; } }

        private PointF Center { get { return new PointF((float)(pictureBox1.Width)/2.0F, (float)(pictureBox1.Height)/2.0F) ; } }

        public float Radius 
        { 
            get
            {
                return Ruler.RulerRadius(this, 2);
            }
        }

        public Overlay TheOverlay { get; set; }

        //public PivotSet PivotsHolder { get; set; }

        public PlanetSet PlanetHolder { get; set; }

        //public PivotWrapper Highlighted 
        //{
        //    get
        //    {
        //        foreach (PivotWrapper pivot in PivotsHolder)
        //        {
        //            if (pivot.IsHighlighted)
        //                return pivot;
        //        }
        //        return null;
        //    }
        //}

        private PriceAdapter adapter = null;
        public PriceAdapter Adapter
        {
            get { return adapter; }
            set
            {
                if (value == null || adapter == null)
                {
                    adapter = value;
                    return;
                }
                else if (value.Step != adapter.Step || value.Zero != adapter.Zero || value.Rule != adapter.Rule)
                {
                    adapter = value;

                    QuoteRangs.Clear();

                    if (AdapterChanged != null)
                    {
                        AdapterChanged();
                    }
                    Redraw();
                }
            }
        }

        public QuoteCollection History { get; set; }

        private DateTimeOffset date;
        public DateTimeOffset Date 
        { 
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;

                    HighlightDate(value);

                    getQuotePath();
                }
            }
        }

        public bool DisplayQuote { get; set; }

        public Dictionary<DateTimeOffset, GraphicsPath> QuoteRangs { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// 	Default constructor.
        /// </summary>
		/// <remarks>
		/// 	The default constructor creates a control with SquareNine calculator.
		/// </remarks>
        public PolygonControl()
        {
            InitializeComponent();

            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
            //    | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            //SetStyle(ControlStyles.Opaque, true);
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            calculator = new PolygonCalculator(this, Polygon.SquareNine);

            //PivotsHolder = new PivotSet(this);
            PlanetHolder = new PlanetSet(this);

            this.Size = FittedSize();

            DisplayQuote = true;

            QuoteRangs = new Dictionary<DateTimeOffset, GraphicsPath>();
        }

        public PolygonControl(QuoteCollection history)
            : this()
        {
            History = history;
        }

        #endregion

        #region Functions

        #region Convertion functions

        public float PhysicalAngleOf(float logicalAngle)
        {
            float result = IsClockwise ? logicalAngle : -logicalAngle;
            result += AngleOffset;
            return Normalized(result);
        }

        public float LogicalAngleOf(float physicalAngle)
        {
            float angle = (IsClockwise ? 1 : -1) * (physicalAngle - AngleOffset);

            float range = 360;

            angle %= range;

            if (angle < 0)
                angle += range;

            return angle;
        }

        public void AngleDistanceOf(Point location, out float angle, out float distance)
        {
            float x = (IsXReversed ? Center.X - location.X : location.X - Center.X) / UnitSize;
            float y = (IsYReversed ? Center.Y - location.Y : location.Y - Center.Y) / UnitSize;

            float refAngel = (float)Math.Atan(y/x) * degreesPerRadian;

            angle = (x >= 0) ? refAngel : 180 + refAngel;
            if (angle < 0)
                angle = 360 + angle;

            distance = (float)Math.Sqrt(x * x + y * y);
        }

        public PointF ToLogicalPosition(Point physicalPos)
        {
            //SizeF size = new SizeF(physicalPos.X - Center.X, physicalPos.Y - Center.Y);
            float x = IsXReversed ? (Center.X  - physicalPos.X) : (physicalPos.X - Center.X);
            float y = IsYReversed ? (Center.Y - physicalPos.Y) : (physicalPos.Y - Center.Y);
            return new PointF(x, y);
        }

        public PointF RelativeToCenter(float distance, float angle, float ptShift)
        {
            float actualDistance = distance * UnitSize + ptShift;
            float actualAngle = PhysicalAngleOf(angle);
            float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
            float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
            return new PointF(x, y);
        }

        public double DistanceBetween(float distance1, float angle1, float distance2, float angle2)
        {
            PointF pos1 = RelativeToCenter(distance1, angle1, 0);
            PointF pos2 = RelativeToCenter(distance2, angle2, 0);
            SizeF dist = new SizeF(pos1.X-pos2.X, pos1.Y-pos2.Y);

            return Math.Sqrt(dist.Width*dist.Width + dist.Height * dist.Height);
        }

        public float RadiusOf(PointF logicalPos)
        {
            throw new NotImplementedException();

            //float x = logicalPos.X;
            //float y = logicalPos.Y;
            //float length = (float)Math.Sqrt(x * x + y * y);
            //return length / UnitSize;
        }

        public float RadiusOf(Double indexValue)
        {
            PointF position = Calculator.PositionOfIndex(indexValue);
            return RadiusOf(position);
        }

        public float AngleOf(PointF logicalPos)
        {
            float x = logicalPos.X;
            float y = logicalPos.Y;
            float tan = y / x;

            float refAngel = (float)Math.Atan(tan) * degreesPerRadian;

            return (x >= 0) ? refAngel : 180 + refAngel;
        }

        public float AngleOf(Double indexValue)
        {
            PointF position = Calculator.PositionOfIndex(indexValue);
            return AngleOf(position);
        }

        public float DistanceBetween(Double first, Double second)
        {
            float angle1 = AngleOf(first);
            float angle2 = AngleOf(second);
            int round1 = Calculator.Shape.RoundOf(first);
            int round2 = Calculator.Shape.RoundOf(second);

            return (round2 - round1) * 360f + angle2 - angle1;
        }

        #endregion

        #region Drawing & resizing functions

        private void PolygonControl_SizeChanged(object sender, EventArgs e)
        {
            if (this.BackgroundImage != null)
                this.BackgroundImage.Dispose();

            this.BackgroundImage = getBackgroundImage();

            if (TheOverlay != null)
            {
                Overlay newOverlay = new Overlay(TheOverlay);
                TheOverlay.Dispose();
                TheOverlay = newOverlay;
            }
            else
                TheOverlay = new Overlay(this, Color.Brown);

            if (pictureBox1.BackgroundImage != null)
                pictureBox1.BackgroundImage = null;

            pictureBox1.BackgroundImage = getOverlayImage();
        }


        private Bitmap getBackgroundImage()
        {
            Ruler theRuler = new Ruler(this, false, 1);

            Bitmap bgrd = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bgrd);
            g.SmoothingMode = SmoothingMode.HighQuality;
            //g.FillRectangle(Brushes.Transparent, pictureBox1.ClientRectangle);
            g.Clear(Color.Wheat);

            g.TranslateTransform(Center.X, Center.Y);

            theRuler.Draw(g);

            Font indexFont;

            float xFactor = IsXReversed ? -UnitSize : UnitSize;
            float yFactor = IsYReversed ? -UnitSize : UnitSize;

            if (Calculator.Shape.IsPolygon)
            {
                float fontSize = Math.Min(this.Font.Size, (float)Math.Round(UnitSize * 0.9f));
                indexFont = new Font(this.Font.FontFamily, fontSize);

                SizeF textSize;
                PointF upperLeft;

                Pen dashPen = new Pen(Color.LightGray, 1);
                dashPen.DashStyle = DashStyle.Dash;
                for (int round = 1; round <= MaxCycle; round++)
                {
                    PointF[] corners = (from pt in Calculator.VerticesPositions[round].Values
                                        select new PointF(pt.X * xFactor, pt.Y * yFactor)).ToArray();
                    g.DrawPolygon(dashPen, corners);

                }
                dashPen.Dispose();

                for (int index = 1; index <= Calculator.Shape.LastOfRound(MaxCycle); index++)
                {
                    PointF centerPos = Calculator.PositionOfIndex(index);

                    Brush brush = (Calculator.IsVertice(index)) ? Brushes.Navy : Brushes.DarkGray;

                    textSize = g.MeasureString(index.ToString(), indexFont);
                    if (textSize.Width > UnitSize)
                    {
                        Font newFont = new Font(indexFont.FontFamily, (float)(indexFont.SizeInPoints - 1));
                        indexFont.Dispose();
                        indexFont = newFont;
                        textSize = g.MeasureString(index.ToString(), indexFont);
                    }

                    upperLeft = new PointF((centerPos.X * xFactor - textSize.Width / 2), (centerPos.Y * yFactor - textSize.Height / 2));
                    g.DrawString(index.ToString(), indexFont, brush, upperLeft);
                }
            }
            else
            {
                float fontSize = Math.Min(this.Font.Size, (float)UnitSize);
                indexFont = new Font(this.Font.FontFamily, fontSize);

                //SizeF textSize;
                //PointF upperLeft;

                CircleSet circles = new CircleSet(this);
                Double tan = Math.Tan(Math.PI / Calculator.Shape.Edges);
                float radius = (float)(0.5 / tan - 0.5);
                SolidBrush evenBrush = new SolidBrush(Color.Transparent);
                SolidBrush oddBrush = new SolidBrush(Color.BlanchedAlmond);

                circles.Add(radius);

                for (int i = 1; i <= Calculator.MaxCycle; i ++ )
                {
                    circles.Add(new PointF(), radius + i, false, i % 2 == 1 ? oddBrush : evenBrush, null);
                }

                circles.Draw(g);
                circles.Dispose();

                Pen dashPen = new Pen(Color.LightGray, 1);
                dashPen.DashStyle = DashStyle.Dash;

                float step = 360/Calculator.Shape.Edges;
                PointF start, end;
                float angle, cos, sin;
                float startLen = radius * unitSize;
                float endLen = (radius + step) * unitSize;

                for (int i = 0; i < Calculator.Shape.Edges; i ++ )
                {
                    angle = i * step;
                    cos = COS(angle);
                    sin = SIN(angle);

                    start = new PointF(cos * startLen, sin * startLen);
                    end = new PointF(cos * endLen, sin * endLen);
                    g.DrawLine(dashPen, start, end);
                }

                dashPen.Dispose();

                //AngledItemSet ticks = new AngledItemSet(this);
                //ticks.AddLines(radius, 0, Calculator.Shape.Edges, 360/Calculator.Shape.Edges, dashPen, null, false);
                //ticks.Draw(g);
                //ticks.Dispose();

                float distance, degree;

                for (int index = 1; index <= Calculator.Shape.LastOfRound(MaxCycle); index++)
                {
                    Calculator.AngleDistanceOf(index, out degree, out distance);

                    AngledLabel label = new AngledLabel(this, index.ToString(), indexFont, Brushes.Navy, distance, degree, -90f);
                    label.Draw(g);

                    //PointF centerPos = Calculator.PositionOfIndex(index);

                    //textSize = g.MeasureString(index.ToString(), indexFont);
                    //upperLeft = new PointF((centerPos.X * xFactor - textSize.Width / 2), (centerPos.Y * yFactor - textSize.Height / 2));
                    //g.DrawString(index.ToString(), indexFont, Brushes.Navy, upperLeft);
                }
            }


            indexFont.Dispose();
            theRuler.Dispose();
            g.Dispose();

            return bgrd;
        }

        private Bitmap getOverlayImage()
        {
            //if (TheOverlay == null || !TheOverlay.IsVisible)
            if (TheOverlay == null)
                return null;

            Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighQuality;

            //g.FillRectangle(Brushes.Transparent, pictureBox1.ClientRectangle);
            g.Clear(Color.Transparent);

            g.TranslateTransform(Center.X, Center.Y);

            if (TheOverlay == null)
                TheOverlay = new Overlay(this, Color.Brown);

            TheOverlay.Draw(g);

            return image;
        }

        public Size FittedSize()
        {
            float actualDiameter = Radius * UnitSize * 2;

            int width = (int)Math.Ceiling(actualDiameter);

            return new System.Drawing.Size(width, width);
        }

        public void Redraw()
        {
            this.Size = FittedSize();

            if (this.BackgroundImage != null)
                this.BackgroundImage.Dispose();

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }

            getQuotePath();

            this.BackgroundImage = getBackgroundImage();

            OverlayRedraw();
        }

		public void OverlayRedraw()
		{
            if (pictureBox1.BackgroundImage != null)
                pictureBox1.BackgroundImage = null;

            pictureBox1.BackgroundImage = getOverlayImage();
            TheOverlay.IsOverlayChanged = false;

			Invalidate();
        }

        public void HighlightDate(DateTimeOffset concernedDate)
        {
            PlanetHolder.Date = concernedDate;

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }

            //if (PivotsHolder.Count == 0)
            //    return;

            //List<OutlineItem> earliers = (from pivot in PivotsHolder.Items
            //                                 where pivot.Pivot.Date <= concernedDate
            //                                 select pivot.Pivot).ToList();

            //int center = earliers.Count != 0 ? (earliers.Count - 1) : 0;
            //int pivotIndex = (earliers.Count != 0 && earliers.Last().Date == concernedDate) ? center : -1;

            //PivotsHolder.Focused = (pivotIndex == -1) ? null : PivotsHolder.Items[pivotIndex];

            //for (int i = 0; i < PivotsHolder.Count; i++)
            //{
            //    PivotWrapper pivot = PivotsHolder.Items[i];
            //    pivot.IsHighlighted = ( i == pivotIndex );
            //    pivot.IsVisible = pivot.DisplayHint = (i <= center + PivotSet.NeighborHood && i >= center - PivotSet.NeighborHood);

            //    if (pivot.DisplayHint)
            //    {
            //        if (i>=center)
            //        {
            //            pivot.Hint = (i - center).ToString();
            //        }
            //        else
            //        {
            //            char ch = (char)(center-i-1+OneBefore);
            //            pivot.Hint = ch.ToString();
            //        }
            //    }
            //}

            //if (TheOverlay.IsVisible && pivotIndex != -1)
            //{
            //    TheOverlay.AngleOffset = PivotsHolder.Items[pivotIndex].Position.Angle;
            //}
            //else if (pivotIndex == -1)
            //{
            //    AstroHelper.Position starPos = AstroHelper.Ephemeris.CurrentEphemeris[concernedDate, AstroHelper.PlanetId.SE_SUN];
            //    TheOverlay.AngleOffset = (float)(starPos.Longitude);
            //}

        }

        private void addArc(GraphicsPath path, float radius, float angleLow, float angleHigh)
        {
            float actualRadius = radius * UnitSize;
            float sweep = IsClockwise ? (angleHigh - angleLow) : (angleLow-angleHigh);

            angleLow = PhysicalAngleOf(angleLow);
            //angleHigh = PhysicalAngleOf(angleHigh);

            if (sweep != 0)
            {
                path.AddArc(-actualRadius, -actualRadius, 2 * actualRadius, 2 * actualRadius, angleLow, sweep);
            }
            else
                path.AddArc(-actualRadius, -actualRadius, 2 * actualRadius, 2 * actualRadius, angleLow-0.1f, 0.2f);

        }

        public GraphicsPath PathOf(double low, double high)
        {
            GraphicsPath path = new GraphicsPath();

            float angleLow, angleHigh, distanceLow, distanceHigh;
            PointF p1, p2;

            List<Double> corners = new List<Double>();

            double lowIndex = adapter.IndexOf(low);
            double highIndex = adapter.IndexOf(high);

            if (lowIndex > highIndex)
            {
                double temp = lowIndex;
                lowIndex = highIndex;
                highIndex = temp;
            }
            else if(low == high)
            {
                lowIndex -= 0.5f;
                highIndex += 0.5f;

                p1 = Calculator.PositionOfIndex(lowIndex);
                p2 = Calculator.PositionOfIndex(highIndex);
                path.AddLine(p1.X * xUnit, p1.Y * yUnit, p2.X * xUnit, p2.Y * yUnit);
                return path;
            }

            int roundLow = Calculator.Shape.RoundOf(lowIndex);
            int roundHigh = Calculator.Shape.RoundOf(highIndex);
            int first, last;

            if (Calculator.Shape.IsPolygon)
            {
                #region Handling of polygon shape
                corners.Add(lowIndex);

                for (int round = roundLow; round <= roundHigh; round++)
                {
                    List<int> vertices = new List<int>();

                    vertices.Add(Calculator.Shape.FirstOfRound(round));

                    foreach (KeyValuePair<int, PointF> kvp in Calculator.VerticesPositions[round])
                    {
                        vertices.Add(kvp.Key);
                    }

                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (vertices[i] < lowIndex)
                            continue;
                        else if (vertices[i] > highIndex)
                            break;
                        else
                            corners.Add(vertices[i]);
                    }

                }

                if (!corners.Contains(highIndex))
                    corners.Add(highIndex);

                PointF before = Calculator.PositionOfIndex(corners[0]);

                for (int i = 1; i < corners.Count; i++)
                {
                    PointF next = Calculator.PositionOfIndex(corners[i]);
                    path.AddLine(before.X * xUnit, before.Y * yUnit, next.X * xUnit, next.Y * yUnit);
                    before = next;
                }
                #endregion
            }
            else if (roundHigh != 1 || roundLow != 360/Calculator.Edges)
            {
                #region handling of normal cases of circular shape
                lowIndex %= 360;
                highIndex %= 360;

                corners.Add(lowIndex);

                for (int round = roundLow; round <= roundHigh; round++)
                {
                    first = Calculator.Shape.FirstOfRound(round);
                    last = Calculator.Shape.LastOfRound(round);

                    if (first > highIndex || last < lowIndex)
                        break;

                    if (first > lowIndex && first < highIndex)
                        corners.Add(first);

                    if (last > lowIndex && last < highIndex)
                        corners.Add(last);
                }

                if (!corners.Contains(highIndex))
                    corners.Add(highIndex);

                if (corners.Count == 2)
                {
                    Calculator.AngleDistanceOf(lowIndex, out angleLow, out distanceLow);
                    Calculator.AngleDistanceOf(highIndex, out angleHigh, out distanceHigh);

                    if (angleHigh < angleLow)
                        angleHigh += 360;

                    if (Math.Abs(distanceHigh - distanceLow) < 0.0001)
                        addArc(path, distanceLow, angleLow, angleHigh);
                    else
                    {
                        p1 = Calculator.PositionOfIndex(lowIndex);
                        p2 = Calculator.PositionOfIndex(highIndex);
                        path.AddLine(p1.X * xUnit, p1.Y * yUnit, p2.X * xUnit, p2.Y * yUnit);                        
                    }
                }
                else
                {
                    for (int i = 1; i < corners.Count; i++)
                    {
                        if (corners[i] % Calculator.Edges == 1 || corners[i - 1] % Calculator.Edges == 0)
                        {
                            p1 = Calculator.PositionOfIndex(corners[i - 1]);
                            p2 = Calculator.PositionOfIndex(corners[i]);
                            path.AddLine(p1.X * xUnit, p1.Y * yUnit, p2.X * xUnit, p2.Y * yUnit);
                        }
                        else
                        {
                            Calculator.AngleDistanceOf(corners[i - 1], out angleLow, out distanceLow);
                            Calculator.AngleDistanceOf(corners[i], out angleHigh, out distanceHigh);

                            addArc(path, distanceLow, angleLow, angleHigh);
                        }
                    }
                }
                #endregion
            }
            else    //Special case when price pass 360
            {
                lowIndex %= 360;
                highIndex %= 360;

                corners.Add(lowIndex);

                first = Calculator.Shape.FirstOfRound(roundLow);
                last = Calculator.Shape.LastOfRound(roundLow);

                if (first > lowIndex)
                    corners.Add(first);

                if (last > lowIndex)
                    corners.Add(last);

                if (last + 0.5f > lowIndex)
                    corners.Add(last + 0.5f);

                first = Calculator.Shape.FirstOfRound(roundHigh);
                last = Calculator.Shape.LastOfRound(roundHigh);
                
                if (first-0.5f < highIndex)
                    corners.Add(first-0.5f);

                if (first < highIndex)
                    corners.Add(first);

                if (last < highIndex)
                    corners.Add(last);

                if (!corners.Contains(highIndex))
                    corners.Add(highIndex);

                for (int i = 1; i < corners.Count; i++)
                {
                    Calculator.AngleDistanceOf(corners[i - 1], out angleLow, out distanceLow);
                    Calculator.AngleDistanceOf(corners[i], out angleHigh, out distanceHigh);

                    if (angleHigh == angleLow)
                        continue;
                    else if (Math.Abs(distanceHigh - distanceLow) < 0.0001)
                        addArc(path, distanceLow, angleLow, angleHigh);
                    else
                    {
                        p1 = Calculator.PositionOfIndex(corners[i-1]);
                        p2 = Calculator.PositionOfIndex(corners[i]);
                        path.AddLine(p1.X * xUnit, p1.Y * yUnit, p2.X * xUnit, p2.Y * yUnit);
                    }
                }

            }
            



            return path;
        }

        public GraphicsPath PathAround(ref DateTimeOffset date)
        {
            Double dateValue = Math.Round(date.UtcDateTime.ToOADate(), 4);
            List<double> around =
                    (from dt in History.Dates
                     where dt - dateValue < 3 && dt - dateValue > -3
                     orderby Math.Abs(dt - dateValue)
                     select dt).ToList();

            if (around.Count == 0)
                return null;

            Quote quote;

            if (around.Contains(dateValue))
            {
                quote = History[date];
                if (quote != null)
                    return PathOf(quote.Low, quote.High);
                else
                    return null;
            }
            //else if (around.Count > 1 && ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) 
            //    || Math.Abs(Math.Abs(around[0] - dateValue) - Math.Abs(around[1] - dateValue)) <= 1))
            else if (around.Count > 1 && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) )
            {
                DateTimeOffset date0 = new DateTimeOffset(DateTime.FromOADate(around[0]), TimeSpan.Zero);
                DateTimeOffset date1 = new DateTimeOffset(DateTime.FromOADate(around[1]), TimeSpan.Zero);
                quote = History[date0];
                Quote quote2 = History[date1];
                double low = Math.Min(quote.Low, quote2.Low);
                double high = Math.Max(quote.High, quote2.High);
                date = (Math.Abs(date0.DateTime.ToOADate()-dateValue) <= Math.Abs(date1.DateTime.ToOADate()-dateValue)) ? date0 : date1;
                return PathOf(low, high);
            }
            else
            {
                DateTimeOffset nearDate = new DateTimeOffset(DateTime.FromOADate(around[0]), TimeSpan.Zero);
                date = nearDate;
                quote = History[nearDate];
                return PathOf(quote.Low, quote.High);
            }
        }

        public void GetPathes(List<Double> dateValues)
        {
            //if (QuoteRangs.Count > 1)
            QuoteRangs.Clear();

            foreach (double dateValue in dateValues)
            {
                DateTimeOffset date = new DateTimeOffset(DateTime.FromOADate(dateValue), TimeSpan.Zero);

                GraphicsPath path = PathAround(ref date);

                if (path != null)
                {
                    if (!QuoteRangs.ContainsKey(date))
                    {
                        QuoteRangs.Add(date, path);
                        //Console.WriteLine("Path of {0} is added to QuoteRanges.", date);
                    }
                    else
                    {
                        //Console.WriteLine("Path of {0} is added already.",  date);
                    }
                }
                else
                {
                    //Console.WriteLine("Failed to get path of {0}.", date);
                }
            }

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }
        }

        private void getQuotePath()
        {
            if (History == null || Date < History.Since || (QuoteRangs.Count == 1 && QuoteRangs.ContainsKey(Date)))
                return;

            QuoteRangs.Clear();

            DateTimeOffset date = Date;
            GraphicsPath path = PathAround(ref date);

            QuoteRangs.Add(date, path);
        }


        //public GraphicsPath PathBetween(Double lowIndex, Double highIndex)
        //{
        //    GraphicsPath path = new GraphicsPath();

        //    Double indexTemp;
        //    float angleLow, angleHigh, distanceLow, distanceHigh;

        //    if (lowIndex > highIndex)
        //    {
        //        indexTemp = lowIndex;
        //        lowIndex = highIndex;
        //        highIndex = indexTemp;
        //    }

        //    int roundLow = Calculator.Shape.RoundOf(lowIndex);
        //    int roundHigh = Calculator.Shape.RoundOf(highIndex);

        //    List<Double> corners = new List<Double>();

        //    if (!Calculator.Shape.IsPolygon)
        //    {
        //        lowIndex %= 360;
        //        highIndex %= 360;

        //        corners.Add(lowIndex);
        //        int first, last;

        //        for (int round = roundLow; round <= roundHigh; round++)
        //        {
        //            first = Calculator.Shape.FirstOfRound(round);
        //            last = Calculator.Shape.LastOfRound(round);

        //            if (first > highIndex || last < lowIndex)
        //                break;

        //            if (first > lowIndex && first < highIndex)
        //                corners.Add(first);

        //            if (last > lowIndex && last < highIndex)
        //                corners.Add(last);
        //        }

        //        if (!corners.Contains(highIndex))
        //            corners.Add(highIndex);

        //        if (corners.Count == 2)
        //        {
        //            Calculator.AngleDistanceOf(lowIndex, out angleLow, out distanceLow);
        //            Calculator.AngleDistanceOf(highIndex, out angleHigh, out distanceHigh);

        //            addArc(path, distanceLow, angleLow, angleHigh);
        //        }
        //        else
        //        {
        //            for (int i = 1; i < corners.Count; i ++ )
        //            {
        //                if (corners[i] % Calculator.Edges == 1 || corners[i-1] % Calculator.Edges == 0 )
        //                {
        //                    PointF p1 = Calculator.PositionOfIndex(corners[i - 1]);
        //                    PointF p2 = Calculator.PositionOfIndex(corners[i]);
        //                    path.AddLine(p1.X * xUnit, p1.Y * yUnit, p2.X * xUnit, p2.Y * yUnit);
        //                }
        //                else
        //                {
        //                    Calculator.AngleDistanceOf(corners[i-1], out angleLow, out distanceLow);
        //                    Calculator.AngleDistanceOf(corners[i], out angleHigh, out distanceHigh);

        //                    addArc(path, distanceLow, angleLow, angleHigh);
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        corners.Add(lowIndex);

        //        for (int round = roundLow; round <= roundHigh; round ++ )
        //        {
        //            List<int> vertices = new List<int>();

        //            vertices.Add(Calculator.Shape.FirstOfRound(round));

        //            foreach (KeyValuePair<int, PointF>kvp in Calculator.VerticesPositions[round])
        //            {
        //                vertices.Add(kvp.Key);
        //            }

        //            for (int i = 0; i < vertices.Count; i ++ )
        //            {
        //                if (vertices[i] < lowIndex)
        //                    continue;
        //                else if (vertices[i] > highIndex)
        //                    break;
        //                else
        //                    corners.Add(vertices[i]);
        //            }

        //        }

        //        if (!corners.Contains(highIndex))
        //            corners.Add(highIndex);

        //        PointF before = Calculator.PositionOfIndex(corners[0]);

        //        for (int i = 1; i < corners.Count; i ++ )
        //        {
        //            PointF next = Calculator.PositionOfIndex(corners[i]);
        //            path.AddLine(before.X * xUnit, before.Y * yUnit, next.X * xUnit, next.Y * yUnit);
        //            before = next;
        //        }
        //    }

        //    return path;

        //}

        #endregion

        private bool isRotating = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            float angle, radius;
            AngleDistanceOf(e.Location, out angle, out radius);

            if (TheOverlay.IsVisible && radius > Ruler.RulerRadius(this, 0))
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (TheOverlay.IsOnHandle(angle, radius) && !isRotating)
                    {
                        this.Cursor = Cursors.Hand;
                        isRotating = true;
                    }
                    else if (isRotating)
                    {
                        this.Cursor = Cursors.Default;
                        isRotating = false;
                        TheOverlay.AngleOffset = angle;
                        pictureBox1.Invalidate();

                    }
                }

            }
            //else if (PivotsHolder.IsVisible && radius < Radius)
            //{
            //    PivotWrapper pivot = PivotsHolder.NearestPivot(angle, radius);

            //    if (pivot == null)
            //        return;

            //    if (e.Button == MouseButtons.Left)
            //    {
            //        this.Cursor = Cursors.Hand;
            //        if (pivot.IsHighlighted != true)
            //        {
            //            HighlightDate(pivot.Pivot.Date);
            //        }
            //    }
            //    else if (e.Button == MouseButtons.Right && pivot.IsHighlighted)
            //    {
            //        foreach (PivotWrapper pvt in PivotsHolder)
            //        {
            //            pvt.IsHighlighted = false;
            //            pvt.IsVisible = true;
            //            pvt.DisplayHint = false;
            //        }

            //        this.pictureBox1.Image.Dispose();
            //        this.pictureBox1.Image = null;
            //    }

            //    if (HighlightChanged != null)
            //    {
            //        HighlightChanged();
            //    }

            //}
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            float angle, radius;
            AngleDistanceOf(e.Location, out angle, out radius);
            PlanetId id;


            if (TheOverlay.IsVisible && isRotating)
            {
                TheOverlay.AngleOffset = angle;

                pictureBox1.Invalidate();
            }
            else if (PlanetHolder.IsVisible & radius > Ruler.RulerRadius(this, 0))
            {
                id = PlanetHolder.NearestPlanet(angle);
                if (id != PlanetId.SE_ECL_NUT)
                {
                    PlanetEvents detail = new PlanetEvents(Ephemeris.CurrentEphemeris, PlanetHolder.Date, id);

                    theToolTip.SetToolTip(this.pictureBox1, detail.ToString());
                    theToolTip.Active = true;
                }
                else
                    theToolTip.Active = false;
            }
            else if (radius < Ruler.RulerRadius(this, 0))
            {
                id = PlanetHolder.NearestPlanet(angle, radius);

                if (id != PlanetId.SE_ECL_NUT)
                {
                    Position pos = Ephemeris.CurrentEphemeris[PlanetHolder.Date, id];
                    String speedString;
                    int round = Calculator.Shape.RoundOf(pos.Longitude);
                    //double angleLow = PlanetHolder.Positions[id].Angle;

                    switch (pos.Owner)
                    {
                        case PlanetId.SE_MOON:
                            speedString = String.Format("{0:F4}/H", pos.LongitudeVelocity / 24);
                            break;
                        case PlanetId.SE_SUN:
                        case PlanetId.SE_MERCURY:
                        case PlanetId.SE_VENUS:
                        case PlanetId.SE_MARS:
                            speedString = String.Format("{0:F4}/D", pos.LongitudeVelocity);
                            break;
                        default:
                            speedString = String.Format("{0:F4}/W", pos.LongitudeVelocity * 7);
                            break;
                    }

                    StringBuilder sb = new StringBuilder();


                    sb.AppendFormat("{0}@{1:dddd}: {2:F4}({3}), round {4}", Planet.SymbolOf(id), PlanetHolder.Date, pos.Longitude, speedString, round);

                    theToolTip.SetToolTip(this.pictureBox1, sb.ToString());
                    theToolTip.Active = true;

                }
                else if (DisplayQuote)
                {
                    PointF mousePos = new PointF(e.Location.X - Center.X, e.Location.Y-Center.Y);

                    foreach (KeyValuePair<DateTimeOffset, GraphicsPath> kvp in QuoteRangs)
                    {
                        if (kvp.Value != null && kvp.Value.IsOutlineVisible(mousePos, UpPathPen))
                        {
                            theToolTip.SetToolTip(this.pictureBox1, History[kvp.Key].ToString());
                            theToolTip.Active = true;
                            return;
                        }
                    }

                    theToolTip.Active = false;

                    //if (RangePath.IsOutlineVisible(mousePos, UpPathPen))
                    //{
                    //    theToolTip.SetToolTip(this.pictureBox1, String.Format("Logical geoPos is on ({0})", mousePos));
                    //    theToolTip.Active = true;
                    //}
                    //else
                    //{
                    //    theToolTip.SetToolTip(this.pictureBox1, String.Format("Mouse is on ({0}, logically as {1})", e.Location, mousePos));
                    //    theToolTip.Active = true;
                    //}
                }
                //else if (PivotsHolder.IsVisible)
                //{
                //    PivotWrapper pivot = PivotsHolder.NearestPivot(angle, radius);

                //    if (pivot != null && pivot.IsVisible)
                //    {
                //        StringBuilder sb = new StringBuilder();
                //        sb.AppendLine(pivot.ToString());

                //        PivotDifference difference = null;

                //        if (pivot != PivotsHolder.Ceiling)
                //        {
                //            difference = new PivotDifference(pivot, PivotsHolder.Ceiling);

                //            sb.AppendFormat("\t{0}({1}@{2:yyMMdd}): {3}({4}D,{5}T,{6}N,{7}X) -- ",
                //                CeilingSymbol, PivotsHolder.Ceiling.Pivot.Price, PivotsHolder.Ceiling.Pivot.Date,
                //                difference.Changes.PriceChanged, difference.Changes.TimeElapsed.TotalDays,
                //                Math.Abs(difference.Changes.IndexChanged),
                //                difference.Changes.TimeElapsed.TotalDays % 365, difference.Changes.TimeElapsed.TotalDays % 360);

                //            sb.AppendLine(difference.Rotated.ToString());
                //        }

                //        if (pivot != PivotsHolder.Floor)
                //        {
                //            difference = new PivotDifference(pivot, PivotsHolder.Floor);

                //            sb.AppendFormat("\t{0}({1}@{2:yyMMdd}): {3}({4}D,{5}T,{6}N,{7}X) -- ",
                //                FloorSymbol, PivotsHolder.Floor.Pivot.Price, PivotsHolder.Floor.Pivot.Date,
                //                difference.Changes.PriceChanged, difference.Changes.TimeElapsed.TotalDays,
                //                Math.Abs(difference.Changes.IndexChanged),
                //                difference.Changes.TimeElapsed.TotalDays % 365, difference.Changes.TimeElapsed.TotalDays % 360);

                //            sb.Append(difference.Rotated.ToString());
                //        }

                //        if (PivotsHolder.Focused != null && pivot != PivotsHolder.Focused)
                //        {
                //            difference = new PivotDifference(pivot, PivotsHolder.Focused);

                //            sb.AppendFormat("\r\n\t{0}({1}@{2:yyMMdd}): {3}({4}D,{5}T,{6}N,{7}X) -- ",
                //                FocusedSymbol, PivotsHolder.Focused.Pivot.Price, PivotsHolder.Focused.Pivot.Date,
                //                difference.Changes.PriceChanged, difference.Changes.TimeElapsed.TotalDays,
                //                Math.Abs(difference.Changes.IndexChanged),
                //                difference.Changes.TimeElapsed.TotalDays % 365, difference.Changes.TimeElapsed.TotalDays % 360);

                //            sb.Append(difference.Rotated.ToString());
                //        }

                //        theToolTip.SetToolTip(this.pictureBox1, sb.ToString());
                //        theToolTip.Active = true;
                //    }
                //    else
                //        theToolTip.Active = false;
                //}
                else
                    theToolTip.Active = false;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (TheOverlay.IsVisible && this.Cursor == Cursors.Hand)
            {
                this.Cursor = Cursors.Default;
                this.Cursor = Cursors.Default;

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(image);

                g.TranslateTransform(Center.X, Center.Y);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                PlanetHolder.Draw(g);

                if (DisplayQuote)
                    showQuotes(g);

                g.Dispose();

                pictureBox1.Image = image;
            }
            else if (TheOverlay.IsOverlayChanged)
            {
                OverlayRedraw();
            }
        }

        private void showQuotes(Graphics g)
        {
            if (DisplayQuote && QuoteRangs != null && QuoteRangs.Count != 0)
            {
                foreach (KeyValuePair<DateTimeOffset, GraphicsPath> kvp in QuoteRangs)
                {
                    if (kvp.Value == null)
                        continue;
                    else if (kvp.Key == Date)
                    {
                        Quote qt = History[kvp.Key];
                        g.DrawPath(qt.Close >= qt.Open ? UpPathPen : DownPathPen, kvp.Value);
                    }
                    else
                        g.DrawPath(AroundPen, kvp.Value);
                }
            }
        }




        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripProperty_Click(object sender, EventArgs e)
        {
            PropertiesForm prop = new PropertiesForm(this);
            prop.ShowDialog();
        }

        private void highlightTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSettingForm setTime = new TimeSettingForm(this);

            setTime.ShowDialog();
        }

        private void PolygonControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Up:
                    Date.AddDays(-1);
                    break;
                case Keys.Down:
                    Date.AddDays(1);
                    break;
                case Keys.Right:
                    Date.AddDays(-7);
                    break;
                case Keys.Left:
                    Date.AddDays(7);
                    break;
                case Keys.PageDown:
                    Date.AddDays(30);
                    break;
                case Keys.PageUp:
                    Date.AddDays(-30);
                    break;
                default:
                    break;
            }
            e.Handled = true;
        }

        #endregion

    }

}
