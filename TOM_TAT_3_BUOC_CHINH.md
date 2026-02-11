# ⚡ TÓM TẮT: Đóng Gói và Build App WPF

## 🎯 Mục Tiêu
Hướng dẫn đóng gói và build ứng dụng WPF BHK Retrieval Attendance System để phát hành cho người dùng.

---

## 📋 CHECKLIST 3 BƯỚC CHÍNH

### ✅ BƯỚC 1: Cập Nhật Thông Tin App (5 phút)

**File cần chỉnh sửa:** `BHK.Retrieval.Attendance.WPF/BHK.Retrieval.Attendance.WPF.csproj`

Thêm vào `<PropertyGroup>`:
```xml
<!-- THÔNG TIN ỨNG DỤNG -->
<AssemblyName>BHK.Retrieval.Attendance</AssemblyName>
<Product>BHK Retrieval Attendance System</Product>
<Version>1.0.0</Version>

<!-- THÔNG TIN TÁC GIẢ -->
<Company>BHK AI Development Team</Company>
<Authors>BHK AI Development Team</Authors>
<Copyright>Copyright © 2025 BHK AI Development Team. All rights reserved.</Copyright>

<!-- ICON APP -->
<ApplicationIcon>Resources\Images\Icons\app-icon.ico</ApplicationIcon>
```

---

### ✅ BƯỚC 2: Thêm Icon App (3 phút)

**1. Chuẩn bị icon:**
- Format: `.ico`
- Tạo tại: https://icoconvert.com/

**2. Đặt icon vào đúng vị trí:**
```
BHK.Retrieval.Attendance.WPF/
└── Resources/
    └── Images/
        └── Icons/
            └── app-icon.ico  ← Đặt file tại đây
```

**3. Include icon trong project:**
Thêm vào `.csproj`:
```xml
<ItemGroup>
    <Resource Include="Resources\Images\Icons\app-icon.ico" />
</ItemGroup>
```

**4. Cập nhật MainWindow.xaml:**
```xml
<Window Icon="/Resources/Images/Icons/app-icon.ico" ...>
```

---

### ✅ BƯỚC 3: Build và Publish (10 phút)

**Cách 1: Sử dụng PowerShell Script (Khuyến nghị)**
```powershell
# Chạy script tự động
.\Build.ps1 -CreateZip -CreateInstaller
```

**Cách 2: Build Thủ Công**
```bash
# Build Release
dotnet build --configuration Release

# Publish Self-Contained
dotnet publish BHK.Retrieval.Attendance.WPF/BHK.Retrieval.Attendance.WPF.csproj `
    --configuration Release `
    --output ./publish `
    --runtime win-x64 `
    --self-contained true `
    --property:PublishSingleFile=true
```

---

## 📂 CẤU TRÚC OUTPUT

Sau khi build xong, bạn sẽ có:

```
publish/
├── app/                          # Ứng dụng đã publish
│   ├── BHK.Retrieval.Attendance.exe
│   ├── *.dll
│   ├── appsettings.json
│   ├── Resources/
│   └── README.txt
│
├── zip/                          # File ZIP để phát hành
│   └── BHK_Attendance_v1.0.0_win-x64.zip
│
└── setup/                        # Installer (nếu tạo)
    └── BHK_Attendance_Setup_v1.0.0.exe
