using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace WpfLib.Converters;

/// <summary>
/// 用于将完整文件路径转换成短文件名
/// </summary>
internal class FilePathConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return Path.GetFileNameWithoutExtension(value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
