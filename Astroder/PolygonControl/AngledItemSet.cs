using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PolygonControl
{
    public class AngledItemSet : GraphicItemSet<AngledItem>
    {
        #region Properties

        public float AngleOffset { get; set; }

        #endregion

        #region Constructors

        public AngledItemSet(PolygonControl container)
            : base(container)
        {
            AngleOffset = 0f;
            //Mapper = new ToControlPositionDelegate(ToControlPosition);
            Mapper = new ToPhysicalAngleDelegate(toContainerAngle);
            //Container.UnitSizeChanged += new PolygonControl.UnitSizeChangedDelegate(Container_UnitSizeChanged);
        }

        #endregion

        #region Functions

        //void Container_UnitSizeChanged(float oldSize, float newSize)
        //{
        //    float sizeFactor = newSize / oldSize;

        //    foreach (AngledItem item in Items)
        //    {
        //        item.Resize(sizeFactor);
        //    }
        //}

        public float toContainerAngle(float angle)
        {
            angle += AngleOffset;
            return Container.PhysicalAngleOf(angle);
        }

        //public PointF ToControlPosition(float distanceLow, float angleLow, float ptShift)
        //{
        //    angleLow += AngleOffset;
        //    return Container.RelativeToCenter(distanceLow, angleLow, ptShift);
        //}

        public CircledPathes AddLines(float distance, float angleOffset, float length, float step, Pen pen, AngleFilterDelegate filter, bool isFixed)
        {
            if (!Toolkit.Contains(pen) && pen != null)
                Toolkit.Add(pen);

            int repetition = (int)(360 / step);
            //path.AddLine(-0.5f * length * Container.UnitSize, 0, 0.5f * length * Container.UnitSize, 0);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(-0.5f * length, 0, 0.5f * length, 0);
            CircledPathes lines = new CircledPathes(Container, path, distance, angleOffset, 0, repetition, filter, isFixed, null, pen);
            Add(lines);
            return lines;
        }

        public CircledPathes AddLines(float distance, float length, float step, Pen pen)
        {
            return AddLines(distance, 0, length, step, pen, null, false);
        }

        public CircledPathes AddPathSet(GraphicsPath path, float distance, float angleOffset, int repetition, bool isFixed, Brush brush, Pen pen)
        {
            if (path == null)
                throw new ArgumentNullException();

            if (!Toolkit.Contains(pen) && pen != null)
                Toolkit.Add(pen);

            if (!Toolkit.Contains(brush) && brush != null)
                Toolkit.Add(brush);

            CircledPathes pathSet = new CircledPathes(Container, path, distance, 0, 0,  repetition, null, isFixed, brush, pen);

            Add(pathSet);

            return pathSet;
        }

        public AngledPath AddPath(GraphicsPath path, float distance, float angle, float rotation, bool isFixed, Brush brush, Pen pen)
        {
            if (path == null)
                throw new ArgumentNullException();

            if (!Toolkit.Contains(pen) && pen != null)
                Toolkit.Add(pen);

            if (!Toolkit.Contains(brush) && brush != null)
                Toolkit.Add(brush);

            AngledPath pathItem = new AngledPath(Container, path, distance, angle, rotation, isFixed, brush, pen);

            Add(pathItem);

            return pathItem;
        }


        public List<AngledLabel> AddLables(Font font, Brush fontBrush, float distance, float angleOffset, float rotation, int repetition, Brush brush, Pen pen)
        {
            if (font == null || fontBrush == null)
                throw new ArgumentNullException();

            if (!Toolkit.Contains(font))
                Toolkit.Add(font);

            if (!Toolkit.Contains(fontBrush))
                Toolkit.Add(fontBrush);

            if (!Toolkit.Contains(pen) && pen != null)
                Toolkit.Add(pen);

            if (!Toolkit.Contains(brush) && brush != null)
                Toolkit.Add(brush);

            float angle;
            float step = 360f / repetition;

            List<AngledLabel> labels = new List<AngledLabel>();

            for (int i = 0; i < repetition; i++)
            {
                angle = i * step + angleOffset;

                AngledLabel label = new AngledLabel(Container, angle.ToString(), font, fontBrush, distance, angle, rotation, brush, pen);
                Add(label);
                labels.Add(label);
            }
            return labels;
        }

        public List<AngledLabel> AddLables(List<String> lables, Font font, Brush fontBrush, float distance, float offset, float rotation, Brush brush, Pen pen)
        {
            if (font == null || fontBrush == null)
                throw new ArgumentNullException();

            if (!Toolkit.Contains(font))
                Toolkit.Add(font);

            if (!Toolkit.Contains(fontBrush))
                Toolkit.Add(fontBrush);

            if (pen != null && !Toolkit.Contains(pen))
                Toolkit.Add(pen);

            if (brush != null && !Toolkit.Contains(brush))
                Toolkit.Add(brush);

            int repetition = lables.Count;
            float step = ( 360 / repetition);
            float angle;
            List<AngledLabel> labels = new List<AngledLabel>();

            for (int i = 0; i < repetition; i++)
            {
                angle = i * step + offset;

                AngledLabel label = new AngledLabel(Container, lables[i], font, fontBrush, distance, angle, rotation, brush, pen);
                Add(label);
                labels.Add(label);
            }
            return labels;
        }

        //public void SetHands(float startDistance, float endDistance, int times, float orb, float offset, Color color1, Color color2)
        //{
        //    color1 = Color.FromArgb(0, color1);
        //    color2 = Color.FromArgb(128, color2);
        //    float angleLow = 0;
        //    int UnitSize = Container.UnitSize;

        //    if (startDistance != 0f)
        //    {
        //        LinearGradientBrush brush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, startDistance * UnitSize), color2, color1);
        //        Pen pen = new Pen(brush, 1.2f);
        //        brush.Dispose();
        //        pen.DashStyle = DashStyle.Dash;
        //        GraphicsPath handPath = new GraphicsPath();
        //        handPath.AddRadiateLine(0, 0, 0, startDistance);

        //        for (int i = 0; i < times; i++)
        //        {
        //            angleLow = Container.PhysicalAngleOf(offset + i * 360f / times);
        //            AddPath(handPath, startDistance, angleLow, 0, null, pen);
        //        }
        //    }

        //    float sin = PolygonControl.SIN(orb);
        //    float cos = PolygonControl.COS(orb);
        //    GraphicsPath path2 = new GraphicsPath();
        //    path2.AddArc(new RectangleF(-endDistance*UnitSize, -endDistance*UnitSize, 2 * endDistance*UnitSize, 2 * endDistance*UnitSize), 
        //        offset - orb, offset + orb);
        //    path2.AddRadiateLine(endDistance * sin*UnitSize, endDistance * cos*UnitSize, startDistance * sin*UnitSize, startDistance * cos*UnitSize);
        //    path2.AddArc(new RectangleF(-startDistance*UnitSize, -startDistance*UnitSize, 2 * startDistance*UnitSize, 2 * startDistance*UnitSize), 
        //        offset + orb, offset - orb);
        //    path2.AddRadiateLine(startDistance * sin*UnitSize, startDistance * cos*UnitSize, endDistance * sin*UnitSize, endDistance * cos*UnitSize);

        //    LinearGradientBrush brush2 = new LinearGradientBrush(new PointF(0, startDistance * UnitSize), new PointF(0, endDistance * UnitSize), color1, color2);
        //    Pen pen2 = new Pen(brush2);

        //    for (int i = 0; i < times; i++)
        //    {
        //        angleLow = Container.PhysicalAngleOf(offset + i * 360f / times);
        //        AddPath(path2, startDistance, angleLow, 0, brush2, pen2);
        //    }

        //}


        public virtual void Draw(Graphics g)
        {
            foreach (AngledItem item in Items)
            {
                item.Draw(g, Mapper);
            }
        }

        public virtual void Draw(Graphics g, ToPhysicalAngleDelegate converter)
        {
            foreach (AngledItem item in Items)
            {
                item.Draw(g, converter);
            }
        }

        //public virtual void Draw(Graphics g, ToControlPositionDelegate converter)
        //{
        //    foreach (AngledItem item in Items)
        //    {
        //        item.Draw(g, converter);
        //    }
        //}

        //public void Resize(float sizeFactor)
        //{
        //    foreach (AngledItem item in Items)
        //    {
        //        item.Resize(sizeFactor);
        //    }
        //}

        #endregion

    }

    //public class AngledItemSet : GraphicItemSet<RadiateGraphicItem>, IDisposable
    //{
    //    #region Properties

    //    public bool IsClockwise { get; set; }

    //    public float AngleMin { get; set; }

    //    public float AngleMax { get; set; }

    //    public float AngleOffset { get; set; }

    //    public bool IsAngleWrapped { get; set; }

    //    public List<Brush> BrushList { get; set; }

    //    public List<Pen> PenList { get; set; }

    //    public List<Font> FontList { get; set; }

    //    public List<GraphicsPath> PathList { get; set; }

    //    #endregion

    //    #region Constructors

    //    public AngledItemSet(PolygonControl container)
    //        : base(container)
    //    {
    //        IsClockwise = false;
    //        AngleMin = 0f;
    //        AngleMax = 360f;
    //        AngleOffset = 0f;
    //        IsAngleWrapped = true;
    //        BrushList = new List<Brush>();
    //        PenList = new List<Pen>();
    //        FontList = new List<Font>();
    //        PathList = new List<GraphicsPath>();
    //    }

    //    public AngledItemSet(PolygonControl container, 
    //        float minAngle, float maxAngle, float angleOffset, bool isWrapped)
    //        : base(container)
    //    {
    //        IsAngleWrapped = isWrapped;
    //        IsClockwise = isClockwise;
    //        AngleMin = minAngle;
    //        AngleMax = maxAngle;
    //        AngleOffset = angleOffset;
    //        BrushList = new List<Brush>();
    //        PenList = new List<Pen>();
    //        FontList = new List<Font>();
    //        PathList = new List<GraphicsPath>();
    //    }

    //    #endregion

    //    #region Functions

    //    //public float ToLogicalAngle(float physicalAngle)
    //    //{            
    //    //    float angleLow = IsClockwise ? (physicalAngle - AngleOffset) : (AngleOffset - physicalAngle);

    //    //    float range = AngleMax - AngleMin;

    //    //    angleLow %= range;

    //    //    if (angleLow < 0)
    //    //        angleLow += range;

    //    //    return angleLow + AngleMin;
    //    //}

    //    //public float PhysicalAngleOf(float angleLow)
    //    //{
    //    //    float angleLow = AngleOffset + (IsClockwise ? angleLow : -angleLow);

    //    //    float range = AngleMax - AngleMin;

    //    //    angleLow %= range;

    //    //    if (angleLow < 0)
    //    //        angleLow += range;

    //    //    return angleLow + AngleMin;
    //    //}

    //    public override void Add(RadiateGraphicItem speedQuery)
    //    {
    //        Items.Add(speedQuery);
    //    }

    //    public void AddRadiateLine(float distanceLow, float angleLow, float length, Pen pen)
    //    {
    //        CircleTickItem tick = new CircleTickItem(distanceLow, angleLow, length, pen);
    //        Add(tick);
    //    }

    //    public void AddTicks(float distanceLow, float length, Pen pen, float step)
    //    {
    //        int repetition = (int)((AngleMax - AngleMin) / step);
    //        float angleLow;

    //        if (pen != null && !PenList.Contains(pen))
    //            PenList.Add(pen);

    //        for (int i = 0; i < repetition; i ++ )
    //        {
    //            angleLow = Container.PhysicalAngleOf(AngleMin + i * step);
    //            AddRadiateLine(distanceLow, angleLow, length, pen);
    //        }
    //    }

    //    public void AddTicks(float distanceLow, float length, Pen pen, float step, int bypassed)
    //    {
    //        int repetition = (int)((AngleMax - AngleMin) / step);
    //        float angleLow;

    //        if (pen != null && !PenList.Contains(pen))
    //            PenList.Add(pen);

    //        for (int i = 0; i < repetition; i ++ )
    //        {
    //            if ((i * step) % bypassed == 0)
    //                continue;

    //            angleLow = Container.PhysicalAngleOf(AngleMin + i * step);
    //            CircleTickItem tick = new CircleTickItem(distanceLow, angleLow, length, pen);
    //            Items.Add(tick);
    //        }
    //    }

    //    //public RadiateGraphicsPathItem(GraphicsPath handPath, float distanceLow, float physicalAngle, float rotation, Brush brush, Pen pen)

    //    public void AddPath(GraphicsPath handPath, float distanceLow, float angleLow, float rotation, Brush brush, Pen pen)
    //    {
    //        if (pen != null && !PenList.Contains(pen))
    //            PenList.Add(pen);

    //        if (brush != null && !BrushList.Contains(brush) && brush != null)
    //            BrushList.Add(brush);

    //        if (handPath != null && !PathList.Contains(handPath))
    //            PathList.Add(handPath);

    //        Add(new RadiateGraphicsPathItem(handPath, distanceLow, angleLow, rotation, brush, pen));
    //    }

    //    //public void AddPaths(GraphicsPath handPath, float startDistance, float endDistance, float offset, float step, float rotation, Color color1, Color color2)
    //    //{
    //    //    if (handPath != null && !PathList.Contains(handPath))
    //    //        PathList.Add(handPath);

    //    //    int repetition = (int)((AngleMax - AngleMin) / step);
    //    //    float angleLow;

    //    //    int UnitSize = Container.UnitSize;
    //    //    LinearGradientBrush brush = new LinearGradientBrush(new PointF(0, startDistance * UnitSize), new PointF(0, endDistance * UnitSize), color1, color2);
    //    //    Pen pen = new Pen(color2);
    //    //    pen.DashStyle = DashStyle.Dash;

    //    //    if (pen != null && !PenList.Contains(pen))
    //    //        PenList.Add(pen);

    //    //    if (brush != null && !BrushList.Contains(brush) && brush != null)
    //    //        BrushList.Add(brush);
            
    //    //    for (int i = 0; i < repetition; i++)
    //    //    {
    //    //        angleLow = PhysicalAngleOf(AngleMin + i * step);
    //    //        CircleTickItem tick = new CircleTickItem(distanceLow, angleLow, length, pen);
    //    //        Items.Add(tick);
    //    //    }

    //    //}

    //    //public void SetHands(float startDistance, float endDistance, int times, float orb, float offset, Color color1, Color color2)
    //    //{
    //    //    color1 = Color.FromArgb(0, color1);
    //    //    color2 = Color.FromArgb(128, color2);
    //    //    float angleLow = 0;
    //    //    int UnitSize = Container.UnitSize;

    //    //    if (startDistance != 0f)
    //    //    {
    //    //        LinearGradientBrush brush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, startDistance * UnitSize), color2, color1);
    //    //        Pen pen = new Pen(brush, 1.2f);
    //    //        brush.Dispose();
    //    //        pen.DashStyle = DashStyle.Dash;
    //    //        GraphicsPath handPath = new GraphicsPath();
    //    //        handPath.AddRadiateLine(0, 0, 0, startDistance);

    //    //        for (int i = 0; i < times; i++)
    //    //        {
    //    //            angleLow = Container.PhysicalAngleOf(offset + i * 360f / times);
    //    //            AddPath(handPath, startDistance, angleLow, 0, null, pen);
    //    //        }
    //    //    }

    //    //    float sin = PolygonControl.SIN(orb);
    //    //    float cos = PolygonControl.COS(orb);
    //    //    GraphicsPath path2 = new GraphicsPath();
    //    //    path2.AddArc(new RectangleF(-endDistance*UnitSize, -endDistance*UnitSize, 2 * endDistance*UnitSize, 2 * endDistance*UnitSize), 
    //    //        offset - orb, offset + orb);
    //    //    path2.AddRadiateLine(endDistance * sin*UnitSize, endDistance * cos*UnitSize, startDistance * sin*UnitSize, startDistance * cos*UnitSize);
    //    //    path2.AddArc(new RectangleF(-startDistance*UnitSize, -startDistance*UnitSize, 2 * startDistance*UnitSize, 2 * startDistance*UnitSize), 
    //    //        offset + orb, offset - orb);
    //    //    path2.AddRadiateLine(startDistance * sin*UnitSize, startDistance * cos*UnitSize, endDistance * sin*UnitSize, endDistance * cos*UnitSize);

    //    //    LinearGradientBrush brush2 = new LinearGradientBrush(new PointF(0, startDistance * UnitSize), new PointF(0, endDistance * UnitSize), color1, color2);
    //    //    Pen pen2 = new Pen(brush2);

    //    //    for (int i = 0; i < times; i++)
    //    //    {
    //    //        angleLow = Container.PhysicalAngleOf(offset + i * 360f / times);
    //    //        AddPath(path2, startDistance, angleLow, 0, brush2, pen2);
    //    //    }

    //    //}

    //    public void AddLables(Font font, Brush fontBrush, float distanceLow, float step, Brush brush, Pen pen)
    //    {
    //        int repetition = (int)((AngleMax - AngleMin) / step);
    //        float angleLow, physicalAngle;

    //        if (!FontList.Contains(font))
    //            FontList.Add(font);

    //        if (pen != null && !PenList.Contains(pen))
    //            PenList.Add(pen);

    //        if (fontBrush != null && !BrushList.Contains(fontBrush) && fontBrush != null)
    //            BrushList.Add(fontBrush);

    //        if (brush != null && !BrushList.Contains(brush) && brush != null)
    //            BrushList.Add(brush);

    //        for (int i = 0; i < repetition; i++)
    //        {
    //            angleLow = AngleMin + i * step;
    //            physicalAngle = Container.PhysicalAngleOf(angleLow);

    //            CircleLabelItem label = new CircleLabelItem(angleLow.ToString(), font, fontBrush, distanceLow, physicalAngle, 0, brush);
    //            Add(label);
    //        }

    //    }

    //    public void AddLables(List<String> lables, Font font, Brush fontBrush, float distanceLow, float offset, float rotation, Brush brush, Pen pen)
    //    {
    //        int repetition = lables.Count;
    //        float step = ((AngleMax - AngleMin) / repetition);
    //        float angleLow, physicalAngle;

    //        if (!FontList.Contains(font))
    //            FontList.Add(font);

    //        if (!PenList.Contains(pen))
    //            PenList.Add(pen);

    //        if (!BrushList.Contains(fontBrush) && fontBrush != null)
    //            BrushList.Add(fontBrush);

    //        if (!BrushList.Contains(brush) && brush != null)
    //            BrushList.Add(brush);

    //        for (int i = 0; i < repetition; i++)
    //        {
    //            angleLow = AngleMin + i * step + offset;
    //            physicalAngle = Container.PhysicalAngleOf(angleLow);

    //            CircleLabelItem label = new CircleLabelItem(lables[i], font, fontBrush, distanceLow, physicalAngle, rotation, brush);
    //            Add(label);
    //        }

    //    }

    //    public override void Clear()
    //    {
    //        Dispose();
    //    }

    //    public override void Dispose()
    //    {
    //        Items.Clear();
    //        foreach (Font font in FontList)
    //        {
    //            font.Dispose();
    //        }
    //        foreach (Pen pen in PenList)
    //        {
    //            if (pen != null )
    //                pen.Dispose();
    //        }
    //        foreach(Brush brush in BrushList)
    //        {
    //            if (brush != null)
    //                brush.Dispose();
    //        }

    //        foreach (GraphicsPath handPath in PathList)
    //        {
    //            if (handPath != null)
    //                handPath.Dispose();
    //        }
    //    }

    //    public override void Draw(Graphics g)
    //    {
    //        foreach (RadiateGraphicItem pivotItem in Items)
    //        {
    //            pivotItem.Draw(g, Container.UnitSize, AngleOffset);
    //        }
    //    }

    //    #endregion

    //}
}
