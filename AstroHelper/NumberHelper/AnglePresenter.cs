using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper.DoubleHelper;
using System.Reflection;

namespace NumberHelper
{
    public delegate Double DegreeConverterDelegate(Double degrees);

    public delegate Double AngleConverterDelegate(Angle angle);

    public class AnglePresenter
    {
        #region Static definitions
        public static List<AnglePresenter> AllMappers = new List<AnglePresenter>();

        public static AnglePresenter Natural = new AnglePresenter(360, false, false);         //Conjuction Detector
        public static AnglePresenter HalfNatural = new AnglePresenter(180, false, false);     //Opposition Detector
        public static AnglePresenter QuadrantNatural = new AnglePresenter(90, false, false);  //Square Detector
        public static AnglePresenter TrineNatural = new AnglePresenter(120, false, false);    //Trine Detector
        public static AnglePresenter SexitileNatural = new AnglePresenter(60, false, false);  //Sextile as well as Trine Detector

        public static AnglePresenter HorizontalMirror = new AnglePresenter(180, false, true); //Horizontal mirror detector
        public static AnglePresenter VerticalMirror = new AnglePresenter(180, true, true);    //Vertical Mirror detector
        public static AnglePresenter CornerMirror = new AnglePresenter(90, true, true);       //Corner Mirror detector
        public static AnglePresenter MutableSignMirror = new AnglePresenter(60, false, true); //Mirror to Mutable signs
        public static AnglePresenter FixedSignMirror = new AnglePresenter(60, true, true);    //Mirror to Fixed signs

        static AnglePresenter()
        {
            Type mapperType = typeof(AnglePresenter);
            FieldInfo[] fields = mapperType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                AnglePresenter mapper = fi.GetValue(null) as AnglePresenter;

                if (mapper != null)
                    AllMappers.Add(mapper);
            }

        }
        #endregion

        public Double Range { get; private set; }

        public Double Max { get; private set; }

        public Double Min { get; private set; }

        public Double Step { get { return (Range / 4); } }

        public bool IsMirrored { get; private set; }

        public bool IsContinuous { get; private set; }

        public DegreeConverterDelegate UnifiedDegrees = null;

        public Double UnifiedDegree(Angle angle)
        {
            return UnifiedDegrees(angle.Degrees);
        }

        public Double[] UnifiedArrayOf(IEnumerable<Double> degrees)
        {
            List<Double> result = new List<Double>();

            foreach (Double degree in degrees)
            {
                result.Add(UnifiedDegrees(degree));
            }

            return result.ToArray();
        }


        private AnglePresenter(Double range, bool isMirrored, bool isContinuous)
        {
            if (360 % range != 0)
                throw new Exception();

            Range = range;
            IsMirrored = isMirrored;
            Max = IsMirrored ? Range/2 : Range;
            Min = IsMirrored ? -Range/2 : 0;

            IsContinuous = isContinuous;

            if (isMirrored)
                UnifiedDegrees = IsContinuous ? new DegreeConverterDelegate(mirroredContinuousConverting)
                    : new DegreeConverterDelegate(mirroredUncontinuousConverting);
            else
                UnifiedDegrees = IsContinuous ? new DegreeConverterDelegate(unmirroredContinuousConverting) : 
                    new DegreeConverterDelegate(unmirroredUncontinuousConverting);
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
    }
}
