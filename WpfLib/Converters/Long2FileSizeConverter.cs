﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters;

public class Long2FileSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return "0 B";
        if (value is long intValue)
        {
            if (intValue < 0)
            {
                return "0 B";
            }
            if (intValue < 1024)
            {
                return $"{intValue} B";
            }
            if (intValue < 1048576)
            {
                return $"{intValue / 1024.0:0.00} KB";
            }
            if (intValue < 1073741824)
            {
                return $"{intValue / 1048576.0:0.00} MB";
            }
            if (intValue < 1099511627776)
            {
                return $"{intValue / 1073741824.0:0.00} GB";
            }
            if (intValue < 1125899906842624)
            {
                return $"{intValue / 1099511627776.0:0.00} TB";
            }
            if (intValue < 1152921504606847000)
            {
                return $"{intValue / 1125899906842624.0:0.00} PB";
            }
        }
        throw new ArgumentOutOfRangeException(nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
