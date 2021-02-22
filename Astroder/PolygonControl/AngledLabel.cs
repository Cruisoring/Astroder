using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class AngledLabel : AngledItem
    {
        #region Fields

        /// <summary>
        /// 	Gets or sets the text to drawPath.
        /// </summary>
        /// <value>
        /// 	The text to drawPath.
        /// </value>
        public string Label { get; set; }

        /// <summary>
        /// 	Gets or sets the <c>System.Drawing.Font</c> that defines the format of the text.
        /// </summary>
        /// <value>
        /// 	The <c>System.Drawing.Font</c> that defines the format of the text.
        /// </value>
        public Font LabelFont { get; set; }

        public Brush FontBrush { get; set; }

        /// <summary>
        /// 	Gets or sets the physicalAngle at which the text is rotated.
        /// </summary>
        /// <value>
        /// 	The physicalAngle at which the text is rotated.
        /// </value>
        //public float Rotation { get; set; }

        #endregion

        #region Constructors

        public AngledLabel(PolygonControl container, string label, Font font, Brush fontBrush, float distance, float angle, float rotation, Brush brush, Pen pen)
            : base(container, distance, angle, rotation, brush, pen)
        {
            Label = label;
            LabelFont = font;
            FontBrush = fontBrush;
        }

        public AngledLabel(PolygonControl container, string label, Font font, Brush fontBrush, float distance, float angle, float rotation)
            : this(container, label, font, fontBrush, distance, angle, rotation, null, null){}

        public AngledLabel(PolygonControl container, string label, Font font, Brush fontBrush, float distance, float angle)
            : this(container, label, font, fontBrush, distance, angle, 0, null, null){}

        public AngledLabel(PolygonControl container, Font font, float distance, float angle, float rotation)
            : this(container, angle.ToString(), font, DefaultBrush, distance, angle, rotation){}

        public AngledLabel(PolygonControl container, Font font, float distance, float angle)
            : this(container, angle.ToString(), font, DefaultBrush, distance, angle, 0) { }

        #endregion

        #region Functions

        //public override void Draw(Graphics g, ToControlPositionDelegate angleOf)
        //{
        //    if (!IsVisible) return;

        //    SizeF labelSize = g.MeasureString(Label, LabelFont);
        //    float shift = (float)(0.5 * Math.Max(Math.Abs(labelSize.Width * COS(Rotation)), Math.Abs(labelSize.Height * SIN(Rotation))));
        //    PointF displayCenter = angleOf(Distance, Angle, shift);

        //    RectangleF rect = new RectangleF(displayCenter.X - labelSize.Width / 2f, displayCenter.Y - labelSize.Height / 2f, labelSize.Width, labelSize.Height);

        //    if (Rotation == 0)
        //    {
        //        if (IsFilled)
        //            g.FillRectangle(TheBrush, rect);

        //        if (IsBordered)
        //            g.DrawRectangle(ThePen, new Rectangle((int)(rect.X), (int)rect.Y, (int)rect.Width, (int)rect.Height));

        //        g.DrawString(Label, LabelFont, FontBrush, rect);
        //    }
        //    else
        //    {
        //        GraphicsPath path = new GraphicsPath();

        //        if (IsFilled || IsBordered)
        //        {
        //            path.AddRectangle(rect);
        //            path.SetMarkers();
        //        }

        //        path.AddString(Label, LabelFont.FontFamily, (int)LabelFont.Style, 26, rect, StringFormat.GenericDefault);

        //        Matrix translateMatrix = new Matrix();
        //        float rotation = Container.IsXReversed ? Rotation : 180 + Rotation; 
        //        translateMatrix.RotateAt((Container.IsClockwise ? Angle : -Angle) + rotation, displayCenter);

        //        path.Transform(translateMatrix);

        //        if (!IsBordered && !IsFilled)
        //        {
        //            g.FillPath(FontBrush, path);
        //        }
        //        else
        //        {
        //            // Create a GraphicsPathIterator for handPath.
        //            GraphicsPathIterator pathIterator = new
        //                GraphicsPathIterator(path);

        //            // Rewind the iterator.
        //            pathIterator.Rewind();

        //            // Create the GraphicsPath section.
        //            GraphicsPath pathSection = new GraphicsPath();

        //            // to the screen.
        //            int subpathPoints = pathIterator.NextMarker(pathSection);

        //            drawPath(g, pathSection);

        //            subpathPoints = pathIterator.NextMarker(pathSection);

        //            g.FillPath(FontBrush, pathSection);
        //        }
        //    }

        //}

        public override void Draw(Graphics g, ToPhysicalAngleDelegate angleOf)
        {
            if (!IsVisible) return;

            SizeF labelSize = g.MeasureString(Label, LabelFont);

            //float shift = (float)(0.5 * Math.Max(Math.Abs(labelSize.Width * COS(Rotation)), Math.Abs(labelSize.Height * SIN(Rotation))));

            float actualDistance = Distance * Container.UnitSize;
            float actualAngle = angleOf(Angle);

            float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
            float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
            RectangleF rect = new RectangleF(x, y, labelSize.Width, labelSize.Height);

            if (Rotation == 0)
            {
                rect.X -= labelSize.Width / 2f;
                rect.Y -= labelSize.Height / 2f;
                if (IsFilled)
                    g.FillRectangle(TheBrush, rect);

                if (IsBordered)
                    g.DrawRectangle(ThePen, new Rectangle((int)(rect.X), (int)rect.Y, (int)rect.Width, (int)rect.Height));

                g.DrawString(Label, LabelFont, FontBrush, rect);
            }
            else
            {
                rect.X -= labelSize.Width / 2f;
                rect.Y -= labelSize.Height / 2f;

                //float cos = COS(actualAngle);
                //float sin = SIN(actualAngle);
                //rect.X -= (cos >= 0 ? cos : -cos) * labelSize.Width / 2f;
                //rect.Y -= (sin >= 0 ? sin : -sin) * labelSize.Height / 2f;

                float rotation = actualAngle - Rotation;

                GraphicsPath path = new GraphicsPath();

                if (IsFilled || IsBordered)
                {
                    path.AddRectangle(rect);
                    path.SetMarkers();
                }

                path.AddString(Label, LabelFont.FontFamily, (int)LabelFont.Style, LabelFont.Size, rect, StringFormat.GenericDefault);

                Matrix translateMatrix = new Matrix();

                translateMatrix.RotateAt(rotation, new PointF(x, y));

                path.Transform(translateMatrix);

                if (!IsBordered && !IsFilled)
                {
                    g.FillPath(FontBrush, path);
                }
                else
                {
                    // Create a GraphicsPathIterator for handPath.
                    GraphicsPathIterator pathIterator = new
                        GraphicsPathIterator(path);

                    // Rewind the iterator.
                    pathIterator.Rewind();

                    // Create the GraphicsPath section.
                    GraphicsPath pathSection = new GraphicsPath();

                    // to the screen.
                    int subpathPoints = pathIterator.NextMarker(pathSection);

                    drawPath(g, pathSection);

                    subpathPoints = pathIterator.NextMarker(pathSection);

                    g.FillPath(FontBrush, pathSection);
                }
            }
            
            
            /*/
            SizeF labelSize = g.MeasureString(Label, LabelFont);
            float shift = (float)(0.5 * Math.Max(Math.Abs(labelSize.Width * COS(Rotation)), Math.Abs(labelSize.Height * SIN(Rotation))));

            float actualDistance = Distance * Container.UnitSize + shift;
            float actualAngle = angleOf(Angle);
            float x = (float)Math.Round(actualDistance * COS(actualAngle), 4);
            float y = (float)Math.Round(actualDistance * SIN(actualAngle), 4);
            PointF displayCenter = new PointF(x, y);

            RectangleF rect = new RectangleF(displayCenter.X - labelSize.Width / 2f, displayCenter.Y - labelSize.Height / 2f, labelSize.Width, labelSize.Height);

            if (Rotation == 0)
            {
                if (IsFilled)
                    g.FillRectangle(TheBrush, rect);

                if (IsBordered)
                    g.DrawRectangle(ThePen, new Rectangle((int)(rect.X), (int)rect.Y, (int)rect.Width, (int)rect.Height));

                g.DrawString(Label, LabelFont, FontBrush, rect);
            }
            else
            {
                GraphicsPath path = new GraphicsPath();

                if (IsFilled || IsBordered)
                {
                    path.AddRectangle(rect);
                    path.SetMarkers();
                }

                path.AddString(Label, LabelFont.FontFamily, (int)LabelFont.Style, 26, rect, StringFormat.GenericDefault);

                Matrix translateMatrix = new Matrix();

                //float rotation = Container.IsXReversed ? Rotation : 180 + Rotation; 
                //translateMatrix.RotateAt((Container.IsClockwise ? Angle : -Angle) + rotation, displayCenter);
                float rotation = actualAngle + Rotation;
                translateMatrix.RotateAt(rotation, displayCenter);

                path.Transform(translateMatrix);

                if (!IsBordered && !IsFilled)
                {
                    g.FillPath(FontBrush, path);
                }
                else
                {
                    // Create a GraphicsPathIterator for handPath.
                    GraphicsPathIterator pathIterator = new
                        GraphicsPathIterator(path);

                    // Rewind the iterator.
                    pathIterator.Rewind();

                    // Create the GraphicsPath section.
                    GraphicsPath pathSection = new GraphicsPath();

                    // to the screen.
                    int subpathPoints = pathIterator.NextMarker(pathSection);

                    drawPath(g, pathSection);

                    subpathPoints = pathIterator.NextMarker(pathSection);

                    g.FillPath(FontBrush, pathSection);
                }
            }
            //*/

        }


        //public override void Draw(Graphics g, LogicalAngleConverterDelegate angleConverter, float unitSize, float angleOffset)
        //{
        //    if (!IsVisible)
        //        return;
        //    else if (Rotation == 0)
        //    {
        //        float angleLow = Normalized(Angle + angleOffset);
        //        float cos = COS(angleLow);
        //        float sin = SIN(angleLow);
        //        SizeF width = g.MeasureString(Label, LabelFont);
        //        float actualDistance = unitSize * Distance + width.Width * Math.Abs(cos) / 2;

        //        float x = actualDistance * cos - width.Width / 2;
        //        float y = actualDistance * sin - (1 - sin) * width.Height / 2;

        //        PointF pos = new PointF(x, y);

        //        if (IsFilled && TheBrush != null)
        //            g.FillRectangle(TheBrush, x, y, width.Width, width.Height);

        //        if (IsBordered && ThePen != null)
        //            g.DrawRectangle(ThePen, x, y, width.Width, width.Height);

        //        g.DrawString(Label, LabelFont, FontBrush, pos);

        //    }
        //    else
        //    {
        //        GraphicsPath handPath = new GraphicsPath();
        //        Matrix translateMatrix = new Matrix();

        //        float angleLow = Normalized(Angle + angleOffset);
        //        float cos = COS(angleLow);
        //        float sin = SIN(angleLow);
        //        SizeF width = g.MeasureString(Label, LabelFont);
        //        float shift = Math.Max(Math.Abs(width.Width * COS(Rotation)), Math.Abs(width.Height * SIN(Rotation)));
        //        float actualDistance = unitSize * Distance + 0.5f * shift;

        //        shift = Math.Max(Math.Abs(width.Width * SIN(Rotation)), Math.Abs(width.Height * COS(Rotation)));
        //        shift = (float)Math.Asin(0.5f * shift / actualDistance) * DegreesPerRadian;

        //        float x = actualDistance * COS(angleLow - shift);
        //        float y = actualDistance * SIN(angleLow - shift);

        //        PointF pos = new PointF(x, y);

        //        float displayAngle = Normalized(Angle + Rotation);

        //        translateMatrix.RotateAt(Normalized(displayAngle), pos);
        //        //translateMatrix.RotateAt(Rotation, pos);

        //        if (IsFilled || IsBordered)
        //        {
        //            handPath.AddRectangle(new RectangleF(x, y, width.Width, width.Height));
        //            handPath.SetMarkers();
        //        }

        //        handPath.AddString(Label, LabelFont.FontFamily, (int)LabelFont.SizeInPoints, LabelFont.SizeInPoints, pos, StringFormat.GenericDefault);

        //        handPath.Transform(translateMatrix);

        //        if (!IsBordered && !IsFilled)
        //            g.FillPath(FontBrush, handPath);
        //        else
        //        {
        //            // Create a GraphicsPathIterator for handPath.
        //            GraphicsPathIterator pathIterator = new
        //                GraphicsPathIterator(handPath);

        //            // Rewind the iterator.
        //            pathIterator.Rewind();

        //            // Create the GraphicsPath section.
        //            GraphicsPath pathSection = new GraphicsPath();

        //            // to the screen.
        //            int subpathPoints = pathIterator.NextMarker(pathSection);

        //            if (IsFilled)
        //                g.FillPath(TheBrush, pathSection);

        //            if (IsBordered)
        //                g.DrawPath(ThePen, pathSection);

        //            subpathPoints = pathIterator.NextMarker(pathSection);

        //            g.FillPath(FontBrush, pathSection);
        //        }

        //    }
        //}

        #endregion


    }
}
