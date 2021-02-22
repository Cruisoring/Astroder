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
//            #region ����
//            {"dm","����",""},
//            {"hq","����",""},
//            {"hqmb","�ֱʳɽ�",""},
//            {"hq0","��̬����",""},
//            {"hq1","һ��������(N/A)",""},
//            {"hq5","���������<(N/A)",""},
//            {"cq","��Ȩ����",""},
//            {"cw0","���²�������",""},
//            {"fp","�ֺ�����(N/A)",""},
//            {"gb","�ɱ��ṹ(N/A)",""},
//            {"gd","ʮ��ɶ�(N/A)",""},
//            {"cw","��������(N/A)",""},
//            {"jjjz","����ֵ(N/A)",""},
//            {"jjzh","����Ͷ�����(N/A)",""},
//            {"bk","���",""},
//            {"pj","����(N/A)",""},
//            {"hqfq","��Ȩ����",""} 

//        };//��˳����Datatypeһ�£��зֱ�Ϊ������������������Ӧ�ļ�����GetTables�����и�ֵ��
//            #endregion
//        public DzhData()
//        {
//            try
//            {
//                //��ע����ж�ȡ���ǻ�����Ŀ¼����c:\dzh\data
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
//                        msg = "û���ҵ����ǻ۰�װ��Ϣ��";
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
//            public string fileName;//�ļ���
//            public int startAddress, blockSize, recordSize;//��ʼ��ַ��ÿ�鳤�ȣ���¼����
//            public bool codeIsLong, isIndexDataStruct;   //codeIsLong�����еĴ���������г�����SH��SZ��;isIndexDataStruct��Day.Dat�����Ľṹ��������+�������; 
//            public string[,] fields;//�ֶ�
//            public fileStruct(DataTypes fileType)
//            {
//                fileName = "";
//                startAddress = 0;
//                blockSize = 0;
//                recordSize = 0;
//                codeIsLong = false;
//                isIndexDataStruct = true;
//                string fieldString = ""; //�ֶ������ֶα�ǩ�����ͣ������ֶΣ��洢˳��ƫ����
//                switch (fileType)
//                {

