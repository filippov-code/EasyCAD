using EasyCAD.Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EasyCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Construction Construction { get; private set; } = new();
        private Solution? currentSolution = null;
        public SolutionDataForChart? SolutionDataForChart { get; private set; } = null;

       

        public MainWindow()
        {
            InitializeComponent();

            SolutionDataForChart = new(Construction);

            Construction.ConstructionChanged += RedrawConstruction;
            SizeChanged += (object sender, SizeChangedEventArgs e) => RedrawConstruction();

            DataContext = this;

        }

        private void AddRod(object sender, RoutedEventArgs e)
        {
            float L, A, E, o;

            if (!float.TryParse(newLTextBox.Text, out L) || L <= 0)
            {
                MessageBox.Show("Значение L для стержня некорректно");
                return;
            }
            if (!float.TryParse(newATextBox.Text, out A) || A <= 0)
            {
                MessageBox.Show("Значение A для стержня некорректно");
                return;
            }
            if (!float.TryParse(newETextBox.Text, out E) || E <= 0)
            {
                MessageBox.Show("Значение E для стержня некорректно");
                return;
            }
            if (!float.TryParse(newOTextBox.Text, out o) || o <= 0)
            {
                MessageBox.Show("Значение o для стержня некорректно");
                return;
            }

            Construction.AddRod( new Rod(L, A, E, o) );
        }

        private void RemoveRod(object sender, RoutedEventArgs e)
        {
            if (rodsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите стержень из списка для удаления");
                return;
            }
            Rod selectedRod = (Rod)rodsDataGrid.SelectedItem;
            Construction.RemoveRod(selectedRod);
        }

        private void AddDistributedForce(object sender, RoutedEventArgs e)
        {
            int number;
            if (!int.TryParse(newQxNumberTextBox.Text, out number) || number < 1)
            {
                MessageBox.Show("Значение номера стержня некорректно");
                return;
            }
            float qx;
            if (!float.TryParse(newQxTextBox.Text, out qx))
            {
                MessageBox.Show("Значение распределённой нагрузки некорректно");
                return;
            }
            Construction.SetDistributedStrain(number, qx);
        }

        private void RemoveDistributedForce(object sender, RoutedEventArgs e)
        {
            if (distStrainsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите нагрузку из списка для удаления");
                return;
            }
            DistributedStrain selectedConcentratedStrain = (DistributedStrain)distStrainsDataGrid.SelectedItem;
            Construction.RemoveDistributedStrain(selectedConcentratedStrain);
        }

        private void AddConcentratedStrain(object sender, RoutedEventArgs e)
        {
            int number;
            if (!int.TryParse(newFNumberTextBox.Text, out number) || number < 1 || number > Construction.NodesCount)
            {
                MessageBox.Show("Значение номера узла некорректно");
                return;
            }
            float F;
            if (!float.TryParse(newFTextBox.Text, out F))
            {
                MessageBox.Show("Значение сосредоточенной нагрузки некорректно");
                return;
            }
            Construction.SetConcentratedStrain(number, F);
        }

        private void RemoveConcentratedStrain(object sender, RoutedEventArgs e)
        {
            if (conStrainsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите нагрузку из списка для удаления");
                return;
            }
            ConcentratedStrain selectedConcentratedStrain = (ConcentratedStrain)conStrainsDataGrid.SelectedItem;
            Construction.RemoveConcentratedStrain(selectedConcentratedStrain);
        }

        private void Solve(object sender, RoutedEventArgs e)
        {
            if (Construction.Length == 0)
            {
                MessageBox.Show("Конструкция не имеет стержней");
                return;
            }
            currentSolution = new(Construction);
            currentSolution.Calculate();
            ShowSolution();
            //logTextBox.Text += solution.ToString();
            
        }

        private void ShowSolution()
        {
            if (currentSolution == null) return;

            logTextBox.Text += "======================================================" + Environment.NewLine;
            logTextBox.Text += "Решение" + Environment.NewLine;
            logTextBox.Text += "======================================================" + Environment.NewLine;
            logTextBox.Text += "---A---" + Environment.NewLine;
            logTextBox.Text += currentSolution.AMatrix.ToString();
            logTextBox.Text += "---B---" + Environment.NewLine;
            logTextBox.Text += currentSolution.BMatrix.ToString();
            logTextBox.Text += "---AB---" + Environment.NewLine;
            logTextBox.Text += currentSolution.ABMatrix.ToString();
            logTextBox.Text += "--delta--" + Environment.NewLine;
            logTextBox.Text += currentSolution.DeltaMatrix.ToString();
            logTextBox.Text += "---Nx---" + Environment.NewLine;
            logTextBox.Text += currentSolution.NxMatrix.ToString();
            logTextBox.Text += "---ox---" + Environment.NewLine;
            logTextBox.Text += currentSolution.OxMatrix.ToString();
            logTextBox.Text += "---Ux---" + Environment.NewLine;
            logTextBox.Text += currentSolution.UxMatrix.ToString();

            SolutionDataForChart.SetSolution(currentSolution);
        }

        private void SolveInPoint(object sender, RoutedEventArgs e)
        {
            if (currentSolution == null)
            {
                MessageBox.Show("Сначала нужно решить задачу для всей конструкции");
                return;
            }

            ShowSolveInPoint();
        }

        private void ShowSolveInPoint()
        {
            float L;
            if (!float.TryParse(LInPointTextBox.Text, out L) || L <= 0 || L > Construction.Length)
            {
                MessageBox.Show("Значение L для точки некорректно");
                return;
            }
            Rod rod = Construction.GetRodByLength(L);
            double lOnRod = L - Construction.GetLengthBeforeRod(L);
            double nxPointSolution = currentSolution.GetNxSolution(rod, lOnRod);
            double oxPointSolution = nxPointSolution / rod.A;
            double uxPointSolution = currentSolution.GetUxSolution(rod, lOnRod);


            logTextBox.Text += "======================================================" + Environment.NewLine;
            logTextBox.Text += $"Решение в точке {L}" + Environment.NewLine;
            logTextBox.Text += "======================================================" + Environment.NewLine;
            logTextBox.Text += $"N = {nxPointSolution}" + Environment.NewLine;
            logTextBox.Text += $"o = {oxPointSolution}" + Environment.NewLine;
            logTextBox.Text += $"U = {uxPointSolution}" + Environment.NewLine;
        }

        //Drawing
        private void RedrawConstruction()
        {
            //Рисование опор
            leftProp.Visibility = Construction.LeftProp ? Visibility.Visible : Visibility.Hidden;
            rightProp.Visibility = Construction.RightProp ? Visibility.Visible : Visibility.Hidden;

            canvas.Children.Clear();
            if (Construction.Rods.Count == 0) return;

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
                    X2 = (strain.Force > 0) ? l + unitsPerMeter * 0.15f : l - unitsPerMeter * 0.15f,
                    Y2 = middle,
                    Stroke = Brushes.Red,
                    StrokeThickness = 25,
                    StrokeEndLineCap = PenLineCap.Triangle,
                };
                canvas.Children.Add(line);
                //canvas.Children.Add(DrawLinkArrow(new(l, middle), new((strain.Force > 0) ? l + unitsPerMeter * 0.15f : l - unitsPerMeter * 0.15f, middle), Brushes.Red));
            }

            //Рисование распределенных нагрузок
            foreach (var strain in Construction.DistributedStrains)
            {
                l = Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter;
                double x1 = (strain.Qx > 0) ? Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter : Construction.GetLengthUpToNode(strain.SequenceNumber + 1) * unitsPerMeter;
                double y1 = middle;
                double x2 = (strain.Qx > 0) ? Construction.GetLengthUpToNode(strain.SequenceNumber + 1) * unitsPerMeter : Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter;
                double y2 = middle;
                canvas.Children.Add(GetLinkArrow(new(x1, y1), new(x2, y2), Brushes.Green));
                //Line line = new()
                //{
                //    X1 = (strain.Qx > 0)? Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter: Construction.GetLengthUpToNode(strain.SequenceNumber + 1) * unitsPerMeter,
                //    Y1 = middle,
                //    X2 = (strain.Qx > 0)? Construction.GetLengthUpToNode(strain.SequenceNumber + 1) * unitsPerMeter: Construction.GetLengthUpToNode(strain.SequenceNumber) * unitsPerMeter,
                //    Y2 = middle,
                //    Stroke = Brushes.Green,
                //    StrokeThickness = 10,
                //    StrokeEndLineCap = PenLineCap.Triangle,
                //};
                //canvas.Children.Add(line);
            }
        }
        private Shape GetLinkArrow(Point p1, Point p2, Brush brush)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1), p1.Y + ((p2.Y - p1.Y) / 1));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = lineGroup;
            path.StrokeThickness = 4;
            path.Stroke = path.Fill = brush;

            return path;
        }

        private void PropChecked(object sender, RoutedEventArgs e)
        {
            Construction.LeftProp = false;
            Construction.RightProp = false;
            if (leftPropRadio.IsChecked != null && leftPropRadio.IsChecked.Value)
            {
                Construction.LeftProp = true;
            }
            else if (rightPropRadio.IsChecked != null && rightPropRadio.IsChecked.Value)
            {
                Construction.RightProp = true;
            }
            else
            {
                Construction.LeftProp = true;
                Construction.RightProp = true;
            }
        }

        private void SaveProject(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveProjectDialog = new SaveFileDialog();
            saveProjectDialog.Filter = "EasyCAD Project (*.ecad)|*.ecad";
            if (Construction == null)
            {
                MessageBox.Show("Конструкция не задана");
            }
            else if (saveProjectDialog.ShowDialog() == true)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fs = new FileStream(saveProjectDialog.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, Construction);
                    }
                    MessageBox.Show("Успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openProjectDialog = new OpenFileDialog();
            openProjectDialog.Filter = "EasyCAD Project (*.ecad)|*.ecad";
            if (openProjectDialog.ShowDialog() == true)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(openProjectDialog.FileName, FileMode.OpenOrCreate))
                {
                    Construction loadedConstruction = (Construction)formatter.Deserialize(fs);

                    Construction.Rods.Clear();
                    Construction.DistributedStrains.Clear();
                    Construction.ConcentratedStrains.Clear();

                    loadedConstruction.Rods.ToList().ForEach(x => Construction.Rods.Add(x));
                    loadedConstruction.DistributedStrains.ToList().ForEach(x => Construction.DistributedStrains.Add(x));
                    loadedConstruction.ConcentratedStrains.ToList().ForEach(x => Construction.ConcentratedStrains.Add(x));
                    Construction.RightProp = loadedConstruction.RightProp;
                    Construction.LeftProp = loadedConstruction.LeftProp;
                    if (Construction.RightProp && Construction.LeftProp)
                    {
                        middlePropRadio.IsChecked = true;
                    }
                    else if (Construction.RightProp)
                    {
                        rightPropRadio.IsChecked = true;
                    }
                    else if (Construction.LeftProp)
                    {
                        leftPropRadio.IsChecked = true;
                    }
                    currentSolution = null;
                    SolutionDataForChart.Clear();

                    RedrawConstruction();
                }
            }
        }

        private void SaveResults(object sender, RoutedEventArgs e)
        {
            if (Construction == null)
            {
                MessageBox.Show("Конструкция не задана");
                return;
            }
            if (currentSolution == null)
            {
                MessageBox.Show("Сначала нужно рассчитать конструкцию");
                return;
            }
            try
            {
                double[,] values = currentSolution.GetValuesTable(2);

                var package = new ExcelPackage();

                var sheet = package.Workbook.Worksheets.Add("Результаты расчёта");

                sheet.Cells["A1"].Value = "L";
                sheet.Cells["B1"].Value = "Nx";
                sheet.Cells["C1"].Value = "Ox";
                sheet.Cells["D1"].Value = "O";
                sheet.Cells["E1"].Value = "Ux";
                sheet.Cells["A1:E1"].Style.Font.Bold = true;

                sheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                sheet.Cells["B1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSeaGreen);
                sheet.Cells["C1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                sheet.Cells["D1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                sheet.Cells["E1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);

                for (int i = 2; i <= values.GetLength(0) + 1; i++)
                {
                    sheet.Cells[i, 1].Value = values[i - 2, 0]; //L
                    sheet.Cells[i, 2].Value = values[i - 2, 1]; //nx
                    sheet.Cells[i, 3].Value = values[i - 2, 2]; //ox
                    sheet.Cells[i, 4].Value = values[i - 2, 3]; //o
                    sheet.Cells[i, 5].Value = values[i - 2, 4]; //ux

                    if (Math.Abs(values[i - 2, 2]) > values[i - 2, 3])
                        sheet.Cells[$"C{i}:D{i}"].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                }

                SaveFileDialog saveResultsDialog = new SaveFileDialog();
                saveResultsDialog.Filter = "Microsoft Excel (*.xlsx)|*.xlsx";
                if (saveResultsDialog.ShowDialog() == true)
                    File.WriteAllBytes(saveResultsDialog.FileName, package.GetAsByteArray());

            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ошибка: {exc.Message}");
            }
        }
    }
}
