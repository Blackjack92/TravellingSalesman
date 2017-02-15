using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    /// <summary>
    /// Base class for each algorithm. A explicite algorithm like brute force
    /// has to inherit from this class. 
    /// </summary>
    public abstract class Algorithm : NotifyPropertyChangedBase
    {
        #region Properties
        public string Name { get; protected set; }
        public double Runtime { get { return runtime; } set { runtime = value; OnPropertyChanged(); } }
        #endregion

        #region Backing Fields
        private double runtime;
        #endregion

        #region Readonly Fields
        // BackgroundWorker in which the algorithm runs
        private readonly BackgroundWorker bw;
        // Timer for the runtime of the algorithm
        private readonly Timer timer;
        #endregion

        #region ctor
        public Algorithm()
        {
            // Is the timer to measure the runtime of an algorithm
            timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += (sender, args) => Runtime += timer.Interval/1000.0;

            // Each algorithm runs in its own BackgroundWorker
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += (sender, args) => OnCalculationFinished();
            bw.DoWork += new DoWorkEventHandler(RunCalculation);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the calculation of the algorithm. Is also responsible for 
        /// starting the runtime measurement. 
        /// </summary>
        /// <param name="points">Collections of all points (cities).</param>
        public void Run(IEnumerable<BindablePoint> points)
        {
            // Reset the runtime before each run, this is necessary, when 
            // a algorithm is called more than once
            Runtime = 0;
            timer.Start();
            bw.RunWorkerAsync(points);
        }

        /// <summary>
        /// Stops the execution of the algorithm.
        /// </summary>
        public void Stop()
        {
            bw.CancelAsync();
            timer.Stop();
        }

        /// <summary>
        /// Needs to be implemented from each algorithm explicite. It is the code, 
        /// which is executed in the BackgroundWorker.
        /// </summary>
        /// <param name="sender">Is the BackgroundWorker.</param>
        /// <param name="args">Arguments for the calculation.</param>
        protected abstract void RunCalculation(object sender, DoWorkEventArgs args);
        #endregion

        #region Events
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
            // Stops the runtime timer
            timer.Stop();
            EdgesCalculationFinished?.Invoke(this);
            // When the calculation is finished the progress should also be set to 100.
            // This is necessary, when Stop() is invoked.
            OnProgressChanged(100);
        }
        #endregion
    }
}
