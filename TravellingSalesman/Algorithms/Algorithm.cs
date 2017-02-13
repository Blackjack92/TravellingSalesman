using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using TravellingSalesman.Models;

namespace TravellingSalesman.Algorithms
{
    public abstract class Algorithm
    {
        public string Name { get; protected set; }

        public event EdgesCalculatedHandler EdgesCalculated;
        public delegate void EdgesCalculatedHandler(object sender, IEnumerable<Edge> edges);

        protected void OnCalculated(IEnumerable<Edge> edges)
        {
            EdgesCalculated?.Invoke(this, edges);
        }

        public event CalculationProgressChangedEventHandler ProgressChanged;
        public delegate void CalculationProgressChangedEventHandler(object sender, int progress);

        protected void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        public event EdgesCalculationFinishedHandler EdgesCalculationFinished;
        public delegate void EdgesCalculationFinishedHandler(object sender);

        protected void OnCalculationFinished()
        {
            EdgesCalculationFinished?.Invoke(this);
        }

        public abstract void Run(IEnumerable<Point> points);

        public abstract void Stop();
    }
}
