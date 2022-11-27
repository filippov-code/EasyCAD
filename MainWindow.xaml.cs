using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Construction Construction { get; private set; } = new();


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

        public MainWindow()
        {
            InitializeComponent();

            

            DataContext = this;
            
            //Series.Add(new LineSeries
            //{
            //    Values = new ChartValues<ObservablePoint>(),
            //    Configuration = Mappers.Xy<ObservablePoint>()
            //                        .X((value, index) => index / 100f)
            //                        .Y((value, index) => value.Y)
            //                        .Fill((value, index) => index == 0 || index == Series.Last().Values.Count - 1 ? null : Brushes.Transparent)
            //                        .Stroke((value, index) => index == 0 || index == Series.Last().Values.Count - 1 ? Brushes.Red : Brushes.Transparent)

            //});

            //for (double i = 0; i <= 20; i += 0.01d)
            //{
            //    Series.Last().Values.Add(new ObservablePoint(i, Math.Sin(i)));
            //}

        }

        private object GetChartMapper(ISeriesView series, float d)
        {
            return Mappers.Xy<ObservablePoint>()
                                    .X((value, index) => index / d)
                                    .Y((value, index) => value.Y)
                                    .Fill((value, index) => index == 0 || index == series.Values.Count - 1 ? null : Brushes.Transparent)
                                    .Stroke((value, index) => index == 0 || index == series.Values.Count - 1 ? Brushes.Red : Brushes.Transparent);

        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = new();
            rec.Width = canvas.ActualWidth /2;
            rec.Height = canvas.ActualHeight/2;
            rec.Stroke = Brushes.Black;
            rec.Fill = Brushes.Red;
            canvas.Children.Add(rec);
            Canvas.SetTop(rec, canvas.ActualHeight/4);
            Canvas.SetLeft(rec, canvas.ActualWidth / 4);
            Canvas.SetRight(rec, canvas.ActualWidth / 4);
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle rect = new();
            rect.Width = canvas.ActualWidth /2;
            rect.Height = canvas.ActualHeight/3;
            rect.Stroke = Brushes.Black;
            rect.Fill = Brushes.Blue;
            canvas.Children.Add(rect);
            Canvas.SetTop(rect, 0);
            Canvas.SetLeft(rect, 0);

            Rectangle rect2 = new()
            {
                Width = canvas.ActualWidth / 2,
                Height = canvas.ActualHeight / 3,
                Stroke = Brushes.Black,
                Fill = Brushes.Red
            };
            canvas.Children.Add(rect2);
            Canvas.SetTop(rect2, 0);
            Canvas.SetLeft(rect2, canvas.ActualWidth / 2);
        }

        private void AddRod(object sender, RoutedEventArgs e)
        {
            float L = float.Parse(newLTextBox.Text);
            float A = float.Parse(newATextBox.Text);
            float E = float.Parse(newETextBox.Text);
            float o = float.Parse(newOTextBox.Text);
            this.Construction.Rods.Add( new Rod(0, L, A, E, o) );
        }

        private void RemoveRod(object sender, RoutedEventArgs e)
        {
            Rod selectedRod = (Rod)rodsDataGrid.SelectedItem;
            this.Construction.Rods.Remove(selectedRod);
        }

        private void AddDistributedForce(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveDistributedForce(object sender, RoutedEventArgs e)
        {

        }

        private void AddConcentratedForce(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveConcentratedForce(object sender, RoutedEventArgs e)
        {

        }

        private void Solve(object sender, RoutedEventArgs e)
        {

        }

        private void SolveInPoint(object sender, RoutedEventArgs e)
        {

        }

        //Drawing
        private void DrawConstruction()
        {
            double unitsPerMeter = canvas.ActualWidth / Construction.Rods.Sum(x => x.L);
            double unitPerArea = canvas.ActualHeight / Construction.Rods.Max(x => x.A);


        }
    }
}
