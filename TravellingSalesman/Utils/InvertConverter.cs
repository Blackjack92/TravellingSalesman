using System;
using System.Globalization;
using System.Windows.Data;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// This converter inverts boolean logic. true -> false, false -> true
    /// </summary>
    public class InvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? !(bool)value : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
