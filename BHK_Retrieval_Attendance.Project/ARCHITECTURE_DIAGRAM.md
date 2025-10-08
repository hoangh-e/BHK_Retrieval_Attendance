# 🏗️ CLEAN ARCHITECTURE - DEPENDENCY FLOW

## 📊 Dependency Direction (One-Way Flow)

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                            │
│                                                                  │
│            BHK.Retrieval.Attendance.WPF                         │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  • ViewModels (MVVM)                                      │  │
│  │  • Views (XAML)                                           │  │
│  │  • Services (UI-specific: Dialog, Navigation, Theme)     │  │
│  │  • Converters, Behaviors, Commands                       │  │
│  │  • Resources (Styles, Templates, Images)                 │  │
│  │  • App.xaml / Startup & DI Configuration                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                  │
│  📦 Packages:                                                   │
│  - CommunityToolkit.Mvvm, MaterialDesign, Serilog              │
│  - Microsoft.Extensions.DependencyInjection                     │
└────────────┬─────────────────────────────┬─────────────────────┘
             │ references ↓                │ references ↓
             │                             │
┌────────────▼──────────────┐   ┌──────────▼──────────────────────┐
│      DOMAIN LAYER         │   │   INFRASTRUCTURE LAYER          │
│                           │   │                                 │
│  BHK.Retrieval.          │   │  BHK.Retrieval.                │
│  Attendance.Core          │   │  Attendance.Infrastructure      │
├───────────────────────────┤   ├─────────────────────────────────┤
│ • Domain/                 │   │ • Devices/                      │
│   - Entities              │   │   - Realand, ZKTeco            │
│   - Value Objects         │   │ • SharePoint/                   │
│   - Enums                 │   │   - PnP Framework              │
│   - Events                │   │ • Persistence/                  │
│   - Specifications        │   │   - EF Core, Repositories      │
│                           │   │ • FileStorage/                  │
│ • UseCases/               │   │   - Excel, PDF, CSV Export     │
│   - Employee              │   │ • External/                     │
│   - Attendance            │   │   - Email, SMS, AD             │
│   - Device                │   │ • Configuration/                │
│   - Report                │   │   - DB, Device, SP setup       │
│                           │   │                                 │
│ • Interfaces/             │   │ 📦 Packages:                   │
│   - Repositories          │   │ - EntityFrameworkCore          │
│   - Services              │   │ - PnP.Framework                │
│                           │   │ - EPPlus, iTextSharp           │
│ • Contracts/              │   │ - MailKit                      │
│   - DTOs                  │   │                                 │
│   - Requests/Responses    │◄──┤ Implements interfaces from Core │
│                           │   │                                 │
│ 📦 Packages:              │   │                                 │
│ - FluentValidation        │   │                                 │
│ - MediatR                 │   │                                 │
└────────────┬──────────────┘   └─────────────┬───────────────────┘
             │ references ↓                   │ references ↓
             │                                │
             └────────────────┬───────────────┘
                              │
                   ┌──────────▼────────────────────┐
                   │     SHARED LAYER              │
                   │                               │
                   │  BHK.Retrieval.              │
                   │  Attendance.Shared            │
                   ├───────────────────────────────┤
                   │ • Extensions/                 │
                   │   - String, DateTime, etc.    │
                   │                               │
                   │ • Results/                    │
                   │   - Result Pattern            │
                   │   - Error, Validation         │
                   │                               │
                   │ • Options/                    │
                   │   - Configuration POCOs       │
                   │                               │
                   │ • Constants/                  │
                   │   - App, DB, Device, etc.     │
                   │                               │
                   │ • Exceptions/                 │
                   │   - Custom Exceptions         │
                   │                               │
                   │ • Utilities/                  │
                   │   - Helpers, Cryptography     │
                   │                               │
                   │ 📦 Packages:                  │
                   │ - Microsoft.Extensions        │
                   │   .Logging.Abstractions       │
                   │ - Microsoft.Extensions        │
                   │   .Configuration.Abstractions │
                   │                               │
                   │ ❌ NO PROJECT REFERENCES      │
                   └───────────────────────────────┘
