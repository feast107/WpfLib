using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters;

public class DoubleMinConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            if (parameter is string str && double.TryParse(str, out var minValue))
            {
                return doubleValue < minValue ? minValue : doubleValue;
            }

            return doubleValue < .0 ? .0 : doubleValue;
        }

        return .0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