//                    #region �����STKINFO60.dat
//                    case DataTypes.dm:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x6D0226;//0x6D0226;//0x68A8A6;Xchui
//                        blockSize = 0;
//                        recordSize = 273;//fxj248
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"jc,���,string,32,1,10,";
//                        break;
//                    #endregion
//                    #region �ֺ�����STKINFO60.dat
//                    case DataTypes.cq:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x44aa;
//                        blockSize = 2227;
//                        recordSize = 20;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,date,4,0,0,;" +
//"sgbl,�͹ɱ���,single,4,1,4,;" +
//"pgbl,��ɱ���,single,4,2,8,;" +
//"pgjg,��ɼ۸�,single,4,3,12,;" +
//"fh,�ֺ�,single,4,4,16,";
//                        break;
//                    #endregion
//                    #region �������ݣ��򵥣�STKINFO60.dat
//                    case DataTypes.cw0:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x4c2a;//0x4A5AA;//0x4c2a;Xchui
//                        blockSize = 2227;
//                        recordSize = 273;//196
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,������,date,4,0,4,;" +
//"gxrq,��������,date,4,0,0,;" +
//"ssrq,��������,date,4,0,8,;" +
//"col1,ÿ������,single,4,0,12,;" +
//"col2,ÿ�ɾ��ʲ�,single,4,0,16,;" +
//"col3,���ʲ�������,single,4,0,20,;" +
//"col4,ÿ�ɾ�Ӫ�ֽ�,single,4,0,24,;" +
//"col5,ÿ�ɹ�����,single,4,0,28,;" +
//"col6,ÿ��δ����,single,4,0,32,;" +
//"col7,�ɶ�Ȩ���,single,4,0,36,;" +
//"col8,������ͬ��,single,4,0,40,;" +
//"col9,��Ӫ����ͬ��,single,4,0,44,;" +
//"col10,����ë����,single,4,0,48,;" +
//"col11,����ÿ�ɾ��ʲ�,single,4,0,52,;" +
//"col12,���ʲ�,single,4,0,56,;" +
//"col13,�����ʲ�,single,4,0,60,;" +
//"col14,�̶��ʲ�,single,4,0,64,;" +
//"col15,�����ʲ�,single,4,0,68,;" +
//"col16,������ծ,single,4,0,72,;" +
//"col17,���ڸ�ծ,single,4,0,76,;" +
//"col18,�ܸ�ծ,single,4,0,80,;" +
//"col19,�ɶ�Ȩ��,single,4,0,84,;" +
//"col20,�ʱ�������,single,4,0,88,;" +
//"col21,��Ӫ�ֽ�����,single,4,0,92,;" +
//"col22,Ͷ���ֽ�����,single,4,0,96,;" +
//"col23,�����ֽ�����,single,4,0,100,;" +
//"col24,�ֽ����Ӷ�,single,4,0,104,;" +
//"col25,��Ӫ����,single,4,0,108,;" +
//"col26,��Ӫ����,single,4,0,112,;" +
//"col27,Ӫҵ����,single,4,0,116,;" +
//"col28,Ͷ������,single,4,0,120,;" +
//"col29,Ӫҵ����֧,single,4,0,124,;" +
//"col30,�����ܶ�,single,4,0,128,;" +
//"col31,������,single,4,0,132,;" +
//"col32,δ��������,single,4,0,136,;" +
//"col33,�ܹɱ�,single,4,0,140,;" +
//"col34,�����۹ɺϼ�,single,4,0,144,;" +
//"col35,A��,single,4,0,148,;" +
//"col36,B��,single,4,0,152,;" +
//"col37,�������й�,single,4,0,156,;" +
//"col38,������ͨ��,single,4,0,160,;" +
//"col39,���۹ɺϼ�,single,4,0,164,;" +
//"col40,���ҳֹ�,single,4,0,168,;" +
//"col41,���з��˹�,single,4,0,172,;" +
//"col42,���ڷ��˹�,single,4,0,176,;" +
//"col43,������Ȼ�˹�,single,4,0,180,;" +
//"col44,���������˹�,single,4,0,184,;" +
//"col45,ļ�����˹�,single,4,0,188,;" +
//"col46,���ⷨ�˹�,single,4,0,192,;" +
//"col47,������Ȼ�˹�,single,4,0,196,;" +
//"col48,���ȹɻ�����,single,4,0,200,";
//                        break;
//                    #endregion
//                    #region ��������STKINFO60.dat
//                    case DataTypes.hq0:
//                        fileName = "STKINFO60.DAT";
//                        startAddress = 0x6D0226;//0x6D0226;//0x68A8A6;Xchui
//                        blockSize = 0;
//                        recordSize = 273;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"jc,���,string,32,1,10,;" +
//"rq,����ʱ��,datetime,4,5,60,;" +
//"zs,����,single,4,7,68,;" +
//"kp,��,single,4,8,72,;" +
//"zg,���,single,4,9,76,;" +
//"zd,���,single,4,10,80,;" +
//"sp,����,single,4,11,84,;" +
//"sl,������,single,4,12,88,;" +
//"je,���,single,4,13,92,;" +
//"xss,������,single,4,14,96,;" +
//"ztj,��ͣ��,single,4,27,184,;" +
//"dtj,��ͣ��,single,4,28,188,;" +
//"np,����,single,4,27,192,;" +
//"wp,����,single,4,28,196,;" +
//"mrjg1,��һ��,single,4,15,100,;" +
//"mrsl1,��һ��,single,4,18,120,;" +
//"mrjg2,�����,single,4,16,104,;" +
//"mrsl2,�����,single,4,19,124,;" +
//"mrjg3,������,single,4,17,108,;" +
//"mrsl3,������,single,4,20,128,;" +
//"mrjg4,���ļ�,single,4,32,112,;" +
//"mrsl4,������,single,4,34,132,;" +
//"mrjg5,�����,single,4,33,116,;" +
//"mrsl5,������,single,4,35,136,;" +
//"mcjg1,��һ��,single,4,21,140,;" +
//"mcsl1,��һ��,single,4,24,160,;" +
//"mcjg2,������,single,4,22,144,;" +
//"mcsl2,������,single,4,25,164,;" +
//"mcjg3,������,single,4,23,148,;" +
//"mcsl3,������,single,4,26,168,;" +
//"mcjg4,���ļ�,single,4,36,152,;" +
//"mcsl4,������,single,4,38,172,;" +
//"mcjg5,�����,single,4,37,156,;" +
//"mcsl5,������,single,4,39,176,";
//                        //"jd,����,int,4,3,52,;" +
//                        //"scbz,ɾ����־,int,4,4,56,";
//                        //"unknown,(δ֪),int,4,31,164,;" +
//                        //",(δ֪),,48,40,200,;"
//                        break;
//                    #endregion
//                    #region �ֱʳɽ������ļ�report.dat���ṹͬday.dat��������һЩ���ݲ���ֱ�ӱ��棩
//                    case DataTypes.hqmb:
//                        fileName = "REPORT.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 12272;
//                        recordSize = 52;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;//����ȫ��ͬ��day.dat�ṹ����˵�������
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,datetime,4,0,0,;" +
//"zjcj,����ɽ���,single,4,1,4,;" +
//"zss,������,single,4,2,8,calc;" +
//"je,���,single,4,3,12,;" +
//"xss,������,single,4,2,8,;" +
//"mm,������,string,2,16,21,;" +
//"mr1jg,��һ��,single,1,10,42,;" +
//"mr1sl,��һ��,single,2,4,22,;" +
//"mr2jg,�����,single,1,11,43,;" +
//"mr2sl,�����,single,2,5,24,;" +
//"mr3jg,������,single,1,12,44,;" +
//"mr3sl,������,single,2,6,26,;" +
//"mr4jg,���ļ�,single,1,12,45,;" +
//"mr4sl,������,single,2,6,28,;" +
//"mr5jg,�����,single,1,12,46,;" +
//"mr5sl,������,single,2,6,30,;" +
//"mc1jg,��һ��,single,1,13,47,;" +
//"mc1sl,��һ��,single,2,7,32,;" +
//"mc2jg,������,single,1,14,48,;" +
//"mc2sl,������,single,2,8,34,;" +
//"mc3jg,������,single,1,15,49,;" +
//"mc3sl,������,single,2,9,36,;" +
//"mc4jg,���ļ�,single,1,14,50,;" +
//"mc4sl,������,single,2,8,38,;" +
//"mc5jg,�����,single,1,14,51,;" +
//"mc5sl,������,single,2,8,40,;" +
//"bs,�ܱ���,int,2,0,16,"
//;
//                        //�����������Ͳ��Ǵ洢���ͣ������в�ֱ����ʵ���������ͣ���/��X��Ϊshort����/��X��Ϊbyte
//                        //������ͨ����������������ã�Ӧ�÷�������������
//                        break;
//                    #endregion
//                    #region ���������ļ�day.dat
//                    case DataTypes.hq:
//                        fileName = "DAY.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,date,4,1,0,;" +
//"kp,����,single,4,2,4,B;" +
//"zg,���,single,4,3,8,B;" +
//"zd,���,single,4,4,12,B;" +
//"sp,����,single,4,5,16,B;" +
//"sl,�ɽ�����,single,4,6,20,A;" +
//"je,�ɽ����,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region 1���������ļ�min1.dat
//                    case DataTypes.hq1:
//                        fileName = "MIN1.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 12288;//8192
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,datetime,4,1,0,;" +
//"kp,����,single,4,2,4,B;" +
//"zg,���,single,4,3,8,B;" +
//"zd,���,single,4,4,12,B;" +
//"sp,����,single,4,5,16,B;" +
//"sl,�ɽ�����,single,4,6,20,A;" +
//"je,�ɽ����,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region 5���������ļ�min.dat
//                    case DataTypes.hq5:
//                        fileName = "MIN.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,datetime,4,1,0,;" +
//"kp,����,single,4,2,4,B;" +
//"zg,���,single,4,3,8,B;" +
//"zd,���,single,4,4,12,B;" +
//"sp,����,single,4,5,16,B;" +
//"sl,�ɽ�����,single,4,6,20,A;" +
//"je,�ɽ����,single,4,7,24,";
//                        break;
//                    #endregion
//                    #region �ֺ����������ļ�exprof.fdt
//                    case DataTypes.fp:
//                        fileName = "EXPROF.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 3776;
//                        recordSize = 236;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0,;" +
//"cqrq,��Ȩ����,date,4,23,176,;" +
//"sgbl,�͹ɱ���,double,8,1,12,;" +
//"sgdjr,�͹ɹ�Ȩ�Ǽ���,date,4,2,20,;" +
//"sgcqr,�͹ɳ�Ȩ��,date,4,3,24,;" +
//"sgssr,���������,date,4,4,28,;" +
//"zzbl,ת������,double,8,5,32,;" +
//"zzdjr,ת����Ȩ�Ǽ���,date,4,6,40,;" +
//"zzcqr,ת����Ȩ��,date,4,7,44,;" +
//"zzssr,ת��������,date,4,8,48,;" +
//"fhbl,�ֺ����,double,8,9,52,;" +
//"fhdjr,�ֺ��Ȩ�Ǽ���,date,4,10,60,;" +
//"fhcxr,�ֺ��Ϣ��,date,4,11,64,;" +
//"fhpxr,�ֺ���Ϣ��,date,4,12,68,;" +
//"pgbl,��ɱ���,double,8,13,72,;" +
//"pgdjr,��ɹ�Ȩ�Ǽ���,date,4,14,80,;" +
//"pgcqr,��ɳ�Ȩ��׼��,date,4,15,84,;" +
//"pgjkqsr,��ɽɿ���ʼ��,date,4,16,88,;" +
//"pgjkzzr,��ɽɿ���ֹ��,date,4,17,92,;" +
//"pgssr,��ɿ���ͨ������,date,4,18,96,;" +
//"pgjg,��ɼ۸�,single,4,19,100,;" +
//"frgpgbl,���ڹ����÷��˹���ɱ���,double,8,20,104,;" +
//"frgmgzrf,�Ϲ����˹����ÿ��ת�÷�,single,4,21,112,;" +
//"pgzcxs,�����������,string,60,22,116,;" +
//"bgrq,��������,date,4,24,180,;" +
//"dshrq,���»�����,date,4,25,184,;" +
//"gdhrq,�ɶ�������,date,4,26,188,;" +
//"fhggrq,�ֺ칫������,date,4,27,192,;" +
//"zgbjs,�ܹɱ�����,double,8,28,196,;" +
//"sgsl,�͹�����,double,8,29,204,;" +
//"zzsl,ת������,double,8,30,212,;" +
//"sjpgs,ʵ���������,double,8,31,220,;" +
//"cqhzgb,��Ȩ���ܹɱ�,double,8,32,228";

