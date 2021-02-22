using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    [Serializable]
    public class OutlineItem : IOutline
    {
        public DateTimeOffset Date { get; set; }
        public Double Price { get; set; }
        public OutlineItem(DateTimeOffset date, Double price)
        {
            Date = date;
            Price = price;
        }
    }

    public interface IOutline
    {
        DateTimeOffset Date {get;}
        Double Price {get;}
    }
}
