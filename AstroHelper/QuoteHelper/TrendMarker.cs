using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum MovementKind
    {
        Inside,
        Outside,
        Up,
        Down
    }

    public class TrendMarker
    {
        public const int DefaultThreshold = 5;

        //public static Dictionary<DateTimeOffset, Major> MajorsOf(List<Quote> dataCollection, int threshold)
        //{
        //    TrendMarker marker = new TrendMarker(dataCollection, threshold);

        //    return marker.MajorQuotes;
        //}

        public static Outline OutlineFromQuotes(QuoteCollection quotes, int threshold)
        {
            TrendMarker marker = new TrendMarker(quotes.DataCollection, threshold);

            return marker.TheOutline;
        }

        public static SortedDictionary<DateTimeOffset, Double> OutlineOf(QuoteCollection quotes, int threshold)
        {
            TrendMarker marker = new TrendMarker(quotes.DataCollection, threshold);

            return marker.OutlineDictionary;
        }

        public static SortedDictionary<DateTimeOffset, Double> OutlineOf(Outline baseOutline, int threshold)
        {
            TrendMarker marker = new TrendMarker(baseOutline, threshold);

            return marker.OutlineDictionary;
        }


        #region Fields & Properties
        private List<Quote> DataCollection { get; set; }

        public int Threshold { get; private set; }

        private List<int> lowers = new List<int>();
        private List<int> highers = new List<int>();

        public int LastLow { get { return lowers.Last(); } }
        public int LastHigh { get { return highers.Last(); } }
        public int FirstLow
        {
            get { return lowers[0]; }
            set { lowers[0] = value; }
        }
        public int FirstHigh
        {
            get { return highers[0]; }
            set { highers[0] = value; }
        }

        //public Dictionary<DateTimeOffset, Major> MajorQuotes
        //{
        //    get 
        //    {
        //        Dictionary<DateTimeOffset, Major> items = new Dictionary<DateTimeOffset, Major>();
        //        Quote quote;

        //        MovementKind existingTrend = MovementKind.Inside;

        //        highers.Clear();
        //        lowers.Clear();

        //        for (int index = 1; index < DataCollection.Count - 1; index++)
        //        {
        //            quote = DataCollection[index];

        //            if (tryPushHigh(index))
        //            {
        //                if (highers.Count >= Threshold)
        //                {
        //                    if (existingTrend == MovementKind.Up)
        //                    {
        //                        items.Add(quote.Date, new Major(PivotType.High, quote));
        //                        lowers.Clear();
        //                    }
        //                    else if (existingTrend == MovementKind.Down)
        //                    {
        //                        if (items.ContainsKey(DataCollection[LastLow].Date))
        //                            items[DataCollection[LastLow].Date].Kind = PivotType.Bottom;
        //                        else
        //                            items.Add(DataCollection[LastLow].Date, new Major(PivotType.Bottom, DataCollection[LastLow]));
        //                        lowers.Clear();

        //                        existingTrend = MovementKind.Up;

        //                        for (int i = Threshold; i < highers.Count; i ++ )
        //                        {
        //                            if (!items.ContainsKey(DataCollection[highers[i]].Date))
        //                                items.Add(DataCollection[highers[i]].Date, new Major(PivotType.High, DataCollection[highers[i]]));
        //                        }
        //                    }
        //                    else if (quote.Low > DataCollection[index-1].Low)
        //                    {
        //                        lowers.Clear();

        //                        existingTrend = MovementKind.Up;

        //                        items.Add(DataCollection[FirstHigh].Date, new Major(PivotType.Bottom, DataCollection[FirstHigh]));
        //                        for (int i = Threshold; i < highers.Count; i++)
        //                        {
        //                            items.Add(DataCollection[highers[i]].Date, new Major(PivotType.High, DataCollection[highers[i]]));
        //                        }

        //                    }
        //                }
        //            }

        //            if (tryPushLow(index))
        //            {
        //                if (lowers.Count > Threshold)
        //                {
        //                    if (existingTrend == MovementKind.Down)
        //                    {
        //                        items.Add(quote.Date, new Major(PivotType.Low, quote));
        //                        highers.Clear();
        //                    }
        //                    else if (existingTrend == MovementKind.Up )
        //                    {
        //                        if (items.ContainsKey(DataCollection[LastHigh].Date))
        //                            items[DataCollection[LastHigh].Date].Kind = PivotType.Top;
        //                        else
        //                            items.Add(DataCollection[LastHigh].Date, new Major(PivotType.Top, DataCollection[LastHigh]));
        //                        highers.Clear();

        //                        existingTrend = MovementKind.Down;

        //                        for (int i = Threshold; i < lowers.Count; i++)
        //                        {
        //                            if (!items.ContainsKey(DataCollection[lowers[i]].Date))
        //                                items.Add(DataCollection[lowers[i]].Date, new Major(PivotType.Low, DataCollection[lowers[i]]));
        //                        }
        //                    }
        //                    else if ( quote.High <= DataCollection[index - 1].High)
        //                    {
        //                        highers.Clear();

        //                        existingTrend = MovementKind.Down;

        //                        items.Add(DataCollection[FirstLow].Date, new Major(PivotType.Top, DataCollection[FirstLow]));
        //                        for (int i = Threshold; i < lowers.Count; i++)
        //                        {
        //                            items.Add(DataCollection[lowers[i]].Date, new Major(PivotType.Low, DataCollection[lowers[i]]));
        //                        }

        //                    }
        //                }
        //            }
        //        }

        //        if (!items.ContainsKey(DataCollection.Last().Date))
        //        {
        //            if (existingTrend == MovementKind.Up)
        //                items.Add(DataCollection.Last().Date, new Major(PivotType.High, DataCollection.Last()));
        //            else if (existingTrend == MovementKind.Down)
        //                items.Add(DataCollection.Last().Date, new Major(PivotType.Low, DataCollection.Last()));
        //            else
        //                throw new Exception();
        //        }

        //        return items;
        //    }
        //}

        public SortedDictionary<DateTimeOffset, Double> OutlineDictionary
        {
            get
            {
                SortedDictionary<DateTimeOffset, Double> outline = new SortedDictionary<DateTimeOffset, Double>();
                Quote quote;
                DateTimeOffset date;

                MovementKind existingTrend = MovementKind.Inside;

                highers.Clear();
                lowers.Clear();

                for (int index = 1; index < DataCollection.Count - 1; index++)
                {
                    quote = DataCollection[index];

                    if (tryPushHigh(index))
                    {
                        if (highers.Count >= Threshold)
                        {
                            if (existingTrend == MovementKind.Up)
                            {
                                outline.Add(quote.Date, quote.High);
                                lowers.Clear();
                            }
                            else if (existingTrend == MovementKind.Down)
                            {
                                date = DataCollection[LastLow].Date;

                                if (!outline.ContainsKey(date))
                                    outline.Add(date, DataCollection[LastLow].Low);
                                else
                                    outline[date] = DataCollection[LastLow].Low;

                                lowers.Clear();

                                existingTrend = MovementKind.Up;

                                for (int i = Threshold; i < highers.Count; i++)
                                {
                                    if (!outline.ContainsKey(DataCollection[highers[i]].Date))
                                        outline.Add(DataCollection[highers[i]].Date, DataCollection[highers[i]].High);
                                }
                            }
                            else if (quote.Low > DataCollection[index - 1].Low)
                            {
                                lowers.Clear();

                                existingTrend = MovementKind.Up;

                                outline.Add(DataCollection[FirstHigh].Date, DataCollection[FirstHigh].High);
                                for (int i = Threshold; i < highers.Count; i++)
                                {
                                    outline.Add(DataCollection[highers[i]].Date, DataCollection[highers[i]].High);
                                }
                            }
                        }
                    }

                    if (tryPushLow(index))
                    {
                        if (lowers.Count > Threshold)
                        {
                            if (existingTrend == MovementKind.Down)
                            {
                                outline.Add(quote.Date, quote.Low);
                                highers.Clear();
                            }
                            else if (existingTrend == MovementKind.Up)
                            {
                                if (outline.ContainsKey(DataCollection[LastHigh].Date))
                                    outline[DataCollection[LastHigh].Date] = DataCollection[LastHigh].High;
                                else
                                    outline.Add(DataCollection[LastHigh].Date, DataCollection[LastHigh].High);
                                highers.Clear();

                                existingTrend = MovementKind.Down;

                                for (int i = Threshold; i < lowers.Count; i++)
                                {
                                    if (!outline.ContainsKey(DataCollection[lowers[i]].Date))
                                        outline.Add(DataCollection[lowers[i]].Date, DataCollection[lowers[i]].Low);
                                }
                            }
                            else if (quote.High <= DataCollection[index - 1].High)
                            {
                                highers.Clear();

                                existingTrend = MovementKind.Down;

                                outline.Add(DataCollection[FirstLow].Date, DataCollection[FirstLow].High);
                                for (int i = Threshold; i < lowers.Count; i++)
                                {
                                    outline.Add(DataCollection[lowers[i]].Date, DataCollection[lowers[i]].Low);
                                }

                            }
                        }
                    }
                }

                if (!outline.ContainsKey(DataCollection.Last().Date))
                {
                    if (existingTrend == MovementKind.Up)
                        outline.Add(DataCollection.Last().Date, DataCollection.Last().High);
                    else if (existingTrend == MovementKind.Down)
                        outline.Add(DataCollection.Last().Date, DataCollection.Last().Low);
                    else
                        throw new Exception();
                }

                return outline;
            }
        }

        public Outline TheOutline
        {
            get
            {
                SortedDictionary<DateTimeOffset, Double> items = new SortedDictionary<DateTimeOffset, Double>();
                List<OutlineItem> pivots = new List<OutlineItem>();
                OutlineItem newOutlineItem = null;

                Quote quote;
                DateTimeOffset date;

                MovementKind existingTrend = MovementKind.Inside;

                highers.Clear();
                lowers.Clear();

                for (int index = 1; index < DataCollection.Count - 1; index++)
                {
                    quote = DataCollection[index];

                    if (tryPushHigh(index))
                    {
                        if (highers.Count >= Threshold)
                        {
                            if (existingTrend == MovementKind.Up)
                            {
                                items.Add(quote.Date, quote.High);
                                lowers.Clear();
                            }
                            else if (existingTrend == MovementKind.Down)
                            {
                                date = DataCollection[LastLow].Date;

                                if (!items.ContainsKey(date))
                                    items.Add(date, DataCollection[LastLow].Low);
                                else
                                    items[date] = DataCollection[LastLow].Low;

                                newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], index, PivotType.Bottom);
                                pivots.Add(newOutlineItem);

                                lowers.Clear();

                                existingTrend = MovementKind.Up;

                                for (int i = Threshold; i < highers.Count; i++)
                                {
                                    if (!items.ContainsKey(DataCollection[highers[i]].Date))
                                        items.Add(DataCollection[highers[i]].Date, DataCollection[highers[i]].High);
                                }
                            }
                            else if (quote.Low > DataCollection[index - 1].Low)
                            {
                                lowers.Clear();

                                existingTrend = MovementKind.Up;

                                items.Add(DataCollection[FirstHigh].Date, DataCollection[FirstHigh].High);
                                for (int i = Threshold; i < highers.Count; i++)
                                {
                                    items.Add(DataCollection[highers[i]].Date, DataCollection[highers[i]].High);
                                }
                            }
                        }
                    }

                    if (tryPushLow(index))
                    {
                        if (lowers.Count > Threshold)
                        {
                            if (existingTrend == MovementKind.Down)
                            {
                                items.Add(quote.Date, quote.Low);
                                highers.Clear();
                            }
                            else if (existingTrend == MovementKind.Up)
                            {
                                date = DataCollection[LastHigh].Date;

                                if (items.ContainsKey(date))
                                    items[date] = DataCollection[LastHigh].High;
                                else
                                    items.Add(date, DataCollection[LastHigh].High);
                                highers.Clear();

                                newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], index, PivotType.Top);
                                pivots.Add(newOutlineItem);

                                existingTrend = MovementKind.Down;

                                for (int i = Threshold; i < lowers.Count; i++)
                                {
                                    if (!items.ContainsKey(DataCollection[lowers[i]].Date))
                                        items.Add(DataCollection[lowers[i]].Date, DataCollection[lowers[i]].Low);
                                }
                            }
                            else if (quote.High <= DataCollection[index - 1].High)
                            {
                                highers.Clear();

                                existingTrend = MovementKind.Down;

                                items.Add(DataCollection[FirstLow].Date, DataCollection[FirstLow].High);
                                for (int i = Threshold; i < lowers.Count; i++)
                                {
                                    items.Add(DataCollection[lowers[i]].Date, DataCollection[lowers[i]].Low);
                                }

                            }
                        }
                    }
                }

                if (pivots.Count == 0)
                {
                    return null;
                }
                if (existingTrend == MovementKind.Up)
                {
                    date = DataCollection[LastHigh].Date;

                    if (!items.ContainsKey(date))
                        items.Add(date, DataCollection[LastHigh].High);
                    else
                        items[date] = DataCollection[LastHigh].High;

                    if (pivots.Last().Date < date)
                    {
                        newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], LastHigh, PivotType.Top);
                        pivots.Add(newOutlineItem);
                        existingTrend = MovementKind.Down;
                    }
                }
                else if (existingTrend == MovementKind.Down)
                {
                    date = DataCollection[LastLow].Date;

                    if (!items.ContainsKey(date))
                        items.Add(date, DataCollection[LastLow].Low);
                    else
                        items[date] = DataCollection[LastLow].Low;

                    if (pivots.Last().Date < date)
                    {
                        newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], LastLow, PivotType.Bottom);
                        pivots.Add(newOutlineItem);
                        existingTrend = MovementKind.Up;
                    }
                }

                if (!items.ContainsKey(DataCollection.Last().Date))
                {
                    DateTimeOffset time = DataCollection.Last().Date;
                    if (existingTrend == MovementKind.Up)
                    {
                        items.Add(DataCollection.Last().Date, DataCollection.Last().High);
                        //pivots.Add(new OutlineItem(time.UtcDateTime.ToOADate(), items[time], DataCollection.Count - 1, PivotType.Top));
                    }
                    else if (existingTrend == MovementKind.Down)
                    {
                        items.Add(DataCollection.Last().Date, DataCollection.Last().Low);
                        //pivots.Add(new OutlineItem(time.UtcDateTime.ToOADate(), items[time], DataCollection.Count - 1, PivotType.Bottom));
                    }
                    //else
                    //    throw new Exception();
                }

                return new Outline(Threshold, items, pivots);
            }
        }

        #endregion

        public TrendMarker(List<Quote> dataCollection, int threshold)
        {
            DataCollection = dataCollection;
            Threshold = threshold;
        }

        public TrendMarker(Outline baseOutline, int threshold)
        {
            DataCollection = new List<Quote>();
            Threshold = threshold;

            foreach (KeyValuePair<DateTimeOffset, Double>kvp in baseOutline.Sequences )
            {
                DataCollection.Add(new Quote(kvp.Key, kvp.Value));
            }
        }

        #region Functions

        private MovementKind MovementFrom(int index)
        {
            bool lower = DataCollection[index + 1].Low < DataCollection[index].Low;
            bool higher = DataCollection[index + 1].High > DataCollection[index].High;

            if (lower && higher)
                return MovementKind.Outside;
            else if (lower)
                return MovementKind.Down;
            else if (higher)
                return MovementKind.Up;
            else
                return MovementKind.Inside;
        }

        private void resetHighersTo(int index)
        {
            highers.Clear();
            highers.Add(index);
        }

        private void resetLowersTo(int index)
        {
            lowers.Clear();
            lowers.Add(index);
        }

        private bool tryPushHigh(int index)
        {
            if (highers.Count == 0)
            {
                highers.Add(index - 1);
            }
            
            if (DataCollection[LastHigh].High < DataCollection[index].High)
            {
                highers.Add(index);
                return true;
            }
            return false;
        }

        private bool tryPushLow(int index)
        {
            if (lowers.Count == 0)
            {
                lowers.Add(index - 1);
            }
            
            if ( DataCollection[LastLow].Low > DataCollection[index].Low)
            {
                lowers.Add(index);
                return true;
            }
            return false;
        }

        #endregion

    }
}
