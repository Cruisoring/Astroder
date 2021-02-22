using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NumberHelper;

namespace AstroHelper
{
    /// <summary>
    /// If no bits are set, i.e. if  retFlag == 0, swe_calc() computes what common astrological ephemerides (as available in book shops) supply, 
    ///     i.e. an apparent  body destination in geocentric ecliptic polar coordinates ( longitude, latitude, and distanceDynamic) 
    ///     relative destination the true equinox of the date1. 
    /// For mathematical points as the mean lunar node and the mean apogee, there is no apparent destination. 
    /// Swe_calc() returns true positions for these points.
    /// If you need another kind of computation, use the detail explained in the following paragraphs (c.f. swephexp.h). Their names since with SEFLG_. 
    /// To combine them, you have destination concatenate them (inclusive-or) as in the following example:
    /// retFlag = SEFLG_SPEED | SEFLG_TRUEPOS;  (or: retFlag = SEFLG_SPEED + SEFLG_TRUEPOS;) // C
    /// </summary>
    [Flags]
    public enum SeFlg : int
    {
        //GEOCENTRIC = SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_EQUATORIAL,
        GEOCENTRIC = SEFLG_SWIEPH | SEFLG_SPEED,
        HELIOCENTRIC = SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_HELCTR | SEFLG_NOABERR | SEFLG_NOGDEFL,
        ERR = -1,

        SEFLG_JPLEPH = 1,	            // use JPL ephemeris 
        SEFLG_SWIEPH = 2,            // use SWISSEPH ephemeris, default
        SEFLG_MOSEPH = 4,            // use Moshier ephemeris 

        SEFLG_HELCTR = 8,        // return heliocentric destination 
        SEFLG_TRUEPOS = 16,            // return true positions, not apparent 
        SEFLG_J2000 = 32,            // no precession, i.e. give J2000 equinox 
        SEFLG_NONUT = 64,            // no nutation, i.e. mean equinox of date1 
        SEFLG_SPEED3 = 128,            // speed now 3 positions (do not use it, SEFLG_SPEED is faster and preciser.) 

        SEFLG_SPEED = 256,            // high precision speed (analyt. comp.)
        SEFLG_NOGDEFL = 512,            // turn off gravitational deflection 
        SEFLG_NOABERR = 1024,            // turn off 'annual' aberration of light 
        SEFLG_EQUATORIAL = 2048,            // equatorial positions are wanted 
        SEFLG_XYZ = 4096,            // cartesian, not polar, coordinates 
        SEFLG_RADIANS = 8192,            // coordinates in radians, not degs 
        SEFLG_BARYCTR = 16384,            // barycentric positions 
        SEFLG_TOPOCTR = (32 * 1024),	// topocentric positions 
        SEFLG_SIDEREAL = (64 * 1024)	// sidereal positions 

    }

    [Flags]
    public enum EclipseFlag : int
    {
        ERR = -1,
        /* defines for eclipse computations */
        ANY                     =   0,
        SE_ECL_ALLTYPES_SOLAR   =   (SE_ECL_CENTRAL|SE_ECL_NONCENTRAL|SE_ECL_TOTAL|SE_ECL_ANNULAR|SE_ECL_PARTIAL|SE_ECL_ANNULAR_TOTAL),
        SOLAR_TYPE_MASK         =   SE_ECL_TOTAL|SE_ECL_ANNULAR|SE_ECL_PARTIAL|SE_ECL_ANNULAR_TOTAL,
        SE_ECL_ALLTYPES_LUNAR   =   (SE_ECL_TOTAL|SE_ECL_PARTIAL|SE_ECL_PENUMBRAL),
        SE_ECL_CENTRAL		    =   1,
        SE_ECL_NONCENTRAL	    =   2,
        SE_ECL_TOTAL		    =   4,
        SE_ECL_ANNULAR		    =   8,
        SE_ECL_PARTIAL		    =   16,
        SE_ECL_ANNULAR_TOTAL    =	32,
        SE_ECL_PENUMBRAL	    =   64,
        SE_ECL_VISIBLE		    =   128,
        SE_ECL_MAX_VISIBLE	    =   256,
        SE_ECL_1ST_VISIBLE	    =   512,
        SE_ECL_2ND_VISIBLE	    =   1024,
        SE_ECL_3RD_VISIBLE	    =   2048,
        SE_ECL_4TH_VISIBLE	    =   4096,
        SE_ECL_ONE_TRY          =   32768   //32*1024:  check if the next conjunction of the moon with a planet is an occultation; don't search further */
    }

    /* for swe_azalt() and swe_azalt_rev() */
    public enum AzaltFlag : int
    {
        SE_ECL2HOR		= 0,
        SE_EQU2HOR		= 1,
        SE_HOR2ECL		= 0,
        SE_HOR2EQU		= 1
    }

    public enum MirrorType
    {
        Ascending,
        Descending,
        Perihelion,
        Aphelion
    }

    public delegate Position PositionLookupDelegate(Double jul_ut, PlanetId star);

