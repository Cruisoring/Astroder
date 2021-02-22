using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockQuote
{
    public enum TypeOfTurning
    {
        Bottom,
        Top
    }

    public class TurningPoint<T> : IComparable<TurningPoint<T>>
        where T : RecordBase
    {
        public TypeOfTurning Kind { get; private set; }

        public T Record { get; set; }

        public DateTime Date { get { return Record.Date; } }
        public double Value { get { return (Kind == TypeOfTurning.Top) ? Record.High : Record.Low; } }

        public TurningPoint(TypeOfTurning kind, T record)
        {
            Kind = kind;
            Record = record;
        }

        public override string ToString()
        {
            return String.Format("{0}({1}): {2}", Date.ToString(Record.TimeFormat), Kind, Value);
        }

        #region IComparable<TurningPoint<T>> 成员

        public int CompareTo(TurningPoint<T> other)
        {
            return Record.CompareTo(other.Record);
        }

        #endregion

        public static string SummaryOf(List<TurningPoint<T>> turnings)
        {
            turnings.Sort();

            int count = turnings.Count;

            if (count < 2)
                Console.WriteLine("Only {0} turnings are included in the list." ) ;

            int highestIndex = -1, lowestIndex = -1, bottomCount = 0, topCount = 0;

            double highest = double.MinValue, lowest = double.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (turnings[i].Kind == TypeOfTurning.Bottom)
                {
                    bottomCount++;
                    if (lowest > turnings[i].Value)
                    {
                        lowestIndex = i;
                        lowest = turnings[i].Value;
                    }
                }
                else // if (turnings[currentIndex].Kind == TypeOfTurning.Top)
                {
                    topCount++;
                    if (highest < turnings[i].Value)
                    {
                        highest = turnings[i].Value;
                        highestIndex = i;
                    }
                }
            }

            return String.Format("{0} turnings, {1} tops, {2} bottoms, highest={3}@{4}, lowest={5}@{6}",
                count, topCount, bottomCount, highest, turnings[highestIndex].Date, lowest, turnings[lowestIndex].Date);
        }
    }

}
