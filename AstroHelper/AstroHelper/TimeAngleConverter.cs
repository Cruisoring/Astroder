using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using NumberHelper.DoubleHelper;
using System.Reflection;
using System.Drawing;

namespace AstroHelper
{
    public enum TimeConvertRules
    {
        SolarBased,
        CalendarBased,
        EquinoxBased,
        SelfBased,
        Outside
    }

    public delegate Double TimeToDegreeDelegate(DateTimeOffset time);
    public delegate DateTimeOffset DegreeToTimeDelegate(Double degree, DateTimeOffset since);

    [Serializable]
    public class TimeAngleConverter : IAngleable<DateTimeOffset>
    {
        #region Static ones
        public static Dictionary<String, TimeAngleConverter> All = new Dictionary<string, TimeAngleConverter>();

        public static TimeAngleConverter ClockOff = null;
        public static TimeAngleConverter SolarClock = new TimeAngleConverter(TimeConvertRules.SolarBased);
        public static TimeAngleConverter CalendarClock = new TimeAngleConverter(TimeConvertRules.CalendarBased);
        public static TimeAngleConverter EquinoxClock = new TimeAngleConverter(TimeConvertRules.EquinoxBased);

        static TimeAngleConverter()
        {
            Type mapperType = typeof(TimeAngleConverter);
            FieldInfo[] fields = mapperType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                TimeAngleConverter instance = fi.GetValue(null) as TimeAngleConverter;

                if (fi.FieldType == typeof(TimeAngleConverter))
                    All.Add(fi.Name, instance);
            }
        }

        #endregion

        #region Fields
        public TimeConvertRules Rule { get; set; }

        public DateTimeOffset Zero { get; set; }

        public Double CycleDays { get; set; }

        public Double ShiftedDays { get; set; }

        public Color DisplayColor { get; set; }

        #endregion

        #region Constructors

        private TimeAngleConverter(TimeConvertRules rule)
        {
            Rule = rule;
            DisplayColor = Color.Purple;
            switch(rule)
            {
                case TimeConvertRules.SolarBased:
                    DegreeOf = new TimeToDegreeDelegate(SolarDegrees);
                    break;
                case TimeConvertRules.CalendarBased:
                    DegreeOf = new TimeToDegreeDelegate(CalendarDegrees);
                    break;
                case TimeConvertRules.EquinoxBased:
                    DegreeOf = new TimeToDegreeDelegate(DegreeToEquinox);
                    break;
                default:
                    throw new Exception();
            }
        }

        public TimeAngleConverter(DateTimeOffset zero)
        {
            Rule = TimeConvertRules.SelfBased;
            Zero = zero;
            CycleDays = Ephemeris.AverageYearLength;
            ShiftedDays = 0;
            DisplayColor = Color.Orchid;
            DegreeOf = new TimeToDegreeDelegate(selfTimeToDegree);
        }

        public TimeAngleConverter(DateTimeOffset zero, Double cycleInDays)
        {
            Rule = TimeConvertRules.SelfBased;
            Zero = zero;
            CycleDays = cycleInDays;
            DisplayColor = Color.Orchid;
            ShiftedDays = 0;
            DegreeOf = new TimeToDegreeDelegate(selfTimeToDegree);
        }

        public TimeAngleConverter(TimeToDegreeDelegate convertDel)
        {
            Rule = TimeConvertRules.Outside;
            DegreeOf = convertDel;
            DisplayColor = Color.Orange;
        }

        #endregion

        #region functions

        public TimeToDegreeDelegate DegreeOf = null;

        //public DegreeToTimeDelegate TimeOfDegree = null;

        private Double selfTimeToDegree(DateTimeOffset time)
        {
            return DegreeToReference(time.AddDays(ShiftedDays), Zero, CycleDays);
        }

        public Angle AngleOf(DateTimeOffset time)
        {
            return new Angle(DegreeOf(time));
        }

        public List<Double> ConverteOf(IEnumerable<DateTimeOffset> dates)
        {
            List<Double> dateValues = new List<Double>();

            foreach (DateTimeOffset date in dates)
            {
                dateValues.Add(DegreeOf(date));
            }

            return dateValues;
        }

        #endregion

        #region Time calculation tools

        public static Double SolarDegrees(DateTimeOffset time)
        {
            return Ephemeris.Geocentric[time, PlanetId.SE_SUN].Longitude;
        }

        public static Angle SolarAngle(DateTimeOffset time)
        {
            return Ephemeris.Geocentric[time, PlanetId.SE_SUN].Longitude;
        }

        public static Double CalendarDegrees(DateTimeOffset time)
        {
            DateTimeOffset start = new DateTimeOffset(time.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset end = new DateTimeOffset(time.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
            Double yearLength = (end - start).TotalDays;
            Double elapsed = (time - start).TotalDays;

            return (elapsed * 360 / yearLength).Normalize();
        }

        public static Angle CalenendarAngle(DateTimeOffset time)
        {
            return CalendarDegrees(time);
        }

        public static Double DegreeToEquinox(DateTimeOffset time)
        {            
            DateTimeOffset thisVernal = Ephemeris.VernalEquinoxTimeOf(time.Year);
            Double yearLength, elapsed;

            if (time >= thisVernal)
            {
                DateTimeOffset end = Ephemeris.VernalEquinoxTimeOf(time.Year + 1);
                yearLength = (end - thisVernal).TotalDays;
                elapsed = (time - thisVernal).TotalDays;
            }
            else
            {
                DateTimeOffset last = Ephemeris.VernalEquinoxTimeOf(time.Year - 1);
                yearLength = (thisVernal - last).TotalDays;
                elapsed = (time - last).TotalDays;
            }
            return (elapsed * 360 / yearLength).Normalize();
        }

        public static Angle AngleToEquinox(DateTimeOffset time)
        {
            return new Angle(DegreeToEquinox(time));
        }

        public static Double DegreeToReference(DateTimeOffset time, DateTimeOffset refDate)
        {
            Double elapsed = (time - refDate).TotalDays;

            return (elapsed * 360 / Ephemeris.AverageYearLength).Normalize();
        }

        public static Angle AngleToReference(DateTimeOffset time, DateTimeOffset refDate)
        {
            Double elapsed = (time - refDate).TotalDays;

            return new Angle(elapsed * 360 / Ephemeris.AverageYearLength);
        }

        public static Double DegreeToReference(DateTimeOffset time, DateTimeOffset refDate, Double cycleDays)
        {
            Double elapsed = (time - refDate).TotalDays;

            return (elapsed * 360 / cycleDays).Normalize();
        }

        public static Angle AngleToReference(DateTimeOffset time, DateTimeOffset refDate, Double cycleDays)
        {
            Double elapsed = (time - refDate).TotalDays;

            return new Angle(elapsed * 360 / cycleDays);
        }
        #endregion

    }
}
