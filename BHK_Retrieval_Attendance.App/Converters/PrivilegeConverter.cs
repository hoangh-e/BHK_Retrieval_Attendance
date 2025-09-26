using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    public class PrivilegeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int privilege)
            {
                return privilege switch
                {
                    0 => "Người dùng thường",
                    1 => "Quản lý",
                    2 => "Quản trị viên",
                    3 => "Siêu quản trị",
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