using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace EpheWrapper
{
    public static class SweWrapper
    {
        public const int OK = 0;
        //public const int ERR = -1;

        static SweWrapper()
        {
            string sedir = Environment.CurrentDirectory;
            int codeIndex = sedir.LastIndexOf('\\');
            sedir = sedir.Substring(0, sedir.Substring(0, codeIndex).LastIndexOf('\\'));
            sedir += "\\EPHEMERIS";
            //string sedir = "C:\\EPHEMERIS";
            swe_set_ephe_path(sedir);
        }

        #region Prototype Definitions
        #region Environment Setup Definition
        /// <summary>
        /// St location for Swiss Ephemeris files
        /// </summary>
        /// <param name="path">Location</param>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_set_ephe_path")]
        private extern static void swe_set_ephe_path(String path);

        /// <summary>
        /// close Swiss Ephemeris
        /// </summary>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_close")]
        public extern static void swe_close();

        #endregion

        #region Date related functions Definition

        /// <summary>
        /// This function returns the absolute Julian day number (JD) for a given calendar date.
        /// </summary>
        /// <param name="year">Year as integer</param>
        /// <param name="month">Month as integer</param>
        /// <param name="day">Day as integer</param>
        /// <param name="hour">Hour as double with decimal fraction.</param>
        /// <param name="gregflag">SE_GREG_CAL (1) means Gregorian calendar is assumed; SE_JUL_CAL (0) means Julian calendar is assumed.</param>
        /// <returns>jul_day_UT, Julian Day number as double</returns>
        /// <remarks>
        ///  The Julian day number is system of numbering all days continously 
        ///  within the time range of known human history. It should be familiar
        ///  for every astrological or astronomical programmer. The time variable
        ///  in astronomical theories is usually expressed in Julian days or
        ///  Julian centuries (36525 days per century) relative to some start day;
        ///  the start day is called 'the epoch'.
        ///  The Julian day number is a double representing the number of
        ///  days since JD = 0.0 on 1 Jan -4712, 12:00 noon.
        ///  
        /// Midnight has always a JD with fraction .5, because traditionally
        /// the astronomical day started at noon. This was practical because
        /// then there was no change of date during a night at the telescope.
        /// From this comes also the fact the noon ephemerides were printed
        /// before midnight ephemerides were introduced early in the 20th century.
        /// 
        /// NOTE: The Julian day number is named after the monk Julianus. It must
        /// not be confused with the Julian calendar system, which is named after
        /// Julius Cesar, the Roman politician who introduced this calendar.
        /// The Julian century is named after Cesar, i.e. a century in the Julian
        /// calendar. The 'gregorian' century has a variable length.
        /// </remarks>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_julday")]
        private extern static double swe_julday(int year, int month, int day, double hour, int gregflag);

        /// <summary>
        /// swe_revjul() is the inverse function to swe_julday(), see the description there.
        /// </summary>
        /// <param name="tjd">julian day number</param>
        /// <param name="gregflag">calendar retFlag (0=julian, 1=gregorian)</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <param name="hour">the hour of the day with decimal fraction (0 .. 23.999999).</param>
        /// <returns>Null</returns>
        /// <remarks>
        ///   Be aware the we use astronomical year numbering for the years 
        ///   before Christ, not the historical year numbering.
        ///   Astronomical years are done with negative numbers, historical
        ///   years with indicators BC or BCE (before common era).
        ///   Year  0 (astronomical)  	= 1 BC historical year
        ///   year -1 (astronomical) 	= 2 BC historical year
        ///   year -234 (astronomical) 	= 235 BC historical year etc.
        /// </remarks>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_revjul")]
        private extern static void swe_revjul(double tjd, int gregflag, out int year, out int month, out int day, out double hour);

        /// <summary>
        /// This function converts some date+time input {d,m,y,uttime}
        /// into the Julian day number tjd.
        /// The function checks that the input is a legal combination
        /// of dates; for illegal dates like 32 January 1993 it returns ERR
        /// but still converts the date correctly, i.e. like 1 Feb 1993.
        /// The function is usually used to convert user input of birth data
        /// into the Julian day number. Illegal dates should be notified to the user.
        /// 
        /// Be aware that we always use astronomical year numbering for the years
        /// before Christ, not the historical year numbering.
        /// Astronomical years are done with negative numbers, historical
        /// years with indicators BC or BCE (before common era).
        /// 
        /// Year 0 (astronomical)  	= 1 BC historical.
        /// year -1 (astronomical) 	= 2 BC 
        /// etc.
        /// Many users of Astro programs do not know about this difference.
        /// </summary>
        /// <param name="y">Year</param>
        /// <param name="m">Month</param>
        /// <param name="d">Day</param>
        /// <param name="uttime">UT time</param>
        /// <param name="c">calender</param>
        /// <param name="tjd">Julian day number</param>
        /// <returns>OK or ERR (for illegal date)</returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_date_conversion")]
        private extern static int swe_date_conversion(
            int y, int m, int d,		// day, month, year
            double uttime, 	            // UT in hours (decimal) 
            char c,		                // calendar indicator, either 'g' or 'j': g[regorian]|j[ulian] 
            out double tjd              // Converted Julian Day number
            );


        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_deltat")]
        public extern static double swe_deltat(double tjd);


        #endregion

        #region Calculation Functions Definitions

        /// <summary>
        /// The routine called by the user.
        /// It checks whether a position for the same planet, the same t, and the same retFlag bits has already been computed. 
        /// If yes, this position is returned.
        /// Otherwise it is computed.
        ///     -> If the SEFLG_SPEED retFlag has been specified, the speed will be returned at offset 3 of position array x[]. 
        ///             Its precision is probably better than 0.002"/day.
        ///     -> If the SEFLG_SPEED3 retFlag has been specified, the speed will be computed
        ///             from three positions. This speed is less accurate than SEFLG_SPEED,
        ///             i.e. better than 0.1"/day. And it is much slower. It is used for program tests only.
        ///     -> If no speed retFlag has been specified, no speed will be returned.
        /// </summary>
        /// <param name="tjd">Julian day, Ephemeris time: tjd_et = tjd_ut + swe_deltat(tjd_ut)</param>
        /// <param name="ipl">body number</param>
        /// <param name="retFlag">a 32 bit integer containing bit flags that indicate what kind of computation is wanted</param>
        /// <param name="result">array of 6 doubles for longitude, latitude, distance, speed in long., speed in lat., and speed in dist</param>
        /// <param name="serr">character string to return errorMsg messages in case of errorMsg</param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_calc")]
        private extern static SeFlg swe_calc(double tjd, int ipl, SeFlg iflag, double[] result, String serr);

        /// <summary>
        /// swe_calc_ut() was introduced with Swisseph version 1.60 and makes planetary calculations a bit simpler.
        /// swe_calc_ut() and swe_calc() work exactly the same way except that swe_calc() requires Ephemeris Time ( more accurate: Dynamical Time ) 
        /// as a parameter whereas swe_calc_ut() expects Universal Time. 
        /// For common astrological calculations, you will only need swe_calc_ut() and will not have to think anymore about 
        /// the conversion between Universal Time and Ephemeris Time.
        /// </summary>
        /// <param name="tjd_ut">Julian day, Universal Time</param>
        /// <param name="ipl">body number</param>
        /// <param name="retFlag">a 32 bit integer containing bit flags that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distance, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string to return errorMsg messages in case of errorMsg</param>
        /// <returns>
        /// On success, swe_calc ( or swe_calc_ut ) returns a 32-bit integer containing retFlag bits that indicate what kind of computation has been done. 
        /// This value may or may not be equal to retFlag. 
        /// If an option specified by retFlag cannot be fulfilled or makes no sense, swe_calc just does what can be done. 
        /// E.g., if you specify that you want JPL ephemeris, but swe_calc cannot find the ephemeris file, it tries to do the computation with 
        /// any available ephemeris. 
        /// This will be indicated in the return value of swe_calc. 
        /// So, to make sure that swe_calc () did exactly what you had wanted, you may want to check whether or not the return code == retFlag.
        /// However, swe_calc() might return an fatal errorMsg code (< 0) and an errorMsg string in one of the following cases:
        /// •	if an illegal body number has been specified
        /// •	if a Julian day beyond the ephemeris limits has been specified
        /// •	if the length of the ephemeris file is not correct (damaged file)
        /// •	on read errorMsg, e.g. a file index points to a position beyond file length ( data on file are corrupt )
        /// •	if the copyright section in the ephemeris file has been destroyed.
        /// 
        /// If any of these errors occurs,
        /// •	the return code of the function is -1,
        /// •	the position and speed variables are set to zero, 
        /// •	the type of errorMsg is indicated in the errorMsg string serr.
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_calc_ut")]
        private extern static SeFlg swe_calc_ut(double tjd_ut, PlanetId ipl, SeFlg iflag, double[] xx, String serr);

        /// <summary>
        /// Nodes and apsides of planets and moon
        /// 
        ///    Planetary nodes can be defined in three different ways:
        ///    a) They can be understood as a direction or as an axis defined by the intersection line of two orbital planes. 
        ///      E.g., the nodes of Mars are defined by the intersection line of Mars' orbital plane with the ecliptic (= the 
        ///      Earths orbit heliocentrically or the solar orbit geocentrically). However, as Michael Erlewine points 
        ///      out in his elaborate web page on this topic  (http://thenewage.com/resources/articles/interface.html),
        ///      planetary nodes can be defined for any couple of  planets. E.g. there is also an intersection line for the 
        ///      two orbital planes of Mars and Saturn.
        ///      Because such lines are, in principle, infinite, the heliocentric and the geocentric positions of the 
        ///      planetary nodes will be the same. There are astrologers that use such heliocentric planetary nodes in geocentric 
        ///      charts.
        ///      The ascending and the descending node will, in this case, be in precise opposition.

        ///    b) The planetary nodes can also be understood in a different way, not as an axis, but as the two points on a 
        ///      planetary orbit that are located precisely on the intersection line of the two planes.
        ///      This second definition makes no difference for the moon or for heliocentric positions of planets, but it does so for 
        ///      geocentric positions. There are two possibilities for geocentric planetary nodes based on this definition. 
        ///      1) The common solution is that the points on the planets orbit are transformed to the geocenter. The 
        ///        two points will not be in opposition anymore, or they will only roughly be so with the outer planets. The 
        ///        advantage of these nodes is that when a planet is in conjunction with its node, then its ecliptic latitude 
        ///        will be zero. This is not true when a planet is in geocentric conjunction with its heliocentric node. 
        ///        (And neither is it always true for the inner planets, i.e. Mercury and Venus.)
        ///        
        ///      2) The second possibility that nobody seems to have thought of so far: One may compute the points of 
        ///        the earth's orbit that lie exactly on another planet's orbital plane and transform it to the geocenter. The two 
        ///        points will always be in an approximate square.

        ///    c) Third, the planetary nodes could be defined as the intersection points of the plane defined by their 
        ///      momentary geocentric position and motion with the plane of the ecliptic. Such points would move very fast 
        ///      around the planetary stations. Here again, as in b)1), the planet would cross the ecliptic and its ecliptic 
        ///      latitude would be 0 exactly when it were in conjunction with one of its nodes.

        ///    The Swiss Ephemeris supports the solutions a) and b) 1).

        ///    Possible definitions for apsides:

        ///    a) The planetary apsides can be defined as the perihelion and aphelion points on a planetary orbit. For a
        ///      geocentric chart, these points could be transformed from the heliocenter to the geocenter.
        ///    b) However, one might consider these points as astrologically relevant axes rather than as points on a 
        ///      planetary orbit. Again, this would allow heliocentric positions in a geocentric chart.

        ///    Note: For the "Dark Moon" or "Lilith", which I usually define as the lunar apogee, some astrologers give a 
        ///    different definition. They understand it as the second focal point of the moon's orbital ellipse. This definition does not 
        ///    make a difference for geocentric positions, because the apogee and the second focus are in exactly the same geocentric 
        ///    direction. However, it makes a difference with topocentric positions, because the two points do not have same distance. 
        ///    Analogous "black planets" have been proposed: they would be the second focal points of the planets' orbital ellipses. The 
        ///    heliocentric positions of these "black planets" are identical with the heliocentric positions of the aphelia, but geocentric 
        ///    positions are not identical, because the focal points are much closer to the sun than the aphelia.

        ///    The Swiss Ephemeris allows to compute the "black planets" as well.

        ///    Mean positions

        ///    Mean nodes and apsides can be computed for the Moon, the Earth and the planets Mercury - Neptune. They are taken 
        ///    from the planetary theory VSOP87. Mean points can not be calculated for Pluto and the asteroids, because there is no 
        ///    planetary theory for them.
         
        ///    Osculating nodes and apsides

        ///    Nodes and apsides can also be derived from the osculating orbital elements of a body, the paramaters that define an  
        ///    ideal unperturbed elliptic (two-body) orbit. 
        ///    For astrology, note that this is a simplification and idealization. 
        ///    Problem with Neptune: Neptune's orbit around the sun does not have much in common with an ellipse. There are often two 
        ///    perihelia and two aphelia within one revolution. As a result, there is a wild oscillation of the osculating perihelion (and 
        ///    aphelion). 
        ///    In actuality, Neptune's orbit is not heliocentric orbit at all. The twofold perihelia and aphelia are an effect of the motion of 
        ///    the sun about the solar system barycenter. This motion is much faster than the motion of Neptune, and Neptune 
        ///    cannot react on such fast displacements of the Sun. As a result, Neptune seems to move around the barycenter (or a 
        ///    mean sun) rather than around the true sun. In fact, Neptune's orbit around the barycenter is therefore closer to 
        ///    an ellipse than the his orbit around the sun. The same statement is also true for Saturn, Uranus and Pluto, but not 
        ///    for Jupiter and the inner planets.

        ///    This fundamental problem about osculating ellipses of planetary orbits does of course not only affect the apsides but also the nodes.

        ///    Two solutions can be thought of for this problem: 
        ///    1) The one would be to interpolate between actual passages of the planets through their nodes and apsides. However, 
        ///      this works only well with Mercury. 
        ///      With all other planets, the supporting points are too far part as to make an accurate interpolation possible. 
        ///      This solution is not implemented, here.
        ///    2) The other solution is to compute the apsides of the orbit around the barycenter rather than around the sun. 
        ///      This procedure makes sense for planets beyond Jupiter, it comes closer to the mean apsides and nodes for 
        ///      planets that have such points defined. For all other transsaturnian planets and asteroids, this solution yields 
        ///      a kind of "mean" nodes and apsides. On the other hand, the barycentric ellipse does not make any sense for 
        ///      inner planets and Jupiter.

        ///    The Swiss Ephemeris supports solution 2) for planets and 
        ///    asteroids beyond Jupiter.

        ///    Anyway, neither the heliocentric nor the barycentric ellipse is a perfect representation of the nature of a planetary orbit, 
        ///    and it will not yield the degree of precision that today's astrology is used to.
        ///    The best choice of method will probably be:
        ///    - For Mercury - Neptune: mean nodes and apsides
        ///    - For asteroids that belong to the inner asteroid belt: osculating nodes/apsides from a heliocentric ellipse
        ///    - For Pluto and outer asteroids: osculating nodes/apsides from a barycentric ellipse

        ///    The Moon is a special case: A "lunar true node" makes more sense, because it can be defined without the idea of an 
        ///    ellipse, e.g. as the intersection axis of the momentary lunar orbital plane with the ecliptic. Or it can be said that the 
        ///    momentary motion of the moon points to one of the two ecliptic points that are called the "true nodes".  So, these 
        ///    points make sense. With planetary nodes, the situation is somewhat different, at least if we make a difference 
        ///    between heliocentric and geocentric positions. If so, the planetary nodes are points on a heliocentric orbital ellipse, 
        ///    which are transformed to the geocenter. An ellipse is required here, because a solar distance is required. In 
        ///    contrast to the planetary nodes, the lunar node does not require a distance, therefore manages without the idea of an 
        ///    ellipse and does not share its weaknesses. 
        ///    On the other hand, the lunar apsides DO require the idea of an ellipse. And because the lunar ellipse is actually 
        ///    extremely distorted, even more than any other celestial ellipse, the "true Lilith" (apogee), for which printed 
        ///    ephemerides are available, does not make any sense at all. 
        ///    (See the chapter on the lunar node and apogee.)

        ///    Special case: the Earth

        ///    The Earth is another special case. Instead of the motion of the Earth herself, the heliocentric motion of the Earth-
        ///    Moon-Barycenter (EMB) is used to determine the osculating perihelion. 
        ///    There is no node of the earth orbit itself. However, there is an axis around which the earth's orbital plane slowly rotates 
        ///    due to planetary precession. The position points of this axis are not calculated by the Swiss Ephemeris.

        ///    Special case: the Sun

        ///    In addition to the Earth (EMB) apsides, the function computes so-to-say "apsides" of the sun, i.e. points on the 
        ///    orbit of the Sun where it is closest to and where it is farthest from the Earth. These points form an opposition and are 
        ///    used by some astrologers, e.g. by the Dutch astrologer George Bode or the Swiss astrologer Liduina Schmed. The 
        ///    perigee, located at about 13 Capricorn, is called the "Black Sun", the other one, in Cancer, the "Diamond".
        ///    So, for a complete set of apsides, one ought to calculate them for the Sun and the Earth and all other planets. 

        ///    The modes of the Swiss Ephemeris function 
        ///    swe_nod_aps()

        ///    The  function swe_nod_aps() can be run in the following modes:
        ///    1) Mean positions are given for nodes and apsides of Sun, Moon, Earth, and the up to Neptune. Osculating 
        ///      positions are given with Pluto and all asteroids. This is the default mode.
        ///    2) Osculating positions are returned for nodes and apsides of all planets.
        ///    3) Same as 2), but for planets and asteroids beyond Jupiter, a barycentric ellipse is used.
        ///    4) Same as 1), but for Pluto and asteroids beyond Jupiter, a barycentric ellipse is used.

        ///    In all of these modes, the second focal point of the ellipse can be computed instead of the aphelion.
        ///    Like the planetary function swe_calc(), swe_nod_aps() is able to return geocentric, topocentric, heliocentric, or 
        ///    barycentric position.
        /// </summary>
        /// <param name="tjd_ut">julian day, ephemeris time</param>
        /// <param name="ipl">planet number</param>
        /// <param name="iflag">as usual, SEFLG_HELCTR, etc.</param>
        /// <param name="method">
        ///     *               - 0 or SE_NODBIT_MEAN. MEAN positions are given for nodes and apsides of Sun, Moon, Earth, and the 
        ///     *                 planets up to Neptune. Osculating positions are given with Pluto and all asteroids.
        ///     *               - SE_NODBIT_OSCU. Osculating positions are given for all nodes and apsides.
        ///     *               - SE_NODBIT_OSCU_BAR. Osculating nodes and apsides are computed from barycentric ellipses, for planets
        ///     *                 beyond Jupiter, but from heliocentric ones for ones for Jupiter and inner planets.
        ///     *               - SE_NODBIT_MEAN and SE_NODBIT_OSCU_BAR can be combined. The program behaves the same way as with simple 
        ///     *                 SE_NODBIT_MEAN, but uses barycentric ellipses for planets beyond Neptune and asteroids beyond Jupiter.
        ///     *               - SE_NODBIT_FOCAL can be combined with any of the other bits. The second focal points of the ellipses will 
        ///     *                 be returned instead of the aphelia.
        /// </param>
        /// <param name="xnasc">an array of 6 doubles: ascending node</param>
        /// <param name="xndsc">an array of 6 doubles: descending node</param>
        /// <param name="xperi">an array of 6 doubles: perihelion</param>
        /// <param name="xaphe">an array of 6 doubles: aphelion</param>
        /// <param name="serr">error message</param>
        /// <returns>)0 or -1: 0 means Ok, -1 means error</returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_nod_aps_ut")]
        private extern static SeFlg swe_nod_aps_ut(double tjd_ut, PlanetId ipl, SeFlg iflag, int method,
            double[] xnasc, double[] xndsc, double[] xperi, double[] xaphe, String serr);

        /// <summary>
        ///  The function swe_fixstar_ut() and swe_fixstar() computes fixed stars. They are defined as follows:
        /// </summary>
        /// <param name="star">
        /// name of fixed star to be searched, returned name of found star
        /// The  parameter star must provide for at least 41 characters for the returned star name
        /// (= 2 x SE_MAX_STNAME + 1, where SE_MAX_STNAME is defined in swephexp.h). If a star is found, 
        /// its name is returned in this field in the format: traditional_name, nomenclature_name e.g. "Aldebaran,alTau".
        /// 
        /// The function has three modes to search for a star in the file fixstars.cat:
        /// •	star contains a positive number ( in ASCII string format, e.g. "234"): The 234-th non-comment line in the file fixstars.cat is used. 
        ///     Comment lines begin with # and are ignored.
        /// •	star contains a traditional name: the first star in the file fixstars.cat is used whose traditional name fits the given name. 
        ///         all names are mapped to lower case before comparison. 
        ///         If star has n characters, only the first n characters of the traditional name field are compared. 
        ///         If a comma appears after a non-zero-length traditional name, the traditional name is cut off at the comma before the search. 
        ///         This allows the reuse of the returned star name from a previous call in the next call.
        /// •	star begins with a comma, followed by a nomenclature name, e.g. ",alTau": 
        ///         the star with this name in the nomenclature field ( the second field ) is returned. 
        ///         Letter case is observed in the comparison for nomenclature names. 
        /// </param>
        /// <param name="tjd_ut">Julian day in Universal Time</param>
        /// <param name="retFlag">an integer containing several flags that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distance, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string to contain errorMsg messages in case of errorMsg.</param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_fixstar_ut")]
        private extern static int swe_fixstar_ut(String star, double tjd_ut, SeFlg iflag, ref double[] xx, String serr);

        /// <summary>
        /// house cusps, ascendant and MC
        /// The cusps are returned in double cusp[13],
        ///     or cusp[37] with house system 'G'.
        ///     cusp[1...12]	houses 1 - 12
        ///     additional points are returned in ascmc[10].
        ///     ascmc[0] = ascendant
        ///     ascmc[1] = mc
        ///     ascmc[2] = armc
        ///     ascmc[3] = vertex
        ///     ascmc[4] = equasc		* "equatorial ascendant" *
        ///     ascmc[5] = coasc1		* "co-ascendant" (W. Koch) *
        ///     ascmc[6] = coasc2		* "co-ascendant" (M. Munkasey) *
        ///     ascmc[7] = polasc		* "polar ascendant" (M. Munkasey) *
        /// </summary>
        /// <param name="tjd_ut">Julian day number, UT </param>
        /// <param name="geolat">geographic latitude, in degs </param>
        /// <param name="geolon">geographic longitude, in degs</param>
        /// <param name="hsys">house method, ascii code of one of the letters PKORCAEVXHTBG</param>
        /// <param name="cusp">array for 13 doubles </param>
        /// <param name="ascmc">array for 10 doubles </param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_houses")]
        private extern static int swe_houses(double tjd_ut, double geolat, double geolon, int hsys, double[] cusp, double[] ascmc);


        /// <summary>
        /// Computes the house position of a planet or another point, in degs: 0 - 30 = 1st house, 30 - 60 = 2nd house, etc.
        /// IMPORTANT: This function should NOT be used for sidereal astrology.
        /// </summary>
        /// <param name="armc">ARMC, sidereal time in degs</param>
        /// <param name="geolat">geographic latitude, in degs</param>
        /// <param name="eps">true ecliptic obliquity, in degs </param>
        /// <param name="hsys">house method, one of the letters PKRCAV</param>
        /// <param name="xpin">array of 6 doubles: only the first two used for longitude and latitude of the planet</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        /// House position is returned by function, a value between 1.0 and 12.999999, 
        /// indicating in which house a planet is and how far from its cusp it is. 
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_house_pos")]
        private extern static double swe_house_pos(double armc, double geolat, double eps, int hsys, double[] xpin, String serr);


        /// <summary>
        /// The function computes phase, phase theAngle, elongation, apparent diameter, apparent magnitude for the Sun, the Moon, 
        /// all stars and asteroids. 
        /// </summary>
        /// <param name="tjd_ut">time Jul. Day UT</param>
        /// <param name="ipl">planet number</param>
        /// <param name="retFlag">ephemeris retFlag</param>
        /// <param name="attr">
        /// return array, 20 doubles, see below
        ///  * attr[0] = phase theAngle (earth-planet-sun)
        ///  * attr[1] = phase (illumined fraction of disc)
        ///  * attr[2] = elongation of planet
        ///  * attr[3] = apparent diameter of disc
        ///  * attr[4] = apparent magnitude
        ///  * attr[5] = geocentric horizontal parallax (Moon)
        ///  *         declare as attr[20] at least !</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_pheno_ut")]
        private extern static SeFlg swe_pheno_ut(double tjd_ut, PlanetId ipl, SeFlg iflag, double[] attr, String serr);

        #endregion
        #endregion


        #region API Definitions
        public static double ToJulianDay(DateTime momentUtc)
        {
            return swe_julday(momentUtc.Year, momentUtc.Month, momentUtc.Day, momentUtc.TimeOfDay.TotalHours, 1);
        }

        public static DateTime UtcFromJulianDay(Double julday)
        {
            int year, month, day;
            double utTime;

            swe_revjul(julday, 1, out year, out month, out day, out utTime);

            TimeSpan inHours = TimeSpan.FromHours(utTime);
            DateTime utcMoment = new DateTime(year, month, day, inHours.Hours, inHours.Minutes, inHours.Seconds, inHours.Milliseconds);
            return utcMoment;
        }

        public static List<Angle> GetObliquityNutation(Double jul_ut)
        {
            double[] result = new double[6];
            String errorMsg = "";

            SeFlg retFlag = swe_calc_ut(jul_ut, PlanetId.SE_ECL_NUT, SeFlg.DEFAULT, result, errorMsg);
            if (retFlag == SeFlg.ERR)
            {
                DateTime utc = SweWrapper.UtcFromJulianDay(jul_ut);
                return null;
            }
            else
            {
                //Console.WriteLine(((SeFlg)retFlag).ToString());
                List<Angle> angles = new List<Angle>();
                Angle trueObliquity = new Angle(result[0]);
                Angle meanObliquity = new Angle(result[1]);
                Angle nutationLongitude = new Angle(result[2]);
                Angle nutationObliquity = new Angle(result[3]);
                angles.Add(trueObliquity);
                angles.Add(meanObliquity);
                angles.Add(nutationLongitude);
                angles.Add(nutationObliquity);
                return angles;
            }
        }

        public static double LongitudeDegreeOf(double jul_ut, PlanetId id)
        {
            double[] result = new double[6];
            String errorMsg = "";

            SeFlg retFlag = SeFlg.ERR;

            if (id < PlanetId.SE_NORTHNODE)
            {
                retFlag = swe_calc_ut(jul_ut, id, SeFlg.DEFAULT, result, errorMsg);
                if (retFlag != SeFlg.ERR)
                {
                    //if (retFlag != flag)
                    //    Console.WriteLine(((SeFlg)retFlag).ToString());
                    return result[0];
                }
            }

            return Longitude.NullDegrees;
        }

        public static PlanetPosition PositionOf(double jul_ut, PlanetId id, SeFlg flag)
        {
            double[] result = new double[6];
            String errorMsg = "";

            SeFlg retFlag = SeFlg.ERR;

            if (id < PlanetId.SE_NORTHNODE)
            {
                retFlag = swe_calc_ut(jul_ut, id, flag, result, errorMsg);
                if (retFlag == SeFlg.ERR)
                {
                    DateTime utc = SweWrapper.UtcFromJulianDay(jul_ut);
                    Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.DEFAULT, errorMsg));
                    return null;
                }
                else
                {
                    if (retFlag != flag)
                        Console.WriteLine(((SeFlg)retFlag).ToString());
                    return new PlanetPosition(id, result);
                }
            }
            else
                return NorthSouthNode(jul_ut, id, flag);

        }

        public static PlanetPosition NorthSouthNode(double jul_ut, PlanetId id, SeFlg flag)
        {
            if (id != PlanetId.SE_NORTHNODE && id != PlanetId.SE_SOUTHNODE)
                throw new Exception("Wrong parameter of " + id.ToString());

            double[] xnasc = new double[6]; 
            double[] xndsc = new double[6];
            double[] xperi = new double[6];
            double[] xaphe = new double[6];
            String errorMsg = "";

            if (swe_nod_aps_ut(jul_ut, PlanetId.SE_MOON, flag, 0, xnasc, xndsc, xperi, xaphe, errorMsg) == SeFlg.ERR)
            {
                DateTime utc = SweWrapper.UtcFromJulianDay(jul_ut);
                Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.DEFAULT, errorMsg));
                return null;
            }
            else
            {
                PlanetPosition nodePos = new PlanetPosition(id, (id ==PlanetId.SE_NORTHNODE) ? xnasc : xndsc);
                return nodePos;
            }
        }

        public static void CalcPhenomenon(DateTime utcMoment, PlanetId ipl, SeFlg flag)
        {
            double jul_ut = ToJulianDay(utcMoment);
            double[] result = new double[20];
            String errorMsg = "";

            SeFlg retFlag = SeFlg.ERR;

            retFlag = swe_pheno_ut(jul_ut, ipl, flag, result, errorMsg);
            if (retFlag == SeFlg.ERR)
            {
                DateTime utc = SweWrapper.UtcFromJulianDay(jul_ut);
                Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", ipl, utc, SeFlg.DEFAULT, errorMsg));
            }
            else
            {
                ///  * attr[0] = phase theAngle (earth-planet-sun)
                ///  * attr[1] = phase (illumined fraction of disc)
                ///  * attr[2] = elongation of planet
                ///  * attr[3] = apparent diameter of disc
                ///  * attr[4] = apparent magnitude
                ///  * attr[5] = geocentric horizontal parallax (Moon)
                DateTime utc = SweWrapper.UtcFromJulianDay(jul_ut);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} at {1}", ipl, utc);
                sb.AppendFormat("PhaseAngle = {0}, Phase={1}, Elongation={2}, Diameter={3}, Magnitude={4}, Parallax={5}",
                    result[0], result[1], result[2], result[3], result[4], result[5]);

                for (int i = 6; i < 20; i++)
                {
                    sb.AppendFormat("result[{0}] = {1}\t", i, result[i]);
                    if ((i - 6) % 5 == 4)
                        sb.AppendLine();
                }

                Console.Write(sb.ToString());
            }
        }

        //public static PlanetPosition CalcPlanet_Et(double jul_et, PlanetId id, SeFlg kind)
        //{
        //    int planetId = (int)id;
        //    double[] result = new double[6];
        //    String errorMsg = "";

        //    SeFlg retFlag = swe_calc(jul_et, planetId, kind, result, errorMsg);
        //    if (retFlag == SeFlg.ERR)
        //    {
        //        DateTime utc = SweWrapper.UtcFromJulianDay(jul_et);
        //        Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.DEFAULT, errorMsg));
        //        return null;
        //    }
        //    else
        //    {
        //        //Console.WriteLine(((SeFlg)retFlag).ToString());
        //        return new PlanetPosition(id, result);
        //    }
        //}

        //public static Astrolabe CalcAstrolobe(DateTime moment, SeFlg kind)
        //{
        //    Astrolabe newAstrolobe = new Astrolabe("Unknown", moment);

        //    double jul_ut = ToJulianDay(moment);

        //    double deltat = swe_deltat(jul_ut);

        //    double jul_et = jul_ut + deltat;

        //    List<Angle> angles1 = GetObliquityNutation(jul_ut);

        //    PlanetPosition pos = null;
        //    PlanetPosition pos2 = null;

        //    foreach (PlanetId id in Astrolabe.Concerned)
        //    {
        //        pos = PositionOf(jul_ut, id, kind);
                
        //        newAstrolobe.StarPositions.Add(id, pos);

        //        pos2 = PositionOf(jul_et, id, kind);
        //    }

        //    swe_close();
        //    return newAstrolobe;
        //}
        #endregion
    }
}
