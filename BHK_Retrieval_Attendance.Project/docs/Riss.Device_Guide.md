# RISS.Device.Dll User's Guide - Tổng hợp đầy đủ

## 1. ENTITY CLASSES (Các lớp thực thể)

### 1.1. Device
**Namespace:** `Riss.Devices`  
**Mục đích:** Lưu trữ thông tin thiết bị

| Thuộc tính | Kiểu | Giới hạn | Mô tả |
|------------|------|----------|-------|
| DN | Int | - | ID thiết bị |
| SerialNumber | String | - | Số serial |
| Model | String | - | Model thiết bị |
| CommunicationType | CommunicationType | 0-2 | 0:Serial, 1:TCP/IP, 2:USB |
| Baudrate | Int | - | Tốc độ baud |
| SerialPort | Int | - | Cổng Serial |
| Password | String | - | Mật khẩu kết nối |
| IpPort | Int | - | Cổng TCP/IP |
| IpAddress | String | - | Địa chỉ IP |
| Label | String | - | Nhãn hiển thị |
| ConnectionModel | Int | 5 | Loại kết nối (ZD2911=5) |

### 1.2. User
**Namespace:** `Riss.Devices`  
**Mục đích:** Lưu trữ thông tin người dùng

| Thuộc tính | Kiểu | Giới hạn | Mô tả |
|------------|------|----------|-------|
| Privilege | Int | 1,2,4,8,16 | 1:User, 2:Registrar, 4:LogQuery, 8:Manager, 16:Client |
| DIN | UInt64 | Max 18 chữ số | ID người dùng |
| UserName | String | - | Tên người dùng |
| IDNumber | String | - | Số ID |
| Sex | Sex | - | Giới tính (F/M) |
| Enable | Bool | - | Kích hoạt |
| Comment | String | - | Ghi chú |
| DeptId | String | - | ID phòng ban |
| AttType | Int | - | Loại chấm công |
| Birthday | DateTime | - | Ngày sinh |
| AccessControl | Int | 0-3 | 0:Disable, 1:Lock1, 2:Lock2, 3:Both |
| ValidityPeriod | Bool | - | Bật thời hạn hiệu lực |
| UseUserGroupACTZ | Bool | - | Dùng nhóm cho vùng thời gian |
| UseUserGroupVM | Bool | - | Dùng nhóm cho chế độ xác minh |
| Department | Int | - | Số phòng ban |
| Enrolls | List<Enroll> | - | Danh sách dữ liệu đăng ký |
| AccessTimeZone | Int | - | Vùng thời gian kiểm soát |
| ValidDate | DateTime | 2010-2099 | Ngày bắt đầu hiệu lực |
| InvalidDate | DateTime | 2010-2099 | Ngày hết hiệu lực |
| UserGroup | Int | - | Nhóm người dùng |
| LockControl | UInt16 | - | Phạm vi điều khiển cửa |

### 1.3. UserExt
**Giống User** nhưng dành cho ngôn ngữ không hỗ trợ Generic (COM)
- `DIN` là String thay vì UInt64
- `Enrolls` là Enroll[] thay vì List<Enroll>

### 1.4. Enroll
**Namespace:** `Riss.Devices`  
**Mục đích:** Lưu dữ liệu đăng ký (vân tay, mật khẩu, thẻ)

| Thuộc tính | Kiểu | Mô tả |
|------------|------|-------|
| DIN | UInt64 | ID người dùng |
| EnrollType | EnrollType | Loại đăng ký (Finger0-9, Password, Card) |
| IsDuress | Bool | Đặt làm báo động cưỡng bức |
| Fingerprint | Byte[] | Dữ liệu vân tay (498 bytes) |
| Password | String | Mật khẩu |
| CardID | String | Số thẻ |

### 1.5. Record
**Namespace:** `Riss.Devices`  
**Mục đích:** Lưu bản ghi chấm công

| Thuộc tính | Kiểu | Mô tả |
|------------|------|-------|
| DIN | UInt64 | ID người dùng |
| DN | Int | ID thiết bị |
| Clock | DateTime | Thời gian chấm công |
| Verify | Int | Chế độ xác minh |
| Action | Int | Chi tiết (vào/ra) |
| Remark | String | Ghi chú |
| MDIN | UInt64 | Admin DIN cho S.Log |
| DoorStatus | Int | Trạng thái cửa |
| JobCode | Int | Mã công việc |
| Antipassback | Int | Chống quay lại |

### 1.6. Monitor
**Namespace:** `Riss.Devices`  
**Mục đích:** Thiết lập giám sát thời gian thực

| Thuộc tính | Kiểu | Giá trị | Mô tả |
|------------|------|---------|-------|
| Mode | Int | 0-1 | 0:UDP, 1:RS485 |
| UDPAddress | String | - | Địa chỉ IP |
| UDPPort | Int | - | Cổng UDP |
| SerialPort | Int | - | Cổng Serial |
| SerialBaudRate | Int | - | Tốc độ baud |

---

## 2. ENUM DEFINITIONS

### 2.1. CommunicationType
```csharp
public enum CommunicationType {
    Serial = 0,  // Giao tiếp Serial
    Tcp,         // Giao tiếp TCP/IP
    Usb          // Giao tiếp USB
}
```

### 2.2. EnrollType
```csharp
public enum EnrollType {
    Finger0 = 0,  // Vân tay thứ 1
    Finger1,      // Vân tay thứ 2
    ...
    Finger9,      // Vân tay thứ 10
    Password,     // Mật khẩu
    Card,         // Thẻ
    AllFinger,    // Tất cả vân tay
    All = 31      // Tất cả
}
```
**Chuyển đổi:** 12 chữ số nhị phân (0-9: vân tay, 10: password, 11: card)

### 2.3. UserProperty
```csharp
public enum UserProperty {
    UserName,              // Tên người dùng
    Enroll,                // Đăng ký
    UserExtInfo,           // Thông tin mở rộng
    AccessControlSettings, // Cài đặt kiểm soát
    Messages,              // Tin nhắn (không hỗ trợ)
    Privilege,             // Quyền hạn
    Attendance,            // Quy tắc chấm công
    UserEnroll             // Đăng ký người dùng
}
```

### 2.4. UserEnrollCommand
```csharp
public enum UserEnrollCommand {
    ReadFingerprint,    // Đọc vân tay
    WriteFingerprint,   // Ghi vân tay
    ClearFingerprint,   // Xóa vân tay
    ReadPassword,       // Đọc mật khẩu
    WritePassword,      // Ghi mật khẩu
    ClearPassword,      // Xóa mật khẩu
    ReadCard,           // Đọc thẻ
    WriteCard,          // Ghi thẻ
    ClearCard           // Xóa thẻ
}
```

### 2.5. DeviceProperty
```csharp
public enum DeviceProperty {
    FirmwareVersion,          // Phiên bản firmware
    FirmwareUpgrade,          // Nâng cấp firmware
    Bell,                     // Chuông
    DoorControl,              // Điều khiển cửa
    AccessControlSettings,    // Cài đặt kiểm soát
    WelcomeTitle,             // Tiêu đề chào
    StandbyTitle,             // Tiêu đề chờ
    InitSettings,             // Khởi tạo
    Status,                   // Trạng thái
    PowerOff,                 // Nguồn
    DeviceTime,               // Thời gian
    ManagementRecords,        // S.Log (log menu)
    AttRecords,               // G.Log (log chấm công)
    Enrolls,                  // Danh sách đăng ký
    Message,                  // Tin nhắn
    Enable,                   // Bật/tắt (1:busy, 0:idle)
    AttRecordsCount,          // Số lượng log chấm công
    ManagementRecordsCount,   // Số lượng S.Log
    PowerOnOffTime,           // Lịch bật/tắt
    MacAddress,               // Địa chỉ MAC
    Attendance,               // Quy tắc chấm công
    Model,                    // Model thiết bị
    SysParam,                 // Tham số hệ thống
    UploadSound               // Tải âm thanh
}
```

### 2.6. AccessControlCommand
```csharp
public enum AccessControlCommand {
    PassTime,         // Thời gian đi qua
    GroupTime,        // Nhóm thời gian (30 nhóm)
    TimeZone,         // Vùng thời gian (30*7 nhóm)
    LockGroup,        // Nhóm khóa
    DoorKey,          // Tham số cửa
    LogWatch,         // Giám sát log
    UserAccessCtrl,   // Kiểm soát người dùng
    UserPeriod        // Thời hạn người dùng
}
```

### 2.7. AttendanceCommand
```csharp
public enum AttendanceCommand {
    TimeSegment,  // Phân đoạn thời gian
    TimeZone,     // Vùng thời gian chấm công (3 nhóm)
    Holiday,      // Ngày lễ (40 nhóm)
    LogTime       // Thời gian log (24 nhóm)
}
```

