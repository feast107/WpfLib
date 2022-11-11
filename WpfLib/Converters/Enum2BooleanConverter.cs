using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters {
	public class Enum2BooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == parameter;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
