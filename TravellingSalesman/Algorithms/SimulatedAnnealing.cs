using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    public class SimulatedAnnealing : Algorithm
    {
        private readonly BackgroundWorker bw;

        public SimulatedAnnealing()
        {
            Name = "Simulated Annealing";

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += WorkerCompleted;
            bw.DoWork += new DoWorkEventHandler(RunCalculation);
        }

        private Point[] Lin2Opt(Point[] generation)
        {
            // Clone dictionary
            Point[] newGeneration = (Point[])generation.Clone();

            // Take two random cities
            int a = newGeneration.RandomIndex();
            int b = newGeneration.RandomIndex();
            while (a == b) { b = newGeneration.RandomIndex(); }

            // Cut connections and reconnect
            int start = Math.Min(a, b);
            int end = Math.Max(a, b);
            IEnumerable<Point> range = newGeneration.SubArray(start, end - start);
            range = range.Reverse();
            for (int i = 0; i < range.Count(); i++)
            {
                newGeneration[start + i] = range.ElementAt(i);
            }

            return newGeneration;
        }

        private void RunCalculation(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            IEnumerable<Point> points = args.Argument as IEnumerable<Point>;
            Point[] generation = points.ToArray();
            OnCalculated(generation.TransformToEdges());

            double fitness = generation.CalculateDistance();
            double temperature = 50;
            double temperatureStop = 1e-8;
            double numberOfSameTemperatureIterations = 10;
            double alpha = 0.9;

            int maxNumberOfGenerations = 1000;
            int numberOfGenerations = 0;
            //int maxNumberOfGenerations = 10000;
            List<int> fitnessValues = new List<int>();

            while (temperature > temperatureStop && numberOfGenerations < maxNumberOfGenerations)
            {
                if (worker.CancellationPending == true)
                {
                    args.Cancel = true;
                    break;
                }
                else
                {
                    Point[] newGeneration = Lin2Opt(generation);
                    double newFitness = newGeneration.CalculateDistance();

                    // Decide if the new generation should be taken
                    if (newFitness <= fitness || (Math.Exp((fitness - newFitness) / temperature) > new Random().NextDouble()))
                    {
                        generation = newGeneration;
                        fitness = newFitness;
                        // Return always the better generation
                        OnCalculated(generation.TransformToEdges());
                    }

                    // Recalculate the temperature
                    if (numberOfGenerations % numberOfSameTemperatureIterations == 0)
                    {
                        temperature *= alpha;
                    }

                    numberOfGenerations++;
                    int progress = (int)(numberOfGenerations * 100.0 / maxNumberOfGenerations);
                    OnProgressChanged(progress);
                }
            }

            OnProgressChanged(100);
        }

        public override void Run(IEnumerable<Point> points)
        {
            bw.RunWorkerAsync(points);
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCalculationFinished();
        }

        public override void Stop()
        {
            bw.CancelAsync();
        }
    }
}
