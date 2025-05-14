using System;
using System.Globalization;
using System.Windows.Data;

namespace Quiz.Converters
{
    public class BoolToAnswerStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCorrect = (bool)value;
            return isCorrect ? "Poprawna odpowiedź" : "Niepoprawna odpowiedź";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}