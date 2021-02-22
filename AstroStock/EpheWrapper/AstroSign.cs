using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EpheWrapper
{
    /// <summary>
    /// Design of all in astrology.
    /// </summary>
    /// <see cref="http://www.astrologycom.com/dignities.html"/>
    /// <see cref="http://x.sheup.com/xingzuo/30/xingzuo12783.htm"/>
    public class AstroSign : IComparable<AstroSign>
    {
        public const string Copyright = "Copyright@2010 by William JIANG";
        public const string SoftwareName = "AstroCalc";
        public const string Email = "williamjiang0218@gmail.com";
        public const int InternalVersion = 20100727;

        #region Constants defintions
        public static readonly AstroSign Aries = 
            new AstroSign(1, "Aries", "Ari", Elements.Fire, Qualities.Cardinal, 
                PlanetId.SE_MARS, PlanetId.SE_SUN, PlanetId.SE_VENUS, PlanetId.SE_SATURN);
        public static readonly AstroSign Taurus = 
            new AstroSign(2, "Taurus", "Tau", Elements.Earth, Qualities.Fixed, 
                PlanetId.SE_VENUS, PlanetId.SE_MOON, PlanetId.SE_MARS, PlanetId.SE_ECL_NUT);
        public static readonly AstroSign Gemini = 
            new AstroSign(3, "Gemini", "Gem", Elements.Air, Qualities.Mutable, 
                PlanetId.SE_MERCURY, PlanetId.SE_ECL_NUT, PlanetId.SE_JUPITER, PlanetId.SE_ECL_NUT);
        public static readonly AstroSign Cancer = 
            new AstroSign(4, "Cancer", "Can", Elements.Water, Qualities.Cardinal, 
                PlanetId.SE_MOON, PlanetId.SE_JUPITER, PlanetId.SE_SATURN, PlanetId.SE_MARS);
        public static readonly AstroSign Leo = 
            new AstroSign(5, "Leo", "Leo", Elements.Fire, Qualities.Fixed, 
                PlanetId.SE_SUN, PlanetId.SE_ECL_NUT, PlanetId.SE_SATURN, PlanetId.SE_ECL_NUT);
        public static readonly AstroSign Virgo = 
            new AstroSign(6, "Virgo", "Vir", Elements.Earth, Qualities.Mutable, 
                PlanetId.SE_MERCURY, PlanetId.SE_MERCURY, PlanetId.SE_JUPITER, PlanetId.SE_VENUS);
        public static readonly AstroSign Libra = 
            new AstroSign(7, "Libra", "Lib", Elements.Air, Qualities.Cardinal, 
                PlanetId.SE_VENUS, PlanetId.SE_SATURN, PlanetId.SE_MARS, PlanetId.SE_SUN);
        public static readonly AstroSign Scorpio = 
            new AstroSign(8, "Scorpio", "Sco", Elements.Water, Qualities.Fixed, 
                PlanetId.SE_PLUTO, PlanetId.SE_ECL_NUT, PlanetId.SE_VENUS, PlanetId.SE_MOON);
        public static readonly AstroSign Sagittarius = 
            new AstroSign(9, "Sagittarius", "Sag", Elements.Fire, Qualities.Mutable, 
                PlanetId.SE_JUPITER, PlanetId.SE_ECL_NUT, PlanetId.SE_MERCURY, PlanetId.SE_ECL_NUT);
        public static readonly AstroSign Capricorn = 
            new AstroSign(10, "Capricorn", "Cap", Elements.Earth, Qualities.Cardinal, 
                PlanetId.SE_SATURN, PlanetId.SE_MARS, PlanetId.SE_MOON, PlanetId.SE_JUPITER);
        public static readonly AstroSign Aquarius = 
            new AstroSign(11, "Aquarius", "Aqu", Elements.Air, Qualities.Fixed, 
                PlanetId.SE_URANUS, PlanetId.SE_ECL_NUT, PlanetId.SE_SUN, PlanetId.SE_ECL_NUT);
        public static readonly AstroSign Pisces = 
            new AstroSign(12, "Pisces", "Pis", Elements.Water, Qualities.Mutable, 
                PlanetId.SE_NEPTUNE, PlanetId.SE_VENUS, PlanetId.SE_VENUS, PlanetId.SE_MERCURY);
        
        public static readonly SortedDictionary<int, AstroSign> All;

        public static readonly Dictionary<string, int> Abbreviations;

        static AstroSign()
        { 
            All = new SortedDictionary<int, AstroSign>();
            Abbreviations = new Dictionary<string, int>();

            Type signType = typeof(AstroSign);
            FieldInfo[] fields = signType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                AstroSign sign = fi.GetValue(null) as AstroSign;
                if (sign != null)
                {
                    All.Add(sign.Index, sign);
                    Abbreviations.Add(sign.Abbrev, sign.Index);
                }
            }

            //all.Add(1, AstroSign.Aries);
            //all.Add(2, AstroSign.Taurus);
            //all.Add(3, AstroSign.Gemini);
            //all.Add(4, AstroSign.Cancer);
            //all.Add(5, AstroSign.Leo);
            //all.Add(6, AstroSign.Virgo);
            //all.Add(7, AstroSign.Libra);
            //all.Add(8, AstroSign.Scorpio);
            //all.Add(9, AstroSign.Sagittarius);
            //all.Add(10, AstroSign.Capricorn);
            //all.Add(11, AstroSign.Aquarius);
            //all.Add(12, AstroSign.Pisces);

            //foreach (KeyValuePair<int, AstroSign> kvp in all)
            //{
            //    Abbreviations.Add(kvp.Value.Abbrev.ToUpper(), kvp.Key);
            //}
        }

        #endregion

        #region Fields and Properties

        private readonly int index;

        public int Index
        {
            get { return index; }
        }

        public double Cusp
        {
            get { return (index - 1.0) * Longitude.DegreesPerSign; }
        }

        private readonly string name;

        public string Name
        {
            get { return name; }
        }

        private readonly string abbrev;

        public string Abbrev
        {
            get { return abbrev; }
        }

        private readonly Elements element;

        public Elements Element
        {
            get { return element; }
        }

        private readonly Qualities quality;

        public Qualities Quality
        {
            get { return quality; }
        }

        public Genders Gender
        {
            get { return (this.element == Elements.Fire || this.element == Elements.Air) ? Genders.Male : Genders.Female; }
        }

        //private readonly Planet[] rulers;

        //public Planet[] Rulers
        //{
        //    get { return rulers; }
        //} 

        //public Planet Ruler
        //{
        //    get { return this.rulers == null ? null : ruler[0]; }
        //}

        private readonly PlanetId rulerId;

        public Planet Ruler
        {
            get { return Planet.PlanetOf(rulerId); }
        }

        private readonly PlanetId exaltedId;

        public Planet Exalted
        {
            get { return Planet.PlanetOf(exaltedId); }
        }

        private readonly PlanetId detrimentId;

        public Planet Detriment
        {
            get { return Planet.PlanetOf(detrimentId); }
        }

        private readonly PlanetId fallId;

        public Planet Fall
        {
            get { return Planet.PlanetOf(fallId); }
        }

        public static object Properties { get; private set; }
        #endregion

        #region Constructors

        private AstroSign(int index, string name, string abbrev, Elements elem, Qualities quality)
        {
            this.index = index;
            this.name = name;
            this.abbrev = abbrev;
            this.element = elem;
            this.quality = quality;
        }

        private AstroSign(int index, string name, string abbrev, Elements elem, Qualities quality, 
            PlanetId ruler, PlanetId exalted, PlanetId detriment, PlanetId fall)
            : this(index, name, abbrev, elem, quality)
        {
            this.rulerId = ruler;
            this.exaltedId = exalted;
            this.detrimentId = detriment;
            this.fallId = fall;
        }

        //private AstroSign(int index, string name, string abbrev, Elements elem, Qualities quality, Planet[] rulers)
        //    : this(index, name, abbrev, elem, quality)
        //{
        //    this.rulers = rulers;
        //}
        #endregion


        #region IComparable<AstroSign> 成员

        public int CompareTo(AstroSign other)
        {
            return this.index.CompareTo(other.Index);
        }

        #endregion
    }

}
