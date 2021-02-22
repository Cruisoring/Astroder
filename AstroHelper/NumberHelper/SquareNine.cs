using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberHelper
{
    using NumberHelper.DoubleHelper;

    //public static class SquareNine
    //{
    //    public const int DefaultCycleNumber = 10;
    //    public const int DefaultDecimals = 1;

    //    public static Dictionary<int, SquareCycle> Cycles;

    //    #region Constructor
    //    static SquareNine()
    //    {
    //        Cycles = new Dictionary<int,SquareCycle>();
    //        for (int i = 0; i < DefaultCycleNumber; i ++)
    //        {
    //            Cycles.Add(i, new SquareCycle(i));
    //        }
    //    }
    //    #endregion

    //    #region Static functions
    //    public static int RoundOf(double index)
    //    {
    //        if (index < 2)
    //            return 0;

    //        int round = (int)Math.Ceiling((Math.Sqrt(index - 1) - 1) / 2);
    //        return (index >= Cycles[round].Last + 1) ? round + 1 : round;
    //    }

    //    private static SquareCycle cycleOf(double index)
    //    {
    //        int num = RoundOf(index);

    //        if (Cycles.ContainsKey(num))
    //            return Cycles[num];
    //        else
    //        {
    //            SquareCycle newCycle = new SquareCycle(num);
    //            Cycles.Add(num, newCycle);
    //            return newCycle;
    //        }
    //    }

    //    public static Double DegreesOf(double index)
    //    {
    //        return cycleOf(index).toDegrees(index);//.Round(DefaultDecimals);
    //    }

    //    public static Double IndexOf(int round, double degrees)
    //    {
    //        return Cycles[round].fromDegrees(degrees).Round(DefaultDecimals);
    //    }
    //    #endregion

    //    public class SquareCycle
    //    {
    //        private static List<double> fixedAspects = new List<double> { 0, 45.0, 90.0, 135.0, 180.0, 225.0, 270.0, 315.0 };

    //        /// <summary>
    //        /// Round number of the cycle
    //        /// </summary>
    //        public int Round { get; private set; }

    //        /// <summary>
    //        /// First index of the cycle
    //        /// </summary>
    //        public int First { get; private set;}

    //        /// <summary>
    //        /// The First index of the next cycle
    //        /// </summary>
    //        public int Last { get; private set;}

    //        /// <summary>
    //        /// The slot index of 315 Degrees in this cycle
    //        /// </summary>
    //        public int SquareOfEven { get; private set; }

    //        /// <summary>
    //        /// The four perigees of the cycle in east, south, west and north direction respectively
    //        /// </summary>
    //        public List<int> Perigees {get; private set;} 

    //        /// <summary>
    //        /// The Degrees of the NextIndex slot index
    //        /// </summary>
    //        public double EndDegrees { get; private set;}

    //        /// <summary>
    //        /// The Degrees of the FirstIndex slot index
    //        /// </summary>
    //        public double StartDegrees { get; private set;}

    //        internal SquareCycle(int round)
    //        {
    //            if (round == 0)
    //            {
    //                Round = 0;
    //                First = 1;
    //                Last = 1;
    //                SquareOfEven = 1;
    //                Perigees = new List<int> { 1, 1, 1, 1 };
    //                EndDegrees = 0;
    //                StartDegrees = 45.0;
    //            }
    //            else if (round > 0)
    //            {
    //                Round = round;
    //                int min = 4 * round * round - 4 * round + 2;
    //                First = 4 * round * round - 4 * round + 2;
    //                Last = First + 8 * round -1;
    //                SquareOfEven = (2 * round + 1) * (2 * round + 1);
    //                Perigees = new List<int> { min - 1 + round, min - 1 + 3 * round, min - 1 + 5 * round, min - 1 + 7 * round };
    //                StartDegrees = toDegrees(First);
    //                EndDegrees = toDegrees(Last+1);
    //            }
    //            else
    //                throw new Exception("Invalid argument of Cycle index: " + round.ToString());
    //        }

    //        internal Double toDegrees(Double index)
    //        {
    //            if (Round == 0) return 0;

    //            if (index > Last+1 || index < First)
    //                throw new ArgumentOutOfRangeException("The index shall within " + (1+Last).ToString() + 
    //                    " and " + First.ToString());

    //            int due = (from perigee in Perigees
    //                               orderby Math.Abs(perigee - index)
    //                               select perigee).First();

    //            double orientation = Perigees.IndexOf(due) * 90.0;

    //            if (index == due)
    //                return orientation;
    //            else if (index > due )
    //            {
    //                double tan = (index - due) / Round;
    //                double temp = Math.Atan((index - due) / Round);
    //                tan = Math.Tan(temp);
    //                temp *= Angle.DegreesPerRadians;
    //                return orientation +  temp;
    //                //return orientation + Math.Atan((index - due) / Round) * Angle.DegreesPerRadians;
    //            }
    //            else
    //                return (orientation == 0 ? 360.0 : orientation) - Math.Atan((due - index) / Round) * Angle.DegreesPerRadians;

    //        }

    //        internal Double fromDegrees(Double degrees)
    //        {
    //            if (Round == 0) return 1.0;

    //            if (Math.IEEERemainder(degrees, 45.0) == 0)
    //            {
    //                return SquareOfEven + (degrees / 45.0 - 7) * Round;
    //            }

    //            double nearAspect = (from aspect in fixedAspects
    //                                 where aspect <= degrees
    //                                 select aspect).Last();

    //            int orient = fixedAspects.IndexOf(nearAspect);

    //            switch(orient)
    //            {
    //                case 0:
    //                case 2:
    //                case 4:
    //                case 6:
    //                    //temp = Math.Abs(angle - nearAspect) * Angle.RadiansPerDegree;
    //                    //temp = Math.Tan(temp);
    //                    //return Perigees[orient / 2] + temp * Round;

    //                    return Perigees[orient / 2] + Math.Tan(Math.Abs(degrees - nearAspect) * Angle.RadiansPerDegree) * Round;
    //                case 1:
    //                case 3:
    //                case 5:
    //                    //temp = Math.Abs(nearAspect+45 - angle) * Angle.RadiansPerDegree;
    //                    //temp = Math.Tan(temp);
    //                    //return Perigees[orient / 2] - temp * Round;

    //                    return Perigees[(1+orient) / 2] - Math.Tan(Math.Abs(nearAspect + 45.0 - degrees) * Angle.RadiansPerDegree) * Round;
    //                case 7:
    //                    if (degrees <= EndDegrees)                            
    //                        return Perigees[3] + Math.Tan(Math.Abs(degrees - 270.0) * Angle.RadiansPerDegree) * Round;
    //                    else if (degrees >= StartDegrees)
    //                        return Perigees[0] - Math.Tan((360.0 - degrees) * Angle.RadiansPerDegree) * Round;
    //                    else
    //                        return (StartDegrees-degrees <= degrees - EndDegrees) ? First : Last+1;
    //                default:
    //                    throw new Exception("Unexpected!");
    //            }


    //            ////if (Degrees > EndDegrees && Degrees < StartDegrees)
    //            ////    throw new ArgumentOutOfRangeException(Degrees.ToString() + " is out of range <" + StartDegrees.ToString() +
    //            ////        ", " + EndDegrees.ToString() + ">");

    //            //if (Math.IEEERemainder(Degrees, 45.0) == 0)
    //            //{
    //            //    int temp = (int)(Degrees / 45.0);
    //            //    return FirstIndex + temp * Round + Round - 1;
    //            //}
    //            //else
    //            //{
    //            //    int quarter = Degrees <= 45.0 ? 0 : (Degrees <= 135.0 ? 1 : Degrees <= 225.0 ? 2 : Degrees <= EndDegrees ? 3 : 0);
    //            //    int due = Apogees[quarter];

    //            //    if (EndDegrees > 315.0 && Degrees >= EndDegrees)
    //            //    {
    //            //        double deviate = Math.Tan((360.0 - Degrees) * RadiansPerDegree) * Round;
    //            //        return due - deviate;
    //            //    }
    //            //    else if (Degrees >= 270.0)
    //            //    {
    //            //        double deviate = Math.Tan(Math.Abs(Degrees - 270.0) * RadiansPerDegree) * Round;

    //            //        return NextIndex - Round - 1 + deviate;
    //            //    }
    //            //    else
    //            //    {
    //            //        double dueDegrees = quarter * 90.0;

    //            //        double deviate = Math.Tan(Math.Abs(Degrees - dueDegrees) * RadiansPerDegree) * Round;

    //            //        if (Degrees < dueDegrees)
    //            //            return due - deviate;
    //            //        else
    //            //            return due + deviate;
    //            //     }
    //            //}
    //        }

    //        public override string ToString()
    //        {
    //            return String.Format("Round {0}: {1}>.>={2}, {3}-{4} ", Round, Last+1, First, StartDegrees, EndDegrees);
    //        }
    //    }

    //}

    //public class SquareCycle
    //{
    //    public const int DefaultCycleNumber = 10;
    //    public const int DefaultDecimals = 1;
    //    public static Dictionary<int, SquareCycle> Cycles;

    //    #region Constructor
    //    static SquareCycle()
    //    {
    //        Cycles = new Dictionary<int,SquareCycle>();
    //        for (int i = 0; i < DefaultCycleNumber; i ++)
    //        {
    //            Cycles.Add(i, new SquareCycle(i));
    //        }
    //    }
    //    #endregion

    //    #region Static functions
    //    public static int RoundOf(double index)
    //    {
    //        if (index < 2)
    //            return 0;

    //        return (int)Math.Ceiling((Math.Sqrt(index-1)-1)/2);
    //    }

    //    public static SquareCycle cycleOf(double index)
    //    {
    //        int num = RoundOf(index);

    //        if (Cycles.ContainsKey(num))
    //            return Cycles[num];
    //        else
    //        {
    //            SquareCycle newCycle = new SquareCycle(num);
    //            Cycles.Add(num, newCycle);
    //            return newCycle;
    //        }
    //    }

    //    public static int MaximumAround(double index)
    //    {
    //        return cycleOf(index).NextIndex;
    //    }

    //    public static int MinimumAround(double index)
    //    {
    //         return cycleOf(index).FirstIndex;
    //    }

    //    public static Double DegreesOf(double index)
    //    {
    //        return cycleOf(index).toDegrees(index).Round(DefaultDecimals);
    //    }
    //    #endregion

    //    public int Round { get; private set; }

    //    private SquareCycle(int index)
    //    {
    //        Round = index;
    //        //this.Corners = new List<int>();
    //        //int last = 2 * index + 1;
    //        //last = last * last;
    //        //for (int i = 7; i >= 0; i--)
    //        //{
    //        //    this.Corners.Add(last - i * index);
    //        //}
    //    }

    //    public int FirstIndex
    //    {
    //        get { return Round == 0 ? 0: 4 * Round * Round - 4 * Round + 2; }
    //    }

    //    public int NextIndex
    //    {
    //        get { return 4 * Round * Round + 4 * Round + 2; }
    //    }

    //    private Double toDegrees(Double index)
    //    {
    //        if (Round == 0) return 0;

    //        int min = FirstIndex;
    //        int quarter = (int)((index - min + 1) / (2 * Round));

    //        int due = Round + min - 1 + 2 * Round * quarter;

    //        if (index == due)
    //            return quarter * 90.0;
    //        else if (index > due)
    //        {
    //            double angle = Math.Atan((index-due)/Round)* (180/Math.PI);
    //            return quarter * 90.0 + angle;
    //        }
    //        else
    //        {
    //            double angle = Math.Atan((due - index) / Round) * (180 / Math.PI);
    //            return (quarter == 0 ? 360 : quarter * 90.0)- angle;
    //        }
    //    }
    //}

    //public class SquareNineUser
    //{
    //    #region Fields
    //    public double Start { get; private set; }
    //    public double Step { get; private set; }
    //    #endregion

    //    #region Constructor
    //    public SquareNineUser(double start, double step)
    //    {
    //        if (step == 0)
    //            throw new ArgumentNullException("Step cannot be 0.");
    //        Step = step;
    //        Start = start;
    //    }
    //    #endregion

    //    #region Functions
    //    public double this[int index]
    //    {
    //        get { return Start + (index - 1) * Step; }
    //    }

    //    public double IndexOf(double original)
    //    {
    //        return (original - Start) / Step + 1;
    //    }

    //    public int cycleOf(int index)
    //    {
    //        return (int)Math.Ceiling(Math.Sqrt(index)) - 1;
    //    }

    //    public int cycleOf(double original)
    //    {
    //        int index = (int)Math.Ceiling(IndexOf(original));
    //        return cycleOf(index);
    //    }

    //    #endregion
    //}
}
