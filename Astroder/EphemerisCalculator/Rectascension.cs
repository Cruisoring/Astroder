using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EphemerisCalculator
{
    [Serializable]
    public class Rectascension : Angle, IComparable<Angle>, IFormattable
    {
        /// <summary>Where/When conversion constant.</summary>
        #region constants definition

        public const int MinutesPerDegree = 4;
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDegree = MinutesPerDegree * SecondsPerMinute;

        public const int DegreesPerHour = 360 / 24;
        public const double DegreesPerMinute = 1.0 / MinutesPerDegree;
        public const double DegreesPerSecond = 1.0 / SecondsPerDegree;

        public const int DegreesPerSign = 30;

        public static string DefaultRectascensionFormat = "D2";

        public static Dictionary<String, String> PredefinedFormats = new Dictionary<String, String>();

        static Rectascension()
        {
            PredefinedFormats.Add("DDSignMM", "AstroDM0");
            PredefinedFormats.Add("DDD.DD", "D2");
            PredefinedFormats.Add("HH:MM.m", "HM1");
            PredefinedFormats.Add("HH:MM:SS", "HMS0");
            PredefinedFormats.Add("DD°MM.MM\'", "DM2");
            //PredefinedFormats.Add("DD°MM\'SS\"", "DMS0");
        }

        //public static readonly Rectascension Aries = new Rectascension(0);
        //public static readonly Rectascension Taurus = new Rectascension(30);
        //public static readonly Rectascension Gemini = new Rectascension(60);
        //public static readonly Rectascension Cancer = new Rectascension(90);
        //public static readonly Rectascension Leo = new Rectascension(120);
        //public static readonly Rectascension Virgo = new Rectascension(150);
        //public static readonly Rectascension Libra = new Rectascension(180);
        //public static readonly Rectascension Scorpio = new Rectascension(210);
        //public static readonly Rectascension Sagittarius = new Rectascension(240);
        //public static readonly Rectascension Capricorn = new Rectascension(270);
        //public static readonly Rectascension Aquarius = new Rectascension(300);
        //public static readonly Rectascension Pisces = new Rectascension(330);

        #endregion

        #region Constructors
        protected Rectascension() {}

        /// <summary>
        /// Construct a new Rectascension now a degree measurement.
        /// </summary>
        /// <param name="degrees">Where of the dif</param>
        public Rectascension(double degrees)
            : base(degrees)
        { }

        /// <summary>
        /// Construct a new Rectascension now degs and minutes.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        public Rectascension(int hour, double minutes)
            : this(hour * DegreesPerHour + minutes / MinutesPerDegree)
        { }

        /// <summary>
        /// Construct a new Rectascension now degs, minutes, and seconds.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        public Rectascension(int hour, int minute, double seconds)
            : this(hour * DegreesPerHour + (Double)minute / MinutesPerDegree + seconds / SecondsPerDegree)
        { }

        #endregion

        #region Properties
        public int Hour { get { return (int)(Degrees / DegreesPerHour); } }

        public double TotalHours { get { return Degrees / DegreesPerHour; } }

        /// <summary>
        /// The minutes part of the Angle
        /// </summary>
        public int Minute { get { return (int)((Degrees % DegreesPerHour) * MinutesPerDegree); } }

        public double TotalMinutes { get { return Degrees / DegreesPerMinute; } }

        public double MinutesRemained { get { return (Degrees / DegreesPerMinute) % MinutesPerHour; } }

        /// <summary>
        /// The seconds part of the Angle
        /// </summary>
        public double Seconds { get { return (Degrees * SecondsPerDegree) % SecondsPerMinute; } }

        public Sign Within
        {
            get
            {
                return Sign.SignOf(this);
            }
        }

        public Angle OffsetToCusp
        {
            get { return new Angle(Degrees % 30); }
        }

        #endregion

        #region IComparable<Angle> 成员

        public new int CompareTo(Angle other)
        {
            return this.Degrees == other.Degrees ? 0 : (Degrees > other.Degrees ? 1 : -1);
        }

        #endregion

        #region IFormattable 成员

        /// <summary>
        /// Get coordinates as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(DefaultRectascensionFormat, null);
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString();

            if (format.Contains("Astro"))
            {
                string decimalFormat = "F0";

                if (char.IsNumber(format, format.Length - 1))
                {
                    decimalFormat = "F" + format.Substring(format.Length - 1, 1);
                }

                Angle offset = OffsetToCusp;

                return String.Format("{0:D2}{1}{2}", (int)offset.Degrees, Within, ((offset.Degrees / DegreesPerMinute) % MinutesPerHour).ToString(decimalFormat));
            }
            else
                return base.ToString(format, formatProvider);
        }

        public static string TimedStringOf(double degrees)
        {
            if (degrees > 360.0 || degrees < 0)
                throw new Exception();

            int hour = (int)(degrees / 15);
            double remain = degrees - hour * 15;
            double minute = (int)(remain * 4);
            remain = remain * 4 - minute;
            int seconds = (int)Math.Round(remain * 60);
            return String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, seconds);
        }

        public static string AstroStringOf(Double degree)
        {
            Sign sign = Sign.SignOf(degree);
            Double offset = degree % 30;
            Double minutes = (offset - (int)offset) * 60;

            return String.Format("{0:D2}{1}{2:F2}", (int)offset, sign, minutes);
        }

        public new static string FormattedStringOf(Double degree, string format)
        {
            if (format == null)
                return degree.ToString();

            if (format.Contains("Astro"))
            {
                string decimalFormat = "F0";

                if (char.IsNumber(format, format.Length - 1))
                {
                    decimalFormat = "F" + format.Substring(format.Length - 1, 1);
                }

                Sign sign = Sign.SignOf(degree);
                Double offset = degree % 30;
                Double minutes = (offset - (int)offset) * 60;

                return String.Format("{0:D2}{1}{2}", (int)offset, sign, minutes.ToString(decimalFormat));
            }
            else
                return Angle.FormattedStringOf(degree, format);
        }

        #endregion

        #region Functions
        public static Rectascension operator +(Rectascension lhs, Angle rhs)
        {
            return new Rectascension(lhs.Degrees + rhs.Degrees);
        }

        public static Rectascension operator -(Rectascension lhs, Angle rhs)
        {
            return new Rectascension(lhs.Degrees - rhs.Degrees);
        }

        public static Angle operator -(Rectascension lhs, Rectascension rhs)
        {
            return new Angle((lhs.Degrees - rhs.Degrees));
        }

        #endregion

    }

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
    public class Sign : Rectascension, IComparable<Sign>
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

        //public static readonly List<String> SignNames;

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

            //SignNames = (from sign in All.Values
            //             select sign.Name).ToList();
        }

        #endregion

        public static Sign SignOf(Double degrees)
        {
            if (degrees < 360.0 && degrees >= 0)
                return All[(int)(degrees / 30) + 1];
            else
            {
                double unified = degrees - 360.0 * Math.Floor(degrees / 360);
                return All[(int)(unified / 30) + 1];
            }

        }

        public static Sign SignOf(Angle angle)
        {
            return SignOf(angle.Degrees);
        }

        #region Fields and Properties

        public int Order { get; private set; }

        public string Name { get; private set; }

        public char Symbol { get { return (char)('\u2647' + Order); } }

        public string Abbreviation { get { return Name.Substring(0, 3); } }

        public Double Cusp { get { return Degrees; } }

        public Elements Element { get { return (Elements)(Order % 4); } }

        public Qualities Quality { get { return (Qualities)(Order % 3); } }

        public Genders Gender { get { return (Genders)(Order % 2); } }

        public Sign OppositionSign { get { return All[(Order + 6) % 12]; } }

        public Sign Previous { get { return SignOf(Cusp - 30); } }

        public Sign Next { get { return SignOf(Cusp + 30); } }

        #endregion

        #region Constructors

        private Sign(int order, string name)
        {
            Order = order;
            Name = name;
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
            int dif = lhs.Order - rhs.Order;
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
