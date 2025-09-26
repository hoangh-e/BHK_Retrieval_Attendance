using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int gender)
            {
                return gender switch
                {
                    0 => "Nữ",
                    1 => "Nam",
                    2 => "Khác",
                    _ => "Không xác định"
                };
            }
            if (value is string genderStr)
            {
                return genderStr.ToLower() switch
                {
                    "male" or "nam" or "m" => "Nam",
                    "female" or "nữ" or "f" => "Nữ",
                    "other" or "khác" => "Khác",
                    _ => "Không xác định"
                };
            }
            return "Không xác định";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}