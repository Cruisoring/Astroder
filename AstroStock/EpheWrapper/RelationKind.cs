using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace EpheWrapper
{
    public struct RelationKind : IComparable<RelationKind>
    {
        #region Static definition
        private static BitVector32.Section reserved;       //The lowest 12 bits are reserved for future usage, not used yet
        private static BitVector32.Section aspectType;     //The bits 12-15 are used to store the Aspect Type
        private static BitVector32.Section exterior;       //The bits 16-23 are used to store the exterior planet id
        private static BitVector32.Section interior;       //The bits 24-31 are used to store the interior planet id
        private static BitVector32.Section partner;     //The bits 16-31 together identify the planet pair

        static RelationKind()
        {
            reserved = BitVector32.CreateSection(0xFFF);
            aspectType = BitVector32.CreateSection(15, reserved);
            exterior = BitVector32.CreateSection(255, aspectType);
            interior = BitVector32.CreateSection(255, exterior);
            partner = BitVector32.CreateSection(Int16.MaxValue, aspectType);
        }
        #endregion

        private BitVector32 kind;

        #region Porperties
        public PlanetPair Pair
        {
            get { return new PlanetPair(kind[partner]); }
        }

        public PlanetId Interior
        {
            get { return (PlanetId)kind[interior]; }
        }

        public PlanetId Exterior
        {
            get { return (PlanetId)kind[exterior]; }
        }

        public AspectType Pattern
        {
            get { return (AspectType)kind[aspectType];}
        }

        public int Data
        {
            get { return kind.Data; }
        }
        #endregion

        #region Constructors
        public RelationKind(RelationKind other)
        {
            this.kind = new BitVector32();
            kind[partner] = other.Pair.Code;
            kind[aspectType] = (int)other.Pattern;
        }

        public RelationKind(PlanetId pA, PlanetId pB, AspectType theType)
        {
            this.kind = new BitVector32(0);
            kind[partner] = PlanetPair.CodeOf(pA, pB);
            kind[aspectType] = (int)theType;            
        }
        #endregion

        #region IComparable<RelationFlag> 成员

        public int CompareTo(RelationKind other)
        {
            return this.kind.Data - other.kind.Data;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Planet.SymbolOf(Interior), 
                Aspect.SymbolOf(Pattern), Planet.SymbolOf(Exterior));
        }

        #endregion
    }
}
