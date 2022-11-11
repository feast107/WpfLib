using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters;

public class Boolean2StringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue) return "";
        if (parameter is not string str) return "";
        var arr = str.Split(';');
        if (arr.Length > 1)
        {
            return boolValue ? arr[1] : arr[0];
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
