using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;

namespace AstroHelper
{
    public class Realm
    {
        public double Start { get; set;}
        public double End { get; set; }

        public Realm(double start, double end)
        {
            Start = start;
            End = end;
        }

        public Realm(double point)
        {
            Start = point;
            End = point;
        }

        public bool Owns(double degree)
        {
            return degree <= End && degree >= Start;
        }

        public bool LessOrEqual(double degree)
        {
            return degree <= End;
        }

        public override string ToString()
        {
            return Start == End ? Start.ToString() : string.Format("{0}-{1}", Start, End);
        }
    }

    public class Rulership
    {
        #region static fields
        public const Double Division = 6.75;

        public static List<Realm> Regions = new List<Realm>();

        public static readonly Dictionary<int, PlanetId> Rulers = new Dictionary<int, PlanetId>
        {
            { 1, PlanetId.SE_MARS },
            { 2, PlanetId.SE_VENUS },
            { 3, PlanetId.SE_MERCURY },
            { 4, PlanetId.SE_MOON },
            { 5, PlanetId.SE_SUN },
            { 6, PlanetId.SE_MERCURY },
            { 7, PlanetId.SE_VENUS },
            { 8, PlanetId.SE_MARS },
            { 9, PlanetId.SE_JUPITER },
            { 10, PlanetId.SE_SATURN },
            { 11, PlanetId.SE_URANUS },
            { 12, PlanetId.SE_NEPTUNE }
        };

        static Rulership()
        {
            double dist = Division / 6;
            double widthEven = Math.IEEERemainder(dist, 1);
            Double distToEven = (dist - widthEven) / 2;

            for (int i = 0; i < 6; i ++ )
            {
                double oddPoint = i * dist;
                Regions.Add(new Realm(oddPoint));
                Regions.Add(new Realm(oddPoint + distToEven, oddPoint + distToEven + widthEven));
            }

            Regions.Add(new Realm(Division));
        }

        private static int HouseOf(double remains)
        {
            for(int i = 0; i < Regions.Count ; i ++)
            {
                if (Regions[i].Owns(remains))
                    return i % 12 + 1;
                else if (Regions[i].LessOrEqual(remains))
                    return i % 12 + 1;
            }

            throw new Exception();
        }

        #endregion

        #region Properties

        public Angle Position { get; set; }

        public Double Remains { get; set; }

        public int House { get; set; }

        public Realm Ruling { get { return Regions[House - 1]; } }

        public PlanetId Ruler { get { return Rulers[House]; } }

        public bool IsExactly { get { return Ruling.Owns(Remains); } }

        #endregion

        public Rulership(Angle angle)
        {
            Position = angle;

            int round = (int) (Position.Degrees / Division);

            Remains = Math.Round(Position.Degrees - round * Division, 2);

            House = HouseOf(Remains);
        }

        public override string ToString()
        {
            return string.Format("{0}: remains={1}, of house {2}, {5} ruled by {3}({4})", Position, Remains, House, Ruler, Ruling,
                IsExactly ? "exactly" : "not exactly");
        }
    }
}
