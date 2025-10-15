using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    /// <summary>
    /// Converter chuyển đổi string không rỗng sang Visibility
    /// </summary>
    public class StringNotEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
