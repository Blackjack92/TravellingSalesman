using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    public class SimulatedAnnealing : Algorithm
    {
        public SimulatedAnnealing() : base()
        {
            Name = "Simulated Annealing";
        }

        private BindablePoint[] Lin2Opt(BindablePoint[] generation)
        {
            // Clone dictionary
            BindablePoint[] newGeneration = (BindablePoint[])generation.Clone();

            // Take two random cities
            int a = newGeneration.RandomIndex();
            int b = newGeneration.RandomIndex();
            while (a == b) { b = newGeneration.RandomIndex(); }

            // Cut connections and reconnect
            int start = Math.Min(a, b);
            int end = Math.Max(a, b);
            IEnumerable<BindablePoint> range = newGeneration.SubArray(start, end - start);
            range = range.Reverse();
            for (int i = 0; i < range.Count(); i++)
            {
                newGeneration[start + i] = range.ElementAt(i);
            }

            return newGeneration;
        }

        protected override void RunCalculation(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            IEnumerable<BindablePoint> points = args.Argument as IEnumerable<BindablePoint>;
            BindablePoint[] generation = points.ToArray();
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
                    BindablePoint[] newGeneration = Lin2Opt(generation);
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
    }
}