### 2.8. NumberType
```csharp
public enum NumberType {
    UInt16Bit,  // UInt16
    Int16Bit,   // Int16
    UInt32Bit,  // UInt32
    Int32Bit,   // Int32
    UInt64Bit,  // UInt64
    Int64Bit    // Int64
}
```

---

## 3. UTILITY CLASSES

### 3.1. Zd2911Utils
**Namespace:** `Riss.Devices`  
**Mục đích:** Cung cấp các hằng số và phương thức tiện ích

#### Hằng số quan trọng:
```csharp
MaxFingerprintLength = 498;      // Độ dài vân tay
MaxFingerprintCount = 10;        // Số lượng vân tay
GroupTimeCount = 30;             // Số nhóm thời gian
TimeZoneCount = 30;              // Số vùng thời gian
TimeZoneWeekCount = 7;           // Số ngày trong tuần
BellGroupCount = 24;             // Số nhóm chuông
PowerTimeCount = 12;             // Số nhóm lịch bật/tắt
MaxDeviceMessageLength = 84;     // Độ dài tin nhắn
MaxDeviceMessageCount = 10;      // Số lượng tin nhắn
MaxAttendTimeZoneCount = 3;      // Số vùng chấm công
MaxHolidayCount = 30;            // Số ngày lễ
MaxValidAttendTimeCount = 24;    // Số thời gian hợp lệ
DeviceBusy = 1;                  // Thiết bị bận
DeviceIdle = 0;                  // Thiết bị rảnh
```

#### Phương thức:
**`public static int BitCheck(int num, int index)`**
- **Công dụng:** Kiểm tra trạng thái đăng ký tại vị trí index
- **Tham số:** num=trạng thái đăng ký, index=0-11 (0-9:vân tay, 10:password, 11:card)
- **Trả về:** ≠0 nếu có dữ liệu

**`public static int SetBit(int num, int index)`**
- **Công dụng:** Gộp trạng thái đăng ký
- **Tham số:** num=0, index=0-11
- **Trả về:** Trạng thái đăng ký mới

**`public static byte[] CreateChunkHeader(byte[] buffer, int dataChunk)`**
- **Công dụng:** Tạo header cho file âm thanh WAVE
- **Trả về:** Byte array header

### 3.2. Zd2911Tools (COM)
**Mở rộng:** `IZd2911Tools`  
**Mục đích:** Giống Zd2911Utils nhưng cho COM

**Phương thức bổ sung:**
- `GetSLogList(ref byte[])` → RecordExt[] - Chuyển byte thành S.Log
- `GetGLogList(ref byte[])` → RecordExt[] - Chuyển byte thành G.Log
- `GetUserExtWithoutEnroll(ref byte[])` → UserExt - Lấy user không có enroll
- `GetAllUserExtWithoutEnroll(ref byte[])` → UserExt[] - Lấy tất cả user
- `GetString(ref byte[])` → string - Decode Unicode
- `GetBytes(string)` → byte[] - Encode Unicode
- `GetStringByNum(ref byte[], int, NumberType)` → string - Chuyển số
- `GetBytesByNum(string, NumberType)` → byte[] - Chuyển số
- `GetASCII(ref byte[], int, int)` → string - Decode ASCII
- `ConvertIPAddressToNumber(string)` → int - IP sang số
- `ConvertNumberToIPAddress(int)` → string - Số sang IP

---

## 4. FILE MANAGEMENT CLASSES

### 4.1. Zd2911EnrollFileManagement
**Namespace:** `Riss.Devices`  
**Mục đích:** Quản lý file đăng ký (hỗ trợ Generic)

| Phương thức | Công dụng |
|-------------|-----------|
| `SaveAllUserEnrollDataAsDB(string, List<User>)` | Lưu tất cả user vào file |
| `SaveUserEnrollDataAsDB(string, User)` | Lưu 1 user vào file |
| `LoadAllUserEnrollDataFromDB(string, ref List<User>)` | Đọc tất cả user từ file |
| `LoadUserEnrollDataFromDB(string, ref User)` | Đọc 1 user từ file |
| `SaveUserNameData(string, List<User>)` | Lưu tên user |
| `LoadUserNameData(string, ref List<User>)` | Đọc tên user |

### 4.2. Zd2911EnrollFile (COM)
**Mở rộng:** `IZd2911EnrollFile`  
**Mục đích:** Giống trên nhưng dùng UserExt[] thay vì List<User>

---

## 5. REAL-TIME MONITORING

### 5.1. Zd2911Monitor
**Namespace:** `Riss.Devices`  
**Mục đích:** Giám sát log thời gian thực (UDP/RS485)

| Thành phần | Kiểu | Công dụng |
|------------|------|-----------|
| `CreateZd2911Monitor(Monitor)` | static method | Tạo monitor |
| `OpenListen()` | bool | Mở kết nối lắng nghe |
| `CloseListen()` | void | Đóng kết nối |
| `ReceiveHandler` | event | Sự kiện nhận log (ReceiveEventArg) |
| `IsBusy` | bool | Kiểm tra monitor có bận không |

### 5.2. Zd2911Listener (COM)
**Mở rộng:** `IZd2911Listener`  
**Khác biệt:** Dùng `ReceiveHandlerExt` (ReceiveEventArgExt)

---

## 6. DEVICE CONNECTION

### 6.1. DeviceConnection (Base Class)
**Namespace:** `Riss.Devices`  
**Mục đích:** Lớp cơ sở cho kết nối thiết bị

#### Phương thức chính:
**`public static DeviceConnection CreateConnection(ref Device device)`**
- Tạo kết nối thiết bị

**`public abstract int Open()`**
- Mở kết nối, >0 = thành công

**`public abstract void Close()`**
- Đóng kết nối

**User Operations:**
- `SetProperty(UserProperty, object, User, object)` - Thiết lập thông tin user
- `GetProperty(UserProperty, object, ref User, ref object)` - Lấy thông tin user
- `SetPropertyExt(UserProperty, ref byte[], UserExt, ref byte[])` - Thiết lập (COM)
- `GetPropertyExt(UserProperty, ref byte[], ref UserExt, ref byte[])` - Lấy (COM)

**Device Operations:**
- `SetProperty(DeviceProperty, object, Device, object)` - Thiết lập thiết bị
- `GetProperty(DeviceProperty, object, ref Device, ref object)` - Lấy thông tin thiết bị
- `SetPropertyExt(DeviceProperty, ref byte[], Device, ref byte[])` - Thiết lập (COM)
- `GetPropertyExt(DeviceProperty, ref byte[], ref Device, ref byte[])` - Lấy (COM)

### 6.2. Zd2911DeviceConnection (COM)
**Mở rộng:** `IZd2911DeviceConnection`  
**Khác biệt:** `Open(Device)` thay vì `Open()`

---

## 7. USER OPERATIONS

### 7.1. Thiết lập thông tin User

#### SetUserName
```csharp
User user = new User { DIN = 1, UserName = "Amaris" };
bool result = conn.SetProperty(UserProperty.UserName, null, user, null);
```

#### SetUserExtInfo (Ghi chú)
```csharp
User user = new User { DIN = 1, Comment = "Note" };
bool result = conn.SetProperty(UserProperty.UserExtInfo, null, user, null);
```

#### SetUserRole (Quyền)
```csharp
User user = new User { DIN = 1, Privilege = 1 };
bool result = conn.SetProperty(UserProperty.Privilege, null, user, null);
```

#### SetUserAccess (Kiểm soát)
```csharp
object extraProperty = AccessControlCommand.UserAccessCtrl;
User user = new User { DIN = 1, AccessTimeZone = 1, Enable = true };
bool result = conn.SetProperty(UserProperty.AccessControlSettings, 
    extraProperty, user, null);
```

#### SetUserPeriod (Thời hạn)
```csharp
object extraProperty = AccessControlCommand.UserPeriod;
byte[] data = new byte[8] {
    (byte)(2012-2000), 8, 8,  // Start: 2012-08-08
    (byte)(2018-2000), 12, 31, // End: 2018-12-31
    0, 0
};
bool result = conn.SetProperty(UserProperty.AccessControlSettings, 
    extraProperty, user, data);
```

#### SetUserAtType (Loại chấm công)
```csharp
object extraData = 1; // Attendance Type
User user = new User { DIN = 1 };
bool result = conn.SetProperty(UserProperty.Attendance, null, user, extraData);
```

### 7.2. Xóa dữ liệu

#### ClearCard
```csharp
object extraProperty = UserEnrollCommand.ClearCard;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { DIN = 1 });
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

#### ClearFingerprint
```csharp
object extraProperty = UserEnrollCommand.ClearFingerprint;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { DIN = 1, EnrollType = (EnrollType)0 });
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

