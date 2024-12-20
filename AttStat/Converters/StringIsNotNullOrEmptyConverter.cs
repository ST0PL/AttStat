﻿using System.Globalization;
using System.Windows.Data;

namespace AttStat.Converters
{
    class StringIsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !string.IsNullOrEmpty(value?.ToString());
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
