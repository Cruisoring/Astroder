using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberHelper
{
    using NumberHelper.DoubleHelper;

    public enum AngleTypes
    {
        Full,
        Acute,
        Right,
        Obtuse,
        Straight,
        Reflex
    }

    //public interface IDegreeable
    //{
    //    Double Degrees { get; }
    //}

    public interface IAngleable<T>
    {
        Angle AngleOf(T obj);
    }

    [Serializable]
    public class Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable //, IDegreeable
    {
        #region constants definition
        public const int DefaultAngleDecimals = 1;

        public const int MinutesPerDegree = 4;
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDegree = MinutesPerDegree * SecondsPerMinute;

        public const int DegreesPerHour = 360 / 24;
        public const double DegreesPerMinute = 1.0 / MinutesPerDegree;
        public const double DegreesPerSecond = 1.0 / SecondsPerDegree;

        public const double RadiansPerDegree = Math.PI / 180.0;
        public const double DegreesPerRadians = 180.0 / Math.PI;

        public static Angle Zero = new Angle(0);
        public static Angle SemiSextile = new Angle(30);
        public static Angle SemiSquare = new Angle(45);
        public static Angle Sextile = new Angle(60);
        public static Angle Quintile = new Angle(72);
        public static Angle Square = new Angle(90);
        public static Angle Trine = new Angle(120);
        public static Angle Sesquiquadrate = new Angle(135);
        public static Angle Biquintile = new Angle(144);
        public static Angle Quincunx = new Angle(150);
        public static Angle Opposition = new Angle(180);
        public static Angle Full = new Angle(360, true);

        #endregion

        #region Static values for initialization usage
        public static string DefaultFormat = "D1";

        public static Dictionary<Double, AspectType> References = new Dictionary<Double, AspectType>()
        {
            { 0, AspectType.Conjuction },
            { 30, AspectType.SemiSextile },
            { 45, AspectType.SemiSquare },
            { 60, AspectType.Sextile },
            { 72, AspectType.Quintile },
            { 90, AspectType.Square },
            { 120, AspectType.Trine },
            { 135, AspectType.Sesquiquadrate },
            { 144, AspectType.Biquintile },
            { 150, AspectType.Quincunx },
            { 180, AspectType.Opposition },
            { 210, AspectType.Quincunx },
            { 216, AspectType.Biquintile},
            { 225, AspectType.Sesquiquadrate },
            { 240, AspectType.Trine },
            { 270, AspectType.Square },
            { 188, AspectType.Quintile },
            { 300, AspectType.Sextile },
            { 315, AspectType.SemiSquare },
            { 330, AspectType.SemiSextile },
            { 360, AspectType.Conjuction }
        };

        public static Dictionary<String, String> PredefinedFormats = new Dictionary<String, String>();

        static Angle()
        {
            PredefinedFormats.Add("DDD.DD", "D2");
            PredefinedFormats.Add("HH:MM.m", "HM1");
            PredefinedFormats.Add("HH:MM:SS", "HMS0");
            //PredefinedFormats.Add("DD°MM\'SS\"", "DMS0");
            //PredefinedFormats.Add("DD°MM.MM\'", "DM2");
        }

        public static Double BeelineOf(Double startDegree, Double endDegree)
        {
            Double result = (endDegree - startDegree).Normalize();

            return result > 180 ? result - 360 : result;
        }

        public static Double BeelineOf(Angle start, Angle end)
        {
            return BeelineOf(start.Degrees, end.Degrees);
        }
        #endregion

        #region Static Functions

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
                        return null;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string DescriptionOf(double var)
        {
            Angle theAngle = new Angle(var);
            return theAngle.ToString();
        }
        #endregion

        public double Degrees { get; protected set; }

        #region Constructors
        protected Angle() {}

        public Angle(double degrees, bool noNormalize)
        {
            Degrees = noNormalize ? degrees : degrees.Normalize();
        }

        public Angle(double degrees)
        {
            Degrees = degrees.Normalize();
        }

        /// <summary>
        /// Construct a new Angle from hour and minutes.
        /// </summary>
        /// <param name="hour">hour portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        protected Angle(int hour, double minutes)
            : this(hour * DegreesPerHour + minutes / MinutesPerDegree)
        { }

        /// <summary>
        /// Construct a new Angle from degs, minutes, and seconds.
        /// </summary>
        /// <param name="hour">hour portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        protected Angle(int hour, int minute, double seconds)
            : this(hour * DegreesPerHour + (Double)minute / MinutesPerDegree + seconds / SecondsPerDegree)
        { }
        #endregion

        #region Properties
        public int Hour { get { return (int)(Degrees / DegreesPerHour); } }

        public double TotalHours { get { return Degrees / DegreesPerHour; } }

        /// <summary>
        /// The minutes part of the Angle
        /// </summary>
        public int Minute { get {  return (int)((Degrees % DegreesPerHour) * MinutesPerDegree); } }

        public double TotalMinutes { get { return Degrees / DegreesPerMinute; } }

        public double MinutesRemained { get { return (Degrees / DegreesPerMinute) % MinutesPerHour; } }

        /// <summary>
        /// The seconds part of the Angle
        /// </summary>
        public double Seconds { get { return (Degrees * SecondsPerDegree) % SecondsPerMinute; } }

        /// <summary>
        /// Get the Angle in radians.
        /// </summary>
        public double Radians
        {
            get { return Degrees * RadiansPerDegree; }
        }

        //public Aspects NearbyAspect
        //{
        //    get { return Aspects.AspectNearby(this); }
        //}

        ///// <summary>
        ///// The Aspect formed when the orb is no greater than DefaultMaxOrb
        ///// </summary>
        //public Aspects ExactAspect
        //{
        //    get { return Aspects.ExactAspectOf(this); }
        //}

        ///// <summary>
        ///// The Aspect formed when the orb is no greater than the adapted PermissibleOrb
        ///// </summary>
        //public Aspects AroundAspect
        //{
        //    get { return Aspects.AroundAspectOf(this); }
        //}

        ///// <summary>
        /// Always returns the Orb to the most nearby aspect
        /// </summary>
        //public double Orb
        //{
        //    get {
        //        return Degrees - NearbyAspect.Degrees;
        //        //double orb = Degrees - NearbyAspect.Degrees;
        //        //return orb < 180 ? orb : orb - 360;
        //    }
        //}

        public AngleTypes AngleType
        {
            get 
            {
                if (Degrees % 360 == 0)
                    return AngleTypes.Full;
                if (Degrees < 90)
                    return AngleTypes.Acute;
                else if (Degrees == 90 || Degrees == 270)
                    return AngleTypes.Right;
                else if (Degrees < 180)
                    return AngleTypes.Obtuse;
                else if (Degrees == 180)
                    return AngleTypes.Straight;
                else if (Degrees < 360)
                    return AngleTypes.Reflex;
                else
                    throw new ArgumentException("Unexpected AngleType when Degree=" + Degrees.ToString());
            }
        }

        /// <summary>
        /// True if the angle is within first or second quadrants (when the Degrees is between 0 and 180);
        /// False if it is within the third or fourth quadrants (The angle is between 180 and 360).
        /// </summary>
        public bool IsPositive
        {
            get { return !(Degrees > 180 && Degrees < 360); }
        }

        /// <summary>
        /// The angle whose angle is 90 - this.Degrees.
        /// </summary>
        public Angle Complementary
        {
            get { return Square - this;}
        }

        /// <summary>
        /// The angle whose angle is 180 - this.Degrees.
        /// </summary>
        public Angle Supplementary
        {
            get {return Opposition - this;}
        }

        /// <summary>
        /// The angle whose angle is 360 - this.Degrees.
        /// </summary>
        public Angle Explementary
        {
            get { return Full - this; }
        }

        /// <summary>
        /// The acute version determined by repeatedly subtracting or adding 180 angle, and subtracting by 180 if needed, until
        /// the angle is between 0 and 90.
        /// </summary>
        public Angle Reference
        {
            get 
            {
                switch(AngleType)
                {
                    case AngleTypes.Acute: return this;
                    case AngleTypes.Right: return Angle.Square;
                    case AngleTypes.Full: return Angle.Zero;
                    case AngleTypes.Obtuse: return this.Supplementary;
                    case AngleTypes.Reflex: return this.Explementary;
                    case AngleTypes.Straight: return Angle.Opposition;
                    default:
                        throw new ArgumentException("Unexpected AngleType when Degree=" + Degrees.ToString());
                }
            }
        }

        #endregion

        #region IComparable<Angle> 成员

        public int CompareTo(Angle other)
        {
            Double difference = (Degrees - other.Degrees).Normalize();
            if (difference == 0)
                return 0;
            else if (difference <= 180.0)
                return 1;
            else
                return -1;
        }

        #endregion

        #region IEquatable<Angle> 成员

        public bool Equals(Angle other)
        {
            return this.Degrees == other.Degrees;
        }

        #endregion

        #region IFormattable 成员

        public override string ToString()
        {
            return ToString(DefaultFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString();

            string decimalFormat = "F0";
            string formatType = format ;

            if (char.IsNumber(format, format.Length - 1))
            {
                decimalFormat = "F" + format.Substring(format.Length - 1, 1);
                formatType = format.Substring(0, format.Length - 1).ToUpper();
            }

            switch (formatType)
            {
                case "D":
                    return String.Format(formatProvider, "<{0}º>", 
                        Degrees.ToString(decimalFormat, formatProvider));
                case "HM":
                    return String.Format(formatProvider, "<{0}:{1}>", 
                        Hour, MinutesRemained.ToString(decimalFormat, formatProvider));
                case "HMS":
                    return String.Format(formatProvider, "<{0}:{1}:{2}>", 
                        Hour, Minute, Seconds.ToString(decimalFormat, formatProvider));
                //case "ASTRO":
                //    return String.Format(formatProvider, "{0}{1}{2}",
                //        DecimalMinutes.ToString(decimalFormat, formatProvider));
                default:
                    return ToString(DefaultFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static string AngleFormatOf(Double degree, string format)
        {
            if (format == null || format == "")
                return String.Format("{0:F2}º", degree);
            else
                return new Angle(degree).ToString(format, null);
        }

        #endregion

        #region Operators & Functions
        /// <summary>
        /// Implicitly cast a double to an Angle
        /// </summary>
        /// <param name="angle">The angle</param>
        /// <returns>Angle with the normalized angle</returns>
        public static implicit operator Angle(double angle)
        {
            return new Angle(angle);
        }

        //public static explicit operator Aspect(Angle angle)
        //{
        //    return Aspect.ExactAspectOf(angle.Degrees);
        //}

        public static Angle operator +(Angle lhs, Angle rhs)
        {
            return new Angle((lhs.Degrees + rhs.Degrees));
        }

        public static Angle operator -(Angle lhs, Angle rhs)
        {
            return new Angle(lhs.Degrees - rhs.Degrees);
        }

        public static Angle operator *(Angle lhs, int multiple)
        {
            return new Angle(lhs.Degrees * multiple);
        }

        public static Angle FarMiddleOf(Angle lhs, Angle rhs)
        {
            double m1 = (lhs.Degrees / 2 + rhs.Degrees / 2).Normalize();

            return (Math.Abs((m1-lhs.Degrees).Normalize()) >= 90.0) ? new Angle(m1) : new Angle(m1 + 180);
        }

        public static Angle NearMiddleOf(Angle lhs, Angle rhs)
        {
            double m1 = (lhs.Degrees / 2 + rhs.Degrees / 2).Normalize();

            return (Math.Abs((m1 - lhs.Degrees).Normalize()) <= 90.0) ? new Angle(m1) : new Angle(m1 + 180);
        }

        public static Angle MirrorOf(Angle lhs, Angle rhs)
        {
            return new Angle(lhs.Degrees * 2 - rhs.Degrees);
        }

        /// <summary>
        /// Supposing this angle identify an isosceles triangle, whose vertex is on center,
        /// startDegree leg is on 0 angle, endDegree on Degrees, and the base length is 1.
        /// Calculate the angle of a point on the base, when its distance to the intersect on 0 angle is provided.
        /// </summary>
        /// <param name="offset">distance to the intersect on 0 angle, may be greater than 1 and less than 0</param>
        /// <returns></returns>
        public Angle AngleByOffset(double offset)
        {
            if (this.Degrees >= 180)
                throw new Exception("Function not supported when Degrees is greater than 180!");

            if (offset == 0)
                return Angle.Zero;
            else if (offset == 1)
                return this;
            else if (offset == 0.5)
                return new Angle(Degrees / 2);

            double leg = 0.5 / Math.Sin(Degrees / 2 * RadiansPerDegree);
            double baseAngle = (180 - Degrees) / 2.0;

            double height = Math.Sqrt(leg * leg - 0.25);

            if(offset < 0.5)
            {
                double refBase = 0.5 - offset;
                double angleToHalf = Math.Atan(refBase / height) * DegreesPerRadians;
                return new Angle(Degrees / 2 - angleToHalf);
            }
            else
            {
                double refBase = offset - 0.5;
                double angleToHalf = Math.Atan(refBase / height) * DegreesPerRadians;
                return new Angle(Degrees / 2 + angleToHalf);
            }
            //double newBase = 0.5 - offset;
            //double newLeg = Math.Sqrt(leg * leg - 0.25 + newBase * newBase);

            //double temp = Math.Sin(baseAngle * RadiansPerDegree);

            //double delta = Math.Asin(temp * offset / newLeg) * DegreesPerRadians;

            //return new Angle(offset > 1 ? 180-delta : delta);
        }

        public double OffsetFromDegree(double degrees)
        {
            if (degrees == 0)
                return 0;
            else if (degrees  == this.Degrees)
                return 1;
            else if (degrees == Degrees / 2)
                return 0.5;
            else if (degrees > 180 || degrees < Degrees - 180)
                throw new Exception("The degree is out of range of the vertex angle!");

            double height = 0.5 / Math.Tan(Degrees/2 * RadiansPerDegree);

            double tan = Math.Tan((Degrees/2 - degrees)*RadiansPerDegree);

            double offset = 0.5 - tan * height;
            return offset;
        }

        public double DistanceTo(double another)
        {
            Angle dif = new Angle(Degrees - another);
            return Math.Min(dif.Degrees, 360 - dif.Degrees);
        }

        public double DistanceTo(Angle another)
        {
            return DistanceTo(another.Degrees);
        }

        public static Angle FromOffsetBetween(Angle start, Angle end, double offset)
        {
            Angle vertex = end - start;
            if (!vertex.IsPositive)
            {
                vertex = vertex.Explementary;
                Angle delta = vertex.AngleByOffset(offset);
                return start - delta;
            }
            else
                return start + vertex.AngleByOffset(offset);
        }

        public static Double FromAngleBetween(Angle start, Angle end, double delta)
        {
            Angle vertex = new Angle(end.Degrees - start.Degrees + (end.Degrees >= start.Degrees ? 0 : 360));

            return vertex.OffsetFromDegree(delta);
        }

        public static Double DistanceBetween(double first, double second)
        {
            first = first.Normalize();
            second = second.Normalize();

            double result = Math.Abs(first - second);
            return Math.Min(result, 360 - result);
        }

        //public Aspects AspectTo(Angle endDegree)
        //{
        //    return (endDegree - this).ExactAspect;
        //}
        #endregion
    }

}
