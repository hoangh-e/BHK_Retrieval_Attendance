using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Service quản lý cấu hình đường dẫn file/folder
    /// ✅ TÁI SỬ DỤNG cho tất cả dialog xuất file
    /// Logic: Properties.Settings (user) > appsettings.json (default)
    /// </summary>
    public interface IPathConfigurationService
    {
        /// <summary>
        /// Lấy đường dẫn folder xuất file attendance
        /// </summary>
        string GetAttendanceExportFolder();

        /// <summary>
        /// Lấy đường dẫn file Excel nhân viên
        /// </summary>
        string GetEmployeeDataFile();

        /// <summary>
        /// Lấy tên table attendance mặc định
        /// </summary>
        string GetAttendanceTableName();

        /// <summary>
        /// Lấy tên table employee mặc định
        /// </summary>
        string GetEmployeeTableName();

        /// <summary>
        /// Lưu đường dẫn folder xuất attendance vào Properties.Settings
        /// </summary>
        void SaveAttendanceExportFolder(string path);

        /// <summary>
        /// Lưu đường dẫn file Excel nhân viên vào Properties.Settings
        /// </summary>
        void SaveEmployeeDataFile(string path);

        /// <summary>
        /// Lưu tên table attendance vào Properties.Settings
        /// </summary>
        void SaveAttendanceTableName(string tableName);

        /// <summary>
        /// Lưu tên table employee vào Properties.Settings
        /// </summary>
        void SaveEmployeeTableName(string tableName);
    }
}
