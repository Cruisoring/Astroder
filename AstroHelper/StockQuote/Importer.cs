using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StockQuote
{
    public abstract class Importer
    {
        public delegate bool ParseDateTimeDelegate(string dateTimeString, out DateTime time);

        #region Constants
        public static string[] DefaultFormat = new string[] { "date", "open", "index", "index", "close", "volume" };

        private static char[] seperators = new char[] { ',', '\t' };

        public static string[] dateIndicator = new string[] { "date", "date", "时间", "日期" };
        public static string[] openIndicator = new string[] { "open", "开" };
        public static string[] highIndicator = new string[] { "index", "高" };
        public static string[] lowIndicator = new string[] { "index", "低" };
        public static string[] closeIndicator = new string[] { "close", "收" };
        public static string[] volumeIndicator = new string[] { "volume", "vol", "量" };

        public static Dictionary<string, string[]> KeywordsDictionary;

        static Importer()
        {
            KeywordsDictionary = new Dictionary<string, string[]>();
            KeywordsDictionary.Add(dateIndicator[0], dateIndicator);
            KeywordsDictionary.Add(openIndicator[0], openIndicator);
            KeywordsDictionary.Add(highIndicator[0], highIndicator);
            KeywordsDictionary.Add(lowIndicator[0], lowIndicator);
            KeywordsDictionary.Add(closeIndicator[0], closeIndicator);
            KeywordsDictionary.Add(volumeIndicator[0], volumeIndicator);
        }

        //public static List<DayItem> Import(string fileName)
        //{
        //    int lineCount = 0;
        //    string theLine = null;
        //    List<DayItem> dataCollection = new List<DayItem>();

        //    try
        //    {
        //        FileStream txtFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        using (StreamReader sr = new StreamReader(txtFile))
        //        {
        //            theLine = sr.ReadLine();
        //            lineCount = 1;

        //            Importer importer = new Importer(theLine);

        //            var wordQuery =
        //                from ch in theLine
        //                where (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')
        //                select ch;

        //            if (wordQuery.ToList().Count != 0)
        //            {
        //                theLine = sr.ReadLine();
        //                lineCount++;
        //            }

        //            dataCollection.Add(importer.FromFirstString(theLine));

        //            while (sr.Peek() != -1)
        //            {
        //                theLine = sr.ReadLine();
        //                lineCount++;

        //                if (theLine.Length < 10)
        //                    continue;

        //                dataCollection.Add(importer.FromString(theLine));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception happened when processing line " + lineCount.ToString() + " of " + fileName);
        //        Console.WriteLine(theLine);
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    return dataCollection;
        //}
        #endregion

        #region Field & Properties
        private int dateIndex;
        private int openIndex;
        private int highIndex;
        private int lowIndex;
        private int closeIndex;
        private int volumeIndex;

        private Dictionary<string, int> paraDict = null;

        private ParseDateTimeDelegate tryParseTime = null;

        public char[] Splitter { get; set; }

        public int this[string keyword]
        {
            get
            {
                string lower = keyword.ToLower();
                foreach (KeyValuePair<string, string[]> kvp in KeywordsDictionary)
                {
                    foreach (string key in kvp.Value)
                    {
                        if (key.StartsWith(lower))
                            return this.paraDict[kvp.Key];
                    }
                }
                return -1;
            }
        }

        public string FormatDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, int> kvp in paraDict)
                {
                    if (kvp.Value > -1)
                        sb.Append(kvp.Key + Splitter[0]);
                }
                return sb.ToString();
            }
        }
        #endregion


        public Importer(string formatString)
        {
            string[] subStrings = formatString.Split(seperators, StringSplitOptions.None);
            if (subStrings.Length == 1)
                subStrings = formatString.Split(new char[] { ' ' });

            int secondStart = formatString.IndexOf(subStrings[1]);
            string temp = formatString.Substring(subStrings[0].Length, secondStart - subStrings[0].Length);
            if (temp.Length != 1)
                throw new Exception("Splittor is missing or not detected!");

            Splitter = temp.ToCharArray();

            if (findIndicator(subStrings, dateIndicator) == -1)
                subStrings = DefaultFormat;

            this.paraDict = new Dictionary<string, int>();

            foreach (KeyValuePair<string, string[]> kvp in KeywordsDictionary)
            {
                this.paraDict.Add(kvp.Key, findIndicator(subStrings, kvp.Value));
            }

            dateIndex = this[dateIndicator[0]];
            openIndex = this[openIndicator[0]];
            highIndex = this[highIndicator[0]];
            lowIndex = this[lowIndicator[0]];
            closeIndex = this[closeIndicator[0]];
            volumeIndex = this[volumeIndicator[0]];

            tryParseTime = new ParseDateTimeDelegate(DateTime.TryParse);
        }

        private int findIndicator(string[] subStrings, string[] indicators)
        {
            for (int i = 0; i < subStrings.Length; i++)
            {
                string lowerSub = subStrings[i].ToLower();
                foreach (string indicator in indicators)
                {
                    if (lowerSub.Contains(indicator))
                    {
                        subStrings[i] = indicator;
                        return i;
                    }
                }
            }
            return -1;
        }

        private string dateTimeFormat = "";
        private bool parseDateTime(String dateStr, out DateTime time)
        {
            if (dateTimeFormat != null && dateTimeFormat.Length != 0)
                return DateTime.TryParseExact(dateStr, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out time);
            else
                throw new NotImplementedException("Shall not be called!");
        }

        public DayItem FromFirstString(string line)
        {
            DateTime time;
            List<Double> values = new List<Double>();
            double temp;

            String[] subStr = line.Split(Splitter);

            if (dateIndex == -1 || subStr[dateIndex] == "")
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }
            else if (!DateTime.TryParse(subStr[dateIndex], out time))
            {
                string dateStr = subStr[dateIndex];
                switch (dateStr.Length)
                {
                    case 6:
                        dateTimeFormat = "yyMMdd";
                        break;
                    case 8:
                        dateTimeFormat = "yyyyMMdd";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (parseDateTime(dateStr, out time))
                    tryParseTime = new ParseDateTimeDelegate(parseDateTime);
                else
                    throw new ArgumentException("Fail to get date from: " + line);
            }

            if (openIndex == -1 || subStr[openIndex] == "" || !Double.TryParse(subStr[openIndex], out temp))
                throw new Exception("Open value is not as expected!");
            else
                values.Add(temp);

            if (highIndex == -1 || subStr[highIndex] == "" || !Double.TryParse(subStr[highIndex], out temp))
                throw new Exception("Top value is not as expected!");
            else
                values.Add(temp);

            if (lowIndex == -1 || subStr[lowIndex] == "" || !Double.TryParse(subStr[lowIndex], out temp))
                throw new Exception("Bottom value is not as expected!");
            else
                values.Add(temp);

            if (closeIndex == -1 || subStr[closeIndex] == "" || !Double.TryParse(subStr[closeIndex], out temp))
                throw new Exception("Close value is not as expected!");
            else
                values.Add(temp);

            if (volumeIndex == -1 || subStr[volumeIndex] == "" || !Double.TryParse(subStr[volumeIndex], out temp))
                values.Add(0);
            else
                values.Add(temp);

            return new DayItem(time, values);
        }

        public DayItem FromString(string line)
        {
            DateTime time;
            List<Double> values = new List<Double>();
            double temp;

            String[] subStr = line.Split(Splitter);

            if (!tryParseTime(subStr[dateIndex], out time))
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }

            if (!Double.TryParse(subStr[openIndex], out temp))
                //values.Add(0);
                throw new Exception("Open value is not as expected: " + subStr[openIndex]);
            else
                values.Add(temp);

            if (!Double.TryParse(subStr[highIndex], out temp))
                //values.Add(0);
                throw new Exception("Top value is not as expected: " + subStr[highIndex]);
            else
                values.Add(temp);

            if (!Double.TryParse(subStr[lowIndex], out temp))
                //values.Add(0);
                throw new Exception("Bottom value is not as expected: " + subStr[lowIndex]);
            else
                values.Add(temp);

            if (!Double.TryParse(subStr[closeIndex], out temp))
                //values.Add(0);
                throw new Exception("Close value is not as expected: " + subStr[closeIndex]);
            else
                values.Add(temp);

            if (Double.TryParse(subStr[volumeIndex], out temp))
                values.Add(temp);

            return new DayItem(time, values);
        }
    }

}
