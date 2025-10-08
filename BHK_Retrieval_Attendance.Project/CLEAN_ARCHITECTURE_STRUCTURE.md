# ✅ Clean Architecture - Cấu Trúc Đã Cập Nhật

## 📁 Cấu Trúc Mới

```
BHK_Retrieval_Attendance.Project/
├── 📄 BHK_Retrieval_Attendance.Project.sln        # Solution file
│
├── 📁 BHK.Retrieval.Attendance.WPF/               # ⭐ MAIN WPF APPLICATION (Presentation Layer)
│   ├── ViewModels/                                # MVVM ViewModels
│   ├── Views/                                     # XAML Views
│   ├── Models/                                    # UI Models
│   ├── Services/                                  # UI Services (Dialog, Navigation)
│   ├── Commands/                                  # Custom Commands
│   ├── Converters/                                # Value Converters
│   ├── Behaviors/                                 # XAML Behaviors
│   ├── Resources/                                 # Styles, Templates, Images
│   ├── Configuration/                             # DI, Mapping, Validation
│   ├── Utilities/                                 # WPF Helpers
│   ├── App.xaml / App.xaml.cs                    # Application Entry Point
│   └── BHK.Retrieval.Attendance.WPF.csproj       # ⭐ Main Project File
│
├── 📁 BHK.Retrieval.Attendance.Core/              # CORE LAYER (Domain + Use Cases)
│   ├── Domain/                                    # Entities, Value Objects, Enums
│   ├── UseCases/                                  # Application Use Cases
│   ├── Interfaces/                                # Repository & Service Contracts
│   ├── Contracts/                                 # DTOs, Requests, Responses
│   └── BHK.Retrieval.Attendance.Core.csproj      # Core Project File
│
├── 📁 BHK.Retrieval.Attendance.Infrastructure/    # INFRASTRUCTURE LAYER
│   ├── Devices/                                   # Device Integration (Realand, ZKTeco)
│   ├── SharePoint/                                # SharePoint Integration
│   ├── Persistence/                               # EF Core, Repositories
│   ├── FileStorage/                               # File Export/Import
│   ├── Configuration/                             # Infrastructure Setup
│   ├── External/                                  # Email, SMS, AD
│   └── BHK.Retrieval.Attendance.Infrastructure.csproj
│
└── 📁 BHK.Retrieval.Attendance.Shared/            # SHARED LAYER
    ├── Extensions/                                # Extension Methods
    ├── Results/                                   # Result Pattern
    ├── Options/                                   # Configuration POCOs
    ├── Logging/                                   # Logging Utilities
    ├── Constants/                                 # Application Constants
    ├── Exceptions/                                # Custom Exceptions
    ├── Utilities/                                 # Shared Helpers
    └── BHK.Retrieval.Attendance.Shared.csproj
```

## 🔗 Dependencies (Clean Architecture)

```
┌─────────────────────────────────────────┐
│   BHK.Retrieval.Attendance.WPF         │  ← Main WPF Application (Startup Project)
│   (Presentation Layer)                  │
└─────────┬───────────────────────────────┘
          │ references ↓
          ├──────────────────────────────────────┐
          │                                      │
┌─────────▼─────────────────────┐   ┌──────────▼──────────────────────┐
│ BHK.Retrieval.Attendance.Core │   │ BHK.Retrieval.Attendance.Infra  │
│ (Domain + Use Cases)          │   │ (Data, Devices, SharePoint)     │
└─────────┬─────────────────────┘   └──────────┬──────────────────────┘
          │ references ↓                       │ references ↓
          │                                    │
          ├────────────────────────────────────┤
          │                                    │
┌─────────▼────────────────────────────────────▼─┐
│    BHK.Retrieval.Attendance.Shared             │
│    (Common Utilities - NO dependencies)        │
└────────────────────────────────────────────────┘
```

## 📦 Project References

### ✅ BHK.Retrieval.Attendance.WPF (Main Project)
- References:
  - ✔ BHK.Retrieval.Attendance.Core
  - ✔ BHK.Retrieval.Attendance.Infrastructure
  - ✔ BHK.Retrieval.Attendance.Shared

### ✅ BHK.Retrieval.Attendance.Infrastructure
- References:
  - ✔ BHK.Retrieval.Attendance.Core
  - ✔ BHK.Retrieval.Attendance.Shared

### ✅ BHK.Retrieval.Attendance.Core
- References:
  - ✔ BHK.Retrieval.Attendance.Shared (only)

### ✅ BHK.Retrieval.Attendance.Shared
- References: ❌ NONE (independent layer)

## 🚀 Cách Build Project

### Option 1: Build từ Solution
```powershell
cd "k:\Workspace\BHK workspace\BHK_Retrieval_Attendance\BHK_Retrieval_Attendance.Project"
dotnet build BHK_Retrieval_Attendance.Project.sln
```

### Option 2: Build trực tiếp Main Project
```powershell
cd "k:\Workspace\BHK workspace\BHK_Retrieval_Attendance\BHK_Retrieval_Attendance.Project"
dotnet build BHK.Retrieval.Attendance.WPF\BHK.Retrieval.Attendance.WPF.csproj
```

### Option 3: Build từng layer riêng biệt
```powershell
# Build theo thứ tự dependencies
dotnet build BHK.Retrieval.Attendance.Shared\BHK.Retrieval.Attendance.Shared.csproj
dotnet build BHK.Retrieval.Attendance.Core\BHK.Retrieval.Attendance.Core.csproj
dotnet build BHK.Retrieval.Attendance.Infrastructure\BHK.Retrieval.Attendance.Infrastructure.csproj
dotnet build BHK.Retrieval.Attendance.WPF\BHK.Retrieval.Attendance.WPF.csproj
```

## 🎯 Startup Project

**Main Startup Project:** `BHK.Retrieval.Attendance.WPF`

Set trong Visual Studio:
1. Right-click vào `BHK.Retrieval.Attendance.WPF` trong Solution Explorer
2. Chọn **"Set as Startup Project"**

## ✨ Các Thay Đổi Chính

1. ✅ **Di chuyển folder** `BHK.Retrieval.Attendance.WPF` vào trong `BHK_Retrieval_Attendance.Project/`
2. ✅ **Tạo các .csproj file** cho tất cả các layer theo Clean Architecture
3. ✅ **Cấu hình dependencies** đúng theo nguyên tắc Clean Architecture
4. ✅ **Cập nhật Solution file** với 4 projects mới
5. ✅ **Backup** file .csproj cũ thành `.OLD`

## 📝 Next Steps

1. **Kiểm tra build:**
   ```powershell
   dotnet restore
   dotnet build
   ```

2. **Tạo folder structure** cho các layer nếu chưa có:
   - Core: Domain/, UseCases/, Interfaces/, Contracts/
   - Infrastructure: Devices/, SharePoint/, Persistence/, External/
   - Shared: Extensions/, Results/, Options/, Constants/

3. **Di chuyển code** từ các folder cũ (Core/, Infrastructure/, Shared/ ở root) vào các project tương ứng

4. **Xóa các folder trùng lặp** sau khi đã di chuyển code

## ⚠️ Lưu Ý

- File `BHK_Retrieval_Attendance.Project.csproj.OLD` là backup của project cũ
- Có thể xóa các folder `Core/`, `Infrastructure/`, `Shared/`, `WPF/` ở root sau khi di chuyển code
- Main project giờ là `BHK.Retrieval.Attendance.WPF`, không còn là `BHK_Retrieval_Attendance.Project`
