using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Color = System.Drawing.Color;

namespace EphemerisCalculator
{
    [Serializable]
    public enum PlanetId : int
    {
        SE_ECL_NUT = -1,            /// Special body number aspectDegree compute obliquity and nutation.

        SE_SUN = 0,                 /// Order for Sun
        SE_MOON = 1,         // Order for Moon
        SE_MERCURY = 2,         // Order for Mercury
        SE_VENUS = 3,         // Order for Venus
        SE_MARS = 4,         // Order for Mars
        SE_JUPITER = 5,         // Order for Jupiter
        SE_SATURN = 6,         // Order for Saturn
        SE_URANUS = 7,         // Order for Uranus
        SE_NEPTUNE = 8,         // Order for Neptune
        SE_PLUTO = 9,         // Order for Pluto

        SE_MEAN_NODE = 10,         // Order for standard node (mean)
        SE_TRUE_NODE = 11,         // oscillating node (true)
        SE_MEAN_APOG = 12,      //
        SE_OSCU_APOG = 13,

        SE_EARTH = 14,
        SE_CHIRON = 15,         // Order for Chiron,凯龙星
        SE_PHOLUS = 16,         // 福鲁斯
        SE_CERES = 17,          // 谷神星
        SE_PALLAS = 18,         // 智神星
        SE_JUNO = 19,           // 婚神星
        SE_VESTA = 20,          // 灶神星
        SE_INTP_APOG = 21,
        SE_INTP_PERG = 22,

        SE_NPLANETS = 23,
        SE_FICT_OFFSET = 40,

        SE_PRICE = 100,
        SE_NORTHNODE = 101,
        SE_SOUTHNODE = 102,

        Earth_Rotation = 200,
        Five_Average = 205,
        Six_Average = 206,
        Eight_Average = 208,

        SE_AST_OFFSET = 10000
    }

    //public enum OrbitInfoType
    //{
    //    //All,
    //    Longitude,
    //    Latitude,
    //    Apparent,
    //    LongitudeVelocities,
    //    LatitudeVelocities,
    //    DistanceVelocities,

    //    LongitudeAcceleration,
    //    LongVelocityAndLatitude,

    //    Ascending,
    //    Descending,
    //    Perigee,
    //    Apogee,

    //    AscendingLatitude,
    //    DescendingLatitude,
    //    PerigeeLatitude,
    //    ApogeeLatitude,

    //    AscendingVelocities,
    //    DescendingVelocities,
    //    PerigeeVelocities,
    //    ApogeeVelocities,

    //    Other
    //}

    [Serializable]
    public class Planet : IComparable<Planet>
    {
        #region Constants and static fields
        public const String Undefined = "Unknown";

        public const double StationaryThresholdPercentage = 0.01;

        public static Dictionary<PlanetId, Char> Glyphs = new Dictionary<PlanetId, Char>()
        {
            { PlanetId.SE_ECL_NUT, '\uFF1F'},
            { PlanetId.SE_SUN, '\u2609'},
            { PlanetId.SE_MOON, '\u263D'},
            { PlanetId.SE_MERCURY, '\u263F'},
            { PlanetId.SE_VENUS, '\u2640'},
            { PlanetId.SE_MARS, '\u2642'},
            { PlanetId.SE_JUPITER, '\u2643'},
            { PlanetId.SE_SATURN, '\u2644'},
            { PlanetId.SE_URANUS, '\u2645'},
            { PlanetId.SE_NEPTUNE, '\u2646'},
            { PlanetId.SE_PLUTO, '\u2647'},
            { PlanetId.SE_PRICE, '$'},
            { PlanetId.SE_SOUTHNODE, '\u260B'},
            { PlanetId.SE_NORTHNODE, '\u260A'},
            { PlanetId.SE_CHIRON, '\u26B7' },
            { PlanetId.SE_EARTH, '\u2295'},
            { PlanetId.Earth_Rotation, '\u2638'},
            { PlanetId.Five_Average, '\u2464'},
            { PlanetId.Six_Average, '\u2465'},
            { PlanetId.Eight_Average, '\u2467'}
        };

        public static Dictionary<Char, PlanetId> GlyphToPlanetId = new Dictionary<Char, PlanetId>() { };