```

---

## 🚀 PHÁT HÀNH

### Option 1: Phát hành qua GitHub Releases
1. Tạo tag version: `git tag -a v1.0.0 -m "Release v1.0.0"`
2. Push tag: `git push origin v1.0.0`
3. Tạo Release trên GitHub
4. Upload các file ZIP và Setup

### Option 2: Phát hành qua Google Drive/OneDrive
1. Upload file ZIP hoặc Setup
2. Chia sẻ link với người dùng
3. Kèm theo file README.txt

### Option 3: Phát hành qua Internal Server
1. Copy file lên server
2. Cấu hình download link
3. Thông báo cho team

---

## 🔍 KIỂM TRA TRƯỚC KHI PHÁT HÀNH

### ✅ Checklist Bắt Buộc
- [ ] Icon hiển thị đúng (EXE, taskbar, window)
- [ ] Thông tin tác giả chính xác (Properties → Details)
- [ ] Version number đúng
- [ ] App chạy được trên máy clean (không cài dev tools)
- [ ] Kết nối database thành công
- [ ] Kết nối thiết bị hoạt động
- [ ] Tất cả tính năng hoạt động
- [ ] README.txt đầy đủ thông tin

### 📝 File Cần Kèm Theo
- [ ] BHK_Attendance_vX.X.X.zip hoặc Setup.exe
- [ ] README.txt (hướng dẫn cài đặt và sử dụng)
- [ ] CHANGELOG.txt (lịch sử thay đổi)
- [ ] LICENSE.txt (giấy phép sử dụng)

---

## 📄 CÁC FILE HƯỚNG DẪN CHI TIẾT

Tham khảo các file sau để biết thêm chi tiết:

1. **HUONG_DAN_DONG_GOI_VA_BUILD_APP.md**
   - Hướng dẫn chi tiết đầy đủ
   - 10 bước từ A-Z
   - Khắc phục sự cố

2. **QUICK_GUIDE_ICON_VA_TAC_GIA.md**
   - Hướng dẫn nhanh cập nhật icon
   - Cập nhật thông tin tác giả
   - Checklist đơn giản

3. **Build.ps1**
   - PowerShell script tự động hóa
   - Build, Publish, ZIP, Installer
   - Tiết kiệm thời gian

4. **BHK_Attendance_Setup.iss**
   - Inno Setup script
   - Tạo installer chuyên nghiệp
   - Tùy chỉnh theo nhu cầu

5. **README.txt**
   - File hướng dẫn cho người dùng
   - Kèm theo khi phát hành
   - Đầy đủ thông tin

6. **CHANGELOG.txt**
   - Lịch sử thay đổi
   - Ghi chú phiên bản
   - Kế hoạch tương lai

---

## 💡 TIPS & TRICKS

### 🎯 Tips Build Nhanh
```powershell
# Build & ZIP trong 1 lệnh
.\Build.ps1 -SingleFile -CreateZip

# Build và tự động mở folder output
.\Build.ps1 -CreateZip
# Sau đó chọn Y khi được hỏi
```

### 🔧 Tips Giảm Dung Lượng
```powershell
# Framework-Dependent (cần .NET Runtime trên máy user)
.\Build.ps1 -FrameworkDependent -CreateZip
# Kích thước: ~50MB thay vì ~150MB
```

### 🚀 Tips Build Nhanh Cho Test
```bash
# Chỉ build, không publish
dotnet build --configuration Release

# Test ngay từ bin folder
.\BHK.Retrieval.Attendance.WPF\bin\Release\net8.0-windows\BHK.Retrieval.Attendance.exe
```

---

## 🆘 KHẮC PHỤC SỰ CỐ NHANH

### ❌ Lỗi: Icon không hiển thị
**Nguyên nhân:** File icon không đúng vị trí hoặc không được include

**Giải pháp:**
1. Kiểm tra file tồn tại: `Resources\Images\Icons\app-icon.ico`
2. Kiểm tra `.csproj` có `<ApplicationIcon>` và `<Resource Include>`
3. Rebuild project: `dotnet clean && dotnet build`

### ❌ Lỗi: Thông tin tác giả không hiển thị
**Nguyên nhân:** Chưa cập nhật `.csproj`

**Giải pháp:**
1. Mở `.csproj`
2. Thêm `<Company>`, `<Authors>`, `<Copyright>` vào `<PropertyGroup>`
3. Rebuild

### ❌ Lỗi: App không chạy trên máy khác
**Nguyên nhân:** Thiếu .NET Runtime hoặc dependencies

**Giải pháp:**
1. Build lại với `--self-contained true`
2. Hoặc yêu cầu user cài .NET 8.0 Runtime

---

## 📞 HỖ TRỢ

**BHK AI Development Team**
- Lead Developer: Trịnh Việt Hoàng
- Email: support@bhk-ai.com

---

## 🎉 HOÀN THÀNH!

Sau khi hoàn tất 3 bước trên, bạn đã có:
✅ File EXE với icon đẹp
✅ Thông tin tác giả chuyên nghiệp
✅ Package sẵn sàng phát hành
✅ Documentation đầy đủ

**Chúc mừng bạn đã hoàn thành việc đóng gói app!** 🎊

---

_Developed by: BHK AI Development Team_  
_Lead Developer: Trịnh Việt Hoàng_  
_Version: 1.0.0_
