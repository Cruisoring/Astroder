using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum AngleTypes
    {
        Full,
        Acute,
        Right,
        Obtuse,
        Straight,
        Reflex
    }

    public interface IAngleable<T>
    {
        Angle AngleOf(T obj);
    }

    [Serializable]
    public class Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable
    {
        #region constants and static variables definition
        public const int DefaultAngleDecimals = 1;

        public static string DefaultFormat = "D1";

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

        public static char[] fieldsIndicators = new char[] { '°', '\'', '\"' };

        #endregion

        #region Static Functions

        /// <summary>
        /// Convert the Degrees to range of 0.0 - maximum
        /// </summary>
        /// <param name="Degrees">Degrees need to be normalized</param>
        /// <param name="minimum">FirstIndex value expected, default as 0</param>
        /// <param name="maximum">NextIndex value expected, default as 360 </param>
        /// <returns></returns>
        public static double Normalize(double degrees, int minimum, int maximum)
        {
            if (degrees < maximum && degrees >= minimum)
                return degrees;
            else
            {
                //Normalize the value to range between minimum and maximum
                int range = (maximum - minimum);
                double round = Math.Floor((degrees - minimum) / maximum);

                return degrees - round * range;
            }
        }

        public static double Normalize(double degrees, int maximum)
        {

            if (degrees < maximum && degrees >= 0)
                return degrees;
            else
            {
                double round = Math.Floor(degrees / maximum);
                return degrees - maximum * round;
            }
        }

        public static double Normalize(double degrees)
        {
            if (degrees < 360.0 && degrees >= 0)
                return degrees;
            else
            {
                //double round = Math.Floor(degrees / 360);
                //return degrees - 360.0 * round;
                degrees %= 360;

                if (degrees < 0)
                    degrees += 360;
                return degrees;
            }
        }

        public static double Round(double original, int decimals)
        {
            return Math.Round(original, decimals);
        }

        public static double Round(double original)
        {
            return Round(original, DefaultAngleDecimals);
        }

        public static Double BeelineOf(Double startDegree, Double endDegree)
        {
            Double result = Normalize(endDegree - startDegree);

            return result > 180 ? result - 360 : result;
        }

        public static Double BeelineOf(Angle start, Angle end)
        {
            return BeelineOf(start.Degrees, end.Degrees);
        }

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

        public static string FormattedStringOf(Double degree, string format)
        {
            if (format == null || format == "")
                return String.Format("{0:F2}º", degree);
            else
                return new Angle(degree).ToString(format, null);
        }

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
            double m1 = Normalize(lhs.Degrees / 2 + rhs.Degrees / 2);

            return (Normalize(Math.Abs((m1 - lhs.Degrees))) >= 90.0) ? new Angle(m1) : new Angle(m1 + 180);
        }

        public static Angle NearMiddleOf(Angle lhs, Angle rhs)
        {
            double m1 = Normalize(lhs.Degrees / 2 + rhs.Degrees / 2);

            return (Normalize(Math.Abs((m1 - lhs.Degrees))) <= 90.0) ? new Angle(m1) : new Angle(m1 + 180);
        }

        public static Angle MirrorOf(Angle lhs, Angle rhs)
        {
            return new Angle(lhs.Degrees * 2 - rhs.Degrees);
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
            first = Normalize(first);
            second = Normalize(second);

            double result = Math.Abs(first - second);
            return Math.Min(result, 360 - result);
        }
        
        public static double ClockwiseMovement(Angle start, Angle end)
        {
            return (360 + end.Degrees - start.Degrees) % 360;
        }

        public static double AntiClockwiseMovement(Angle start, Angle end)
        {
            return - ((360 + start.Degrees - end.Degrees) % 360);
        }

        public static double ShortestMovement(Angle start, Angle end)
        {
            double dif = (360 + end.Degrees - start.Degrees) % 360;
            return  (dif <= 180) ? dif : dif - 360;
        }

        #endregion

        #region Constructors

        protected Angle() { }

        public Angle(double degrees, bool noNormalize)
        {
            Degrees = noNormalize ? degrees : Normalize(degrees);
        }

        public Angle(double degrees)
        {
            Degrees = Normalize(degrees);
        }

        /// <summary>
        /// Construct a new Angle from hour and minutes.
        /// </summary>
        /// <param name="hour">hour portion of theAngle measurement</param>
        /// <param name="minutes">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        protected Angle(int hour, double minutes)
            : this(hour * Rectascension.DegreesPerHour + minutes / Rectascension.MinutesPerDegree)
        { }

        /// <summary>
        /// Construct a new Angle from degs, minutes, and seconds.
        /// </summary>
        /// <param name="hour">hour portion of theAngle measurement</param>
        /// <param name="minute">minutes portion of theAngle measurement (0 <= minutes < 60)</param>
        /// <param name="seconds">seconds portion of theAngle measurement (0 <= seconds < 60)</param>
        protected Angle(int hour, int minute, double seconds)
            : this(hour * Rectascension.DegreesPerHour + (Double)minute / Rectascension.MinutesPerDegree + seconds / Rectascension.SecondsPerDegree)
        { }
        #endregion

        #region Properties
        public double Degrees { get; protected set; }

        /// <summary>
        /// Get the Angle in radians.
        /// </summary>
        public double Radians
        {
            get { return Degrees * RadiansPerDegree; }
        }

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
            get { return Square - this; }
        }

        /// <summary>
        /// The angle whose angle is 180 - this.Degrees.
        /// </summary>
        public Angle Supplementary
        {
            get { return Opposition - this; }
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
                switch (AngleType)
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
            Double difference = (Degrees - other.Degrees);
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
            string formatType = format;

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
                //case "HM":
                //    return String.Format(formatProvider, "<{0}:{1}>",
                //        Hour, MinutesRemained.ToString(decimalFormat, formatProvider));
                //case "HMS":
                //    return String.Format(formatProvider, "<{0}:{1}:{2}>",
                //        Hour, Minute, Seconds.ToString(decimalFormat, formatProvider));
                //case "ASTRO":
                //    return String.Format(formatProvider, "{0}{1}{2}",
                //        DecimalMinutes.AstroStringOf(decimalFormat, formatProvider));
                default:
                    return ToString(DefaultFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region Functions

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

            if (offset < 0.5)
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
            else if (degrees == this.Degrees)
                return 1;
            else if (degrees == Degrees / 2)
                return 0.5;
            else if (degrees > 180 || degrees < Degrees - 180)
                throw new Exception("The degree is out of range of the vertex angle!");

            double height = 0.5 / Math.Tan(Degrees / 2 * RadiansPerDegree);

            double tan = Math.Tan((Degrees / 2 - degrees) * RadiansPerDegree);

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


        #endregion
    }

    public enum AspectImportance
    {
        Minor,
        Effective,
        Important,
        Critical
    }

    //[Flags]
    //public enum AspectType
    //{

    //}

    [Serializable]
    public class Aspect : Angle, IFormattable
    {
        #region static field definitions

        public const double DefaultOrb = 0.5;
        public const AspectImportance DefaultImportance = AspectImportance.Effective;
        public const char DefaultSymbol = '?';

        public static string SymbolOnlyFormat = "SymbolOnly";
        public static string DegreeOnlyFormat = "DegreeOnly";

        public static Dictionary<Double, Aspect> All = new Dictionary<Double, Aspect>
        {
            { 0, new Aspect(0, "Conjunction", '\u260C', AspectImportance.Critical, 2.0)}, 
            { 30, new Aspect(30, "SemiSextile", '\u26BA', AspectImportance.Effective, DefaultOrb)}, 
            { 45, new Aspect(45, "SemiSquare", '\u2220', AspectImportance.Effective, DefaultOrb)}, 
            { 60, new Aspect(60, "Sextile", '\u26B9', AspectImportance.Effective, DefaultOrb)}, 
            { 72, new Aspect(72, "Quintile", '\u2606', AspectImportance.Minor, DefaultOrb)}, 
            { 90, new Aspect(90, "Square", '\u25A1', AspectImportance.Important, 1.0)}, 
            { 120, new Aspect(120, "Trine", '\u26B3', AspectImportance.Important, 1.0)}, 
            { 135, new Aspect(135, "Sesquiquadrate", '\u26BC', AspectImportance.Effective, DefaultOrb)}, 
            { 144, new Aspect(144, "Biquintile", 'B', AspectImportance.Effective, DefaultOrb)}, 
            { 150, new Aspect(150, "Quincunx", '\u26BB', AspectImportance.Effective, DefaultOrb)}, 
            { 180, new Aspect(180, "Opposition", '\u260D', AspectImportance.Critical, 2.0)}, 
            { 210, new Aspect(210, "Quincunx", '\u26BB', AspectImportance.Effective, DefaultOrb)}, 
            { 216, new Aspect(216, "Biquintile", 'B', AspectImportance.Effective, DefaultOrb)}, 
            { 225, new Aspect(225, "Sesquiquadrate", '\u26BC', AspectImportance.Effective, DefaultOrb)}, 
            { 240, new Aspect(240, "Trine", '\u26B3', AspectImportance.Important, 1.0)}, 
            { 270, new Aspect(270, "Square", '\u25A1', AspectImportance.Important, 1.0)}, 
            { 288, new Aspect(288, "Quintile", '\u2606', AspectImportance.Minor, DefaultOrb)}, 
            { 300, new Aspect(300, "Sextile", '\u26B9', AspectImportance.Effective, DefaultOrb)},
            { 315, new Aspect(315, "SemiSquare", '\u2220', AspectImportance.Effective, DefaultOrb)}, 
            { 330, new Aspect(330, "SemiSextile", '\u26BA', AspectImportance.Effective, DefaultOrb)},
            { 360, new Aspect(360, "Conjunction", '\u260C', AspectImportance.Critical, 2.0)}
        };

        public static List<double> CriticalAspectDegrees = null;

        public static List<double> ImportantAspectDegrees = null;

        public static List<double> EffectiveAspectDegrees = null;

        public static List<double> AllAspectDegrees = null;

        static Aspect()
        {
            AllAspectDegrees = All.Keys.ToList();
            AllAspectDegrees.Sort();

            CriticalAspectDegrees = (from kvp in All
                                     where kvp.Value.Importance == AspectImportance.Critical
                                     select kvp.Key).ToList();
            CriticalAspectDegrees.Sort();

            ImportantAspectDegrees = (from kvp in All
                                      where kvp.Value.Importance >= AspectImportance.Important
                                      select kvp.Key).ToList();
            ImportantAspectDegrees.Sort();

            EffectiveAspectDegrees = (from kvp in All
                                      where kvp.Value.Importance >= AspectImportance.Effective
                                      select kvp.Key).ToList();
            EffectiveAspectDegrees.Sort();
        }

        public static List<Double> DegreesOf(AspectImportance importance)
        {
            switch (importance)
            {
                case AspectImportance.Critical:
                    return Aspect.CriticalAspectDegrees;
                case AspectImportance.Important:
                    return Aspect.ImportantAspectDegrees;
                case AspectImportance.Effective:
                    return Aspect.EffectiveAspectDegrees;
                default:
                    return  Aspect.AllAspectDegrees;
            }
        }

        public static List<Double> DegreesOf(Double step)
        {
            if (step % 2.5 != 0 && step % 11.25 != 0)
                return AllAspectDegrees;

            List<Double> result = new List<Double>(AllAspectDegrees);

            for (double deg = step; deg < 360; deg += step )
            {
                if (!result.Contains(deg))
                    result.Add(deg);
            }

            result.Sort();
            return result;
        }

        //public static Aspect AspectBetween(double degree1, double degree2)
        //{
        //    double dif = ( 360 + degree1 - degree2) % 360;
        //    double aspectDegree = NearAspectDegree(degree1, degree2);

        //    Aspect theAspect = All[aspectDegree];

        //    if (Math.Abs(dif - aspectDegree) <= theAspect.Orb)
        //        return theAspect;
        //    else
        //        return null;
        //}


        //public static double NearAspectDegree(double degree1, double degree2)
        //{
        //    double dif = ( 360 + degree1 - degree2) % 360;

        //    double aspectDegree = (from deg in AllAspectDegrees
        //                         orderby Math.Abs(dif - deg)
        //                         select deg).FirstOrDefault();

        //    return aspectDegree;
        //}

        //public static double NearAspectDegree(double dif)
        //{
        //    dif = (360 + dif) % 360;
        //    return (from deg in AllAspectDegrees
        //            orderby Math.Abs(dif - deg)
        //            select deg).FirstOrDefault();
        //}

        #endregion

        #region Properties

        public String Name { get; private set; }

        public char Symbol { get; private set; }

        public AspectImportance Importance { get; private set; }

        public double Orb { get; private set; }

        #endregion

        private Aspect(Double degree, string name, char symbol, AspectImportance importance, double orb)
            : base(degree)
        {
            Name = name;
            Symbol = symbol;
            Importance = importance;
            Orb = orb;
        }

        private Aspect(Double degree, string name, char symbol)
            : this(degree, name, symbol, DefaultImportance, DefaultOrb) { }

        private Aspect(Double degree, string name)
            : this(degree, name, DefaultSymbol, DefaultImportance, DefaultOrb) { }

        public override string ToString()
        {
            return String.Format("<{0}º>", Degrees);
            //return Symbol == DefaultSymbol ?
            //    String.Format("({0})", Degrees) :
            //    Degrees > 180 ? Symbol.ToString() : String.Format("{0}", Symbol);
        }


        #region IFormattable 成员

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == SymbolOnlyFormat)
                return Symbol.ToString();
            else if (format == DegreeOnlyFormat)
                return String.Format("({0})", Degrees);
            else
                return ToString();
        }

        #endregion
    }
}
