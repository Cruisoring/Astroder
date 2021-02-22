using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EphemerisCalculator
{
    public interface IPlanetEvent : IComparable<IPlanetEvent>
    {
        PlanetEventFlag Category { get; }
        PlanetEventFlag Kind { get; }
        DateTimeOffset When { get; }
        double OADate { get; }
        Position Where { get; }
        PlanetId Who { get; }
        String ShortDescription { get; }
        bool IsSimilar(IPlanetEvent another);
        Similarity SimilarityWith(IPlanetEvent another);
    }


    [Serializable]
    //[Flags]
    public enum PlanetEventFlag : uint
    {
        //None                        =   0,
        CategoryMask                =   0xF0000000,
        KindMask                    =   0x0F000000,
        FirstPlanetMask             =   0x00FF0000,
        AspectTypeMask              =   0x0000FF00,
        SecondPlanetmask            =   0x000000FF,

        // Eclipses and Occultation
        EclipseOccultationCategory  =   0x10000000,
        SolarEclipse                =   0x11000000,
        LunarEclipse                =   0x12000000,
        LunarOccultation            =   0x13000000,

        // Rectascension related planet events
        DirectionalCategory         =   0x20000000,
        Direct                      =   0x22000000,    //Resume direct movement on ecliptic
        Retrograde                  =   0x23000000,    //Begin retrograde movement on ecliptic
        PassDirectPoint             =   0x24000000,     //Pass the rectascension where the planet will resume direct in future,
                                                        //This happen before the retrograde movement even happen
        RevisitRetrogradePoint      =   0x25000000,     //Pass the rectascension where the planet begin retrograde movement
                                                        // This happens after the planet resume direct movement.

        SignChangedCategory         =   0x30000000,
        SignEntered                 =   0x31000000,    //The Planet enters another sign, and the longitude speed > 0
        SignRetreated               =   0x32000000,     //The planet retreat to adjacent sign, the longitude speed < 0

        // Declination related PlanetEvent
        DeclinationCategory         =   0x40000000,
        OnEquator                   =   0x41000000,      //The planet is crossing the celestial equator
        NorthMost                   =   0x42000000,      //The planet is highest on vertical dimension
        SouthMost                   =   0x43000000,      //The planet is lowest on vertical dimension

        //Transcension related PlanetEvent
        //TranscensionCategory        =   0x40000000,
        //KeyPointTranscension        =   0x41000000,
        //DirectPointTranscension     =   0x42000000,
        //RetrogradePointTranscension =   0x43000000,
        
        //Aspect events between two planets
        AspectCategory              =   0x50000000,
        HorizontalAspected          =   0x51000000,
        VerticalAspected            =   0x52000000,
        Transcended                 =   0x53000000,
        AspectRecurred              =   0x54000000
    }

    [Serializable]
    public abstract class PlanetEvent : IPlanetEvent
    {
        public const char Enter = '\u21A6';
        public const char Retreate = '\u21A4';
        public const char Direct = '\u21AA';
        public const char Retrograde = '\u21A9';
        public const char Ceiling = '\u2229';
        public const char Nadia = '\u222A';

        public static int CompareByPlanetId(IPlanetEvent event1, IPlanetEvent event2)
        {
            return event1.Who - event2.Who;
        }

        public static Dictionary<PlanetEventFlag, char> PlanetEventCategorySymbols = new Dictionary<PlanetEventFlag, char>
        {
            {PlanetEventFlag.EclipseOccultationCategory, '\u2600'},
            {PlanetEventFlag.DirectionalCategory, '\u2277'},
            {PlanetEventFlag.SignChangedCategory, '\u2355'},
            {PlanetEventFlag.DeclinationCategory, '\u223F'},
            {PlanetEventFlag.AspectCategory, '\u235F'}
        };

        public static Dictionary<char, PlanetEventFlag> PlanetEventSymbolToCategory = new Dictionary<char, PlanetEventFlag>();

        static PlanetEvent()
        {
            foreach (KeyValuePair<PlanetEventFlag, char>kvp in PlanetEventCategorySymbols)
            {
                PlanetEventSymbolToCategory.Add(kvp.Value, kvp.Key);
            }
        }

        #region Variables definition

        public PlanetEventFlag Category { get { return (Kind & PlanetEventFlag.CategoryMask); } }

        public PlanetEventFlag Kind { get; protected set; }

        public DateTimeOffset When { get; protected set; }

        public double OADate { get { return When.UtcDateTime.ToOADate(); } }

        public PlanetId Who { get; protected set; }

        public Position Where { get; protected set; }

        public abstract string ShortDescription { get; }

        #endregion

        public PlanetEvent(PlanetId id, DateTimeOffset time, Position pos)
        {
            Who = id;
            When = time;
            Where = pos;
        }

        public virtual bool IsSimilar(IPlanetEvent another)
        {
            return this.Who == another.Who && this.Category == another.Category;
        }

        public virtual Similarity SimilarityWith(IPlanetEvent another)
        {
            return Similarity.Between(this, another);
        }

        #region IComparable<IPlanetEvent> 成员

        public int CompareTo(IPlanetEvent other)
        {
            if (this.When == other.When)
                return 0;
            else if (this.When > other.When)
                return 1;
            else
                return -1;
        }

        #endregion

    }

    [Serializable]
    public class RectascensionEvent : PlanetEvent
    {
        #region Variables definition

        public double Longitude { get { return Where.Longitude; } }

        public override String ShortDescription 
        {
            get
            {
                return String.Format("{0}{1}", Planet.SymbolOf(Who), Kind == PlanetEventFlag.Retrograde ? Retrograde : Direct); 
            }
        }

        #endregion

        public RectascensionEvent(PlanetId id, DateTimeOffset time, Position pos, PlanetEventFlag kind)
            : base(id, time, pos)
        {
            if (kind != PlanetEventFlag.Direct && kind != PlanetEventFlag.Retrograde)
                throw new Exception();

            Kind = kind;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} @{2} on {3}",
                Planet.SymbolOf(Who),
                Kind == PlanetEventFlag.Retrograde ? Retrograde : Direct,
                Longitude.ToString("F1", null),
                //Where.TheRectascension.ToString(),
                When.ToString("MM-dd HH:mm"));
        }
    }

    [Serializable]
    public class SignEntrance : PlanetEvent
    {
        #region Variables definition

        public bool IsRetrograde { get { return Where.LongitudeVelocity < 0; } }

        public Sign To { get; private set; }

        public Sign From { get { return IsRetrograde ? To.Next : To.Previous; } }

        public override String ShortDescription 
        {
            get
            {
                return String.Format("{0}{1}{2}", Planet.SymbolOf(Who), IsRetrograde ? Retreate : Enter, To.Symbol); 
            }
        }

        #endregion

        public SignEntrance(PlanetId id, DateTimeOffset time, Position pos)
            : base(id, time, pos)
        {
            Kind = pos.LongitudeVelocity >= 0 ? PlanetEventFlag.SignEntered : PlanetEventFlag.SignRetreated;
            double cuspDeg = Math.Round(pos.Longitude / 30) * 30;
            To = Sign.SignOf(pos.LongitudeVelocity >= 0 ? cuspDeg : cuspDeg - 30);
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} on {3}",
                Planet.SymbolOf(Who),
                IsRetrograde ? "retreats" : "enters",
                To.Symbol,
                When.ToString("MM-dd HH:mm"));
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            if (! base.IsSimilar(another))
                return false;

            SignEntrance signChange = another as SignEntrance;

            if (signChange == null)
                return false;

            return this.To == signChange.To && this.IsRetrograde == signChange.IsRetrograde;
        }

        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;
            else
                return new Similarity(this.Category, Who);
        }
    }

    [Serializable]
    public class DeclinationEvent : PlanetEvent
    {
        #region Variables definition

        public double Latitude { get { return Where.Latitude; } }

        public bool IsRising { get { return Where.LatitudeVelocity > 0; } }

        public override String ShortDescription { get { return String.Format("{0} {1}", Planet.SymbolOf(Who), 
            Kind == PlanetEventFlag.OnEquator ? (IsRising ? "0S" : "0N") 
            : (Kind == PlanetEventFlag.NorthMost ? '\u22C2'.ToString() : '\u22C3'.ToString())); } }

        #endregion

        public DeclinationEvent(PlanetId id, DateTimeOffset time, Position pos, PlanetEventFlag kind)
            : base(id, time, pos)
        {
            if (kind < PlanetEventFlag.OnEquator || kind > PlanetEventFlag.SouthMost )
                throw new Exception();

            Kind = kind;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} @ {2:F2} on {3}",
                Planet.SymbolOf(Who),
                ((Kind == PlanetEventFlag.OnEquator) ? (IsRising ? "0S" : "0N") : (Kind== PlanetEventFlag.NorthMost ? Ceiling.ToString() : Nadia.ToString())), 
                Latitude,
                When.ToString("MM-dd HH:mm"));
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            if (!base.IsSimilar(another) || !(another is DeclinationEvent))
                return false;

            DeclinationEvent anotherDecl = another as DeclinationEvent;

            return (anotherDecl.Kind == this.Kind);
            //    return true;
            //else if (anotherDecl.Kind == PlanetEventFlag.OnEquator || this.Kind == PlanetEventFlag.OnEquator)
            //    return false;
            //else
            //    return true;
        }

        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(Kind, Who);
        }
    }

    [Serializable]
    public class ExactAspectEvent : PlanetEvent, IComparable<ExactAspectEvent>
    {
        public static int ComparsionByPlanetPair(ExactAspectEvent event1, ExactAspectEvent event2)
        {
            return event1.Pair.CompareTo(event2.Pair);
        }

        public PlanetPair Pair { get; private set; }

        public PlanetId Interior { get { return Pair.Interior; } }

        public PlanetId Exterior { get { return Pair.Exterior; } }

        public Position InteriorPosition { get { return Where; } }

        public Position ExteriorPosition { get; private set; }

        public Aspect TheAspect { get; private set; }

        public override String ShortDescription
        {
            get
            {
                return String.Format("{0}{1}{2}", Planet.SymbolOf(Interior), TheAspect.Symbol, Planet.SymbolOf(Exterior));
            }
        }

        public ExactAspectEvent(DateTimeOffset time, Position pos1, Position pos2, Aspect theAspect, PlanetEventFlag kind)
            : base(pos1.Owner, time, pos1)
        {
            Pair = new PlanetPair(pos1.Owner, pos2.Owner);

            if (Interior != Where.Owner)
            {
                Who = Pair.Interior;
                Where = pos2;
            }

            ExteriorPosition = pos1.Owner > pos2.Owner ? pos1 : pos2;
            TheAspect = theAspect;
            Kind = kind;
        }

        public override string ToString()
        {
            //return String.Format("{0} {1} {2} on {3}", InteriorPosition.ToString("Astro", null), TheAspect, ExteriorPosition.ToString("Astro", null), When);
            return String.Format("{0}({1:F1})<{2}>{3}({4:F1}) on {5}", Planet.SymbolOf(Interior), InteriorPosition.Longitude,
                TheAspect.Degrees, Planet.SymbolOf(Exterior), ExteriorPosition.Longitude, When.ToString("MM-dd HH:mm"));
        }

        public override bool IsSimilar(IPlanetEvent another)
        {
            if (!base.IsSimilar(another) || !(another is ExactAspectEvent))
                return false;

            ExactAspectEvent anotherAspect = another as ExactAspectEvent;

            return anotherAspect.Pair == this.Pair;
        }

        public override Similarity SimilarityWith(IPlanetEvent another)
        {
            if (!IsSimilar(another))
                return null;

            return new Similarity(Pair);
        }

        #region IComparable<ExactAspectEvent> 成员

        public int CompareTo(ExactAspectEvent other)
        {
            return this.Pair.CompareTo(other.Pair)* 10000 + this.When.Date.ToOADate().CompareTo(other.When.Date.ToOADate());
        }

        #endregion
    }

    [Serializable]
    public class AspectRecurrenceEvent : PlanetEvent, IEquatable<AspectRecurrenceEvent>, IComparable<AspectRecurrenceEvent>
    {
        public PlanetPair Pair { get; private set; }

        public PlanetId Interior { get { return Pair.Interior; } }

        public PlanetId Exterior { get { return Pair.Exterior; } }

        public Position InteriorPosition { get { return Where; } }

        public Position ExteriorPosition { get; private set; }

        public Double RefAspectDegree { get; private set; }

        public Aspect TheAspect { get; private set; }

        public override String ShortDescription
        {
            get
            {
                return String.Format("{0}<{1:F0}>{2}", Planet.SymbolOf(Interior), InteriorPosition.Longitude - ExteriorPosition.Longitude, Planet.SymbolOf(Exterior));
            }
        }

        public AspectRecurrenceEvent(DateTimeOffset time, Position pos1, Position pos2, Double refAspectDegree)
            : base(pos1.Owner, time, pos1)
        {
            Pair = new PlanetPair(pos1.Owner, pos2.Owner);

            if (Interior != Where.Owner)
            {
                Who = Pair.Interior;
                Where = pos2;
            }

            ExteriorPosition = pos1.Owner > pos2.Owner ? pos1 : pos2;
            RefAspectDegree = refAspectDegree;
            double dif = Math.Round(InteriorPosition.Longitude - ExteriorPosition.Longitude - refAspectDegree + 360) % 360;
            if (Aspect.All.ContainsKey(dif))
                TheAspect = Aspect.All[dif];
            else
                TheAspect = null;

            Kind = PlanetEventFlag.AspectRecurred;
        }

        public override string ToString()
        {
            if (TheAspect != null)
                return String.Format("{0}-{1}({2:F0}) <{3:F0}> {4:F1}",
                    Planet.SymbolOf(Interior), Planet.SymbolOf(Exterior), InteriorPosition.Longitude - ExteriorPosition.Longitude, 
                    TheAspect.Degrees, RefAspectDegree);
            else
                return String.Format("{0}-{1}({2:F0}) <{3:F0}> {4:F1}",
                    Planet.SymbolOf(Interior), Planet.SymbolOf(Exterior), InteriorPosition.Longitude - ExteriorPosition.Longitude,
                    Math.Round(InteriorPosition.Longitude - ExteriorPosition.Longitude - RefAspectDegree + 360) % 360, RefAspectDegree);
        }


        #region IEquatable<ExactAspectEvent> 成员

        public bool Equals(AspectRecurrenceEvent other)
        {
            return this.Pair.Equals(other.Pair) && this.When.Date == other.When.Date;
        }

        #endregion

        #region IComparable<AspectRecurrenceEvent> 成员

        public int CompareTo(AspectRecurrenceEvent other)
        {
            return this.Pair.CompareTo(other.Pair) * 10000 + this.When.Date.ToOADate().CompareTo(other.When.Date.ToOADate());
        }

        #endregion
    }

    [Serializable]
    public class TranscensionEvent : PlanetEvent
    {
        public Rectascension RefRecscension { get; private set; }

        public Double AspectDegree { get; private set; }

        public override String ShortDescription
        {
            get
            {
                if (Aspect.All.ContainsKey(AspectDegree))
                    return String.Format("{0}{1}{0}", Planet.SymbolOf(Who), Aspect.All[AspectDegree].Symbol);
                else
                    return String.Format("{0}<{1:F2}>{0}", Planet.SymbolOf(Who), AspectDegree);
            }
        }


        public TranscensionEvent(PlanetId id, DateTimeOffset time, Position pos, Rectascension refDegree, Double theAspectDegree)
            : base(id, time, pos)
        {
            RefRecscension = refDegree;
            AspectDegree = theAspectDegree;
            Kind = PlanetEventFlag.Transcended;
        }

        public override string ToString()
        {
            //if (Aspect.All.ContainsKey(AspectDegree))
            //    return String.Format("{0}@{1} {2} with {0} on {3}",
            //        Planet.All[Who], Where.ToString("Astro", null), Aspect.All[AspectDegree], When);
            //else
                return String.Format("{0}@{1} <{2:F2}> with {3} on {4}",
                    Planet.All[Who], Where.ToString("Astro", null), AspectDegree, RefRecscension, When);
        }
    }
}
