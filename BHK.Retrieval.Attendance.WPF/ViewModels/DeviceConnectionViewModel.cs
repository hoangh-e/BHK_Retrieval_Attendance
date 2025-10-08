using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.WPF.Models.Device;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho giao diện kết nối TCP/IP
    /// </summary>
    public class DeviceConnectionViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly ILogger<DeviceConnectionViewModel> _logger;
        
        private DeviceConnectionModel _connectionModel;

        public DeviceConnectionViewModel(
            IDeviceService deviceService,
            IDialogService dialogService,
            ILogger<DeviceConnectionViewModel> logger)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Title = "Device Connection - TCP/IP";
            ConnectionModel = new DeviceConnectionModel();

            // Khởi tạo Commands
            ConnectCommand = new AsyncRelayCommand(ConnectAsync, CanConnect);
            DisconnectCommand = new AsyncRelayCommand(DisconnectAsync, CanDisconnect);
            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync, CanTestConnection);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        #region Properties

        /// <summary>
        /// Model chứa thông tin kết nối
        /// </summary>
        public DeviceConnectionModel ConnectionModel
        {
            get => _connectionModel;
            set => SetProperty(ref _connectionModel, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command để kết nối thiết bị
        /// </summary>
        public IAsyncRelayCommand ConnectCommand { get; }

        /// <summary>
        /// Command để ngắt kết nối
        /// </summary>
        public IAsyncRelayCommand DisconnectCommand { get; }

        /// <summary>
        /// Command để kiểm tra kết nối
        /// </summary>
        public IAsyncRelayCommand TestConnectionCommand { get; }

        /// <summary>
        /// Command để refresh trạng thái
        /// </summary>
        public IAsyncRelayCommand RefreshCommand { get; }

        #endregion

        #region Command Methods

        /// <summary>
        /// Kiểm tra có thể kết nối không
        /// </summary>
        private bool CanConnect()
        {
            return !ConnectionModel.IsConnected && 
                   !string.IsNullOrWhiteSpace(ConnectionModel.IpAddress) &&
                   !IsBusy;
        }

        /// <summary>
        /// Thực hiện kết nối tới thiết bị
        /// </summary>
        private async Task ConnectAsync()
        {
            try
            {
                IsBusy = true;
                ConnectionModel.ConnectionStatus = "Connecting...";
                _logger.LogInformation("Attempting to connect to device at {IpAddress}:{Port}", 
                    ConnectionModel.IpAddress, ConnectionModel.Port);

                // TODO: Gọi service để kết nối thiết bị
                var result = await _deviceService.ConnectTcpAsync(
                    ConnectionModel.IpAddress,
                    ConnectionModel.Port,
                    ConnectionModel.DeviceNumber,
                    ConnectionModel.Password
                );

                if (result)
                {
                    ConnectionModel.IsConnected = true;
                    ConnectionModel.ConnectionStatus = $"Connected to {ConnectionModel.IpAddress}";
                    
                    _logger.LogInformation("Successfully connected to device");
                    await _dialogService.ShowMessageAsync("Success", "Device connected successfully!", "OK");
                }
                else
                {
                    ConnectionModel.IsConnected = false;
                    ConnectionModel.ConnectionStatus = "Connection Failed";
                    await _dialogService.ShowErrorAsync("Connection Error", "Failed to connect to device");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to device");
                ConnectionModel.IsConnected = false;
                ConnectionModel.ConnectionStatus = "Connection Failed";
                await _dialogService.ShowErrorAsync("Connection Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
                ConnectCommand.NotifyCanExecuteChanged();
                DisconnectCommand.NotifyCanExecuteChanged();
                TestConnectionCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Kiểm tra có thể ngắt kết nối không
        /// </summary>
        private bool CanDisconnect()
        {
            return ConnectionModel.IsConnected && !IsBusy;
        }

        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        private async Task DisconnectAsync()
        {
            try
            {
                IsBusy = true;
                ConnectionModel.ConnectionStatus = "Disconnecting...";
                _logger.LogInformation("Disconnecting from device");

                await _deviceService.DisconnectAsync();

                ConnectionModel.IsConnected = false;
                ConnectionModel.ConnectionStatus = "Disconnected";
                
                _logger.LogInformation("Successfully disconnected from device");
                await _dialogService.ShowMessageAsync("Disconnected", "Device disconnected successfully!", "OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disconnect from device");
                await _dialogService.ShowErrorAsync("Disconnect Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
                ConnectCommand.NotifyCanExecuteChanged();
                DisconnectCommand.NotifyCanExecuteChanged();
                TestConnectionCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Kiểm tra có thể test kết nối không
        /// </summary>
        private bool CanTestConnection()
        {
            return !string.IsNullOrWhiteSpace(ConnectionModel.IpAddress) && !IsBusy;
        }

        /// <summary>
        /// Test kết nối tới thiết bị
        /// </summary>
        private async Task TestConnectionAsync()
        {
            try
            {
                IsBusy = true;
                ConnectionModel.ConnectionStatus = "Testing connection...";
                _logger.LogInformation("Testing connection to device");

                var isReachable = await _deviceService.TestConnectionAsync(
                    ConnectionModel.IpAddress,
                    ConnectionModel.Port
                );

                if (isReachable)
                {
                    ConnectionModel.ConnectionStatus = "Device is reachable";
                    await _dialogService.ShowMessageAsync("Test Result", 
                        $"Device at {ConnectionModel.IpAddress}:{ConnectionModel.Port} is reachable!", "OK");
                }
                else
                {
                    ConnectionModel.ConnectionStatus = "Device is not reachable";
                    await _dialogService.ShowWarningAsync("Test Result", 
                        "Device is not reachable. Please check IP and Port.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test connection");
                ConnectionModel.ConnectionStatus = "Test failed";
                await _dialogService.ShowErrorAsync("Test Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Refresh trạng thái kết nối
        /// </summary>
        private async Task RefreshAsync()
        {
            try
            {
                IsBusy = true;
                _logger.LogInformation("Refreshing device status");

                var status = await _deviceService.GetDeviceStatusAsync();
                ConnectionModel.ConnectionStatus = status;

                _logger.LogInformation("Device status refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh device status");
                await _dialogService.ShowErrorAsync("Refresh Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validate địa chỉ IP
        /// </summary>
        private bool ValidateIpAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value) || value < 0 || value > 255)
                    return false;
            }

            return true;
        }

        #endregion
    }
}
