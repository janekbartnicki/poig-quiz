using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Quiz.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Jeśli wartość to true, zwróć Visible, w przeciwnym razie Collapsed
            return value is bool && (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Zwróć true, jeśli wartość to Visibility.Visible, w przeciwnym razie false
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
