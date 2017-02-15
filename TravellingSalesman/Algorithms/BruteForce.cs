using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    /// <summary>
    /// A brute force implementation for the TSP.
    /// </summary>
    public class BruteForce : Algorithm
    {
        #region ctor
        public BruteForce() : base()
        {
            Name = "Brute Force";
        }
        #endregion

        #region Methods
        /// <summary>
        /// Explicite implementation of the run method, which will be executed
        /// in its own BackgroundWorker.
        /// </summary>
        /// <param name="sender">Is the BackgroundWorker.</param>
        /// <param name="args">Arguments for the calculation. Normally they contain
        /// a list of all points (cities).</param>
        protected override void RunCalculation(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            IEnumerable<BindablePoint> points = args.Argument as IEnumerable<BindablePoint>;
            // Set the best permutation to the first solution
            IEnumerable<BindablePoint> bestPermutation = points;
            int count = 0;
            foreach (var permutation in points.Permute(points.Count()))
            {
                // This is used to cancel the worker
                if (worker.CancellationPending == true)
                {
                    args.Cancel = true;
                    break;
                }
                else
                {
                    // If a better permutation (solution) is found, it will replace the former
                    // best permutation
                    if (bestPermutation.CalculateDistance() > permutation.CalculateDistance())
                    {
                        bestPermutation = permutation;
                        // Each time a better solution was found the Calculated event is raised.
                        OnCalculated(bestPermutation.TransformToEdges());
                    }

                    // Calculate the progress
                    count++;
                    OnProgressChanged((int)((count * 100.0) / points.Count().Factorial()));
                }
            }
        }
        #endregion
    }
}
