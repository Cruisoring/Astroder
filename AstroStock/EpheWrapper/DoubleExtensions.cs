using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Convert the degrees to range of 0.0 - maximum
        /// </summary>
        /// <param name="degrees">degrees need to be normalized</param>
        /// <param name="minimum">Minimum value expected, default as 0</param>
        /// <param name="maximum">Maximum value expected, default as 360 </param>
        /// <returns></returns>
        public static double Normalize(this double degrees, int minimum, int maximum)
        {
            if (degrees <= maximum && degrees >= minimum)
                return degrees;
            else
            {
                //Normalize the value to range between minimum and maximum
                int range = (maximum - minimum);
                double round = Math.Floor((degrees - minimum) / range);

                return degrees - round * range;
            }
        }

        public static double Normalize(this double degrees, int maximum)
        {

            if (degrees <= maximum && degrees >= 0)
                return degrees;
            else 
            {
                double round = Math.Floor(degrees / maximum);
                return degrees - maximum * round;
            }
        }

        public static double Normalize(this double degrees)
        {
            return Normalize(degrees, 360);
        }

        public static double Round(this double original, int decimals)
        {
            return Math.Round(original, decimals);
        }
    }
}
