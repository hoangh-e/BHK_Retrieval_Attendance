using System;
using System.Collections.Generic;
using BHK.Retrieval.Attendance.Core.Enums;

namespace BHK.Retrieval.Attendance.Core.DTOs
{
    /// <summary>
    /// Bộ lọc cho truy vấn lịch sử hoạt động
    /// </summary>
    public class ActivityFilter
    {
        /// <summary>
        /// Lọc theo địa chỉ IP thiết bị
        /// </summary>
        public string? DeviceIp { get; set; }

        /// <summary>
        /// Lọc theo loại hoạt động
        /// </summary>
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Lọc theo trạng thái
        /// </summary>
        public List<ActivityStatus>? Statuses { get; set; }

        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Tìm kiếm trong Action và Details
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Số trang (0-based)
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// Số bản ghi trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Factory methods cho các quick filters phổ biến
        /// </summary>
        public static ActivityFilter Today()
        {
            var today = DateTime.Today;
            return new ActivityFilter
            {
                FromDate = today,
                ToDate = today.AddDays(1).AddTicks(-1)
            };
        }

        public static ActivityFilter Last7Days()
        {
            return new ActivityFilter
            {
                FromDate = DateTime.Today.AddDays(-7),
                ToDate = DateTime.Now
            };
        }

        public static ActivityFilter Last30Days()
        {
            return new ActivityFilter
            {
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Now
            };
        }

        public static ActivityFilter Last90Days()
        {
            return new ActivityFilter
            {
                FromDate = DateTime.Today.AddDays(-90),
                ToDate = DateTime.Now
            };
        }
    }
}
