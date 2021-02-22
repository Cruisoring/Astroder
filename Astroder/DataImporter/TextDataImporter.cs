using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace DataImporter
{
    public class TextDataImporter
    {
        public delegate bool DateTimeParseDelegate(string dateTimeString, out DateTimeOffset time);

        #region Constants
        public static TimeSpan DefaultTimeCorrection = TimeSpan.Zero;

        public static string[] DefaultLineFormat = new string[] { "date", "open", "high", "low", "close", "volume" };

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

        public static Dictionary<int, string[]> KeywordsDictionary = new Dictionary<int, string[]>{
            { 0, new string[] { "date", "date", "时间", "日期" } },
            { 1, new string[] { "open", "开" }},
            { 2, new string[] { "high", "高" }},
            { 3, new string[] { "low", "低" }},
            { 4, new string[] { "close", "收" }},
            { 5, new string[] { "volume", "vol", "量" }}
        };

        public static List<Quote> Import(string fileName)
        {
            int lineCount = 0;
            string theLine = null;
            List<Quote> items = new List<Quote>();

            try
            {
                FileStream txtFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(txtFile, Encoding.GetEncoding("gb2312")))
                {
                    theLine = sr.ReadLine();
                    lineCount = 1;

                    TextDataImporter importer = new TextDataImporter(theLine);

                    string[] subStrings = theLine.Split(importer.Splitter);

                    bool noNumbers = true;
                    foreach (string s in subStrings)
                    {
                        double result;
                        if (!Double.TryParse(s, out result))
                            continue;

                        noNumbers = false;
                        break;
                    }

                    if (noNumbers)
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

        private static bool tryParseAsLocal(string dateStr, out DateTimeOffset targetTime)
        {
            return DateTimeOffset.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out targetTime);
        }

        private static bool tryParseAsUtc(string dateStr, out DateTimeOffset targetTime)
        {
            return DateTimeOffset.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out targetTime);
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

        #region Constructors
        public TextDataImporter(string formatString)
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

            if (findIndicator(subStrings, KeywordsDictionary[0]) == -1)
                subStrings = DefaultLineFormat;

            foreach (KeyValuePair<int, string[]> kvp in KeywordsDictionary)
            {
                IndexPositions[kvp.Key] = findIndicator(subStrings, kvp.Value);
            }

            dateTimeParser = new DateTimeParseDelegate(tryParseAsUtc);
        }

        #endregion

        #region Functions
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
        private bool parseDateTime(String dateStr, out DateTimeOffset time)
        {
            if (dateTimeFormat != null && dateTimeFormat.Length != 0)
            {
                return DateTimeOffset.TryParseExact(dateStr, dateTimeFormat, theCultureInfo,
                    DateTimeStyles.AssumeUniversal, out time);
            }
            else
                throw new NotImplementedException("Shall not be called!");
        }

        private bool parseWithPredefinedFormat(String dateStr, out DateTimeOffset time)
        {
            time = DateTimeOffset.UtcNow;

            foreach (string format in PredefinedDateFormats)
            {
                if (DateTimeOffset.TryParseExact(dateStr, format, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeLocal, out time))
                {
                    dateTimeFormat = format;
                    theCultureInfo = new CultureInfo("en-US");
                    return true;
                }
                else if (DateTimeOffset.TryParseExact(dateStr, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out time))
                {
                    dateTimeFormat = format;
                    theCultureInfo = CultureInfo.CurrentCulture;
                    return true;
                }
            }

            return false;
        }

        public Quote FromFirstString(string line)
        {
            DateTimeOffset time;

            String[] subStr = line.Split(Splitter);

            if (IndexPositions[0] == -1 || subStr[0] == "")
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }
            else if (!dateTimeParser(subStr[IndexPositions[0]], out time))
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

            return new Quote(time, values);
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

        public Quote FromString(string line)
        {
            DateTimeOffset time;

            String[] subStr = line.Split(Splitter);

            if (!dateTimeParser(subStr[IndexPositions[0]], out time))
            {
                throw new ArgumentException("Fail to get date from: " + line);
            }

            List<Double> values = getValues(subStr);

            return new Quote(time, values);
        }

        #endregion
    }

}