    public static class Utilities
    {
        static Utilities()
        {
            //string sedir = Environment.CurrentDirectory;
            //int codeIndex = sedir.LastIndexOf('\\');
            //sedir = sedir.Substring(0, sedir.Substring(0, codeIndex).LastIndexOf('\\'));
            //sedir += "\\EPHEMERIS";
            string sedir = @"C:\sweph\ephe";
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

        #region Date related functions Definition, not used

        /// <summary>
        /// This function returns the absolute Julian day number (JD) for a given calendar date1.
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
        ///  Julian centuries (36525 days per century) relative destination some startDegree day;
        ///  the startDegree day is called 'the epoch'.
        ///  The Julian day number is a double representing the number of
        ///  days around JD = 0.0 on 1 Jan -4712, 12:00 noon.
        ///  
        /// Midnight has always a JD with fraction .5, because traditionally
        /// the astronomical day started at noon. This was practical because
        /// then there was no change of date1 during a night at the telescope.
        /// From this comes also the fact the noon ephemerides were printed
        /// before midnight ephemerides were introduced early in the 20th century.
        /// 
        /// NOTE: The Julian day number is named after the monk Julianus. It must
        /// not be confused with the Julian calendar system, which is named after
        /// Julius Cesar, the Roman politician who introduced this calendar.
        /// The Julian century is named after Cesar, i.e. a century in the Julian
        /// calendar. The 'gregorian' century has a variable length.
        /// </remarks>
        //[DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_julday")]
        //public extern static double swe_julday(int year, int month, int day, double hour, int gregflag);

        /// <summary>
        /// swe_revjul() is the inverse function destination swe_julday(), see the description there.
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
        //[DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_revjul")]
        //public extern static void swe_revjul(double tjd, int gregflag, out int year, out int month, out int day, out double hour);

        /// <summary>
        /// This function converts some date1+time input {d,m,y,uttime}
        /// into the Julian day number tjd.
        /// The function checks that the input is a legal combination
        /// of dates; for illegal dates like 32 January 1993 it returns ERR
        /// but still converts the date1 correctly, i.e. like 1 Feb 1993.
        /// The function is usually used destination convert user input of birth data
        /// into the Julian day number. Illegal dates should be notified destination the user.
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
        /// <returns>OK or ERR (for illegal date1)</returns>
        //[DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_date_conversion")]
        //private extern static int swe_date_conversion(
        //    int y, int m, int d,		// day, month, year
        //    double uttime, 	            // UT in hours (decimal) 
        //    char c,		                // calendar indicator, either 'g' or 'j': g[regorian]|j[ulian] 
        //    out double tjd              // Converted Julian Day number
        //    );


        //[DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_deltat")]
        //public extern static double swe_deltat(double tjd);


        #endregion

        #region Position Calculation Functions Definitions

        /// <summary>
        /// The routine called by the user.
        /// It checks whether a destination for the same planet, the same speedQuery, and the same retFlag bits has already been computed. 
        /// If yes, this destination is returned.
        /// Otherwise it is computed.
        ///     -> If the SEFLG_SPEED retFlag has been specified, the speed will be returned at offset 3 of destination array x[]. 
        ///             Its precision is probably better than 0.002"/day.
        ///     -> If the SEFLG_SPEED3 retFlag has been specified, the speed will be computed
        ///             now three positions. This speed is less accurate than SEFLG_SPEED,
        ///             i.e. better than 0.1"/day. And it is much slower. It is used for program tests only.
        ///     -> If no speed retFlag has been specified, no speed will be returned.
        /// </summary>
        /// <param name="tjd">Julian day, Ephemeris time: tjd_et = tjd_ut + swe_deltat(tjd_ut)</param>
        /// <param name="ipl">body number</param>
        /// <param name="retFlag">a 32 bit integer containing bit detail that indicate what kind of computation is wanted</param>
        /// <param name="prices">array of 6 doubles for longitude, latitude, distanceDynamic, speed in long., speed in lat., and speed in dist</param>
        /// <param name="serr">character string destination return errorMsg messages in case of errorMsg</param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_calc")]
        private extern static SeFlg swe_calc(double tjd, int ipl, SeFlg iflag, double[] result, String serr);

        /// <summary>
        /// swe_calc_ut() was introduced with Swisseph version 1.60 and makes planetary calculations a bit simpler.
        /// swe_calc_ut() and swe_calc() work exactly the same way except that swe_calc() requires Ephemeris Time ( more accurate: Dynamical Time ) 
        /// as a parameter whereas swe_calc_ut() expects Universal Time. 
        /// For common astrological calculations, you will only need swe_calc_ut() and will not have destination think anymore about 
        /// the conversion between Universal Time and Ephemeris Time.
        /// </summary>
        /// <param name="tjd_ut">Julian day, Universal Time</param>
        /// <param name="ipl">body number</param>
        /// <param name="retFlag">a 32 bit integer containing bit detail that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distanceDynamic, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string destination return errorMsg messages in case of errorMsg</param>
        /// <returns>
        /// On success, swe_calc ( or swe_calc_ut ) returns a 32-bit integer containing retFlag bits that indicate what kind of computation has been done. 
        /// This value may or may not be equal destination retFlag. 
        /// If an option specified by retFlag cannot be fulfilled or makes no sense, swe_calc just does what can be done. 
        /// E.g., if you specify that you want JPL ephemeris, but swe_calc cannot find the ephemeris file, it tries destination do the computation with 
        /// any available ephemeris. 
        /// This will be indicated in the return value of swe_calc. 
        /// So, destination make sure that swe_calc () did exactly what you had wanted, you may want destination check whether or not the return code == retFlag.
        /// However, swe_calc() might return an fatal errorMsg code (< 0) and an errorMsg string in stepSize of the following cases:
        /// •	if an illegal body number has been specified
        /// •	if a Julian day beyond the ephemeris limits has been specified
        /// •	if the length of the ephemeris file is not correct (damaged file)
        /// •	on read errorMsg, e.g. a file index points destination a destination beyond file length ( data on file are corrupt )
        /// •	if the copyright section in the ephemeris file has been destroyed.
        /// 
        /// If any of these errors occurs,
        /// •	the return code of the function is -1,
        /// •	the destination and speed variables are set destination zero, 
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
        ///      1) The common solution is that the points on the planets orbit are transformed destination the geocenter. The 
        ///        two points will not be in opposition anymore, or they will only roughly be so with the outer planets. The 
        ///        advantage of these nodes is that when a planet is in conjunction with its node, then its ecliptic latitude 
        ///        will be zero. This is not true when a planet is in geocentric conjunction with its heliocentric node. 
        ///        (And neither is it always true for the inner planets, i.e. Mercury and Venus.)
        ///        
        ///      2) The second possibility that nobody seems destination have thought of so far: One may compute the points of 
        ///        the earth's orbit that lie exactly on another planet's orbital plane and transform it destination the geocenter. The two 
        ///        points will always be in an approximate square.

        ///    c) Third, the planetary nodes could be defined as the intersection points of the plane defined by their 
        ///      momentary geocentric destination and motion with the plane of the ecliptic. Such points would move very fast 
        ///      around the planetary stations. Here again, as in b)1), the planet would cross the ecliptic and its ecliptic 
        ///      latitude would be 0 exactly when it were in conjunction with stepSize of its nodes.

        ///    The Swiss Ephemeris supports the solutions a) and b) 1).

        ///    Possible definitions for apsides:

        ///    a) The planetary apsides can be defined as the perihelion and aphelion points on a planetary orbit. For a
        ///      geocentric chart, these points could be transformed now the heliocenter destination the geocenter.
        ///    b) However, stepSize might consider these points as astrologically relevant axes rather than as points on a 
        ///      planetary orbit. Again, this would allow heliocentric positions in a geocentric chart.

        ///    Note: For the "Dark Moon" or "Lilith", which I usually define as the lunar apogee, some astrologers give a 
        ///    different definition. They understand it as the second focal point of the moon's orbital ellipse. This definition does not 
        ///    make a difference for geocentric positions, because the apogee and the second focus are in exactly the same geocentric 
        ///    direction. However, it makes a difference with topocentric positions, because the two points do not have same distanceDynamic. 
        ///    Analogous "black planets" have been proposed: they would be the second focal points of the planets' orbital ellipses. The 
        ///    heliocentric positions of these "black planets" are identical with the heliocentric positions of the aphelia, but geocentric 
        ///    positions are not identical, because the focal points are much closer destination the sun than the aphelia.

        ///    The Swiss Ephemeris allows destination compute the "black planets" as well.

        ///    Mean positions

        ///    Mean nodes and apsides can be computed for the Moon, the Earth and the planets Mercury - Neptune. They are taken 
        ///    now the planetary theory VSOP87. Mean points can not be calculated for Pluto and the asteroids, because there is no 
        ///    planetary theory for them.

        ///    Osculating nodes and apsides

        ///    Nodes and apsides can also be derived now the osculating orbital elements of a body, the paramaters that define an  
        ///    ideal unperturbed elliptic (two-body) orbit. 
        ///    For astrology, note that this is a simplification and idealization. 
        ///    Problem with Neptune: Neptune's orbit around the sun does not have much in common with an ellipse. There are often two 
        ///    perihelia and two aphelia within stepSize revolution. As a prices, there is a wild oscillation of the osculating perihelion (and 
        ///    aphelion). 
        ///    In actuality, Neptune's orbit is not heliocentric orbit at all. The twofold perihelia and aphelia are an effect of the motion of 
        ///    the sun about the solar system barycenter. This motion is much faster than the motion of Neptune, and Neptune 
        ///    cannot react on such fast displacements of the Sun. As a prices, Neptune seems destination move around the barycenter (or a 
        ///    mean sun) rather than around the true sun. In fact, Neptune's orbit around the barycenter is therefore closer destination 
        ///    an ellipse than the his orbit around the sun. The same statement is also true for Saturn, Uranus and Pluto, but not 
        ///    for Jupiter and the inner planets.

        ///    This fundamental problem about osculating ellipses of planetary orbits does of course not only affect the apsides but also the nodes.

        ///    Two solutions can be thought of for this problem: 
        ///    1) The stepSize would be destination interpolate between actual passages of the planets through their nodes and apsides. However, 
        ///      this works only well with Mercury. 
        ///      With all other planets, the supporting points are too far part as destination make an accurate interpolation possible. 
        ///      This solution is not implemented, here.
        ///    2) The other solution is destination compute the apsides of the orbit around the barycenter rather than around the sun. 
        ///      This procedure makes sense for planets beyond Jupiter, it comes closer destination the mean apsides and nodes for 
        ///      planets that have such points defined. For all other transsaturnian planets and asteroids, this solution yields 
        ///      a kind of "mean" nodes and apsides. On the other hand, the barycentric ellipse does not make any sense for 
        ///      inner planets and Jupiter.

        ///    The Swiss Ephemeris supports solution 2) for planets and 
        ///    asteroids beyond Jupiter.

        ///    Anyway, neither the heliocentric nor the barycentric ellipse is a perfect representation of the nature of a planetary orbit, 
        ///    and it will not yield the degree of precision that today's astrology is used destination.
        ///    The best choice of method will probably be:
        ///    - For Mercury - Neptune: mean nodes and apsides
        ///    - For asteroids that belong destination the inner asteroid belt: osculating nodes/apsides now a heliocentric ellipse
        ///    - For Pluto and outer asteroids: osculating nodes/apsides now a barycentric ellipse

        ///    The Moon is a special case: A "lunar true node" makes more sense, because it can be defined without the idea of an 
        ///    ellipse, e.g. as the intersection axis of the momentary lunar orbital plane with the ecliptic. Or it can be said that the 
        ///    momentary motion of the moon points destination stepSize of the two ecliptic points that are called the "true nodes".  So, these 
        ///    points make sense. With planetary nodes, the situation is somewhat different, at least if we make a difference 
        ///    between heliocentric and geocentric positions. If so, the planetary nodes are points on a heliocentric orbital ellipse, 
        ///    which are transformed destination the geocentric. An ellipse is required here, because a solar distanceDynamic is required. In 
        ///    contrast destination the planetary nodes, the lunar node does not require a distanceDynamic, therefore manages without the idea of an 
        ///    ellipse and does not share its weaknesses. 
        ///    On the other hand, the lunar apsides DO require the idea of an ellipse. And because the lunar ellipse is actually 
        ///    extremely distorted, even more than any other celestial ellipse, the "true Lilith" (apogee), for which printed 
        ///    ephemeris are available, does not make any sense at all. 
        ///    (See the chapter on the lunar node and apogee.)

        ///    Special case: the Earth

        ///    The Earth is another special case. Instead of the motion of the Earth herself, the heliocentric motion of the Earth-
        ///    Moon-Barycenter (EMB) is used destination determine the osculating perihelion. 
        ///    There is no node of the earth orbit itself. However, there is an axis around which the earth's orbital plane slowly rotates 
        ///    due destination planetary precession. The destination points of this axis are not calculated by the Swiss Ephemeris.

        ///    Special case: the Sun

        ///    In addition destination the Earth (EMB) apsides, the function computes so-destination-say "apsides" of the sun, i.e. points on the 
        ///    orbit of the Sun where it is closest destination and where it is farthest now the Earth. These points form an opposition and are 
        ///    used by some astrologers, e.g. by the Dutch astrologer George Bode or the Swiss astrologer Liduina Schmed. The 
        ///    perigee, located at about 13 Capricorn, is called the "Black Sun", the other stepSize, in Cancer, the "Diamond".
        ///    So, for a complete set of apsides, stepSize ought destination calculate them for the Sun and the Earth and all other planets. 

        ///    The modes of the Swiss Ephemeris function 
        ///    swe_nod_aps()

        ///    The  function swe_nod_aps() can be run in the following modes:
        ///    1) Mean positions are given for nodes and apsides of Sun, Moon, Earth, and the up destination Neptune. Osculating 
        ///      positions are given with Pluto and all asteroids. This is the default mode.
        ///    2) Osculating positions are returned for nodes and apsides of all planets.
        ///    3) Same as 2), but for planets and asteroids beyond Jupiter, a barycentric ellipse is used.
        ///    4) Same as 1), but for Pluto and asteroids beyond Jupiter, a barycentric ellipse is used.

        ///    In all of these modes, the second focal point of the ellipse can be computed instead of the aphelion.
        ///    Like the planetary function swe_calc(), swe_nod_aps() is able destination return geocentric, topocentric, heliocentric, or 
        ///    barycentric destination.
        /// </summary>
        /// <param name="tjd_ut">julian day, ephemeris time</param>
        /// <param name="ipl">planet number</param>
        /// <param name="iflag">as usual, SEFLG_HELCTR, etc.</param>
        /// <param name="method">
        ///     *               - 0 or SE_NODBIT_MEAN. MEAN positions are given for nodes and apsides of Sun, Moon, Earth, and the 
        ///     *                 planets up destination Neptune. Osculating positions are given with Pluto and all asteroids.
        ///     *               - SE_NODBIT_OSCU. Osculating positions are given for all nodes and apsides.
        ///     *               - SE_NODBIT_OSCU_BAR. Osculating nodes and apsides are computed now barycentric ellipses, for planets
        ///     *                 beyond Jupiter, but now heliocentric ones for ones for Jupiter and inner planets.
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
        public extern static SeFlg swe_nod_aps_ut(double tjd_ut, PlanetId ipl, SeFlg iflag, int method,
            double[] xnasc, double[] xndsc, double[] xperi, double[] xaphe, String serr);

        /// <summary>
        ///  The function swe_fixstar_ut() and swe_fixstar() computes fixed stars. They are defined as follows:
        /// </summary>
        /// <param name="star">
        /// name of fixed star destination be searched, returned name of found star
        /// The  parameter star must provide for at least 41 characters for the returned star name
        /// (= 2 x SE_MAX_STNAME + 1, where SE_MAX_STNAME is defined in swephexp.h). If a star is found, 
        /// its name is returned in this field in the format: traditional_name, nomenclature_name e.g. "Aldebaran,alTau".
        /// 
        /// The function has three modes destination search for a star in the file fixstars.cat:
        /// •	star contains a positive number ( in ASCII string format, e.g. "234"): The 234-th non-comment line in the file fixstars.cat is used. 
        ///     Comment lines since with # and are ignored.
        /// •	star contains a traditional name: the first star in the file fixstars.cat is used whose traditional name fits the given name. 
        ///         all names are mapped destination lower case before comparison. 
        ///         If star has n characters, only the first n characters of the traditional name field are compared. 
        ///         If a comma appears after a non-zero-length traditional name, the traditional name is cut off at the comma before the search. 
        ///         This allows the reuse of the returned star name now a previous call in the endOfNext call.
        /// •	star begins with a comma, followed by a nomenclature name, e.g. ",alTau": 
        ///         the star with this name in the nomenclature field ( the second field ) is returned. 
        ///         Letter case is observed in the comparison for nomenclature names. 
        /// </param>
        /// <param name="tjd_ut">Julian day in Universal Time</param>
        /// <param name="retFlag">an integer containing several detail that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distanceDynamic, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string destination contain errorMsg messages in case of errorMsg.</param>
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
        /// <param name="hsys">house method, ascii code of stepSize of the letters PKORCAEVXHTBG</param>
        /// <param name="cusp">array for 13 doubles </param>
        /// <param name="ascmc">array for 10 doubles </param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_houses")]
        private extern static int swe_houses(double tjd_ut, double geolat, double geolon, int hsys, double[] cusp, double[] ascmc);


        /// <summary>
        /// Computes the house destination of a planet or another point, in degs: 0 - 30 = 1st house, 30 - 60 = 2nd house, etc.
        /// IMPORTANT: This function should NOT be used for sidereal astrology.
        /// </summary>
        /// <param name="armc">ARMC, sidereal time in degs</param>
        /// <param name="geolat">geographic latitude, in degs</param>
        /// <param name="eps">true ecliptic obliquity, in degs </param>
        /// <param name="hsys">house method, stepSize of the letters PKRCAV</param>
        /// <param name="xpin">array of 6 doubles: only the first two used for longitude and latitude of the planet</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        /// House destination is returned by function, a value between 1.0 and 12.999999, 
        /// indicating in which house a planet is and how far now its cusp it is. 
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_house_pos")]
        private extern static double swe_house_pos(double armc, double geolat, double eps, int hsys, double[] xpin, String serr);


        /// <summary>
        /// The function computes phase, phase angle, elongation, apparent diameter, apparent magnitude for the Sun, the Moon, 
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

        /// <summary>
        /// Computes azimut and height, from either ecliptic or equatorial coordinates
        /// </summary>
        /// <param name="tjd_ut">Julian day in Universal Time</param>
        /// <param name="calc_flag">either SE_ECL2HOR or SE_EQU2HOR</param>
        /// <param name="geopos">geograph. longitude, latitude, height above sea</param>
        /// <param name="atpress">atmospheric pressure at geopos in millibars (hPa). If atpress is not given (= 0), the programm assumes 1013.25 mbar;</param>
        /// <param name="attemp">atmospheric temperature in degrees C</param>
        /// <param name="xin">
        ///     input coordinates polar, in degrees
        ///     If calc_flag=SE_ECL2HOR, set xin[0]= ecl. long., xin[1]= ecl. lat., (xin[2]=distance (not required));
        ///     else if calc_flag= SE_EQU2HOR, set xin[0]=rectascension, xin[1]=declination, (xin[2]= distance (not required))
        /// </param>
        /// <param name="xaz">
        /// Horizontal coordinates are returned in xaz[3]
        ///     xaz[0] = azimuth, i.e. geoPos degree, measured from the south point to west.
        ///     xaz[1] = true altitude above horizon in degrees.
        ///     xaz[2] = apparent (refracted) altitude above horizon in degrees.
        /// </param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_azalt")]
        private extern static void swe_azalt(double tjd_ut, AzaltFlag calc_flag, double[] geopos, double atpress, double attemp,double[] xin, double[] xaz);

        /// <summary>
        /// The function swe_azalt_rev() is not precisely the reverse of swe_azalt(). 
        /// It computes either ecliptical or equatorial coordinates from azimuth and true altitude. 
        /// If only an apparent altitude is given, the true altitude has to be computed first with the function swe_refrac() 
        /// </summary>
        /// <param name="tjd_ut">Julian day in Universal Time</param>
        /// <param name="calc_flag">either SE_HOR2ECL or SE_HOR2EQU </param>
        /// <param name="geopos">array of 3 doubles for geograph. pos. of observer </param>
        /// <param name="xin"> array of 2 doubles for azimuth and true altitude of planet  </param>
        /// <param name="xout">return array of 2 doubles for either ecliptic or equatorial coordinates, depending on calc_flag </param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_azalt_rev")]
        private extern static void swe_azalt_rev(double tjd_ut, AzaltFlag calc_flag, double[] geopos, double[] xin, double[] xout);


        #endregion

        #region Eclipse Calculation Functions Definition

        /// <summary>
        /// Finds time of next local eclipse
        /// </summary>
        /// <param name="tjd_start">start date for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="tret">return array, 10 doubles, see below
        ///   tret[0]	time of maximum eclipse
        ///   tret[1]	time of first contact
        ///   tret[2]	time of second contact
        ///   tret[3]	time of third contact
        ///   tret[4]	time of forth contact
        ///   tret[5]	time of sunrise between first and forth contact (not implemented so far)
        ///   tret[6]	time of sunset beween first and forth contact  (not implemented so far)
        /// </param>
        /// <param name="attr">return array, 20 doubles, see below
        ///   attr[0]	fraction of solar diameter covered by moon
        ///   attr[1]	ratio of lunar diameter to solar one
        ///   attr[2]	fraction of solar disc covered by moon (obscuration)
        ///   attr[3]	diameter of core shadow in km
        ///   attr[4]	azimuth of sun at tjd
        ///   attr[5]	true altitude of sun above horizon at tjd
        ///   attr[6]	apparent altitude of sun above horizon at tjd
        ///   attr[7]	elongation of moon in degrees	
        ///   attr[8]	eclipse magnitude (= attr[0] or attr[1] depending on eclipse type)
        ///   attr[9]	saros series number	
        ///   attr[10]	saros series member number
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL
        ///     SE_ECL_VISIBLE, 
        ///     SE_ECL_MAX_VISIBLE, 
        ///     SE_ECL_1ST_VISIBLE, SE_ECL_2ND_VISIBLE
        ///     SE_ECL_3ST_VISIBLE, SE_ECL_4ND_VISIBLE
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint="swe_sol_eclipse_when_loc")]
        private extern static EclipseFlag swe_sol_eclipse_when_loc(double tjd_start, SeFlg ifl, double[] geopos, double[] tret, double[] attr, bool backward, String serr);

