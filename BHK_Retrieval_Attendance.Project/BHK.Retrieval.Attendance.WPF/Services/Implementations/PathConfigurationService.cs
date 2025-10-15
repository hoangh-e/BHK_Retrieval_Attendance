using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.Shared.Options;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service quản lý cấu hình đường dẫn file/folder
    /// ✅ TÁI SỬ DỤNG cho tất cả dialog xuất file
    /// Logic: Properties.Settings (user custom) > appsettings.json (default)
    /// </summary>
    public class PathConfigurationService : IPathConfigurationService
    {
        private readonly OneDriveSettings _oneDriveSettings;
        private readonly SharePointSettings _sharePointSettings;
        private readonly ILogger<PathConfigurationService> _logger;

        public PathConfigurationService(
            IOptions<OneDriveSettings> oneDriveSettings,
            IOptions<SharePointSettings> sharePointSettings,
            ILogger<PathConfigurationService> logger)
        {
            _oneDriveSettings = oneDriveSettings.Value;
            _sharePointSettings = sharePointSettings.Value;
            _logger = logger;
        }

        #region Get Methods

        public string GetAttendanceExportFolder()
        {
            // Kiểm tra Properties.Settings (user đã set)
            var userSetting = Properties.Settings.Default.AttendanceExportFolder;
            
            if (!string.IsNullOrWhiteSpace(userSetting))
            {
                _logger.LogDebug($"Using user-configured AttendanceExportFolder: {userSetting}");
                return userSetting;
            }

            // Fallback về appsettings.json
            _logger.LogDebug($"Using default AttendanceExportFolder from appsettings: {_oneDriveSettings.AttendanceExportFolder}");
            return _oneDriveSettings.AttendanceExportFolder;
        }

        public string GetEmployeeDataFile()
        {
            var userSetting = Properties.Settings.Default.EmployeeDataFile;
            
            if (!string.IsNullOrWhiteSpace(userSetting))
            {
                _logger.LogDebug($"Using user-configured EmployeeDataFile: {userSetting}");
                return userSetting;
            }

            _logger.LogDebug($"Using default EmployeeDataFile from appsettings: {_oneDriveSettings.EmployeeDataFile}");
            return _oneDriveSettings.EmployeeDataFile;
        }

        public string GetAttendanceTableName()
        {
            var userSetting = Properties.Settings.Default.AttendanceTableName;
            
            if (!string.IsNullOrWhiteSpace(userSetting))
            {
                _logger.LogDebug($"Using user-configured AttendanceTableName: {userSetting}");
                return userSetting;
            }

            _logger.LogDebug($"Using default AttendanceTableName from appsettings: {_sharePointSettings.AttendanceTableName}");
            return _sharePointSettings.AttendanceTableName;
        }

        public string GetEmployeeTableName()
        {
            var userSetting = Properties.Settings.Default.EmployeeTableName;
            
            if (!string.IsNullOrWhiteSpace(userSetting))
            {
                _logger.LogDebug($"Using user-configured EmployeeTableName: {userSetting}");
                return userSetting;
            }

            _logger.LogDebug($"Using default EmployeeTableName from appsettings: {_oneDriveSettings.EmployeeTableName}");
            return _oneDriveSettings.EmployeeTableName;
        }

        #endregion

        #region Save Methods

        public void SaveAttendanceExportFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogWarning("Attempted to save empty AttendanceExportFolder");
                return;
            }

            Properties.Settings.Default.AttendanceExportFolder = path;
            Properties.Settings.Default.Save();
            _logger.LogInformation($"Saved AttendanceExportFolder: {path}");
        }

        public void SaveEmployeeDataFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogWarning("Attempted to save empty EmployeeDataFile");
                return;
            }

            Properties.Settings.Default.EmployeeDataFile = path;
            Properties.Settings.Default.Save();
            _logger.LogInformation($"Saved EmployeeDataFile: {path}");
        }

        public void SaveAttendanceTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                _logger.LogWarning("Attempted to save empty AttendanceTableName");
                return;
            }

            Properties.Settings.Default.AttendanceTableName = tableName;
            Properties.Settings.Default.Save();
            _logger.LogInformation($"Saved AttendanceTableName: {tableName}");
        }

        public void SaveEmployeeTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                _logger.LogWarning("Attempted to save empty EmployeeTableName");
                return;
            }

            Properties.Settings.Default.EmployeeTableName = tableName;
            Properties.Settings.Default.Save();
            _logger.LogInformation($"Saved EmployeeTableName: {tableName}");
        }

        #endregion
    }
}
