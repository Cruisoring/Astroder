using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum MajorType
    {
        Unknown,
        Low,
        High,
        Bottom,
        Top
    }

    [Serializable]
    public class Major : IOutline
    {
        public MajorType Kind { get; set; }

        public Quote Quote { get; set; }

        public DateTimeOffset Date { get { return Quote.Date; } }

        public Double JulianDay { get { return Quote.JulianDay; } }

        public double Price { 
            get { 
                switch(Kind)
                {
                    case MajorType.Low:
                    case MajorType.Bottom:
                        return Quote.Low;
                    case MajorType.High:
                    case MajorType.Top:
                        return Quote.High;
                    case MajorType.Unknown:
                    default:
                        return Quote.Close;
                }
            } 
        }

        public OutlineItem Simplified
        {
            get { return new OutlineItem(Date, Price); }
        }

        public Major(MajorType kind, Quote quote)
        {
            Kind = kind;
            Quote = quote;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}@{2}", Kind, Price, Date);
        }
    }
}
