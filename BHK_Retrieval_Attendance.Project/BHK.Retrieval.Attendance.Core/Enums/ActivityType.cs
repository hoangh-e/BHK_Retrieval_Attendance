namespace BHK.Retrieval.Attendance.Core.Enums
{
    /// <summary>
    /// Loại hoạt động trong hệ thống
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Kết nối/ngắt kết nối thiết bị
        /// </summary>
        Connection = 1,

        /// <summary>
        /// Đồng bộ dữ liệu nhân viên
        /// </summary>
        EmployeeSync = 2,

        /// <summary>
        /// Đồng bộ dữ liệu chấm công
        /// </summary>
        AttendanceSync = 3,

        /// <summary>
        /// Xuất báo cáo
        /// </summary>
        Export = 4,

        /// <summary>
        /// Nhập dữ liệu
        /// </summary>
        Import = 5,

        /// <summary>
        /// Thay đổi cấu hình hệ thống
        /// </summary>
        Configuration = 6,

        /// <summary>
        /// Bảo trì (xóa dữ liệu, backup, cleanup)
        /// </summary>
        Maintenance = 7,

        /// <summary>
        /// Lỗi hệ thống
        /// </summary>
        SystemError = 8,

        /// <summary>
        /// Hoạt động khác
        /// </summary>
        Other = 99
    }
}
