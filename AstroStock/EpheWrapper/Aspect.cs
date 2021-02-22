using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EpheWrapper
{
    public class Aspect //: IComparable<Aspect>, IEquatable<Aspect>
    {
        #region Constants and static functions/values
        private static bool negelectMinors = true;

        public static bool NegelectMinors
        {
            get { return Aspect.negelectMinors; }
            set { Aspect.negelectMinors = value; Concerned = negelectMinors ? Major : all; }
        }

        public static bool StrictAspect = true;

        //public const double MinimumDegrees = 0.0;
        //public const double MaximumDegrees = 360.0;

        public const double DefaultMaxOrb = 1.0;
        //public const string DefaultName = "Unknown";

        public static Aspect Conjuction = new Aspect(AspectType.Conjuction, 0.0, '\u260C', 8.0);
        public static Aspect Sextile = new Aspect(AspectType.Sextile, 60.0, '\u26B9', 4.0);
        public static Aspect Square = new Aspect(AspectType.Square, 90.0, '\u25A1', 4.0);
        public static Aspect Trine = new Aspect(AspectType.Trine, 120.0, '\u25B3', 4.0);
        public static Aspect Opposition = new Aspect(AspectType.Opposition, 180.0, '\u260D', 4.0);

        public static Aspect Quintile = new Aspect(AspectType.Quintile, 72.0, '\u0051', 2.0);
        public static Aspect Biquintile = new Aspect(AspectType.Biquintile, 144.0, 'B');
        public static Aspect Quincunx = new Aspect(AspectType.Quincunx, 150.0, '\u26BB');
        public static Aspect SemiSquare = new Aspect(AspectType.SemiSquare, 45.0, '\u2220');
        public static Aspect SemiSextile = new Aspect(AspectType.SemiSextile, 30.0, '\u26BA');
        public static Aspect Sesquiquadrate = new Aspect(AspectType.Sesquiquadrate, 135.0, '\u26BC');

        public static SortedDictionary<double, Aspect> Major;
        private static SortedDictionary<double, Aspect> all;
        public static SortedDictionary<double, Aspect> Concerned;

        private static Dictionary<AspectType, Aspect> typeDict;

        static Aspect()
        {
            Major = new SortedDictionary<double, Aspect>();
            Major.Add(Conjuction.Degrees, Conjuction);
            Major.Add(Sextile.Degrees, Sextile);
            Major.Add(Square.Degrees, Square);
            Major.Add(Trine.Degrees, Trine);
            Major.Add(Opposition.Degrees, Opposition);

            all = new SortedDictionary<double, Aspect>();
            typeDict = new Dictionary<AspectType, Aspect>();
            Type aspectType = typeof(Aspect);
            FieldInfo[] fields = aspectType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                Aspect asp = fi.GetValue(null) as Aspect;
                if (asp != null)
                {
                    all.Add(asp.Degrees, asp);
                    typeDict.Add(asp.Type, asp);
                }
            }

            Concerned = negelectMinors ? Major : all;
        }
        
        //public static double Normalize(double degrees)
        //{
        //    return degrees.Normalize(180);
        //    //double degs = ((degrees < 0) ? -degrees : degrees) % 360.0;

        //    //return (degs <= 180.0) ? degs : 360.0 - degs;
        //}

        public static AspectType AspectTypeOf(Angle angle)
        {
            double degs = (angle.Degrees).Normalize(180);

            foreach (KeyValuePair<double, Aspect> kvp in Concerned )
            {
                double orbs = (StrictAspect) ? Math.Min(kvp.Value.MaxOrb, DefaultMaxOrb) : kvp.Value.MaxOrb;
                double aspDeg = kvp.Key;
                if (degs == aspDeg)
                    return kvp.Value.Type;
                else if (degs < aspDeg)
                {
                    if (aspDeg - degs <= orbs)
                        return kvp.Value.Type;
                    else
                        return AspectType.None;
                }
                else if (degs - aspDeg <= orbs)
                    return kvp.Value.Type;
            }
            return AspectType.None;
        }

        public static AspectType AspectTypeOf(double degrees)
        {
            if (Concerned.ContainsKey(degrees))
                return Concerned[degrees].type;
            else
                return AspectType.None;
        }

        public static Aspect AspectOf(AspectType type)
        {
            if (typeDict.ContainsKey(type))
                return typeDict[type];
            else
                return null;
        }

        public static char SymbolOf(AspectType type)
        {
            if (typeDict.ContainsKey(type))
                return typeDict[type].Symbol;
            else
                return '\uFF1F';
        }

        public static double DegreesOf(AspectType type)
        {
            if (typeDict.ContainsKey(type))
                return typeDict[type].Degrees;
            else
                return -1;
        }

        //public static double AspectDegreesOf(Double angle)
        //{
        //    double temp = (angle + 360.0) % 360;
        //    if (temp > 180.0)
        //        temp = 360.0 - temp;

        //    double delta = 180;
        //    double nearAspect = 0;

        //    foreach (double deg in Concerned.Keys)
        //    {
        //        if (delta > Math.Abs(deg - temp))
        //        {
        //            delta = Math.Abs(deg - temp);
        //            nearAspect = deg;
        //        }
        //    }
        //    return nearAspect;
        //}

        public static Aspect NearAspect(Angle angle)
        {
            AspectType type = AspectTypeOf(angle);

            return AspectOf(type);
        }
        #endregion

        #region Fields
        /// <summary>
        /// Aspect Type
        /// </summary>
        private readonly AspectType type;

        public AspectType Type
        {
            get { return type; }
        }

        //public string Name
        //{
        //    get { return type.ToString(); }
        //}

        /// <summary>Aspect value in degs.</summary>
        private readonly double degrees;

        public double Degrees
        {
            get { return degrees; }
        }

        private readonly char symbol;

        public char Symbol
        {
            get { return symbol; }
        } 

        /// <summary>
        /// The difference between the exact aspect and the actual aspect in degs.
        /// </summary>
        private double maxOrb;

        public double MaxOrb
        {
            get { return maxOrb; }
            set { maxOrb = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Aspect from with defined type, degree and maxOrb.
        /// </summary>
        /// <param name="degs">theAngle measurement</param>
        private Aspect(AspectType type, double degrees, char symbol, double maxOrb)
        {
            this.type = type;
            this.degrees = degrees;
            this.symbol = symbol;
            //this.degrees = (degrees != 360.0) ? Normalize(degrees) : 360.0;
            this.maxOrb = maxOrb;
        }

        private Aspect(AspectType type, double degrees, char symbol) : this(type, degrees, symbol, DefaultMaxOrb)
        { }

        //public Aspect(double degs) : this(DefaultName, degs, DefaultMaxOrb)
        //{ }

        //public Aspect(double degs, double maxOrb) : this(DefaultName, degs, maxOrb)
        //{ }

        #endregion

        #region Functions
        public double OrbOf(Angle angle)
        {
            return angle.Degrees - this.degrees;
        }

        public bool IsExact(Angle angle)
        {
            double degrees = angle.Degrees;
            return (degrees >= this.degrees ? degrees - this.degrees : this.degrees - degrees) < DefaultMaxOrb;
        }

        public bool IsAbout(Angle angle)
        {
            double degrees = angle.Degrees;
            return (degrees >= this.degrees ? degrees - this.degrees : this.degrees - degrees) < maxOrb;
        }

        public override string ToString()
        {
            return string.Format("{0}({1:F1})", this.type, this.degrees);
        }
        #endregion

        //#region IComparable<Aspect> 成员

        //public int CompareTo(Aspect other)
        //{
        //    if (other == null)
        //        return -1;
        //    else
        //        return this.degs.CompareTo(other.degs);
        //}

        //#endregion

        //#region IEquatable<Aspect> 成员

        //public override int GetHashCode()
        //{
        //    return this.degs.GetHashCode();
        //}

        //public bool Equals(Aspect other)
        //{
        //    return Math.Abs(this.degs - other.Degrees) <= this.maxOrb;
        //}

        //#endregion
    }

    public enum AspectType
    {
        None = 0,
        Conjuction,
        Sextile,
        Square,
        Trine,
        Opposition,
        Quintile,
        Biquintile,
        Quincunx,
        SemiSquare,
        SemiSextile,
        Sesquiquadrate
    }
}
