using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataImporter;
using System.Drawing.Drawing2D;
using System.Drawing;
using EphemerisCalculator;

namespace PolygonControl
{
    public class PivotWrapper : AngledPath
    {
        #region constants

        public static float DefaultSymbolSize = 0.25f;

        public static double AsIs(double original) { return original; }

        public static Font HintFont = new Font("AstroSymbols", 12f, FontStyle.Bold);

        public static bool NeglectMoon = true;

        #endregion

        #region Property

        public OutlineItem Pivot { get; set; }

        public IndexedPosition Position { get; set; }

        public Pen LeadingPen {get; set; }

        public bool IsHighlighted { get; set; }

        public bool DisplayHint { get; set; }

        public string Hint { get; set; }

        public PriceToIndexValueDelegate ToIndex
        {
            get
            {
                return Container.Adapter == null ? AsIs : Container.Adapter.IndexOf;
            }
        }

        #endregion

        #region Constructor

        public PivotWrapper(PolygonControl container, GraphicsPath path, Brush brush, Pen pen, Pen leadingPen, OutlineItem pivot)
            : base(container, path, 0, 0, 0, false, brush, pen)
        {
            Pivot = pivot;
            double index = ToIndex(pivot.Price);
            Position = container.Calculator.IndexedPositionOf(index);
            Distance = Position.Radius;
            Angle = Position.Angle;
            LeadingPen = leadingPen;
            DisplayHint = false;
            Hint = "";
        }

        #endregion

        #region Functions

        public override void Draw(Graphics g, ToPhysicalAngleDelegate angleOf)
        {
            if (!IsVisible)  return;

            //if (angleOf == null)
            //    angleOf = Container.PhysicalAngleOf;

            GraphicsPath path = Multiply(Path, Container.UnitSize * DefaultSymbolSize);

            if (Distance != 0 || Angle != 0)
            {
                float actualDistance = Distance * Container.UnitSize;
                float actualAngle = angleOf(Angle);
                float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
                float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
                PointF pos = new PointF(x, y);

                Matrix translateMatrix = new Matrix();
                translateMatrix.Translate(pos.X, pos.Y);
                path.Transform(translateMatrix);
                translateMatrix.Dispose();
            }

            if (IsHighlighted)
                HighLight(g, path);
            else
                drawWithLeadingLine(g, path);

            if (DisplayHint)
            {
                drawHint(g);
            }

            path.Dispose();
        }

        public void ReCalculate()
        {
            double index  = ToIndex(Pivot.Price);
            Position = Container.Calculator.IndexedPositionOf(index);
            Distance = Position.Radius;
            Angle = Position.Angle;
        }

        private void drawWithLeadingLine(Graphics g, GraphicsPath path)
        {
            drawPath(g, path);
            PointF start = Container.RelativeToCenter(3, Angle, 0);
            PointF end = Container.RelativeToCenter(Container.Radius, Angle, 0);
            g.DrawLine(LeadingPen, start, end);

        }

        private void drawHint(Graphics g)
        {
            if (Hint == null || Hint == "") return;

            SizeF size = g.MeasureString(Hint, HintFont);
            PointF pos = Container.RelativeToCenter(Distance, Angle, 0);

            pos = new PointF(pos.X - size.Width/2, pos.Y - size.Height  - DefaultSymbolSize * Container.UnitSize);
            g.DrawString(Hint, HintFont, TheBrush, pos);
        }

        private void HighLight(Graphics g, GraphicsPath path)
        {
            //g.FillPath(Container.PivotsHolder.HightlightBrush, path);
            //g.DrawPath(Container.PivotsHolder.HightlightBorderPen, path);
            //PointF end = Container.RelativeToCenter(Container.Radius, Angle, 0);
            //g.DrawLine(Container.PivotsHolder.HighlightLeadingPen, new PointF(0, 0), end);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} on {2}: translated as {3} on Round {4}, Angle={5}, Radius={6}\r\n",
                Pivot.Type, Pivot.Price, Pivot.Date.ToString("yyyyMMdd"), Position.IndexValue, Position.Round, Position.Angle, Position.Radius);

            MatchRules during = new MatchRules(Pivot.Date, Ephemeris.DefaultSearchMode);
            List<Relation> relations = Ephemeris.RelationsWithin(during);

            foreach (Relation rel in relations)
            {
                if (NeglectMoon &&rel.Inferior == PlanetId.SE_MOON || rel.Superior == PlanetId.SE_MOON)
                    continue;

                sb.AppendFormat(", {0}{1}{2}(Orb={3})",
                    Planet.SymbolOf(rel.Superior), rel.Aspect.Symbol, Planet.SymbolOf(rel.Inferior), rel.Orb.ToString("F3"));
            }

            return sb.ToString();
        }

        #endregion

    }

    public class PivotDifference
    {
        public OutlineChanges Changes { get; private set; }

        public IndexedPosition Rotated { get; private set; }

        public PivotDifference(PivotWrapper current, PivotWrapper reference)
        {
            Rotated = current.Position - reference.Position;
            Changes = current.Pivot - reference.Pivot;
        }

        public override string ToString()
        {
            return Changes.ToString() + "\t" + Rotated.ToString();
        }
    }
}
