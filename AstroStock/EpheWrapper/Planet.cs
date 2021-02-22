using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EpheWrapper
{
    public class Planet
    {
        #region Constants and static functions/fields
        public const String Undefined = "Undefined";

        public static Planet Sun = new Planet(PlanetId.SE_SUN, "Sun", '\u2609', DayOfWeek.Sunday);
        public static Planet Moon = new Planet(PlanetId.SE_MOON, "Moon", '\u263D', DayOfWeek.Monday);
        public static Planet Mercury = new Planet(PlanetId.SE_MERCURY, "Mercury", '\u263F', DayOfWeek.Wednesday);
        public static Planet Venus = new Planet(PlanetId.SE_VENUS, "Venus", '\u2640', DayOfWeek.Friday);
        public static Planet Mars = new Planet(PlanetId.SE_MARS, "Mars", '\u2642', DayOfWeek.Tuesday);
        public static Planet Jupiter = new Planet(PlanetId.SE_JUPITER, "Jupiter", '\u2643', DayOfWeek.Thursday);
        public static Planet Saturn = new Planet(PlanetId.SE_SATURN, "Saturn", '\u2644', DayOfWeek.Saturday);
        public static Planet Uranus = new Planet(PlanetId.SE_URANUS, "Uranus", '\u2645');
        public static Planet Neptune = new Planet(PlanetId.SE_NEPTUNE, "Neptune", '\u2646');
        public static Planet Pluto = new Planet(PlanetId.SE_PLUTO, "Pluto", '\u2647');
        public static Planet NorthNode = new Planet(PlanetId.SE_NORTHNODE, "NorthNode", '\u260A');
        //public static Planet SouthNode = new Planet(PlanetId.SE_SOUTHNODE, "SouthNode", '\u260B');

        public static Dictionary<PlanetId, Planet> All;

        static Planet()
        {
            All = new Dictionary<PlanetId, Planet>();

            Type planetType = typeof(Planet);
            FieldInfo[] fields = planetType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                Planet star = fi.GetValue(null) as Planet;
                if (star != null)
                {
                    All.Add(star.Id, star);
                }
            }

            //all.Add(PlanetId.SE_SUN, Sun);
            //all.Add(PlanetId.SE_MOON, Moon);
            //all.Add(PlanetId.SE_MERCURY, Mercury);
            //all.Add(PlanetId.SE_VENUS, Venus);
            //all.Add(PlanetId.SE_MARS, Mars);
            //all.Add(PlanetId.SE_JUPITER, Jupiter);
            //all.Add(PlanetId.SE_SATURN, Saturn);
            //all.Add(PlanetId.SE_URANUS, Uranus);
            //all.Add(PlanetId.SE_NEPTUNE, Neptune);
            //all.Add(PlanetId.SE_PLUTO, Pluto);
        }

        public static Planet PlanetOf(PlanetId id)
        {
            if (All.ContainsKey(id))
                return All[id];
            else
                return null;
        }

        public static string NameOf(PlanetId id)
        {
            if (All.ContainsKey(id))
                return All[id].Name;
            else
                return Undefined;
        }

        public static char SymbolOf(PlanetId id)
        {
            if (All.ContainsKey(id))
                return All[id].Symbol;
            else
                return '\uFF1F';
        }

        #endregion

        #region Properties and Variables

        private readonly PlanetId id;

        public PlanetId Id
        {
            get { return id; }
        }

        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        private readonly char symbol;

        public char Symbol
        {
            get { return symbol; }
        } 
        
        private readonly DayOfWeek day;

        public DayOfWeek Day
        {
            get { return day; }
        }

        private readonly List<AstroSign> rules;

        public List<AstroSign> Rules
        {
            get { return rules; }
        }

        private readonly List<AstroSign> exalts;

        public List<AstroSign> Exalts
        {
            get { return exalts; }
        }

        private readonly List<AstroSign> detriments;

        public List<AstroSign> Detriments
        {
            get { return detriments; }
        }

        private readonly List<AstroSign> falls;

        public List<AstroSign> Falls
        {
            get { return falls; }
        }

        #endregion

        #region Constructors

        private Planet(PlanetId id, string name, char symbol, DayOfWeek day)
        {
            this.id = id;
            this.name = name;
            this.symbol = symbol;
            this.day = day;
            this.rules = new List<AstroSign>();
            this.exalts = new List<AstroSign>();
            this.detriments = new List<AstroSign>();
            this.falls = new List<AstroSign>();
        }

        private Planet(PlanetId id, string name, char symbol) : this(id, name, symbol, DayOfWeek.Sunday)
        { }
        #endregion

        public override string ToString()
        {
            return this.name;
        }
    }

    public enum PlanetId : int
    {
        SE_ECL_NUT = -1,            /// Special body number to compute obliquity and nutation.

        SE_SUN = 0,                 /// Index for Sun
        SE_MOON = 1,         // Index for Moon
        SE_MERCURY = 2,         // Index for Mercury
        SE_VENUS = 3,         // Index for Venus
        SE_MARS = 4,         // Index for Mars
        SE_JUPITER = 5,         // Index for Jupiter
        SE_SATURN = 6,         // Index for Saturn
        SE_URANUS = 7,         // Index for Uranus
        SE_NEPTUNE = 8,         // Index for Neptune
        SE_PLUTO = 9,         // Index for Pluto

        SE_MEAN_NODE = 10,         // Index for standard node (mean)
        SE_TRUE_NODE = 11,         // oscillating node (true)
        SE_MEAN_APOG = 12,      //
        SE_OSCU_APOG = 13,

        SE_EARTH = 14,
        SE_CHIRON = 15,         // Index for Chiron
        SE_PHOLUS = 16,
        SE_CERES = 17,
        SE_PALLAS = 18,
        SE_JUNO = 19,
        SE_VESTA = 20,
        SE_INTP_APOG = 21,
        SE_INTP_PERG = 22,

        SE_NPLANETS = 23,
        SE_FICT_OFFSET = 40,

        SE_NORTHNODE = 101,
        SE_SOUTHNODE = 102,

        SE_AST_OFFSET = 10000
    }

    public struct PlanetPair : IComparable<PlanetPair>
    {
        #region Static
        public static int CodeOf(PlanetId pA, PlanetId pB)
        {
            if (pA <= pB)
            {
                int temp = (int)pA;
                temp = temp << 8;
                temp += (int)pB;
                return temp;
            }
            else
            {
                int temp = (int)pB;
                temp = temp << 8;
                temp += (int)pA;
                return temp;
            }
            //return (pA <= pB) ?
            //    (((int)pA) << 8 + ((int)pB))
            //    : (((int)pB) << 8 + ((int)pA));
        }
        #endregion

        private readonly int code;

        public int Code
        {
            get { return code; }
        }

        public PlanetId InteriorId
        {
            get { return (PlanetId)(code >> 8); }
        }

        public PlanetId ExteriorId
        {
            get { return (PlanetId)(code & 0xFF); }
        }

        public PlanetPair(PlanetId pA, PlanetId pB)
        {
            this.code = CodeOf(pA, pB);
        }

        public PlanetPair(int code)
        {
            if (code > 65535 || code < 0)
                throw new Exception("Out of range: 0 - 65535");

            this.code = code;
        }

        #region IComparable<PlanetPair> 成员

        public int CompareTo(PlanetPair other)
        {
            return this.code - other.code;
        }

        #endregion
    }
}
