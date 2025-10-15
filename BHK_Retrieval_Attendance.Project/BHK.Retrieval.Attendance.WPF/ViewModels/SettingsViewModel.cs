using System;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Views.Dialogs;
using BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IPathConfigurationService _pathConfig;
    private readonly IExcelTableService _excelService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _attendanceExportFolder = string.Empty;

    [ObservableProperty]
    private string _employeeDataFilePath = string.Empty;

    [ObservableProperty]
    private string _attendanceTableName = string.Empty;

    [ObservableProperty]
    private string _employeeTableName = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ICommand BrowseAttendanceFolderCommand { get; }
    public ICommand BrowseEmployeeFileCommand { get; }
    public ICommand SaveSettingsCommand { get; }
    public ICommand ResetSettingsCommand { get; }
    public ICommand TestExportAttendanceCommand { get; }
    public ICommand TestExportEmployeeCommand { get; }

    public SettingsViewModel(
        IPathConfigurationService pathConfig,
        IExcelTableService excelService,
        ILogger<SettingsViewModel> logger,
        IServiceProvider serviceProvider)
    {
        _pathConfig = pathConfig ?? throw new ArgumentNullException(nameof(pathConfig));
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        BrowseAttendanceFolderCommand = new RelayCommand(BrowseAttendanceFolder);
        BrowseEmployeeFileCommand = new RelayCommand(BrowseEmployeeFile);
        SaveSettingsCommand = new RelayCommand(SaveSettings);
        ResetSettingsCommand = new RelayCommand(ResetSettings);
        TestExportAttendanceCommand = new AsyncRelayCommand(TestExportAttendanceAsync);
        TestExportEmployeeCommand = new AsyncRelayCommand(TestExportEmployeeAsync);

        LoadSettings();
    }

    private void LoadSettings()
    {
        AttendanceExportFolder = _pathConfig.GetAttendanceExportFolder();
        EmployeeDataFilePath = _pathConfig.GetEmployeeDataFile();
        AttendanceTableName = _pathConfig.GetAttendanceTableName();
        EmployeeTableName = _pathConfig.GetEmployeeTableName();
    }

    private void BrowseAttendanceFolder()
    {
        var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            AttendanceExportFolder = dialog.SelectedPath;
            _pathConfig.SaveAttendanceExportFolder(dialog.SelectedPath);
        }
    }

    private void BrowseEmployeeFile()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Excel Files|*.xlsx" };
        if (dialog.ShowDialog() == true)
        {
            EmployeeDataFilePath = dialog.FileName;
            _pathConfig.SaveEmployeeDataFile(dialog.FileName);
        }
    }

    private void SaveSettings()
    {
        // Lưu vào PathConfigurationService
        _pathConfig.SaveAttendanceExportFolder(AttendanceExportFolder);
        _pathConfig.SaveEmployeeDataFile(EmployeeDataFilePath);
        _pathConfig.SaveAttendanceTableName(AttendanceTableName);
        _pathConfig.SaveEmployeeTableName(EmployeeTableName);
        
        System.Windows.MessageBox.Show("Lưu cài đặt thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ResetSettings()
    {
        // Load lại default values
        LoadSettings();
    }

    private async Task TestExportAttendanceAsync()
    {
        // ✅ Tạo 5 dữ liệu TEST
        var testData = new List<AttendanceExportDto>
        {
            new() { ID = "001", Date = "2025-10-15", Time = "08:00", Verify = "FP" },
            new() { ID = "002", Date = "2025-10-15", Time = "08:15", Verify = "Card" },
            new() { ID = "003", Date = "2025-10-15", Time = "08:30", Verify = "Password" },
            new() { ID = "004", Date = "2025-10-15", Time = "17:00", Verify = "FP" },
            new() { ID = "005", Date = "2025-10-15", Time = "17:15", Verify = "Card" }
        };

        var dialog = new ExportAttendanceDialog();
        var vm = _serviceProvider.GetRequiredService<ExportAttendanceDialogViewModel>();
        vm.SetData(testData);
        vm.SetDialog(dialog);
        
        dialog.DataContext = vm;
        dialog.Owner = System.Windows.Application.Current.MainWindow;
        dialog.ShowDialog();
    }

    private async Task TestExportEmployeeAsync()
    {
        // ✅ Tạo 5 dữ liệu TEST với thông tin chi tiết
        var testData = new List<EmployeeExportDto>
        {
            new() { 
                ID = "E001", 
                Name = "Nguyễn Văn A", 
                IDNumber = "NV001",
                Department = "IT",
                Sex = "Male",
                Birthday = "1990-01-01",
                Created = "2025-01-01", 
                Status = "Active",
                Comment = "Test Employee 1",
                EnrollmentCount = 3
            },
            new() { 
                ID = "E002", 
                Name = "Trần Thị B", 
                IDNumber = "NV002",
                Department = "HR", 
                Sex = "Female",
                Birthday = "1992-05-15",
                Created = "2025-01-02", 
                Status = "Active",
                Comment = "Test Employee 2",
                EnrollmentCount = 2
            },
            new() { 
                ID = "E003", 
                Name = "Lê Văn C", 
                IDNumber = "NV003",
                Department = "Finance",
                Sex = "Male",
                Birthday = "1988-12-10", 
                Created = "2025-01-03", 
                Status = "Inactive",
                Comment = "Test Employee 3",
                EnrollmentCount = 1
            },
            new() { 
                ID = "E004", 
                Name = "Phạm Thị D", 
                IDNumber = "NV004",
                Department = "Marketing",
                Sex = "Female", 
                Birthday = "1995-03-20",
                Created = "2025-01-04", 
                Status = "Active",
                Comment = "Test Employee 4",
                EnrollmentCount = 4
            },
            new() { 
                ID = "E005", 
                Name = "Hoàng Văn E", 
                IDNumber = "NV005",
                Department = "Sales",
                Sex = "Male",
                Birthday = "1987-08-05", 
                Created = "2025-01-05", 
                Status = "Active",
                Comment = "Test Employee 5",
                EnrollmentCount = 2
            }
        };

        var dialog = new ExportEmployeeDialog();
        var vm = _serviceProvider.GetRequiredService<ExportEmployeeDialogViewModel>();
        vm.SetData(testData);
        vm.SetDialog(dialog);
        
        dialog.DataContext = vm;
        dialog.Owner = System.Windows.Application.Current.MainWindow;
        dialog.ShowDialog();
    }


}
