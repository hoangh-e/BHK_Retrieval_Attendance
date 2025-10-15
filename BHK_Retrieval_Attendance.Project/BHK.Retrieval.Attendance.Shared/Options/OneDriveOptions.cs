namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// Options cho cấu hình OneDrive/File Export
/// </summary>
public class OneDriveOptions
{
    public const string SectionName = "OneDriveSettings";

    /// <summary>
    /// Đường dẫn folder mặc định để xuất file điểm danh
    /// </summary>
    public string AttendanceExportFolder { get; set; } = "C:\\Data\\AttendanceExports";

    /// <summary>
    /// Đường dẫn file Excel chứa dữ liệu nhân viên
    /// </summary>
    public string EmployeeDataFile { get; set; } = "C:\\Data\\EmployeeData.xlsx";

    /// <summary>
    /// Tên table nhân viên mặc định
    /// </summary>
    public string EmployeeTableName { get; set; } = "EmployeeTable";
}
