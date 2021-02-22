using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    [Serializable]
    public class OrbitDescription : IEquatable<OrbitDescription>
    {
        public bool IsLocked { get; set; }

        public PlanetId Owner { get; set; }

        public SeFlg Centric { get; set; }

        public bool IsReversed { get; set; }

        public double Slope { get; set; }

        public double Shift { get; set; }

        public OrbitDescription() {}

        public OrbitDescription(bool isLocked, PlanetId owner, SeFlg centric, bool isReversed, double slope, double shift)
        {
            IsLocked = isLocked;
            Owner = owner;
            Centric = centric;
            IsReversed = isReversed;
            Slope = slope;
            Shift = shift;
        }

        public override string ToString()
        {
            return String.Format("{0}{1}{2}{3}{4}{5:F2}*{6}", IsLocked?"#":"", IsReversed ? "-" : "", 
                Planet.Glyphs[Owner], Centric == SeFlg.HELIOCENTRIC ? "(H)" : "",
                Shift < 0 ? "-" : "+", Math.Abs(Shift), Slope);
        }

        #region IEquatable<OrbitDescription> 成员

        public bool Equals(OrbitDescription other)
        {
            return this.Owner == other.Owner && this.Centric == other.Centric && this.IsReversed == other.IsReversed && this.Slope == other.Slope
                && this.Shift == other.Shift;
        }

        #endregion
    }

}
