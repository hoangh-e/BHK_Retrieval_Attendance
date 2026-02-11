# 🏢 BHK Retrieval Attendance System

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-orange)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![MVVM](https://img.shields.io/badge/Pattern-MVVM-purple)](https://docs.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

Hệ thống quản lý chấm công tích hợp với thiết bị vân tay Realand và SharePoint.

---

## 📋 **MỤC LỤC**

- [Tổng Quan](#-tổng-quan)
- [Tính Năng](#-tính-năng)
- [Kiến Trúc](#-kiến-trúc)
- [Công Nghệ](#-công-nghệ)
- [Cấu Trúc Project](#-cấu-trúc-project)
- [Bắt Đầu](#-bắt-đầu)
- [Tài Liệu](#-tài-liệu)

---

## 🎯 **TỔNG QUAN**

**BHK Retrieval Attendance** là hệ thống quản lý chấm công hiện đại được xây dựng trên nền tảng .NET 8.0 với WPF, áp dụng Clean Architecture và MVVM pattern.

### **Mục Tiêu Chính:**
- ✅ Quản lý chấm công từ thiết bị vân tay (Realand/ZKTeco)
- ✅ Tích hợp với SharePoint Online
- ✅ Báo cáo và export dữ liệu (Excel, PDF)
- ✅ Giao diện đẹp mắt với Material Design
- ✅ Kiến trúc dễ bảo trì và mở rộng

---

## ✨ **TÍNH NĂNG**

### **1. Quản Lý Nhân Viên**
- 👤 Thêm/Sửa/Xóa thông tin nhân viên
- 📸 Quản lý ảnh và thông tin sinh trắc học
- 🏢 Phân loại theo phòng ban
- 📊 Theo dõi trạng thái làm việc

### **2. Chấm Công**
- 🔌 Kết nối với thiết bị vân tay (Realand, ZKTeco)
- 📥 Đồng bộ dữ liệu tự động/thủ công
- ⏰ Theo dõi giờ vào/ra
- 📅 Lịch sử chấm công chi tiết

### **3. Báo Cáo**
- 📊 Dashboard thống kê tổng quan
- 📈 Báo cáo theo ngày/tuần/tháng
- 📄 Export Excel, PDF, CSV
- 📧 Gửi báo cáo tự động qua email

### **4. Tích Hợp SharePoint**
- ☁️ Đồng bộ dữ liệu với SharePoint Online
- 📁 Lưu trữ tài liệu trên cloud
- 👥 Quản lý người dùng từ SharePoint

### **5. Cấu Hình & Quản Trị**
- ⚙️ Cấu hình kết nối thiết bị
- 🎨 Tùy chỉnh giao diện (Light/Dark theme)
- 🌍 Hỗ trợ đa ngôn ngữ (EN, VI, ZH)
- 📝 Logging & Audit trail

---

## 🏗️ **KIẾN TRÚC**

Project được xây dựng theo **Clean Architecture** với 4 layers chính:

```
┌─────────────────────────────────────────┐
│    Presentation (WPF)                   │  ← UI Layer
├─────────────────────────────────────────┤
│    Core (Business Logic)                │  ← Domain Layer
├─────────────────────────────────────────┤
│    Infrastructure (Data & External)     │  ← Data Layer
├─────────────────────────────────────────┤
│    Shared (Cross-cutting)               │  ← Utilities
└─────────────────────────────────────────┘
```

**Chi tiết:** Xem [ARCHITECTURE_DIAGRAM.md](./ARCHITECTURE_DIAGRAM.md)

---

## 🛠️ **CÔNG NGHỆ**

### **Frontend (WPF)**
- .NET 8.0 WPF
- Material Design In XAML
- CommunityToolkit.Mvvm (MVVM)
- Microsoft.Xaml.Behaviors

### **Backend & Data**
- Entity Framework Core 8.0
- SQL Server
- AutoMapper
- FluentValidation
- MediatR (CQRS)

### **External Integrations**
- Riss.Devices.dll (Realand SDK)
- PnP.Framework (SharePoint)
- MailKit (Email)
- EPPlus (Excel Export)

### **Logging & Configuration**
- Serilog
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.DependencyInjection

---

## 📁 **CẤU TRÚC PROJECT**

```
BHK_Retrieval_Attendance/
├── 📁 BHK.Retrieval.Attendance.WPF/          # Main WPF App (38 folders)
├── 📁 BHK.Retrieval.Attendance.Core/         # Business Logic (18 folders)
├── 📁 BHK.Retrieval.Attendance.Infrastructure/ # Data & External (17 folders)
├── 📁 BHK.Retrieval.Attendance.Shared/       # Shared Utilities (11 folders)
├── 📁 tests/                                 # Tests (16 folders)
├── 📁 docs/                                  # Documentation (4 folders)
├── 📁 assets/                                # Resources (7 folders)
├── 📁 build/                                 # Build outputs
└── 📁 packages/                              # Local packages
```

**Tổng cộng: 113 folders**

**Chi tiết:** Xem [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE.md)

---

## 🚀 **BẮT ĐẦU**

### **Yêu Cầu Hệ Thống**

- Windows 10/11
- .NET 8.0 SDK
- Visual Studio 2022 (hoặc VS Code)
- SQL Server 2019+
- Thiết bị chấm công Realand (tùy chọn)

### **Cài Đặt**

1. **Clone repository**
   ```bash
   git clone https://github.com/hoangh-e/BHK_Retrieval_Attendance.git
   cd BHK_Retrieval_Attendance
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Cấu hình Database**
   ```bash
   # Cập nhật connection string trong appsettings.json
   # Chạy migrations
   dotnet ef database update --project BHK.Retrieval.Attendance.Infrastructure
   ```

4. **Build & Run**
   ```bash
   dotnet build
   dotnet run --project BHK.Retrieval.Attendance.WPF
   ```

### **Cấu Hình**

Chỉnh sửa `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BHK_Attendance;..."
  },
  "DeviceSettings": {
    "IPAddress": "192.168.1.100",
    "Port": 4370
  },
  "SharePointSettings": {
    "SiteUrl": "https://yourcompany.sharepoint.com",
    "ClientId": "your-client-id"
  }
}
```

---

## 📚 **TÀI LIỆU**

### **Tài Liệu Kỹ Thuật**
- [Kiến Trúc Hệ Thống](./docs/Technical/Architecture.md)
- [Database Schema](./docs/Technical/DatabaseSchema.md)
- [Device Integration](./docs/Technical/DeviceIntegration.md)
- [SharePoint Integration](./docs/Technical/SharePointIntegration.md)

### **Tài Liệu Người Dùng**
- [Hướng Dẫn Sử Dụng](./docs/User/UserManual.pdf)
- [Cài Đặt Hệ Thống](./docs/User/InstallationGuide.md)
- [Quick Start Guide](./docs/User/QuickStartGuide.md)

### **Tài Liệu Phát Triển**
- [Coding Standards](./docs/Development/CodingStandards.md)
- [Setup Dev Environment](./docs/Development/SetupDevelopmentEnvironment.md)
- [Contribution Guide](./docs/Development/ContributionGuide.md)

---

## 🧪 **TESTING**

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/BHK.Retrieval.Attendance.Core.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 📦 **BUILD & DEPLOYMENT**

### **Build Release**
```bash
dotnet publish -c Release -o ./publish
```

### **Create Installer**
```bash
# Sử dụng WiX Toolset hoặc Inno Setup
# Chi tiết xem docs/Technical/DeploymentGuide.md
```

---

## 📊 **TRẠNG THÁI PROJECT**

| Component | Trạng Thái | Hoàn Thành |
|-----------|-----------|-----------|
| Cấu trúc folders | ✅ Hoàn thành | 100% |
| NuGet packages | ✅ Đã cài | 100% |
| WPF Project setup | ⏳ Đang làm | 40% |
| Core Business Logic | ⏳ Chưa bắt đầu | 0% |
| Infrastructure | ⏳ Chưa bắt đầu | 0% |
| Tests | ⏳ Chưa bắt đầu | 0% |
| Documentation | ⏳ Đang làm | 30% |

---

## 🤝 **CONTRIBUTING**

Contributions are welcome! Vui lòng đọc [Contribution Guide](./docs/Development/ContributionGuide.md) trước khi bắt đầu.

---

## 📄 **LICENSE**

Xem file [LICENSE](./LICENSE) để biết thêm chi tiết.

---

## 👥 **TEAM**

- **Developer:** hoangh-e
- **Project:** BHK Attendance System
- **Created:** October 2025

---

## 📞 **LIÊN HỆ & HỖ TRỢ**

- **GitHub Issues:** [Create Issue](https://github.com/hoangh-e/BHK_Retrieval_Attendance/issues)
- **Documentation:** [Wiki](https://github.com/hoangh-e/BHK_Retrieval_Attendance/wiki)

---

## 🎉 **CHANGELOG**

### **v0.1.0** (October 8, 2025)
- ✅ Tạo cấu trúc project (113 folders)
- ✅ Setup NuGet packages (23 packages)
- ✅ Build thành công initial project
- ✅ Tạo tài liệu kiến trúc

---

<div align="center">

**Built with using .NET 8.0 & WPF**

[⬆ Back to top](#-bhk-retrieval-attendance-system)

</div>
