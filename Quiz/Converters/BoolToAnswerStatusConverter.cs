using System;
using System.Globalization;
using System.Windows.Data;

namespace Quiz.Converters
{
    public class BoolToAnswerStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Sprawdź, czy mamy przekazany parametr z alternatywnymi tekstami
            if (parameter != null && parameter is string paramStr && paramStr.Contains(";"))
            {
                string[] parts = paramStr.Split(';');
                if (parts.Length == 2)
                {
                    bool isCorrect = (bool)value;
                    return isCorrect ? parts[1] : parts[0];
                }
            }

            // Domyślne wartości
            bool isCorrectValue = (bool)value;
            return isCorrectValue ? "Odpowiedź prawidłowa" : "Odpowiedź nieprawidłowa";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}