        #region Physical fact data of planets
        public static Dictionary<PlanetId, double> AverageDistances = new Dictionary<PlanetId, Double>()
        {
            { PlanetId.SE_ECL_NUT, 1},
            { PlanetId.SE_SUN, 1},
            { PlanetId.SE_MOON, 0.0025 },
            { PlanetId.SE_MERCURY, 1},
            { PlanetId.SE_VENUS, 1.1},
            { PlanetId.SE_MARS, 1.8},
            { PlanetId.SE_JUPITER, 5},
            { PlanetId.SE_SATURN, 9.3},
            { PlanetId.SE_URANUS, 20},
            { PlanetId.SE_NEPTUNE, 30},
            { PlanetId.SE_PLUTO, 30},
            { PlanetId.SE_PRICE, 0},
            { PlanetId.SE_SOUTHNODE, 0.0025},
            { PlanetId.SE_NORTHNODE, 0.0025},
            { PlanetId.SE_CHIRON, 30}
        };

        /// <summary>
        /// The average orbital duration in days.
        /// </summary>
        public static Dictionary<PlanetId, double> OrbitalPeriods = new Dictionary<PlanetId, double>()
        {
            { PlanetId.SE_SUN, Ephemeris.AverageYearLength },
            { PlanetId.SE_EARTH, Ephemeris.AverageYearLength },
            { PlanetId.SE_MOON,  27.321582 },
            { PlanetId.SE_MERCURY, 87.9691 },
            { PlanetId.SE_VENUS, 224.70069 },
            { PlanetId.SE_MARS, 686.971 },
            { PlanetId.SE_JUPITER, 4331.572 },
            { PlanetId.SE_SATURN, 10759.22 },
            { PlanetId.SE_URANUS, 30799.095 },
            { PlanetId.SE_NEPTUNE, 60190},
            { PlanetId.SE_PLUTO, 90613.305},
            { PlanetId.SE_SOUTHNODE, 6585.3213},
            { PlanetId.SE_NORTHNODE, 6585.3213},
            { PlanetId.SE_CHIRON, 18540},
            { PlanetId.SE_PHOLUS, 33547.41 },
            { PlanetId.SE_CERES, 1680.5 },
            { PlanetId.SE_PALLAS, 1686.33 },
            { PlanetId.SE_JUNO, 1591.93 },
            { PlanetId.SE_VESTA, 1325.46 },
            { PlanetId.Five_Average, 500},
            { PlanetId.Six_Average, 180 },
            { PlanetId.Eight_Average, 80 }

        };

        /// <summary>
        /// The approximate retrograde periods of major planets.
        /// </summary>
        public static Dictionary<PlanetId, double> RetrogradePeriods = new Dictionary<PlanetId, double>()
        {
            { PlanetId.SE_MERCURY, 21 },
            { PlanetId.SE_VENUS, 45 },
            { PlanetId.SE_MARS, 72 },
            { PlanetId.SE_JUPITER, 121 },
            { PlanetId.SE_SATURN, 138 },
            { PlanetId.SE_URANUS, 151 },
            { PlanetId.SE_NEPTUNE, 158},
            { PlanetId.SE_PLUTO, 180},
            { PlanetId.SE_CHIRON, 150 }
        };

        #endregion

        /// <summary>
        /// http://www.articlesbase.com/astrology-articles/characteristic-of-colours-in-relation-to-planets-in-astrology-206344.html
        /// </summary>
        public static Dictionary<PlanetId, List<Color>> PlanetsColors = new Dictionary<PlanetId, List<Color>>
        {
            { PlanetId.SE_ECL_NUT, new List<Color>{Color.DarkGray, Color.Gray, Color.LightGray} },
            { PlanetId.SE_SUN, new List<Color>{Color.Gold, Color.Chocolate, Color.Orange} },
            { PlanetId.SE_MOON,  new List<Color>{Color.BlueViolet, Color.Ivory, Color.Black} },
            { PlanetId.SE_MERCURY, new List<Color>{Color.Green, Color.Turquoise, Color.Olive}  },
            { PlanetId.SE_VENUS, new List<Color>{Color.Violet, Color.Purple, Color.Orchid}  },
            { PlanetId.SE_MARS, new List<Color>{Color.Red, Color.Maroon, Color.IndianRed}  },
            { PlanetId.SE_JUPITER, new List<Color>{Color.DarkSalmon, Color.Yellow, Color.SandyBrown}  },
            { PlanetId.SE_SATURN, new List<Color>{Color.Blue, Color.Navy, Color.SkyBlue}  },
            { PlanetId.SE_URANUS, new List<Color>{Color.LightBlue, Color.RoyalBlue, Color.PowderBlue}  },
            { PlanetId.SE_NEPTUNE, new List<Color>{Color.LawnGreen, Color.DarkOliveGreen, Color.YellowGreen} },
            { PlanetId.SE_PLUTO, new List<Color>{Color.Brown, Color.Firebrick, Color.LightSalmon} },
            { PlanetId.SE_NORTHNODE, new List<Color>{Color.CadetBlue, Color.DimGray, Color.Indigo} },
            { PlanetId.SE_CHIRON, new List<Color>{Color.HotPink, Color.DeepPink, Color.Pink} },
            { PlanetId.SE_EARTH, new List<Color>{Color.Gold, Color.Chocolate, Color.Orange}  },
            { PlanetId.Five_Average, new List<Color>{Color.IndianRed, Color.Red, Color.Maroon} },
            { PlanetId.Six_Average, new List<Color>{Color.Indigo, Color.CadetBlue, Color.DimGray} },
            { PlanetId.Eight_Average, new List<Color>{Color.Pink, Color.HotPink, Color.DeepPink} },
            { PlanetId.Earth_Rotation, new List<Color>{Color.DarkOliveGreen, Color.Chocolate, Color.Orange} }
        };

