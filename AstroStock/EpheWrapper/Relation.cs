using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpheWrapper
{
    public class Relation
    {
        #region Static Functions
        public static bool HasRelation(PlanetPosition posA, PlanetPosition posB)
        {
            Angle theAngle = posA - posB;
            return Aspect.AspectTypeOf(theAngle) != AspectType.None;
        }

        #endregion

        #region Fields and Properties
        
        //private PlanetPosition interior;

        //public PlanetPosition Interior
        //{
        //    get { return interior; }
        //    //set { interior = value; }
        //}

        //private PlanetPosition exterior;

        //public PlanetPosition Exterior
        //{
        //    get { return exterior; }
        //    //set { exterior = value; }
        //} 
        
        //private readonly Aspect theAspect;

        //public Aspect Pattern
        //{
        //    get { return theAspect; }
        //} 

        private readonly RelationKind kind;

        public RelationKind Kind
        {
            get { return kind; }
        } 

        //public PlanetPair Pair
        //{
        //    get { return kind.Pair; }
        //}

        public PlanetId Interior
        {
            get { return kind.Interior; }
        }

        public PlanetId Exterior
        {
            get { return kind.Exterior; }
        }

        public AspectType Pattern
        {
            get { return kind.Pattern; }
        }

        private readonly double inLongitude;

        public double InLongitude
        {
            get { return inLongitude; }
        }

        private readonly double outLongitude;

        public double OutLongitude
        {
            get { return outLongitude; }
        } 

        private readonly Angle apart;

        public Angle Apart
        {
            get { return apart; }
        }

        public double Orb
        {
            get {
                if (this.Pattern != AspectType.None)
                    return this.apart.Degrees - Aspect.DegreesOf(this.Pattern);
                else
                    return -360;
            }
        }

        #endregion

        #region constructors
        public Relation(PlanetPosition ppA, PlanetPosition ppB)
        {
            this.apart = ppA.Rectascension - ppB.Rectascension;
            AspectType type = Aspect.AspectTypeOf(apart);
            this.kind = new RelationKind(ppA.Id, ppB.Id, type);
            if (ppA.Id <= ppB.Id)
            {
                this.inLongitude = ppA.Rectascension.Degrees;
                this.outLongitude = ppB.Rectascension.Degrees;
            }
            else
            {
                this.inLongitude = ppB.Rectascension.Degrees;
                this.outLongitude = ppA.Rectascension.Degrees;            }
        }

        #endregion

        #region Functions
        public override string ToString()
        {
            return String.Format("{0}({4:F5}) {1} {2}({5:F5}):\tapart={6}, \torb={3:F5}", Planet.PlanetOf(Interior).Symbol, Pattern, 
                Planet.PlanetOf(Exterior).Symbol, this.Orb, this.inLongitude, this.outLongitude, this.apart);
            //return String.Format("{0}({4:F5}) {1} {2}({5:F5}):\t\torb={3:F5}", Planet.PlanetOf(Interior).Symbol, Aspect.AspectOf(Pattern).Symbol,
            //    Planet.PlanetOf(Exterior).Symbol, this.Orb, this.inLongitude, this.outLongitude);
        }

        #endregion

    }
}
