using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EphemerisCalculator;
using System.Diagnostics;
using DataImporter;
using System.Collections.Specialized;
using System.IO;

namespace EphemerisTest
{
    class Program
    {
        static void Main(string[] args)
        {
            eclipseTest3();
            //testRelationCalculate();

            //translateVolume();

            //readTr2Test();
            //testDateTime();

            //testSerialize();

            //eclipseTest();

            //DateTimeOffset now = DateTimeOffset.Now;
            //Debug.WriteLine(string.Format("Start calculate positions: ------{0}", now.ToString("hh:mm:ss.fff")));

            //Position sunPos = Ephemeris.Geocentric[new DateTimeOffset(1999, 3, 31, 0, 0, 0, TimeSpan.Zero), PlanetId.SE_MARS];

            //Debug.WriteLine(string.Format("End of calculate positions: ------{0}, {1}ms elapsed", DateTimeOffset.Now.ToString("hh:mm:ss.fff"), (DateTimeOffset.Now - now).TotalMilliseconds));
            //now = DateTimeOffset.Now;

            //sunPos = Ephemeris.Geocentric[new DateTimeOffset(2011, 3, 31, 0, 0, 0, TimeSpan.Zero), PlanetId.SE_SUN];

            //test1();

            //testAverage();

            //testPeriod();

            //eclipseTest2();

            Console.ReadLine();
        }

        private static void testRelationCalculate()
        {
            PlanetPair pair = new   PlanetPair(PlanetId.SE_VENUS, PlanetId.SE_PLUTO);

            //double degree1, degree2;
            do
            {
                //Console.WriteLine("Input degree1:");
                //string input = Console.ReadLine();

                //if (!double.TryParse(input, out degree1))
                //    return;

                //Console.WriteLine("Input degree2:");
                //input = Console.ReadLine();

                //if (!double.TryParse(input, out degree2))
                //    return;

                //List<RelationBrief> relations = RelationDetector.RelationsOf(pair, degree1, degree2);

                //foreach(RelationBrief rlt in relations)
                //{
                //    Console.WriteLine(rlt.ToString());
                //}

                Console.WriteLine("Input Date1:");

                DateTimeOffset time1, time2;

                string input = Console.ReadLine();

                if (!DateTimeOffset.TryParse(input, out time1))
                    return;

                Console.WriteLine("Input Date2:");

                input = Console.ReadLine();

                if (!DateTimeOffset.TryParse(input, out time2))
                    return;

                Dictionary<PlanetPair, List<Relation>> GeoRelations = RelationDetector.GeoRelationsOf(time1, time2);

                if (GeoRelations.Count != 0)
                {
                    foreach (KeyValuePair<PlanetPair, List<Relation>> kvp in GeoRelations)
                    {
                        Console.WriteLine("---------------------" + kvp.Key.Interior.ToString() + "  " + kvp.Key.Exterior.ToString() + "---------------------");
                        foreach (Relation rlt in kvp.Value)
                        {
                            Console.WriteLine(rlt.ToString());
                        }
                    }
                }

                //List<Relation> relations = RelationDetector.RelationsBetween(pair, time1, time2);

                //foreach (Relation rlt in relations)
                //{
                //    Console.WriteLine(rlt.ToString());
                //}

                Console.WriteLine();
            } while (true);
        }

        private static void testDateTime()
        {
            DateTime end = new DateTime(2011, 6, 7, 11, 30, 0);
            UInt32 uVal = (UInt32)end.Ticks;

            Console.WriteLine("DateTime={0}, Val={1:x}", end, uVal);
        }

        private static void readTr2Test()
        {
            string tickFileName = @"C:\Program Files\南华期货博易大师客户端\pobo3\Data\cnfut\Tick\010512.tr2";

            if (!File.Exists(tickFileName))
            {
                Console.WriteLine("Failed to locate the file!");
                return;
            }

            FileInfo fileInfo = new FileInfo(tickFileName);

            if ((fileInfo.Length - 16) % PoboTr2Structure.Size != 0)
            {
                Console.WriteLine("File length is not expected: " + fileInfo.Length.ToString());
                return;
            }            

            using (FileStream fs = new FileStream(tickFileName, FileMode.Open, FileAccess.Read))
            {
                byte[] temp = new byte[16];
                int readed = 0;

                readed = fs.Read(temp, 0, temp.Length);
                

                if (readed != 16)
                    throw new IOException("Failed to read all data!");

                UInt32[] uArray = PoboDataImporter.BytesToStructures<UInt32>(temp);

                Int32 sellVolume = PoboDataImporter.ToVolume(uArray[0]);
                Int32 buyVolume = PoboDataImporter.ToVolume(uArray[1]);
                double lastPrice = (double)(uArray[2]) / 1000;
                int lastPosition = PoboDataImporter.ToVolume(uArray[3]);

                byte[] buffer = new byte[fs.Length - 16];                             

                readed = fs.Read(buffer, 0, buffer.Length);

                fs.Close();

                if (readed != buffer.Length)
                    throw new IOException("Failed to read all data!");

                PoboTr2Structure[] rawRecords = PoboDataImporter.BytesToStructures<PoboTr2Structure>(buffer);

                foreach (PoboTr2Structure raw in rawRecords)
                {
                    Console.WriteLine("P={0}, U2={2:x}, U1={1:x}", raw.Position, raw.Unknown1, raw.Unknown2);

                    //Console.WriteLine("P={0}, U1={1:x}, U2={2:x}, {3}{4}", raw.Position, raw.Unknown1, raw.Unknown2,
                    //    Convert.ToString(raw.Unknown2, 2), Convert.ToString(raw.Unknown1, 2));

                    //Console.WriteLine(raw.ToString());
                }
            }

        }

