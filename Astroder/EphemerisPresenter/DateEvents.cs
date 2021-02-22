using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public enum ConcernedEvent
    {
        None,
        Rectascension,
        Declination,
        Distance,
        Relations,
        All
    }

    public class DateEvents : IEnumerable<PlanetEvents>
    {
        #region Properties
        public Ephemeris TheEphemeris { get { return Ephemeris.Geocentric; } }

        public DateTimeOffset Date { get; set; }

        List<Relation> Relations { get; set; }

        Dictionary<PlanetId, PlanetEvents> Positions { get; set; }

        public PlanetEvents this[PlanetId id]
        {
            get { return (Positions.ContainsKey(id)) ? Positions[id] : null; }
        }

        #endregion

        #region Constructors

        public DateEvents(DateTimeOffset date)
        {
            try
            {
                if (date.TimeOfDay == TimeSpan.Zero && date.Offset == TimeSpan.Zero)
                    Date = date;
                else
                    Date = new DateTimeOffset(date.UtcDateTime.Date, TimeSpan.Zero);

                MatchRules during = new MatchRules(Date, SearchMode.WithinTheDay);
                Relations = Ephemeris.RelationsWithin(during);

                Positions = new Dictionary<PlanetId, PlanetEvents>();

                for (PlanetId id = PlanetId.SE_SUN; id <= PlanetId.SE_PLUTO; id++)
                {
                    Positions.Add(id, new PlanetEvents(TheEphemeris, Date, id));
                }
                Positions.Add(PlanetId.SE_NORTHNODE, new PlanetEvents(TheEphemeris, Date, PlanetId.SE_NORTHNODE));
                Positions.Add(PlanetId.SE_CHIRON, new PlanetEvents(TheEphemeris, Date, PlanetId.SE_CHIRON));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        #endregion


        #region IEnumerable<PositionAttributes> 成员

        public IEnumerator<PlanetEvents> GetEnumerator()
        {
            return Positions.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
