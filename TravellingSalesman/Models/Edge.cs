using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TravellingSalesman.Models
{
    public class Edge
    {
        public Point Start { get; }
        public Point End { get; }

        public Edge(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}
