using System;
using BHK.Retrieval.Attendance.Core.Enums;

namespace BHK.Retrieval.Attendance.Core.Models
{
    /// <summary>
    /// Entity model cho lịch sử hoạt động
    /// </summary>
    public class ActivityHistory
    {
        /// <summary>
        /// ID tự động tăng
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Thời điểm xảy ra hoạt động
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Địa chỉ IP của thiết bị
        /// </summary>
        public string DeviceIp { get; set; } = string.Empty;

        /// <summary>
        /// Tên thiết bị (nếu có)
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Loại hoạt động
        /// </summary>
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// Mô tả hành động cụ thể
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái thực hiện
        /// </summary>
        public ActivityStatus Status { get; set; }

        /// <summary>
        /// Chi tiết bổ sung (JSON format)
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Số lượng nhân viên (nếu áp dụng)
        /// </summary>
        public int? UserCount { get; set; }

        /// <summary>
        /// Số lượng bản ghi (nếu áp dụng)
        /// </summary>
        public int? RecordCount { get; set; }

        /// <summary>
        /// Thời gian thực hiện
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Thông báo lỗi (nếu có)
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
