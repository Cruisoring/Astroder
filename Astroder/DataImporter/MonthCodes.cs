using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DataImporter
{
    public class MonthCodes : IComparable<MonthCodes>
    {
        #region Static Definitions
        public static MonthCodes Continuous = new MonthCodes(0, 'C');
        public static MonthCodes January = new MonthCodes(1, 'F');
        public static MonthCodes February = new MonthCodes(2, 'G');
        public static MonthCodes March = new MonthCodes(3, 'H');
        public static MonthCodes April = new MonthCodes(4, 'J');
        public static MonthCodes May = new MonthCodes(5, 'K');
        public static MonthCodes June = new MonthCodes(6, 'M');
        public static MonthCodes July = new MonthCodes(7, 'N');
        public static MonthCodes August = new MonthCodes(8, 'Q');
        public static MonthCodes September = new MonthCodes(9, 'U');
        public static MonthCodes October = new MonthCodes(10, 'V');
        public static MonthCodes November = new MonthCodes(11, 'X');
        public static MonthCodes December = new MonthCodes(12, 'Z');

        public static Dictionary<int, MonthCodes> MonthNumDict = null;

        public static Dictionary<char, MonthCodes> MonthCodeDict = null;
        //new Dictionary<char, MonthCodes>()
        //{
        //    { 'C', Continuous },
        //    { 'F', January},
        //    { 'G', February},
        //    { 'H', March},
        //    { 'J', April},
        //    { 'K', May},
        //    { 'M', June},
        //    { 'N', July},
        //    { 'Q', August},
        //    { 'U', September},
        //    { 'V', October},
        //    { 'X', November},
        //    { 'Z', December}
        //};

        static MonthCodes()
        {
            MonthNumDict = new Dictionary<int, MonthCodes>() 
                                {
                                    { 0, Continuous },
                                    { 1, January},
                                    { 2, February},
                                    { 3, March},
                                    { 4, April},
                                    { 5, May},
                                    { 6, June},
                                    { 7, July},
                                    { 8, August},
                                    { 9, September},
                                    { 10, October},
                                    { 11, November},
                                    { 12, December}
                                };
            MonthCodeDict = new Dictionary<char, MonthCodes>();

            foreach (KeyValuePair<int, MonthCodes> kvp in MonthNumDict)
            {
                MonthCodeDict.Add(kvp.Value.Symbol, kvp.Value);
            }
        }

        public static MonthCodes MonthCodeOf(int month)
        {
            if (MonthNumDict.ContainsKey(month))
                return MonthNumDict[month];
            else
                return null;
                //throw new Exception();
        }

        public static MonthCodes MonthCodeOf(char code)
        {
            if (MonthCodeDict.ContainsKey(code))
                return MonthCodeDict[code];
            else
                return null;
                //throw new Exception();
        }

        public static int MonthOf(char code)
        {
            if (MonthCodeDict.ContainsKey(code))
                return MonthCodeDict[code].Month;
            else
                return -1;
                //throw new Exception();
        }
        #endregion

        #region Property and Constructor
        public int Month { get; private set; }

        public char Symbol { get; private set; }

        public string Name { get; private set; }

        private MonthCodes(int month, char symbol)
        {
            Month = month;
            Symbol = symbol;
            Name = (month == 0) ? "Continuous" :
                System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region IComparable<MonthCodes> 成员

        public int CompareTo(MonthCodes other)
        {
            return this.Month.CompareTo(other.Month);
        }

        #endregion
    }

    public class ExchangeInfo
    {
        public static ExchangeInfo CBOT = new ExchangeInfo("CBOT", "Chicago Board of Trade");
        public static ExchangeInfo CME = new ExchangeInfo("CME", "Chicago Mercantile Exchange");
        public static ExchangeInfo CMX = new ExchangeInfo("CMX", "COMEX Division - New York Mercantile Exchange Division of CME Group");
        public static ExchangeInfo NYBOT = new ExchangeInfo("NYBOT", "New York Board of Trade");
        public static ExchangeInfo KCBT = new ExchangeInfo("KCBT", "Kansas City Board of Trade");
        public static ExchangeInfo MGE = new ExchangeInfo("MGE", "Minneapolis Grain Exchange");
        public static ExchangeInfo MATIF = new ExchangeInfo("MATIF", "ParisBourse SA");
        public static ExchangeInfo SFE = new ExchangeInfo("SFE", "Sydney Futures Exchange");
        public static ExchangeInfo NYM = new ExchangeInfo("NYM", "NYMEX Division - New York Mercantile Exchange Division of CME Group");
        public static ExchangeInfo LIFFE = new ExchangeInfo("LIFFE", @"London Int'l Financial Futures Exchange");
        public static ExchangeInfo EUREX = new ExchangeInfo("EUREX", "Eurex");
        public static ExchangeInfo ICE = new ExchangeInfo("ICE", "Int'l Commodity Exchange");

        public static Dictionary<string, ExchangeInfo> Exchanges = new Dictionary<string, ExchangeInfo>()
        {
            { "CBOT", CBOT},
            { "CME", CME},
            { "CMX", CMX},
            { "NYBOT", NYBOT},
            { "KCBT", KCBT},
            { "MGE", MGE},
            { "MATIF", MATIF},
            { "SEF", SFE},
            { "NYM", NYM},
            { "LIFFE", LIFFE},
            { "EUREX", EUREX},
            { "ICE", ICE}
        };

        public static ExchangeInfo ExchangeOf(string abbr)
        {
            if (Exchanges.ContainsKey(abbr))
                return Exchanges[abbr];
            else
                return null;
                //throw new Exception();
        }

        public string Abbrievation { get; private set; }

        public string Description { get; private set; }

        private ExchangeInfo(string abbr, string name)
        {
            Abbrievation = abbr;
            Description = name;
        }
    }

    public class CommodityInfomation
    {
        #region Property and Constructor
        public string Symbol { get; private set; }

        public string Description { get; private set; }

        public ExchangeInfo Exchange { get; private set; }

        public List<MonthCodes> Months { get; private set; }

        public string MonthCodeString { get; private set; }

        public double MinTick { get; private set; }

        public double UnitMove { get; private set; }

        private CommodityInfomation(string symbol, string contract, string exchange, string monthCodes, double minTick, double unitMove)
        {
            Symbol = symbol;
            Description = contract;
            Exchange = ExchangeInfo.ExchangeOf(exchange);
            MonthCodeString = monthCodes.Replace(",", "");

            char ch = MonthCodes.January.Symbol;
            Months = new List<MonthCodes>();
            foreach (char code in MonthCodeString)
            {
                Months.Add(MonthCodes.MonthCodeOf(code));
            }


            MinTick = minTick;
            UnitMove = unitMove;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (MonthCodes code in Months)
            {
                sb.AppendLine(code.Symbol + ": " + code.Name);
            }

            return string.Format("{0}(Symbol: {1}) of {2}, MinTick={3}, UnitMove={4}, Contracts:\r\n{5}", Description, Symbol, Exchange.Abbrievation, MinTick, UnitMove, sb);
        }

        public bool HasContractOn(int month)
        {
            if (month < 1 || month > 12)
                return false;

            char symbol = MonthCodes.MonthCodeOf(month).Symbol;
            return MonthCodeString.Contains(symbol);
        }

        public MonthCodes NextOf(MonthCodes theCode)
        {
            int index = Months.IndexOf(theCode);

            if (index == -1)
                return null;

            index = (index++) % Months.Count;

            return Months[index];
        }

        public ContractInfomation ContractOf(string fileName)
        {
            int index = fileName.LastIndexOf('\\');

            fileName = fileName.Substring(index + 1);

            if (!fileName.StartsWith(Symbol))
                throw new Exception("Unexpected filename: " + fileName);

            if (fileName.EndsWith("da1"))
            {
                if (fileName.Contains('.'))
                    fileName = fileName.Substring(0, fileName.IndexOf('.'));

                char code = fileName[fileName.Length - 1];

                int month, year = DateTimeOffset.Now.Year;

                month = MonthCodes.MonthOf(code);

                return new ContractInfomation(this, year , month);
            }
            else
            {
                if (fileName.Contains('.'))
                    fileName = fileName.Substring(0, fileName.IndexOf('.'));

                char code = fileName[fileName.Length - 1];
                if (!MonthCodeString.Contains(code))
                {
                    //throw new Exception(string.Format("The commodity contains no contract for month of {0}({1})", code, MonthCodes.MonthCodeOf(code).Name));
                    MonthCodeString = string.Format("{0}{1}", code, MonthCodeString);
                    Months.Add(MonthCodes.MonthCodeOf(code));
                    Months.Sort();
                }

                int month, year;

                month = MonthCodes.MonthOf(code);

                string yearString = fileName.Substring(Symbol.Length, fileName.Length - Symbol.Length - 1);

                if (int.TryParse(yearString, out year))
                {
                    return new ContractInfomation(this, year > 30 ? 1900 + year : 2000 + year, month);
                }
                else
                    throw new Exception("Failed to parse year from " + yearString);
            }
        }


        #endregion
        #region Static definitions

        //Currencies
        public static CommodityInfomation AD = new CommodityInfomation("AD", "Australian Dollar", "CME", "HMUZ", 0.01, 1000);
        public static CommodityInfomation BP = new CommodityInfomation("BP", "British Pound", "CME", "HMUZ", 0.01, 625);
        public static CommodityInfomation CD = new CommodityInfomation("CD", "Canadian Dollar", "CME", "HMUZ", 0.01, 1000);
        public static CommodityInfomation DX = new CommodityInfomation("DX", "US Dollar Index", "ICE", "HMUZ", 0.01, 1000);
        public static CommodityInfomation EU = new CommodityInfomation("EU", "EuroFx", "CME", "HMUZ", 0.01, 1250);
        public static CommodityInfomation JY = new CommodityInfomation("JY", "Japanese Yen", "CME", "HMUZ", 0.01, 1250);
        public static CommodityInfomation SF = new CommodityInfomation("SF", "Swiss Franc", "CME", "HMUZ", 0.01, 1250);

        //Energies
        public static CommodityInfomation CL = new CommodityInfomation("CL", "Crude Oil", "NYM", "FGHJKMNQUVXZ", 0.01, 1000);
        public static CommodityInfomation HO = new CommodityInfomation("HO", "Heating Oil", "NYM", "FGHJKMNQUVXZ", 0.01, 420);
        public static CommodityInfomation HU = new CommodityInfomation("HU", "Unleaded Gas", "NYM", "FGHJKMNQUVXZ", 0.01, 420);
        public static CommodityInfomation NG = new CommodityInfomation("NG", "Natural Gas", "NYM", "FGHJKMNQUVXZ", 0.001, 10000);
        public static CommodityInfomation RB = new CommodityInfomation("RB", "RBOB Gas", "NYM", "FGHJKMNQUVXZ", 0.01, 420);

        //Grains & Soy Complex
        public static CommodityInfomation BO = new CommodityInfomation("BO", "Soybean Oil", "CBOT", "F,H,K,N,Q,U,V,Z", 0.01, 600);
        public static CommodityInfomation C = new CommodityInfomation("C", "Corn", "CBOT", "F,H,K,N,U,X,Z", 0.25, 50);
        public static CommodityInfomation KW = new CommodityInfomation("KW", "Kansas City Wheat", "CBOT", "H,K,N,U,Z", 0.25, 50);
        public static CommodityInfomation MW = new CommodityInfomation("MW", "Minneapolis Wheat", "CBOT", "H,K,N,U,Z", 0.25, 50);
        public static CommodityInfomation O = new CommodityInfomation("O", "Oats", "CBOT", "H,K,N,U,Z", 0.25, 50);
        public static CommodityInfomation S = new CommodityInfomation("S", "Soybeans", "CBOT", "F,H,K,N,Q,U,X", 0.25, 50);
        public static CommodityInfomation SM = new CommodityInfomation("SM", "Soybean Meal", "CBOT", "F,H,K,N,Q,U,V,Z", 0.1, 100);
        public static CommodityInfomation W = new CommodityInfomation("W", "Wheat", "CBOT", "H,K,N,U,Z", 0.25, 50);

        //Stock Indices
        public static CommodityInfomation DJ = new CommodityInfomation("DJ", "Dow Jones Industrials", "CBOT", "H,M,U,Z", 1, 10);
        public static CommodityInfomation KV = new CommodityInfomation("KV", "Value Line (Discontinued)", "CBOT", "H,M,U,Z", 0.05, 250);
        public static CommodityInfomation MV = new CommodityInfomation("MV", "Value Line (Mini)", "CBOT", "H,M,U,Z", 0.05, 100);
        public static CommodityInfomation ND = new CommodityInfomation("ND", "Nasdaq-100", "CME", "H,M,U,Z", 0.25, 100);
        public static CommodityInfomation RL = new CommodityInfomation("RL", "Russell 2000 (Discontinued)", "CME", "H,M,U,Z", 0.05, 500);
        public static CommodityInfomation SP = new CommodityInfomation("SP", "S & P 500", "CME", "H,M,U,Z", 0.05, 250);
        public static CommodityInfomation YU = new CommodityInfomation("YU", "NYSE Composite (Discontinued)", "NYFE", "H,M,U,Z", 0.05, 500);

        //Interest Rates
        public static CommodityInfomation ED = new CommodityInfomation("ED", "Eurodollars", "CME", "H,M,U,Z", 0.005, 2500);
        public static CommodityInfomation FV = new CommodityInfomation("FV", "5-Yr T-Notes", "CBOT", "H,M,U,Z", 0.015625, 1000);
        public static CommodityInfomation MB = new CommodityInfomation("MB", "Municipal Bonds1/32", "CBOT", "H,M,U,Z", 0.03125, 1000);
        public static CommodityInfomation TU = new CommodityInfomation("TU", "2-Yr T-Notes", "CBOT", "H,M,U,Z", 0.0078125, 2000);
        public static CommodityInfomation TY = new CommodityInfomation("FV", "10-Yr T-Notes", "CBOT", "H,M,U,Z", 0.015625, 1000);
        public static CommodityInfomation US = new CommodityInfomation("US", "30-Yr T-Bonds", "CBOT", "H,M,U,Z", 0.03125, 1000);

        //Meats
        public static CommodityInfomation FC = new CommodityInfomation("FC", "Feeder Cattle", "CME", "F,H,J,K,Q,U,V,X", 0.025, 500);
        public static CommodityInfomation LC = new CommodityInfomation("LC", "Live Cattle", "CME", "G,J,M,Q,V,Z", 0.025, 400);
        public static CommodityInfomation LH = new CommodityInfomation("LH", "Lean Hogs", "CME", "G,J,K,M,N,Q,V,Z", 0.025, 400);
        public static CommodityInfomation LE = new CommodityInfomation("LE", "Lean Hogs", "CME", "G,J,K,M,N,Q,V,Z", 0.025, 400);
        public static CommodityInfomation PB = new CommodityInfomation("PB", "Pork Bellies", "CME", "G,H,K,N,Q", 0.025, 400);
        public static CommodityInfomation DA = new CommodityInfomation("DA", "Milk Class III", "CME", "F,G,H,J,K,M,N,Q,U,V,X,Z", 0.01, 2000);

        //Metals
        public static CommodityInfomation GC = new CommodityInfomation("GC", "Gold", "CMX", "G,J,M,Q,V,Z", 0.1, 100);
        public static CommodityInfomation HG = new CommodityInfomation("HG", "Copper", "CMX", "H,K,N,U,Z", 0.05, 250);
        public static CommodityInfomation PL = new CommodityInfomation("PL", "Platinum", "NYM", "F,J,N,V", 0.1, 50);
        public static CommodityInfomation SI = new CommodityInfomation("SI", "Silver", "CMX", "F,H,K,N,U,Z", 0.005, 50);

        //Softs & Fibers
        public static CommodityInfomation RR = new CommodityInfomation("RR", "Rice", "CBOT", "F,H,K,N,U,X", 0.5, 100);
        public static CommodityInfomation CC = new CommodityInfomation("CC", "Cocoa", "ICE", "H,K,N,U,Z", 1, 10);
        public static CommodityInfomation CT = new CommodityInfomation("CT", "Cotton", "ICE", "H,K,N,V,Z", 0.01, 500);
        public static CommodityInfomation KC = new CommodityInfomation("KC", "Coffee", "ICE", "H,K,N,U,Z", 0.05, 375);
        public static CommodityInfomation LB = new CommodityInfomation("LB", "Lumber", "CME", "F,H,K,N,U,X", 0.1, 110);
        public static CommodityInfomation JO = new CommodityInfomation("JO", "Orange Juice", "ICE", "F,H,K,N,U,X", 0.05, 150);
        public static CommodityInfomation SB = new CommodityInfomation("SB", "Sugar #11", "ICE", "H,K,N,V", 0.01, 1120);

        public static Dictionary<string, CommodityInfomation> CommodityDict = new Dictionary<string, CommodityInfomation>();

        static CommodityInfomation()
        {
            Type theCommodityInfoType = RR.GetType();

            FieldInfo[] fields = theCommodityInfoType.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fld in fields)
            {
                if (fld.FieldType == theCommodityInfoType)
                {
                    CommodityDict.Add(fld.Name, fld.GetValue(null) as CommodityInfomation);
                }
            }
        }

        public static CommodityInfomation CommodityOf(string fileName)
        {
            int index = fileName.LastIndexOf('\\');

            string name  = fileName.Substring(index + 1);

            if (name.Contains('.'))
                name = name.Substring(0, name.IndexOf('.'));

            index = -1;
            for (int i = 0; i < name.Length; i ++ )
            {
                char ch = name[i];
                if (ch >= '0' && ch <= '9')
                {
                    index = i;
                    break;
                }
            }
            
            string symbol = null;

            if (index != -1)
            {
                symbol = name.Substring(0, index).ToUpper();
            }
            else
            {
                symbol = name.Substring(0, name.Length - 2);
            }

            if (!CommodityDict.ContainsKey(symbol))
                return null;

            return CommodityDict[symbol];
        }

        #endregion

    }

    public class ContractInfomation : IComparable<ContractInfomation>, IEquatable<ContractInfomation>
    {
        public CommodityInfomation Commodity { get; set; }

        public int Year { get; set; }

        public int Month { get; private set; }

        public MonthCodes Code { get { return MonthCodes.MonthCodeOf(Month); } }

        public string Name
        {
            get { return string.Format("{0}{1}{2}", Commodity.Symbol, Year.ToString().Substring(0, 2), Code.Symbol); }
        }

        public ContractInfomation(CommodityInfomation commodity, int year, int month)
        {
            Commodity = commodity;
            Year = year;
            Month = month;
        }

        public ContractInfomation Next()
        {
            MonthCodes nextCode = Commodity.NextOf(Code);

            if (Code.Month < nextCode.Month)
                return new ContractInfomation(Commodity, Year, MonthCodes.MonthOf(nextCode.Symbol));
            else
                return new ContractInfomation(Commodity, Year + 1, MonthCodes.MonthOf(nextCode.Symbol));
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Commodity.Description, Year, Code.Name);
        }

        #region IComparable<ContractInfomation> 成员

        public int CompareTo(ContractInfomation other)
        {
            return (this.Year - other.Year) * 12 + this.Month - other.Month;
        }

        #endregion

        #region IEquatable<ContractInfomation> 成员

        public bool Equals(ContractInfomation other)
        {
            return this.Commodity == other.Commodity && this.Year == other.Year && this.Month == other.Month;
        }

        #endregion
    }
}
