using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    public class SolarEclipse : EclipseOccultation
    {
        public DateTimeOffset TimeOfFirstContact { get { return Utilities.UtcFromJulianDay(tret[1]); } }

        public DateTimeOffset TimeOfSecondContact { get { return Utilities.UtcFromJulianDay(tret[2]); } }

        public DateTimeOffset TimeOfThirdContact { get { return Utilities.UtcFromJulianDay(tret[3]); } }

        public DateTimeOffset TimeOfFourthContact { get { return Utilities.UtcFromJulianDay(tret[4]); } }

        public Double FractionCovered { get { return attr[0]; } }
        public Double LunarDiameterRatio { get { return attr[1]; } }
        public Double FractionObscuration { get { return attr[2]; } }
        public Double CoreShadowDiameterInKm { get { return attr[3]; } }
        public Double SunAzimuth { get { return attr[4]; } }
        public Double SunTrueAltitude { get { return attr[5]; } }
        public Double SunApparentAltitude { get { return attr[6]; } }
        public Double AngularDistance { get { return attr[7]; } }
        public Double EclipseMagnitude { get { return attr[8]; } }
        public Double SorosSeriesNumber { get { return attr[9]; } }
        public Double SorosSeriesMemberNumber { get { return attr[10]; } }

        public override string EclipseType
        {
            get 
            { 
                if (Result == EclipseFlag.ERR)
                    return "Error"; 
                else
                {
                    //EclipseFlag type = (EclipseFlag.SE_ECL_ALLTYPES_SOLAR & Result) & EclipseFlag.SE_ECL_TOTAL & EclipseFlag.SE_ECL_ANNULAR & EclipseFlag.SE_ECL_PARTIAL & EclipseFlag.SE_ECL_ANNULAR_TOTAL;
                    EclipseFlag type = (EclipseFlag.SOLAR_TYPE_MASK & Result) ;

                    String descritpion = type.ToString().Substring(7);
                    return descritpion + " Solar Eclipse";
                }
            }
        }

        public override string ToString()
        {
            return String.Format(EclipseType + ": " + base.ToString());
        }
    }
}
