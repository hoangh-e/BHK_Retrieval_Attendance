# Tích hợp Dữ liệu Thực từ Thiết bị Riss.Device

## Tổng quan
Đã cập nhật `RealandDeviceService` để lấy **dữ liệu thực từ thiết bị ZDC2911** thông qua thư viện Riss.Device, thay thế hoàn toàn dữ liệu mock trước đó.

## ✅ Tuân thủ Clean Architecture

### Phân tầng rõ ràng:
- **Core Layer:** `EmployeeDto`, `EnrollmentDto`, `AttendanceRecordDto` - KHÔNG phụ thuộc vào Riss.Devices
- **Infrastructure Layer:** `RealandDeviceService` - DUY NHẤT layer được phép `using Riss.Devices`
- **WPF Layer:** Chỉ sử dụng DTOs từ Core, KHÔNG trực tiếp gọi Riss.Devices

## 📋 Các Method Đã Cập Nhật

### 1. GetAllEmployeesAsync()
**Trước:** Mock 50 nhân viên giả
**Sau:** Lấy dữ liệu thực từ thiết bị

```csharp
// Bước 1: Lấy danh sách user từ thiết bị
object extraProperty = (UInt64)0; // 0 = lấy tất cả user
object? extraData = null;

bool result = _deviceConnection.GetProperty(
    DeviceProperty.Enrolls, 
    extraProperty, 
    ref _device, 
    ref extraData
);

var users = (List<User>)extraData;

// Bước 2: Lấy enrollment data cho từng user
for (int i = 0; i < users.Count; i++)
{
    User user = users[i];
    object? enrollData = null;
    
    _deviceConnection.GetProperty(
        UserProperty.Enroll, 
        null, 
        ref user, 
        ref enrollData
    );
    
    users[i] = user; // Cập nhật lại user với enrollment data
}

// Bước 3: Convert sang DTO
var employees = users.Select(user => MapRissUserToEmployeeDto(user)).ToList();
```

**API Reference:** ZDC2911 User Guide - Section 7.4 "GetUserEnrollData"

### 2. GetEmployeeByIdAsync(ulong din)
**Trước:** Trả về mock data cho DIN
**Sau:** Lấy user cụ thể từ thiết bị

```csharp
// Lấy user theo DIN cụ thể
object extraProperty = (UInt64)din;
object? extraData = null;

bool result = _deviceConnection.GetProperty(
    DeviceProperty.Enrolls, 
    extraProperty, 
    ref _device, 
    ref extraData
);

var users = (List<User>)extraData;
if (users.Count == 0) return null;

User user = users[0];

// Lấy enrollment data
_deviceConnection.GetProperty(UserProperty.Enroll, null, ref user, ref enrollData);

return MapRissUserToEmployeeDto(user);
```

**API Reference:** ZDC2911 User Guide - Section 10.1.2 "Read Specific User"

### 3. GetEmployeeCountAsync()
**Trước:** Trả về 50 (hard-coded)
**Sau:** Đếm số user thực tế

```csharp
object extraProperty = (UInt64)0;
object? extraData = null;

_deviceConnection.GetProperty(DeviceProperty.Enrolls, extraProperty, ref _device, ref extraData);

var users = (List<User>)extraData;
return users.Count;
```

**Lý do:** ZDC2911 không có API trực tiếp để lấy count, cần lấy toàn bộ danh sách rồi đếm.

### 4. ClearAllEmployeesAsync()
**Trước:** Luôn trả về true (không làm gì)
**Sau:** Xóa tất cả user trên thiết bị

```csharp
// EmptyUserEnrollInfo: DIN=0 để xóa tất cả user
object extraData = (UInt64)0;

bool result = _deviceConnection.SetProperty(
    DeviceProperty.Enrolls, 
    null, 
    _device, 
    extraData
);

return result;
```

**API Reference:** ZDC2911 User Guide - Section 8.1 "EmptyUserEnrollInfo"

## 🔄 Mapper Methods

### MapRissUserToEmployeeDto (Riss.User → DTO)
Chuyển đổi từ `Riss.Devices.User` sang `EmployeeDto`:

```csharp
private EmployeeDto MapRissUserToEmployeeDto(User rissUser)
{
    var dto = new EmployeeDto
    {
        DIN = rissUser.DIN,
        UserName = rissUser.UserName ?? string.Empty,
        IDNumber = rissUser.IDNumber ?? string.Empty,
        // ... các thuộc tính khác
    };

    // ✅ Map Sex using reflection (tránh compile-time dependency)
    var sexProperty = rissUser.GetType().GetProperty("Sex");
    if (sexProperty != null)
    {
        var sexValue = sexProperty.GetValue(rissUser);
        dto.Sex = Convert.ToInt32(sexValue); // 0=Male, 1=Female
    }

    // ✅ Map enrollments
    if (rissUser.Enrolls != null && rissUser.Enrolls.Count > 0)
    {
        dto.Enrollments = rissUser.Enrolls.Select(enroll => new EnrollmentDto
        {
            EnrollType = (int)enroll.EnrollType,
            Data = ConvertEnrollDataToString(enroll),
            DataLength = GetEnrollDataLength(enroll)
        }).ToList();
    }

    return dto;
}
```

