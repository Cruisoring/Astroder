using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImporter
{
    public class OutlineHelper
    {
        public static Outline OutlineFromQuotes(QuoteCollection quotes, int threshold)
        {
            OutlineHelper marker = new OutlineHelper(quotes.DataCollection, threshold);

            return marker.TheOutline;
        }

        #region Fields & Properties
        private List<Quote> DataCollection { get; set; }

        public int Threshold { get; private set; }

        private List<int> lowers = new List<int>();
        private List<int> highers = new List<int>();

        public int LastLow { get { return lowers.LastOrDefault(); } }
        public int LastHigh { get { return highers.LastOrDefault(); } }
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

                for (int index = 1; index < DataCollection.Count; index++)
                {
                    quote = DataCollection[index];

                    if (existingTrend == MovementKind.Down)
                    {
                        if (tryPushLow(index))      //Still down, clear the highers
                        {
                            items.Add(quote.Time, quote.Low);
                            highers.Clear();
                        }
                        else if (tryPushHigh(index))
                        {
                            if (highers.Count > Threshold)      //Trend changed from Down to Up
                            {
                                //Trend is changed from Downward to Upward
                                existingTrend = MovementKind.Up;

                                changeToUp(items, pivots, LastLow);
                            }
                        }
                    }
                    else if (existingTrend == MovementKind.Up)
                    {
                        if (tryPushHigh(index))      //Still up, clear the lowers
                        {
                            items.Add(quote.Time, quote.High);
                            lowers.Clear();
                        }
                        else if (tryPushLow(index))
                        {
                            if (lowers.Count > Threshold)      //Trend changed from Up to Down
                            {
                                existingTrend = MovementKind.Down;

                                changeToDown(items, pivots, LastHigh);
                            }
                        }
                    }
                    else    //For trend undetermined state
                    {
                        if (tryPushHigh(index) && highers.Count >= Threshold && quote.Low >= DataCollection[LastLow].Low)    //Trend changed from undetermined to Upward
                        {
                            existingTrend = MovementKind.Up;

                            changeToUp(items, pivots, 0);                            
                        }
                        else if (tryPushLow(index) && lowers.Count > Threshold && quote.High <= DataCollection[LastHigh].High) //Trend change from undetermined to downward
                        {
                            existingTrend = MovementKind.Down;

                            changeToDown(items, pivots, 0);                          
                        }
                    }

                    //if (tryPushHigh(index))
                    //{
                    //    //if (existingTrend == MovementKind.Down && tryPushLow(index))
                    //    //{
                    //    //    items.Add(quote.Time, quote.Low);
                    //    //    highers.Clear();
                    //    //}
                    //    //else 
                    //    if (highers.Count >= Threshold)
                    //    {
                    //        if (existingTrend == MovementKind.Up)
                    //        {
                    //            items.Add(quote.Time, quote.High);
                    //            lowers.Clear();
                    //        }
                    //        else if (existingTrend == MovementKind.Down)
                    //        {
                    //            date = DataCollection[LastLow].Time;

                    //            if (!items.ContainsKey(date))
                    //                items.Add(date, DataCollection[LastLow].Low);
                    //            else
                    //                items[date] = DataCollection[LastLow].Low;

                    //            newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], index, PivotType.Bottom);
                    //            pivots.Add(newOutlineItem);

                    //            lowers.Clear();

                    //            existingTrend = MovementKind.Up;

                    //            for (int i = Threshold; i < highers.Count; i++)
                    //            {
                    //                if (!items.ContainsKey(DataCollection[highers[i]].Time))
                    //                    items.Add(DataCollection[highers[i]].Time, DataCollection[highers[i]].High);
                    //            }
                    //        }
                    //        else if (quote.Low > DataCollection[index - 1].Low)
                    //        {
                    //            lowers.Clear();

                    //            existingTrend = MovementKind.Up;

                    //            items.Add(DataCollection[FirstHigh].Time, DataCollection[FirstHigh].High);
                    //            for (int i = Threshold; i < highers.Count; i++)
                    //            {
                    //                items.Add(DataCollection[highers[i]].Time, DataCollection[highers[i]].High);
                    //            }
                    //        }
                    //    }
                    //}

                    //if (tryPushLow(index))
                    //{
                    //    //if (existingTrend == MovementKind.Up && tryPushHigh(index))
                    //    //{
                    //    //    items.Add(quote.Time, quote.High);
                    //    //    lowers.Clear();
                    //    //}
                    //    //else 
                    //    if (lowers.Count > Threshold)
                    //    {
                    //        if (existingTrend == MovementKind.Down)
                    //        {
                    //            items.Add(quote.Time, quote.Low);
                    //            highers.Clear();
                    //        }
                    //        else if (existingTrend == MovementKind.Up)
                    //        {
                    //            date = DataCollection[LastHigh].Time;

                    //            if (items.ContainsKey(date))
                    //                items[date] = DataCollection[LastHigh].High;
                    //            else
                    //                items.Add(date, DataCollection[LastHigh].High);
                    //            highers.Clear();

                    //            newOutlineItem = new OutlineItem(date.UtcDateTime.ToOADate(), items[date], index, PivotType.Top);
                    //            pivots.Add(newOutlineItem);

                    //            existingTrend = MovementKind.Down;

                    //            for (int i = Threshold; i < lowers.Count; i++)
                    //            {
                    //                if (!items.ContainsKey(DataCollection[lowers[i]].Time))
                    //                    items.Add(DataCollection[lowers[i]].Time, DataCollection[lowers[i]].Low);
                    //            }
                    //        }
                    //        else if (quote.High <= DataCollection[index - 1].High)
                    //        {
                    //            highers.Clear();

                    //            existingTrend = MovementKind.Down;

                    //            items.Add(DataCollection[FirstLow].Time, DataCollection[FirstLow].High);
                    //            for (int i = Threshold; i < lowers.Count; i++)
                    //            {
                    //                items.Add(DataCollection[lowers[i]].Time, DataCollection[lowers[i]].Low);
                    //            }

                    //        }
                    //    }
                    //}
                }

                    if (pivots.Count == 0)
                        return null;

                    //if (pivots[0].Type == PivotType.High || pivots[0].Type == PivotType.Top)
                    //{
                    //    newOutlineItem = new OutlineItem(DataCollection[0].Time.UtcDateTime.ToOADate(), DataCollection[0].Low, 0, PivotType.Bottom);
                    //    pivots.Insert(0, newOutlineItem);
                    //}
                    //else
                    //{
                    //    newOutlineItem = new OutlineItem(DataCollection[0].Time.UtcDateTime.ToOADate(), DataCollection[0].High, 0, PivotType.Top);
                    //    pivots.Insert(0, newOutlineItem);
                    //}

                    if (existingTrend == MovementKind.Up)
                    {
                        date = DataCollection[LastHigh].Time;

                        if (!items.ContainsKey(date))
                            items.Add(date, DataCollection[LastHigh].High);
                        else
                            items[date] = DataCollection[LastHigh].High;

                        if (pivots.Last().Time < date)
                        {
                            newOutlineItem = new OutlineItem(date, items[date], LastHigh, PivotType.Top);
                            pivots.Add(newOutlineItem);
                            existingTrend = MovementKind.Down;
                        }
                    }
                    else if (existingTrend == MovementKind.Down)
                    {
                        date = DataCollection[LastLow].Time;

                        if (!items.ContainsKey(date))
                            items.Add(date, DataCollection[LastLow].Low);
                        else
                            items[date] = DataCollection[LastLow].Low;

                        if (pivots.Last().Time < date)
                        {
                            newOutlineItem = new OutlineItem(date, items[date], LastLow, PivotType.Bottom);
                            pivots.Add(newOutlineItem);
                            existingTrend = MovementKind.Up;
                        }
                    }

                    if (!items.ContainsKey(DataCollection.Last().Time))
                    {
                        DateTimeOffset time = DataCollection.Last().Time;
                        if (existingTrend == MovementKind.Up)
                        {
                            items.Add(DataCollection.Last().Time, DataCollection.Last().High);
                            //Pivots.Add(new OutlineItem(time.UtcDateTime.ToOADate(), Sequences[time], DataCollection.Count - 1, PivotType.Top));
                        }
                        else if (existingTrend == MovementKind.Down)
                        {
                            items.Add(DataCollection.Last().Time, DataCollection.Last().Low);
                            //Pivots.Add(new OutlineItem(time.UtcDateTime.ToOADate(), Sequences[time], DataCollection.Count - 1, PivotType.Bottom));
                        }
                        //else
                        //    throw new Exception();
                    }

                    return new Outline(Threshold, items, pivots);
            }
        }

        private void changeToDown(SortedDictionary<DateTimeOffset, Double> items, List<OutlineItem> pivots, int index)
        {
            DateTimeOffset date= DataCollection[index].Time;
            if (items.ContainsKey(date))
                items[date] = DataCollection[index].High;
            else
                items.Add(date, DataCollection[index].High);

            for (int i = Threshold; i < lowers.Count; i++)
            {
                if (!items.ContainsKey(DataCollection[lowers[i]].Time))
                    items.Add(DataCollection[lowers[i]].Time, DataCollection[lowers[i]].Low);
            }

            OutlineItem newOutlineItem = new OutlineItem(date, items[date], index, PivotType.Top);
            pivots.Add(newOutlineItem);
            highers.Clear();
        }

        private void changeToUp(SortedDictionary<DateTimeOffset, Double> items, List<OutlineItem> pivots, int index)
        {
            DateTimeOffset date = DataCollection[index].Time;
            if (items.ContainsKey(date))
                items[date] = DataCollection[index].Low;
            else
                items.Add(date, DataCollection[index].Low);

            for (int i = Threshold; i < highers.Count; i++)
            {
                if (!items.ContainsKey(DataCollection[highers[i]].Time))
                    items.Add(DataCollection[highers[i]].Time, DataCollection[highers[i]].High);
            }

            OutlineItem newOutlineItem = new OutlineItem(date, items[date], index, PivotType.Bottom);
            pivots.Add(newOutlineItem);
            lowers.Clear();
        }

        #endregion

        public OutlineHelper(List<Quote> dataCollection, int threshold)
        {
            DataCollection = dataCollection;
            Threshold = threshold;
        }

        public OutlineHelper(Outline baseOutline, int threshold)
        {
            DataCollection = new List<Quote>();
            Threshold = threshold;

            foreach (KeyValuePair<DateTimeOffset, Double> kvp in baseOutline.Sequences)
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

            if (DataCollection[LastLow].Low > DataCollection[index].Low)
            {
                lowers.Add(index);
                return true;
            }
            return false;
        }

        #endregion

    }

}
