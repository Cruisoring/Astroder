using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper.DoubleHelper;
using System.Reflection;

namespace NumberHelper
{
    public class Polygon
    {
        #region Constants
        public const int DefaultCycleNumber = 10;
        public const int DefaultDecimals = 1;
        public const string DefaultName = "Unknown";
        public const int MaximumIndex = 100000;

        public static List<double> DueDirections = new List<Double>()
        {
            0,
            90,
            180,
            270,
            360
        };

        public static Dictionary<int, string> PolygonNames = new Dictionary<int, string>
        {
            { 1, "Henagon"},
            { 2, "Digon"},

            { 3, "Triangle"},
            { 4, "Tetragon"},

            { 5, "Pentagon"},
            { 6, "Hexagon"},

            { 7, "Heptagon"},
            { 8, "Octagon"},

            { 9, "Enneagon"},
            { 10, "Decagon"},

            { 11, "Hendecagon"},
            { 12, "Dodecagon"},

            { 13, "Tridecagon"},
            { 14, "Tetradecagon"},

            { 15, "Pentadecagon"},
            { 16, "Hexadecagon"},

            { 17, "Heptadecagon"},
            { 18, "Octadecagon"},

            { 19, "Enneadecagon"},
            { 20, "Icosagon"}
        };

        protected const double RadiansPerDegree = Math.PI / 180.0;
        protected const double DegreesPerRadians = 180.0 / Math.PI;
        #endregion

        #region Delegate definition
        public delegate Angle IndexToAngleDelegate(Double index);
        public delegate Double AngleOnRoundToIndexDelegate(int round, Angle angle);
        #endregion

        #region Static Definition
        public static Polygon SquareNine = new Polygon("SquareNine", 4, true, 2, false);
        public static Polygon SquareFour = new Polygon("SquareFour", 4, true, 2, true);
        public static Polygon Hexagon = new Polygon(6);
        public static Polygon Octagon = new Polygon(8);
        public static Polygon Pentagon = new Polygon(5);
        //public static Polygon Triangle = new Polygon(3);
        public static Polygon Circle24 = new Polygon(24, false, 0, true);
        public static Polygon Circle30 = new Polygon(30, false, 0, true);
        //public static Polygon Circle360 = new Polygon(360, false, 0, true);

        public static Dictionary<String, Polygon> All = new Dictionary<string, Polygon>();

        static Polygon()
        {
            Type polygonType = typeof(Polygon);
            FieldInfo[] fields = polygonType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                Polygon instance = fi.GetValue(null) as Polygon;

                if (fi.FieldType == typeof(Polygon))
                    All.Add(fi.Name, instance);
            }
        }

        #endregion

        #region Fields
        public string Name { get; private set; }

        /// <summary>
        /// Number of the edges
        /// </summary>
        public int Edges { get; private set; }
        public int Fold { get; private set; }

        public bool IsEven {get; private set;}
        public bool IsPolygon { get; private set; }
        public Dictionary<int, Angle> Vertices { get; private set; }
        public SortedDictionary<double, Aspects> Outstandings { get; private set; }
        public Angle Sector { get; private set; }

        public IndexToAngleDelegate AngleOf { get; private set;}
        public AngleOnRoundToIndexDelegate IndexOf {get; private set; }

        public Double FirstVertexDegree 
        {
            get { return (Edges == 4 || !IsPolygon) ? 180.0 / Edges : 360.0 / Edges; }
        }
        #endregion

        #region Constructors
        private Polygon(string name, int edges, bool isPolygon, int fold, bool isEven)
        {
            Name = name;
            Edges = edges;
            Fold = fold;
            IsEven = isEven;
            IsPolygon = isPolygon;

            if (isPolygon)
            {
                AngleOf = new IndexToAngleDelegate(AngleOnLine);
                IndexOf = new AngleOnRoundToIndexDelegate(IndexOnLine);
            }
            else
            {
                AngleOf = new IndexToAngleDelegate(AngleOnArc);
                IndexOf = new AngleOnRoundToIndexDelegate(IndexOnArc);
            }

            Vertices = new Dictionary<int, Angle>();

            double sectorDegrees = 360.0 / Edges;
            Sector = new Angle(sectorDegrees);

            List<double> concerned = new List<double>(DueDirections);

            for (int i = 0; i <= Edges; i++)
            {
                double vertexDegree = FirstVertexDegree + i * sectorDegrees;

                //if (vertexDegree > 360)
                //    continue;

                Vertices.Add(i, new Angle(vertexDegree));

                if (!concerned.Contains(vertexDegree))
                    concerned.Add(vertexDegree);
            }

            Outstandings = Aspects.GetAspectsDictionary(concerned);

        }

        private Polygon(int edges, bool isPolygon, int fold, bool isEven) 
            : this(DefaultName, edges, isPolygon, fold, isEven)
        {
            if (isPolygon)
            {
                if (PolygonNames.ContainsKey(edges))
                    Name = PolygonNames[edges];
                else
                    Name = "Polygon" + edges.ToString();
            }
            else
            {
                Name = "Circle" + edges.ToString();
            }
        }

