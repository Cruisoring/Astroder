using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    public enum Elements
    {
        Fire,
        Earth,
        Air,
        Water
    }

    public enum Qualities
    {
        Cardinal,
        Fixed,
        Mutable
    }

    public enum Genders
    {
        Male,
        Female
    }

    public enum PlanetId : int
    {
        SE_ECL_NUT = -1,            /// Special body number to compute obliquity and nutation.
        
        SE_SUN = 0,                 /// Index for Sun
        SE_MOON = 1,         // Index for Moon
        SE_MERCURY = 2,         // Index for Mercury
        SE_VENUS = 3,         // Index for Venus
        SE_MARS = 4,         // Index for Mars
        SE_JUPITER = 5,         // Index for Jupiter
        SE_SATURN = 6,         // Index for Saturn
        SE_URANUS = 7,         // Index for Uranus
        SE_NEPTUNE = 8,         // Index for Neptune
        SE_PLUTO = 9,         // Index for Pluto

        SE_MEAN_NODE = 10,         // Index for standard node (mean)
        SE_TRUE_NODE = 11,         // oscillating node (true)
        SE_MEAN_APOG = 12,      //
        SE_OSCU_APOG = 13,

        SE_EARTH = 14,
        SE_CHIRON = 15,         // Index for Chiron
        SE_PHOLUS = 16,
        SE_CERES  = 17,
        SE_PALLAS  = 18,
        SE_JUNO    = 19,
        SE_VESTA   = 20,
        SE_INTP_APOG  = 21,
        SE_INTP_PERG  = 22,

        SE_NPLANETS   = 23,
        SE_FICT_OFFSET  = 40,
        
        //SE_ASC_AS_BODY = 200,         // Index for ascendant to be used during analysis
        //SE_MC_AS_BODY = 201,         // INdex for MC to be used during analysis

        SE_AST_OFFSET  = 10000
    }
}
