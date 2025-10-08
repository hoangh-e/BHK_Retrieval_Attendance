namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// Configuration options cho Device settings từ appsettings.json
/// </summary>
public class DeviceOptions
{
    /// <summary>
    /// Section name trong appsettings.json
    /// </summary>
    public const string SectionName = "DeviceSettings";

    /// <summary>
    /// Chế độ Test - nếu true thì kết nối luôn thành công (demo mode)
    /// </summary>
    public bool Test { get; set; } = false;

    /// <summary>
    /// Địa chỉ IP mặc định của thiết bị
    /// </summary>
    public string DefaultIpAddress { get; set; } = "192.168.1.225";

    /// <summary>
    /// Cổng UDP mặc định
    /// </summary>
    public int DefaultPort { get; set; } = 4370;

    /// <summary>
    /// Device Number mặc định
    /// </summary>
    public int DefaultDeviceNumber { get; set; } = 1;

    /// <summary>
    /// Password mặc định
    /// </summary>
    public string DefaultPassword { get; set; } = "0";

    /// <summary>
    /// Model thiết bị (ZDC2911)
    /// </summary>
    public string DeviceModel { get; set; } = "ZDC2911";

    /// <summary>
    /// Connection Model (5 cho ZD2911 Platform)
    /// </summary>
    public int ConnectionModel { get; set; } = 5;

    /// <summary>
    /// Loại kết nối mặc định (USB/SerialPort/TCP/IP)
    /// </summary>
    public string DefaultCommunicationType { get; set; } = "TCP/IP";

    /// <summary>
    /// Timeout cho kết nối thiết bị (giây)
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30;

    /// <summary>
    /// Số lần thử kết nối lại khi thất bại
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Độ trễ giữa các lần thử lại (giây)
    /// </summary>
    public int RetryDelay { get; set; } = 5;

    /// <summary>
    /// Cổng Monitor (mặc định 4370)
    /// </summary>
    public int MonitorPort { get; set; } = 4370;

    /// <summary>
    /// Thời gian đồng bộ dữ liệu (giây)
    /// </summary>
    public int SyncInterval { get; set; } = 300;

    /// <summary>
    /// Kích thước buffer cho nhận dữ liệu
    /// </summary>
    public int BufferSize { get; set; } = 4096;

    /// <summary>
    /// Bật tính năng tự động kết nối lại
    /// </summary>
    public bool EnableAutoReconnect { get; set; } = true;

    /// <summary>
    /// Ghi log chi tiết cho device communication
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}
