using System.Globalization;
using System.Windows.Data;

namespace AttStat.Converters
{
    class NotNullOrZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !((value is int && (int)value == 0) || value is null);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
