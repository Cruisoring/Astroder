using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    /// <summary>
    /// Structure stores the GPS coordinates as a double value, and represented in degrees, minutes, and seconds.  
    /// </summary>
    [Serializable]
    public struct Angle : IComparable<Angle>, IFormattable
    {
        /// <summary>Degrees/Radians conversion constant.</summary>
        #region constants definition
        public const double MaximumDegrees = 180.0;
        public const double MinimumDegrees = -180.0;
        public const int MinutesPerDegree = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDegree = 3600;
        public const double DegreesPerMinute = 1.0 / MinutesPerDegree;
        public const double DegreesPerSecond = 1.0 / SecondsPerDegree;

        private const double MaximumTotalSeconds = MaximumDegrees * SecondsPerDegree;
        private const double MinimumTotalSeconds = MinimumDegrees * SecondsPerDegree;

        public const double RadiansPerDegree = Math.PI / 180.0;
        public const double DegreesPerRadian = 180.0 / Math.PI;

        #endregion

        #region Static values for initialization usage
        public static Dictionary<String, String> PredefinedFormats = new Dictionary<String, String>();

        static Angle()
        {
            PredefinedFormats.Add("Default", "");
            PredefinedFormats.Add("Degrees", "D6");
            PredefinedFormats.Add("+DD°MM\'SS.SS\"", "DMS2");
            PredefinedFormats.Add("+DD°MM.MMMM\'", "DM4");      //Default format used by GPS receiver
            PredefinedFormats.Add("+DD.DDDDº", "D4");
            PredefinedFormats.Add("MMM.MMMM\'", "M4");
            PredefinedFormats.Add("SSSS.SSS\"", "S3");
        }

        /// <summary>Empty Angle</summary>
        public static readonly Angle Zero = new Angle(0.0);

        /// <summary>Maximum degree Angle</summary>
        public static readonly Angle Maximum = new Angle(MaximumTotalSeconds);

        /// <summary>Minimum degree Angle</summary>
        public static readonly Angle Minimum = new Angle(MinimumTotalSeconds);

        public static readonly Angle OneSecond = new Angle(0, 0, 1);

        public static readonly Angle OneMinute = new Angle(0, 1);

        public static readonly Angle OneDegree = new Angle(1);
        #endregion

        /// <summary>Angle value in degrees.</summary>
        private double degrees;

        #region Properties

        //public double TotalSeconds
        //{
        //    get { return degrees * SecondsPerDegree; }
        //    set { degrees = normalize(value); }
        //}

        //public double TotalMinutes
        //{
        //    get { return degrees * MinimumDegrees; }
        //}

        /// <summary>
        /// Get/set angle measured in degrees.
        /// </summary>
        public double Degrees
        {
            get { return degrees; }
            set { this.degrees = normalize(value); }
        }

        /// <summary>
        /// Get the absolute value of the angle in degrees.
        /// </summary>
        public double AbsDegrees
        {
            get { return (degrees < 0) ? -degrees : degrees; }
        }

        public double RoundedDegrees
        {
            get
            {
                return Math.Round(degrees, 0);
            }
        }

        //public bool IsRounded
        //{
        //    get
        //    {
        //        return Math.Round(degrees) == degrees;
        //    }
        //}

        //public Angle Round
        //{
        //    get { return new Angle(RoundedDegrees); }
        //}

        //public Angle Half
        //{
        //    get { return new Angle(this.Degrees / 2); }
        //}

        ///// <summary>
        ///// Returns true if the degrees, minutes and seconds refer to a positive value,
        ///// false otherwise.
        ///// </summary>
        //public bool IsPositive
        //{
        //    get { return this.degrees >= 0; }
        //}

        ///// <summary>
        ///// The degrees part of the Angle as a signed Int value
        ///// </summary>
        //public int Hour
        //{
        //    get
        //    {
        //        return (int)(totalSeconds / SecondsPerDegree);
        //    }
        //}

        ///// <summary>
        ///// The remaining part of the Angle besides of the Degree, as minutes in form of a unsigned Double value
        ///// </summary>
        //public double DecimalMinutes
        //{
        //    //get { return (AbsDegrees * MinutesPerDegree) % MinutesPerDegree; }
        //    get
        //    {
        //        return (totalSeconds >= 0) ?
        //            (totalSeconds % SecondsPerDegree) / SecondsPerMinute :
        //            -(totalSeconds % SecondsPerDegree) / SecondsPerMinute;
        //    }
        //}

        ///// <summary>
        ///// The minutes part of the Angle as a unsigned Int value
        ///// </summary>
        //public int Minute
        //{
        //    get
        //    {
        //        //return Math.Floor(DecimalMinutes);
        //        return (totalSeconds >= 0) ?
        //            (int)((totalSeconds % SecondsPerDegree) / SecondsPerMinute) :
        //            (int)(-(totalSeconds % SecondsPerDegree) / SecondsPerMinute);
        //    }
        //}

        ///// <summary>
        ///// The seconds part of the Angle as a unsigned Double value
        ///// </summary>
        //public double Seconds
        //{
        //    get
        //    {
        //        return ((totalSeconds < 0) ? (-totalSeconds) : totalSeconds) % SecondsPerMinute;
        //    }
        //}

        /// <summary>
        /// Get/set angle measured in radians.
        /// </summary>
        public double Radians
        {
            get { return degrees * RadiansPerDegree; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Angle from a degree measurement.
        /// </summary>
        /// <param name="degrees">angle measurement</param>
        public Angle(double degrees)
        {
            this.degrees = normalize(degrees);
        }

        /// <summary>
        /// Construct a new Angle from degrees and minutes.
        /// </summary>
        /// <param name="hours">degree portion of angle measurement</param>
        /// <param name="minutes">minutes portion of angle measurement (0 <= minutes < 60)</param>
        public Angle(int hour, double minutes)
            : this(hour + minutes / MinutesPerDegree)
        { }

        /// <summary>
        /// Construct a new Angle from degrees, minutes, and seconds.
        /// </summary>
        /// <param name="hours">degree portion of angle measurement</param>
        /// <param name="minute">minutes portion of angle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of angle measurement (0 <= seconds < 60)</param>
        public Angle(int hour, int minute, double seconds)
            : this(hour + (Double)minute / MinutesPerDegree + seconds / SecondsPerDegree)
        { }

        #endregion

        #region functions

        /// <summary>
        /// Make sure the angle in degrees within the range of Angle definition
        /// </summary>
        /// <param name="degreeValue">Angle in Degrees</param>
        /// <returns>Nomalized Degrees value in double</returns>
        private static double normalize(double degreeValue)
        {
            if (degreeValue <= MaximumDegrees && degreeValue >= MinimumDegrees) return degreeValue;
            else //Normalize the value to range between MinimumDegrees and MaximumDegrees
            {
                int range = (int)(MaximumDegrees - MinimumDegrees);
                return (degreeValue - MinimumDegrees) % range + MinimumDegrees;
            }
        }

        public static char[] fieldsIndicators = new char[] { '°', '\'', '\"' };
        public static Angle Parse(string angleString)
        {
            Double remains;
            int hour, min;

            try
            {
                string[] subStr = angleString.Split(fieldsIndicators, StringSplitOptions.RemoveEmptyEntries);

                switch (subStr.Length)
                {
                    case 3:
                        hour = int.Parse(subStr[0]);
                        min = int.Parse(subStr[1]);
                        remains = double.Parse(subStr[2]);
                        return new Angle(hour, min, remains);
                    case 2:
                        hour = int.Parse(subStr[0]);
                        remains = double.Parse(subStr[1]);
                        return new Angle(hour, remains);
                    case 1:
                        remains = double.Parse(subStr[0]);
                        return new Angle(remains);
                    default:
                        return Angle.Zero;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return Angle.Zero;
            }
        }

        public static bool TryParse(string angleString, out Angle result)
        {
            result = Parse(angleString);

            if (result == Angle.Zero)
            {
                double value;

                if (!Double.TryParse(angleString, out value) || value != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compare this angle to another angle.
        /// </summary>
        /// <param name="other">other angle to compare to.</param>
        /// <returns>result according to IComparable contract/></returns>
        public int CompareTo(Angle other)
        {
            double dif = (this.totalSeconds - other.totalSeconds) * Magnifier;

            return (int)Math.Round(dif, MidpointRounding.AwayFromZero);
        }


        /// <summary>
        /// Calculate a hash code for the angle.
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return totalSeconds.GetHashCode();
        }

        /// <summary>
        /// Compare this Angle to another Angle for equality.  
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>'true' if angles are equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Angle)) return false;

            return this.CompareTo((Angle)obj) == 0;
        }

        /// <summary>
        /// Get coordinates as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
            return Degrees.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(string format)
        {
            return ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null || format == "")
                return ToString();

            if (char.IsNumber(format, format.Length - 1))
            {
                string lastChar = format.Substring(format.Length - 1, 1);
                switch (format.Substring(0, format.Length - 1))
                {
                    case "D":
                        return String.Format(formatProvider, "{0}{1}°", (IsPositive ? '+' : '-'), AbsDegrees.ToString("F" + lastChar, formatProvider));
                    case "DM":
                        return String.Format(formatProvider, "{0}{1}°{2}\'", (IsPositive ? '+' : '-'),
                            Hour, DecimalMinutes.ToString("F" + lastChar, formatProvider));
                    case "DMS":
                        return String.Format(formatProvider, "{0}{1}°{2}\'{3}\"", (IsPositive ? '+' : '-'),
                            Hour, Minute, Seconds.ToString("F" + lastChar, formatProvider));
                    case "M":
                        return String.Format(formatProvider, "{0}\'", TotalMinutes.ToString("F" + lastChar, formatProvider));
                    case "MS":
                        return String.Format(formatProvider, "{0}\'{1}\"", Minute, Seconds.ToString("F" + lastChar, formatProvider));
                    case "S":
                        return String.Format(formatProvider, "{0}\"", totalSeconds.ToString("F" + lastChar, formatProvider));
                    default:
                        throw new FormatException("Format Exception of Angle.ToString(): " + format);
                }
            }
            else
            {
                if (format == "DMs")
                    return String.Format(formatProvider, "{0}{1:D2}°{2:D2}\'{3,2:F0}\"", IsPositive ? '+' : '-', Hour, Minute, Seconds);
                else
                    return ToString(format + "2", formatProvider);
            }
        }

        #endregion

        #region Operators
        public static Angle GreaterOf(Angle lhs, Angle rhs)
        {
            return lhs.GreaterOf(rhs);
        }

        public static Angle LesserOf(Angle lhs, Angle rhs)
        {
            return lhs.LesserOf(rhs);
        }

        public static Angle MiddleOf(Angle lhs, Angle rhs)
        {
            return new Angle((lhs.Degrees + rhs.Degrees) / 2);
        }

        public static Angle operator +(Angle lhs, Angle rhs)
        {
            //if (lhs.IsEmpty || rhs.IsEmpty) return Angle.Empty;
            return new Angle(lhs.Degrees + rhs.Degrees);
        }

        public static Angle operator +(Angle lhs, Double offsetDegrees)
        {
            //if (lhs.IsEmpty) return Angle.Empty;

            return new Angle(lhs.Degrees + offsetDegrees);
        }

        public static Angle operator -(Angle lhs, Angle rhs)
        {
            //if (lhs.IsEmpty || rhs.IsEmpty) return Angle.Empty;
            return new Angle(lhs.Degrees - rhs.Degrees);
        }

        public static Angle operator -(Angle lhs, Double offsetDegrees)
        {
            //if (lhs.IsEmpty) return Angle.Empty;

            return new Angle(lhs.Degrees - offsetDegrees);
        }

        public static Angle operator ++(Angle x)
        {
            //if (x.IsEmpty) return Angle.Empty;

            return new Angle(x.Degrees + DegreesPerSecond);
        }

        public static Angle operator --(Angle x)
        {
            //if (x.IsEmpty) return Angle.Empty;

            return new Angle(x.Degrees - DegreesPerSecond);
        }

        public static bool operator >(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) > 0;
            //return lhs.totalSeconds > rhs.totalSeconds;
        }

        public static bool operator >=(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
            //return lhs.totalSeconds >= rhs.totalSeconds;
        }

        public static bool operator <(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) < 0;
            //return !(lhs.totalSeconds >= rhs.totalSeconds);
        }

        public static bool operator <=(Angle lhs, Angle rhs)
        {
            return !(lhs > rhs);
        }

        public static bool operator ==(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) == 0;
            //return lhs.totalSeconds == rhs.totalSeconds;
        }

        public static bool operator !=(Angle lhs, Angle rhs)
        {
            return !(lhs == rhs);
        }

        public static Angle FromSeconds(Double seconds)
        {
            return new Angle(seconds / SecondsPerDegree);
        }

        public static Angle FromMinutes(Double minutes)
        {
            return new Angle(minutes / MinutesPerDegree);
        }

        /// <summary>
        /// Implicitly cast a double as an Angle measured in degrees.
        /// </summary>
        /// <param name="degrees">angle in degrees</param>
        /// <returns>double cast as an Angle</returns>
        public static implicit operator Angle(double degrees)
        {
            return new Angle(degrees);
        }
        #endregion
    }
}
