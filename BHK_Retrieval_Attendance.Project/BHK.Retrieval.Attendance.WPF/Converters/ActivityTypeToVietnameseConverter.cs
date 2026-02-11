using System;
using System.Globalization;
using System.Windows.Data;
using BHK.Retrieval.Attendance.Core.Enums;

namespace BHK.Retrieval.Attendance.WPF.Converters
{
    /// <summary>
    /// Converter để chuyển ActivityType enum sang tiếng Việt
    /// </summary>
    public class ActivityTypeToVietnameseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ActivityType activityType)
            {
                return value?.ToString() ?? string.Empty;
            }

            return activityType switch
            {
                ActivityType.Connection => "Kết nối thiết bị",
                ActivityType.EmployeeSync => "Đồng bộ nhân viên",
                ActivityType.AttendanceSync => "Đồng bộ chấm công",
                ActivityType.Export => "Xuất báo cáo",
                ActivityType.Import => "Nhập dữ liệu",
                ActivityType.Configuration => "Cấu hình hệ thống",
                ActivityType.Maintenance => "Bảo trì hệ thống",
                ActivityType.SystemError => "Lỗi hệ thống",
                ActivityType.Other => "Khác",
                _ => activityType.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
