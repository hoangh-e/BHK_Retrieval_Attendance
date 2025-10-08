namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// Device configuration options
/// </summary>
public class DeviceOptions
{
    public const string SectionName = "DeviceSettings";

    public string DefaultCommunicationType { get; set; } = "TCP/IP";
    public int ConnectionTimeout { get; set; } = 30;
    public int RetryAttempts { get; set; } = 3;
    public int RetryDelay { get; set; } = 5;
    public string DevicePassword { get; set; } = string.Empty;
    public string MonitorMode { get; set; } = "UDP";
    public int MonitorPort { get; set; } = 4370;
    public int SyncInterval { get; set; } = 300;
}
