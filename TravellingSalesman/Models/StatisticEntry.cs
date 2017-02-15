using System.Collections.Generic;

namespace TravellingSalesman.Models
{
    /// <summary>
    /// For each algorithm execution a statistic entry is created.
    /// This entry represents the solution of the run, with the following
    /// information: name of the algorithm, distance (path length), runtime and connections 
    /// respectively a connection text (path).
    /// </summary>
    public class StatisticEntry
    {
        #region Properties
        public string AlgorithmName { get; }
        public double Distance { get; }
        public double Runtime { get; }

        // The path is the textual representation of the edges.
        public IEnumerable<Edge> Edges { get; }
        public string Path { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Only constructor for a StatisticEntry.
        /// </summary>
        /// <param name="algorithmName">Name of the algorithm e.g. Simulated Annealing.</param>
        /// <param name="distance">Tour (Path) length.</param>
        /// <param name="runtime">Runtime of the algorithm before it was stopped or it finished.</param>
        /// <param name="edges">Solution from the algorithm.</param>
        public StatisticEntry(string algorithmName, double distance, double runtime, IEnumerable<Edge> edges)
        {
            AlgorithmName = algorithmName;
            Distance = distance;
            Runtime = runtime;
            Edges = edges;

            // Create textual representation
            foreach (var edge in edges)
            {
                Path += edge + "->";
            }
            Path = Path.EndsWith("->") ? Path.Substring(0, Path.Length - 2) : Path;
        }
        #endregion
    }
}
