using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using System.Reflection;

namespace AstroHelper
{
    public enum Elements
    {
        Fire = 1,
        Earth = 2,
        Air = 3,
        Water = 0
    }

    public enum Qualities
    {
        Cardinal = 1,
        Fixed = 2,
        Mutable = 0
    }

    public enum Genders
    {
        Male = 1,
        Female = 0
    }

    /// <summary>
    /// Design of all in astrology.
    /// </summary>
    /// <see cref="http://www.astrologycom.com/dignities.html"/>
    /// <see cref="http://x.sheup.com/xingzuo/30/xingzuo12783.htm"/>
    [SerializableAttribute]
    public class Sign : Angle, IComparable<Sign>
    {
        #region Constants defintions
        public static readonly Sign Aries = new Sign(1, "Aries");
        public static readonly Sign Taurus = new Sign(2, "Taurus");
        public static readonly Sign Gemini = new Sign(3, "Gemini");
        public static readonly Sign Cancer = new Sign(4, "Cancer");
        public static readonly Sign Leo = new Sign(5, "Leo");
        public static readonly Sign Virgo = new Sign(6, "Virgo");
        public static readonly Sign Libra = new Sign(7, "Libra");
        public static readonly Sign Scorpio = new Sign(8, "Scorpio");
        public static readonly Sign Sagittarius = new Sign(9, "Sagittarius");
        public static readonly Sign Capricorn = new Sign(10, "Capricorn");
        public static readonly Sign Aquarius = new Sign(11, "Aquarius");
        public static readonly Sign Pisces = new Sign(12, "Pisces");

        //public static readonly Sign Aries =
        //    new Sign(1, Properties.Resources.Aries, "Ari", Elements.Fire, Qualities.Cardinal,
        //        PlanetId.SE_MARS, PlanetId.SE_SUN, PlanetId.SE_VENUS, PlanetId.SE_SATURN);
        //public static readonly Sign Taurus =
        //    new Sign(2, Properties.Resources.Taurus, "Tau", Elements.Earth, Qualities.Fixed,
        //        PlanetId.SE_VENUS, PlanetId.SE_MOON, PlanetId.SE_MARS, PlanetId.SE_ECL_NUT);
        //public static readonly Sign Gemini =
        //    new Sign(3, Properties.Resources.Gemini, "Gem", Elements.Air, Qualities.Mutable,
        //        PlanetId.SE_MERCURY, PlanetId.SE_ECL_NUT, PlanetId.SE_JUPITER, PlanetId.SE_ECL_NUT);
        //public static readonly Sign Cancer =
        //    new Sign(4, Properties.Resources.Cancer, "Can", Elements.Water, Qualities.Cardinal,
        //        PlanetId.SE_MOON, PlanetId.SE_JUPITER, PlanetId.SE_SATURN, PlanetId.SE_MARS);
        //public static readonly Sign Leo =
        //    new Sign(5, Properties.Resources.Leo, "Leo", Elements.Fire, Qualities.Fixed,
        //        PlanetId.SE_SUN, PlanetId.SE_ECL_NUT, PlanetId.SE_SATURN, PlanetId.SE_ECL_NUT);
        //public static readonly Sign Virgo =
        //    new Sign(6, Properties.Resources.Virgo, "Vir", Elements.Earth, Qualities.Mutable,
        //        PlanetId.SE_MERCURY, PlanetId.SE_MERCURY, PlanetId.SE_JUPITER, PlanetId.SE_VENUS);
        //public static readonly Sign Libra =
        //    new Sign(7, Properties.Resources.Libra, "Lib", Elements.Air, Qualities.Cardinal,
        //        PlanetId.SE_VENUS, PlanetId.SE_SATURN, PlanetId.SE_MARS, PlanetId.SE_SUN);
        //public static readonly Sign Scorpio =
        //    new Sign(8, Properties.Resources.Scorpio, "Sco", Elements.Water, Qualities.Fixed,
        //        PlanetId.SE_PLUTO, PlanetId.SE_ECL_NUT, PlanetId.SE_VENUS, PlanetId.SE_MOON);
        //public static readonly Sign Sagittarius =
        //    new Sign(9, Properties.Resources.Sagittarius, "Sag", Elements.Fire, Qualities.Mutable,
        //        PlanetId.SE_JUPITER, PlanetId.SE_ECL_NUT, PlanetId.SE_MERCURY, PlanetId.SE_ECL_NUT);
        //public static readonly Sign Capricorn =
        //    new Sign(10, Properties.Resources.Capricorn, "Cap", Elements.Earth, Qualities.Cardinal,
        //        PlanetId.SE_SATURN, PlanetId.SE_MARS, PlanetId.SE_MOON, PlanetId.SE_JUPITER);
        //public static readonly Sign Aquarius =
        //    new Sign(11, Properties.Resources.Aquarius, "Aqu", Elements.Air, Qualities.Fixed,
        //        PlanetId.SE_URANUS, PlanetId.SE_ECL_NUT, PlanetId.SE_SUN, PlanetId.SE_ECL_NUT);
        //public static readonly Sign Pisces =
        //    new Sign(12, Properties.Resources.Pisces, "Pis", Elements.Water, Qualities.Mutable,
        //        PlanetId.SE_NEPTUNE, PlanetId.SE_VENUS, PlanetId.SE_VENUS, PlanetId.SE_MERCURY);

