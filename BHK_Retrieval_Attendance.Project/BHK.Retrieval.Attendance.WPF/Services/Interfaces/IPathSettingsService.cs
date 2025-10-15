using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces;

/// <summary>
/// Service quản lý settings đường dẫn và table name
/// Đồng bộ giữa Properties.Settings và appsettings.json
/// </summary>
public interface IPathSettingsService
{
    /// <summary>
    /// Lấy đường dẫn folder xuất file điểm danh
    /// Ưu tiên: Properties.Settings -> appsettings.json
    /// </summary>
    string GetAttendanceExportFolder();

    /// <summary>
    /// Lưu đường dẫn folder xuất file điểm danh vào Properties.Settings
    /// </summary>
    void SetAttendanceExportFolder(string path);

    /// <summary>
    /// Lấy đường dẫn file Excel dữ liệu nhân viên
    /// </summary>
    string GetEmployeeDataFilePath();

    /// <summary>
    /// Lưu đường dẫn file Excel nhân viên
    /// </summary>
    void SetEmployeeDataFilePath(string path);

    /// <summary>
    /// Lấy tên table điểm danh mặc định
    /// </summary>
    string GetAttendanceTableName();

    /// <summary>
    /// Lưu tên table điểm danh
    /// </summary>
    void SetAttendanceTableName(string tableName);

    /// <summary>
    /// Lấy tên table nhân viên mặc định
    /// </summary>
    string GetEmployeeTableName();

    /// <summary>
    /// Lưu tên table nhân viên
    /// </summary>
    void SetEmployeeTableName(string tableName);

    /// <summary>
    /// Reset tất cả settings về mặc định từ appsettings.json
    /// </summary>
    void ResetToDefaults();
}
