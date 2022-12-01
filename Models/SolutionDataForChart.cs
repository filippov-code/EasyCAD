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
                    Title = "Nx",
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
                    Title = "ox",
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

        public SeriesCollection UxSeries { get; set; } = new();
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
            for (int i = 0; i < solution.NxMatrix.RowsCount; i++)
            {
                NxSeries[0].Values.Add(new ObservablePoint(l, solution.NxMatrix[i, 0]));
                oxSeries[0].Values.Add(new ObservablePoint(l, solution.OxMatrix[i, 0]));
                l += construction.Rods[i].L;
                NxSeries[0].Values.Add(new ObservablePoint(l, solution.NxMatrix[i, 1]));
                oxSeries[0].Values.Add(new ObservablePoint(l, solution.OxMatrix[i, 1]));
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
                        Title = "Ux",
                        Values = new ChartValues<ObservablePoint>(),
                        Stroke = Brushes.Gray,
                        Fill = new SolidColorBrush(Color.FromRgb(128, 128, 128)) { Opacity = 0.35 },
                        LineSmoothness = 1
                    });
                
                for (float progress = 0; Math.Round(progress, 2) <= rod.L; progress += 0.1f)
                {
                    UxSeries.Last().Values.Add(new ObservablePoint(Math.Round(L + progress, 3), solution.GetUxSolution(rod, progress)));
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
    }
}
