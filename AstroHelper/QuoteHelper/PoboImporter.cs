using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace QuoteHelper
{
    /// <summary>
    /// Importer of Pobo3.0 Daily quotes.
    /// </summary>
    public static class PoboImporter
    {
        public static string FilePostfix = ".da1";
        public static string DayPath = @"Day\";
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
                foreach(KeyValuePair<string, string> kvp in MarketsTable)
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
            marketDir += DayPath;
            String fileName = null;

            foreach (CodeNameStructure table in tables)
            {
                fileName = marketDir + table.Code + FilePostfix;
                if (File.Exists(fileName))
                    result.Add(table.Name, fileName);
                else
                    Console.WriteLine(fileName + " is not found.");
            }

            return result;
        }

        public static List<Quote> Import(string fileName)
        {
            if (!fileName.EndsWith(FilePostfix))
            {
                Console.WriteLine("Unexpected file type!");
                return null;
            }

            FileStream fs = null;

            try
            {
                FileInfo info = new FileInfo(fileName);

                long fileSize = info.Length;

                if (fileSize % PoboDayStructure.Size != 0)
                {
                    Console.WriteLine("File size = " + info.Length.ToString() + ", check to see if the file is corrupted!");
                    return null;
                }

                byte[] buffer = new byte[info.Length];
                
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                int readed = fs.Read(buffer, 0, (int)fileSize);
                fs.Close();

                if (readed != info.Length)
                    throw new IOException("Failed to read all data!");

                PoboDayStructure[] rawRecords = BytesToStructures<PoboDayStructure>(buffer);

                ////Mapping the byte[] to PoboDayStructure[]
                //PoboDayStructure[] rawRecords = new PoboDayStructure[readed/PoboDayStructure.Size];

                //GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                //Int32 addr = pinned.AddrOfPinnedObject().ToInt32();

                //for (int i = 0; i < rawRecords.Length; i++)
                //{
                //    rawRecords[i] = (PoboDayStructure)Marshal.PtrToStructure(new IntPtr(i * PoboDayStructure.Size + addr), typeof(PoboDayStructure));
                //}

                //pinned.Free();

                return PoboDayStructure.QuotesOf(rawRecords);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
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

    }

    [StructLayout(LayoutKind.Sequential, Size=32)]
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
}
