using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using NumberHelper.DoubleHelper;

namespace AstroHelper
{
    /// <summary>
    /// Containing the coordinates of destination and velocity of a celestial body
    /// </summary>
    [Serializable]
    public class Position : IFormattable
    {
        public PlanetId Owner { get; private set; }

        private double[] values = null;
        /// <summary>
        /// the coordinates of destination and velocity in the following order:
        /// Rectascension	Rectascension
        /// Declination     Latitude
        /// Distance in AU	distanceDynamic in AU
        /// Speed on longitude (degs/day)	Speed in rectascension (degs/day)
        /// Speed on latitude (degs/day)	Speed in declination (degs/day)
        /// Speed on distanceDynamic (AU/day)	    Speed in distanceDynamic (AU/day)
        /// </summary>
        public Double Longitude 
        {
            get { return values[0]; }
        }

        public Double Latitude
        {
            get { return values[1]; }
        }

        public Double Distance
        {
            get { return values[2]; }
        }

        public Double LongitudeVelocity
        {
            get { return values[3]; }
        }

        public Double LatitudeVelocity
        {
            get { return values[4]; }
        }

        public Double DistanceVelocity
        {
            get { return values[5]; }
        }

        #region Constructors
        public Position(PlanetId id, double[] result)
        {
            Owner = id;
            values = result;
        }
        #endregion

        #region Functions

        public override string ToString()
        {
            return String.Format("[{0}:{1}, Speed={2:F4}]", Planet.SymbolOf(Owner), Longitude.AstrologyFormat(), LongitudeVelocity);
        }

        public static Angle operator -(Position posA, Position posB)
        {
            return new Angle(posA.Longitude - posB.Longitude);
        }
        #endregion

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("{0}: {1}({2:F3})", Planet.SymbolOf(Owner), Rectascension.AngleFormatOf(Longitude, format), LongitudeVelocity);
        }

        #endregion
    }
}