        //public static Color ColorOf(PlanetId id1)
        //{
        //    return (PlanetsColors.ContainsKey(id1)) ? PlanetsColors[id1] : Color.Black;
        //}

        //public static Color ColorOf(PlanetId id1, OrbitInfoType kind)
        //{
        //    if (!PlanetsColors.ContainsKey(id1))
        //        return ColorOf(PlanetId.SE_ECL_NUT, kind);
        //    else
        //    {
        //        List<Color> colorSet = PlanetsColors[id1];

        //        int i = (int)kind;
        //        if (i < colorSet.Count)
        //            return colorSet[i];
        //        else if (i < colorSet.Count * 2)
        //        {
        //            Color velocityColor = WeightedColor(colorSet[i % 3], 0.8f, false);
        //            return velocityColor;
        //        }
        //        else
        //        {
        //            Color elseColor = WeightedColor(colorSet[i % 3], 1.0f, true);
        //            return elseColor;
        //        }
        //    }
        //}

        public static Color WeightedColor(Color baseColor, float percentage, bool isTransparent)
        {
            int a = isTransparent ? (int)(255 * percentage) : baseColor.A;
            int r = (int)(baseColor.R * percentage);
            int g = (int)(baseColor.G * percentage);
            int b = (int)(baseColor.B * percentage);
            return Color.FromArgb(a, r, g, b);
        }

        //public static Planet Sun = new Planet(PlanetId.SE_SUN, "Sun", DayOfWeek.Sunday);
        //public static Planet Moon = new Planet(PlanetId.SE_MOON, "Moon", DayOfWeek.Monday);
        //public static Planet Mercury = new Planet(PlanetId.SE_MERCURY, "Mercury", DayOfWeek.Wednesday);
        //public static Planet Venus = new Planet(PlanetId.SE_VENUS, "Venus", DayOfWeek.Friday);
        //public static Planet Mars = new Planet(PlanetId.SE_MARS, "Mars", DayOfWeek.Tuesday);
        //public static Planet Jupiter = new Planet(PlanetId.SE_JUPITER, "Jupiter", DayOfWeek.Thursday);
        //public static Planet Saturn = new Planet(PlanetId.SE_SATURN, "Saturn", DayOfWeek.Saturday);
        //public static Planet Uranus = new Planet(PlanetId.SE_URANUS, "Uranus");
        //public static Planet Neptune = new Planet(PlanetId.SE_NEPTUNE, "Neptune");
        //public static Planet Pluto = new Planet(PlanetId.SE_PLUTO, "Pluto");
        //public static Planet NorthNode = new Planet(PlanetId.SE_NORTHNODE, "NorthNode");
        //public static Planet Chiron = new Planet(PlanetId.SE_CHIRON, "Chiron");
        //public static Planet FiveAverage = new Planet(PlanetId.Five_Average, "FiveAverage");
        //public static Planet SixAverage = new Planet(PlanetId.Five_Average, "SixAverage");
        //public static Planet EightAverage = new Planet(PlanetId.Five_Average, "EightAverage");
        //public static Planet SouthNode = new Planet(PlanetId.SE_SOUTHNODE, "SouthNode");

