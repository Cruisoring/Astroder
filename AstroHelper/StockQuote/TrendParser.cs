using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockQuote
{
    public enum TrendDirection
    {
        Inside,
        Outside,
        Up,
        Down
    }

    public class TrendParser<T>
        where T : RecordBase
    {
        #region constructors
        public TrendParser(int threshold)
        {
            Threshold = threshold;
        }
        #endregion


        #region Properties and Fields
        public int Threshold { get; private set; }

        private List<T> items;

        private List<int> lowers = new List<int>();
        private List<int> highers = new List<int>();

        //private T this[int index] { get { return dataCollection == null ? null : dataCollection[index]; } }

        int LastLow { get { return lowers.Last(); } }
        int LastHigh { get { return highers.Last(); } }
        int FirstLow 
        { 
            get { return lowers[0]; }
            set { lowers[0] = value; }
        }
        int FirstHigh 
        {
            get { return highers[0]; } 
            set { highers[0] = value; }
        }

        #endregion


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
            if (LastHigh != index && items[LastHigh].High < items[index].High)
            {
                highers.Add(index);
                return true;
            }
            return false;
        }

        private bool tryPushLow(int index)
        {
            if (LastLow != index && items[LastLow].Low > items[index].Low)
            {
                lowers.Add(index);
                return true;
            }
            return false;
        }

        private TrendDirection MovementFrom(int index)
        {
            bool lower = items[index+1].Low < items[index].Low;
            bool higher = items[index + 1].High > items[index].High;

            if (lower && higher)
                return TrendDirection.Outside;
            else if (lower)
                return TrendDirection.Down;
            else if (higher)
                return TrendDirection.Up;
            else
                return TrendDirection.Inside;
        }

        public List<TurningPoint<T>> PivotsOf(List<T> items)
        {
            this.items = items;
            List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();
            TrendDirection existingTrend = TrendDirection.Inside;
            TrendDirection movement;

            resetHighersTo(0);
            resetLowersTo(0);

            for (int index = 0; index < items.Count-1; index++)
            {
                movement = MovementFrom(index);

                switch(movement)
                {
                    case TrendDirection.Inside:
                        continue;
                    case TrendDirection.Outside:
                        tryPushHigh(index);
                        tryPushLow(index);
                        break;
                    case TrendDirection.Up:
                        if (tryPushHigh(index) && highers.Count > Threshold)
                        {
                            if (existingTrend != TrendDirection.Up)
                            {
                                pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, items[FirstHigh]));
                                existingTrend = TrendDirection.Up;
                            }
                            else
                                FirstHigh = index+1;

                            resetLowersTo(index+1);
                        }
                        break;
                    case TrendDirection.Down:
                        if(tryPushLow(index) &&  lowers.Count > Threshold)
                        {
                            if (existingTrend != TrendDirection.Down)
                            {
                                pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, items[FirstLow]));
                                existingTrend = TrendDirection.Down;
                            }
                            else
                                FirstLow = index+1;

                            resetHighersTo(index+1);
                        }
                        break;
                    default:
                        break;
                }
            }

            return pivots;
        }

        //public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
        //{
        //    List<TurningPoint<T>> result = new List<TurningPoint<T>>();

        //    getCeilingFloor(dataCollection);

        //    TrendDirection trend, lastTrend = TrendDirection.Inside;

        //    T current, next, last = dataCollection[0];

        //    lowers.Clear();
        //    highers.Clear();

        //    for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
        //    {
        //        current = dataCollection[currentIndex];

        //        if (currentIndex == ceilingIndex)
        //        {
        //            //if (existingTrend == TrendDirection.Down)
        //            //{
        //            //    existingTrend = TrendDirection.Up;
        //            //    highers.Pop();
        //            //}
        //            //else if (existingTrend != TrendDirection.Up)
        //            //    throw new Exception();
        //            lowers.Clear();
        //            highers.Clear();

        //            if (lastTrend == TrendDirection.Down && highers.Count != 0)
        //                result.RemoveAt(result.Count-1);

        //            result.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
        //            lastTrend = TrendDirection.Down;
        //            continue;
        //        }
        //        else if (currentIndex == floorIndex)
        //        {
        //            //if (existingTrend == TrendDirection.Up)
        //            //{
        //            //    existingTrend = TrendDirection.Down;
        //            //    lowers.Pop();
        //            //}
        //            //else if (existingTrend != TrendDirection.Down)
        //            //    throw new Exception();


        //            lowers.Clear();
        //            highers.Clear();

        //            if (lastTrend == TrendDirection.Up && highers.Count != 0)
        //                result.RemoveAt(result.Count - 1);

        //            result.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
        //            lastTrend = TrendDirection.Up;
        //            continue;
        //        }                

        //        for (int j = currentIndex + 1; j < dataCollection.Count; j ++ )
        //        {
        //            next = dataCollection[j];
        //            trend = TrendOf(current, next);

        //            switch(trend)
        //            {
        //                case TrendDirection.Outside:
        //                    highers.Push(j);
        //                    lowers.Push(j);
        //                    break;
        //                case TrendDirection.Up:
        //                    highers.Push(j);
        //                    break;
        //                case TrendDirection.Down:
        //                    lowers.Push(j);
        //                    break;
        //                default:
        //                    break;
        //            }

        //            currentIndex = j - 1;
        //            switch(lastTrend)
        //            {
        //                case TrendDirection.Outside:
        //                case TrendDirection.Inside:
        //                    if (next.Low < current.Low && lowers.Count >= threshold)
        //                    {
        //                        lastTrend = TrendDirection.Down;
        //                        j = dataCollection.Count;
        //                        highers.Clear();
        //                    }
        //                    else if (next.High > current.High && highers.Count >= threshold)
        //                    {
        //                        lastTrend = TrendDirection.Up;
        //                        //currentIndex = j-1;
        //                        j = dataCollection.Count;
        //                        lowers.Clear();
        //                    } 
        //                    break;
        //                case TrendDirection.Up:
        //                    if (next.High > current.High)
        //                    {
        //                        lowers.Clear();
        //                        //currentIndex = j - 1;
        //                        j = dataCollection.Count;
        //                    }
        //                    else if (next.Low < current.Low && lowers.Count >= threshold)
        //                    {
        //                        result.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
        //                        lastTrend = TrendDirection.Down;
        //                        highers.Clear();
        //                        //currentIndex = j-1;
        //                        j = dataCollection.Count;
        //                    }
        //                    break;
        //                case TrendDirection.Down:
        //                    if (next.Low < current.Low)
        //                    {
        //                        highers.Clear();
        //                        //currentIndex = j - 1;
        //                        j = dataCollection.Count;
        //                    }
        //                    else if (next.High > current.High && highers.Count >= threshold)
        //                    {
        //                        result.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
        //                        lastTrend = TrendDirection.Up;
        //                        lowers.Clear();
        //                        //currentIndex = j - 1;
        //                        j = dataCollection.Count;
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }

        //            //if (j == dataCollection.Count -1)
        //            //{
        //            //    pivots.Add(new TurningPoint<T>(existingTrend == TrendDirection.Up ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[currentIndex-1]));
        //            //    currentIndex = j;
        //            //}
        //        }

        //    }

        //    if (result[0].Date != dataCollection[0].Date)
        //    {
        //        TurningPoint<T> startMajor = new TurningPoint<T>(result[0].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[0]);
        //        result.Insert(0, startMajor);
        //    }

        //    if (result[result.Count-1].Date != dataCollection[dataCollection.Count-1].Date)
        //    {
        //        TurningPoint<T> endMajor = new TurningPoint<T>(result[result.Count - 1].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top
        //            : TypeOfTurning.Bottom, dataCollection[dataCollection.Count - 1]);
        //        result.Add(endMajor);
        //    }

        //    //bool isEvenUp = false;

        //    //for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
        //    //{
        //    //    trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

        //    //    if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
        //    //        continue;
        //    //    else if (trend == TrendDirection.Up)
        //    //        isEvenUp = (currentIndex % 2 == 0) ? true : false;
        //    //    else if (trend == TrendDirection.Down)
        //    //        isEvenUp = (currentIndex % 2 == 1) ? true : false;

        //    //    break;
        //    //}

        //    //TypeOfTurning kind;

        //    //for (int currentIndex = 0; currentIndex < majors.Count; currentIndex++)
        //    //{
        //    //    kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
        //    //    TurningPoint<T> item = new TurningPoint<T>(kind, dataCollection[majors[currentIndex]]);
        //    //    pivots.Add(item);
        //    //}

        //    return result;
        //}

        /*/ Bad codes
        #region functions

        public int NextHigherIndex(List<T> dataCollection, int highIndex)
        {
            T record = dataCollection[highIndex];

            for (int index = highIndex + 1; index < dataCollection.Count; index++)
            {
                if (dataCollection[index].LastHigh > record.LastHigh)
                    return index;
            }
            return -1;
        }

        public int NextLowerIndex(List<T> dataCollection, int lowIndex)
        {
            T record = dataCollection[lowIndex];

            for (int index = lowIndex + 1; index < dataCollection.Count; index++)
            {
                if (dataCollection[index].LastLow < record.LastLow)
                    return index;
            }
            return -1;
        }

        public List<int> HigherAndHigher(List<T> item, int start, int end)
        {
            List<int> pivots = new List<int>();

            int index = start;

            do
            {
                index = NextHigherIndex(item, index);
                pivots.Add(index);
            } while (index < end && index != -1);

            return pivots;
        }

        public List<int> LowerAndLower(List<T> item, int start, int end)
        {
            List<int> pivots = new List<int>();

            int index = start;

            do
            {
                index = NextLowerIndex(item, index);
                pivots.Add(index);
            } while (index < end && index != -1);

            return pivots;
        }

        public int TrendedCount(List<T> dataCollection, int start, int end)
        {
            T record, last = dataCollection[start];

            TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

            int count = 0;
            if (trend == TrendDirection.Up)
            {
                for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
                {
                    record = dataCollection[currentIndex];
                    if (record.LastHigh > last.LastHigh)
                    {
                        last = record;
                        count++;
                    }
                }
            }
            else if (trend == TrendDirection.Down)
            {
                for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
                {
                    record = dataCollection[currentIndex];
                    if (record.LastLow < last.LastLow)
                    {
                        last = record;
                        count++;
                    }
                }
            }
            return count;
        }

        public bool IsTrending(List<T> dataCollection, int start, int end)
        {
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

            if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
                return false;
            else
                return TrendedCount(dataCollection, start, end) >= threshold;
        }

        public int LowestIndexBetween(List<T> dataCollection, int firstHighIndex, int secondHighIndex)
        {
            if (secondHighIndex - firstHighIndex <= threshold + 1)
                return -1;

            double lowest = dataCollection[firstHighIndex].LastLow;
            int lowestIndex = firstHighIndex;

            for (int currentIndex = firstHighIndex + 1; currentIndex < secondHighIndex; currentIndex++)
            {
                if (lowest < dataCollection[currentIndex].LastLow)
                {
                    lowest = dataCollection[currentIndex].LastLow;
                    lowestIndex = currentIndex;
                }
            }
            return lowestIndex;
        }

        public int HighestIndexBetween(List<T> dataCollection, int firstLowIndex, int secondLowIndex)
        {
            if (secondLowIndex - firstLowIndex <= threshold + 1)
                return -1;

            double highest = dataCollection[firstLowIndex].LastHigh;
            int highestIndex = firstLowIndex;

            for (int currentIndex = firstLowIndex + 1; currentIndex < secondLowIndex; currentIndex++)
            {
                if (highest < dataCollection[currentIndex].LastHigh)
                {
                    highest = dataCollection[currentIndex].LastHigh;
                    highestIndex = currentIndex;
                }
            }
            return highestIndex;
        }

        private void add(List<int> results, int pivots)
        {
            if (!results.Contains(pivots))
                results.Add(pivots);
        }

        private void addRange(List<int> results, ICollection<int> extra)
        {
            if (extra == null || extra.Count == 0)
                return;

            foreach (int pivots in extra)
            {
                if (!results.Contains(pivots))
                    results.Add(pivots);
            }
        }

        public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
        {
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            if (end - start < threshold + 2)
                return null;

            List<int> results = new List<int> { start, end };
            List<int> extras;

            int highest, lowest;

            lowest = LowestIndexBetween(dataCollection, start, end);
            highest = HighestIndexBetween(dataCollection, start, end);

            int first = Math.Min(highest, lowest);
            int second = Math.Max(highest, lowest);

            if (second - first >= threshold && IsTrending(dataCollection, first, second))
            {
                add(results, first);
                add(results, second);
            }

            if (first - start >= threshold && IsTrending(dataCollection, start, first))
            {
                add(results, first);
            }

            if (end - second >= threshold && IsTrending(dataCollection, second, end))
            {
                add(results, second);
            }

            if (results.Count == 2)
            {
                TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
                if (trend == TrendDirection.Up)
                {
                    List<int> highers = HigherAndHigher(dataCollection, start, end);

                    for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
                    {
                        lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

                        if (lowest == -1) continue;

                        if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
                        {
                            add(results, lowest);
                        }
                    }
                }
                else if (trend == TrendDirection.Down)
                {
                    List<int> lowers = LowerAndLower(dataCollection, start, end);

                    for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
                    {
                        highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

                        if (highest == -1) continue;

                        if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
                        {
                            add(results, highest);
                        }
                    }
                }
                else
                    throw new NotImplementedException();
            }

            results.Sort();

            extras = new List<int>();
            List<int> others;

            for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
            {
                others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
                addRange(extras, others);
            }

            addRange(results, extras);
            results.Sort();

            return results;

        }

        public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
        {
            List<int> turnings = TurningsBetween(dataCollection, 0, dataCollection.Count - 1);
            List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();

            add(turnings, 0);
            add(turnings, dataCollection.Count - 1);
            turnings.Sort();

            if (turnings.Count < 3)
                return null;

            TrendDirection trend;

            bool isEvenUp = false;

            for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
            {
                trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

                if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
                    continue;
                else if (trend == TrendDirection.Up)
                    isEvenUp = (currentIndex % 2 == 0) ? true : false;
                else if (trend == TrendDirection.Down)
                    isEvenUp = (currentIndex % 2 == 1) ? true : false;

                break;
            }

            TypeOfTurning kind;
            TurningPoint<T> pivots;

            for (int currentIndex = 0; currentIndex < turnings.Count; currentIndex++)
            {
                kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
                pivots = new TurningPoint<T>(kind, dataCollection[currentIndex]);
                pivots.Add(pivots);
            }

            return pivots;
        }

        public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
        {
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            if (end - start < threshold + 2)
                return null;

            List<int> results = new List<int>();
            List<int> extras;

            TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
            int highest, lowest;

            switch (trend)
            {
                case TrendDirection.Inside:
                case TrendDirection.Outside:
                    {
                        lowest = LowestIndexBetween(dataCollection, start, end);
                        highest = HighestIndexBetween(dataCollection, start, end);

                        int first = Math.Min(highest, lowest);
                        int second = Math.Max(highest, lowest);

                        if (IsTrending(dataCollection, start, first))
                        {
                            add(results, first);
                        }

                        if (IsTrending(dataCollection, first, second))
                        {
                            add(results, second);
                            add(results, first);
                        }

                        if (IsTrending(dataCollection, second, end))
                        {
                            add(results, second);
                        }

                        break;
                    }
                case TrendDirection.Up:
                    {
                        lowest = LowestIndexBetween(dataCollection, start, end);

                        if (lowest != start && lowest != end)
                        {
                            if (IsTrending(dataCollection, start, lowest))
                            {
                                add(results, lowest);
                            }

                            if (IsTrending(dataCollection, lowest, end))
                            {
                                add(results, lowest);
                            }

                        }
                        else
                        {
                            List<int> highers = HigherAndHigher(dataCollection, start, end);

                            for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
                            {
                                lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

                                if (lowest == -1) continue;

                                if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
                                {
                                    add(results, lowest);
                                }
                            }
                        }
                        break;
                    }

                case TrendDirection.Down:
                    {
                        highest = HighestIndexBetween(dataCollection, start, end);

                        if (highest != start && highest != end)
                        {
                            if (highest != start && IsTrending(dataCollection, start, highest))
                            {
                                add(results, highest);
                            }
                            else if (highest != end && IsTrending(dataCollection, highest, end))
                            {
                                add(results, highest);
                            }
                        }
                        else
                        {
                            List<int> lowers = LowerAndLower(dataCollection, start, end);

                            for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
                            {
                                highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

                                if (highest == -1) continue;

                                if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
                                {
                                    add(results, highest);
                                }
                            }
                        }

                        break;
                    }
                default:
                    throw new Exception();
            }

            if (results.Count == 0)
                return null;

            add(results, start);
            add(results, end);
            results.Sort();

            extras = new List<int>();
            List<int> others;

            for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
            {
                others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
                addRange(extras, others);
            }

            addRange(results, extras);
            results.Sort();

            return results;
        }


        public int NextMajorIndex(List<T> dataCollection, int lastMajorIndex, TrendDirection existingTrend)
        {
            T previous = dataCollection[lastMajorIndex];
            T record = dataCollection[lastMajorIndex + 1];
            T next;
            TrendDirection currentTrend = TrendOf(previous, record);
            TrendDirection nextTrend;
            int count = (existingTrend == TrendDirection.Inside) ? 1 : 0;

            for (int currentIndex = lastMajorIndex + 2; currentIndex < dataCollection.Count; currentIndex++)
            {
                next = dataCollection[currentIndex];
                nextTrend = TrendOf(record, next);

                if (nextTrend == TrendDirection.Inside)
                    continue;
                else if (nextTrend == TrendDirection.Outside || nextTrend == currentTrend)
                    count++;
                else  //Trend change may happen here
                {
                    if (LengthOfTrend(dataCollection, currentIndex - 1, currentTrend) < threshold)
                        return -1;
                    else
                        return currentIndex - 1;
                }

            }
        }

        public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
        {
            List<TurningPoint<T>> results = new List<TurningPoint<T>>();
            TrendDirection previousTrend = TrendDirection.Inside;
            TrendDirection nextTrend, followingTrend;
            TrendDirection currentTrend = TrendDirection.Inside;

            T previous = dataCollection[0], record, next, following;
            int count = 0, count2 = 0;

            for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
            {
                record = dataCollection[currentIndex];
                currentTrend = TrendOf(previous, record);
                for (int j = currentIndex + 1; j < dataCollection.Count; j++)
                {
                    next = dataCollection[j];
                    nextTrend = TrendOf(record, next);

                    if (count >= threshold)
                    {
                        previous = record;
                        previousTrend = (nextTrend == TrendDirection.Down) ? TrendDirection.Up : TrendDirection.Down;
                    }
                    else
                    {
                        count2 = 1;

                        for (int k = j + 1; k < dataCollection.Count; k++)
                        {
                            following = dataCollection[k];
                            followingTrend = TrendOf(next, following);
                            if (followingTrend == nextTrend || followingTrend == TrendDirection.Outside)
                                count2++;
                            else if (followingTrend != TrendDirection.Inside)
                                continue;
                            else
                            {
                                if (count2 < threshold)
                                {
                                    count++;
                                }
                            }
                        }
                    }

                }

                //if (nextTrend == TrendDirection.None)
                //    continue;
                //else if (nextTrend != nextTrend)
                //{
                //    if (nextTrend == TrendDirection.None)
                //    {
                //        nextTrend = nextTrend;
                //        count = 1;
                //    }
                //    else
                //    {
                //        nextTrend = nextTrend;

                //    }
                //}
                //else
                //{
                //    count++;
                //    if (count >= threshold && results.Count == 0)
                //        results.Add(new TurningPoint(nextTrend == TrendDirection.Up ? TypeOfTurning.Bottom : TypeOfTurning.Top, record));
                //}
            }

        }
        #endregion
        //*/
    }

    //public class TrendParser<T>
    //    where T : RecordBase
    //{
    //    #region constructors
    //    public TrendParser(int threshold)
    //    {
    //        threshold = threshold;
    //    }
    //    #endregion


    //    #region Properties and Fields
    //    public int threshold { get; private set; }

    //    private List<T> dataCollection;

    //    private List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();
    //    private List<int> lowers = new List<int>();
    //    private List<int> highers = new List<int>();
    //    private int ceilingIndex = -1;
    //    private int floorIndex = -1;

    //    //private int currentIndex = -1;
    //    //private int nextIndex = -1;
    //    //private int lowIndex = -1;
    //    //private int highIndex = -1;

    //    private T this[int index] { get { return dataCollection == null ? null : dataCollection[index]; } }

    //    int LastLow { get { return lowers.Last(); } }
    //    int LastHigh { get { return highers.Last(); } }
    //    int FirstLow 
    //    { 
    //        get { return lowers[0]; }
    //        set { lowers[0] = value; }
    //    }
    //    int FirstHigh 
    //    {
    //        get { return highers[0]; } 
    //        set { highers[0] = value; }
    //    }

    //    //private TrendDirection existingTrend;
    //    //private TrendDirection currentTrend;

    //    //private TrendDirection NextMovement
    //    //{
    //    //    get
    //    //    {
    //    //        bool lower = Next.Low < BeforeNext.Low;
    //    //        bool higher = Next.High > BeforeNext.High;

    //    //        if (lower && higher)
    //    //            return TrendDirection.Outside;
    //    //        else if (lower)
    //    //            return TrendDirection.Down;
    //    //        else if (higher)
    //    //            return TrendDirection.Up;
    //    //        else
    //    //            return TrendDirection.Inside;
    //    //    }
    //    //}

    //    //public TrendDirection CurrentTrend
    //    //{
    //    //    get { return currentTrend; }
    //    //    set 
    //    //    {
    //    //        currentTrend = value;
    //    //        switch (currentTrend)
    //    //        {
    //    //            case TrendDirection.Inside:
    //    //                return;
    //    //            case TrendDirection.Outside:
    //    //                highers.Push(nextIndex);
    //    //                lowers.Push(nextIndex);
    //    //                break;
    //    //            case TrendDirection.Up:
    //    //                highers.Push(nextIndex);
    //    //                break;
    //    //            case TrendDirection.Down:
    //    //                lowers.Push(nextIndex);
    //    //                break;
    //    //            default:
    //    //                break;
    //    //        }

    //    //        switch (ExistingTrend)
    //    //        {
    //    //            case TrendDirection.Outside:
    //    //            case TrendDirection.Inside:
    //    //                if (currentTrend == TrendDirection.Up && highers.Count >= threshold)
    //    //                {
    //    //                    ExistingTrend = TrendDirection.Up;
    //    //                }
    //    //                else if (currentTrend == TrendDirection.Down && lowers.Count >= threshold)
    //    //                {
    //    //                    ExistingTrend = TrendDirection.Down;
    //    //                }

    //    //                if (next.Low < current.Low && lowers.Count >= threshold)
    //    //                {
    //    //                    existingTrend = TrendDirection.Down;
    //    //                    j = dataCollection.Count;
    //    //                    highers.Clear();
    //    //                }
    //    //                else if (next.High > current.High && highers.Count >= threshold)
    //    //                {
    //    //                    existingTrend = TrendDirection.Up;
    //    //                    //currentIndex = j-1;
    //    //                    j = dataCollection.Count;
    //    //                    lowers.Clear();
    //    //                }
    //    //                break;
    //    //            case TrendDirection.Up:
    //    //                if (next.High > current.High)
    //    //                {
    //    //                    lowers.Clear();
    //    //                    //currentIndex = j - 1;
    //    //                    j = dataCollection.Count;
    //    //                }
    //    //                else if (next.Low < current.Low && lowers.Count >= threshold)
    //    //                {
    //    //                    pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
    //    //                    existingTrend = TrendDirection.Down;
    //    //                    highers.Clear();
    //    //                    //currentIndex = j-1;
    //    //                    j = dataCollection.Count;
    //    //                }
    //    //                break;
    //    //            case TrendDirection.Down:
    //    //                if (next.Low < current.Low)
    //    //                {
    //    //                    highers.Clear();
    //    //                    //currentIndex = j - 1;
    //    //                    j = dataCollection.Count;
    //    //                }
    //    //                else if (next.High > current.High && highers.Count >= threshold)
    //    //                {
    //    //                    pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
    //    //                    existingTrend = TrendDirection.Up;
    //    //                    lowers.Clear();
    //    //                    //currentIndex = j - 1;
    //    //                    j = dataCollection.Count;
    //    //                }
    //    //                break;
    //    //            default:
    //    //                break;
    //    //        }
    //    //    }
    //    //}
    //    //public TrendDirection ExistingTrend
    //    //{
    //    //    get { return existingTrend; }
    //    //    set { 
    //    //        existingTrend = value;

    //    //        if (value == TrendDirection.Down)
    //    //        {
    //    //            pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, Current));
    //    //        }
    //    //        else if (value == TrendDirection.Up)
    //    //        {
    //    //            pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, Current));
    //    //            highIndex = nextIndex;
    //    //        }
    //    //        else
    //    //            throw new Exception();
    //    //    }
    //    //}

    //    #endregion


    //    private void resetHighersTo(int index)
    //    {
    //        highers.Clear();
    //        highers.Add(index);
    //    }

    //    private void resetLowersTo(int index)
    //    {
    //        lowers.Clear();
    //        lowers.Add(index);
    //    }

    //    private bool tryPushHigh(int index)
    //    {
    //        if (LastHigh != index && this[LastHigh].High < this[index].High)
    //        {
    //            highers.Add(index);
    //            return true;
    //        }
    //        return false;
    //    }

    //    private bool tryPushLow(int index)
    //    {
    //        if (LastLow != index && this[LastLow].Low > this[index].Low)
    //        {
    //            lowers.Add(index);
    //            return true;
    //        }
    //        return false;
    //    }

    //    private void getCeilingFloor(List<T> dataCollection)
    //    {
    //        double highest = double.MinValue, lowest = double.MaxValue;

    //        for (int i = 0; i < dataCollection.Count; i++)
    //        {
    //            if (highest < dataCollection[i].High)
    //            {
    //                highest = dataCollection[i].High;
    //                ceilingIndex = i;
    //            }

    //            if (lowest > dataCollection[i].Low)
    //            {
    //                lowest = dataCollection[i].Low;
    //                floorIndex = i;
    //            }
    //        }
    //    }

    //    private TrendDirection MovementFrom(int index)
    //    {
    //        bool lower = this[index+1].Low < this[index].Low;
    //        bool higher = this[index + 1].High > this[index].High;

    //        if (lower && higher)
    //            return TrendDirection.Outside;
    //        else if (lower)
    //            return TrendDirection.Down;
    //        else if (higher)
    //            return TrendDirection.Up;
    //        else
    //            return TrendDirection.Inside;
    //    }

    //    public List<TurningPoint<T>> PivotsOf(List<T> dataCollection)
    //    {
    //        this.dataCollection = dataCollection;
    //        pivots.Clear();
    //        getCeilingFloor(dataCollection);
    //        TrendDirection existingTrend = TrendDirection.Inside;
    //        TrendDirection movement;

    //        resetHighersTo(0);
    //        resetLowersTo(0);

    //        for (int index = 0; index < dataCollection.Count-1; index++)
    //        {
    //            movement = MovementFrom(index);

    //            switch(movement)
    //            {
    //                case TrendDirection.Inside:
    //                    continue;
    //                case TrendDirection.Outside:
    //                    tryPushHigh(index);
    //                    tryPushLow(index);
    //                    break;
    //                case TrendDirection.Up:
    //                    if (tryPushHigh(index) && highers.Count > threshold)
    //                    {
    //                        if (existingTrend != TrendDirection.Up)
    //                        {
    //                            pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, this[FirstHigh]));
    //                            existingTrend = TrendDirection.Up;
    //                        }
    //                        else
    //                            FirstHigh = index+1;

    //                        resetLowersTo(index+1);
    //                    }
    //                    break;
    //                case TrendDirection.Down:
    //                    if(tryPushLow(index) &&  lowers.Count > threshold)
    //                    {
    //                        if (existingTrend != TrendDirection.Down)
    //                        {
    //                            pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, this[FirstLow]));
    //                            existingTrend = TrendDirection.Down;
    //                        }
    //                        else
    //                            FirstLow = index+1;

    //                        resetHighersTo(index+1);
    //                    }
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }

    //        return pivots;
    //    }

    //    //public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    //{
    //    //    List<TurningPoint<T>> result = new List<TurningPoint<T>>();

    //    //    getCeilingFloor(dataCollection);

    //    //    TrendDirection trend, lastTrend = TrendDirection.Inside;

    //    //    T current, next, last = dataCollection[0];

    //    //    lowers.Clear();
    //    //    highers.Clear();

    //    //    for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //    //    {
    //    //        current = dataCollection[currentIndex];

    //    //        if (currentIndex == ceilingIndex)
    //    //        {
    //    //            //if (existingTrend == TrendDirection.Down)
    //    //            //{
    //    //            //    existingTrend = TrendDirection.Up;
    //    //            //    highers.Pop();
    //    //            //}
    //    //            //else if (existingTrend != TrendDirection.Up)
    //    //            //    throw new Exception();
    //    //            lowers.Clear();
    //    //            highers.Clear();

    //    //            if (lastTrend == TrendDirection.Down && highers.Count != 0)
    //    //                result.RemoveAt(result.Count-1);

    //    //            result.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
    //    //            lastTrend = TrendDirection.Down;
    //    //            continue;
    //    //        }
    //    //        else if (currentIndex == floorIndex)
    //    //        {
    //    //            //if (existingTrend == TrendDirection.Up)
    //    //            //{
    //    //            //    existingTrend = TrendDirection.Down;
    //    //            //    lowers.Pop();
    //    //            //}
    //    //            //else if (existingTrend != TrendDirection.Down)
    //    //            //    throw new Exception();


    //    //            lowers.Clear();
    //    //            highers.Clear();

    //    //            if (lastTrend == TrendDirection.Up && highers.Count != 0)
    //    //                result.RemoveAt(result.Count - 1);

    //    //            result.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
    //    //            lastTrend = TrendDirection.Up;
    //    //            continue;
    //    //        }                

    //    //        for (int j = currentIndex + 1; j < dataCollection.Count; j ++ )
    //    //        {
    //    //            next = dataCollection[j];
    //    //            trend = TrendOf(current, next);

    //    //            switch(trend)
    //    //            {
    //    //                case TrendDirection.Outside:
    //    //                    highers.Push(j);
    //    //                    lowers.Push(j);
    //    //                    break;
    //    //                case TrendDirection.Up:
    //    //                    highers.Push(j);
    //    //                    break;
    //    //                case TrendDirection.Down:
    //    //                    lowers.Push(j);
    //    //                    break;
    //    //                default:
    //    //                    break;
    //    //            }

    //    //            currentIndex = j - 1;
    //    //            switch(lastTrend)
    //    //            {
    //    //                case TrendDirection.Outside:
    //    //                case TrendDirection.Inside:
    //    //                    if (next.Low < current.Low && lowers.Count >= threshold)
    //    //                    {
    //    //                        lastTrend = TrendDirection.Down;
    //    //                        j = dataCollection.Count;
    //    //                        highers.Clear();
    //    //                    }
    //    //                    else if (next.High > current.High && highers.Count >= threshold)
    //    //                    {
    //    //                        lastTrend = TrendDirection.Up;
    //    //                        //currentIndex = j-1;
    //    //                        j = dataCollection.Count;
    //    //                        lowers.Clear();
    //    //                    } 
    //    //                    break;
    //    //                case TrendDirection.Up:
    //    //                    if (next.High > current.High)
    //    //                    {
    //    //                        lowers.Clear();
    //    //                        //currentIndex = j - 1;
    //    //                        j = dataCollection.Count;
    //    //                    }
    //    //                    else if (next.Low < current.Low && lowers.Count >= threshold)
    //    //                    {
    //    //                        result.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
    //    //                        lastTrend = TrendDirection.Down;
    //    //                        highers.Clear();
    //    //                        //currentIndex = j-1;
    //    //                        j = dataCollection.Count;
    //    //                    }
    //    //                    break;
    //    //                case TrendDirection.Down:
    //    //                    if (next.Low < current.Low)
    //    //                    {
    //    //                        highers.Clear();
    //    //                        //currentIndex = j - 1;
    //    //                        j = dataCollection.Count;
    //    //                    }
    //    //                    else if (next.High > current.High && highers.Count >= threshold)
    //    //                    {
    //    //                        result.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
    //    //                        lastTrend = TrendDirection.Up;
    //    //                        lowers.Clear();
    //    //                        //currentIndex = j - 1;
    //    //                        j = dataCollection.Count;
    //    //                    }
    //    //                    break;
    //    //                default:
    //    //                    break;
    //    //            }

    //    //            //if (j == dataCollection.Count -1)
    //    //            //{
    //    //            //    pivots.Add(new TurningPoint<T>(existingTrend == TrendDirection.Up ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[currentIndex-1]));
    //    //            //    currentIndex = j;
    //    //            //}
    //    //        }

    //    //    }

    //    //    if (result[0].Date != dataCollection[0].Date)
    //    //    {
    //    //        TurningPoint<T> startMajor = new TurningPoint<T>(result[0].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[0]);
    //    //        result.Insert(0, startMajor);
    //    //    }

    //    //    if (result[result.Count-1].Date != dataCollection[dataCollection.Count-1].Date)
    //    //    {
    //    //        TurningPoint<T> endMajor = new TurningPoint<T>(result[result.Count - 1].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top
    //    //            : TypeOfTurning.Bottom, dataCollection[dataCollection.Count - 1]);
    //    //        result.Add(endMajor);
    //    //    }

    //    //    //bool isEvenUp = false;

    //    //    //for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //    //    //{
    //    //    //    trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

    //    //    //    if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //    //    //        continue;
    //    //    //    else if (trend == TrendDirection.Up)
    //    //    //        isEvenUp = (currentIndex % 2 == 0) ? true : false;
    //    //    //    else if (trend == TrendDirection.Down)
    //    //    //        isEvenUp = (currentIndex % 2 == 1) ? true : false;

    //    //    //    break;
    //    //    //}

    //    //    //TypeOfTurning kind;

    //    //    //for (int currentIndex = 0; currentIndex < majors.Count; currentIndex++)
    //    //    //{
    //    //    //    kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
    //    //    //    TurningPoint<T> item = new TurningPoint<T>(kind, dataCollection[majors[currentIndex]]);
    //    //    //    pivots.Add(item);
    //    //    //}

    //    //    return result;
    //    //}

    //    /*/ Bad codes
    //    #region functions

    //    public int NextHigherIndex(List<T> dataCollection, int highIndex)
    //    {
    //        T record = dataCollection[highIndex];

    //        for (int index = highIndex + 1; index < dataCollection.Count; index++)
    //        {
    //            if (dataCollection[index].LastHigh > record.LastHigh)
    //                return index;
    //        }
    //        return -1;
    //    }

    //    public int NextLowerIndex(List<T> dataCollection, int lowIndex)
    //    {
    //        T record = dataCollection[lowIndex];

    //        for (int index = lowIndex + 1; index < dataCollection.Count; index++)
    //        {
    //            if (dataCollection[index].LastLow < record.LastLow)
    //                return index;
    //        }
    //        return -1;
    //    }

    //    public List<int> HigherAndHigher(List<T> item, int start, int end)
    //    {
    //        List<int> pivots = new List<int>();

    //        int index = start;

    //        do
    //        {
    //            index = NextHigherIndex(item, index);
    //            pivots.Add(index);
    //        } while (index < end && index != -1);

    //        return pivots;
    //    }

    //    public List<int> LowerAndLower(List<T> item, int start, int end)
    //    {
    //        List<int> pivots = new List<int>();

    //        int index = start;

    //        do
    //        {
    //            index = NextLowerIndex(item, index);
    //            pivots.Add(index);
    //        } while (index < end && index != -1);

    //        return pivots;
    //    }

    //    public int TrendedCount(List<T> dataCollection, int start, int end)
    //    {
    //        T record, last = dataCollection[start];

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

    //        int count = 0;
    //        if (trend == TrendDirection.Up)
    //        {
    //            for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
    //            {
    //                record = dataCollection[currentIndex];
    //                if (record.LastHigh > last.LastHigh)
    //                {
    //                    last = record;
    //                    count++;
    //                }
    //            }
    //        }
    //        else if (trend == TrendDirection.Down)
    //        {
    //            for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
    //            {
    //                record = dataCollection[currentIndex];
    //                if (record.LastLow < last.LastLow)
    //                {
    //                    last = record;
    //                    count++;
    //                }
    //            }
    //        }
    //        return count;
    //    }

    //    public bool IsTrending(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

    //        if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //            return false;
    //        else
    //            return TrendedCount(dataCollection, start, end) >= threshold;
    //    }

    //    public int LowestIndexBetween(List<T> dataCollection, int firstHighIndex, int secondHighIndex)
    //    {
    //        if (secondHighIndex - firstHighIndex <= threshold + 1)
    //            return -1;

    //        double lowest = dataCollection[firstHighIndex].LastLow;
    //        int lowestIndex = firstHighIndex;

    //        for (int currentIndex = firstHighIndex + 1; currentIndex < secondHighIndex; currentIndex++)
    //        {
    //            if (lowest < dataCollection[currentIndex].LastLow)
    //            {
    //                lowest = dataCollection[currentIndex].LastLow;
    //                lowestIndex = currentIndex;
    //            }
    //        }
    //        return lowestIndex;
    //    }

    //    public int HighestIndexBetween(List<T> dataCollection, int firstLowIndex, int secondLowIndex)
    //    {
    //        if (secondLowIndex - firstLowIndex <= threshold + 1)
    //            return -1;

    //        double highest = dataCollection[firstLowIndex].LastHigh;
    //        int highestIndex = firstLowIndex;

    //        for (int currentIndex = firstLowIndex + 1; currentIndex < secondLowIndex; currentIndex++)
    //        {
    //            if (highest < dataCollection[currentIndex].LastHigh)
    //            {
    //                highest = dataCollection[currentIndex].LastHigh;
    //                highestIndex = currentIndex;
    //            }
    //        }
    //        return highestIndex;
    //    }

    //    private void add(List<int> results, int pivots)
    //    {
    //        if (!results.Contains(pivots))
    //            results.Add(pivots);
    //    }

    //    private void addRange(List<int> results, ICollection<int> extra)
    //    {
    //        if (extra == null || extra.Count == 0)
    //            return;

    //        foreach (int pivots in extra)
    //        {
    //            if (!results.Contains(pivots))
    //                results.Add(pivots);
    //        }
    //    }

    //    public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        if (end - start < threshold + 2)
    //            return null;

    //        List<int> results = new List<int> { start, end };
    //        List<int> extras;

    //        int highest, lowest;

    //        lowest = LowestIndexBetween(dataCollection, start, end);
    //        highest = HighestIndexBetween(dataCollection, start, end);

    //        int first = Math.Min(highest, lowest);
    //        int second = Math.Max(highest, lowest);

    //        if (second - first >= threshold && IsTrending(dataCollection, first, second))
    //        {
    //            add(results, first);
    //            add(results, second);
    //        }

    //        if (first - start >= threshold && IsTrending(dataCollection, start, first))
    //        {
    //            add(results, first);
    //        }

    //        if (end - second >= threshold && IsTrending(dataCollection, second, end))
    //        {
    //            add(results, second);
    //        }

    //        if (results.Count == 2)
    //        {
    //            TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
    //            if (trend == TrendDirection.Up)
    //            {
    //                List<int> highers = HigherAndHigher(dataCollection, start, end);

    //                for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

    //                    if (lowest == -1) continue;

    //                    if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
    //                    {
    //                        add(results, lowest);
    //                    }
    //                }
    //            }
    //            else if (trend == TrendDirection.Down)
    //            {
    //                List<int> lowers = LowerAndLower(dataCollection, start, end);

    //                for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
    //                {
    //                    highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

    //                    if (highest == -1) continue;

    //                    if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
    //                    {
    //                        add(results, highest);
    //                    }
    //                }
    //            }
    //            else
    //                throw new NotImplementedException();
    //        }

    //        results.Sort();

    //        extras = new List<int>();
    //        List<int> others;

    //        for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
    //        {
    //            others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
    //            addRange(extras, others);
    //        }

    //        addRange(results, extras);
    //        results.Sort();

    //        return results;

    //    }

    //    public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    {
    //        List<int> turnings = TurningsBetween(dataCollection, 0, dataCollection.Count - 1);
    //        List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();

    //        add(turnings, 0);
    //        add(turnings, dataCollection.Count - 1);
    //        turnings.Sort();

    //        if (turnings.Count < 3)
    //            return null;

    //        TrendDirection trend;

    //        bool isEvenUp = false;

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        {
    //            trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

    //            if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //                continue;
    //            else if (trend == TrendDirection.Up)
    //                isEvenUp = (currentIndex % 2 == 0) ? true : false;
    //            else if (trend == TrendDirection.Down)
    //                isEvenUp = (currentIndex % 2 == 1) ? true : false;

    //            break;
    //        }

    //        TypeOfTurning kind;
    //        TurningPoint<T> pivots;

    //        for (int currentIndex = 0; currentIndex < turnings.Count; currentIndex++)
    //        {
    //            kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
    //            pivots = new TurningPoint<T>(kind, dataCollection[currentIndex]);
    //            pivots.Add(pivots);
    //        }

    //        return pivots;
    //    }

    //    public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        if (end - start < threshold + 2)
    //            return null;

    //        List<int> results = new List<int>();
    //        List<int> extras;

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
    //        int highest, lowest;

    //        switch (trend)
    //        {
    //            case TrendDirection.Inside:
    //            case TrendDirection.Outside:
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, start, end);
    //                    highest = HighestIndexBetween(dataCollection, start, end);

    //                    int first = Math.Min(highest, lowest);
    //                    int second = Math.Max(highest, lowest);

    //                    if (IsTrending(dataCollection, start, first))
    //                    {
    //                        add(results, first);
    //                    }

    //                    if (IsTrending(dataCollection, first, second))
    //                    {
    //                        add(results, second);
    //                        add(results, first);
    //                    }

    //                    if (IsTrending(dataCollection, second, end))
    //                    {
    //                        add(results, second);
    //                    }

    //                    break;
    //                }
    //            case TrendDirection.Up:
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, start, end);

    //                    if (lowest != start && lowest != end)
    //                    {
    //                        if (IsTrending(dataCollection, start, lowest))
    //                        {
    //                            add(results, lowest);
    //                        }

    //                        if (IsTrending(dataCollection, lowest, end))
    //                        {
    //                            add(results, lowest);
    //                        }

    //                    }
    //                    else
    //                    {
    //                        List<int> highers = HigherAndHigher(dataCollection, start, end);

    //                        for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
    //                        {
    //                            lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

    //                            if (lowest == -1) continue;

    //                            if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
    //                            {
    //                                add(results, lowest);
    //                            }
    //                        }
    //                    }
    //                    break;
    //                }

    //            case TrendDirection.Down:
    //                {
    //                    highest = HighestIndexBetween(dataCollection, start, end);

    //                    if (highest != start && highest != end)
    //                    {
    //                        if (highest != start && IsTrending(dataCollection, start, highest))
    //                        {
    //                            add(results, highest);
    //                        }
    //                        else if (highest != end && IsTrending(dataCollection, highest, end))
    //                        {
    //                            add(results, highest);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        List<int> lowers = LowerAndLower(dataCollection, start, end);

    //                        for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
    //                        {
    //                            highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

    //                            if (highest == -1) continue;

    //                            if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
    //                            {
    //                                add(results, highest);
    //                            }
    //                        }
    //                    }

    //                    break;
    //                }
    //            default:
    //                throw new Exception();
    //        }

    //        if (results.Count == 0)
    //            return null;

    //        add(results, start);
    //        add(results, end);
    //        results.Sort();

    //        extras = new List<int>();
    //        List<int> others;

    //        for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
    //        {
    //            others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
    //            addRange(extras, others);
    //        }

    //        addRange(results, extras);
    //        results.Sort();

    //        return results;
    //    }


    //    public int NextMajorIndex(List<T> dataCollection, int lastMajorIndex, TrendDirection existingTrend)
    //    {
    //        T previous = dataCollection[lastMajorIndex];
    //        T record = dataCollection[lastMajorIndex + 1];
    //        T next;
    //        TrendDirection currentTrend = TrendOf(previous, record);
    //        TrendDirection nextTrend;
    //        int count = (existingTrend == TrendDirection.Inside) ? 1 : 0;

    //        for (int currentIndex = lastMajorIndex + 2; currentIndex < dataCollection.Count; currentIndex++)
    //        {
    //            next = dataCollection[currentIndex];
    //            nextTrend = TrendOf(record, next);

    //            if (nextTrend == TrendDirection.Inside)
    //                continue;
    //            else if (nextTrend == TrendDirection.Outside || nextTrend == currentTrend)
    //                count++;
    //            else  //Trend change may happen here
    //            {
    //                if (LengthOfTrend(dataCollection, currentIndex - 1, currentTrend) < threshold)
    //                    return -1;
    //                else
    //                    return currentIndex - 1;
    //            }

    //        }
    //    }

    //    public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    {
    //        List<TurningPoint<T>> results = new List<TurningPoint<T>>();
    //        TrendDirection previousTrend = TrendDirection.Inside;
    //        TrendDirection nextTrend, followingTrend;
    //        TrendDirection currentTrend = TrendDirection.Inside;

    //        T previous = dataCollection[0], record, next, following;
    //        int count = 0, count2 = 0;

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        {
    //            record = dataCollection[currentIndex];
    //            currentTrend = TrendOf(previous, record);
    //            for (int j = currentIndex + 1; j < dataCollection.Count; j++)
    //            {
    //                next = dataCollection[j];
    //                nextTrend = TrendOf(record, next);

    //                if (count >= threshold)
    //                {
    //                    previous = record;
    //                    previousTrend = (nextTrend == TrendDirection.Down) ? TrendDirection.Up : TrendDirection.Down;
    //                }
    //                else
    //                {
    //                    count2 = 1;

    //                    for (int k = j + 1; k < dataCollection.Count; k++)
    //                    {
    //                        following = dataCollection[k];
    //                        followingTrend = TrendOf(next, following);
    //                        if (followingTrend == nextTrend || followingTrend == TrendDirection.Outside)
    //                            count2++;
    //                        else if (followingTrend != TrendDirection.Inside)
    //                            continue;
    //                        else
    //                        {
    //                            if (count2 < threshold)
    //                            {
    //                                count++;
    //                            }
    //                        }
    //                    }
    //                }

    //            }

    //            //if (nextTrend == TrendDirection.None)
    //            //    continue;
    //            //else if (nextTrend != nextTrend)
    //            //{
    //            //    if (nextTrend == TrendDirection.None)
    //            //    {
    //            //        nextTrend = nextTrend;
    //            //        count = 1;
    //            //    }
    //            //    else
    //            //    {
    //            //        nextTrend = nextTrend;

    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    count++;
    //            //    if (count >= threshold && results.Count == 0)
    //            //        results.Add(new TurningPoint(nextTrend == TrendDirection.Up ? TypeOfTurning.Bottom : TypeOfTurning.Top, record));
    //            //}
    //        }

    //    }
    //    #endregion
    //    //*/
    //}


    //public class TrendParser<T>
    //    where T : RecordBase
    //{
    //    public const int DefaultMinOfTrend = 3;
    //    public const int DefaultLong = 20;

    //    public int threshold { get; private set; }

    //    private Stack<int> lowers = new Stack<int>();
    //    private Stack<int> highers = new Stack<int>();
    //    private int ceilingIndex = -1;
    //    private int floorIndex = -1;

    //    public TrendParser(int threshold)
    //    {
    //        threshold = threshold;
    //    }

    //    public TrendDirection TrendOf(T current, T next)
    //    {
    //        if (next.LastHigh > current.LastHigh)
    //        {
    //            return next.LastLow >= current.LastLow ? TrendDirection.Up : TrendDirection.Outside;
    //        }
    //        else if (next.LastHigh < current.LastHigh)
    //        {
    //            return next.LastLow < current.LastLow ? TrendDirection.Down : TrendDirection.Inside;
    //        }
    //        else
    //        {
    //            return next.LastLow <= current.LastLow ? TrendDirection.Outside : TrendDirection.Inside;
    //        }
    //    }

    //    private void getCeilingFloor(List<T> dataCollection)
    //    {
    //        double highest = double.MinValue, lowest = double.MaxValue;

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count; currentIndex++)
    //        {
    //            if (highest < dataCollection[currentIndex].LastHigh)
    //            {
    //                highest = dataCollection[currentIndex].LastHigh;
    //                ceilingIndex = currentIndex;
    //            }

    //            if (lowest > dataCollection[currentIndex].LastLow)
    //            {
    //                lowest = dataCollection[currentIndex].LastLow;
    //                floorIndex = currentIndex;
    //            }
    //        }
    //    }

    //    public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    {
    //        List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();

    //        getCeilingFloor(dataCollection);

    //        List<int> majors = new List<int>();

    //        TrendDirection trend, existingTrend = TrendDirection.Inside;

    //        T current, next, last = dataCollection[0];

    //        lowers.Clear();
    //        highers.Clear();

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        {
    //            current = dataCollection[currentIndex];

    //            if (currentIndex == ceilingIndex)
    //            {
    //                //if (existingTrend == TrendDirection.Down)
    //                //{
    //                //    existingTrend = TrendDirection.Up;
    //                //    highers.Pop();
    //                //}
    //                //else if (existingTrend != TrendDirection.Up)
    //                //    throw new Exception();
    //                lowers.Clear();
    //                highers.Clear();

    //                if (existingTrend == TrendDirection.Down && highers.Count != 0)
    //                    pivots.RemoveAt(pivots.Count-1);

    //                pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
    //                existingTrend = TrendDirection.Down;
    //                continue;
    //            }
    //            else if (currentIndex == floorIndex)
    //            {
    //                //if (existingTrend == TrendDirection.Up)
    //                //{
    //                //    existingTrend = TrendDirection.Down;
    //                //    lowers.Pop();
    //                //}
    //                //else if (existingTrend != TrendDirection.Down)
    //                //    throw new Exception();


    //                lowers.Clear();
    //                highers.Clear();

    //                if (existingTrend == TrendDirection.Up && highers.Count != 0)
    //                    pivots.RemoveAt(pivots.Count - 1);

    //                pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
    //                existingTrend = TrendDirection.Up;
    //                continue;
    //            }                

    //            for (int j = currentIndex + 1; j < dataCollection.Count; j ++ )
    //            {
    //                next = dataCollection[j];
    //                trend = TrendOf(current, next);

    //                switch(trend)
    //                {
    //                    case TrendDirection.Outside:
    //                        highers.Push(j);
    //                        lowers.Push(j);
    //                        break;
    //                    case TrendDirection.Up:
    //                        highers.Push(j);
    //                        break;
    //                    case TrendDirection.Down:
    //                        lowers.Push(j);
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                currentIndex = j - 1;
    //                switch(existingTrend)
    //                {
    //                    case TrendDirection.Outside:
    //                    case TrendDirection.Inside:
    //                        if (next.LastLow < current.LastLow && lowers.Count >= threshold)
    //                        {
    //                            existingTrend = TrendDirection.Down;
    //                            j = dataCollection.Count;
    //                            highers.Clear();
    //                        }
    //                        else if (next.LastHigh > current.LastHigh && highers.Count >= threshold)
    //                        {
    //                            existingTrend = TrendDirection.Up;
    //                            //currentIndex = j-1;
    //                            j = dataCollection.Count;
    //                            lowers.Clear();
    //                        } 
    //                        break;
    //                    case TrendDirection.Up:
    //                        if (next.LastHigh > current.LastHigh)
    //                        {
    //                            lowers.Clear();
    //                            //currentIndex = j - 1;
    //                            j = dataCollection.Count;
    //                        }
    //                        else if (next.LastLow < current.LastLow && lowers.Count >= threshold)
    //                        {
    //                            pivots.Add(new TurningPoint<T>(TypeOfTurning.Top, dataCollection[currentIndex]));
    //                            existingTrend = TrendDirection.Down;
    //                            highers.Clear();
    //                            //currentIndex = j-1;
    //                            j = dataCollection.Count;
    //                        }
    //                        break;
    //                    case TrendDirection.Down:
    //                        if (next.LastLow < current.LastLow)
    //                        {
    //                            highers.Clear();
    //                            //currentIndex = j - 1;
    //                            j = dataCollection.Count;
    //                        }
    //                        else if (next.LastHigh > current.LastHigh && highers.Count >= threshold)
    //                        {
    //                            pivots.Add(new TurningPoint<T>(TypeOfTurning.Bottom, dataCollection[currentIndex]));
    //                            existingTrend = TrendDirection.Up;
    //                            lowers.Clear();
    //                            //currentIndex = j - 1;
    //                            j = dataCollection.Count;
    //                        }
    //                        break;
    //                    default:
    //                        break;
    //                }

    //                //if (j == dataCollection.Count -1)
    //                //{
    //                //    pivots.Add(new TurningPoint<T>(existingTrend == TrendDirection.Up ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[currentIndex-1]));
    //                //    currentIndex = j;
    //                //}
    //            }

    //        }

    //        if (pivots[0].Date != dataCollection[0].Date)
    //        {
    //            TurningPoint<T> startMajor = new TurningPoint<T>(pivots[0].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top : TypeOfTurning.Bottom, dataCollection[0]);
    //            pivots.Insert(0, startMajor);
    //        }

    //        if (pivots[pivots.Count-1].Date != dataCollection[dataCollection.Count-1].Date)
    //        {
    //            TurningPoint<T> endMajor = new TurningPoint<T>(pivots[pivots.Count - 1].Kind == TypeOfTurning.Bottom ? TypeOfTurning.Top
    //                : TypeOfTurning.Bottom, dataCollection[dataCollection.Count - 1]);
    //            pivots.Add(endMajor);
    //        }

    //        //bool isEvenUp = false;

    //        //for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        //{
    //        //    trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

    //        //    if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //        //        continue;
    //        //    else if (trend == TrendDirection.Up)
    //        //        isEvenUp = (currentIndex % 2 == 0) ? true : false;
    //        //    else if (trend == TrendDirection.Down)
    //        //        isEvenUp = (currentIndex % 2 == 1) ? true : false;

    //        //    break;
    //        //}

    //        //TypeOfTurning kind;

    //        //for (int currentIndex = 0; currentIndex < majors.Count; currentIndex++)
    //        //{
    //        //    kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
    //        //    TurningPoint<T> item = new TurningPoint<T>(kind, dataCollection[majors[currentIndex]]);
    //        //    pivots.Add(item);
    //        //}

    //        return pivots;
    //    }


    //    /*/ Bad codes
    //    #region functions

    //    public int NextHigherIndex(List<T> dataCollection, int highIndex)
    //    {
    //        T record = dataCollection[highIndex];

    //        for (int index = highIndex + 1; index < dataCollection.Count; index++)
    //        {
    //            if (dataCollection[index].LastHigh > record.LastHigh)
    //                return index;
    //        }
    //        return -1;
    //    }

    //    public int NextLowerIndex(List<T> dataCollection, int lowIndex)
    //    {
    //        T record = dataCollection[lowIndex];

    //        for (int index = lowIndex + 1; index < dataCollection.Count; index++)
    //        {
    //            if (dataCollection[index].LastLow < record.LastLow)
    //                return index;
    //        }
    //        return -1;
    //    }

    //    public List<int> HigherAndHigher(List<T> item, int start, int end)
    //    {
    //        List<int> pivots = new List<int>();

    //        int index = start;

    //        do
    //        {
    //            index = NextHigherIndex(item, index);
    //            pivots.Add(index);
    //        } while (index < end && index != -1);

    //        return pivots;
    //    }

    //    public List<int> LowerAndLower(List<T> item, int start, int end)
    //    {
    //        List<int> pivots = new List<int>();

    //        int index = start;

    //        do
    //        {
    //            index = NextLowerIndex(item, index);
    //            pivots.Add(index);
    //        } while (index < end && index != -1);

    //        return pivots;
    //    }

    //    public int TrendedCount(List<T> dataCollection, int start, int end)
    //    {
    //        T record, last = dataCollection[start];

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

    //        int count = 0;
    //        if (trend == TrendDirection.Up)
    //        {
    //            for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
    //            {
    //                record = dataCollection[currentIndex];
    //                if (record.LastHigh > last.LastHigh)
    //                {
    //                    last = record;
    //                    count++;
    //                }
    //            }
    //        }
    //        else if (trend == TrendDirection.Down)
    //        {
    //            for (int currentIndex = start + 1; currentIndex <= end; currentIndex++)
    //            {
    //                record = dataCollection[currentIndex];
    //                if (record.LastLow < last.LastLow)
    //                {
    //                    last = record;
    //                    count++;
    //                }
    //            }
    //        }
    //        return count;
    //    }

    //    public bool IsTrending(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);

    //        if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //            return false;
    //        else
    //            return TrendedCount(dataCollection, start, end) >= threshold;
    //    }

    //    public int LowestIndexBetween(List<T> dataCollection, int firstHighIndex, int secondHighIndex)
    //    {
    //        if (secondHighIndex - firstHighIndex <= threshold + 1)
    //            return -1;

    //        double lowest = dataCollection[firstHighIndex].LastLow;
    //        int lowestIndex = firstHighIndex;

    //        for (int currentIndex = firstHighIndex + 1; currentIndex < secondHighIndex; currentIndex++)
    //        {
    //            if (lowest < dataCollection[currentIndex].LastLow)
    //            {
    //                lowest = dataCollection[currentIndex].LastLow;
    //                lowestIndex = currentIndex;
    //            }
    //        }
    //        return lowestIndex;
    //    }

    //    public int HighestIndexBetween(List<T> dataCollection, int firstLowIndex, int secondLowIndex)
    //    {
    //        if (secondLowIndex - firstLowIndex <= threshold + 1)
    //            return -1;

    //        double highest = dataCollection[firstLowIndex].LastHigh;
    //        int highestIndex = firstLowIndex;

    //        for (int currentIndex = firstLowIndex + 1; currentIndex < secondLowIndex; currentIndex++)
    //        {
    //            if (highest < dataCollection[currentIndex].LastHigh)
    //            {
    //                highest = dataCollection[currentIndex].LastHigh;
    //                highestIndex = currentIndex;
    //            }
    //        }
    //        return highestIndex;
    //    }

    //    private void add(List<int> results, int pivots)
    //    {
    //        if (!results.Contains(pivots))
    //            results.Add(pivots);
    //    }

    //    private void addRange(List<int> results, ICollection<int> extra)
    //    {
    //        if (extra == null || extra.Count == 0)
    //            return;

    //        foreach (int pivots in extra)
    //        {
    //            if (!results.Contains(pivots))
    //                results.Add(pivots);
    //        }
    //    }

    //    public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        if (end - start < threshold + 2)
    //            return null;

    //        List<int> results = new List<int> { start, end };
    //        List<int> extras;

    //        int highest, lowest;

    //        lowest = LowestIndexBetween(dataCollection, start, end);
    //        highest = HighestIndexBetween(dataCollection, start, end);

    //        int first = Math.Min(highest, lowest);
    //        int second = Math.Max(highest, lowest);

    //        if (second - first >= threshold && IsTrending(dataCollection, first, second))
    //        {
    //            add(results, first);
    //            add(results, second);
    //        }

    //        if (first - start >= threshold && IsTrending(dataCollection, start, first))
    //        {
    //            add(results, first);
    //        }

    //        if (end - second >= threshold && IsTrending(dataCollection, second, end))
    //        {
    //            add(results, second);
    //        }

    //        if (results.Count == 2)
    //        {
    //            TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
    //            if (trend == TrendDirection.Up)
    //            {
    //                List<int> highers = HigherAndHigher(dataCollection, start, end);

    //                for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

    //                    if (lowest == -1) continue;

    //                    if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
    //                    {
    //                        add(results, lowest);
    //                    }
    //                }
    //            }
    //            else if (trend == TrendDirection.Down)
    //            {
    //                List<int> lowers = LowerAndLower(dataCollection, start, end);

    //                for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
    //                {
    //                    highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

    //                    if (highest == -1) continue;

    //                    if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
    //                    {
    //                        add(results, highest);
    //                    }
    //                }
    //            }
    //            else
    //                throw new NotImplementedException();
    //        }

    //        results.Sort();

    //        extras = new List<int>();
    //        List<int> others;

    //        for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
    //        {
    //            others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
    //            addRange(extras, others);
    //        }

    //        addRange(results, extras);
    //        results.Sort();

    //        return results;

    //    }

    //    public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    {
    //        List<int> turnings = TurningsBetween(dataCollection, 0, dataCollection.Count - 1);
    //        List<TurningPoint<T>> pivots = new List<TurningPoint<T>>();

    //        add(turnings, 0);
    //        add(turnings, dataCollection.Count - 1);
    //        turnings.Sort();

    //        if (turnings.Count < 3)
    //            return null;

    //        TrendDirection trend;

    //        bool isEvenUp = false;

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        {
    //            trend = TrendOf(dataCollection[currentIndex], dataCollection[currentIndex + 1]);

    //            if (trend == TrendDirection.Inside || trend == TrendDirection.Outside)
    //                continue;
    //            else if (trend == TrendDirection.Up)
    //                isEvenUp = (currentIndex % 2 == 0) ? true : false;
    //            else if (trend == TrendDirection.Down)
    //                isEvenUp = (currentIndex % 2 == 1) ? true : false;

    //            break;
    //        }

    //        TypeOfTurning kind;
    //        TurningPoint<T> pivots;

    //        for (int currentIndex = 0; currentIndex < turnings.Count; currentIndex++)
    //        {
    //            kind = isEvenUp ? (currentIndex % 2 == 0 ? TypeOfTurning.Bottom : TypeOfTurning.Top) : (currentIndex % 2 == 1 ? TypeOfTurning.Bottom : TypeOfTurning.Top);
    //            pivots = new TurningPoint<T>(kind, dataCollection[currentIndex]);
    //            pivots.Add(pivots);
    //        }

    //        return pivots;
    //    }

    //    public List<int> TurningsBetween(List<T> dataCollection, int start, int end)
    //    {
    //        if (start > end)
    //        {
    //            int temp = start;
    //            start = end;
    //            end = temp;
    //        }

    //        if (end - start < threshold + 2)
    //            return null;

    //        List<int> results = new List<int>();
    //        List<int> extras;

    //        TrendDirection trend = TrendOf(dataCollection[start], dataCollection[end]);
    //        int highest, lowest;

    //        switch (trend)
    //        {
    //            case TrendDirection.Inside:
    //            case TrendDirection.Outside:
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, start, end);
    //                    highest = HighestIndexBetween(dataCollection, start, end);

    //                    int first = Math.Min(highest, lowest);
    //                    int second = Math.Max(highest, lowest);

    //                    if (IsTrending(dataCollection, start, first))
    //                    {
    //                        add(results, first);
    //                    }

    //                    if (IsTrending(dataCollection, first, second))
    //                    {
    //                        add(results, second);
    //                        add(results, first);
    //                    }

    //                    if (IsTrending(dataCollection, second, end))
    //                    {
    //                        add(results, second);
    //                    }

    //                    break;
    //                }
    //            case TrendDirection.Up:
    //                {
    //                    lowest = LowestIndexBetween(dataCollection, start, end);

    //                    if (lowest != start && lowest != end)
    //                    {
    //                        if (IsTrending(dataCollection, start, lowest))
    //                        {
    //                            add(results, lowest);
    //                        }

    //                        if (IsTrending(dataCollection, lowest, end))
    //                        {
    //                            add(results, lowest);
    //                        }

    //                    }
    //                    else
    //                    {
    //                        List<int> highers = HigherAndHigher(dataCollection, start, end);

    //                        for (int currentIndex = 0; currentIndex < highers.Count - 1; currentIndex++)
    //                        {
    //                            lowest = LowestIndexBetween(dataCollection, highers[currentIndex], highers[currentIndex + 1]);

    //                            if (lowest == -1) continue;

    //                            if (lowest != highers[currentIndex] && IsTrending(dataCollection, highers[currentIndex], lowest))
    //                            {
    //                                add(results, lowest);
    //                            }
    //                        }
    //                    }
    //                    break;
    //                }

    //            case TrendDirection.Down:
    //                {
    //                    highest = HighestIndexBetween(dataCollection, start, end);

    //                    if (highest != start && highest != end)
    //                    {
    //                        if (highest != start && IsTrending(dataCollection, start, highest))
    //                        {
    //                            add(results, highest);
    //                        }
    //                        else if (highest != end && IsTrending(dataCollection, highest, end))
    //                        {
    //                            add(results, highest);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        List<int> lowers = LowerAndLower(dataCollection, start, end);

    //                        for (int currentIndex = 0; currentIndex < lowers.Count - 1; currentIndex++)
    //                        {
    //                            highest = HighestIndexBetween(dataCollection, lowers[currentIndex], lowers[currentIndex + 1]);

    //                            if (highest == -1) continue;

    //                            if (highest != lowers[currentIndex] && IsTrending(dataCollection, lowers[currentIndex], highest))
    //                            {
    //                                add(results, highest);
    //                            }
    //                        }
    //                    }

    //                    break;
    //                }
    //            default:
    //                throw new Exception();
    //        }

    //        if (results.Count == 0)
    //            return null;

    //        add(results, start);
    //        add(results, end);
    //        results.Sort();

    //        extras = new List<int>();
    //        List<int> others;

    //        for (int currentIndex = 0; currentIndex < results.Count - 1; currentIndex++)
    //        {
    //            others = TurningsBetween(dataCollection, results[currentIndex], results[currentIndex + 1]);
    //            addRange(extras, others);
    //        }

    //        addRange(results, extras);
    //        results.Sort();

    //        return results;
    //    }


    //    public int NextMajorIndex(List<T> dataCollection, int lastMajorIndex, TrendDirection existingTrend)
    //    {
    //        T previous = dataCollection[lastMajorIndex];
    //        T record = dataCollection[lastMajorIndex + 1];
    //        T next;
    //        TrendDirection currentTrend = TrendOf(previous, record);
    //        TrendDirection nextTrend;
    //        int count = (existingTrend == TrendDirection.Inside) ? 1 : 0;

    //        for (int currentIndex = lastMajorIndex + 2; currentIndex < dataCollection.Count; currentIndex++)
    //        {
    //            next = dataCollection[currentIndex];
    //            nextTrend = TrendOf(record, next);

    //            if (nextTrend == TrendDirection.Inside)
    //                continue;
    //            else if (nextTrend == TrendDirection.Outside || nextTrend == currentTrend)
    //                count++;
    //            else  //Trend change may happen here
    //            {
    //                if (LengthOfTrend(dataCollection, currentIndex - 1, currentTrend) < threshold)
    //                    return -1;
    //                else
    //                    return currentIndex - 1;
    //            }

    //        }
    //    }

    //    public List<TurningPoint<T>> MajorsOf(List<T> dataCollection)
    //    {
    //        List<TurningPoint<T>> results = new List<TurningPoint<T>>();
    //        TrendDirection previousTrend = TrendDirection.Inside;
    //        TrendDirection nextTrend, followingTrend;
    //        TrendDirection currentTrend = TrendDirection.Inside;

    //        T previous = dataCollection[0], record, next, following;
    //        int count = 0, count2 = 0;

    //        for (int currentIndex = 0; currentIndex < dataCollection.Count - 1; currentIndex++)
    //        {
    //            record = dataCollection[currentIndex];
    //            currentTrend = TrendOf(previous, record);
    //            for (int j = currentIndex + 1; j < dataCollection.Count; j++)
    //            {
    //                next = dataCollection[j];
    //                nextTrend = TrendOf(record, next);

    //                if (count >= threshold)
    //                {
    //                    previous = record;
    //                    previousTrend = (nextTrend == TrendDirection.Down) ? TrendDirection.Up : TrendDirection.Down;
    //                }
    //                else
    //                {
    //                    count2 = 1;

    //                    for (int k = j + 1; k < dataCollection.Count; k++)
    //                    {
    //                        following = dataCollection[k];
    //                        followingTrend = TrendOf(next, following);
    //                        if (followingTrend == nextTrend || followingTrend == TrendDirection.Outside)
    //                            count2++;
    //                        else if (followingTrend != TrendDirection.Inside)
    //                            continue;
    //                        else
    //                        {
    //                            if (count2 < threshold)
    //                            {
    //                                count++;
    //                            }
    //                        }
    //                    }
    //                }

    //            }

    //            //if (nextTrend == TrendDirection.None)
    //            //    continue;
    //            //else if (nextTrend != nextTrend)
    //            //{
    //            //    if (nextTrend == TrendDirection.None)
    //            //    {
    //            //        nextTrend = nextTrend;
    //            //        count = 1;
    //            //    }
    //            //    else
    //            //    {
    //            //        nextTrend = nextTrend;

    //            //    }
    //            //}
    //            //else
    //            //{
    //            //    count++;
    //            //    if (count >= threshold && results.Count == 0)
    //            //        results.Add(new TurningPoint(nextTrend == TrendDirection.Up ? TypeOfTurning.Bottom : TypeOfTurning.Top, record));
    //            //}
    //        }

    //    }
    //    #endregion
    //    //*/
    //}
}
