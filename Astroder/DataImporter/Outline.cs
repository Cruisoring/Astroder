using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DataImporter
{
    public enum PivotType
    {
        Unknown,
        Low,
        High,
        Bottom,
        Top
    }

    public enum MovementKind
    {
        Inside,
        Outside,
        Up,
        Down
    }
    
    [Serializable]
    public class OutlineItem : IEquatable<OutlineItem>
    {
        [System.Xml.Serialization.XmlElement("Time")]
        public string TimeForXml
        {
            get { return Time.ToString(); }
            set { Time = DateTimeOffset.Parse(value); }
        }
        [System.Xml.Serialization.XmlIgnore]
        public DateTimeOffset Time { get; set; }

        public double DateValue { get { return Time.UtcDateTime.ToOADate(); } }
        public double Price { get;  set; }
        public int RecordIndex { get;  set; }
        public PivotType Type { get;  set; }

        public OutlineItem() {}

        public OutlineItem(DateTimeOffset time, Double price, int index, PivotType type)
        {
            Price = price;
            Time = time;
            Type = type;
            RecordIndex = index;
        }

        public OutlineItem(OutlineItem original, double newPrice)
        {
            Price = newPrice;
            Time = original.Time;
            Type = PivotType.Unknown;
            RecordIndex = original.RecordIndex;
        }

        public override string ToString()
        {
            //return String.Format("{0}({1}, {2}): {3}", Type, Time.ToString("yyyy-MM-dd"), RecordIndex, Price);
            return String.Format("{0}{1}[{2}]", Time.ToString("yyyyMMdd"), Type, Price);
        }


        #region IEquatable<OutlineItem> 成员

        public bool Equals(OutlineItem other)
        {
            return Math.Abs(DateValue - other.DateValue) < 1 && Price == other.Price;
        }

        #endregion
    }

    [Serializable]
    public class Outline : ICollection<DateTimeOffset>
    {
        public static int DefaultThreshold = 2;

        public static Outline OutlineOf(QuoteCollection quotes, int threshold)
        {
            return OutlineHelper.OutlineFromQuotes(quotes, threshold);
        }

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

        public IEnumerable Tops
        {
            get
            {
                foreach(OutlineItem item in Pivots)
                {
                    if(item.Type == PivotType.Top)
                        yield return item;
                }
            }
        }

        public IEnumerable Bottoms
        {
            get
            {
                foreach (OutlineItem item in Pivots)
                {
                    if (item.Type == PivotType.Bottom)
                        yield return item;
                }
            }
        }

        public IEnumerable Turnings
        {
            get
            {
                foreach (OutlineItem item in Pivots)
                {
                    if (item.Type == PivotType.Bottom || item.Type == PivotType.Top)
                        yield return item;
                }
            }
        }


        #endregion

        #region Constructors

        public Outline(int threshold, SortedDictionary<DateTimeOffset, Double> sequence, List<OutlineItem> pivots)
        {
            Threshold = threshold;
            Sequences = sequence;

            if (pivots.Count == 0)
                Console.WriteLine("The Pivots contains no valid data!");

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

        #endregion

        #region Indexer
        public OutlineItem this[DateTimeOffset date]
        {
            get 
            {
                double oaDate = date.UtcDateTime.ToOADate();
                return this[oaDate];
            }
        }

        public OutlineItem this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    return null;
                else
                    return Pivots[index];
            }
        }

        public OutlineItem this[double oaDate]
        {
            get
            {
                if (!PivotDates.Contains(oaDate))
                    return null;

                int index = PivotDates.IndexOf(oaDate);
                return Pivots[index];
            }
        }

        #endregion

        #region Functions

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

        public override string ToString()
        {
            return String.Format("Outline({0}): from {1} to {2}, {3} swings between {4} and {5}",
                Threshold, Since.UtcDateTime, Until.UtcDateTime, Count, Min, Max);
        }

        #region Trend Helper


        #endregion

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
            if (Sequences.ContainsKey(item))
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

        #endregion
    }

}
