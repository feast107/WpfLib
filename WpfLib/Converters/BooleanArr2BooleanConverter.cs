using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfLib.Converters;

public class BooleanArr2BooleanConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null)
        {
            return false;
        }

        var arr = new List<bool>();
        foreach (var item in values)
        {
            if (item is bool boolValue)
            {
                arr.Add(boolValue);
            }
            else
            {
                return false;
            }
        }

        return arr.All(item => item);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
