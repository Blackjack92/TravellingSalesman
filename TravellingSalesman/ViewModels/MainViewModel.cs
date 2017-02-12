using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public List<Algorithm> Algorithms { get; private set; }

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
