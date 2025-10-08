namespace BHK.Retrieval.Attendance.WPF.Models.Device
{
    /// <summary>
    /// Model chứa thông tin chi tiết thiết bị
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// Tên thiết bị
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// Số serial thiết bị
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Phiên bản firmware
        /// </summary>
        public string FirmwareVersion { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng người dùng tối đa
        /// </summary>
        public int MaxUsers { get; set; }

        /// <summary>
        /// Số lượng bản ghi chấm công tối đa
        /// </summary>
        public int MaxLogs { get; set; }

        /// <summary>
        /// Thời gian của thiết bị
        /// </summary>
        public DateTime? DeviceTime { get; set; }
    }
}