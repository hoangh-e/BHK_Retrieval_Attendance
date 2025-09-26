using System;
using System.Globalization;
using System.Windows.Data;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    public class AccessControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAccessGranted)
            {
                return isAccessGranted ? "Được phép" : "Không được phép";
            }
            if (value is int accessLevel)
            {
                return accessLevel switch
                {
                    0 => "Không quyền",
                    1 => "Người dùng",
                    2 => "Quản lý",
                    3 => "Quản trị viên",
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