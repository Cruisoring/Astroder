using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum GannTrendWays
    {
        Original,
        CalendarNatural,
        IndexNatural,
        OriginalNatural
    }

    public delegate GannTrend GetTrendDelegate(OutlineItem start, OutlineItem end);

    public class GannTrend
    {
        #region Static Definitions

        public static List<Double> NaturalSteps = new List<Double>() { 100, 75, 50, 25, 12.5, 10, 7.5, 5, 2.5, 1.25, 1 };

        public static List<Double> Ratios = new List<Double> { 4, 2, 1, 0.5, 0.25 };

        public static Dictionary<String, GetTrendDelegate> Methods = new Dictionary<String, GetTrendDelegate>{
            {"Trend Off", null},
            {"Original", GetOriginalTrend},
            {"Calendar Natural", GetCalendarNaturalTrend},
            {"Index Natural", GetCalendarNaturalTrend},
            {"Original Natural", GetOriginalNaturalTrend}
        };

        #endregion


        #region Fields

        public GannTrendWays Rule {get; private set; }
        
        public OutlineItem Original { get; private set; }

        public OutlineItem Destinational { get; private set; }

        public Double Rate { get; private set; }

        #region other rates
        //public Double DoubleRate { get { return Rate * 2; } }

        //public Double QuadrupleRate { get { return Rate * 4; } }

        //public Double OctupleRate { get { return Rate * 8; } }

        //public Double HalfRate { get { return Rate * 0.5; } }

        //public Double QuarterRate { get { return Rate * 0.25; } }

        //public Double OctantRate { get { return Rate * 0.125; } }

        #endregion

        #endregion

        #region Constructors

        public GannTrend(OutlineItem start, OutlineItem end, GannTrendWays way)
        {
            Rule = way;
            OutlineItem ori = null, des = null;
            double rate = 0;

            switch (Rule)
            {
                case GannTrendWays.Original:
                    getOriginal(start, end, out ori, out des, out rate);
                    break;
                case GannTrendWays.CalendarNatural:
                    getCalendarNatural(start, end, out ori, out des, out rate);
                    break;
                case GannTrendWays.IndexNatural:
                    getIndexNatural(start, end, out ori, out des, out rate);
                    break;
                case GannTrendWays.OriginalNatural:
                    getOriginalNaturalTrend(start, end, out ori, out des, out rate);
                    break;
                default:
                    break;
            }
            Original = ori;
            Destinational = des;
            Rate = rate;
        }

        #endregion

        private void getEndCoordinate(Double rate, Double xExtreme, Double yExtreme, ref Double x, ref Double y)
        {

            x = Original.DateValue + (yExtreme-Original.Price) / rate;
            
            if (x <= xExtreme)
            {
                y = yExtreme;
                return;
            }
            else
            {
                x = xExtreme;
                y = Original.Price + (xExtreme - Original.DateValue) * rate;
                return;
            }
        }

        public List<List<Double>> CoordinatesWithin(Double xExtreme, Double yExtreme)
        {
            List<double> xList = new List<double>();
            List<Double> yList = new List<Double>();
            List<List<Double>> result = new List<List<Double>>();
            double x = 0, y = 0, rate;

            foreach (double ratio in Ratios)
            {
                rate = ratio * Rate;
                getEndCoordinate(rate, xExtreme, yExtreme, ref x, ref y);
                xList.Add(x);
                yList.Add(y);
                xList.Add(Original.DateValue);
                yList.Add(Original.Price);
            }
            xList.RemoveAt(xList.Count -1);
            yList.RemoveAt(yList.Count -1);
            result.Add(xList);
            result.Add(yList);
            return result;
        }

        private static double naturalRateOf(double step)
        {
            double mag = Math.Floor(Math.Log10(Math.Abs(step))) - 2;
            double minorStep = Math.Pow(10, mag);

            double multiplier = Math.Abs(step / minorStep);
            double deviation = 100;

            foreach(Double naturalStep in NaturalSteps)
            {
                if (Math.Abs(deviation) > Math.Abs(multiplier - naturalStep))
                    deviation = multiplier - naturalStep;
            }

            return step > 0 ? (multiplier - deviation) * minorStep : (deviation - multiplier) * minorStep;
        }

        private static double naturalValueOf(Double originalValue, double step)
        {
            Double nearest = Math.Round(originalValue / step);
            return nearest * step;
        }

        private static void getOriginal(OutlineItem start, OutlineItem end, out OutlineItem original, out OutlineItem destinational, out Double rate)
        {
            rate = (end.Price - start.Price) / (end.DateValue - start.DateValue);
            original = start;
            destinational = end;
        }

        private static void getCalendarNatural(OutlineItem start, OutlineItem end, out OutlineItem original, out OutlineItem destinational, out Double rate)
        {
            Double interval = (end.DateValue - start.DateValue);
            Double tempRate = (end.Price - start.Price) / interval;
            if(Math.Abs(tempRate) > 1000000 || Math.Abs(tempRate) < 0.0000001)
                throw new Exception();

            rate = naturalRateOf(tempRate);
            original = start;
            destinational = new OutlineItem(end, original.Price + rate * interval);
        }

        private static void getIndexNatural(OutlineItem start, OutlineItem end, out OutlineItem original, out OutlineItem destinational, out Double rate)
        {
            Double interval = (end.RecordIndex - start.RecordIndex);
            Double tempRate = (end.Price - start.Price) / interval;
            if (Math.Abs(tempRate) > 10000 || Math.Abs(tempRate) < 0.00001)
                throw new Exception();

            rate = naturalRateOf(tempRate);
            original = start;
            destinational = new OutlineItem(end, original.Price + rate * interval);
        }

        private static void getOriginalNaturalTrend(OutlineItem start, OutlineItem end, out OutlineItem original, out OutlineItem destinational, out Double rate)
        {
            Double interval = (end.DateValue - start.DateValue);
            Double tempRate = (end.Price - start.Price) / interval;
            if (Math.Abs(tempRate) > 100 || Math.Abs(tempRate) < 0.0000001)
                throw new Exception();

            rate = naturalRateOf(tempRate);
            original = new OutlineItem(start, naturalValueOf(start.Price, tempRate));
            destinational = new OutlineItem(end, original.Price + rate * interval);
        }

        public static GannTrend GetOriginalTrend(OutlineItem start, OutlineItem end)
        {
            return new GannTrend(start, end, GannTrendWays.Original);
        }

        public static GannTrend GetCalendarNaturalTrend(OutlineItem start, OutlineItem end)
        {
            return new GannTrend(start, end, GannTrendWays.CalendarNatural);
        }

        public static GannTrend GetIndexNaturalTrend(OutlineItem start, OutlineItem end)
        {
            return new GannTrend(start, end, GannTrendWays.IndexNatural);
        }

        public static GannTrend GetOriginalNaturalTrend(OutlineItem start, OutlineItem end)
        {
            return new GannTrend(start, end, GannTrendWays.OriginalNatural);
        }
    }
}
