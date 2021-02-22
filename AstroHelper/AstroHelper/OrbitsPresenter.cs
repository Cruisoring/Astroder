using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumberHelper;
using QuoteHelper;

namespace AstroHelper
{
    [Serializable]
    public class OrbitsPresenter
    {
        public OrbitsCollection Orbits { get; set; }

        public AnglePresenter Presenter { get; set; }

        public TimeToAngle TimeTranslator { get; set; }

        public PriceToAngle PriceTranslator { get; set; }

        public Dictionary<PlanetId, PlanetManager> Controller = null;

        public OrbitsPresenter(DateTimeOffset start, DateTimeOffset end)
        {
            Orbits = new OrbitsCollection(start, end);
            Presenter = AnglePresenter.Natural;
            TimeTranslator = TimeToAngle.Default;
            PriceTranslator = PriceToAngle.DimeNatural;

            Controller = new Dictionary<PlanetId, PlanetManager>();

            for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
            {
                PlanetManager manager =new PlanetManager(id);
                Controller.Add(id, manager);
                //manager.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(planetsOrbitsShownChanged);
            }

        }

        //void planetsOrbitsShownChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    PlanetId id = (PlanetId)sender;
        //    OrbitInfoType type = (OrbitInfoType)Enum.Parse(typeof(OrbitInfoType), e.PropertyName);

        //    if(Controller[id][type] == false)
        //    {
        //        List<DateTimeOffset> timeStamps = Orbits.DayByDay;
        //        List<Double> orbit = Orbits[id, type];
        //    }
        //}
    }
}
