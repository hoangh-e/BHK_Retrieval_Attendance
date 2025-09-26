using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    public class BooleanToStatusConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Hoạt động";
        public string FalseText { get; set; } = "Không hoạt động";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueText : FalseText;
            }
            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}