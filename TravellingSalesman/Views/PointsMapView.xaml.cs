using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;
using TravellingSalesman.ViewModels;

namespace TravellingSalesman.Views
{
    /// <summary>
    /// Interaktionslogik für PointsMapView.xaml
    /// </summary>
    public partial class PointsMapView : UserControl
    {
        public PointsMapView()
        {
            InitializeComponent();

            DataContextChanged += UpdateMap;
        }

        private void UpdateMap(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainViewModel model = e.NewValue as MainViewModel;
            if (model == null) { return; }

            model.Algorithm.EdgesCalculated += Model_EdgesCalculated;
            model.Algorithm.ProgressChanged += Algorithm_ProgressChanged;

            int height = 10;
            int width = 10;

            foreach (var point in model.Points)
            {
                Ellipse myEllipse = new Ellipse();

                // Create a SolidColorBrush with a red color to fill the 
                // Ellipse with.
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();

                // Describes the brush's color using RGB values. 
                // Each value has a range of 0-255.
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Black;

                myEllipse.Height = height;
                myEllipse.Width = width;

                double left = point.X - (width / 2);
                double top = point.Y - (height / 2);
                myEllipse.Margin = new Thickness(left, top, 0, 0);
                pointCanvas.Children.Add(myEllipse);
            }
        }

        private void Algorithm_ProgressChanged(object sender, int progress)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    pbCalculation.Value = progress;
                }
                ));
        }

        private void Model_EdgesCalculated(object sender, IEnumerable<Point> points)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    // Remove all drawn edges
                    List<Line> drawnEdges = pointCanvas.Children.OfType<Line>().ToList();
                    foreach (var edge in drawnEdges)
                    {
                        pointCanvas.Children.Remove(edge);
                    }


                    Point p1;
                    Point p2;
                    // Add new edges
                    for (int i = 1; i < points.Count(); i++)
                    {
                        // Console.WriteLine("[" + edge.Start + "] -> [" + edge.End + "]");

                        Line line = new Line();
                        line.Stroke = Brushes.LightSteelBlue;

                        p1 = points.ElementAt(i - 1);
                        p2 = points.ElementAt(i);
                        line.X1 = p1.X;
                        line.X2 = p2.X;
                        line.Y1 = p1.Y;
                        line.Y2 = p2.Y;

                        line.StrokeThickness = 2;
                        pointCanvas.Children.Add(line);
                    }

                    Line l = new Line();
                    l.Stroke = Brushes.LightSteelBlue;

                    p1 = points.ElementAt(0);
                    p2 = points.Last();
                    l.X1 = p1.X;
                    l.X2 = p2.X;
                    l.Y1 = p1.Y;
                    l.Y2 = p2.Y;

                    l.StrokeThickness = 2;
                    pointCanvas.Children.Add(l);

                    lblDistance.Content = points.CalculateDistance();
                }));
        }
    }
}
