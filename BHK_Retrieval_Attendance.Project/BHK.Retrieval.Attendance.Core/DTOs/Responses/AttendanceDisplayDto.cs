namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO hiển thị bản ghi chấm công trên UI
    /// Chứa đầy đủ dữ liệu: STT, DN, DIN, Ngày, Giờ, Loại, Phương thức, Hành động, Ghi chú
    /// </summary>
    public class AttendanceDisplayDto
    {
        /// <summary>
        /// Số thứ tự (STT) - Index cho DataGrid
        /// </summary>
        public int Index { get; set; }
        
        /// <summary>
        /// Device Number - Số hiệu thiết bị
        /// </summary>
        public string DN { get; set; } = string.Empty;
        
        /// <summary>
        /// Device Identification Number - Mã nhân viên trên thiết bị
        /// </summary>
        public string DIN { get; set; } = string.Empty;
        
        /// <summary>
        /// Ngày chấm công (format: dd/MM/yyyy)
        /// </summary>
        public string Date { get; set; } = string.Empty;
        
        /// <summary>
        /// Giờ chấm công (format: HH:mm:ss)
        /// </summary>
        public string Time { get; set; } = string.Empty;
        
        /// <summary>
        /// Loại chấm công: "Check In" hoặc "Check Out"
        /// Logic: 4-11h = Check In, 13-18h = Check Out
        /// </summary>
        public string Type { get; set; } = string.Empty;
        
        /// <summary>
        /// Phương thức xác minh: PW, FP, Card, Face, Iris
        /// </summary>
        public string Verify { get; set; } = string.Empty;
        
        /// <summary>
        /// Hành động: In, Out, Break, etc.
        /// </summary>
        public string Action { get; set; } = string.Empty;
        
        /// <summary>
        /// Ghi chú (nếu có)
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }
}
