using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using TravellingSalesman.Models;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// This is a helper class, which handels interactions with a csv.
    /// </summary>
    public static class CSVHelper
    {
        /// <summary>
        /// Saves the given points into a file.
        /// </summary>
        /// <param name="points">They will be stored.</param>
        public static void Save(IEnumerable<BindablePoint> points)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV File|*.csv";
            dialog.Title = "Save to file";
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                using (var writer = new StreamWriter(dialog.FileName))
                {
                    foreach (var point in points)
                    {
                        var first = point.X;
                        var second = point.Y;
                        var line = string.Format("{0},{1}", first, second);
                        writer.WriteLine(line);
                        writer.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Loads points from a existing file into a given list.
        /// </summary>
        /// <param name="points">They will be updated.</param>
        public static void Open(ObservableCollection<BindablePoint> points)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV File|*.csv";
            dialog.Title = "Open file";
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                using (var reader = new StreamReader(File.OpenRead(dialog.FileName)))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] coordinates = line.Split(',');

                        if (coordinates.Length == 2)
                        {
                            int x;
                            int y;
                            if (int.TryParse(coordinates[0], out x) && int.TryParse(coordinates[1], out y))
                            {
                                points.Add(new BindablePoint(x, y));
                            }
                        }
                    }
                }
            }
        }
    }
}
