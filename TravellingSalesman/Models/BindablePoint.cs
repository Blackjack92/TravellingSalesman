using System;
using System.Windows;
using TravellingSalesman.Utils;

namespace TravellingSalesman.Models
{
    /// <summary>
    /// This class is a kind of adapter for the Point struct, because the Point
    /// is a struct and so there binding problems.
    /// </summary>
    public class BindablePoint : NotifyPropertyChangedBase, ICloneable
    {
        #region Properties
        public double X { get { return instance.X; } set { instance.X = value; OnPropertyChanged(); } }
        public double Y { get { return instance.Y; } set { instance.Y = value; OnPropertyChanged(); } }
        #endregion

        #region Private Fields
        private Point instance;
        #endregion

        #region ctor
        public BindablePoint(double x, double y)
        {
            instance = new Point(x, y);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the length (distance) between two points. This is used for the
        /// solution evaluation.
        /// </summary>
        /// <param name="point">The other point.</param>
        /// <returns>Distance between the two points.</returns>
        public double GetLength(BindablePoint point)
        {
            return point == null ? 0 : (instance - point.instance).Length;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var point = obj as BindablePoint;
            return instance.Equals(point.instance);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return instance.GetHashCode();
        }

        public object Clone()
        {
            return new BindablePoint(X, Y);
        }

        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }
        #endregion
    }
}
