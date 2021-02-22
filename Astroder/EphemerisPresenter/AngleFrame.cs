using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZedGraph;
using System.Drawing;

namespace EphemerisCalculator
{
    public delegate Double DegreeConverterDelegate(Double degrees);

    public delegate Double AngleConverterDelegate(Angle angle);

    public delegate void TransitionsHandler(List<Double> dateValues);

    public class AngleFrame
    {
        #region Static definitions
        public static Dictionary<String, AngleFrame> AllPresenters = new Dictionary<string,AngleFrame>();

        public static AngleFrame Natural = new AngleFrame(360, false, false);         //Conjuction Detector
        public static AngleFrame HalfNatural = new AngleFrame(180, false, false);     //Opposition Detector
        public static AngleFrame QuadrantNatural = new AngleFrame(90, false, false);  //Square Detector
        public static AngleFrame TrineNatural = new AngleFrame(120, false, false);    //Trine Detector
        public static AngleFrame SexitileNatural = new AngleFrame(60, false, false);  //Sextile as well as Trine Detector
        public static AngleFrame SemiSexitileNatural = new AngleFrame(30, false, false);  //Sextile as well as Trine Detector
        public static AngleFrame TwentyFourNatural = new AngleFrame(24, false, false);

        public static AngleFrame HorizontalMirror = new AngleFrame(180, false, true); //Horizontal mirror detector
        public static AngleFrame VerticalMirror = new AngleFrame(180, true, true);    //Vertical Mirror detector
        public static AngleFrame CornerMirror = new AngleFrame(90, true, true);       //Corner Mirror detector
        public static AngleFrame MutableSignMirror = new AngleFrame(60, false, true); //Mirror to Mutable signs
        public static AngleFrame FixedSignMirror = new AngleFrame(60, true, true);    //Mirror to Fixed signs

        static AngleFrame()
        {
            Type mapperType = typeof(AngleFrame);
            FieldInfo[] fields = mapperType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                AngleFrame mapper = fi.GetValue(null) as AngleFrame;

                if (mapper != null)
                    AllPresenters.Add(fi.Name, mapper);
            }

        }
        #endregion

        //public static event TransitionsHandler TransitionsChanged;

        public static List<Double> Transitions = new List<Double>();

        #region Fields
        public Double Range { get; private set; }

        public Double Max { get; private set; }

        public Double Min { get; private set; }

        public Double Step { get; private set; }

        public bool IsMirrored { get; private set; }

        public bool IsContinuous { get; private set; }

        #endregion

        private AngleFrame(Double range, bool isMirrored, bool isContinuous)
        {
            if (360 % range != 0)
                throw new Exception();

            Range = range;
            IsMirrored = isMirrored;
            Max = IsMirrored ? Range / 2 : Range;
            Min = IsMirrored ? -Range / 2 : 0;
            if (Range % 12 == 0)
                Step = Range / 12;
            else if (Range % 8 == 0)
                Step = Range / 8;
            else if (Range % 6 == 0)
                Step = Range / 6;
            else if (Range % 4 == 0)
                Step = Range / 4;
            else
                Step = 10;

            IsContinuous = isContinuous;

            if (isMirrored)
                UnifiedDegrees = IsContinuous ? new DegreeConverterDelegate(mirroredContinuousConverting)
                    : new DegreeConverterDelegate(mirroredUncontinuousConverting);
            else
                UnifiedDegrees = IsContinuous ? new DegreeConverterDelegate(unmirroredContinuousConverting) :
                    new DegreeConverterDelegate(unmirroredUncontinuousConverting);
        }

        #region Functions

        public DegreeConverterDelegate UnifiedDegrees = null;

        public Double UnifiedDegree(Angle angle)
        {
            return UnifiedDegrees(angle.Degrees);
        }

        public LineItem CurveOf(String label, List<Double> originalX, List<Double> originalY, System.Drawing.Color color, SymbolType symbol)
        {
            return CurveOf(label, originalX, originalY, color, symbol, false);
        }

        public LineItem CurveOf(String label, List<Double> originalX, List<Double> originalY, System.Drawing.Color color, SymbolType symbol, bool noSmoothing)
        {
            if (originalX.Count != originalY.Count)
                throw new Exception();

            Transitions.Clear();

            List<Double> xList = new List<double>();
            List<Double> yList = new List<double>();

            double yLast, yNext, yTemp, yTemp2, xInserted;

            xList.Add(originalX[0]);
            yLast = UnifiedDegrees(originalY[0]);
            yList.Add(yLast);

            for (int i = 1; i < originalY.Count; i++)
            {
                yNext = UnifiedDegrees(originalY[i]);

                double before = originalY[i - 1];
                double after = originalY[i];
                if (noSmoothing || !needShift(before, after))
                {
                    xList.Add(originalX[i]);
                    yList.Add(yNext);
                    yLast = yNext;
                }
                else
                {
                    double elapsed = originalX[i] - originalX[i - 1];
                    yTemp = turningBetween(before, after);
                    yTemp2 = UnifiedDegrees(yTemp);
                    //needShift(before, after);
                    xInserted = originalX[i - 1] + (yTemp - before) / ((360 + after - before) % 360) * elapsed;
                    if (!IsContinuous)
                    {
                        xList.Add(xInserted);
                        yList.Add(Math.Abs(yTemp - yLast) < Math.Abs(yTemp2 - yLast) ? yTemp : yTemp2);
                    }
                    xList.Add(xInserted);
                    Transitions.Add(xInserted);
                    yList.Add(Math.Abs(yTemp - yNext) < Math.Abs(yTemp2 - yNext) ? yTemp : yTemp2);
                    xList.Add(originalX[i]);
                    yList.Add(yNext);
                    yLast = yNext;
                }
            }

            //if (TransitionsChanged != null && label == "*")
            //    TransitionsChanged(Transitions);

            return new LineItem(label, xList.ToArray(), yList.ToArray(), color, SymbolType.None);
        }

