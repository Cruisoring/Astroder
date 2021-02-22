using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public struct Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable
    {
        /// <summary>Degrees/Radians conversion constant.</summary>
        #region constants definition
        //public const double MaximumDegrees = 180.0;
        //public const double MinimumDegrees = -180.0;

        public const int MinutesPerDegree = 4;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDegree = MinutesPerDegree * SecondsPerMinute;

        public const int DegreesPerHour = 360/24;
        public const double DegreesPerMinute = 1.0 / MinutesPerDegree;
        public const double DegreesPerSecond = 1.0 / SecondsPerDegree;

        //private const double MaximumTotalSeconds = MaximumDegrees * SecondsPerDegree;
        //private const double MinimumTotalSeconds = MinimumDegrees * SecondsPerDegree;
        private const double RadiansPerSeconds = Math.PI / (180.0 * SecondsPerDegree);

        ///// <summary>
        ///// The magnifier is used to highlight the difference between two Angles,
        ///// 1 means two angle will be treated as equal, when the difference of their TotalSeconds is less than 1 seconds.
        ///// 100 means two angle will be treated as equal, when the difference of their TotalSeconds is less than 0.01 seconds.
        ///// </summary>
        //public static readonly int Magnifier = 1;

        #endregion

        #region Static values for initialization usage
        public static string DefaultFormat = "DM4";

        public static Dictionary<String, String> PredefinedFormats = new Dictionary<String, String>();

        static Angle()
        {
            PredefinedFormats.Add("Decimal", "D6");
            PredefinedFormats.Add("HH:MM:SS", "Time");
            PredefinedFormats.Add("+DD°MM\'SS.SS\"", "DMS2");
            PredefinedFormats.Add("+DD°MM.MMMM\'", "DM4");      //Default format used by GPS receiver
            PredefinedFormats.Add("+DD.DDDDº", "D4");
            PredefinedFormats.Add("MM.MMMM\'", "M4");
            PredefinedFormats.Add("SS.SSS\"", "S3");
        }

        /// <summary>Empty Angle</summary>
        public static readonly Angle Zero = new Angle(0.0);

        ///// <summary>Maximum degree Angle</summary>
        //public static readonly Angle Maximum = new Angle(MaximumDegrees);

        ///// <summary>Minimum degree Angle</summary>
        //public static readonly Angle Minimum = new Angle(MinimumDegrees);
        #endregion

        #region Static Functions
        ///// <summary>
        ///// Make sure the theAngle in seconds within the range of Angle definition
        ///// </summary>
        ///// <param name="degs">Angle in TotalSeconds</param>
        ///// <returns>Nomalized TotoalSeconds value in double</returns>
        //private static double normalize(double degrees)
        //{
        //    return degrees.Normalize(-180, 180);
        //    //if (degrees <= MaximumDegrees && degrees > MinimumDegrees)
        //    //    return degrees;
        //    //else
        //    //{
        //    //    //Normalize the value to range between MinimumDegrees and MaximumDegrees
        //    //    int range = (int)(MaximumDegrees - MinimumDegrees);
        //    //    int round = 0;

        //    //    if (degrees > MaximumDegrees)
        //    //    {
        //    //        round = (int)((degrees - MinimumDegrees) / range);
        //    //        return degrees - round * range;
        //    //    }
        //    //    else
        //    //    {
        //    //        round = (int)((MaximumDegrees - degrees) / range);
        //    //        return degrees + round * range;
        //    //    }
        //    //}
        //}

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

        public static string InAngles(double var)
        {
            Angle theAngle = new Angle(var);
            return theAngle.ToString();
        }
        #endregion

        /// <summary>Angle value in degrees.</summary>
        private double degrees;

        #region Properties

        /// <summary>
        /// Get/set theAngle measured in degs.
        /// </summary>
        public double Degrees
        {
            get { return degrees; }
            set { this.degrees = value.Normalize(-180, 180); }
        }

        /// <summary>
        /// Get the absolute value of the theAngle in degs.
        /// </summary>
        public double AbsDegrees
        {
            get { return (degrees < 0) ? -degrees : degrees ; }
        }

        public double RoundedDegrees
        {
            get
            {
                return Math.Round(degrees);
            }
        }

        public bool IsRounded
        {
            get
            {
                return Math.Round(degrees) == degrees;
            }
        }

        public bool IsEmpty 
        { 
            get { return this.degrees == 0.0; } 
        }

        public Angle Round
        {
            get { return new Angle(RoundedDegrees); }
        }

        /// <summary>
        /// Returns true if the degs, minutes and seconds refer to a positive value,
        /// false otherwise.
        /// </summary>
        public bool IsPositive
        {
            get { return this.degrees >= 0; }
        }

        /// <summary>
        /// The degs part of the Angle as a signed Int value
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)(degrees/DegreesPerHour);
            }
        }

        /// <summary>
        /// The remaining part of the Angle besides of the Degree, as minutes in form of a unsigned Double value
        /// </summary>
        public double DecimalMinutes
        {
            get
            {
                return (Math.Abs(degrees) % DegreesPerHour) * MinutesPerDegree;
                //if (degs >= 0)
                //{
                //    //return (degs * MinutesPerDegree) % MinutesPerDegree;
                //    return (degs % DegreesPerHour) * 60;
                //}
                //else
                //{
                //    return ((-degs) % DegreesPerHour) * 60;
                //    //return (-degs * MinutesPerDegree) % MinutesPerDegree;
                //}
                //return (degs >= 0) ?
                //    (degs % 1.0) * MinutesPerDegree :
                //    -(degs % 1.0) * MinutesPerDegree;
            }
        }

        /// <summary>
        /// The minutes part of the Angle as a unsigned Int value
        /// </summary>
        public int Minute
        {
            get
            {
                return (int)(Math.Abs(degrees) % DegreesPerHour) * MinutesPerDegree;
                //return (int)((Math.Abs(degs) % DegreesPerHour) * 60);
            }
        }

        /// <summary>
        /// The seconds part of the Angle as a unsigned Double value
        /// </summary>
        public double Seconds
        {
            get
            {
                return (Math.Abs(degrees) * SecondsPerDegree) % SecondsPerMinute;
                //return ((degs < 0) ? (-degs) : degs) % SecondsPerMinute;
            }
        }

        /// <summary>
        /// Get/set theAngle measured in radians.
        /// </summary>
        public double Radians
        {
            get { return degrees * RadiansPerSeconds; }
        }

        public Aspect NearAspect
        {
            get { return Aspect.NearAspect(this); }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Angle from a degree measurement.
        /// </summary>
        /// <param name="degs">theAngle measurement</param>
        public Angle(double degrees)
        {
            this.degrees = degrees.Normalize(-180, 180);
        }

        /// <summary>
        /// Construct a new Angle from degs and minutes.
        /// </summary>
        /// <param name="hour">degree portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        public Angle(int hour, double minutes)
            : this(hour * DegreesPerHour + minutes / MinutesPerDegree)
        { }

        /// <summary>
        /// Construct a new Angle from degs, minutes, and seconds.
        /// </summary>
        /// <param name="hour">degree portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        public Angle(int hour, int minute, double seconds)
            : this(hour * DegreesPerHour + (Double)minute / MinutesPerDegree + seconds / SecondsPerDegree)
        { }

        #endregion

        #region functions

        /// <summary>
        /// Compare this theAngle to another theAngle.
        /// </summary>
        /// <param name="other">other theAngle to compare to.</param>
        /// <returns>result according to IComparable contract/></returns>
        public int CompareTo(Angle other)
        {
            return this.degrees.CompareTo(other.degrees);
        }

        /// <returns>An <strong>Angle</strong> containing the largest value.</returns>
        /// <summary>Returns the object with the largest value.</summary>
        /// <param name="value">An <strong>Angle</strong> object to compare to the current instance.</param>
        public Angle GreaterOf(Angle value)
        {
            if (degrees > value.Degrees)
                return this;
            else
                return value;
        }

        /// <summary>Returns the object with the smallest value.</summary>
        /// <returns>The <strong>Angle</strong> containing the smallest value.</returns>
        /// <param name="value">An <strong>Angle</strong> object to compare to the current instance.</param>
        public Angle LesserOf(Angle value)
        {
            if (degrees < value.Degrees)
                return this;
            else
                return value;
        }


        /// <summary>
        /// Calculate a hash code for the theAngle.
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return degrees.GetHashCode();
        }

        /// <summary>
        /// Compare this Angle to another Angle for equality.  
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>'true' if angle are equal</returns>
        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Angle)) return false;

        //    return this.degs == (Angle)obj;
        //}

        /// <summary>
        /// Get coordinates as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
            return this.ToString(DefaultFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString();

            if (char.IsNumber(format, format.Length - 1))
            {
                string lastChar = format.Substring(format.Length - 1, 1);
                switch (format.Substring(0, format.Length - 1))
                {
                    case "D":
                        return String.Format(formatProvider, "{0}{1}°", (IsPositive ? '+' : '-'), 
                            AbsDegrees.ToString("F" + lastChar, formatProvider));
                    case "DM":
                        return String.Format(formatProvider, "{0}{1}°{2}\'", (IsPositive ? '+' : '-'),
                            (int)AbsDegrees, DecimalMinutes.ToString("F" + lastChar, formatProvider));
                    case "DMS":
                        return String.Format(formatProvider, "{0}{1}°{2}\'{3}\"", (IsPositive ? '+' : '-'),
                            (int)AbsDegrees, Minute, Seconds.ToString("F" + lastChar, formatProvider));
                    case "M":
                        return String.Format(formatProvider, "{0}\'", DecimalMinutes.ToString("F" + lastChar, formatProvider));
                    case "MS":
                        return String.Format(formatProvider, "{0}\'{1}\"", Minute, Seconds.ToString("F" + lastChar, formatProvider));
                    case "S":
                        return String.Format(formatProvider, "{0}\"", Seconds.ToString("F" + lastChar, formatProvider));
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
            //return lhs.degs > rhs.degs;
        }

        public static bool operator >=(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
            //return lhs.degs >= rhs.degs;
        }

        public static bool operator <(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) < 0;
            //return !(lhs.degs >= rhs.degs);
        }

        public static bool operator <=(Angle lhs, Angle rhs)
        {
            return !(lhs > rhs);
        }

        public static bool operator ==(Angle lhs, Angle rhs)
        {
            return lhs.CompareTo(rhs) == 0;
            //return lhs.degs == rhs.degs;
        }

        public static bool operator !=(Angle lhs, Angle rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object o)
        {
            if (o is Angle)
            {
                return Equals((Angle)o);
            }
            else
                return false;
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
        /// Implicitly cast a double as an Angle measured in degs.
        /// </summary>
        /// <param name="degs">theAngle in degs</param>
        /// <returns>double cast as an Angle</returns>
        public static implicit operator Angle(double degrees)
        {
            return new Angle(degrees);
        }
        #endregion

        #region IEquatable<Angle> 成员

        public bool Equals(Angle other)
        {
            return this.degrees == other.degrees;
        }

        #endregion
    }
}
