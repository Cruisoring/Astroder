using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    [Serializable]
    public class QuoteCollection : ICollection<Quote> //, IPointList
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
            { RecordType.MonthRecord, "yyyy-MMMM"},
            { RecordType.QuarterRecord, "yyyy-MM" },
            { RecordType.YearRecord, "yyyy"},
            { RecordType.Unknown, "yyyyMMdd, ddd"}
        };

        #endregion

        #region Properties

        #region Variables

        public string Name { get; set; }

        private string description = null;
        public string Description {
            get { return description == null ? QuoteType.ToString() + " of " + Name : description; }
            set { description = value; } 
        }

        public RecordType QuoteType { get; set; }

        public string DateTimeFormat { get { return DefaultDateTimeFormats[QuoteType]; } }

        public List<Quote> DataCollection { get; private set; }

        public DateTimeOffset Since { get { return DataCollection[0].Date; } }

        public DateTimeOffset Until { get { return DataCollection[Count - 1].Date; } }

        public Outline ShortTermOutline { get; private set; }

        public Outline CurrentOutline { get; set; }

        public List<Double> OutlineValues { get; set; }

        public List<Double> Dates { get; private set; }

        #endregion

        #region Properties for version control
        public DateTime LastModify { get; private set; }

        public DateTime LastSummary { get; private set; }

        private bool isUpToDate
        {
            get { return LastSummary >= LastModify; }
        }

        #endregion

        #region Indexer

        private List<QuoteCollection> longTerms = new List<QuoteCollection>();

        public QuoteCollection this[RecordType longer]
        {
            get
            {
                if (longer < QuoteType)
                    throw new Exception();
                else if (longer == QuoteType)
                    return this;

                QuoteCollection result = null;

                foreach (QuoteCollection longQuotes in longTerms)
                {
                    if (longQuotes.QuoteType == longer)
                    {
                        result = longQuotes;
                    }
                }

                if (result == null)
                {
                    result = new QuoteCollection(this, longer);
                    longTerms.Add(result);
                }
                else if (result.LastModify < LastModify)
                {
                    longTerms.Clear();
                    result = new QuoteCollection(this, longer);
                    longTerms.Add(result);
                }

                return result;

            }
        }

        public Quote this[DateTimeOffset time]
        {
            get
            {
                int index = indexOf(time);

                return index != -1 ? DataCollection[index] : null;
            }

        }

        public DateTimeOffset NearestOf(Double rough)
        {
            if (rough <= Dates[0])
                return Since;
            else if (rough >= Dates.Last())
                return Until;
            else
            {
                Double dateValue = Dates.Find(a => a >= rough);

                DateTimeOffset nearest = new DateTimeOffset(DateTime.FromOADate(dateValue), TimeSpan.Zero);

                return nearest;
            }
        }

        #endregion

        #region Deducted items

        private int ceilingIndex, floorIndex;
        public int CelingIndex { 
            get 
            {
                if (!isUpToDate)
                    summarize();
                return ceilingIndex;
            }
            private set { ceilingIndex = value; } 
        }

        public int FloorIndex
        {
            get
            {
                if (!isUpToDate)
                    summarize();
                return floorIndex;
            }
            private set { floorIndex = value; }
        }

        public double Ceiling { get { return DataCollection[CelingIndex].High; } }

        public double Floor { get { return DataCollection[FloorIndex].Low; } }

        //private List<MajorCollection> majorsCollection = new List<MajorCollection>();
        //public MajorCollection MajorsOf(int threshold)
        //{
        //    MajorCollection result = null;

        //    foreach (MajorCollection items in majorsCollection)
        //    {
        //        if (items.Threshold == threshold)
        //        {
        //            result = items;
        //        }
        //    }

        //    if (result == null)
        //    {
        //        result = new MajorCollection(DataCollection, threshold);
        //        majorsCollection.Add(result);
        //    }
        //    else if (result.LastMark < LastModify)
        //    {
        //        majorsCollection.Clear();
        //        result = new MajorCollection(DataCollection, threshold);
        //        majorsCollection.Add(result);
        //    }

        //    return result;
        //}

        //public List<OutlineItem> OutlineDictionary
        //{
        //    get { return MajorsOf(Threshold).OutlineDictionary; }
        //}

        #endregion

        #endregion

        #region Constructors

        public QuoteCollection(string name, List<Quote> quotes ) //, ValueType yType)
        {
            Name = name;
            //YData = yType;
            //Threshold = TrendMarker.DefaultThreshold;

            DataCollection = quotes;
            DataCollection.Sort();
            LastModify = DateTime.Now;

            QuoteType = GuessQuoteType(DataCollection);

            ShortTermOutline = TrendMarker.OutlineFromQuotes(this, 1);
            CurrentOutline = TrendMarker.OutlineFromQuotes(this, TrendMarker.DefaultThreshold);
            OutlineValues = getDefaultValues();
            Dates = getDates();
        }
        
        public QuoteCollection(QuoteCollection original, RecordType longer)
        {
            if (QuoteType > longer)
                throw new Exception();

            Name = original.Name;
            QuoteType = longer;
            //DateTimeFormat = DefaultDateTimeFormats[QuoteType];

            DataCollection = FromShortTerms(original, longer);

            getCeilingFloor();

            //YData = original.YData;
            //Threshold = original.Threshold;
            LastModify = DateTime.Now;
            ShortTermOutline = TrendMarker.OutlineFromQuotes(this, 1);
            CurrentOutline = TrendMarker.OutlineFromQuotes(this, TrendMarker.DefaultThreshold);

            if (CurrentOutline == null)
                CurrentOutline = ShortTermOutline;

            OutlineValues = getDefaultValues();
            Dates = getDates();
        }

        #endregion

        private void summarize()
        {
            getCeilingFloor();

            LastSummary = DateTime.Now;
        }

        private List<Double> getDefaultValues()
        {
             List<double> prices = new List<double>();
            int pivotIndex = 0;
            bool isDowning = false;

            if (CurrentOutline == null || CurrentOutline.Count == 0)
                return prices;
            if (CurrentOutline.Count < 3 || CurrentOutline[1] == CurrentOutline[0])
                return prices;
            else
                isDowning = CurrentOutline[0] < CurrentOutline[1];

            List<DateTimeOffset> pivotDates = new List<DateTimeOffset>();

            foreach (DateTimeOffset time in CurrentOutline.Sequences.Keys)
            {
                pivotDates.Add(time);
            }

            DateTimeOffset nextPivotDate = pivotDates[pivotIndex];
            for (int i = 0; i < Count; i ++)
            {
                Quote quote = DataCollection[i];
                if (quote.Date == nextPivotDate)
                {
                    prices.Add(CurrentOutline[nextPivotDate]);

                    //pivotIndex++;
                    //nextPivotDate = pivotDates[pivotIndex];
                    //isDowning = CurrentOutline[nextPivotDate] < CurrentOutline[pivotIndex - 1];
                    if (pivotIndex == CurrentOutline.Count - 1)
                    {
                        if (i == Count - 1)
                            return prices;
                        else
                            throw new Exception();
                    }
                    else
                    {
                        pivotIndex++;
                        nextPivotDate = pivotDates[pivotIndex];
                        isDowning = CurrentOutline[nextPivotDate] < CurrentOutline[pivotIndex - 1];
                    }
                }
                else
                {
                    if (isDowning)
                        prices.Add(quote.Low);
                    else
                        prices.Add(quote.High);
                }
            }

            return prices;
        }

        private List<Double> getDates()
        {
            List<double> dates = new List<double>();
            DateTimeOffset start = DataCollection[0].Date;
            Double first = start.UtcDateTime.ToOADate();

            for (int i = 0; i < Count; i++)
            {
                dates.Add(DataCollection[i].Date.UtcDateTime.ToOADate());
            }

            return dates;
        }

        private void getCeilingFloor()
        {
            double highest = double.MinValue, lowest = double.MaxValue;

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
            }
        }

        public bool Contains(DateTimeOffset time)
        {
            if (time < Since || time > Until)
                return false;
            else
                return indexOf(time) != -1;
        }

        private int indexOf(DateTimeOffset date)
        {
            if (date < Since || date > Until) 
                return Count - 1;
            else
            {
                Double dateValue = date.UtcDateTime.ToOADate();
                return Dates.IndexOf(dateValue);
            }
        }

        public Outline OutlineOf(int threshold)
        {
            if (threshold < 1)
                return null;
            else if (threshold == 1)
                return ShortTermOutline;
            else if (threshold == TrendMarker.DefaultThreshold)
                return CurrentOutline;
            else
                return TrendMarker.OutlineFromQuotes(this, threshold);
        }

        public void ValueRangeOf( int startIndex, int endIndex, out double ceiling, out double bottom)
        {
            ceiling = double.MinValue;
            bottom = double.MaxValue;

            for (int i = startIndex; i <= endIndex; i ++ )
            {
                Quote data = DataCollection[i];
                ceiling = Math.Max(data.High, ceiling);
                bottom = Math.Min(data.Low, bottom);
            }
        }

        public bool GetRangeOf(Double start, Double end, out double ceiling, out double bottom)
        {
            int startIndex = start <= Dates[0] ? 0 : Dates.FindIndex(x => x >= start);
            int endIndex = end >= Dates.Last() ? Count - 1 : Dates.FindIndex(x => x >= end);

            if (startIndex < Count && startIndex >= 0 && endIndex < Count && endIndex >= 0 )
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

        #region ICollection<Quote> 成员

        public void Add(Quote item)
        {
            if (item.Date < Since || item.Date > Until)
            {
                DataCollection.Add(item);
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

            for (int i = 0; i < Count; i ++ )
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

        //public QuoteCollection Clone()
        //{
        //    return QuoteCollection(this);
        //}

        public object Clone()
        {
            return Clone();
        }

        #endregion

        #region Static Functions

        private delegate int periodDelegate(Quote lhs);

        private static int yearValueOf(Quote lhs)
        {
            return lhs.Date.Year;
        }

        private static int quarterValueOf(Quote lhs)
        {
            return lhs.Date.Year * 4 + (lhs.Date.Month / 3);
        }

        private static int monthValueOf(Quote lhs)
        {
            return lhs.Date.Year* 12 + lhs.Date.Month;
        }

        private static int weekValueOf(Quote lhs)
        {
            return lhs.Date.Year * 52 + lhs.Date.DayOfYear / 7;
        }

        private static int dayValueOf(Quote lhs)
        {
            return lhs.Date.Year * 365 + lhs.Date.DayOfYear;
        }

        public static List<Quote> FromShortTerms(QuoteCollection shorters, RecordType longerType)
        {
            if (longerType <= shorters.QuoteType)
                throw new Exception();
            else if (longerType <= RecordType.DayRecord)
                throw new NotImplementedException();
            else if (shorters.QuoteType == RecordType.WeekRecord)
                throw new NotImplementedException();

            periodDelegate differenceComparer = null;
            
            switch(longerType)
            {
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
                from item in shorters.DataCollection
                let period = differenceComparer(item)
                group item by period into clusters
                orderby clusters.Key
                select clusters;

            foreach (var cluster in groupQuery)
            {
                double open = cluster.First().Open, high = double.MinValue, low = double.MaxValue, close = cluster.Last().Close, volume=0;
                DateTimeOffset lastTime = cluster.Last().Date;

                foreach(Quote item in cluster)
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
                return RecordType.Unknown;
            else
            {
                TimeSpan span = quotes[1].Date - quotes[0].Date;
                TimeSpan minSpan = span, maxSpan = span;

                for (int i = 1; i < 4; i++)
                {
                    span = quotes[i + 1].Date - quotes[i].Date;

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
                            return RecordType.Unknown;
                }
            }
        }

        #endregion
    }
}
