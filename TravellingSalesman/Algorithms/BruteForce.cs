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
        private readonly BackgroundWorker bw;

        public BruteForce()
        {
            bw = new BackgroundWorker();
            bw.RunWorkerCompleted += WorkerCompleted;
            bw.DoWork += new DoWorkEventHandler(RunCalculation);
        }

        public override void Run(IEnumerable<Point> points)
        {
            bw.RunWorkerAsync(points);
        }

        private void RunCalculation(object o, DoWorkEventArgs args)
        {
            IEnumerable<Point> points = args.Argument as IEnumerable<Point>;
            IEnumerable<Point> bestPermutation = points;
            int count = 0;
            foreach (var permutation in points.Permute(points.Count()))
            {
                if (bestPermutation.CalculateDistance() > permutation.CalculateDistance())
                {
                    bestPermutation = permutation;
                    OnCalculated(bestPermutation);
                }

                count++;
                OnProgressChanged((int)((count * 100.0) / points.Count().Factorial()));
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCalculationFinished();
        }
    }
}
