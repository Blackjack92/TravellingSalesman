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
            Name = "Brute Force";

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += WorkerCompleted;
            bw.DoWork += new DoWorkEventHandler(RunCalculation);
        }

        public override void Run(IEnumerable<Point> points)
        {
            bw.RunWorkerAsync(points);
        }

        public override void Stop()
        {
            bw.CancelAsync();
        }

        private void RunCalculation(object sender, DoWorkEventArgs args)
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
                        OnCalculated(bestPermutation);
                    }

                    count++;
                    OnProgressChanged((int)((count * 100.0) / points.Count().Factorial()));
                }
            }

            OnProgressChanged(100);
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCalculationFinished();
        }
    }
}