```

## ✅ Dependency Rules (Clean Architecture Principles)

### 1️⃣ **Dependency Direction**
```
Outer Layers → Inner Layers
(WPF → Infrastructure → Core → Shared)

❌ NEVER: Inner Layers → Outer Layers
```

### 2️⃣ **Layer Dependencies**

| Layer | Can Reference | Cannot Reference |
|-------|--------------|------------------|
| **WPF** | ✅ Core, Infrastructure, Shared | ❌ Nothing (outermost layer) |
| **Infrastructure** | ✅ Core, Shared | ❌ WPF |
| **Core** | ✅ Shared only | ❌ WPF, Infrastructure |
| **Shared** | ❌ None | ❌ All projects (innermost layer) |

### 3️⃣ **Interface Implementation**
```
Core Layer:
├── Defines: IEmployeeRepository, IDeviceService, etc.
└── Does NOT implement

Infrastructure Layer:
├── Implements: EmployeeRepository, DeviceService, etc.
└── References Core to implement interfaces

WPF Layer:
└── Uses implementations through DI
```

## 🎯 Communication Flow

### Example: Get Employee List

```
┌─────────────────────────────────────────────────────────────┐
│ 1. User clicks "Load Employees" button                      │
└─────────────────────────┬───────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. EmployeeListViewModel.LoadEmployeesCommand               │
│    (WPF Layer)                                              │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ calls
┌─────────────────────────────────────────────────────────────┐
│ 3. GetEmployeeListUseCase.Execute()                         │
│    (Core Layer - Business Logic)                            │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ uses
┌─────────────────────────────────────────────────────────────┐
│ 4. IEmployeeRepository.GetAllAsync()                        │
│    (Core Layer - Interface)                                 │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ implemented by
┌─────────────────────────────────────────────────────────────┐
│ 5. EmployeeRepository.GetAllAsync()                         │
│    (Infrastructure Layer - EF Core Implementation)          │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ queries
┌─────────────────────────────────────────────────────────────┐
│ 6. Database (SQL Server)                                    │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ returns data
┌─────────────────────────────────────────────────────────────┐
│ 7. Employee entities → EmployeeDto (mapping)                │
│    (Core Layer)                                             │
└─────────────────────────┬───────────────────────────────────┘
                          ↓ maps to
┌─────────────────────────────────────────────────────────────┐
│ 8. EmployeeDisplayModel (ViewModel updates UI)              │
│    (WPF Layer)                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Dependency Injection Setup

### App.xaml.cs (WPF Layer)
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    var services = new ServiceCollection();
    
    // 1. Register Shared layer services
    services.AddSharedServices();
    
    // 2. Register Core layer services
    services.AddCoreServices();
    
    // 3. Register Infrastructure layer services
    services.AddInfrastructureServices(configuration);
    
    // 4. Register WPF layer services
    services.AddWpfServices();
    
    // 5. Register ViewModels & Views
    services.AddViewModels();
    services.AddViews();
    
    var serviceProvider = services.BuildServiceProvider();
    var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
    mainWindow.Show();
}
```

## 📋 Summary

### ✅ Benefits of This Structure

1. **Separation of Concerns**
   - Each layer has clear responsibility
   - Easy to understand and maintain

2. **Testability**
   - Core layer can be tested without UI or Database
   - Infrastructure can be mocked
   - WPF ViewModels can be tested independently

3. **Flexibility**
   - Can replace Infrastructure (e.g., SQL Server → PostgreSQL)
   - Can replace UI (WPF → Blazor)
   - Core business logic remains unchanged

4. **Maintainability**
   - Changes in one layer don't affect others
   - Clear boundaries between layers
   - Easy to locate and fix bugs

5. **Scalability**
   - Easy to add new features
   - Easy to add new integrations
   - Easy to add new UI components

### ✅ Current Status

✔ **All 4 projects configured correctly**  
✔ **Dependencies follow Clean Architecture rules**  
✔ **Build successful**  
✔ **Ready for development**
