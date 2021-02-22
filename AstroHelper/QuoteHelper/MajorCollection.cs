using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    [Serializable]
    public class MajorCollection : ICollection<Major>
    {
        public DateTime LastMark { get; private set; }

        public List<Quote> Source { get; private set; }

        public int Threshold { get; private set; }

        public Dictionary<DateTimeOffset, Major> Prominents { get; private set; }

        public List<OutlineItem> Outline
        {
            get
            {
                if (Prominents == null) return null;
                else
                {
                    List<OutlineItem> outlines = new List<OutlineItem>();
                    foreach(KeyValuePair<DateTimeOffset, Major> major in Prominents)
                    {
                        outlines.Add(major.Value.Simplified);
                    }
                    return outlines;
                }
            }
        }

        public MajorCollection(List<Quote> dataCollection, int threshold)
        {
            Source = dataCollection;
            LastMark = DateTime.Now;
            Threshold = threshold;

            Prominents = TrendMarker.MajorsOf(dataCollection, threshold);
        }

        public Major this[DateTimeOffset time]
        {
            get
            {
                if (Prominents.ContainsKey(time))
                    return Prominents[time];
                else
                    return null;
            }
        }

        public Major this[int indexOfSource]
        {
            get
            {
                Quote theQuote = Source[indexOfSource];
                return this[theQuote.Date];
            }
        }

        public Major MajorBefore(DateTimeOffset time)
        {
            List<Major> pastTopBottoms = (
                from major in Prominents.Values
                where major.Date < time && (major.Kind == MajorType.Bottom || major.Kind == MajorType.Top)
                select major).ToList();

            return pastTopBottoms.Count == 0 ? null : pastTopBottoms.Last();                 
        }

        public Major MajorAfter(DateTimeOffset time)
        {
            List<Major> nextTopBottoms = (
                from major in Prominents.Values
                where major.Date > time && (major.Kind == MajorType.Bottom || major.Kind == MajorType.Top)
                select major).ToList();

            return nextTopBottoms.Count == 0 ? null : nextTopBottoms.First();
        }


        #region ICollection<Major> 成员

        public void Add(Major item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            Prominents.Clear();
        }

        public bool Contains(Major item)
        {
            return Prominents.ContainsValue(item);
        }

        public void CopyTo(Major[] array, int arrayIndex)
        {
            Prominents.Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Prominents.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(Major item)
        {
            return Prominents.Remove(item.Date);
        }

        #endregion

        #region IEnumerable<Major> 成员

        public IEnumerator<Major> GetEnumerator()
        {
            return Prominents.Values.GetEnumerator();
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
