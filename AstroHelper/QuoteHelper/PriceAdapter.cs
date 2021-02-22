using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using NumberHelper.DoubleHelper;
using System.Reflection;

namespace QuoteHelper
{
    public enum PriceMappingRules
    {
        Filled,
        FilledPivots,
        Contained,
        FloorNatural,
        CeilingNatural,
        FloorStep,
        Natural
    }

    //public delegate Double PriceToDegreeDelegate(double price);

    public delegate Double PriceToIndexValueDelegate(double price);

    [Serializable]
    public class PriceAdapter : IAngleable<Double>
    {
        #region Constant and static definitions
        public const int DefaultDecimals = 1;

        public static PriceAdapter PriceOff = null;
        public static PriceAdapter Natural = new PriceAdapter(PriceMappingRules.Natural);

        public static Dictionary<String, PriceAdapter> All = new Dictionary<string, PriceAdapter>();

        static PriceAdapter()
        {
            Type mapperType = typeof(PriceAdapter);
            FieldInfo[] fields = mapperType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                PriceAdapter instance = fi.GetValue(null) as PriceAdapter;

                if (fi.FieldType == typeof(PriceAdapter))
                    All.Add(fi.Name, instance);
            }
        }

        #endregion

        #region Fields
        public PriceMappingRules Rule { get; private set; }

        //public PriceToDegreeDelegate DegreeOf = null;

        public PriceToIndexValueDelegate IndexOf = null;

        public double Zero { get; set; }

        public double Step { get; set; }

        #endregion

        #region Constructors
        private PriceAdapter(PriceMappingRules rule)
        {
            if (rule != PriceMappingRules.Natural)
                throw new Exception();

            Rule = rule;
            IndexOf = new PriceToIndexValueDelegate(NatureIndexOf);
            Zero = 0;
            Step = 1;
        }

        public PriceAdapter(Double step)
        {
            Rule = PriceMappingRules.Natural;
            IndexOf = new PriceToIndexValueDelegate(steppedIndexOf);
            Zero = 0;
            Step = step;
        }

        public PriceAdapter(PriceMappingRules rule, Double floor, double ceiling)
        {
            Rule = rule;

            switch(rule)
            {
                default:
                    throw new Exception();
                case PriceMappingRules.Natural:
                    //double range = ceiling - floor;
                    //Step = StepOf(range, 3);
                    if (ceiling >= 36000)
                        Step = 100;
                    else if (ceiling >= 18000)
                        Step = 50;
                    else if (ceiling >= 7200)
                        Step = 10;
                    else if (ceiling >= 1800)
                        Step = 5;
                    else if (ceiling >= 720)
                        Step = 2;
                    else if (ceiling >= 180)
                        Step = 1;
                    else if (ceiling >= 72)
                        Step = 0.5;
                    else if (ceiling >= 18)
                        Step = 0.1;
                    else if (ceiling >= 9)
                        Step = 0.05;
                    else
                        Step = 0.01;
                    Zero = 0;
                    break;
                case PriceMappingRules.FilledPivots:
                case PriceMappingRules.Filled:
                    Zero = gracefulOf(floor, true);
                    Double max = gracefulOf(ceiling, false);
                    Step = (max - Zero) / 360;
                    break;
                case PriceMappingRules.Contained:
                    double temp = ceiling - floor;
                    Step =StepOf(temp * 3, 2);
                    Zero = Math.Round(floor / Step) * Step;
                    break;
                case PriceMappingRules.FloorNatural:
                    Step = StepOf(floor, 2);
                    Zero = Math.Round(floor / Step) * Step;
                    break;
                case PriceMappingRules.CeilingNatural:
                    Step =StepOf(ceiling, 3);
                    Zero = 0;
                    break;
                case PriceMappingRules.FloorStep:
                    Step = gracefulOf(floor);
                    Zero = Math.Round(floor/Step) * Step;
                    break;
            }

            IndexOf = new PriceToIndexValueDelegate(relativeIndexOf);
        }

        public PriceAdapter(PriceMappingRules rule, Outline outline) : this(rule, outline.Min, outline.Max) { }

        #endregion

        #region Functions

        //private Double relativeDegreeOf(Double price)
        //{
        //    return RelativeDegreeOf(price, Zero, Step);
        //}

        private Double relativeIndexOf(Double price)
        {
            return RelativeIndexOf(price, Zero, Step);
        }

        //private Double steppedDegreeOf(Double price)
        //{
        //    return SteppedNatureDegreeOf(price, Step);
        //}

        private Double steppedIndexOf(Double price)
        {
            return SteppedNatureIndexOf(price, Step);
        }

        public Angle AngleOf(Double price)
        {
            return new Angle(IndexOf(price));
        }

        #endregion

        #region Price calculation tools

        private static Double gracefulOf(double price)
        {
            double step = StepOf(price, 0);

            double temp = price / step;

            if (temp < 2)
                return step;
            else if (temp < 5)
                return step * 2;
            else if (temp < 10)
                return step * 5;
            else
                throw new Exception();
        }

        private static Double gracefulOf(double price, bool smaller)
        {
            double mag = Math.Floor(Math.Log10(price) - 1);
            double step = Math.Pow(10, mag);

            return smaller ? Math.Floor(price / step) * step : Math.Ceiling(price / step) * step;
        }

        public static Double StepOf(Double price, int digits)
        {
            double mag = Math.Floor(Math.Log10(price) - digits);
            return Math.Pow(10, mag);
        }

        public static Double LogOf(Double price, Double zero, Double logBase)
        {
            return Math.Log(price - zero, logBase);
        }

        //public static Double NatureDegreeOf(Double price)
        //{
        //    double one = StepOf(price, 1);
        //    return Math.Round(price / one, DefaultDecimals).Normalize();
        //}

        public static Double NatureIndexOf(Double price)
        {
            double one = StepOf(price, 1);
            return Math.Round(price / one, DefaultDecimals);
        }

        //public static double SteppedNatureDegreeOf(Double price, Double stepSize)
        //{
        //    return Math.Round(price / stepSize, DefaultDecimals).Normalize();
        //}

        public static double SteppedNatureIndexOf(Double price, Double stepSize)
        {
            return Math.Round(price / stepSize, DefaultDecimals);
        }

        //public static double RelativeDegreeOf(Double price, Double zero)
        //{
        //    double one = StepOf(price, 3);
        //    zero = Math.Round(zero/one, 0) * one;
        //    return Math.Round((price-zero) / one, DefaultDecimals).Normalize();
        //}

        public static double RelativeIndexOf(Double price, Double zero)
        {
            double one = StepOf(price, 3);
            zero = Math.Round(zero / one, 0) * one;
            return Math.Round((price - zero) / one, DefaultDecimals);
        }

        //public static double RelativeDegreeOf(Double price, Double zero, Double stepSize)
        //{
        //    return Math.Round((price - zero) / stepSize, DefaultDecimals).Normalize();
        //}

        public static double RelativeIndexOf(Double price, Double zero, Double stepSize)
        {
            return Math.Round((price - zero) / stepSize, DefaultDecimals);
        }

        #endregion
    }
}