        private Polygon(int sides) : this(sides, true, 1, true) { }

        #endregion

        public int LengthOfRound(int round)
        {
            if (IsPolygon)
            {
                if (round > 0)
                    return (IsEven == true && Fold == 2) ?
                        (2 * round - 1) * Edges * Fold / 2 :
                        round * Edges * Fold;
                else if (round == 0)
                    return IsEven == false ? 1 : 0;
                else
                    throw new ArgumentOutOfRangeException("Round shall be no less than 0!");
            }
            else
            {
                if (round == 0) return 0;
                else
                    return Edges * (int)Math.Pow(Fold, round - 1);
            }
        }

        public int LastOfRound(int round)
        {
            if (IsPolygon)
            {
                if (round > 0)
                    return ((IsEven == true && Fold == 2) ? round : round + 1) * round * Edges * Fold / 2
                        + (IsEven == false ? 1 : 0);
                else if (round == 0)
                    return IsEven == false ? 1 : 0;
                else
                    throw new ArgumentOutOfRangeException("Round shall be no less than 0!");
            }
            else
            {
                if (Fold == 0)
                {
                    return round * Edges;
                }
                else
                    throw new NotImplementedException();
            }
        }

        public int FirstOfRound(int round)
        {
            return LastOfRound(round - 1) + 1;                  
        }

        private int distanceToRound(int round, double index)
        {
            int max = LastOfRound(round) + 1;
            int min = (LengthOfRound(round) == 0 ? max -1 : max - LengthOfRound(round));

            if (index < max && index >= min)
                return 0;
            else if (max < index)
                return Math.Max(1, (int)Math.Sqrt(index - max));
            else
                return -1;
        }

        public int RoundOf(Double index)
        {
            if (IsPolygon)
            {
                int roundNum = (int)Math.Sqrt(index);

                if (roundNum <= 1)
                    return 1;

                int delta = 0;
                do
                {
                    delta = distanceToRound(roundNum, index);
                    roundNum += delta;
                } while (delta != 0);
                return roundNum;
            }
            else
            {
                if (index > 360 && index != 361)
                    index %= 360;

                if (index % Edges == 0)
                    return (int)index / Edges;
                else
                    return (int)Math.Ceiling(index / Edges);
            }
        }

        //public Angle AngleOf(Double index)
        //{
        //    return (IsPolygon == LineType.Opposition) ? AngleOnLine(index) : AngleOnArc(index);
        //}

        public Angle AngleOnArc(Double index)
        {
            if (Fold != 0)
                throw new NotImplementedException();

            if (index < 0)
            {
                double multi = Math.Floor(index / Edges);
                index -= multi * Edges;
            }

            int roundNum = RoundOf(index);

            int first = Edges * (roundNum -1 );

            return new Angle((index - first) * 360 / Edges);
        }

