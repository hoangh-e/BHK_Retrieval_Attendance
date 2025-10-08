using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.ViewModels;

/// <summary>
/// Demo ViewModel để test configuration từ appsettings.json
/// </summary>
public class ConfigurationDemoViewModel
{
    private readonly IConfigurationService _configService;
    private readonly IOptions<ApplicationOptions> _appOptions;
    private readonly IOptions<DatabaseOptions> _databaseOptions;
    private readonly IOptions<UIOptions> _uiOptions;
    private readonly ILogger<ConfigurationDemoViewModel> _logger;

    public ConfigurationDemoViewModel(
        IConfigurationService configService,
        IOptions<ApplicationOptions> appOptions,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<UIOptions> uiOptions,
        ILogger<ConfigurationDemoViewModel> logger)
    {
        _configService = configService;
        _appOptions = appOptions;
        _databaseOptions = databaseOptions;
        _uiOptions = uiOptions;
        _logger = logger;

        // Test configuration ngay khi khởi tạo
        TestConfiguration();
    }

    private void TestConfiguration()
    {
        _logger.LogInformation("=== TESTING APPSETTINGS.JSON CONFIGURATION ===");

        // Test qua IConfigurationService
        _logger.LogInformation("App Title từ ConfigService: {Title}", _configService.GetApplicationTitle());
        _logger.LogInformation("Window Size từ ConfigService: {Size}", _configService.GetWindowSize());

        // Test trực tiếp qua IOptions
        _logger.LogInformation("App Name từ IOptions: {Name}", _appOptions.Value.ApplicationName);
        _logger.LogInformation("Database Provider từ IOptions: {Provider}", _databaseOptions.Value.Provider);
        _logger.LogInformation("UI Page Size từ IOptions: {PageSize}", _uiOptions.Value.PageSize);

        // Log toàn bộ configuration
        _configService.LogConfiguration();

        _logger.LogInformation("=== CONFIGURATION TEST COMPLETED ===");
    }

    public string AppTitle => _configService.GetApplicationTitle();
    public string ConnectionString => _configService.GetConnectionString();
    public (int Width, int Height) WindowSize => _configService.GetWindowSize();
    public string Culture => _appOptions.Value.Culture;
    public string Theme => _appOptions.Value.Theme;
    public string DateFormat => _uiOptions.Value.DateFormat;
}