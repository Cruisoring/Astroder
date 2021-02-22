using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AstroCalc.Engineer
{
    /// <summary>
    /// Access Swiss Ephemeris
    /// </summary>
    public static class Sweph
    {
        /// <summary>
        /// Julian calendar
        /// </summary>
        public const int SE_JUL_CAL = 0;
        /// <summary>
        /// Gregorian Calendar
        /// </summary>
        public const int SE_GREG_CAL = 1;

        static Sweph()
        {
            string sedir = Environment.CurrentDirectory + "\\sweph\\";
            swe_set_ephe_path(sedir);
        }

        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_set_ephe_path")]
        private extern static void swe_set_ephe_path(String path);


        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_julday")]
        private extern static double swe_julday(int year, int month, int day, double hour, int gregflag);

        /// <summary>
        /// Access JD number
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month</param>
        /// <param name="day">day</param>
        /// <param name="hours">fractional hours</param>
        /// <param name="cal">Calendar, either SE_JUL_CAL or SE_GREG_CAL</param>
        /// <returns>jd number</returns>
        public static double getJD(int year, int month, int day, double hours, int cal)
        {
            return swe_julday(year, month, day, hours, cal);
        }

        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_revjul")]
        private extern static double ext_swe_revjul(double tjd, int gregflag, ref int year, ref int month, ref int day, ref double hour);

        private static double swe_revjul(double tjd, int gregflag, ref int year, ref int month, ref int day, ref double hour)
        {
            return ext_swe_revjul(tjd, gregflag, ref year, ref month, ref day, ref hour);
        }

        /// <summary>
        /// Returns the daynumber for a given Julain day number
        /// </summary>
        /// <param name="jdnr">The Julian Day</param>
        /// <param name="cal">Calendar used</param>
        /// <returns>The day number</returns>
        public static int getDayFromJd(double jdnr, int cal)
        {
            int day = 0, month = 0, year = 0;
            double hour = 0;
            swe_revjul(jdnr, cal, ref year, ref month, ref day, ref hour);
            return day;
        }


        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_calc")]
        private extern static long ext_swe_calc(double jdnr, int index, int y, double[] x, String serr);
        /// <summary>
        /// Access Obliquity
        /// </summary>
        /// <param name="jdnr">Julian Day number</param>
        /// <returns>Mean obliquity</returns>
        public static double getObliquity(double jdnr)
        {
            double[] x = new double[6];
            String serr = "";
            long iflgret = ext_swe_calc(jdnr, Constants.SE_ECL_NUT, 0, x, serr);
            return x[1];  // mean obliquity
        }


        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_calc_ut")]
        private extern static long ext_swe_calc_ut(double jdnr, int ipl, int iflag, double[] xx, String serr);

        /// <summary>
        /// Calculate planetary position
        /// </summary>
        /// <param name="ipl">Index of planetary body</param>
        /// <param name="jdnr">Julain day</param>
        /// <returns>Array with 6 doubles: 0:longitude, 1:latitude, 2:distance,3:speel in longitude, 
        ///          4: speed in latitude, 5: speed in distance </returns>
        public static double[] getPlanet(int ipl, double jdnr)
        {
            //   String ephePath = "Q:\\sweph\\";
            //   Sweph.setEphePath(ephePath);
            double[] xx2 = new double[8];
            double[] xx = new double[6];
            String serr = "";
            int iflag = Constants.SEFLG_SPEED;
            long iflgret = ext_swe_calc_ut(jdnr, ipl, iflag, xx, serr);
            for (int i = 0; i < 6; i++)
            {
                xx2[i] = xx[i];
            }
            iflag = Constants.SEFLG_SWIEPH | Constants.SEFLG_SPEED | Constants.SEFLG_EQUATORIAL;
            iflgret = ext_swe_calc_ut(jdnr, ipl, iflag, xx, serr);
            xx2[6] = xx[0];
            xx2[7] = xx[1];

            return xx2;
        }

        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_houses")]
        private extern static int ext_swe_houses(double jdnr, double lat, double lon, int system,
                               double[] xx, double[] yy);

        /// <summary>
        /// Calculate houses
        /// </summary>
        /// <param name="jdnr">Julian day number</param>
        /// <param name="lat">Geographical latitude</param>
        /// <param name="lon">Geographical longitude</param>
        /// <param name="system">Index to define housesystem</param>
        /// <returns>Array of doubles with with the following values:
        ///  0: not used, 1..12 cusps 1..12, 13: asc., 14: MC, 15: ARMC, 16: Vertex,
        ///  17: Equatorial asc., 18: co-ascendant (Koch), 19: co-ascendant(Munkasey),
        ///  20: polar ascendant 
        ///</returns>
        public static double[] getHouses(double jdnr, double lat, double lon, char system)
        {
            double[] xx = new double[13];
            double[] yy = new double[10];
            double[] zz = new double[23];
            int flag = ext_swe_houses(jdnr, lat, lon, (int)(system), xx, yy);

            for (int i = 0; i < 13; i++)
            {
                zz[i] = xx[i];
            }
            for (int i = 0; i < 10; i++)
            {
                zz[i + 13] = yy[i];
            }
            return zz;
        }
    }

}
