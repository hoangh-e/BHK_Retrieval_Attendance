namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho xuất dữ liệu Employee chi tiết vào Excel
    /// Columns: ID, Name, IDNumber, Department, Sex, Birthday, Created, Status, Comment, EnrollmentCount
    /// </summary>
    public class EmployeeExportDto
    {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IDNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Created { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
    }
}
