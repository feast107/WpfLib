using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Brush = System.Windows.Media.Brush;

namespace WpfLib.Converters;

/// <summary>
/// 文件夹是否存在转换成对应的颜色
/// </summary>
internal class FolderExists2ColorConverter : IValueConverter {
    public Brush SuccessBrush { get; set; }
    public Brush FailedBrush { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string path && Directory.Exists(path)) {
            return SuccessBrush;
        }
        return FailedBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
