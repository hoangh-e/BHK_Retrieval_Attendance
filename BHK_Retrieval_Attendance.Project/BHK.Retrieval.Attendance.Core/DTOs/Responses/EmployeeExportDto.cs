namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho xuất dữ liệu Employee vào Excel
    /// Columns: DIN, Name, Sex, Birthday, Created, Status, Comment
    /// </summary>
    public class EmployeeExportDto
    {
        public string DIN { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Created { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
