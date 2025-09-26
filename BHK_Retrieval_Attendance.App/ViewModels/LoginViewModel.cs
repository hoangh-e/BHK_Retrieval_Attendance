using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Models;
using BHK.Retrieval.Attendance.WPF.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginViewModel> _logger;

        public LoginViewModel(IUserService userService,
                            IDeviceService deviceService,
                            INavigationService navigationService,
                            IDialogService dialogService,
                            IConfiguration configuration,
                            ILogger<LoginViewModel> logger)
        {
            _userService = userService;
            _deviceService = deviceService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _configuration = configuration;
            _logger = logger;

            // Initialize commands
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);

            // Load saved settings
            LoadSavedSettings();

            // Start monitoring device connection
            StartDeviceConnectionMonitoring();
        }

        #region Properties

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        private bool _rememberMe = false;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool CanLogin => !IsLoading && 
                               !string.IsNullOrWhiteSpace(Username) && 
                               !string.IsNullOrWhiteSpace(Password);

        private DeviceConnectionStatus _deviceConnectionStatus = DeviceConnectionStatus.Disconnected;
        public DeviceConnectionStatus DeviceConnectionStatus
        {
            get => _deviceConnectionStatus;
            set
            {
                _deviceConnectionStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceConnectionText));
            }
        }

        public string DeviceConnectionText
        {
            get
            {
                return DeviceConnectionStatus switch
                {
                    DeviceConnectionStatus.Connected => "Thiết bị đã kết nối",
                    DeviceConnectionStatus.Connecting => "Đang kết nối thiết bị...",
                    DeviceConnectionStatus.Disconnected => "Thiết bị chưa kết nối",
                    DeviceConnectionStatus.Error => "Lỗi kết nối thiết bị",
                    _ => "Không xác định"
                };
            }
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }

        #endregion

        #region Command Implementations

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = "";
                
                _logger.LogInformation($"Attempting login for user: {Username}");

                // Validate credentials
                var loginResult = await _userService.ValidateUserAsync(Username, Password);
                
                if (loginResult.IsSuccess && loginResult.Data != null)
                {
                    var user = loginResult.Data;
                    _logger.LogInformation($"Login successful for user: {user.Username}");

                    // Save credentials if remember me is checked
                    if (RememberMe)
                    {
                        SaveUserCredentials();
                    }
                    else
                    {
                        ClearSavedCredentials();
                    }

                    // Set current user in user service
                    await _userService.SetCurrentUserAsync(user);

                    // Navigate to main application
                    _navigationService.NavigateToMainWindow();
                }
                else
                {
                    ErrorMessage = loginResult.ErrorMessage ?? "Thông tin đăng nhập không chính xác.";
                    _logger.LogWarning($"Login failed for user {Username}: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Có lỗi xảy ra trong quá trình đăng nhập.";
                _logger.LogError(ex, $"Login error for user: {Username}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        private void LoadSavedSettings()
        {
            try
            {
                var savedUsername = _configuration["Login:SavedUsername"];
                var rememberMeSetting = _configuration["Login:RememberMe"];

                if (!string.IsNullOrEmpty(savedUsername))
                {
                    Username = savedUsername;
                    RememberMe = bool.TryParse(rememberMeSetting, out var remember) && remember;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading saved login settings");
            }
        }

        private void SaveUserCredentials()
        {
            try
            {
                // In a production app, you'd want to encrypt the password or use secure storage
                // For this demo, we'll just save the username
                _configuration["Login:SavedUsername"] = Username;
                _configuration["Login:RememberMe"] = RememberMe.ToString();
                
                _logger.LogInformation("User credentials saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user credentials");
            }
        }

        private void ClearSavedCredentials()
        {
            try
            {
                _configuration["Login:SavedUsername"] = "";
                _configuration["Login:RememberMe"] = "false";
                
                _logger.LogInformation("Saved credentials cleared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing saved credentials");
            }
        }

        private async void StartDeviceConnectionMonitoring()
        {
            try
            {
                // Start monitoring device connection status
                while (true)
                {
                    var connectionStatus = await _deviceService.CheckConnectionAsync();
                    DeviceConnectionStatus = connectionStatus.IsSuccess ? 
                        DeviceConnectionStatus.Connected : 
                        DeviceConnectionStatus.Disconnected;

                    // Wait 5 seconds before next check
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in device connection monitoring");
                DeviceConnectionStatus = DeviceConnectionStatus.Error;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}