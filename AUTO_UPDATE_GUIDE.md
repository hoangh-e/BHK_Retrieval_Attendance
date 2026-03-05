# AUTO-UPDATE FEATURE - IMPLEMENTATION GUIDE

## 📦 Tổng quan

Tính năng auto-update đã được triển khai cho ứng dụng BHK Retrieval Attendance System với các đặc điểm:

- ✅ Kiểm tra phiên bản mới từ GitHub Pages
- ✅ Download MSI installer tự động
- ✅ Không block UI thread
- ✅ Xử lý lỗi network gracefully
- ✅ Tích hợp với architecture hiện tại (Clean Architecture + MVVM)

---

## 📁 Files đã tạo mới

### 1. Core Layer (Domain Models & Interfaces)

**`Core/Models/UpdateInfo.cs`**
- Model chứa thông tin phiên bản mới
- Phương thức `IsNewerThan()` để so sánh version

**`Core/Interfaces/IUpdateService.cs`**
- Interface định nghĩa service kiểm tra cập nhật
- 2 methods: `CheckForUpdateAsync()` và `DownloadAndInstallUpdateAsync()`

### 2. Infrastructure Layer (Implementation)

**`Infrastructure/Services/UpdateService.cs`**
- Implementation của IUpdateService
- Sử dụng HttpClient để download version.json và MSI
- Launch MSI bằng msiexec.exe
- Hỗ trợ progress tracking khi download

### 3. WPF Layer (UI)

**`WPF/ViewModels/Dialogs/UpdateDialogViewModel.cs`**
- ViewModel cho dialog cập nhật
- Commands: UpdateNow, UpdateLater
- Properties: NewVersion, ReleaseNotes, DownloadProgress

**`WPF/Views/Dialogs/UpdateDialog.xaml`**
- Material Design UI cho dialog thông báo cập nhật
- Hiển thị release notes
- Progress bar khi download

**`WPF/Views/Dialogs/UpdateDialog.xaml.cs`**
- Code-behind (minimal logic)

---

## 🔧 Files đã chỉnh sửa

### 1. **ServiceRegistrar.cs**

Đã thêm:
```csharp
// HttpClient for UpdateService
services.AddHttpClient();

// Update Service - Singleton
services.AddSingleton<IUpdateService, UpdateService>();

// Update Dialog ViewModel
services.AddTransient<ViewModels.Dialogs.UpdateDialogViewModel>();
```

### 2. **App.xaml.cs**

Đã thêm:
```csharp
// Trong OnStartup
_ = CheckForUpdatesAsync();

// Method mới
private async Task CheckForUpdatesAsync()
{
    // Đợi 3 giây để app khởi động
    // Kiểm tra version.json
    // Hiển thị UpdateDialog nếu có phiên bản mới
}
```

---

## 🎯 Cách hoạt động

### Flow tổng quan:

```
1. App khởi động (App.xaml.cs OnStartup)
   ↓
2. Đợi 3 giây (để UI load hoàn toàn)
   ↓
3. UpdateService.CheckForUpdateAsync()
   ↓
4. HTTP GET: https://hoangh-e.github.io/realand-app-update/version.json
   ↓
5. So sánh version:
   - Nếu có phiên bản mới → Hiển thị UpdateDialog
   - Nếu không → Silent (chỉ log)
   ↓
6. User click "CẬP NHẬT NGAY":
   ↓
7. Download MSI với progress tracking
   ↓
8. Launch: msiexec.exe /i "MSI_PATH" /passive
   ↓
9. Đóng ứng dụng để cài đặt upgrade
```

---

## 🧪 Cách test

### Test 1: Kiểm tra kết nối version.json

1. Build project (Release mode)
2. Chạy ứng dụng
3. Đợi 3 giây
4. Check log file trong `Logs/` → phải thấy:
   ```
   [INFO] Checking for application updates...
   [INFO] Application is up to date
   ```

### Test 2: Giả lập có phiên bản mới

**Cách 1: Sửa version trong .csproj**

File: `BHK.Retrieval.Attendance.WPF.csproj`
```xml
<Version>0.9.0</Version>  <!-- Giảm xuống thấp hơn version.json -->
```

**Cách 2: Tạo version.json local**

1. Tạo file test: `C:\Temp\version.json`
   ```json
   {
     "version": "2.0.0",
     "url": "https://example.com/test.msi",
     "notes": "Test update notes\n- Feature 1\n- Feature 2"
   }
   ```

2. Sửa `UpdateService.cs`:
   ```csharp
   // Thay đổi tạm thời URL
   private const string UPDATE_URL = "file:///C:/Temp/version.json";
   ```

3. Build và chạy → Dialog sẽ hiện

### Test 3: Test download MSI (Production)

**Yêu cầu:**
- Cần có MSI thật tại GitHub Release
- Version trong version.json phải cao hơn app hiện tại

