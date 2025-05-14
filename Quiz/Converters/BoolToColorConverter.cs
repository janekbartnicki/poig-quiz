using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Quiz.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCorrect = (bool)value;
            bool forBorder = parameter != null && parameter.ToString().ToLower() == "true";

            if (isCorrect)
            {
                // Jasny zielony dla tła, ciemniejszy dla ramki
                return forBorder ? new SolidColorBrush(Colors.ForestGreen) : new SolidColorBrush(Color.FromRgb(220, 255, 220));
            }
            else
            {
                // Jasny czerwony dla tła, ciemniejszy dla ramki
                return forBorder ? new SolidColorBrush(Colors.Firebrick) : new SolidColorBrush(Color.FromRgb(255, 220, 220));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}