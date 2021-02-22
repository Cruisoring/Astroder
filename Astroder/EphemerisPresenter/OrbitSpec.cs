using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = System.Drawing.Color;
//using ZedGraph;

namespace EphemerisCalculator
{
    [Serializable]
    public class OrbitSpec : EphemerisCalculator.IOrbitable
    {
        #region Static definitions
        public static Dictionary<OrbitInfoType, String> OrbitTypeAbbrv = new Dictionary<OrbitInfoType, String>
        {
            //{ OrbitInfoType.All,""},
            { OrbitInfoType.Longitude, ""},
            { OrbitInfoType.Latitude, "H"},
            { OrbitInfoType.Distance, "D"},
            { OrbitInfoType.LongitudeVelocities, "V"},
            { OrbitInfoType.LatitudeVelocities, "HV"},
            { OrbitInfoType.DistanceVelocities, "DV"},

            { OrbitInfoType.LongitudeAcceleration, "Acc"},
            { OrbitInfoType.LongVelocityAndLatitude, "VH"},

            { OrbitInfoType.Ascending, "Asc"},
            { OrbitInfoType.Descending, "Dsc"},
            { OrbitInfoType.Perigee, "Per"},
            { OrbitInfoType.Apogee, "Apo"},

            { OrbitInfoType.AscendingLatitude, "AscH"},
            { OrbitInfoType.DescendingLatitude, "DscH"},
            { OrbitInfoType.PerigeeLatitude, "PerH"},
            { OrbitInfoType.ApogeeLatitude, "ApoH"},

            { OrbitInfoType.AscendingVelocities, "AscV"},
            { OrbitInfoType.DescendingVelocities, "DscV"},
            { OrbitInfoType.PerigeeVelocities, "PerV"},
            { OrbitInfoType.ApogeeVelocities, "ApoV"}
        };

        public static string LableOf(PlanetId id, OrbitInfoType kind)
        {
            return String.Format("{0}{1}", Planet.SymbolOf(id), OrbitTypeAbbrv[kind]);
        }

        #endregion

        #region Fields

        public PlanetId Id { get; set; }

        public OrbitInfoType Kind { get; set; }

        public List<Double> YList { get; private set; }

        public List<Double> XList { get; private set; }

        public bool ContainsX(Double x)
        {
            return XList.Contains(x);
        }

        public Double this[Double x]
        {
            get
            {
                if (!ContainsX(x))
                    throw new Exception();
                else
                {
                    int index = XList.IndexOf(x);
                    return YList[index];
                }
            }
        }

        public int Count { get { return XList.Count; } }

        public string Label { get { return LableOf(Id, Kind); } }

        public Color DisplayColor { get; set; }

        public SymbolType Symbol { get; set; }

        public LineItem OriginalCurve { get; set; }

        public Double MaxY { get { return YList.Max(); } }

        public Double MinY { get { return YList.Min(); } }

        #endregion

        #region Constructors

        //public OrbitSpec(OrbitsCollection orbits, PlanetId id, OrbitInfoType kind)
        //{
        //    YList = orbits[id, kind];
        //    XList = (kind < OrbitInfoType.Ascending) ? orbits.DayByDay : orbits.MonthByMonth;
        //    if (Count != YList.Count)
        //        throw new Exception();

        //    Label = LableOf(id, kind);
        //    if (kind == OrbitInfoType.Longitude)
        //        DisplayColor = Planet.ColorOf(id);
        //    else if (kind.ToString().Contains("Distance") || kind.ToString().Contains("Velocities"))
        //    {
        //        DisplayColor = Color.DarkCyan;
        //    }
        //    else if (kind.ToString().Contains("Latitude"))
        //    {
        //        DisplayColor = Color.DarkGray;
        //    }
        //    else
        //    {
        //        Color baseColor = Planet.ColorOf(id);
        //        int g = (baseColor.G == 0) ? 0 : (int)(baseColor.G * (1 - (int)kind / 16.0));
        //        int b = (baseColor.B == 0) ? 0 : (int)(baseColor.B * (1 - (int)kind / 16.0));
        //        int r = (baseColor.R == 0) ? 0 : (int)(baseColor.R * (1 - (int)kind / 16.0));
        //        int a = (baseColor.A == 0) ? 0 : (int)(baseColor.A * (1 - (int)kind / 16.0));

        //        DisplayColor = Color.FromArgb(a, r, g, b);
        //    }

        //    Symbol = SymbolType.None;

        //    OriginalCurve  = new LineItem(Label, XList.ToArray(), YList.ToArray(), DisplayColor, Symbol);
        //}

        public OrbitSpec(PlanetId id, OrbitInfoType kind, List<double> xList, List<double> yList)
        {
            Id = id;
            Kind = kind;
            
            XList = xList;

            if (kind == OrbitInfoType.Distance)
            {
                Double averageDistance = Planet.AverageDistances[id];
                YList = (from y in yList
                         select y - averageDistance).ToList();
            }
            else
                YList = yList;

            if (Count != YList.Count)
                throw new Exception();

            DisplayColor = Planet.ColorOf(id, kind);
            //if (kind == OrbitInfoType.Longitude || kind == OrbitInfoType.LongVelocityAndLatitude || kind == OrbitInfoType.LongitudeAcceleration)
            //{
            //    DisplayColor = Planet.ColorOf(id);
            //}
            //else if (kind.ToString().EndsWith("Latitude"))
            //{
            //    DisplayColor = WeightedColor(Planet.ColorOf(id), 0.9f, true);
            //}
            //else if (kind.ToString().EndsWith("Distance"))
            //{
            //    DisplayColor = WeightedColor(Planet.ColorOf(id), 0.5f, true);
            //}
            //else if (kind.ToString().EndsWith("Velocities"))
            //{
            //    DisplayColor = WeightedColor(Planet.ColorOf(id), 0.8f, false);
            //}
            //else
            //{
            //    Color baseColor = Planet.ColorOf(id);
            //    int g = (baseColor.G == 0) ? 0 : (int)(baseColor.G * (1 - (int)kind / 16.0));
            //    int b = (baseColor.B == 0) ? 0 : (int)(baseColor.B * (1 - (int)kind / 16.0));
            //    int r = (baseColor.R == 0) ? 0 : (int)(baseColor.R * (1 - (int)kind / 16.0));
            //    int a = (baseColor.A == 0) ? 0 : (int)(baseColor.A * (1 - (int)kind / 16.0));

            //    DisplayColor = Color.FromArgb(a, r, g, b);
            //}

            Symbol = SymbolType.None;

            OriginalCurve = new LineItem(Label, XList.ToArray(), YList.ToArray(), DisplayColor, Symbol);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0}: from {1} to {2}", Label, DateTime.FromOADate(XList.First()), DateTime.FromOADate(XList.Last()));
        }
    }
}
