using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravellingSalesman.Models;

namespace TravellingSalesman.Utils
{
    public static class TSExtensions
    {
        public static T RandomElement<T>(this T[] array)
        {
            return array.RandomElementUsing(new Random());
        }

        public static T RandomElementUsing<T>(this T[] array, Random rand)
        {
            int index = rand.Next(0, array.Count());
            return array[index];
        }

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

        public static int Factorial(this int i)
        {
            int result = i < 0 ? -1 : i == 0 || i == 1 ? 1 : 1;
            if (i > 0)
                Enumerable.Range(1, i).ToList<int>().ForEach(element => result = result * element);
            return result;
        }

        // Enumerates over contents of both lists.
        public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
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

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

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

        public static double CalculateDistance(this BindablePoint[] generation)
        {
            double sum = 0;
            for (int i = 1; i < generation.Count(); i++)
            {
                sum += (generation[i - 1].ToPoint() - generation[i].ToPoint()).Length;
            }

            sum += generation.Count() >= 2 ? (generation[generation.Count() - 1].ToPoint() - generation[0].ToPoint()).Length : 0;

            return sum;
        }

        public static double CalculateDistance(this IEnumerable<BindablePoint> generation)
        {
            return generation.ToArray().CalculateDistance();
        }

        public static double CalculateDistance(this IEnumerable<Edge> edges)
        {
            return edges.Sum(e => (e.Start.ToPoint() - e.End.ToPoint()).Length);
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
