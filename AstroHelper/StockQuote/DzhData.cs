using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlTypes;

namespace StockQuote
{
//    public class DzhData
//    {
//        public enum DataTypes { dm, hq, hqmb, hq0, hq1, hq5, cq, cw0, fp, gb, gd, cw, jjjz, jjzh, bk, pj, hqfq };
//        private string[,] tableNames = new string[,] {
//            #region 表名
//            {"dm","代码",""},
//            {"hq","行情",""},
//            {"hqmb","分笔成交",""},
//            {"hq0","动态行情",""},
//            {"hq1","一分钟行情(N/A)",""},
//            {"hq5","五分钟行情<(N/A)",""},
//            {"cq","除权数据",""},
//            {"cw0","最新财务数据",""},
//            {"fp","分红送配(N/A)",""},
//            {"gb","股本结构(N/A)",""},
//            {"gd","十大股东(N/A)",""},
//            {"cw","财务数据(N/A)",""},
//            {"jjjz","基金净值(N/A)",""},
//            {"jjzh","基金投资组合(N/A)",""},
//            {"bk","板块",""},
//            {"pj","评级(N/A)",""},
//            {"hqfq","复权行情",""} 

//        };//行顺序与Datatype一致，列分别为表名、表中文名、对应文件名（GetTables函数中赋值）
//            #endregion
//        public DzhData()
//        {
//            try
//            {
//                //从注册表中读取大智慧数据目录，如c:\dzh\data
//                RegistryKey keyDzh;
//                RegistryKey keySoftware = Registry.LocalMachine.OpenSubKey("Software");
//                keyDzh = keySoftware.OpenSubKey("DZH");
//                if (keyDzh == null)
//                {
//                    keyDzh = keySoftware.OpenSubKey("Huitianqi");
//                    if (keyDzh == null)
//                    {
//                        dzhPath = "";
//                        dzhDataPath = "";
//                        msg = "没有找到大智慧安装信息！";
//                        return;
//                    }
//                }
//                RegistryKey keySuperstk = keyDzh.OpenSubKey("SUPERSTK");
//                if (keySuperstk != null)
//                {
//                    dzhPath = (string)keySuperstk.GetValue("InstPath");
//                    if (dzhPath != "")
//                    {
//                        dzhPath = dzhPath.ToUpper();
//                        if (dzhPath != "" && dzhPath.EndsWith(@"\") == false)
//                            dzhPath = dzhPath + @"\";
//                        dzhDataPath = dzhPath + @"DATA\";
//                        dzhDataPath = dzhDataPath.ToUpper();
//                        RegistryKey keyMarket = keySuperstk.OpenSubKey("Market");
//                        if (keyMarket != null)
//                        {
//                            string[] marketSubKeyNames = keyMarket.GetSubKeyNames();
//                            if (marketSubKeyNames.Length > 0)
//                            {
//                                RegistryKey[] marketSubKey = new RegistryKey[marketSubKeyNames.Length];
//                                dzhMarket = new string[marketSubKeyNames.Length, 3];
//                                for (int currentIndex = 0; currentIndex < marketSubKeyNames.Length; currentIndex++)
//                                {
//                                    marketSubKey[currentIndex] = keyMarket.OpenSubKey(marketSubKeyNames[currentIndex]);
//                                    if (marketSubKey[currentIndex] != null)
//                                    {
//                                        dzhMarket[currentIndex, 0] = marketSubKeyNames[currentIndex];
//                                        dzhMarket[currentIndex, 1] = (string)marketSubKey[currentIndex].GetValue("name");
//                                        dzhMarket[currentIndex, 2] = (string)marketSubKey[currentIndex].GetValue("shortname");
//                                    }
//                                }
//                                for (int currentIndex = 0; currentIndex < marketSubKeyNames.Length; currentIndex++)
//                                {
//                                    int lastI = marketSubKeyNames.Length - 1;
//                                    if (dzhMarket[currentIndex, 0].ToUpper() == "SH")
//                                    {
//                                        string[] temp = new string[3];
//                                        temp[0] = dzhMarket[0, 0];
//                                        temp[1] = dzhMarket[0, 1];
//                                        temp[2] = dzhMarket[0, 2];
//                                        dzhMarket[0, 0] = dzhMarket[currentIndex, 0];
//                                        dzhMarket[0, 1] = dzhMarket[currentIndex, 1];
//                                        dzhMarket[0, 2] = dzhMarket[currentIndex, 2];
//                                        dzhMarket[currentIndex, 0] = temp[0];
//                                        dzhMarket[currentIndex, 1] = temp[1];
//                                        dzhMarket[currentIndex, 2] = temp[2];
//                                    }
//                                    if (dzhMarket[currentIndex, 0].ToUpper() == "SZ")
//                                    {
//                                        string[] temp = new string[3];
//                                        temp[0] = dzhMarket[1, 0];
//                                        temp[1] = dzhMarket[1, 1];
//                                        temp[2] = dzhMarket[1, 2];
//                                        dzhMarket[1, 0] = dzhMarket[currentIndex, 0];
//                                        dzhMarket[1, 1] = dzhMarket[currentIndex, 1];
//                                        dzhMarket[1, 2] = dzhMarket[currentIndex, 2];
//                                        dzhMarket[currentIndex, 0] = temp[0];
//                                        dzhMarket[currentIndex, 1] = temp[1];
//                                        dzhMarket[currentIndex, 2] = temp[2];
//                                    }
//                                    if (dzhMarket[currentIndex, 0].ToUpper() == "$$")
//                                    {
//                                        string[] temp = new string[3];
//                                        temp[0] = dzhMarket[lastI, 0];
//                                        temp[1] = dzhMarket[lastI, 1];
//                                        temp[2] = dzhMarket[lastI, 2];
//                                        dzhMarket[lastI, 0] = dzhMarket[currentIndex, 0];
//                                        dzhMarket[lastI, 1] = dzhMarket[currentIndex, 1];
//                                        dzhMarket[lastI, 2] = dzhMarket[currentIndex, 2];
//                                        dzhMarket[currentIndex, 0] = temp[0];
//                                        dzhMarket[currentIndex, 1] = temp[1];
//                                        dzhMarket[currentIndex, 2] = temp[2];
//                                    }
//                                }
//                            }
//                        }
//                        return;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                msg = ex.Message;
//            }
//        }
//        struct fileStruct
//        {
//            public string fileName;//文件名
//            public int startAddress, blockSize, recordSize;//起始地址，每块长度，记录长度
//            public bool codeIsLong, isIndexDataStruct;   //codeIsLong索引中的代码包含有市场代码SH、SZ等;isIndexDataStruct象Day.Dat那样的结构即由索引+数据组成; 
//            public string[,] fields;//字段
//            public fileStruct(DataTypes fileType)
//            {
//                fileName = "";
//                startAddress = 0;
//                blockSize = 0;
//                recordSize = 0;
//                codeIsLong = false;
//                isIndexDataStruct = true;
//                string fieldString = ""; //字段名，字段标签，类型，长度字段，存储顺序，偏移量
//                switch (fileType)
//                {

//                    #region 代码表STKINFO60.dat
//                    case DataTypes.dm:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x6D0226;//0x6D0226;//0x68A8A6;Xchui
//                        blockSize = 0;
//                        recordSize = 273;//fxj248
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"jc,简称,string,32,1,10,";
//                        break;
//                    #endregion
//                    #region 分红送配STKINFO60.dat
//                    case DataTypes.cq:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x44aa;
//                        blockSize = 2227;
//                        recordSize = 20;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,date,4,0,0,;" +
//"sgbl,送股比例,single,4,1,4,;" +
//"pgbl,配股比例,single,4,2,8,;" +
//"pgjg,配股价格,single,4,3,12,;" +
//"fh,分红,single,4,4,16,";
//                        break;
//                    #endregion
//                    #region 财务数据（简单）STKINFO60.dat
//                    case DataTypes.cw0:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x4c2a;//0x4A5AA;//0x4c2a;Xchui
//                        blockSize = 2227;
//                        recordSize = 273;//196
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,报告期,date,4,0,4,;" +
//"gxrq,更新日期,date,4,0,0,;" +
//"ssrq,上市日期,date,4,0,8,;" +
//"col1,每股收益,single,4,0,12,;" +
//"col2,每股净资产,single,4,0,16,;" +
//"col3,净资产收益率,single,4,0,20,;" +
//"col4,每股经营现金,single,4,0,24,;" +
//"col5,每股公积金,single,4,0,28,;" +
//"col6,每股未分配,single,4,0,32,;" +
//"col7,股东权益比,single,4,0,36,;" +
//"col8,净利润同比,single,4,0,40,;" +
//"col9,主营收入同比,single,4,0,44,;" +
//"col10,销售毛利率,single,4,0,48,;" +
//"col11,调整每股净资产,single,4,0,52,;" +
//"col12,总资产,single,4,0,56,;" +
//"col13,流动资产,single,4,0,60,;" +
//"col14,固定资产,single,4,0,64,;" +
//"col15,无形资产,single,4,0,68,;" +
//"col16,流动负债,single,4,0,72,;" +
//"col17,长期负债,single,4,0,76,;" +
//"col18,总负债,single,4,0,80,;" +
//"col19,股东权益,single,4,0,84,;" +
//"col20,资本公积金,single,4,0,88,;" +
//"col21,经营现金流量,single,4,0,92,;" +
//"col22,投资现金流量,single,4,0,96,;" +
//"col23,筹资现金流量,single,4,0,100,;" +
//"col24,现金增加额,single,4,0,104,;" +
//"col25,主营收入,single,4,0,108,;" +
//"col26,主营利润,single,4,0,112,;" +
//"col27,营业利润,single,4,0,116,;" +
//"col28,投资收益,single,4,0,120,;" +
//"col29,营业外收支,single,4,0,124,;" +
//"col30,利润总额,single,4,0,128,;" +
//"col31,净利润,single,4,0,132,;" +
//"col32,未分配利润,single,4,0,136,;" +
//"col33,总股本,single,4,0,140,;" +
//"col34,无限售股合计,single,4,0,144,;" +
//"col35,A股,single,4,0,148,;" +
//"col36,B股,single,4,0,152,;" +
//"col37,境外上市股,single,4,0,156,;" +
//"col38,其他流通股,single,4,0,160,;" +
//"col39,限售股合计,single,4,0,164,;" +
//"col40,国家持股,single,4,0,168,;" +
//"col41,国有法人股,single,4,0,172,;" +
//"col42,境内法人股,single,4,0,176,;" +
//"col43,境内自然人股,single,4,0,180,;" +
//"col44,其他发起人股,single,4,0,184,;" +
//"col45,募集法人股,single,4,0,188,;" +
//"col46,境外法人股,single,4,0,192,;" +
//"col47,境外自然人股,single,4,0,196,;" +
//"col48,优先股或其他,single,4,0,200,";
//                        break;
//                    #endregion
//                    #region 最新行情STKINFO60.dat
//                    case DataTypes.hq0:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x6D0226;//0x6D0226;//0x68A8A6;Xchui
//                        blockSize = 0;
//                        recordSize = 273;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"jc,简称,string,32,1,10,;" +
//"rq,更新时间,datetime,4,5,60,;" +
//"zs,昨收,single,4,7,68,;" +
//"kp,今开,single,4,8,72,;" +
//"zg,最高,single,4,9,76,;" +
//"zd,最低,single,4,10,80,;" +
//"sp,最新,single,4,11,84,;" +
//"sl,总手数,single,4,12,88,;" +
//"je,金额,single,4,13,92,;" +
//"xss,现手数,single,4,14,96,;" +
//"ztj,涨停价,single,4,27,184,;" +
//"dtj,跌停价,single,4,28,188,;" +
//"np,内盘,single,4,27,192,;" +
//"wp,外盘,single,4,28,196,;" +
//"mrjg1,买一价,single,4,15,100,;" +
//"mrsl1,买一量,single,4,18,120,;" +
//"mrjg2,买二价,single,4,16,104,;" +
//"mrsl2,买二量,single,4,19,124,;" +
//"mrjg3,买三价,single,4,17,108,;" +
//"mrsl3,买三量,single,4,20,128,;" +
//"mrjg4,买四价,single,4,32,112,;" +
//"mrsl4,买四量,single,4,34,132,;" +
//"mrjg5,买五价,single,4,33,116,;" +
//"mrsl5,买五量,single,4,35,136,;" +
//"mcjg1,卖一价,single,4,21,140,;" +
//"mcsl1,卖一量,single,4,24,160,;" +
//"mcjg2,卖二价,single,4,22,144,;" +
//"mcsl2,卖二量,single,4,25,164,;" +
//"mcjg3,卖三价,single,4,23,148,;" +
//"mcsl3,卖三量,single,4,26,168,;" +
//"mcjg4,卖四价,single,4,36,152,;" +
//"mcsl4,卖四量,single,4,38,172,;" +
//"mcjg5,卖五价,single,4,37,156,;" +
//"mcsl5,卖五量,single,4,39,176,";
//                        //"jd,精度,int,4,3,52,;" +
//                        //"scbz,删除标志,int,4,4,56,";
//                        //"unknown,(未知),int,4,31,164,;" +
//                        //",(未知),,48,40,200,;"
//                        break;
//                    #endregion
//                    #region 分笔成交数据文件report.dat（结构同day.dat，但其中一些数据不是直接保存）
//                    case DataTypes.hqmb:
//                        fileName = "REPORT.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 12272;
//                        recordSize = 52;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;//不完全等同于day.dat结构，因此单独处理
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,datetime,4,0,0,;" +
//"zjcj,最近成交价,single,4,1,4,;" +
//"zss,总手数,single,4,2,8,calc;" +
//"je,金额,single,4,3,12,;" +
//"xss,现手数,single,4,2,8,;" +
//"mm,内外盘,string,2,16,21,;" +
//"mr1jg,买一价,single,1,10,42,;" +
//"mr1sl,买一量,single,2,4,22,;" +
//"mr2jg,买二价,single,1,11,43,;" +
//"mr2sl,买二量,single,2,5,24,;" +
//"mr3jg,买三价,single,1,12,44,;" +
//"mr3sl,买三量,single,2,6,26,;" +
//"mr4jg,买四价,single,1,12,45,;" +
//"mr4sl,买四量,single,2,6,28,;" +
//"mr5jg,买五价,single,1,12,46,;" +
//"mr5sl,买五量,single,2,6,30,;" +
//"mc1jg,卖一价,single,1,13,47,;" +
//"mc1sl,卖一量,single,2,7,32,;" +
//"mc2jg,卖二价,single,1,14,48,;" +
//"mc2sl,卖二量,single,2,8,34,;" +
//"mc3jg,卖三价,single,1,15,49,;" +
//"mc3sl,卖三量,single,2,9,36,;" +
//"mc4jg,卖四价,single,1,14,50,;" +
//"mc4sl,卖四量,single,2,8,38,;" +
//"mc5jg,卖五价,single,1,14,51,;" +
//"mc5sl,卖五量,single,2,8,40,;" +
//"bs,总笔数,int,2,0,16,"
//;
//                        //以上数据类型不是存储类型，程序中不直接用实际数据类型：买/卖X量为short，买/卖X价为byte
//                        //现手数通过当总手数计算而得，应该放在总手数后面
//                        break;
//                    #endregion
//                    #region 日线数据文件day.dat
//                    case DataTypes.hq:
//                        fileName = "DAY.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,date,4,1,0,;" +
//"kp,开盘,single,4,2,4,B;" +
//"zg,最高,single,4,3,8,B;" +
//"zd,最低,single,4,4,12,B;" +
//"sp,收盘,single,4,5,16,B;" +
//"sl,成交数量,single,4,6,20,A;" +
//"je,成交金额,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region 1分钟数据文件min1.dat
//                    case DataTypes.hq1:
//                        fileName = "MIN1.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 12288;//8192
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,datetime,4,1,0,;" +
//"kp,开盘,single,4,2,4,B;" +
//"zg,最高,single,4,3,8,B;" +
//"zd,最低,single,4,4,12,B;" +
//"sp,收盘,single,4,5,16,B;" +
//"sl,成交数量,single,4,6,20,A;" +
//"je,成交金额,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region 5分钟数据文件min.dat
//                    case DataTypes.hq5:
//                        fileName = "MIN.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,datetime,4,1,0,;" +
//"kp,开盘,single,4,2,4,B;" +
//"zg,最高,single,4,3,8,B;" +
//"zd,最低,single,4,4,12,B;" +
//"sp,收盘,single,4,5,16,B;" +
//"sl,成交数量,single,4,6,20,A;" +
//"je,成交金额,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region 分红送配数据文件exprof.fdt
//                    case DataTypes.fp:
//                        fileName = "EXPROF.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 3776;
//                        recordSize = 236;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0,;" +
//"cqrq,除权日期,date,4,23,176,;" +
//"sgbl,送股比例,double,8,1,12,;" +
//"sgdjr,送股股权登记日,date,4,2,20,;" +
//"sgcqr,送股除权日,date,4,3,24,;" +
//"sgssr,红股上市日,date,4,4,28,;" +
//"zzbl,转增比例,double,8,5,32,;" +
//"zzdjr,转增股权登记日,date,4,6,40,;" +
//"zzcqr,转增除权日,date,4,7,44,;" +
//"zzssr,转增上市日,date,4,8,48,;" +
//"fhbl,分红比例,double,8,9,52,;" +
//"fhdjr,分红股权登记日,date,4,10,60,;" +
//"fhcxr,分红除息日,date,4,11,64,;" +
//"fhpxr,分红派息日,date,4,12,68,;" +
//"pgbl,配股比例,double,8,13,72,;" +
//"pgdjr,配股股权登记日,date,4,14,80,;" +
//"pgcqr,配股除权基准日,date,4,15,84,;" +
//"pgjkqsr,配股缴款起始日,date,4,16,88,;" +
//"pgjkzzr,配股缴款终止日,date,4,17,92,;" +
//"pgssr,配股可流通上市日,date,4,18,96,;" +
//"pgjg,配股价格,single,4,19,100,;" +
//"frgpgbl,公众股受让法人股配股比例,double,8,20,104,;" +
//"frgmgzrf,认购法人股配股每股转让费,single,4,21,112,;" +
//"pgzcxs,配股主承销商,string,60,22,116,;" +
//"bgrq,报告日期,date,4,24,180,;" +
//"dshrq,董事会日期,date,4,25,184,;" +
//"gdhrq,股东会日期,date,4,26,188,;" +
//"fhggrq,分红公告日期,date,4,27,192,;" +
//"zgbjs,总股本基数,double,8,28,196,;" +
//"sgsl,送股数量,double,8,29,204,;" +
//"zzsl,转增数量,double,8,30,212,;" +
//"sjpgs,实际配股总数,double,8,31,220,;" +
//"cqhzgb,除权后总股本,double,8,32,228";

//                        break;
//                    #endregion
//                    #region 股本结构Capital.fdt
//                    case DataTypes.gb:
//                        fileName = "CAPITAL.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 3488;
//                        recordSize = 218;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0;" +
//"rq,日期,date,4,17,214;" +
//"zgb,总股本,double,8,1,12;" +
//"gjg,国家股,double,8,2,20;" +
//"fqrg,发起人股,double,8,3,28;" +
//"frg,法人股,double,8,4,36;" +
//"ybfrps,一般法人配售,double,8,5,44;" +
//"zgg,内部职工股,double,8,6,52;" +
//"a,流通A股,double,8,7,60;" +
//"zltzag,战略投资A股,double,8,8,68;" +
//"zpg,转配股,double,8,9,76;" +
//"jjps,基金配售,double,8,10,84;" +
//"h,H股,double,8,11,92;" +
//"b,B股,double,8,12,100;" +
//"yxg,优先股,double,8,13,108;" +
//"ggcg,高级管理人员持股,double,8,14,116;" +
//"gbbdyy,股本变动原因,string,56,15,124;" +
//"gbbdyylb,股本变动原因类别,string,34,16,180";

//                        break;
//                    #endregion
//                    #region 财务数据Finance.fdt
//                    case DataTypes.cw:
//                        fileName = "FINANCE.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 14848;
//                        recordSize = 464;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0,;" +
//"rq,日期,date,4,,460,;" +
//"bsdqtzje,短期投资净额,double,8,1,12,;" +
//"bsyszkje,应收帐款净额,double,8,2,20,;" +
//"bschje,存货净额,double,8,3,28,;" +
//"bsldzc,流动资产,double,8,4,36,;" +
//"bscqtzje,长期投资净额,double,8,5,44,;" +
//"bsgdzc,固定资产,double,8,6,52,;" +
//"bswxzc,无形及其他资产,double,8,7,60,;" +
//"bszzc,总资产,double,8,8,68,;" +
//"bsdqjk,短期借款,double,8,9,76,;" +
//"bsyfzk,应付帐款,double,8,10,84,;" +
//"bsldfz,流动负债,double,8,11,92,;" +
//"bscqfz,长期负债,double,8,12,100,;" +
//"bsfz,负债合计,double,8,13,108,;" +
//"bsgb,股本,double,8,14,116,;" +
//"bsssgdqy,少数股东权益,double,8,15,124,;" +
//"bsgdqy,股东权益,double,8,16,132,;" +
//"bszbgj,资本公积,double,8,17,140,;" +
//"bsyygj,盈余公积,double,8,18,148,;" +
//"iszysr,主营业务收入净额,double,8,1,156,;" +
//"iszycb,主营业务成本,double,8,2,164,;" +
//"iszylr,主营业务利润,double,8,3,172,;" +
//"isqtlr,其它业务利润,double,8,4,180,;" +
//"isyyfy,营业费用,double,8,5,188,;" +
//"isglfy,管理费用,double,8,6,196,;" +
//"iscwfy,财务费用,double,8,7,204,;" +
//"istzsy,投资收益,double,8,8,212,;" +
//"islrze,利润总额,double,8,9,220,;" +
//"issds,所得税,double,8,10,228,;" +
//"isjlr,净利润,double,8,11,236,;" +
//"iskchjlr,扣除经常性损益后的净利润,double,8,12,244,;" +
//"iswfplr,未分配利润,double,8,13,252,;" +
//"cfjyhdxjlr,经营活动现金流入,double,8,1,260,;" +
//"cfjyhdxjlc,经营活动现金流出,double,8,2,268,;" +
//"cfjyhdxjje,经营活动现金净额,double,8,3,276,;" +
//"cftzxjlr,投资现金流入,double,8,4,284,;" +
//"cftzxjlc,投资现金流出,double,8,5,292,;" +
//"cftzxjje,投资现金净额,double,8,6,300,;" +
//"cfczxjlr,筹措现金流入,double,8,7,308,;" +
//"cfczxjlc,筹措现金流出,double,8,8,316,;" +
//"cfczxjje,筹措现金净额,double,8,9,324,;" +
//"cdzhjze,现金及现金等价物净增额,double,8,10,332,;" +
//"cfxsspxj,销售商品收到的现金,double,8,11,340,;" +
//"mgsy,每股收益,single,4,1,348,;" +
//"mgjzc,每股净资产,single,4,2,352,;" +
//"tzmgjzc,调整后每股净资产,single,4,3,356,;" +
//"mgzbgjj,每股资本公积金,single,4,4,360,;" +
//"mgwfplr,每股未分配利润,single,4,5,364,;" +
//"mgjyxjllje,每股经营活动产生的现金流量净额,single,4,6,368,;" +
//"mgxjzjje,每股现金及现金等价物增加净额,single,4,7,372,;" +
//"mll,毛利率,single,4,8,376,;" +
//"zyywlrl,主营业务利润率,single,4,9,380,;" +
//"jll,净利率,single,4,10,384,;" +
//"zzcbcl,总资产报酬率,single,4,11,388,;" +
//"jzcsyl,净资产收益率,single,4,12,392,;" +
//"xsxjzb,销售商品收到的现金占主营收入比例,single,4,13,396,;" +
//"yszczzl,应收帐款周转率,single,4,14,400,;" +
//"chzzl,存货周转率,single,4,15,404,;" +
//"gdzczzl,固定资产周转率,single,4,16,408,;" +
//"zyywzzl,主营业务增长率,single,4,17,412,;" +
//"jlrzzl,净利润增长率,single,4,18,416,;" +
//"zzczzl,总资产增长率,single,4,19,420,;" +
//"jzczzl,净资产增长率,single,4,20,424,;" +
//"ldbl,流动比率,single,4,21,428,;" +
//"sdbl,速动比率,single,4,22,432,;" +
//"zcfzbl,资产负债比率,single,4,23,436,;" +
//"fzbl,负债比率,single,4,24,440,;" +
//"gdqybl,股东权益比率,single,4,25,444,;" +
//"gdzcbl,固定资产比率,single,4,26,448,;" +
//"kchmgjlr,扣除经常性损益后每股净利润,single,4,27,452,";

//                        break;
//                    #endregion
//                    #region 十大股东stkhold.fdt
//                    case DataTypes.gd:
//                        fileName = "STKHOLD.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 17568;
//                        recordSize = 2196;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0,;" +
//"rq,日期,date,4,66,2192,;" +
//"gd1mc,股东1名称,string,160,1,12,;" +
//"gd1cgsl,股东1持股数量,double,8,2,172,;" +
//"gd1cgbl,股东1持股比例,single,4,3,180,;" +
//"gd1bz,股东1备注,string,20,4,184,;" +
//"gd1fr,股东1法人,string,8,5,204,;" +
//"gd1jyfw,股东1经营范围,string,16,6,212,;" +
//"gd2mc,股东2名称,string,160,7,228,;" +
//"gd2cgsl,股东2持股数量,double,8,8,388,;" +
//"gd2cgbl,股东2持股比例,single,4,9,396,;" +
//"gd2bz,股东2备注,string,20,10,400,;" +
//"gd2fr,股东2法人,string,8,11,420,;" +
//"gd2jyfw,股东2经营范围,string,16,12,428,;" +
//"gd3mc,股东3名称,string,160,13,444,;" +
//"gd3cgsl,股东3持股数量,double,8,14,604,;" +
//"gd3cgbl,股东3持股比例,single,4,15,612,;" +
//"gd3bz,股东3备注,string,20,16,616,;" +
//"gd3fr,股东3法人,string,8,17,636,;" +
//"gd3jyfw,股东3经营范围,string,16,18,644,;" +
//"gd4mc,股东4名称,string,160,19,660,;" +
//"gd4cgsl,股东4持股数量,double,8,20,820,;" +
//"gd4cgbl,股东4持股比例,single,4,21,828,;" +
//"gd4bz,股东4备注,string,20,22,832,;" +
//"gd4fr,股东4法人,string,8,23,852,;" +
//"gd4jyfw,股东4经营范围,string,16,24,860,;" +
//"gd5mc,股东5名称,string,160,25,876,;" +
//"gd5cgsl,股东5持股数量,double,8,26,1036,;" +
//"gd5cgbl,股东5持股比例,single,4,27,1044,;" +
//"gd5bz,股东5备注,string,20,28,1048,;" +
//"gd5fr,股东5法人,string,8,29,1068,;" +
//"gd5jyfw,股东5经营范围,string,16,30,1076,;" +
//"gd6mc,股东6名称,string,160,31,1092,;" +
//"gd6cgsl,股东6持股数量,double,8,32,1252,;" +
//"gd6cgbl,股东6持股比例,single,4,33,1260,;" +
//"gd6bz,股东6备注,string,20,34,1264,;" +
//"gd6fr,股东6法人,string,8,35,1284,;" +
//"gd6jyfw,股东6经营范围,string,16,36,1292,;" +
//"gd7mc,股东7名称,string,160,37,1308,;" +
//"gd7cgsl,股东7持股数量,double,8,38,1468,;" +
//"gd7cgbl,股东7持股比例,single,4,39,1476,;" +
//"gd7bz,股东7备注,string,20,40,1480,;" +
//"gd7fr,股东7法人,string,8,41,1500,;" +
//"gd7jyfw,股东7经营范围,string,16,42,1508,;" +
//"gd8mc,股东8名称,string,160,43,1524,;" +
//"gd8cgsl,股东8持股数量,double,8,44,1684,;" +
//"gd8cgbl,股东8持股比例,single,4,45,1692,;" +
//"gd8bz,股东8备注,string,20,46,1696,;" +
//"gd8fr,股东8法人,string,8,47,1716,;" +
//"gd8jyfw,股东8经营范围,string,16,48,1724,;" +
//"gd9mc,股东9名称,string,160,49,1740,;" +
//"gd9cgsl,股东9持股数量,double,8,50,1900,;" +
//"gd9cgbl,股东9持股比例,single,4,51,1908,;" +
//"gd9bz,股东9备注,string,20,52,1912,;" +
//"gd9fr,股东9法人,string,8,53,1932,;" +
//"gd9jyfw,股东9经营范围,string,16,54,1940,;" +
//"gd10mc,股东10名称,string,160,55,1956,;" +
//"gd10cgsl,股东10持股数量,double,8,56,2116,;" +
//"gd10cgbl,股东10持股比例,single,4,57,2124,;" +
//"gd10bz,股东10备注,string,20,58,2128,;" +
//"gd10fr,股东10法人,string,8,59,2148,;" +
//"gd10jyfw,股东10经营范围,string,16,60,2156,;" +
//"gdzs,股东总数,int,4,61,2172,;" +
//"gjgfrggds,国家股法人股股东数,int,4,62,2176,;" +
//"aggds,流通股A股股东数,int,4,63,2180,;" +
//"bggds,流通股B股股东数,int,4,64,2184,";

//                        break;
//                    #endregion
//                    #region 基金周报fundweek.fdt
//                    case DataTypes.jjjz:
//                        fileName = "FUNDWEEK.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 12032;
//                        recordSize = 188;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0,;" +
//"rq,日期,date,4,13,184,;" +
//"dwjz,基金单位净值,single,4,6,152,;" +
//"jjze,基金净值总额,double,8,5,144,;" +
//"gm,基金规模,double,8,4,136,;" +
//"dwcz,基金单位初值,single,4,7,156,;" +
//"tzhjz,基金调整后净值,single,4,8,160,;" +
//"tzhcz,基金调整后初值,single,4,9,164,;" +
//"zzl,基金增长率(%),double,8,10,168,;" +
//"ljjz,基金累计净值,single,4,11,176,;" +
//"slrq,基金设立日期,date,4,1,12,;" +
//"glr,基金管理人,string,60,2,16,;" +
//"tgr,基金托管人,string,60,3,76,"
//;//12为保留字段

//                        break;
//                    #endregion
//                    #region 基金投资组合funddiv.fdt
//                    case DataTypes.jjzh:
//                        fileName = "FUNDDIV.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 8320;
//                        recordSize = 260;
//                        codeIsLong = true;
//                        fieldString =
//"dm,代码,code,12,0,0,;" +
//"bgrq,报告日期,date,4,31,252,;" +
//"zzrq,截止日期,date,4,32,256,;" +
//"dm1,证券1代码,string,12,1,12,;" +
//"sz1,证券1市值,double,8,2,24,;" +
//"bl1,证券1占净值比例(%),single,4,3,32,;" +
//"dm2,证券2代码,string,12,4,36,;" +
//"sz2,证券2市值,double,8,5,48,;" +
//"bl2,证券2占净值比例(%),single,4,6,56,;" +
//"dm3,证券3代码,string,12,7,60,;" +
//"sz3,证券3市值,double,8,8,72,;" +
//"bl3,证券3占净值比例(%),single,4,9,80,;" +
//"dm4,证券4代码,string,12,10,84,;" +
//"sz4,证券4市值,double,8,11,96,;" +
//"bl4,证券4占净值比例(%),single,4,12,104,;" +
//"dm5,证券5代码,string,12,13,108,;" +
//"sz5,证券5市值,double,8,14,120,;" +
//"bl5,证券5占净值比例(%),single,4,15,128,;" +
//"dm6,证券6代码,string,12,16,132,;" +
//"sz6,证券6市值,double,8,17,144,;" +
//"bl6,证券6占净值比例(%),single,4,18,152,;" +
//"dm7,证券7代码,string,12,19,156,;" +
//"sz7,证券7市值,double,8,20,168,;" +
//"bl7,证券7占净值比例(%),single,4,21,176,;" +
//"dm8,证券8代码,string,12,22,180,;" +
//"sz8,证券8市值,double,8,23,192,;" +
//"bl8,证券8占净值比例(%),single,4,24,200,;" +
//"dm9,证券9代码,string,12,25,204,;" +
//"sz9,证券9市值,double,8,26,216,;" +
//"bl9,证券9占净值比例(%),single,4,27,224,;" +
//"dm10,证券10代码,string,12,28,228,;" +
//"sz10,证券10市值,double,8,29,240,;" +
//"bl10,证券10占净值比例(%),single,4,30,248,";


//                        break;
//                    #endregion
//                    #region 板块userdata\block
//                    case DataTypes.bk:
//                        fileName = "BLOCK.DEF";
//                        startAddress = 0;
//                        blockSize = 0;
//                        recordSize = 248;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"lb,类别,string,20,0,0,;" +
//"bk,板块,string,20,1,10,;" +
//"dm,证券代码,string,10,2,42,";
//                        break;
//                    #endregion
//                    #region 评级
//                    case DataTypes.pj:
//                        fileName = "评级.str";
//                        startAddress = 0;
//                        blockSize = 256;
//                        recordSize = 256;
//                        codeIsLong = true;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,证券代码,string,12,0,0,;" +
//"pj,评级,string,2,2,0,;" +
//"sm,说明,string,244,2,0,";
//                        break;
//                    #endregion
//                    #region 复权行情，计算而得
//                    case DataTypes.hqfq:
//                        fileName = "DAY.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,代码,code,10,0,0,;" +
//"rq,日期,date,4,1,0,;" +
//"kp,开盘复权价,single,4,2,4,B;" +
//"zg,最高复权价,single,4,3,8,B;" +
//"zd,最低复权价,single,4,4,12,B;" +
//"sp,收盘复权价,single,4,5,16,B;" +
//"sl,复权成交数量,single,4,6,20,A;" +
//"je,成交金额,single,4,7,24,;" +
//"spsyl,收盘收益率,single,4,0,0,";
//                        break;
//                    #endregion
//                }
//                string[] fieldLine = fieldString.Split(new char[] { ';' });
//                fields = new string[fieldLine.Length, 7];
//                for (int currentIndex = 0; currentIndex < fieldLine.Length; currentIndex++)
//                {
//                    string[] field = fieldLine[currentIndex].Split(new char[] { ',' } , 7);
//                    for (int j = 0; j < field.Length; j++)
//                    {
//                        fields[currentIndex, j] = field[j];
//                    }
//                }
//            }

//        }
//        private string dzhPath = "";
//        private string dzhDataPath = "";
//        private string[,] dzhMarket;
//        private string msg = "";
//        private DateTime date19700101 = new DateTime(1970, 1, 1);
//        private FileStream fs; private BinaryReader br;
//        private void checkFileStream(string dzhFileName)
//        {
//            if (this.fs == null || (this.fs != null && this.fs.Name.ToUpper() != dzhFileName))
//            {
//                if (this.fs != null)
//                {
//                    fs.Close();
//                    br.Close();
//                }
//                fs = new FileStream(dzhFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
//                br = new BinaryReader(fs);
//            }
//        }

//        public string Version
//        {
//            get
//            {
//                return ("2.06");
//            }
//        }
//        public int Error
//        {
//            get
//            {
//                if (msg != "") return 1;
//                else return 0;
//            }
//        }
//        public string Msg
//        {
//            get { return (msg); }
//        }
//        public string DzhPath   //属性DzhPath
//        {
//            get
//            {
//                return (dzhPath);
//            }
//            set
//            {
//                dzhPath = value;
//                dzhPath = dzhPath.Trim().ToUpper();
//                if (dzhPath != "" && !dzhPath.EndsWith(@"\"))
//                {
//                    dzhPath += @"\";
//                }
//                dzhPath = dzhPath.ToUpper();
//            }
//        }
//        public string DzhDataPath   //属性DzhDataPath
//        {
//            get
//            {
//                return (dzhDataPath);
//            }
//            set
//            {
//                dzhDataPath = value;
//                dzhDataPath = dzhDataPath.Trim().ToUpper();
//                if (dzhDataPath != "" && !dzhDataPath.EndsWith(@"\"))
//                {
//                    dzhDataPath += @"\";
//                }
//                dzhDataPath = dzhDataPath.ToUpper();
//            }
//        }

//        public string[,] GetMarkets()
//        {
//            return (dzhMarket);
//        }
//        public string[,] GetTables()
//        {
//            if (tableNames[0, 2] == "")
//            {
//                for (int currentIndex = 0; currentIndex < tableNames.GetLength(0); currentIndex++)
//                {
//                    DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), tableNames[currentIndex, 0].ToLower());
//                    fileStruct dzhFileStruct = new fileStruct(d);
//                    tableNames[currentIndex, 2] = dzhFileStruct.fileName;
//                }

//            }

//            return tableNames;
//        }
//        public string GetTableDef(string dataType, string descDataType, bool delOldTable)
//        {
//            dataType = dataType.Trim(); descDataType = descDataType.Trim();
//            string pivots = "";
//            fileStruct dzhFileStruct = new fileStruct((DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower()));
//            switch (descDataType.ToUpper())
//            {
//                case "SAS":
//                    for (int currentIndex = 0; currentIndex < dzhFileStruct.fields.GetLength(0); currentIndex++)
//                    {
//                        if (pivots != "") pivots += ",";
//                        pivots += dzhFileStruct.fields[currentIndex, 0];//字段
//                        if ("  ,code,string".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " char(" + dzhFileStruct.fields[currentIndex, 3] + ") format=$" + dzhFileStruct.fields[currentIndex, 3] + "."; //字符串
//                        }
//                        else if ("  ,int,single,double".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num "; //数值类型
//                        }
//                        else if ("  ,date".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num format=YYMMDD10."; //date类型
//                        }
//                        else if ("  ,datetime".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num format=datetime."; //datetime类型
//                        }
//                        pivots += " label='" + dzhFileStruct.fields[currentIndex, 1] + "'";//标签

//                    }
//                    pivots = "create table FinData." + dataType + "(" + pivots + ");";
//                    if (delOldTable == true)
//                    {
//                        pivots = "drop table FinData." + dataType + ";" + pivots;
//                    }
//                    pivots = "proc sql;" + pivots + "quit;";
//                    break;
//                case "SASINPUT"://用于SAS直接读取数据时所用的INPUT语句，需进一步修改
//                    for (int currentIndex = 0; currentIndex < dzhFileStruct.fields.GetLength(0); currentIndex++)
//                    {
//                        if ("  ,code,string".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " $" + dzhFileStruct.fields[currentIndex, 3] + "."; //字符串
//                        }
//                        else if ("  ,int,date,datetime".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " ib" + dzhFileStruct.fields[currentIndex, 3] + "."; //数值类型
//                        }
//                        else if ("  ,single".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " float" + dzhFileStruct.fields[currentIndex, 3] + "."; //数值类型
//                        }
//                        else if ("  ,double".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " rb" + dzhFileStruct.fields[currentIndex, 3] + "."; //数值类型
//                        }
//                    }
//                    break;
//                case "FIELDS"://列出字段名称
//                    for (int currentIndex = 0; currentIndex < dzhFileStruct.fields.GetLength(0); currentIndex++)
//                    {
//                        pivots += " " + dzhFileStruct.fields[currentIndex, 0];
//                    }
//                    break;

//                default:
//                    pivots = "";
//                    break;
//            }
//            return pivots;

//        }
//        public string GetCodeType(string code)
//        {
//            code = code.Trim().ToUpper();
//            if (Regex.IsMatch(code, @"(SH000300)") == true)
//            {
//                return "zs";
//            }
//            if (Regex.IsMatch(code, @"(SH60[0-8]\d{3})|(SH90\d{4})|(SZ00[01256789]\d{3})|(SZ20\d{4})|(SZ4[02]\d{4})") == true)
//            {
//                return "gp";
//            }
//            else if (Regex.IsMatch(code, @"(SH00000\d)|(SH00001[0-6])") == true)
//            {
//                return "zs";
//            }
//            else if (Regex.IsMatch(code, @"(SH[012]\d{5})|(SZ1[0123]\d{4})") == true && Regex.IsMatch(code, @"(SH181\d{3})") == false && Regex.IsMatch(code, @"(SH190\d{3})") == false)
//            {
//                return "zq";
//            }
//            else if (Regex.IsMatch(code, @"(SH5[01]\d{4})|(SZ184\d{3})|(SZ1[56]\d{4})") == true)
//            {
//                return "jj";
//            }
//            else if (Regex.IsMatch(code, @"(SH58\d{4})|(SZ03\d{4})") == true)
//            {
//                return "qz";
//            }
//            else if (Regex.IsMatch(code, @"(SH000\d{3})|(SZ399\d{3})|(SH8[013]\d{4})") == true)
//            {
//                return "zs";
//            }
//            return "";
//        }
//        private string[] GetCodes(string Market)   //读取Day.dat中的代码
//        {
//            //沪市指数代码转换表,大智慧同时保存沪市两类代码
//            string[,] codesRename = new string[,] 
//            {
//            {"SH1A0001","SH000001"},
//            {"SH1A0002","SH000002"},
//            {"SH1A0003","SH000003"},
//            {"SH1B0001","SH000004"},
//            {"SH1B0002","SH000005"},
//            {"SH1B0004","SH000006"},
//            {"SH1B0005","SH000007"},
//            {"SH1B0006","SH000008"},
//            {"SH1B0007","SH000010"},
//            {"SH1B0008","SH000011"},
//            {"SH1B0009","SH000012"},
//            {"SH1B0010","SH000013"},
//            {"SH1C0003","SH000016"}         
//            };
//            long len = -1;
//            long pos = 0;
//            int flag;
//            if (DzhDataPath == "")
//            {
//                msg = @"无法在注册表中到大智慧数据文件目录，请自行将属性 DzhDataPath设置为有效路径，如c:\dzh\data\。";
//                return new string[1] { null };
//            }
//            Market = Market.Trim().ToUpper();
//            if (Market == "")
//            {
//                msg = "Market参数只能是市场简称，如沪市为SH，深市为SZ，香港为HK等。";
//                return null;
//            }
//            string DzhFile = dzhDataPath + Market + @"\DAY.DAT";
//            msg = "";
//            if (!File.Exists(DzhFile))  //DAY.DAT文件不存在
//            {
//                msg = DzhFile + "不存在！";
//                return new string[1] { null };
//            }
//            try
//            {
//                this.checkFileStream(DzhFile);
//                int secCounts = 0;//文件中证券总数
//                string code = "";
//                len = fs.Length;
//                fs.Position = 0;
//                flag = br.ReadInt32();
//                if (flag == -65823756)   //0xFC139BF4
//                {
//                    fs.Position = 12;
//                    secCounts = br.ReadInt32();
//                    string[] codes = new string[secCounts];
//                    for (int currentIndex = 0; currentIndex < secCounts; currentIndex++)
//                    {
//                        pos = 24 + 64 * currentIndex;
//                        if (pos <= len)
//                        {
//                            fs.Position = pos;
//                            code = new string(br.ReadChars(10));//大智慧用10个字节保存代码，一般用6个字节
//                            code = Market + code.Replace("\0", "");
//                            code = code.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
//                            code = code.ToUpper();
//                            for (int icode = 0; icode < codesRename.GetLength(0); icode++)
//                            {
//                                code = code.Replace(codesRename[icode, 0], codesRename[icode, 1]);
//                            }
//                            codes[currentIndex] = code;
//                        }
//                    }
//                    //fs.Close();
//                    msg = "";
//                    return codes;
//                }
//            }
//            catch (Exception e)
//            {
//                msg = e.Message;

//            }
//            return new string[1] { null };

//        }
//        public string[,] GetFields(string dataType)
//        {
//            msg = "";
//            try
//            {
//                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
//                return GetFields(d);
//            }
//            catch
//            {
//                msg = @"输入的参数有误。参数只能是:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" 或者 ";
//                foreach (int currentIndex in Enum.GetValues(typeof(DataTypes)))
//                    msg += " " + currentIndex.ToString();

//                return new string[1, 1] { { null } };
//            }

//        }
//        private string[,] GetFields(DataTypes dataType)
//        {
//            msg = "";
//            try
//            {
//                fileStruct dzhFileStruct = new fileStruct(dataType);
//                string[,] fields = new string[dzhFileStruct.fields.GetLength(0), 3];
//                //fields[0, 0] = "<字段名>"; fields[0, 1] = "<含义>"; fields[0, 2] = "<类型>";
//                for (int currentIndex = 0; currentIndex < dzhFileStruct.fields.GetLength(0); currentIndex++)
//                {
//                    for (int j = 0; j < 3; j++)
//                    {
//                        fields[currentIndex, j] = dzhFileStruct.fields[currentIndex, j];
//                    }
//                }

//                return fields;
//            }
//            catch
//            {
//                msg = "错误"; return new string[1, 1] { { null } };
//            }

//        }

//        public static void SqlGetData(string dataType, string code)
//        {
//            SqlPipe pipe = SqlContext.Pipe;
//            DzhData mdzh = new DzhData();
//            string[,] dataArray = mdzh.GetData(dataType, code);

//            if (mdzh.Error == 0 && dataArray.GetLength(0) > 0)
//            {
//                SqlMetaData[] cols = null;
//                string[,] colname = mdzh.GetFields(dataType);
//                if (mdzh.Error == 0 && colname.GetLength(0) > 0)
//                {
//                    cols = new SqlMetaData[colname.GetLength(0)];
//                    for (int currentIndex = 0; currentIndex < colname.GetLength(0); currentIndex++) { cols[currentIndex] = new SqlMetaData(colname[currentIndex, 0], SqlDbType.NVarChar, 1024); }
//                }
//                else
//                { throw new Exception("错误:" + mdzh.Msg); }
//                SqlDataRecord rec = new SqlDataRecord(cols);
//                pipe.SendResultsStart(rec);
//                for (int currentIndex = 0; currentIndex < dataArray.GetLength(0); currentIndex++)
//                {
//                    for (int j = 0; j < dataArray.GetLength(1); j++)
//                    { rec.SetString(j, dataArray[currentIndex, j]); }
//                    pipe.SendResultsRow(rec);
//                }
//                pipe.SendResultsEnd();
//            }
//            else
//            { throw new Exception("错误:" + mdzh.Msg); }
//        }
//        public static string SqlGetData_hq0_sp(string code)//返回成交价
//        {
//            DzhData mdzh = new DzhData();
//            string[,] dataArray = mdzh.GetData("hq0", code);
//            if (mdzh.Error == 0 && dataArray.GetLength(0) > 0 && dataArray.GetLength(1) >= 8)
//            {
//                return dataArray[0, 7];
//            }
//            else
//            { throw new Exception("错误:" + mdzh.Msg); }
//        }
//        public string[,] GetData(string dataType, string code)
//        {
//            return GetData(dataType, code, "");
//        }
//        public string[,] GetData(string dataType, string code, string newFileName)
//        {
//            try
//            {
//                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
//                return GetData(d, code, newFileName);
//            }
//            catch
//            {
//                msg = @"输入的参数有误。第一个参数只能是:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" 或者 ";
//                foreach (int currentIndex in Enum.GetValues(typeof(DataTypes)))
//                    msg += " " + currentIndex.ToString();

//                return new string[1, 1] { { null } };
//            }
//        }
//        public string[,] GetData2(string dataType, string code, string newFileName)
//        {
//            try
//            {
//                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
//                return GetData(d, code, newFileName);
//            }
//            catch
//            {
//                msg = @"输入的参数有误。第一个参数只能是:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" 或者 ";
//                foreach (int currentIndex in Enum.GetValues(typeof(DataTypes)))
//                    msg += " " + currentIndex.ToString();

//                return new string[1, 1] { { null } };
//            }
//        }
//        private string[,] GetData(DataTypes dataType, string code, string newFileName) //读取数据，重载
//        {
//            if (dataType == DataTypes.bk) return GetBK(code);
//            if (dataType == DataTypes.pj) return GetPJ(code);
//            if (dataType == DataTypes.hqfq) return GetHqfq(dataType, code, newFileName);
//            #region 读取数据前初始化
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(dataType);
//            if (newFileName != "") dzhFileStruct.fileName = newFileName; //如果用户重新指定了文件名
//            code = code.Trim().ToUpper();
//            if (code == "")
//            {
//                msg = @"CODE参数不可为空。请提供证券代码，如SZ000001。";
//                return new string[1, 1] { { null } };
//            }
//            ArrayList recordList = new ArrayList();
//            int intField; float floatField; double doubleField; //string stringField;
//            System.Globalization.CultureInfo cnCultureInfo = new System.Globalization.CultureInfo("zh-CN");
//            string market = code.Substring(0, 2);
//            int recordCounts = 0;
//            short[] blocks = new short[25];
//            long len = -1;
//            long pos = 0;
//            if (this.DzhDataPath == "")
//            {
//                msg = @"无法在注册表中到大智慧数据文件目录，请自行将属性 DzhDataPath设置为有效路径，如c:\dzh\data\。";
//                return new string[1, 1] { { null } };
//            }
//            string DzhFile = dzhDataPath + dzhFileStruct.fileName;
//            DzhFile = DzhFile.ToUpper();
//            if (!File.Exists(DzhFile))
//            {
//                DzhFile = dzhDataPath + market + @"\" + dzhFileStruct.fileName;
//            }
//            msg = "";
//            if (!File.Exists(DzhFile))
//            {
//                msg = dzhFileStruct.fileName + "没有找到！";
//                return new string[1, 1] { { null } };
//            }
//            #endregion
//            if (dzhFileStruct.isIndexDataStruct == true)
//            {
//                #region 处理DAY.DAT等结构（索引/数据）的数据
//                try
//                {
//                    this.checkFileStream(DzhFile);
//                    int secCounts = 0;//文件中证券总数
//                    string code0 = "";
//                    len = fs.Length;
//                    fs.Position = 12;
//                    secCounts = br.ReadInt32();
//                    bool codeRead = false;
//                    for (int currentIndex = 0; currentIndex < secCounts && codeRead == false; currentIndex++)
//                    {
//                        pos = 24 + 64 * currentIndex;
//                        if (pos <= len)
//                        {
//                            fs.Position = pos;
//                            //code0 = new string(br.ReadChars(10));//大智慧用10个字节保存代码，一般用8个字节
//                            code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                            code0 = code0.Replace("\0", "");
//                            code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
//                            if (dzhFileStruct.codeIsLong == false && code == market + code0 || dzhFileStruct.codeIsLong == true && code == code0)
//                            {
//                                recordCounts = br.ReadInt32();
//                                for (int j = 0; j < 25; j++)
//                                {
//                                    blocks[j] = br.ReadInt16();
//                                }
//                                codeRead = true;
//                            }
//                        }
//                    }
//                    int iRecord = 1;//记录
//                    int iBlock = 0;//第iBlock块
//                    int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                    while (iBlock < 25 && blocks[iBlock] != -1)
//                    {
//                        int r = 0;
//                        while (iRecord < recordCounts + 1 && r < dzhFileStruct.blockSize / dzhFileStruct.recordSize)   //16=3776/236
//                        {
//                            string[] record = new string[fieldCounts];
//                            pos = dzhFileStruct.startAddress + blocks[iBlock] * dzhFileStruct.blockSize + r * dzhFileStruct.recordSize;
//                            for (int iField = 0; iField < fieldCounts; iField++)
//                            {
//                                fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                switch (dzhFileStruct.fields[iField, 2].ToLower())
//                                {
//                                    case "code":
//                                        //code0 = new string(br.ReadChars(8));//有12位，实际用了8位，第9-12位一般为\0，有时是错误字节，因为只读8位
//                                        //code0 = code0.Replace("\0", "");
//                                        record[iField] = code;
//                                        break;
//                                    case "date":
//                                        intField = br.ReadInt32();
//                                        record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
//                                        break;
//                                    case "datetime":
//                                        intField = br.ReadInt32();
//                                        record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                        break;
//                                    case "int":
//                                        intField = br.ReadInt32();
//                                        record[iField] = intField.ToString("D", cnCultureInfo);
//                                        break;
//                                    case "single":
//                                        //floatField = br.ReadSingle();
//                                        //if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
//                                        //record[iField] = floatField.ToString("G", cnCultureInfo);
//                                        doubleField = (double)br.ReadSingle();
//                                        if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") doubleField *= 100;
//                                        record[iField] = doubleField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F", cnCultureInfo);
//                                        break;
//                                    case "double":
//                                        doubleField = br.ReadDouble();
//                                        record[iField] = doubleField.ToString("F", cnCultureInfo);
//                                        break;
//                                    case "string":
//                                        record[iField] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(Convert.ToInt32(dzhFileStruct.fields[iField, 3]))).Replace("\0", "");
//                                        break;
//                                }
//                            }


//                            recordList.Add(record);

//                            r = r + 1;
//                            iRecord = iRecord + 1;
//                        }
//                        iBlock = iBlock + 1;
//                    }

//                    //fs.Close();
//                    string[,] records = new string[recordList.Count, fieldCounts];
//                    for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                    {
//                        string[] record0 = (string[])recordList[currentIndex];
//                        for (int j = 0; j < fieldCounts; j++)
//                        {
//                            records[currentIndex, j] = record0[j];
//                        }
//                    }
//                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                    return records;
//                }
//                catch (Exception e)
//                {
//                    msg = e.Message;
//                }
//                #endregion
//            }
//            else
//            {
//                // DzhFile = @"C:\DzhTools\STKINFO60\STKINFO60.DAT";//Xchui
//                //StringBuilder lstr = new StringBuilder();//Xchui
//                switch (dataType)
//                {
//                    case DataTypes.dm:
//                        #region 代码表（处理STKINFO60.DAT等结构的数据）
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            string[,] codesRename = new string[,] 
//                                    {
//                                    {"SH1A0001","SH000001"},
//                                    {"SH1A0002","SH000002"},
//                                    {"SH1A0003","SH000003"},
//                                    {"SH1B0001","SH000004"},
//                                    {"SH1B0002","SH000005"},
//                                    {"SH1B0004","SH000006"},
//                                    {"SH1B0005","SH000007"},
//                                    {"SH1B0006","SH000008"},
//                                    {"SH1B0007","SH000010"},
//                                    {"SH1B0008","SH000011"},
//                                    {"SH1B0009","SH000012"},
//                                    {"SH1B0010","SH000013"},
//                                    {"SH1C0003","SH000016"}         
//                                    };
//                            int secCounts = 0;//文件中证券总数
//                            string code0 = "";
//                            fs.Position = 12;//8
//                            secCounts = br.ReadInt32();
//                            int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                            for (int currentIndex = 0; currentIndex < secCounts; currentIndex++)
//                            {
//                                pos = dzhFileStruct.startAddress + currentIndex * dzhFileStruct.recordSize;
//                                fs.Position = pos;
//                                code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                code0 = code0.Replace("\0", "");
//                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
//                                if (Regex.IsMatch(code0, @"(1[ABC]00\d\d)") == false)
//                                {
//                                    string[] recordFieldName = new string[fieldCounts];
//                                    string[] record = new string[fieldCounts];
//                                    for (int iField = 0; iField < fieldCounts; iField++)
//                                    {
//                                        fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                        switch (dzhFileStruct.fields[iField, 2].ToLower())
//                                        {
//                                            case "code":
//                                                record[iField] = dzhFileStruct.codeIsLong == true ? code0 : market + code0;
//                                                record[iField] = record[iField].Replace("HKHK", "HK");
//                                                for (int icode = 0; icode < codesRename.GetLength(0); icode++)
//                                                {
//                                                    record[iField] = record[iField].Replace(codesRename[icode, 0], codesRename[icode, 1]);
//                                                }
//                                                break;
//                                            case "date":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
//                                                break;
//                                            case "datetime":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                                break;
//                                            case "int":
//                                                intField = br.ReadInt32();
//                                                record[iField] = intField.ToString("D");
//                                                break;
//                                            case "single":
//                                                floatField = br.ReadSingle();
//                                                if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
//                                                record[iField] = floatField.ToString("F");
//                                                break;
//                                            case "double":
//                                                doubleField = br.ReadDouble();
//                                                record[iField] = doubleField.ToString("F");
//                                                break;
//                                            case "string":
//                                                record[iField] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(Convert.ToInt32(dzhFileStruct.fields[iField, 3]))).Replace("\0", "");
//                                                break;
//                                        }

//                                    }
//                                    recordList.Add(record);
//                                }


//                            }

//                            //fs.Close();
//                            string[,] records = new string[recordList.Count, fieldCounts];
//                            for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                            {
//                                string[] record0 = (string[])recordList[currentIndex];
//                                for (int j = 0; j < fieldCounts; j++)
//                                {
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.hq0:
//                        #region 最新行情（处理STKINFO60.DAT等结构的数据）
//                        try
//                        {
//                            this.checkFileStream(DzhFile);

//                            int secCounts = 0;//文件中证券总数
//                            string code0 = "";
//                            fs.Position = 12;
//                            secCounts = br.ReadInt32();
//                            int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                            bool hasCode = false;
//                            for (int currentIndex = 0; currentIndex < secCounts && hasCode == false; currentIndex++)
//                            {
//                                pos = dzhFileStruct.startAddress + currentIndex * dzhFileStruct.recordSize;//Xchui
//                                fs.Position = pos;
//                                code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                code0 = code0.Replace("\0", "");
//                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx

//                                if (dzhFileStruct.codeIsLong == false && code == market + code0 || dzhFileStruct.codeIsLong == true && code == code0)
//                                {
//                                    hasCode = true;
//                                    string[] record = new string[fieldCounts];
//                                    for (int iField = 0; iField < fieldCounts; iField++)
//                                    {
//                                        fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                        switch (dzhFileStruct.fields[iField, 2].ToLower())
//                                        {
//                                            case "code":
//                                                record[iField] = code;
//                                                break;
//                                            case "date":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
//                                                break;
//                                            case "datetime":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                                break;
//                                            case "int":
//                                                intField = br.ReadInt32();
//                                                record[iField] = intField.ToString("D");
//                                                break;
//                                            case "single":
//                                                //floatField = br.ReadSingle();
//                                                //if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
//                                                //record[iField] = Math.Round(floatField, 2).ToString("F");
//                                                doubleField = (double)br.ReadSingle();
//                                                if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") doubleField *= 100;
//                                                record[iField] = doubleField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F", cnCultureInfo);
//                                                break;
//                                            case "double":
//                                                doubleField = br.ReadDouble();
//                                                record[iField] = Math.Round(doubleField, 2).ToString("F");
//                                                break;
//                                            case "string":
//                                                record[iField] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(Convert.ToInt32(dzhFileStruct.fields[iField, 3]))).Replace("\0", "");
//                                                break;
//                                        }

//                                    }
//                                    recordList.Add(record);

//                                }

//                            }

//                            //fs.Close();
//                            string[,] records = new string[recordList.Count, fieldCounts];
//                            for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                            {
//                                string[] record0 = (string[])recordList[currentIndex];
//                                for (int j = 0; j < fieldCounts; j++)
//                                {
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.cq:
//                        #region 分红送配（处理STKINFO60.DAT等结构的数据）
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//文件中证券总数
//                            string code0 = "";
//                            fileStruct dzhdmStruct = new fileStruct(DataTypes.dm);//    代码的结构
//                            int dmpos = 0;
//                            fs.Position = 12;
//                            secCounts = br.ReadInt32();
//                            int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                            bool hasCode = false;
//                            for (int currentIndex = 0; currentIndex < secCounts && hasCode == false; currentIndex++)
//                            {
//                                dmpos = dzhdmStruct.startAddress + currentIndex * dzhdmStruct.recordSize;
//                                fs.Position = dmpos;
//                                code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                code0 = code0.Replace("\0", "");
//                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
//                                if (dzhdmStruct.codeIsLong == false && code == market + code0 || dzhdmStruct.codeIsLong == true && code == code0)
//                                {
//                                    hasCode = true;
//                                    int iRecord = 0;
//                                    pos = dzhFileStruct.startAddress + currentIndex * dzhFileStruct.blockSize + iRecord * dzhFileStruct.recordSize;
//                                    fs.Position = pos;
//                                    while (br.ReadInt32() != 0)
//                                    {
//                                        string[] record = new string[fieldCounts];
//                                        for (int iField = 0; iField < fieldCounts; iField++)
//                                        {
//                                            fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                            switch (dzhFileStruct.fields[iField, 2].ToLower())
//                                            {
//                                                case "code":
//                                                    record[iField] = code;
//                                                    break;
//                                                case "date":
//                                                    intField = br.ReadInt32();
//                                                    record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
//                                                    break;
//                                                case "datetime":
//                                                    intField = br.ReadInt32();
//                                                    record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                                    break;
//                                                case "int":
//                                                    intField = br.ReadInt32();
//                                                    record[iField] = intField.ToString("D");
//                                                    break;
//                                                case "single":
//                                                    floatField = br.ReadSingle();
//                                                    if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
//                                                    record[iField] = Math.Round(floatField, 2).ToString("F");
//                                                    break;
//                                                case "double":
//                                                    doubleField = br.ReadDouble();
//                                                    record[iField] = Math.Round(doubleField, 2).ToString("F");
//                                                    break;
//                                                case "string":
//                                                    record[iField] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(Convert.ToInt32(dzhFileStruct.fields[iField, 3]))).Replace("\0", "");
//                                                    break;
//                                            }

//                                        }
//                                        recordList.Add(record);
//                                        iRecord = iRecord + 1;
//                                        pos = dzhFileStruct.startAddress + currentIndex * dzhFileStruct.blockSize + iRecord * dzhFileStruct.recordSize;
//                                        fs.Position = pos;

//                                    }

//                                }

//                            }

//                            //fs.Close();
//                            string[,] records = new string[recordList.Count, fieldCounts];
//                            for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                            {
//                                string[] record0 = (string[])recordList[currentIndex];
//                                for (int j = 0; j < fieldCounts; j++)
//                                {
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.cw0:
//                        #region 财务数据--简单（处理STKINFO60.DAT等结构的数据）
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//文件中证券总数
//                            string code0 = "";
//                            fileStruct dzhdmStruct = new fileStruct(DataTypes.dm);//    代码的结构
//                            int dmpos = 0;
//                            fs.Position = 12;
//                            secCounts = br.ReadInt32();
//                            int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                            bool hasCode = false;
//                            for (int currentIndex = 0; currentIndex < secCounts && hasCode == false; currentIndex++)
//                            {
//                                dmpos = dzhdmStruct.startAddress + currentIndex * dzhdmStruct.recordSize;
//                                fs.Position = dmpos;
//                                code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                code0 = code0.Replace("\0", "");
//                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx

//                                //if (code0.Length > 0)//Xchui  测试
//                                //{
//                                //    lstr.Append(code0); lstr.AppendLine();
//                                //}
//                                if (dzhdmStruct.codeIsLong == false && code == market + code0 || dzhdmStruct.codeIsLong == true && code == code0)
//                                {
//                                    hasCode = true;
//                                    //int iRecord = 0;
//                                    pos = dzhFileStruct.startAddress + currentIndex * dzhFileStruct.blockSize;// +iRecord * dzhFileStruct.recordSize;
//                                    fs.Position = pos;
//                                    string[] record = new string[fieldCounts];
//                                    for (int iField = 0; iField < fieldCounts; iField++)
//                                    {
//                                        fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                        switch (dzhFileStruct.fields[iField, 2].ToLower())
//                                        {
//                                            case "code":
//                                                record[iField] = code;
//                                                break;
//                                            case "date":
//                                                intField = br.ReadInt32();
//                                                //record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
//                                                record[iField] = intField.ToString().Insert(4, "-").Insert(7, "-");
//                                                break;
//                                            case "datetime":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                                break;
//                                            case "int":
//                                                intField = br.ReadInt32();
//                                                record[iField] = intField.ToString("D");
//                                                break;
//                                            case "single":
//                                                floatField = br.ReadSingle();
//                                                if (dzhFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
//                                                record[iField] = Math.Round(floatField, 2).ToString("F");
//                                                break;
//                                            case "double":
//                                                doubleField = br.ReadDouble();
//                                                record[iField] = Math.Round(doubleField, 2).ToString("F");
//                                                break;
//                                            case "string":
//                                                record[iField] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(Convert.ToInt32(dzhFileStruct.fields[iField, 3]))).Replace("\0", "");
//                                                break;
//                                        }

//                                    }
//                                    recordList.Add(record);
//                                }

//                            }

//                            //fs.Close();
//                            string[,] records = new string[recordList.Count, fieldCounts];
//                            for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                            {
//                                string[] record0 = (string[])recordList[currentIndex];
//                                for (int j = 0; j < fieldCounts; j++)
//                                {
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.hqmb:
//                        #region 处理Report.DAT数据（结构类似DAY.DAT，但有些数值需要进一步计算而来）
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//文件中证券总数
//                            string code0 = "";
//                            len = fs.Length;
//                            fs.Position = 12;
//                            secCounts = br.ReadInt32();
//                            bool codeRead = false;
//                            for (int currentIndex = 0; currentIndex < secCounts && codeRead == false; currentIndex++)
//                            {
//                                pos = 24 + 64 * currentIndex;
//                                if (pos <= len)
//                                {
//                                    fs.Position = pos;
//                                    //code0 = new string(br.ReadChars(10));//大智慧用10个字节保存代码，一般用8个字节
//                                    code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                    int start = code0.IndexOf('\0');
//                                    code0 = code0.Remove(start, code0.Length - start);
//                                    code0 = code0.Replace("\0", "");
//                                    code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
//                                    if (dzhFileStruct.codeIsLong == false && code == market + code0 || dzhFileStruct.codeIsLong == true && code == code0)
//                                    {
//                                        recordCounts = br.ReadInt32();
//                                        for (int j = 0; j < 25; j++)
//                                        {
//                                            blocks[j] = br.ReadInt16();
//                                        }
//                                        codeRead = true;
//                                    }
//                                }
//                            }
//                            int iRecord = 1;//记录
//                            int iBlock = 0;//第iBlock块
//                            int fieldCounts = dzhFileStruct.fields.GetLength(0);
//                            while (iBlock < 25 && blocks[iBlock] != -1)
//                            {
//                                int r = 0;
//                                while (iRecord < recordCounts + 1 && r < dzhFileStruct.blockSize / dzhFileStruct.recordSize)
//                                {
//                                    string[] record = new string[fieldCounts];
//                                    pos = dzhFileStruct.startAddress + blocks[iBlock] * dzhFileStruct.blockSize + r * dzhFileStruct.recordSize;
//                                    for (int iField = 0; iField < fieldCounts; iField++)
//                                    {
//                                        int orival, flag;
//                                        fs.Position = pos + Convert.ToInt64(dzhFileStruct.fields[iField, 5]);
//                                        switch (dzhFileStruct.fields[iField, 0].ToLower()) //这里与读取DAY.DAT用法不同，判断的是代码而不是类型
//                                        {
//                                            case "dm":
//                                                record[iField] = code;
//                                                break;
//                                            case "rq":
//                                                intField = br.ReadInt32();
//                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
//                                                break;
//                                            case "zjcj":
//                                            case "zss":
//                                            case "je":
//                                                floatField = br.ReadSingle();
//                                                record[iField] = floatField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F");
//                                                break;
//                                            case "bs":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 16;
//                                                record[iField] = orival.ToString("D");
//                                                break;

//                                            case "mr1sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 18; flag = br.ReadByte();
//                                                if ((flag & 1) == 1) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mr2sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 18; flag = br.ReadByte();
//                                                if ((flag & 16) == 16) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mr3sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 19; flag = br.ReadByte();
//                                                if ((flag & 1) == 1) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mr4sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 19; flag = br.ReadByte();
//                                                if ((flag & 16) == 16) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mr5sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 20; flag = br.ReadByte();
//                                                if ((flag & 1) == 1) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mc1sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 18; flag = br.ReadByte();
//                                                if ((flag & 2) == 2) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mc2sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 18; flag = br.ReadByte();
//                                                if ((flag & 32) == 32) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mc3sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 19; flag = br.ReadByte();
//                                                if ((flag & 2) == 2) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mc4sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 19; flag = br.ReadByte();
//                                                if ((flag & 32) == 32) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mc5sl":
//                                                orival = br.ReadUInt16();
//                                                fs.Position = pos + 20; flag = br.ReadByte();
//                                                if ((flag & 2) == 2) orival = orival * 32;
//                                                record[iField] = orival.ToString("D");
//                                                break;
//                                            case "mr1jg":
//                                            case "mr2jg":
//                                            case "mr3jg":
//                                            case "mr4jg":
//                                            case "mr5jg":
//                                            case "mc1jg":
//                                            case "mc2jg":
//                                            case "mc3jg":
//                                            case "mc4jg":
//                                            case "mc5jg":
//                                                float jg = br.ReadSByte();
//                                                if ("_jj_qz".IndexOf(this.GetCodeType(code)) > 0)
//                                                {
//                                                    jg = Convert.ToSingle(record[2]) + jg / 1000;
//                                                    record[iField] = jg.ToString("F3");
//                                                }
//                                                else
//                                                {
//                                                    jg = Convert.ToSingle(record[2]) + jg / 100;
//                                                    record[iField] = jg.ToString("F");
//                                                }
//                                                break;
//                                            case "xss":
//                                                record[iField] = "";//现手数在下面计算
//                                                break;
//                                            case "mm":
//                                                int mm = br.ReadSByte();
//                                                record[iField] = "";
//                                                if (mm == -128 || mm == -96) record[iField] = "外盘"; //-128 = 0x80
//                                                if (mm == -64) record[iField] = "内盘";  //-64 = 0xC0
//                                                break;
//                                        }

//                                    }


//                                    recordList.Add(record);

//                                    r = r + 1;
//                                    iRecord = iRecord + 1;
//                                }
//                                iBlock = iBlock + 1;
//                            }

//                            //fs.Close();
//                            float zssSaved = 0;
//                            string[,] records = new string[recordList.Count, fieldCounts];
//                            for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                            {
//                                string[] record0 = (string[])recordList[currentIndex];
//                                for (int j = 0; j < fieldCounts; j++)
//                                {
//                                    if (j == 5)  //现手数
//                                    {
//                                        record0[j] = (Convert.ToSingle(record0[3]) - zssSaved).ToString();
//                                        zssSaved = Convert.ToSingle(record0[3]);
//                                    }
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                }