        public static Dictionary<PlanetId, Planet> All = new Dictionary<PlanetId, Planet>
        {
            { PlanetId.SE_SUN, new Planet(PlanetId.SE_SUN, "Sun", DayOfWeek.Sunday) },
            { PlanetId.SE_MOON, new Planet(PlanetId.SE_MOON, "Moon", DayOfWeek.Monday)},
            { PlanetId.SE_MERCURY, new Planet(PlanetId.SE_MERCURY, "Mercury", DayOfWeek.Wednesday)},
            { PlanetId.SE_VENUS,  new Planet(PlanetId.SE_VENUS, "Venus", DayOfWeek.Friday)},
            { PlanetId.SE_MARS, new Planet(PlanetId.SE_MARS, "Mars", DayOfWeek.Tuesday)},
            { PlanetId.SE_JUPITER, new Planet(PlanetId.SE_JUPITER, "Jupiter", DayOfWeek.Thursday)},
            { PlanetId.SE_SATURN, new Planet(PlanetId.SE_SATURN, "Saturn", DayOfWeek.Saturday)},
            { PlanetId.SE_URANUS, new Planet(PlanetId.SE_URANUS, "Uranus")},
            { PlanetId.SE_NEPTUNE, new Planet(PlanetId.SE_NEPTUNE, "Neptune")},
            { PlanetId.SE_PLUTO, new Planet(PlanetId.SE_PLUTO, "Pluto")},
            { PlanetId.SE_NORTHNODE, new Planet(PlanetId.SE_NORTHNODE, "NorthNode")},
            { PlanetId.SE_CHIRON, new Planet(PlanetId.SE_CHIRON, "Chiron")},
            { PlanetId.SE_EARTH, new Planet(PlanetId.SE_EARTH, "Earth")},
            { PlanetId.Earth_Rotation, new Planet(PlanetId.Earth_Rotation, "EarthRotation")},
            { PlanetId.Five_Average, new Planet(PlanetId.Five_Average, "FiveAverage")},
            { PlanetId.Six_Average, new Planet(PlanetId.Six_Average, "SixAverage")},
            { PlanetId.Eight_Average, new Planet(PlanetId.Eight_Average, "EightAverage")},
            { PlanetId.SE_ECL_NUT, new Planet(PlanetId.SE_ECL_NUT, "None")}
            //{ PlanetId.SE_SOUTHNODE, new Planet(PlanetId.SE_SOUTHNODE, "SouthNode")}
        };

        public static Planet PlanetOf(PlanetId id)
        {
            if (All.ContainsKey(id))
                return All[id];
            else
                return null;
        }

        #endregion

        static Planet()
        {
            foreach (KeyValuePair<PlanetId, char> kvp in Glyphs)
            {
                GlyphToPlanetId.Add(kvp.Value, kvp.Key);
            }
        }

        #region Static functions

        public static bool TryParseId(string s, out PlanetId result)
        {
            if (Enum.IsDefined(typeof(PlanetId), s))
            {
                result = (PlanetId)Enum.Parse(typeof(PlanetId), s, true);
                return true;
            }
            else
            {
                foreach (string value in Enum.GetNames(typeof(PlanetId)))
                {
                    if (value.Equals(s, StringComparison.OrdinalIgnoreCase))
                    {
                        result = (PlanetId)Enum.Parse(typeof(PlanetId), value);
                        return true;
                    }
                }

                foreach (Planet star in All.Values)
                {
                    if (star.Name.Equals(s, StringComparison.OrdinalIgnoreCase))
                    {
                        result = star.Id;
                        return true;
                    }
                }

                result = PlanetId.SE_ECL_NUT;
                return false;
            }
        }

        public static Double AverageSpeedOf(PlanetId star)
        {
            return 360.0 / OrbitalPeriods[star]; 
        }

        public static char SymbolOf(PlanetId id)
        {
            if (Glyphs.ContainsKey(id))
                return Glyphs[id];
            else
                return '?';
        }

        #endregion

        #region Properties and Variables

        public PlanetId Id { get; private set; }

        public String Name { get; private set; }

        public Char Symbol
        {
            get { return Glyphs.ContainsKey(Id) ? Glyphs[Id] : Glyphs[PlanetId.SE_ECL_NUT]; }
        }

        public DayOfWeek Day { get; private set; }

        public Double OrbitalPeriod { get; private set; }

        public Double OrbitalSpeed { get; private set; }

        //public Func<List<Double>, List<

        #region Temporary useless fields
        //private readonly List<Sign> rules;

        //public List<Sign> Rules
        //{
        //    get { return rules; }
        //}

