using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    public class Sign
    {
        public const string Copyright = "Copyright@2010 by William JIANG";
        public const string SoftwareName = "AstroCalc";
        public const string Email = "williamjiang0218@gmail.com";
        public const int InternalVersion = 20100727;

        public static readonly Sign Aries = new Sign(1, Properties.Resources.Aries, "Ari", Elements.Fire, Qualities.Cardinal);
        public static readonly Sign Taurus = new Sign(2, Properties.Resources.Taurus, "Tau", Elements.Earth, Qualities.Fixed);
        public static readonly Sign Gemini = new Sign(3, Properties.Resources.Gemini, "Gem", Elements.Air, Qualities.Mutable);
        public static readonly Sign Cancer = new Sign(4, Properties.Resources.Cancer, "Can", Elements.Water, Qualities.Cardinal);
        public static readonly Sign Leo = new Sign(5, Properties.Resources.Leo, "Leo", Elements.Fire, Qualities.Fixed);
        public static readonly Sign Virgo = new Sign(6, Properties.Resources.Virgo, "Vir", Elements.Earth, Qualities.Mutable);
        public static readonly Sign Libra = new Sign(7, Properties.Resources.Libra, "Lib", Elements.Air, Qualities.Cardinal);
        public static readonly Sign Scorpio = new Sign(8, Properties.Resources.Scorpio, "Sco", Elements.Water, Qualities.Fixed);
        public static readonly Sign Sagittarius = new Sign(9, Properties.Resources.Sagittarius, "Sag", Elements.Fire, Qualities.Mutable);
        public static readonly Sign Capricorn = new Sign(10, Properties.Resources.Capricorn, "Cap", Elements.Earth, Qualities.Cardinal);
        public static readonly Sign Aquarius = new Sign(11, Properties.Resources.Aquarius, "Aqu", Elements.Air, Qualities.Fixed);
        public static readonly Sign Pisces = new Sign(12, Properties.Resources.Pisces, "Pis", Elements.Water, Qualities.Mutable);

        private readonly int index;

        public int Index
        {
            get { return index; }
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

        private Sign(int index, string name, string abbrev, Elements elem, Qualities quality)
        {
            this.index = index;
            this.name = name;
            this.abbrev = abbrev;
            this.element = elem;
            this.quality = quality;
        }
    }
}
