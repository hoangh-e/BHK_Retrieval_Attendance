namespace BHK.Retrieval.Attendance.Shared.Options;

/// <summary>
/// UI configuration options
/// </summary>
public class UIOptions
{
    public const string SectionName = "UISettings";

    public int WindowWidth { get; set; } = 1280;
    public int WindowHeight { get; set; } = 720;
    public string WindowState { get; set; } = "Normal";
    public string StartPage { get; set; } = "Dashboard";
    public int PageSize { get; set; } = 50;
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "HH:mm:ss";
}