        private static void testSerialize()
        {
            OutlineItem pivot = new OutlineItem(DateTimeOffset.Now.Date, 200, 5, PivotType.Top);
            OrbitDescription description = new OrbitDescription(false, false, PlanetId.SE_MARS, SeFlg.GEOCENTRIC, 0, pivot, 1);

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(description.GetType());

            System.IO.MemoryStream stream = new System.IO.MemoryStream(500);
            ser.Serialize(stream, description);

            stream.Position = 0;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
            {

                Console.Write(reader.ReadToEnd());
            }
        }

        private static void translateVolume()
        {
            UInt32 raw = 0;
            Int32 volume = 0;
            BitVector32.Section minutesMask;             //The bits 0-5: 6 bits
            BitVector32.Section hoursMask;             //The bits 6-10: 5 bits
            BitVector32.Section dayMask;                 //The bits 11 - 15: 5bits
            BitVector32.Section monthMask;               //The bits 16 - 19: 4 bits
            BitVector32.Section yearMask;                //The bits 20 - 31 : 12 bits

            minutesMask = BitVector32.CreateSection(0x3F);
            hoursMask = BitVector32.CreateSection(0x1F, minutesMask);
            dayMask = BitVector32.CreateSection(0x1F, hoursMask);
            monthMask = BitVector32.CreateSection(0xF, dayMask);
            yearMask = BitVector32.CreateSection(0xFFF, monthMask);

            string hexString = null;


            do
            {
                Console.WriteLine("Input data in hex format:");

                hexString = Console.ReadLine();

                if (hexString.StartsWith("T", true, null))
                {
                    if (!UInt32.TryParse(hexString.Substring(1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out raw))
                        break;

                    BitVector32 time = new BitVector32((int)raw);

                    DateTime theTime = new DateTime(time[yearMask], time[monthMask], time[dayMask], time[hoursMask], time[minutesMask], 0);

                    Console.WriteLine("Time = {0}-{1}-{2} {3}:{4}", theTime.Year, theTime.Month, theTime.Day, theTime.Hour, theTime.Minute);
                }
                else
                {
                    if (!UInt32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out raw))
                        break;

                    volume = DataImporter.PoboDataImporter.ToVolume(raw);

                    Console.WriteLine("0x{0:x} = {1}", raw, volume);
                }


            } while (true);
        }

        private static void testAverage()
        {
            DateTimeOffset since = new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset until = new DateTimeOffset(2011, 3, 31, 0, 0, 0, TimeSpan.Zero);
            Position pos = null;
            for(DateTimeOffset date = since; date <= until; date = date.AddDays(1))
            {
                pos = Ephemeris.Heliocentric[date, PlanetId.Eight_Average];

                Console.WriteLine("{0}: Eight_Average={1}", date, pos.Longitude);
            }
        }

        private static void testPeriod()
        {
            Phenomena testPeriod = new Phenomena(DateTimeOffset.Now, PeriodType.AroundTheWeek, AspectImportance.Important);

        }

        private static void test1()
        {
            List<IPlanetEvent> aspectEvents = Ephemeris.Geocentric[new DateTimeOffset(1932, 12, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1),
                new DateTimeOffset(1933, 1, 2, 0, 0, 0, TimeSpan.Zero), AspectImportance.Important];

            foreach (ExactAspectEvent evt in aspectEvents)
            {
                Console.WriteLine(evt.ToString());
            }

            List<IPlanetEvent> aspectEvents2 = Ephemeris.Geocentric[new DateTimeOffset(1919, 12, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1),
                new DateTimeOffset(1920, 1, 2, 0, 0, 0, TimeSpan.Zero), AspectImportance.Important];

            foreach (IPlanetEvent evt in aspectEvents2)
            {
                Console.WriteLine(evt.ToString());
            }

            List<IPlanetEvent> aspectEvents3 = Ephemeris.Geocentric[new DateTimeOffset(1945, 12, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1),
                new DateTimeOffset(1946, 1, 2, 0, 0, 0, TimeSpan.Zero), AspectImportance.Important];

            foreach (IPlanetEvent evt in aspectEvents3)
            {
                Console.WriteLine(evt.ToString());
            }
        }

