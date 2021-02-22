using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;

namespace DataImporter
{
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CodeNameStructure
    {
        public UInt16 marketId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string code;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] unknown;
        public UInt32 order;

        public String Code { get { return code; } }

        public String Name { get { return name; } }

        public UInt32 Order { get { return order; } }

        public override string ToString()
        {
            return String.Format("{0} : {1}", Name, Code);
        }
    }

    /// <summary>
    /// Block structure of Pobo3.0 Day records.
    /// 日线数据为*.da1
    /// 40个字节为1个交易日的数据，每4个字节一个数据分别为
    /// 
    /// 日期
    ///     bit 0 -   5        分
    ///     bit 6 - 10         时
    ///     bit 11 - 15        日
    ///     bit 16 - 19        月份
    ///     bit 20 - 31        年
    ///     
    /// 收盘价 
    /// 开盘价
    /// 最高价
    /// 最低价    （bit0-31）/1000
    /// 
    /// 成交量
    /// 持仓量
    /// 结算价    同结算价
    /// 内盘
    /// 外盘
    /// 
    /// 成交量/持仓量/内盘/外盘用了一个算法，如下是试算过程，看得懂看，看不懂也自己琢磨
    /// bit 24 - 31    bit 16 - 23    (y = bit0～15)     
    /// 0x43        x < 0x80        128 + x        
    /// 0x7F < x <= 0xFF    2x
    /// 
    /// 例：0x435E0000 == 0x5E + 128 = 222
    /// 0x43940000 == 2 * 0x94 = 296
    ///    0x44        x < 0x80        512 + 4x + y / 16384        >> 14
    ///            0x7F < x <= 0xFF    8x + y / 8192            >> 13
    ///    例：0x44260000 == 512 + 4 * 0x26 = 512 + 152 = 664
    ///            0x44118000 == 512 + 4 * 0x11 + 0x8000 / 16384 = 512 + 68 + 2 = 582
    ///          0x44874000 == 8 * 0x87 + 0x4000 / 8192 = 1080 + 2=1082

    ///    0x45        x < 0x80        2048 + 16 * x + y / 4096        >> 12
    ///            0x7F < x <= 0xFF    32 * x + y / 2048         >> 11
    ///    例：0x4553C000 == 2048 + 16 * 0x53 + 0xC000 / 4096 = 2048 + 1328 + 12 = 3388
    ///          0x459FE000 == 32 * 0x9F + 0xE000 / 2048 =  5088 + 28 = 5116

    ///    0x46        x < 0x80        8192 + 64 * x + y / 1024        >> 10
    ///            0x7F < x <= 0xFF    128 * x + y / 512        >> 9
    ///    例：0x46067800 == 8192 + 64 * 0x06 + 0x7800 / 1024= 8192 + 384 + 30 = 8606
    ///                  0x46843400 == 128 * 0x84 + 0x3400 / 512 = 16896 + 26 = 16922

    ///    0x47        x < 0x80        32768 + 256 * x + y / 256    >> 8 
    ///            0x7F < x <= 0xFF    512 * x + y / 128        >> 7
    ///    例：0x4760BC00 == 32768 + 256 * 0x60 + 0xBC00 / 256 = 32768 + 24576 + 188 = 57532
    ///          0x47CA6300 == 512 * 0xCA + 0x6300 / 128 = 103424 + 198 = 103622

    ///    0x48        x < 0x80        131072 + 1024 * x + y / 64      >> 6
    ///            0x7F < x <= 0xFF    2048 * x + y / 32        >> 5
    ///    例：0x48127B80 == 131072 + 1024 * 0x12 + 0x7B80 / 64 = 131072 + 18432 + 494 = 149998
    ///          0x48984A40 == 2048 * 0x98 + 0x4A40 / 32 = 311296 + 594 = 311890

    ///    0x49        x < 0x80        524288 + 4096 * x + y / 16    >> 4
    ///            0x7F < x <= 0xFF    8192 * x + y / 8            >> 3
    ///    例：0x494EB380 == 524288 + 4096 * 0x4E + 0xB380 / 16 = 524288 + 319488 + 2872 = 846648
    ///          0x49A4F3F0 == 8192 * 0xA4 + 0xF3F0 / 8 = 1343488 + 7806 = 1351294
    ///归纳起来为：n = bit23~27;   2^(1 + n) + ((bit0~22) >> (22 - n))
    /// </summary>
    [Serializable]
    public struct PoboBarStructure
    {
        #region Static definition
        public static int Size = 40;

        private static BitVector32.Section minutesMask;             //The bits 0-5: 6 bits
        private static BitVector32.Section hoursMask;             //The bits 6-10: 5 bits
        private static BitVector32.Section dayMask;                 //The bits 11 - 15: 5bits
        private static BitVector32.Section monthMask;               //The bits 16 - 19: 4 bits
        private static BitVector32.Section yearMask;                //The bits 20 - 31 : 12 bits

        static PoboBarStructure()
        {
            minutesMask = BitVector32.CreateSection(0x3F);
            hoursMask = BitVector32.CreateSection(0x1F, minutesMask);
            dayMask = BitVector32.CreateSection(0x1F, hoursMask);
            monthMask = BitVector32.CreateSection(0xF, dayMask);
            yearMask = BitVector32.CreateSection(0xFFF, monthMask);

            Size = Marshal.SizeOf(typeof(PoboBarStructure));
        }
        #endregion

        private BitVector32 time;
        private UInt32 close;
        private UInt32 open;
        private UInt32 high;
        private UInt32 low;
        private UInt32 volume;
        private UInt32 position;
        private UInt32 settlement;
        private UInt32 sell;
        private UInt32 buy;

        public int Year { get { return time[yearMask]; } }
        public int Month { get { return time[monthMask]; } }
        public int Day { get { return time[dayMask]; } }
        public int Hour { get { return time[hoursMask]; } }
        public int Minute { get { return time[minutesMask]; } }

        public DateTimeOffset Time
        {
            get
            {
                DateTime theTime = new DateTime(time[yearMask], time[monthMask], time[dayMask], time[hoursMask], time[minutesMask], 0);
                return new DateTimeOffset(theTime);
            }
        }

        public Double Close { 
            get { return (Double)close / 1000; } 
            set { close = (UInt32)(value * 1000);}
        }

        public Double Open
        {
            get { return (Double)open / 1000; }
            set { open = (UInt32)(value * 1000); }
        }

        public Double High
        {
            get { return (Double)high / 1000; }
            set { high = (UInt32)(value * 1000); }
        }

        public Double Low
        {
            get { return (Double)low / 1000; }
            set { low = (UInt32)(value * 1000); }
        }

        public Double Settlement
        {
            get { return (Double)settlement / 1000; }
            set { settlement = (UInt32)(value * 1000); }
        }

        public Double Sell
        {
            get { return (Double)sell / 1000; }
            set { sell = (UInt32)(value * 1000); }
        }

        public Double Buy
        {
            get { return (Double)buy / 1000; }
            set { buy = (UInt32)(value * 1000); }
        }

        public Int32 Volume
        {
            get { return PoboDataImporter.ToVolume(volume); }
            set { volume = (UInt32)(value * 1000); }
        }

        public Int32 Position
        {
            get { return PoboDataImporter.ToVolume(position); }
            set { position = (UInt32)(value * 1000); }
        }

        public static Quote QuoteOf(PoboBarStructure record, RecordType type)
        {
            return new Quote(type, record.Time, record.Open, record.High, record.Low, record.Close, record.Volume);
        }

        public static List<Quote> QuotesOf(PoboBarStructure[] records, RecordType type)
        {
            List<Quote> result = new List<Quote>();

            foreach (PoboBarStructure record in records)
            {
                result.Add(QuoteOf(record, type));
            }
            return result;
        }
    }

    [Serializable]
    public struct PoboTickStructure
    {
        #region Static definition
        public static int Size = 20;

        private static BitVector32.Section minutesMask;             //The bits 0-5: 6 bits
        private static BitVector32.Section hoursMask;             //The bits 6-10: 5 bits
        private static BitVector32.Section dayMask;                 //The bits 11 - 15: 5bits
        private static BitVector32.Section monthMask;               //The bits 16 - 19: 4 bits
        private static BitVector32.Section yearMask;                //The bits 20 - 31 : 12 bits

        static PoboTickStructure()
        {
            minutesMask = BitVector32.CreateSection(0x3F);
            hoursMask = BitVector32.CreateSection(0x1F, minutesMask);
            dayMask = BitVector32.CreateSection(0x1F, hoursMask);
            monthMask = BitVector32.CreateSection(0xF, dayMask);
            yearMask = BitVector32.CreateSection(0xFFF, monthMask);

            //Size = Marshal.SizeOf(typeof(PoboTickStructure));
        }
        #endregion

        private BitVector32 time;
        private UInt32 price;
        private UInt32 volume;
        private UInt32 settlement;
        private UInt32 direction;

        public int Year { get { return time[yearMask]; } }
        public int Month { get { return time[monthMask]; } }
        public int Day { get { return time[dayMask]; } }
        public int Hour { get { return time[hoursMask]; } }
        public int Minute { get { return time[minutesMask]; } }

        public DateTimeOffset Time
        {
            get
            {
                DateTime theTime = new DateTime(time[yearMask], time[monthMask], time[dayMask], time[hoursMask], time[minutesMask], 0);
                return new DateTimeOffset(theTime);
            }
        }

        public Double Price { 
            get { return (Double)price / 1000; } 
            set { price = (UInt32)(value * 1000);}
        }

        public Int32 Volume
        {
            get { return PoboDataImporter.ToVolume(volume); }
            set { volume = (UInt32)(value * 1000); }
        }

        public Int32 Settlement
        {
            get { return PoboDataImporter.ToVolume(settlement); }
            set { settlement = (UInt32)(value * 1000); }
        }

        public bool IsBuy
        {
            get { return (direction >> 24) != 0; }
            set { direction = value ? (uint)0x0040C801 : (uint)0x0040C800; }
        }

        public static List<Quote> DayBarOf(PoboTickStructure[] records)
        {
            List<Quote> result = new List<Quote>();

            //double open, low, high, close, volume, settlement;
            DateTime date;
            Quote lastDayQuote = null;

            foreach (PoboTickStructure record in records)
            {
                if (lastDayQuote == null || record.Time.Date != lastDayQuote.Time.Date)
                {
                    if (lastDayQuote != null)
                    {
                        lastDayQuote.Close = record.Price;
                        lastDayQuote.Low = Math.Min(record.Price, lastDayQuote.Low);
                        lastDayQuote.High = Math.Max(record.Price, lastDayQuote.High);
                        lastDayQuote.Volume = record.Volume;
                        result.Add(lastDayQuote);
                    }

                    date = record.Time.Date;
                    lastDayQuote = new Quote(new DateTimeOffset(date), record.Price); 
                }
                else
                {
                    lastDayQuote.Close = record.Price;
                    lastDayQuote.Low = Math.Min(record.Price, lastDayQuote.Low);
                    lastDayQuote.High = Math.Max(record.Price, lastDayQuote.High);
                    lastDayQuote.Volume = record.Volume;
                }
            }

            if (lastDayQuote != null)
            {
                result.Add(lastDayQuote);
            }

            return result;
        }

        //public static Quote QuoteOf(PoboBarStructure record)
        //{
        //    return new Quote(record.Time, record.Open, record.High, record.Low, record.Close, record.Volume);
        //}

        //public static List<Quote> QuotesOf(PoboTickStructure[] records, RecordType type)
        //{
        //    if (type > RecordType.DayRecord)
        //        return null;

        //    List<Quote> result = new List<Quote>();

        //    foreach (PoboTickStructure record in records)
        //    {
        //        result.Add(QuoteOf(record));
        //    }
        //    return result;
        //}
    }

    [Serializable]
    public struct PoboTr2Structure
    {
        #region Static definition
        public static int Size = 40;

        #endregion

        private UInt32 close;
        private UInt32 volume;
        private UInt32 position;
        private UInt32 unknown1;
        private UInt32 unknown2;
        private UInt32 open;
        private UInt32 high;
        private UInt32 low;
        private UInt32 sellVolume;
        private UInt32 buyVolume;

        public Double Close { 
            get { return (Double)close / 1000; } 
            set { close = (UInt32)(value * 1000);}
        }

        public Int32 Volume
        {
            get { return PoboDataImporter.ToVolume(volume); }
            set { volume = (UInt32)(value * 1000); }
        }

        public Int32 Position
        {
            get { return PoboDataImporter.ToVolume(position); }
            set { position = (UInt32)(value * 1000); }
        }
        
        public UInt32 Unknown1
        {
            get { return unknown1; }
            set { unknown1 = value; }
        }

        public UInt32 Unknown2
        {
            get { return unknown2; }
            set { unknown2 = value; }
        }

        public Double Open
        {
            get { return (Double)open / 1000; }
            set { open = (UInt32)(value * 1000); }
        }

        public Double High
        {
            get { return (Double)high / 1000; }
            set { high = (UInt32)(value * 1000); }
        }

        public Double Low
        {
            get { return (Double)low / 1000; }
            set { low = (UInt32)(value * 1000); }
        }

        public Double SellVolume
        {
            get { return PoboDataImporter.ToVolume(sellVolume); }
            set { sellVolume = (UInt32)(value * 1000); }
        }

        public Double BuyVolume
        {
            get { return PoboDataImporter.ToVolume(buyVolume); }
            set { buyVolume = (UInt32)(value * 1000); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("O={0}, H={1}, L={2}, C={3}, P={4}", Open, High, Low, Close, Position);
            //sb.AppendFormat("P={0}, V={1}, S={2}, B={3}, ", Position, Volume, SellVolume, BuyVolume);
            //sb.AppendFormat("C={0}, O={1}, H={2}, L={3}\r\n", Close, Open, High, Low);
            //sb.AppendFormat("U1={0:x}({1}), U2={2:x}({3})----\r\n", Unknown1, Unknown1, Unknown2, Unknown2);
            //sb.AppendFormat("P={0}, U1={1:x}, U2={2:x}, {3}", Position, Unknown1, Unknown2, Convert.ToString(Unknown2, 2));
            return sb.ToString();
        }

    }

    [Serializable]
    public struct PoboTr3Structure
    {
        #region Static definition
        public static int Size = 48;

        #endregion

        private UInt32 close;
        private UInt32 volume;
        private UInt32 position;
        private UInt32 unknown1;
        private UInt32 unknown2;
        private UInt32 open;
        private UInt32 high;
        private UInt32 low;
        private UInt32 sellVolume;
        private UInt32 buyVolume;
        private UInt32 unknown3;
        private UInt32 unknown4;

        public Double Close { 
            get { return (Double)close / 1000; } 
            set { close = (UInt32)(value * 1000);}
        }

        public Int32 Volume
        {
            get { return PoboDataImporter.ToVolume(volume); }
            set { volume = (UInt32)(value * 1000); }
        }

        public Int32 Position
        {
            get { return PoboDataImporter.ToVolume(position); }
            set { position = (UInt32)(value * 1000); }
        }
        
        public UInt32 Unknown1
        {
            get { return unknown1; }
            set { unknown1 = value; }
        }

        public UInt32 Unknown2
        {
            get { return unknown2; }
            set { unknown2 = value; }
        }

        public UInt32 Unknown3
        {
            get { return unknown3; }
            set { unknown3 = value; }
        }

        public UInt32 Unknown4
        {
            get { return unknown4; }
            set { unknown4 = value; }
        }

        public Double Open
        {
            get { return (Double)open / 1000; }
            set { open = (UInt32)(value * 1000); }
        }

        public Double High
        {
            get { return (Double)high / 1000; }
            set { high = (UInt32)(value * 1000); }
        }

        public Double Low
        {
            get { return (Double)low / 1000; }
            set { low = (UInt32)(value * 1000); }
        }

        public Double SellVolume
        {
            get { return PoboDataImporter.ToVolume(sellVolume); }
            set { sellVolume = (UInt32)(value * 1000); }
        }

        public Double BuyVolume
        {
            get { return PoboDataImporter.ToVolume(buyVolume); }
            set { buyVolume = (UInt32)(value * 1000); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("O={0}, H={1}, L={2}, C={3}, P={4}", Open, High, Low, Close, Position);
            //sb.AppendFormat("P={0}, V={1}, S={2}, B={3}, ", Position, Volume, SellVolume, BuyVolume);
            //sb.AppendFormat("C={0}, O={1}, H={2}, L={3}\r\n", Close, Open, High, Low);
            //sb.AppendFormat("U1={0:x}({1}), U2={2:x}({3})----\r\n", Unknown1, Unknown1, Unknown2, Unknown2);
            //sb.AppendFormat("P={0}, U1={1:x}, U2={2:x}, {3}", Position, Unknown1, Unknown2, Convert.ToString(Unknown2, 2));
            return sb.ToString();
        }

    }

    public class PoboDayFileSummary
    {
        public String Name { get; private set; }

        public int ItemsCount { get; private set; }

        public DateTime Since { get; private set; }

        public DateTime Until { get; private set; }

        public int TotalDays { get; private set; }

        public bool IsAlive { get; private set; }

        public PoboDayFileSummary(string name, string fileName)
        {
            Name = name;

            FileInfo info = new FileInfo(fileName);

            int fileSize = (int)info.Length;

            if (fileSize % PoboBarStructure.Size != 0)
            {
                Console.WriteLine("Wrong file size of " + fileSize.ToString());
                return;
            }

            byte[] buffer1 = new byte[PoboBarStructure.Size];
            byte[] buffer2 = new byte[PoboBarStructure.Size];

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            buffer1 = br.ReadBytes(PoboBarStructure.Size);

            fs.Seek(-PoboBarStructure.Size, SeekOrigin.End);
            buffer2 = br.ReadBytes(PoboBarStructure.Size);

            br.Close();
            fs.Close();

            PoboBarStructure first = PoboDataImporter.BytesToStructures<PoboBarStructure>(buffer1)[0];
            PoboBarStructure last = PoboDataImporter.BytesToStructures<PoboBarStructure>(buffer2)[0];

            Since = first.Time.Date;
            Until = last.Time.Date;
            ItemsCount = fileSize / PoboBarStructure.Size;
            TotalDays = (int)(Until - Since).TotalDays + 1;
            IsAlive = true;
        }
    }

    /// <summary>
    /// Importer of Pobo3.0 Daily quotes.
    /// </summary>
    public static class PoboDataImporter
    {
        //public static string FilePostfix = ".da1";
        //public static string DayPath = @"Day\";

        public static Dictionary<RecordType, string> Folders = new Dictionary<RecordType, string>()
        {
            {RecordType.DayRecord, @"Day\"},
            {RecordType.HourRecord, @"Min60\"},
            {RecordType.FiveMinuteRecord, @"Min\"},
            {RecordType.MinuteRecord, @"Min1\"}
        };

        public static Dictionary<RecordType, string> Postfixes = new Dictionary<RecordType, string>()
        {
            {RecordType.DayRecord, @".da1"},
            {RecordType.HourRecord, @".hr1"},
            {RecordType.FiveMinuteRecord, @".mi1"},
            {RecordType.MinuteRecord, @".m11"}
        };

        public static string PoboDataDirectory = @"C:\Program Files\南华期货博易大师客户端\pobo3\Data";
        public static Dictionary<string, string> MarketsTable = new Dictionary<string, string>()
        {
            {"cnfut", "国内期货"},
            {"ccgfut", "CBOT粮油"},
            {"index", "全球指数"},
            {"jpfut", "日本期货"},
            {"ldfut", "伦敦期货"},
            {"mlfut", "棕榈油"},
            {"money", "美汇"},
            {"nybfut", "NYBOT商品"},
            {"nyefut", "NYME能源"},
            {"nymfut", "纽约金属"},
            {"sgpfut", "新加坡期货"},
            {"shanghai", "上交所"},
            {"shenzhen", "深交所"}
        };
        public static string NameTableFile = "NAMETBL.PB";

        public static Dictionary<String, String> AllGoodsOf(String market)
        {
            string marketDir = PoboDataDirectory + @"\" + market + @"\";
            string tableFilename = null;
            if (MarketsTable.ContainsKey(market))
            {
                tableFilename = marketDir + NameTableFile;
            }
            else if (MarketsTable.ContainsValue(market))
            {
                foreach (KeyValuePair<string, string> kvp in MarketsTable)
                {
                    if (kvp.Value != market)
                        continue;

                    market = kvp.Key;
                    break;
                }

                tableFilename = marketDir + NameTableFile;
            }
            else
            {
                Console.WriteLine("Undefined market folder of " + market);
                return null;
            }

            FileInfo info = new FileInfo(tableFilename);

            long fileSize = info.Length;

            if (info.Length % 0x20 != 0)
            {
                Console.WriteLine("Wrong file size of " + fileSize.ToString());
                return null;
            }

            byte[] buffer = new byte[(int)fileSize];

            FileStream fs = new FileStream(tableFilename, FileMode.Open, FileAccess.Read);
            int readed = fs.Read(buffer, 0, (int)fileSize);
            fs.Close();

            if (readed != info.Length)
                throw new IOException("Failed to read all data!");

            CodeNameStructure[] tables = BytesToStructures<CodeNameStructure>(buffer);

            Dictionary<string, string> result = new Dictionary<string, string>();
            marketDir += Folders[RecordType.DayRecord];
            String fileName = null;

            foreach (CodeNameStructure table in tables)
            {
                fileName = marketDir + table.Code + Postfixes[RecordType.DayRecord];
                if (File.Exists(fileName))
                    result.Add(table.Name, fileName);
                else
                    Console.WriteLine(fileName + " is not found.");
            }

            return result;
        }

        public static string FullNameOf(string dayFileName, RecordType type)
        {
            if (type == RecordType.DayRecord)
                return dayFileName;

            if (!Folders.ContainsKey(type))
                return null;

            string temp = dayFileName.Replace(Postfixes[RecordType.DayRecord], Postfixes[type]);
            return temp.Replace(Folders[RecordType.DayRecord], Folders[type]);
        }

        //public static List<Quote> Import(string fileName)
        //{
        //    return Import(fileName, 0);
        //}

        //public static List<Quote> Import(string fileName, int offset)
        //{
        //    FileStream fs = null;

        //    try
        //    {
        //        FileInfo info = new FileInfo(fileName);

        //        int fileSize = (int)(info.Length);

        //        if (fileSize % PoboBarStructure.Size != 0)
        //        {
        //            Console.WriteLine("File size = " + fileSize.ToString() + ", check to see if the file is corrupted!");
        //            return null;
        //        }
        //        else if (offset >= fileSize / PoboBarStructure.Size - 1 || offset < 0)
        //            throw new ArgumentOutOfRangeException("Offset is out of range!");

        //        byte[] buffer = new byte[fileSize - offset * PoboBarStructure.Size];

        //        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        int start = offset * PoboBarStructure.Size;
        //        int count = fileSize - offset * PoboBarStructure.Size;
        //        int readed = fs.Read(buffer, start, count);
        //        fs.Close();

        //        if (readed != fileSize - offset * PoboBarStructure.Size)
        //            throw new IOException("Failed to read all data!");

        //        PoboBarStructure[] rawRecords = BytesToStructures<PoboBarStructure>(buffer);

        //        return PoboBarStructure.QuotesOf(rawRecords);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //        return null;
        //    }
        //}

        public static List<Quote> Import(string fileName, RecordType type)
        {
            FileStream fs = null;

            try
            {
                FileInfo info = new FileInfo(fileName);

                int fileSize = (int)(info.Length);

                if (fileSize % PoboBarStructure.Size != 0)
                {
                    Console.WriteLine("File size = " + fileSize.ToString() + ", check to see if the file is corrupted!");
                    return null;
                }
                byte[] buffer = new byte[fileSize];

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                int readed = fs.Read(buffer, 0, fileSize);
                fs.Close();

                if (readed != fileSize)
                    throw new IOException("Failed to read all data!");

                PoboBarStructure[] rawRecords = BytesToStructures<PoboBarStructure>(buffer);

                return PoboBarStructure.QuotesOf(rawRecords, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public static Quote LastDayQuote(string dayFileName)
        {
            string tr3FileName = dayFileName.Replace("da1", "tr3").Replace("Day", "Tick");

            if (!File.Exists(tr3FileName))
                return null;

            FileInfo fileInfo = new FileInfo(tr3FileName);

            DateTime date = fileInfo.LastAccessTime.Date;

            if ((fileInfo.Length - 16) % PoboTr3Structure.Size != 0)
            {
                Console.WriteLine("File length is not expected: " + fileInfo.Length.ToString());
                return null;
            }

            double open = -1, low = -1, high = -1, close = -1;

            using (FileStream fs = new FileStream(tr3FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length - 16];

                fs.Read(buffer, 0, 16);

                int readed = fs.Read(buffer, 0, buffer.Length);

                fs.Close();

                if (readed != buffer.Length)
                {
                    throw new IOException("Failed to read all data!");
                }

                PoboTr3Structure[] rawRecords = PoboDataImporter.BytesToStructures<PoboTr3Structure>(buffer);

                //int next = 0;
                //if (rawRecords[0].Open != 0)
                //{
                //    open = rawRecords[0].Open;
                //    low = rawRecords[0].Low != 0 ? rawRecords[0].Low : open;
                //    high = rawRecords[0].High != 0 ? rawRecords[0].High : open;
                //    close = rawRecords[0].Close;
                //    next = 1;
                //}
                //else
                //{
                //    open = rawRecords[1].Open;
                //    low = rawRecords[1].Low != 0 ? rawRecords[1].Low : open;
                //    high = rawRecords[1].High != 0 ? rawRecords[1].High : open;
                //    close = rawRecords[1].Close != 0 ? rawRecords[1].Close : open;
                //    next = 2;
                //}

                for (int i = 0; i < rawRecords.Length; i++)
                {
                    if (rawRecords[i].Low == 0 || (rawRecords[i].Open == 0))
                        continue;

                    if (open == -1 && rawRecords[i].Open != 0)
                        open = rawRecords[i].Open;

                    if (low == -1)
                        low = rawRecords[i].Low;
                    else
                        low = Math.Min(low, rawRecords[i].Low);

                    if (rawRecords[i].High != 0)
                    {
                        high = Math.Max(high, rawRecords[i].High);
                    }

                    if (rawRecords[i].Close != 0)
                        close = rawRecords[i].Close;
                }

                //PoboTr2Structure last = rawRecords[rawRecords.Length - 1];
                //if (last.Close != 0)
                //{
                //    close = last.Close;
                //}

                return new Quote(date, open, high, low, close);
            }
        }

        //public static Quote LastDayQuote(string dayFileName)
        //{
        //    string tr3FileName = dayFileName.Replace("da1", "tr2").Replace("Day", "Tick");

        //    if (!File.Exists(tr3FileName))
        //        return null;

        //    FileInfo fileInfo = new FileInfo(tr3FileName);

        //    DateTime date = fileInfo.LastAccessTime.Date;

        //    if ((fileInfo.Length - 16) % PoboTr2Structure.Size != 0)
        //    {
        //        Console.WriteLine("File length is not expected: " + fileInfo.Length.ToString());
        //        return null;
        //    }

        //    double open = -1, low = -1, high = -1, close = -1;

        //    using (FileStream fs = new FileStream(tr3FileName, FileMode.Open, FileAccess.Read))
        //    {
        //        byte[] buffer = new byte[fs.Length - 16];

        //        fs.Read(buffer, 0, 16);

        //        int readed = fs.Read(buffer, 0, buffer.Length);

        //        fs.Close();

        //        if (readed != buffer.Length)
        //        {
        //            throw new IOException("Failed to read all data!");
        //        }

        //        PoboTr2Structure[] rawRecords = PoboDataImporter.BytesToStructures<PoboTr2Structure>(buffer);

        //        //int next = 0;
        //        //if (rawRecords[0].Open != 0)
        //        //{
        //        //    open = rawRecords[0].Open;
        //        //    low = rawRecords[0].Low != 0 ? rawRecords[0].Low : open;
        //        //    high = rawRecords[0].High != 0 ? rawRecords[0].High : open;
        //        //    close = rawRecords[0].Close;
        //        //    next = 1;
        //        //}
        //        //else
        //        //{
        //        //    open = rawRecords[1].Open;
        //        //    low = rawRecords[1].Low != 0 ? rawRecords[1].Low : open;
        //        //    high = rawRecords[1].High != 0 ? rawRecords[1].High : open;
        //        //    close = rawRecords[1].Close != 0 ? rawRecords[1].Close : open;
        //        //    next = 2;
        //        //}

        //        for (int i = 0; i < rawRecords.Length; i++)
        //        {
        //            if (rawRecords[i].Low == 0 || (rawRecords[i].Open == 0))
        //                continue;

        //            if (open == -1 && rawRecords[i].Open != 0)
        //                open = rawRecords[i].Open;

        //            if (low == -1)
        //                low = rawRecords[i].Low;
        //            else
        //                low = Math.Min(low, rawRecords[i].Low);

        //            if (rawRecords[i].High != 0)
        //            {
        //                high = Math.Max(high, rawRecords[i].High);
        //            }

        //            if (rawRecords[i].Close != 0)
        //                close = rawRecords[i].Close;
        //        }

        //        //PoboTr2Structure last = rawRecords[rawRecords.Length - 1];
        //        //if (last.Close != 0)
        //        //{
        //        //    close = last.Close;
        //        //}

        //        return new Quote(date, open, high, low, close);
        //    }
        //}

        public static List<Quote> MinQuotesFromTr2(string tr2FileName, Dictionary<int, TimeSpan> tr2TimeIndex)
        {
            if (!tr2FileName.EndsWith("tr2") || !File.Exists(tr2FileName))
                return null;

            FileInfo fileInfo = new FileInfo(tr2FileName);

            DateTime date = fileInfo.LastWriteTime.Date;

            if ((fileInfo.Length - 16) % PoboTr2Structure.Size != 0)
            {
                Console.WriteLine("File length is not expected: " + fileInfo.Length.ToString());
                return null;
            }
            else if ((fileInfo.Length - 16) / PoboTr2Structure.Size > tr2TimeIndex.Count+1)
            {
                Console.WriteLine("The time index contains less items than the actual tr2 file.");
                return null;
            }

            List<Quote> result = new List<Quote>();

            using (FileStream fs = new FileStream(tr2FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length - 16];

                fs.Read(buffer, 0, 16);

                int readed = fs.Read(buffer, 0, buffer.Length);

                fs.Close();

                if (readed != buffer.Length)
                {
                    throw new IOException("Failed to read all data!");
                }

                PoboTr2Structure[] rawRecords = PoboDataImporter.BytesToStructures<PoboTr2Structure>(buffer);

                for (int i = 1; i < rawRecords.Length; i++)
                {
                    if (rawRecords[i].High == 0 || rawRecords[i].Low == 0)
                        continue;

                    Quote minQuote = new Quote(RecordType.MinuteRecord, date + tr2TimeIndex[i-1], rawRecords[i].Open, rawRecords[i].High,
                        rawRecords[i].Low, rawRecords[i].Close);
                    result.Add(minQuote);
                }

                return result;
            }
        }

        public static List<Quote> MinQuotesFromTr3(string tr2FileName, Dictionary<int, TimeSpan> tr2TimeIndex)
        {
            if (!tr2FileName.EndsWith("tr3") || !File.Exists(tr2FileName))
                return null;

            FileInfo fileInfo = new FileInfo(tr2FileName);

            DateTime date = fileInfo.LastWriteTime.Date;

            if ((fileInfo.Length - 16) % PoboTr3Structure.Size != 0)
            {
                Console.WriteLine("File length is not expected: " + fileInfo.Length.ToString());
                return null;
            }
            else if ((fileInfo.Length - 16) / PoboTr3Structure.Size > tr2TimeIndex.Count + 1)
            {
                Console.WriteLine("The time index contains less items than the actual tr2 file.");
                return null;
            }

            List<Quote> result = new List<Quote>();

            using (FileStream fs = new FileStream(tr2FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length - 16];

                fs.Read(buffer, 0, 16);

                int readed = fs.Read(buffer, 0, buffer.Length);

                fs.Close();

                if (readed != buffer.Length)
                {
                    throw new IOException("Failed to read all data!");
                }

                PoboTr3Structure[] rawRecords = PoboDataImporter.BytesToStructures<PoboTr3Structure>(buffer);

                for (int i = 1; i < rawRecords.Length; i++)
                {
                    if (rawRecords[i].High == 0 || rawRecords[i].Low == 0)
                        continue;

                    Quote minQuote = new Quote(RecordType.MinuteRecord, date + tr2TimeIndex[i - 1], rawRecords[i].Open, rawRecords[i].High,
                        rawRecords[i].Low, rawRecords[i].Close);
                    result.Add(minQuote);
                }

                return result;
            }
        }

        //convert byte array to structure
        public static T[] BytesToStructures<T>(byte[] buffer)
        {
            //get size of sturcture
            int size = Marshal.SizeOf(typeof(T));
            //can not be convert
            if (buffer.Length < size || buffer.Length % size != 0)
            {
                Console.WriteLine("Wrong byte[] size = " + buffer.Length.ToString());
                return null;
            }

            T[] result = new T[buffer.Length / size];

            GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Int32 addr = pinned.AddrOfPinnedObject().ToInt32();

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (T)Marshal.PtrToStructure(new IntPtr(i * size + addr), typeof(T));
            }

            pinned.Free();

            return result;
        }

        public static Int32 ToVolume(UInt32 raw)
        {
            UInt32 flag = raw >> 24;
            if (flag < 0x42 || flag > 0x49)
                return 0;

            Int32 n = (int)((raw >> 23) & 0x1F);
            Int32 remain = (int)(raw & 0x7FFFFF);

            int offset = 2 << n;
            int shifted = remain >> (22 - n);

            return offset + shifted;
        }


    }

}
