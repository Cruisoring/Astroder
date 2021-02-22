using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace QuoteHelper
{
    /// <summary>
    /// Block structure of Pobo3.0 Day records.
    /// 日线数据为*.da1
    /// 40个字节为1个交易日的数据，每4个字节一个数据分别为
    /// 
    /// 日期
    ///     bit 0 -   5        分
    ///     bit 6 - 10        秒
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
    public struct PoboDayStructure
    {
        #region Static definition
        public static int Size = 40;

        private static BitVector32.Section minutesMask;             //The bits 0-5: 6 bits
        private static BitVector32.Section hoursMask;             //The bits 6-10: 5 bits
        private static BitVector32.Section dayMask;                 //The bits 11 - 15: 5bits
        private static BitVector32.Section monthMask;               //The bits 16 - 19: 4 bits
        private static BitVector32.Section yearMask;                //The bits 20 - 31 : 12 bits

        static PoboDayStructure()
        {
            minutesMask = BitVector32.CreateSection(0x3F);
            hoursMask = BitVector32.CreateSection(0x1F, minutesMask);
            dayMask = BitVector32.CreateSection(0x1F, hoursMask);
            monthMask = BitVector32.CreateSection(0xF, dayMask);
            yearMask = BitVector32.CreateSection(0xFFF, monthMask);

            Size = Marshal.SizeOf(typeof(PoboDayStructure));
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
                //DateTime time = new DateTime(Year, Month, Day, Hour, Minute, 0, DateTimeKind.Local);
                //return new DateTimeOffset(time);
                return new DateTimeOffset(time[yearMask], time[monthMask], time[dayMask], time[hoursMask], time[minutesMask], 0, TimeSpan.Zero);
            }
        }

        public Double Close { get { return (Double)close / 1000; } }

        public Double Open { get { return (Double)open / 1000; } }

        public Double High { get { return (Double)high / 1000; } }

        public Double Low { get { return (Double)low / 1000; } }

        public Double Settlement { get { return (Double)settlement / 1000; } }

        public Double Sell { get { return (Double)sell / 1000; } }

        public Double Buy { get { return (Double)buy / 1000; } }

        public UInt32 Volume { get { return ToVolume(volume); } }

        public UInt32 Position { get { return ToVolume(position); } }

        public static UInt32 ToVolume(UInt32 raw)
        {
            UInt32 flag = raw >> 24;
            UInt32 x = (raw >> 16) & 0xFF;
            UInt32 y = raw & 0xFFFF;

            switch(flag)
            {
                default:
                    return 0;
                case 0x43:
                    return x < 0x80 ? 128+x : 2 * x;
                case 0x44:
                    return x < 0x80 ? (512+4*x+y>>14) : (8*x + y >>13);
                case 0x45:
                    return x < 0x80 ? (2048 + 16 * x + y >> 12) : (32 * x + y >> 11);
                case 0x46:
                    return x < 0x80 ? (8192 + 64 * x + y >> 10) : (128 * x + y >> 9);
                case 0x47:
                    return x < 0x80 ? (32768 + 256 * x + y >> 8) : (512 * x + y >> 7);
                case 0x48:
                    return x < 0x80 ? (131072 + 1024 * x + y >> 6) : (2048 * x + y >> 5);
                case 0x49:
                    return x < 0x80 ? (524288 + 4096 * x + y >> 4) : (192 * x + y >> 3);
            }
        }

        public static Quote QuoteOf(PoboDayStructure record)
        {
            return new Quote(record.Time, record.Open, record.High, record.Low, record.Close, record.Volume);
        }

        public static List<Quote> QuotesOf(PoboDayStructure[] records)
        {
            List<Quote> result = new List<Quote>();

            foreach (PoboDayStructure record in records)
            {
                result.Add(QuoteOf(record));
            }
            return result;
        }
    }

}