#### ClearPassword
```csharp
object extraProperty = UserEnrollCommand.ClearPassword;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { DIN = 1 });
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

### 7.3. Ghi dữ liệu

#### WriteCard
```csharp
object extraProperty = UserEnrollCommand.WriteCard;
User user = new User { DIN = 1, Privilege = 1 };
user.Enrolls.Add(new Enroll { DIN = 1, CardID = "12345678" });
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

#### WriteFingerprint
```csharp
object extraProperty = UserEnrollCommand.WriteFingerprint;
User user = new User { DIN = 1, Privilege = 1 };
byte[] fingerprint = new byte[498 * (fpNo + 1)];
Array.Copy(fpData, 0, fingerprint, fpNo * 498, fpData.Length);
user.Enrolls.Add(new Enroll { 
    DIN = 1, 
    EnrollType = (EnrollType)fpNo,
    Fingerprint = fingerprint 
});
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

#### WritePassword
```csharp
object extraProperty = UserEnrollCommand.WritePassword;
User user = new User { DIN = 1, Privilege = 1 };
user.Enrolls.Add(new Enroll { DIN = 1, Password = "123123" });
bool result = conn.SetProperty(UserProperty.UserEnroll, extraProperty, user, null);
```

### 7.4. Đọc dữ liệu

#### GetUserName
```csharp
User user = new User { DIN = 1 };
bool result = conn.GetProperty(UserProperty.UserName, null, ref user, ref extraData);
// user.UserName chứa tên
```

#### ReadCard
```csharp
object extraProperty = UserEnrollCommand.ReadCard;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { DIN = 1 });
bool result = conn.GetProperty(UserProperty.UserEnroll, extraProperty, 
    ref user, ref extraData);
// user.Enrolls[0].CardID chứa số thẻ
```

#### ReadFingerprint
```csharp
object extraProperty = UserEnrollCommand.ReadFingerprint;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { 
    DIN = 1, 
    EnrollType = (EnrollType)fpNo,
    Fingerprint = new byte[498]
});
bool result = conn.GetProperty(UserProperty.UserEnroll, extraProperty, 
    ref user, ref extraData);
// user.Enrolls[0].Fingerprint chứa dữ liệu
```

#### ReadPassword
```csharp
object extraProperty = UserEnrollCommand.ReadPassword;
User user = new User { DIN = 1 };
user.Enrolls.Add(new Enroll { DIN = 1 });
bool result = conn.GetProperty(UserProperty.UserEnroll, extraProperty, 
    ref user, ref extraData);
// user.Enrolls[0].Password chứa mật khẩu
```

#### GetUserEnrollData (Tất cả user)
```csharp
// Bước 1: Lấy danh sách user
object extraProperty = (UInt64)0;
bool result = conn.GetProperty(DeviceProperty.Enrolls, extraProperty, 
    ref device, ref extraData);
List<User> userList = (List<User>)extraData;

// Bước 2: Lấy enroll cho từng user
foreach (User user in userList) {
    conn.GetProperty(UserProperty.Enroll, null, ref user, ref extraData);
}
```

---

## 8. DEVICE OPERATIONS

### 8.1. Thiết lập thiết bị

#### SetGroupTime (30 nhóm × 10 bytes)
```csharp
object extraProperty = AccessControlCommand.GroupTime;
// Lấy dữ liệu hiện tại
conn.GetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 30*10 = 300 bytes

// Sửa nhóm thứ i
int offset = i * 10;
data[offset + 2] = 1; // Multi-user
data[offset + 3] = 1; // Zone 1
data[offset + 4] = 1; // Zone 2
data[offset + 5] = 1; // Zone 3

conn.SetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    device, data);
```

#### SetTimeZone (30 vùng × 7 ngày × 6 bytes)
```csharp
object extraProperty = AccessControlCommand.TimeZone;
conn.GetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 30*7*6 = 1260 bytes

// Sửa vùng i, ngày j
int index = 6 * (i * 7 + j);
data[index + 2] = (byte)startHour;
data[index + 3] = (byte)startMinute;
data[index + 4] = (byte)endHour;
data[index + 5] = (byte)endMinute;

conn.SetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    device, data);
```

#### SetSysParam
```csharp
byte[] data = new byte[8];
Array.Copy(BitConverter.GetBytes(paramIndex), 0, data, 0, 4);
Array.Copy(BitConverter.GetBytes(paramValue), 0, data, 4, 4);
bool result = conn.SetProperty(DeviceProperty.SysParam, null, device, data);
```

#### InitDevice (Khởi tạo)
```csharp
bool result = conn.SetProperty(DeviceProperty.InitSettings, null, device, null);
```

#### EnableDevice (Bận/Rảnh)
```csharp
object extraData = (long)1; // 1=Busy, 0=Idle
bool result = conn.SetProperty(DeviceProperty.Enable, null, device, extraData);
```

#### SetDeviceTime
```csharp
bool result = conn.SetProperty(DeviceProperty.DeviceTime, null, device, null);
```

#### SetWelcomeTitle
```csharp
object extraProperty = Zd2911Utils.DeviceTile;
object extraData = "Welcome";
bool result = conn.SetProperty(DeviceProperty.WelcomeTitle, extraProperty, 
    device, extraData);
```

#### SetStandbyTitle
```csharp
object extraProperty = Zd2911Utils.DeviceStandbyTitle;
object extraData = "Press any key";
bool result = conn.SetProperty(DeviceProperty.StandbyTitle, extraProperty, 
    device, extraData);
