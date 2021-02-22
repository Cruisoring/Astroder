using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using AstroHelper;
using QuoteHelper;
using ZedGraph;
using NumberHelper.DoubleHelper;
using PolygonControl;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace NumberTest
{

    class Program
    {
        static void Main(string[] args)
        {
            //MarsTest2(PlanetId.SE_MARS);
            //MarsTest(PlanetId.SE_JUPITER);
            //VernalTest();
            //RulersTest();
            //ImportTest();
            //DateTest();

            //TestPolygon();

            // NodesTest();
            // AngleMapperTest();

            //TimeMapperTest();

            //TimeSpanTest();

            //PriceMappingTest();

            //OrbitsPresenterTest();

            //xDateTest();

            //HelioCentricTest();

            TestPolygonCalculator();

            Console.ReadKey();
        }

        private static void TestPolygonCalculator()
        {
            PolygonControl.PolygonControl.PolygonCalculator calculator = new PolygonControl.PolygonControl.PolygonCalculator(Polygon.Hexagon, 10);

            for(int round = calculator.MinCycle; round < 5; round ++)
            {
                Console.Write("Round " + round.ToString() + ": ");
                foreach (KeyValuePair<int, PointF> kvp in calculator.VerticesPositions[round])
                {
                    Console.Write(String.Format("{0}({1}),\t", kvp.Key, kvp.Value));
                }
                Console.WriteLine();
            }

            Console.ReadKey();

            Console.WriteLine("Reference Position of Indexes in Round 5:");
            int last = calculator.Calculator.LastOfRound(5);
            for (int index = calculator.Calculator.FirstOfRound(5); index <= last; index ++ )
            {
                Console.WriteLine("{0}: {1}", index, calculator.PositionOfIndex(index));
            }
        }

        private static void HelioCentricTest()
        {
            DateTimeOffset time = new DateTimeOffset(1954, 3, 19, 12, 0, 0, TimeSpan.Zero);
            Double timeVal = Utilities.ToJulianDay(time);
            Position jupiterPos = Utilities.HeliocentricPositionOfJulian(timeVal, PlanetId.SE_JUPITER);
            Console.WriteLine("{0}, or {1}", jupiterPos.Longitude, jupiterPos.Longitude.AstrologyFormat());
        }

        private static void xDateTest()
        {
            DateTime time = DateTimeOffset.UtcNow.Date;
            XDate x = new XDate(time);

            Double xVal = x.XLDate;

            Double xl = time.ToOADate();

            Console.WriteLine("Form time = {0}, XDate = {1}, XLDate = {2}, OADate={3}", time, x, xVal, xl);
        }

        private static void OrbitsPresenterTest()
        {
            //OrbitsPresenter present = new OrbitsPresenter(new DateTimeOffset(2010, 9, 15, 0, 0, 0, TimeSpan.Zero), 
            //    new DateTimeOffset(2010, 9, 22, 0, 0, 0, TimeSpan.Zero));

            //present.Controller[PlanetId.SE_MARS][OrbitInfoType.Longitude] = true;
        }

        private static void PriceMappingTest()
        {

            //for (double nadia = 0.43; nadia < 900; nadia *= 5 )
            //{
            //    //for (double zenith = around + 0.3; zenith < 1000; zenith *= 2 )
            //    //{
            //    //    PriceToAngle mapper = PriceToAngle.NaturalOf(around);

            //    //    Console.WriteLine("Nadia={0}, Zenith={1}: Zero={2}, Step={3}",
            //    //        around, zenith,mapper.Zero, mapper.Step);


            //    //    //for (PriceMappingRules rule = PriceMappingRules.CalendarNatural; rule <= PriceMappingRules.FloorStep; rule ++ )
            //    //    //{
            //    //    //    PriceToAngle mapper = new PriceToAngle(around, zenith, rule);
            //    //    //    Console.WriteLine("Nadia={0}, Zenith={1}, Rule={2}: Zero={3}, Step={4}",
            //    //    //        around, zenith, rule, mapper.Zero, mapper.Step);
            //    //    //}
            //    //}
            //    PriceAngleConverter mapper = PriceAngleConverter.NaturalOf(nadia);

            //    Console.WriteLine("Nadia={0}: Zero={1}, Step={2}",
            //        nadia, mapper.Zero, mapper.Step);

            //    Console.WriteLine();
            //    Console.ReadKey();
            //}

            //for (double range = 0.43; range < 40; range *= 2 )
            //{
            //    mag = Math.Zero(Math.Log10(range) - 2);
            //    step = Math.Pow(10, mag);

            //    Console.WriteLine("For range = {0}, step ={1}", range, step);
            //    for (double price = 0; price < range * 2; price += step * 30 )
            //    {
            //        Angle angle = new Angle((price - 0) / step);
            //        degree = angle.Degrees;
            //        Console.WriteLine("Price={0}, Angle={1} or Degree={2}", price, angle, degree);
            //    }
            //    Console.WriteLine();
            //    Console.ReadKey();
            //}
        }

        private static void TimeSpanTest()
        {
            DateTimeOffset time1 = new DateTimeOffset(1865, 4, 4, 0, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset time2 = new DateTimeOffset(1963, 11, 22, 0, 0, 0, 0, TimeSpan.Zero);

            Double time1Val = Utilities.ToJulianDay(time1);
            Double time2Val = Utilities.ToJulianDay(time2);

            TimeSpan span = time2 - time1;

            Char ch = Planet.SymbolOf(PlanetId.SE_URANUS);

            Console.WriteLine("On {0}: Jupiter is at {1}", time1, Utilities.GeocentricPositionOfJulian(time1Val, PlanetId.SE_JUPITER));
            Console.WriteLine("On {0}: Jupiter is at {1}", time2, Utilities.GeocentricPositionOfJulian(time2Val, PlanetId.SE_JUPITER));

            double jupiterYears = span.TotalDays / Planet.Jupiter.AveragePeriod;

            double signs = span.TotalDays / (Planet.Jupiter.AveragePeriod / 12);

            Console.WriteLine("{0}: {1} Jupiter years, {2} signs.", span, jupiterYears, signs);
        }

        //private static void TimeMapperTest()
        //{
        //    DateTimeOffset deadLine = new DateTimeOffset(2009, 1, 1, 1, 1, 1, TimeSpan.Zero);

        //    Angle angle;
        //    for(DateTimeOffset date = DateTimeOffset.UtcNow; date > deadLine; date -= TimeSpan.FromDays(30.2))
        //    {
        //        angle = TimeAngleConverter.Default.AngleOf(date);

        //        Console.WriteLine("Time: {0}\t Angle={1}", date, angle);
        //    }
        //}

        //private static void AngleMapperTest()
        //{
        //    Type mapperType = typeof(AngleFrame);
        //    FieldInfo[] fields = mapperType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        //    foreach (FieldInfo fi in fields)
        //    {
        //        AngleFrame mapper = fi.GetValue(null) as AngleFrame;
        //        if (mapper != null)
        //        {
        //            Console.WriteLine("Test of " + fi.Name);

        //            for (double deg = 0; deg < 380; deg += 10 )
        //            {
        //                Console.WriteLine("{0} is mapped to {1}", deg, mapper.UnifiedDegrees(deg));
        //            }

        //            Console.ReadKey();
        //        }
        //    }

        //}

        private static void NodesTest()
        {
            //DateTimeOffset vernal = Ephemeris.VernalEquinoxTimeOf(1937);
            //for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id ++ )
            //{
            //    for (MirrorType mirror = MirrorType.Ascending; mirror <= MirrorType.Aphelion; mirror ++ )
            //    {
            //        Position mirrorPos = Utilities.GeocentricPositionOf(DateTimeOffset.UtcNow, id, mirror, SeFlg.SEFLG_SPEED);// | SeFlg.SEFLG_HELCTR);

            //        Console.WriteLine("Today: {0} of {1}: {2}, {3:F4}, {4:F4}, {5:F4}, {6:F4}, {7:F4}", 
            //            mirror, id, mirrorPos.Longitude, mirrorPos.Latitude, mirrorPos.Distance,
            //            mirrorPos.LongitudeVelocity, mirrorPos.LatitudeVelocity, mirrorPos.DistanceVelocity);

            //        mirrorPos = Utilities.GeocentricPositionOf(vernal, id, mirror, SeFlg.SEFLG_SPEED | SeFlg.SEFLG_HELCTR);

            //        Console.WriteLine("On vernal Equinox: {0} of {1}: {2}, {3:F4}, {4:F4}, {5:F4}, {6:F4}, {7:F4}", 
            //            mirror, id, mirrorPos.Longitude, mirrorPos.Latitude, mirrorPos.Distance,
            //            mirrorPos.LongitudeVelocity, mirrorPos.LatitudeVelocity, mirrorPos.DistanceVelocity);
            //    }
            //}
        }

        private static void MarsTest2(PlanetId id)
        {
            Rectascension destination = new Rectascension(346.7);

            DateTimeOffset since = new DateTimeOffset(1951, 3, 14, 1, 1, 0, TimeSpan.Zero);

            for (int i = 0; i < 10; i ++ )
            {
                destination = new Rectascension(destination.Degrees + i * 330);
                DateTimeOffset date = Ephemeris.DateOfPlanetPosition(id, since, destination);

                double jul_ut = Utilities.ToJulianDay(date);
                Position actual = Utilities.GeocentricPositionOfJulian(jul_ut, id);
                double dif = actual.Longitude - destination.Degrees;

                Console.WriteLine("{0}: {1} on {2}, dif = {3:F4}", date, id, actual.ToString("Astro0", null), dif);
                since = date + TimeSpan.FromDays(60);
            }
        }

        //private static void MarsTest(PlanetId id)
        //{
        //    Rectascension destination = new Rectascension(166.7);
        //    Double averageSpeed = Ephemeris.AverageSpeedOf(id);

        //    TimeSpan timeShift = TimeSpan.FromDays(210 / averageSpeed);

        //    DateTimeOffset since = new DateTimeOffset(1931, 3, 14, 1, 1, 0, TimeSpan.Zero);

        //    for (int i = 0; i < 100; i++)
        //    {
        //        destination = new Rectascension(destination.Degrees + i * 330);
        //        DateTimeOffset date = Ephemeris.ExactlyTimeOf(id, since, destination);

        //        double date = Utilities.ToJulianDay(date);
        //        Position actual = Utilities.GeocentricPositionOf(date, id, SeFlg.SEFLG_SPEED);
        //        double dif = actual.Longitude.Degrees - destination.Degrees;

        //        Console.WriteLine("{0}: Mars on {1}, dif = {2:F4}", date, actual.ToString("Astro0", null), dif);
        //        since = date + timeShift;
        //    }
        //}

        //public static void VernalTest()
        //{
        //    //for (int year = 1991; year < 2010; year ++ )
        //    //{
        //    //    DateTimeOffset equinox = Ephemeris.VernalEquinoxTimeOf(year);

        //    //    Console.WriteLine("Year {0}: {1}", year, equinox);
        //    //}
        //    DateTimeOffset time;

        //    for (int i = 1; i < 13; i ++ )
        //    {
        //        time = new DateTimeOffset(2010, i, 1, 12, 0, 0, TimeSpan.Zero);

        //        Console.WriteLine("Angle of {0}: {1}", time, Ephemeris.AngleOfTime(time));

        //        Console.WriteLine("--------------Sun Longitude: {0}.", Ephemeris.CurrentEphemeris[time, PlanetId.SE_SUN].Longitude);

        //        time = new DateTimeOffset(2010, i, 15, 12, 0, 0, TimeSpan.Zero);

        //        Console.WriteLine("Angle of {0}: {1}", time, Ephemeris.AngleOfTime(time));

        //        Console.WriteLine("--------------Sun Longitude: {0}.", Ephemeris.CurrentEphemeris[time, PlanetId.SE_SUN].Longitude);
        //    }
        //}

        public static void RulersTest()
        {
            List<double> lowers = new List<double>{ 8.5, 7.5, 7.75, 5.125, 8.125, 3.75, 10.75, 3.5, 7.25, 8.75, 3, 5.25, 22, 70,
                44, 15, 5, 10, 6.5, 9.75, 17.875, 15.875, 3.625, 12.375, 1.875, 42.5, 27.625, 3.125, 0.5, 10.5, 29.625, 15.625, 21.125, 16.75 };

            //for (double low = 0; low < 15.0; low += 0.1 )
            foreach (double low in lowers)
            {
                Rulership rule = new Rulership(new Angle(low));

                Console.WriteLine("{0} or {1}: equals destination {2}, of house {3}, ruled by {4}({5})", 
                    low, rule.Position, rule.Remains, rule.House, rule.Ruler, rule.Ruling);
            }
        }

        public static void PolygonTest()
        {
            //foreach (Polygon shape in polygons)
            //{
            //    Console.WriteLine("---------{0}-------", shape);
            //    for (int currentIndex = 0; currentIndex < 10; currentIndex ++)
            //    {
            //        Console.WriteLine("Round {0}: Length={1}, Last={2}", currentIndex, shape.LengthOfRound(currentIndex), shape.LastOfRound(currentIndex));
            //    }

            //}

            //Angle sixty = new Angle(60.0);

            //for (double offset = -1.0; offset < 3.1; offset += 0.5)
            //{
            //    Angle offsetAngle = sixty.AngleByOffset(offset);
            //    Console.WriteLine("Offset={0}: distanceDynamic ={1}", offset, offsetAngle);
            //    for (int currentIndex = 0; currentIndex < 6; currentIndex ++ )
            //    {
            //        Angle record = new Angle(currentIndex * 60.0);
            //        Angle endDegree = record + sixty;
            //        offsetAngle = Angle.FromOffsetBetween(record, endDegree, offset);
            //        Console.WriteLine("Start={0} & End={1}: distanceDynamic ={2}", record, endDegree, offsetAngle);
            //    }
            //    Console.WriteLine("------");
            //}

            //Console.WriteLine();

            //for (double indexOfSource = 9.5; indexOfSource < 100.0; indexOfSource += 0.5)
            //{
            //    indexOfSource = indexOfSource.Round(2);
            //    int cycleNum = SquareNine.RoundOf(indexOfSource);
            //    double Position = SquareNine.DegreesOf(indexOfSource);
            //    double Degrees2 = Polygon.SquareNine.AngleOf(indexOfSource).Position;
            //    double reversed = SquareNine.indexOf(cycleNum, Position).Round(2);

            //    if (reversed != indexOfSource || Math.Abs(Degrees2 - Position) > 0.00001)
            //        Console.WriteLine("{0}: in cycle {1}, Position = {2}, Degrees2 = {3}, back destination {4}", indexOfSource, cycleNum, Position, Degrees2, reversed);
            //}

            //for (int currentIndex = 1; currentIndex < 19; currentIndex += 2)
            //{
            //    for (double indexOfSource = currentIndex * currentIndex; indexOfSource < currentIndex * currentIndex + 2.3; indexOfSource += 0.1)
            //    {
            //        indexOfSource = indexOfSource.Round(2);
            //        int cycleNum = SquareNine.RoundOf(indexOfSource);
            //        double distanceDynamic = SquareNine.DegreesOf(indexOfSource);
            //        double reversed = SquareNine.indexOf(cycleNum, distanceDynamic);

            //        //if (reversed != indexOfSource)
            //        Console.WriteLine("{0}: in cycle {1}, Position = {2}, back destination {3}", indexOfSource, cycleNum, distanceDynamic, reversed);

            //    }
            //}
        }

        public static void AngleTest1(Angle sector)
        {
            for (double degree = sector.Degrees / 2 - 85; degree < 85 - sector.Degrees / 2; degree += 5)
            {
                double offset = sector.OffsetFromDegree(degree);

                Console.WriteLine("Angle={0}, offset={1}", degree, offset);
            }
        }

        public static void AngleTest2(Angle start, Angle end)
        {
            Angle sector = (start - end).Reference;
            Angle middle = Angle.NearMiddleOf(start, end);

            for (double delta = sector.Degrees / 2 - 85; delta < 90- sector.Degrees/2; delta += 5)
            {
                Angle angle = new Angle(start.Degrees + delta);
                double offset = Angle.FromAngleBetween(start, end, delta);

                Console.WriteLine("Start={0}, End={1}, Angle={2}: delta={3}, offset={4}",start, end, angle, delta, offset);
            }
        }

        public static void TestReverse(Polygon shape)
        {
            //Console.WriteLine("\r\n{0}-------", shape);
            for (double index = 8.8; index < 61.0; index = (index+0.1).Round(1))
            {
                int cycle = shape.RoundOf(index);
                Angle angle = shape.AngleOf(index);
                double reverse = shape.IndexOf(cycle, angle).Round(2);

                if (reverse != index)
                    Console.WriteLine("{0}: in cycle {1}, Angle = {2}, back destination {3}", index, cycle, angle, reverse);
                //if (shape == Polygon.SquareNine)
                //{
                //    double reverse2 = SquareNine.indexOf(cycleNum, Position).Round(2);

                //    //if (reverse != indexOfSource || Math.Abs(Degrees2 - Position) > 0.00001)
                //        Console.WriteLine("{0}: in cycle {1}, Angle = {2}, back destination {3}", indexOfSource, cycleNum, distanceDynamic, reverse);
                //}

                //Console.WriteLine("{0}: in cycle {1}, distanceDynamic={2}, reversed back destination {3}", indexOfSource, cycle, distanceDynamic, reverse);
            }
        }

        public static void DateTest()
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            Console.WriteLine("Now is " + utcNow.ToString());

            //double julday = Utilities.swe_julday(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.TimeOfDay.TotalHours, 1);
            //Console.WriteLine(julday={0}", julday);

            double jd = Utilities.ToJulianDay(utcNow);

            Console.WriteLine("jd={0}", jd);

            //int year, month, day;
            //double utTime;

            //Utilities.swe_revjul(julday, 1, out year, out month, out day, out utTime);
            //TimeSpan inHours = TimeSpan.FromHours(utTime);
            //DateTime utc1 = new DateTime(year, month, day, inHours.Hours, inHours.Minutes, inHours.Seconds, inHours.Milliseconds);
            //DateTimeOffset utcMoment1 = new DateTimeOffset(utc1, TimeSpan.Zero);
            //Console.WriteLine("Utc1={0}", utcMoment1);

            DateTimeOffset utcMoment2 = Utilities.UtcFromJulianDay(jd);

            Console.WriteLine("Utc2={0}", utcMoment2);

        }

        public static void ImportTest()
        {
            string fileName = @"C:\Documents and Settings\snjp7464\My Documents\HisotryData\Wheat\W86K.txt";

            List<Quote> items = TextImporter.Import(fileName);

            foreach (Quote item in items)
            {
                Console.WriteLine(item.ToString());
            }
        }

        private static void TestPolygon()
        {
            Console.WriteLine("Test of NumberHelper.");

            List<Polygon> polygons = new List<Polygon> { 
                Polygon.SquareNine
                , 
                Polygon.Triangle
                , 
                Polygon.Hexagon
                , 
                Polygon.SquareFour
                , 
                Polygon.Octagon
                ,
                Polygon.Circle24
                ,
                Polygon.Circle360
            };

            foreach (Polygon shape in polygons)
            {
                Console.WriteLine("\r\nTry using {0}:", shape);

                RelativityTest(shape);
                Console.ReadKey();
                //TestReverse(shape);
                //for (int currentIndex = 0; currentIndex < shape.Edges; currentIndex ++ )
                //{
                //    AngleTest2(shape.Vertices[currentIndex], shape.Vertices[currentIndex + 1]);
                //}
            }
        }

        private static void RelativityTest(Polygon shape)
        {
            List<Double> numbers = new List<double> 
            { 104.96, 1429.01, 386.85, 1558.95, 325.89, 1510.17, 1025.13, 
                1047.83, 2245.43, 1339.2, 1783.01, 998.23, 1165.67, 6124.04, 1664.93, 1814.75,
                3478.01, 2639.76, 3361.39, 3181.66};
            //List<Double> numbers = new List<double> { 44.0, 45, 46, 65.0, 122.0, 90.0, 210.0, 211.0, 313.0, 315.0, 330, 360, 388, 403, 450, -20, -30, -330 };

            List<Angle> angles = ( from number in numbers
                                 select shape.AngleOf(number) ).ToList();

            StringBuilder sb = new StringBuilder();

            Aspects aspect;

            Console.WriteLine("Special Angles: ");
            for (int i = 0; i < angles.Count - 1; i++)
            {
                if (shape.IsOutstanding(angles[i]))
                {
                    Console.WriteLine("{0} @ {1}", numbers[i], angles[i]);
                }
            }

            for (int i = 0; i < angles.Count - 1; i++)
            {
                for (int j = i + 1; j < angles.Count; j++)
                {
                    aspect = shape.AspectBetween(angles[i], angles[j]);

                    if (aspect != null)
                    {
                        Angle theAngle = angles[j] - angles[i];

                        Double orb = theAngle.Degrees - aspect.Degrees;

                        sb.AppendFormat("{0}{1} vs {2}{3}: {4}, Orb={5}\r\n",
                            numbers[i], angles[i], numbers[j], angles[j], aspect, orb);
                    }
                }
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
