using System.Windows;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Models
{
    public class BindablePoint : NotifyPropertyChangedBase
    {
        public int X { get { return x; } set { x = value; OnPropertyChanged(); } }
        private int x;

        public int Y { get { return y; } set { y = value; OnPropertyChanged(); } }
        private int y;

        public BindablePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point ToPoint()
        {
            return new Point(x, y);
        }

        public double GetLength(BindablePoint point)
        {
            return (ToPoint() - point.ToPoint()).Length;
        }
    }
}
