namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// Application configuration options
/// </summary>
public class ApplicationOptions
{
    public const string SectionName = "ApplicationSettings";

    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Culture { get; set; } = "vi-VN";
    public string Theme { get; set; } = "Light";
}
