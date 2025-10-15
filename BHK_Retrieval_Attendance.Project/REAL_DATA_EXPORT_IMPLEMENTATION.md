# TRIỂN KHAI XUẤT DỮ LIỆU THẬT - SUMMARY

## Tổng quan
Đã triển khai xuất dữ liệu THẬT (không dùng dữ liệu mẫu) cho 2 chức năng theo kiến trúc Clean Architecture:
1. **Quản lý phân công (AttendanceManagement)**: Xuất dữ liệu sau khi lọc
2. **Quản lý nhân viên (Employee)**: Xuất toàn bộ danh sách nhân viên

---

## Chi tiết thay đổi

### 1. Quản lý Phân Công (Attendance Management)

#### File: `AttendanceManagementViewModel.cs`
**Thay đổi:** Cập nhật method `OpenExportDialogAsync()`

```csharp
// ✅ Kiểm tra dữ liệu THẬT
if (AttendanceRecords == null || AttendanceRecords.Count == 0)
{
    await _notificationService.ShowWarningAsync("Cảnh báo", 
        "Không có dữ liệu để xuất. Vui lòng tải dữ liệu trước.");
    _logger.LogWarning("No attendance records to export");
    return;
}

// ✅ Sử dụng dữ liệu THẬT từ danh sách đã lọc
var attendanceData = AttendanceRecords.ToList();
var viewModel = new ViewModels.Dialogs.ExportConfigurationViewModel(
    dialog,
    $"attendance_{DateTime.Now:yyyy-MM-dd}",
    attendanceData.Count,
    attendanceData  // ✅ Dữ liệu THẬT sau khi lọc
);
```

**Đặc điểm:**
- ✅ Sử dụng `AttendanceRecords` - dữ liệu sau khi lọc theo filter
- ✅ Kiểm tra null/empty trước khi xuất
- ✅ Log số lượng bản ghi xuất
- ✅ Hiển thị thông báo thành công với số lượng chính xác

---

### 2. Quản lý Nhân Viên (Employee Management)

#### A. File: `EmployeeViewModel.cs`

**Thêm Command mới:**
```csharp
public ICommand ExportAllEmployeesCommand { get; } // ✅ Command xuất toàn bộ nhân viên
```

**Khởi tạo Command:**
```csharp
ExportAllEmployeesCommand = new RelayCommand(async _ => await ExportAllEmployeesAsync());
```

**Thêm Method mới: `ExportAllEmployeesAsync()`**
```csharp
/// <summary>
/// Xuất TOÀN BỘ danh sách nhân viên (không phân trang)
/// ✅ Sử dụng dữ liệu THẬT từ thiết bị
/// </summary>
private async Task ExportAllEmployeesAsync()
{
    // ✅ Lấy TOÀN BỘ danh sách nhân viên THẬT từ thiết bị (không phân trang)
    var allUsers = await _deviceService.GetBasicUsersAsync();
    
    if (allUsers == null || !allUsers.Any())
    {
        _logger.LogWarning("No employees found in device");
        ShowWarningMessage("Không có nhân viên nào để xuất");
        return;
    }

    // Chuyển đổi sang EmployeeDisplayModel với STT
    var employeeList = allUsers
        .Select((user, index) => new EmployeeDisplayModel
        {
            Index = index + 1,  // STT bắt đầu từ 1
            DIN = user.DIN.ToString(),
            UserName = user.UserName ?? "N/A",
            Department = GetDepartmentName(user.DeptId)
        })
        .ToList();

    // Tạo dialog xuất file với dữ liệu THẬT
    var dialog = new Views.Dialogs.ExportEmployeeDialog();
    var viewModel = new ViewModels.Dialogs.ExportEmployeeViewModel(
        dialog,
        $"employees_{DateTime.Now:yyyy-MM-dd}",
        employeeList.Count,
        employeeList  // ✅ Dữ liệu THẬT - toàn bộ nhân viên
    );
}
```

**Đặc điểm:**
- ✅ Gọi `_deviceService.GetBasicUsersAsync()` để lấy **TOÀN BỘ** nhân viên (không phân trang)
- ✅ Chuyển đổi từ `EmployeeDto` sang `EmployeeDisplayModel` với đầy đủ thông tin
- ✅ Tự động tạo STT (Index) từ 1 đến N
- ✅ Hiển thị loading indicator
- ✅ Log chi tiết số lượng nhân viên xuất

**Thêm Helper Method:**
```csharp
private void ShowSuccessMessage(string message)
{
    _logger.LogInformation(message);
    MessageBox.Show(message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
}
```

