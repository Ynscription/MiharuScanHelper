
using System.Windows;
using System.Windows.Data;

namespace Miharu.FrontEnd.TextEntry
{
	public class TextEntryKanjiExpanderMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new Thickness(0, 0, 0,
								-(System.Convert.ToDouble(values[0]) - System.Convert.ToDouble(values[1])));
		}

		public object[] ConvertBack(object value, System.Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
