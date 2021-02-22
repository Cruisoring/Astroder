using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum MovementFlag
    {
        GeoLongitudeMovement,
        GeoLatitudeMovement,
        GeoDistanceMovement,
        HelioLongitudeMovement,
        HelioLatitudeMovement,
        HelioDistanceMovement
    }

    public class Movements
    {
        public static int HelioIndex = -1;

        public static double MovementOf(PlanetId id, bool isHeliocentric, DateTimeOffset time1, DateTimeOffset time2)
        {
            Position pos1, pos2;

            if (time1 == time2)
                return 0;
            else if (time1 > time2)
            {
                DateTimeOffset tempTime = time1;
                time1 = time2;
                time2 = tempTime;
            }

            TimeSpan duration = time2 - time1;
            double period = duration.TotalDays / Planet.OrbitalPeriods[id];
            double round = period * 360;

            if (isHeliocentric)
            {
                pos1 = Ephemeris.HeliocentricPositionOf(time1, id);
                pos2 = Ephemeris.HeliocentricPositionOf(time2, id);

                double temp = (360 + pos2.Longitude - pos1.Longitude) % 360;
                return Math.Round((round - temp) / 360) * 360 + temp;
            }
            else
            {
                pos1 = Ephemeris.GeocentricPositionOf(time1, id);
                pos2 = Ephemeris.GeocentricPositionOf(time2, id);

                double temp = pos2.Longitude - pos1.Longitude;
                if (temp < 0 && (period > 0.4 || duration.Days > Planet.RetrogradePeriods[id]))
                    temp += 360;
                else if (temp > 300 && period < 0.4)
                    temp -= 360;

                return Math.Round((round - temp) / 360) * 360 + temp;
            }
        }

        static Movements()
        {
            HelioIndex = Ephemeris.GeocentricLuminaries.IndexOf(PlanetId.SE_PLUTO) + 1;
        }

        public DateTimeOffset Time1 { get; set; }
        public DateTimeOffset Time2 { get; set; }

        public List<double> LongitudeChanges { get; set; }
        public List<double> LatitudeChanges { get; set; }
        public List<double> DistanceChanges { get; set; }

        public double this[PlanetId id, MovementFlag flag]
        {
            get
            {
                if (id > PlanetId.SE_PLUTO && id != PlanetId.SE_EARTH)
                    return 0;
                else if (Time1 == Time2)
                    return 0;

                switch(flag)
                {
                    case MovementFlag.GeoLongitudeMovement:
                        return LongitudeChanges[Ephemeris.GeocentricLuminaries.IndexOf(id)];
                    case MovementFlag.GeoLatitudeMovement:
                        return LatitudeChanges[Ephemeris.GeocentricLuminaries.IndexOf(id)];
                    case MovementFlag.GeoDistanceMovement:
                        return DistanceChanges[Ephemeris.GeocentricLuminaries.IndexOf(id)];
                    case MovementFlag.HelioLongitudeMovement:
                        return LongitudeChanges[HelioIndex + Ephemeris.HeliocentricLuminaries.IndexOf(id)];
                    case MovementFlag.HelioLatitudeMovement:
                        return LatitudeChanges[HelioIndex + Ephemeris.HeliocentricLuminaries.IndexOf(id)];
                    case MovementFlag.HelioDistanceMovement:
                        return DistanceChanges[HelioIndex + Ephemeris.HeliocentricLuminaries.IndexOf(id)];
                    default:
                        return 0;
                }
            }
        }

        public Movements()
        {
            LongitudeChanges = new List<double>();
            LatitudeChanges = new List<double>();
            DistanceChanges = new List<double>();
        }

        public Movements(DateTimeOffset time1, DateTimeOffset time2) : this()
        {
            Time1 = time1>time2 ? time2 : time1;
            Time2 = time2>time1 ? time2 : time1;

            if (Time1 == Time2)
                return;

            Position pos1, pos2;
            double jul_ut1 = Ephemeris.ToJulianDay(Time1);
            double jul_ut2 = Ephemeris.ToJulianDay(Time2);
            double temp, movement, round;
            TimeSpan duration = Time2 - Time1;

            foreach (PlanetId id in Ephemeris.GeocentricLuminaries)
            {
                if (id > PlanetId.SE_PLUTO)
                    continue;

                pos1 = Ephemeris.GeocentricPositionOf(jul_ut1, id);
                pos2 = Ephemeris.GeocentricPositionOf(jul_ut2, id);

                double period = duration.TotalDays / Planet.OrbitalPeriods[id];
                round = period * 360;

                temp = pos2.Longitude - pos1.Longitude;
                if (temp < 0 && (period > 0.4||duration.Days > Planet.RetrogradePeriods[id]))
                    temp += 360;
                else if (temp > 300 && period < 0.4)
                    temp -= 360;
                
                movement = Math.Round((round - temp)/360) * 360 + temp;
                LongitudeChanges.Add(movement);
                movement = pos2.Latitude - pos1.Latitude;
                LatitudeChanges.Add(movement);
                movement = pos2.Distance - pos1.Distance;
                DistanceChanges.Add(movement);
            }

            foreach (PlanetId id in Ephemeris.HeliocentricLuminaries)
            {
                if (id > PlanetId.SE_PLUTO && id != PlanetId.SE_EARTH)
                    continue;

                pos1 = Ephemeris.HeliocentricPositionOf(jul_ut1, id);
                pos2 = Ephemeris.HeliocentricPositionOf(jul_ut2, id);

                double period = duration.TotalDays / Planet.OrbitalPeriods[id];
                round = period * 360;

                temp = (360 + pos2.Longitude - pos1.Longitude) % 360;
                movement = Math.Round((round - temp) / 360) * 360 + temp;
                //movement = (Math.Round((Time2 - Time1).TotalDays / Planet.OrbitalPeriods[id]) * 360 + temp);
                LongitudeChanges.Add(movement);
                movement = pos2.Latitude - pos1.Latitude;
                LatitudeChanges.Add(movement);
                movement = pos2.Distance - pos1.Distance;
                DistanceChanges.Add(movement);
            }

        }
    }
}
