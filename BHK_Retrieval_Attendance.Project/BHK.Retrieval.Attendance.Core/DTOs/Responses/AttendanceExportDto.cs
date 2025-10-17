namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho xuất dữ liệu Attendance vào Excel
    /// Columns: DeviceNumber, DIN, Date, Time, Verify, Action
    /// </summary>
    public class AttendanceExportDto
    {
        public string DeviceNumber { get; set; } = string.Empty;
        public string DIN { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Verify { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}
