using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    /// <summary>
    /// Converter để đảo ngược giá trị Boolean
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }
}
