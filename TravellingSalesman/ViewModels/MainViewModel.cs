using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TravellingSalesman.Algorithms;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Point> Points { get; }
        public Algorithm Algorithm { get; set; }
        public DelegateCommand StartCommand { get; }
        public ICommand AddCommand { get; }
        public List<Algorithm> Algorithms { get; private set; }
        public string X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }
        public string Y { get { return y; } set { y = value; OnPropertyChanged(); } }


        private string x;
        private string y;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        // This could be replaced by using PostSharp
        protected void OnPropertyChanged([CallerMemberName]string name = "Default")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event EdgesCalculatedHandler EdgesCalculated;
        public delegate void EdgesCalculatedHandler(object sender, IEnumerable<Point> points);

        protected void OnCalculated(IEnumerable<Point> points)
        {
            EdgesCalculated?.Invoke(this, points);
        }

        public event CalculationProgressChangedEventHandler ProgressChanged;
        public delegate void CalculationProgressChangedEventHandler(object sender, int progress);

        protected void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        public bool IsAlgorithmRunning
        {
            get { return isAlgorithmRunning; }
            private set
            {
                isAlgorithmRunning = value;
                OnPropertyChanged();
            }
        }
        private bool isAlgorithmRunning;

        public MainViewModel()
        {
            Algorithms = new List<Algorithm>();
            Algorithms.Add(new SimulatedAnnealing());
            Algorithms.Add(new BruteForce());
            Algorithms.ForEach(a =>
            {
                a.EdgesCalculated += (sender, points) => OnCalculated(points);
                a.ProgressChanged += (sender, progress) => OnProgressChanged(progress);
                a.EdgesCalculationFinished += AlgorithmFinished;
            });

            Points = new ObservableCollection<Point>();
            Points.Add(new Point(0, 0));
            Points.Add(new Point(150, 150));
            Points.Add(new Point(250, 150));
            Points.Add(new Point(50, 250));
            Points.Add(new Point(300, 100));
            Points.Add(new Point(250, 400));
            Points.Add(new Point(150, 100));
            Points.Add(new Point(250, 100));
            Points.Add(new Point(400, 400));
            Points.Add(new Point(450, 380));
            Points.Add(new Point(0, 320));
            Algorithm = Algorithms[0];
            StartCommand = new DelegateCommand(Calculate);
            AddCommand = new DelegateCommand(Add);
        }

        private void Add(object obj)
        {
            int x;
            if (!int.TryParse(X, out x))
            {
                x = 0;
            }

            int y;
            if (!int.TryParse(Y, out y))
            {
                y = 0;
            }

            Point p = new Point(x, y);
            if (Points.Any(point => point.Equals(p)))
            {
                MessageBox.Show("Point was already added.");
            }
            else
            {
                Points.Add(p);
                X = "";
                Y = "";
            }

        }

        private void AlgorithmFinished(object sender)
        {
            IsAlgorithmRunning = false;
            StartCommand.RaiseCanExecuteChanged();
        }

        private void Calculate(object obj)
        {
            if (IsAlgorithmRunning)
            {
                Algorithm.Stop();
            }
            else
            {
                IsAlgorithmRunning = true;
                StartCommand.RaiseCanExecuteChanged();

                Algorithm.Run(Points);
            }
        }
    }
}