//                        break;
//                    #endregion
//                    #region �ɱ��ṹCapital.fdt
//                    case DataTypes.gb:
//                        fileName = "CAPITAL.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 3488;
//                        recordSize = 218;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0;" +
//"rq,����,date,4,17,214;" +
//"zgb,�ܹɱ�,double,8,1,12;" +
//"gjg,���ҹ�,double,8,2,20;" +
//"fqrg,�����˹�,double,8,3,28;" +
//"frg,���˹�,double,8,4,36;" +
//"ybfrps,һ�㷨������,double,8,5,44;" +
//"zgg,�ڲ�ְ����,double,8,6,52;" +
//"a,��ͨA��,double,8,7,60;" +
//"zltzag,ս��Ͷ��A��,double,8,8,68;" +
//"zpg,ת���,double,8,9,76;" +
//"jjps,��������,double,8,10,84;" +
//"h,H��,double,8,11,92;" +
//"b,B��,double,8,12,100;" +
//"yxg,���ȹ�,double,8,13,108;" +
//"ggcg,�߼�������Ա�ֹ�,double,8,14,116;" +
//"gbbdyy,�ɱ��䶯ԭ��,string,56,15,124;" +
//"gbbdyylb,�ɱ��䶯ԭ�����,string,34,16,180";

