using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
//using System.Collections.ObjectModel;

namespace StockQuote
{
    [Serializable]
    public class DailyQuote : INotifyPropertyChanged
    {
        //private static string Undefined = "Undefined";

        public string Name { get; set; }

        public string Description { get; set; }

        public List<DayItem> DailyData { get; set; }

        public int CelingIndex { get; private set; }

        public int FloorIndex { get; private set; }

        public int Count { get { return DailyData.Count; } }

        public double Ceiling { get { return DailyData[CelingIndex].High; } }

        public double Floor { get { return DailyData[FloorIndex].Low; } }

        public DateTime Since { get { return DailyData[0].Date; } }

        public DateTime Until { get { return DailyData[Count - 1].Date; } }

        public DayItem this[int index]
        {
            get {
                if (index < 0 || index > Count)
                    return null;
                else
                    return DailyData[index];
            }
        }

        //public DailyQuote()
        //{
        //    Description = Undefined;
        //    Name = Undefined;
        //    DailyData = new List<DayItem>();
        //}

        public DailyQuote(String name, List<DayItem> items)
        {
            Name = name;
            DailyData = items;

            DailyData.Sort();

            getTopBottom();

        }

        //public DailyQuote(string[] fileNames)
        //{
        //    int pos = fileNames[0].LastIndexOf('\\');
        //    string dirName = fileNames[0].Substring(0, pos);

        //    pos = dirName.LastIndexOf('\\');
        //    Name = dirName.Substring(pos + 1, dirName.Length - pos - 1);
        //    DailyData = new List<DayItem>();

        //    List<DayItem> newItems = null;

        //    foreach (string fileName in fileNames)
        //    {
        //        newItems = TextImporter.Import(fileName);
        //        foreach (DayItem item in newItems)
        //        {
        //            DailyData.Add(item);
        //        }
        //    }

        //    DailyData.Sort();

        //    getCeilingFloor();

        //}

        private void getTopBottom()
        {
            double highest = double.MinValue, lowest = double.MaxValue;

            for (int i = 0; i < Count; i++)
            {
                if (highest < DailyData[i].High)
                {
                    highest = DailyData[i].High;
                    CelingIndex = i;
                }

                if (lowest > DailyData[i].Low)
                {
                    lowest = DailyData[i].Low;
                    FloorIndex = i;
                }
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}: {2} dataCollection, from {3} to {4}", Name, 
                (DailyData.Count == 0 ? "": DailyData[0].TheRecordType.ToString()), Count, DailyData[0].Date, DailyData.Last().Date);
        }

        public void Summary()
        {

        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
