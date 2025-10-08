namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// Report configuration options
/// </summary>
public class ReportOptions
{
    public const string SectionName = "ReportSettings";

    public string DefaultFormat { get; set; } = "Excel";
    public string OutputDirectory { get; set; } = "Reports";
    public bool IncludeCharts { get; set; } = true;
    public string CompanyLogo { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}