//                        break;
//                    #endregion
//                    #region ��������Finance.fdt
//                    case DataTypes.cw:
//                        fileName = "FINANCE.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 14848;
//                        recordSize = 464;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0,;" +
//"rq,����,date,4,,460,;" +
//"bsdqtzje,����Ͷ�ʾ���,double,8,1,12,;" +
//"bsyszkje,Ӧ���ʿ��,double,8,2,20,;" +
//"bschje,�������,double,8,3,28,;" +
//"bsldzc,�����ʲ�,double,8,4,36,;" +
//"bscqtzje,����Ͷ�ʾ���,double,8,5,44,;" +
//"bsgdzc,�̶��ʲ�,double,8,6,52,;" +
//"bswxzc,���μ������ʲ�,double,8,7,60,;" +
//"bszzc,���ʲ�,double,8,8,68,;" +
//"bsdqjk,���ڽ��,double,8,9,76,;" +
//"bsyfzk,Ӧ���ʿ�,double,8,10,84,;" +
//"bsldfz,������ծ,double,8,11,92,;" +
//"bscqfz,���ڸ�ծ,double,8,12,100,;" +
//"bsfz,��ծ�ϼ�,double,8,13,108,;" +
//"bsgb,�ɱ�,double,8,14,116,;" +
//"bsssgdqy,�����ɶ�Ȩ��,double,8,15,124,;" +
//"bsgdqy,�ɶ�Ȩ��,double,8,16,132,;" +
//"bszbgj,�ʱ�����,double,8,17,140,;" +
//"bsyygj,ӯ�๫��,double,8,18,148,;" +
//"iszysr,��Ӫҵ�����뾻��,double,8,1,156,;" +
//"iszycb,��Ӫҵ��ɱ�,double,8,2,164,;" +
//"iszylr,��Ӫҵ������,double,8,3,172,;" +
//"isqtlr,����ҵ������,double,8,4,180,;" +
//"isyyfy,Ӫҵ����,double,8,5,188,;" +
//"isglfy,�������,double,8,6,196,;" +
//"iscwfy,�������,double,8,7,204,;" +
//"istzsy,Ͷ������,double,8,8,212,;" +
//"islrze,�����ܶ�,double,8,9,220,;" +
//"issds,����˰,double,8,10,228,;" +
//"isjlr,������,double,8,11,236,;" +
//"iskchjlr,�۳������������ľ�����,double,8,12,244,;" +
//"iswfplr,δ��������,double,8,13,252,;" +
//"cfjyhdxjlr,��Ӫ��ֽ�����,double,8,1,260,;" +
//"cfjyhdxjlc,��Ӫ��ֽ�����,double,8,2,268,;" +
//"cfjyhdxjje,��Ӫ��ֽ𾻶�,double,8,3,276,;" +
//"cftzxjlr,Ͷ���ֽ�����,double,8,4,284,;" +
//"cftzxjlc,Ͷ���ֽ�����,double,8,5,292,;" +
//"cftzxjje,Ͷ���ֽ𾻶�,double,8,6,300,;" +
//"cfczxjlr,����ֽ�����,double,8,7,308,;" +
//"cfczxjlc,����ֽ�����,double,8,8,316,;" +
//"cfczxjje,����ֽ𾻶�,double,8,9,324,;" +
//"cdzhjze,�ֽ��ֽ�ȼ��ﾻ����,double,8,10,332,;" +
//"cfxsspxj,������Ʒ�յ����ֽ�,double,8,11,340,;" +
//"mgsy,ÿ������,single,4,1,348,;" +
//"mgjzc,ÿ�ɾ��ʲ�,single,4,2,352,;" +
//"tzmgjzc,������ÿ�ɾ��ʲ�,single,4,3,356,;" +
//"mgzbgjj,ÿ���ʱ�������,single,4,4,360,;" +
//"mgwfplr,ÿ��δ��������,single,4,5,364,;" +
//"mgjyxjllje,ÿ�ɾ�Ӫ��������ֽ���������,single,4,6,368,;" +
//"mgxjzjje,ÿ���ֽ��ֽ�ȼ������Ӿ���,single,4,7,372,;" +
//"mll,ë����,single,4,8,376,;" +
//"zyywlrl,��Ӫҵ��������,single,4,9,380,;" +
//"jll,������,single,4,10,384,;" +
//"zzcbcl,���ʲ�������,single,4,11,388,;" +
//"jzcsyl,���ʲ�������,single,4,12,392,;" +
//"xsxjzb,������Ʒ�յ����ֽ�ռ��Ӫ�������,single,4,13,396,;" +
//"yszczzl,Ӧ���ʿ���ת��,single,4,14,400,;" +
//"chzzl,�����ת��,single,4,15,404,;" +
//"gdzczzl,�̶��ʲ���ת��,single,4,16,408,;" +
//"zyywzzl,��Ӫҵ��������,single,4,17,412,;" +
//"jlrzzl,������������,single,4,18,416,;" +
//"zzczzl,���ʲ�������,single,4,19,420,;" +
//"jzczzl,���ʲ�������,single,4,20,424,;" +
//"ldbl,��������,single,4,21,428,;" +
//"sdbl,�ٶ�����,single,4,22,432,;" +
//"zcfzbl,�ʲ���ծ����,single,4,23,436,;" +
//"fzbl,��ծ����,single,4,24,440,;" +
//"gdqybl,�ɶ�Ȩ�����,single,4,25,444,;" +
//"gdzcbl,�̶��ʲ�����,single,4,26,448,;" +
//"kchmgjlr,�۳������������ÿ�ɾ�����,single,4,27,452,";

