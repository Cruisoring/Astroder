using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper.DoubleHelper;


namespace NumberHelper
{
    public enum AspectType
    {
        None = -1,

        Conjuction = 0,
        Sextile = 1,
        Square = 2,
        Trine = 3,
        Opposition = 4,

        Quintile = 11,
        Biquintile = 12,
        Quincunx = 13,
        SemiSquare = 14,
        SemiSextile = 15,
        Sesquiquadrate = 16
    }

    public class Aspects : Angle
    {
        #region Static properties or functions
        //public static bool ConcernsMajorOnly = false;
        public static SortedDictionary<double, Aspects> MajorDegrees;

        public const double DefaultMaxOrb = 1.0;

        public const double Negligible = 0.00001;

        #region Aspect detail definitions
        public static Dictionary<AspectType, double> AspectOrbs = new Dictionary<AspectType, double>() {
            { AspectType.Conjuction, 2}, 
            { AspectType.Sextile, DefaultMaxOrb}, 
            { AspectType.Square, 2}, 
            { AspectType.Trine, 2},
            { AspectType.Opposition, DefaultMaxOrb},
            { AspectType.Quintile, DefaultMaxOrb},
            { AspectType.Biquintile, DefaultMaxOrb},
            { AspectType.Quincunx, DefaultMaxOrb},
            { AspectType.SemiSquare, DefaultMaxOrb},
            { AspectType.SemiSextile, DefaultMaxOrb},
            { AspectType.Sesquiquadrate, DefaultMaxOrb}
        };

        public static Dictionary<AspectType, Angle> AspectAngles = new Dictionary<AspectType, Angle>()
        {
            { AspectType.Conjuction, Angle.Zero },
            { AspectType.SemiSextile, Angle.SemiSextile },
            { AspectType.SemiSquare, Angle.SemiSquare },
            { AspectType.Sextile, Angle.Sextile },
            { AspectType.Quintile, Angle.Quintile },
            { AspectType.Square, Angle.Square },
            { AspectType.Trine, Angle.Trine },
            { AspectType.Sesquiquadrate, Angle.Sesquiquadrate },
            { AspectType.Biquintile, Angle.Biquintile },
            { AspectType.Quincunx,  Angle.Quincunx },
            { AspectType.Opposition, Angle.Opposition }//,
        };

        public static Dictionary<AspectType, Char> Symbols = new Dictionary<AspectType, char>()
        {
            //{ AspectType.None, '\uFF1F'},
            { AspectType.None, '\u26D4'},
            { AspectType.Conjuction, '\u260C'},
            { AspectType.Sextile, '\u26B9'},
            { AspectType.Square, '\u25A1'},
            { AspectType.Trine, '\u25B3'},
            { AspectType.Opposition, '\u260D'},
            { AspectType.Quintile, '\u0051'},
            { AspectType.Biquintile, 'B'},
            { AspectType.Quincunx, '\u26BB'},
            { AspectType.SemiSquare, '\u2220'},
            { AspectType.SemiSextile, '\u26BA'},
            { AspectType.Sesquiquadrate, '\u26BC'}
        };
        #endregion

        //public static SortedDictionary<double, Aspects> DegreesConcerned;

        static Aspects()
        {
            List<Double> defaultConcerns = new List<Double>() { 0, 45, 60, 90, 120, 135, 180, 225, 240, 270, 300, 315, 360 };
            MajorDegrees = GetAspectsDictionary(defaultConcerns);
        }

        public static SortedDictionary<Double, Aspects> GetAspectsDictionary(ICollection<Double> concernedDegrees)
        {
            SortedDictionary<Double, Aspects> DegreesConcerned = new SortedDictionary<Double, Aspects>();

            double maxOrb = 0;
            AspectType type = AspectType.None;

            foreach (double degrees in concernedDegrees)
            {
                if (Angle.References.ContainsKey(degrees))
                {
                    type = Angle.References[degrees];
                    maxOrb = AspectOrbs.ContainsKey(type) ? AspectOrbs[type] : DefaultMaxOrb;
                }
                else
                {
                    type = AspectType.None;
                    maxOrb = DefaultMaxOrb;
                }

                if (!DegreesConcerned.ContainsKey(degrees))
                    DegreesConcerned.Add(degrees, new Aspects((degrees > 360 ? degrees % 360 : degrees), type, maxOrb));
            }

            return DegreesConcerned;
        }

        public static Aspects NearestAspect(SortedDictionary<Double, Aspects> DegreesConcerned, Angle angle)
        {
            IEnumerable<double> aspectQuery =
                from deg in DegreesConcerned.Keys
                let dev = angle.Degrees - deg
                where dev <= 45 && dev >= -45
                orderby Math.Abs(dev)
                select deg;

            double nearTo = (aspectQuery).First();
            return DegreesConcerned[nearTo];
        }

