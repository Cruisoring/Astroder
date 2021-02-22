using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EphemerisCalculator;
using System.Globalization;

namespace DataImporter
{
    public enum RecordType :int
    {
        MinuteRecord = -6,
        FiveMinuteRecord = -5,
        ThirteenMinuteRecord = -4,
        HalfHourRecord = -3,
        HourRecord = -2,
        TwoHourRecord = -1,
        DayRecord = 0,
        WeekRecord = 1,
        MonthRecord = 2,
        QuarterRecord = 3,
        YearRecord = 4,
        UserDefined = 5
    }

    //public enum ValueType
    //{
    //    Typical,
    //    Time,
    //    Open,
    //    High,
    //    Low,
    //    Close,
    //    Volume
    //}

    [Serializable]
    public class Quote : IComparer<Quote>, IComparable<Quote>, IFormattable
    {
        #region Static fields and properties
        public static Dictionary<RecordType, String> DefaultDateTimeFormats = new Dictionary<RecordType, String> {
            { RecordType.MinuteRecord, "HH:mm"},
            { RecordType.FiveMinuteRecord, "HH:mm"},
            { RecordType.ThirteenMinuteRecord, "HH:mm"},
            { RecordType.HourRecord, "MM-dd HH:mm"},
            { RecordType.TwoHourRecord, "MM-dd HH:mm"},
            { RecordType.DayRecord, "yyyy-MM-dd ddd"},
            { RecordType.WeekRecord, "yyyy-MM-dd"},
            { RecordType.MonthRecord, "yyyy-MM"},
            { RecordType.QuarterRecord, "yyyy-MM" },
            { RecordType.YearRecord, "yyyy"},
            { RecordType.UserDefined, "yyyyMMdd, ddd"}
        };

        #endregion

        #region properties definition

        public RecordType Type { get; set; }

        [System.Xml.Serialization.XmlElement("Time")]
        public string TimeString
        {
            get { return Time.ToString(); }
            set { Time = DateTimeOffset.Parse(value); }
        }
        [System.Xml.Serialization.XmlIgnore]
        public DateTimeOffset Time { get; set; }

        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public double JulianDay { get { return Ephemeris.ToJulianDay(Time); } }
        public int WeekOfYear
        {
            get
            {
                return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Time.UtcDateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            }
        }

        #endregion

        #region Constructors

        private Quote() { Type = RecordType.DayRecord; }

        public Quote(DateTimeOffset date, Double value) 
            : this()
        {
            Time = date;
            Open = High = Low = Close = value;
            Volume = 0;
        }

