using System;
using System.Globalization;
using System.Windows.Data;

namespace TravellingSalesman.Utils
{
    /// <summary>
    /// This converter is responsible to center the drawn ellipses.
    /// Because normally they would be drawn on the top left corner.
    /// </summary>
    public class CenteringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && parameter is string)
            {
                int offset;
                if (!int.TryParse((string)parameter, out offset))
                {
                    offset = 0;
                }

                // The offset is the width/height of the ellipse. 
                // Because height and width are the same for this ellipse
                // only one offset is needed.
                return (int)((double)value - (offset / 2.0));

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
