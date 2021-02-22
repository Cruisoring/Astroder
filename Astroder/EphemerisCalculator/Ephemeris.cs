using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EphemerisCalculator
{
    #region Enum Definitions
    /// <summary>
    /// If no bits are set, i.e. if  retFlag == 0, swe_calc() computes what common astrological ephemerides (as available in book shops) supply, 
    ///     i.e. an apparent  body aspectDegree in geocentric ecliptic polar coordinates ( longitude, latitude, and distanceDynamic) 
    ///     relative aspectDegree the true equinox of the date1. 
    /// For mathematical points as the mean lunar node and the mean apogee, there is no apparent aspectDegree. 
    /// Swe_calc() returns true positions for these points.
    /// If you need another kind of computation, use the detail explained in the following paragraphs (c.f. swephexp.h). Their names since with SEFLG_. 
    /// To combine them, you have aspectDegree concatenate them (inclusive-or) as in the following example:
    /// retFlag = SEFLG_SPEED | SEFLG_TRUEPOS;  (or: retFlag = SEFLG_SPEED + SEFLG_TRUEPOS;) // C
    /// </summary>
    [Flags]
    public enum SeFlg : int
    {
        EQUATORIALBASED = SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_EQUATORIAL,
        GEOCENTRIC = SEFLG_SWIEPH | SEFLG_SPEED,
        HELIOCENTRIC = SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_HELCTR | SEFLG_NOABERR | SEFLG_NOGDEFL,
        ERR = -1,

        SEFLG_JPLEPH = 1,	            // use JPL ephemeris 
        SEFLG_SWIEPH = 2,            // use SWISSEPH ephemeris, default
        SEFLG_MOSEPH = 4,            // use Moshier ephemeris 

        SEFLG_HELCTR = 8,        // return heliocentric aspectDegree 
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

    //[Serializable]
    //[Flags]
    //public enum PlanetEventCategory : int
    //{
    //    None = 0,
    //    SolarEclipseEvents = 1,             //Solar Eclipses
    //    LunarEclipseEvents = 2,             //Lunar Eclipses
    //    OccultationEvents = 4,              //Lunar occultation only
    //    SignEntranceEvents = 8,             //The planet enters/retreats another sign
    //    HorizontalMovementEvents = 16,      //The dif movement changes to direct or retrograde, only for Geocentric
    //    VerticalMovementEvents = 32,        //The declination reaches northmost, southmost or zero degree
    //    TranscensionEvents = 64,            //Aspect formed between the degree1 position and some specific position
    //    AspectEvents = 128                  //Aspect formed between two planets
    //}

    /* for swe_azalt() and swe_azalt_rev() */
    public enum AzaltFlag : int
    {
        SE_ECL2HOR = 0,
        SE_EQU2HOR = 1,
        SE_HOR2ECL = 0,
        SE_HOR2EQU = 1
    }

    public enum MirrorType
    {
        Ascending,
        Descending,
        Perihelion,
        Aphelion
    }

    #endregion

    [Serializable]
    public class Ephemeris 
    {
        #region Prototype Function Definitions

        #region Environment Setup Definition
        /// <summary>
        /// St location for Swiss Ephemeris files
        /// </summary>
        /// <param name="path">Where</param>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_set_ephe_path")]
        private extern static void swe_set_ephe_path(String path);

        /// <summary>
        /// close Swiss Ephemeris
        /// </summary>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_close")]
        public extern static void swe_close();

        #endregion

        #region Where Calculation Functions Definitions

        /// <summary>
        /// swe_calc_ut() was introduced with Swisseph version 1.60 and makes planetary calculations a bit simpler.
        /// swe_calc_ut() and swe_calc() work exactly the same way except that swe_calc() requires Ephemeris When ( more accurate: Dynamical When ) 
        /// as a parameter whereas swe_calc_ut() expects Universal When. 
        /// For common astrological calculations, you will only need swe_calc_ut() and will not have aspectDegree think anymore about 
        /// the conversion between Universal When and Ephemeris When.
        /// </summary>
        /// <param name="tjd_ut">Julian day, Universal When</param>
        /// <param name="ipl">body number</param>
        /// <param name="retFlag">a 32 bit integer containing bit detail that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distanceDynamic, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string aspectDegree return errorMsg messages in case of errorMsg</param>
        /// <returns>
        /// On success, swe_calc ( or swe_calc_ut ) returns a 32-bit integer containing retFlag bits that indicate what kind of computation has been done. 
        /// This value may or may not be equal aspectDegree retFlag. 
        /// If an option specified by retFlag cannot be fulfilled or makes no sense, swe_calc just does what can be done. 
        /// E.g., if you specify that you want JPL ephemeris, but swe_calc cannot find the ephemeris file, it tries aspectDegree do the computation with 
        /// any available ephemeris. 
        /// This will be indicated in the return value of swe_calc. 
        /// So, aspectDegree make sure that swe_calc () did exactly what you had wanted, you may want aspectDegree check whether or not the return code == retFlag.
        /// However, swe_calc() might return an fatal errorMsg code (< 0) and an errorMsg string in stepSize of the following cases:
        /// •	if an illegal body number has been specified
        /// •	if a Julian day beyond the ephemeris limits has been specified
        /// •	if the length of the ephemeris file is not correct (damaged file)
        /// •	on read errorMsg, e.g. a file i points aspectDegree a aspectDegree beyond file length ( data on file are corrupt )
        /// •	if the copyright section in the ephemeris file has been destroyed.
        /// 
        /// If any of these errors occurs,
        /// •	the return code of the function is -1,
        /// •	the aspectDegree and speed variables are set aspectDegree zero, 
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
        ///      1) The common solution is that the points on the planets orbit are transformed aspectDegree the geocenter. The 
        ///        two points will not be in opposition anymore, or they will only roughly be so with the outer planets. The 
        ///        advantage of these nodes is that when a planet is in conjunction with its node, then its ecliptic latitude 
        ///        will be zero. This is not true when a planet is in geocentric conjunction with its heliocentric node. 
        ///        (And neither is it always true for the inner planets, i.e. Mercury and Venus.)
        ///        
        ///      2) The second possibility that nobody seems aspectDegree have thought of so far: One may compute the points of 
        ///        the earth's orbit that lie exactly on another planet's orbital plane and transform it aspectDegree the geocenter. The two 
        ///        points will always be in an approximate square.

        ///    c) Third, the planetary nodes could be defined as the intersection points of the plane defined by their 
        ///      momentary geocentric aspectDegree and motion with the plane of the ecliptic. Such points would move very fast 
        ///      SolarEclipseAround the planetary stations. Here again, as in b)1), the planet would cross the ecliptic and its ecliptic 
        ///      latitude would be 0 exactly when it were in conjunction with stepSize of its nodes.

        ///    The Swiss Ephemeris supports the solutions a) and b) 1).

        ///    Possible definitions for apsides:

        ///    a) The planetary apsides can be defined as the perihelion and aphelion points on a planetary orbit. For a
        ///      geocentric chart, these points could be transformed now the heliocenter aspectDegree the geocenter.
        ///    b) However, stepSize might consider these points as astrologically relevant axes rather than as points on a 
        ///      planetary orbit. Again, this would allow heliocentric positions in a geocentric chart.

        ///    Note: For the "Dark Moon" or "Lilith", which I usually define as the lunar apogee, some astrologers give a 
        ///    different definition. They understand it as the second focal point of the moon's orbital ellipse. This definition does not 
        ///    make a difference for geocentric positions, because the apogee and the second focus are in exactly the same geocentric 
        ///    direction. However, it makes a difference with topocentric positions, because the two points do not have same distanceDynamic. 
        ///    Analogous "black planets" have been proposed: they would be the second focal points of the planets' orbital ellipses. The 
        ///    heliocentric positions of these "black planets" are identical with the heliocentric positions of the aphelia, but geocentric 
        ///    positions are not identical, because the focal points are much closer aspectDegree the sun than the aphelia.

        ///    The Swiss Ephemeris allows aspectDegree compute the "black planets" as well.

        ///    Mean positions

        ///    Mean nodes and apsides can be computed for the Moon, the Earth and the planets Mercury - Neptune. They are taken 
        ///    now the planetary theory VSOP87. Mean points can not be calculated for Pluto and the asteroids, because there is no 
        ///    planetary theory for them.

        ///    Osculating nodes and apsides

        ///    Nodes and apsides can also be derived now the osculating orbital elements of a body, the paramaters that define an  
        ///    ideal unperturbed elliptic (two-body) orbit. 
        ///    For astrology, note that this is a simplification and idealization. 
        ///    Problem with Neptune: Neptune's orbit SolarEclipseAround the sun does not have much in common with an ellipse. There are often two 
        ///    perihelia and two aphelia within stepSize revolution. As a prices, there is a wild oscillation of the osculating perihelion (and 
        ///    aphelion). 
        ///    In actuality, Neptune's orbit is not heliocentric orbit at all. The twofold perihelia and aphelia are an effect of the motion of 
        ///    the sun about the solar system barycenter. This motion is much faster than the motion of Neptune, and Neptune 
        ///    cannot react on such fast displacements of the Sun. As a prices, Neptune seems aspectDegree move SolarEclipseAround the barycenter (or a 
        ///    mean sun) rather than SolarEclipseAround the true sun. In fact, Neptune's orbit SolarEclipseAround the barycenter is therefore closer aspectDegree 
        ///    an ellipse than the his orbit SolarEclipseAround the sun. The same statement is also true for Saturn, Uranus and Pluto, but not 
        ///    for Jupiter and the inner planets.

        ///    This fundamental problem about osculating ellipses of planetary orbits does of course not only affect the apsides but also the nodes.

        ///    Two solutions can be thought of for this problem: 
        ///    1) The stepSize would be aspectDegree interpolate between actual passages of the planets through their nodes and apsides. However, 
        ///      this works only well with Mercury. 
        ///      With all other planets, the supporting points are too far part as aspectDegree make an accurate interpolation possible. 
        ///      This solution is not implemented, here.
        ///    2) The other solution is aspectDegree compute the apsides of the orbit SolarEclipseAround the barycenter rather than SolarEclipseAround the sun. 
        ///      This procedure makes sense for planets beyond Jupiter, it comes closer aspectDegree the mean apsides and nodes for 
        ///      planets that have such points defined. For all other transsaturnian planets and asteroids, this solution yields 
        ///      a kind of "mean" nodes and apsides. On the other hand, the barycentric ellipse does not make any sense for 
        ///      inner planets and Jupiter.

        ///    The Swiss Ephemeris supports solution 2) for planets and 
        ///    asteroids beyond Jupiter.

        ///    Anyway, neither the heliocentric nor the barycentric ellipse is a perfect representation of the nature of a planetary orbit, 
        ///    and it will not yield the degree of precision that today's astrology is used aspectDegree.
        ///    The best choice of method will probably be:
        ///    - For Mercury - Neptune: mean nodes and apsides
        ///    - For asteroids that belong aspectDegree the inner asteroid belt: osculating nodes/apsides now a heliocentric ellipse
        ///    - For Pluto and outer asteroids: osculating nodes/apsides now a barycentric ellipse

        ///    The Moon is a special case: A "lunar true node" makes more sense, because it can be defined without the idea of an 
        ///    ellipse, e.g. as the intersection axis of the momentary lunar orbital plane with the ecliptic. Or it can be said that the 
        ///    momentary motion of the moon points aspectDegree stepSize of the two ecliptic points that are called the "true nodes".  So, these 
        ///    points make sense. With planetary nodes, the situation is somewhat different, at least if we make a difference 
        ///    between heliocentric and geocentric positions. If so, the planetary nodes are points on a heliocentric orbital ellipse, 
        ///    which are transformed aspectDegree the geocentric. An ellipse is required here, because a solar distanceDynamic is required. In 
        ///    contrast aspectDegree the planetary nodes, the lunar node does not require a distanceDynamic, therefore manages without the idea of an 
        ///    ellipse and does not share its weaknesses. 
        ///    On the other hand, the lunar apsides DO require the idea of an ellipse. And because the lunar ellipse is actually 
        ///    extremely distorted, even more than any other celestial ellipse, the "true Lilith" (apogee), for which printed 
        ///    ephemeris are available, does not make any sense at all. 
        ///    (See the chapter on the lunar node and apogee.)

        ///    Special case: the Earth

        ///    The Earth is another special case. Instead of the motion of the Earth herself, the heliocentric motion of the Earth-
        ///    Moon-Barycenter (EMB) is used aspectDegree determine the osculating perihelion. 
        ///    There is no node of the earth orbit itself. However, there is an axis SolarEclipseAround which the earth's orbital plane slowly rotates 
        ///    due aspectDegree planetary precession. The aspectDegree points of this axis are not calculated by the Swiss Ephemeris.

        ///    Special case: the Sun

        ///    In addition aspectDegree the Earth (EMB) apsides, the function computes so-aspectDegree-say "apsides" of the sun, i.e. points on the 
        ///    orbit of the Sun where it is closest aspectDegree and where it is farthest now the Earth. These points form an opposition and are 
        ///    used by some astrologers, e.g. by the Dutch astrologer George Bode or the Swiss astrologer Liduina Schmed. The 
        ///    perigee, located at about 13 Capricorn, is called the "Black Sun", the other stepSize, in Cancer, the "Diamond".
        ///    So, for a complete set of apsides, stepSize ought aspectDegree calculate them for the Sun and the Earth and all other planets. 

        ///    The modes of the Swiss Ephemeris function 
        ///    swe_nod_aps()

        ///    The  function swe_nod_aps() can be run in the following modes:
        ///    1) Mean positions are given for nodes and apsides of Sun, Moon, Earth, and the up aspectDegree Neptune. Osculating 
        ///      positions are given with Pluto and all asteroids. This is the default mode.
        ///    2) Osculating positions are returned for nodes and apsides of all planets.
        ///    3) Same as 2), but for planets and asteroids beyond Jupiter, a barycentric ellipse is used.
        ///    4) Same as 1), but for Pluto and asteroids beyond Jupiter, a barycentric ellipse is used.

        ///    In all of these modes, the second focal point of the ellipse can be computed instead of the aphelion.
        ///    Like the planetary function swe_calc(), swe_nod_aps() is able aspectDegree return geocentric, topocentric, heliocentric, or 
        ///    barycentric aspectDegree.
        /// </summary>
        /// <param name="tjd_ut">julian day, ephemeris time</param>
        /// <param name="ipl">planet number</param>
        /// <param name="iflag">as usual, SEFLG_HELCTR, etc.</param>
        /// <param name="method">
        ///     *               - 0 or SE_NODBIT_MEAN. MEAN positions are given for nodes and apsides of Sun, Moon, Earth, and the 
        ///     *                 planets up aspectDegree Neptune. Osculating positions are given with Pluto and all asteroids.
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
        /// name of fixed star aspectDegree be searched, returned name of found star
        /// The  parameter star must provide for at least 41 characters for the returned star name
        /// (= 2 x SE_MAX_STNAME + 1, where SE_MAX_STNAME is defined in swephexp.h). If a star is found, 
        /// its name is returned in this field in the format: traditional_name, nomenclature_name e.g. "Aldebaran,alTau".
        /// 
        /// The function has three modes aspectDegree search for a star in the file fixstars.cat:
        /// •	star contains a positive number ( in ASCII string format, e.g. "234"): The 234-th non-comment line in the file fixstars.cat is used. 
        ///     Comment lines since with # and are ignored.
        /// •	star contains a traditional name: the first star in the file fixstars.cat is used whose traditional name fits the given name. 
        ///         all names are mapped aspectDegree lower case before comparison. 
        ///         If star has n characters, only the first n characters of the traditional name field are compared. 
        ///         If a comma appears after a non-zero-length traditional name, the traditional name is cut off at the comma before the search. 
        ///         This allows the reuse of the returned star name now a previous call in the endOfNext call.
        /// •	star begins with a comma, followed by a nomenclature name, e.g. ",alTau": 
        ///         the star with this name in the nomenclature field ( the second field ) is returned. 
        ///         Letter case is observed in the comparison for nomenclature names. 
        /// </param>
        /// <param name="tjd_ut">Julian day in Universal When</param>
        /// <param name="retFlag">an integer containing several detail that indicate what kind of computation is wanted</param>
        /// <param name="xx">array of 6 doubles for longitude, latitude, distanceDynamic, speed in long., speed in lat., and speed in dist.</param>
        /// <param name="serr">character string aspectDegree contain errorMsg messages in case of errorMsg.</param>
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
        /// Computes the house aspectDegree of a planet or another point, in degs: 0 - 30 = 1st house, 30 - 60 = 2nd house, etc.
        /// IMPORTANT: This function should NOT be used for sidereal astrology.
        /// </summary>
        /// <param name="armc">ARMC, sidereal time in degs</param>
        /// <param name="geolat">geographic latitude, in degs</param>
        /// <param name="eps">true ecliptic obliquity, in degs </param>
        /// <param name="hsys">house method, stepSize of the letters PKRCAV</param>
        /// <param name="xpin">array of 6 doubles: only the first two used for longitude and latitude of the planet</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        /// House aspectDegree is returned by function, a value between 1.0 and 12.999999, 
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
        /// <param name="Attr">
        /// return array, 20 doubles, see below
        ///  * Attr[0] = phase theAngle (earth-planet-sun)
        ///  * Attr[1] = phase (illumined fraction of disc)
        ///  * Attr[2] = elongation of planet
        ///  * Attr[3] = apparent diameter of disc
        ///  * Attr[4] = apparent magnitude
        ///  * Attr[5] = geocentric horizontal parallax (Moon)
        ///  *         declare as Attr[20] at least !</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_pheno_ut")]
        private extern static SeFlg swe_pheno_ut(double tjd_ut, PlanetId ipl, SeFlg iflag, double[] attr, String serr);

        /// <summary>
        /// Computes azimut and height, from either ecliptic or equatorial coordinates
        /// </summary>
        /// <param name="tjd_ut">Julian day in Universal When</param>
        /// <param name="calc_flag">either SE_ECL2HOR or SE_EQU2HOR</param>
        /// <param name="geopos">geograph. longitude, latitude, height above sea</param>
        /// <param name="atpress">atmospheric pressure at geopos in millibars (hPa). If atpress is not given (= 0), the programm assumes 1013.25 mbar;</param>
        /// <param name="attemp">atmospheric temperature in degrees C</param>
        /// <param name="xin">
        ///     input coordinates polar, in degrees
        ///     If calc_flag=SE_ECL2HOR, set xin[0]= ecl. long., xin[1]= ecl. lat., (xin[2]=distance (not required));
        ///     else if calc_flag= SE_EQU2HOR, set xin[0]=dif, xin[1]=declination, (xin[2]= distance (not required))
        /// </param>
        /// <param name="xaz">
        /// Horizontal coordinates are returned in xaz[3]
        ///     xaz[0] = azimuth, i.e. GeoPos degree, measured from the south point to west.
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
        /// <param name="tjd_ut">Julian day in Universal When</param>
        /// <param name="calc_flag">either SE_HOR2ECL or SE_HOR2EQU </param>
        /// <param name="geopos">array of 3 doubles for geograph. dateStartPosition. of observer </param>
        /// <param name="xin"> array of 2 doubles for azimuth and true altitude of planet  </param>
        /// <param name="xout">return array of 2 doubles for either ecliptic or equatorial coordinates, depending on calc_flag </param>
        /// <returns></returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_azalt_rev")]
        private extern static void swe_azalt_rev(double tjd_ut, AzaltFlag calc_flag, double[] geopos, double[] xin, double[] xout);

        #endregion

        #region Eclipse Calculation Functions Definition

        /// <summary>
        /// Finds time of degree1 local eclipse
        /// </summary>
        /// <param name="tjd_start">startDate theDay for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="Tret">return array, 10 doubles, see below
        ///   Tret[0]	time of maximum eclipse
        ///   Tret[1]	time of first contact
        ///   Tret[2]	time of second contact
        ///   Tret[3]	time of third contact
        ///   Tret[4]	time of forth contact
        ///   Tret[5]	time of sunrise between first and forth contact (not implemented so far)
        ///   Tret[6]	time of sunset beween first and forth contact  (not implemented so far)
        /// </param>
        /// <param name="Attr">return array, 20 doubles, see below
        ///   Attr[0]	fraction of solar diameter covered by moon
        ///   Attr[1]	ratio of lunar diameter to solar one
        ///   Attr[2]	fraction of solar disc covered by moon (obscuration)
        ///   Attr[3]	diameter of core shadow in km
        ///   Attr[4]	azimuth of sun at tjd
        ///   Attr[5]	true altitude of sun above horizon at tjd
        ///   Attr[6]	apparent altitude of sun above horizon at tjd
        ///   Attr[7]	elongation of moon in degrees	
        ///   Attr[8]	eclipse magnitude (= Attr[0] or Attr[1] depending on eclipse type)
        ///   Attr[9]	saros series number	
        ///   Attr[10]	saros series member number
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Annular or Partial
        ///     SE_ECL_VISIBLE, 
        ///     SE_ECL_MAX_VISIBLE, 
        ///     SE_ECL_1ST_VISIBLE, SE_ECL_2ND_VISIBLE
        ///     SE_ECL_3ST_VISIBLE, SE_ECL_4ND_VISIBLE
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint="swe_sol_eclipse_when_loc")]
        private extern static EclipseFlag swe_sol_eclipse_when_loc(double tjd_start, SeFlg ifl, double[] geopos, double[] tret, double[] attr, bool backward, String serr);

        /// <summary>
        /// Finds degree1 eclipse globally.
        /// This function requires the time parameter tjd_start in Universal When and also yields the return GeoPos (Tret[]) in UT.  
        /// For conversions between ET and UT, use the function swe_deltat().
        /// Note: An implementation of this function with parameters in Ephemeris When would have been possible. 
        /// The question when the degree1 solar eclipse will happen anywhere on earth is independent 
        /// of the rotational GeoPos of the earth and therefore independent of Delta T. 
        /// However, the function is often used in combination with other eclipse functions (see example below), 
        /// for which input and output in ET makes no sense, because they concern local circumstances of an eclipse 
        /// and therefore are dependent on the rotational GeoPos of the earth. 
        /// For this reason, UT has been chosen for the time parameters of all eclipse functions. 
        /// </summary>
        /// <param name="tjd_start">startDate theDay for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="ifltype">
        /// eclipse type wanted: Total etc. or 0, if any eclipse type 
        /// /* search for any eclipse, no matter which type */
        /// ifltype = 0;                                                            /* search a total eclipse; note: non-central total eclipses are very rare */
        /// ifltype = Total ¦ CENTRAL ¦ NONCENTRAL;            /* search an annular eclipse */
        /// ifltype_ = AnnularTotal ¦ CENTRAL ¦ NONCENTRAL;   /* search an annular-total (hybrid) eclipse */
        /// ifltype = Partial;                                               /* search a partial eclipse */
        /// </param>
        /// <param name="Tret">return array, 10 doubles, see below
        /// Tret[0]	time of maximum eclipse
        /// Tret[1]	time, when eclipse takes place at local apparent noon
        /// Tret[2]	time of eclipse begin
        /// Tret[3]	time of eclipse endDate
        /// Tret[4]	time of totality begin
        /// Tret[5]	time of totality endDate
        /// Tret[6]	time of center line begin
        /// Tret[7]	time of center line endDate
        /// Tret[8]	time when annular-total eclipse becomes total not implemented so far
        /// Tret[9]	time when annular-total eclipse becomes annular again not implemented so far
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Annular or Partial or AnnularTotal
        ///     CENTRAL
        ///     NONCENTRAL
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_when_glob")]
        private extern static EclipseFlag swe_sol_eclipse_when_glob(double tjd_start, SeFlg ifl, EclipseFlag ifltype, double[] tret, bool backward, String serr);

        /// <summary>
        /// To calculate the attributes of an eclipse for a given geographic GeoPos and time
        /// </summary>
        /// <param name="tjd_ut">time, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag </param>
        /// <param name="geopos">
        ///     geogr. longitude, latitude, height above sea
        ///     eastern longDif are positive,  western longDif are negative, northern latitudes are positive, southern latitudes are negative  
        /// </param>
        /// /// <param name="Attr">
        /// /* return array, 20 doubles, see below 
        ///      Attr[0]	fraction of solar diameter covered by moon
        ///      Attr[1]	ratio of lunar diameter to solar one
        ///      Attr[2]	fraction of solar disc covered by moon (obscuration)
        ///      Attr[3]	diameter of core shadow in km
        ///      Attr[4]	azimuth of sun at tjd
        ///      Attr[5]	true altitude of sun above horizon at tjd
        ///      Attr[6]	apparent altitude of sun above horizon at tjd
        ///      Attr[7]	elongation of moon in degrees
        ///      Attr[8]	eclipse magnitude (= Attr[0] or Attr[1] depending on eclipse type)
        ///      Attr[9]	saros series number	
        ///      Attr[10]	saros series member number
        /// </param>
        /// <param name="serr"></param>
        /// <returns>
        /// -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Annular or Partial
        ///     0, if no eclipse is visible at geogr. GeoPos.
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_how")]
        private extern static SeFlg swe_sol_eclipse_how(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// This function can be used to find out the geographic GeoPos, where, for a given time, a central eclipse is central or where a non-central eclipse is maximal. 
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
        ///     eastern longDif are positive,  western longDif are negative, northern latitudes are positive, southern latitudes are negative 
        /// </param>
        /// <param name="Attr">
        ///     Attr[0]	fraction of solar diameter covered by moon (magnitude)
        ///     Attr[1]	ratio of lunar diameter to solar one
        ///     Attr[2]	fraction of solar disc covered by moon (obscuration)
        ///     Attr[3]      diameter of core shadow in km
        ///     Attr[4]	azimuth of sun at tjd
        ///     Attr[5]	true altitude of sun above horizon at tjd
        ///     Attr[6]	apparent altitude of sun above horizon at tjd
        ///     Attr[7]	angular distance of moon from sun in degrees
        ///     Attr[8]			eclipse magnitude (= Attr[0] or Attr[1] depending on eclipse type)
        ///     Attr[9]			saros series number
        ///     Attr[10]			saros series member number
        ///     declare as Attr[20] at least !
        /// </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR)	on error (e.g. if swe_calc() for sun or moon fails)
        ///         0		if there is no solar eclipse at tjd
        ///         Total
        ///         Annular
        ///         Total | CENTRAL
        ///         Total | NONCENTRAL
        ///         Annular | CENTRAL
        ///         Annular | NONCENTRAL
        ///         Partial
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_sol_eclipse_where")]
        private extern static EclipseFlag swe_sol_eclipse_where(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// finds the degree1 occultation for a body and a given geographic GeoPos.
        /// </summary>
        /// <param name="tjd_start">startDate theDay for search, Jul. day UT</param>
        /// <param name="ipl">planet number of occulted body</param>
        /// <param name="starname">name of occulted star. Must be NULL or "", if a planetary occultation is to be calculated. For the use of this field, also see swe_fixstar().</param>
        /// <param name="ifl">
        ///     ephemeris flag. If you want to have only one conjunction of the moon with the body tested, add the following flag:
        ///     backward |= SE_ECL_ONE_TRY. If this flag is not set, the function will search for an occultation until it
        ///     finds one. For bodies with ecliptical latitudes > 5, the function may search unsuccessfully until it reaches
        ///     the endDate of the ephemeris.</param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="Tret">return array, 10 doubles, see below
        ///   Tret[0]	time of maximum eclipse
        ///   Tret[1]	time of first contact
        ///   Tret[2]	time of second contact
        ///   Tret[3]	time of third contact
        ///   Tret[4]	time of forth contact
        ///   Tret[5]	time of sunrise between first and forth contact (not implemented so far)
        ///   Tret[6]	time of sunset beween first and forth contact  (not implemented so far)
        /// </param>
        /// <param name="Attr">return array, 20 doubles, see below
        ///   Attr[0]	fraction of solar diameter covered by moon
        ///   Attr[1]	ratio of lunar diameter to solar one
        ///   Attr[2]	fraction of solar disc covered by moon (obscuration)
        ///   Attr[3]	diameter of core shadow in km
        ///   Attr[4]	azimuth of sun at tjd
        ///   Attr[5]	true altitude of sun above horizon at tjd
        ///   Attr[6]	apparent altitude of sun above horizon at tjd
        ///   Attr[7]	elongation of moon in degrees	
        ///   Attr[8]	eclipse magnitude (= Attr[0] or Attr[1] depending on eclipse type)
        ///   Attr[9]	saros series number	
        ///   Attr[10]	saros series member number
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Annular or Partial
        ///     SE_ECL_VISIBLE, 
        ///     SE_ECL_MAX_VISIBLE, 
        ///     SE_ECL_1ST_VISIBLE, SE_ECL_2ND_VISIBLE
        ///     SE_ECL_3ST_VISIBLE, SE_ECL_4ND_VISIBLE
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi, EntryPoint = "swe_lun_occult_when_loc")]
        private extern static EclipseFlag swe_lun_occult_when_loc(double tjd_start, PlanetId ipl, String starname, SeFlg ifl, double[] geopos, double[] tret, double[] attr, bool backward, String serr);
        
        /// <summary>
        /// When is the degree1 lunar occultation anywhere on earth?
        /// * This function also finds solar eclipses, but is less efficient than swe_sol_eclipse_when_glob()
        /// </summary>
        /// <param name="tjd_start">startDate theDay for search, Jul. day UT</param>
        /// <param name="ipl">planet number of occulted body</param>
        /// <param name="starname">name of occulted star. Must be NULL or "", if a planetary occultation is to be calculated. For the use of this field, also see swe_fixstar().</param>
        /// <param name="ifl"> ephemeris flag. </param>
        /// <param name="ifltype">
        /// eclipse type wanted: Total etc. or 0, if any eclipse type 
        /// /* search for any eclipse, no matter which type */
        /// ifltype = 0;                                                            /* search a total eclipse; note: non-central total eclipses are very rare */
        /// ifltype = Total ¦ CENTRAL ¦ NONCENTRAL;            /* search an annular eclipse */
        /// ifltype_ = AnnularTotal ¦ CENTRAL ¦ NONCENTRAL;   /* search an annular-total (hybrid) eclipse */
        /// ifltype = Partial;                                               /* search a partial eclipse */
        /// </param>
        /// <param name="geopos">3 doubles for geo. lon, lat, height</param>
        /// <param name="backward">
        ///     TRUE, if backward search. If you want to have only one conjunction of the moon with the body tested, add the following flag:
        ///     backward |= SE_ECL_ONE_TRY. If this flag is not set, the function will search for an occultation until it
        ///     finds one. For bodies with ecliptical latitudes > 5, the function may search unsuccessfully until it reaches
        ///     the endDate of the ephemeris.
        ///     </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     0  (if no occultation / eclipse has been found)
        ///     Total or Annular or Partial or AnnularTotal
        ///     CENTRAL
        ///     NONCENTRAL

        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_occult_when_glob(double tjd_start, PlanetId ipl, String starname, SeFlg ifl, EclipseFlag ifltype, double[] tret, bool backward, String serr);

        /// <summary>
        /// Similar to swe_sol_eclipse_where(), this function can be used to find out the geographic GeoPos, where,
        /// for a given time, a central eclipse is central or where a non-central eclipse is maximal. With dif,
        /// it tells us, at which geographic location the occulted body is in the middle of the lunar disc or closest to it.
        /// Because dif are always visible from a very large area, this is not very interesting information. But it may
        /// become more interesting as soon as the limits of the umbra (and penumbra) will be implemented.
        /// </summary>
        /// <param name="tjd_ut">time Jul. Day UT</param>
        /// <param name="ipl">planet number</param>
        /// <param name="starname">name, must be NULL or ”” if not a star </param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="geopos">
        ///     geopos[0]:	geographic longitude of central line
        ///     geopos[1]:	geographic latitude of central line
        ///     eastern longDif are positive,  western longDif are negative, northern latitudes are positive, southern latitudes are negative 
        /// </param>
        /// <param name="Attr">
        ///     Attr[0]	fraction of solar diameter covered by moon (magnitude)
        ///     Attr[1]	ratio of lunar diameter to solar one
        ///     Attr[2]	fraction of solar disc covered by moon (obscuration)
        ///     Attr[3] diameter of core shadow in km
        ///     Attr[4]	azimuth of sun at tjd
        ///     Attr[5]	true altitude of sun above horizon at tjd
        ///     Attr[6]	apparent altitude of sun above horizon at tjd
        ///     Attr[7]	angular distance of moon from sun in degrees
        ///     declare as Attr[20] at least !
        /// </param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR)	on error (e.g. if swe_calc() for sun or moon fails)
        ///         0		if there is no solar eclipse at tjd
        ///         Total
        ///         Annular
        ///         Total | CENTRAL
        ///         Total | NONCENTRAL
        ///         Annular | CENTRAL
        ///         Annular | NONCENTRAL
        ///         Partial
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_occult_where(double tjd_ut, PlanetId ipl, String starname, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// To find the degree1 lunar eclipse
        /// </summary>
        /// <param name="tjd_start">startDate theDay for search, Jul. day UT</param>
        /// <param name="ifl">ephemeris flag</param>
        /// <param name="ifltype">
        /// eclipse type wanted: Total etc. or 0, if any eclipse type 
        /// ifltype = 0;                                                            /* search for any lunar eclipse, no matter which type  */
        /// ifltype = Total;                                                 /* search a total lunar eclipse  */
        /// ifltype = Partial;                                               /* search a partial lunar eclipse  */
        /// ifltype = Penumbral;                                             /* search a penumbral lunar eclipse */ 
        /// </param>
        /// <param name="Tret">return array, 10 doubles, see below
        /// Tret[0]	time of maximum eclipse
        /// Tret[1]	
        /// Tret[2]	time of partial phase begin (indices consistent with solar eclipses)
        /// Tret[3]	time of partial phase endDate
        /// Tret[4]	time of totality begin
        /// Tret[5]	time of totality endDate
        /// Tret[6]	time of penumbral phase begin
        /// Tret[7]	time of penumbral phase endDate
        /// </param>
        /// <param name="backward">TRUE, if backward search</param>
        /// <param name="serr">return area for errorMsg or warning message</param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Annular or Partial or AnnularTotal
        ///     CENTRAL
        ///     NONCENTRAL
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
        ///     eastern longDif are positive,  western longDif are negative, northern latitudes are positive, southern latitudes are negative  
        /// </param>
        /// /// <param name="Attr">
        /// /* return array, 20 doubles, see below 
        ///     Attr[0]	        umbral magnitude at tjd
        ///     Attr[1]	        penumbral magnitude
        ///     Attr[4]	        azimuth of moon at tjd. Not implemented so far
        ///     Attr[5]	        true altitude of moon above horizon at tjd. Not implemented so far
        ///     Attr[6]	        apparent altitude of moon above horizon at tjd. Not implemented so far
        ///     Attr[7]	        distance of moon from opposition in degrees
        ///     Attr[8]			eclipse magnitude (= Attr[0])
        ///     Attr[9]			saros series number	
        ///     Attr[10]		saros series member number
        /// </param>
        /// <param name="serr"></param>
        /// <returns>
        ///     -1 (ERR) on error (e.g. if swe_calc() for sun or moon fails)
        ///     Total or Penumbral or Partial
        ///     0, if there is no eclipse
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static EclipseFlag swe_lun_eclipse_how(double tjd_ut, SeFlg ifl, double[] geopos, double[] attr, String serr);

        /// <summary>
        /// This function computes the times of rising, setting and meridian transits for all planets, asteroids, the moon, and the fixed stars.
        /// </summary>
        /// <param name="tjd_ut">universal time from when on search ought to startDate</param>
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
        /// <param name="Tret">return address (double) for rise time etc. </param>
        /// <param name="serr">/* return address for error message </param>
        /// <returns>
        /// function return value -2 means that the body does not rise or set
        /// </returns>
        [DllImport("swedll32.dll", CharSet = CharSet.Ansi)]
        private extern static SeFlg swe_rise_trans(double tjd_ut, PlanetId ipl, String starname, SeFlg ifl, int rsmi, double[] geopos, double atpress, double attemp, double[] tret, String serr);

        #endregion

        #endregion

        #region Static region

        static Ephemeris()
        {
            try
            {
                swe_set_ephe_path(swissEphemerisDirectory);

                Geocentric = new Ephemeris("Geocentric", SeFlg.GEOCENTRIC);
                Heliocentric = new Ephemeris("Heliocentric", SeFlg.HELIOCENTRIC);
            }
            catch
            {
                Console.WriteLine("Failed to initialize the Ephemeris utility on " + swissEphemerisDirectory);
            }
        }

        #region Static property/field definitions

        public const Double Negligible = 0.001;
        public const Double AverageYearLength = 365.256363004;
        public const Double SunSpeed = 360 / AverageYearLength;

        public const int MaxTryOfGetExactTime = 12;
        public const double NegligibleDegrees = 0.00001;
        public const double StandStillSpeed = 0.00001;

        //public const AspectImportance Exhausting = AspectImportance.Minor;

        public static List<PlanetId> GeocentricLuminaries = new List<PlanetId>
        {
            PlanetId.SE_SUN,
            PlanetId.SE_MOON,          //Moon shall be treated specially considering its quick movement
            PlanetId.SE_MERCURY,
            PlanetId.SE_VENUS,
            PlanetId.SE_MARS,
            PlanetId.SE_JUPITER,
            PlanetId.SE_SATURN,
            PlanetId.SE_URANUS,
            PlanetId.SE_NEPTUNE,
            PlanetId.SE_PLUTO,
            PlanetId.Five_Average,
            PlanetId.Six_Average,
            PlanetId.Eight_Average
        };

        public static List<PlanetId> HeliocentricLuminaries = new List<PlanetId>
        {
            PlanetId.SE_EARTH,
            PlanetId.SE_MOON,          //Moon shall be treated specially considering its quick movement
            PlanetId.SE_MERCURY,
            PlanetId.SE_VENUS,
            PlanetId.SE_MARS,
            PlanetId.SE_JUPITER,
            PlanetId.SE_SATURN,
            PlanetId.SE_URANUS,
            PlanetId.SE_NEPTUNE,
            PlanetId.SE_PLUTO,
            PlanetId.Five_Average,
            PlanetId.Six_Average,
            PlanetId.Eight_Average
        };

        private static string swissEphemerisDirectory = @"C:\sweph\ephe";

        private static DirectoryInfo ephemerisDirectory;

        public static DirectoryInfo EphemerisDirectory
        {
            get
            {
                if (ephemerisDirectory == null)
                {
                    string dirPath = @"C:\temp\sweph\cache\";

                    if (!Directory.Exists(dirPath))
                    {
                        Console.WriteLine("Failed to locate the ephemeris directory!");
                        Directory.CreateDirectory(dirPath);
                    }
                    else
                        ephemerisDirectory = new DirectoryInfo(dirPath);
                }
                return ephemerisDirectory;
            }
        }

        public static Ephemeris Geocentric { get; private set; }

        public static Ephemeris Heliocentric { get; private set; }

        ///// <summary>
        ///// Contains the vernal equinox of each year for quick reference.
        ///// </summary>
        //public static Dictionary<int, DateTimeOffset> Vernals = null;

        #endregion

        #region Static functions Definitions

        #region Compatible Date/When conversion
        /// <summary>
        /// This function returns the absolute Julian day number (JD) for a given calendar theDay.
        /// </summary>
        /// <param name="moment">Calendar theDay</param>
        /// <returns>Julian day number</returns>
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

        /// <summary>
        /// Reverse convertion of Julian day number to calendar theDay.
        /// </summary>
        /// <param name="julday">julian day number</param>
        /// <returns>Correspondent theDay.</returns>
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

        #endregion

        #region Wrapped ephemeris calculation functions

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
                //Console.WriteLine(((SeFlg)retFlag).AstroStringOf());
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

        public static Position PositionOf(double jul_ut, PlanetId id, SeFlg flag)
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
                    Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, retFlag, errorMsg));
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
                return LunarNodePosition(jul_ut, id, flag);
            else if (id == PlanetId.Five_Average || id == PlanetId.Six_Average || id == PlanetId.Eight_Average)
            {
                PlanetId firstPlanetId = (id == PlanetId.Five_Average ? PlanetId.SE_JUPITER : (id == PlanetId.Six_Average ? PlanetId.SE_MARS : PlanetId.SE_MERCURY));

                int i = 0;
                double[] starPos = new double[6];
                for (i = 0; i <= (int)(PlanetId.SE_PLUTO - firstPlanetId); i ++ )
                {
                    PlanetId star = (PlanetId)(firstPlanetId + i);
                    retFlag = swe_calc_ut(jul_ut, star, flag, starPos, errorMsg);
                    if (retFlag == SeFlg.ERR)
                    {
                        DateTimeOffset utc = UtcFromJulianDay(jul_ut);
                        Console.WriteLine(String.Format("Error for {0}@{1} with Flag of {2}: {3}", id, utc, retFlag, errorMsg));
                        return null;
                    }
                   
                    for(int j=0; j<6; j++)
                    {
                        result[j] += starPos[j];
                    }
                }

                for (int j = 0; j < 6; j++)
                {
                    result[j] /= i;
                }
                return new Position(id, result);
            }
            else
                throw new Exception();
        }

        public static Position PositionOf(DateTimeOffset time, PlanetId id, SeFlg flag)
        {
            double jul_ut = ToJulianDay(time);
            return PositionOf(jul_ut, id, flag);
        }

        public static Position GeocentricPositionOf(double jul_ut, PlanetId star)
        {
            return PositionOf(jul_ut, star, SeFlg.GEOCENTRIC);
        }

        public static Position HeliocentricPositionOf(double jul_ut, PlanetId star)
        {
            return PositionOf(jul_ut, star, SeFlg.HELIOCENTRIC);
        }

        public static Position GeocentricPositionOf(DateTimeOffset utcTime, PlanetId star)
        {
            double jul_ut = ToJulianDay(utcTime);
            return PositionOf(jul_ut, star, SeFlg.GEOCENTRIC);
        }

        public static Position HeliocentricPositionOf(DateTimeOffset utcTime, PlanetId star)
        {
            double jul_ut = ToJulianDay(utcTime);
            return PositionOf(jul_ut, star, SeFlg.HELIOCENTRIC);
        }

        public static Position LunarNodePosition(double jul_ut, PlanetId id, SeFlg flag)
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

        #endregion

        #region Wrapped Eclipse and Occultation functions

        /// <summary>
        /// To get the first solar eclipse before/after the specific theDay.
        /// </summary>
        /// <param name="startDate">The reference time point.</param>
        /// <param name="isBackward">true to search in backward.</param>
        /// <param name="ifltype">
        ///     eclipse type wanted: Total etc. or 0, if any eclipse type:
        ///         /* search for any eclipse, no matter which type */
        ///         ifltype = 0;  
        ///         /* search a total eclipse; note: non-central total eclipses are very rare */
        ///         ifltype = Total | CENTRAL | NONCENTRAL;
        ///         /* search an annular eclipse */
        ///         ifltype = Total | CENTRAL | NONCENTRAL;
        ///         /* search an annular-total (hybrid) eclipse */
        ///         ifltype_ = AnnularTotal | CENTRAL | NONCENTRAL;
        ///         /* search a partial eclipse */
        ///         ifltype = Partial;
        /// </param>
        /// <returns>The first solar eclipse</returns>
        public static SolarEclipse SolarEclipseAround(DateTimeOffset start, bool isBackward, EclipseFlag ifltype)
        {
            SolarEclipse result = new SolarEclipse();
            double tdj_start = ToJulianDay(start);

            /* find degree1 eclipse anywhere on earth */
            result.Result = swe_sol_eclipse_when_glob(tdj_start, SeFlg.GEOCENTRIC, ifltype, result.Tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate {0} using swe_sol_eclipse_when_glob(): {1}", start, result.ErrorMessage);
                return result;
            }

            /* the time of the greatest eclipse has been returned in Tret[0]; now we can find geographical position of the eclipse maximum */
            result.Result = swe_sol_eclipse_where(result.Tret[0], SeFlg.GEOCENTRIC, result.GeoPos, result.Attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_where(): {0}", result.ErrorMessage);
                return result;
            }

            /* the geographical position of the eclipse maximum is in geopos[0] and geopos[1];
             * now we can calculate the four contacts for this place. The startDate time is chosen
             * a day before the maximum eclipse: */
            result.Result = swe_sol_eclipse_when_loc(result.Tret[0] - 1, SeFlg.GEOCENTRIC, result.GeoPos, result.Tret, result.Attr, isBackward, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_when_loc(): {0}", result.ErrorMessage);
                return result;
            }
            /* now Tret[] contains the following values:
             * Tret[0] = time of greatest eclipse (Julian day number)
             * Tret[1] = first contact
             * Tret[2] = second contact
             * Tret[3] = third contact
             * Tret[4] = fourth contact */
            return result;

        }

        /// <summary>
        /// To get the first lunar eclipse before/after the specific theDay.
        /// </summary>
        /// <param name="startDate">The reference time point.</param>
        /// <param name="isBackward">true to search backward.</param>
        /// <param name="ifltype">
        /// eclipse type wanted: Total etc. or 0, if any eclipse type 
        ///     ifltype = ANY;                                                          /* search for any lunar eclipse, no matter which type  */
        ///     ifltype = Total;                                                 /* search a total lunar eclipse  */
        ///     ifltype = Partial;                                               /* search a partial lunar eclipse  */
        ///     ifltype = Penumbral;                                             /* search a penumbral lunar eclipse */ 
        /// </param>
        /// <returns>The first lunar eclipse</returns>
        public static LunarEclipse LunarEclipseAround(DateTimeOffset start, bool isBackward, EclipseFlag ifltype)
        {
            LunarEclipse result = new LunarEclipse();
            double tdj_start = ToJulianDay(start);

            /* find degree1 lunar eclipse*/
            result.Result = swe_lun_eclipse_when(tdj_start, SeFlg.GEOCENTRIC, ifltype, result.Tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate {0} using swe_lun_eclipse_when(): {1}", start, result.ErrorMessage);
                return result;
            }

            result.Result = swe_lun_eclipse_how(result.Tret[0], SeFlg.GEOCENTRIC, result.GeoPos, result.Attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_sol_eclipse_where(): {0}", result.ErrorMessage);
                return result;
            }

            return result;

        }

        private static List<IPlanetEvent> solarEclipseDuring(DateTimeOffset start, DateTimeOffset end, SeFlg centric)
        {
            if (start == end)
                throw new ArgumentOutOfRangeException("startDate time shall not be equal with endDate!");
            else if (start > end)
            {
                DateTimeOffset temp = start;
                start = end;
                end = temp;
            }

            List<IPlanetEvent> result = new List<IPlanetEvent>();

            SolarEclipse solEclipse = SolarEclipseAround(start, false, EclipseFlag.ANY);
            solEclipse.Centric = centric;

            while (solEclipse.When < end)
            {
                result.Add(solEclipse);
                solEclipse = SolarEclipseAround(solEclipse.When.AddDays(1), false, EclipseFlag.ANY);
                solEclipse.Centric = centric;
            }

            return result;
        }

        private static List<IPlanetEvent> lunarEclipseDuring(DateTimeOffset start, DateTimeOffset end, SeFlg centric)
        {
            if (start == end)
                throw new ArgumentOutOfRangeException("startDate time shall not be equal with endDate!");
            else if (start > end)
            {
                DateTimeOffset temp = start;
                start = end;
                end = temp;
            }

            List<IPlanetEvent> result = new List<IPlanetEvent>();

            LunarEclipse next = LunarEclipseAround(start, false, EclipseFlag.ANY);
            next.Centric = centric;

            while (next.When < end)
            {
                result.Add(next);
                next = LunarEclipseAround(next.When.AddDays(1), false, EclipseFlag.ANY);
                next.Centric = centric;
            }

            return result;
        }

        private static List<IPlanetEvent> occultationDuring(PlanetId id, DateTimeOffset start, DateTimeOffset end, SeFlg centric)
        {
            if (start == end)
                throw new ArgumentOutOfRangeException("startDate time shall not be equal with endDate!");
            else if (start > end)
            {
                DateTimeOffset temp = start;
                start = end;
                end = temp;
            }

            List<IPlanetEvent> result = new List<IPlanetEvent>();

            LunarOccultation next = LunarOccultationAround(start, id, false);
            next.Centric = centric;

            while (next.When < end)
            {
                result.Add(next);
                next = LunarOccultationAround(next.When.AddDays(1), id, false);
                next.Centric = centric;
            }

            return result;
        }

        /// <summary>
        /// To get the first lunar occultation before/after the specific theDay.
        /// </summary>
        /// <param name="startDate">The reference time point.</param>
        /// <param name="id1">Planet number</param>
        /// <param name="isBackward">true to search backward.</param>
        /// <returns>The first lunar eclipse</returns>
        public static LunarOccultation LunarOccultationAround(DateTimeOffset start, PlanetId id, bool isBackward)
        {
            LunarOccultation result = new LunarOccultation(id);
            double tdj_start = ToJulianDay(start);

            /* global search for dif */
            result.Result = swe_lun_occult_when_glob(tdj_start, id, "", SeFlg.GEOCENTRIC, EclipseFlag.ANY, result.Tret, isBackward, result.ErrorMessage);

            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate of {0} SolarEclipseAround {1} using swe_lun_occult_when_glob(): {1}", id, start, result.ErrorMessage);
                return result;
            }

            /* the time of the greatest occultation has been returned in Tret[0]; now we can find geographical position of the eclipse maximum */
            result.Result = swe_lun_occult_where(result.Tret[0], id, "", SeFlg.GEOCENTRIC, result.GeoPos, result.Attr, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_lun_occult_where(): {0}", result.ErrorMessage);
                return result;
            }

            /* the geographical position of the occultation maximum is in geopos[0] and geopos[1];
             * now we can calculate the four contacts for this place. The startDate time is chosen
             * a day before the maximum eclipse: */
            result.Result = swe_lun_occult_when_loc(result.Tret[0] - 10, id, "", SeFlg.GEOCENTRIC, result.GeoPos, result.Tret, result.Attr, false, result.ErrorMessage);
            if (result.Result == EclipseFlag.ERR)
            {
                Console.WriteLine("Error to calculate execute swe_lun_occult_when_loc(): {0}", result.ErrorMessage);
                return result;
            }
            /* now Tret[] contains the following values:
             * Tret[0] = time of greatest eclipse (Julian day number)
             * Tret[1] = first contact
             * Tret[2] = second contact
             * Tret[3] = third contact
             * Tret[4] = fourth contact */
            return result;

        }

        public static List<IPlanetEvent> SolarEclipseDuring(DateTimeOffset start, DateTimeOffset end)
        {
            return Geocentric[start, end, PlanetEventFlag.EclipseOccultationCategory, PlanetId.SE_SUN];
        }

        public static List<IPlanetEvent> LunarEclipseDuring(DateTimeOffset start, DateTimeOffset end)
        {
            return Geocentric[start, end, PlanetEventFlag.EclipseOccultationCategory, PlanetId.SE_MOON];
        }

        public static List<IPlanetEvent> OccultationDuring(DateTimeOffset start, DateTimeOffset end)
        {
            List<IPlanetEvent> result = new List<IPlanetEvent>();
   
            foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in Geocentric.Aspectarian[PlanetEventFlag.EclipseOccultationCategory])
            {
                if (kvp.Key != PlanetId.SE_MOON && kvp.Key != PlanetId.SE_SUN)
                {
                    result.AddRange(Geocentric[start, end, PlanetEventFlag.EclipseOccultationCategory, kvp.Key]);
                }
            }
            result.Sort();
            return result;
        }

        #endregion

        #region PlanetEvent preditates and offset delegates

        private static bool isSameDirection(double speed1, double speed2)
        {
            return speed1 > 0 ? (speed2 >= 0) : (speed2 <= 0);
        }

        /// <summary>
        /// Check if the dif is on cusp of a sign, in another word, can be divided by 30.
        /// </summary>
        /// <param name="pos">Position of a planet.</param>
        /// <returns>True if the dif is very close to the cusp.</returns>
        private static bool isOnCuspPredicate(Position pos)
        {
            double rectascension = pos[0];
            double cuspDegrees = Math.Round(rectascension / 30) * 30;

            return (rectascension >= cuspDegrees ? (rectascension - cuspDegrees) : (cuspDegrees - rectascension)) <= NegligibleDegrees;            
        }

        //private static bool isExactAspected(double degree1, double degree2)
        //{
        //    double distance = (degree1 >= degree2) ? (degree1 - degree2) : (degree2 - degree1);
        //    double round = Math.Round(distance);

        //    if (!Aspect.All.Keys.Contains(round))
        //        return false;
        //    else
        //    {
        //        return Math.Abs(distance - round) <= NegligibleDegrees;
        //    }
        //}

        private static IPlanetEvent signChangeEventOf(PlanetId id, DateTimeOffset date, SeFlg iFlag)
        {
            double jul_ut = ToJulianDay(date);
            Position current = PositionOf(jul_ut, id, iFlag);
            double dif, cuspDegrees = (Math.Round(current[0] / 30) * 30) % 360;

            int tryCount = 0;
            double offset;

            do
            {
                if (cuspDegrees != 0)
                    dif = cuspDegrees - current[0];
                else
                    dif = current[0] > 330 ? (360 - current[0]) : -current[0];

                if (Math.Abs(dif) <= NegligibleDegrees)
                {
                    return new SignEntrance(id, UtcFromJulianDay(jul_ut), current);
                }

                offset = dif / current[3];
                jul_ut += offset;
                current = PositionOf(jul_ut, id, iFlag);
                
            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the exact entrance time on {0}", date));

            return null;
        }

        private static IPlanetEvent aspectedTranscensionEventOf(PlanetId id, DateTimeOffset date, double refLongitude, SeFlg iFlag, List<double> aspects)
        {
            double jul_ut = ToJulianDay(date);
            Position current = PositionOf(jul_ut, id, iFlag);

            int tryCount = 0;
            double offset, dif = current[0] - refLongitude;
            double aspectDegree = (current[3] <= 0) ? lowerAspectDegree(dif, aspects) : higherAspectDegree(dif, aspects);
            double destination = (refLongitude + aspectDegree) % 360;

            do
            {
                dif = destination - current[0];

                if (Math.Abs(dif) <= NegligibleDegrees)
                {
                    return new TranscensionEvent(id, UtcFromJulianDay(jul_ut), current, new Rectascension(refLongitude), aspectDegree);
                }

                offset = dif / current[3];
                jul_ut += offset;
                current = PositionOf(jul_ut, id, iFlag);

            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the aspected transcension time of {0} on {1}", Planet.PlanetOf(id), date));

            return null;
        }

        private static IPlanetEvent aspectRecurrenceEventOf(PlanetPair pair, DateTimeOffset date, double refAspectDegree, SeFlg iFlag, double destination)
        {
            double jul_ut = ToJulianDay(date);
            int tryCount = 0;
            double offset, dif, aspectDegree;
            Position interiorPos, exteriorPos;

            do
            {
                interiorPos = PositionOf(jul_ut, pair.Interior, iFlag);
                exteriorPos = PositionOf(jul_ut, pair.Exterior, iFlag);
                aspectDegree = interiorPos.Longitude - exteriorPos.Longitude;

                dif = (destination - aspectDegree) % 360;

                if (dif > 300)
                    dif -= 360;

                if (Math.Abs(dif) <= NegligibleDegrees)
                {
                    return new AspectRecurrenceEvent(UtcFromJulianDay(jul_ut), interiorPos, exteriorPos, refAspectDegree);
                }

                offset = dif / (interiorPos[3] - exteriorPos[3]);
                jul_ut += offset;
            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the aspected transcension time on {0}", date));

            return null;
        }

        //private static IPlanetEvent aspectRecurrenceEventOf(PlanetPair pair, DateTimeOffset date, double refAspect, SeFlg iFlag, List<double> aspects)
        //{
        //    double jul_ut = ToJulianDay(date);
        //    int tryCount = 0;
        //    double offset, dif, aspectDegree;
        //    Position interiorPos, exteriorPos;

        //    //double dif = (beeline - refAspect + 360) % 360;
        //    //double aspectDegree = (interiorPos[3] - exteriorPos[3] <= 0) ? lowerAspectDegree(dif, aspects) : higherAspectDegree(dif, aspects);
        //    //double destination = refAspect + aspectDegree;

        //    do
        //    {
        //        interiorPos = PositionOf(jul_ut, pair.Interior, iFlag);
        //        exteriorPos = PositionOf(jul_ut, pair.Exterior, iFlag);
        //        aspectDegree = interiorPos.Longitude - exteriorPos.Longitude;

        //        dif = (destination - interiorPos[0] + exteriorPos[0]);

        //        if (Math.Abs(dif) <= NegligibleDegrees)
        //        {
        //            return new AspectRecurrenceEvent(UtcFromJulianDay(jul_ut), interiorPos, exteriorPos, aspectDegree);
        //        }

        //        offset = dif / interiorPos[3];
        //        jul_ut += offset;
        //        interiorPos = PositionOf(jul_ut, pair.Interior, iFlag);
        //        exteriorPos = PositionOf(jul_ut, pair.Exterior, iFlag); 
        //        beeline = Angle.BeelineOf(interiorPos.Longitude, exteriorPos.Longitude);


        //    } while (tryCount++ < MaxTryOfGetExactTime);

        //    Console.WriteLine(String.Format("Failed to get the aspected transcension time on {0}", date));

        //    return null;
        //}

        private static IPlanetEvent onHorizonEventOf(PlanetId id, DateTimeOffset date, SeFlg iFlag)
        {
            double jul_ut = ToJulianDay(date);
            Position current;
            double dif, offset;

            int tryCount = 0;

            do
            {
                current = PositionOf(jul_ut, id, iFlag);
                dif = current.Latitude;

                if (Math.Abs(dif) <= NegligibleDegrees)
                {
                    return new DeclinationEvent(id, UtcFromJulianDay(jul_ut), current, PlanetEventFlag.OnEquator);
                }

                offset = -dif / current.LatitudeVelocity;
                jul_ut += offset;
            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the exact onHorizon time on {0}", date));

            return null;
        }

        private static IPlanetEvent exactAspectEventOf(PlanetId id1, PlanetId id2, DateTimeOffset date, SeFlg iFlag)
        {
            double jul_ut = ToJulianDay(date);
            Position pos1, pos2;
            pos1 = PositionOf(jul_ut, id1, iFlag);
            pos2 = PositionOf(jul_ut, id2, iFlag);
            double offset, aspDegree, dif = pos2.Longitude - pos1.Longitude;

            aspDegree = (pos1.LongitudeVelocity >= pos2.LongitudeVelocity) ? lowerAspectDegree(dif, Aspect.AllAspectDegrees) : higherAspectDegree(dif, Aspect.AllAspectDegrees);
            int tryCount = 0;

            do
            {
                dif = dif - aspDegree;

                if (Math.Abs(dif) > 300)
                    dif = (360 + dif) % 360;

                if (Math.Abs(dif) <= NegligibleDegrees)
                {
                    return new ExactAspectEvent(UtcFromJulianDay(jul_ut), pos1, pos2, Aspect.All[aspDegree], PlanetEventFlag.HorizontalAspected);
                }

                offset = dif / (pos1.LongitudeVelocity - pos2.LongitudeVelocity);
                jul_ut += offset;
                pos1 = PositionOf(jul_ut, id1, iFlag);
                pos2 = PositionOf(jul_ut, id2, iFlag);
                dif = pos2.Longitude - pos1.Longitude;
            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the exact aspect time on {0}", date));

            return null;
        }


        /// <summary>
        /// Check if the declination/latitude is approximate to 0.
        /// </summary>
        /// <param name="pos">Position of a planet.</param>
        /// <returns>True if the latitude/declination is very close to 0 degree.</returns>
        private static bool isOnHorizonPredicate(Position pos)
        {
            return Math.Abs(pos.Latitude) <= NegligibleDegrees;
        }

        /// <summary>
        /// When adjustment needed for the planet to be on exact point of the cusp.
        /// </summary>
        /// <param name="currentPos">Current Position of the planet.</param>
        /// <returns>When offset in days.</returns>
        private static double timeToCusp(Position currentPos)
        {
            double rectascension = currentPos[0];
            double cuspDegrees = Math.Round(rectascension / 30) * 30;

            return (cuspDegrees - rectascension) / currentPos[3];
        }

        ///// <summary>
        ///// Check if the dif/longitude is not change, or its longitude speed is almost 0.
        ///// </summary>
        ///// <param name="pos">Position of a planet.</param>
        ///// <returns>True if the dif/longitude speed is very close to 0 degree/day.</returns>
        //private static bool isLatitudeNoChangePredicate(Position pos)
        //{
        //    return Math.Abs(pos[3]) <= StandStillSpeed;
        //}

        ///// <summary>
        ///// Check if the declination/latitude is not change, or its latitude speed is almost 0.
        ///// </summary>
        ///// <param name="pos">Position of a planet.</param>
        ///// <returns>True if the latitude/declination speed is very close to 0 degree/day.</returns>
        //private static bool isLatitudeNoChangePredicate(Position pos)
        //{
        //    return Math.Abs(pos[4]) <= StandStillSpeed;
        //}

        ///// <summary>
        ///// Time in percentage of the interval of the degree1 and the end state, when the movement is halted.
        ///// </summary>
        ///// <param name="degree1">Current Position values of a planet.</param>
        ///// <param name="end">End Position of the planet.</param>
        ///// <returns>Percentage of the time to turning.</returns>
        //public static double relativeTimeToStandStill(double[] degree1, double[] end, int posIndex, int speedIndex)
        //{
        //    double firstPos = degree1[posIndex];
        //    double secondPos = end[posIndex];
        //    double firstSpeed = degree1[speedIndex];
        //    double secondSpeed = end[speedIndex];
        //    //double endPos = (firstPos + secondPos) / 2 + (firstSpeed - secondSpeed) / 8;

        //}

        ///// <summary>
        ///// Evaluate to get the approximate time to the turning point.
        ///// </summary>
        ///// <param name="firstPos">The first reference's position.</param>
        ///// <param name="secondPos">The second reference's position, for rectascention evaluation, its distance to firstPos shall be within reasonable range.</param>
        ///// <param name="firstSpeed">Speed on the first reference position.</param>
        ///// <param name="secondSpeed">Speed on the second reference position.</param>
        ///// <returns>Time in days.</returns>
        //public static double timeToTurning(double firstPos, double secondPos, double firstSpeed, double secondSpeed)
        //{
        //    double averageSpeed, distance;

        //    if ((firstSpeed < 0 && secondSpeed > 0) || (firstSpeed > 0 && secondSpeed < 0)) //Means the turning point is between them
        //    {
        //        averageSpeed = Math.Min(Math.Abs(firstSpeed), Math.Abs(secondSpeed));
        //        distance = Math.Abs(firstPos - secondPos);

        //        return Math.Abs((secondPos - firstPos) / (firstSpeed + secondSpeed));
        //    }
        //    else if (firstSpeed > StandStillSpeed)
        //    {

        //    }
        //    else if (firstSpeed < -StandStillSpeed)
        //    {

        //    }
        //    else
        //        throw new NotImplementedException(String.Format("The case is not considered when degree1={0}, s1={1}, degree2={2}, s2={3}", 
        //            firstPos, firstSpeed, secondPos, secondSpeed));
        //}

        #endregion

        #region Date of Events Detection functions

        //private static double lowerAspectDegree(double dif, AspectImportance importance)
        //{
        //    List<double> aspects = null;

        //    switch (importance)
        //    {
        //        case AspectImportance.Critical:
        //            aspects = Aspect.CriticalAspectDegrees;
        //            break;
        //        case AspectImportance.Important:
        //            aspects = Aspect.ImportantAspectDegrees;
        //            break;
        //        case AspectImportance.Effective:
        //            aspects = Aspect.EffectiveAspectDegrees;
        //            break;
        //        default:
        //            aspects = Aspect.AllAspectDegrees;
        //            break;
        //    }

        //    dif = (dif + 360) % 360;

        //    for (int i = aspects.Count - 1; i >= 0; i--)
        //    {
        //        if (aspects[i] <= dif)
        //            return aspects[i];
        //    }
        //    return 0;
        //}

        //private static double higherAspectDegree(double dif, AspectImportance importance)
        //{
        //    List<double> aspects = null;

        //    switch (importance)
        //    {
        //        case AspectImportance.Critical:
        //            aspects = Aspect.CriticalAspectDegrees;
        //            break;
        //        case AspectImportance.Important:
        //            aspects = Aspect.ImportantAspectDegrees;
        //            break;
        //        case AspectImportance.Effective:
        //            aspects = Aspect.EffectiveAspectDegrees;
        //            break;
        //        default:
        //            aspects = Aspect.AllAspectDegrees;
        //            break;
        //    }

        //    dif = (dif + 360) % 360;

        //    for (int i = 0; i < aspects.Count; i++)
        //    {
        //        if (aspects[i] >= dif)
        //            return aspects[i];
        //    }
        //    return 0;
        //}

        private static double lowerAspectDegree(double dif, List<double> aspects)
        {
            dif = (dif + 360) % 360;

            for (int i = aspects.Count - 1; i >= 0; i--)
            {
                if (aspects[i] <= dif)
                    return aspects[i];
            }
            return 0;
        }

        private static double higherAspectDegree(double dif, List<double> aspects)
        {
            dif = (dif + 360) % 360;

            for (int i = 0; i < aspects.Count; i++)
            {
                if (aspects[i] >= dif)
                    return aspects[i];
            }
            return 0;
        }

        /// <summary>
        /// Calculate the i of the dif where sign is changed, or pass multiples of 30 degrees.
        /// </summary>
        /// <param name="longDif">The continuous dif values</param>
        /// <returns>Indexes of days where entrance of new sign happened.</returns>
        public static List<int> SignChangeDateDetector(List<double> longitudes)
        {
            List<int> result = new List<int>();
            List<int> signNums = new List<int>();
            foreach (Double rect in longitudes)
            {
                signNums.Add((int)(rect / 30));
            }

            int prev = signNums[0];
            for (int i = 1; i < signNums.Count; i++)
            {
                if (prev == signNums[i])
                    continue;

                result.Add(i - 1);
                prev = signNums[i];
            }

            return result;
        }

        /// <summary>
        /// Calculate the i of values where crossing zero is detected.
        /// </summary>
        /// <param name="values">
        ///     The continuous values, either dif, declination, distance or velocity.
        /// </param>
        /// <returns>Indexes of days of such value pattern changes are detected.</returns>
        public static List<int> CrossZeroDateDetector(List<double> values)
        {
            List<int> result = new List<int>();

            double old = values[0];
            double current;

            for (int i = 1; i < values.Count; i++)
            {
                current = values[i];

                if ((old < 0 && current >= 0) || (old > 0 && current <= 0))
                    result.Add(i-1);

                old = current;
            }

            return result;
        }

        public static List<int> TranscensionDateDetector(List<double> everyDays, double refDegree, List<double> aspects)
        {
            List<int> result = new List<int>();
            double dif, higherAspect, lowerAspect, offset = 720 - refDegree;
            List<double> deltas = new List<double>();

            for (int i = 0; i < everyDays.Count; i++)
            {
                dif = (offset + everyDays[i]) % 360;
                deltas.Add(dif);
            }

            lowerAspect = lowerAspectDegree(deltas[0], aspects);
            higherAspect = higherAspectDegree(deltas[0], aspects);

            for (int i = 1; i < deltas.Count; i++)
            {
                dif = deltas[i];

                if (dif <= 0)
                {
                    if (lowerAspect == 0)
                    {
                        lowerAspect = lowerAspectDegree(dif, aspects);
                        higherAspect = higherAspectDegree(dif, aspects);

                        result.Add(i - 1);
                    }
                }
                else if (dif <= lowerAspect || (dif >= higherAspect && higherAspect != 0))
                {
                    lowerAspect = lowerAspectDegree(dif, aspects);
                    higherAspect = higherAspectDegree(dif, aspects);

                    result.Add(i - 1);
                }

            }

            return result;
        }

        public static List<int> AspectDateDetector(List<double> orbits1, List<double> orbits2, AspectImportance importance)
        {
            List<double> aspects = null;

            switch (importance)
            {
                case AspectImportance.Critical:
                    aspects = Aspect.CriticalAspectDegrees;
                    break;
                case AspectImportance.Important:
                    aspects = Aspect.ImportantAspectDegrees;
                    break;
                case AspectImportance.Effective:
                    aspects = Aspect.EffectiveAspectDegrees;
                    break;
                default:
                    aspects = Aspect.AllAspectDegrees;
                    break;
            }

            if (orbits1.Count != orbits2.Count)
                throw new Exception();

            List<double> difToAspects = new List<double>();
            double dif, higherAspect, lowerAspect;
            List<int> result = new List<int>();

            for(int i = 0; i < orbits1.Count; i ++)
            {
                dif = (375 + orbits1[i] - orbits2[i] ) % 360 - 15;
                difToAspects.Add(dif);
            }

            lowerAspect = lowerAspectDegree(difToAspects[0], aspects);
            higherAspect = higherAspectDegree(difToAspects[0], aspects);

            for (int i = 1; i < difToAspects.Count; i ++ )
            {
                dif = difToAspects[i];

                if (dif <= 0)
                {
                    if (lowerAspect == 0)
                    {
                        lowerAspect = lowerAspectDegree(difToAspects[i], aspects);
                        higherAspect = higherAspectDegree(difToAspects[i], aspects);

                        result.Add(i - 1);
                    }
                }
                else if (dif <= lowerAspect || (dif >= higherAspect && higherAspect != 0))
                {
                    lowerAspect = lowerAspectDegree(difToAspects[i], aspects);
                    higherAspect = higherAspectDegree(difToAspects[i], aspects);

                    result.Add(i - 1);
                }

            }

            return result;
        }

        ///// <summary>
        ///// Calculate the i of values where is trend (continuously increase/decrease) or cross zero is detected.
        ///// </summary>
        ///// <param name="values">
        /////     The continuous values, either dif, declination, distance or velocity.
        /////     Caution: the values shall append one extra before the eventDate theDay, as well as one extra data after the end theDay
        /////             to detect increase/decrease changes.
        ///// </param>
        ///// <returns>Indexes of days of such value pattern changes are detected.</returns>
        //public static List<int> MaxMinZeroDateDetector(List<double> values)
        //{
        //    List<int> dif = new List<int>();
        //    List<int> temp = new List<int>();

        //    double old = values[0];
        //    double degree1;
        //    bool oldTrend = values[1] >= values[0];
        //    bool currentTrend;

        //    for (int i = 1; i < values.Count - 1; i++)
        //    {
        //        degree1 = values[i];
        //        currentTrend = values[i + 1] >= degree1;

        //        if (old < 0)
        //        {
        //            if (degree1 > 0)              //Cross over zero is detected.
        //                dif.Add(i - 1);
        //            else if (oldTrend != currentTrend)
        //                dif.Add(i);
        //        }
        //        else if (old > 0)
        //        {
        //            if (degree1 < 0)              //Cross over zero is detected.
        //                dif.Add(i - 1);
        //            else if (oldTrend != currentTrend)
        //                dif.Add(i);
        //        }
        //        else
        //            dif.Add(i - 1);

        //        old = degree1;
        //        oldTrend = currentTrend;
        //    }

        //    if (dif.Contains(0))
        //        dif.RemoveAt(0);

        //    return dif;
        //}

        #endregion

        #region Other feasibilities

        public static List<double> SmoothingOfAverage(List<double> original)
        {
            List<double> result = new List<double>();
            result.Add(original[0]);
            double next, dif;

            for (int i = 1; i < original.Count; i++)
            {
                next = original[i];
                dif = next - original[i-1];

                if (dif > 30 || dif < -30)
                {
                    dif = Math.Round(dif);
                    for (int j = i; j < original.Count; j ++ )
                    {
                        original[j] = (original[j] - dif + 360) % 360;
                    }
                }

                result.Add(original[i]);
            }

            return result;
        }

        #endregion

        //public static DateTimeOffset DateOfPlanetPosition(PlanetId star, DateTimeOffset eventDate, Rectascension aspectDegree)
        //{
        //    DateTimeOffset theDay;
        //    Where posPrev, posNext;
        //    Double difPrev, difNext;

        //    posPrev = CurrentEphemeris[eventDate, star];
        //    difPrev = Angle.DistanceBetween(posPrev.Longitude, aspectDegree.Degrees);

        //    if (difPrev > 60)
        //        throw new ArgumentOutOfRangeException("The theDay of starting search is not set properly.");
        //    else if (difPrev < Negligible)
        //        return eventDate;

        //    for (int i = 1; i < 200; i ++ )
        //    {
        //        theDay = eventDate.AddDays(i);
        //        posNext = CurrentEphemeris[theDay, star];

        //        difNext = Angle.DistanceBetween(posNext.Longitude, aspectDegree.Degrees);

        //        if (difPrev <= difNext)
        //        {
        //            return theDay.AddDays(-1);
        //        }
        //        else if (difPrev < Angle.DistanceBetween(posNext.Longitude, posPrev.Longitude))
        //        {
        //            return theDay.AddDays(-1);
        //        }

        //        posPrev = posNext;
        //        difPrev = difNext;
        //    }

        //    return DateTimeOffset.MinValue;

        //    //Double cusp = aspectDegree.Within.Cusp;
        //    //Sign destSign = aspectDegree.Within;
        //    //Where dateStartPosition, dateEndPosition;

        //    //DateTimeOffset date1, date2, enter, exit;

        //    //enter = entryDateAround(star, destSign, eventDate);
        //    //exit = entryDateAround(star, destSign + 1, enter);

        //    //for (date1 = enter.AddDays(-1); date1 < exit; date1 += TimeSpan.FromDays(1) )
        //    //{
        //    //    date2 = date1.AddDays(1);
        //    //    dateStartPosition = Geocentric[date1, star];
        //    //    dateEndPosition = Geocentric[date2, star];

        //    //    Double b2 = Angle.BeelineOf(dateEndPosition.Longitude, aspectDegree.Degrees);
        //    //    Double b1 = Angle.BeelineOf(dateStartPosition.Longitude, aspectDegree.Degrees);

        //    //    if (b2 > 0 || b1 < 0)
        //    //        continue;

        //    //    return date2;
        //    //}

        //    throw new Exception();
        //}

        //private static DateTimeOffset RetroOrDirectDate(PlanetId star, DateTimeOffset SolarEclipseAround, int period)
        //{
        //    if (!Planet.RetrogradePeriods.ContainsKey(star))
        //        return DateTimeOffset.MinValue;

        //    DateTimeOffset theDay = SolarEclipseAround;

        //    Where dateStartPosition = Geocentric[SolarEclipseAround, star];

        //    if (dateStartPosition.LongitudeVelocity < 0)
        //        theDay -= TimeSpan.FromDays((int)(period));

        //    List<DateTimeOffset> watched = new List<DateTimeOffset>();

        //    for (int i = -period; i <= period; i++)
        //    {
        //        watched.Add(theDay.AddDays(i));
        //    }

        //    List<double> longVelocities = new List<double>();
        //    List<double> longDif = new List<double>();

        //    foreach (DateTimeOffset day in watched)
        //    {
        //        dateStartPosition = Geocentric[day, star];
        //        longVelocities.Add(dateStartPosition.LongitudeVelocity);
        //        longDif.Add(dateStartPosition.Longitude);
        //    }

        //    List<double> reversedVelocities = TurningsOf(longVelocities);

        //    if (reversedVelocities.Count == 0)
        //        return DateTimeOffset.MinValue;
        //    else
        //        return watched[longVelocities.IndexOf(reversedVelocities[0])];
        //}

        //public static List<DateTimeOffset> RetrogradeDateAround(PlanetId star, DateTimeOffset around)
        //{
        //    if (!Planet.RetrogradePeriods.ContainsKey(star))
        //    {
        //        return null;
        //    }

        //    int period = (int)Planet.RetrogradePeriods[star];
        //    DateTimeOffset theDay = around;

        //    List<DateTimeOffset> results = new List<DateTimeOffset>();
        //    Double longRetro = -1, longDirect = -1;

        //    Where dateStartPosition =  Geocentric[around, star];
            
        //    if (dateStartPosition.LongitudeVelocity < 0)
        //        theDay -= TimeSpan.FromDays((int)(period/2));

        //    List<DateTimeOffset> watched = new List<DateTimeOffset>();

        //    for (int i = -period; i <= period; i ++)
        //    {
        //        watched.Add(theDay.AddDays(i));
        //    }

        //    //List<double> longVelocities = new List<double>();
        //    //List<double> longDif = new List<double>();

        //    //foreach (DateTimeOffset day in watched)
        //    //{
        //    //    dateStartPosition =  Geocentric[day, star];
        //    //    longVelocities.Add(dateStartPosition.LongitudeVelocity);
        //    //    longDif.Add(dateStartPosition.Longitude);
        //    //}

        //    //List<double> reversedVelocities = TurningsOf(longVelocities);

        //    //if (reversedVelocities.Count == 2)
        //    //{
        //    //    int i = longVelocities.IndexOf(reversedVelocities[0]);
        //    //    dateStartPosition = Geocentric[watched[i], star];
        //    //    if (reversedVelocities[0] > 0)
        //    //        longRetro = dateStartPosition.Longitude;
        //    //    else
        //    //        longDirect = dateStartPosition.Longitude;

        //    //    i = longVelocities.IndexOf(reversedVelocities[1]);
        //    //    dateStartPosition = Geocentric[watched[i], star];
        //    //    if (reversedVelocities[1] > 0)
        //    //        longRetro = dateStartPosition.Longitude;
        //    //    else
        //    //        longDirect = dateStartPosition.Longitude;
        //    //}
        //    //else
        //    //    return null;


        //    if (dateStartPosition.LongitudeVelocity >= 0)
        //    {
        //        for (int i = -2; i <= 2; i++)
        //        {
        //            theDay = around.AddDays((int)(i * period / 2));
        //            dateStartPosition = Geocentric[theDay, star];

        //            if (dateStartPosition.LongitudeVelocity < 0)
        //                break;
        //        }

        //        if (dateStartPosition.LongitudeVelocity < 0)
        //            around = theDay;
        //        else
        //            return null;
        //    }

        //    for (int j = 1; j <= period + 5; j++)
        //    {
        //        theDay = around.AddDays(-j);
        //        dateStartPosition = Geocentric[theDay, star];
        //        if (dateStartPosition.LongitudeVelocity < 0)
        //            continue;

        //        results.Add(theDay);
        //        longRetro = dateStartPosition.Longitude;
        //        break;
        //    }

        //    for (int k = 1; k <= period + 5; k++)
        //    {
        //        theDay = around.AddDays(k);

        //        dateStartPosition = Geocentric[theDay, star];
        //        if (dateStartPosition.LongitudeVelocity < 0)
        //            continue;

        //        results.Add(theDay.AddDays(-1));
        //        longDirect = dateStartPosition.Longitude;
        //        break;
        //    }

        //    if (longDirect == -1 || longRetro == -1)
        //    {
        //        return null;
        //        //throw new Exception("Failed to get either direct or retrograde SolarEclipseAround, the retrograde period may not be proper?");
        //    }
        //    else
        //    {
        //        theDay = results[0].AddDays(-period - 10);
        //        DateTimeOffset temp = DateOfPlanetPosition(star, theDay, new Rectascension(longDirect));

        //        if (temp != DateTimeOffset.MinValue)
        //            results.Add(temp);

        //        temp = DateOfPlanetPosition(star, results[1], new Rectascension(longRetro));

        //        if (temp != DateTimeOffset.MinValue)
        //            results.Add(temp);
        //    }

        //    if (results.Count == 4)
        //    {
        //        results.Sort();
        //        return results;
        //    }
        //    else
        //    {
        //        return null;
        //        //throw new Exception("Something is wrong to the the retrograde related events?");
        //    }

        //}

        //private static DateTimeOffset entryDateAround(PlanetId star, Sign destSign, DateTimeOffset around)
        //{
        //    Double speed = Planet.AverageSpeedOf(star);
        //    int dayAdjustment = 0;
        //    DateTimeOffset theDay = around.UtcDateTime.Date;
        //    Where dateStartPosition = CurrentEphemeris[theDay, star];

        //    int dif = destSign.Order - Sign.SignOf(dateStartPosition.Longitude).Order;
        //    dayAdjustment = dif > 0 ? (int)((destSign.Order - Sign.SignOf(dateStartPosition.Longitude).Order - 1) * 30 / speed)
        //        : (int)((11 + destSign.Order - Sign.SignOf(dateStartPosition.Longitude).Order) * 30 / speed);

        //    do
        //    {
        //        theDay = theDay.AddDays(dayAdjustment);
        //        dateStartPosition = CurrentEphemeris[theDay, star];
        //        dayAdjustment--;
        //    } while (Sign.SignOf(dateStartPosition.Longitude) != destSign.Previous);

        //    if (Sign.SignOf(dateStartPosition.Longitude) != destSign.Previous)
        //        throw new Exception();

        //    do 
        //    {
        //        theDay = theDay.AddDays(1);
        //        dateStartPosition = CurrentEphemeris[theDay, star];
        //        if (Sign.SignOf(dateStartPosition.Longitude) == destSign)
        //            return theDay;
        //    } while (true);

        //    throw new Exception();
        //}

        //public static DateTimeOffset VernalEquinoxTimeOf(int year)
        //{
        //    if (Vernals == null)
        //        Vernals = new Dictionary<int, DateTimeOffset>();

        //    if (!Vernals.ContainsKey(year))
        //    {
        //        double jul_ut = ToJulianDay(new DateTimeOffset(year, 3, 15, 12, 0, 0, TimeSpan.Zero));
        //        double averageSpeed = 360.0 / AverageYearLength;
        //        Where posDynamic = Geocentric.PositionDelegate(jul_ut, PlanetId.SE_SUN);

        //        double shiftDynamic;

        //        double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, 360);

        //        shiftDynamic = distanceDynamic / averageSpeed;

        //        for (int i = 0; i < 10; i++)
        //        {
        //            posDynamic = Geocentric.PositionDelegate(jul_ut + shiftDynamic, PlanetId.SE_SUN);

        //            distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, 360);

        //            if (Math.Abs(distanceDynamic) < Negligible)
        //                break;

        //            jul_ut += shiftDynamic;

        //            shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //        }

        //        Vernals.Add(year, UtcFromJulianDay(jul_ut));
        //    }

        //    return Vernals[year];
        //}

        //public static List<RelationBrief> RelationsOn(DateTimeOffset theDay)
        //{
        //    List<Where> positions = CurrentEphemeris[theDay];

        //    List<RelationBrief> allRelations = new List<RelationBrief>();

        //    Where superiorPos, inferiorPos;
        //    Angle distance;
        //    Aspects aspect;

        //    for (int i = Masters.Count - 1; i > 0; i--)
        //    {
        //        PlanetId superiorId = Masters[i];
        //        superiorPos = CurrentEphemeris.PositionOf(positions, superiorId);

        //        for (int j = i - 1; j >= 0; j--)
        //        {
        //            PlanetId inferiorId = Masters[j];
        //            inferiorPos = CurrentEphemeris.PositionOf(positions, inferiorId);
        //            distance = inferiorPos.Longitude - superiorPos.Longitude;
        //            aspect = Aspects.CurrentAspectOf(distance);

        //            if (aspect != null)
        //                allRelations.Add(new RelationBrief(theDay, superiorPos, inferiorPos));
        //        }
        //    }

        //    return allRelations;
        //}

        //public static List<RelationBrief> RelationsOn(DateTimeOffset theDay, PlanetId refStar)
        //{
        //    List<RelationBrief> allRelations = new List<RelationBrief>();

        //    Where refDegree = CurrentEphemeris[theDay, refStar];

        //    Where anotherPos;
        //    Angle distance;
        //    Aspects aspect;

        //    for (PlanetId anotherId = PlanetId.SE_SUN; anotherId <= PlanetId.SE_CHIRON; anotherId ++ )
        //    {
        //        if (anotherId == refStar || !Masters.Contains(anotherId))
        //            continue;

        //        anotherPos = CurrentEphemeris[theDay, anotherId];
        //        distance = refDegree.Longitude - anotherPos.Longitude;

        //        aspect = Aspects.CurrentAspectOf(distance);

        //        if (aspect != null)
        //            allRelations.Add(new RelationBrief(theDay, anotherPos, refDegree));
        //    }

        //    return allRelations;
            
        //    //List<RelationBrief> allRelations = RelationsOn(SolarEclipseAround);
        //    //return (from relation in allRelations
        //    //        where relation.Inferior == refStar || relation.Superior == refStar
        //    //        select relation).ToList();
        //}

        //public static List<RelationBrief> RelationsWithin(MatchRules period)
        //{
        //    DateTimeOffset startDate = period.Since.UtcDateTime.Date;
        //    DateTimeOffset endDate = period.Until.UtcDateTime.Date;

        //    List<RelationBrief> allRelations = new List<RelationBrief>();

        //    for (DateTimeOffset theDay = startDate; theDay < endDate; theDay += TimeSpan.FromDays(1))
        //    {
        //        allRelations.AddRange(RelationsOn(theDay));
        //    }

        //    if (endDate - startDate <= TimeSpan.FromDays(1))
        //        return allRelations;

        //    allRelations = Optimize(allRelations);

        //    return allRelations;
        //}

        //public static List<RelationBrief> RelationsWithin(MatchRules period, PlanetId oneParty)
        //{
        //    DateTimeOffset startDate = period.Since.UtcDateTime.Date;
        //    DateTimeOffset endDate = period.Until.UtcDateTime.Date;

        //    List<RelationBrief> allRelations = new List<RelationBrief>();

        //    for (DateTimeOffset theDay = startDate; theDay < endDate; theDay += TimeSpan.FromDays(1))
        //    {
        //        allRelations.AddRange(RelationsOn(theDay, oneParty));
        //    }

        //    if (endDate - startDate <= TimeSpan.FromDays(1))
        //        return allRelations;

        //    allRelations = Optimize(allRelations);

        //    return allRelations;
        //}

        //public static List<RelationBrief> Optimize(List<RelationBrief> allRelations)
        //{
        //    List<RelationBrief> dif = new List<RelationBrief>();
        //    RelationBrief newRelation = null;

        //    var relationClassifier =
        //        from relation in allRelations
        //        group relation by new { relation.Superior, relation.Inferior, relation.Flag.Around } into relationGroup
        //        orderby relationGroup.Key.Superior descending, relationGroup.Key.Inferior descending
        //        select relationGroup;


        //    foreach (var relations in relationClassifier)
        //    {
        //        if (relations.Count() == 0)
        //            throw new Exception();
        //        else if (relations.Count() == 1)
        //            dif.Add(relations.First());
        //        else
        //        {
        //            newRelation = null;
        //            double orb = 100;
        //            foreach (RelationBrief relation in relations)
        //            {
        //                if (Math.Abs(relation.Orb) < orb)
        //                {
        //                    orb = Math.Abs(relation.Orb);
        //                    newRelation = relation;
        //                }
        //            }
        //            dif.Add(newRelation);
        //        }
        //    }

        //    return dif;
        //}

        #endregion

        #endregion

        #region Properties

        public string Name { get; private set; }

        public SeFlg CenterFlag { get; private set; }

        public List<PlanetId> Luminaries { get { return CenterFlag == SeFlg.HELIOCENTRIC ? HeliocentricLuminaries : GeocentricLuminaries; } }

        public DateTimeOffset Since { get { return Buffer.Keys.FirstOrDefault(); } }

        public DateTimeOffset Until { get { return Buffer.Keys.LastOrDefault(); } }

        public int Count { get { return Buffer.Count; } }

        public SortedDictionary<DateTimeOffset, List<Position>> Buffer { get; private set; }

        public Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> Aspectarian { get; private set; }

        public List<Position> this[DateTimeOffset date]
        {
            get
            {
                if (date.TimeOfDay != TimeSpan.Zero || date.Offset != TimeSpan.Zero)
                {
                    return lookFor(ToJulianDay(date));
                }

                if (!Buffer.ContainsKey(date))
                {
                    Ephemeris decade = decadeEphemerisOf(date);
                    this.merge(decade);
                }

                return Buffer[date];

            }
        }

        public List<Position> this[Double julianDay]
        {
            get
            {
                return this[UtcFromJulianDay(julianDay)];
            }
        }

        public Position this[DateTimeOffset date, PlanetId id]
        {
            get
            {
                if (date.TimeOfDay != TimeSpan.Zero)
                {
                    double jul_ut = ToJulianDay(date);
                    return PositionOf(jul_ut, id, CenterFlag);
                }

                if (!Buffer.ContainsKey(date))
                {
                    Ephemeris decade = decadeEphemerisOf(date);
                    this.merge(decade);
                }

                Position result = PositionOf(Buffer[date], id);

                if (result == null)
                {
                    double jul_ut = ToJulianDay(date);
                    result = PositionOf(jul_ut, id, CenterFlag);
                    Buffer[date].Add(result);
                }

                return result;
            }
        }

        public Position this[Double julianDay, PlanetId id]
        {
            get
            {
                return this[UtcFromJulianDay(julianDay), id];
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end]
        {
            get
            {
                checkFullness(start, end);

                List<IPlanetEvent> result = new List<IPlanetEvent>();

                foreach (Dictionary<PlanetId, List<IPlanetEvent>> category in this.Aspectarian.Values)
                {
                    foreach (List<IPlanetEvent> evtList in category.Values)
                    {
                        foreach (IPlanetEvent evt in evtList)
                        {
                            if (evt.When > end)
                                break;
                            else if (evt.When < start)
                                continue;
                            else
                                result.Add(evt);
                        }
                    }
                }

                result.Sort();
                return result;
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, PlanetEventFlag category]
        {
            get
            {
                checkFullness(start, end);

                if (Aspectarian.ContainsKey(category))
                {
                    List<IPlanetEvent> result = new List<IPlanetEvent>();

                    foreach (List<IPlanetEvent> evtList in Aspectarian[category].Values)
                    {
                        foreach (IPlanetEvent evt in evtList)
                        {
                            if (evt.When > end)
                                break;
                            else if (evt.When < start)
                                continue;
                            else if(!result.Contains(evt))
                                result.Add(evt);
                        }
                    }

                    result.Sort();
                    return result;
                }
                else
                    return null;

            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, AspectImportance importance]
        {
            get
            {
                checkFullness(start, end);

                if (Aspectarian.ContainsKey(PlanetEventFlag.AspectCategory))
                {
                    List<IPlanetEvent> result = new List<IPlanetEvent>();

                    foreach (List<IPlanetEvent> evtList in Aspectarian[PlanetEventFlag.AspectCategory].Values)
                    {
                        foreach (IPlanetEvent evt in evtList)
                        {
                            if (evt.When > end)
                                break;
                            else if (evt.When < start)
                                continue;
                            else
                            {
                                ExactAspectEvent aspEvent = evt as ExactAspectEvent;

                                if (aspEvent != null && aspEvent.TheAspect.Importance >= importance)
                                    result.Add(aspEvent);
                            }
                        }
                    }

                    result.Sort();
                    return result;
                }

                return null;
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, PlanetId id, AspectImportance importance]
        {
            get
            {
                checkFullness(start, end);

                if (Aspectarian.ContainsKey(PlanetEventFlag.AspectCategory) && Aspectarian[PlanetEventFlag.AspectCategory].ContainsKey(id))
                {
                    List<IPlanetEvent> result = new List<IPlanetEvent>();

                    foreach (IPlanetEvent evt in Aspectarian[PlanetEventFlag.AspectCategory][id])
                    {
                        if (evt.When > end)
                            break;
                        else if (evt.When < start)
                            continue;
                        else
                        {
                            ExactAspectEvent aspEvent = evt as ExactAspectEvent;
                            if (aspEvent.TheAspect.Importance >= importance)
                                result.Add(evt);
                        }
                    }
                    return result;
                }
                else
                    return null;
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, PlanetEventFlag category, PlanetId id]
        {
            get
            {
                checkFullness(start, end);

                if (Aspectarian.ContainsKey(category) && Aspectarian[category].ContainsKey(id))
                {
                    List<IPlanetEvent> result = new List<IPlanetEvent>();

                    foreach (IPlanetEvent evt in Aspectarian[category][id])
                    {
                        if (evt.When > end)
                            break;
                        else if (evt.When < start)
                            continue;
                        else
                            result.Add(evt);
                    }
                    result.Sort();
                    return result;
                }
                else
                    return null;
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, PlanetId star1, PlanetId star2]
        {
            get
            {
                return this[start, end, star1, star2, AspectImportance.Important];
            }
        }

        public List<IPlanetEvent> this[DateTimeOffset start, DateTimeOffset end, PlanetId star1, PlanetId star2, AspectImportance importance]
        {
            get
            {
                checkFullness(start, end);

                if (!Luminaries.Contains(star1) || !Luminaries.Contains(star2) || star1 == star2 || star1 > PlanetId.SE_FICT_OFFSET || star2 > PlanetId.SE_FICT_OFFSET)
                    throw new Exception();

                if (Luminaries.IndexOf(star1) > Luminaries.IndexOf(star2))
                {
                    PlanetId temp = star2;
                    star2 = star1;
                    star1 = temp;
                }

                List<IPlanetEvent> events = Aspectarian[PlanetEventFlag.AspectCategory][star1];
                List<IPlanetEvent> result = new List<IPlanetEvent>();

                foreach (IPlanetEvent evt in events)
                {
                    ExactAspectEvent aspectEvent = evt as ExactAspectEvent;

                    if (aspectEvent != null && aspectEvent.Exterior == star2 && aspectEvent.When <= end && aspectEvent.TheAspect.Importance >= importance)
                        result.Add(aspectEvent);
                }
                return result;
            }
        }

        #endregion

        #region Constructors

        private Ephemeris(string name, SeFlg iFlag)
        {
            Buffer = new SortedDictionary<DateTimeOffset, List<Position>>();
            Aspectarian = new Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>>();
            Name = name;
            CenterFlag = iFlag;
        }

        #endregion

        #region functions

        public Position PositionOf(List<Position> positions, PlanetId id)
        {
            if (Luminaries.Contains(id))
            {
                int index = Luminaries.IndexOf(id);
                Position result = positions[Luminaries.IndexOf(id)];

                if (result.Owner == id)
                    return result;
                else
                    throw new Exception();
            }
            else
                throw new Exception("The buffer doesn't contain position of " + id.ToString());
            //{
            //    for (int i = Luminaries.Count; i < positions.Count; i++)
            //    {
            //        Position pos = positions[i];
            //        if (pos.Owner == id)
            //            return pos;
            //    }
            //}
        }

        private Ephemeris decadeEphemerisOf(DateTimeOffset date)
        {
            int startYear = 10 * (date.Year / 10);
            string fileName = String.Format("{0}{1}.eph", Name, startYear);

            FileInfo[] files = EphemerisDirectory.GetFiles(fileName, SearchOption.AllDirectories);
            Ephemeris decadeEphe = null;

            if (files.Length != 0)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(EphemerisDirectory + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                decadeEphe = (Ephemeris)formatter.Deserialize(stream);
                stream.Close();
            }

            if (decadeEphe == null)
            {
                decadeEphe = new Ephemeris(Name + startYear.ToString(), CenterFlag);
                DateTimeOffset start = new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset end = new DateTimeOffset(startYear + 10, 1, 1, 0, 0, 0, TimeSpan.Zero);
                decadeEphe.calculate(start, end);

                decadeEphe.save();
            }

            return decadeEphe;
        }

        private List<Position> lookFor(Double jul_ut)
        {
            List<Position> positions = new List<Position>();
            Dictionary<PlanetId, Position> posDict = new Dictionary<PlanetId, Position>();

            Position pos = null;

            foreach (PlanetId id in Luminaries)
            {
                if (id < PlanetId.SE_FICT_OFFSET)
                {
                    pos = PositionOf(jul_ut, id, CenterFlag);

                    posDict.Add(id, pos);
                }
                else if (id == PlanetId.Five_Average || id == PlanetId.Six_Average || id == PlanetId.Eight_Average)
                {
                    PlanetId firstPlanetId = (id == PlanetId.Five_Average ? PlanetId.SE_JUPITER : (id == PlanetId.Six_Average ? PlanetId.SE_MARS : PlanetId.SE_MERCURY));

                    int i = 0;
                    double[] starPos, result = new double[6];
                    for (i = 0; i <= (int)(PlanetId.SE_PLUTO - firstPlanetId); i++)
                    {
                        PlanetId star = (PlanetId)(firstPlanetId + i);
                        starPos = posDict[star].Values;

                        for (int j = 0; j < 6; j++)
                        {
                            result[j] += starPos[j];
                        }
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        result[j] /= i;
                    }

                    posDict.Add(id, new Position(id, result));
                }
                else
                    throw new Exception("Unexpected PlanetId: " + id.ToString());
            }

            foreach (PlanetId id in Luminaries)
            {
                positions.Add(posDict[id]);
            }

            return positions;
        }

        private bool merge(Ephemeris other)
        {
            if (!other.Name.StartsWith(this.Name))
                throw new Exception();

            if (Buffer.Count == 0)
            {
                Buffer = other.Buffer;
                Aspectarian = other.Aspectarian;
            }
            else if (Buffer.ContainsKey(other.Buffer.First().Key.AddDays(10)))
            {
                throw new Exception("No need of merge at all ! ?");
            }
            else
            {
                foreach (KeyValuePair<DateTimeOffset, List<Position>> kvp in other.Buffer)
                {
                    if (!Buffer.ContainsKey(kvp.Key))
                        Buffer.Add(kvp.Key, kvp.Value);
                }

                foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> kvp in other.Aspectarian)
                {
                    if (!this.Aspectarian.ContainsKey(kvp.Key))
                        this.Aspectarian.Add(kvp.Key, kvp.Value);
                    else
                    {
                        Dictionary<PlanetId, List<IPlanetEvent>> dict = kvp.Value;

                        foreach(KeyValuePair<PlanetId, List<IPlanetEvent>> events in dict)
                        {
                            if (!this.Aspectarian[kvp.Key].ContainsKey(events.Key))
                                this.Aspectarian[kvp.Key].Add(events.Key, events.Value);
                            else
                            {
                                this.Aspectarian[kvp.Key][events.Key].AddRange(events.Value);
                                this.Aspectarian[kvp.Key][events.Key].Sort();
                            }
                        }
                    }
                }
            }
            
            return true;
        }

        private void checkFullness(DateTimeOffset start, DateTimeOffset end)
        {
            DateTimeOffset dateStart = new DateTimeOffset(start.Year <= end.Year ? start.Year : end.Year, 3, 1, 0, 0, 0, TimeSpan.Zero);
            end = (start < end) ? end : start;

            do 
            {
                Position pos = this[dateStart, PlanetId.SE_MARS];
                dateStart = dateStart.AddYears(10);
            } while (dateStart <= end);
        }

        private void calculate(DateTimeOffset start, DateTimeOffset end)
        {
            DateTimeOffset firstDate = new DateTimeOffset(start.UtcDateTime.Date, TimeSpan.Zero);
            DateTimeOffset lastDate = new DateTimeOffset(end.UtcDateTime.Date, TimeSpan.Zero);
            TimeSpan step = TimeSpan.FromDays(1);

            Double dateVal = ToJulianDay(firstDate);

            for (DateTimeOffset date = firstDate; date <= lastDate; date +=  step, dateVal++)
            {                
                Buffer[date] = lookFor(dateVal);
            }

            getPlanetEvents();

            swe_close();
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getOccultationEvents()
        {
            if (CenterFlag == SeFlg.GEOCENTRIC)
            {
                DateTimeOffset now = DateTimeOffset.Now;

                Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
                List<IPlanetEvent> planetEventList = solarEclipseDuring(Since, Until, CenterFlag);
                planetEventList.AddRange(lunarEclipseDuring(Since, Until, CenterFlag));
                planetEventList.Sort();

                planetEventsDict.Add(PlanetId.SE_SUN, planetEventList);
                planetEventsDict.Add(PlanetId.SE_MOON, new List<IPlanetEvent>(planetEventList));

                int count = 0;
                for (PlanetId id = PlanetId.SE_MERCURY; id <= PlanetId.SE_PLUTO; id++)
                {
                    planetEventList = occultationDuring(id, Since, Until, CenterFlag);
                    planetEventsDict.Add(id, planetEventList);
                    planetEventsDict[PlanetId.SE_MOON].AddRange(planetEventList);
                    count += planetEventList.Count;
                }

                planetEventsDict[PlanetId.SE_MOON].Sort();

                Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} SolarEclipse and LunarEclipse, {2}Occultation events.",
                    (DateTimeOffset.Now - now).TotalMilliseconds, planetEventsDict[PlanetId.SE_SUN].Count, count));
                return planetEventsDict;
            }
            else if (CenterFlag == SeFlg.HELIOCENTRIC)
            {
                Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
                List<IPlanetEvent> planetEventList = null, helioList = null;

                if(this.Since < Ephemeris.Geocentric.Since)
                {
                    Position pos = Ephemeris.Geocentric[Since, PlanetId.SE_SUN];
                }
                else if (this.Until > Ephemeris.Geocentric.Until)
                {
                    Position pos = Ephemeris.Geocentric[Until.AddDays(-10), PlanetId.SE_SUN];
                }

                if (Ephemeris.Geocentric.Since <= this.Since && Ephemeris.Geocentric.Until >= this.Until)
                {
                    foreach (PlanetId id in GeocentricLuminaries)
                    {
                        if (id > PlanetId.SE_PLUTO)
                            continue;

                        planetEventList = Ephemeris.Geocentric[Since, Until, PlanetEventFlag.EclipseOccultationCategory, id];
                        if(planetEventList != null && planetEventList.Count != 0)
                        {
                            helioList = new List<IPlanetEvent>();
                            foreach (IPlanetEvent evt in planetEventList)
                            {
                                if(id == PlanetId.SE_SUN || id == PlanetId.SE_MOON)
                                {
                                    if(evt is SolarEclipse)
                                    {
                                        helioList.Add(new SolarEclipse(evt as SolarEclipse, CenterFlag));
                                    }
                                    else if (evt is LunarEclipse)
                                    {
                                        helioList.Add(new LunarEclipse(evt as LunarEclipse, CenterFlag));
                                    }
                                }
                                else
                                {
                                    helioList.Add(new LunarOccultation(evt as LunarOccultation, SeFlg.HELIOCENTRIC));
                                }
                            }

                            planetEventsDict.Add(id == PlanetId.SE_SUN ? PlanetId.SE_EARTH : id, helioList);
                        }
                    }
                    return planetEventsDict;
                }
                return null;
            }
            else
                return null;
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getSignChangeEvents(Dictionary<PlanetId, List<double>> longitudesDict)
        {
            DateTimeOffset now = DateTimeOffset.Now;

            List<int> dateIndexes = null;
            IPlanetEvent theEvent = null;
            Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
            int count = 0;

            for (int index = 0; index < Luminaries.Count; index++)
            {
                PlanetId id = Luminaries[index];
                if (id > PlanetId.SE_FICT_OFFSET)
                    continue;

                dateIndexes = SignChangeDateDetector(longitudesDict[id]);
                List<IPlanetEvent> planetEventList = new List<IPlanetEvent>();

                foreach (int days in dateIndexes)
                {
                    theEvent = signChangeEventOf(id, Since.AddDays(days), CenterFlag);
                    if (theEvent != null)
                    {
                        planetEventList.Add(theEvent);
                        count++;
                    }
                }
                planetEventsDict.Add(id, planetEventList);
            }

            Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} Sign Change events.",
                (DateTimeOffset.Now - now).TotalMilliseconds, count));
            return planetEventsDict;
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getDirectRetrogradeEvents(Dictionary<PlanetId, List<double>> longSpeedsDict)
        {
            if (CenterFlag == SeFlg.GEOCENTRIC)
            {
                DateTimeOffset now = DateTimeOffset.Now;

                List<int> dateIndexes = null;
                Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
                int count = 0;
                Position thePosition = null;

                for (int index = 0; index < Luminaries.Count; index++)
                {
                    PlanetId id = Luminaries[index];
                    if (id == PlanetId.SE_SUN || id == PlanetId.SE_MOON || id > PlanetId.SE_FICT_OFFSET)
                        continue;           // Sun and moon will not retrograde, thus skip

                    dateIndexes = CrossZeroDateDetector(longSpeedsDict[id]);
                    List<IPlanetEvent> planetEventList = new List<IPlanetEvent>();
                    Predicate<Position> isLongitudeNoChangePredicate = Planet.PlanetOf(id).IsHorizontalStationary;

                    foreach (int days in dateIndexes)
                    {
                        DateTimeOffset time = timeOfTurningEvent(id, Since.AddDays(days), isLongitudeNoChangePredicate, 3, out thePosition);
                        if (thePosition != null)
                        {
                            PlanetEventFlag kind = longSpeedsDict[id][days] > 0 ? PlanetEventFlag.Retrograde : PlanetEventFlag.Direct;
                            planetEventList.Add(new RectascensionEvent(id, time, thePosition, kind));
                            count++;
                        }
                    }
                    planetEventsDict.Add(id, planetEventList);
                }

                Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} Direct/Retrograde events.",
                    (DateTimeOffset.Now - now).TotalMilliseconds, count));
                return planetEventsDict;
            }
            else
                return null;
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getAdjacentVisitEvents(Dictionary<PlanetId, List<IPlanetEvent>> directionalEvents)
        {
            if (CenterFlag != SeFlg.GEOCENTRIC)
                return null;

            DateTimeOffset now = DateTimeOffset.Now;

            Dictionary<PlanetId, List<IPlanetEvent>> result = new Dictionary<PlanetId, List<IPlanetEvent>>();
            int count = 0;
            RectascensionEvent revisitEvent, directionalEvent = null;

            foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in directionalEvents)
            {
                PlanetId id = kvp.Key;
                List<IPlanetEvent> planetEventList = new List<IPlanetEvent>();

                foreach (IPlanetEvent evt in kvp.Value)
                {
                    directionalEvent = evt as RectascensionEvent;

                    if (directionalEvent == null || directionalEvent.Kind == PlanetEventFlag.OnEquator)
                        continue;
                    else
                    {
                        revisitEvent = AdjacentVisitOf(id, directionalEvent.Longitude, directionalEvent.When, directionalEvent.Kind == PlanetEventFlag.Direct) as RectascensionEvent;
                        if (revisitEvent != null)
                        {
                            planetEventList.Add(revisitEvent);
                        }
                    }
                }

                if (planetEventList.Count != 0)
                {
                    result.Add(id, planetEventList);
                }
            }

            Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} Adjacent Visit of Direct/Retrograde position events.",
                (DateTimeOffset.Now - now).TotalMilliseconds, count));
            return result;

        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getDeclinationEvents(Dictionary<PlanetId, List<double>> latSpeedsDict, Dictionary<PlanetId, List<double>> latitudesDict)
        {
            DateTimeOffset now = DateTimeOffset.Now;

            List<int> dateIndexes = null;
            IPlanetEvent theEvent = null;
            Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
            int count = 0;
            Position thePosition = null;

            for (int index = 0; index < Luminaries.Count; index++)
            {
                PlanetId id = Luminaries[index];
                if (id == PlanetId.SE_SUN || id > PlanetId.SE_FICT_OFFSET)
                    continue;           // Sun and lunar nodes are skiped

                Predicate<Position> isLatitudeNoChangePredicate = Planet.PlanetOf(id).IsVerticalStationary;

                dateIndexes = CrossZeroDateDetector(latSpeedsDict[id]);
                List<IPlanetEvent> planetEventList = new List<IPlanetEvent>();

                foreach (int days in dateIndexes)
                {
                    DateTimeOffset time = timeOfTurningEvent(id, Since.AddDays(days), isLatitudeNoChangePredicate, 4, out thePosition);
                    if (thePosition != null)
                    {
                        PlanetEventFlag kind = latSpeedsDict[id][days] < 0 ? PlanetEventFlag.SouthMost : PlanetEventFlag.NorthMost;
                        planetEventList.Add(new DeclinationEvent(id, time, thePosition, kind));
                        count++;
                    }
                }

                dateIndexes = CrossZeroDateDetector(latitudesDict[id]);

                foreach (int days in dateIndexes)
                {
                    theEvent = onHorizonEventOf(id, Since.AddDays(days), CenterFlag);
                    if (theEvent != null)
                    {
                        planetEventList.Add(theEvent);
                        count++;
                    }
                    else
                        continue;
                }

                planetEventList.Sort();
                planetEventsDict.Add(id, planetEventList);
            }

            Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} Declination events.",
                (DateTimeOffset.Now - now).TotalMilliseconds, count));
            return planetEventsDict;
        }

        private Dictionary<PlanetId, List<IPlanetEvent>> getAspectFormedEvents(Dictionary<PlanetId, List<double>> longitudesDict, AspectImportance importance)
        {
            Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = new Dictionary<PlanetId, List<IPlanetEvent>>();
            int count = 0;
            DateTimeOffset now = DateTimeOffset.Now;

            List<int> dateIndexes = null;
            IPlanetEvent theEvent = null;
            PlanetId interior, exterior;

            for (int i = 0; i < Luminaries.Count - 1; i++)
            {
                interior = Luminaries[i];
                if (interior == PlanetId.SE_MOON || interior > PlanetId.SE_FICT_OFFSET)
                    continue;           // Moon is not considered here

                List<IPlanetEvent> planetEventList = new List<IPlanetEvent>();

                for (int j = i + 1; j < Luminaries.Count; j++)
                {
                    exterior = Luminaries[j];
                    if (exterior == PlanetId.SE_MOON || exterior > PlanetId.SE_PLUTO)
                        continue;

                    dateIndexes = AspectDateDetector(longitudesDict[interior], longitudesDict[exterior], importance);

                    foreach (int days in dateIndexes)
                    {
                        theEvent = exactAspectEventOf(interior, exterior, Since.AddDays(days), CenterFlag);
                        if (theEvent != null)
                        {
                            planetEventList.Add(theEvent);
                            count++;
                        }
                        else
                            continue;
                    }
                }

                planetEventList.Sort();
                planetEventsDict.Add(interior, planetEventList);
            }

            Debug.WriteLine(string.Format("{0:F1}ms cost to get {1} exact aspect events.",
                (DateTimeOffset.Now - now).TotalMilliseconds, count));
            return planetEventsDict;
        }

        public Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> AllOrbitsCollectionDuring(DateTimeOffset since, DateTimeOffset until)
        {
            if (since.Offset != TimeSpan.Zero || since.TimeOfDay != TimeSpan.Zero)
                since = new DateTimeOffset(since.Year, since.Month, since.Day, 0, 0, 0, TimeSpan.Zero);

            Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> result = new Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>>();

            List<PositionValueIndex> positionIndexes = new List<PositionValueIndex>();

            foreach (PositionValueIndex index in Enum.GetValues(typeof(PositionValueIndex)))
            {
                positionIndexes.Add(index);
                Dictionary<PlanetId, List<double>> orbitDict = new Dictionary<PlanetId, List<double>>();

                foreach (PlanetId id in Luminaries)
                {
                    orbitDict.Add(id, new List<double>());
                }

                result.Add(index, orbitDict);
            }

            TimeSpan step = TimeSpan.FromDays(1);            

            for (DateTimeOffset date = since; date <= until; date += step)
            {
                List<Position> allPlanets = this[date];

                for (int index = 0; index < Luminaries.Count; index++)
                {
                    PlanetId id = Luminaries[index];

                    foreach (PositionValueIndex posIndex in positionIndexes)
                    {
                        result[posIndex][id].Add(allPlanets[index][posIndex]);
                    }
                }
            }

            return result;
        }

        public Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> AspectarianDuring(DateTimeOffset start, DateTimeOffset end, AspectImportance aspectLevel)
        {
            Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> result = new Dictionary<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>>();

            checkFullness(start, end);

            List<IPlanetEvent> events = null;

            foreach (KeyValuePair<PlanetEventFlag, Dictionary<PlanetId, List<IPlanetEvent>>> category in Aspectarian)
            {
                if (category.Key != PlanetEventFlag.AspectCategory)
                {
                    foreach (PlanetId id in Luminaries)
                    {
                        events = this[start, end, category.Key, id];

                        if (events == null || events.Count == 0)
                            continue;

                        if (!result.ContainsKey(category.Key))
                            result.Add(category.Key, new Dictionary<PlanetId, List<IPlanetEvent>>());

                        result[category.Key].Add(id, events);
                    }
                }
                else
                {
                    Dictionary<PlanetId, List<IPlanetEvent>> aspDict = Aspectarian[PlanetEventFlag.AspectCategory];

                    foreach (PlanetId id in Luminaries)
                    {
                        events = this[start, end, id, aspectLevel];

                        if (events == null || events.Count == 0)
                            continue;

                        if (!result.ContainsKey(PlanetEventFlag.AspectCategory))
                            result.Add(PlanetEventFlag.AspectCategory, new Dictionary<PlanetId, List<IPlanetEvent>>());

                        result[PlanetEventFlag.AspectCategory].Add(id, events);
                    }
                }
            }

            return result;
        }

        private void getPlanetEvents()
        {
            #region Preparation daily data
            Debug.WriteLine(String.Format("Start to get planet events during {0} to {1}", Since, Until));

            DateTimeOffset now = DateTimeOffset.Now;
            Debug.WriteLine(String.Format("Start collect position data: ------{0}", now.ToString("hh:mm:ss.fff")));

            Dictionary<PositionValueIndex, Dictionary<PlanetId, List<double>>> orbitsCollection = AllOrbitsCollectionDuring(Since, Until);

            Debug.WriteLine(string.Format("End of collect position data: ------{0}, {1:F1}ms elapsed", DateTimeOffset.Now.ToString("hh:mm:ss.fff"), (DateTimeOffset.Now - now).TotalMilliseconds));
            Debug.WriteLine(string.Format("Totally {0}*{1}*{2} orbit data are retrieved.\r\n", orbitsCollection.Count, orbitsCollection[PositionValueIndex.Longitude].Count,
                orbitsCollection[PositionValueIndex.Longitude][PlanetId.SE_MERCURY].Count));
            now = DateTimeOffset.Now;
            #endregion

            #region Get the Eclipse/Occultation events

            Dictionary<PlanetId, List<IPlanetEvent>> planetEventsDict = getOccultationEvents();
            if (planetEventsDict != null)
                Aspectarian.Add(PlanetEventFlag.EclipseOccultationCategory, planetEventsDict);

            #endregion

            #region Find the Sign Change events

            planetEventsDict = getSignChangeEvents(orbitsCollection[PositionValueIndex.Longitude]);
            Aspectarian.Add(PlanetEventFlag.SignChangedCategory, planetEventsDict);

            #endregion

            #region Find the Longitude Direction Change events

            planetEventsDict = getDirectRetrogradeEvents(orbitsCollection[PositionValueIndex.LongitudeVelocity]);

            if (planetEventsDict != null)
                Aspectarian.Add(PlanetEventFlag.DirectionalCategory, planetEventsDict);

            #endregion

            #region Find the Longitude Transcension events

            //planetEventsDict = getAdjacentVisitEvents(Aspectarian[PlanetEventFlag.DirectionalCategory]);

            //foreach (KeyValuePair<PlanetId, List<IPlanetEvent>> kvp in Aspectarian[PlanetEventFlag.DirectionalCategory])
            //{
            //    kvp.Value.AddRange(planetEventsDict[kvp.Key]);
            //    kvp.Value.Sort();
            //}

            #endregion

            #region Find the Latitude Direction Change events

            planetEventsDict = getDeclinationEvents(orbitsCollection[PositionValueIndex.LatitudeVelocity], orbitsCollection[PositionValueIndex.Latitude]);

            Aspectarian.Add(PlanetEventFlag.DeclinationCategory, planetEventsDict);

            #endregion

            #region Find the Longitude Aspect events

            planetEventsDict = getAspectFormedEvents(orbitsCollection[PositionValueIndex.Longitude], AspectImportance.Minor);

            Aspectarian.Add(PlanetEventFlag.AspectCategory, planetEventsDict);

            #endregion

        }

        private void save()
        {
            string fileName = Name + ".eph";

            FileInfo[] files = EphemerisDirectory.GetFiles(fileName, SearchOption.AllDirectories);

            if (files.Length != 0)
                throw new Exception("Ephemeris already existed: " + fileName + " in " + EphemerisDirectory.Name);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(EphemerisDirectory + fileName, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, this);
            stream.Close();

        }

        /// <summary>
        /// To get the time when a specific planet event is exactly happening.
        /// </summary>
        /// <param name="id1">Concerned star.</param>
        /// <param name="theDay">The theDay when this event is expected.</param>
        /// <param name="criteriaCheckDel">Delegate to check if the dif meeting predefined criteria.</param>
        /// <param name="speedIndex">Index of the Position's value containing the star's speed.</param>
        /// <returns>The time in UTC when the event happen.</returns>
        private DateTimeOffset timeOfTurningEvent(PlanetId id, DateTimeOffset date, Predicate<Position> criteriaCheckDel, int speedIndex, out Position thePos)
        {
            double jul_ut = ToJulianDay(date);
            double percentage, duration = 1;
            int tryCount = 0;

            Position current, end;
            current = PositionOf(jul_ut, id, CenterFlag);
            end = PositionOf(jul_ut + duration, id, CenterFlag);

            if (isSameDirection(current[speedIndex], end[speedIndex]))
                throw new Exception("No turning detected!");

            do
            {
                if (criteriaCheckDel(current))
                {
                    thePos = current;
                    return UtcFromJulianDay(jul_ut);
                }

                percentage = current[speedIndex] / (current[speedIndex] - end[speedIndex]);
                jul_ut += percentage * duration;

                duration = (1-percentage) * duration;
                current = PositionOf(jul_ut, id, CenterFlag);

            } while (tryCount++ < MaxTryOfGetExactTime);

            Console.WriteLine(String.Format("Failed to get the turning point of {0} event on {1}", Planet.PlanetOf(id), date));

            thePos = null;
            return DateTimeOffset.MaxValue;
        }

        public override string ToString()
        {
            return String.Format("{0}-{1}: {2} records.", Since.ToString("yyyy"), Until.ToString("yyyy"), Count);
        }

        public IPlanetEvent ExactAspectEventOf(PlanetId id1, PlanetId id2, DateTimeOffset date)
        {
            return exactAspectEventOf(id1, id2, date, CenterFlag);
        }

        public List<IPlanetEvent> AspectRecurredEventOf(PlanetPair pair, double refAspectDegree, DateTimeOffset start, DateTimeOffset end, List<double> aspects)
        {
            //if (Math.Abs(refAspectDegree) > 180)
            //    refAspectDegree = Math.Abs(360 - refAspectDegree);
            //else if (refAspectDegree < 0)
            //    refAspectDegree = -refAspectDegree;

            DateTimeOffset firstDay = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, TimeSpan.Zero);
            double next = ToJulianDay(firstDay);
            double last = ToJulianDay(end);

            List<double> longDif = new List<double>();

            do
            {
                longDif.Add(this[next, pair.Interior].Longitude - this[next, pair.Exterior].Longitude);
                next++;
            } while (next < last);

            List<int> dateIndexes = TranscensionDateDetector(longDif, refAspectDegree, aspects);
            List<IPlanetEvent> result = new List<IPlanetEvent>();
            int count = 0;
            IPlanetEvent theEvent = null;

            double dif, aspect, destination;
            foreach (int days in dateIndexes)
            {
                dif = (longDif[days] + 720 - refAspectDegree) % 360;
                if (dif > 355)
                    aspect = 0;
                else
                {
                    aspect = (from asp in aspects
                              orderby Math.Abs(dif - asp)
                              select asp
                                         ).First();
                }

                destination = refAspectDegree + aspect;
                if (Math.Abs(destination - longDif[days]) > 300)
                {
                    destination -= 360 * Math.Round((destination - longDif[days]) / 360);
                }
                //if (destination - longDif[days] > 300)
                //    destination -= 360;
                //else if (destination - longDif[days] < -300)
                //    destination += 360;
 
                theEvent = aspectRecurrenceEventOf(pair, firstDay.AddDays(days), refAspectDegree, CenterFlag, destination);
                if (theEvent != null)
                {
                    result.Add(theEvent);
                    count++;
                }
            }

            return result;
        }

        //public List<IPlanetEvent> AspectRecurredEventOf(PlanetPair pair, double refAspectDegree, DateTimeOffset start, DateTimeOffset end, List<double> aspects)
        //{
        //    if (Math.Abs(refAspectDegree) > 180)
        //        refAspectDegree = Math.Abs(360 - refAspectDegree);
        //    else if (refAspectDegree < 0)
        //        refAspectDegree = -refAspectDegree;

        //    DateTimeOffset firstDay = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, TimeSpan.Zero);
        //    double next = ToJulianDay(firstDay);
        //    double last = ToJulianDay(end);

        //    List<double> longDif = new List<double>();

        //    do
        //    {
        //        longDif.Add(Math.Abs(this[next, pair.Interior].Longitude - this[next, pair.Exterior].Longitude));
        //        next++;
        //    } while (next < last);

        //    List<int> dateIndexes = TranscensionDateDetector(longDif, refAspectDegree, aspects);
        //    List<IPlanetEvent> result = new List<IPlanetEvent>();
        //    int count = 0;
        //    IPlanetEvent theEvent = null;

        //    foreach (int days in dateIndexes)
        //    {
        //        theEvent = aspectRecurrenceEventOf(pair, firstDay.AddDays(days), refAspectDegree, CenterFlag, aspects);
        //        if (theEvent != null)
        //        {
        //            result.Add(theEvent);
        //            count++;
        //        }
        //    }

        //    return result;
        //}

        public List<IPlanetEvent> TranscensionEventOf(PlanetId id, double refRectascension, DateTimeOffset start, DateTimeOffset end, List<double> aspects)
        {
            DateTimeOffset firstDay = new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, TimeSpan.Zero);
            Double next = ToJulianDay(firstDay);
            double last = ToJulianDay(end);

            List<double> longitudes = new List<double>();

            do 
            {
                longitudes.Add(this[next, id].Longitude);
                next++;
            } while (next < last);


            List<int> dateIndexes = TranscensionDateDetector(longitudes, refRectascension, aspects);
            List<IPlanetEvent> result = new List<IPlanetEvent>();
            int count = 0;
            IPlanetEvent theEvent = null;

            foreach (int days in dateIndexes)
            {
                theEvent = aspectedTranscensionEventOf(id, firstDay.AddDays(days), refRectascension, CenterFlag, aspects);
                if (theEvent != null)
                {
                    result.Add(theEvent);
                    count++;
                }
            }

            return result;
        }

        public List<IPlanetEvent> TranscensionEventOf(PlanetId id, double refRectascension, DateTimeOffset start, DateTimeOffset end)
        {
            List<double> aspects = Aspect.DegreesOf(AspectImportance.Important);

            if (id <= PlanetId.SE_MARS)
                aspects = Aspect.DegreesOf(AspectImportance.Critical);
            else if (id >= PlanetId.SE_URANUS)
                aspects = Aspect.DegreesOf(7.5);
            else if (id >= PlanetId.SE_SATURN)
                aspects = Aspect.DegreesOf(AspectImportance.Minor);

            return TranscensionEventOf(id, refRectascension, start, end, aspects);
        }

        public IPlanetEvent AdjacentVisitOf(PlanetId id, double refRectascension, DateTimeOffset eventDate, bool isLookingBack)
        {
            int halfOrbitPeriod = (int)(Planet.PlanetOf(id).OrbitalPeriod / 2);

            List<double> longitudes = new List<double>();

            DateTimeOffset theDay = new DateTimeOffset(eventDate.Year, eventDate.Month, eventDate.Day, 0, 0, 0, TimeSpan.Zero);

            if (isLookingBack)
                theDay += TimeSpan.FromDays(-halfOrbitPeriod);
            else
                theDay += TimeSpan.FromDays(5);

            double startDayValue = ToJulianDay(theDay);
            double theLongitude;

            for (int i = 5; i <= halfOrbitPeriod; i ++ )
            {
                theLongitude = PositionOf(startDayValue + i, id, CenterFlag).Longitude;
                longitudes.Add(theLongitude);
            }

            List<int> dateIndexes = TranscensionDateDetector(longitudes, refRectascension, Aspect.CriticalAspectDegrees);

            if (dateIndexes.Count == 0)
                throw new Exception(String.Format("Failed to get the {0} visit of {1} to {2:F2}", isLookingBack?"last":"next", Planet.Glyphs[id], refRectascension) );

            IPlanetEvent theEvent = aspectedTranscensionEventOf(id, theDay.AddDays(dateIndexes[0]), refRectascension, CenterFlag, Aspect.CriticalAspectDegrees);

            if (theEvent != null && theEvent is TranscensionEvent)
            {
                TranscensionEvent transEvent = theEvent as TranscensionEvent;

                return new RectascensionEvent(id, transEvent.When, transEvent.Where, isLookingBack ? PlanetEventFlag.PassDirectPoint : PlanetEventFlag.RevisitRetrogradePoint);
            }
            else
                throw new Exception(String.Format("Failed to get the {0} visit of {1} to {2:F2}", isLookingBack ? "last" : "next", Planet.Glyphs[id], refRectascension));

        }

        #endregion

        //public static void LoadEphemeris()
        //{
        //    string fileName = null;

        //    foreach (FileInfo fi in dir.GetFiles())
        //    {
        //        if (fi.Name.ToLower() != "geocentric.eph")
        //            continue;

        //        fileName = fi.FullName;
        //    }

        //    if (fileName != null)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        Geocentric = (Ephemeris)formatter.Deserialize(stream);
        //        stream.Close();
        //    }

        //    if (Geocentric == null)
        //    {
        //        Geocentric = new Ephemeris("Geocentric");
        //    }
        //}

        //public static DateTimeOffset ExactlyTimeOf(PlanetId id1, DateTimeOffset refDate, Rectascension aspectDegree)
        //{
        //    double SolarEclipseAround = ToJulianDay(refDate);
        //    double averageSpeed = AverageSpeedOf(id1);
        //    Where posAverage, posDynamic = GeocentricPositionOf(SolarEclipseAround, id1, SeFlg.SEFLG_SPEED);

        //    double shiftDynamic, shiftAverage;
        //    double dest = aspectDegree.Degrees;

        //    double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);

        //    if (distanceDynamic < 0) distanceDynamic += 360;

        //    double lastDistance = 360, lastShift, minDistance, distanceAverage = distanceDynamic;

        //    shiftDynamic = distanceDynamic / (posDynamic.LongitudeVelocity < 0 ? averageSpeed : posDynamic.LongitudeVelocity);
        //    shiftAverage = Math.Abs(distanceAverage / averageSpeed);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        posDynamic = GeocentricPositionOf(SolarEclipseAround + shiftDynamic, id1, SeFlg.SEFLG_SPEED);
        //        posAverage = GeocentricPositionOf(SolarEclipseAround + shiftAverage, id1, SeFlg.GEOCENTRIC);

        //        distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);
        //        distanceAverage = Angle.BeelineOf(posAverage.Longitude.Degrees, dest);

        //        minDistance = (Math.Abs(distanceAverage) < Math.Abs(distanceDynamic)) ? Math.Abs(distanceAverage) : Math.Abs(distanceDynamic);

        //        if (minDistance > Math.Abs(lastDistance) && minDistance < 8)
        //        {
        //            SolarEclipseAround += 4 * majorStep;
        //            i++;

        //            posDynamic = GeocentricPositionOf(SolarEclipseAround, id1, SeFlg.SEFLG_SPEED);
        //            distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);

        //            shiftDynamic = Math.Abs(distanceDynamic / posDynamic.LongitudeVelocity);
        //            shiftAverage = Math.Abs(distanceDynamic / posDynamic.LongitudeVelocity);

        //            posDynamic = GeocentricPositionOf(SolarEclipseAround + shiftDynamic, id1, SeFlg.SEFLG_SPEED);
        //            posAverage = GeocentricPositionOf(SolarEclipseAround + shiftAverage, id1, SeFlg.GEOCENTRIC);

        //            distanceDynamic = Angle.BeelineOf(posDynamic.Longitude.Degrees, dest);
        //            distanceAverage = Angle.BeelineOf(posAverage.Longitude.Degrees, dest);

        //            if (Math.Abs(distanceAverage) < Math.Abs(distanceDynamic))
        //            {
        //                lastDistance = distanceAverage;
        //                lastShift = shiftAverage;
        //                shiftDynamic = distanceAverage / posAverage.LongitudeVelocity;
        //                shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //            }
        //            else
        //            {
        //                lastDistance = distanceDynamic;
        //                lastShift = shiftDynamic;
        //                shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //                shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //            }
        //        }
        //        else
        //        {
        //            if (Math.Abs(distanceAverage) > Math.Abs(distanceDynamic))
        //            {
        //                if (Math.Abs(distanceDynamic) < Negligible)
        //                    return UtcFromJulianDay(SolarEclipseAround);

        //                SolarEclipseAround += shiftDynamic;
        //                lastShift = shiftDynamic;
        //                distanceAverage = distanceDynamic;
        //                posAverage = posDynamic;
        //                lastDistance = distanceDynamic;
        //            }
        //            else
        //            {
        //                if (Math.Abs(distanceAverage) < Negligible)
        //                    return UtcFromJulianDay(SolarEclipseAround);

        //                SolarEclipseAround += shiftAverage;
        //                lastShift = shiftAverage;
        //                distanceDynamic = distanceAverage;
        //                posDynamic = posAverage;
        //                lastDistance = distanceAverage;

        //            }

        //            shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //            shiftAverage = Math.Abs(distanceAverage / averageSpeed);
        //        }

        //    }

        //    throw new Exception("Failed aspectDegree get the exact time!");
        //}

        //public DateTimeOffset ExactlyTimeOf(PlanetId id1, DateTimeOffset refDate, Rectascension aspectDegree)
        //{
        //    double jul_ut = ToJulianDay(refDate);
        //    double averageSpeed = Planet.AverageSpeedOf(id1);
        //    Where posAverage, posDynamic = PositionOf(jul_ut, id1, CenterFlag);

        //    double shiftDynamic, shiftAverage;
        //    double dest = aspectDegree.Degrees;

        //    double distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);

        //    if (distanceDynamic < 0) distanceDynamic += 360;

        //    double lastDistance = 360, lastShift, distanceAverage = distanceDynamic;

        //    shiftDynamic = distanceDynamic / (posDynamic.LongitudeVelocity < 0 ? averageSpeed : posDynamic.LongitudeVelocity);
        //    shiftAverage = distanceAverage / averageSpeed;

        //    for (int i = 0; i < 10; i++)
        //    {
        //        posDynamic = PositionOf(jul_ut + shiftDynamic, id1, CenterFlag);
        //        posAverage = PositionOf(jul_ut + shiftAverage, id1, CenterFlag);

        //        distanceDynamic = Angle.BeelineOf(posDynamic.Longitude, dest);
        //        distanceAverage = Angle.BeelineOf(posAverage.Longitude, dest);


        //        if (Math.Abs(distanceAverage) > Math.Abs(distanceDynamic))
        //        {
        //            if (Math.Abs(distanceDynamic) < Negligible)
        //                return UtcFromJulianDay(jul_ut);

        //            jul_ut += shiftDynamic;
        //            lastShift = shiftDynamic;
        //            distanceAverage = distanceDynamic;
        //            posAverage = posDynamic;
        //            lastDistance = distanceDynamic;
        //        }
        //        else
        //        {
        //            if (Math.Abs(distanceAverage) < Negligible)
        //                return UtcFromJulianDay(jul_ut);

        //            jul_ut += shiftAverage;
        //            lastShift = shiftAverage;
        //            distanceDynamic = distanceAverage;
        //            posDynamic = posAverage;
        //            lastDistance = distanceAverage;

        //        }

        //        shiftDynamic = distanceDynamic / posDynamic.LongitudeVelocity;
        //        shiftAverage = distanceAverage / averageSpeed;

        //    }

        //    throw new Exception("Failed aspectDegree get the exact time!");
        //}

        //public void Load(DateTimeOffset eventDate, DateTimeOffset end)
        //{
        //    int startYear = 10 * (eventDate.Year / 10);
        //    int endYear = 10 * (end.Year / 10);

        //    for (; startYear <= endYear; startYear += 10)
        //    {
        //        DateTimeOffset theDay = new DateTimeOffset(startYear, 1, 1, 0, 0, 0, TimeSpan.Zero);

        //        if (!Buffer.ContainsKey(theDay))
        //        {
        //            Ephemeris decade = decadeEphemerisOf(theDay);
        //            this.merge(decade);
        //            theDay = decade.Until.AddDays(1);
        //        };
        //    }
        //}
        //private Ephemeris(string name, PositionLookupDelegate positionDel) 
        //{
        //    Buffer = new SortedDictionary<DateTimeOffset, List<Where>>();
        //    Name = name;
        //    PositionDelegate = positionDel;
        //}

        //private Ephemeris(string name) : this (name, new PositionLookupDelegate(GeocentricPositionOfJulian))
        //{ }

        //private Ephemeris(string name, DateTimeOffset theDay) : this(name)
        //{
        //    Ephemeris decade = decadeEphemerisOf(theDay);
        //    this.merge(decade);
        //}

        //private Ephemeris(string name, PositionLookupDelegate positionDel, DateTimeOffset theDay)
        //    : this(name, positionDel)
        //{
        //    Ephemeris decade = decadeEphemerisOf(theDay);
        //    this.merge(decade);
        //}


    }
}
