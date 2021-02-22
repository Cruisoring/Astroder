using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using System.Drawing;

namespace PolygonControl
{
    partial class PolygonControl
    {
        public class PolygonCalculator
        {
            public static int DefaultMaxCycle = 15;

            #region Fields

            public Polygon Shape { get; private set; }

            public int MinCycle { get; private set; }

            private int maxCycle = 15;
            public int MaxCycle 
            {
                get { return maxCycle; }
                set 
                {
                    if (maxCycle != value)
                        maxCycle = value;

                    if (VerticesPositions != null && !VerticesPositions.ContainsKey(value))
                        VerticesPositions = getVertices();
                }
            }

            public Dictionary<int, Dictionary<int, PointF>> VerticesPositions { get; private set; }

            public float Radius 
            {
                get
                {
                    float maxDist; 
                    
                    if (Shape.IsPolygon)
                    {
                        maxDist = (float)(from pt in VerticesPositions[MaxCycle].Values
                                          select Math.Sqrt(((0.3f + pt.X) * (0.3f + pt.X)) + ((0.3f + pt.Y) * (0.3f + pt.Y)))).Max();
                    }
                    else
                    {
                        Double tan = Math.Tan(Math.PI / Shape.Edges);
                        maxDist = (float)(0.5 / tan + 360/Shape.Edges);
                    }

                    return maxDist;
                }
            }

            public int Edges { get { return Shape.Edges; } }

            #endregion

            #region Constructors
            public PolygonCalculator(PolygonControl container, Polygon polygon) : this(container, polygon, DefaultMaxCycle){}

            public PolygonCalculator(PolygonControl container, Polygon polygon, int cycles)
            {
                Shape = polygon;
                MaxCycle = Shape.IsPolygon ? cycles : 360/Shape.Edges;
                MinCycle = Shape.IsEven ? 1 : 0;
                VerticesPositions = getVertices();
            }

            #endregion

            #region Functions

            private Dictionary<int, Dictionary<int, PointF>> getVertices()
            {
                Dictionary<int, Dictionary<int, PointF>> result = new Dictionary<int, Dictionary<int, PointF>>();

                if (MinCycle == 0)
                    result.Add(0, new Dictionary<int,PointF>{ {1, new PointF(0, 0)} });

                //Double bevelLength, edgeLength, x, y, sinValue = Math.Sin(Math.PI/Shape.Edges);

                for (int round = 1; round <= 30; round ++ )
                {
                    result.Add(round, getVerticeOfRound(round));
                }

                return result;
            }

            private Dictionary<int, PointF> getVerticeOfRound(int round)
            {
                Dictionary<int, PointF> vertices = new Dictionary<int, PointF>();

                if (Shape.IsPolygon)
                {
                    Double sinValue = Math.Sin(Math.PI / Shape.Edges);
                    Double bevelLength = Shape.LengthOfRound(round) / (2 * Shape.Edges * sinValue);
                    double x, y;

                    for (int i = 0; i < Shape.Edges; i++)
                    {
                        Angle angle = Shape.Vertices[i];
                        int index = (int)Shape.IndexOf(round, angle);
                        x = bevelLength * Math.Cos(angle.Radians);
                        y = bevelLength * Math.Sin(angle.Radians);
                        vertices.Add(index, new PointF((float)x, (float)y));
                    }
                }
                else
                {
                    Double tan = Math.Tan(Math.PI / Shape.Edges);
                    Double radius = 0.5 / tan + round - 1;
                    double x, y;

                    for (int i = 1; i <= Shape.Edges; i++)
                    {
                        Angle angle = Shape.Vertices[i-1];
                        //int index = (int)Shape.IndexOf(round, angleLow);
                        int index = (round - 1) * Shape.Edges + i;
                        x = radius * Math.Cos(angle.Radians);
                        y = radius * Math.Sin(angle.Radians);
                        vertices.Add(index, new PointF((float)x, (float)y));
                    }
                }

                return vertices;
            }

            public PointF PositionOfIndex(int index)
            {
                if (Shape.IsPolygon)
                {
                    int round = Shape.RoundOf(index);

                    if (!VerticesPositions.ContainsKey(round))
                        VerticesPositions.Add(round, getVerticeOfRound(round));

                    if (VerticesPositions[round].ContainsKey(index))
                        return VerticesPositions[round][index];
                    else
                    {
                        Double edgeLength = Shape.LengthOfRound(round) / Shape.Edges;

                        int previousVertex = (from vIndex in VerticesPositions[round].Keys
                                              where index > vIndex
                                              select vIndex).LastOrDefault();

                        int nextVertex = (from vIndex in VerticesPositions[round].Keys
                                          where index < vIndex
                                          select vIndex).Min();

                        if (nextVertex == 0)
                            throw new Exception();

                        PointF prev, next;
                        if (previousVertex == 0)
                        {
                            previousVertex = (from vIndex in VerticesPositions[round].Keys
                                              select vIndex).Last();
                            prev = VerticesPositions[round][previousVertex];
                            next = VerticesPositions[round][nextVertex];

                        }
                        else
                        {
                            prev = VerticesPositions[round][previousVertex];
                            next = VerticesPositions[round][nextVertex];
                        }
                        double x, y;

                        x = next.X - (next.X - prev.X) * (nextVertex - index) / edgeLength;
                        y = next.Y - (next.Y - prev.Y) * (nextVertex - index) / edgeLength;

                        return new PointF((float)x, (float)y);
                    }
                }
                else
                {
                    Double tan, radius, x, y;
                    int round;

                    if (index > 360 && index != 361)
                        index %= 360;

                    round = Shape.RoundOf(index);

                    tan = Math.Tan(Math.PI / Shape.Edges);
                    radius = 0.5 / tan + round - 1;
                    Angle angle;

                    if (round == 0)
                    {
                        angle = Shape.Vertices[Shape.Edges-1];
                    }
                    else
                    {
                        angle = Shape.Vertices[(index + Shape.Edges - 1 ) % Shape.Edges];
                    }
                    x = radius * Math.Cos(angle.Radians);
                    y = radius * Math.Sin(angle.Radians);
                    return new PointF((float)x, (float)y);
                }
            }

