using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum RecordType
    {
        Unknown,
        MinuteRecord,
        FiveMinuteRecord,
        ThirteenMinuteRecord,
        HalfHourRecord,
        HourRecord,
        TwoHourRecord,
        DayRecord,
        WeekRecord,
        MonthRecord,
        QuarterRecord,
        YearRecord
    }

    //public enum ValueType
    //{
    //    Typical,
    //    Date,
    //    Open,
    //    High,
    //    Low,
    //    Close,
    //    Volume
    //}

    [Serializable]
    public class Quote : IComparer<Quote>, IComparable<Quote>, IFormattable
    {
        public DateTimeOffset Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        public double JulianDay { get { return ToJulianDay(Date); } }

        //public double this[ValueType type]
        //{
        //    get
        //    {
        //        switch (type)
        //        {
        //            case ValueType.Date:
        //                return ToJulianDay(Date);
        //            case ValueType.Open:
        //                return Open;
        //            case ValueType.High:
        //                return High;
        //            case ValueType.Low:
        //                return Low;
        //            default:
        //            case ValueType.Close:
        //                return Close;
        //            case ValueType.Volume:
        //                return Volume;
        //        }
        //    }
        //    set
        //    {
        //        switch (type)
        //        {
        //            case ValueType.Date:
        //                Date = UtcFromJulianDay(value);
        //                break;
        //            case ValueType.Open:
        //                Open = value;
        //                break;
        //            case ValueType.High:
        //                High = value;
        //                break;
        //            case ValueType.Low:
        //                Low = value;
        //                break;
        //            default:
        //            case ValueType.Close:
        //                Close = value;
        //                break;
        //            case ValueType.Volume:
        //                Volume = value;
        //                break;
        //        }
        //    }
        //}

        public Quote(DateTimeOffset date, Double value)
        {
            Date = date;
            Open = High = Low = Close = value;
            Volume = 0;
        }

        public Quote(DateTimeOffset date, List<Double> values)
        {
            Date = date;

            switch (values.Count)
            {
                case 0:
                    throw new Exception("No price data provided ? !");
                case 1:
                    Open = High = Low = Close = values[0];
                    Volume = 0;
                    break;
                case 4:
                    Open = values[0];
                    High = values[1];
                    Low = values[2];
                    Close = values[3];
                    break;
                case 5:
                case 6:
                    Open = values[0];
                    High = values[1];
                    Low = values[2];
                    Close = values[3];
                    Volume = values[4];
                    break;
                default:
                    throw new ArgumentException("The price data is lack of order?");
            }
        }

        public Quote(DateTimeOffset date, double open, double high, double low, double close, double volume)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public Quote(DateTimeOffset date, double open, double high, double low, double close)
            : this(date, open, high, low, close, 0)
        { }

        public override string ToString()
        {
            return string.Format("{0}: Open={1}, High={2}, Low={3}, Close={4}",
                Date, Open, High, Low, Close);
        }

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("{0}({5}, Week {6}): Open={1}, High={2}, Low={3}, Close={4}",
                Date.ToString(format, formatProvider), Open, High, Low, Close, Date.UtcDateTime.ToOADate(), Math.Ceiling(Date.DayOfYear/7.0));
        }

        #endregion

        #region IComparer<Quote> 成员

        public virtual int Compare(Quote x, Quote y)
        {
            return x.CompareTo(y);
        }

        #endregion

        #region IComparable<Quote> 成员

        public virtual int CompareTo(Quote other)
        {
            return Date.CompareTo(other.Date);
            //if (TheRecordType == other.TheRecordType)
            //    return Date.CompareTo(other.Date);
            //else if (TheRecordType < other.TheRecordType)
            //{
            //    if (Date < other.Date)
            //        return -1;
            //    else if (Date >= other.EndOfRecord)
            //        return 1;
            //    else
            //        return 0;
            //}
            //else
            //{
            //    if (EndOfRecord < other.Date)
            //        return -1;
            //    else if (Date >= other.EndOfRecord)
            //        return 1;
            //    else
            //        return 0;
            //}
        }

        #endregion

        #region Static Functions

        public static double ToJulianDay(DateTimeOffset moment)
        {
            DateTime momentUtc = moment.UtcDateTime;
            double jd;
            double u, u0, u1, u2;

            u = momentUtc.Year;
            if (momentUtc.Month < 3)
                u -= 1;
            u0 = u + 4712.0;
            u1 = momentUtc.Month + 1.0;
            if (u1 < 4)
                u1 += 12.0;
            jd = Math.Floor(u0 * 365.25)
               + Math.Floor(30.6 * u1 + 0.000001)
               + momentUtc.Day + momentUtc.TimeOfDay.TotalHours / 24.0 - 63.5;
            u2 = Math.Floor(Math.Abs(u) / 100) - Math.Floor(Math.Abs(u) / 400);

            if (u < 0.0)
                u2 = -u2;
            jd = jd - u2 + 2;
            if ((u < 0.0) && (u / 100 == Math.Floor(u / 100)) && (u / 400 != Math.Floor(u / 400)))
                jd -= 1;

            return jd;
        }

        public static DateTimeOffset UtcFromJulianDay(Double julday)
        {
            int year, month, day;
            double utTime;

            double u0, u1, u2, u3, u4;
            u0 = julday + 32082.5;
            u1 = u0 + Math.Floor(u0 / 36525.0) - Math.Floor(u0 / 146100.0) - 38.0;
            if (julday >= 1830691.5) u1 += 1;
            u0 = u0 + Math.Floor(u1 / 36525.0) - Math.Floor(u1 / 146100.0) - 38.0;
            u2 = Math.Floor(u0 + 123.0);
            u3 = Math.Floor((u2 - 122.2) / 365.25);
            u4 = Math.Floor((u2 - Math.Floor(365.25 * u3)) / 30.6001);
            month = (int)(u4 - 1.0);
            if (month > 12) month -= 12;
            day = (int)(u2 - Math.Floor(365.25 * u3) - Math.Floor(30.6001 * u4));
            year = (int)(u3 + Math.Floor((u4 - 2.0) / 12.0) - 4800);
            utTime = (julday - Math.Floor(julday + 0.5) + 0.5) * 24.0;

            TimeSpan inHours = TimeSpan.FromHours(utTime);
            DateTime utcMoment = new DateTime(year, month, day, inHours.Hours, inHours.Minutes, inHours.Seconds, inHours.Milliseconds);
            return new DateTimeOffset(utcMoment, TimeSpan.Zero);
        }

        #endregion

    }


}
