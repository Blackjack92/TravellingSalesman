using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TravellingSalesman.Algorithms;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;
using System;

namespace TravellingSalesman.ViewModels
{
    /// <summary>
    /// This class represents the ViewModel for all four views. 
    /// This could be improved but for the actual size of the project 
    /// it is sufficient.
    /// </summary>
    public class MainViewModel : NotifyPropertyChangedBase
    {
        #region Properties
        public List<Algorithm> Algorithms { get; private set; }

        public ObservableCollection<BindablePoint> Points { get; private set; }
        public ObservableCollection<Edge> Edges { get; private set; }
        public ObservableCollection<StatisticEntry> Statistics { get; private set;  }

        public BindablePoint SelectedPoint { get { return selectedPoint; } set { selectedPoint = value; OnPropertyChanged(); } }
        public Algorithm Algorithm { get { return algorithm; } set { algorithm = value; OnPropertyChanged(); } }
        public bool IsAlgorithmRunning { get { return isAlgorithmRunning; } private set { isAlgorithmRunning = value; OnPropertyChanged(); } }
        public int Progress { get { return progress; } private set { progress = value; OnPropertyChanged(); } }
        public double Distance { get { return distance; } private set { distance = value; OnPropertyChanged(); } }

        // This is used for adding a new point (city)
        public int X { get { return x; } set { x = value; OnPropertyChanged(); } }
        public int Y { get { return y; } set { y = value; OnPropertyChanged(); } }
        #endregion

        #region Backing Fields
        private BindablePoint selectedPoint;

        private int x;
        private int y;

        private Algorithm algorithm;
        private bool isAlgorithmRunning;
        private int progress;
        private double distance;
        #endregion

        #region Commands
        // This needs to be a delegate command, so that it is possible to change 
        // the button from running to stopped, when the algorithm is finished
        public DelegateCommand StartCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand DeleteCommand { get; }
        #endregion

        #region ctor
        public MainViewModel()
        {
            Algorithms = new List<Algorithm>();
            Algorithms.Add(new SimulatedAnnealing());
            Algorithms.Add(new BruteForce());
            Algorithms.ForEach(a =>
            {
                a.EdgesCalculated += UpdateEdges;
                a.ProgressChanged += (sender, progress) => Progress = progress;
                a.EdgesCalculationFinished += AlgorithmFinished;
            });
            // Select the first algorithm in the list
            Algorithm = Algorithms[0];

            Statistics = new ObservableCollection<StatisticEntry>();
            Edges = new ObservableCollection<Edge>();
            Points = new ObservableCollection<BindablePoint>();

            // Set commands
            StartCommand = new DelegateCommand(Calculate);
            AddCommand = new DelegateCommand(Add);
            ExitCommand = new DelegateCommand((obj) => Application.Current.Shutdown());
            OpenCommand = new DelegateCommand(Open);
            SaveCommand = new DelegateCommand((obj) => CSVHelper.Open(Points));
            DeleteCommand = new DelegateCommand(DeletePoint);

            // Register to points collection changes, this is necessary to 
            // update the view, when a point was deleted
            Points.CollectionChanged += PointsCollectionChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method loads points from a given file.
        /// </summary>
        /// <param name="obj">Is the CommandParameter, which is unused.</param>
        private void Open(object obj)
        {
            // This clears the edges and points before new data gets loaded
            // This could be added to a callback method, so that it is only
            // called, when the load was successfull
            Edges.Clear();
            Points.Clear();

            CSVHelper.Open(Points);
        }

        /// <summary>
        /// Delets a point from the list.
        /// </summary>
        /// <param name="obj"></param>
        private void DeletePoint(object obj)
        {
            if (SelectedPoint != null)
            {
                Points.Remove(SelectedPoint);
            }
        }

        /// <summary>
        /// Updates the edges. This means the old edges are removed and the 
        /// new one are added into the list.
        /// </summary>
        /// <param name="sender">Algorithm, which calculated the edges.</param>
        /// <param name="edges">Connections between the cities.</param>
        private void UpdateEdges(object sender, IEnumerable<Edge> edges)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    // Set the new solution
                    Edges.Clear();
                    foreach (var edge in edges)
                    {
                        Edges.Add(edge);
                    }

                    // Update the tour distance
                    Distance = Edges.CalculateDistance();
                });
        }

        /// <summary>
        /// This is used for updating the edges, when a point was removed.
        /// Otherwise there would be edges, without legal points.
        /// </summary>
        /// <param name="sender">The edge collection.</param>
        /// <param name="e">The args, which contains the changed items.</param>
        private void PointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                BindablePoint point = e.OldItems.OfType<BindablePoint>().First();
                if (point != null)
                {
                    // Get the edges, which referenced the deleted point
                    Edge to = Edges.First(edge => edge.End.Equals(point));
                    Edge from = Edges.First(edge => edge.Start.Equals(point));

                    // Create a new edge, which combines the old points
                    // E.g. A-->B-->C (B removed) A-->C
                    Edge combine = new Edge(to.Start, from.End);

                    // Remove the old edges add the new combined edge
                    Edges.Remove(to);
                    Edges.Remove(from);
                    Edges.Add(combine);
                }
            }

            // Calculate the new distance after the edges where changed.
            Distance = Edges.CalculateDistance();
        }
        
        /// <summary>
        /// Adds a point to the point list. If the point already exists, a MessageBox appears.
        /// </summary>
        /// <param name="obj">The CommandParameter, which is unused.</param>
        private void Add(object obj)
        {
            BindablePoint p = new BindablePoint(X, Y);
            if (Points.Any(point => point.Equals(p)))
            {
                MessageBox.Show("Point was already added.");
            }
            else
            {
                Points.Add(p);
                X = 0;
                Y = 0;
            }

        }

        /// <summary>
        /// This method updates the IsAlgorithmRunning and adds a algorithm statistic.
        /// </summary>
        /// <param name="sender">The algorithm, which was finished.</param>
        private void AlgorithmFinished(object sender)
        {
            IsAlgorithmRunning = false;
            StartCommand.RaiseCanExecuteChanged();

            Statistics.Add(new StatisticEntry(Algorithm.Name, Distance, Algorithm.Runtime, Edges.Clone()));
        }

        /// <summary>
        /// Starts the execution of the selected algorithm. When the algorithm is running
        /// it gets stopped. This is a kind of toggle behavior.
        /// </summary>
        /// <param name="obj"></param>
        private void Calculate(object obj)
        {
            if (IsAlgorithmRunning)
            {
                Algorithm.Stop();
            }
            // Start calculation only when >= 2 points exist, otherwise it has no sense
            else if (Points.Count >= 2)
            {
                IsAlgorithmRunning = true;
                StartCommand.RaiseCanExecuteChanged();

                Algorithm.Run(Points);
            }
        }
        #endregion
    }
}
