using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public struct Longitude : IComparable<Longitude> //, IFormattable
    {
        /// <summary>Degrees/Radians conversion constant.</summary>
        #region constants definition
        //public const double MinimumDegrees = 0.0;
        //public const double MaximumDegrees = 360.0;
        public const double NullDegrees = - 360.0;
        //private const double spanOfLongitude = MaximumDegrees - MinimumDegrees;

        public const int HoursPerCircle = 24;
        public const int DegreesPerHour = 360/24;
        public const int MinutesPerDegree = 4;
        public const int SecondsPerDegree = 240;
        public const int SecondsPerMinute = 60;

        public const int DegreesPerSign = 30;

        public static Longitude Null = new Longitude(NullDegrees);

        #endregion

        #region Static values & functions
        public static Dictionary<String, String> PredefinedFormats ;

        static Longitude()
        { 
            PredefinedFormats = new Dictionary<String, String>();
            PredefinedFormats.Add("Default", "");
            PredefinedFormats.Add("HH:MM:SS", "Time");
            PredefinedFormats.Add("Degrees", "D6");
            PredefinedFormats.Add("+DD°MM\'SS.SS\"", "DMS2");
            PredefinedFormats.Add("+DD°MM.MMMM\'", "DM4");      //Default format used by GPS receiver
            PredefinedFormats.Add("+DD.DDDDº", "D4");
            PredefinedFormats.Add("MMM.MMMM\'", "M4");
            PredefinedFormats.Add("SSSS.SSS\"", "S3");
        }

        ///// <summary>
        ///// Make sure the theAngle in degs within the range of Longitude definition
        ///// </summary>
        ///// <param name="original">Longitude in Degrees</param>
        ///// <returns>Normalized Degrees value in double</returns>
        //public static double Normalize(double original)
        //{
        //    //if (original < MaximumDegrees && original >= MinimumDegrees) 
        //    //    return original;
        //    //else //Normalize the value to range between MinimumDegrees and MaximumDegrees
        //    //    return (original + 3600.0) % 360.0;
        //    return original.Normalize();
        //}

        private static string getSign(string input)
        {
            int start = -1;
            int end = -1;

            for (int i = 0; i < input.Length; i ++)
            {
                if (start == -1)
                {
                    if (char.IsLetter(input[i]))
                        start = i;
                }
                else if (end == -1)
                {
                    if (!char.IsLetter(input[i]))
                        end = i;
                }
                else
                    break;
            }

            if (start != -1 && end != -1)
            {
                return input.Substring(start, end - start);
            }
            else
                return "";
        }

        public static char[] fieldsIndicators = new char[] { '°', '\'', '\"' };
        public static char[] timeIndicator = new char[] { ':' };
        public static Longitude Parse(string angleString)
        {
            Double remains;
            int hour, min;

            try
            {
                String signName = getSign(angleString);
                if (signName != null && signName != "")
                {
                    if (signName.Length == 3 && AstroSign.Abbreviations.ContainsKey(signName.ToUpper()))
                    {
                        int signPos = angleString.IndexOf(signName);
                        int minute = Int32.Parse(angleString.Substring(0, signPos)) ;
                        double seconds = double.Parse(angleString.Substring(signPos + 3));
                        return new Longitude(signPos * 2, minute, seconds);
                    }
                    else
                    {
                        return Longitude.Null;
                    }
                }
                else
                {
                    if (!angleString.Contains(':'))
                    {
                        string[] subStr = angleString.Split(fieldsIndicators, StringSplitOptions.RemoveEmptyEntries);

                        switch (subStr.Length)
                        {
                            case 3:
                                hour = int.Parse(subStr[0]);
                                min = int.Parse(subStr[1]);
                                remains = double.Parse(subStr[2]);
                                return new Longitude(hour, min, remains);
                            case 2:
                                hour = int.Parse(subStr[0]);
                                remains = double.Parse(subStr[1]);
                                return new Longitude(hour, remains);
                            case 1:
                                remains = double.Parse(subStr[0]);
                                return new Longitude(remains);
                            default:
                                return Longitude.Null;
                        }
                    }
                    else
                    {
                        string[] subStr = angleString.Split(timeIndicator, StringSplitOptions.RemoveEmptyEntries);
                        switch (subStr.Length)
                        {
                            case 3:
                                hour = int.Parse(subStr[0]);
                                min = int.Parse(subStr[1]);
                                remains = double.Parse(subStr[2]);
                                return new Longitude(hour, min, remains);
                            case 2:
                                hour = int.Parse(subStr[0]);
                                remains = double.Parse(subStr[1]);
                                return new Longitude((hour + remains / 60.0) * DegreesPerHour);
                            case 1:
                                remains = double.Parse(subStr[0]);
                                return new Longitude(remains * DegreesPerHour);
                            default:
                                return Longitude.Null;
                        }

                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return Longitude.Null;
            }
        }

        public static bool TryParse(string angleString, out Longitude result)
        {
            result = Parse(angleString);

            return result.degrees != NullDegrees;
        }

        //public static Longitude FromTime(int hours, int minutes, double seconds)
        //{
        //    double totalHours = hours + (minutes + seconds / 60.0) / 60.0;
        //    return new Longitude(DegreesPerHour * totalHours);
        //}

        //public static Longitude FromSeconds(Double seconds)
        //{
        //    return new Longitude(seconds / SecondsPerDegree);
        //}

        //public static Longitude FromMinutes(Double minutes)
        //{
        //    return new Longitude(minutes / MinutesPerDegree);
        //}

        //public static Longitude FromHours(Double hours)
        //{
        //    return new Longitude(hours * DegreesPerHour);
        //}

        /// <summary>
        /// Implicitly cast a double as an Longitude measured in degs.
        /// </summary>
        /// <param name="degs">theAngle in degs</param>
        /// <returns>double cast as an Longitude</returns>
        public static implicit operator Longitude(double degrees)
        {
            return new Longitude(degrees);
        }

        #endregion

        /// <summary>Longitude value in degs.</summary>
        private double degrees;

        #region Constructors
        /// <summary>
        /// Construct a new Longitude from a degree measurement.
        /// </summary>
        /// <param name="degs">theAngle measurement</param>
        public Longitude(double degrees)
        {
            this.degrees =  (degrees == NullDegrees) ? NullDegrees : degrees.Normalize();
        }

        /// <summary>
        /// Construct a new Longitude from degs and minutes.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        public Longitude(int hour, double minutes)
        {
            double totalHours = hour + minutes / 60.0;
            this.degrees = (DegreesPerHour * totalHours).Normalize();
        }

        /// <summary>
        /// Construct a new Longitude from degs, minutes, and seconds.
        /// </summary>
        /// <param name="hours">degree portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        public Longitude(int hour, int minute, double seconds)
        {
            double totalHours = hour + (minute + seconds / 60.0) / 60.0;
            this.degrees = (DegreesPerHour * totalHours).Normalize();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Get/set longitude measured in degs.
        /// </summary>
        public double Degrees
        {
            get { return degrees; }
            set { this.degrees = value.Normalize(); }
        }

        public AstroSign Sign
        {
            get {
                int index = (int)(degrees / DegreesPerSign) + 1;
                return AstroSign.All[index];
            }
        }

        public double ToCusp
        {
            get   { return this.degrees % DegreesPerSign;  }
        }

        /// <summary>
        /// The hour part of the Longitude as a Int value
        /// </summary>
        public int Hour
        {
            get { return (int)(degrees / DegreesPerHour); }
        }

        /// <summary>
        /// The remaining part of the Longitude besides of the Degree, as minutes in form of a unsigned Double value
        /// </summary>
        //public double DecimalMinutes
        //{
        //    get { return (degs % DegreesPerHour) * MinutesPerDegree; }
        //}

        /// <summary>
        /// The minutes part of the Longitude as Int value
        /// </summary>
        public int Minute
        {
            get { 
                double rem = (degrees*60) % 60;
                return (int)rem; 
            }
        }

        /// <summary>
        /// The seconds part of the Longitude as a Double value
        /// </summary>
        public double Seconds
        {
            get {
                double rem = (degrees* 3600)% 60;
                return rem; 
            }
        }

        public bool IsNull
        {
            get { return this.degrees == NullDegrees; }
        }

        #endregion

        #region IComparable<Longitude> 成员

        public int CompareTo(Longitude other)
        {
            return (this.Sign == other.Sign) ? this.degrees.CompareTo(other.degrees) : this.Sign.CompareTo(other.Sign);
        }

        #endregion

        #region IFormattable 成员

        /// <summary>
        /// Get coordinates as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            double remains = ToCusp;

            return String.Format("{0:D2}{1}{2:D2}\'{3:D2}\"", (int)remains, this.Sign.Abbrev, Minute, (int)Seconds);
        }

        //public string ToString(string format)
        //{
        //    return ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        //}

        //public string ToString(string format, IFormatProvider formatProvider)
        //{
        //    if (format == null || format == "")
        //        return ToString();

        //    if (char.IsNumber(format, format.Length - 1))
        //    {
        //        string lastChar = format.Substring(format.Length - 1, 1);
        //        switch (format.Substring(0, format.Length - 1))
        //        {
        //            case "D":
        //                return String.Format(formatProvider, "{0}{1}°", (IsPositive ? '+' : '-'), AbsDegrees.ToString("F" + lastChar, formatProvider));
        //            case "DM":
        //                return String.Format(formatProvider, "{0}{1}°{2}\'", (IsPositive ? '+' : '-'),
        //                    Hour, DecimalMinutes.ToString("F" + lastChar, formatProvider));
        //            case "DMS":
        //                return String.Format(formatProvider, "{0}{1}°{2}\'{3}\"", (IsPositive ? '+' : '-'),
        //                    Hour, Minute, Seconds.ToString("F" + lastChar, formatProvider));
        //            case "M":
        //                return String.Format(formatProvider, "{0}\'", TotalMinutes.ToString("F" + lastChar, formatProvider));
        //            case "MS":
        //                return String.Format(formatProvider, "{0}\'{1}\"", Minute, Seconds.ToString("F" + lastChar, formatProvider));
        //            case "S":
        //                return String.Format(formatProvider, "{0}\"", degs.ToString("F" + lastChar, formatProvider));
        //            default:
        //                throw new FormatException("Format Exception of Longitude.ToString(): " + format);
        //        }
        //    }
        //    else
        //    {
        //        if (format == "HH:MM:SS")
        //        {
        //            return String.Format(formatProvider, "{0:D2}:{1:D2}:{2,2:F0}\"", Hour, Minute, Seconds);
        //        }
        //        else if (format == "DMs")
        //            return String.Format(formatProvider, "{0}{1:D2}°{2:D2}\'{3,2:F0}\"", IsPositive ? '+' : '-', Hour, Minute, Seconds);
        //        else
        //            return ToString(format + "2", formatProvider);
        //    }
        //}

        #endregion

        #region Other functions
        public static Longitude operator +(Longitude lhs, Angle rhs)
        {
            return new Longitude(lhs.Degrees + rhs.Degrees);
        }

        public static Longitude operator -(Longitude lhs, Angle rhs)
        {
            return new Longitude(lhs.Degrees - rhs.Degrees);
        }

        public static Angle operator -(Longitude lhs, Longitude rhs)
        {
            return new Angle((lhs.Degrees - rhs.Degrees).Normalize(180));
        }

        #endregion


        //#region Operators

        //public static Longitude MiddleOf(Longitude lhs, Longitude rhs)
        //{
        //    return new Longitude((lhs.Degrees + rhs.Degrees) / 2);
        //}

        //public static Longitude operator +(Longitude lhs, Double offsetDegrees)
        //{
        //    //if (lhs.IsEmpty) return Longitude.Empty;

        //    return new Longitude(lhs.Degrees + offsetDegrees);
        //}

        //public static Longitude operator -(Longitude lhs, Longitude rhs)
        //{
        //    //if (lhs.IsEmpty || rhs.IsEmpty) return Longitude.Empty;
        //    return new Longitude(lhs.Degrees - rhs.Degrees);
        //}

        //public static Longitude operator -(Longitude lhs, Double offsetDegrees)
        //{
        //    //if (lhs.IsEmpty) return Longitude.Empty;

        //    return new Longitude(lhs.Degrees - offsetDegrees);
        //}

        //public static Longitude operator ++(Longitude x)
        //{
        //    //if (x.IsEmpty) return Longitude.Empty;

        //    return new Longitude(x.Degrees + DegreesPerSecond);
        //}

        //public static Longitude operator --(Longitude x)
        //{
        //    //if (x.IsEmpty) return Longitude.Empty;

        //    return new Longitude(x.Degrees - DegreesPerSecond);
        //}

        //public static bool operator >(Longitude lhs, Longitude rhs)
        //{
        //    return lhs.CompareTo(rhs) > 0;
        //    //return lhs.degs > rhs.degs;
        //}

        //public static bool operator >=(Longitude lhs, Longitude rhs)
        //{
        //    return lhs.CompareTo(rhs) >= 0;
        //    //return lhs.degs >= rhs.degs;
        //}

        //public static bool operator <(Longitude lhs, Longitude rhs)
        //{
        //    return lhs.CompareTo(rhs) < 0;
        //    //return !(lhs.degs >= rhs.degs);
        //}

        //public static bool operator <=(Longitude lhs, Longitude rhs)
        //{
        //    return !(lhs > rhs);
        //}

        //public static bool operator ==(Longitude lhs, Longitude rhs)
        //{
        //    return lhs.CompareTo(rhs) == 0;
        //    //return lhs.degs == rhs.degs;
        //}

        //public static bool operator !=(Longitude lhs, Longitude rhs)
        //{
        //    return !(lhs == rhs);
        //}

        //#endregion


        //#region Functions and properties

        ///// <summary>
        ///// Calculate a hash code for the theAngle.
        ///// </summary>
        ///// <returns>hash code</returns>
        //public override int GetHashCode()
        //{
        //    return degs.GetHashCode();
        //}

        ///// <summary>
        ///// Compare this Longitude to another Longitude for equality.  
        ///// </summary>
        ///// <param name="obj">object to compare to</param>
        ///// <returns>'true' if angle are equal</returns>
        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Longitude)) return false;

        //    return this.CompareTo((Longitude)obj) == 0;
        //}


        //#endregion
    }
}