//                        break;
//                    #endregion
//                    #region ʮ��ɶ�stkhold.fdt
//                    case DataTypes.gd:
//                        fileName = "STKHOLD.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 17568;
//                        recordSize = 2196;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0,;" +
//"rq,����,date,4,66,2192,;" +
//"gd1mc,�ɶ�1����,string,160,1,12,;" +
//"gd1cgsl,�ɶ�1�ֹ�����,double,8,2,172,;" +
//"gd1cgbl,�ɶ�1�ֹɱ���,single,4,3,180,;" +
//"gd1bz,�ɶ�1��ע,string,20,4,184,;" +
//"gd1fr,�ɶ�1����,string,8,5,204,;" +
//"gd1jyfw,�ɶ�1��Ӫ��Χ,string,16,6,212,;" +
//"gd2mc,�ɶ�2����,string,160,7,228,;" +
//"gd2cgsl,�ɶ�2�ֹ�����,double,8,8,388,;" +
//"gd2cgbl,�ɶ�2�ֹɱ���,single,4,9,396,;" +
//"gd2bz,�ɶ�2��ע,string,20,10,400,;" +
//"gd2fr,�ɶ�2����,string,8,11,420,;" +
//"gd2jyfw,�ɶ�2��Ӫ��Χ,string,16,12,428,;" +
//"gd3mc,�ɶ�3����,string,160,13,444,;" +
//"gd3cgsl,�ɶ�3�ֹ�����,double,8,14,604,;" +
//"gd3cgbl,�ɶ�3�ֹɱ���,single,4,15,612,;" +
//"gd3bz,�ɶ�3��ע,string,20,16,616,;" +
//"gd3fr,�ɶ�3����,string,8,17,636,;" +
//"gd3jyfw,�ɶ�3��Ӫ��Χ,string,16,18,644,;" +
//"gd4mc,�ɶ�4����,string,160,19,660,;" +
//"gd4cgsl,�ɶ�4�ֹ�����,double,8,20,820,;" +
//"gd4cgbl,�ɶ�4�ֹɱ���,single,4,21,828,;" +
//"gd4bz,�ɶ�4��ע,string,20,22,832,;" +
//"gd4fr,�ɶ�4����,string,8,23,852,;" +
//"gd4jyfw,�ɶ�4��Ӫ��Χ,string,16,24,860,;" +
//"gd5mc,�ɶ�5����,string,160,25,876,;" +
//"gd5cgsl,�ɶ�5�ֹ�����,double,8,26,1036,;" +
//"gd5cgbl,�ɶ�5�ֹɱ���,single,4,27,1044,;" +
//"gd5bz,�ɶ�5��ע,string,20,28,1048,;" +
//"gd5fr,�ɶ�5����,string,8,29,1068,;" +
//"gd5jyfw,�ɶ�5��Ӫ��Χ,string,16,30,1076,;" +
//"gd6mc,�ɶ�6����,string,160,31,1092,;" +
//"gd6cgsl,�ɶ�6�ֹ�����,double,8,32,1252,;" +
//"gd6cgbl,�ɶ�6�ֹɱ���,single,4,33,1260,;" +
//"gd6bz,�ɶ�6��ע,string,20,34,1264,;" +
//"gd6fr,�ɶ�6����,string,8,35,1284,;" +
//"gd6jyfw,�ɶ�6��Ӫ��Χ,string,16,36,1292,;" +
//"gd7mc,�ɶ�7����,string,160,37,1308,;" +
//"gd7cgsl,�ɶ�7�ֹ�����,double,8,38,1468,;" +
//"gd7cgbl,�ɶ�7�ֹɱ���,single,4,39,1476,;" +
//"gd7bz,�ɶ�7��ע,string,20,40,1480,;" +
//"gd7fr,�ɶ�7����,string,8,41,1500,;" +
//"gd7jyfw,�ɶ�7��Ӫ��Χ,string,16,42,1508,;" +
//"gd8mc,�ɶ�8����,string,160,43,1524,;" +
//"gd8cgsl,�ɶ�8�ֹ�����,double,8,44,1684,;" +
//"gd8cgbl,�ɶ�8�ֹɱ���,single,4,45,1692,;" +
//"gd8bz,�ɶ�8��ע,string,20,46,1696,;" +
//"gd8fr,�ɶ�8����,string,8,47,1716,;" +
//"gd8jyfw,�ɶ�8��Ӫ��Χ,string,16,48,1724,;" +
//"gd9mc,�ɶ�9����,string,160,49,1740,;" +
//"gd9cgsl,�ɶ�9�ֹ�����,double,8,50,1900,;" +
//"gd9cgbl,�ɶ�9�ֹɱ���,single,4,51,1908,;" +
//"gd9bz,�ɶ�9��ע,string,20,52,1912,;" +
//"gd9fr,�ɶ�9����,string,8,53,1932,;" +
//"gd9jyfw,�ɶ�9��Ӫ��Χ,string,16,54,1940,;" +
//"gd10mc,�ɶ�10����,string,160,55,1956,;" +
//"gd10cgsl,�ɶ�10�ֹ�����,double,8,56,2116,;" +
//"gd10cgbl,�ɶ�10�ֹɱ���,single,4,57,2124,;" +
//"gd10bz,�ɶ�10��ע,string,20,58,2128,;" +
//"gd10fr,�ɶ�10����,string,8,59,2148,;" +
//"gd10jyfw,�ɶ�10��Ӫ��Χ,string,16,60,2156,;" +
//"gdzs,�ɶ�����,int,4,61,2172,;" +
//"gjgfrggds,���ҹɷ��˹ɹɶ���,int,4,62,2176,;" +
//"aggds,��ͨ��A�ɹɶ���,int,4,63,2180,;" +
//"bggds,��ͨ��B�ɹɶ���,int,4,64,2184,";

//                        break;
//                    #endregion
//                    #region �����ܱ�fundweek.fdt
//                    case DataTypes.jjjz:
//                        fileName = "FUNDWEEK.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 12032;
//                        recordSize = 188;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0,;" +
//"rq,����,date,4,13,184,;" +
//"dwjz,����λ��ֵ,single,4,6,152,;" +
//"jjze,����ֵ�ܶ�,double,8,5,144,;" +
//"gm,�����ģ,double,8,4,136,;" +
//"dwcz,����λ��ֵ,single,4,7,156,;" +
//"tzhjz,���������ֵ,single,4,8,160,;" +
//"tzhcz,����������ֵ,single,4,9,164,;" +
//"zzl,����������(%),double,8,10,168,;" +
//"ljjz,�����ۼƾ�ֵ,single,4,11,176,;" +
//"slrq,������������,date,4,1,12,;" +
//"glr,���������,string,60,2,16,;" +
//"tgr,�����й���,string,60,3,76,"
//;//12Ϊ�����ֶ�

