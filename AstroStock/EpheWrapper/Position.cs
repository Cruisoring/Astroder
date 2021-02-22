using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace EpheWrapper
{
    /// <summary>
    /// Containing the coordinates of position and velocity of a celestrial body
    /// </summary>
    public class Position
    {
        /// <summary>
        /// the coordinates of position and velocity in the following order:
        /// Longitude	Rectascension
        /// Latitude	Declination
        /// Distance in AU	distance in AU
        /// Speed in longitude (degs/day)	Speed in rectascension (degs/day)
        /// Speed in latitude (degs/day)	Speed in declination (degs/day)
        /// Speed in distance (AU/day)	Speed in distance (AU/day)
        /// </summary>
        private Longitude rectascension;
        private Angle declination;
        private double distance;
        private Angle longitudeSpeed;
        private Angle latitudeSpeed;
        private double distanceSpeed;

        #region Fields and Properties
        //Longitude or Rectascension
        public Longitude Rectascension
        {
            get { return rectascension.Degrees; }
            set
            {
                rectascension = value;
            }
        }

        // Latitude or Declination
        public Angle Declination
        {
            get { return declination; }
            set { declination = value; }
        }

        /// Distance in AU	distance in AU
        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// Speed in longitude (degs/day)	Speed in rectascension (degs/day)
        public Angle LongVelo
        {
            get { return longitudeSpeed; }
            set { longitudeSpeed = value; }
        }

        /// Speed in latitude (degs/day)	Speed in declination (degs/day)
        public Angle LatVelo
        {
            get { return latitudeSpeed; }
            set { latitudeSpeed = value; }
        }

        /// Speed in distance (AU/day)	Speed in distance (AU/day)
        public double DistVelo
        {
            get { return distanceSpeed; }
            set { distanceSpeed = value; }
        }
        #endregion

        #region Constructors
        public Position(double[] result)
        {          
            this.rectascension = new Longitude(result[0]);
            this.declination = new Angle(result[1]);
            this.distance = result[2];
            this.longitudeSpeed = new Angle(result[3]);
            this.latitudeSpeed = new Angle(result[4]);
            this.distanceSpeed = result[5];
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Coordinates: ({0} {1}, {2: F9})", rectascension.ToString(), declination.ToString("DM4", CultureInfo.InvariantCulture), distance);
            sb.AppendFormat(", Velocity: {0}, {1}, {2: F9}", longitudeSpeed, latitudeSpeed, distanceSpeed);
            return sb.ToString();
        }

        public static Angle operator -(Position posA, Position posB)
        {
            return posA.rectascension - posB.Rectascension;
        }
        #endregion
    }

    public class PlanetPosition : Position
    {
        private PlanetId id;

        public PlanetId Id
        {
            get { return id; }
            set { id = value; }
        }

        public Planet Owner
        {
            get { return Planet.PlanetOf(id); }
        }

        #region Constructors
        public PlanetPosition(PlanetId id, double[] returned) : base(returned)
        {
            this.id = id;
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return String.Format("{0}: {1} ({2})", Planet.NameOf(id), Rectascension, Angle.InAngles(Rectascension.Degrees));
        }
        #endregion
    }
}