        public Quote(DateTimeOffset date, List<Double> values) 
            : this()
        {
            Time = date;

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
            : this()
        {
            Time = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        public Quote(DateTimeOffset date, double open, double high, double low, double close)
            : this(date, open, high, low, close, 0)
        { }

        public Quote(RecordType type, DateTimeOffset date, double open, double high, double low, double close)
            : this(type, date, open, high, low, close, 0)
        {}


        public Quote(RecordType type, DateTimeOffset date, double open, double high, double low, double close, double volume)
        {
            Time = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Type = type;
        }

        public Quote(Quote original)
            :this(original.Type, original.Time, original.Open, original.High, original.Low, original.Close, original.Volume)
        { }

        public Quote(Quote original, double delta)
            : this(original.Type, original.Time, original.Open + delta, original.High + delta, original.Low + delta, original.Close + delta, original.Volume)
        { }

        public Quote(Quote original, double delta, double divisor)
            //: this(original.Type, original.Time, original.Open/divisor + delta, original.High/divisor + delta, 
            //original.Low/divisor + delta, original.Close/divisor + delta, original.Volume)
        {
            Time = original.Time;
            Open = Math.Round(original.Open*divisor+delta,3);
            High = Math.Round(original.High * divisor + delta, 3);
            Low = Math.Round(original.Low * divisor + delta, 3);
            Close = Math.Round(original.Close * divisor + delta, 3);
            Volume = original.Volume;
            Type = original.Type;
        }


        #endregion

        public override string ToString()
        {
            switch(Type)
            {
                case RecordType.WeekRecord:
                    return String.Format("{0} Week{1}: Open={2}, High={3}, Low={4}, Close={5}", Time.Year, WeekOfYear, Open, High, Low, Close);
                case RecordType.MonthRecord:
                    return String.Format("{0}-{1}: Open={2}, High={3}, Low={4}, Close={5}", Time.Year, Time.Month, Open, High, Low, Close);
                case RecordType.QuarterRecord:
                    return String.Format("{0} Q{1}: Open={2}, High={3}, Low={4}, Close={5}", Time.Year, (int)((Time.Month+2)/3), Open, High, Low, Close);
                case RecordType.MinuteRecord:
                case RecordType.FiveMinuteRecord:
                case RecordType.ThirteenMinuteRecord:
                case RecordType.HalfHourRecord:
                    return String.Format("{0}: Open={1}, High={2}, Low={3}, Close={4}", Time.ToString("MM-dd HH:mm"), Open, High, Low, Close);
                case RecordType.HourRecord:
                    return String.Format("{0}: Open={1}, High={2}, Low={3}, Close={4}", Time.ToString("MM-dd:HH"), Open, High, Low, Close);
                default:
                    return string.Format("{0}: Open={1}, High={2}, Low={3}, Close={4}",
                        Time.ToString(DefaultDateTimeFormats[Type], CultureInfo.InvariantCulture), Open, High, Low, Close);
            }
        }

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("{0}({5}): Open={1}, High={2}, Low={3}, Close={4}",
                Time.ToString(format, formatProvider), Open, High, Low, Close, JulianDay);
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
            return Time.CompareTo(other.Time);
            //if (TheRecordType == other.TheRecordType)
            //    return Time.CompareTo(other.Time);
            //else if (TheRecordType < other.TheRecordType)
            //{
            //    if (Time < other.Time)
            //        return -1;
            //    else if (Time >= other.EndOfRecord)
            //        return 1;
            //    else
            //        return 0;
            //}
            //else
            //{
            //    if (EndOfRecord < other.Time)
            //        return -1;
            //    else if (Time >= other.EndOfRecord)
            //        return 1;
            //    else
            //        return 0;
            //}
        }

        #endregion

    }

    [Serializable]
    public class QuoteCollection : ICollection<Quote>
    {
        #region Static Functions

        private delegate int periodDelegate(Quote lhs);

        private static int yearValueOf(Quote lhs)
        {
            return lhs.Time.Year;
        }

        private static int quarterValueOf(Quote lhs)
        {
            return lhs.Time.Year * 4 + (lhs.Time.Month / 3);
        }

        private static int monthValueOf(Quote lhs)
        {
            return lhs.Time.Year * 12 + lhs.Time.Month;
        }

