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

        public HomePageViewModel(
            IDeviceService deviceService,
            ILogger<HomePageViewModel> logger,
            IOptions<DeviceOptions> deviceOptions)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));

            // Initialize properties with device connection info
            _ipAddress = _deviceOptions.DefaultIpAddress;
            _port = _deviceOptions.DefaultPort;
            _deviceNumber = _deviceOptions.DefaultDeviceNumber;
            _deviceModel = _deviceOptions.DeviceModel;
            _connectionTime = DateTime.Now;
            _isTestMode = _deviceOptions.Test;

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Load device info - được gọi khi cần refresh thông tin thiết bị
        /// </summary>
        public void LoadDeviceInfo()
        {
            _logger.LogInformation("Loading device information");
            
            // TODO: Lấy thông tin thực tế từ DeviceService khi có API
            // Hiện tại sử dụng thông tin từ Options
            
            ConnectionTime = DateTime.Now;
            
            _logger.LogInformation($"Device info loaded - IP: {IpAddress}, Port: {Port}");
        }

        #endregion
    }
}