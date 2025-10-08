using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho giao diện Connection Success
    /// </summary>
    public class ConnectionSuccessViewModel : BaseViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly INavigationService _navigationService;
        private readonly ILogger<ConnectionSuccessViewModel> _logger;
        private readonly DeviceOptions _deviceOptions;

        private string _ipAddress;
        private int _port;
        private int _deviceNumber;
        private string _deviceModel;
        private DateTime _connectionTime;
        private bool _isTestMode;

        public ConnectionSuccessViewModel(
            IDeviceService deviceService,
            INavigationService navigationService,
            ILogger<ConnectionSuccessViewModel> logger,
            IOptions<DeviceOptions> deviceOptions)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));

            // Initialize properties
            _ipAddress = _deviceOptions.DefaultIpAddress;
            _port = _deviceOptions.DefaultPort;
            _deviceNumber = _deviceOptions.DefaultDeviceNumber;
            _deviceModel = _deviceOptions.DeviceModel;
            _connectionTime = DateTime.Now;
            _isTestMode = _deviceOptions.Test;

            // Initialize commands
            NavigateToDashboardCommand = new RelayCommand(async _ => await NavigateToDashboardAsync());
            DisconnectCommand = new RelayCommand(async _ => await DisconnectAsync());

            _logger.LogInformation("ConnectionSuccessViewModel initialized");
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

        #region Commands

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand DisconnectCommand { get; }

        #endregion

        #region Command Implementations

        private async Task NavigateToDashboardAsync()
        {
            try
            {
                _logger.LogInformation("Navigating to Dashboard from Connection Success screen");
                
                // TODO: Navigate to actual Dashboard when it's implemented
                _navigationService.NavigateTo("Dashboard");
                
                _logger.LogInformation("Successfully navigated to Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to navigate to Dashboard");
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting from device");
                
                bool success = await _deviceService.DisconnectAsync();
                
                if (success)
                {
                    _logger.LogInformation("Disconnected successfully, navigating back to connection screen");
                    _navigationService.NavigateTo("DeviceConnection");
                }
                else
                {
                    _logger.LogWarning("Disconnect failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during disconnection");
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
                add => System.Windows.Input.CommandManager.RequerySuggested += value;
                remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
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

        /// <summary>
        /// Cập nhật thông tin từ connection model
        /// </summary>
        public void UpdateConnectionInfo(string ipAddress, int port, int deviceNumber, string deviceModel)
        {
            IpAddress = ipAddress;
            Port = port;
            DeviceNumber = deviceNumber;
            DeviceModel = deviceModel;
            ConnectionTime = DateTime.Now;
            IsTestMode = _deviceOptions.Test;

            _logger.LogInformation("Connection info updated - IP: {IP}, Port: {Port}, DN: {DN}", 
                ipAddress, port, deviceNumber);
        }
    }
}