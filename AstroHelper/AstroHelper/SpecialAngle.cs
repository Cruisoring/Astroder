using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroHelper
{
    public class SpecialAngle : IOrbitable
    {
        public static Dictionary<Double, SpecialAngle> Famous = new Dictionary<Double, SpecialAngle>()
        {
            { -1, null},
            { 0, new SpecialAngle(0) },
            { 60, new SpecialAngle(60) },
            { 90, new SpecialAngle(90) },
            { 120, new SpecialAngle(120) },
            { 180, new SpecialAngle(180) },
            { 240, new SpecialAngle(240) },
            { 300, new SpecialAngle(300) }
        };

        public Double Degree { get; private set; }

        public SpecialAngle(Double degree)
        {
            Degree = degree;
        }

        #region IOrbitable 成员

        public bool ContainsX(double x)
        {
            return true;
        }

        public double this[double x]
        {
            get { return Degree; }
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0}", Degree);
        }
    }
}