---

#### B. File: `ExportEmployeeViewModel.cs` (MỚI)

**Tạo ViewModel riêng cho xuất nhân viên:**

```csharp
public partial class ExportEmployeeViewModel : ObservableObject
{
    private readonly List<EmployeeDisplayModel> _data; // ✅ Dữ liệu THẬT
    
    public ExportEmployeeViewModel(Window dialog, string fileName, 
        int recordCount, List<EmployeeDisplayModel> data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
```

**Các format xuất được hỗ trợ:**
1. **JSON** - UTF-8, có indent, escape tiếng Việt
2. **Excel (.xlsx)** - Tạm thời dùng CSV
3. **Text (.txt)** - Format table đẹp, có header và footer
4. **CSV** - Escape dấu phẩy, hỗ trợ tiếng Việt

**Export Methods:**
```csharp
private void ExportToJson(string filePath)
{
    var json = System.Text.Json.JsonSerializer.Serialize(_data, 
        new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
}

private void ExportToText(string filePath)
{
    using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
    writer.WriteLine("DANH SÁCH NHÂN VIÊN");
    writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
    writer.WriteLine($"{"STT",-5} {"DIN",-15} {"Họ tên",-30} {"Phòng ban",-25}");
    
    foreach (var employee in _data)
    {
        writer.WriteLine($"{employee.Index,-5} {employee.DIN,-15} " +
            $"{employee.UserName,-30} {employee.Department,-25}");
    }
    
    writer.WriteLine($"Tổng số: {_data.Count} nhân viên");
}

private void ExportToCsv(string filePath)
{
    using var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
    writer.WriteLine("STT,DIN,Họ tên,Phòng ban");
    
    foreach (var employee in _data)
    {
        var userName = EscapeCsvField(employee.UserName);
        var department = EscapeCsvField(employee.Department);
        writer.WriteLine($"{employee.Index},{employee.DIN},{userName},{department}");
    }
}
```

---

#### C. File: `ExportEmployeeDialog.xaml`

**Đơn giản hóa UI giống ExportConfigurationDialog:**

```xaml
<Window Title="Xuất danh sách nhân viên" 
        Height="400" Width="500">
    <Grid Margin="20">
        <!-- ComboBox chọn format -->
        <ComboBox SelectedItem="{Binding SelectedFileType}">
            <x:Array Type="{x:Type system:String}">
                <system:String>JSON (.json)</system:String>
                <system:String>Excel (.xlsx)</system:String>
                <system:String>Text (.txt)</system:String>
                <system:String>CSV (.csv)</system:String>
            </x:Array>
        </ComboBox>
        
        <!-- TextBox tên file -->
        <TextBox Text="{Binding FileName}"/>
        
        <!-- Hiển thị số lượng -->
        <TextBlock Text="{Binding RecordCount, 
            StringFormat='Số lượng nhân viên: {0}'}"/>
        
        <!-- Nút Hủy/Xuất -->
        <Button Command="{Binding CancelCommand}"/>
        <Button Command="{Binding ExportCommand}"/>
    </Grid>
</Window>
```

---

#### D. File: `EmployeeView.xaml`

**Thêm nút xuất file vào header:**

```xaml
<Grid Grid.Row="0" Background="{DynamicResource PrimaryHueMidBrush}">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="Auto"/> <!-- ✅ NÚT XUẤT -->
        <ColumnDefinition Width="Auto"/> <!-- Nút Refresh -->
    </Grid.ColumnDefinitions>

    <!-- Nút xuất file -->
    <Button Grid.Column="2"
            Command="{Binding ExportAllEmployeesCommand}"
            ToolTip="Xuất toàn bộ danh sách nhân viên">
        <materialDesign:PackIcon Kind="FileExport" 
                               Width="24" Height="24"
                               Foreground="White"/>
    </Button>
</Grid>
```

---

## Kiến trúc Clean được đảm bảo

### Layer Separation
```
┌─────────────────────────────────────────┐
│  Presentation Layer (WPF)               │
│  - AttendanceManagementViewModel        │
│  - EmployeeViewModel                    │
│  - ExportEmployeeViewModel              │
│  - ExportConfigurationViewModel         │
└─────────────────┬───────────────────────┘
                  │
                  │ Sử dụng
                  ▼
┌─────────────────────────────────────────┐
│  Application Layer (Services)           │
│  - IDeviceService                       │
│  - IAttendanceService                   │
│  - IDialogService                       │
│  - INotificationService                 │
└─────────────────┬───────────────────────┘
                  │
                  │ Trả về
                  ▼
┌─────────────────────────────────────────┐
│  Core Layer (DTOs)                      │
│  - AttendanceDisplayDto                 │
│  - EmployeeDto                          │
│  - EmployeeDisplayModel                 │
└─────────────────────────────────────────┘
```