        //private readonly List<Sign> exalts;

        //public List<Sign> Exalts
        //{
        //    get { return exalts; }
        //}

        //private readonly List<Sign> detriments;

        //public List<Sign> Detriments
        //{
        //    get { return detriments; }
        //}

        //private readonly List<Sign> falls;

        //public List<Sign> Falls
        //{
        //    get { return falls; }
        //}
        #endregion

        #endregion

        #region Constructors

        private Planet(PlanetId id, string name, DayOfWeek day)
        {
            Id = id;
            Name = name;
            Day = day;
            OrbitalPeriod = (OrbitalPeriods.ContainsKey(id)) ? OrbitalPeriods[id] : Ephemeris.AverageYearLength;
            OrbitalSpeed = 360 / OrbitalPeriod;
        }

        private Planet(PlanetId id, string name) : this(id, name, DayOfWeek.Sunday) {}
        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region IComparable<Planet> 成员

        public int CompareTo(Planet other)
        {
            return ((int)this.Id).CompareTo((int)other.Id);
        }

        #endregion

        public Position GeoPositionOn(DateTimeOffset time)
        {
            return Ephemeris.GeocentricPositionOf(time, Id);
        }

        public Position HelioPositionOn(DateTimeOffset time)
        {
            return Ephemeris.HeliocentricPositionOf(time, Id);
        }

        public Double GeoMovementDegrees(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            Position startPos = GeoPositionOn(startDate);
            Position endPos = GeoPositionOn(endDate);

            Double cycles = (endDate - startDate).TotalDays / OrbitalPeriod;
            Double degrees = (int)cycles * 360;
            double remains = cycles - (int)cycles;

            degrees += (remains < 0.3) ? Angle.ShortestMovement(startPos.TheRectascension, endPos.TheRectascension) : Angle.ClockwiseMovement(startPos.TheRectascension, endPos.TheRectascension);
            return degrees;
        }

        public Double HelioMovementDegrees(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            Position startPos = HelioPositionOn(startDate);
            Position endPos = HelioPositionOn(endDate);

            int cycles = (int)((endDate - startDate).TotalDays / OrbitalPeriod);
            return cycles * 360 + Angle.ClockwiseMovement(startPos.TheRectascension, endPos.TheRectascension);
        }

        /// <summary>
        /// Check if the longitude is stationary by checking if its longitude speed is almost 0.
        /// </summary>
        /// <param name="pos">Position of the planet.</param>
        /// <returns>True if the longitude speed less than .</returns>
        public bool IsHorizontalStationary(Position pos)
        {
            return Math.Abs(pos.LongitudeVelocity) <= StationaryThresholdPercentage * OrbitalSpeed;
        }

        /// <summary>
        /// Check if the latitude is stationary by checking if its latitude speed is almost 0.
        /// </summary>
        /// <param name="pos">Position of the planet.</param>
        /// <returns>True if the latitude speed less than .</returns>
        public bool IsVerticalStationary(Position pos)
        {
            return Math.Abs(pos.LatitudeVelocity) <= 0.0001;
        }
    }

    [Serializable]
    public class PlanetPair : IComparable<PlanetPair>, IEquatable<PlanetPair>
    {
        public const double DefaultOrb = 1.0;

        public PlanetId Interior { get; private set; }

        public PlanetId Exterior { get; private set; }

        public PlanetPair(PlanetId id1, PlanetId id2)
        {
            if (id1 == id2 && id1 != PlanetId.SE_ECL_NUT)
            {
                Interior = id1;
                Exterior = PlanetId.SE_ECL_NUT;
            }
            else if (id1 == PlanetId.SE_ECL_NUT)
            {
                Interior = id2;
                Exterior = id1;
            }
            else if (id2 == PlanetId.SE_ECL_NUT || (id1 < id2 && id2 != PlanetId.SE_EARTH))
            {
                Interior = id1;
                Exterior = id2;
            }
            else
            {
                Interior = id2;
                Exterior = id1;
            }
        }

        public double Orb
        {
            get
            {
                if (Exterior == PlanetId.SE_ECL_NUT)
                {
                    if (Interior <= PlanetId.SE_PLUTO && Interior >= PlanetId.SE_URANUS)
                        return 3650 / Planet.OrbitalPeriods[Interior];

                    return DefaultOrb;
                }
                else if (Exterior > PlanetId.SE_SATURN)
                {
                    return 1;
                }
                else
                {
                    return 0.5;
                }
            }
        }

