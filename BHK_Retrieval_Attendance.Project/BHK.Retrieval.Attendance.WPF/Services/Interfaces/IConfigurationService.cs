namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces;

/// <summary>
/// Interface for configuration service
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Get application title with version
    /// </summary>
    string GetApplicationTitle();

    /// <summary>
    /// Get database connection string
    /// </summary>
    string GetConnectionString();

    /// <summary>
    /// Get device connection timeout
    /// </summary>
    int GetDeviceTimeout();

    /// <summary>
    /// Get UI window size
    /// </summary>
    (int Width, int Height) GetWindowSize();

    /// <summary>
    /// Log all current configuration values
    /// </summary>
    void LogConfiguration();
}