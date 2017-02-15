namespace TravellingSalesman.Models
{
    public class Edge
    {
        public BindablePoint Start { get; }
        public BindablePoint End { get; }

        public Edge(BindablePoint start, BindablePoint end)
        {
            Start = start;
            End = end;
        }
    }
}
