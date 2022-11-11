using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters;

/// <summary>
/// 用于将ToggleButton的开关状态转为ToolTip，例如勾选时，显示 XXX：开
/// <para name="parameter">前缀，如XXX：</para>
/// </summary>
public class ToggleButtonToolTipConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        Debug.Assert(value != null, nameof(value) + " Cannot be null.");
        return parameter + ((bool)value ? "开" : "关");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
