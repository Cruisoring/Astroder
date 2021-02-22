using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZedGraph;

namespace StockQuote
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

    public enum ValueType
    {
        Typical,
        Date,
        Open,
        High,
        Low,
        Close,
        Volume        
    }

    //public class RecordAttribute : Attribute
    //{
    //    private static RecordType[] allRecordTypes;

    //    private static List<Kind> TypesOfAllItem;
        
    //    public static RecordAttribute()
    //    {
    //        allRecordTypes = (RecordType)Enum.GetValues(typeof(RecordType));
    //    }

    //    private static List<RecordType> ;

    //    public RecordType TheRecordType {get; set;}
    //    public List<RecordType> Siblings { get; set;}
    //    public List<RecordType> Parents { get; set;}
    //    public RecordAttribute(RecordType theType)
    //    {
    //        TheRecordType = theType;

    //        var siblingQuery =
    //            from type in allRecordTypes
    //            where type < theType
    //            select type;

    //        Siblings = siblingQuery.ToList();

    //        var parentsQuery = 
    //            from type in allRecordTypes
    //            where type > theType
    //            select type;

    //        Parents = parentsQuery.ToList();
    //    }

    //    public RecordAttribute(RecordType theType, List<RecordType> siblingTypes, List<RecordType> parentTypes)
    //    {
    //        TheRecordType = theType;
    //        Siblings = siblingTypes;            
    //        Parents = parentTypes;
    //    }
    //}

    [Serializable]
    public class RecordBase : IComparer<RecordBase>, IComparable<RecordBase>
    {
        #region Properties of static features
        //public abstract TimeSpan TimeZoneOffset { get; }
        //public abstract RecordBase FromSiblings(ICollection<RecordBase> siblings);
        public static DateTime TimeOfRecord(RecordType type, DateTime refTime)
        {
            switch(type)
            {
                case RecordType.DayRecord:
                    return refTime.Date;
                case RecordType.WeekRecord:
                    return refTime.Date - TimeSpan.FromDays(refTime.DayOfWeek - DayOfWeek.Friday);
                case RecordType.MonthRecord:
                    DateTime temp = refTime.AddDays(refTime.Day < 20 ? 31 : 20); 
                    return new DateTime(temp.Year, temp.Month, 1).AddDays(-1);
                case RecordType.QuarterRecord:
                    return new DateTime(refTime.Year, refTime.Month - ((refTime.Month+2)%3), 1);
                case RecordType.Unknown:
                    return refTime;
                default:
                    throw new NotImplementedException();
            }
        }

        public RecordType TheRecordType { get{ return RecordType.Unknown; }}
        public String TimeFormat { get { return ""; } }
        //public abstract DateTime EndOfRecord { get; }
        #endregion

        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        public double this[ValueType type]
        {
            get {
                switch(type)
                {
                    case ValueType.Date:
                        return XDate.DateTimeToXLDate(Date);
                    case ValueType.Open:
                        return Open;
                    case ValueType.High:
                        return High;
                    case ValueType.Low:
                        return Low;
                    default:
                    case ValueType.Close:
                        return Close;
                    case ValueType.Volume:
                        return Volume;
                }
            }
            set{
                switch (type)
                {
                    case ValueType.Date:
                        Date = XDate.XLDateToDateTime(value);
                        break;
                    case ValueType.Open:
                        Open = value;
                        break;
                    case ValueType.High:
                        High = value;
                        break;
                    case ValueType.Low:
                        Low = value;
                        break;
                    default:
                    case ValueType.Close:
                        Close = value;
                        break;
                    case ValueType.Volume:
                        Volume = value;
                        break;
                }
            }
        }

        public RecordBase(DateTime date)
        {
            //Date = TimeOfRecord(TheRecordType, date);
            Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public RecordBase(DateTime date, List<Double> values) : this(date)
        {
            switch(values.Count)
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

        public RecordBase(DateTime date, double open, double high, double low, double close, double volume)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public RecordBase(DateTime date, double open, double high, double low, double close) : this(date, open, high, low, close, 0)
        {  }
        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}, {3}, {4}", 
                Date.ToString(TimeFormat, System.Globalization.CultureInfo.InvariantCulture), Open, High, Low, Close);
        }

        #region IComparer<RecordBase> 成员

        public virtual int Compare(RecordBase x, RecordBase y)
        {
            return x.CompareTo(y);
        }

        #endregion

        #region IComparable<RecordBase> 成员

        public virtual int CompareTo(RecordBase other)
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
    }

    [Serializable]
    public class DayItem : RecordBase, IComparable<DayItem>
    {
        //private static TimeSpan theTimeZoneOffset;

        //public static void ChangeOffset(TimeSpan newOffset)
        //{
        //    theTimeZoneOffset = newOffset;
        //}

        //static DayItem()
        //{
        //    theTimeZoneOffset = DateTimeOffset.Now.Offset;
        //}

        //public override TimeSpan TimeZoneOffset
        //{
        //    get { return theTimeZoneOffset; }
        //}

        //public override DateTime EndOfRecord
        //{
        //    get { return Date.Date.AddDays(1); }
        //}

        //public override RecordType TheRecordType
        //{
        //    get { return RecordType.DayRecord; }
        //}

        //public override String TimeFormat 
        //{
        //    get { return "yyyy-MM-dd ddd"; }
        //}

        public DayItem(DateTime time, List<Double> values) : base(time, values){}

        #region IComparable<DayItem> 成员

        public int CompareTo(DayItem other)
        {
            return base.CompareTo(other);
        }

        #endregion
    }

    public class WeekItem : RecordBase
    {
        //private static TimeSpan theTimeZoneOffset;

        //public static void ChangeOffset(TimeSpan newOffset)
        //{
        //    theTimeZoneOffset = newOffset;
        //}

        //static WeekItem()
        //{
        //    theTimeZoneOffset = DateTimeOffset.Now.Offset;
        //}

        //public override TimeSpan TimeZoneOffset
        //{
        //    get { return theTimeZoneOffset; }
        //}

        //public override DateTime EndOfRecord
        //{
        //    get { return Date.AddDays(7); }
        //}

        //public override RecordType TheRecordType
        //{
        //    get { return RecordType.WeekRecord; }
        //}

        //public override String TimeFormat
        //{
        //    get { return "D"; }
        //}

        public WeekItem(DateTime time, List<Double> values) : base(time, values) { }
    }

    public class MonthItem : RecordBase
    {
        //private static TimeSpan theTimeZoneOffset;

        //public static void ChangeOffset(TimeSpan newOffset)
        //{
        //    theTimeZoneOffset = newOffset;
        //}

        //static MonthItem()
        //{
        //    theTimeZoneOffset = DateTimeOffset.Now.Offset;
        //}

        //public override TimeSpan TimeZoneOffset
        //{
        //    get { return theTimeZoneOffset; }
        //}

        //public override DateTime EndOfRecord
        //{
        //    get { DateTime temp = Date.AddDays(31); return new DateTime(temp.Year, temp.Month, 1); }
        //}

        //public override RecordType TheRecordType
        //{
        //    get { return RecordType.MonthRecord; }
        //}

        //public override String TimeFormat
        //{
        //    get { return "y"; }
        //}

        public MonthItem(DateTime time, List<Double> values) : base(time, values) { }
    }

}
