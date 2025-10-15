namespace BHK.Retrieval.Attendance.Shared.Options
{
    /// <summary>
    /// Cấu hình OneDrive settings từ appsettings.json
    /// </summary>
    public class OneDriveSettings
    {
        public string AttendanceExportFolder { get; set; } = "C:\\Data\\AttendanceExports";
        public string EmployeeDataFile { get; set; } = "C:\\Data\\EmployeeData.xlsx";
        public string EmployeeTableName { get; set; } = "EmployeeTable";
    }
}
