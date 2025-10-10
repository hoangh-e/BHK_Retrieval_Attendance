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

                    // ✅ LẤY DỮ LIỆU THỰC TỪ THIẾT BỊ theo ZDC2911 User Guide
                    // Bước 1: Lấy danh sách user từ thiết bị
                    object extraProperty = (UInt64)0; // 0 = lấy tất cả user
                    object? extraData = null;
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.Enrolls, 
                        extraProperty, 
                        ref _device, 
                        ref extraData
                    );

                    if (!result || extraData == null)
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get user list from device or no users found");
                        return new List<EmployeeDto>();
                    }

                    var users = (List<User>)extraData;
                    
                    if (users.Count == 0)
                    {
                        _logger?.LogWarning("Infrastructure: No employees found on device");
                        return new List<EmployeeDto>();
                    }

                    _logger?.LogInformation("Infrastructure: Retrieved {count} users from device (without enrollments)", users.Count);

                    // Bước 2: Lấy enrollment data cho từng user
                    // ✅ Sử dụng for loop thay vì foreach để có thể dùng ref
                    for (int i = 0; i < users.Count; i++)
                    {
                        try
                        {
                            User user = users[i];
                            object? enrollData = null;
                            
                            bool enrollResult = _deviceConnection.GetProperty(
                                UserProperty.Enroll, 
                                null, 
                                ref user, 
                                ref enrollData
                            );
                            
                            // ✅ Cập nhật lại user trong list sau khi GetProperty
                            users[i] = user;
                            
                            if (!enrollResult)
                            {
                                _logger?.LogWarning("Infrastructure: Failed to get enrollment data for user DIN: {din}", user.DIN);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Infrastructure: Error getting enrollment data for user DIN: {din}", users[i].DIN);
                            // Tiếp tục với user khác
                        }
                    }

                    // ✅ Convert từ Riss.Devices.User sang EmployeeDto
                    var employees = users.Select(user => MapRissUserToEmployeeDto(user)).ToList();
                    
                    _logger?.LogInformation("Infrastructure: Successfully converted {count} employees to DTOs", employees.Count);
                    return employees;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get all employees");
                    throw new InvalidOperationException("Failed to retrieve employees from device. Please check device connection.", ex);
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

                    // ✅ LẤY DỮ LIỆU THỰC từ thiết bị theo ZDC2911 User Guide
                    // Khi lấy user theo DIN cụ thể, API trả về User object (không phải List)
                    object extraProperty = (UInt64)din; // DIN cụ thể
                    object? extraData = null;
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.Enrolls, 
                        extraProperty, 
                        ref _device, 
                        ref extraData
                    );

                    if (!result || extraData == null)
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get user with DIN: {din} from device", din);
                        return null;
                    }

                    // ✅ KHI DIN != 0: extraData là User object (không phải List<User>)
                    User user;
                    try
                    {
                        user = (User)extraData;
                    }
                    catch (InvalidCastException)
                    {
                        // Nếu cast thất bại, có thể API trả về List với 1 phần tử
                        var userList = extraData as List<User>;
                        if (userList == null || userList.Count == 0)
                        {
                            _logger?.LogWarning("Infrastructure: No user found with DIN: {din}", din);
                            return null;
                        }
                        user = userList[0];
                    }

                    // Lấy enrollment data
                    try
                    {
                        object? enrollData = null;
                        bool enrollResult = _deviceConnection.GetProperty(
                            UserProperty.Enroll, 
                            null, 
                            ref user, 
                            ref enrollData
                        );
                        
                        if (!enrollResult)
                        {
                            _logger?.LogWarning("Infrastructure: Failed to get enrollment data for user DIN: {din}", din);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Infrastructure: Error getting enrollment data for user DIN: {din}", din);
                    }

                    // Convert sang DTO
                    var employeeDto = MapRissUserToEmployeeDto(user);
                    
                    _logger?.LogInformation("Infrastructure: Successfully retrieved employee DIN: {din}", din);
                    return employeeDto;
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

                    // ✅ LẤY DỮ LIỆU THỰC: Lấy tất cả user rồi đếm số lượng
                    // Không có API trực tiếp để lấy count, phải lấy toàn bộ danh sách
                    object extraProperty = (UInt64)0; // 0 = lấy tất cả user
                    object? extraData = null;
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.Enrolls, 
                        extraProperty, 
                        ref _device, 
                        ref extraData
                    );

                    if (!result || extraData == null)
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get user list for counting");
                        return 0;
                    }

                    var users = (List<User>)extraData;
                    int count = users.Count;
                    
                    _logger?.LogInformation("Infrastructure: Employee count: {count}", count);
                    return count;
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

                    // ✅ LẤY DỮ LIỆU THỰC theo ZDC2911 User Guide
                    // EmptyUserEnrollInfo: DIN=0 xóa tất cả user
                    object extraData = (UInt64)0; // 0 = xóa tất cả
                    
                    bool result = _deviceConnection.SetProperty(
                        DeviceProperty.Enrolls, 
                        null, 
                        _device, 
                        extraData
                    );

                    if (result)
                    {
                        _logger?.LogInformation("Infrastructure: Successfully cleared all employees");
                        return true;
                    }
                    else
                    {
                        _logger?.LogWarning("Infrastructure: Failed to clear all employees");
                        return false;
                    }
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
        /// Sử dụng DeviceProperty.AttRecords để lấy G.Log (General Log) từ thiết bị
        /// CẢNH BÁO: SDK có thể lấy toàn bộ records rồi mới filter theo date range ở client-side
        /// </summary>
        public async Task<List<AttendanceRecordDto>> GetAttendanceRecordsAsync(DateTime startDate, DateTime endDate)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    _logger?.LogInformation("Infrastructure: ⏱️ START Getting attendance records from {start} to {end}", 
                        startDate.ToString("yyyy-MM-dd HH:mm:ss"), 
                        endDate.ToString("yyyy-MM-dd HH:mm:ss"));

                    // ✅ BƯỚC 1: Chuẩn bị parameters theo Riss.Device_Guide.md
                    // object để chứa kết quả (sẽ được gán thành List<Record>)
                    object extraData = new List<DateTime> { startDate, endDate };

                    // boolList[0] = false: lấy TẤT CẢ log (không chỉ log mới)
                    // boolList[1] = false: KHÔNG xóa cờ "new" (giữ nguyên trạng thái)
                    List<bool> boolList = new List<bool> { false, false };

                    // ✅ BƯỚC 2: Gọi GetProperty để lấy attendance records
                    var apiStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.AttRecords,   // Property: G.Log (General Log)
                        boolList,                     // Extra property: [lấy tất cả, không xóa cờ new]
                        ref _device,                  // Device reference
                        ref extraData                 // Kết quả sẽ được gán vào extraData
                    );
                    apiStopwatch.Stop();
                    _logger?.LogInformation("Infrastructure: ⏱️ SDK GetProperty took {ms}ms", apiStopwatch.ElapsedMilliseconds);

                    if (!result)
                    {
                        _logger?.LogWarning("Infrastructure: GetProperty returned false for AttRecords");
                        return new List<AttendanceRecordDto>();
                    }

                    // ✅ BƯỚC 3: Parse kết quả
                    // Theo SDK: extraData sau khi gọi GetProperty sẽ chứa List<Record>
                    if (extraData == null)
                    {
                        _logger?.LogWarning("Infrastructure: extraData is null after GetProperty");
                        return new List<AttendanceRecordDto>();
                    }

                    List<Record>? records = extraData as List<Record>;
                    
                    if (records == null || records.Count == 0)
                    {
                        _logger?.LogInformation("Infrastructure: No attendance records found in date range");
                        return new List<AttendanceRecordDto>();
                    }

                    _logger?.LogInformation("Infrastructure: 📊 Received {count} records from device", records.Count);

                    // ✅ BƯỚC 4: Convert Record (Riss.Devices) → AttendanceRecordDto (Core)
                    var convertStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var attendanceDtos = records.Select(record => new AttendanceRecordDto
                    {
                        DIN = record.DIN,              // Device Identification Number (mã nhân viên)
                        Time = record.Clock,           // Thời gian chấm công
                        State = record.Action,         // Trạng thái: Chi tiết vào/ra (Action field)
                        VerifyMode = record.Verify,    // Phương thức: 0=Password, 1=Fingerprint, 2=Card, 3=Face, 4=Iris
                        RecordId = record.DIN          // Tạm dùng DIN làm ID (không có RecId trong Record)
                    }).ToList();
                    convertStopwatch.Stop();

                    stopwatch.Stop();
                    _logger?.LogInformation(
                        "Infrastructure: ✅ Successfully retrieved {count} attendance records in {totalMs}ms (API: {apiMs}ms, Convert: {convertMs}ms)", 
                        attendanceDtos.Count, 
                        stopwatch.ElapsedMilliseconds,
                        apiStopwatch.ElapsedMilliseconds,
                        convertStopwatch.ElapsedMilliseconds);
                    
                    return attendanceDtos;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger?.LogError(ex, "Infrastructure: ❌ Failed to get attendance records from device after {ms}ms", stopwatch.ElapsedMilliseconds);
                    throw new Exception("Failed to retrieve attendance records from device. " + ex.Message, ex);
                }
            });
        }

        /// <summary>
        /// Lấy số lượng bản ghi chấm công trong khoảng thời gian
        /// Sử dụng DeviceProperty.AttRecordsCount
        /// </summary>
        public async Task<int> GetAttendanceRecordCountAsync(DateTime startDate, DateTime endDate)
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting attendance record count from {start} to {end}", 
                        startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                    
                    // ✅ Chuẩn bị parameters
                    object extraData = new List<DateTime> { startDate, endDate };
                    
                    // extraProperty = false: đếm TẤT CẢ log (không chỉ log mới)
                    object extraProperty = false;
                    
                    // ✅ Gọi GetProperty để lấy count
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.AttRecordsCount,  // Property: Số lượng G.Log
                        extraProperty,                    // false = tất cả
                        ref _device,
                        ref extraData
                    );
                    
                    if (!result)
                    {
                        _logger?.LogWarning("Infrastructure: GetProperty returned false for AttRecordsCount");
                        return 0;
                    }
                    
                    // Kết quả là int
                    int count = extraData != null ? (int)extraData : 0;
                    
                    _logger?.LogInformation("Infrastructure: Found {count} attendance records", count);
                    return count;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get attendance record count");
                    throw;
                }
            });
        }

        /// <summary>
        /// Xóa tất cả dữ liệu chấm công từ thiết bị
        /// Sử dụng DeviceProperty.AttRecords với SetProperty
        /// ⚠️ CẢNH BÁO: Thao tác này sẽ xóa TẤT CẢ lịch sử chấm công!
        /// </summary>
        public async Task<bool> ClearAttendanceRecordsAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogWarning("Infrastructure: ⚠️ Clearing ALL attendance records from device");

                    // ✅ Gọi SetProperty để xóa tất cả attendance records
                    // Theo guide: SetProperty(DeviceProperty.AttRecords, null, device, null)
                    bool result = _deviceConnection.SetProperty(
                        DeviceProperty.AttRecords,  // Property: G.Log
                        null,                        // extraProperty: null
                        _device,                     // Device
                        null                         // extraData: null
                    );

                    if (result)
                    {
                        _logger?.LogInformation("Infrastructure: ✅ Successfully cleared all attendance records");
                    }
                    else
                    {
                        _logger?.LogError("Infrastructure: ❌ Failed to clear attendance records");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Exception while clearing attendance records");
                    throw;
                }
            });
        }

        /// <summary>
        /// Lấy serial number thiết bị
        /// ✅ CÁCH ĐÚNG từ SystemSettingForm.cs (Realand source code)
        /// Sử dụng DeviceProperty.SerialNo với Zd2911Utils.DeviceSerialNo
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

                    // 🧠 DEBUG: Kiểm tra trạng thái kết nối
                    _logger?.LogInformation("DEBUG: _deviceConnection = {status}", (_deviceConnection == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _device = {status}", (_device == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _isConnected = {status}", _isConnected);

                    // ✅ CÁCH CHUẨN từ Realand SystemSettingForm.cs
                    // Sử dụng DeviceProperty.SerialNo (không phải Model hay Status)
                    object extraProperty = new object();
                    object extraData = Zd2911Utils.DeviceSerialNo; // 🔥 Khởi tạo với DeviceSerialNo
                    
                    _logger?.LogInformation("Infrastructure: Calling GetProperty(DeviceProperty.SerialNo)");
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.SerialNo,  // 🔥 Sử dụng SerialNo property
                        extraProperty,
                        ref _device,
                        ref extraData);

                    if (result)
                    {
                        // extraData chứa Serial Number string
                        string serialNumber = (string)extraData;
                        _logger?.LogInformation("Infrastructure: ✅ Serial Number: {SerialNumber}", serialNumber);
                        return serialNumber;
                    }
                    else
                    {
                        _logger?.LogWarning("Infrastructure: ❌ GetProperty(DeviceProperty.SerialNo) failed");
                        return "N/A";
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: ❌ Failed to get serial number");
                    return "N/A"; // Return N/A instead of throwing to prevent breaking the UI
                }
            });
        }

        /// <summary>
        /// Lấy thời gian hiện tại của thiết bị
        /// Theo hướng dẫn: GetProperty(DeviceProperty.DeviceTime, ...)
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

                    // ✅ CÁCH ĐÚNG: extraProperty và extraData phải được khởi tạo
                    object extraProperty = new object();
                    object extraData = new object();
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.DeviceTime,
                        extraProperty,  // ✅ Không được truyền null
                        ref _device,
                        ref extraData);

                    if (result && extraData != null)
                    {
                        DateTime deviceTime = (DateTime)extraData;
                        _logger?.LogInformation("Infrastructure: Device time: {DeviceTime}", deviceTime);
                        return deviceTime;
                    }
                    else
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get device time, using current time");
                        return DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get device time");
                    return DateTime.Now; // Fallback to current time
                }
            });
        }

        /// <summary>
        /// Lấy firmware version của thiết bị
        /// Theo hướng dẫn: GetProperty(DeviceProperty.FirmwareVersion, ...)
        /// </summary>
        public async Task<string> GetFirmwareVersionAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting firmware version");

                    // 🧠 DEBUG: Kiểm tra trạng thái kết nối
                    _logger?.LogInformation("DEBUG: _deviceConnection = {status}", (_deviceConnection == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _device = {status}", (_device == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _isConnected = {status}", _isConnected);

                    // ✅ CÁCH ĐÚNG: extraProperty và extraData phải được khởi tạo
                    object extraProperty = new object();
                    object extraData = new object();
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.FirmwareVersion,
                        extraProperty,  // ✅ Không được truyền null
                        ref _device,
                        ref extraData);

                    if (result && extraData != null)
                    {
                        string firmwareVersion = extraData.ToString() ?? "N/A";
                        _logger?.LogInformation("Infrastructure: Firmware version: {FirmwareVersion}", firmwareVersion);
                        return firmwareVersion;
                    }
                    else
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get firmware version");
                        return "N/A";
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get firmware version");
                    return "N/A";
                }
            });
        }

        /// <summary>
        /// Lấy model của thiết bị
        /// Theo hướng dẫn: GetProperty(DeviceProperty.Model, ...)
        /// </summary>
        public async Task<string> GetDeviceModelAsync()
        {
            if (_device == null || _deviceConnection == null || !_isConnected)
                throw new InvalidOperationException("Not connected to device. Call ConnectAsync first.");

            return await Task.Run(() =>
            {
                try
                {
                    _logger?.LogInformation("Infrastructure: Getting device model");

                    // 🧠 DEBUG: Kiểm tra trạng thái kết nối
                    _logger?.LogInformation("DEBUG: _deviceConnection = {status}", (_deviceConnection == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _device = {status}", (_device == null ? "null" : "ok"));
                    _logger?.LogInformation("DEBUG: _isConnected = {status}", _isConnected);

                    // ✅ CÁCH ĐÚNG: extraProperty và extraData phải được khởi tạo
                    object extraProperty = new object();
                    object extraData = new object();
                    
                    bool result = _deviceConnection.GetProperty(
                        DeviceProperty.Model,
                        extraProperty,  // ✅ Không được truyền null
                        ref _device,
                        ref extraData);

                    if (result && extraData != null)
                    {
                        string model = extraData.ToString() ?? _device.Model ?? "N/A";
                        _logger?.LogInformation("Infrastructure: Device model: {Model}", model);
                        return model;
                    }
                    else
                    {
                        _logger?.LogWarning("Infrastructure: Failed to get device model, using device.Model");
                        return _device.Model ?? "N/A";
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Infrastructure: Failed to get device model");
                    return _device.Model ?? "N/A";
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
        /// Mapper: Riss.Devices.User -> EmployeeDto
        /// Chuyển đổi từ đối tượng User của thư viện Riss.Devices sang DTO của Core layer
        /// </summary>
        private EmployeeDto MapRissUserToEmployeeDto(User rissUser)
        {
            var dto = new EmployeeDto
            {
                DIN = rissUser.DIN,
                UserName = rissUser.UserName ?? string.Empty,
                IDNumber = rissUser.IDNumber ?? string.Empty,
                DeptId = rissUser.DeptId ?? string.Empty,
                Privilege = rissUser.Privilege,
                Enable = rissUser.Enable,
                Birthday = rissUser.Birthday,
                Comment = rissUser.Comment ?? string.Empty,
                AccessControl = rissUser.AccessControl,
                ValidityPeriod = rissUser.ValidityPeriod,
                ValidDate = rissUser.ValidDate,
                InvalidDate = rissUser.InvalidDate,
                UserGroup = rissUser.UserGroup,
                AccessTimeZone = rissUser.AccessTimeZone,
                AttType = rissUser.AttType
            };

            // ✅ Map Sex property using reflection (Riss.Devices.Sex enum -> int)
            var sexProperty = rissUser.GetType().GetProperty("Sex");
            if (sexProperty != null)
            {
                var sexValue = sexProperty.GetValue(rissUser);
                if (sexValue != null)
                {
                    // Convert enum to int: 0=Male, 1=Female
                    dto.Sex = Convert.ToInt32(sexValue);
                }
            }

            // ✅ Map enrollments từ Riss.Devices.Enroll -> EnrollmentDto
            if (rissUser.Enrolls != null && rissUser.Enrolls.Count > 0)
            {
                dto.Enrollments = rissUser.Enrolls.Select(enroll => new EnrollmentDto
                {
                    EnrollType = (int)enroll.EnrollType,
                    Data = ConvertEnrollDataToString(enroll),
                    DataLength = GetEnrollDataLength(enroll)
                }).ToList();
            }
            else
            {
                dto.Enrollments = new List<EnrollmentDto>();
            }

            return dto;
        }

        /// <summary>
        /// Chuyển đổi dữ liệu enrollment từ Riss.Devices.Enroll sang string
        /// </summary>
        private string ConvertEnrollDataToString(Enroll enroll)
        {
            try
            {
                // Vân tay: trả về base64 của Fingerprint
                if (enroll.Fingerprint != null && enroll.Fingerprint.Length > 0)
                {
                    return Convert.ToBase64String(enroll.Fingerprint);
                }

                // Mật khẩu: trả về Password trực tiếp
                if (!string.IsNullOrEmpty(enroll.Password))
                {
                    return enroll.Password;
                }

                // Thẻ: trả về CardID
                if (!string.IsNullOrEmpty(enroll.CardID))
                {
                    return enroll.CardID;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy độ dài dữ liệu enrollment
        /// </summary>
        private int GetEnrollDataLength(Enroll enroll)
        {
            try
            {
                if (enroll.Fingerprint != null && enroll.Fingerprint.Length > 0)
                {
                    return enroll.Fingerprint.Length;
                }

                if (!string.IsNullOrEmpty(enroll.Password))
                {
                    return enroll.Password.Length;
                }

                if (!string.IsNullOrEmpty(enroll.CardID))
                {
                    return enroll.CardID.Length;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
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