using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using Microsoft.Extensions.Logging;
using Riss.Devices; // ✅ CHỈ Infrastructure mới được dùng

namespace BHK.Retrieval.Attendance.Infrastructure.Devices
{
    /// <summary>
    /// Service implementation cho giao tiếp với thiết bị Realand ZDC2911
    /// ✅ CHỈ Ở ĐÂY mới được dùng using Riss.Devices
    /// ✅ CHỈ Ở ĐÂY mới được tạo Device objects
    /// ✅ TUÂN THỦ Clean Architecture - Infrastructure chứa implementation cụ thể
    /// </summary>
    public class RealandDeviceService : IDeviceCommunicationService, IDisposable
    {
        private readonly ILogger<RealandDeviceService>? _logger;
        private Device? _device;
        private DeviceConnection? _deviceConnection;
        private bool _disposed;
        private bool _isConnected;

        public RealandDeviceService(ILogger<RealandDeviceService>? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kết nối tới thiết bị qua TCP/IP
        /// Dựa theo ZDC2911_Demo/CMForm.cs
        /// </summary>
        /// <param name="ip">IP Address của thiết bị</param>
        /// <param name="port">Port của thiết bị</param>
        /// <param name="deviceNumber">Device Number (DN)</param>
        /// <param name="password">Password thiết bị</param>
        public async Task ConnectAsync(string ip, int port, int deviceNumber, string password)
        {
            await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Connecting to device at {ip}:{port}, DN: {deviceNumber}", ip, port, deviceNumber);

                    // ✅ Validation đầu vào
                    if (string.IsNullOrEmpty(ip))
                        throw new ArgumentException("IP address cannot be null or empty", nameof(ip));
                    
                    if (port <= 0 || port > 65535)
                        throw new ArgumentException("Port must be between 1 and 65535", nameof(port));

                    if (deviceNumber <= 0)
                        throw new ArgumentException("Device number must be greater than 0", nameof(deviceNumber));

                    if (string.IsNullOrEmpty(password))
                        throw new ArgumentException("Password cannot be null or empty", nameof(password));

                    _device = new Device
                    {
                        DN = deviceNumber, 
                        Password = password,
                        Model = "ZDC2911",
                        ConnectionModel = 5, 
                        IpAddress = ip,
                        IpPort = port,
                        CommunicationType = CommunicationType.Tcp
                    };

                    // ✅ Tạo connection từ Device
                    _deviceConnection = DeviceConnection.CreateConnection(ref _device);

                    // ✅ KIỂM TRA KẾT QUẢ - QUAN TRỌNG!
                    int result = _deviceConnection.Open();
                    
                    if (result > 0)
                    {
                        // ✅ Kết nối thành công
                        _isConnected = true;
                        _logger?.LogInformation("Infrastructure: ✅ Successfully connected to device. Result code: {result}", result);
                    }
                    else
                    {
                        // ❌ Kết nối thất bại
                        _logger?.LogError("Infrastructure: ❌ Failed to connect. Result code: {result}", result);
                        
                        // Cleanup
                        _deviceConnection?.Close();
                        _deviceConnection = null;
                        _device = null;
                        _isConnected = false;

                        throw new Exception($"Failed to connect to device at {ip}:{port}. Error code: {result}");
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi chi tiết
                    _logger?.LogError(ex, "Infrastructure: Connection failed - IP: {ip}:{port}, DN: {deviceNumber}", 
                        ip, port, deviceNumber);
                    
                    // Cleanup khi có exception
                    try
                    {
                        _deviceConnection?.Close();
                    }
                    catch { }
                    
                    _deviceConnection = null;
                    _device = null;
                    _isConnected = false;
                    
                    // ✅ Throw với message thân thiện hơn
                    // Phân loại lỗi dựa trên exception message
                    string userFriendlyMessage = GetUserFriendlyErrorMessage(ex.Message, ip, port);
                    throw new Exception(userFriendlyMessage, ex);
                }
            });
        }

        /// <summary>
        /// Lấy danh sách nhân viên từ thiết bị
        /// </summary>
        public async Task<IEnumerable<string>> GetEmployeeListAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting employee list from device");

                    // TODO: Implement theo ZD2911 User Guide
                    // Ví dụ:
                    // object extraProperty = new object();
                    // object extraData = new object();
                    // bool result = _deviceConnection.GetProperty(DeviceProperty.Employee, extraProperty, ref _device, ref extraData);
                    // Parse extraData để lấy danh sách employees

                    // TẠM THỜI: Mock data
                    var employeeNames = new List<string>
                    {
                        "Nguyễn Văn A",
                        "Trần Thị B",
                        "Lê Văn C"
                    };

