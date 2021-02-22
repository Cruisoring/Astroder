using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper.DoubleHelper;

namespace NumberHelper
{
    public static class Hexagon
    {
        public const int DefaultCycleNumber = 10;
        public const int DefaultDecimals = 1;

        public static double ApogeeVector = 1;

        public static Dictionary<int, HexagonCycle> Cycles;

        #region Constructor
        static Hexagon()
        {
            ApogeeVector = 2.0/ Math.Sqrt(5.0);
            Cycles = new Dictionary<int,HexagonCycle>();
            for (int i = 1; i < DefaultCycleNumber; i ++)
            {
                Cycles.Add(i, new HexagonCycle(i));
            }
        }
        #endregion

        #region Static functions
        public static int RoundOf(double index)
        {
            if (index < 2)
                return 0;

            int round = (int)Math.Ceiling((Math.Sqrt(index - 1) - 1) / 2);
            return (index >= Cycles[round].Last + 1) ? round + 1 : round;
        }

        private static HexagonCycle cycleOf(double index)
        {
            int num = RoundOf(index);

            if (Cycles.ContainsKey(num))
                return Cycles[num];
            else
            {
                HexagonCycle newCycle = new HexagonCycle(num);
                Cycles.Add(num, newCycle);
                return newCycle;
            }
        }

        public static Double DegreesOf(double index)
        {
            return cycleOf(index).toDegrees(index);
        }

        public static Double IndexOf(int round, double degrees)
        {
            return Cycles[round].fromDegrees(degrees);
        }
        #endregion

        public class HexagonCycle
        {
            private static List<double> fixedAspects = new List<double> { 0, 60.0, 120.0, 180.0, 240.0, 300.0, 360.0 };

            /// <summary>
            /// Round number of the cycle
            /// </summary>
            public int Round { get; private set; }

            /// <summary>
            /// First index of the cycle
            /// </summary>
            public int First { get; private set;}

            /// <summary>
            /// Last index of the cycle
            /// </summary>
            public int Last { get; private set;}

            /// <summary>
            /// The apogees of the cycle
            /// </summary>
            public List<int> Apogees {get; private set;} 

            /// <summary>
            /// The Degrees of the NextIndex slot index
            /// </summary>
            public double EndDegrees { get; private set;}

            /// <summary>
            /// The Degrees of the FirstIndex slot index
            /// </summary>
            public double StartDegrees { get; private set;}

            internal HexagonCycle(int round)
            {
                if (round <= 0)
                {
                    throw new Exception("Invalid argument of Cycle index: " + round.ToString());
                }
                else if (round > 0)
                {
                    Round = round;
                    Last = 3 * round * (round + 1);
                    First = Last + 1 - round * 6;
                    Apogees = new List<int>();
                    for (int i = 0; i <= 6; i++)
                    {
                        Apogees.Add(Last - (6-i) * round);
                    }
                    StartDegrees = toDegrees(First).Normalize();
                    EndDegrees = toDegrees(Last+1).Normalize();
                }
            }

            internal Double toDegrees(Double index)
            {
                if (index > Last+1 || index < First)
                    throw new ArgumentOutOfRangeException("The index shall within " + (1+Last).ToString() + 
                        " and " + First.ToString());

                int due = (from apogee in Apogees
                                   orderby Math.Abs(apogee - index)
                                   select apogee).First();

                int orientation = (Apogees.IndexOf(due)) *60;

                if (index == due)
                    return orientation;
                else if (index > due )
                {
                    if (orientation != 0)
                    {
                        double deviate = Math.Atan((due + Round / 2.0 - index)/ (Round * ApogeeVector) ) * Angle.DegreesPerRadians;
                        return orientation + 30.0 - deviate;
                    }
                    else
                    {                        
                        double deviate = Math.Atan((index + Round/2.0 - due) / (ApogeeVector * Round) ) * Angle.DegreesPerRadians;
                        return orientation + deviate - Round/2.0;
                    }
                }
                else
                {
                    double deviate = Math.Atan((index + Round / 2.0 - due) / (Round * ApogeeVector)) * Angle.DegreesPerRadians;
                    return (orientation == 0 ? 330.0 : orientation - 30) + deviate;
                }

            }

            internal Double fromDegrees(Double degrees)
            {
                if (Math.IEEERemainder(degrees, 30.0) == 0)
                {
                    return Last - (12 - degrees / 30.0) * Round/2.0;
                }

                int sector = (int)(degrees/30.0);

                switch(sector)
                {
                    case 0:
                        double deviate = Math.Tan((degrees + 30) * Angle.RadiansPerDegree) * Round / ApogeeVector;
                        return deviate + Last - Round/2.0;
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                    case 10:
                    case 12:
                        return Apogees[sector / 2] + Round/2.0 - Math.Tan((sector* 30+30 - degrees) * Angle.RadiansPerDegree) * Round / ApogeeVector;
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 9:
                    case 11:
                        return Apogees[(sector+1)/2] - Math.Tan((degrees- sector* 30) * Angle.RadiansPerDegree) * Round / ApogeeVector;
                    default:
                        throw new Exception("Unexpected!");
                }


                ////if (Degrees > EndDegrees && Degrees < StartDegrees)
                ////    throw new ArgumentOutOfRangeException(Degrees.ToString() + " is out of range <" + StartDegrees.ToString() +
                ////        ", " + EndDegrees.ToString() + ">");

                //if (Math.IEEERemainder(Degrees, 45.0) == 0)
                //{
                //    int temp = (int)(Degrees / 45.0);
                //    return FirstIndex + temp * Round + Round - 1;
                //}
                //else
                //{
                //    int quarter = Degrees <= 45.0 ? 0 : (Degrees <= 135.0 ? 1 : Degrees <= 225.0 ? 2 : Degrees <= EndDegrees ? 3 : 0);
                //    int due = Apogees[quarter];

                //    if (EndDegrees > 315.0 && Degrees >= EndDegrees)
                //    {
                //        double deviate = Math.Tan((360.0 - Degrees) * RadiansPerDegree) * Round;
                //        return due - deviate;
                //    }
                //    else if (Degrees >= 270.0)
                //    {
                //        double deviate = Math.Tan(Math.Abs(Degrees - 270.0) * RadiansPerDegree) * Round;

                //        return NextIndex - Round - 1 + deviate;
                //    }
                //    else
                //    {
                //        double dueDegrees = quarter * 90.0;

                //        double deviate = Math.Tan(Math.Abs(Degrees - dueDegrees) * RadiansPerDegree) * Round;

                //        if (Degrees < dueDegrees)
                //            return due - deviate;
                //        else
                //            return due + deviate;
                //     }
                //}
            }

            public override string ToString()
            {
                return String.Format("Round {0}: {1}>.>={2}, {3}-{4} ", Round, Last+1, First, StartDegrees, EndDegrees);
            }
        }
    }
}