        private static int weekValueOf(Quote lhs)
        {
            return lhs.Time.Year * 52 + CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(lhs.Time.UtcDateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        private static int dayValueOf(Quote lhs)
        {
            return lhs.Time.Year * 365 + lhs.Time.DayOfYear;
        }

        private static int hourValueOf(Quote lhs)
        {
            return (lhs.Time.Year * 365 + lhs.Time.DayOfYear) * 24 + lhs.Time.Hour;
        }

        private static int halfHourValueOf(Quote lhs)
        {
            return (lhs.Time.Year * 365 + lhs.Time.DayOfYear) * 48 + lhs.Time.Hour * 2 + lhs.Time.Minute>=30?1:0;
        }

        private static int thirteenMinutesValueOf(Quote lhs)
        {
            return ((lhs.Time.Year * 365 + lhs.Time.DayOfYear) * 24 + lhs.Time.Hour) * 4 + (int)(Math.Ceiling((lhs.Time.Minute)/15.0));
        }

        private static int fiveMinutesValueOf(Quote lhs)
        {
            return ((lhs.Time.Year * 365 + lhs.Time.DayOfYear) * 24 + lhs.Time.Hour) * 12 + (int)(Math.Ceiling((lhs.Time.Minute) / 5.0));
        }

        public static List<Quote> FromShortTerms(QuoteCollection shorters, RecordType longerType)
        {
            return FromShortTerms(shorters.DataCollection, longerType);
        }

        public static List<Quote> FromShortTerms(List<Quote> shorters, RecordType longerType)
        {
            //if (longerType <= shorters.QuoteType)
            //    throw new Exception();
            //else if (longerType <= RecordType.DayRecord)
            //    throw new NotImplementedException();
            //else if (shorters.QuoteType == RecordType.WeekRecord)
            //    throw new NotImplementedException();

            RecordType shortType = shorters[0].Type;

            if (longerType <= shortType)
                throw new Exception();
            else if (shortType == RecordType.WeekRecord)
                throw new NotImplementedException();

            periodDelegate differenceComparer = null;

            switch (longerType)
            {
                case RecordType.FiveMinuteRecord:
                    differenceComparer = new periodDelegate(fiveMinutesValueOf);
                    break;
                case RecordType.ThirteenMinuteRecord:
                    differenceComparer = new periodDelegate(thirteenMinutesValueOf);
                    break;
                case RecordType.HalfHourRecord:
                    differenceComparer = new periodDelegate(halfHourValueOf);
                    break;
                case RecordType.HourRecord:
                    differenceComparer = new periodDelegate(hourValueOf);
                    break;
                case RecordType.DayRecord:
                    differenceComparer = new periodDelegate(dayValueOf);
                    break;
                case RecordType.WeekRecord:
                    differenceComparer = new periodDelegate(weekValueOf);
                    break;
                case RecordType.MonthRecord:
                    differenceComparer = new periodDelegate(monthValueOf);
                    break;
                case RecordType.QuarterRecord:
                    differenceComparer = new periodDelegate(quarterValueOf);
                    break;
                case RecordType.YearRecord:
                    differenceComparer = new periodDelegate(yearValueOf);
                    break;
                default:
                    throw new NotImplementedException();
            }

            List<Quote> result = new List<Quote>();

            var groupQuery =
                from item in shorters
                let period = differenceComparer(item)
                group item by period into clusters
                orderby clusters.Key
                select clusters;

            foreach (var cluster in groupQuery)
            {
                double open = cluster.First().Open, high = double.MinValue, low = double.MaxValue, close = cluster.Last().Close, volume = 0;
                DateTimeOffset lastTime = cluster.Last().Time;

                foreach (Quote item in cluster)
                {
                    if (item.High > high)
                        high = item.High;

                    if (item.Low < low)
                        low = item.Low;

                    volume += item.Volume;
                }
                Quote newRecord = new Quote(lastTime, open, high, low, close, volume);
                result.Add(newRecord);
            }

            return result;
        }

        public static RecordType GuessQuoteType(List<Quote> quotes)
        {
            if (quotes.Count < 4)
                return RecordType.UserDefined;
            else
            {
                TimeSpan span = quotes[1].Time - quotes[0].Time;
                TimeSpan minSpan = span, maxSpan = span;

                for (int i = 1; i < 4; i++)
                {
                    span = quotes[i + 1].Time - quotes[i].Time;

                    if (span > maxSpan)
                        maxSpan = span;
                    else if (span < minSpan)
                        minSpan = span;
                }

                int minutes = (int)minSpan.TotalMinutes;

                switch (minutes)
                {
                    case 1440:
                        return RecordType.DayRecord;
                    case 120:
                        return RecordType.TwoHourRecord;
                    case 60:
                        return RecordType.HourRecord;
                    case 30:
                        return RecordType.HalfHourRecord;
                    case 15:
                        return RecordType.ThirteenMinuteRecord;
                    case 5:
                        return RecordType.FiveMinuteRecord;
                    case 1:
                        return RecordType.MinuteRecord;
                    default:
                        if (minSpan > TimeSpan.FromDays(1) && maxSpan <= TimeSpan.FromDays(7))
                            return RecordType.WeekRecord;
                        else if (minSpan > TimeSpan.FromDays(28) && maxSpan <= TimeSpan.FromDays(31))
                            return RecordType.MonthRecord;
                        else if (minSpan > TimeSpan.FromDays(31) && maxSpan <= TimeSpan.FromDays(92))
                            return RecordType.QuarterRecord;
                        else if (minSpan > TimeSpan.FromDays(92) && maxSpan <= TimeSpan.FromDays(366))
                            return RecordType.YearRecord;
                        else
                            return RecordType.UserDefined;
                }
            }
        }

        #endregion

        #region Properties

        #region Variables

        public string Name { get; set; }

        public string Source { get; private set; }

        public string Description
        {
            get { return QuoteType.ToString() + " of " + Name; }
        }

        public RecordType QuoteType { get; set; }

        public List<Quote> DataCollection { get; private set; }

        public DateTimeOffset Since { get { return DataCollection.First().Time; } }

        public DateTimeOffset Until { get { return DataCollection.Last().Time; } }

        public List<Double> QuoteOADates { get; private set; }

        public int CelingIndex { get; set; }

        public int FloorIndex { get; set; }

        public double Ceiling { get { return DataCollection[CelingIndex].High; } }

        public double Floor { get { return DataCollection[FloorIndex].Low; } }

        private Outline baseOutline = null;
        public Outline BaseOutline 
        { 
            get 
            {
                if (baseOutline == null || baseOutline.Threshold != Outline.DefaultThreshold)
                    baseOutline = OutlineHelper.OutlineFromQuotes(this, Outline.DefaultThreshold);
                return baseOutline;
            }
            private set { baseOutline = value; } 
        }

        public List<Double> OutlineValues 
        {
            get { return BaseOutline.Values;}
        }

        public List<Double> OutlineOADates 
        { 
            get  { return BaseOutline.Dates; } 
        }

        #endregion

        #region Indexer

        public QuoteCollection this[RecordType longer]
        {
            get
            {
                if (QuoteType == RecordType.UserDefined || longer < QuoteType)
                    return null;
                //throw new Exception("Can't generate shorter term quotes.");
                else if (longer == QuoteType)
                    return this;

                return new QuoteCollection(this, longer);
            }
        }

        public Quote this[DateTimeOffset time]
        {
            get
            {
                double oaDate = time.UtcDateTime.ToOADate();
                int index = QuoteOADates.IndexOf(oaDate);

                return index != -1 ? DataCollection[index] : null;
            }

        }

        //public Outline this[int threshold]
        //{
        //     get
        //     {
        //         if (threshold < 1)
        //             return null;
        //         else if (threshold == Outline.DefaultThreshold)
        //             return BaseOutline;
        //         else
        //             return OutlineHelper.OutlineFromQuotes(this, threshold);
        //     }
        //}

        //public DateTimeOffset NearestOf(Double rough)
        //{
        //    if (rough <= Dates[0])
        //        return Since;
        //    else if (rough >= Dates.Last())
        //        return Until;
        //    else
        //    {
        //        Double dateValue = Dates.Find(a => a >= rough);

        //        DateTimeOffset nearest = new DateTimeOffset(DateTime.FromOADate(dateValue), TimeSpan.Zero);

        //        return nearest;
        //    }
        //}

        #endregion

        #endregion

        #region Constructors

        public QuoteCollection(string name, string source, RecordType type, List<Quote> quotes)
        {
            Name = name;
            Source = source;
            QuoteType = type;

            DataCollection = quotes;

            QuoteOADates = new List<double>();

            BaseOutline = OutlineHelper.OutlineFromQuotes(this, Outline.DefaultThreshold);

            summary();
        }

        public QuoteCollection(ContractInfomation contract, string source, RecordType type, List<Quote> quotes)
            : this(contract.ToString(), source, type, quotes)
        {
        }


        public QuoteCollection(QuoteCollection original, RecordType longer)
        {
            if (original.QuoteType > longer)
                throw new Exception();

            Name = original.Name;
            Source = original.Source;
            QuoteType = longer;

            QuoteOADates = new List<double>();

            DataCollection = FromShortTerms(original, longer);

            BaseOutline = OutlineHelper.OutlineFromQuotes(this, Outline.DefaultThreshold);

            summary();
        }

        #endregion

        private void summary()
        {
            double highest = double.MinValue, lowest = double.MaxValue;

            if (QuoteOADates.Count != 0)
                QuoteOADates.Clear();

            for (int i = 0; i < Count; i++)
            {
                if (highest < DataCollection[i].High)
                {
                    highest = DataCollection[i].High;
                    CelingIndex = i;
                }

                if (lowest > DataCollection[i].Low)
                {
                    lowest = DataCollection[i].Low;
                    FloorIndex = i;
                }

                //if (QuoteType >= RecordType.DayRecord)
                //    QuoteOADates.Add(DataCollection[i].Time.UtcDateTime.Time.ToOADate()+0.5);
                //else
                    QuoteOADates.Add(DataCollection[i].Time.UtcDateTime.ToOADate());
            }
        }

        public bool Contains(DateTimeOffset time)
        {
            if (time < Since || time > Until)
                return false;
            else
            {
                Double dateValue = time.UtcDateTime.ToOADate();
                return OutlineOADates.IndexOf(dateValue) != -1;
            }            
        }

        public void ValueRangeOf(int startIndex, int endIndex, out double ceiling, out double bottom)
        {
            ceiling = double.MinValue;
            bottom = double.MaxValue;

            if (startIndex < 0)
                startIndex = 0;

            if (endIndex > Count - 1)
                endIndex = Count - 1;

            for (int i = startIndex; i <= endIndex; i++)
            {
                Quote data = DataCollection[i];
                ceiling = Math.Max(data.High, ceiling);
                bottom = Math.Min(data.Low, bottom);
            }
        }

        public bool GetRangeOf(Double start, Double end, out double ceiling, out double bottom)
        {
            int startIndex = start <= OutlineOADates[0] ? 0 : QuoteOADates.FindIndex(x => x >= start);
            int endIndex = end >= OutlineOADates.Last() ? Count - 1 : QuoteOADates.FindIndex(x => x >= end);

            if (startIndex < Count && startIndex >= 0 && endIndex < Count && endIndex >= 0)
            {
                ValueRangeOf(startIndex, endIndex, out ceiling, out bottom);
                return true;
            }
            else
            {
                ceiling = -1;
                bottom = -1;
                return false;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}-{2}, {3} records.", Name, Since.ToString("yyyyMMdd"), Until.ToString("yyyyMMdd"), Count);
        }

        #region ICollection<Quote> 成员

        public void Add(Quote item)
        {
            if (item.Time < Since || item.Time > Until)
            {
                DataCollection.Add(item);
                summary();
            }
            else
                throw new Exception();
        }

        public void AddRange(IEnumerable<Quote> extraItems)
        {
            //extraItems = extraItems.Sort();
            if (this.Until < extraItems.First().Time)
            {
                DataCollection.AddRange(extraItems);
                summary();
            }
            else
                throw new Exception();

        }

        public void Clear()
        {
            DataCollection.Clear();
        }

        public bool Contains(Quote item)
        {
            return DataCollection.Contains(item);
        }

        public void CopyTo(Quote[] array, int arrayIndex)
        {
            if (array.Length < arrayIndex + Count)
            {
                throw new ArgumentException("The length of array is not enough to hold the whole contents");
            }

            for (int i = 0; i < Count; i++)
            {
                array[i + arrayIndex] = DataCollection[i];
            }
        }

        public int Count
        {
            get { return DataCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Quote item)
        {
            if (DataCollection.Contains(item))
                return DataCollection.Remove(item);
            else
                return false;
        }

        public void RemoveAt(int index)
        {
            DataCollection.RemoveAt(index);
        }

        #endregion

        #region IEnumerable<Quote> 成员

        public IEnumerator<Quote> GetEnumerator()
        {
            return DataCollection.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            return Clone();
        }

        #endregion
    }


    [Serializable]
    public class QuoteProperty
    {
        public List<PlanetId> Masters { get; set; }

        public double BaseRatio { get; set; }

        public List<OrbitDescription> LockedOrbits { get; set; }

        public QuoteProperty() 
        {
            Masters = new List<PlanetId>();
            BaseRatio = 1;
            LockedOrbits = new List<OrbitDescription>();
        }

        //public QuoteProperty(Double baseRatio, List<PlanetId> masters)
        //{
        //    BaseRatio = baseRatio;
        //    Masters = masters;
        //}
    }
}
