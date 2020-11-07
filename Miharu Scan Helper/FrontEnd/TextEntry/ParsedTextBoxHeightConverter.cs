
using System.Windows.Data;

namespace Miharu.FrontEnd.TextEntry
{
	public class ParsedTextBoxHeightConverter : IValueConverter {

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
		
			return System.Convert.ToDouble(value) -6;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