        private static void eclipseTest3()
        {
            int year = 1927;
            DateTimeOffset date = new DateTimeOffset(year, 5, 1, 0, 0, 0, TimeSpan.Zero);

            List<SeFlg> flags = new List<SeFlg>() { (SeFlg)0, 
                SeFlg.GEOCENTRIC, 
                SeFlg.GEOCENTRIC | SeFlg.SEFLG_TRUEPOS,
                SeFlg.EQUATORIALBASED, 
                SeFlg.HELIOCENTRIC,
                SeFlg.HELIOCENTRIC | SeFlg.SEFLG_EQUATORIAL
            };

            Position pos = null;
            PlanetId star = PlanetId.SE_URANUS;

            while(true)
            {
                Console.WriteLine("On " + date.ToString());
                foreach (SeFlg flg in flags)
                {
                    pos = Ephemeris.PositionOf(date, star, flg);

                    Console.WriteLine("{0}: Long={1}, Lat={2}", flg, pos.Longitude, pos.Latitude);
                }

                Console.WriteLine("Input the date: ");
                string line = Console.ReadLine();

                if (!DateTimeOffset.TryParse(line, out date))
                    return;

                Console.WriteLine("Input the planet: ");

                PlanetId id = PlanetId.SE_URANUS;
                line = Console.ReadLine();
                if (Planet.TryParseId(line, out id))
                {
                    star = id;
                }
            }

        }



        private static void eclipseTest2()
        {
            int year = 1907;
            DateTimeOffset start = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset end = new DateTimeOffset(year, 1, 31, 0, 0, 0, TimeSpan.Zero);
            List<IPlanetEvent> events = Ephemeris.Geocentric[start, end, PlanetEventFlag.EclipseOccultationCategory];

            SolarEclipse eclipse = events[1] as SolarEclipse;

            Console.WriteLine("{0} happened on {1}, Sun @ {2}", eclipse.ShortDescription, eclipse.When, eclipse.Where);

            Position venusPos = Ephemeris.Geocentric[eclipse.When, PlanetId.SE_VENUS];

            List<IPlanetEvent> transcents = Ephemeris.Geocentric.TranscensionEventOf(PlanetId.SE_VENUS, venusPos.Longitude,
                eclipse.When.AddDays(1), start.AddYears(1));

            Console.WriteLine("Venus @ {0}, and transcension afterwards are:\r\n", venusPos.Longitude);

            foreach (IPlanetEvent evt in transcents)
            {
                Console.WriteLine(evt.ToString());
            }

            transcents = Ephemeris.Geocentric.TranscensionEventOf(PlanetId.SE_VENUS, eclipse.Where.Longitude,
                eclipse.When.AddDays(1), start.AddYears(1));

            Console.WriteLine("Venus transcend Sun position of {0} are:\r\n", eclipse.Where.Longitude);

            foreach (IPlanetEvent evt in transcents)
            {
                Console.WriteLine(evt.ToString());
            }

            LunarEclipse lun = events[3] as LunarEclipse;

            Console.WriteLine("{0} happened on {1}, Sun @ {2}", lun.ShortDescription, lun.When, lun.Where);

            venusPos = Ephemeris.Geocentric[lun.When, PlanetId.SE_VENUS];

            transcents = Ephemeris.Geocentric.TranscensionEventOf(PlanetId.SE_VENUS, lun.Where.Longitude,
                eclipse.When.AddDays(1), start.AddYears(1));

            foreach (IPlanetEvent evt in transcents)
            {
                Console.WriteLine(evt.ToString());
            }

            Console.ReadKey();
        }

        private static void eclipseTest()
        {
            String y = null;
            int year = 0;

            while(true)
            {
                Console.WriteLine("Input the year: ");
                y = Console.ReadLine();

                if (!int.TryParse(y, out year))
                    return;

                Console.WriteLine("Eclipse/Occultations on year {0}:", year);
                DateTimeOffset start = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset end = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

                List<IPlanetEvent> lunarEvents = Ephemeris.SolarEclipseDuring(start, end);

                foreach(IPlanetEvent evt in lunarEvents)
                {
                    Console.WriteLine(evt.ToString());
                }
                Console.ReadKey();

                lunarEvents = Ephemeris.LunarEclipseDuring(start, end);

                foreach (IPlanetEvent evt in lunarEvents)
                {
                    Console.WriteLine(evt.ToString());
                }
                Console.ReadKey();

                lunarEvents = Ephemeris.OccultationDuring(start, end);

                StringBuilder sb = new StringBuilder();
                foreach (Occultation evt in lunarEvents)
                {
                    sb.AppendLine(evt.ToString());
                }

                Console.WriteLine(sb.ToString());
            }

        }
    }
}
