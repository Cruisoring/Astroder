using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum PivotType
    {
        Unknown,
        Low,
        High,
        Bottom,
        Top
    }

    public class OutlineItem
    {
        public double DateValue { get; private set; }
        public double Price { get; private set; }
        public int RecordIndex { get; private set; }
        public DateTimeOffset Date { get { return new DateTimeOffset(DateTime.FromOADate(DateValue), TimeSpan.Zero); } }
        public PivotType Type { get; private set; }

        public OutlineItem(Double dateValue, Double price, int index, PivotType type)
        {
            Price = price;
            DateValue = dateValue;
            Type = type;
            RecordIndex = index;
        }

        public OutlineItem(OutlineItem original, double newPrice)
        {
            Price = newPrice;
            DateValue = original.DateValue;
            Type = original.Type;
            RecordIndex = original.RecordIndex;
        }

        public override string ToString()
        {
            return String.Format("{0} on {1}: price = {2}, index = {3}", Type, Date.ToString("yyyy-MM-dd"), Price, RecordIndex);
        }

        public static OutlineChanges operator -(OutlineItem lhs, OutlineItem rhs)
        {
            return new OutlineChanges(lhs.Price - rhs.Price, lhs.RecordIndex - rhs.RecordIndex, lhs.Date - rhs.Date);
        }
    }

    public class OutlineChanges
    {
        public double PriceChanged { get; private set; }
        public int IndexChanged { get; private set; }
        public TimeSpan TimeElapsed { get; private set; }
        public OutlineChanges(double priceChanged, int indexDif, TimeSpan elapsed)
        {
            PriceChanged = priceChanged;
            IndexChanged = indexDif;
            TimeElapsed = elapsed;
        }

        public override string ToString()
        {
            return String.Format("Price changed {0}, {1} {2} days ({3} trading).", PriceChanged, 
                TimeElapsed.TotalDays > 0 ? "after" : "before", Math.Abs(TimeElapsed.TotalDays), Math.Abs(IndexChanged));
        }
    }

    [Serializable]
    public class Outline : ICollection<DateTimeOffset>
    {
        #region Variables
        public int Threshold { get; private set; }

        public SortedDictionary<DateTimeOffset, Double> Sequences { get; private set; }

        public List<Double> Values { get; private set; }

        public List<Double> Dates { get; private set; }

        public Double Max { get; private set; }

        public Double Min { get; private set; }

        public DateTimeOffset Since { get { return Sequences.FirstOrDefault().Key; } }

        public DateTimeOffset Until { get { return Sequences.LastOrDefault().Key; } }

        public List<OutlineItem> Pivots { get; private set; }

        public List<Double> PivotValues
        {
            get 
            {
                return (from pivot in Pivots
                        select pivot.Price).ToList();
            }
        }

        public List<Double> PivotDates
        {
            get
            {
                return (from pivot in Pivots
                        select pivot.DateValue).ToList();
            }
        }

        #endregion

        #region Constructors

        public Outline(int threshold, SortedDictionary<DateTimeOffset, Double> sequence, List<OutlineItem> pivots)
        {
            Threshold = threshold;
            Sequences = sequence;

            if (pivots.Count == 0)
                Console.WriteLine("The pivots contains no valid data!");

            Pivots = pivots;
            Values = Sequences.Values.ToList();
            Max = Values.Count != 0 ? Values.Max() : 0;
            Min = Values.Count != 0 ? Values.Min() : 0;
            Dates = new List<double>();

            foreach (DateTimeOffset date in Sequences.Keys)
            {
                Dates.Add(date.UtcDateTime.ToOADate());
            }            
        }

        //public Outline(QuoteCollection quotes, int threshold)
        //{
        //    Threshold = threshold;
        //    Sequences = TrendMarker.OutlineOf(quotes, threshold);
        //    Values = Sequences.Values.ToList();
        //    Max = Values.Max();
        //    Min = Values.Min();
        //    Dates = new List<double>();

        //    foreach (DateTimeOffset date in Sequences.Keys)
        //    {
        //        Dates.Add(date.UtcDateTime.ToOADate());
        //    }

        //    Pivots = new List<OutlineItem>();
        //    OutlineItem item = null;

        //    for (int i = 0; i < Count; i++)
        //    {
        //        item = new OutlineItem(Dates[i], Values[i], i, KindOf(i));
        //        if (item.Type == PivotType.Top || item.Type == PivotType.Bottom)
        //            Pivots.Add(item);
        //    }
        //}

        //public Outline(Outline baseOutline, int threshold)
        //{
        //    if (baseOutline.Threshold != 1)
        //        throw new Exception();

        //    Threshold = threshold;
        //    Sequences = TrendMarker.OutlineOf(baseOutline, threshold);
        //    Values = Sequences.Values.ToList();
        //    Max = Values.Max();
        //    Min = Values.Min();

        //    Dates = new List<double>();

        //    foreach (DateTimeOffset date in Sequences.Keys)
        //    {
        //        Dates.Add(date.UtcDateTime.ToOADate());
        //    }

        //    Pivots = new List<OutlineItem>();
        //    OutlineItem item = null;

        //    for (int i = 0; i < Count; i ++ )
        //    {
        //        item = new OutlineItem(Dates[i], Values[i], i, KindOf(i));
        //        if (item.Type == PivotType.Top || item.Type == PivotType.Bottom)
        //            Pivots.Add(item);
        //    }
        //}

        #endregion

        #region Indexer
        public Double this[DateTimeOffset date]
        {
            get { return Sequences.ContainsKey(date) ? Sequences[date] : Double.MinValue; }
        }

        public Double this[int index]
        {
            get 
            {
                if(index < 0 || index >= Count)
                    return Double.MinValue;
                else
                {
                    DateTimeOffset date = new DateTimeOffset(DateTime.FromOADate(Dates[index]), TimeSpan.Zero);
                    return this[date];
                }
            }
        }

        #endregion

        //public OutlineItem ItemAt(int index)
        //{
        //    return new OutlineItem(Dates[index], Values[index], index, KindOf(index));
        //}

        public int IndexOf(DateTimeOffset date)
        {
            if (!Contains(date))
                return -1;
            else
            {
                Double dateValue = date.UtcDateTime.ToOADate();
                return Dates.IndexOf(dateValue);
            }
        }

        //public PivotType KindOf(DateTimeOffset date)
        //{
        //    if (date == Since)
        //        return this[0] > this[1] ? PivotType.Top : PivotType.Bottom;
        //    else if (date == Until)
        //        return PivotType.Unknown;
        //    else
        //    {
        //        int index = IndexOf(date);
        //        if (this[date] > this[index + 1])
        //            return this[date] > this[index - 1] ? PivotType.Top : PivotType.Low;
        //        else
        //            return this[date] > this[index - 1] ? PivotType.High : PivotType.Bottom;
        //    }
        //}

        //public PivotType KindOf(int index)
        //{
        //    if (index < 0 || index >= Count)
        //        throw new Exception();

        //    DateTimeOffset date = new DateTimeOffset(DateTime.FromOADate(Dates[index]), TimeSpan.Zero);

        //    return KindOf(date);
        //}

        public override string ToString()
        {
            return String.Format("Outline({0}): from {1} to {2}, {3} swings between {4} and {5}",
                Threshold, Since.UtcDateTime, Until.UtcDateTime, Count, Min, Max);
        }

        #region ICollection<OutlineItem> 成员

        public void Add(DateTimeOffset item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            Sequences.Clear();
        }

        public bool Contains(DateTimeOffset item)
        {
            return Sequences.ContainsKey(item);
        }

        public void CopyTo(DateTimeOffset[] array, int arrayIndex)
        {
            Sequences.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Sequences.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DateTimeOffset item)
        {
            if(Sequences.ContainsKey(item))
            {
                Sequences.Remove(item);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<OutlineItem> 成员

        public IEnumerator<DateTimeOffset> GetEnumerator()
        {
            foreach (DateTimeOffset item in Sequences.Keys)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
