using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StockQuote;
//using AmCharts.Windows.Stock;

namespace ChartTest
{
    /// <summary>
    /// SimpleDataBoundChart.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleDataBoundChart : Window
    {
        public List<DayRecord> Data { get; set; }

        private void AddData()
        {
            Data = new List<DayRecord>();
            Random rnd = new Random();

            DateTime currentDate = new DateTime(2009, 3, 8);
            double baseValue = 20 + rnd.NextDouble() * 10;
            for (int i = 0; i < 1000; i++)
            {
                currentDate = currentDate.AddDays(rnd.Next(1, 3));
                double value = baseValue + rnd.NextDouble() * 6 - 3;
                Data.Add(new DayRecord()
                {
                    Time = currentDate,
                    Open = value + rnd.NextDouble() * 4 - 2,
                    High = value + 2 + rnd.NextDouble() * 3,
                    Low = value - 2 - rnd.NextDouble() * 3,
                    Close = value + rnd.NextDouble() * 4 - 2,
                    Volume = rnd.NextDouble() * 300
                }
                );
                baseValue = value < 6 ? value + rnd.NextDouble() * 3 : value;
            }
        }


        public SimpleDataBoundChart()
        {
            base.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            AddData();
        }
    }
}