### Data Flow (Quản lý Phân Công)
```
User Action
    ↓
[Nút Xuất File] → AttendanceManagementViewModel.OpenExportDialogAsync()
    ↓
[Kiểm tra] → AttendanceRecords (dữ liệu đã lọc)
    ↓
[Tạo Dialog] → ExportConfigurationDialog
    ↓
[Bind ViewModel] → ExportConfigurationViewModel(_data = AttendanceRecords.ToList())
    ↓
[User chọn format] → JSON/Excel/CSV/Text
    ↓
[SaveFileDialog] → Chọn đường dẫn
    ↓
[Export] → ExportToJson/Excel/Csv/Text(_data)
    ↓
[File được tạo] → Thông báo thành công
```

### Data Flow (Quản lý Nhân Viên)
```
User Action
    ↓
[Nút Xuất File] → EmployeeViewModel.ExportAllEmployeesAsync()
    ↓
[Gọi Service] → _deviceService.GetBasicUsersAsync()
    ↓
[Nhận dữ liệu] → List<EmployeeDto> allUsers (TOÀN BỘ từ thiết bị)
    ↓
[Transform] → List<EmployeeDisplayModel> với Index tự động
    ↓
[Tạo Dialog] → ExportEmployeeDialog
    ↓
[Bind ViewModel] → ExportEmployeeViewModel(_data = employeeList)
    ↓
[User chọn format] → JSON/Excel/CSV/Text
    ↓
[SaveFileDialog] → Chọn đường dẫn
    ↓
[Export] → ExportToJson/Excel/Csv/Text(_data)
    ↓
[File được tạo] → Thông báo thành công
```

---

## Đảm bảo KHÔNG Fallback về dữ liệu mẫu

### 1. Quản lý Phân Công
- ✅ **KHÔNG** tạo dữ liệu mẫu
- ✅ Kiểm tra `AttendanceRecords.Count == 0` → Hiển thị cảnh báo
- ✅ Chỉ xuất khi có dữ liệu THẬT từ thiết bị
- ✅ Dữ liệu = kết quả sau khi áp dụng filter

### 2. Quản lý Nhân Viên
- ✅ **KHÔNG** tạo dữ liệu mẫu
- ✅ Gọi `_deviceService.GetBasicUsersAsync()` trực tiếp
- ✅ Kiểm tra `allUsers == null || !allUsers.Any()` → Hiển thị cảnh báo
- ✅ Chỉ xuất khi có dữ liệu THẬT từ thiết bị
- ✅ Lấy **TOÀN BỘ** nhân viên (không giới hạn bởi phân trang)

### Validation Flow
```csharp
// Attendance Management
if (AttendanceRecords == null || AttendanceRecords.Count == 0)
{
    await _notificationService.ShowWarningAsync(...);
    return; // ✅ KHÔNG xuất, KHÔNG tạo dữ liệu mẫu
}

// Employee Management
var allUsers = await _deviceService.GetBasicUsersAsync();
if (allUsers == null || !allUsers.Any())
{
    ShowWarningMessage("Không có nhân viên nào để xuất");
    return; // ✅ KHÔNG xuất, KHÔNG tạo dữ liệu mẫu
}
```

---

## Hướng dẫn sử dụng

### Xuất dữ liệu Chấm Công
1. Vào **Quản lý phân công**
2. Áp dụng filter (ngày, giờ) nếu muốn
3. Click **"Tải dữ liệu"** để lấy dữ liệu từ thiết bị
4. Click nút **"Xuất file"**
5. Chọn format: JSON, Excel, CSV, hoặc Text
6. Nhập tên file
7. Click **"XUẤT FILE"**
8. Chọn đường dẫn lưu → Hoàn tất

### Xuất danh sách Nhân Viên
1. Vào **Quản lý nhân viên**
2. Click nút **"Xuất file"** (icon FileExport ở header)
3. Chọn format: JSON, Excel, CSV, hoặc Text
4. Nhập tên file (mặc định: `employees_yyyy-MM-dd`)
5. Click **"XUẤT FILE"**
6. Chọn đường dẫn lưu → Hoàn tất

**Lưu ý:** Xuất nhân viên sẽ lấy **TOÀN BỘ** danh sách (không bị giới hạn bởi phân trang)

---

## Format xuất file

