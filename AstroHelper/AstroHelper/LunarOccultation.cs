using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    public class LunarOccultation : EclipseOccultation
    {
        public PlanetId Occulted { get; protected set; }

        public DateTimeOffset TimeOfEclipseAtLocal { get { return Utilities.UtcFromJulianDay(tret[1]);} }
        public DateTimeOffset TimeOfEclipseBegin { get { return Utilities.UtcFromJulianDay(tret[2]); } }
        public DateTimeOffset TimeOfEclipseEnd { get { return Utilities.UtcFromJulianDay(tret[3]); } }
        public DateTimeOffset TimeOfTotalityBegin { get { return Utilities.UtcFromJulianDay(tret[4]); } }
        public DateTimeOffset TimeOfTotalityEnd { get { return Utilities.UtcFromJulianDay(tret[5]); } }
        public DateTimeOffset TimeOfCenterLineBegin { get { return Utilities.UtcFromJulianDay(tret[6]); } }
        public DateTimeOffset TimeOfCenterLineEnd { get { return Utilities.UtcFromJulianDay(tret[7]); } }

        public Double FractionCovered { get { return attr[0]; } }
        public Double LunarDiameterRatio { get { return attr[1]; } }
        public Double FractionObscuration { get { return attr[2]; } }
        public Double CoreShadowDiameterInKm { get { return attr[3]; } }
        public Double SunAzimuth { get { return attr[4]; } }
        public Double SunTrueAltitude { get { return attr[5]; } }
        public Double SunApparentAltitude { get { return attr[6]; } }
        public Double AngularDistance { get { return attr[7]; } }

        public LunarOccultation(PlanetId occulted) : base()
        {
            Occulted = occulted;
        }

        public override string EclipseType
        {
            get
            {
                if (Result == EclipseFlag.ERR)
                    return "Error";
                else if (Result == 0)
                    return "None";
                else
                {
                    return "Occultation of " + Occulted.ToString();
                }
            }
        }

        public override string ToString()
        {
            return String.Format(EclipseType + ": " + base.ToString());
        }
    }

}
