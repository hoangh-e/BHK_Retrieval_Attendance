namespace BHK.Retrieval.Attendance.Core.DTOs.Requests
{
    /// <summary>
    /// DTO cấu hình xuất file
    /// </summary>
    public class ExportConfigDto
    {
        public ExportFileType FileType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public List<AttendanceDisplayDto> Data { get; set; } = new();
    }

    public enum ExportFileType
    {
        Json,
        Excel,
        Text,
        Csv
    }
}
