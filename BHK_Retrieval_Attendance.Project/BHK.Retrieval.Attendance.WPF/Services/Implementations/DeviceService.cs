using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service xử lý giao tiếp với thiết bị chấm công ZDC2911
    /// Tuân thủ Clean Architecture - chỉ gọi Infrastructure qua Interface
    /// </summary>
    public class DeviceService : IDeviceService, IDisposable
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly DeviceOptions _deviceOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDeviceCommunicationService _deviceCommunicationService;
        
        private bool _isConnected;
        private bool _disposed;
        
        // DTO nội bộ thay vì dùng class Device từ Riss.Devices
        private DeviceConnectionInfo? _deviceInfo;

        public DeviceService(
            ILogger<DeviceService> logger,
            IOptions<DeviceOptions> deviceOptions,
            IServiceProvider serviceProvider,
            IDeviceCommunicationService deviceCommunicationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deviceOptions = deviceOptions?.Value ?? throw new ArgumentNullException(nameof(deviceOptions));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _deviceCommunicationService = deviceCommunicationService ?? throw new ArgumentNullException(nameof(deviceCommunicationService));
            _isConnected = false;
        }

        /// <summary>
        /// Kiểm tra trạng thái kết nối
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Lấy thông tin Device hiện tại (chỉ trả về DTO, không expose Riss.Device)
        /// </summary>
        public object? CurrentDevice => _deviceInfo;

        /// <summary>
        /// Kết nối tới thiết bị qua TCP/IP
        /// </summary>
        public async Task<bool> ConnectTcpAsync(string ipAddress, int port, int deviceNumber, string password)
        {
            try
            {
                _logger.LogInformation("Starting TCP connection - IP: {IpAddress}, Port: {Port}, DN: {DeviceNumber}", 
                    ipAddress, port, deviceNumber);

                // ===== TEST MODE =====
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE ENABLED - Simulating successful connection");
                    await Task.Delay(1000); // Simulate connection delay
                    
                    // Tạo DTO giả để test
                    _deviceInfo = new DeviceConnectionInfo
                    {
                        DeviceNumber = deviceNumber,
                        IpAddress = ipAddress,
                        Port = port,
                        Password = password,
                        Model = _deviceOptions.DeviceModel,
                        SerialNumber = "TEST-SN-12345678"
                    };
                    
                    _isConnected = true;
                    _logger.LogInformation("✅ [TEST MODE] Device connected successfully (simulated)");
                    return true;
                }

                // ===== PRODUCTION MODE - Gọi Infrastructure Service =====
                _logger.LogInformation("Connecting to device using Infrastructure Service...");

                // Gọi Infrastructure Service qua Interface (tuân thủ Clean Architecture)
                // ✅ Truyền đầy đủ parameters: ip, port, deviceNumber, password
                await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);
                

                _isConnected = true;
                _logger.LogInformation("✅ Device connected successfully via Infrastructure");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to device via TCP/IP");
                _isConnected = false;
                CleanupConnection();
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối với thiết bị
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                _logger.LogInformation("Disconnecting from device...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE ENABLED - Simulating disconnection");
                    await Task.Delay(500);
                    _isConnected = false;
                    CleanupConnection();
                    _logger.LogInformation("✅ [TEST MODE] Device disconnected successfully (simulated)");
                    return;
                }

                // PRODUCTION MODE - Gọi Infrastructure Service
                await _deviceCommunicationService.DisconnectAsync();

                CleanupConnection();
                _isConnected = false;
                _logger.LogInformation("Device disconnected successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disconnecting from device");
                throw;
            }
        }

        /// <summary>
        /// Test kết nối tới thiết bị (không lưu connection)
        /// </summary>
        public async Task<bool> TestConnectionAsync(string ipAddress, int port, int deviceNumber, string password)
        {
            try
            {
                _logger.LogInformation("Testing connection to device...");

                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Simulating test connection");
                    await Task.Delay(800);
                    _logger.LogInformation("✅ [TEST MODE] Connection test successful (simulated)");
                    return true;
                }

                // PRODUCTION MODE - Test connection qua Infrastructure
                // Note: Cần implement TestConnectionAsync trong IDeviceCommunicationService
                _logger.LogInformation("Testing connection via Infrastructure Service...");
                
                // Tạm thời dùng ConnectAsync rồi DisconnectAsync
                // ✅ Truyền đầy đủ parameters: ip, port, deviceNumber, password
                await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);
                await _deviceCommunicationService.DisconnectAsync();
                
                _logger.LogInformation("✅ Connection test successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed");
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin thiết bị
        /// </summary>
        public async Task<string> GetDeviceInfoAsync()
        {
            try
            {
                if (!_isConnected || _deviceInfo == null)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting device information...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Returning simulated device info");
                    
                    var testInfo = $"📱 DEVICE INFO (TEST MODE)\n" +
                                  $"Model: {_deviceInfo.Model}\n" +
                                  $"Serial: {_deviceInfo.SerialNumber}\n" +
                                  $"IP: {_deviceInfo.IpAddress}:{_deviceInfo.Port}\n" +
                                  $"Device Number: {_deviceInfo.DeviceNumber}\n" +
                                  $"Status: Connected ✅";

                    await Task.Delay(300); // Simulate data retrieval
                    return testInfo;
                }

                // PRODUCTION MODE - Lấy thông tin từ Infrastructure
                var deviceInfo = $"📱 DEVICE INFO\n" +
                                $"Model: {_deviceInfo.Model}\n" +
                                $"Serial: {_deviceInfo.SerialNumber}\n" +
                                $"IP: {_deviceInfo.IpAddress}:{_deviceInfo.Port}\n" +
                                $"Device Number: {_deviceInfo.DeviceNumber}\n" +
                                $"Status: Connected ✅";

                return await Task.FromResult(deviceInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get device information");
                throw;
            }
        }

        #region Employee Management Implementation

        /// <summary>
        /// Lấy tất cả nhân viên từ thiết bị dưới dạng EmployeeDto
        /// </summary>
        public async Task<List<EmployeeDto>> GetAllUsersAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting all users from device...");

                // Gọi Infrastructure Service
                var employees = await _deviceCommunicationService.GetAllEmployeesAsync();
                
                _logger.LogInformation("Retrieved {count} employees from device", employees.Count);
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users");
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin nhân viên theo DIN
        /// </summary>
        public async Task<EmployeeDto?> GetUserByIdAsync(ulong din)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting user by DIN: {din}", din);
                var employee = await _deviceCommunicationService.GetEmployeeByIdAsync(din);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user by DIN: {din}", din);
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng nhân viên
        /// </summary>
        public async Task<int> GetUserCountAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting user count");
                var count = await _deviceCommunicationService.GetEmployeeCountAsync();
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user count");
                throw;
            }
        }

        /// <summary>
        /// Thêm nhân viên mới
        /// </summary>
        public async Task<bool> AddUserAsync(EmployeeDto user)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Adding user: {din}", user.DIN);
                var result = await _deviceCommunicationService.AddEmployeeAsync(user);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user: {din}", user.DIN);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public async Task<bool> UpdateUserAsync(EmployeeDto user)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Updating user: {din}", user.DIN);
                var result = await _deviceCommunicationService.UpdateEmployeeAsync(user);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user: {din}", user.DIN);
                throw;
            }
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        public async Task<bool> DeleteUserAsync(ulong din)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Deleting user: {din}", din);
                var result = await _deviceCommunicationService.DeleteEmployeeAsync(din);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user: {din}", din);
                throw;
            }
        }

        /// <summary>
        /// Xóa tất cả nhân viên
        /// </summary>
        public async Task<bool> ClearAllUsersAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Clearing all users");
                var result = await _deviceCommunicationService.ClearAllEmployeesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all users");
                throw;
            }
        }

        #endregion

        #region Enrollment Management Implementation

        public async Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(ulong din)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting enrollments for user: {din}", din);
                var employee = await _deviceCommunicationService.GetEmployeeByIdAsync(din);
                return employee?.Enrollments?.ToList() ?? new List<EnrollmentDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get enrollments for user: {din}", din);
                throw;
            }
        }

        public async Task<bool> EnrollFingerprintAsync(ulong din, int fingerprintIndex, byte[] fingerprintData)
        {
            // TODO: Implement fingerprint enrollment
            _logger.LogWarning("EnrollFingerprintAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> EnrollPasswordAsync(ulong din, string password)
        {
            // TODO: Implement password enrollment
            _logger.LogWarning("EnrollPasswordAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> EnrollCardAsync(ulong din, string cardId)
        {
            // TODO: Implement card enrollment
            _logger.LogWarning("EnrollCardAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> ClearFingerprintAsync(ulong din, int fingerprintIndex)
        {
            // TODO: Implement fingerprint clearing
            _logger.LogWarning("ClearFingerprintAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> ClearPasswordAsync(ulong din)
        {
            // TODO: Implement password clearing
            _logger.LogWarning("ClearPasswordAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> ClearCardAsync(ulong din)
        {
            // TODO: Implement card clearing
            _logger.LogWarning("ClearCardAsync not yet implemented");
            await Task.CompletedTask;
            return false;
        }

        #endregion

        #region Attendance Records Implementation

        public async Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting attendance records from {start} to {end}", startDate, endDate);
                var records = await _deviceCommunicationService.GetAttendanceRecordsAsync(startDate, endDate);
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attendance records");
                throw;
            }
        }

        public async Task<int> GetAttendanceRecordCountAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting attendance record count");
                var count = await _deviceCommunicationService.GetAttendanceRecordCountAsync(startDate, endDate);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get attendance record count");
                throw;
            }
        }

        public async Task<bool> ClearAttendanceRecordsAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Clearing attendance records");
                var result = await _deviceCommunicationService.ClearAttendanceRecordsAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear attendance records");
                throw;
            }
        }

        #endregion

        #region Device Information Implementation

        public async Task<string> GetSerialNumberAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                return await _deviceCommunicationService.GetSerialNumberAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get serial number");
                throw;
            }
        }

        public async Task<string> GetDeviceModelAsync()
        {
            return await Task.FromResult(_deviceInfo?.Model ?? "Unknown");
        }

        public async Task<DateTime> GetDeviceTimeAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                return await _deviceCommunicationService.GetDeviceTimeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get device time");
                throw;
            }
        }

        public async Task<bool> SyncDeviceTimeAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                return await _deviceCommunicationService.SyncDeviceTimeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync device time");
                throw;
            }
        }

        #endregion

        #region Additional Methods

        /// <summary>
        /// ConnectAsync implementation with different parameter order
        /// </summary>
        public async Task<bool> ConnectAsync(int deviceNumber, string ipAddress, int port, string password)
        {
            return await ConnectTcpAsync(ipAddress, port, deviceNumber, password);
        }

        /// <summary>
        /// TestConnectionAsync without parameters - uses last known connection info
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            if (_deviceInfo == null)
            {
                throw new InvalidOperationException("No device connection info available");
            }

            return await TestConnectionAsync(_deviceInfo.IpAddress, _deviceInfo.Port, _deviceInfo.DeviceNumber, _deviceInfo.Password);
        }

        /// <summary>
        /// Lấy danh sách nhân viên dạng text (legacy method for backward compatibility)
        /// </summary>
        public async Task<IEnumerable<string>> GetEmployeeListAsync()
        {
            try
            {
                if (!_isConnected)
                {
                    throw new InvalidOperationException("Device is not connected");
                }

                _logger.LogInformation("Getting employee list from device...");

                // TEST MODE
                if (_deviceOptions.Test)
                {
                    _logger.LogWarning("⚠️ TEST MODE - Returning simulated employee list");
                    await Task.Delay(1000); // Simulate data retrieval
                    
                    return new List<string>
                    {
                        "Nguyễn Văn A (ID: 001)",
                        "Trần Thị B (ID: 002)",
                        "Lê Văn C (ID: 003)",
                        "Phạm Thị D (ID: 004)",
                        "Hoàng Văn E (ID: 005)"
                    };
                }

                // PRODUCTION MODE - Lấy từ Infrastructure Service
                var employees = await _deviceCommunicationService.GetAllEmployeesAsync();
                return employees.Select(e => $"{e.UserName} (ID: {e.IDNumber})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee list");
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Clean up connection resources
        /// </summary>
        private void CleanupConnection()
        {
            _deviceInfo = null;
            // Không cần cleanup Riss.Device objects vì không còn dùng nữa
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_isConnected)
                {
                    // Async dispose pattern - log warning nếu chưa disconnect
                    _logger.LogWarning("DeviceService disposed while still connected. Call DisconnectAsync() first.");
                }

                CleanupConnection();
                _disposed = true;
                
                _logger.LogInformation("DeviceService disposed successfully");
            }
        }

        /// <summary>
        /// DTO nội bộ để lưu thông tin kết nối thiết bị
        /// Thay thế cho việc dùng class Device từ Riss.Devices (vi phạm Clean Architecture)
        /// </summary>
        private class DeviceConnectionInfo
        {
            public int DeviceNumber { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public int Port { get; set; }
            public string Password { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string SerialNumber { get; set; } = string.Empty;
        }
    }
}
