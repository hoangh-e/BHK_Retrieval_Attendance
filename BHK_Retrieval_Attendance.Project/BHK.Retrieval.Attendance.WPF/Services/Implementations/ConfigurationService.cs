using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations;

/// <summary>
/// Configuration service để minh họa cách sử dụng IOptions pattern
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly ApplicationOptions _appOptions;
    private readonly DatabaseOptions _databaseOptions;
    private readonly DeviceOptions _deviceOptions;
    private readonly UIOptions _uiOptions;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        IOptions<ApplicationOptions> appOptions,
        IOptions<DatabaseOptions> databaseOptions, 
        IOptions<DeviceOptions> deviceOptions,
        IOptions<UIOptions> uiOptions,
        ILogger<ConfigurationService> logger)
    {
        _appOptions = appOptions.Value;
        _databaseOptions = databaseOptions.Value;
        _deviceOptions = deviceOptions.Value;
        _uiOptions = uiOptions.Value;
        _logger = logger;

        _logger.LogInformation("ConfigurationService initialized with app: {AppName} v{Version}", 
            _appOptions.ApplicationName, _appOptions.Version);
    }

    /// <summary>
    /// Get application title with version
    /// </summary>
    public string GetApplicationTitle()
    {
        return $"{_appOptions.ApplicationName} v{_appOptions.Version}";
    }

    /// <summary>
    /// Get database connection string
    /// </summary>
    public string GetConnectionString()
    {
        return _databaseOptions.ConnectionString;
    }

    /// <summary>
    /// Get device connection timeout
    /// </summary>
    public int GetDeviceTimeout()
    {
        return _deviceOptions.ConnectionTimeout;
    }

    /// <summary>
    /// Get UI window size
    /// </summary>
    public (int Width, int Height) GetWindowSize()
    {
        return (_uiOptions.WindowWidth, _uiOptions.WindowHeight);
    }

    /// <summary>
    /// Log all current configuration values
    /// </summary>
    public void LogConfiguration()
    {
        _logger.LogInformation("=== Current Configuration ===");
        _logger.LogInformation("App: {AppName} v{Version} ({Culture})", 
            _appOptions.ApplicationName, _appOptions.Version, _appOptions.Culture);
        _logger.LogInformation("Database: {Provider} - Timeout: {Timeout}s", 
            _databaseOptions.Provider, _databaseOptions.CommandTimeout);
        _logger.LogInformation("Device: {Type} - Timeout: {Timeout}s - Port: {Port}", 
            _deviceOptions.DefaultCommunicationType, _deviceOptions.ConnectionTimeout, _deviceOptions.MonitorPort);
        _logger.LogInformation("UI: {Width}x{Height} - State: {State}", 
            _uiOptions.WindowWidth, _uiOptions.WindowHeight, _uiOptions.WindowState);
        _logger.LogInformation("=============================");
    }
}