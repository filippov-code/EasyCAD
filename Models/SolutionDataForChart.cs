using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.CodeDom;
using LiveCharts.Configurations;
using System.Configuration;

namespace EasyCAD.Models
{
    public class SolutionDataForChart
    {
        private readonly Construction construction;
        #region Properties
        public SeriesCollection NxSeries { get; set; } = new()
        {
            new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    Stroke = new SolidColorBrush(Color.FromRgb(33, 149, 242)),
                    Fill = new SolidColorBrush(Color.FromRgb(33, 149, 242)){ Opacity = 0.35d},
                    LineSmoothness = 0
                }
        };
        public AxesCollection NxXAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };
        public AxesCollection NxYAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };

        public SeriesCollection oxSeries { get; set; } = new()
        {
            new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    Stroke = new SolidColorBrush(Color.FromRgb(254, 192, 7)),
                    Fill = new SolidColorBrush(Color.FromRgb(254, 192, 7)) { Opacity = 0.35},
                    LineSmoothness = 0,
                }
        };
        public AxesCollection oxXAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };
        public AxesCollection oxYAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };

        public SeriesCollection UxSeries { get; set; } = new()
        {
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>
                {
                    new(0, 0),
                    new(1, 1),
                    new(2, 2),
                    new(3, 3),
                    new(double.NaN, double.NaN)
                }
            },
            new LineSeries
            {
                Values = new ChartValues<ObservablePoint>
                {
                    new(3, 3),
                    new(4, 3),
                    new(5, 2),
                    new(6, 1),
                    new(7, 0),
                    new(double.NaN, double.NaN)
                }
            }
        };
        public AxesCollection UxXAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };
        public AxesCollection UxYAxis { get; set; } = new()
        {
            new Axis()
            {
                ShowLabels = false,
            }
        };
        #endregion

        public SolutionDataForChart(Construction construction)
        {
            this.construction = construction;
        }

        public void SetSolution(Solution solution)
        {
            NxSeries[0].Values.Clear();
            oxSeries[0].Values.Clear();

            float l = 0;
            for (int i = 0; i < solution.Nsolutions.RowsCount; i++)
            {
                NxSeries[0].Values.Add(new ObservablePoint(l, solution.Nsolutions[i, 0]));
                oxSeries[0].Values.Add(new ObservablePoint(l, solution.osolutions[i, 0]));
                l += construction.Rods[i].L;
                NxSeries[0].Values.Add(new ObservablePoint(l, solution.Nsolutions[i, 1]));
                oxSeries[0].Values.Add(new ObservablePoint(l, solution.osolutions[i, 1]));
                NxSeries[0].Values.Add(new ObservablePoint(double.NaN, double.NaN));
                oxSeries[0].Values.Add(new ObservablePoint(double.NaN, double.NaN));
            }

            foreach (var series in UxSeries)
                series.Configuration = null;
            UxSeries.Clear();

            float L = 0;
            foreach (var rod in construction.Rods)
            {
                UxSeries.Add(
                    new LineSeries
                    {
                        Values = new ChartValues<ObservablePoint>(),
                        Stroke = Brushes.Gray,
                        Fill = new SolidColorBrush(Color.FromRgb(128, 128, 128)) { Opacity = 0.35 },
                        LineSmoothness = 0
                    });
                
                for (float progress = 0; Math.Round(progress, 2) <= rod.L; progress += 0.1f)
                {
                    UxSeries.Last().Values.Add(new ObservablePoint(L + progress, solution.GetUxSolution(rod, progress)));
                }
                L += rod.L;
                UxSeries.Last().Values.Add(new ObservablePoint(double.NaN, double.NaN));

                UxSeries.Last().Configuration = 
                    Mappers.Xy<ObservablePoint>()
                           .X((value, index) => Math.Round(value.X, 2))
                           .Y((value, index) => value.Y)
                           .Fill((value, index) => index == 0 || value == UxSeries.Last().Values[UxSeries.Last().Values.Count - 2] ? null : Brushes.Transparent)
                           .Stroke((value, index) => index == 0 || value == UxSeries.Last().Values[UxSeries.Last().Values.Count - 2] ? Brushes.Gray : Brushes.Transparent);
            }
        }
        //public SeriesCollection Series { get; set; } = new()
        //{
        //    //new LineSeries
        //    //{
        //    //    Values = new ChartValues<int>{1, 2, 5, 4, 1, 7, 3, 7, 8, 4, 1}
        //    //},
        //    //new LineSeries
        //    //{
        //    //    Values = new ChartValues<ObservablePoint>
        //    //    {
        //    //        new ObservablePoint(0, 4),
        //    //        new ObservablePoint(1, 3),
        //    //        new ObservablePoint(3, 8),
        //    //        new (double.NaN, double.NaN),
        //    //        new ObservablePoint(3, 5),
        //    //        new ObservablePoint(18, 6),
        //    //        new ObservablePoint(20, 12)
        //    //    },
        //    //    LineSmoothness = 0
        //    //}
        //};
    }
}