        public List<double> ConcernedAspects
        {
            get
            {
                if (Exterior == PlanetId.SE_ECL_NUT)
                {
                    if (Interior < PlanetId.SE_JUPITER || Interior > PlanetId.SE_PLUTO)
                        return Aspect.ImportantAspectDegrees;
                    else if (Interior < PlanetId.SE_URANUS)
                        return Aspect.EffectiveAspectDegrees;
                    else
                        return Aspect.DegreesOf(7.5);
                }
                else if (Exterior > PlanetId.SE_SATURN)
                {
                    if (Interior >= PlanetId.SE_JUPITER)
                        return Aspect.DegreesOf(7.5);
                    else
                        return Aspect.ImportantAspectDegrees;
                }
                else if (Exterior == PlanetId.SE_SATURN)
                {
                    if (Interior == PlanetId.SE_JUPITER)
                        return Aspect.EffectiveAspectDegrees;
                    else
                        return Aspect.ImportantAspectDegrees;
                }
                else
                {
                    return Aspect.ImportantAspectDegrees;
                }
            }
        }

        public bool Contains(PlanetId id)
        {
            if (Interior == PlanetId.SE_ECL_NUT)
                return true;
            else if (Exterior == PlanetId.SE_ECL_NUT)
                return id == Interior;
            else
                return id == Interior || id == Exterior;
        }

        public bool Contains(PlanetPair pair)
        {
            if (Interior == PlanetId.SE_ECL_NUT)
                return true;
            else if (Exterior == PlanetId.SE_ECL_NUT)
                return pair.Contains(Interior);
            else
                return this.CompareTo(pair) == 0;
        }

        public override string ToString()
        {
            return String.Format("{0}?{1}", Planet.SymbolOf(Interior), Planet.SymbolOf(Exterior));
        }

        #region IComparable<PlanetPair> 成员

        public int CompareTo(PlanetPair other)
        {
            //if (this.Interior == other.Interior && this.Exterior == other.Exterior)
            //    return 0;
            //else if (this.Interior < other.Interior)
            //    return -1;
            //else if (this.Interior == other.Interior && this.Exterior < other.Exterior)
            //    return -1;
            //else
            //    return 1;

            return (int)(this.Interior) + (int)(this.Exterior) - (int)(other.Interior) - (int)(other.Exterior);
        }

        #endregion

        #region IEquatable<PlanetPair> 成员

        public bool Equals(PlanetPair other)
        {
            if (other == null)
                return false;

            return this.Exterior == other.Exterior && this.Interior == other.Interior;
        }

        //public static bool operator ==(PlanetPair lhs, PlanetPair rhs)
        //{
        //    return lhs.Equals(rhs);
        //}

        //public static bool operator !=(PlanetPair lhs, PlanetPair rhs)
        //{
        //    return !lhs.Equals(rhs);
        //}

        #endregion
    }

}

    //public struct PlanetPair : IComparable<PlanetPair>
    //{
    //    #region Static
    //    public static int CodeOf(PlanetId pA, PlanetId pB)
    //    {
    //        if (pA > pB)
    //        {
    //            int temp = (int)pA;
    //            temp = temp << 8;
    //            temp += (int)pB;
    //            return temp;
    //        }
    //        else
    //        {
    //            int temp = (int)pB;
    //            temp = temp << 8;
    //            temp += (int)pA;
    //            return temp;
    //        }
    //        //return (pA <= pB) ?
    //        //    (((int)pA) << 8 + ((int)pB))
    //        //    : (((int)pB) << 8 + ((int)pA));
    //    }
    //    #endregion

    //    private readonly int code;

    //    public int Code
    //    {
    //        get { return code; }
    //    }

    //    public PlanetId InteriorId
    //    {
    //        get { return (PlanetId)(code >> 8); }
    //    }

    //    public PlanetId ExteriorId
    //    {
    //        get { return (PlanetId)(code & 0xFF); }
    //    }

    //    public PlanetPair(PlanetId pA, PlanetId pB)
    //    {
    //        this.code = CodeOf(pA, pB);
    //    }

    //    public PlanetPair(int code)
    //    {
    //        if (code > 65535 || code < 0)
    //            throw new Exception("Out of range: 0 - 65535");

    //        this.code = code;
    //    }

    //    #region IComparable<PlanetPair> 成员

    //    public int CompareTo(PlanetPair other)
    //    {
    //        return this.code - other.code;
    //    }

    //    #endregion
    //}
