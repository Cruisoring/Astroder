using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace StockQuote
{
    public class TextImporter
    {
        public delegate bool DateTimeParseDelegate(string dateTimeString, out DateTime time);

        #region Constants

        public static string[] DefaultFormat = new string[] { "date", "open", "high", "low", "close", "volume" };

        public static string[] PredefinedDateFormats = new string[]
        {
            "MMMM dd, yyyy",
            "dddd MMMM dd, yyyy",
            "yyMMdd",
            "d",
            "D",
            "F",
            "M",
            "yyyy'-'MM'-'dd'"
        };
        private static char[] seperators = new char[] { '\t', ' ', ',' };

        public static string[] dateIndicator = new string[] { "date", "date", "时间", "日期" };
        public static string[] openIndicator = new string[] { "open", "开" };
        public static string[] highIndicator = new string[] { "high", "高" };
        public static string[] lowIndicator = new string[] { "low", "低" };
        public static string[] closeIndicator = new string[] { "close", "收" };
        public static string[] volumeIndicator = new string[] { "volume", "vol", "量" };

        public static Dictionary<int, string[]> KeywordsDictionary = new Dictionary<int, string[]>{
            { 0, dateIndicator},
            { 1, openIndicator},
            { 2, highIndicator},
            { 3, lowIndicator},
            { 4, closeIndicator},
            { 5, volumeIndicator}
        };

        public static List<DayItem> Import(string fileName)
        {
            int lineCount = 0;
            string theLine = null;
            List<DayItem> items = new List<DayItem>();

            try
            {
                FileStream txtFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(txtFile))
                {
                    theLine = sr.ReadLine();
                    lineCount = 1;

                    TextImporter importer = new TextImporter(theLine);

                    var wordQuery =
                        from ch in theLine
                        where (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')
                        select ch;

                    if (wordQuery.ToList().Count != 0)
                    {
                        theLine = sr.ReadLine();
                        lineCount++;
                    }

                    items.Add(importer.FromFirstString(theLine));

                    while (sr.Peek() != -1)
                    {
                        theLine = sr.ReadLine();
                        lineCount++;

                        if (theLine.Length < 10)
                            continue;

                        items.Add(importer.FromString(theLine));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception happened when processing line " + lineCount.ToString() + " of " + fileName);
                Console.WriteLine(theLine);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return items;
        }
        #endregion

        #region Field & Properties
        public int[] IndexPositions = new int[] { -1, -1, -1, -1, -1, -1 };

        private DateTimeParseDelegate dateTimeParser = null;

        public char[] Splitter { get; set; }

        public string FormatDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<int, string[]> kvp in KeywordsDictionary)
                {
                    sb.Append(kvp.Value[0] + Splitter[0]);
                }
                return sb.ToString();
            }
        }
        #endregion


        public TextImporter(string formatString)
        {
            string[] subStrings = null;

            for (int i = 0; i < seperators.Length; i++)
            {
                subStrings = formatString.Split(new char[] { seperators[i] }, StringSplitOptions.None);
                if (subStrings.Length < 4)
                    continue;

                Splitter = new char[] { seperators[i] };
                break;
            }

            if (Splitter.Length != 1)
                throw new Exception("Splitter is missing or not detected!");

            if (findIndicator(subStrings, dateIndicator) == -1)
                subStrings = DefaultFormat;

            foreach (KeyValuePair<int, string[]> kvp in KeywordsDictionary)
            {
                IndexPositions[kvp.Key] = findIndicator(subStrings, kvp.Value);
            }

            dateTimeParser = new DateTimeParseDelegate(DateTime.TryParse);
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
        private CultureInfo theCultureInfo = null;
        private bool parseDateTime(String dateStr, out DateTime time)
        {
            if (dateTimeFormat != null && dateTimeFormat.Length != 0)
                return DateTime.TryParseExact(dateStr, dateTimeFormat, theCultureInfo,
                    DateTimeStyles.AssumeLocal, out time);
            else
                throw new NotImplementedException("Shall not be called!");
        }

        private bool parseWithPredefinedFormat(String dateStr, out DateTime time)
        {
            time = DateTime.Now;

            foreach (string format in PredefinedDateFormats)
            {
                if (DateTime.TryParseExact(dateStr, format, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeLocal, out time))
                {
                    dateTimeFormat = format;
                    theCultureInfo = new CultureInfo("en-US");
                    return true;
                }
                else if (DateTime.TryParseExact(dateStr, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out time))
                {
                    dateTimeFormat = format;
                    theCultureInfo = CultureInfo.CurrentCulture;
                    return true;
                }
            }

            return false;
        }

        public DayItem FromFirstString(string line)
        {
            DateTime time;

            String[] subStr = line.Split(Splitter);

            if (IndexPositions[0] == -1 || subStr[0] == "")
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }
            else if (!DateTime.TryParse(subStr[IndexPositions[0]], out time))
            {
                string dateStr = subStr[IndexPositions[0]];
                switch (dateStr.Length)
                {
                    case 6:
                        dateTimeFormat = "yyMMdd";
                        break;
                    case 8:
                        dateTimeFormat = "yyyyMMdd";
                        break;
                    default:
                        break;
                }

                if (dateTimeFormat != null && dateTimeFormat.Length != 0 && parseDateTime(dateStr, out time))
                    dateTimeParser = new DateTimeParseDelegate(parseDateTime);
                else if (parseWithPredefinedFormat(dateStr, out time))
                    dateTimeParser = new DateTimeParseDelegate(parseDateTime);
                else
                    throw new ArgumentException("Fail to get date from: " + line);
            }

            List<Double> values = getValues(subStr);

            return new DayItem(time, values);
        }

        private List<double> getValues(String[] subStr)
        {
            List<Double> values = new List<Double>();
            double temp;

            for (int i = 1; i < 5; i++)
            {
                if (IndexPositions[i] != -1 && subStr[IndexPositions[i]] != "" && Double.TryParse(subStr[IndexPositions[i]], out temp))
                    values.Add(temp);
            }

            if (values.Count != 4)
            {
                if (values.Count == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        values.Add(values[0]);
                    }
                }
                else
                    throw new Exception();
            }

            if (IndexPositions[5] == -1 || subStr[IndexPositions[5]] == "" || !Double.TryParse(subStr[IndexPositions[5]], out temp))
                values.Add(0);
            else
                values.Add(temp);

            return values;
        }

        public DayItem FromString(string line)
        {
            DateTime time;

            String[] subStr = line.Split(Splitter);

            if (!dateTimeParser(subStr[IndexPositions[0]], out time))
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }

            List<Double> values = getValues(subStr);

            return new DayItem(time, values);
        }
    }


}
