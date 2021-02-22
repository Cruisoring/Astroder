using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImporter
{
    [Flags]
    public enum GroupingFlag
    {
        Default = 3,
        OddYearOnly = 1,
        EvenYearOnly = 2,

        AsIs = 16,
        BackwardCompatible = 32,
        ForewardCompatible = 64
    }

    public class GroupedQuoteCollection
    {
        public static int DaysForRollover = 30;

        public CommodityInfomation Commodity { get; set; }
        public SortedDictionary<ContractInfomation, QuoteCollection> Group = new SortedDictionary<ContractInfomation, QuoteCollection>();

        public QuoteCollection this[int year, int month]
        {
             get
             {
                 ContractInfomation contract = new ContractInfomation(Commodity, year, month);

                 return Group.ContainsKey(contract) ? Group[contract] : null;
             }
        }

        public QuoteCollection this[int month, GroupingFlag flag]
        {
            get
            {
                GroupingFlag yearFlag = flag & GroupingFlag.Default;

                if (yearFlag == GroupingFlag.EvenYearOnly)
                {
                    List<QuoteCollection> evenYearQuotes = QuotesOfMonth(month, GroupingFlag.EvenYearOnly);

                    List<Quote> records = quotesFromCollections(evenYearQuotes);

                    string name = string.Format("{0}{1}Even", Commodity.Description, MonthCodes.MonthCodeOf(month).Name);
                    return new QuoteCollection(name, "", RecordType.DayRecord, records);
                }
                else if (yearFlag == GroupingFlag.OddYearOnly)
                {
                    List<QuoteCollection> oddYearQuotes = QuotesOfMonth(month, GroupingFlag.OddYearOnly);

                    List<Quote> records = quotesFromCollections(oddYearQuotes);

                    string name = string.Format("{0}{1}Odd", Commodity.Description, MonthCodes.MonthCodeOf(month).Name);
                    return new QuoteCollection(name, "", RecordType.DayRecord, records);
                }
                else
                    return null;
            }
        }

        public List<QuoteCollection> this[int month]
        {
            get
            {
                List<QuoteCollection> result = new  List<QuoteCollection>();

                List<QuoteCollection> quotes = Group.Values.ToList();
                bool isMergeable = true;

                for (int i = 1; i < quotes.Count; i ++ )
                {
                    if (quotes[i-1].Until >= quotes[i].Since)
                    {
                        isMergeable = false;
                        break;
                    }
                }

                if (isMergeable)
                {
                    List<QuoteCollection> allYearQuotes = QuotesOfMonth(month, GroupingFlag.Default);

                    List<Quote> records = quotesFromCollections(allYearQuotes);

                    string name = string.Format("{0}{1}", Commodity.Description, MonthCodes.MonthCodeOf(month).Name);
                    result.Add(new QuoteCollection(name, "", RecordType.DayRecord, records));
                }
                else
                {
                    result.Add(this[month, GroupingFlag.EvenYearOnly]);
                    result.Add(this[month, GroupingFlag.OddYearOnly]);
                }
                return result;
            }
        }

        private int indexOfMaxVolume(List<QuoteCollection> collections, int currentIndex, DateTimeOffset date)
        {
            int maxIndex = -1;
            double maxVolume = -1;

            for (int i = currentIndex; i < collections.Count; i ++)
            {
                if (collections[i].Since > date)
                    continue;

                Quote theQuote = collections[i][date];

                if (theQuote == null)
                    continue;

                if (theQuote.Volume > maxVolume)
                {
                    maxVolume = theQuote.Volume;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public QuoteCollection this[GroupingFlag flag, bool isVolumeBased]
        {
            get
            {
                Dictionary<int, int[]> indexDict = new Dictionary<int, int[]>();
                List<QuoteCollection> collections = Group.Values.ToList();

                DateTimeOffset rollover = collections[0].Since;

                int first, last;
                if (!isVolumeBased)
                {
                    for (int i = 0; i < collections.Count; i++)
                    {
                        QuoteCollection current = collections[i];

                        if (current.Since <= rollover) //&& current.Until> startDate.AddDays(DaysForRollover)
                        {
                            double oa = rollover.UtcDateTime.ToOADate();
                            first = current.QuoteOADates.FindIndex(x => x >= oa);

                            if (i == collections.Count - 1)
                                last = current.Count - 1;
                            else
                            {
                                oa = current.Until.AddDays(-DaysForRollover).UtcDateTime.ToOADate();
                                last = current.QuoteOADates.FindIndex(x => x >= oa);
                            }
                            rollover = current.Until.AddDays(-DaysForRollover);
                            indexDict.Add(i, new int[] { first, last });
                        }
                        else
                        {
                            if (i == collections.Count - 1)
                                throw new Exception(String.Format("Record after {0} are absent!", rollover));
                        }
                    }
                }
                else
                {
                    List<DateTimeOffset> ranges = new List<DateTimeOffset>();

                    for (int i = 0; i < collections.Count; i++)
                    {
                        ranges.Add(collections[i].Since);
                    }

                    DateTimeOffset startDate = collections[0].Since;
                    double oa;

                    for (int i = 0; i < collections.Count - 1; i++)
                    {
                        oa = startDate.UtcDateTime.ToOADate();
                        QuoteCollection current = collections[i];

                        DateTimeOffset nextSince = collections[i + 1].Since;

                        for (int j = i + 2; j < collections.Count; j ++ )
                        {
                            if (nextSince > collections[j].Since)
                                nextSince = collections[j].Since;
                        }

                        if (startDate < nextSince)
                        {
                            first = current.QuoteOADates.FindIndex(x => x >= oa);
                        }
                        else
                        {
                            int collectionIndex = indexOfMaxVolume(collections, i, startDate);
                            if (i != collectionIndex)
                                continue;

                            first = current.QuoteOADates.FindIndex(x => x >= oa);
                        }

                        if (i == collections.Count - 1)
                            last = current.Count - 1;
                        else
                        {
                            last = first;
                            for (int j = first + 1; j < current.Count; j++)
                            {
                                DateTimeOffset endDate = current.DataCollection[j].Time;

                                int collectionIndex = indexOfMaxVolume(collections, i, endDate);

                                if (collectionIndex == -1)
                                {
                                    endDate = current.DataCollection[j - 1].Time;
                                    if (indexOfMaxVolume(collections, i, endDate) == i)
                                    {
                                        last = j - 1;
                                        break;
                                    }
                                }

                                if (i != collectionIndex)
                                {
                                    last = j;
                                    break;
                                }
                            }
                        }

                        startDate = current.DataCollection[last].Time;
                        indexDict.Add(i, new int[] { first, last });
                    }
                }

                List<double> multipliers = new List<double>() { 1 };
                List<double> deltas = new List<double>() { 0 };
                List<double> adjustments = new List<double>() { 0 };
                double folds = 1, multiplier=1, dif = 0;
                bool multiplyNeeded = false;

                List<int> collectionIndices = indexDict.Keys.ToList();
                for (int i=1; i < collectionIndices.Count; i ++)
                {
                    int currentIndex = collectionIndices[i];
                    int lastIndex = collectionIndices[i - 1];
                    Quote quoteEnd = collections[lastIndex].DataCollection[indexDict[lastIndex][1]];
                    Quote quoteStart = collections[currentIndex].DataCollection[indexDict[currentIndex][0]];

                    double around = quoteEnd.Close / quoteStart.Close;

                    if (around > 10 || around < 0.1)
                    {
                        double x = Math.Log10(around);
                        folds = Math.Pow(10, Math.Round(x));
                        multiplyNeeded = true;
                    }
                    else
                        folds = 1;

                    multiplier *= folds;
                    multipliers.Add(multiplier);

                    if (folds != 1)
                        dif = Math.Round((quoteEnd.Close - quoteStart.Close * folds) / Commodity.MinTick) * Commodity.MinTick;
                    else
                        dif = Math.Round(((quoteEnd.Close - quoteStart.Close) * multiplier) / Commodity.MinTick) * Commodity.MinTick;

                    deltas.Add(dif);
                }

                if (flag == GroupingFlag.BackwardCompatible)
                {
                    for (int i = 1; i < deltas.Count; i++)
                    {
                        //factor *= multipliers[i];
                        double adj = Math.Round(deltas[i] + adjustments[i - 1], 4);
                        adjustments.Add(adj);
                    }
                }
                else if (flag == GroupingFlag.ForewardCompatible)
                {
                    for (int i = deltas.Count - 2; i >= 0; i--)
                    {
                        //factor *= multipliers[i];
                        double adj = Math.Round(adjustments[0] - deltas[i + 1], 4);
                        adjustments.Insert(0, adj);
                    }
                }
                else
                {
                    for (int i = 0; i < collections.Count; i++)
                    {
                        adjustments.Insert(0, 0);
                    }
                }

                List<Quote> continuous = new List<Quote>();

                if (multiplyNeeded)
                {
                    int j = 0;
                    foreach (KeyValuePair<int, int[]> kvp in indexDict)
                    {
                        QuoteCollection current = collections[kvp.Key];
                        double divisor = multipliers[j];
                        double delta = adjustments[j++];

                        for (int i = kvp.Value[0]; i < kvp.Value[1]; i++)
                        {
                            continuous.Add(new Quote(current.DataCollection[i], delta, divisor));
                        }
                        Console.WriteLine("For {0} to {1}, delta = {2}, divisor={3}", 
                            current.DataCollection[kvp.Value[0]].Time, current.DataCollection[kvp.Value[1]].Time, delta, divisor);
                    }

                }
                else
                {
                    int j = 0;
                    foreach (KeyValuePair<int, int[]> kvp in indexDict)
                    {
                        QuoteCollection current = collections[kvp.Key];
                        double delta = adjustments[j++];

                        for (int i = kvp.Value[0]; i < kvp.Value[1]; i ++ )
                        {
                            continuous.Add(new Quote(current.DataCollection[i], delta));
                        }
                        Console.WriteLine("For {0} to {1}, delta = {2}", current.DataCollection[kvp.Value[0]].Time, current.DataCollection[kvp.Value[1]].Time, delta);
                    }

                }


                string name = string.Format("{0}{1}", Commodity.Description, flag);
                return new QuoteCollection(name, "", RecordType.DayRecord, continuous);

                //string name = string.Format("{0}{1}", Commodity.Description, flag);
                //return new QuoteCollection(name, "", RecordType.DayRecord, continuous);

                //List<List<Quote>> recordSet = new List<List<Quote>>();
                //List<double> deltas = new List<double>() {0};
                //List<double> multipliers = new List<double>() { 1 };
                //List<double> adjustments = new List<double>(){0};
                //bool multiplyNeeded = false;
                //bool bypassNext = false;

                //DateTimeOffset firstDay = collections[0].Since;

                //double factor = 1;
                //for ( int i = 0; i < collections.Count-1; i ++)
                //{
                //    if (bypassNext)
                //    {
                //        recordSet.Add(new List<Quote>());
                //        multipliers.Add(1);
                //        deltas.Add(0);
                //        bypassNext = false;
                //        continue;
                //    }

                //    QuoteCollection collection = collections[i];
                //    QuoteCollection nextCollection = collections[i + 1];

                //    DateTimeOffset end = collection.Until;
                //    DateTimeOffset nextStart = (i == collections.Count-1) ? DateTimeOffset.MaxValue : nextCollection.Since;

                //    if (i < collections.Count-2 && nextStart > end && collections[i+2].Since < end)
                //    {
                //        bypassNext = true;
                //        nextCollection = collections[i + 2];
                //        nextStart =(i == collections.Count-3) ? DateTimeOffset.MaxValue : nextCollection.Since;
                //    }

                //    DateTimeOffset last = (nextStart > end) ? end : (nextStart < end.AddDays(-DaysForRollover) ? end.AddDays(-DaysForRollover) : nextStart);

                //    double temp = firstDay.UtcDateTime.ToOADate();
                //    int firstIndex = collection.QuoteOADates.FindIndex(x => x >= temp);
                //    temp = last.UtcDateTime.ToOADate();
                //    int lastIndex = last >= end ? collection.Count -1 : collection.QuoteOADates.FindIndex(x => x >= temp);
                //    List<Quote> conceredQuotes = new List<Quote>();

                //    for (int j = firstIndex; j < lastIndex; j ++ )
                //    {
                //        conceredQuotes.Add(collection.DataCollection[j]);
                //    }

                //    double dif = 0, folds = 1;

                //    Quote theOldLast = collection.DataCollection[lastIndex];
                //    Quote theNextCorrespondent = collections[i + 1][theOldLast.Time];

                //    if (theNextCorrespondent == null)
                //    {
                //        theNextCorrespondent = collections[i + 1][theOldLast.Time.AddDays(1)];

                //        if (theNextCorrespondent == null)
                //            theNextCorrespondent = collections[i + 1][theOldLast.Time.AddDays(2)];
                //    }

                //    double around = theOldLast.Close / theNextCorrespondent.Close;

                //    if (around > 10 || around < 0.1)
                //    {
                //        //double magNew = Math.Floor(Math.Log10(theNextCorrespondent.Close));
                //        //double magOld = Math.Floor(Math.Log10(theOldLast.Close));
                //        //folds = Math.Pow(10, magNew - magOld);
                //        double x = Math.Log10(around);
                //        folds = Math.Pow(10, Math.Round(x));
                //        multiplyNeeded = true;
                //    }

                //    factor *= folds;
                //    multipliers.Add(factor);

                //    if (flag != GroupingFlag.AsIs && i != collections.Count - 1)
                //    {
                //        if (folds != 1)
                //            dif = Math.Round((theOldLast.Close - theNextCorrespondent.Close*folds) / Commodity.MinTick) * Commodity.MinTick;
                //        else
                //            dif = Math.Round(((theOldLast.Close - theNextCorrespondent.Close )* factor) / Commodity.MinTick) * Commodity.MinTick;
                //    }

                //    deltas.Add(dif);

                //    recordSet.Add(conceredQuotes);

                //    if (conceredQuotes.Count != 0)
                //        Console.WriteLine("Append records between {0} and {1}, from {2}, delta = {3}",
                //        collection.DataCollection[firstIndex].Time.ToString("yyyy-MM-dd"), collection.DataCollection[lastIndex - 1].Time.ToString("yyyy-MM-dd"), collection.Name, dif);
                //    else
                //        Console.WriteLine("No records appended!");

                //    firstDay = last;
                //}

                //if (flag == GroupingFlag.BackwardCompatible)
                //{
                //    for (int i = 1; i < deltas.Count; i++)
                //    {
                //        //factor *= multipliers[i];
                //        double adj = Math.Round(deltas[i] + adjustments[i - 1], 4);
                //        adjustments.Add(adj);
                //    }
                //}
                //else if (flag == GroupingFlag.ForewardCompatible)
                //{
                //    for (int i = collections.Count - 2; i >= 0; i--)
                //    {
                //        //factor *= multipliers[i];
                //        double adj = Math.Round(adjustments[0]  - deltas[i + 1], 4);
                //        adjustments.Insert(0, adj);
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < collections.Count; i ++ )
                //    {
                //        adjustments.Insert(0, 0);
                //    }
                //}

                //List<Quote> continuous = new List<Quote>();

                //if (multiplyNeeded)
                //{
                //    int multiplyFrom = multipliers.FindIndex(x => x != 1);

                //    for (int index = 0; index < multiplyFrom; index++)
                //    {
                //        List<Quote> part = recordSet[index];
                //        double delta = adjustments[index];

                //        Console.WriteLine("For {0}, the adjustment = {1}", flag, delta);
                //        foreach (Quote record in part)
                //        {
                //            continuous.Add(new Quote(record, delta));
                //        }
                //    }

                //    for (int index = multiplyFrom; index < recordSet.Count; index++)
                //    {
                //        List<Quote> part = recordSet[index];
                //        double delta = adjustments[index];
                //        double divisor =multipliers[multiplyFrom];

                //        Console.WriteLine("For {0}, the adjustment = {1}, divided by {2}", flag, delta, divisor);
                //        foreach (Quote record in part)
                //        {
                //            continuous.Add(new Quote(record, delta, divisor));
                //        }
                //    }

                //}
                //else
                //{
                //    for (int index = 0; index < recordSet.Count; index++)
                //    {
                //        List<Quote> part = recordSet[index];
                //        double delta = adjustments[index];

                //        Console.WriteLine("For {0}, the adjustment = {1}", flag, delta);
                //        foreach (Quote record in part)
                //        {
                //            continuous.Add(new Quote(record, delta));
                //        }
                //    }
                //}
            }
        }

        private static List<Quote> quotesFromCollections(List<QuoteCollection> collections)
        {
            List<int> startIndexes = new List<int>(){0};

            for (int i = 1; i < collections.Count; i++)
            {
                if (collections[i - 1].Until >= collections[i].Since)
                {
                    double temp = collections[i - 1].Until.UtcDateTime.ToOADate();
                    int start = collections[i].QuoteOADates.FindIndex(x => x > temp);
                    startIndexes.Add(start);
                }
                else
                    startIndexes.Add(0);
            }

            List<Quote> records = new List<Quote>();


            for ( int index = 0; index < collections.Count; index ++)
            {
                QuoteCollection quotes =collections[index];
                int start = startIndexes[index];
                for(int i= start; i < quotes.Count; i ++)
                {
                    records.Add(new Quote(quotes.DataCollection[i]));
                }
            }

            return records;
        }

        public List<QuoteCollection> QuotesOfMonth(int month, GroupingFlag flag)
        {
            if (!Commodity.HasContractOn(month) || (flag != GroupingFlag.EvenYearOnly && flag != GroupingFlag.OddYearOnly && flag != GroupingFlag.Default))
                return null;

            List<QuoteCollection> result = new List<QuoteCollection>();

            foreach (KeyValuePair<ContractInfomation, QuoteCollection> kvp in Group)
            {
                if (kvp.Key.Month != month)
                    continue;
                else if (flag == GroupingFlag.EvenYearOnly && kvp.Key.Year % 2 != 0)
                    continue;
                else if (flag == GroupingFlag.OddYearOnly && kvp.Key.Year % 2 == 0)
                    continue;
                
                result.Add(kvp.Value);
            }
            return result;
        }


        public GroupedQuoteCollection(CommodityInfomation commodity, SortedDictionary<ContractInfomation, QuoteCollection> theGroup)
        {
            Commodity = commodity;
            Group = theGroup;
        }

    }
}
