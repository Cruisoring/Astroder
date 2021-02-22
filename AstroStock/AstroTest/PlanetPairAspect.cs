using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpheWrapper;

namespace AstroTest
{
    public class PlanetPairAspect
    {
        private DateTime utcMoment;

        public DateTime UtcMoment
        {
            get { return utcMoment; }
            set { utcMoment = value; }
        }

        private PlanetPosition interiorPos;

        public char Interior
        {
            get { return interiorPos.Owner.Symbol; }
        }

        //public PlanetId InteriorPlanet
        //{
        //    get { return interiorPos.Id; }
        //}

        //public Longitude InteriorLongitude
        //{
        //    get { return interiorPos.Rectascension; }
        //}

        public double InteriorDegrees
        {
            get { return interiorPos.Rectascension.Degrees.Round(6); }
        }

        public Angle InteriorSpeed
        {
            get { return interiorPos.LongVelo; }
        }
        
        private PlanetPosition exterirorPos;

        public char Exterior
        {
            get { return exterirorPos.Owner.Symbol; }
        }

        //public PlanetId ExteriorPlanet
        //{
        //    get { return exterirorPos.Id; }
        //}

        //public Longitude ExteriorLongitude
        //{
        //    get { return exterirorPos.Rectascension; }
        //}

        public double ExteriorDegrees
        {
            get { return exterirorPos.Rectascension.Degrees.Round(6); }
        }
        
        public Angle ExteriorSpeed
        {
            get { return exterirorPos.LongVelo; }
        }

        public Double Apart
        {
            get { return Math.Round((interiorPos.Rectascension.Degrees - exterirorPos.Rectascension.Degrees).Normalize(180), 6); }
        }

        private Relation theRelation;

        public AspectType Pattern
        {
            get { return (theRelation == null) ? AspectType.None : theRelation.Pattern; }
        }

        public RelationKind Kind
        {
            get { return new RelationKind(interiorPos.Id, exterirorPos.Id, Pattern); }
        }

        public Double Orb
        {
            get { return (theRelation == null) ? -360 : Math.Round(theRelation.Orb, 6); }            
        }

        public PlanetPairAspect(DateTime utc, PlanetId inStar, PlanetId exStar)
        {
            utcMoment = utc;
            Double jul_ut = SweWrapper.ToJulianDay(utc);

            interiorPos = SweWrapper.PositionOf(jul_ut, inStar, SeFlg.SEFLG_SPEED);
            exterirorPos = SweWrapper.PositionOf(jul_ut, exStar, SeFlg.SEFLG_SPEED);

            if (Relation.HasRelation(interiorPos, exterirorPos))
            {
                theRelation = new Relation(interiorPos, exterirorPos);
            }
        }

    }
}
