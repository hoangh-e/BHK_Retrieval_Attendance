using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho HomePage
    /// </summary>
    public class HomePageViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<HomePageViewModel> _logger;
        private readonly DeviceOptions _deviceOptions;
        private readonly INavigationService _navigationService;
        private readonly IActivityHistoryService _activityHistoryService;

        private string _ipAddress;
        private int _port;
        private int _deviceNumber;
        private string _deviceModel;
        private DateTime _connectionTime;
        private bool _isTestMode;
        private EmployeeViewModel _employeeViewModel;
        private AttendanceManagementViewModel _attendanceManagementViewModel;
        private SettingsViewModel _settingsViewModel;
        private AboutViewModel _aboutViewModel;
        private ActivityHistoryViewModel _activityHistoryViewModel;
        
        // ✅ Bổ sung thêm thông tin thiết bị
        private string _serialNumber;
        private string _firmwareVersion;
        private int _userCount;
        private int _attendanceRecordCount;
        private string _memoryUsage;
        private string _connectionStatus;

        public HomePageViewModel(
            IDeviceService deviceService,
            ILogger<HomePageViewModel> logger,
            IOptions<DeviceOptions> deviceOptions,
            INavigationService navigationService,
            IActivityHistoryService activityHistoryService,
            EmployeeViewModel employeeViewModel,
            AttendanceManagementViewModel attendanceManagementViewModel,
            SettingsViewModel settingsViewModel,
            AboutViewModel aboutViewModel,
            ActivityHistoryViewModel activityHistoryViewModel)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _activityHistoryService = activityHistoryService ?? throw new ArgumentNullException(nameof(activityHistoryService));
            
            // ✅ THÊM: Khởi tạo EmployeeViewModel
            _employeeViewModel = employeeViewModel ?? throw new ArgumentNullException(nameof(employeeViewModel));
            
            // ✅ THÊM: Khởi tạo AttendanceManagementViewModel
            _attendanceManagementViewModel = attendanceManagementViewModel ?? throw new ArgumentNullException(nameof(attendanceManagementViewModel));
            
            // ✅ THÊM: Khởi tạo SettingsViewModel
            _settingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));

            // ✅ THÊM: Khởi tạo AboutViewModel
            _aboutViewModel = aboutViewModel ?? throw new ArgumentNullException(nameof(aboutViewModel));

            // ✅ THÊM: Khởi tạo ActivityHistoryViewModel
            _activityHistoryViewModel = activityHistoryViewModel ?? throw new ArgumentNullException(nameof(activityHistoryViewModel));

            // Initialize disconnect command
            DisconnectAndReturnCommand = new RelayCommand(async _ => await DisconnectAndReturnAsync(), _ => true);

            // Initialize properties with device connection info
            _ipAddress = _deviceOptions.DefaultIpAddress;
            _port = _deviceOptions.DefaultPort;
            _deviceNumber = _deviceOptions.DefaultDeviceNumber;
            _deviceModel = _deviceOptions.DeviceModel;
            _connectionTime = DateTime.Now;
            _isTestMode = _deviceOptions.Test;
            
            // ✅ Khởi tạo thông tin mở rộng
            _serialNumber = "N/A";
            _firmwareVersion = "N/A";
            _userCount = 0;
            _attendanceRecordCount = 0;
            _memoryUsage = "N/A";
            _connectionStatus = "Connected";

            _logger.LogInformation("HomePageViewModel initialized");
        }

        #region Properties

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        public int DeviceNumber
        {
            get => _deviceNumber;
            set => SetProperty(ref _deviceNumber, value);
        }

        public string DeviceModel
        {
            get => _deviceModel;
            set => SetProperty(ref _deviceModel, value);
        }

        public DateTime ConnectionTime
        {
            get => _connectionTime;
            set => SetProperty(ref _connectionTime, value);
        }

        public bool IsTestMode
        {
            get => _isTestMode;
            set => SetProperty(ref _isTestMode, value);
        }

        /// <summary>
        /// Serial number của thiết bị
        /// </summary>
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        /// <summary>
        /// Phiên bản firmware của thiết bị
        /// </summary>
        public string FirmwareVersion
        {
            get => _firmwareVersion;
            set => SetProperty(ref _firmwareVersion, value);
        }

        /// <summary>
        /// Số lượng nhân viên trong thiết bị
        /// </summary>
        public int UserCount
        {
            get => _userCount;
            set => SetProperty(ref _userCount, value);
        }

        /// <summary>
        /// Số lượng bản ghi chấm công
        /// </summary>
        public int AttendanceRecordCount
        {
            get => _attendanceRecordCount;
            set => SetProperty(ref _attendanceRecordCount, value);
        }

        /// <summary>
        /// Thông tin sử dụng bộ nhớ
        /// </summary>
        public string MemoryUsage
        {
            get => _memoryUsage;
            set => SetProperty(ref _memoryUsage, value);
        }

        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        /// <summary>
        /// ViewModel cho trang quản lý nhân viên
        /// </summary>
        public EmployeeViewModel EmployeeViewModel
        {
            get => _employeeViewModel;
            set => SetProperty(ref _employeeViewModel, value);
        }

        /// <summary>
        /// ViewModel cho trang quản lý chấm công
        /// </summary>
        public AttendanceManagementViewModel AttendanceManagementViewModel
        {
            get => _attendanceManagementViewModel;
            set => SetProperty(ref _attendanceManagementViewModel, value);
        }

        /// <summary>
        /// ViewModel cho trang cài đặt hệ thống
        /// </summary>
        public SettingsViewModel SettingsViewModel
        {
            get => _settingsViewModel;
            set => SetProperty(ref _settingsViewModel, value);
        }

        /// <summary>
        /// ViewModel cho trang thông tin phần mềm
        /// </summary>
        public AboutViewModel AboutViewModel
        {
            get => _aboutViewModel;
            set => SetProperty(ref _aboutViewModel, value);
        }

        /// <summary>
        /// ViewModel cho trang lịch sử hoạt động
        /// </summary>
        public ActivityHistoryViewModel ActivityHistoryViewModel
        {
            get => _activityHistoryViewModel;
            set => SetProperty(ref _activityHistoryViewModel, value);
        }

        /// <summary>
        /// Command để ngắt kết nối và quay về màn hình kết nối
        /// </summary>
        public ICommand DisconnectAndReturnCommand { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load device info - được gọi khi cần refresh thông tin thiết bị
        /// </summary>
        public async Task LoadDeviceInfoAsync()
        {
            _logger.LogInformation("Loading device information");
            
            try
            {
                ConnectionTime = DateTime.Now;
                ConnectionStatus = _deviceService.IsConnected ? "Connected ✅" : "Disconnected ❌";
                
                if (_deviceService.IsConnected)
                {
                    // ✅ Lấy số lượng nhân viên từ thiết bị
                    try
                    {
                        UserCount = await _deviceService.GetUserCountAsync();
                        _logger.LogInformation("User count loaded: {UserCount}", UserCount);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get user count");
                        UserCount = 0;
                    }
                    
                    // ✅ Lấy số lượng bản ghi chấm công (30 ngày gần nhất)
                    try
                    {
                        var endDate = DateTime.Now;
                        var startDate = endDate.AddDays(-30);
                        AttendanceRecordCount = await _deviceService.GetAttendanceRecordCountAsync(startDate, endDate);
                        _logger.LogInformation("Attendance record count loaded: {Count} (last 30 days)", AttendanceRecordCount);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get attendance record count");
                        AttendanceRecordCount = 0;
                    }
                    
                    // ✅ Lấy serial number (nếu có)
                    try
                    {
                        SerialNumber = await _deviceService.GetSerialNumberAsync();
                        _logger.LogInformation("Serial number loaded: {SerialNumber}", SerialNumber);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get serial number");
                        SerialNumber = "N/A";
                    }

                    // ✅ Lấy firmware version từ thiết bị
                    try
                    {
                        FirmwareVersion = await _deviceService.GetFirmwareVersionAsync();
                        _logger.LogInformation("Firmware version loaded: {FirmwareVersion}", FirmwareVersion);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get firmware version");
                        FirmwareVersion = "N/A";
                    }

                    // ✅ Lấy device model từ thiết bị
                    try
                    {
                        DeviceModel = await _deviceService.GetDeviceModelAsync();
                        _logger.LogInformation("Device model loaded: {DeviceModel}", DeviceModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get device model");
                        DeviceModel = _deviceOptions.DeviceModel;
                    }

                    // ✅ TODO: Implement memory usage calculation
                    // Tạm thời không có API để lấy memory usage
                    MemoryUsage = "N/A";
                }
                else
                {
                    // Không kết nối - reset về giá trị mặc định
                    UserCount = 0;
                    AttendanceRecordCount = 0;
                    SerialNumber = "N/A";
                    FirmwareVersion = "N/A";
                    MemoryUsage = "N/A";
                }
                
                _logger.LogInformation("Device info loaded - IP: {IpAddress}, Port: {Port}, Users: {UserCount}", 
                    IpAddress, Port, UserCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading device information");
                ConnectionStatus = "Error ⚠️";
            }
        }

        /// <summary>
        /// Ngắt kết nối thiết bị và quay về màn hình kết nối
        /// </summary>
        private async Task DisconnectAndReturnAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting device and returning to connection view");
                
                // Ngắt kết nối thiết bị
                await _deviceService.DisconnectAsync();
                _logger.LogInformation("✅ Device disconnected successfully");
                
                // ✅ Log disconnect activity to Activity History
                await _activityHistoryService.LogSuccessAsync(
                    IpAddress,
                    null, // Device name not available in HomePage
                    ActivityType.Connection,
                    "Ngắt kết nối thiết bị",
                    $"Device Model: {DeviceModel}, Disconnected from HomePage"
                );
                
                // Chuyển về màn hình kết nối
                _navigationService.NavigateTo<DeviceConnectionViewModel>();
                _logger.LogInformation("✅ Navigated back to DeviceConnectionView");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during disconnect and return");
                
                // ✅ Log failed disconnect to Activity History
                await _activityHistoryService.LogFailedAsync(
                    IpAddress,
                    null,
                    ActivityType.Connection,
                    "Ngắt kết nối thiết bị thất bại",
                    ex.Message,
                    $"Device Model: {DeviceModel}"
                );
            }
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// RelayCommand implementation for ICommand
        /// </summary>
        private class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;

            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }
        }

        #endregion
    }
}