        public Angle AngleOnLine(Double index)
        {
            //if (index < 1 || index > MaximumIndex)
            //    throw new ArgumentOutOfRangeException("Too big index, adapter is needed.");

            if (index <= 1 )
                return Angle.Zero;

            try
            {
                int roundNum = RoundOf(index);

                if (roundNum <= 0)
                    return Angle.Zero;

                int last = LastOfRound(roundNum);
                int sideLength = LengthOfRound(roundNum) / Edges;
                int first = (sideLength == 0 ? last : last - sideLength * Edges + 1);

                double offsetToMax = last - index;

                if (offsetToMax % sideLength == 0)
                {
                    return Vertices[Edges - 1 - (int)offsetToMax / sideLength];
                }
                else if (offsetToMax < 0)
                {
                    return Angle.FromOffsetBetween(Vertices[Edges - 2], Vertices[Edges - 1], 1 - offsetToMax / sideLength);
                }
                else
                {
                    int startIndex = Edges - 1 - (int)Math.Ceiling(offsetToMax / sideLength);
                    int endIndex = startIndex + 1;
                    Double offset = (Edges - 1 - startIndex) - offsetToMax / sideLength;

                    return Angle.FromOffsetBetween(Vertices[(Edges + startIndex) % Edges], Vertices[(Edges + endIndex) % Edges], offset);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to convert {0}", index);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Angle.Zero;
            }
        }

        //public Double IndexOf(int round, Angle angle)
        //{
        //    if (IsPolygon == LineType.Opposition)
        //        return IndexOnLine(round, angle);
        //    else
        //        return IndexOnArc(round, angle);
        //}

        public double IndexOnArc(int round, Angle angle)
        {
            if (Fold != 0)
                throw new NotImplementedException();

            int first = Edges * (round - 1) + 1;

            return (angle.Degrees - Vertices[0].Degrees) * Edges / 360 + first;
        }

        public Double IndexOnLine(int round, Angle angle)
        {
            if (round == 0)
                return 1;
            else if (round < 0)
                throw new ArgumentOutOfRangeException("Round Number shall be bigger than 1.");

            int last = LastOfRound(round);
            int sideLength = LengthOfRound(round) / Edges;
            int first = last - sideLength * Edges + 1;

            double sectorAngle = 360.0 / Edges;

            if ((angle.Degrees - FirstVertexDegree) % sectorAngle == 0)
                return (FirstVertexDegree == sectorAngle) ? 
                    last - (angle.Degrees == 0 ? 0 : (Edges - angle.Degrees/sectorAngle)) * sideLength
                    : last - (Edges - 1 - (angle.Degrees - FirstVertexDegree) / sectorAngle) * sideLength;

            double offset;

            try
            {
                if (FirstVertexDegree == sectorAngle)
                {
                    int nextCorner = (int)Math.Ceiling((angle.Degrees - FirstVertexDegree) / sectorAngle);

                    if (nextCorner == 0)
                    {
                        Double degreeOfFirst = AngleOf(first).Degrees;

                        if (angle.Degrees == degreeOfFirst)
                            return first;
                        else if (angle.Degrees > degreeOfFirst)
                        {
                            offset = Sector.OffsetFromDegree((FirstVertexDegree - angle.Degrees));
                            return last - (Edges - 1 + offset) * sideLength;
                        }
                        else
                        {
                            double degreeOfLast = AngleOf(last + 1).Degrees;
                            if (angle.Degrees >= degreeOfLast)
                                return last;
                            else
                            {
                                offset = Sector.OffsetFromDegree((angle.Degrees - Vertices[Edges - 2].Degrees).Normalize());
                                return last - (1 - offset) * sideLength;
                            }
                        }
                    }
                    else
                    {
                        offset = Sector.OffsetFromDegree((Vertices[nextCorner].Degrees - angle.Degrees).Normalize());
                        return last - (Edges - 1 - nextCorner + offset) * sideLength;
                    }
                }
                else
                {
                    int lowerCorner = (int)Math.Floor((angle.Degrees - FirstVertexDegree) / sectorAngle);

                    if (lowerCorner == -1)
                    {
                        double delta = FirstVertexDegree - angle.Degrees;
                        //offset = Sector.OffsetFromDegree((FirstVertexDegree - angle.Degrees).Normalize());
                        //return last  + offset * sideLength;
                        offset = Sector.OffsetFromDegree(delta);
                        return last - (Edges - 1 + offset) * sideLength;
                    }
                    else if (lowerCorner >= Edges - 1)
                    {
                        Double degreeOfFirst = AngleOf(first).Degrees;

                        if (angle.Degrees == degreeOfFirst)
                            return first;
                        else if (degreeOfFirst > Vertices[Edges - 1].Degrees && angle.Degrees > degreeOfFirst)
                        {
                            offset = Sector.OffsetFromDegree((FirstVertexDegree - angle.Degrees).Normalize());
                            return last - (Edges - 1 + offset) * sideLength;
                        }
                        else
                        {
                            double degreeOfLast = AngleOf(last + 1).Degrees;
                            if (angle.Degrees >= (degreeOfLast == 0 ? 360.0 : degreeOfLast))
                                return last;
                            else
                            {
                                offset = Sector.OffsetFromDegree((angle.Degrees - Vertices[Edges - 2].Degrees));
                                return last - (1 - offset) * sideLength;
                            }
                        }
                    }
                    else
                    {
                        offset = Sector.OffsetFromDegree((angle.Degrees - Vertices[lowerCorner].Degrees).Normalize());
                        return last - (Edges - 1 - lowerCorner - offset) * sideLength;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return 0;
            }
        }

        public bool IsOutstanding(Angle angle)
        {
            if (IsPolygon)
            {
                double degree = angle.Degrees.Normalize();

                IEnumerable<double> aspectQuery =
                    from refDeg in Outstandings.Keys
                    let dev = Math.Abs(degree - refDeg)
                    orderby dev
                    select refDeg;

                double reference = (aspectQuery).First();
                return Math.Abs(reference - degree) <= Outstandings[reference].PermissibleOrb;
            }
            else
                return angle.Degrees == Math.Round(angle.Degrees);
        }

        public bool IsOutstanding(Double index)
        {
            return IsOutstanding(AngleOf(index));
        }

        public Aspects CurrentAspectOf(Angle angle)
        {
            return Aspects.CurrentAspectOf(Outstandings, angle);
        }

        public bool HasAspectBetween(Angle start, Angle end)
        {
            return CurrentAspectOf(end - start) != null;
        }

        public Aspects AspectBetween(Angle start, Angle end)
        {
            return Aspects.AspectBetween(Outstandings, start, end);
        }

        public Aspects NextAspectOf(Angle angle)
        {
            return Aspects.NextAspectOf(Outstandings, angle);
        }

        public Aspects PreviousAspectOf(Angle angle)
        {
            return Aspects.PreviousAspectOf(Outstandings, angle);
        }


        public override string ToString()
        {
            return String.Format("{0}{1}+{2}", Name, Sector, Fold);
            //return string.Format("\r\nPolygon with {0edgeses, expand step = {1}, Mirror of {2}", Edges, Fold, IsEven);
        }
    }


}
