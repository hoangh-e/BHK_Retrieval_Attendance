using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Models.Device;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.WPF.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho giao diện kết nối thiết bị TCP/IP
    /// </summary>
    public class DeviceConnectionViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly ILogger<DeviceConnectionViewModel> _logger;
        private readonly DeviceOptions _deviceOptions;
        private readonly INavigationService _navigationService;

        private DeviceConnectionModel _connectionModel;
        private bool _isBusy;
        private string _statusMessage;

        public DeviceConnectionViewModel(
            IDeviceService deviceService,
            IDialogService dialogService,
            ILogger<DeviceConnectionViewModel> logger,
            IOptions<DeviceOptions> deviceOptions,
            INavigationService navigationService)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            // Khởi tạo Model với giá trị từ appsettings.json
            _connectionModel = new DeviceConnectionModel
            {
                IpAddress = _deviceOptions.DefaultIpAddress,
                Port = _deviceOptions.DefaultPort,
                DeviceNumber = _deviceOptions.DefaultDeviceNumber,
                Password = _deviceOptions.DefaultPassword,
                DeviceModel = _deviceOptions.DeviceModel
            };

            _statusMessage = "Ready to connect";

            // Initialize commands
            ConnectCommand = new RelayCommand(async _ => await ConnectAsync(), _ => CanConnect());
            DisconnectCommand = new RelayCommand(async _ => await DisconnectAsync(), _ => CanDisconnect());
            ManageDevicesCommand = new RelayCommand(async _ => await ManageDevicesAsync(), _ => CanManageDevices());
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync(), _ => !IsBusy);

            _logger.LogInformation("DeviceConnectionViewModel initialized with config - IP: {IP}, Port: {Port}, TestMode: {TestMode}", 
                _deviceOptions.DefaultIpAddress, _deviceOptions.DefaultPort, _deviceOptions.Test);

            // Hiển thị thông báo nếu đang ở Test Mode
            if (_deviceOptions.Test)
            {
                StatusMessage = "⚠️ TEST MODE - Connection will be simulated";
                _logger.LogWarning("Application is running in TEST MODE");
            }
            
            // Check connection status khi khởi tạo
            CheckConnectionStatus();
        }

        #region Properties

        public DeviceConnectionModel ConnectionModel
        {
            get => _connectionModel;
            set => SetProperty(ref _connectionModel, value);
        }

        public new bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    // Refresh command can execute state
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsTestMode => _deviceOptions.Test;

        #endregion

        #region Commands

        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand ManageDevicesCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Kiểm tra và đồng bộ trạng thái kết nối từ DeviceService
        /// </summary>
        public void CheckConnectionStatus()
        {
            bool isConnected = _deviceService.IsConnected;
            
            if (ConnectionModel.IsConnected != isConnected)
            {
                ConnectionModel.IsConnected = isConnected;
                StatusMessage = isConnected ? "Connected" : "Disconnected";
                _logger.LogInformation("✅ Connection status synced: {Status}", StatusMessage);
                
                // Refresh command states
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region Command Implementations

        private bool CanConnect()
        {
            return !IsBusy && !ConnectionModel.IsConnected && !string.IsNullOrWhiteSpace(ConnectionModel.IpAddress);
        }

        private async Task ConnectAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = _deviceOptions.Test ? "Connecting (TEST MODE)..." : "Connecting to device...";
                _logger.LogInformation("Attempting to connect - IP: {IP}, Port: {Port}", 
                    ConnectionModel.IpAddress, ConnectionModel.Port);

                // ✅ Bước 1: Phát hiện tên thiết bị trước khi kết nối
                if (!_deviceOptions.Test)
                {
                    StatusMessage = "Đang phát hiện thiết bị...";
                    string? deviceName = await DetectDeviceNameAsync(ConnectionModel.IpAddress);
                    
                    if (!string.IsNullOrEmpty(deviceName))
                    {
                        ConnectionModel.DeviceName = deviceName;
                        _logger.LogInformation("✅ Device name detected: {DeviceName}", deviceName);
                    }
                    else
                    {
                        ConnectionModel.DeviceName = string.Empty;
                        _logger.LogWarning("⚠️ Could not detect device name, but will continue connecting");
                    }
                }

                // ✅ Bước 2: Thực hiện kết nối với logic ban đầu
                StatusMessage = _deviceOptions.Test ? "Connecting (TEST MODE)..." : "Connecting to device...";
                bool success = await _deviceService.ConnectTcpAsync(
                    ConnectionModel.IpAddress,
                    ConnectionModel.Port,
                    ConnectionModel.DeviceNumber,
                    ConnectionModel.Password);

                if (success)
                {
                    ConnectionModel.IsConnected = true;
                    StatusMessage = _deviceOptions.Test ? "Connected (TEST MODE)" : "Connected successfully";
                    
                    _logger.LogInformation("✅ Connection successful");

                    // ✅ Enable nút Quản lý thiết bị sau khi kết nối thành công
                    // CommandManager.InvalidateRequerySuggested() sẽ trigger CanExecute của ManageDevicesCommand
                }
                else
                {
                    ConnectionModel.IsConnected = false;
                    StatusMessage = "Connection failed";
                    
                    _logger.LogWarning("❌ Connection failed");
                    
                    // ✅ Use DialogHelper for error message
                    DialogHelper.ShowError(
                        "Không thể kết nối đến thiết bị",
                        "Vui lòng kiểm tra:\n" +
                        "• Thiết bị đã được bật nguồn\n" +
                        "• Kết nối mạng\n" +
                        "• Địa chỉ IP và cổng kết nối đúng",
                        "Kết nối thất bại"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during connection");
                StatusMessage = "Connection error";
                ConnectionModel.IsConnected = false;
                
                // ✅ Use DialogHelper for exception error
                DialogHelper.ShowError(
                    "Lỗi khi kết nối thiết bị",
                    ex.Message,
                    "Lỗi kết nối"
                );
            }
            finally
            {
                IsBusy = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool CanDisconnect()
        {
            return !IsBusy && ConnectionModel.IsConnected;
        }

        private async Task DisconnectAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = "Disconnecting...";
                _logger.LogInformation("Attempting to disconnect");

                await _deviceService.DisconnectAsync();

                ConnectionModel.IsConnected = false;
                StatusMessage = "Disconnected";
                _logger.LogInformation("✅ Disconnected successfully");
            }
            catch (Exception ex)
            {
                StatusMessage = "Disconnect failed";
                _logger.LogError(ex, "Disconnect error");
                
                // ✅ Use DialogHelper for error
                DialogHelper.ShowError("Không thể ngắt kết nối thiết bị", "Lỗi");
            }
            finally
            {
                IsBusy = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool CanTestConnection()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(ConnectionModel.IpAddress);
        }

        private async Task TestConnectionAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = _deviceOptions.Test ? "Testing connection (TEST MODE)..." : "Testing connection...";
                _logger.LogInformation("Testing connection - IP: {IP}, Port: {Port}", 
                    ConnectionModel.IpAddress, ConnectionModel.Port);

                bool success = await _deviceService.TestConnectionAsync(
                    ConnectionModel.IpAddress,
                    ConnectionModel.Port,
                    ConnectionModel.DeviceNumber,
                    ConnectionModel.Password);

                if (success)
                {
                    StatusMessage = _deviceOptions.Test ? "Test successful (TEST MODE)" : "Test successful";
                    _logger.LogInformation("✅ Test connection successful");
                    
                    // ✅ Use DialogHelper for test success
                    DialogHelper.ShowSuccess(
                        _deviceOptions.Test 
                            ? "Kiểm tra kết nối thành công (TEST MODE)\n\nThiết bị có thể kết nối được (mô phỏng)." 
                            : "Kiểm tra kết nối thành công!\n\nThiết bị có thể kết nối và sẵn sàng.",
                        "Kiểm tra kết nối"
                    );
                }
                else
                {
                    StatusMessage = "Test failed";
                    _logger.LogWarning("❌ Test connection failed");
                    
                    // ✅ Use DialogHelper for test failure
                    DialogHelper.ShowError(
                        "Kiểm tra kết nối thất bại",
                        "Không thể kết nối đến thiết bị. Vui lòng kiểm tra cài đặt mạng.",
                        "Kiểm tra kết nối"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during test connection");
                StatusMessage = "Test error";
                
                // ✅ Use DialogHelper for test error
                DialogHelper.ShowError("Lỗi khi kiểm tra kết nối", ex.Message, "Lỗi kiểm tra");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanManageDevices()
        {
            return !IsBusy && ConnectionModel.IsConnected;
        }

        private async Task ManageDevicesAsync()
        {
            if (IsBusy) return;

            try
            {
                _logger.LogInformation("Manage Devices clicked - Navigating to next view");
                
                // Chuyển sang giao diện kế tiếp
                await NavigateToNextViewAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating from Manage Devices");
                DialogHelper.ShowError("Lỗi khi chuyển sang giao diện quản lý", ex.Message, "Lỗi");
            }
        }

        private async Task RefreshAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _logger.LogInformation("Refreshing connection settings");
                
                // ✅ CHỈ RESET KHI TEST MODE hoặc user xác nhận
                if (_deviceOptions.Test)
                {
                    // Test mode - tự động reset
                    ConnectionModel.IpAddress = _deviceOptions.DefaultIpAddress;
                    ConnectionModel.Port = _deviceOptions.DefaultPort;
                    ConnectionModel.DeviceNumber = _deviceOptions.DefaultDeviceNumber;
                    ConnectionModel.Password = _deviceOptions.DefaultPassword;
                    StatusMessage = "⚠️ TEST MODE - Settings refreshed to default";
                    _logger.LogInformation("Settings refreshed in TEST MODE");
                }
                else
                {
                    // Production mode - xác nhận trước khi reset
                    bool userConfirmed = await _dialogService.ShowConfirmationAsync(
                        "Reset Settings", 
                        "Do you want to reset connection settings to default values?\n\nThis will overwrite your current settings.");
                    
                    if (userConfirmed)
                    {
                        ConnectionModel.IpAddress = _deviceOptions.DefaultIpAddress;
                        ConnectionModel.Port = _deviceOptions.DefaultPort;
                        ConnectionModel.DeviceNumber = _deviceOptions.DefaultDeviceNumber;
                        ConnectionModel.Password = _deviceOptions.DefaultPassword;
                        StatusMessage = "Settings reset to default values";
                        _logger.LogInformation("Settings reset to default values by user confirmation");
                    }
                    else
                    {
                        StatusMessage = "Refresh cancelled by user";
                        _logger.LogInformation("Settings refresh cancelled by user");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during settings refresh");
                StatusMessage = "Error refreshing settings";
                // ✅ Use DialogHelper for refresh error
                DialogHelper.ShowError("Lỗi khi làm mới cài đặt", ex.Message, "Lỗi làm mới");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Chuyển sang giao diện kế tiếp sau khi kết nối thành công
        /// </summary>
        private async Task NavigateToNextViewAsync()
        {
            try
            {
                _logger.LogInformation("Navigating to HomePage after successful connection");
                
                // Delay ngắn để user thấy thông báo
                await Task.Delay(1500);
                
                // ✅ SỬA: Navigate bằng generic type
                _navigationService.NavigateTo<HomePageViewModel>();
                
                _logger.LogInformation("Successfully navigated to HomePage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to navigate to HomePage");
                DialogHelper.ShowWarning(
                    "Kết nối thành công nhưng không thể chuyển đến màn hình tiếp theo", 
                    ex.Message);
            }
        }

        /// <summary>
        /// Phát hiện tên thiết bị từ IP address bằng Ping và DNS lookup
        /// </summary>
        /// <param name="ipAddress">Địa chỉ IP của thiết bị</param>
        /// <returns>Tên thiết bị nếu phát hiện được, null nếu không</returns>
        private async Task<string?> DetectDeviceNameAsync(string ipAddress)
        {
            try
            {
                _logger.LogInformation("Detecting device name for IP: {IP}", ipAddress);

                // Bước 1: Ping thiết bị để kiểm tra thiết bị có online không
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ipAddress, timeout: 3000);
                    
                    if (reply.Status != IPStatus.Success)
                    {
                        _logger.LogWarning("Ping failed for {IP}: {Status}", ipAddress, reply.Status);
                        return null;
                    }
                    
                    _logger.LogInformation("✅ Ping successful to {IP}, roundtrip: {RoundtripTime}ms", 
                        ipAddress, reply.RoundtripTime);
                }

                // Bước 2: Lấy hostname từ IP bằng reverse DNS lookup
                var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                
                if (!string.IsNullOrEmpty(hostEntry.HostName))
                {
                    _logger.LogInformation("✅ Device name found: {HostName}", hostEntry.HostName);
                    return hostEntry.HostName;
                }
                
                _logger.LogWarning("⚠️ No hostname found for {IP}", ipAddress);
                return null;
            }
            catch (PingException pingEx)
            {
                _logger.LogWarning(pingEx, "Ping exception for {IP}", ipAddress);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to detect device name for {IP}", ipAddress);
                return null;
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