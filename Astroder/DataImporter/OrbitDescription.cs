using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EphemerisCalculator;

namespace DataImporter
{
    [Serializable]
    public class OrbitDescription : IEquatable<OrbitDescription>
    {
        public bool IsLocked { get; set; }

        public PlanetId Owner { get; set; }

        public SeFlg Centric { get; set; }

        public OutlineItem Pivot { get; set; }

        public bool IsReversed { get; set; }

        public double Slope { get; set; }

        public double Offset { get; set; }

        private double pivotShift = double.MinValue;
        public double PivotShift
        {
            get
            {
                if (Pivot == null)
                    return 0;
                else if (pivotShift != double.MinValue)
                    return pivotShift;

                if (Owner == PlanetId.Earth_Rotation)
                {
                    pivotShift = 0;
                }
                else
                {
                    Position pos = (Centric == SeFlg.GEOCENTRIC) ? Ephemeris.GeocentricPositionOf(Pivot.Time, Owner)
                        : Ephemeris.HeliocentricPositionOf(Pivot.Time, Owner);
                    double orig = IsReversed ? 360 - pos.Longitude : pos.Longitude;

                    pivotShift = (Pivot.Price / Slope - orig);
                }
                return pivotShift;
            }
        }

        public OrbitDescription() {}

        public OrbitDescription(bool isLocked, bool isReversed, PlanetId owner, SeFlg centric, double offset, OutlineItem pivot, double slope)
        {
            IsLocked = isLocked;
            Pivot = pivot;
            Owner = owner;
            Centric = centric;
            IsReversed = isReversed;
            Slope = slope;
            Offset = offset;
        }

        public OrbitDescription(bool isLocked, SeFlg centric, OrbitSet orbits, int round, OutlineItem pivot, double slope)
        {
            IsLocked = isLocked;
            Pivot = pivot;
            Owner = orbits.Owner;
            Centric = centric;
            IsReversed = orbits.IsReversed;
            Slope = slope;
            Offset = orbits.Offset + 360*round;
        }

        public override string ToString()
        {
            string name = (IsReversed ? "-" : "") + Planet.Glyphs[Owner].ToString() + (Centric == SeFlg.HELIOCENTRIC ? "(H)" : "");
            return String.Format("{0} {1}+{2} {3} {4}",
                IsLocked?"#":"",
                name,
                Offset,
                Pivot == null ? "" : "&"+Pivot.ToString(),
                Slope == 1 ? "" : "*" + Slope.ToString());
            //return String.Format("{0}{1}{2}{3}{4}{5:F2}*{6}", IsLocked?"#":"", IsReversed ? "-" : "", 
            //    Planet.Glyphs[Owner], Centric == SeFlg.HELIOCENTRIC ? "(H)" : "",
            //    Offset < 0 ? "-" : "+", Math.Abs(Offset), Slope);
        }

        public double DegreeOn(DateTimeOffset time)
        {
            if (Owner == PlanetId.Earth_Rotation)
            {
                TimeSpan duration = time - Pivot.Time;

                return duration.TotalMinutes / 4;

            }
            else
            {
                Position pos = Ephemeris.PositionOf(time, Owner, Centric);
                double result = (IsReversed ? 360 - pos.Longitude : pos.Longitude) + Offset + PivotShift;

                return result;
            }
        }

        public double DegreeOn(Double oaDate)
        {
            return DegreeOn(new DateTimeOffset(DateTime.FromOADate(oaDate)));
        }

        #region IEquatable<OrbitDescription> 成员

        public bool Equals(OrbitDescription other)
        {
            return this.Owner == other.Owner && this.Centric == other.Centric && this.IsReversed == other.IsReversed && this.Slope == other.Slope
                && this.Offset == other.Offset && this.Pivot == other.Pivot;
        }

        #endregion
    }

    [Serializable]
    public class OrbitSet : IEquatable<OrbitSet>
    {
        public PlanetId Owner { get; set; }

        public bool IsReversed { get; set; }

        public double Offset { get; set; }

        //public Color TheColor { get { return Planet.PlanetsColors.ContainsKey(Owner) ? Planet.PlanetsColors[Owner].First() : Color.Gray; } }

        public OrbitSet() { }

        public OrbitSet(PlanetId owner, bool isReversed, double offset)
        {
            Owner = owner;
            IsReversed = isReversed;
            Offset = offset;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}+{2}", (IsReversed ? "-" : ""), Planet.Glyphs[Owner], Offset);
        }

        public bool Contains(OrbitDescription desc)
        {
            double offset = desc.Offset % 360;
            if (offset < 0)
                offset += 360;

            return this.Owner == desc.Owner && this.Offset == offset && this.IsReversed == desc.IsReversed;
        }


        #region IEquatable<OrbitSet> 成员

        public bool Equals(OrbitSet other)
        {
            return this.Owner == other.Owner && this.IsReversed == other.IsReversed && this.Offset == other.Offset;
        }

        #endregion
    }
}
