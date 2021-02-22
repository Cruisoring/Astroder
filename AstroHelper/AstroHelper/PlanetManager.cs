using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AstroHelper
{
    [Serializable]
    public class PlanetManager : INotifyPropertyChanged
    {
        #region Static definition
        public const int DefaultOff = 0;
        public const int DefaultOn = 1;

        public static BitVector32.Section LongitudeMask;            //The bit 0 indicates if the longitude is display or not
        public static BitVector32.Section LatitudeMask;             //The bit 1 indicates if the latitude is on
        public static BitVector32.Section DistanceMask;             //The bit 2 indicates if the distance is on
        public static BitVector32.Section LongitudeVelocitiesMask;    //The bit 3 indicates if the longitude velocity is display or not
        public static BitVector32.Section LatitudeVelocitiesMask;     //The bit 4 indicates if the latitude velocity is display or not
        public static BitVector32.Section DistanceVelocitiesMask;     //The bit 5 indicates if the distance velocity is display or not

        public static BitVector32.Section AscendingMask;            //The bit 6 indicates if the Ascending is displayed or not
        public static BitVector32.Section DescendingMask;           //The bit 7 indicates if the Descending is displayed or not
        public static BitVector32.Section PerigeeMask;              //The bit 8 indicates if the Perigee is displayed or not
        public static BitVector32.Section ApogeeMask;               //The bit 9 indicates if the Apogee is displayed or not

        public static BitVector32.Section AscendingLatitudeMask;    //The bit 10 indicates if the AscendingLatitude is displayed or not
        public static BitVector32.Section DescendingLatitudeMask;   //The bit 11 indicates if the DescendingLatitude is displayed or not
        public static BitVector32.Section PerigeeLatitudeMask;      //The bit 12 indicates if the PerigeeLatitude is displayed or not
        public static BitVector32.Section ApogeeLatitudeMask;       //The bit 13 indicates if the ApogeeLatitude is displayed or not

        public static BitVector32.Section AscendingVelocitiesMask;  //The bit 14 indicates if the AscendingVelocities is displayed or not
        public static BitVector32.Section DescendingVelocitiesMask; //The bit 15 indicates if the DescendingVelocities is displayed or not
        public static BitVector32.Section PerigeeVelocitiesMask;    //The bit 16 indicates if the PerigeeVelocities is displayed or not
        public static BitVector32.Section ApogeeVelocitiesMask;     //The bit 17 indicates if the ApogeeVelocities is displayed or not
        public static BitVector32.Section LowBitsMask;
        public static BitVector32.Section HighBitsMask;

        static PlanetManager()
        {
            LowBitsMask = BitVector32.CreateSection(0xFF);
            HighBitsMask = BitVector32.CreateSection(0xFF, LowBitsMask);
            LongitudeMask = BitVector32.CreateSection(1);            //The bit 0 indicates if the longitude is display or not
            LatitudeMask = BitVector32.CreateSection(1, LongitudeMask);             //The bit 1 indicates if the latitude is on
            DistanceMask = BitVector32.CreateSection(1, LatitudeMask);             //The bit 2 indicates if the distance is on
            LongitudeVelocitiesMask = BitVector32.CreateSection(1, DistanceMask);    //The bit 3 indicates if the longitude velocity is display or not
            LatitudeVelocitiesMask = BitVector32.CreateSection(1, LongitudeVelocitiesMask);     //The bit 4 indicates if the latitude velocity is display or not
            DistanceVelocitiesMask = BitVector32.CreateSection(1, LatitudeVelocitiesMask);     //The bit 5 indicates if the distance velocity is display or not

            AscendingMask = BitVector32.CreateSection(1, DistanceVelocitiesMask);            //The bit 6 indicates if the Ascending is displayed or not
            DescendingMask = BitVector32.CreateSection(1, AscendingMask);           //The bit 7 indicates if the Descending is displayed or not
            PerigeeMask = BitVector32.CreateSection(1, DescendingMask);              //The bit 8 indicates if the Perigee is displayed or not
            ApogeeMask = BitVector32.CreateSection(1, PerigeeMask);               //The bit 9 indicates if the Apogee is displayed or not

            AscendingLatitudeMask = BitVector32.CreateSection(1, ApogeeMask);    //The bit 10 indicates if the AscendingLatitude is displayed or not
            DescendingLatitudeMask = BitVector32.CreateSection(1, AscendingLatitudeMask);   //The bit 11 indicates if the DescendingLatitude is displayed or not
            PerigeeLatitudeMask = BitVector32.CreateSection(1, DescendingLatitudeMask);      //The bit 12 indicates if the PerigeeLatitude is displayed or not
            ApogeeLatitudeMask = BitVector32.CreateSection(1, PerigeeLatitudeMask);       //The bit 13 indicates if the ApogeeLatitude is displayed or not

            AscendingVelocitiesMask = BitVector32.CreateSection(1, ApogeeLatitudeMask);  //The bit 14 indicates if the AscendingVelocities is displayed or not
            DescendingVelocitiesMask = BitVector32.CreateSection(1, AscendingVelocitiesMask); //The bit 15 indicates if the DescendingVelocities is displayed or not
            PerigeeVelocitiesMask = BitVector32.CreateSection(1, DescendingVelocitiesMask);    //The bit 16 indicates if the PerigeeVelocities is displayed or not
            ApogeeVelocitiesMask = BitVector32.CreateSection(1, PerigeeVelocitiesMask);     //The bit 17 indicates if the ApogeeVelocities is displayed or not

        }
        #endregion

        #region Properties

        public PlanetId Star { get; set; }

        private BitVector32 manager;

        public bool IsPlanetShown
        {
            get { return manager.Data != DefaultOff; }
            set 
            {
                if(value != (manager.Data != DefaultOff))
                {
                    manager[LowBitsMask] = value ? DefaultOn : DefaultOff;
                    manager[HighBitsMask] = 0;
                    NotifyPropertyChanged(OrbitInfoType.Longitude);
                }
            }
        }

        public bool LongitudeShown 
        {
            get { return manager[LongitudeMask] == 1; }
            set
            {
                if (value != (manager[LongitudeMask] == 1))
                {
                    manager[LongitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Longitude);
                }
            }
        }
        public bool LatitudeShown
        {
            get { return manager[LatitudeMask] == 1; }
            set
            {
                if (value != (manager[LatitudeMask] == 1))
                {
                    manager[LatitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Latitude);
                }
            }
        }
        public bool DistanceShown 
        {
            get { return manager[DistanceMask] == 1; }
            set
            {
                if (value != (manager[DistanceMask] == 1))
                {
                    manager[DistanceMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Distance);
                }
            }
        }
        public bool LongitudeVelocitiesShown
        {
            get { return manager[LongitudeVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[LongitudeVelocitiesMask] == 1))
                {
                    manager[LongitudeVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.LongitudeVelocities);
                }
            }
        }
        public bool LatitudeVelocitiesShown {
            get { return manager[LatitudeVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[LatitudeVelocitiesMask] == 1))
                {
                    manager[LatitudeVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.LatitudeVelocities);
                }
            }
        }
        public bool DistanceVelocitiesShown {
            get { return manager[DistanceVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[DistanceVelocitiesMask] == 1))
                {
                    manager[DistanceVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.DistanceVelocities);
                }
            }
        }

        public bool AscendingShown {
            get { return manager[AscendingMask] == 1; }
            set
            {
                if (value != (manager[AscendingMask] == 1))
                {
                    manager[AscendingMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Ascending);
                }
            }
        }
        public bool DescendingShown {
            get { return manager[DescendingMask] == 1; }
            set
            {
                if (value != (manager[DescendingMask] == 1))
                {
                    manager[DescendingMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Descending);
                }
            }

        }
        public bool PerigeeShown {
            get { return manager[PerigeeMask] == 1; }
            set
            {
                if (value != (manager[PerigeeMask] == 1))
                {
                    manager[PerigeeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Perigee);
                }
            }
        }
        public bool ApogeeShown {
            get { return manager[ApogeeMask] == 1; }
            set
            {
                if (value != (manager[ApogeeMask] == 1))
                {
                    manager[ApogeeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.Apogee);
                }
            }
        }

        public bool AscendingLatitudeShown {
            get { return manager[AscendingLatitudeMask] == 1; }
            set
            {
                if (value != (manager[AscendingLatitudeMask] == 1))
                {
                    manager[AscendingLatitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.AscendingLatitude);
                }
            }
        }
        public bool DescendingLatitudeShown {
            get { return manager[DescendingLatitudeMask] == 1; }
            set
            {
                if (value != (manager[DescendingLatitudeMask] == 1))
                {
                    manager[DescendingLatitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.DescendingLatitude);
                }
            }
        }
        public bool PerigeeLatitudeShown {
            get { return manager[PerigeeLatitudeMask] == 1; }
            set
            {
                if (value != (manager[PerigeeLatitudeMask] == 1))
                {
                    manager[PerigeeLatitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.PerigeeLatitude);
                }
            }
        }
        public bool ApogeeLatitudeShown {
            get { return manager[ApogeeLatitudeMask] == 1; }
            set
            {
                if (value != (manager[ApogeeLatitudeMask] == 1))
                {
                    manager[ApogeeLatitudeMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.ApogeeLatitude);
                }
            }
        }

        public bool AscendingVelocitiesShown {
            get { return manager[AscendingVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[AscendingVelocitiesMask] == 1))
                {
                    manager[AscendingVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.AscendingVelocities);
                }
            }
        }
        public bool DescendingVelocitiesShown {
            get { return manager[DescendingVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[DescendingVelocitiesMask] == 1))
                {
                    manager[DescendingVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.DescendingVelocities);
                }
            }
        }
        public bool PerigeeVelocitiesShown
        {
            get { return manager[PerigeeVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[PerigeeVelocitiesMask] == 1))
                {
                    manager[PerigeeVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.PerigeeVelocities);
                }
            }
        }
        public bool ApogeeVelocitiesShown {
            get { return manager[ApogeeVelocitiesMask] == 1; }
            set
            {
                if (value != (manager[ApogeeVelocitiesMask] == 1))
                {
                    manager[ApogeeVelocitiesMask] = value ? 1 : 0;
                    NotifyPropertyChanged(OrbitInfoType.ApogeeVelocities);
                }
            }
        }

        public bool this[OrbitInfoType type]
        {
            get
            {
                switch(type)
                {
                    //case OrbitInfoType.All: return IsPlanetShown;
                    case OrbitInfoType.Longitude: return LatitudeShown;
                    case OrbitInfoType.Latitude: return LatitudeShown;
                    case OrbitInfoType.Distance: return DistanceShown;
                    case OrbitInfoType.LongitudeVelocities: return LongitudeVelocitiesShown;
                    case OrbitInfoType.LatitudeVelocities: return LatitudeVelocitiesShown;
                    case OrbitInfoType.DistanceVelocities: return DistanceVelocitiesShown;
                    case OrbitInfoType.Ascending: return AscendingShown;
                    case OrbitInfoType.Descending: return DescendingShown;
                    case OrbitInfoType.Perigee: return PerigeeShown;
                    case OrbitInfoType.Apogee: return ApogeeShown;
                    case OrbitInfoType.AscendingLatitude: return AscendingLatitudeShown;
                    case OrbitInfoType.DescendingLatitude: return DescendingLatitudeShown;
                    case OrbitInfoType.PerigeeLatitude: return PerigeeLatitudeShown;
                    case OrbitInfoType.ApogeeLatitude: return ApogeeLatitudeShown;
                    case OrbitInfoType.AscendingVelocities: return AscendingVelocitiesShown;
                    case OrbitInfoType.DescendingVelocities: return DescendingVelocitiesShown;
                    case OrbitInfoType.PerigeeVelocities: return PerigeeVelocitiesShown;
                    case OrbitInfoType.ApogeeVelocities: return ApogeeVelocitiesShown;
                    default: throw new NotImplementedException();
                }
            }
            set
            {
                switch(type)
                {
                    //case OrbitInfoType.All: IsPlanetShown = value; break;
                    case OrbitInfoType.Longitude: LatitudeShown = value; break;
                    case OrbitInfoType.Latitude: LatitudeShown = value; break;
                    case OrbitInfoType.Distance: DistanceShown = value; break;
                    case OrbitInfoType.LongitudeVelocities: LongitudeVelocitiesShown = value; break;
                    case OrbitInfoType.LatitudeVelocities: LatitudeVelocitiesShown = value; break;
                    case OrbitInfoType.DistanceVelocities: DistanceVelocitiesShown = value; break;
                    case OrbitInfoType.Ascending: AscendingShown = value; break;
                    case OrbitInfoType.Descending: DescendingShown = value; break;
                    case OrbitInfoType.Perigee: PerigeeShown = value; break;
                    case OrbitInfoType.Apogee: ApogeeShown = value; break;
                    case OrbitInfoType.AscendingLatitude: AscendingLatitudeShown = value; break;
                    case OrbitInfoType.DescendingLatitude: DescendingLatitudeShown = value; break;
                    case OrbitInfoType.PerigeeLatitude: PerigeeLatitudeShown = value; break;
                    case OrbitInfoType.ApogeeLatitude: ApogeeLatitudeShown = value; break;
                    case OrbitInfoType.AscendingVelocities: AscendingVelocitiesShown = value; break;
                    case OrbitInfoType.DescendingVelocities: DescendingVelocitiesShown = value; break;
                    case OrbitInfoType.PerigeeVelocities: PerigeeVelocitiesShown = value; break;
                    case OrbitInfoType.ApogeeVelocities: ApogeeVelocitiesShown = value; break;
                    default: throw new NotImplementedException();
                }
            }
        }
        #endregion

        public PlanetManager(PlanetId star)
        {
            Star = star;
            manager = new BitVector32();
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(OrbitInfoType info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(Star, new PropertyChangedEventArgs(info.ToString()));
            }
        }

        #endregion
    }
}
