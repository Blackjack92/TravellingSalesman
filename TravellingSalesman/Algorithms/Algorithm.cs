using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    public abstract class Algorithm : NotifyPropertyChangedBase
    {
        public string Name { get; protected set; }

        public double Runtime { get { return runtime; } set { runtime = value; OnPropertyChanged(); } }
        private double runtime;

        private readonly BackgroundWorker bw;
        private readonly Timer timer;

        public Algorithm()
        {
            timer = new Timer();
            timer.Interval = 100;
            timer.Enabled = true;
            timer.Elapsed += (sender, args) => Runtime += timer.Interval/1000.0;

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += (sender, args) => OnCalculationFinished();
            bw.DoWork += new DoWorkEventHandler(RunCalculation);
        }

        public void Run(IEnumerable<Point> points)
        {
            Runtime = 0;
            timer.Start();
            bw.RunWorkerAsync(points);
        }

        public void Stop()
        {
            bw.CancelAsync();
            timer.Stop();
        }

        protected abstract void RunCalculation(object sender, DoWorkEventArgs args);

        // Event will be raised, when a new better path is found
        public event EdgesCalculatedHandler EdgesCalculated;
        public delegate void EdgesCalculatedHandler(object sender, IEnumerable<Edge> edges);

        protected void OnCalculated(IEnumerable<Edge> edges)
        {
            EdgesCalculated?.Invoke(this, edges);
        }

        // Event will be raised, when the progress changes
        public event CalculationProgressChangedEventHandler ProgressChanged;
        public delegate void CalculationProgressChangedEventHandler(object sender, int progress);

        protected void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        // Event will be raised, when the calculation is finished
        public event EdgesCalculationFinishedHandler EdgesCalculationFinished;
        public delegate void EdgesCalculationFinishedHandler(object sender);

        protected void OnCalculationFinished()
        {
            EdgesCalculationFinished?.Invoke(this);
        }
    }
}
