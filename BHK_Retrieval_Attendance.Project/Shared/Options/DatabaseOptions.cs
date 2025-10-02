namespace BHK.Retrieval.Attendance.Shared.Options
{
    public class DatabaseOptions
    {
        public string Provider { get; set; } = "SqlServer";
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; }
        public bool EnableDetailedErrors { get; set; }
    }
}
