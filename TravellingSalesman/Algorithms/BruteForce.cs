using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    public class BruteForce : Algorithm
    {
        public BruteForce() : base()
        {
            Name = "Brute Force";
        }

        protected override void RunCalculation(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            IEnumerable<Point> points = args.Argument as IEnumerable<Point>;
            IEnumerable<Point> bestPermutation = points;
            int count = 0;
            foreach (var permutation in points.Permute(points.Count()))
            {
                if (worker.CancellationPending == true)
                {
                    args.Cancel = true;
                    break;
                }
                else
                {
                    if (bestPermutation.CalculateDistance() > permutation.CalculateDistance())
                    {
                        bestPermutation = permutation;
                        OnCalculated(bestPermutation.TransformToEdges());
                    }

                    count++;
                    OnProgressChanged((int)((count * 100.0) / points.Count().Factorial()));
                }
            }

            OnProgressChanged(100);
        }
    }
}
