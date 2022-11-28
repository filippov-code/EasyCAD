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
            Construction.AddRod( new Rod(Construction.Rods.Count + 1, L, A, E, o) );
        }

        private void RemoveRod(object sender, RoutedEventArgs e)
        {
            Rod selectedRod = (Rod)rodsDataGrid.SelectedItem;
            Construction.RemoveRod(selectedRod);
        }

        private void AddDistributedForce(object sender, RoutedEventArgs e)
        {
            int number = int.Parse(newQxNumberTextBox.Text);
            float qx = float.Parse(newQxTextBox.Text);
            Construction.SetDistributedStrain(number, qx);
        }

        private void RemoveDistributedForce(object sender, RoutedEventArgs e)
        {
            DistributedStrain selectedConcentratedStrain = (DistributedStrain)distStrainsDataGrid.SelectedItem;
            Construction.RemoveDistributedStrain(selectedConcentratedStrain);
        }

        private void AddConcentratedStrain(object sender, RoutedEventArgs e)
        {
            int number = int.Parse(newFNumberTextBox.Text);
            float F = float.Parse(newFTextBox.Text);
            Construction.SetConcentratedStrain(number, F);
        }

        private void RemoveConcentratedStrain(object sender, RoutedEventArgs e)
        {
            ConcentratedStrain selectedConcentratedStrain = (ConcentratedStrain)conStrainsDataGrid.SelectedItem;
            Construction.RemoveConcentratedStrain(selectedConcentratedStrain);
        }

        private void Solve(object sender, RoutedEventArgs e)
        {
            DrawConstruction();
        }

        private void SolveInPoint(object sender, RoutedEventArgs e)
        {

        }

        //Drawing
        private void DrawConstruction()
        {
            canvas.Children.Clear();

            double unitsPerMeter = canvas.ActualWidth / Construction.Rods.Sum(x => x.L);
            double unitsPerArea = canvas.ActualHeight / Construction.Rods.Max(x => x.A);
            double middle = canvas.ActualHeight / 2;

            //Рисование стержней
            double l = 0;
            foreach (Rod rod in Construction.Rods)
            {
                Rectangle rect = new()
                {
                    Width = rod.L * unitsPerMeter,
                    Height = rod.A * unitsPerArea,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.White
                };
                canvas.Children.Add(rect);
                Canvas.SetTop(rect, middle - rod.A * unitsPerArea / 2);
                Canvas.SetLeft(rect, l * unitsPerMeter);
                l += rod.L;
            }

            //Рисование сосредоточенных нагрузок
            foreach (var strain in Construction.ConcentratedStrains)
            {
                l = Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter;
                Line line = new()
                {
                    X1 = l,
                    Y1 = middle,
                    X2 = (strain.Force > 0)? l + unitsPerMeter * 0.15f: l - unitsPerMeter * 0.15f,
                    Y2 = middle,
                    Stroke = Brushes.Red,
                    StrokeThickness = 25,
                    StrokeEndLineCap = PenLineCap.Triangle,
                };
                canvas.Children.Add(line);
            }

            //Рисование распределенных нагрузок
            foreach (var strain in Construction.DistributedStrains)
            {
                l = Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter;
                Line line = new()
                {
                    X1 = l,
                    Y1 = middle,
                    X2 = (strain.qx > 0)? l + unitsPerMeter * 0.3: l - unitsPerMeter * 0.3,
                    Y2 = middle,
                    Stroke = Brushes.Green,
                    StrokeThickness = 10,
                    StrokeEndLineCap = PenLineCap.Triangle,
                };
                canvas.Children.Add(line);
            }

        }
    }
}
