namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// SharePoint configuration options
/// </summary>
public class SharePointOptions
{
    public const string SectionName = "SharePointSettings";

    public string SiteUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public string ListName { get; set; } = "AttendanceRecords";
    public bool SyncEnabled { get; set; }
    
    /// <summary>
    /// Tên table điểm danh mặc định
    /// </summary>
    public string AttendanceTableName { get; set; } = "AttendanceTable";
}