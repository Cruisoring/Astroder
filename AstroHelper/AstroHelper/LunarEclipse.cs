using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    public class LunarEclipse : EclipseOccultation
    {
        //public double[] geoPos { get; protected set; }

        //public double[] tret { get; protected set; }

        //public double[] attr { get; protected set; }

        //public String ErrorMessage { get; set; }

        //public EclipseFlag Result { get; set; }

        //public double Longitude { get { return geoPos[0]; } }

        //public double Latitude { get { return geoPos[1]; } }

        //public double Height { get { return geoPos[2]; } }

        //public DateTimeOffset TimeOfMaximumEclipse { get { return Utilities.UtcFromJulianDay(tret[0]); } }
        public DateTimeOffset TimeOfPartialPhaseBegin { get { return Utilities.UtcFromJulianDay(tret[2]); } }
        public DateTimeOffset TimeOfPartialPhaseEnd { get { return Utilities.UtcFromJulianDay(tret[3]); } }
        public DateTimeOffset TimeOfTotalityBegin { get { return Utilities.UtcFromJulianDay(tret[4]); } }
        public DateTimeOffset TimeOfTotalityEnd { get { return Utilities.UtcFromJulianDay(tret[5]); } }
        public DateTimeOffset TimeOfPenumbralBegin { get { return Utilities.UtcFromJulianDay(tret[6]); } }
        public DateTimeOffset TimeOfPenumbralEnd { get { return Utilities.UtcFromJulianDay(tret[7]); } }

        public Double UmbralMagnitude { get { return attr[0]; } }
        public Double PenumbralMagnitude { get { return attr[1]; } }
        public Double MoonAzimuth { get { return attr[4]; } }
        public Double MoonTrueAltitude { get { return attr[5]; } }
        public Double MoonApparentAltitude { get { return attr[6]; } }
        public Double MoonDistanceFromOpposition { get { return attr[7]; } }
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
                    EclipseFlag type = EclipseFlag.SE_ECL_ALLTYPES_LUNAR & Result;
                    String descritpion = type.ToString().Substring(7);
                    return descritpion + " Lunar Eclipse";
                }
            }
        }

        public override string ToString()
        {
            return String.Format(EclipseType + ": " + base.ToString());
        }

    }

}