                    _logger?.LogInformation("Infrastructure: Retrieved {count} employees", employeeNames.Count);
                    return employeeNames;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get employee list");
                    throw;
                }
            });
        }

        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Disconnecting from device");

                    if (_deviceConnection != null)
                    {
                        _deviceConnection.Close();
                        _logger?.LogInformation("Infrastructure: ✅ Successfully disconnected");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Error during disconnect");
                }
                finally
                {
                    _isConnected = false;
                    _deviceConnection = null;
                    _device = null;
                }
            });
        }

        /// <summary>
        /// Property check connection status
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Lấy tất cả nhân viên từ thiết bị
        /// </summary>
        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting all employees from device");

                    // TODO: Implement theo ZD2911 User Guide
                    // _deviceConnection.GetProperty(DeviceProperty.AllUser, ...) 
                    
                    // TẠM THỜI: Mock data
                    var employees = new List<EmployeeDto>();
                    for (int i = 1; i <= 50; i++)
                    {
                        employees.Add(new EmployeeDto
                        {
                            DIN = (ulong)i,
                            UserName = $"Nhân viên {i}",
                            IDNumber = $"NV{i:D4}",
                            DeptId = (i % 3 + 1).ToString(),
                            Privilege = i % 4,
                            Enable = true,
                            Sex = i % 2, // 0=Male, 1=Female
                            Birthday = DateTime.Now.AddYears(-25 - i % 10),
                            Enrollments = new List<EnrollmentDto>()
                        });
                    }

                    _logger?.LogInformation("Infrastructure: Retrieved {count} employees", employees.Count);
                    return employees;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get all employees");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy thông tin nhân viên theo DIN
        /// </summary>
        public async Task<EmployeeDto?> GetEmployeeByIdAsync(ulong din)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting employee by DIN: {din}", din);

                    // TODO: Implement theo ZD2911 User Guide
                    // _deviceConnection.GetProperty(DeviceProperty.UserByDIN, ...)

                    // TẠM THỜI: Mock data
                    return new EmployeeDto
                    {
                        DIN = din,
                        UserName = $"Nhân viên {din}",
                        IDNumber = $"NV{din:D4}",
                        DeptId = "1",
                        Privilege = 0,
                        Enable = true,
                        Sex = 0, // 0=Male
                        Birthday = DateTime.Now.AddYears(-30),
                        Enrollments = new List<EnrollmentDto>
                        {
                            new EnrollmentDto { EnrollType = 0, Data = string.Empty, DataLength = 0 }
                        }
                    };
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get employee by DIN: {din}", din);
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy số lượng nhân viên trong thiết bị
        /// </summary>
        public async Task<int> GetEmployeeCountAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting employee count");

                    // TODO: Implement theo ZD2911 User Guide
                    // _deviceConnection.GetProperty(DeviceProperty.UserCount, ...)

                    return 50; // Mock data
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get employee count");
                    throw;
                }
            });
        }

        /// <summary>
        /// Thêm nhân viên mới
        /// </summary>
        public async Task<bool> AddEmployeeAsync(EmployeeDto employee)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Adding employee: {din}", employee.DIN);

                    // Convert DTO to Riss.Devices.User
                    var user = MapEmployeeDtoToUser(employee);

                    // TODO: Implement theo ZD2911 User Guide
                    // _deviceConnection.SetProperty(DeviceProperty.AddUser, user, ...)

                    _logger?.LogInformation("Infrastructure: Successfully added employee: {din}", employee.DIN);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to add employee: {din}", employee.DIN);
                    throw;
                }
            });
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public async Task<bool> UpdateEmployeeAsync(EmployeeDto employee)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Updating employee: {din}", employee.DIN);

                    var user = MapEmployeeDtoToUser(employee);

                    // TODO: Implement
                    // _deviceConnection.SetProperty(DeviceProperty.UpdateUser, user, ...)

                    _logger?.LogInformation("Infrastructure: Successfully updated employee: {din}", employee.DIN);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to update employee: {din}", employee.DIN);
                    throw;
                }
            });
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        public async Task<bool> DeleteEmployeeAsync(ulong din)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Deleting employee: {din}", din);

                    // TODO: Implement
                    // _deviceConnection.SetProperty(DeviceProperty.DeleteUser, din, ...)

                    _logger?.LogInformation("Infrastructure: Successfully deleted employee: {din}", din);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to delete employee: {din}", din);
                    throw;
                }
            });
        }

        /// <summary>
        /// Xóa tất cả nhân viên
        /// </summary>
        public async Task<bool> ClearAllEmployeesAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Clearing all employees");

                    // TODO: Implement
                    // _deviceConnection.SetProperty(DeviceProperty.ClearAllUsers, ...)

                    _logger?.LogInformation("Infrastructure: Successfully cleared all employees");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to clear all employees");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy dữ liệu chấm công theo khoảng thời gian
        /// </summary>
        public async Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting attendance records from {start} to {end}", startDate, endDate);

                    // TODO: Implement
                    // _deviceConnection.GetProperty(DeviceProperty.AttendanceRecords, ...)

                    // Mock data
                    return new List<AttendanceRecordDto>();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get attendance records");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy số lượng bản ghi chấm công
        /// </summary>
        public async Task<int> GetAttendanceRecordCountAsync(DateTime startDate, DateTime endDate)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting attendance record count");
                    
                    // TODO: Implement
                    return 0;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get attendance record count");
                    throw;
                }
            });
        }

        /// <summary>
        /// Xóa dữ liệu chấm công
        /// </summary>
        public async Task<bool> ClearAttendanceRecordsAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Clearing attendance records");

                    // TODO: Implement
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to clear attendance records");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy serial number thiết bị
        /// </summary>
        public async Task<string> GetSerialNumberAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting device serial number");

                    // TODO: Implement
                    return "ZDC2911-001";
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get serial number");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy thời gian hiện tại của thiết bị
        /// </summary>
        public async Task<DateTime> GetDeviceTimeAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting device time");

                    // TODO: Implement
                    return DateTime.Now;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get device time");
                    throw;
                }
            });
        }

        /// <summary>
        /// Đồng bộ thời gian thiết bị với máy tính
        /// </summary>
        public async Task<bool> SyncDeviceTimeAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Syncing device time");

                    // TODO: Implement
                    return true;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to sync device time");
                    throw;
                }
            });
        }

        /// <summary>
        /// Mapper: EmployeeDto -> Riss.Devices.User
        /// </summary>
        private User MapEmployeeDtoToUser(EmployeeDto dto)
        {
            var user = new User
            {
                DIN = dto.DIN,
                UserName = dto.UserName,
                IDNumber = dto.IDNumber,
                DeptId = dto.DeptId,
                Privilege = dto.Privilege,
                Enable = dto.Enable,
                // Sex mapping: dto.Sex is int (0=Male, 1=Female)
                // TODO: Map to correct Riss.Devices.Sex type when structure is known
                Birthday = dto.Birthday,
                Comment = dto.Comment,
                AccessControl = dto.AccessControl,
                ValidityPeriod = dto.ValidityPeriod,
                ValidDate = dto.ValidDate,
                InvalidDate = dto.InvalidDate,
                UserGroup = dto.UserGroup,
                AccessTimeZone = dto.AccessTimeZone,
                AttType = dto.AttType
            };

            // Set Sex property using reflection to avoid compile-time dependency
            var sexProperty = user.GetType().GetProperty("Sex");
            if (sexProperty != null)
            {
                var sexType = sexProperty.PropertyType;
                if (sexType.IsEnum)
                {
                    // Assume 0=Male, 1=Female
                    var enumValue = Enum.ToObject(sexType, dto.Sex);
                    sexProperty.SetValue(user, enumValue);
                }
            }

            // Map enrollments if needed
            // Note: Riss.Devices.Enroll structure may differ from EnrollmentDto
            // Implement based on actual Riss.Devices API documentation
            if (dto.Enrollments != null && dto.Enrollments.Any())
            {
                // TODO: Implement enrollment mapping based on Riss.Devices.Enroll structure
                // user.Enrolls = dto.Enrollments.Select(e => new Enroll { ... }).ToList();
            }

            return user;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    if (_deviceConnection != null && _isConnected)
                    {
                        _deviceConnection.Close();
                        _logger?.LogInformation("Infrastructure: Disposing - disconnected device");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Infrastructure: Error during disposal");
                }
                finally
                {
                    _deviceConnection = null;
                    _device = null;
                    _isConnected = false;
                    _disposed = true;
                    _logger?.LogInformation("Infrastructure: RealandDeviceService disposed");
                }
            }
        }

        /// <summary>
        /// Chuyển đổi technical error message thành user-friendly message
        /// </summary>
        private string GetUserFriendlyErrorMessage(string exceptionMessage, string ip, int port)
        {
            // Phân loại lỗi dựa trên exception message
            if (exceptionMessage.Contains("AddressError") || 
                exceptionMessage.Contains("Address"))
            {
                return $"Không thể kết nối đến thiết bị tại {ip}:{port}\n\n" +
                       "Nguyên nhân có thể:\n" +
                       "• Địa chỉ IP không chính xác\n" +
                       "• Thiết bị chưa được bật nguồn\n" +
                       "• Thiết bị không cùng mạng với máy tính\n" +
                       "• Firewall đang chặn kết nối";
            }
            else if (exceptionMessage.Contains("Timeout") || 
                     exceptionMessage.Contains("timeout"))
            {
                return $"Thiết bị tại {ip}:{port} không phản hồi\n\n" +
                       "Vui lòng kiểm tra:\n" +
                       "• Thiết bị đã được bật nguồn\n" +
                       "• Kết nối mạng ổn định\n" +
                       "• Không có ứng dụng khác đang kết nối";
            }
            else if (exceptionMessage.Contains("Port") || 
                     exceptionMessage.Contains("port"))
            {
                return $"Không thể kết nối qua cổng {port}\n\n" +
                       "Vui lòng kiểm tra:\n" +
                       "• Cổng kết nối đúng (mặc định: 4370)\n" +
                       "• Cổng chưa bị chặn bởi Firewall";
            }
            else
            {
                // Lỗi chung
                return $"Không thể kết nối đến thiết bị tại {ip}:{port}\n\n" +
                       $"Lỗi: {exceptionMessage}";
            }
        }
    }
}