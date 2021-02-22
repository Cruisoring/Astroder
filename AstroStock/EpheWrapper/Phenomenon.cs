using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public class Phenomenon
    {
        public const int Decimals = 6;

        #region Fields
        private readonly RelationKind kind;

        private readonly DateTime exactUtcTime;

        private double interiorDegrees, exteriorDegrees;

        #endregion

        #region Properties
        //public RelationKind Kind
        //{
        //    get { return kind; }
        //}

        public AspectType Pattern
        {
            get { return kind.Pattern; }
        }

        public PlanetId InteriorId
        {
            get { return kind.Interior; }
        }        

        public char InteriorSymbol
        {
            get { return Planet.SymbolOf(kind.Interior); }
        }

        public PlanetId ExteriorId
        {
            get { return kind.Exterior; }
        }

        public char ExteriorSymbol
        {
            get { return Planet.SymbolOf(kind.Exterior); }
        }

        public DateTime ExactUtcTime
        {
            get { return exactUtcTime; }
        }

        public double InteriorDegrees
        {
            get { return interiorDegrees; }
        }

        public double ExteriorDegrees
        {
            get { return exteriorDegrees; }
        }

        public Double Angles
        {
            //get { return Math.Round((interiorDegrees - exteriorDegrees + 360) % 360.0, Decimals); }
            get { return (interiorDegrees - exteriorDegrees).Normalize().Round(Decimals); }
        }

        public Double Orb
        {
            get {
                double theAngle = Angles;
                double expected = Aspect.DegreesOf(Pattern);

                return (((theAngle > 180.0) ? 360.0 - expected : expected) - theAngle).Round(Decimals);
            }
        }

        #endregion


        public Phenomenon(DateTime utc, RelationKind theKind, double inDegrees, double exDegrees)
        {
            this.exactUtcTime = utc;
            this.kind = new RelationKind(theKind);
            this.interiorDegrees = inDegrees.Round(Decimals);
            this.exteriorDegrees = exDegrees.Round(Decimals);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}({2}) {3} {4}({5})", exactUtcTime, InteriorSymbol, interiorDegrees, Aspect.SymbolOf(Pattern),
                ExteriorSymbol, exteriorDegrees);
        }
    }

}