//            }
//            msg = "返回空数组。";
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetBK(string code)//板块定义数据
//        {
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(DataTypes.bk);
//            if (code == null) code = "";
//            code = code.Trim().ToUpper();
//            ArrayList recordList = new ArrayList();
//            if (this.DzhDataPath == "")
//            {
//                msg = @"无法在注册表中到大智慧数据文件目录，请自行将属性 DzhDataPath设置为有效路径，如c:\dzh\data\。";
//                return new string[1, 1] { { null } };
//            }
//            string DzhBlockPath = dzhDataPath;
//            DzhBlockPath = DzhBlockPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\BLOCK\\"); //假设目录中含有data文字
//            string DzhFile = DzhBlockPath + dzhFileStruct.fileName;

//            msg = "";
//            if (!File.Exists(DzhFile))
//            {
//                msg = "板块文件无法找到。";
//                return new string[1, 1] { { null } };
//            }
//            try
//            {
//                this.checkFileStream(DzhFile);
//                string bklines = ""; string lb = ""; string bk = "";
//                string bkFile = ""; string dmLines = ""; int n = -1;
//                bklines = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes((int)fs.Length));
//                string[] bks = bklines.Replace("\r\n", "\n").Split(new Char[] { '\n' });
//                for (int currentIndex = 0; currentIndex < bks.Length; currentIndex++)
//                {
//                    if (bks[currentIndex] != "")
//                    {
//                        bks[currentIndex] = bks[currentIndex].Trim();
//                        if (bks[currentIndex].StartsWith("[") && bks[currentIndex].EndsWith("]"))
//                        {
//                            lb = bks[currentIndex].Replace("[", "").Replace("]", "");
//                        }
//                        else
//                        {
//                            bk = bks[currentIndex];
//                            if (bk != "")
//                            {
//                                if (code == "" || (code != "" && bk.ToUpper() == code))
//                                {
//                                    bkFile = DzhBlockPath + bk + ".blk";
//                                    if (File.Exists(bkFile))
//                                    {
//                                        StreamReader bkReader = new StreamReader(bkFile);
//                                        bkReader.Read(); bkReader.Read();
//                                        dmLines = bkReader.ReadToEnd();
//                                        dmLines = dmLines.Replace("\x05", "\0").Replace("\0Z00", "\0\0\0\0").Replace("\0\0\0\0", ",");
//                                        string[] dms = dmLines.Split(',');
//                                        string[,] record = new string[dms.Length, 3];
//                                        for (int r = 0; r < dms.Length; r++)
//                                        {
//                                            if (dms[r] != "")
//                                            {
//                                                n = n + 1;
//                                                record[r, 0] = lb;
//                                                record[r, 1] = bk;
//                                                record[r, 2] = dms[r];
//                                            }
//                                        }
//                                        recordList.Add(record);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                //fs.Close();
//                if (n > 0)
//                {
//                    string[,] records = new string[n + 1, 3];
//                    int rr = 0;
//                    for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                    {
//                        string[,] record0 = (string[,])recordList[currentIndex];
//                        for (int j = 0; j < record0.GetLength(0); j++)
//                        {
//                            if (record0[j, 0] != null && record0[j, 1] != null && record0[j, 2] != null)
//                            {
//                                records[rr, 0] = record0[j, 0];
//                                records[rr, 1] = record0[j, 1];
//                                records[rr, 2] = record0[j, 2];
//                                rr = rr + 1;
//                            }
//                        }
//                    }
//                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                    return records;
//                }
//            }
//            catch (Exception e)
//            {
//                msg = e.Message;
//            }
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetPJ(string code)//评级数据
//        {
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(DataTypes.pj);
//            code = code.Trim().ToUpper();
//            ArrayList recordList = new ArrayList();
//            if (this.DzhDataPath == "")
//            {
//                msg = @"无法在注册表中到大智慧数据文件目录，请自行将属性 DzhDataPath设置为有效路径，如c:\dzh\data\。";
//                return new string[1, 1] { { null } };
//            }
//            string dzhSubPath = dzhDataPath;
//            dzhSubPath = dzhSubPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\SelfData\\"); //假设目录中含有data文字
//            string DzhFile = dzhSubPath + dzhFileStruct.fileName;

