namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO hiển thị bản ghi chấm công trên UI
    /// </summary>
    public class AttendanceDisplayDto
    {
        public ulong DIN { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CheckTime { get; set; }
        public string Date { get; set; } = string.Empty;  // Ngày (dd/MM/yyyy)
        public string Time { get; set; } = string.Empty;  // Giờ (HH:mm:ss)
        public string VerifyMode { get; set; } = string.Empty;  // Vân tay/Thẻ/Mật khẩu
        public string CheckType { get; set; } = string.Empty;  // Check-in/Check-out
        public int DeviceId { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}
