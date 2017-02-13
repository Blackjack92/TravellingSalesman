using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private MainViewModel model;

        public PointsMapView()
        {
            InitializeComponent();

            DataContextChanged += UpdateMap;
        }

        private void UpdateMap(object sender, DependencyPropertyChangedEventArgs e)
        {
            model = e.NewValue as MainViewModel;
            if (model == null) { return; }

            model.Points.CollectionChanged += Points_CollectionChanged;
            model.Edges.CollectionChanged += Model_EdgesCalculated;
            drawPoints();
        }

        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            pointCanvas.Children.Clear();
            drawPoints();
        }

        private void drawPoints()
        {
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

        private void Model_EdgesCalculated(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    List<Edge> edges = model.Edges.ToList();

                    // Remove all drawn edges
                    List<Line> drawnEdges = pointCanvas.Children.OfType<Line>().ToList();
                    foreach (var edge in drawnEdges)
                    {
                        pointCanvas.Children.Remove(edge);
                    }

                    foreach (var edge in edges)
                    {
                        Line line = new Line();
                        line.Stroke = Brushes.LightSteelBlue;

                        Point p1 = edge.Start;
                        Point p2 = edge.End;
                        line.X1 = p1.X;
                        line.X2 = p2.X;
                        line.Y1 = p1.Y;
                        line.Y2 = p2.Y;

                        line.StrokeThickness = 2;
                        pointCanvas.Children.Add(line);
                    }

                    lblDistance.Content = edges.CalculateDistance();
                }));
        }
    }
}