//            msg = "";
//            if (!File.Exists(DzhFile))
//            {
//                msg = dzhFileStruct.fileName + "无法找到。";
//                return new string[1, 1] { { null } };
//            }
//            try
//            {
//                this.checkFileStream(DzhFile);
//                int n = 0;
//                int pos = dzhFileStruct.startAddress + n * dzhFileStruct.recordSize;
//                fs.Position = pos;
//                while (br.PeekChar() != -1)
//                {
//                    string[] record = new string[3];
//                    pos = dzhFileStruct.startAddress + n * dzhFileStruct.recordSize;
//                    fs.Position = pos;
//                    record[0] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(8));//dm 
//                    if (code == "" || (code != "" && code == record[0]))
//                    {
//                        fs.Position = pos + 12;
//                        record[2] = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(244));
//                        record[1] = record[2].Substring(0, 2).Trim();
//                        record[2] = record[2].Replace("\0", "").Trim();
//                        if (record[0] != "") recordList.Add(record);
//                    }
//                    n = n + 1;
//                }
//                //fs.Close();
//                if (n > 0)
//                {
//                    string[,] records = new string[recordList.Count, 3];
//                    for (int currentIndex = 0; currentIndex < recordList.Count; currentIndex++)
//                    {
//                        string[] record0 = (string[])recordList[currentIndex];
//                        if (record0[0] != null)
//                        {
//                            records[currentIndex, 0] = record0[0];
//                            records[currentIndex, 1] = record0[1];
//                            records[currentIndex, 2] = record0[2];
//                        }
//                    }
//                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
//                    return records;
//                }
//            }
//            catch (Exception e)
//            {
//                msg = e.Message;
//            }
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetHqfq(DataTypes dataType, string code, string newFileName)//复权价格,分红再投资,向前复权法
//        {
//            DzhData dzh = new DzhData();
//            string[,] hq = dzh.GetData("hq", code, newFileName);
//            if (dzh.Error != 0 || hq.GetLength(1) < 4) return new string[1, 1] { { null } };
//            string[,] x = new string[hq.GetLength(0), 9];
//            string[,] cq = dzh.GetData("cq", code, newFileName);
//            string fmt = "_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F";
//            if (dzh.Error != 0 || cq.GetLength(1) < 4 || cq.GetLength(0) == 0) //没有除权信息
//            {
//                for (int currentIndex = 0; currentIndex < hq.GetLength(0); currentIndex++)
//                {
//                    for (int j = 0; j < hq.GetLength(1); j++)
//                    {
//                        x[currentIndex, j] = hq[currentIndex, j];
//                    }
//                    if (currentIndex == 0)
//                    {
//                        x[currentIndex, hq.GetLength(1)] = "0.00000";
//                    }
//                    else
//                    {
//                        x[currentIndex, hq.GetLength(1)] = (Single.Parse(hq[currentIndex, 5]) / Single.Parse(hq[currentIndex - 1, 5]) - 1).ToString("0.00000");
//                    }
//                }
//            }
//            else  //有除权信息
//            {
//                DateTime[] cqdt = new DateTime[cq.GetLength(0)];
//                for (int j = 0; j < cq.GetLength(0); j++) cqdt[j] = new DateTime(int.Parse(cq[j, 1].Split('-')[0]), int.Parse(cq[j, 1].Split('-')[1]), int.Parse(cq[j, 1].Split('-')[2]));
//                int i0 = hq.GetLength(0) - 1;
//                DateTime hqdt_1, hqdt;
//                double kp_1, zg_1, zd_1, sp_1, kp, zg, zd, sp, kpx, zgx, zdx, spx, sgbl, kpsyl, zgsyl, zdsyl, spsyl, pgbl, pgjg, fh;
//                for (int k = 0; k < 8; k++) x[i0, k] = hq[i0, k];  //最后一条记录
//                x[0, 8] = "0.00000";
//                kpx = double.Parse(x[i0, 2]);
//                zgx = double.Parse(x[i0, 3]);
//                zdx = double.Parse(x[i0, 4]);
//                spx = double.Parse(x[i0, 5]);
//                for (int currentIndex = i0; currentIndex > 0; currentIndex--)
//                {
//                    sgbl = 0; pgbl = 0; pgjg = 0; fh = 0;
//                    hqdt_1 = new DateTime(int.Parse(hq[currentIndex - 1, 1].Split('-')[0]), int.Parse(hq[currentIndex - 1, 1].Split('-')[1]), int.Parse(hq[currentIndex - 1, 1].Split('-')[2]));
//                    hqdt = new DateTime(int.Parse(hq[currentIndex, 1].Split('-')[0]), int.Parse(hq[currentIndex, 1].Split('-')[1]), int.Parse(hq[currentIndex, 1].Split('-')[2]));
//                    for (int j = 0; j < cq.GetLength(0); j++)
//                    {
//                        if (hqdt_1 < cqdt[j] && cqdt[j] <= hqdt)
//                        {
//                            sgbl = double.Parse(cq[j, 2]);
//                            pgbl = double.Parse(cq[j, 3]);
//                            pgjg = double.Parse(cq[j, 4]);
//                            fh = double.Parse(cq[j, 5]);
//                        }
//                    }
//                    x[currentIndex - 1, 0] = hq[currentIndex - 1, 0];//dm
//                    x[currentIndex - 1, 1] = hq[currentIndex - 1, 1];//rq
//                    //syl=1+第t日收益率 =( t日收盘价*(1+送股比例+配股比例)+分红金额-配股价格*配股比例)/(t-1日收盘价)
//                    kp = double.Parse(hq[currentIndex, 2]);
//                    zg = double.Parse(hq[currentIndex, 3]);
//                    zd = double.Parse(hq[currentIndex, 4]);
//                    sp = double.Parse(hq[currentIndex, 5]);
//                    kp_1 = double.Parse(hq[currentIndex - 1, 2]);
//                    zg_1 = double.Parse(hq[currentIndex - 1, 3]);
//                    zd_1 = double.Parse(hq[currentIndex - 1, 4]);
//                    sp_1 = double.Parse(hq[currentIndex - 1, 5]);
//                    kpsyl = (kp * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / kp_1;
//                    zgsyl = (zg * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / zg_1;
//                    zdsyl = (zd * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / zd_1;
//                    spsyl = (sp * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / sp_1;
//                    kpx = kpx / kpsyl;
//                    zgx = zgx / zgsyl;
//                    zdx = zdx / zdsyl;
//                    spx = spx / spsyl;
//                    x[currentIndex - 1, 2] = kpx.ToString(fmt);
//                    x[currentIndex - 1, 3] = zgx.ToString(fmt);
//                    x[currentIndex - 1, 4] = zdx.ToString(fmt);
//                    x[currentIndex - 1, 5] = spx.ToString(fmt);
//                    x[currentIndex - 1, 6] = hq[currentIndex - 1, 6];//sl 成交量未复权
//                    x[currentIndex - 1, 7] = hq[currentIndex - 1, 7];//je
//                    x[currentIndex, 8] = (spsyl - 1).ToString("0.00000");//spsyl 收盘价收益率

//                }

//            }

//            return x;

//        }
//    }
}