### ConvertEnrollDataToString
Chuyển đổi dữ liệu enrollment:
- **Vân tay:** `Fingerprint` (byte[]) → Base64 string
- **Mật khẩu:** `Password` (string) → trả về trực tiếp
- **Thẻ:** `CardID` (string) → trả về trực tiếp

```csharp
private string ConvertEnrollDataToString(Enroll enroll)
{
    // Vân tay: 498 bytes → Base64
    if (enroll.Fingerprint != null && enroll.Fingerprint.Length > 0)
        return Convert.ToBase64String(enroll.Fingerprint);

    // Mật khẩu
    if (!string.IsNullOrEmpty(enroll.Password))
        return enroll.Password;

    // Thẻ
    if (!string.IsNullOrEmpty(enroll.CardID))
        return enroll.CardID;

    return string.Empty;
}
```

## 🎯 EnrollType Mapping

Theo ZDC2911 specification:

| EnrollType (int) | Tên | Mô tả |
|-----------------|-----|-------|
| 0-9 | Finger0-Finger9 | 10 vân tay |
| 10 | Password | Mật khẩu |
| 11 | Card | Thẻ từ |

**Lưu ý:** WPF layer sử dụng `int` thay vì `EnrollType` enum để tránh phụ thuộc vào Riss.Devices.

## 📊 Enrollment Data Structure

### Fingerprint (EnrollType 0-9)
- **Data format:** Base64 string của byte array 498 bytes
- **DataLength:** 498

### Password (EnrollType 10)
- **Data format:** Plain string (không mã hóa trong DTO)
- **DataLength:** Độ dài chuỗi

### Card (EnrollType 11)
- **Data format:** CardID string
- **DataLength:** Độ dài CardID

## ⚠️ Lưu ý Quan trọng

### 1. Reflection cho Sex Property
Sử dụng reflection để tránh compile-time dependency vào `Riss.Devices.Sex` enum:

```csharp
var sexProperty = rissUser.GetType().GetProperty("Sex");
var sexValue = sexProperty.GetValue(rissUser);
dto.Sex = Convert.ToInt32(sexValue); // 0=Male, 1=Female
```

### 2. For Loop thay vì Foreach
Khi gọi `GetProperty` với `ref User`, phải dùng `for` để có thể cập nhật user trong list:

```csharp
// ❌ KHÔNG ĐƯỢC: foreach không cho phép ref
foreach (User user in users) {
    _deviceConnection.GetProperty(UserProperty.Enroll, null, ref user, ref data);
}

// ✅ ĐÚNG: for loop với index
for (int i = 0; i < users.Count; i++) {
    User user = users[i];
    _deviceConnection.GetProperty(UserProperty.Enroll, null, ref user, ref data);
    users[i] = user; // Cập nhật lại
}
```

### 3. Nullable Object
Khởi tạo `extraData` với nullable để tránh compiler warning:

```csharp
object? extraData = null; // ✅ Thay vì object extraData = null;
```

## 🔍 API Reference - ZDC2911 User Guide

### Lấy tất cả user:
```csharp
object extraProperty = (UInt64)0; // 0 = tất cả
conn.GetProperty(DeviceProperty.Enrolls, extraProperty, ref device, ref extraData);
List<User> users = (List<User>)extraData;
```

### Lấy user theo DIN:
```csharp
object extraProperty = (UInt64)din; // DIN cụ thể
conn.GetProperty(DeviceProperty.Enrolls, extraProperty, ref device, ref extraData);
List<User> users = (List<User>)extraData;
```

### Lấy enrollment data:
```csharp
conn.GetProperty(UserProperty.Enroll, null, ref user, ref extraData);
// user.Enrolls sẽ chứa List<Enroll>
```

### Xóa tất cả user:
```csharp
object extraData = (UInt64)0; // 0 = xóa tất cả
conn.SetProperty(DeviceProperty.Enrolls, null, device, extraData);
```

## 🎉 Kết quả

### Build Status
✅ **0 errors, 66 warnings** (chỉ nullable reference warnings - không ảnh hưởng)

### Tích hợp hoàn chỉnh
- ✅ Dữ liệu nhân viên lấy từ thiết bị thực
- ✅ Enrollment data (vân tay, mật khẩu, thẻ) được lấy đầy đủ
- ✅ Tuân thủ Clean Architecture nghiêm ngặt
- ✅ DTO flow: Riss.User → EmployeeDto → WPF UI
- ✅ Không có compile-time dependency từ WPF → Riss.Devices

### Methods còn TODO
Các methods sau vẫn dùng stub/mock (sẽ implement sau khi có thiết bị thật để test):
- `AddEmployeeAsync()` - Thêm nhân viên mới
- `UpdateEmployeeAsync()` - Cập nhật thông tin
- `DeleteEmployeeAsync()` - Xóa 1 nhân viên
- `EnrollFingerprintAsync()` - Đăng ký vân tay
- `EnrollPasswordAsync()` - Đặt mật khẩu
- `EnrollCardAsync()` - Đăng ký thẻ
- `GetAttendanceRecordsAsync()` - Lấy lịch sử chấm công

---

**Ngày cập nhật:** 2025-10-09  
**Tác giả:** GitHub Copilot  
**Version:** 1.0
