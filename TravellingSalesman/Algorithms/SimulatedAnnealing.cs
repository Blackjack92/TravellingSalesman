using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TravellingSalesman.Models;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Algorithms
{
    /// <summary>
    /// A simulated annealing implementation for the TSP.
    /// </summary>
    public class SimulatedAnnealing : Algorithm
    {
        #region Algorithm-Settings
        // Maximal number of generations, which will be tested
        private readonly int maxNumberOfGenerations = 10000;
        // Temperature decreasing factor
        private readonly double alpha = 0.9;
        // Start temperature
        double temperature = 50;
        // Minimal temperature
        double temperatureStop = 1e-8;
        #endregion

        #region ctor
        public SimulatedAnnealing() : base()
        {
            Name = "Simulated Annealing";
        }
        #endregion

        #region Methods
        /// <summary>
        /// This is a implementation of the k-opt heuristic, with k = 2.
        /// It cuts the connection of two points and reconnects the points so, 
        /// that a tour is still possible and the amount tour crosses is minimized.
        /// No optimal solution contains a cross of two connections!
        /// </summary>
        /// <param name="generation">Is a already found tour.</param>
        /// <returns>A new tour</returns>
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
            BindablePoint[] generation = points.ToArray();
            OnCalculated(generation.TransformToEdges());

            // Calculate first fitness (TSP evaluation value)
            double fitness = generation.CalculateDistance();
            double numberOfSameTemperatureIterations = points.Count();
            int numberOfGenerations = 0;

            // As long as the minimal temperature is not reached and the maximal 
            // number of iterations is not reached, go ahead with calculation.
            // The algorithm does not know if a best solution was found! - So different
            // boundaries are needed.
            while (temperature > temperatureStop && numberOfGenerations < maxNumberOfGenerations)
            {
                // This is used to cancel the worker
                if (worker.CancellationPending == true)
                {
                    args.Cancel = true;
                    break;
                }
                else
                {
                    // Create a new generation and evaluate it
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
                    // Calculate new progress and return some information to the user
                    int progress = (int)(numberOfGenerations * 100.0 / maxNumberOfGenerations);
                    OnProgressChanged(progress);
                }
            }
        }
        #endregion
    }
}
