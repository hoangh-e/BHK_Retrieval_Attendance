using System;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations;

public class PathSettingsService : IPathSettingsService
{
    private readonly ILogger<PathSettingsService> _logger;
    private readonly OneDriveOptions _oneDriveOptions;
    private readonly SharePointOptions _sharePointOptions;

    public PathSettingsService(
        ILogger<PathSettingsService> logger,
        IOptions<OneDriveOptions> oneDriveOptions,
        IOptions<SharePointOptions> sharePointOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oneDriveOptions = oneDriveOptions?.Value ?? throw new ArgumentNullException(nameof(oneDriveOptions));
        _sharePointOptions = sharePointOptions?.Value ?? throw new ArgumentNullException(nameof(sharePointOptions));
    }

    public string GetAttendanceExportFolder()
    {
        // Ưu tiên Properties.Settings, fallback về appsettings.json
        var userSetting = Properties.Settings.Default.AttendanceExportFolder;
        
        if (string.IsNullOrWhiteSpace(userSetting))
        {
            _logger.LogInformation("AttendanceExportFolder not set in user settings, using default from appsettings.json");
            return _oneDriveOptions.AttendanceExportFolder;
        }

        return userSetting;
    }

    public void SetAttendanceExportFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        Properties.Settings.Default.AttendanceExportFolder = path;
        Properties.Settings.Default.Save();
        _logger.LogInformation("AttendanceExportFolder saved: {Path}", path);
    }

    public string GetEmployeeDataFilePath()
    {
        var userSetting = Properties.Settings.Default.EmployeeDataFile;
        
        if (string.IsNullOrWhiteSpace(userSetting))
        {
            _logger.LogInformation("EmployeeDataFile not set in user settings, using default from appsettings.json");
            return _oneDriveOptions.EmployeeDataFile;
        }

        return userSetting;
    }

    public void SetEmployeeDataFilePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        Properties.Settings.Default.EmployeeDataFile = path;
        Properties.Settings.Default.Save();
        _logger.LogInformation("EmployeeDataFile saved: {Path}", path);
    }

    public string GetAttendanceTableName()
    {
        var userSetting = Properties.Settings.Default.AttendanceTableName;
        
        if (string.IsNullOrWhiteSpace(userSetting))
        {
            _logger.LogInformation("AttendanceTableName not set, using default from appsettings.json");
            return _sharePointOptions.AttendanceTableName;
        }

        return userSetting;
    }

    public void SetAttendanceTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be empty", nameof(tableName));

        Properties.Settings.Default.AttendanceTableName = tableName;
        Properties.Settings.Default.Save();
        _logger.LogInformation("AttendanceTableName saved: {TableName}", tableName);
    }

    public string GetEmployeeTableName()
    {
        var userSetting = Properties.Settings.Default.EmployeeTableName;
        
        if (string.IsNullOrWhiteSpace(userSetting))
        {
            _logger.LogInformation("EmployeeTableName not set, using default from appsettings.json");
            return _oneDriveOptions.EmployeeTableName;
        }

        return userSetting;
    }

    public void SetEmployeeTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be empty", nameof(tableName));

        Properties.Settings.Default.EmployeeTableName = tableName;
        Properties.Settings.Default.Save();
        _logger.LogInformation("EmployeeTableName saved: {TableName}", tableName);
    }

    public void ResetToDefaults()
    {
        Properties.Settings.Default.AttendanceExportFolder = string.Empty;
        Properties.Settings.Default.EmployeeDataFile = string.Empty;
        Properties.Settings.Default.AttendanceTableName = string.Empty;
        Properties.Settings.Default.EmployeeTableName = string.Empty;
        Properties.Settings.Default.Save();
        _logger.LogInformation("All path settings reset to defaults");
    }
}
