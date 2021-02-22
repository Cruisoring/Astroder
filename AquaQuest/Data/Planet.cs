using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroCalc.Data
{
    public class Planet
    {
        public static Planet Sun = new Planet(PlanetId.SE_SUN, "Sun", DayOfWeek.Sunday);
        public static Planet Moon = new Planet(PlanetId.SE_MOON, "Moon", DayOfWeek.Monday);
        public static Planet Mecury = new Planet(PlanetId.SE_MERCURY, "Mercury", DayOfWeek.Wednesday);
        public static Planet Venus = new Planet(PlanetId.SE_VENUS, "Venus", DayOfWeek.Friday);
        public static Planet Mars = new Planet(PlanetId.SE_MARS, "Mars", DayOfWeek.Tuesday);
        public static Planet Jupiter = new Planet(PlanetId.SE_JUPITER, "Jupiter", DayOfWeek.Thursday);
        public static Planet Saturn = new Planet(PlanetId.SE_SATURN, "Saturn", DayOfWeek.Saturday);
        public static Planet Uranus = new Planet(PlanetId.SE_URANUS, "Uranus");
        public static Planet Neptune = new Planet(PlanetId.SE_NEPTUNE, "Neptune");
        public static Planet Pluto = new Planet(PlanetId.SE_PLUTO, "Pluto");

        private readonly PlanetId id;

        public PlanetId Id
        {
            get { return id; }
        }


        private readonly string name;

        public string Name
        {
            get { return name; }
        }


        private readonly DayOfWeek day;

        public DayOfWeek Day
        {
            get { return day; }
        } 


        private Planet(PlanetId id, string name, DayOfWeek day)
        {
            this.id = id;
            this.name = name;
            this.day = day;
        }

        private Planet(PlanetId, string name)
        {
            this.id = id;
            this.name = name;
            this.day = DayOfWeek.Sunday;
        }
    }
}
