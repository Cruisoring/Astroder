using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public class Similarity
    {
        public static Similarity Between(IPlanetEvent event1, IPlanetEvent event2)
        {
            if (event1.Category == event2.Category && event1.Who == event2.Who)
            {
                if (event1.Kind == event2.Kind)
                    return new Similarity(event1.Kind, event1.Who);
                else
                    return new Similarity(event1.Category, event1.Who);
            }
            else
                return null;
        }

        public PlanetEventFlag Category { get; private set; }

        public PlanetId Who { get; private set;  }

        public PlanetPair Pair { get; private set; }

        public Similarity(PlanetEventFlag category, PlanetId who)
        {
            Category = category;
            Who = who;
        }

        public Similarity(PlanetPair pair)
        {
            Pair = pair;
            Category = PlanetEventFlag.AspectCategory;
        }

        public override string ToString()
        {
            if (Category == PlanetEventFlag.AspectCategory)
            {
                return String.Format("{0} form aspect.", Pair);
            }
            else
                return string.Format("{0} of {1}", Category, Who);
        }
    }
}