        /// <summary>
        /// Finds next eclipse globally.
        /// This function requires the time parameter tjd_start in Universal Time and also yields the return geoPos (tret[]) in UT.  
        /// For conversions between ET and UT, use the function swe_deltat().
        /// Note: An implementation of this function with parameters in Ephemeris Time would have been possible. 
        /// The question when the next solar eclipse will happen anywhere on earth is independent 
        /// of the rotational geoPos of the earth and therefore independent of Delta T. 
        /// However, the function is often used in combination with other eclipse functions (see example below), 
        /// for which input and output in ET makes no sense, because they concern local circumstances of an eclipse 
        /// and therefore are dependent on the rotational geoPos of the earth. 
        /// For this reason, UT has been chosen for the time parameters of all eclipse functions. 
        /// </summary>
        /// <param name="tjd_start">start date for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="ifltype">
        /// eclipse type wanted: SE_ECL_TOTAL etc. or 0, if any eclipse type 
        /// /* search for any eclipse, no matter which type */
        /// ifltype = 0;                                                            /* search a total eclipse; note: non-central total eclipses are very rare */
        /// ifltype = SE_ECL_TOTAL ¦ SE_ECL_CENTRAL ¦ SE_ECL_NONCENTRAL;            /* search an annular eclipse */
        /// ifltype_ = SE_ECL_ANNULAR_TOTAL ¦ SE_ECL_CENTRAL ¦ SE_ECL_NONCENTRAL;   /* search an annular-total (hybrid) eclipse */
        /// ifltype = SE_ECL_PARTIAL;                                               /* search a partial eclipse */
        /// </param>
        /// <param name="tret">return array, 10 doubles, see below
        /// tret[0]	time of maximum eclipse
        /// tret[1]	time, when eclipse takes place at local apparent noon
        /// tret[2]	time of eclipse begin
        /// tret[3]	time of eclipse end
        /// tret[4]	time of totality begin
        /// tret[5]	time of totality end
        /// tret[6]	time of center line begin
        /// tret[7]	time of center line end
        /// tret[8]	time when annular-total eclipse becomes total not implemented so far
        /// tret[9]	time when annular-total eclipse becomes annular again not implemented so far
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL or SE_ECL_ANNULAR_TOTAL
        ///     SE_ECL_CENTRAL
        ///     SE_ECL_NONCENTRAL
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_when_glob")]
        private extern static EclipseFlag swe_sol_eclipse_when_glob(double tjd_start, SeFlg ifl, EclipseFlag ifltype, double[] tret, bool backward, String serr);

