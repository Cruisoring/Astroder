using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteHelper
{
    public enum ReferenceType
    {
        Nadia,
        Zenith,
        First,
        FirstFloor,
        FirstTop
    }

    public class Vibration
    {
        public SortedDictionary< Double, Double> TopsSpectrum {get; private set; }
        public SortedDictionary<Double, Double> BottomsSpectrum { get; private set; }
        public SortedDictionary<Double, Double> Spectrum
        {
            get
            {
                SortedDictionary<Double, Double> wave = new SortedDictionary<Double, Double>(TopsSpectrum);

                foreach (KeyValuePair<Double, Double> kvp in BottomsSpectrum)
                {
                    if (wave.ContainsKey(kvp.Key))
                        wave[kvp.Key] += kvp.Value;
                    else
                        wave.Add(kvp.Key, kvp.Value);
                }
                return wave;
            }
        }

        public Vibration(Outline outline, ReferenceType start)
        {
            TopsSpectrum = new SortedDictionary<double, Double>();
            BottomsSpectrum = new SortedDictionary<double, Double>();


        }

        //public Vibration(OutlineDictionary items)
        //{
        //    TopsSpectrum = new SortedDictionary<double, Double>();
        //    BottomsSpectrum = new SortedDictionary<double, Double>();

        //    List<Double> dates = items.Dates;
        //    Double lastTop = -1, lastBottom = -1, now, period;

        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        if (items.KindOf(i) == PivotType.Bottom)
        //        {
        //            now = items.Dates[i];
        //            if (lastBottom != -1)
        //            {
        //                period = now - lastBottom;
        //                if (BottomsSpectrum.ContainsKey(period))
        //                    BottomsSpectrum[period]++;
        //                else
        //                    BottomsSpectrum.Add(period, 1);
        //            }
        //            lastBottom = now;
        //        }
        //        else if (items.KindOf(i) == PivotType.Top)
        //        {
        //            now = items.Dates[i];
        //            if (lastTop != -1)
        //            {
        //                period = now - lastTop;
        //                if (TopsSpectrum.ContainsKey(period))
        //                    TopsSpectrum[period]++;
        //                else
        //                    TopsSpectrum.Add(period, 1);
        //            }
        //            lastTop = now;
        //        }
        //    }
        //}
    }
}