```

#### SetMessage (10 tin nhắn × 84 bytes)
```csharp
object extraData = Zd2911Utils.DeviceMessage;
conn.GetProperty(DeviceProperty.Message, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 10*84=840 bytes

int offset = msgIndex * 84;
data[offset + 0] = Convert.ToByte(enabled);
data[offset + 1] = (byte)msgType;
data[offset + 2] = (byte)sound;
data[offset + 6] = (byte)(startYear - 2000);
data[offset + 7] = (byte)startMonth;
data[offset + 8] = (byte)startDay;
data[offset + 9] = (byte)startHour;
data[offset + 10] = (byte)startMinute;
data[offset + 11] = (byte)(endYear - 2000);
data[offset + 12] = (byte)endMonth;
data[offset + 13] = (byte)endDay;
data[offset + 14] = (byte)endHour;
data[offset + 15] = (byte)endMinute;
Array.Copy(BitConverter.GetBytes(userDIN), 0, data, offset + 16, 8);
byte[] contentBytes = Encoding.Unicode.GetBytes(content);
Array.Copy(contentBytes, 0, data, offset + 24, contentBytes.Length);

extraData = Encoding.Unicode.GetString(data);
extraProperty = Zd2911Utils.DeviceMessage;
conn.SetProperty(DeviceProperty.Message, extraProperty, device, extraData);
```

#### SetBell (24 chuông × 8 bytes)
```csharp
object extraData = Zd2911Utils.DeviceAlarmClock;
conn.GetProperty(DeviceProperty.Bell, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 24*8=192 bytes

int offset = bellIndex * 8;
data[offset + 0] = (byte)hour;
data[offset + 1] = (byte)minute;
data[offset + 2] = (byte)cycle;
data[offset + 3] = (byte)delay;

extraProperty = Zd2911Utils.DeviceAlarmClock;
extraData = Encoding.Unicode.GetString(data);
conn.SetProperty(DeviceProperty.Bell, extraProperty, device, extraData);
```

#### SetPowerOnOffTime (12 lịch × 2 loại × 4 bytes)
```csharp
object extraData = Zd2911Utils.DevicePowerTimer;
conn.GetProperty(DeviceProperty.PowerOnOffTime, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 86 bytes
// Index 0-47: 12 lịch bật (4 bytes mỗi lịch)
// Index 48-85: 12 lịch tắt (4 bytes mỗi lịch)

int index = powerType * 12 * 4 + scheduleIndex * 4;
data[index + 0] = (byte)hour;
data[index + 1] = (byte)minute;
data[index + 2] = Convert.ToByte(enabled);

extraProperty = Zd2911Utils.DevicePowerTimer;
extraData = Encoding.Unicode.GetString(data);
conn.SetProperty(DeviceProperty.PowerOnOffTime, extraProperty, device, extraData);
```

#### EmptySuperLogData (Xóa S.Log)
```csharp
bool result = conn.SetProperty(DeviceProperty.ManagementRecords, null, device, null);
```

#### EmptyGeneralLogData (Xóa G.Log)
```csharp
bool result = conn.SetProperty(DeviceProperty.AttRecords, null, device, null);
```

#### EmptyUserEnrollInfo
```csharp
object extraData = (UInt64)1; // DIN=1: xóa user này, DIN=0: xóa tất cả
bool result = conn.SetProperty(DeviceProperty.Enrolls, null, device, extraData);
```

#### SetMacAddress
```csharp
byte[] macAddress = new byte[6] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55 };
bool result = conn.SetProperty(DeviceProperty.MacAddress, null, device, macAddress);
```

#### SetAttendTimeZone (3 vùng × 14 bytes)
```csharp
object extraProperty = AttendanceCommand.TimeZone;
conn.GetProperty(DeviceProperty.Attendance, extraProperty, ref device, ref extraData);
byte[] data = (byte[])extraData; // 3*14=42 bytes

int offset = zoneIndex * 14;
data[offset + 2] = (byte)startHour;
data[offset + 3] = (byte)startMinute;
data[offset + 4] = (byte)endHour;
data[offset + 5] = (byte)endMinute;
data[offset + 6] = (byte)time1Left;
data[offset + 7] = (byte)time1Right;
data[offset + 8] = (byte)time2Left;
data[offset + 9] = (byte)time2Right;
data[offset + 10] = Convert.ToByte(time1Enabled);
data[offset + 11] = Convert.ToByte(time2Enabled);
data[offset + 12] = Convert.ToByte(zoneEnabled);

conn.SetProperty(DeviceProperty.Attendance, extraProperty, device, data);
```

#### SetHoliday (40 ngày lễ × 10 bytes)
```csharp
object extraProperty = AttendanceCommand.Holiday;
conn.GetProperty(DeviceProperty.Attendance, extraProperty, ref device, ref extraData);
byte[] data = (byte[])extraData; // 40*10=400 bytes

int offset = holidayIndex * 10;
data[offset + 2] = (byte)(startYear - 2000);
data[offset + 3] = (byte)startMonth;
data[offset + 4] = (byte)startDay;
data[offset + 5] = (byte)(endYear - 2000);
data[offset + 6] = (byte)endMonth;
data[offset + 7] = (byte)endDay;

conn.SetProperty(DeviceProperty.Attendance, extraProperty, device, data);
```

#### SetValidAtTime (24 khoảng × 6 bytes)
```csharp
object extraProperty = AttendanceCommand.LogTime;
object extraData = attendanceType; // Loại chấm công
conn.GetProperty(DeviceProperty.Attendance, extraProperty, ref device, ref extraData);
byte[] data = (byte[])extraData; // 24*6=144 bytes

int offset = timeIndex * 6;
data[offset + 2] = (byte)startHour;
data[offset + 3] = (byte)startMinute;
data[offset + 4] = (byte)endHour;
data[offset + 5] = (byte)endMinute;

conn.SetProperty(DeviceProperty.Attendance, extraProperty, device, data);
```

#### UploadSound (Chỉ USB, định dạng WAVE)
```csharp
private bool UploadSound(byte[] soundBuffer) {
    byte[] chunkHeader = Zd2911Utils.CreateChunkHeader(soundBuffer, dataChunk);
    int len = BitConverter.ToInt32(chunkHeader, 4);
    byte[] downData = new byte[len + 4];
    Array.Copy(chunkHeader, 4, downData, 0, 4);
    Array.Copy(soundBuffer, dataChunk + chunkHeaderLen, downData, 4, len);
    
    int unit = 1024 * 60; // 60KB mỗi lần
    int n = downData.Length / unit;
    
    // Upload từng phần
    for (int i = 0; i < n; i++) {
        byte[] dataBytes = new byte[unit];
        Array.Copy(downData, i * unit, dataBytes, 0, unit);
        
        List<int> soundParam = new List<int> { 
            soundType + 8, 
            i * unit 
        };
        bool result = conn.SetProperty(DeviceProperty.UploadSound, 
            soundParam, device, dataBytes);
        if (!result) return false;
    }
    
    // Upload phần còn lại
    int remaining = downData.Length % unit;
    if (remaining > 0) {
        byte[] dataBytes = new byte[remaining];
        Array.Copy(downData, n * unit, dataBytes, 0, remaining);
        
        List<int> soundParam = new List<int> { 
            soundType + 8, 
            n * unit 
        };
        return conn.SetProperty(DeviceProperty.UploadSound, 
            soundParam, device, dataBytes);
    }
    return true;
}
```

### 8.2. Lấy thông tin thiết bị

#### FirmwareVersion
```csharp
object extraData = Zd2911Utils.DeviceFirmwareVersion;
bool result = conn.GetProperty(DeviceProperty.FirmwareVersion, null, 
    ref device, ref extraData);
string version = (string)extraData;
```

#### GetModel
```csharp
object extraData = Zd2911Utils.DeviceModel;
bool result = conn.GetProperty(DeviceProperty.Model, null, ref device, ref extraData);
string model = (string)extraData;
```

#### GetDeviceTime
```csharp
bool result = conn.GetProperty(DeviceProperty.DeviceTime, null, 
    ref device, ref extraData);
DateTime deviceTime = (DateTime)extraData;
```

#### GetWelcomeTitle
```csharp
object extraData = Zd2911Utils.DeviceTile;
bool result = conn.GetProperty(DeviceProperty.WelcomeTitle, null, 
    ref device, ref extraData);
string title = (string)extraData;
```

#### GetStandbyTitle
```csharp
object extraData = Zd2911Utils.DeviceStandbyTitle;
bool result = conn.GetProperty(DeviceProperty.StandbyTitle, null, 
    ref device, ref extraData);
string title = (string)extraData;
```

#### GetGroupTime (30 nhóm × 10 bytes)
```csharp
object extraProperty = AccessControlCommand.GroupTime;
conn.GetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 300 bytes

// Đọc nhóm thứ i
int offset = i * 10;
int multiUser = data[offset + 2] - 1;
int zone1 = data[offset + 3] - 1;
int zone2 = data[offset + 4] - 1;
int zone3 = data[offset + 5] - 1;
```

#### GetTimeZone (30 × 7 × 6 bytes)
```csharp
object extraProperty = AccessControlCommand.TimeZone;
conn.GetProperty(DeviceProperty.AccessControlSettings, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 1260 bytes

// Đọc vùng i, ngày j
int index = 6 * (i * 7 + j);
int startHour = data[index + 2];
int startMinute = data[index + 3];
int endHour = data[index + 4];
int endMinute = data[index + 5];
```

#### GetSysParam
```csharp
byte[] paramIndex = BitConverter.GetBytes(1);
object extraData = paramIndex;
bool result = conn.GetProperty(DeviceProperty.SysParam, null, 
    ref device, ref extraData);
byte[] paramValue = (byte[])extraData;
```

#### GetMessage (10 × 84 bytes)
```csharp
object extraData = Zd2911Utils.DeviceMessage;
conn.GetProperty(DeviceProperty.Message, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 840 bytes

// Đọc tin nhắn thứ i
int offset = i * 84;
bool enabled = Convert.ToBoolean(data[offset + 0]);
int msgType = data[offset + 1];
int sound = data[offset + 2];
DateTime startDate = new DateTime(
    data[offset + 6] + 2000, 
    data[offset + 7], 
    data[offset + 8],
    data[offset + 9], 
    data[offset + 10], 
    0
);
DateTime endDate = new DateTime(
    data[offset + 11] + 2000, 
    data[offset + 12], 
    data[offset + 13],
    data[offset + 14], 
    data[offset + 15], 
    0
);
UInt64 userDIN = BitConverter.ToUInt64(data, offset + 16);
string content = Encoding.Unicode.GetString(data, offset + 24, 60)
    .Replace("\0", "");
```

#### GetBell (24 × 8 bytes)
```csharp
object extraData = Zd2911Utils.DeviceAlarmClock;
conn.GetProperty(DeviceProperty.Bell, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 192 bytes

// Đọc chuông thứ i
int offset = i * 8;
int hour = data[offset + 0];
int minute = data[offset + 1];
int cycle = data[offset + 2];
int delay = data[offset + 3];
```

#### GetPowerOnOffTime (24 × 4 bytes)
```csharp
object extraData = Zd2911Utils.DevicePowerTimer;
conn.GetProperty(DeviceProperty.PowerOnOffTime, null, ref device, ref extraData);
byte[] data = Encoding.Unicode.GetBytes((string)extraData); // 86 bytes

// powerType: 0=PowerOn, 1=PowerOff
// scheduleIndex: 0-11
int index = powerType * 12 * 4 + scheduleIndex * 4;
int hour = data[index + 0];
int minute = data[index + 1];
bool enabled = Convert.ToBoolean(data[index + 2]);
```

#### GetMacAddress
```csharp
bool result = conn.GetProperty(DeviceProperty.MacAddress, null, 
    ref device, ref extraData);
byte[] mac = (byte[])extraData; // 6 bytes
```

#### GetDeviceStatus
```csharp
bool result = conn.GetProperty(DeviceProperty.Status, null, 
    ref device, ref extraData);
UInt32[] count = (UInt32[])extraData;
// count[0]: User Count
// count[1]: Admin Count
// count[2]: Fingerprint Count
// count[3]: Card Count
// count[4]: Password Count
// count[5]: Newly SLog Count
// count[6]: Newly GLog Count
// count[7]: All SLog Count
// count[8]: All GLog Count
```

#### GetAttendTimeZone (3 × 14 bytes)
```csharp
object extraProperty = AttendanceCommand.TimeZone;
conn.GetProperty(DeviceProperty.Attendance, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 42 bytes

// Đọc vùng thứ i
int offset = i * 14;
int startHour = data[offset + 2];
int startMinute = data[offset + 3];
int endHour = data[offset + 4];
int endMinute = data[offset + 5];
int time1Left = data[offset + 6];
int time1Right = data[offset + 7];
int time2Left = data[offset + 8];
int time2Right = data[offset + 9];
bool time1Enabled = Convert.ToBoolean(data[offset + 10]);
bool time2Enabled = Convert.ToBoolean(data[offset + 11]);
bool zoneEnabled = Convert.ToBoolean(data[offset + 12]);
```

#### GetHoliday (40 × 10 bytes)
```csharp
object extraProperty = AttendanceCommand.Holiday;
conn.GetProperty(DeviceProperty.Attendance, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 400 bytes

// Đọc ngày lễ thứ i
int offset = i * 10;
DateTime startDate = new DateTime(
    data[offset + 2] + 2000, 
    data[offset + 3], 
    data[offset + 4]
);
DateTime endDate = new DateTime(
    data[offset + 5] + 2000, 
    data[offset + 6], 
    data[offset + 7]
);
```

#### GetValidAtTime (24 × 6 bytes)
```csharp
object extraProperty = AttendanceCommand.LogTime;
object extraData = attendanceType;
conn.GetProperty(DeviceProperty.Attendance, extraProperty, 
    ref device, ref extraData);
byte[] data = (byte[])extraData; // 144 bytes

// Đọc khoảng thời gian thứ i
int offset = i * 6;
int startHour = data[offset + 2];
int startMinute = data[offset + 3];
int endHour = data[offset + 4];
int endMinute = data[offset + 5];
```

#### GetUserEnrollInfo (Danh sách user)
```csharp
object extraProperty = (UInt64)0; // 0: tất cả
conn.GetProperty(DeviceProperty.Enrolls, extraProperty, 
    ref device, ref extraData);
List<User> userList = (List<User>)extraData;
```

#### GetUserEnrollInfoByUserID
```csharp
object extraProperty = (UInt64)1; // DIN cụ thể
conn.GetProperty(DeviceProperty.Enrolls, extraProperty, 
    ref device, ref extraData);
User user = (User)extraData;
```

### 8.3. Lấy Log

#### GetNewlySuperLogData (S.Log mới)
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
List<bool> boolList = new List<bool> { 
    true,              // Lấy log mới
    removeNewFlag      // Xóa dấu mới
};
conn.GetProperty(DeviceProperty.ManagementRecords, boolList, 
    ref device, ref dtList);
List<Record> records = (List<Record>)dtList;
```

#### GetAllSuperLogData (Tất cả S.Log)
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
List<bool> boolList = new List<bool> { 
    false,             // Lấy tất cả
    removeNewFlag      // Xóa dấu mới
};
conn.GetProperty(DeviceProperty.ManagementRecords, boolList, 
    ref device, ref dtList);
List<Record> records = (List<Record>)dtList;
```

#### GetNewlySuperLogCount
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
object extraProperty = true; // Log mới
conn.GetProperty(DeviceProperty.ManagementRecordsCount, extraProperty, 
    ref device, ref dtList);
int count = (int)dtList;
```

#### GetAllSuperLogCount
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
object extraProperty = false; // Tất cả
conn.GetProperty(DeviceProperty.ManagementRecordsCount, extraProperty, 
    ref device, ref dtList);
int count = (int)dtList;
```

#### GetNewlyGeneralLogData (G.Log mới)
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
List<bool> boolList = new List<bool> { 
    true,              // Lấy log mới
    removeNewFlag      // Xóa dấu mới
};
conn.GetProperty(DeviceProperty.AttRecords, boolList, 
    ref device, ref dtList);
List<Record> records = (List<Record>)dtList;
```

#### GetAllGeneralLogData (Tất cả G.Log)
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
List<bool> boolList = new List<bool> { 
    false,             // Lấy tất cả
    removeNewFlag      // Xóa dấu mới
};
conn.GetProperty(DeviceProperty.AttRecords, boolList, 
    ref device, ref dtList);
List<Record> records = (List<Record>)dtList;
```

#### GetNewlyGeneralLogCount
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
object extraProperty = true; // Log mới
conn.GetProperty(DeviceProperty.AttRecordsCount, extraProperty, 
    ref device, ref dtList);
int count = (int)dtList;
```

#### GetAllGeneralLogCount
```csharp
List<DateTime> dtList = new List<DateTime> { 
    startDate, 
    endDate 
};
object extraProperty = false; // Tất cả
conn.GetProperty(DeviceProperty.AttRecordsCount, extraProperty, 
    ref device, ref dtList);
int count = (int)dtList;
```

---

## 9. SYSTEM PARAMETERS

**Danh sách tham số hệ thống (Index 0-36):**

| Index | Tên tham số | Mô tả |
|-------|-------------|-------|
| 0 | Admin Count | Số lượng quản trị viên |
| 1 | Language Format | Định dạng ngôn ngữ |
| 2 | ID Length | Độ dài ID |
| 3 | Volume Size | Âm lượng |
| 4 | Auto Off Time | Thời gian tự tắt |
| 5 | Auto Power On | Tự động bật nguồn |
| 6 | Verify Mode | Chế độ xác minh |
| 7 | Auto Learning | Tự động học |
| 8 | Auto Return Time | Thời gian tự quay lại |
| 9 | Standby Time | Thời gian chờ |
| 10 | Enable Alarm In Standby | Báo động khi chờ |
| 11 | Card ID Type | Loại ID thẻ |
| 12 | Auto Restart | Tự động khởi động lại |
| 13 | Enable Shutdown | Cho phép tắt máy |
| 14 | Enable Relay Alarm | Báo động relay |
| 15 | Fire Alarm | Báo cháy |
| 16 | One To One Security Level | Mức bảo mật 1:1 |
| 17 | One To N Security Level | Mức bảo mật 1:N |
| 18 | SLog Warning Count | Ngưỡng cảnh báo S.Log |
| 19 | GLog Warning Count | Ngưỡng cảnh báo G.Log |
| 20 | Reverify Time | Thời gian xác minh lại |
| 21 | Device ID | ID thiết bị |
| 22 | Baudrate | Tốc độ baud |
| 23 | User Real Time Log | Log thời gian thực |
| 24 | UDP Port | Cổng UDP |
| 25 | Device Password | Mật khẩu thiết bị |
| 26 | IP Address | Địa chỉ IP |
| 27 | Sub Net Address | Địa chỉ Subnet |
| 28 | Default Gate | Gateway mặc định |
| 29 | Server IP Address | Địa chỉ IP Server |
| 30 | Server UDP Port | Cổng UDP Server |
| 31 | RS485 Use | Sử dụng RS485 |
| 32 | Lock Delay Time | Thời gian trễ khóa (ACS) |
| 33 | Wiegand Mode | Chế độ Wiegand (ACS) |
| 34 | Check Door State | Kiểm tra trạng thái cửa (ACS) |
| 35 | Menace Open Door | Mở cửa khẩn cấp (ACS) |
| 36 | Menace Alarm | Báo động khẩn cấp (ACS) |

**Cách sử dụng:**
```csharp
// Đọc tham số
byte[] paramIndex = BitConverter.GetBytes(21); // Device ID
object extraData = paramIndex;
conn.GetProperty(DeviceProperty.SysParam, null, ref device, ref extraData);
int deviceID = BitConverter.ToInt32((byte[])extraData, 0);

// Ghi tham số
byte[] data = new byte[8];
Array.Copy(BitConverter.GetBytes(21), 0, data, 0, 4); // Index
Array.Copy(BitConverter.GetBytes(999), 0, data, 4, 4); // Value
conn.SetProperty(DeviceProperty.SysParam, null, device, data);
```

---

## 10. QUY TRÌNH SỬ DỤNG CHUẨN

### 10.1. Kết nối thiết bị
```csharp
// Bước 1: Tạo Device
Device device = new Device {
    DN = 1,
    CommunicationType = CommunicationType.Tcp,
    IpAddress = "192.168.1.201",
    IpPort = 4370,
    Password = "0",
    ConnectionModel = 5
};

// Bước 2: Tạo kết nối
DeviceConnection conn = DeviceConnection.CreateConnection(ref device);

// Bước 3: Mở kết nối
int result = conn.Open();
if (result > 0) {
    // Kết nối thành công
    // ... thực hiện các thao tác
    
    // Bước 4: Đóng kết nối
    conn.Close();
}
```

### 10.2. Thêm người dùng mới
```csharp
// 1. Thiết lập thông tin cơ bản
User user = new User {
    DIN = 1,
    UserName = "John Doe",
    Privilege = 1,
    Enable = true
};
conn.SetProperty(UserProperty.UserName, null, user, null);

// 2. Ghi vân tay
user.Enrolls.Add(new Enroll {
    DIN = 1,
    EnrollType = EnrollType.Finger0,
    Fingerprint = fingerprintData // 498 bytes
});
conn.SetProperty(UserProperty.UserEnroll, 
    UserEnrollCommand.WriteFingerprint, user, null);

// 3. Ghi thẻ
user.Enrolls.Clear();
user.Enrolls.Add(new Enroll {
    DIN = 1,
    CardID = "12345678"
});
conn.SetProperty(UserProperty.UserEnroll, 
    UserEnrollCommand.WriteCard, user, null);

// 4. Ghi mật khẩu
user.Enrolls.Clear();
user.Enrolls.Add(new Enroll {
    DIN = 1,
    Password = "123456"
});
conn.SetProperty(UserProperty.UserEnroll, 
    UserEnrollCommand.WritePassword, user, null);
```

### 10.3. Giám sát thời gian thực
```csharp
// Tạo Monitor
Monitor monitor = new Monitor {
    Mode = 0,              // UDP
    UDPAddress = "0.0.0.0",
    UDPPort = 4370
};

// Tạo Listener
Zd2911Monitor listener = Zd2911Monitor.CreateZd2911Monitor(monitor);

// Đăng ký sự kiện
listener.ReceiveHandler += (sender, e) => {
    Record record = e.record;
    Console.WriteLine($"DIN: {record.DIN}, Time: {record.Clock}");
};

// Mở lắng nghe
if (listener.OpenListen()) {
    Console.WriteLine("Listening...");
    // ... chờ sự kiện
    
    // Đóng khi xong
    listener.CloseListen();
}
```

### 10.4. Lấy log chấm công
```csharp
// 1. Kiểm tra số lượng log mới
List<DateTime> dtList = new List<DateTime> { 
    DateTime.Now.AddDays(-7), 
    DateTime.Now 
};
object extraProperty = true; // Log mới
conn.GetProperty(DeviceProperty.AttRecordsCount, extraProperty, 
    ref device, ref dtList);
int newLogCount = (int)dtList;

// 2. Lấy log mới
if (newLogCount > 0) {
    dtList = new List<DateTime> { 
        DateTime.Now.AddDays(-7), 
        DateTime.Now 
    };
    List<bool> boolList = new List<bool> { true, true }; // Lấy mới, xóa dấu
    conn.GetProperty(DeviceProperty.AttRecords, boolList, 
        ref device, ref dtList);
    List<Record> records = (List<Record>)dtList;
    
    foreach (Record rec in records) {
        Console.WriteLine($"{rec.DIN} - {rec.Clock} - Verify:{rec.Verify}");
    }
}
```

### 10.5. Backup/Restore dữ liệu
```csharp
// Backup
Zd2911EnrollFileManagement fileMgr = new Zd2911EnrollFileManagement();

// 1. Lấy danh sách user
object extraProperty = (UInt64)0;
object extraData = new object();
conn.GetProperty(DeviceProperty.Enrolls, extraProperty, 
    ref device, ref extraData);
List<User> userList = (List<User>)extraData;

// 2. Lấy enroll data cho từng user
foreach (User user in userList) {
    conn.GetProperty(UserProperty.Enroll, null, ref user, ref extraData);
}

// 3. Lưu file
fileMgr.SaveAllUserEnrollDataAsDB("backup.dat", userList);

// Restore
List<User> restoredUsers = new List<User>();
fileMgr.LoadAllUserEnrollDataFromDB("backup.dat", ref restoredUsers);

// Upload lại lên thiết bị
foreach (User user in restoredUsers) {
    conn.SetProperty(UserProperty.UserName, null, user, null);
    conn.SetProperty(UserProperty.Enroll, null, user, false);
}
```

---

## 11. LƯU Ý QUAN TRỌNG

### 11.1. Kích thước dữ liệu
- **Vân tay:** 498 bytes/vân tay, tối đa 10 vân tay
- **GroupTime:** 30 nhóm × 10 bytes = 300 bytes
- **TimeZone:** 30 vùng × 7 ngày × 6 bytes = 1,260 bytes
- **Bell:** 24 chuông × 8 bytes = 192 bytes
- **Message:** 10 tin nhắn × 84 bytes = 840 bytes
- **PowerOnOff:** 24 lịch × 4 bytes = 96 bytes (86 bytes thực tế)
- **AttendTimeZone:** 3 vùng × 14 bytes = 42 bytes
- **Holiday:** 40 ngày × 10 bytes = 400 bytes
- **ValidAtTime:** 24 khoảng × 6 bytes = 144 bytes

### 11.2. Giới hạn và ràng buộc
- **DIN:** Tối đa 18 chữ số (UInt64)
- **UserName:** Chuỗi Unicode
- **Password:** Chuỗi, độ dài tùy thiết bị
- **CardID:** Chuỗi số
- **ValidDate/InvalidDate:** 2010-01-01 đến 2099-12-31
- **AccessControl:** 0=Disable, 1=Lock1, 2=Lock2, 3=Both
- **Privilege:** 1=User, 2=Registrar, 4=LogQuery, 8=Manager, 16=Client

### 11.3. Encoding
- **Unicode:** Dùng cho tên, tin nhắn, tiêu đề
- **ASCII:** Dùng cho một số trường đặc biệt
- **Binary:** Dùng cho vân tay, MAC address
- **Chuyển đổi:**
  ```csharp
  // Unicode to bytes
  byte[] bytes = Encoding.Unicode.GetBytes(text);
  
  // Bytes to Unicode
  string text = Encoding.Unicode.GetString(bytes);
  
  // ASCII to bytes
  byte[] bytes = Encoding.ASCII.GetBytes(text);
  ```

### 11.4. Xử lý lỗi
```csharp
try {
    DeviceConnection conn = DeviceConnection.CreateConnection(ref device);
    int result = conn.Open();
    
    if (result <= 0) {
        throw new Exception("Cannot connect to device");
    }
    
    bool success = conn.SetProperty(UserProperty.UserName, null, user, null);
    if (!success) {
        throw new Exception("Failed to set user name");
    }
    
    conn.Close();
}
catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}
```

### 11.5. Thread Safety
- Không nên gọi đồng thời nhiều thao tác trên cùng một kết nối
- Sử dụng lock khi cần:
  ```csharp
  private static readonly object deviceLock = new object();
  
  lock (deviceLock) {
      conn.SetProperty(UserProperty.UserName, null, user, null);
  }
  ```

### 11.6. Memory Management
- Đóng kết nối sau khi sử dụng
- Giải phóng Monitor/Listener:
  ```csharp
  if (listener != null) {
      listener.CloseListen();
      listener = null;
  }
  ```

### 11.7. Bit Operations (EnrollType)
```csharp
// Kiểm tra có vân tay thứ 5 không
int enrollStatus = 2048; // Binary: 100000000000
int hasFinger5 = Zd2911Utils.BitCheck(enrollStatus, 5);
// hasFinger5 != 0 → có vân tay

// Đánh dấu có vân tay thứ 5
int newStatus = Zd2911Utils.SetBit(0, 5);
// newStatus = 32 (2^5)

// Giải thích:
// Bit 0-9: Vân tay 0-9
// Bit 10: Password
// Bit 11: Card
// Ví dụ: 111111111111 (4095) = có đủ 10 vân tay + password + card
```

### 11.8. Date/Time Format
```csharp
// Thiết bị lưu năm từ 2000
// VD: 2024 → lưu là 24

// Chuyển DateTime sang byte
byte year = (byte)(dateTime.Year - 2000);
byte month = (byte)dateTime.Month;
byte day = (byte)dateTime.Day;
byte hour = (byte)dateTime.Hour;
byte minute = (byte)dateTime.Minute;

// Chuyển byte sang DateTime
DateTime dt = new DateTime(
    year + 2000,
    month,
    day,
    hour,
    minute,
    0
);
```

---

## 12. COMMON USE CASES

### 12.1. Thêm user với đầy đủ thông tin
```csharp
void AddCompleteUser(DeviceConnection conn, UInt64 din, string name,
    byte[] fingerprint, string cardID, string password) {
    
    // 1. Thông tin cơ bản
    User user = new User {
        DIN = din,
        UserName = name,
        Privilege = 1,
        Enable = true,
        AccessControl = 3, // Both locks
        ValidityPeriod = true
    };
    
    // 2. Ghi tên
    conn.SetProperty(UserProperty.UserName, null, user, null);
    
    // 3. Ghi quyền
    conn.SetProperty(UserProperty.Privilege, null, user, null);
    
    // 4. Ghi vân tay
    if (fingerprint != null && fingerprint.Length == 498) {
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll {
            DIN = din,
            EnrollType = EnrollType.Finger0,
            Fingerprint = fingerprint
        });
        conn.SetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.WriteFingerprint, user, null);
    }
    
    // 5. Ghi thẻ
    if (!string.IsNullOrEmpty(cardID)) {
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll {
            DIN = din,
            CardID = cardID
        });
        conn.SetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.WriteCard, user, null);
    }
    
    // 6. Ghi mật khẩu
    if (!string.IsNullOrEmpty(password)) {
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll {
            DIN = din,
            Password = password
        });
        conn.SetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.WritePassword, user, null);
    }
    
    // 7. Thiết lập kiểm soát
    object extraProperty = AccessControlCommand.UserAccessCtrl;
    user.AccessTimeZone = 1;
    conn.SetProperty(UserProperty.AccessControlSettings,
        extraProperty, user, null);
    
    // 8. Thiết lập thời hạn
    extraProperty = AccessControlCommand.UserPeriod;
    byte[] periodData = new byte[8] {
        (byte)(DateTime.Now.Year - 2000),
        (byte)DateTime.Now.Month,
        (byte)DateTime.Now.Day,
        (byte)(DateTime.Now.AddYears(1).Year - 2000),
        (byte)DateTime.Now.AddYears(1).Month,
        (byte)DateTime.Now.AddYears(1).Day,
        0, 0
    };
    conn.SetProperty(UserProperty.AccessControlSettings,
        extraProperty, user, periodData);
}
```

### 12.2. Xóa user hoàn toàn
```csharp
void DeleteUser(DeviceConnection conn, UInt64 din) {
    User user = new User { DIN = din };
    
    // Xóa tất cả vân tay
    for (int i = 0; i < 10; i++) {
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll {
            DIN = din,
            EnrollType = (EnrollType)i
        });
        conn.SetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.ClearFingerprint, user, null);
    }
    
    // Xóa thẻ
    user.Enrolls.Clear();
    user.Enrolls.Add(new Enroll { DIN = din });
    conn.SetProperty(UserProperty.UserEnroll,
        UserEnrollCommand.ClearCard, user, null);
    
    // Xóa mật khẩu
    conn.SetProperty(UserProperty.UserEnroll,
        UserEnrollCommand.ClearPassword, user, null);
    
    // Xóa thông tin user
    object extraData = din;
    conn.SetProperty(DeviceProperty.Enrolls, null, device, extraData);
}
```

### 12.3. Sync toàn bộ user
```csharp
List<User> SyncAllUsers(DeviceConnection conn, Device device) {
    // 1. Lấy danh sách
    object extraProperty = (UInt64)0;
    object extraData = new object();
    conn.GetProperty(DeviceProperty.Enrolls, extraProperty,
        ref device, ref extraData);
    List<User> userList = (List<User>)extraData;
    
    // 2. Lấy chi tiết từng user
    foreach (User user in userList) {
        // Lấy tên
        conn.GetProperty(UserProperty.UserName, null, ref user, ref extraData);
        
        // Lấy thông tin mở rộng
        conn.GetProperty(UserProperty.UserExtInfo, null, ref user, ref extraData);
        
        // Lấy enroll data
        conn.GetProperty(UserProperty.Enroll, null, ref user, ref extraData);
        
        // Lấy thẻ
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll { DIN = user.DIN });
        conn.GetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.ReadCard, ref user, ref extraData);
        
        // Lấy mật khẩu
        user.Enrolls.Clear();
        user.Enrolls.Add(new Enroll { DIN = user.DIN });
        conn.GetProperty(UserProperty.UserEnroll,
            UserEnrollCommand.ReadPassword, ref user, ref extraData);
        
        // Lấy tất cả vân tay
        for (int i = 0; i < 10; i++) {
            user.Enrolls.Clear();
            user.Enrolls.Add(new Enroll {
                DIN = user.DIN,
                EnrollType = (EnrollType)i,
                Fingerprint = new byte[498]
            });
            conn.GetProperty(UserProperty.UserEnroll,
                UserEnrollCommand.ReadFingerprint, ref user, ref extraData);
        }
    }
    
    return userList;
}
```

### 12.4. Thiết lập lịch làm việc
```csharp
void SetWorkSchedule(DeviceConnection conn, Device device) {
    // 1. Thiết lập TimeZone (8:00-17:00, Thứ 2-6)
    object extraProperty = AccessControlCommand.TimeZone;
    object extraData = new object();
    conn.GetProperty(DeviceProperty.AccessControlSettings,
        extraProperty, ref device, ref extraData);
    byte[] data = (byte[])extraData;
    
    int zoneIndex = 0; // Zone 1
    for (int weekday = 0; weekday < 5; weekday++) { // Mon-Fri
        int offset = 6 * (zoneIndex * 7 + weekday);
        data[offset + 2] = 8;  // Start: 8:00
        data[offset + 3] = 0;
        data[offset + 4] = 17; // End: 17:00
        data[offset + 5] = 0;
    }
    
    conn.SetProperty(DeviceProperty.AccessControlSettings,
        extraProperty, device, data);
    
    // 2. Thiết lập GroupTime
    extraProperty = AccessControlCommand.GroupTime;
    conn.GetProperty(DeviceProperty.AccessControlSettings,
        extraProperty, ref device, ref extraData);
    data = (byte[])extraData;
    
    int groupIndex = 0;
    int groupOffset = groupIndex * 10;
    data[groupOffset + 2] = 1; // Multi-user
    data[groupOffset + 3] = 1; // Zone 1
    data[groupOffset + 4] = 0;
    data[groupOffset + 5] = 0;
    
    conn.SetProperty(DeviceProperty.AccessControlSettings,
        extraProperty, device, data);
    
    // 3. Gán user vào group
    User user = new User { DIN = 1 };
    extraProperty = AccessControlCommand.UserAccessCtrl;
    user.AccessTimeZone = 1; // Group 1
    user.Enable = true;
    conn.SetProperty(UserProperty.AccessControlSettings,
        extraProperty, user, null);
}
```

### 12.5. Thiết lập chuông tự động
```csharp
void SetAutoBell(DeviceConnection conn, Device device) {
    object extraData = Zd2911Utils.DeviceAlarmClock;
    conn.GetProperty(DeviceProperty.Bell, null, ref device, ref extraData);
    byte[] data = Encoding.Unicode.GetBytes((string)extraData);
    
    // Chuông 1: 8:00 mỗi ngày
    data[0 * 8 + 0] = 8;  // Hour
    data[0 * 8 + 1] = 0;  // Minute
    data[0 * 8 + 2] = 0;  // Cycle: Daily
    data[0 * 8 + 3] = 5;  // Delay: 5 seconds
    
    // Chuông 2: 12:00 mỗi ngày
    data[1 * 8 + 0] = 12;
    data[1 * 8 + 1] = 0;
    data[1 * 8 + 2] = 0;
    data[1 * 8 + 3] = 5;
    
    // Chuông 3: 17:00 mỗi ngày
    data[2 * 8 + 0] = 17;
    data[2 * 8 + 1] = 0;
    data[2 * 8 + 2] = 0;
    data[2 * 8 + 3] = 5;
    
    object extraProperty = Zd2911Utils.DeviceAlarmClock;
    extraData = Encoding.Unicode.GetString(data);
    conn.SetProperty(DeviceProperty.Bell, extraProperty, device, extraData);
}
```

### 12.6. Export log ra CSV
```csharp
void ExportLogToCSV(DeviceConnection conn, Device device, string filePath) {
    // Lấy log
    List<DateTime> dtList = new List<DateTime> {
        DateTime.Now.AddMonths(-1),
        DateTime.Now
    };
    List<bool> boolList = new List<bool> { false, false };
    conn.GetProperty(DeviceProperty.AttRecords, boolList,
        ref device, ref dtList);
    List<Record> records = (List<Record>)dtList;
    
    // Lấy danh sách user để map tên
    object extraProperty = (UInt64)0;
    object extraData = new object();
    conn.GetProperty(DeviceProperty.Enrolls, extraProperty,
        ref device, ref extraData);
    List<User> users = (List<User>)extraData;
    
    Dictionary<UInt64, string> userNames = new Dictionary<UInt64, string>();
    foreach (User u in users) {
        conn.GetProperty(UserProperty.UserName, null, ref u, ref extraData);
        userNames[u.DIN] = u.UserName;
    }
    
    // Ghi CSV
    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8)) {
        sw.WriteLine("DIN,Name,Time,VerifyMode,Action");
        
        foreach (Record rec in records) {
            string name = userNames.ContainsKey(rec.DIN) ?
                userNames[rec.DIN] : "Unknown";
            sw.WriteLine($"{rec.DIN},{name},{rec.Clock:yyyy-MM-dd HH:mm:ss}," +
                $"{rec.Verify},{rec.Action}");
        }
    }
}
```

### 12.7. Auto-sync với database
```csharp
void AutoSyncWithDatabase(DeviceConnection conn, Device device,
    string connectionString) {
    
    // 1. Lấy log mới từ thiết bị
    List<DateTime> dtList = new List<DateTime> {
        DateTime.Now.AddDays(-1),
        DateTime.Now
    };
    List<bool> boolList = new List<bool> { true, true }; // New + Clear flag
    conn.GetProperty(DeviceProperty.AttRecords, boolList,
        ref device, ref dtList);
    List<Record> records = (List<Record>)dtList;
    
    if (records.Count == 0) return;
    
    // 2. Lưu vào database
    using (SqlConnection sqlConn = new SqlConnection(connectionString)) {
        sqlConn.Open();
        
        foreach (Record rec in records) {
            string query = @"
                INSERT INTO AttendanceLogs (DIN, DeviceID, LogTime, VerifyMode, Action)
                VALUES (@DIN, @DN, @Clock, @Verify, @Action)";
            
            using (SqlCommand cmd = new SqlCommand(query, sqlConn)) {
                cmd.Parameters.AddWithValue("@DIN", rec.DIN);
                cmd.Parameters.AddWithValue("@DN", rec.DN);
                cmd.Parameters.AddWithValue("@Clock", rec.Clock);
                cmd.Parameters.AddWithValue("@Verify", rec.Verify);
                cmd.Parameters.AddWithValue("@Action", rec.Action);
                cmd.ExecuteNonQuery();
            }
        }
    }
    
    Console.WriteLine($"Synced {records.Count} records to database");
}
```

### 12.8. Giám sát real-time với xử lý
```csharp
class RealtimeMonitor {
    private Zd2911Monitor listener;
    private Dictionary<UInt64, DateTime> lastAccess;
    
    public void Start(string ipAddress, int port) {
        lastAccess = new Dictionary<UInt64, DateTime>();
        
        Monitor monitor = new Monitor {
            Mode = 0,
            UDPAddress = ipAddress,
            UDPPort = port
        };
        
        listener = Zd2911Monitor.CreateZd2911Monitor(monitor);
        listener.ReceiveHandler += OnReceiveLog;
        
        if (listener.OpenListen()) {
            Console.WriteLine("Monitor started...");
        }
    }
    
    private void OnReceiveLog(object sender, ReceiveEventArg e) {
        Record rec = e.record;
        
        // Kiểm tra duplicate (trong 3 giây)
        if (lastAccess.ContainsKey(rec.DIN)) {
            TimeSpan diff = rec.Clock - lastAccess[rec.DIN];
            if (diff.TotalSeconds < 3) {
                return; // Bỏ qua duplicate
            }
        }
        
        lastAccess[rec.DIN] = rec.Clock;
        
        // Xử lý log
        ProcessLog(rec);
    }
    
    private void ProcessLog(Record rec) {
        // 1. Lưu database
        SaveToDatabase(rec);
        
        // 2. Gửi notification
        if (rec.Action == 1) { // Check-in
            SendNotification($"User {rec.DIN} checked in at {rec.Clock}");
        }
        
        // 3. Kiểm tra quy tắc
        CheckBusinessRules(rec);
    }
    
    private void SaveToDatabase(Record rec) {
        // Implementation
    }
    
    private void SendNotification(string message) {
        // Implementation
    }
    
    private void CheckBusinessRules(Record rec) {
        // Ví dụ: Kiểm tra check-in muộn
        if (rec.Clock.Hour > 8 && rec.Action == 1) {
            Console.WriteLine($"Late check-in: User {rec.DIN}");
        }
    }
    
    public void Stop() {
        if (listener != null) {
            listener.CloseListen();
            listener = null;
        }
    }
}
```

---

## 13. TROUBLESHOOTING

### 13.1. Không kết nối được
```csharp
// Kiểm tra kết nối
int result = conn.Open();
if (result <= 0) {
    // Kiểm tra:
    // 1. IP address đúng chưa?
    // 2. Port đúng chưa? (mặc định 4370)
    // 3. Firewall có chặn không?
    // 4. Thiết bị có bật không?
    // 5. Password đúng chưa?
    
    // Test ping
    Ping ping = new Ping();
    PingReply reply = ping.Send(device.IpAddress);
    if (reply.Status != IPStatus.Success) {
        Console.WriteLine("Cannot ping device");
    }
}
```

### 13.2. Không lấy được log
```csharp
// Kiểm tra số lượng log
List<DateTime> dtList = new List<DateTime> { 
    DateTime.Now.AddDays(-30), 
    DateTime.Now 
};
object extraProperty = false;
conn.GetProperty(DeviceProperty.AttRecordsCount, extraProperty,
    ref device, ref dtList);
int count = (int)dtList;

if (count == 0) {
    Console.WriteLine("No logs in device");
} else {
    Console.WriteLine($"Device has {count} logs");
    // Thử lấy lại với tham số khác
}
```

### 13.3. Vân tay không nhận
```csharp
// Kiểm tra dữ liệu vân tay
if (fingerprint == null || fingerprint.Length != 498) {
    Console.WriteLine("Invalid fingerprint data");
    return;
}

// Kiểm tra tất cả byte có = 0 không
bool allZero = fingerprint.All(b => b == 0);
if (allZero) {
    Console.WriteLine("Fingerprint data is empty");
    return;
}

// Thử ghi lại
User user = new User { DIN = 1, Privilege = 1 };
user.Enrolls.Add(new Enroll {
    DIN = 1,
    EnrollType = EnrollType.Finger0,
    Fingerprint = fingerprint
});

bool success = conn.SetProperty(UserProperty.UserEnroll,
    UserEnrollCommand.WriteFingerprint, user, null);

if (!success) {
    // Kiểm tra:
    // 1. User đã có trong thiết bị chưa?
    // 2. Privilege có đúng không?
    // 3. Dung lượng thiết bị còn không?
}
```

### 13.4. Memory leak
```csharp
// Đảm bảo giải phóng tài nguyên
public class DeviceManager : IDisposable {
    private DeviceConnection conn;
    private Zd2911Monitor listener;
    
    public void Dispose() {
        if (conn != null) {
            try {
                conn.Close();
            } catch { }
            conn = null;
        }
        
        if (listener != null) {
            try {
                listener.CloseListen();
            } catch { }
            listener = null;
        }
    }
}

// Sử dụng
using (DeviceManager dm = new DeviceManager()) {
    // Làm việc với thiết bị
}
```

---

## 14. BEST PRACTICES

### 14.1. Kết nối
- ✅ Luôn đóng kết nối sau khi sử dụng
- ✅ Sử dụng using hoặc try-finally
- ✅ Kiểm tra kết quả Open() trước khi tiếp tục
- ❌ Không mở nhiều kết nối cùng lúc đến 1 thiết bị

### 14.2. Xử lý dữ liệu
- ✅ Validate dữ liệu trước khi ghi
- ✅ Kiểm tra null/empty
- ✅ Kiểm tra độ dài byte array
- ❌ Không tin tưởng dữ liệu từ thiết bị 100%

### 14.3. Performance
- ✅ Batch operations khi có thể
- ✅ Sử dụng async/await cho UI
- ✅ Cache user list nếu cần query nhiều lần
- ❌ Không query từng user trong loop lớn

### 14.4. Error Handling
- ✅ Luôn có try-catch
- ✅ Log lỗi để debug
- ✅ Retry khi cần (network timeout)
- ❌ Không ignore exception

### 14.5. Security
- ✅ Mã hóa password khi lưu
- ✅ Validate DIN input
- ✅ Kiểm tra quyền trước khi thao tác
- ❌ Không hardcode password
