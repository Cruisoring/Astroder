using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    /// <summary>
    /// If no bits are set, i.e. if  iflag == 0, swe_calc() computes what common astrological ephemerides (as available in book shops) supply, 
    ///     i.e. an apparent  body position in geocentric ecliptic polar coordinates ( longitude, latitude, and distance) 
    ///     relative to the true equinox of the date. 
    /// For mathematical points as the mean lunar node and the mean apogee, there is no apparent position. 
    /// Swe_calc() returns true positions for these points.
    /// If you need another kind of computation, use the flags explained in the following paragraphs (c.f. swephexp.h). Their names begin with SEFLG_. 
    /// To combine them, you have to concatenate them (inclusive-or) as in the following example:
    /// iflag = SEFLG_SPEED | SEFLG_TRUEPOS;  (or: iflag = SEFLG_SPEED + SEFLG_TRUEPOS;) // C
    /// </summary>
    [Flags]
    public enum SeFlg : int
    {
        SEFLG_JPLEPH    =	1,	            // use JPL ephemeris 
        SEFLG_SWIEPH    =	2,            // use SWISSEPH ephemeris, default
        SEFLG_MOSEPH    =	4,            // use Moshier ephemeris 

        SEFLG_HELCTR     =    	8,        // return heliocentric position 
        SEFLG_TRUEPOS 	 =      16,            // return true positions, not apparent 
        SEFLG_J2000      =    	32,            // no precession, i.e. give J2000 equinox 
        SEFLG_NONUT      =    	64,            // no nutation, i.e. mean equinox of date 
        SEFLG_SPEED3     =    	128,            // speed from 3 positions (do not use it, SEFLG_SPEED is faster and preciser.) 

        SEFLG_SPEED      =   	256,            // high precision speed (analyt. comp.)
        SEFLG_NOGDEFL 	 =      512,            // turn off gravitational deflection 
        SEFLG_NOABERR 	 =      1024,            // turn off 'annual' aberration of light 
        SEFLG_EQUATORIAL =  	2048,            // equatorial positions are wanted 
        SEFLG_XYZ        =    	4096,            // cartesian, not polar, coordinates 
        SEFLG_RADIANS    =   	8192,            // coordinates in radians, not degrees 
        SEFLG_BARYCTR    =   	16384,            // barycentric positions 
        SEFLG_TOPOCTR 	 =      (32*1024),	// topocentric positions 
        SEFLG_SIDEREAL	 =      (64*1024)	// sidereal positions 

    }
}