**Bước test:**
1. Upload MSI lên GitHub Release (ví dụ: v1.0.1)
2. Update `version.json` trên GitHub Pages:
   ```json
   {
     "version": "1.0.1",
     "url": "https://github.com/hoangh-e/realand-app-update/releases/download/v1.0.1/BHK_Realand_App.msi",
     "notes": "### Improvements\n- Fixed bug XYZ\n- Added feature ABC"
   }
   ```
3. Chạy app version 1.0.0
4. Dialog hiện → Click "CẬP NHẬT NGAY"
5. Progress bar hiển thị
6. MSI tự động chạy
7. App đóng

---

## 🔒 Security & Error Handling

### 1. Network Errors

- Timeout: 30 giây
- Lỗi network → Silent (log only), không làm phiền user
- Không crash app nếu version.json không tồn tại

### 2. MSI Download

- Tải vào `%TEMP%\BHK_Attendance_Update\`
- Progress tracking để user biết tiến độ
- Nếu download fail → Hiển thị lỗi trong dialog

### 3. MSI Install

- Sử dụng `msiexec.exe /i "path" /passive`
- `/passive`: Hiển thị progress bar, không cần tương tác
- Tự động request quyền admin (`Verb = "runas"`)

### 4. Upgrade Process

- MSI phải có `RemovePreviousVersions = true` (đã có trong .vdproj)
- App tự động đóng sau khi launch installer
- MSI sẽ uninstall version cũ → install version mới

---

## 📝 Cấu hình

### Thay đổi URL update server

File: `Infrastructure/Services/UpdateService.cs`

```csharp
private const string UPDATE_URL = "YOUR_UPDATE_SERVER/version.json";
```

### Thay đổi thời gian đợi trước khi check update

File: `App.xaml.cs` → Method `CheckForUpdatesAsync()`

```csharp
await Task.Delay(3000); // Đổi 3000 thành ms khác
```

### Vô hiệu hóa auto-update

File: `App.xaml.cs` → Method `OnStartup()`

```csharp
// Comment dòng này
// _ = CheckForUpdatesAsync();
```

---

## 🐛 Troubleshooting

### Lỗi: InitializeComponent not found

**Nguyên nhân:** XAML chưa compile

**Giải pháp:** 
```powershell
# Clean và rebuild
dotnet clean
dotnet build
```

### Lỗi: HttpClient not registered

**Nguyên nhân:** Thiếu HttpClientFactory

**Giải pháp:** Đã thêm `services.AddHttpClient()` trong ServiceRegistrar

### Dialog không hiện dù có version mới

**Check list:**
1. Version trong `.csproj` có thấp hơn `version.json` không?
2. `version.json` có format đúng không?
3. Check log file → có message "Update available" không?
4. App có quyền show dialog không? (UAC)

### MSI không chạy

**Nguyên nhân:** Thiếu quyền admin

**Giải pháp:**
- Chạy app với quyền admin
- Hoặc build MSI với elevation requirement

---

## 🚀 Deployment Checklist

### Khi release version mới:

1. ✅ Build MSI với version mới (ví dụ: 1.0.1)
2. ✅ Upload MSI lên GitHub Release
3. ✅ Update `version.json` trên GitHub Pages:
   ```json
   {
     "version": "1.0.1",
     "url": "https://github.com/USER/REPO/releases/download/v1.0.1/BHK_Realand_App.msi",
     "notes": "Release notes here"
   }
   ```
4. ✅ Test download từ version cũ
5. ✅ Verify upgrade thành công

---

## 📊 Version Format

**Hỗ trợ:** Semantic Versioning (SemVer)

```
MAJOR.MINOR.PATCH
1.0.0
1.0.1
1.1.0
2.0.0
```

**So sánh:**
- 1.0.1 > 1.0.0 ✅
- 1.1.0 > 1.0.9 ✅
- 2.0.0 > 1.9.9 ✅

---

## 💡 Recommendations

### 1. Thêm release notes formatting

Current: Plain text  
Recommended: Markdown support

File: `UpdateDialog.xaml`
```xaml
<!-- Thay TextBlock bằng RichTextBox hoặc WebView -->
```

### 2. Thêm "Skip this version"

File: `UpdateDialogViewModel.cs`
```csharp
[RelayCommand]
private void SkipVersion()
{
    // Lưu version vào settings
    // Không hiện lại cho version này
}
```

### 3. Thêm auto-check interval

Example: Check mỗi 24h thay vì mỗi khi startup

File: `App.xaml.cs`
```csharp
// Lưu last check time vào settings
// Chỉ check nếu > 24h
```

### 4. Thêm rollback mechanism

- Backup version cũ trước khi upgrade
- Nút "Restore previous version" nếu có lỗi

---

## 📞 Support

Nếu có vấn đề:

1. Check log files trong `Logs/app-*.log`
2. Search for keywords: "update", "download", "install"
3. Report log snippet cùng error message

---

**Implementation Date:** March 6, 2026  
**Author:** AI Assistant  
**Version:** 1.0
