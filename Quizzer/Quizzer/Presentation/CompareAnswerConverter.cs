using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace QuizApp.Presentation
{
    [ValueConversion(typeof(int[]), typeof(bool))]
    public class CompareAnswerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            values = values.ToArray();
            var correctAnswer = System.Convert.ToString(values[0]);
            var givenAnswer = System.Convert.ToString(values[1]);

            if (correctAnswer == givenAnswer)
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
