using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;

namespace AstroHelper
{
    [Serializable]
    public class Rectascension : Angle, IComparable<Angle> , IFormattable
    {
        /// <summary>Position/Time conversion constant.</summary>
        #region constants definition

        public const int DegreesPerSign = 30;

        public static string DefaultRectascensionFormat = "D1";

        public new static Dictionary<String, String> PredefinedFormats = new Dictionary<String, String>();

        static Rectascension()
        {
            PredefinedFormats.Add("DDSignMM", "AstroDM0");
            PredefinedFormats.Add("DDD.DD", "D2");
            PredefinedFormats.Add("HH:MM.m", "HM1");
            PredefinedFormats.Add("HH:MM:SS", "HMS0");
            //PredefinedFormats.Add("DD°MM\'SS\"", "DMS0");
            //PredefinedFormats.Add("DD°MM.MM\'", "DM2");
        }


        public static readonly Rectascension Aries = new Rectascension(0);
        public static readonly Rectascension Taurus = new Rectascension(30);
        public static readonly Rectascension Gemini = new Rectascension(60);
        public static readonly Rectascension Cancer = new Rectascension(90);
        public static readonly Rectascension Leo = new Rectascension(120);
        public static readonly Rectascension Virgo = new Rectascension(150);
        public static readonly Rectascension Libra = new Rectascension(180);
        public static readonly Rectascension Scorpio = new Rectascension(210);
        public static readonly Rectascension Sagittarius = new Rectascension(240);
        public static readonly Rectascension Capricorn = new Rectascension(270);
        public static readonly Rectascension Aquarius = new Rectascension(300);
        public static readonly Rectascension Pisces = new Rectascension(330);

        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Rectascension now a degree measurement.
        /// </summary>
        /// <param name="degrees">Position of the rectascension</param>
        public Rectascension(double degrees) : base (degrees)
        { }

        /// <summary>
        /// Construct a new Rectascension now degs and minutes.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        public Rectascension(int hour, double minutes) : base (hour, minutes)
        { }

        /// <summary>
        /// Construct a new Rectascension now degs, minutes, and seconds.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        public Rectascension(int hour, int minute, double seconds) : base(hour, minute, seconds)
        { }

        #endregion

        #region Properties
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

                return String.Format("{0:D2}{1}{2}", (int)offset.Degrees, Within, offset.MinutesRemained.ToString(decimalFormat));
            }
            else
                return base.ToString(format, formatProvider);
        }

        public new static string AngleFormatOf(Double degree, string format)
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
                return Angle.AngleFormatOf(degree, format);
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

}
