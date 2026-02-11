using System;
using BHK.Retrieval.Attendance.Core.Enums;

namespace BHK.Retrieval.Attendance.Core.DTOs
{
    /// <summary>
    /// DTO để tạo mới hoạt động
    /// </summary>
    public class ActivityHistoryDto
    {
        public DateTime Timestamp { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
        public ActivityType ActivityType { get; set; }
        public string Action { get; set; } = string.Empty;
        public ActivityStatus Status { get; set; }
        public string? Details { get; set; }
        public int? UserCount { get; set; }
        public int? RecordCount { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Factory method để tạo activity thành công
        /// </summary>
        public static ActivityHistoryDto Success(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string? details = null,
            int? userCount = null,
            int? recordCount = null,
            TimeSpan? duration = null)
        {
            return new ActivityHistoryDto
            {
                Timestamp = DateTime.Now,
                DeviceIp = deviceIp,
                DeviceName = deviceName,
                ActivityType = activityType,
                Action = action,
                Status = ActivityStatus.Success,
                Details = details,
                UserCount = userCount,
                RecordCount = recordCount,
                Duration = duration
            };
        }

        /// <summary>
        /// Factory method để tạo activity thất bại
        /// </summary>
        public static ActivityHistoryDto Failed(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string errorMessage,
            string? details = null)
        {
            return new ActivityHistoryDto
            {
                Timestamp = DateTime.Now,
                DeviceIp = deviceIp,
                DeviceName = deviceName,
                ActivityType = activityType,
                Action = action,
                Status = ActivityStatus.Failed,
                ErrorMessage = errorMessage,
                Details = details
            };
        }

        /// <summary>
        /// Factory method để tạo activity có cảnh báo
        /// </summary>
        public static ActivityHistoryDto Warning(
            string deviceIp,
            string? deviceName,
            ActivityType activityType,
            string action,
            string warningMessage,
            string? details = null,
            int? userCount = null,
            int? recordCount = null)
        {
            return new ActivityHistoryDto
            {
                Timestamp = DateTime.Now,
                DeviceIp = deviceIp,
                DeviceName = deviceName,
                ActivityType = activityType,
                Action = action,
                Status = ActivityStatus.Warning,
                ErrorMessage = warningMessage,
                Details = details,
                UserCount = userCount,
                RecordCount = recordCount
            };
        }
    }
}