        public LineItem CurveOf(OrbitSpec spec)
        {
            LineItem curve = CurveOf(spec.Label, spec.XList, spec.YList, spec.DisplayColor, SymbolType.None);
            return curve;
        }

        public LineItem CurveOf(TimeAngleConverter clock, DateTimeOffset since, DateTimeOffset until)
        {
            if (clock == null)
                return null;

            List<Double> dateValues = new List<Double>();
            List<Double> clockValues = new List<Double>();
            double clockDegree;

            for (DateTimeOffset date = since; date <= until; date += TimeSpan.FromDays(1))
            {
                dateValues.Add(date.UtcDateTime.ToOADate());
                clockDegree = clock.DegreeOf(date);
                clockValues.Add(clockDegree);
            }

            return CurveOf("A", dateValues, clockValues, clock.DisplayColor, SymbolType.None);
        }

        public LineItem CurveOf(QuoteHelper.PriceAdapter PriceTranslator, List<Double> dateValues, List<Double> prices, Polygon cycleMapper, bool noSmoothing)
        {
            if (PriceTranslator == null)
                return null;

            List<Double> priceValues = new List<Double>();
            double priceDegree;
            Double factor = Range / 360;

            foreach (Double price in prices)
            {
                if (PriceTranslator.Rule == QuoteHelper.PriceMappingRules.Filled || PriceTranslator.Rule == QuoteHelper.PriceMappingRules.FilledPivots)
                {
                    priceDegree = PriceTranslator.IndexOf(price).Normalize();
                    priceDegree = factor * priceDegree + Min;
                }
                else
                {
                    double index = PriceTranslator.IndexOf(price);
                    priceDegree = cycleMapper.AngleOf(index).Degrees;
                }
                priceValues.Add(priceDegree);
            }

            return CurveOf("$", dateValues, priceValues, System.Drawing.Color.DarkGray, SymbolType.Triangle, noSmoothing);
        }        

        //public LineItem CurveOf(QuoteHelper.PriceAngleConverter PriceTranslator, List<QuoteHelper.OutlineItem> items)
        //{
        //    if (PriceTranslator == null)
        //        return null;

        //    List<Double> dateValues = new List<Double>();
        //    List<Double> priceValues = new List<Double>();
        //    double priceDegree;

        //    foreach (QuoteHelper.OutlineItem item in items)
        //    {
        //        DateTimeOffset around = item.Date;
        //        dateValues.Add(around.UtcDateTime.ToOADate());
        //        priceDegree = PriceTranslator.DegreeOf(item.Price);
        //        priceValues.Add(priceDegree);
        //    }

        //    return CurveOf("$", dateValues, priceValues, System.Drawing.Color.Black, SymbolType.Triangle);
        //}

        private bool ofSameSector(double before, double after)
        {
            int sectorBefore = (int)(before * 2 / Range);
            int sectorAfter = (int)(after * 2 / Range);

            return (sectorBefore == sectorAfter);
        }

        private bool needShift(double before, double after)
        {
            if (ofSameSector(before, after))
                return false;

            double min = relativeMinOf(before, after);

            int sectorMin = (int)(min * 2 / Range) % 2;

            return IsMirrored ? sectorMin == 0 : sectorMin == 1;
        }

        private double relativeMaxOf(double before, double after)
        {
            Double delta = after - before;
            if (delta > 300)
                return before + 360;
            else if (delta < -300)
                return after + 360;
            else
                return delta > 0 ? after : before;
        }

        private double relativeMinOf(double before, double after)
        {
            Double delta = after - before;
            if (delta > 300)
                return after;
            else if (delta < -300)
                return before;
            else
                return delta < 0 ? after : before;
        }

        private double turningBetween(double before, double after)
        {
            double max = relativeMaxOf(before, after);
            double min = relativeMinOf(before, after);

            double turning = (int)(max * 2 / Range) * Range / 2;

            if (turning <= max && turning >= min)
                return turning;
            else
                throw new Exception();
        }

        private Double mirroredContinuousConverting(Double degrees)
        {
            int sector = (int)(degrees / Max);

            switch(sector%4)
            {
                case 0:
                    return degrees % Max;
                case 1:
                case 2:
                    return (((sector+1) / 2)) * Range - degrees;
                case 3:
                    return degrees - (sector+1) / 2 * Range;
                default:
                    throw new Exception();
            }
        }

        private Double mirroredUncontinuousConverting(Double degrees)
        {
            return (degrees + Max) % Range - Max;
        }

        private Double unmirroredUncontinuousConverting(Double degrees)
        {
            return degrees % Range;
        }

        private Double unmirroredContinuousConverting(Double degrees)
        {
            int sector = (int)(degrees / Range);

            switch(sector % 2)
            {
                case 0:
                    return (degrees % (Range * 2));
                case 1:
                    return (sector + 1) * Range - degrees;
                default:
                    throw new Exception();
            }
        }

        #endregion
    }
}
