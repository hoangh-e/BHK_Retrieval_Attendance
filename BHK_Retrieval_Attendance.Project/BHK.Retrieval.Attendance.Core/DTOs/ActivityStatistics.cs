using System.Collections.Generic;

namespace BHK.Retrieval.Attendance.Core.DTOs
{
    /// <summary>
    /// Thống kê lịch sử hoạt động
    /// </summary>
    public class ActivityStatistics
    {
        /// <summary>
        /// Tổng số hoạt động
        /// </summary>
        public int TotalActivities { get; set; }

        /// <summary>
        /// Số hoạt động thành công
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Số hoạt động thất bại
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// Số hoạt động có cảnh báo
        /// </summary>
        public int WarningCount { get; set; }

        /// <summary>
        /// Tỷ lệ thành công (%)
        /// </summary>
        public double SuccessRate => TotalActivities > 0 
            ? (double)SuccessCount / TotalActivities * 100 
            : 0;

        /// <summary>
        /// Số thiết bị đã kết nối
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        /// Danh sách thiết bị và số lần hoạt động
        /// </summary>
        public Dictionary<string, int> DeviceActivityCounts { get; set; } = new();

        /// <summary>
        /// Loại hoạt động phổ biến nhất
        /// </summary>
        public Dictionary<string, int> TopActivityTypes { get; set; } = new();
    }
}