//                        break;
//                    #endregion
//                    #region ����Ͷ�����funddiv.fdt
//                    case DataTypes.jjzh:
//                        fileName = "FUNDDIV.FDT";
//                        startAddress = 0x41000;
//                        blockSize = 8320;
//                        recordSize = 260;
//                        codeIsLong = true;
//                        fieldString =
//"dm,����,code,12,0,0,;" +
//"bgrq,��������,date,4,31,252,;" +
//"zzrq,��ֹ����,date,4,32,256,;" +
//"dm1,֤ȯ1����,string,12,1,12,;" +
//"sz1,֤ȯ1��ֵ,double,8,2,24,;" +
//"bl1,֤ȯ1ռ��ֵ����(%),single,4,3,32,;" +
//"dm2,֤ȯ2����,string,12,4,36,;" +
//"sz2,֤ȯ2��ֵ,double,8,5,48,;" +
//"bl2,֤ȯ2ռ��ֵ����(%),single,4,6,56,;" +
//"dm3,֤ȯ3����,string,12,7,60,;" +
//"sz3,֤ȯ3��ֵ,double,8,8,72,;" +
//"bl3,֤ȯ3ռ��ֵ����(%),single,4,9,80,;" +
//"dm4,֤ȯ4����,string,12,10,84,;" +
//"sz4,֤ȯ4��ֵ,double,8,11,96,;" +
//"bl4,֤ȯ4ռ��ֵ����(%),single,4,12,104,;" +
//"dm5,֤ȯ5����,string,12,13,108,;" +
//"sz5,֤ȯ5��ֵ,double,8,14,120,;" +
//"bl5,֤ȯ5ռ��ֵ����(%),single,4,15,128,;" +
//"dm6,֤ȯ6����,string,12,16,132,;" +
//"sz6,֤ȯ6��ֵ,double,8,17,144,;" +
//"bl6,֤ȯ6ռ��ֵ����(%),single,4,18,152,;" +
//"dm7,֤ȯ7����,string,12,19,156,;" +
//"sz7,֤ȯ7��ֵ,double,8,20,168,;" +
//"bl7,֤ȯ7ռ��ֵ����(%),single,4,21,176,;" +
//"dm8,֤ȯ8����,string,12,22,180,;" +
//"sz8,֤ȯ8��ֵ,double,8,23,192,;" +
//"bl8,֤ȯ8ռ��ֵ����(%),single,4,24,200,;" +
//"dm9,֤ȯ9����,string,12,25,204,;" +
//"sz9,֤ȯ9��ֵ,double,8,26,216,;" +
//"bl9,֤ȯ9ռ��ֵ����(%),single,4,27,224,;" +
//"dm10,֤ȯ10����,string,12,28,228,;" +
//"sz10,֤ȯ10��ֵ,double,8,29,240,;" +
//"bl10,֤ȯ10ռ��ֵ����(%),single,4,30,248,";


//                        break;
//                    #endregion
//                    #region ���userdata\block
//                    case DataTypes.bk:
//                        fileName = "BLOCK.DEF";
//                        startAddress = 0;
//                        blockSize = 0;
//                        recordSize = 248;
//                        codeIsLong = false;
//                        isIndexDataStruct = false;
//                        fieldString =
//"lb,���,string,20,0,0,;" +
//"bk,���,string,20,1,10,;" +
//"dm,֤ȯ����,string,10,2,42,";
//                        break;
//                    #endregion
//                    #region ����
//                    case DataTypes.pj:
//                        fileName = "����.str";
//                        startAddress = 0;
//                        blockSize = 256;
//                        recordSize = 256;
//                        codeIsLong = true;
//                        isIndexDataStruct = false;
//                        fieldString =
//"dm,֤ȯ����,string,12,0,0,;" +
//"pj,����,string,2,2,0,;" +
//"sm,˵��,string,244,2,0,";
//                        break;
//                    #endregion
//                    #region ��Ȩ���飬�������
//                    case DataTypes.hqfq:
//                        fileName = "DAY.DAT";
//                        startAddress = 0x41000;
//                        blockSize = 8192;
//                        recordSize = 32;
//                        codeIsLong = false;
//                        fieldString =
//"dm,����,code,10,0,0,;" +
//"rq,����,date,4,1,0,;" +
//"kp,���̸�Ȩ��,single,4,2,4,B;" +
//"zg,��߸�Ȩ��,single,4,3,8,B;" +
//"zd,��͸�Ȩ��,single,4,4,12,B;" +
//"sp,���̸�Ȩ��,single,4,5,16,B;" +
//"sl,��Ȩ�ɽ�����,single,4,6,20,A;" +
//"je,�ɽ����,single,4,7,24,;" +
//"spsyl,����������,single,4,0,0,";
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
//        public string DzhPath   //����DzhPath
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
//        public string DzhDataPath   //����DzhDataPath
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
//                        pivots += dzhFileStruct.fields[currentIndex, 0];//�ֶ�
//                        if ("  ,code,string".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " char(" + dzhFileStruct.fields[currentIndex, 3] + ") format=$" + dzhFileStruct.fields[currentIndex, 3] + "."; //�ַ���
//                        }
//                        else if ("  ,int,single,double".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num "; //��ֵ����
//                        }
//                        else if ("  ,date".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num format=YYMMDD10."; //date����
//                        }
//                        else if ("  ,datetime".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " num format=datetime."; //datetime����
//                        }
//                        pivots += " label='" + dzhFileStruct.fields[currentIndex, 1] + "'";//��ǩ

