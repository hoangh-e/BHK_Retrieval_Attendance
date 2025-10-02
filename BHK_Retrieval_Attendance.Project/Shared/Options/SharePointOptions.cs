namespace BHK.Retrieval.Attendance.Shared.Options
{
    public class SharePointOptions
    {
        public string SiteUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string ListName { get; set; } = "AttendanceRecords";
        public bool SyncEnabled { get; set; }
    }
}
