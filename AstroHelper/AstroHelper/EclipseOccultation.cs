using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    public abstract class EclipseOccultation : IComparable<EclipseOccultation>
    {
        public double[] geoPos { get; protected set; }

        public double[] tret { get; protected set; }

        public double[] attr { get; protected set; }

        public String ErrorMessage { get; set; }

        public EclipseFlag Result { get; set; }

        public DateTimeOffset TimeOfMax { get { return Utilities.UtcFromJulianDay(tret[0]); } }

        public double Longitude { get { return geoPos[0]; } }

        public double Latitude { get { return geoPos[1]; } }

        public double Height { get { return geoPos[2]; } }

        public abstract String EclipseType { get; }

        public EclipseOccultation()
        {
            geoPos = new double[10];
            tret = new double[10];
            attr = new double[20];
            ErrorMessage = "";
            Result = EclipseFlag.ERR;
        }

        #region IComparable<EclipseOccultation> 成员

        public int CompareTo(EclipseOccultation other)
        {
            if (this.TimeOfMax == other.TimeOfMax)
                return 0;
            else if (this.TimeOfMax > other.TimeOfMax)
                return 1;
            else
                return -1;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0} @ ({1:F3}{2}, {3:F3}{4})", TimeOfMax, Math.Abs(Longitude), Longitude >= 0 ? 'E' : 'W', Math.Abs(Latitude), Latitude >= 0 ? 'N' : 'S');
        }
    }
}
