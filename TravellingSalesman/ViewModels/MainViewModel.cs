using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TravellingSalesman.Algorithms;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedBase
    {
        public ObservableCollection<Point> Points { get; }
        public ObservableCollection<Edge> Edges { get; }

        public ObservableCollection<StatisticEntry> Statistics { get; }

        public double Distance { get { return distance; } private set { distance = value; OnPropertyChanged(); } }
        private double distance;

        public int Progress { get { return progress; } private set { progress = value; OnPropertyChanged(); } }
        private int progress;

        public Algorithm Algorithm { get { return algorithm; } set { algorithm = value; OnPropertyChanged(); } }
        private Algorithm algorithm;

        public DelegateCommand StartCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand ExitCommand { get; }
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
                a.EdgesCalculated += (sender, edges) => {
                    App.Current.Dispatcher.Invoke(
                        () =>
                        {
                            Edges.Clear();
                            foreach (var edge in edges)
                            {
                                Edges.Add(edge);
                            }

                            Distance = Edges.CalculateDistance();
                        });
                };

                a.ProgressChanged += (sender, progress) => Progress = progress;
                a.EdgesCalculationFinished += AlgorithmFinished;
            });

            Statistics = new ObservableCollection<StatisticEntry>();
            Edges = new ObservableCollection<Edge>();
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
            ExitCommand = new DelegateCommand((obj) => Application.Current.Shutdown());
            OpenCommand = new DelegateCommand(Open);
            SaveCommand = new DelegateCommand(Save);
        }

        private void Save(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV File|*.csv";
            dialog.Title = "Save to file";
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                using (var writer = new StreamWriter(dialog.FileName))
                {
                    foreach (var point in Points)
                    {
                        var first = point.X;
                        var second = point.Y;
                        var line = string.Format("{0},{1}", first, second);
                        writer.WriteLine(line);
                        writer.Flush();
                    }
                }
            }
        }

        private void Open(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV File|*.csv";
            dialog.Title = "Open file";
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                Edges.Clear();
                Points.Clear();

                using(var reader = new StreamReader(File.OpenRead(dialog.FileName)))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] coordinates = line.Split(',');

                        if (coordinates.Length == 2)
                        {
                            int x;
                            int y;
                            if (int.TryParse(coordinates[0], out x) && int.TryParse(coordinates[1], out y))
                            {
                                Points.Add(new Point(x, y));
                            }
                        }
                    }
                }
            }
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

            Statistics.Add(new StatisticEntry(Algorithm.Name, Distance, Algorithm.Runtime));
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