### 1. JSON
```json
[
  {
    "Index": 1,
    "DIN": "123456",
    "UserName": "Nguyễn Văn A",
    "Department": "Kinh doanh"
  },
  ...
]
```

### 2. CSV
```csv
STT,DIN,Họ tên,Phòng ban
1,123456,Nguyễn Văn A,Kinh doanh
2,123457,Trần Thị B,Kế toán
```

### 3. Text
```
DANH SÁCH NHÂN VIÊN
Ngày xuất: 15/10/2025 14:30:00
================================================================================

STT   DIN             Họ tên                        Phòng ban                
--------------------------------------------------------------------------------
1     123456          Nguyễn Văn A                  Kinh doanh               
2     123457          Trần Thị B                    Kế toán                  

================================================================================
Tổng số: 2 nhân viên
```

---

## Testing Checklist

### Quản lý Phân Công
- [ ] Không có dữ liệu → Hiển thị cảnh báo "Không có dữ liệu để xuất"
- [ ] Có dữ liệu → Mở dialog xuất file
- [ ] Xuất JSON → File đúng format, có tiếng Việt
- [ ] Xuất CSV → File đúng format, Excel mở được
- [ ] Xuất Text → File format đẹp, dễ đọc
- [ ] Số lượng hiển thị = số lượng bản ghi thật

### Quản lý Nhân Viên
- [ ] Không kết nối thiết bị → Hiển thị cảnh báo
- [ ] Thiết bị không có nhân viên → Hiển thị cảnh báo
- [ ] Có nhân viên → Mở dialog xuất file
- [ ] Xuất JSON → File đúng format, có tiếng Việt
- [ ] Xuất CSV → File đúng format, Excel mở được
- [ ] Xuất Text → File format đẹp, có STT
- [ ] Số lượng = tổng số nhân viên trên thiết bị (không bị giới hạn 10/trang)

---

## Files đã thay đổi

### Modified (2)
1. `BHK.Retrieval.Attendance.WPF/ViewModels/AttendanceManagementViewModel.cs`
   - Cập nhật `OpenExportDialogAsync()` với logging và validation

2. `BHK.Retrieval.Attendance.WPF/ViewModels/EmployeeViewModel.cs`
   - Thêm `ExportAllEmployeesCommand`
   - Thêm method `ExportAllEmployeesAsync()`
   - Thêm helper `ShowSuccessMessage()`

### Created (2)
3. `BHK.Retrieval.Attendance.WPF/ViewModels/Dialogs/ExportEmployeeViewModel.cs`
   - ViewModel mới cho xuất nhân viên
   - Hỗ trợ 4 format: JSON, Excel, CSV, Text
   - CSV escape dấu phẩy, Text format đẹp

4. `BHK.Retrieval.Attendance.WPF/Views/Dialogs/ExportEmployeeDialog.xaml`
   - Đơn giản hóa từ version cũ
   - UI giống ExportConfigurationDialog
   - ComboBox chọn format, TextBox tên file

### Modified (UI)
5. `BHK.Retrieval.Attendance.WPF/Views/Pages/EmployeeView.xaml`
   - Thêm nút xuất file vào header
   - Icon: FileExport
   - Tooltip: "Xuất toàn bộ danh sách nhân viên"

---

## Kết luận

✅ **Đảm bảo Clean Architecture:**
- Presentation Layer: ViewModels, Views
- Application Layer: Services (IDeviceService, IAttendanceService)
- Core Layer: DTOs (AttendanceDisplayDto, EmployeeDto, EmployeeDisplayModel)

✅ **Đảm bảo dữ liệu THẬT:**
- Quản lý Phân Công: `AttendanceRecords` (dữ liệu sau filter)
- Quản lý Nhân Viên: `_deviceService.GetBasicUsersAsync()` (toàn bộ từ thiết bị)

✅ **KHÔNG fallback về dữ liệu mẫu:**
- Kiểm tra null/empty → Hiển thị cảnh báo → Return
- Không tạo dữ liệu test/mock/sample

✅ **Hỗ trợ đa format:**
- JSON (UTF-8, pretty print)
- Excel (.xlsx - tạm dùng CSV)
- CSV (escape comma, tiếng Việt)
- Text (format table đẹp)

✅ **Logging đầy đủ:**
- Log số lượng bản ghi xuất
- Log thành công/thất bại
- Log warning khi không có dữ liệu

✅ **UX tốt:**
- Loading indicator
- Thông báo thành công với số lượng
- Cảnh báo rõ ràng khi không có dữ liệu
- Default filename có timestamp
