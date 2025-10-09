using System;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
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

        private string _ipAddress;
        private int _port;
        private int _deviceNumber;
        private string _deviceModel;
        private DateTime _connectionTime;
        private bool _isTestMode;
        private EmployeeViewModel _employeeViewModel;
        
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
            EmployeeViewModel employeeViewModel)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            
            // ✅ THÊM: Khởi tạo EmployeeViewModel
            _employeeViewModel = employeeViewModel ?? throw new ArgumentNullException(nameof(employeeViewModel));

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
                        if (_isTestMode)
                        {
                            SerialNumber = "TEST-SN-12345678";
                            FirmwareVersion = "v2.5.0 (TEST)";
                            MemoryUsage = "25% (TEST)";
                        }
                        else
                        {
                            // TODO: Implement GetSerialNumberAsync, GetFirmwareVersionAsync trong DeviceService
                            SerialNumber = "N/A";
                            FirmwareVersion = "N/A";
                            MemoryUsage = "N/A";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get additional device info");
                        SerialNumber = "N/A";
                        FirmwareVersion = "N/A";
                        MemoryUsage = "N/A";
                    }
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

        #endregion
    }
}