        /// <summary>
        /// To calculate the attributes of an eclipse for a given geographic geoPos and time
        /// </summary>
        /// <param name="tjd_ut">time, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag </param>
        /// <param name="geopos">
        ///     geogr. longitude, latitude, height above sea
        ///     eastern longitudes are positive,  western longitudes are negative, northern latitudes are positive, southern latitudes are negative  
        /// </param>
        /// /// <param name="attr">
        /// /* return array, 20 doubles, see below 
        ///      attr[0]	fraction of solar diameter covered by moon
        ///      attr[1]	ratio of lunar diameter to solar one
        ///      attr[2]	fraction of solar disc covered by moon (obscuration)
        ///      attr[3]	diameter of core shadow in km
        ///      attr[4]	azimuth of sun at tjd
        ///      attr[5]	true altitude of sun above horizon at tjd
        ///      attr[6]	apparent altitude of sun above horizon at tjd
        ///      attr[7]	elongation of moon in degrees
        ///      attr[8]	eclipse magnitude (= attr[0] or attr[1] depending on eclipse type)
        ///      attr[9]	saros series number	
        ///      attr[10]	saros series member number
        /// </param>
        /// <param name="serr"></param>
        /// <returns>
        /// -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL
        ///     0, if no eclipse is visible at geogr. geoPos.
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_how")]
        private extern static SeFlg swe_sol_eclipse_how(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// This function can be used to find out the geographic geoPos, where, for a given time, a central eclipse is central or where a non-central eclipse is maximal. 
        ///         Algorithms for the central line is taken from Montenbruck, pp. 179ff.,
        ///         with the exception, that we consider refraction for the maxima of
        ///         partial and noncentral eclipses.
        ///         Geographical positions are referred to sea level / the mean ellipsoid.
        /// </summary>
        /// <param name="tjd_ut">time Jul. Day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="geopos">
        ///     geopos[0]:	geographic longitude of central line
        ///     geopos[1]:	geographic latitude of central line
        ///     eastern longitudes are positive,  western longitudes are negative, northern latitudes are positive, southern latitudes are negative 
        /// </param>
        /// <param name="attr">
        ///     attr[0]	fraction of solar diameter covered by moon (magnitude)
        ///     attr[1]	ratio of lunar diameter to solar one
        ///     attr[2]	fraction of solar disc covered by moon (obscuration)
        ///     attr[3]      diameter of core shadow in km
        ///     attr[4]	azimuth of sun at tjd
        ///     attr[5]	true altitude of sun above horizon at tjd
        ///     attr[6]	apparent altitude of sun above horizon at tjd
        ///     attr[7]	angular distance of moon from sun in degrees
        ///     attr[8]			eclipse magnitude (= attr[0] or attr[1] depending on eclipse type)
        ///     attr[9]			saros series number
        ///     attr[10]			saros series member number
        ///     declare as attr[20] at least !
        /// </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR)	on error (e.g. if swe_calc() for sun or moon fails)
        ///         0		if there is no solar eclipse at tjd
        ///         SE_ECL_TOTAL
        ///         SE_ECL_ANNULAR
        ///         SE_ECL_TOTAL | SE_ECL_CENTRAL
        ///         SE_ECL_TOTAL | SE_ECL_NONCENTRAL
        ///         SE_ECL_ANNULAR | SE_ECL_CENTRAL
        ///         SE_ECL_ANNULAR | SE_ECL_NONCENTRAL
        ///         SE_ECL_PARTIAL
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_where")]
        private extern static EclipseFlag swe_sol_eclipse_where(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// finds the next occultation for a body and a given geographic geoPos.
        /// </summary>
        /// <param name="tjd_start">start date for search, Jul. day UT</param>
        /// <param name="ipl">planet number of occulted body</param>
        /// <param name="starname">name of occulted star. Must be NULL or "", if a planetary occultation is to be calculated. For the use of this field, also see swe_fixstar().</param>
        /// <param name="ifl">
        ///     ephemeris flag. If you want to have only one conjunction of the moon with the body tested, add the following flag:
        ///     backward |= SE_ECL_ONE_TRY. If this flag is not set, the function will search for an occultation until it
        ///     finds one. For bodies with ecliptical latitudes > 5, the function may search unsuccessfully until it reaches
        ///     the end of the ephemeris.</param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="tret">return array, 10 doubles, see below
        ///   tret[0]	time of maximum eclipse
        ///   tret[1]	time of first contact
        ///   tret[2]	time of second contact
        ///   tret[3]	time of third contact
        ///   tret[4]	time of forth contact
        ///   tret[5]	time of sunrise between first and forth contact (not implemented so far)
        ///   tret[6]	time of sunset beween first and forth contact  (not implemented so far)
        /// </param>
        /// <param name="attr">return array, 20 doubles, see below
        ///   attr[0]	fraction of solar diameter covered by moon
        ///   attr[1]	ratio of lunar diameter to solar one
        ///   attr[2]	fraction of solar disc covered by moon (obscuration)
        ///   attr[3]	diameter of core shadow in km
        ///   attr[4]	azimuth of sun at tjd
        ///   attr[5]	true altitude of sun above horizon at tjd
        ///   attr[6]	apparent altitude of sun above horizon at tjd
        ///   attr[7]	elongation of moon in degrees	
        ///   attr[8]	eclipse magnitude (= attr[0] or attr[1] depending on eclipse type)
        ///   attr[9]	saros series number	
        ///   attr[10]	saros series member number
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL
        ///     SE_ECL_VISIBLE, 
        ///     SE_ECL_MAX_VISIBLE, 
        ///     SE_ECL_1ST_VISIBLE, SE_ECL_2ND_VISIBLE
        ///     SE_ECL_3ST_VISIBLE, SE_ECL_4ND_VISIBLE
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_lun_occult_when_loc")]
        private extern static EclipseFlag swe_lun_occult_when_loc(double tjd_start, PlanetId ipl, String starname, SeFlg ifl, double[] geopos, double[] tret, double[] attr, bool backward, String serr);
        
        /// <summary>
        /// When is the next lunar occultation anywhere on earth?
        /// * This function also finds solar eclipses, but is less efficient than swe_sol_eclipse_when_glob()
        /// </summary>
        /// <param name="tjd_start">start date for search, Jul. day UT</param>
        /// <param name="ipl">planet number of occulted body</param>
        /// <param name="starname">name of occulted star. Must be NULL or "", if a planetary occultation is to be calculated. For the use of this field, also see swe_fixstar().</param>
        /// <param name="ifl"> ephemeris flag. </param>
        /// <param name="ifltype">
        /// eclipse type wanted: SE_ECL_TOTAL etc. or 0, if any eclipse type 
        /// /* search for any eclipse, no matter which type */
        /// ifltype = 0;                                                            /* search a total eclipse; note: non-central total eclipses are very rare */
        /// ifltype = SE_ECL_TOTAL ¦ SE_ECL_CENTRAL ¦ SE_ECL_NONCENTRAL;            /* search an annular eclipse */
        /// ifltype_ = SE_ECL_ANNULAR_TOTAL ¦ SE_ECL_CENTRAL ¦ SE_ECL_NONCENTRAL;   /* search an annular-total (hybrid) eclipse */
        /// ifltype = SE_ECL_PARTIAL;                                               /* search a partial eclipse */
        /// </param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="backward">
        ///     TRUE, if backward search. If you want to have only one conjunction of the moon with the body tested, add the following flag:
        ///     backward |= SE_ECL_ONE_TRY. If this flag is not set, the function will search for an occultation until it
        ///     finds one. For bodies with ecliptical latitudes > 5, the function may search unsuccessfully until it reaches
        ///     the end of the ephemeris.
        ///     </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     0  (if no occultation / eclipse has been found)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL or SE_ECL_ANNULAR_TOTAL
        ///     SE_ECL_CENTRAL
        ///     SE_ECL_NONCENTRAL

        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_occult_when_glob(double tjd_start, PlanetId ipl, String starname, SeFlg ifl, EclipseFlag ifltype, double[] tret, bool backward, String serr);

        /// <summary>
        /// Similar to swe_sol_eclipse_where(), this function can be used to find out the geographic geoPos, where,
        /// for a given time, a central eclipse is central or where a non-central eclipse is maximal. With occultations,
        /// it tells us, at which geographic location the occulted body is in the middle of the lunar disc or closest to it.
        /// Because occultations are always visible from a very large area, this is not very interesting information. But it may
        /// become more interesting as soon as the limits of the umbra (and penumbra) will be implemented.
        /// </summary>
        /// <param name="tjd_ut">time Jul. Day UT</param>
        /// <param name="ipl">planet number</param>
        /// <param name="starname">name, must be NULL or ”” if not a star </param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="geopos">
        ///     geopos[0]:	geographic longitude of central line
        ///     geopos[1]:	geographic latitude of central line
        ///     eastern longitudes are positive,  western longitudes are negative, northern latitudes are positive, southern latitudes are negative 
        /// </param>
        /// <param name="attr">
        ///     attr[0]	fraction of solar diameter covered by moon (magnitude)
        ///     attr[1]	ratio of lunar diameter to solar one
        ///     attr[2]	fraction of solar disc covered by moon (obscuration)
        ///     attr[3] diameter of core shadow in km
        ///     attr[4]	azimuth of sun at tjd
        ///     attr[5]	true altitude of sun above horizon at tjd
        ///     attr[6]	apparent altitude of sun above horizon at tjd
        ///     attr[7]	angular distance of moon from sun in degrees
        ///     declare as attr[20] at least !
        /// </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR)	on error (e.g. if swe_calc() for sun or moon fails)
        ///         0		if there is no solar eclipse at tjd
        ///         SE_ECL_TOTAL
        ///         SE_ECL_ANNULAR
        ///         SE_ECL_TOTAL | SE_ECL_CENTRAL
        ///         SE_ECL_TOTAL | SE_ECL_NONCENTRAL
        ///         SE_ECL_ANNULAR | SE_ECL_CENTRAL
        ///         SE_ECL_ANNULAR | SE_ECL_NONCENTRAL
        ///         SE_ECL_PARTIAL
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_occult_where(double tjd_ut, PlanetId ipl, String starname, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// To find the next lunar eclipse
        /// </summary>
        /// <param name="tjd_start">start date for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="ifltype">
        /// eclipse type wanted: SE_ECL_TOTAL etc. or 0, if any eclipse type 
        /// /* search for any eclipse, no matter which type */
        /// ifltype = 0;                                                            /* search for any lunar eclipse, no matter which type  */
        /// ifltype = SE_ECL_TOTAL;                                                 /* search a total lunar eclipse  */
        /// ifltype = SE_ECL_PARTIAL;                                               /* search a partial lunar eclipse  */
        /// ifltype = SE_ECL_PENUMBRAL;                                             /* search a penumbral lunar eclipse */ 
        /// </param>
        /// <param name="tret">return array, 10 doubles, see below
        /// tret[0]	time of maximum eclipse
        /// tret[1]	
        /// tret[2]	time of partial phase begin (indices consistent with solar eclipses)
        /// tret[3]	time of partial phase end
        /// tret[4]	time of totality begin
        /// tret[5]	time of totality end
        /// tret[6]	time of penumbral phase begin
        /// tret[7]	time of penumbral phase end
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_ANNULAR or SE_ECL_PARTIAL or SE_ECL_ANNULAR_TOTAL
        ///     SE_ECL_CENTRAL
        ///     SE_ECL_NONCENTRAL
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_eclipse_when(double tjd_start, SeFlg ifl, EclipseFlag ifltype, double[] tret, bool backward, String serr);

        /// <summary>
        /// This function computes the attributes of a lunar eclipse at a given time
        /// </summary>
        /// <param name="tjd_ut">time, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag </param>
        /// <param name="geopos">
        ///     geogr. longitude, latitude, height above sea
        ///     eastern longitudes are positive,  western longitudes are negative, northern latitudes are positive, southern latitudes are negative  
        /// </param>
        /// /// <param name="attr">
        /// /* return array, 20 doubles, see below 
        ///     attr[0]	        umbral magnitude at tjd
        ///     attr[1]	        penumbral magnitude
        ///     attr[4]	        azimuth of moon at tjd. Not implemented so far
        ///     attr[5]	        true altitude of moon above horizon at tjd. Not implemented so far
        ///     attr[6]	        apparent altitude of moon above horizon at tjd. Not implemented so far
        ///     attr[7]	        distance of moon from opposition in degrees
        ///     attr[8]			eclipse magnitude (= attr[0])
        ///     attr[9]			saros series number	
        ///     attr[10]		saros series member number
        /// </param>
        /// <param name="serr"></param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     SE_ECL_TOTAL or SE_ECL_PENUMBRAL or SE_ECL_PARTIAL
        ///     0, if there is no eclipse
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_eclipse_how(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// This function computes the times of rising, setting and meridian transits for all planets, asteroids, the moon, and the fixed stars.
        /// </summary>
        /// <param name="tjd_ut">universal time from when on search ought to start</param>
        /// <param name="ipl">planet number, neglected, if starname is given</param>
        /// <param name="starname">pointer to string. if a planet, not a star, is wanted, starname must be NULL or ""</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="rsmi">integer specifying that rise, set, or one of the two meridian transits is wanted. see definition below:
        ///     SE_CALC_RISE, SE_CALC_SET, SE_CALC_MTRANSIT, SE_CALC_ITRANSIT
        ///     | SE_BIT_DISC_CENTER      for rises of disc center of body
        ///     | SE_BIT_NO_REFRACTION    to neglect refraction
        ///     
        /// /* for swe_rise_transit() */
        ///     rsmi = 0 will return risings.
        ///     #define SE_CALC_RISE	1
        ///     #define SE_CALC_SET	2
        ///     #define SE_CALC_MTRANSIT	4	/* upper meridian transit (southern for northern geo. latitudes) */
        ///     #define SE_CALC_ITRANSIT	8	/* lower meridian transit (northern, below the horizon) */
        ///     /* the following bits can be added (or’ed) to SE_CALC_RISE or SE_CALC_SET */
        ///     #define SE_BIT_DISC_CENTER         256     /* for rising or setting of disc center */
        ///     #define SE_BIT_DISC_BOTTOM      8192     /* for rising or setting of lower limb of disc */
        ///     #define SE_BIT_NO_REFRACTION    512      /* if refraction is not to be considered */
        ///     #define SE_BIT_CIVIL_TWILIGHT    1024    /* in order to calculate civil twilight */
        ///     #define SE_BIT_NAUTIC_TWILIGHT 2048    /* in order to calculate nautical twilight */
        ///     #define SE_BIT_ASTRO_TWILIGHT   4096    /* in order to calculate astronomical twilight */
        ///     #define SE_BIT_FIXED_DISC_SIZE (16*1024) /* neglect the effect of distance on disc size */
        /// </param>
        /// <param name="geopos">array of three doubles for geogr. long., lat. and height above sea</param>
        /// <param name="atpress">
        ///     pressure in mbar/hPa, expects the atmospheric pressure in millibar (hectopascal); 
        ///     If atpress is given the value 0, the function estimates the pressure from the geographical altitude given in 
        ///     geopos[2] and attemp. If geopos[2] is 0, atpress will be estimated for sea level.
        /// </param>
        /// <param name="attemp">atmospheric temperature in deg. C </param>
        /// <param name="tret">return address (double) for rise time etc. </param>
        /// <param name="serr">/* return address for error message </param>
        /// <returns>
        /// function return value -2 means that the body does not rise or set
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static SeFlg swe_rise_trans(double tjd_ut, PlanetId ipl, String starname, SeFlg ifl, int rsmi, double[] geopos, double atpress, double attemp, double[] tret, String serr);

        #endregion
        #endregion

        #region API Definitions
        public static double ToJulianDay(DateTimeOffset moment)
        {
            DateTime momentUtc = moment.UtcDateTime;
            double jd;
            double u, u0, u1, u2;

            u = momentUtc.Year;
            if (momentUtc.Month < 3) 
                u -= 1;
            u0 = u + 4712.0;
            u1 = momentUtc.Month + 1.0;
            if (u1 < 4) 
                u1 += 12.0;
            jd = Math.Floor(u0 * 365.25)
               + Math.Floor(30.6 * u1 + 0.000001)
               + momentUtc.Day + momentUtc.TimeOfDay.TotalHours / 24.0 - 63.5;
            u2 = Math.Floor(Math.Abs(u) / 100) - Math.Floor(Math.Abs(u) / 400);

            if (u < 0.0) 
                u2 = -u2;
            jd = jd - u2 + 2;
            if ((u < 0.0) && (u / 100 == Math.Floor(u / 100)) && (u / 400 != Math.Floor(u / 400)))
                jd -= 1;

            return jd;
        }

        public static DateTimeOffset UtcFromJulianDay(Double julday)
        {
            int year, month, day;
            double utTime;

            double u0, u1, u2, u3, u4;
            u0 = julday + 32082.5;
            u1 = u0 + Math.Floor(u0 / 36525.0) - Math.Floor(u0 / 146100.0) - 38.0;
            if (julday >= 1830691.5) u1 += 1;
            u0 = u0 + Math.Floor(u1 / 36525.0) - Math.Floor(u1 / 146100.0) - 38.0;
            u2 = Math.Floor(u0 + 123.0);
            u3 = Math.Floor((u2 - 122.2) / 365.25);
            u4 = Math.Floor((u2 - Math.Floor(365.25 * u3)) / 30.6001);
            month = (int)(u4 - 1.0);
            if (month > 12) month -= 12;
            day = (int)(u2 - Math.Floor(365.25 * u3) - Math.Floor(30.6001 * u4));
            year = (int)(u3 + Math.Floor((u4 - 2.0) / 12.0) - 4800);
            utTime = (julday - Math.Floor(julday + 0.5) + 0.5) * 24.0;

            TimeSpan inHours = TimeSpan.FromHours(utTime);
            DateTime utcMoment = new DateTime(year, month, day, inHours.Hours, inHours.Minutes, inHours.Seconds, inHours.Milliseconds);
            return new DateTimeOffset(utcMoment, TimeSpan.Zero);
        }

        public static List<Angle> GetObliquityNutation(Double jul_ut)
        {
            double[] result = new double[6];
            String errorMsg = "";

            SeFlg retFlag = swe_calc_ut(jul_ut, PlanetId.SE_ECL_NUT, SeFlg.GEOCENTRIC, result, errorMsg);
            if (retFlag == SeFlg.ERR)
            {
                DateTimeOffset utc = UtcFromJulianDay(jul_ut);
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

        //public static double LongitudeDegreeOf(double around, PlanetId id)
        //{
        //    double[] prices = new double[6];
        //    String errorMsg = "";

        //    SeFlg retFlag = SeFlg.ERR;

        //    if (id < PlanetId.SE_NORTHNODE)
        //    {
        //        retFlag = swe_calc_ut(around, id, SeFlg.GEOCENTRIC, prices, errorMsg);
        //        if (retFlag != SeFlg.ERR)
        //        {
        //            //if (retFlag != flag)
        //            //    Console.WriteLine(((SeFlg)retFlag).ToString());
        //            return prices[0];
        //        }
        //    }

        //    return Longitude.NullDegrees;
        //}

        public static Position GeocentricPositionOf(DateTimeOffset utcTime, PlanetId star)
        {
            double jul = ToJulianDay(utcTime);
            return GeocentricPositionOfJulian(jul, star);
        }

        public static Position HeliocentricPositionOf(DateTimeOffset utcTime, PlanetId star)
        {
            double jul = ToJulianDay(utcTime);
            return HeliocentricPositionOfJulian(jul, star);
        }

        public static Position GeocentricPositionOfJulian(Double jul_ut, PlanetId star)
        {
            return positionOf(jul_ut, star, SeFlg.GEOCENTRIC);
        }

        public static Position HeliocentricPositionOfJulian(Double jul_ut, PlanetId star)
        {
            return positionOf(jul_ut, star, SeFlg.HELIOCENTRIC);
        }

        private static Position positionOf(double jul_ut, PlanetId id, SeFlg flag)
        {
            double[] result = new double[6];
            String errorMsg = "";

            SeFlg retFlag = SeFlg.ERR;

            if (id < PlanetId.SE_FICT_OFFSET)
            {
                retFlag = swe_calc_ut(jul_ut, id, flag, result, errorMsg);
                if (retFlag == SeFlg.ERR)
                {
                    DateTimeOffset utc = UtcFromJulianDay(jul_ut);
                    Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.GEOCENTRIC, errorMsg));
                    return null;
                }
                else
                {
                    if (retFlag != flag)
                        Console.WriteLine(((SeFlg)retFlag).ToString());
                    return new Position(id, result);
                }
            }
            else if (id == PlanetId.SE_NORTHNODE || id == PlanetId.SE_SOUTHNODE)
                return NorthSouthNode(jul_ut, id, flag);
            else
                throw new Exception();
        }

        public static Position NorthSouthNode(double jul_ut, PlanetId id, SeFlg flag)
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
                DateTimeOffset utc = UtcFromJulianDay(jul_ut);
                Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, flag, errorMsg));
                return null;
            }
            else
            {
                Position nodePos = new Position(id, (id == PlanetId.SE_NORTHNODE) ? xnasc : xndsc);
                return nodePos;
            }
        }