//                    }
//                    pivots = "create table FinData." + dataType + "(" + pivots + ");";
//                    if (delOldTable == true)
//                    {
//                        pivots = "drop table FinData." + dataType + ";" + pivots;
//                    }
//                    pivots = "proc sql;" + pivots + "quit;";
//                    break;
//                case "SASINPUT"://����SASֱ�Ӷ�ȡ����ʱ���õ�INPUT��䣬���һ���޸�
//                    for (int currentIndex = 0; currentIndex < dzhFileStruct.fields.GetLength(0); currentIndex++)
//                    {
//                        if ("  ,code,string".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " $" + dzhFileStruct.fields[currentIndex, 3] + "."; //�ַ���
//                        }
//                        else if ("  ,int,date,datetime".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " ib" + dzhFileStruct.fields[currentIndex, 3] + "."; //��ֵ����
//                        }
//                        else if ("  ,single".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " float" + dzhFileStruct.fields[currentIndex, 3] + "."; //��ֵ����
//                        }
//                        else if ("  ,double".IndexOf(dzhFileStruct.fields[currentIndex, 2]) > 0)
//                        {
//                            pivots += " @(p+" + dzhFileStruct.fields[currentIndex, 5] + ") " + dzhFileStruct.fields[currentIndex, 0] + " rb" + dzhFileStruct.fields[currentIndex, 3] + "."; //��ֵ����
//                        }
//                    }
//                    break;
//                case "FIELDS"://�г��ֶ�����
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
//        private string[] GetCodes(string Market)   //��ȡDay.dat�еĴ���
//        {
//            //����ָ������ת����,���ǻ�ͬʱ���滦���������
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
//                msg = @"�޷���ע����е����ǻ������ļ�Ŀ¼�������н����� DzhDataPath����Ϊ��Ч·������c:\dzh\data\��";
//                return new string[1] { null };
//            }
//            Market = Market.Trim().ToUpper();
//            if (Market == "")
//            {
//                msg = "Market����ֻ�����г���ƣ��绦��ΪSH������ΪSZ�����ΪHK�ȡ�";
//                return null;
//            }
//            string DzhFile = dzhDataPath + Market + @"\DAY.DAT";
//            msg = "";
//            if (!File.Exists(DzhFile))  //DAY.DAT�ļ�������
//            {
//                msg = DzhFile + "�����ڣ�";
//                return new string[1] { null };
//            }
//            try
//            {
//                this.checkFileStream(DzhFile);
//                int secCounts = 0;//�ļ���֤ȯ����
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
//                            code = new string(br.ReadChars(10));//���ǻ���10���ֽڱ�����룬һ����6���ֽ�
//                            code = Market + code.Replace("\0", "");
//                            code = code.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
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
//                msg = @"����Ĳ������󡣲���ֻ����:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" ���� ";
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
//                //fields[0, 0] = "<�ֶ���>"; fields[0, 1] = "<����>"; fields[0, 2] = "<����>";
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
//                msg = "����"; return new string[1, 1] { { null } };
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
//                { throw new Exception("����:" + mdzh.Msg); }
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
//            { throw new Exception("����:" + mdzh.Msg); }
//        }
//        public static string SqlGetData_hq0_sp(string code)//���سɽ���
//        {
//            DzhData mdzh = new DzhData();
//            string[,] dataArray = mdzh.GetData("hq0", code);
//            if (mdzh.Error == 0 && dataArray.GetLength(0) > 0 && dataArray.GetLength(1) >= 8)
//            {
//                return dataArray[0, 7];
//            }
//            else
//            { throw new Exception("����:" + mdzh.Msg); }
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
//                msg = @"����Ĳ������󡣵�һ������ֻ����:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" ���� ";
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
//                msg = @"����Ĳ������󡣵�һ������ֻ����:";
//                foreach (string s in Enum.GetNames(typeof(DataTypes)))
//                    msg += " \"" + s + "\"";
//                msg += @" ���� ";
//                foreach (int currentIndex in Enum.GetValues(typeof(DataTypes)))
//                    msg += " " + currentIndex.ToString();

//                return new string[1, 1] { { null } };
//            }
//        }
//        private string[,] GetData(DataTypes dataType, string code, string newFileName) //��ȡ���ݣ�����
//        {
//            if (dataType == DataTypes.bk) return GetBK(code);
//            if (dataType == DataTypes.pj) return GetPJ(code);
//            if (dataType == DataTypes.hqfq) return GetHqfq(dataType, code, newFileName);
//            #region ��ȡ����ǰ��ʼ��
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(dataType);
//            if (newFileName != "") dzhFileStruct.fileName = newFileName; //����û�����ָ�����ļ���
//            code = code.Trim().ToUpper();
//            if (code == "")
//            {
//                msg = @"CODE��������Ϊ�ա����ṩ֤ȯ���룬��SZ000001��";
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
//                msg = @"�޷���ע����е����ǻ������ļ�Ŀ¼�������н����� DzhDataPath����Ϊ��Ч·������c:\dzh\data\��";
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
//                msg = dzhFileStruct.fileName + "û���ҵ���";
//                return new string[1, 1] { { null } };
//            }
//            #endregion
//            if (dzhFileStruct.isIndexDataStruct == true)
//            {
//                #region ����DAY.DAT�Ƚṹ������/���ݣ�������
//                try
//                {
//                    this.checkFileStream(DzhFile);
//                    int secCounts = 0;//�ļ���֤ȯ����
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
//                            //code0 = new string(br.ReadChars(10));//���ǻ���10���ֽڱ�����룬һ����8���ֽ�
//                            code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                            code0 = code0.Replace("\0", "");
//                            code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
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
//                    int iRecord = 1;//��¼
//                    int iBlock = 0;//��iBlock��
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
//                                        //code0 = new string(br.ReadChars(8));//��12λ��ʵ������8λ����9-12λһ��Ϊ\0����ʱ�Ǵ����ֽڣ���Ϊֻ��8λ
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
//                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
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
//                        #region ���������STKINFO60.DAT�Ƚṹ�����ݣ�
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
//                            int secCounts = 0;//�ļ���֤ȯ����
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
//                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
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
//                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.hq0:
//                        #region �������飨����STKINFO60.DAT�Ƚṹ�����ݣ�
//                        try
//                        {
//                            this.checkFileStream(DzhFile);

//                            int secCounts = 0;//�ļ���֤ȯ����
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
//                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx

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
//                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.cq:
//                        #region �ֺ����䣨����STKINFO60.DAT�Ƚṹ�����ݣ�
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//�ļ���֤ȯ����
//                            string code0 = "";
//                            fileStruct dzhdmStruct = new fileStruct(DataTypes.dm);//    ����Ľṹ
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
//                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
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
//                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.cw0:
//                        #region ��������--�򵥣�����STKINFO60.DAT�Ƚṹ�����ݣ�
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//�ļ���֤ȯ����
//                            string code0 = "";
//                            fileStruct dzhdmStruct = new fileStruct(DataTypes.dm);//    ����Ľṹ
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
//                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx

