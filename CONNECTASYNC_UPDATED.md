# 🔧 **CẬP NHẬT: ConnectAsync với Password và Device Number** ✅

## 📊 **TÓM TẮT CẬP NHẬT**

### ✅ **INTERFACE CẬP NHẬT - IDeviceCommunicationService**

**File:** `BHK.Retrieval.Attendance.Core/Interfaces/Services/IDeviceCommunicationService.cs`

**Trước:**
```csharp
Task ConnectAsync(string ip, int port);
```

**Sau:**
```csharp
/// <summary>
/// Kết nối tới thiết bị
/// </summary>
/// <param name="ip">IP Address của thiết bị</param>
/// <param name="port">Port của thiết bị</param>
/// <param name="deviceNumber">Device Number (DN)</param>
/// <param name="password">Password thiết bị</param>
Task ConnectAsync(string ip, int port, int deviceNumber, string password);
```

---

### ✅ **INFRASTRUCTURE IMPLEMENTATION CẬP NHẬT**

**File:** `BHK.Retrieval.Attendance.Infrastructure/Devices/RealandDeviceService.cs`

**Cập nhật chính:**

```csharp
public async Task ConnectAsync(string ip, int port, int deviceNumber, string password)
{
    await Task.Run(() =>
    {
        try
        {
            _logger?.LogInformation("Infrastructure: Connecting to device at {ip}:{port}, DN: {deviceNumber}", 
                ip, port, deviceNumber);

            // ✅ Validation đầy đủ
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentException("IP address cannot be null or empty", nameof(ip));
            
            if (port <= 0 || port > 65535)
                throw new ArgumentException("Port must be between 1 and 65535", nameof(port));

            if (deviceNumber <= 0)
                throw new ArgumentException("Device number must be greater than 0", nameof(deviceNumber));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // ✅ Khởi tạo Device với input parameters
            _device = new Device
            {
                DN = deviceNumber, // ✅ Sử dụng deviceNumber từ input
                Password = password, // ✅ Sử dụng password từ input
                Model = "ZDC2911",
                ConnectionModel = 5, // ✅ QUAN TRỌNG: Phải = 5 cho ZD2911
                IpAddress = ip,
                IpPort = port,
                CommunicationType = CommunicationType.Tcp
            };

            // ✅ Tạo connection và kiểm tra kết quả
            _deviceConnection = DeviceConnection.CreateConnection(ref _device);
            int result = _deviceConnection.Open();
            
            if (result > 0)
            {
                _isConnected = true;
                _logger?.LogInformation("Infrastructure: ✅ Successfully connected. Result: {result}", result);
            }
            else
            {
                // Cleanup và throw exception
                _deviceConnection?.Close();
                _deviceConnection = null;
                _device = null;
                _isConnected = false;
                throw new Exception($"Failed to connect. Error code: {result}");
            }
        }
        catch (Exception ex)
        {
            // Complete cleanup
            try { _deviceConnection?.Close(); } catch { }
            _deviceConnection = null;
            _device = null;
            _isConnected = false;
            throw; // Re-throw để WPF bắt được
        }
    });
}
```

---

### ✅ **WPF SERVICE CẬP NHẬT**

**File:** `BHK.Retrieval.Attendance.WPF/Services/Implementations/DeviceService.cs`

**Cập nhật chính:**

```csharp
// ✅ Gọi Infrastructure với đầy đủ parameters
await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);

// Trong TestConnectionAsync cũng tương tự:
await _deviceCommunicationService.ConnectAsync(ipAddress, port, deviceNumber, password);
await _deviceCommunicationService.DisconnectAsync();
```

---

## 🎯 **VALIDATION CẬP NHẬT**

### ✅ **Input Validation Mở Rộng:**

1. **IP Address:** Không null/empty
2. **Port:** 1-65535
3. **Device Number:** > 0 ✨ **MỚI**
4. **Password:** Không null/empty ✨ **MỚI**

### ✅ **Logging Cải Thiện:**

```csharp
_logger?.LogInformation("Infrastructure: Connecting to device at {ip}:{port}, DN: {deviceNumber}", 
    ip, port, deviceNumber);
```

---

## 🏗️ **LUỒNG HOẠT ĐỘNG MỚI**

```
┌─────────────────────────────────────────────────────────────────┐
│ 🎨 WPF DeviceService                                            │
│ ─────────────────────────────────────────────────────────────── │
│ ConnectTcpAsync(ip, port, deviceNumber, password)              │
│           ↓                                                     │
│ _deviceCommunicationService.ConnectAsync(ip, port, DN, pwd)    │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ Dependency Injection
┌─────────────────────────────────────────────────────────────────┐
│ 🧠 IDeviceCommunicationService (Interface)                     │
│ ─────────────────────────────────────────────────────────────── │
│ ConnectAsync(ip, port, deviceNumber, password)                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ Implementation
┌─────────────────────────────────────────────────────────────────┐
│ 🔧 RealandDeviceService (Infrastructure)                       │
│ ─────────────────────────────────────────────────────────────── │
│ • Validate input parameters                                    │
│ • Create Device with DN + Password                             │
│ • DeviceConnection.CreateConnection(ref device)                │
│ • connection.Open() → Check result code                        │
│ • Success/Failure handling                                     │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎉 **BENEFITS**

### ✅ **Flexibility:**
- Device Number có thể khác nhau cho từng thiết bị
- Password có thể khác nhau cho từng thiết bị
- Không còn hardcode values

### ✅ **Validation:**
- Kiểm tra đầy đủ tất cả input parameters
- Clear error messages cho từng validation failure
- Proper exception handling

### ✅ **Clean Architecture:**
- Interface và Implementation đồng bộ
- WPF gọi Infrastructure với full parameters
- Không vi phạm dependency rules

### ✅ **Production Ready:**
- Flexible configuration
- Robust error handling
- Proper logging với full context

---

## 🚀 **TESTING SCENARIOS**

### **Test với parameters thực:**

```csharp
// Trong DeviceConnectionView hoặc test
var result = await deviceService.ConnectTcpAsync(
    ipAddress: "192.168.1.201",
    port: 4370,
    deviceNumber: 1,          // ✨ Có thể thay đổi theo thiết bị
    password: "0"             // ✨ Có thể thay đổi theo cấu hình
);
```

### **Error Scenarios:**

1. **❌ Empty Password:** `ArgumentException: Password cannot be null or empty`
2. **❌ Invalid Device Number:** `ArgumentException: Device number must be greater than 0`
3. **❌ Device Reject Connection:** `Exception: Failed to connect. Error code: -1`
4. **✅ Successful Connection:** `result > 0` → Success

---

## 📋 **NEXT STEPS**

1. **Test với device thực** với các Device Numbers khác nhau
2. **Test với passwords khác nhau** nếu device có bảo mật
3. **Consider thêm TestConnectionAsync** vào Interface để tối ưu testing
4. **Implement advanced device operations** với validated connection

**🎯 Bây giờ ConnectAsync đã hoàn thiện với đầy đủ input validation và flexibility!**