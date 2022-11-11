using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfLib.Converters {
	internal class EprTick2TimeSpanConverter : IValueConverter {
		private readonly int fps;

		public EprTick2TimeSpanConverter(int fps) {
			this.fps = fps;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int tick) {
				return TimeSpan.FromSeconds((double)tick / fps);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
	}
}
