using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberHelper
{
    public class RegularCycle
    {
        #region Properties
        /// <summary>
        /// Number of sides
        /// </summary>
        public int Sides { get; private set; }

        /// <summary>
        /// The Last Index of the cycle
        /// </summary>
        public int Last { get; private set; }

        /// <summary>
        /// The index number within the cycle
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The degree of the first vertex
        /// </summary>
        public double OffsetDegree {get; private set;}

        public Dictionary<int, Angle> Vertices { get; private set; }

        public int SideLength { get { return Length / Sides; } }

        /// <summary>
        /// First Index of the cycle
        /// </summary>
        public int First { get { return Last + 1 - Length; } }

        #endregion

        ///// <summary>
        ///// The Degrees of the First index
        ///// </summary>
        //public double EntryDegrees { get; private set; }

        ///// <summary>
        ///// The Degrees of the index of Last+1
        ///// </summary>
        //public double ExitDegrees { get; private set; }

        #region Constructor

        internal RegularCycle(int sides, int last, int length, double offset)
        {
            Sides = sides;
            Last = last;
            Length = length;
            OffsetDegree = offset;
            int sideLength = length / sides;

            Vertices = new Dictionary<int, double>();
            Double sectorDeg = 360.0 / sides;
            int firstVertex = last - (side-1) * sideLength;

            for(int i = 0; i < Sides; i ++)
            {
                Vertices.Add(firstVertex + i * sideLength, new Angle(i * sectorDeg + offset));
            }
        }

        #endregion

        public Angle FromIndex(Double index)
        {
            if (index > Last+1 || index < First)
                throw new ArgumentOutOfRangeException("The index shall within " + (Last+1).ToString() +
                    " and " + First.ToString());

            int due = (from perigee in Perigees
                       orderby Math.Abs(perigee - index)
                       select perigee).First();

            double orientation = Perigees.IndexOf(due) * 90.0;

            if (index == due)
                return orientation;
            else if (index > due)
                return orientation + Math.Atan((index - due) / Round) * DegreesPerRadians;
            else
                return (orientation == 0 ? 360.0 : orientation) - Math.Atan((due - index) / Round) * DegreesPerRadians;

        }

        //internal Double fromDegrees(Double degrees)
        //{
        //    if (Round == 0) return 1.0;

        //    if (Math.IEEERemainder(degrees, 45.0) == 0)
        //    {
        //        return SquareOfEven + (degrees / 45.0 - 7) * Round;
        //    }

        //    double nearAspect = (from aspect in fixedAspects
        //                         where aspect <= degrees
        //                         select aspect).Last();

        //    int orient = fixedAspects.IndexOf(nearAspect);

        //    switch (orient)
        //    {
        //        case 0:
        //        case 2:
        //        case 4:
        //        case 6:
        //            return Perigees[orient / 2] + Math.Tan(Math.Abs(degrees - nearAspect) * RadiansPerDegree) * Round;
        //        case 1:
        //        case 3:
        //        case 5:
        //            return Perigees[(1 + orient) / 2] - Math.Tan(Math.Abs(nearAspect + 45.0 - degrees) * RadiansPerDegree) * Round;
        //        case 7:
        //            if (degrees <= ExitDegrees)
        //                return Perigees[3] + Math.Tan(Math.Abs(degrees - 270.0) * RadiansPerDegree) * Round;
        //            else if (degrees >= EntryDegrees)
        //                return Perigees[0] - Math.Tan((360.0 - degrees) * RadiansPerDegree) * Round;
        //            else
        //                return (EntryDegrees - degrees <= degrees - ExitDegrees) ? Minimum : Maximum;
        //        default:
        //            throw new Exception("Unexpected!");
        //    }


        //    ////if (Degrees > EndDegrees && Degrees < StartDegrees)
        //    ////    throw new ArgumentOutOfRangeException(Degrees.ToString() + " is out of range <" + StartDegrees.ToString() +
        //    ////        ", " + EndDegrees.ToString() + ">");

        //    //if (Math.IEEERemainder(Degrees, 45.0) == 0)
        //    //{
        //    //    int temp = (int)(Degrees / 45.0);
        //    //    return Minimum + temp * Round + Round - 1;
        //    //}
        //    //else
        //    //{
        //    //    int quarter = Degrees <= 45.0 ? 0 : (Degrees <= 135.0 ? 1 : Degrees <= 225.0 ? 2 : Degrees <= EndDegrees ? 3 : 0);
        //    //    int due = Perigees[quarter];

        //    //    if (EndDegrees > 315.0 && Degrees >= EndDegrees)
        //    //    {
        //    //        double deviate = Math.Tan((360.0 - Degrees) * RadiansPerDegree) * Round;
        //    //        return due - deviate;
        //    //    }
        //    //    else if (Degrees >= 270.0)
        //    //    {
        //    //        double deviate = Math.Tan(Math.Abs(Degrees - 270.0) * RadiansPerDegree) * Round;

        //    //        return Maximum - Round - 1 + deviate;
        //    //    }
        //    //    else
        //    //    {
        //    //        double dueDegrees = quarter * 90.0;

        //    //        double deviate = Math.Tan(Math.Abs(Degrees - dueDegrees) * RadiansPerDegree) * Round;

        //    //        if (Degrees < dueDegrees)
        //    //            return due - deviate;
        //    //        else
        //    //            return due + deviate;
        //    //     }
        //    //}
        //}

        //public override string ToString()
        //{
        //    return String.Format("Round {0}: {1}>.>={2}, {3}-{4} ", Round, Maximum, Minimum, EntryDegrees, ExitDegrees);
        //}
    }

}
