using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Views.Dialogs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IPathSettingsService _pathSettingsService;
    private readonly IExcelService _excelService;
    private readonly IDialogService _dialogService;
    private readonly Func<ExportEmployeeViewModel> _exportEmployeeViewModelFactory;

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
        ILogger<SettingsViewModel> logger,
        IPathSettingsService pathSettingsService,
        IExcelService excelService,
        IDialogService dialogService,
        Func<ExportEmployeeViewModel> exportEmployeeViewModelFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pathSettingsService = pathSettingsService ?? throw new ArgumentNullException(nameof(pathSettingsService));
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _exportEmployeeViewModelFactory = exportEmployeeViewModelFactory ?? throw new ArgumentNullException(nameof(exportEmployeeViewModelFactory));

        BrowseAttendanceFolderCommand = new AsyncRelayCommand(BrowseAttendanceFolderAsync);
        BrowseEmployeeFileCommand = new AsyncRelayCommand(BrowseEmployeeFileAsync);
        SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
        ResetSettingsCommand = new RelayCommand(ResetSettings);
        TestExportAttendanceCommand = new AsyncRelayCommand(TestExportAttendanceAsync);
        TestExportEmployeeCommand = new AsyncRelayCommand(TestExportEmployeeAsync);

        LoadSettings();
    }

    private void LoadSettings()
    {
        AttendanceExportFolder = _pathSettingsService.GetAttendanceExportFolder();
        EmployeeDataFilePath = _pathSettingsService.GetEmployeeDataFilePath();
        AttendanceTableName = _pathSettingsService.GetAttendanceTableName();
        EmployeeTableName = _pathSettingsService.GetEmployeeTableName();
    }

    private async Task BrowseAttendanceFolderAsync()
    {
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "Chọn thư mục lưu file điểm danh",
            UseDescriptionForTitle = true,
            SelectedPath = AttendanceExportFolder
        };

        if (dialog.ShowDialog() == true)
        {
            AttendanceExportFolder = dialog.SelectedPath;
        }
    }

    private async Task BrowseEmployeeFileAsync()
    {
        var dialog = new VistaOpenFileDialog
        {
            Title = "Chọn file Excel dữ liệu nhân viên",
            Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls",
            CheckFileExists = false,
            FileName = EmployeeDataFilePath
        };

        if (dialog.ShowDialog() == true)
        {
            EmployeeDataFilePath = dialog.FileName;
        }
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            IsLoading = true;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(AttendanceExportFolder))
            {
                await _dialogService.ShowWarningAsync("Cảnh báo", "Vui lòng chọn thư mục xuất file điểm danh");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmployeeDataFilePath))
            {
                await _dialogService.ShowWarningAsync("Cảnh báo", "Vui lòng chọn file dữ liệu nhân viên");
                return;
            }

            if (string.IsNullOrWhiteSpace(AttendanceTableName))
            {
                await _dialogService.ShowWarningAsync("Cảnh báo", "Vui lòng nhập tên table điểm danh");
                return;
            }

            // Tạo folder nếu chưa tồn tại
            if (!Directory.Exists(AttendanceExportFolder))
            {
                Directory.CreateDirectory(AttendanceExportFolder);
            }

            // Save settings
            _pathSettingsService.SetAttendanceExportFolder(AttendanceExportFolder);
            _pathSettingsService.SetEmployeeDataFilePath(EmployeeDataFilePath);
            _pathSettingsService.SetAttendanceTableName(AttendanceTableName);
            _pathSettingsService.SetEmployeeTableName(EmployeeTableName);

            await _dialogService.ShowMessageAsync("Thành công", "Lưu cài đặt thành công!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings");
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi lưu cài đặt: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetSettings()
    {
        _pathSettingsService.ResetToDefaults();
        LoadSettings();
    }

    private async Task TestExportAttendanceAsync()
    {
        try
        {
            IsLoading = true;

            // Tạo dữ liệu test (5 records)
            var testData = GenerateTestAttendanceData();

            // ✅ SỬ DỤNG ExportConfigurationDialog (dialog đơn giản cho attendance)
            var dialogViewModel = new ExportConfigurationDialogViewModel
            {
                RecordCount = testData.Count,
                FileName = $"test_attendance_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx"
            };

            var dialog = new ExportConfigurationDialog
            {
                DataContext = dialogViewModel,
                Owner = System.Windows.Application.Current.MainWindow
            };

            // Set DialogWindow reference
            dialogViewModel.DialogWindow = dialog;

            // Hiển thị dialog và chờ user action
            if (dialog.ShowDialog() == true)
            {
                // Export thực tế khi user click XUẤT FILE
                var filePath = Path.Combine(AttendanceExportFolder, dialogViewModel.FileName);
                
                // Tạo table nếu chưa tồn tại
                if (File.Exists(filePath))
                {
                    var tableExists = await _excelService.TableExistsAsync(filePath, AttendanceTableName);
                    if (!tableExists)
                    {
                        await _excelService.CreateAttendanceTableAsync(filePath, AttendanceTableName);
                    }
                }
                else
                {
                    // Tạo file và table mới
                    var directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    await _excelService.CreateAttendanceTableAsync(filePath, AttendanceTableName);
                }
                
                await _excelService.ExportAttendanceDataAsync(filePath, AttendanceTableName, testData);

                await _dialogService.ShowMessageAsync("Thành công", 
                    $"Đã xuất {testData.Count} bản ghi test vào:\n{filePath}\nTable: {AttendanceTableName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing attendance export");
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi test xuất điểm danh: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task TestExportEmployeeAsync()
    {
        try
        {
            IsLoading = true;

            // Tạo dữ liệu test (5 employees)
            var testData = GenerateTestEmployeeData();

            // ✅ SỬ DỤNG ExportEmployeeDialog (dialog phức tạp cho employee)
            // Tạo ViewModel từ factory
            var dialogViewModel = _exportEmployeeViewModelFactory();
            
            // Set test data
            dialogViewModel.SetTestData(testData);

            // Tạo dialog
            var dialog = new ExportEmployeeDialog
            {
                DataContext = dialogViewModel,
                Owner = System.Windows.Application.Current.MainWindow
            };

            // Set DialogWindow reference
            dialogViewModel.DialogWindow = dialog;

            // Hiển thị dialog - ViewModel sẽ tự handle export
            dialog.ShowDialog();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing employee export");
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi test xuất nhân viên: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private List<AttendanceDisplayDto> GenerateTestAttendanceData()
    {
        return new List<AttendanceDisplayDto>
        {
            new AttendanceDisplayDto
            {
                EmployeeId = "NV001",
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Time = "08:30:00",
                VerifyMode = "Fingerprint"
            },
            new AttendanceDisplayDto
            {
                EmployeeId = "NV002",
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Time = "08:45:00",
                VerifyMode = "Card"
            },
            new AttendanceDisplayDto
            {
                EmployeeId = "NV003",
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Time = "09:00:00",
                VerifyMode = "Password"
            },
            new AttendanceDisplayDto
            {
                EmployeeId = "NV004",
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Time = "09:15:00",
                VerifyMode = "Fingerprint"
            },
            new AttendanceDisplayDto
            {
                EmployeeId = "NV005",
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Time = "09:30:00",
                VerifyMode = "Face"
            }
        };
    }

    private List<EmployeeDto> GenerateTestEmployeeData()
    {
        return new List<EmployeeDto>
        {
            new EmployeeDto
            {
                IDNumber = "NV001",
                UserName = "Nguyễn Văn A",
                Enable = true
            },
            new EmployeeDto
            {
                IDNumber = "NV002",
                UserName = "Trần Thị B",
                Enable = true
            },
            new EmployeeDto
            {
                IDNumber = "NV003",
                UserName = "Lê Văn C",
                Enable = false
            },
            new EmployeeDto
            {
                IDNumber = "NV004",
                UserName = "Phạm Thị D",
                Enable = true
            },
            new EmployeeDto
            {
                IDNumber = "NV005",
                UserName = "Hoàng Văn E",
                Enable = true
            }
        };
    }
}
