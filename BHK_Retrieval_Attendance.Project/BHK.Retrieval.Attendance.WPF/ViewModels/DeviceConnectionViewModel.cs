using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Models.Device;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
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
            TestConnectionCommand = new RelayCommand(async _ => await TestConnectionAsync(), _ => CanTestConnection());
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync(), _ => !IsBusy);

            _logger.LogInformation("DeviceConnectionViewModel initialized with config - IP: {IP}, Port: {Port}, TestMode: {TestMode}", 
                _deviceOptions.DefaultIpAddress, _deviceOptions.DefaultPort, _deviceOptions.Test);

            // Hiển thị thông báo nếu đang ở Test Mode
            if (_deviceOptions.Test)
            {
                StatusMessage = "⚠️ TEST MODE - Connection will be simulated";
                _logger.LogWarning("Application is running in TEST MODE");
            }
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
        public ICommand TestConnectionCommand { get; }
        public ICommand RefreshCommand { get; }

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
                    
                    await _dialogService.ShowMessageAsync(
                        "Success", 
                        _deviceOptions.Test 
                            ? "Connected successfully (TEST MODE)\n\nTest mode is enabled. This is a simulated connection." 
                            : "Connected to device successfully!");

                    // Chuyển sang giao diện kế tiếp sau khi kết nối thành công
                    await NavigateToNextViewAsync();
                }
                else
                {
                    ConnectionModel.IsConnected = false;
                    StatusMessage = "Connection failed";
                    
                    _logger.LogWarning("❌ Connection failed");
                    
                    await _dialogService.ShowMessageAsync(
                        "Error", 
                        "Failed to connect to device.\n\nPlease check:\n" +
                        "• Device is powered on\n" +
                        "• Network connection\n" +
                        "• IP address and port are correct");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during connection");
                StatusMessage = "Connection error";
                ConnectionModel.IsConnected = false;
                
                await _dialogService.ShowMessageAsync("Error", $"Connection error: {ex.Message}");
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

                bool success = await _deviceService.DisconnectAsync();

                if (success)
                {
                    ConnectionModel.IsConnected = false;
                    StatusMessage = "Disconnected";
                    _logger.LogInformation("✅ Disconnected successfully");
                    
                    await _dialogService.ShowMessageAsync("Success", "Disconnected from device successfully");
                }
                else
                {
                    StatusMessage = "Disconnect failed";
                    _logger.LogWarning("❌ Disconnect failed");
                    
                    await _dialogService.ShowMessageAsync("Warning", "Failed to disconnect properly");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during disconnection");
                StatusMessage = "Disconnect error";
                
                await _dialogService.ShowMessageAsync("Error", $"Disconnect error: {ex.Message}");
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
                    
                    await _dialogService.ShowMessageAsync(
                        "Success", 
                        _deviceOptions.Test 
                            ? "Test connection successful (TEST MODE)\n\nThe device is reachable (simulated)." 
                            : "Test connection successful!\n\nThe device is reachable and ready to connect.");
                }
                else
                {
                    StatusMessage = "Test failed";
                    _logger.LogWarning("❌ Test connection failed");
                    
                    await _dialogService.ShowMessageAsync(
                        "Failed", 
                        "Test connection failed.\n\nThe device is not reachable. Please check your network settings.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during test connection");
                StatusMessage = "Test error";
                
                await _dialogService.ShowMessageAsync("Error", $"Test error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
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
                await _dialogService.ShowErrorAsync("Error", $"Failed to refresh settings: {ex.Message}");
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
                _logger.LogInformation("Navigating to Connection Success view after successful connection");
                
                // Delay ngắn để user thấy thông báo
                await Task.Delay(1500);
                
                // Navigate tới ConnectionSuccess view
                _navigationService.NavigateTo("ConnectionSuccess");
                
                _logger.LogInformation("Successfully navigated to Connection Success screen");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to navigate to Connection Success view");
                await _dialogService.ShowMessageAsync("Warning", "Connected successfully but failed to navigate to success screen.");
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