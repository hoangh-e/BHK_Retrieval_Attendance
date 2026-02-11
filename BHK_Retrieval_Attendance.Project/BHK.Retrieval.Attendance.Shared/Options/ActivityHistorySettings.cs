namespace BHK.Retrieval.Attendance.Shared.Options
{
    /// <summary>
    /// Cấu hình cho Activity History
    /// </summary>
    public class ActivityHistorySettings
    {
        /// <summary>
        /// Đường dẫn database SQLite. Nếu để trống sẽ dùng %LocalAppData%\BHK\Attendance\ActivityHistory.db
        /// </summary>
        public string DatabasePath { get; set; } = string.Empty;

        /// <summary>
        /// Số bản ghi tối đa giữ lại (auto cleanup khi vượt quá)
        /// </summary>
        public int MaxRecords { get; set; } = 10000;

        /// <summary>
        /// Tự động xóa bản ghi cũ hơn số ngày này
        /// </summary>
        public int AutoCleanupDays { get; set; } = 90;

        /// <summary>
        /// Số bản ghi mỗi trang khi phân trang
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Bật/tắt tính năng tự động cleanup khi app khởi động
        /// </summary>
        public bool EnableAutoCleanup { get; set; } = true;
    }
}
