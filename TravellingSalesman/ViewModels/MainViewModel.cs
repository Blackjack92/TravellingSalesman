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
        public Algorithm Algorithm { get; }
        public DelegateCommand StartCommand { get; }

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
            Algorithm = new SimulatedAnnealing();
            // Algorithm = new BruteForce();
            Algorithm.EdgesCalculationFinished += AlgorithmFinished;
            StartCommand = new DelegateCommand(Calculate);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        // Create the OnPropertyChanged method to raise the event
        // This could be replaced by using PostSharp
        protected void OnPropertyChanged([CallerMemberName]string name = "Default")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
