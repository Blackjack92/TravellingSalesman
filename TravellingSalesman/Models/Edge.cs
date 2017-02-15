using System;

namespace TravellingSalesman.Models
{
    /// <summary>
    /// This class represents a connection between two points (cities).
    /// </summary>
    public class Edge : ICloneable
    {
        #region Properties
        public BindablePoint Start { get; }
        public BindablePoint End { get; }
        #endregion

        #region ctor
        public Edge(BindablePoint start, BindablePoint end)
        {
            if (start == null || end == null)
            {
                throw new ArgumentException("Start and end point of an edge cannot be null.");
            }

            Start = start;
            End = end;
        }
        #endregion

        #region Methods
        public object Clone()
        {
            BindablePoint start = Start.Clone() as BindablePoint;
            BindablePoint end = End.Clone() as BindablePoint;
            return new Edge(start, end);
        }

        public override string ToString()
        {
            return Start + "->" + End;
        }
        #endregion
    }
}
