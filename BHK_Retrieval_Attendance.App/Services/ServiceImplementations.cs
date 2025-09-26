using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.WPF.Models;
using Riss.Devices;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly ILogger<DeviceService> _logger;
        private DeviceConnection? _deviceConnection;
        
        public DeviceService(ILogger<DeviceService> logger)
        {
            _logger = logger;
        }

        public bool IsConnected => _deviceConnection?.IsConnected ?? false;

        public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        public event EventHandler<DeviceDataReceivedEventArgs>? DeviceDataReceived;

        public async Task<ServiceResult<bool>> ConnectAsync(DeviceConnectionConfig config)
        {
            try
            {
                _deviceConnection = new DeviceConnection();
                
                // Simulate connection logic based on config type
                await Task.Delay(1000); // Simulate connection time
                
                ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    Status = ConnectionStatus.Connected,
                    Message = "Kết nối thiết bị thành công"
                });

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to device");
                ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    Status = ConnectionStatus.Error,
                    Message = $"Lỗi kết nối: {ex.Message}"
                });
                return ServiceResult<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DisconnectAsync()
        {
            try
            {
                await Task.Delay(500);
                _deviceConnection = null;
                
                ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    Status = ConnectionStatus.Disconnected,
                    Message = "Đã ngắt kết nối thiết bị"
                });

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disconnect from device");
                return ServiceResult<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> TestConnectionAsync(DeviceConnectionConfig config)
        {
            try
            {
                await Task.Delay(500);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<DeviceInfo>> GetDeviceInfoAsync()
        {
            try
            {
                await Task.Delay(200);
                var deviceInfo = new DeviceInfo
                {
                    Name = "ZD2911 Attendance Device",
                    SerialNumber = "ZD2911001",
                    Model = "ZD2911",
                    UserCount = 150,
                    RecordCount = 5000,
                    FreeSpace = 85,
                    DateTime = DateTime.Now,
                    FirmwareVersion = "1.2.3"
                };

                return ServiceResult<DeviceInfo>.Success(deviceInfo);
            }
            catch (Exception ex)
            {
                return ServiceResult<DeviceInfo>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<List<UserInfo>>> GetAllEmployeesAsync()
        {
            try
            {
                await Task.Delay(1000);
                var users = new List<UserInfo>();
                // Simulate getting user data
                return ServiceResult<List<UserInfo>>.Success(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<UserInfo>>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<List<UserInfo>>> SynchronizeEmployeeDataAsync()
        {
            try
            {
                await Task.Delay(2000);
                var users = new List<UserInfo>();
                return ServiceResult<List<UserInfo>>.Success(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<UserInfo>>.Failure(ex.Message);
            }
        }
    }

    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _backStack = new();
        private readonly Stack<object> _forwardStack = new();
        private object? _currentView;

        public bool CanGoBack => _backStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public void NavigateTo(object view)
        {
            if (_currentView != null)
            {
                _backStack.Push(_currentView);
            }
            
            _currentView = view;
            _forwardStack.Clear();
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            if (_currentView != null)
            {
                _forwardStack.Push(_currentView);
            }

            _currentView = _backStack.Pop();
        }

        public void GoForward()
        {
            if (!CanGoForward) return;

            if (_currentView != null)
            {
                _backStack.Push(_currentView);
            }

            _currentView = _forwardStack.Pop();
        }
    }

    public class DialogService : IDialogService
    {
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            await Task.Delay(100);
            return System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes;
        }

        public async Task ShowInformationAsync(string title, string message)
        {
            await Task.Delay(100);
            System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            await Task.Delay(100);
            System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        public async Task<string?> ShowInputAsync(string title, string message)
        {
            await Task.Delay(100);
            // Simple implementation - in real app you'd use a proper input dialog
            return ""; // Placeholder - replace with proper input dialog
        }
    }

    public class UserService : IUserService
    {
        public LoginResponse? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                await Task.Delay(1000); // Simulate authentication

                // Demo authentication
                var isValid = (request.Username, request.Password) switch
                {
                    ("admin", "admin123") => true,
                    ("manager", "manager123") => true,
                    ("user", "user123") => true,
                    _ => false
                };

                if (isValid)
                {
                    var role = request.Username switch
                    {
                        "admin" => "Quản trị viên",
                        "manager" => "Quản lý",
                        "user" => "Nhân viên",
                        _ => "User"
                    };

                    CurrentUser = new LoginResponse
                    {
                        Id = 1,
                        FullName = $"{role} - {request.Username}",
                        Role = role,
                        Token = Guid.NewGuid().ToString()
                    };

                    return ServiceResult<LoginResponse>.Success(CurrentUser);
                }

                return ServiceResult<LoginResponse>.Failure("Tên đăng nhập hoặc mật khẩu không đúng");
            }
            catch (Exception ex)
            {
                return ServiceResult<LoginResponse>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> LogoutAsync()
        {
            await Task.Delay(100);
            CurrentUser = null;
            return ServiceResult<bool>.Success(true);
        }
    }

    public class AttendanceService : IAttendanceService
    {
        public async Task<ServiceResult<List<object>>> GetAttendanceRecordsAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            await Task.Delay(500);
            var records = new List<object>();
            return ServiceResult<List<object>>.Success(records);
        }

        public async Task<ServiceResult<bool>> ExportAttendanceAsync(List<object> records, string filePath)
        {
            await Task.Delay(1000);
            return ServiceResult<bool>.Success(true);
        }
    }
}