//                                //if (code0.Length > 0)//Xchui  ����
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
//                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                            return records;
//                        }
//                        catch (Exception e)
//                        {
//                            msg = e.Message;
//                        }
//                        #endregion
//                        break;
//                    case DataTypes.hqmb:
//                        #region ����Report.DAT���ݣ��ṹ����DAY.DAT������Щ��ֵ��Ҫ��һ�����������
//                        try
//                        {
//                            this.checkFileStream(DzhFile);
//                            int secCounts = 0;//�ļ���֤ȯ����
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
//                                    //code0 = new string(br.ReadChars(10));//���ǻ���10���ֽڱ�����룬һ����8���ֽ�
//                                    code0 = System.Text.Encoding.GetEncoding(936).GetString(br.ReadBytes(10));
//                                    int start = code0.IndexOf('\0');
//                                    code0 = code0.Remove(start, code0.Length - start);
//                                    code0 = code0.Replace("\0", "");
//                                    code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
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
//                            int iRecord = 1;//��¼
//                            int iBlock = 0;//��iBlock��
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
//                                        switch (dzhFileStruct.fields[iField, 0].ToLower()) //�������ȡDAY.DAT�÷���ͬ���жϵ��Ǵ������������
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
//                                                record[iField] = "";//���������������
//                                                break;
//                                            case "mm":
//                                                int mm = br.ReadSByte();
//                                                record[iField] = "";
//                                                if (mm == -128 || mm == -96) record[iField] = "����"; //-128 = 0x80
//                                                if (mm == -64) record[iField] = "����";  //-64 = 0xC0
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
//                                    if (j == 5)  //������
//                                    {
//                                        record0[j] = (Convert.ToSingle(record0[3]) - zssSaved).ToString();
//                                        zssSaved = Convert.ToSingle(record0[3]);
//                                    }
//                                    records[currentIndex, j] = record0[j];
//                                }
//                            }
//                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
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
//            msg = "���ؿ����顣";
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetBK(string code)//��鶨������
//        {
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(DataTypes.bk);
//            if (code == null) code = "";
//            code = code.Trim().ToUpper();
//            ArrayList recordList = new ArrayList();
//            if (this.DzhDataPath == "")
//            {
//                msg = @"�޷���ע����е����ǻ������ļ�Ŀ¼�������н����� DzhDataPath����Ϊ��Ч·������c:\dzh\data\��";
//                return new string[1, 1] { { null } };
//            }
//            string DzhBlockPath = dzhDataPath;
//            DzhBlockPath = DzhBlockPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\BLOCK\\"); //����Ŀ¼�к���data����
//            string DzhFile = DzhBlockPath + dzhFileStruct.fileName;

//            msg = "";
//            if (!File.Exists(DzhFile))
//            {
//                msg = "����ļ��޷��ҵ���";
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
//                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                    return records;
//                }
//            }
//            catch (Exception e)
//            {
//                msg = e.Message;
//            }
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetPJ(string code)//��������
//        {
//            msg = "";
//            fileStruct dzhFileStruct = new fileStruct(DataTypes.pj);
//            code = code.Trim().ToUpper();
//            ArrayList recordList = new ArrayList();
//            if (this.DzhDataPath == "")
//            {
//                msg = @"�޷���ע����е����ǻ������ļ�Ŀ¼�������н����� DzhDataPath����Ϊ��Ч·������c:\dzh\data\��";
//                return new string[1, 1] { { null } };
//            }
//            string dzhSubPath = dzhDataPath;
//            dzhSubPath = dzhSubPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\SelfData\\"); //����Ŀ¼�к���data����
//            string DzhFile = dzhSubPath + dzhFileStruct.fileName;

//            msg = "";
//            if (!File.Exists(DzhFile))
//            {
//                msg = dzhFileStruct.fileName + "�޷��ҵ���";
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
//                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
//                    return records;
//                }
//            }
//            catch (Exception e)
//            {
//                msg = e.Message;
//            }
//            return new string[1, 1] { { null } };

//        }
//        private string[,] GetHqfq(DataTypes dataType, string code, string newFileName)//��Ȩ�۸�,�ֺ���Ͷ��,��ǰ��Ȩ��
//        {
//            DzhData dzh = new DzhData();
//            string[,] hq = dzh.GetData("hq", code, newFileName);
//            if (dzh.Error != 0 || hq.GetLength(1) < 4) return new string[1, 1] { { null } };
//            string[,] x = new string[hq.GetLength(0), 9];
//            string[,] cq = dzh.GetData("cq", code, newFileName);
//            string fmt = "_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F";
//            if (dzh.Error != 0 || cq.GetLength(1) < 4 || cq.GetLength(0) == 0) //û�г�Ȩ��Ϣ
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
//            else  //�г�Ȩ��Ϣ
//            {
//                DateTime[] cqdt = new DateTime[cq.GetLength(0)];
//                for (int j = 0; j < cq.GetLength(0); j++) cqdt[j] = new DateTime(int.Parse(cq[j, 1].Split('-')[0]), int.Parse(cq[j, 1].Split('-')[1]), int.Parse(cq[j, 1].Split('-')[2]));
//                int i0 = hq.GetLength(0) - 1;
//                DateTime hqdt_1, hqdt;
//                double kp_1, zg_1, zd_1, sp_1, kp, zg, zd, sp, kpx, zgx, zdx, spx, sgbl, kpsyl, zgsyl, zdsyl, spsyl, pgbl, pgjg, fh;
//                for (int k = 0; k < 8; k++) x[i0, k] = hq[i0, k];  //���һ����¼
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
//                    //syl=1+��t�������� =( t�����̼�*(1+�͹ɱ���+��ɱ���)+�ֺ���-��ɼ۸�*��ɱ���)/(t-1�����̼�)
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
//                    x[currentIndex - 1, 6] = hq[currentIndex - 1, 6];//sl �ɽ���δ��Ȩ
//                    x[currentIndex - 1, 7] = hq[currentIndex - 1, 7];//je
//                    x[currentIndex, 8] = (spsyl - 1).ToString("0.00000");//spsyl ���̼�������

//                }

//            }

//            return x;

//        }
//    }
}