        public static readonly SortedDictionary<int, Sign> All;

        public static readonly Dictionary<string, int> Abbreviations;

        static Sign()
        {
            All = new SortedDictionary<int, Sign>();
            Abbreviations = new Dictionary<string, int>();

            Type signType = typeof(Sign);
            FieldInfo[] fields = signType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                Sign sign = fi.GetValue(null) as Sign;
                if (sign != null)
                {
                    All.Add(sign.Order, sign);
                    Abbreviations.Add(sign.Abbreviation, sign.Order);
                }
            }
        }

        #endregion

        public static Sign SignOf(Double degrees) 
        {
            if (degrees < 360.0 && degrees >= 0)
                return All[(int)(degrees/30) + 1];
            else
            {
                double unified = degrees - 360.0 * Math.Floor(degrees / 360);
                return All[(int)(unified/30) + 1];
            }

        }

        public static Sign SignOf(Angle angle)
        {
            return SignOf(angle.Degrees);
        }

        //public static Double DegreesFromCusp(Double degrees)
        //{
        //    double unified = degrees - 360.0 * Math.Zero(degrees / 360);
        //    Sign sign = All[(int)(unified / 30)];
        //    return (unified - sign.Position);
        //}

        //public static Angle AngleFromCusp(Angle distanceDynamic)
        //{
        //    return new Angle(DegreesFromCusp(distanceDynamic.Position));
        //}

        //public static String SignedDescription(Double degrees)
        //{
        //    double unified = degrees - 360.0 * Math.Zero(degrees / 360);
        //    Sign sign = All[(int)(unified / 30)];
        //    Angle offset = new Angle(unified - sign.Cusp.Position);
        //    return String.Format("{0}{1}{2}", (int)offset.Position, sign, Math.Round(offset.MinutesRemained));
        //}

        //public static String SignedDescription(Angle distanceDynamic)
        //{
        //    return SignedDescription(distanceDynamic.Position);
        //}

        #region Fields and Properties

        public int Order { get; private set; }

        public string Name { get; private set; }

        public string Abbreviation { get; private set; }

        public Double Cusp { get { return Degrees; } }

        public Elements Element { get; private set; }

        public Qualities Quality { get; private set; }

        public Genders Gender { get; private set; }

        public Sign OppositionSign { get { return All[(Order + 6) % 12]; } }

        public Sign Previous { get { return SignOf(Cusp - 30); } }

        public Sign Next { get { return SignOf(Cusp + 30); } }

        #endregion

        #region Constructors

        private Sign(int order, string name, string abbrev, Double cusp, Elements elem, Qualities quality, Genders gender)
        {
            Order = order;
            Name = name;
            Abbreviation = abbrev;
            Degrees = cusp;
            Element = elem;
            Quality = quality;
            Gender = gender;
        }

        private Sign(int order, string name)
        {
            Order = order;
            Name = name;
            Abbreviation = name.Substring(0, 3);
            Degrees = (order-1)* 30;
            Element = (Elements)(order % 4);
            Quality = (Qualities)(order % 3);
            Gender = (Genders)(order % 2);
        }

        #endregion

        #region IComparable<Sign> 成员

        public int CompareTo(Sign other)
        {
            return this.Order.CompareTo(other.Order);
        }

        #endregion

        public override string ToString()
        {
            return Abbreviation;
        }

        #region Operators
        public static int operator -(Sign lhs, Sign rhs)
        {
            int dif =  lhs.Order - rhs.Order;
            return (dif > 6) ? 12 - dif : dif;
        }

        public static Sign operator +(Sign lhs, int rhs)
        {
            return SignOf(lhs.Cusp + rhs * 30);
        }

        public static Sign operator -(Sign lhs, int rhs)
        {
            return SignOf(lhs.Cusp - rhs * 30);
        }

        #endregion
    }

}
