namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho test/xuất dữ liệu Attendance vào Excel
    /// Columns: ID, Date, Time, Verify
    /// </summary>
    public class AttendanceExportDto
    {
        public string ID { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Verify { get; set; } = string.Empty;
    }
}