            public PointF PositionOfIndex(Double indexValue)
            {
                if (Shape.IsPolygon)
                {
                    int floor = (int)Math.Floor(indexValue);
                    int ceiling = (int)Math.Ceiling(indexValue);
                    PointF prev = PositionOfIndex(floor);
                    PointF next = PositionOfIndex(ceiling);

                    double x, y;

                    x = next.X - (next.X - prev.X) * (ceiling - indexValue);
                    y = next.Y - (next.Y - prev.Y) * (ceiling - indexValue);

                    return new PointF((float)x, (float)y);
                }
                else
                {
                    indexValue = (indexValue <= 360.5 ? indexValue : Math.Round(indexValue % 360, 2));
                    int ceiling = (int)Math.Ceiling(indexValue);
                    double x, y;

                    if (ceiling % Shape.Edges == 1)
                    {
                        if (indexValue == ceiling - 0.5)
                        {
                            int round = Shape.RoundOf(ceiling);

                            Double tan = Math.Tan(Math.PI / Shape.Edges);
                            Double radius = 0.5 / tan + round - 1.5;

                            return new PointF((float)radius, 0);
                        }

                        int floor = (int)Math.Floor(indexValue);

                        PointF prev = PositionOfIndex(floor);
                        PointF next = PositionOfIndex(ceiling);

                        x = next.X - (next.X - prev.X) * (ceiling - indexValue);
                        y = next.Y - (next.Y - prev.Y) * (ceiling - indexValue);

                        return new PointF((float)x, (float)y);
                    }
                    else
                    {
                        Angle ceilingAngle = Shape.Vertices[(ceiling + Shape.Edges - 1) % Shape.Edges];

                        int round = Shape.RoundOf(indexValue);

                        Double tan = Math.Tan(Math.PI / Shape.Edges);
                        Double radius = 0.5 / tan + round - 1;

                        int first = Shape.FirstOfRound(round);

                        float degree = (float)(ceilingAngle.Degrees - (ceiling - indexValue) * 360.0 / Edges);

                        x = radius * COS(degree);
                        y = radius * SIN(degree);
                        return new PointF((float)x, (float)y);
                    }
                }
            }

            public IndexedPosition IndexedPositionOf(Double indexValue)
            {
                int round = Shape.RoundOf(indexValue);

                PointF pos = PositionOfIndex(indexValue);

                float x = pos.X;
                float y = pos.Y;
                float radius = (float)Math.Sqrt(x * x + y * y);

                float refAngel = (float)Math.Atan(y / x) * degreesPerRadian;

                float angle = (x >= 0) ? refAngel : 180 + refAngel;

                if (angle < 0)
                    angle += 360;

                return new IndexedPosition(indexValue, radius, angle, round);
            }

            public void AngleDistanceOf(Double indexValue, out float angle, out float distance)
            {
                double x, y;

                if (Shape.IsPolygon)
                {
                    int floor = (int)Math.Floor(indexValue);
                    int ceiling = (int)Math.Ceiling(indexValue);
                    PointF prev = PositionOfIndex(floor);
                    PointF next = PositionOfIndex(ceiling);

                    x = next.X - (next.X - prev.X) * (ceiling - indexValue);
                    y = next.Y - (next.Y - prev.Y) * (ceiling - indexValue);
                }
                else
                {
                    PointF pos = PositionOfIndex(indexValue);
                    x = pos.X;
                    y = pos.Y;
                }

                distance = (float)Math.Sqrt(x * x + y * y);

                double tan = y / x;
                float temp = (float)Math.Round(Math.Atan(tan) * degreesPerRadian, 3);

                angle = (x >= 0) ? temp : 180 + temp;

                if (angle < 0)
                    angle += 360;
            }

            public bool IsVertice(int index)
            {
                int round = Shape.RoundOf(index);

                if (!VerticesPositions.ContainsKey(round))
                    VerticesPositions.Add(round, getVerticeOfRound(round));

                return VerticesPositions[round].ContainsKey(index);
            }

            #endregion
        }
    }
}
