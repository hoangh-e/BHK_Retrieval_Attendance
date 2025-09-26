using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    public class AttendanceActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int actionType)
            {
                return actionType switch
                {
                    0 => "Vào",
                    1 => "Ra",
                    2 => "Nghỉ trưa",
                    3 => "Kết thúc nghỉ trưa",
                    4 => "Tăng ca vào",
                    5 => "Tăng ca ra",
                    _ => "Không xác định"
                };
            }
            return value?.ToString() ?? "Không xác định";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}