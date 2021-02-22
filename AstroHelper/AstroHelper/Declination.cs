using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;

namespace AstroHelper
{
    public enum RelationToEquator
    {
        OnEquator,
        North,
        South
    }

    [Serializable]
    public class Declination : Angle
    {
        #region Static prices & functions
        public const double Permissible = 0.01; //Maximum Permissible Error in degrees
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new Declination with input in degrees.
        /// </summary>
        /// <param name="degrees">Position of the declination</param>
        public Declination(double degrees)
        {
            if (degrees <= 90.0 && degrees >= -90.0)
                Degrees = degrees;
            else //if (degrees > 90 || degrees < -90)
                throw new ArgumentOutOfRangeException("The Declination shall be within -90 and +90 degrees!");
        }

        #endregion

        #region Properties
        public RelationToEquator NorthOrSouth
        {
            get { 
                if (Degrees > Permissible)
                    return RelationToEquator.North;
                else if (Degrees < -Permissible)
                    return RelationToEquator.South;
                else
                    return RelationToEquator.OnEquator;
            }
        }
        #endregion

        #region IComparable<Angle> 成员

        public new int CompareTo(Angle other)
        {
            return this.Degrees == other.Degrees ? 0 : (Degrees > other.Degrees ? 1 : -1);
        }

        #endregion

        #region IFormattable 成员

        /// <summary>
        /// Get coordinates as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char sign = Degrees > 0 ? '+' : (Degrees < 0 ? '-' : ' ');
            return String.Format("<{0}{1:F2}>", sign, Math.Abs(Degrees));
        }

        #endregion

        #region Other functions
        public static Declination operator +(Declination lhs, Angle rhs)
        {
            return new Declination(lhs.Degrees + rhs.Degrees);
        }

        public static Declination operator -(Declination lhs, Angle rhs)
        {
            return new Declination(lhs.Degrees - rhs.Degrees);
        }

        //public static Angle operator -(Declination startDegree, Declination endDegree)
        //{
        //    return new Angle(startDegree.Position - endDegree.Position);
        //}

        #endregion

    }
}
