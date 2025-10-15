namespace BHK.Retrieval.Attendance.Shared.Options
{
    /// <summary>
    /// Cấu hình SharePoint settings từ appsettings.json
    /// </summary>
    public class SharePointSettings
    {
        public string SiteUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string ListName { get; set; } = "AttendanceRecords";
        public bool SyncEnabled { get; set; }
        public string AttendanceTableName { get; set; } = "AttendanceTable";
    }
}
