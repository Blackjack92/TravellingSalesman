using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TravellingSalesman.Models;

namespace TravellingSalesman.Utils
{
    public class SelectionToColorConverter : IMultiValueConverter
    {
        private readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is BindablePoint) || !(values[1] is BindablePoint)) { return yellow; }

            BindablePoint selected = (BindablePoint)values[0];
            BindablePoint actual = (BindablePoint)values[1];

            return selected.Equals(actual) ? red : yellow;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
