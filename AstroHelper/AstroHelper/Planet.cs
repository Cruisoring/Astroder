using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using System.Reflection;
using Color = System.Drawing.Color;

namespace AstroHelper
{
    [Serializable]
    public enum PlanetId : int
    {
        SE_ECL_NUT = -1,            /// Special body number destination compute obliquity and nutation.

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
        SE_CHIRON = 15,         // Order for Chiron
        SE_PHOLUS = 16,
        SE_CERES = 17,
        SE_PALLAS = 18,
        SE_JUNO = 19,
        SE_VESTA = 20,
        SE_INTP_APOG = 21,
        SE_INTP_PERG = 22,

        SE_NPLANETS = 23,
        SE_FICT_OFFSET = 40,

        SE_PRICE = 100,
        SE_NORTHNODE = 101,
        SE_SOUTHNODE = 102,

        SE_AST_OFFSET = 10000
    }

    [Serializable]
    public class Planet : IComparable<Planet>
    {
        #region Constants and static functions/fields
        public const String Undefined = "Unknown";

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
                result = PlanetId.SE_ECL_NUT;
                return false;
            }
        }

        public static Dictionary<PlanetId, Char> Symbols = new Dictionary<PlanetId, Char>()
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
            { PlanetId.SE_CHIRON, '\u26B7' }
        };

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

        public static Dictionary<PlanetId, double> OrbitalPeriods = new Dictionary<PlanetId, double>()
        {
            { PlanetId.SE_SUN, Ephemeris.AverageYearLength },
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
            { PlanetId.SE_CHIRON, 90000}
        };

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

        /// <summary>
        /// http://www.articlesbase.com/astrology-articles/characteristic-of-colours-in-relation-to-planets-in-astrology-206344.html
        /// </summary>
        public static Dictionary<PlanetId, List<Color>> PlanetsColors = new Dictionary<PlanetId, List<Color>>
        {
            { PlanetId.SE_ECL_NUT, new List<Color>{Color.DarkGray, Color.Gray, Color.LightGray} },
            { PlanetId.SE_SUN, new List<Color>{Color.Gold, Color.Chocolate, Color.Orange} },
            { PlanetId.SE_MOON,  new List<Color>{Color.CadetBlue, Color.Ivory, Color.Snow} },
            { PlanetId.SE_MERCURY, new List<Color>{Color.Green, Color.Turquoise, Color.Olive}  },
            { PlanetId.SE_VENUS, new List<Color>{Color.Violet, Color.Purple, Color.Orchid}  },
            { PlanetId.SE_MARS, new List<Color>{Color.Red, Color.Maroon, Color.IndianRed}  },
            { PlanetId.SE_JUPITER, new List<Color>{Color.DarkSalmon, Color.Yellow, Color.SandyBrown}  },
            { PlanetId.SE_SATURN, new List<Color>{Color.Blue, Color.Navy, Color.SkyBlue}  },
            { PlanetId.SE_URANUS, new List<Color>{Color.LightBlue, Color.RoyalBlue, Color.PowderBlue}  },
            { PlanetId.SE_NEPTUNE, new List<Color>{Color.LawnGreen, Color.DarkOliveGreen, Color.YellowGreen} },
            { PlanetId.SE_PLUTO, new List<Color>{Color.Brown, Color.Firebrick, Color.LightSalmon} },
            { PlanetId.SE_NORTHNODE, new List<Color>{Color.CadetBlue, Color.DimGray, Color.Indigo} },
            { PlanetId.SE_CHIRON, new List<Color>{Color.HotPink, Color.DeepPink, Color.Pink} }
        };

        //public static Color ColorOf(PlanetId id)
        //{
        //    return (PlanetsColors.ContainsKey(id)) ? PlanetsColors[id] : Color.Black;
        //}

        public static Color ColorOf(PlanetId id, OrbitInfoType kind)
        {
            if (!PlanetsColors.ContainsKey(id))
                return ColorOf(PlanetId.SE_ECL_NUT, kind);
            else
            {
                List<Color> colorSet = PlanetsColors[id];

                int index = (int)kind;
                if (index < colorSet.Count)
                    return colorSet[index];
                else if (index < colorSet.Count * 2)
                {
                    Color velocityColor = WeightedColor(colorSet[index % 3], 0.8f, false);
                    return velocityColor;
                }
                else
                {
                    Color elseColor = WeightedColor(colorSet[index % 3], 1.0f, true);
                    return elseColor;
                }
            }
        }

        public static Color WeightedColor(Color baseColor, float percentage, bool isTransparent)
        {
            int a = isTransparent ? (int)(255 * percentage) : baseColor.A;
            int r = (int)(baseColor.R * percentage);
            int g = (int)(baseColor.G * percentage);
            int b = (int)(baseColor.B * percentage);
            return Color.FromArgb(a, r, g, b);
        }



        //public static Dictionary<PlanetId, Color> PlanetsColors = new Dictionary<PlanetId, Color>
        //{
        //    { PlanetId.SE_SUN, Color.Pink },
        //    { PlanetId.SE_MOON,  Color.Silver },
        //    { PlanetId.SE_MERCURY, Color.Brown },
        //    { PlanetId.SE_VENUS, Color.Gold },
        //    { PlanetId.SE_MARS, Color.Red },
        //    { PlanetId.SE_JUPITER, Color.Green },
        //    { PlanetId.SE_SATURN, Color.DarkGray },
        //    { PlanetId.SE_URANUS, Color.SkyBlue },
        //    { PlanetId.SE_NEPTUNE, Color.Blue},
        //    { PlanetId.SE_PLUTO, Color.SeaGreen}
        //};

        //public static Color ColorOf(PlanetId id)
        //{
        //    return (PlanetsColors.ContainsKey(id)) ? PlanetsColors[id] : Color.Black;
        //}

        public static Planet Sun = new Planet(PlanetId.SE_SUN, "Sun", DayOfWeek.Sunday);
        public static Planet Moon = new Planet(PlanetId.SE_MOON, "Moon", DayOfWeek.Monday);
        public static Planet Mercury = new Planet(PlanetId.SE_MERCURY, "Mercury", DayOfWeek.Wednesday);
        public static Planet Venus = new Planet(PlanetId.SE_VENUS, "Venus", DayOfWeek.Friday);
        public static Planet Mars = new Planet(PlanetId.SE_MARS, "Mars", DayOfWeek.Tuesday);
        public static Planet Jupiter = new Planet(PlanetId.SE_JUPITER, "Jupiter", DayOfWeek.Thursday);
        public static Planet Saturn = new Planet(PlanetId.SE_SATURN, "Saturn", DayOfWeek.Saturday);
        public static Planet Uranus = new Planet(PlanetId.SE_URANUS, "Uranus");
        public static Planet Neptune = new Planet(PlanetId.SE_NEPTUNE, "Neptune");
        public static Planet Pluto = new Planet(PlanetId.SE_PLUTO, "Pluto");
        public static Planet NorthNode = new Planet(PlanetId.SE_NORTHNODE, "NorthNode");
        public static Planet Chiron = new Planet(PlanetId.SE_CHIRON, "Chiron");
        //public static Planet SouthNode = new Planet(PlanetId.SE_SOUTHNODE, "SouthNode");

        //public static Dictionary<PlanetId, Planet> All = null;

        public static Double AverageSpeedOf(PlanetId star)
        {
            return 360.0 / OrbitalPeriods[star]; 
        }

        public static char SymbolOf(PlanetId id)
        {
            if (Symbols.ContainsKey(id))
                return Symbols[id];
            else
                return '?';
        }

        #endregion

        #region Properties and Variables

        public PlanetId Id { get; private set; }

        public String Name { get; private set; }

        public Char Symbol
        {
            get { return Symbols.ContainsKey(Id) ? Symbols[Id] : Symbols[PlanetId.SE_ECL_NUT]; }
        }

        public DayOfWeek Day { get; private set; }

        public Double AveragePeriod { get; private set; }

        public Double AverageSpeed { get; private set; }

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

        #region Constructors

        private Planet(PlanetId id, string name, DayOfWeek day)
        {
            Id = id;
            Name = name;
            Day = day;
            AveragePeriod = (OrbitalPeriods.ContainsKey(id)) ? OrbitalPeriods[id] : Ephemeris.AverageYearLength;
            AverageSpeed = 360 / AveragePeriod;
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
}
