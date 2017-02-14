using System;
using System.Globalization;
using System.Windows.Data;

namespace TravellingSalesman.Utils
{
    public class CenteringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && parameter is string)
            {
                double offset;
                if (!double.TryParse((string)parameter, out offset))
                {
                    offset = 0;
                }

                return (double)value - (offset / 2.0);

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
