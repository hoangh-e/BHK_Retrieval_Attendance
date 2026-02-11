namespace BHK.Retrieval.Attendance.Core.Enums
{
    /// <summary>
    /// Trạng thái thực hiện hoạt động
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 1,

        /// <summary>
        /// Thất bại
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Cảnh báo (có vấn đề nhưng vẫn hoàn thành)
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Đang thực hiện
        /// </summary>
        InProgress = 4,

        /// <summary>
        /// Đã hủy
        /// </summary>
        Cancelled = 5
    }
}