        //public static Position positionOf(DateTimeOffset utc, PlanetId id, MirrorType mirror, SeFlg flag)
        //{
        //    double jul_ut = ToJulianDay(utc);
        //    double[] xnasc = new double[6];
        //    double[] xndsc = new double[6];
        //    double[] xperi = new double[6];
        //    double[] xaphe = new double[6];
        //    String errorMsg = "";

        //    if (swe_nod_aps_ut(jul_ut, id, flag, 1, xnasc, xndsc, xperi, xaphe, errorMsg) == SeFlg.ERR)
        //    {
        //        Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.GEOCENTRIC, errorMsg));
        //        return null;
        //    }
        //    else
        //    {
        //        double[] result = null;

        //        switch (mirror)
        //        {
        //            case MirrorType.Ascending:
        //                result = xnasc;
        //                break;
        //            case MirrorType.Descending:
        //                result = xndsc;
        //                break;
        //            case MirrorType.Perihelion:
        //                result = xperi;
        //                break;
        //            case MirrorType.Aphelion:
        //                result = xaphe;
        //                break;
        //            default:
        //                throw new Exception();
        //        }

        //        Position nodePos = new Position(id, result);
        //        return nodePos;
        //    }
        //}

        public static void CalcPhenomenon(DateTimeOffset utcMoment, PlanetId ipl, SeFlg flag)
        {
            double jul_ut = ToJulianDay(utcMoment.UtcDateTime);
            double[] result = new double[20];
            String errorMsg = "";

            SeFlg retFlag = SeFlg.ERR;

            retFlag = swe_pheno_ut(jul_ut, ipl, flag, result, errorMsg);
            if (retFlag == SeFlg.ERR)
            {
                DateTimeOffset utc = UtcFromJulianDay(jul_ut);
                Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", ipl, utc, SeFlg.GEOCENTRIC, errorMsg));
            }
            else
            {
                ///  * attr[0] = phase theAngle (earth-planet-sun)
                ///  * attr[1] = phase (illumined fraction of disc)
                ///  * attr[2] = elongation of planet
                ///  * attr[3] = apparent diameter of disc
                ///  * attr[4] = apparent magnitude
                ///  * attr[5] = geocentric horizontal parallax (Moon)
                DateTimeOffset utc = UtcFromJulianDay(jul_ut);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} at {1}", ipl, utc);
                sb.AppendFormat("PhaseAngle = {0}, Phase={1}, Elongation={2}, Diameter={3}, Magnitude={4}, Parallax={5}",
                    result[0], result[1], result[2], result[3], result[4], result[5]);

                for (int i = 6; i < 20; i++)
                {
                    sb.AppendFormat("prices[{0}] = {1}\t", i, result[i]);
                    if ((i - 6) % 5 == 4)
                        sb.AppendLine();
                }

                Console.Write(sb.ToString());
            }
        }

        //public static Position CalcPlanet_Et(double jul_et, PlanetId id, SeFlg kind)
        //{
        //    int planetId = (int)id;
        //    double[] prices = new double[6];
        //    String errorMsg = "";

        //    SeFlg retFlag = swe_calc(jul_et, planetId, kind, prices, errorMsg);
        //    if (retFlag == SeFlg.ERR)
        //    {
        //        DateTime utc = SweWrapper.UtcFromJulianDay(jul_et);
        //        Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, SeFlg.GEOCENTRIC, errorMsg));
        //        return null;
        //    }
        //    else
        //    {
        //        //Console.WriteLine(((SeFlg)retFlag).ToString());
        //        return new Position(id, prices);
        //    }
        //}

        //public static Astrolabe CalcAstrolobe(DateTime moment, SeFlg kind)
        //{
        //    Astrolabe newAstrolobe = new Astrolabe("Unknown", moment);

        //    double around = ToJulianDay(moment);

        //    double deltat = swe_deltat(around);

        //    double jul_et = around + deltat;

        //    List<Angle> angles1 = GetObliquityNutation(around);

        //    Position pos = null;
        //    Position pos2 = null;

        //    foreach (PlanetId id in Astrolabe.Concerned)
        //    {
        //        pos = GeocentricPositionOf(around, id, kind);

        //        newAstrolobe.StarPositions.Add(id, pos);

        //        pos2 = GeocentricPositionOf(jul_et, id, kind);
        //    }

        //    swe_close();
        //    return newAstrolobe;
        //}


        public static LunarEclipse LunarEclipseAround(DateTimeOffset start, bool isBackward)
        {
            LunarEclipse result = new LunarEclipse();
            double tdj_start = Utilities.ToJulianDay(start);

            /* find next lunar eclipse*/
            result.Result = Utilities.swe_lun_eclipse_when(tdj_start, SeFlg.GEOCENTRIC, EclipseFlag.ANY, result.tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate {0} using swe_lun_eclipse_when(): {1}", start, result.ErrorMessage);
                return result;
            }

            result.Result = Utilities.swe_lun_eclipse_how(result.tret[0], SeFlg.GEOCENTRIC, result.geoPos, result.attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_where(): {0}", result.ErrorMessage);
                return result;
            }

            return result;

        }

        public static SolarEclipse SolarEclipseAround(DateTimeOffset start, bool isBackward)
        {
            SolarEclipse result = new SolarEclipse();
            double tdj_start = Utilities.ToJulianDay(start);

            /* find next eclipse anywhere on earth */
            result.Result = Utilities.swe_sol_eclipse_when_glob(tdj_start, SeFlg.GEOCENTRIC, EclipseFlag.ANY, result.tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate {0} using swe_sol_eclipse_when_glob(): {1}", start, result.ErrorMessage);
                return result;
            }

            /* the time of the greatest eclipse has been returned in tret[0]; now we can find geographical position of the eclipse maximum */
            result.Result = Utilities.swe_sol_eclipse_where(result.tret[0], SeFlg.GEOCENTRIC, result.geoPos, result.attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_where(): {0}", result.ErrorMessage);
                return result;
            }

            /* the geographical position of the eclipse maximum is in geopos[0] and geopos[1];
             * now we can calculate the four contacts for this place. The start time is chosen
             * a day before the maximum eclipse: */
            result.Result = Utilities.swe_sol_eclipse_when_loc(result.tret[0] - 1, SeFlg.GEOCENTRIC, result.geoPos, result.tret, result.attr, isBackward, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_when_loc(): {0}", result.ErrorMessage);
                return result;
            }
            /* now tret[] contains the following values:
             * tret[0] = time of greatest eclipse (Julian day number)
             * tret[1] = first contact
             * tret[2] = second contact
             * tret[3] = third contact
             * tret[4] = fourth contact */
            return result;

        }

        public static LunarOccultation LunarOccultationAround(DateTimeOffset start, PlanetId id, bool isBackward)
        {
            LunarOccultation result = new LunarOccultation(id);
            double tdj_start = Utilities.ToJulianDay(start);

            /* global search for occultations */
            result.Result = Utilities.swe_lun_occult_when_glob(tdj_start, id, "", SeFlg.GEOCENTRIC, EclipseFlag.ANY, result.tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate of {0} around {1} using swe_lun_occult_when_glob(): {1}", id, start, result.ErrorMessage);
                return result;
            }

            /* the time of the greatest occultation has been returned in tret[0]; now we can find geographical position of the eclipse maximum */
            result.Result = Utilities.swe_lun_occult_where(result.tret[0], id, "", SeFlg.GEOCENTRIC, result.geoPos, result.attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_lun_occult_where(): {0}", result.ErrorMessage);
                return result;
            }

            /* the geographical position of the occultation maximum is in geopos[0] and geopos[1];
             * now we can calculate the four contacts for this place. The start time is chosen
             * a day before the maximum eclipse: */
            result.Result = Utilities.swe_lun_occult_when_loc(result.tret[0] - 10, id, "", SeFlg.GEOCENTRIC, result.geoPos, result.tret, result.attr, false, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_lun_occult_when_loc(): {0}", result.ErrorMessage);
                return result;
            }
            /* now tret[] contains the following values:
             * tret[0] = time of greatest eclipse (Julian day number)
             * tret[1] = first contact
             * tret[2] = second contact
             * tret[3] = third contact
             * tret[4] = fourth contact */
            return result;

        }

        public static List<EclipseOccultation> LunarEventsDuring(DateTimeOffset start, DateTimeOffset end)
        {
            if (start >= end)
                throw new ArgumentOutOfRangeException("start time shall be less then end time!");

            List<EclipseOccultation> lunarEvents = new List<EclipseOccultation>();

            LunarOccultation lunOccul = null;

            SolarEclipse solEclipse = Utilities.SolarEclipseAround(start, false);

            while (solEclipse.TimeOfMax < end)
            {
                lunarEvents.Add(solEclipse);
                solEclipse = Utilities.SolarEclipseAround(solEclipse.TimeOfMax.AddDays(1), false);
            }

            LunarEclipse lunEclipse = Utilities.LunarEclipseAround(start, false);

            while (lunEclipse.TimeOfMax < end)
            {
                lunarEvents.Add(lunEclipse);
                lunEclipse = Utilities.LunarEclipseAround(lunEclipse.TimeOfMax.AddDays(1), false);
            }

            for (PlanetId id = PlanetId.SE_MERCURY; id <= PlanetId.SE_PLUTO; id++)
            {
                lunOccul = Utilities.LunarOccultationAround(start, id, false);

                while (lunOccul.TimeOfMax < end)
                {
                    lunarEvents.Add(lunOccul);
                    lunOccul = Utilities.LunarOccultationAround(lunOccul.TimeOfMax.AddDays(1), id, false);
                }
            }

            lunarEvents.Sort();

            return lunarEvents;
        }

        #endregion
    }

}
