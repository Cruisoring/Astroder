using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    /// <summary>
    /// EclipseFlag specifies the eclipse type wanted. It can be a combination of the following bits (see swephexp.h). 
    /// </summary>
    [Flags]
    public enum EclipseFlag : int
    {
        ERR = -1,

        ANY = 0,
        
        CENTRAL = 1,
        NONCENTRAL = 2,
        Total = 4,
        Annular = 8,
        Partial = 16,
        AnnularTotal = 32,
        Penumbral = 64,

        SE_ECL_VISIBLE = 128,
        SE_ECL_MAX_VISIBLE = 256,
        SE_ECL_1ST_VISIBLE = 512,
        SE_ECL_2ND_VISIBLE = 1024,
        SE_ECL_3RD_VISIBLE = 2048,
        SE_ECL_4TH_VISIBLE = 4096,
        SE_ECL_ONE_TRY = 32768,    //32*1024:  check if the degree1 conjunction of the moon with a planet is an occultation; don't search further */

        SE_ECL_ALLTYPES_SOLAR = (CENTRAL | NONCENTRAL | Total | Annular | Partial | AnnularTotal),
        SOLAR_TYPE_MASK = Total | Annular | Partial | AnnularTotal,
        TOTAL_SOLAR_ECLIPSE = Total | CENTRAL | NONCENTRAL,    //search a total eclipse; note: non-central total eclipses are very rare
        ANNULAR_SOLAR_ECLIPSE = Annular | CENTRAL | NONCENTRAL,//search an annular eclipse
        HYBRID_SOLAR_ECLIPSE = AnnularTotal | CENTRAL | NONCENTRAL, //search an annular-total (hybrid) eclipse 

        LUNAR_TYPE_MASK = (Total | Partial | Penumbral),

        OCCULTATION_TYPE_MASK = (Total | Annular | Partial)

    }

    /// <summary>
    /// Contains the properties describing the Eclipses or Occultation.
    /// </summary>
    [Serializable]
    public abstract class Occultation : IPlanetEvent
    {
        #region Property

        public PlanetEventFlag Category { get { return (Kind & PlanetEventFlag.CategoryMask); } }

        public PlanetEventFlag Kind { get; protected set; }

        private SeFlg centric = SeFlg.GEOCENTRIC;

        public SeFlg Centric
        {
            get { return centric; }
            set 
            {
                centric = value;
                if (centric == SeFlg.GEOCENTRIC)
                    Where = Ephemeris.GeocentricPositionOf(Tret[0], Who);
                else
                    Where = Ephemeris.HeliocentricPositionOf(Tret[0], Who);
            }
        }

        public abstract PlanetId Who { get; }

        public double[] GeoPos { get; protected set; }

        public double[] Tret { get; protected set; }

        public double[] Attr { get; protected set; }

        public Position Where { get; set; }

        public String ErrorMessage { get; set; }

        public EclipseFlag Result { get; set; }

        //The time when occultation is maximum
        public DateTimeOffset When { get { return Ephemeris.UtcFromJulianDay(Tret[0]); } }

        public double OADate { get { return When.UtcDateTime.ToOADate(); } }

        //public Position Where { get { return new Position(Who, GeoPos); } }

        public double Longitude { get { return GeoPos[0]; } }

        public double Latitude { get { return GeoPos[1]; } }

        public double Height { get { return GeoPos[2]; } }

        public abstract String ShortDescription { get; }

        #endregion

        #region Constructor
        public Occultation()
        {
            GeoPos = new double[10];
            Tret = new double[10];
            Attr = new double[20];
            Kind = PlanetEventFlag.LunarOccultation;
            ErrorMessage = "";
            Result = EclipseFlag.ERR;
        }

        public Occultation(Occultation another)
            :this()
        {
            another.GeoPos.CopyTo(GeoPos, 0);
            another.Tret.CopyTo(Tret, 0);
            another.Attr.CopyTo(Attr, 0);
            ErrorMessage = another.ErrorMessage;
            Result = another.Result;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0} @ ({1:F3}{2}, {3:F3}{4})", When.ToLocalTime().ToString("MM-dd HH:mm"), 
                Math.Abs(Longitude), Longitude >= 0 ? 'E' : 'W', Math.Abs(Latitude), Latitude >= 0 ? 'N' : 'S');
        }

        public virtual bool IsSimilar(IPlanetEvent another)
        {
            return this.Category == another.Category && this.Who == another.Who;
        }

        public virtual Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(PlanetEventFlag.EclipseOccultationCategory, Who);
        }

        #region IComparable<IPlanetEvent> 成员

        public int CompareTo(IPlanetEvent other)
        {
            if (this.When == other.When)
                return 0;
            else if (this.When > other.When)
                return 1;
            else
                return -1;
        }

        #endregion
    }


    [Serializable]
    public class SolarEclipse : Occultation
    {
        #region Properties
        public override PlanetId Who { get { return Centric == SeFlg.GEOCENTRIC ? PlanetId.SE_SUN : PlanetId.SE_EARTH; }  }

        public DateTimeOffset TimeOfFirstContact { get { return Ephemeris.UtcFromJulianDay(Tret[1]); } }

        public DateTimeOffset TimeOfSecondContact { get { return Ephemeris.UtcFromJulianDay(Tret[2]); } }

        public DateTimeOffset TimeOfThirdContact { get { return Ephemeris.UtcFromJulianDay(Tret[3]); } }

        public DateTimeOffset TimeOfFourthContact { get { return Ephemeris.UtcFromJulianDay(Tret[4]); } }

        public Double FractionCovered { get { return Attr[0]; } }
        public Double LunarDiameterRatio { get { return Attr[1]; } }
        public Double FractionObscuration { get { return Attr[2]; } }
        public Double CoreShadowDiameterInKm { get { return Attr[3]; } }
        public Double SunAzimuth { get { return Attr[4]; } }
        public Double SunTrueAltitude { get { return Attr[5]; } }
        public Double SunApparentAltitude { get { return Attr[6]; } }
        public Double AngularDistance { get { return Attr[7]; } }
        public Double EclipseMagnitude { get { return Attr[8]; } }
        public Double SorosSeriesNumber { get { return Attr[9]; } }
        public Double SorosSeriesMemberNumber { get { return Attr[10]; } }

        public override string ShortDescription
        {
            get
            {
                if (Result == EclipseFlag.ERR)
                    return "Error";
                else
                {
                    //EclipseFlag type = (EclipseFlag.SE_ECL_ALLTYPES_SOLAR & Result) & EclipseFlag.Total & EclipseFlag.Annular & EclipseFlag.Partial & EclipseFlag.AnnularTotal;
                    EclipseFlag type = (EclipseFlag.SOLAR_TYPE_MASK & Result);

                    return type.ToString() + " SE";
                }
            }
        }

        public SolarEclipse() : base() { Kind = PlanetEventFlag.SolarEclipse; }

        public SolarEclipse(SolarEclipse geoSE, SeFlg centric)
            : base(geoSE)
        {
            if (geoSE.Centric != SeFlg.GEOCENTRIC && centric != SeFlg.HELIOCENTRIC)
                throw new Exception();

            Centric = centric;
            Kind = PlanetEventFlag.SolarEclipse;
        }

        //private Position occultedPosition = null;

        //public override Position Where
        //{
        //    get 
        //    {
        //        if (occultedPosition == null && Tret[0] != 0)
        //            occultedPosition = Centric == SeFlg.GEOCENTRIC ? Ephemeris.GeocentricPositionOf(Tret[0], PlanetId.SE_SUN)
        //                : Ephemeris.Heliocentric(Tret[0], PlanetId.SE_EARTH);
        //        return occultedPosition;
        //    }

        //    set
        //    {
        //        occultedPosition = value;
        //    }
        //}

        #endregion

        public override string ToString()
        {
            return String.Format("{0} : {1}, {2}@{3}", ShortDescription, base.ToString(),
                Planet.PlanetOf(Who).Symbol, Where.TheRectascension);
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            return another is SolarEclipse;
        }


        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(PlanetEventFlag.SolarEclipse, PlanetId.SE_SUN);
        }
        
    }

    [Serializable]
    public class LunarEclipse : Occultation
    {
        #region Properties
        public override PlanetId Who { get { return Centric == SeFlg.GEOCENTRIC ? PlanetId.SE_SUN : PlanetId.SE_EARTH; } }

        public DateTimeOffset TimeOfPartialPhaseBegin { get { return Ephemeris.UtcFromJulianDay(Tret[2]); } }
        public DateTimeOffset TimeOfPartialPhaseEnd { get { return Ephemeris.UtcFromJulianDay(Tret[3]); } }
        public DateTimeOffset TimeOfTotalityBegin { get { return Ephemeris.UtcFromJulianDay(Tret[4]); } }
        public DateTimeOffset TimeOfTotalityEnd { get { return Ephemeris.UtcFromJulianDay(Tret[5]); } }
        public DateTimeOffset TimeOfPenumbralBegin { get { return Ephemeris.UtcFromJulianDay(Tret[6]); } }
        public DateTimeOffset TimeOfPenumbralEnd { get { return Ephemeris.UtcFromJulianDay(Tret[7]); } }

        public Double UmbralMagnitude { get { return Attr[0]; } }
        public Double PenumbralMagnitude { get { return Attr[1]; } }
        public Double MoonAzimuth { get { return Attr[4]; } }
        public Double MoonTrueAltitude { get { return Attr[5]; } }
        public Double MoonApparentAltitude { get { return Attr[6]; } }
        public Double MoonDistanceFromOpposition { get { return Attr[7]; } }
        public Double EclipseMagnitude { get { return Attr[8]; } }
        public Double SorosSeriesNumber { get { return Attr[9]; } }
        public Double SorosSeriesMemberNumber { get { return Attr[10]; } }

        public override string ShortDescription
        {
            get
            {
                if (Result == EclipseFlag.ERR)
                    return "Error";
                else
                {
                    EclipseFlag type = EclipseFlag.LUNAR_TYPE_MASK & Result;
                    return type.ToString() + " LE";
                }
            }
        }

        #endregion

        public LunarEclipse() : base() { Kind = PlanetEventFlag.LunarEclipse; }

        public LunarEclipse(LunarEclipse geoLE, SeFlg centric)
            : base(geoLE)
        {
            if (geoLE.Centric != SeFlg.GEOCENTRIC && centric != SeFlg.HELIOCENTRIC)
                throw new Exception();

            Centric = centric;
            Kind = PlanetEventFlag.LunarEclipse;
        }

        public override string ToString()
        {

            return String.Format("{0} : {1}, {2}@{3}", ShortDescription, base.ToString(), Planet.PlanetOf(PlanetId.SE_SUN).Symbol, Where.TheRectascension);
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            return another is LunarEclipse;
        }


        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(PlanetEventFlag.LunarEclipse, PlanetId.SE_MOON);
        }

    }

    [Serializable]
    public class LunarOccultation : Occultation
    {
        #region property

        private PlanetId occulted;
        public override PlanetId Who { get { return occulted; } }

        public DateTimeOffset TimeOfEclipseAtLocal { get { return Ephemeris.UtcFromJulianDay(Tret[1]); } }
        public DateTimeOffset TimeOfEclipseBegin { get { return Ephemeris.UtcFromJulianDay(Tret[2]); } }
        public DateTimeOffset TimeOfEclipseEnd { get { return Ephemeris.UtcFromJulianDay(Tret[3]); } }
        public DateTimeOffset TimeOfTotalityBegin { get { return Ephemeris.UtcFromJulianDay(Tret[4]); } }
        public DateTimeOffset TimeOfTotalityEnd { get { return Ephemeris.UtcFromJulianDay(Tret[5]); } }
        public DateTimeOffset TimeOfCenterLineBegin { get { return Ephemeris.UtcFromJulianDay(Tret[6]); } }
        public DateTimeOffset TimeOfCenterLineEnd { get { return Ephemeris.UtcFromJulianDay(Tret[7]); } }

        public Double FractionCovered { get { return Attr[0]; } }
        public Double LunarDiameterRatio { get { return Attr[1]; } }
        public Double FractionObscuration { get { return Attr[2]; } }
        public Double CoreShadowDiameterInKm { get { return Attr[3]; } }
        public Double SunAzimuth { get { return Attr[4]; } }
        public Double SunTrueAltitude { get { return Attr[5]; } }
        public Double SunApparentAltitude { get { return Attr[6]; } }
        public Double AngularDistance { get { return Attr[7]; } }

        public override string ShortDescription
        {
            get
            {
                if (Result == EclipseFlag.ERR)
                    return "Error";
                else if (Result == 0)
                    return "None";
                else
                {
                    return String.Format("{0}{1}{2}", Planet.Glyphs[Who], '\u2600', Planet.SymbolOf(PlanetId.SE_MOON));
                }
            }
        }

        //private Position occultedPosition = null;

        //public override Position Where
        //{
        //    get
        //    {
        //        if (occultedPosition == null && Tret[0] != 0)
        //            occultedPosition = Centric == SeFlg.GEOCENTRIC ? Ephemeris.GeocentricPositionOf(Tret[0], Who) : Ephemeris.HeliocentricPositionOf(Tret[0], Who);
        //        return occultedPosition;
        //    }

        //    set
        //    {
        //        occultedPosition = value;
        //    }
        //}

        #endregion

        public LunarOccultation(LunarOccultation another, SeFlg centric)
            :base(another)
        {
            if (another.Centric != SeFlg.GEOCENTRIC || centric != SeFlg.HELIOCENTRIC)
                throw new Exception();

            this.occulted = another.occulted;
            Centric = centric;
        }

        public LunarOccultation(PlanetId occulted)
            : base()
        {
            this.occulted = occulted;
        }


        public override string ToString()
        {
            return String.Format("{0}: {1}, {2}@{3}", ShortDescription, base.ToString(), Planet.SymbolOf(Who), Where.TheRectascension);
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            return another is LunarOccultation && this.Who == another.Who;
        }


        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(PlanetEventFlag.LunarOccultation, Who);
        }

    }

}
