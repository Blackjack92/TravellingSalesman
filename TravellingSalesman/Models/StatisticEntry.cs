namespace TravellingSalesman.Models
{
    public class StatisticEntry
    {
        public string AlgorithmName { get; }
        public double Distance { get; }
        public double Runtime { get; }

        public StatisticEntry(string algorithmName, double distance, double runtime)
        {
            AlgorithmName = algorithmName;
            Distance = distance;
            Runtime = runtime;
        }

    }
}
