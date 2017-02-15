using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravellingSalesman.Models;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// Some extension methods for this project.
    /// </summary>
    public static class TSExtensions
    {
        /// <summary>
        /// Returns a random index from a given array.
        /// </summary>
        /// <typeparam name="T">Type of the array elements.</typeparam>
        /// <param name="array">Reference to the array.</param>
        /// <returns>A random index from the array. When it does not contain 
        /// any item the return value is 0.</returns>
        public static int RandomIndex<T>(this T[] array)
        {
            return new Random().Next(0, array.Count());
        }

        // Returns an enumeration of enumerators, one for each permutation
        // of the input.
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }

        // Enumerates over contents of both lists.
        // This method is a sub method form the Permute
        public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
        }

        /// <summary>
        /// Calculates the factorial from a given number i.
        /// </summary>
        /// <param name="i">The number from, which the factorial should be calculated.</param>
        /// <returns>The factorial.</returns>
        public static int Factorial(this int i)
        {
            int result = i < 0 ? -1 : i == 0 || i == 1 ? 1 : 1;
            if (i > 0)
                Enumerable.Range(1, i).ToList<int>().ForEach(element => result = result * element);
            return result;
        }

        // Enumerates over all items in the input, skipping over the item
        // with the specified offset.
        public static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
        {
            int index = 0;
            foreach (T item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }

        /// <summary>
        /// Returns a sub array from a given one.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="data">Reference to the array.</param>
        /// <param name="index">Start index from which the sub array should be taken.</param>
        /// <param name="length">Number of array elements, which should be added to the new 
        /// sub array (beginning by the start). </param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Creates Edges from a given BindablePoint IEnumerable. Important is, that the 
        /// order from the points is responsible for the edge creation. This means two 
        /// points, which are ordered in a row create an edge.
        /// </summary>
        /// <param name="points">List, wich contains all the points (cities).</param>
        /// <returns>The connected points (cities).</returns>
        public static IEnumerable<Edge> TransformToEdges(this IEnumerable<BindablePoint> points)
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 1; i < points.Count(); i++)
            {
                BindablePoint start = points.ElementAt(i - 1);
                BindablePoint end = points.ElementAt(i);
                edges.Add(new Edge(start, end));
            }

            // Create cycle
            if (points.Count() > 1)
            {
                edges.Add(new Edge(points.Last(), points.First()));
            }

            return edges;
        }

        /// <summary>
        /// Calculates the tour length. This is used for evaluation.
        /// </summary>
        /// <param name="generation">The tour, which should be evaluated.</param>
        /// <returns>The tour length.</returns>
        public static double CalculateDistance(this BindablePoint[] generation)
        {
            double sum = 0;
            for (int i = 1; i < generation.Count(); i++)
            {
                sum += generation[i - 1].GetLength(generation[i]);
            }

            sum += generation.Count() >= 2 ? generation[generation.Count() - 1].GetLength(generation[0]) : 0;

            return sum;
        }

        /// <summary>
        /// Calculates the tour length. This is used for evaluation.
        /// </summary>
        /// <param name="generation">The tour, which should be evaluated.</param>
        /// <returns>The tour length.</returns>
        public static double CalculateDistance(this IEnumerable<BindablePoint> generation)
        {
            return generation.ToArray().CalculateDistance();
        }

        /// <summary>
        /// Calculates the tour length. This is used for evaluation.
        /// </summary>
        /// <param name="edges">The tour, which should be evaluated.</param>
        /// <returns>The tour length.</returns>
        public static double CalculateDistance(this IEnumerable<Edge> edges)
        {
            return edges.Sum(e => (e.Start.GetLength(e.End)));
        }

        /// <summary>
        /// Creates a clone of a given list.
        /// </summary>
        /// <typeparam name="T">Type of the list.</typeparam>
        /// <param name="listToClone">Reference of the list, which should be cloned.</param>
        /// <returns>The cloned list.</returns>
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
