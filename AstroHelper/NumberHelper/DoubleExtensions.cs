using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberHelper.DoubleHelper
{
    public static class DoubleExtensions
    {
        public const int DefaultDecimals = 2;

        /// <summary>
        /// Convert the Degrees to range of 0.0 - maximum
        /// </summary>
        /// <param name="Degrees">Degrees need to be normalized</param>
        /// <param name="minimum">FirstIndex value expected, default as 0</param>
        /// <param name="maximum">NextIndex value expected, default as 360 </param>
        /// <returns></returns>
        public static double Normalize(this double degrees, int minimum, int maximum)
        {
            if (degrees < maximum && degrees >= minimum)
                return degrees;
            else
            {
                //Normalize the value to range between minimum and maximum
                int range = (maximum - minimum);
                double round = Math.Floor((degrees - minimum) / maximum);

                return degrees - round * range;
            }
        }

        public static double Normalize(this double degrees, int maximum)
        {

            if (degrees < maximum && degrees >= 0)
                return degrees;
            else
            {
                double round = Math.Floor(degrees / maximum) ;
                return degrees - maximum * round;
            }
        }

        public static double Normalize(this double degrees)
        {
            if (degrees < 360.0 && degrees >= 0)
                return degrees;
            else
            {
                //double round = Math.Floor(degrees / 360);
                //return degrees - 360.0 * round;
                degrees %= 360;

                if (degrees < 0)
                    degrees += 360;
                return degrees;
            }
        }

        public static double Round(this double original, int decimals)
        {
            return Math.Round(original, decimals);
        }

        public static double Round(this double original)
        {
            return Round(original, DefaultDecimals);
        }

        public static string AngleFormatOf(this double degrees, string format)
        {
            return Angle.AngleFormatOf(degrees, format);
        }

        public static string HourMinuteSecondFormat(this double degrees)
        {
            if (degrees > 360.0 || degrees < 0)
                throw new Exception();

            int hour = (int)(degrees / 15);
            double remain = degrees - hour * 15;
            double minute = (int)(remain * 4);
            remain = remain * 4 - minute;
            int seconds = (int)Math.Round(remain * 60);
            return String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, seconds);
        }

        public static Dictionary<int, String> SignAbbrevs = new Dictionary<int, String>
        {
            {0, "Ari"},
            {1, "Tau"},
            {2, "Gem"},
            {3, "Can"},
            {4, "Leo"},
            {5, "Vir"},
            {6, "Lib"},
            {7, "Sco"},
            {8, "Sag"},
            {9, "Cap"},
            {10, "Aqu"},
            {11, "Pis"}
        };

        public static string AstrologyFormat(this double degree)
        {
            double deg = degree.Normalize();
            int sign = (int)(deg / 30);
            Double offset = degree - sign * 30;
            Double minutes = (offset - (int)offset) * 60;

            return String.Format("{0:D2}{1}{2:F0}", (int)offset, SignAbbrevs[sign], minutes);
        }
    }

}