        public static Aspects NearestAspect(Angle angle)
        {
            return NextAspectOf(MajorDegrees, angle);
        }

        public static Aspects CurrentAspectOf(SortedDictionary<Double, Aspects> DegreesConcerned, Angle angle)
        {
            double degree = angle.Degrees.Normalize();

            IEnumerable<double> aspectQuery =
                from refDeg in DegreesConcerned.Keys
                let dev = Math.Abs(degree - refDeg)
                orderby dev
                select refDeg;

            double reference = (aspectQuery).First();
            return Math.Abs(reference - degree) <= DegreesConcerned[reference].PermissibleOrb ? DegreesConcerned[reference == 360 ? 0 : reference] : null;

        }

        public static Aspects CurrentAspectOf(Angle angle)
        {
            return CurrentAspectOf(MajorDegrees, angle);
        }

        public static bool HasAspectBetween(SortedDictionary<Double, Aspects> DegreesConcerned, Angle start, Angle end)
        {
            return CurrentAspectOf(DegreesConcerned, end - start) != null;
        }

        public static bool HasAspectBetween(Angle start, Angle end)
        {
            return CurrentAspectOf(MajorDegrees, end - start) != null;
        }

        public static Aspects AspectBetween(SortedDictionary<Double, Aspects> DegreesConcerned, Angle start, Angle end)
        {
            return CurrentAspectOf(DegreesConcerned, end - start);
        }

        public static Aspects AspectBetween(Angle start, Angle end)
        {
            return CurrentAspectOf(MajorDegrees, end - start);
        }

        public static Aspects NextAspectOf(SortedDictionary<Double, Aspects> DegreesConcerned, Angle angle)
        {
            Double degrees = (angle.Degrees > 360-Negligible) ? 0 : (angle.Degrees.Normalize());
            var followingQuery =
                from KeyValuePair<double, Aspects> kvp in DegreesConcerned
                where kvp.Key > degrees 
                orderby kvp.Key - degrees
                select kvp.Value;

            List<Aspects> aspects = followingQuery.ToList();

            return aspects.First();
        }

        public static Aspects NextAspectOf(Angle angle)
        {
            return NextAspectOf(MajorDegrees, angle);
        }

        public static Aspects PreviousAspectOf(SortedDictionary<Double, Aspects> DegreesConcerned, Angle angle)
        {
            //Double degrees = (angle.Degrees % 360 == 0) ? 360 : angle.Degrees.Normalize();
            Double degrees = (angle.Degrees <= Negligible) ? 360 : angle.Degrees.Normalize();

            var previousQuery =
                from KeyValuePair<double, Aspects> kvp in DegreesConcerned
                where kvp.Key < degrees 
                orderby degrees - kvp.Key
                select kvp.Value;

            List<Aspects> aspects = previousQuery.ToList();

            return aspects.First();
        }

        public static Aspects PreviousAspectOf(Angle angle)
        {
            return PreviousAspectOf(MajorDegrees, angle);
        }

        public static Aspects NextAspect(Angle angle, bool isExpanding)
        {
            Aspects temp = isExpanding ? Aspects.NextAspectOf(angle) : Aspects.PreviousAspectOf(angle);

            if (Math.Abs(temp.OrbOf(angle)) > Negligible)
                return temp;
            else
                return isExpanding ? Aspects.NextAspectOf(temp) : Aspects.PreviousAspectOf(temp);
        }

        #endregion

        #region Fields
        public AspectType Kind { get; private set; }

        public string Name { get; private set; }

        public Char Symbol { get { return Symbols[Kind]; } }

        public double PermissibleOrb { get; private set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor of Aspect
        /// </summary>
        /// <param name="angle">Corresponding angle</param>
        /// <param name="kind">Type of the aspect</param>
        /// <param name="maxOrb">Acceptable Orb</param>
        private Aspects(double degrees, AspectType kind, double maxOrb) // : base(degrees)
        {
            Degrees = degrees;  //To avoid normalize() of the Angle base class
            Kind = kind;
            Name = kind.ToString();
            PermissibleOrb = maxOrb;
        }

        private Aspects(double degrees, AspectType kind) : this(degrees, kind, DefaultMaxOrb)
        { }

        #endregion

        #region Functions
        public double OrbOf(Angle angle)
        {
            Double temp = (angle.Degrees - Degrees).Normalize();
            return (temp > 180) ? temp - 360 : temp;
        }

        public bool IsExact(Angle angle)
        {
            return Math.Abs(Degrees - angle.Degrees) <= DefaultMaxOrb;
        }

        public bool IsAround(Angle angle)
        {
            return Math.Abs(Degrees - angle.Degrees) <= PermissibleOrb;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", Symbol, Degrees);
        }
        #endregion

    }
}
