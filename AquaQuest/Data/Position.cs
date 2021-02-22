using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    public class Position
    {
        private Angle longitude;       // Rectascension
        private Angle latitude;        // Declination
        private double distance;        // distance in AU
        private double speedLongitude;  // Speed in rectascension (deg/day)
        private double rightAscension;  // Speed in declination (deg/day)
        private double declination;     // Speed in distance (AU/day)

        /// <summary>
        /// Longitude 
        /// </summary>
        public Angle Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        /// <summary>
        /// Latitude, new values will be converted to a range of -90..+90
        /// </summary>
        public Angle Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }


        /// <summary>
        /// Distance of body (radius vector) in AU
        /// </summary>
        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        /// Speed in longitude
        /// </summary>
        public double SpeedLongitude
        {
            get { return speedLongitude; }
            set { speedLongitude = value; }
        }

        /// <summary>
        /// Right ascension in degrees
        /// </summary>
        public double RightAscension
        {
            get { return rightAscension; }
            set { rightAscension = value; }
        }

        /// <summary>
        /// Declination
        /// </summary>
        public double declination
        {
            get { return _declination; }
            set { _declination = value; }
        }
    }
}
