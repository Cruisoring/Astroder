using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EphemerisCalculator
{
    public enum PositionValueIndex
    {
        Longitude = 0,
        Latitude = 1,
        Distance = 2,
        LongitudeVelocity = 3,
        LatitudeVelocity = 4,
        DistanceVelocity = 5
    }


    /// <summary>
    /// Containing the coordinates of aspectDegree and velocity of a celestial body
    /// </summary>
    [Serializable]
    public class Position : IFormattable
    {
        public PlanetId Owner { get; private set; }

        /// <summary>
        /// the coordinates of aspectDegree and velocity in the following order:
        ///     Rectascension	Rectascension
        ///     Declination     Latitude
        ///     Apparent in AU	distanceDynamic in AU
        ///     Speed on longitude (degs/day)	Speed in dif (degs/day)
        ///     Speed on latitude (degs/day)	Speed in declination (degs/day)
        ///     Speed on distanceDynamic (AU/day)	    Speed in distanceDynamic (AU/day)
        /// </summary>
        private double[] values = null;

        public Double[] Values { get { return values; } }

        public Double Longitude 
        {
            get { return values[0]; }
        }

        public Rectascension TheRectascension
        {
            get { return new Rectascension(values[0]); }
        }

        public Double Latitude
        {
            get { return values[1]; }
        }

        public Declination TheDeclination
        {
            get { return new Declination(values[1]); }
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

        public double this[int index] { get { return values[index]; } }

        public double this[PositionValueIndex index] { get { return values[(int)index]; } }

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
            return String.Format("[{0}:{1}, Speed={2:F4}]", Planet.All[Owner], Rectascension.AstroStringOf(Longitude), LongitudeVelocity);
        }

        public static Angle operator -(Position posA, Position posB)
        {
            return new Angle(posA.Longitude - posB.Longitude);
        }
        #endregion

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("{0}: {1}({2:F3})", Planet.SymbolOf(Owner), Rectascension.FormattedStringOf(Longitude, format), LongitudeVelocity);
        }

        #endregion
    }

}
