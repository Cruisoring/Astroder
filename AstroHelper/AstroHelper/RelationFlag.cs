using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using NumberHelper;

namespace AstroHelper
{
    [Serializable]
    public class RelationFlag : IComparable<RelationFlag>
    {
        #region Static definition
        private static BitVector32.Section superiorMask;       //The bits 20-31 of kind are used destination store the SuperiorId planet id
        private static BitVector32.Section inferiorMask;       //The bits 8 -19 of kind are used destination store the interior planet id
        private static BitVector32.Section aspectTypeMask;     //The bits 1 - 7 bits of kind are used destination store the AspectType between the kind
        private static BitVector32.Section directionMask;      //The bit 0 indicate if the degree is less or greater than 180

        private static BitVector32.Section superiorDirection;   //The bit 7 of detail identify SuperiorId planet movement: 0 for direct, 1 for retrograde
        private static BitVector32.Section inferiorDirection;   //The bit 6 of detail identify InferiorId planet movement: 0 for direct, 1 for retrograde
        private static BitVector32.Section angleExpanding;      //The bit 4 of detail indicates if formed Angle is getting larger or not
        private static BitVector32.Section sameDirection;       //The bit 5 of detail identify if the two planets are of same direction
        private static BitVector32.Section trendMask;           //The bit 4-7 of detail marks the trend types

        private static BitVector32.Section horizontalMask;

        private static BitVector32.Section superiorNorthOrSouth;   //The bit 3 of detail identify SuperiorId planet's latitude: 0 means on north, 1 on south
        private static BitVector32.Section superiorInclination;    //The bit 2 of detail identify SuperiorId planet's inclination: 0 destination north, 1 destination south
        private static BitVector32.Section inferiorNorthOrSouth;    //The bit 1 of detail identify interior planet's latitude: 0 means on north, 1 on south
        private static BitVector32.Section inferiorInclination;    //The bit 0 of detail identify interior planet's inclination: 0 destination north, 1 destination south
        

        static RelationFlag()
        {
            directionMask = BitVector32.CreateSection(1);
            aspectTypeMask = BitVector32.CreateSection(0x7F, directionMask);
            inferiorMask = BitVector32.CreateSection(0xFFF, aspectTypeMask);
            superiorMask = BitVector32.CreateSection(0xFFF, inferiorMask);

            inferiorInclination = BitVector32.CreateSection(1);
            inferiorNorthOrSouth = BitVector32.CreateSection(1, inferiorInclination);
            superiorInclination = BitVector32.CreateSection(1, inferiorNorthOrSouth);
            superiorNorthOrSouth = BitVector32.CreateSection(1, superiorInclination);
            horizontalMask = BitVector32.CreateSection(15, superiorNorthOrSouth);

            sameDirection = BitVector32.CreateSection(1, superiorNorthOrSouth);
            angleExpanding = BitVector32.CreateSection(1, sameDirection);
            inferiorDirection = BitVector32.CreateSection(1, angleExpanding);
            superiorDirection = BitVector32.CreateSection(1, inferiorDirection);
            trendMask = BitVector32.CreateSection(7, superiorNorthOrSouth);
            //reserved = BitVector32.CreateSection(15, superiorDirection);
        }
        #endregion

        private BitVector32 kind;
        private BitVector32 detail;

        #region Porperties
        public int Kind
        {
            get { return kind.Data; }
        }

        public int Detail
        {
            get { return detail.Data; }
        }

        public PlanetId SupperiorId
        {
            get { return (PlanetId)kind[superiorMask]; }
        }

        public PlanetId InferiorId
        {
            get { return (PlanetId)kind[inferiorMask]; }
        }

        public AspectType Type
        {
            get { return (AspectType)kind[aspectTypeMask]; }
        }

        public Boolean Over180
        {
            get { return kind[directionMask] == 1; }
        }

        public Double Around
        {
            get { return Over180 ? 360 - Aspects.AspectAngles[Type].Degrees : Aspects.AspectAngles[Type].Degrees; }
        }

        public int Trend
        {
            get { return detail[trendMask]; }
        }
                
        public bool IsSuperiorRetrograde
        {
            get { return detail[superiorDirection] == 1; }
        }

        public bool IsInteriorRetrograde
        {
            get { return detail[inferiorDirection] == 1; }
        }

        public bool IsSameDirection
        {
            get { return detail[sameDirection] == 1; }
        }

        public bool IsExpanding
        {
            get { return detail[angleExpanding] == 1; }
        }

        public int Horizontal
        {
            get { return detail[horizontalMask]; }
        }

        public bool IsSuperiorOnNorth
        {
            get { return detail[superiorNorthOrSouth] == 0; }
        }

        public bool IsSuperiorNorthward
        {
            get { return detail[superiorInclination] == 0; }
        }

        public bool IsInteriorOnNorth
        {
            get { return detail[inferiorNorthOrSouth] == 0; }
        }

        public bool IsInferiorNorthward
        {
            get { return detail[inferiorInclination] == 0; }
        }

        #endregion

        #region Constructors
        public RelationFlag(Relation relation)
        {
            this.detail = new BitVector32();
            this.kind = new BitVector32();
            kind[superiorMask] = (int)(relation.Superior);
            kind[inferiorMask] = (int)relation.Inferior;
            kind[aspectTypeMask] = (int)relation.Aspect.Kind;
            kind[directionMask] = (relation.Aspect.Degrees > 180) ? 1 : 0;


            detail[superiorDirection] = relation.SuperiorPosition.LongitudeVelocity >= 0 ? 0 : 1;
            detail[inferiorDirection] = relation.InferiorPosition.LongitudeVelocity >= 0 ? 0 : 1;
            detail[sameDirection] = (detail[superiorDirection] == detail[inferiorDirection]) ? 1 : 0;

            Angle current = new Rectascension(relation.InferiorPosition.Longitude - relation.SuperiorPosition.Longitude).Reference;
            Angle next = new Angle(relation.SuperiorPosition.Longitude + relation.SuperiorPosition.LongitudeVelocity
                - relation.InferiorPosition.Longitude - relation.InferiorPosition.LongitudeVelocity).Reference;
            detail[angleExpanding] = (next.Degrees > current.Degrees) ? 1 : 0;

            detail[superiorNorthOrSouth] = relation.SuperiorPosition.Latitude >= 0 ? 0 : 1;
            detail[superiorInclination] = relation.SuperiorPosition.LatitudeVelocity >= 0 ? 1 : 0;
            detail[inferiorNorthOrSouth] = relation.InferiorPosition.Latitude >= 0 ? 0 : 1;
            detail[inferiorInclination] = relation.InferiorPosition.LatitudeVelocity >= 0 ? 1 : 0;
        }
        #endregion

        #region IComparable<RelationFlag> 成员

        public int CompareTo(RelationFlag other)
        {
            //return (this.GeneralType == other.GeneralType) ? (this.Horizontal - other.Horizontal) : (this.GeneralType - other.GeneralType);
            return this.Kind.CompareTo(other.Kind);
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Planet.SymbolOf(SupperiorId), Aspects.Symbols[Type], Planet.SymbolOf(InferiorId));
        }

        #endregion
    }

}
