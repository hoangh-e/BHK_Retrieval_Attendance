using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Ookii.Dialogs.Wpf;

namespace BHK.Retrieval.Attendance.WPF.ViewModels;

/// <summary>
/// ViewModel cho ExportEmployeeDialog - Tái sử dụng được cho nhiều chức năng
/// </summary>
public partial class ExportEmployeeViewModel : ObservableObject
{
    private readonly IExcelService _excelService;
    private readonly IPathSettingsService _pathSettingsService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<ExportEmployeeViewModel> _logger;

    // Properties
    [ObservableProperty]
    private string _employeeFilePath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _selectedTable = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _availableTables = new();

    [ObservableProperty]
    private int _recordCount;

    [ObservableProperty]
    private bool _isTableSelected;

    [ObservableProperty]
    private bool _canCreateTable;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Reference tới dialog window để có thể đóng sau khi export
    /// </summary>
    public Window? DialogWindow { get; set; }

    /// <summary>
    /// Test data được truyền vào từ bên ngoài
    /// </summary>
    private List<EmployeeDto>? _testData;

    // Commands
    public ICommand BrowseFileCommand { get; }
    public ICommand CreateTableCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand CancelCommand { get; }

    public ExportEmployeeViewModel(
        IExcelService excelService,
        IPathSettingsService pathSettingsService,
        IDialogService dialogService,
        ILogger<ExportEmployeeViewModel> logger)
    {
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _pathSettingsService = pathSettingsService ?? throw new ArgumentNullException(nameof(pathSettingsService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        BrowseFileCommand = new AsyncRelayCommand(BrowseFileAsync);
        CreateTableCommand = new AsyncRelayCommand(CreateTableAsync);
        ExportCommand = new AsyncRelayCommand(ExportAsync);
        CancelCommand = new RelayCommand(Cancel);

        // Load default path from settings
        var defaultPath = _pathSettingsService.GetEmployeeDataFilePath();
        if (!string.IsNullOrWhiteSpace(defaultPath))
        {
            EmployeeFilePath = defaultPath;
            _ = LoadFileInfoAsync();
        }
    }

    /// <summary>
    /// Set test data từ bên ngoài (cho test mode)
    /// </summary>
    public void SetTestData(List<EmployeeDto> testData)
    {
        _testData = testData;
    }

    private async Task BrowseFileAsync()
    {
        try
        {
            var dialog = new VistaOpenFileDialog
            {
                Title = "Chọn file Excel nhân viên",
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                CheckFileExists = false, // Allow creating new file
                InitialDirectory = !string.IsNullOrWhiteSpace(EmployeeFilePath) 
                    ? Path.GetDirectoryName(EmployeeFilePath) 
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true)
            {
                EmployeeFilePath = dialog.FileName;
                _pathSettingsService.SetEmployeeDataFilePath(EmployeeFilePath);
                
                await LoadFileInfoAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error browsing file");
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi chọn file: {ex.Message}");
        }
    }

    private async Task LoadFileInfoAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang kiểm tra file...";

            FileName = Path.GetFileName(EmployeeFilePath);

            if (!File.Exists(EmployeeFilePath))
            {
                StatusMessage = "File chưa tồn tại. Sẽ tạo mới khi xuất.";
                AvailableTables.Clear();
                CanCreateTable = true;
                IsTableSelected = false;
                RecordCount = 0;
                return;
            }

            // Validate file
            var isValid = await _excelService.ValidateExcelFileAsync(EmployeeFilePath);
            if (!isValid)
            {
                StatusMessage = "File không hợp lệ hoặc không thể đọc";
                AvailableTables.Clear();
                CanCreateTable = false;
                IsTableSelected = false;
                return;
            }

            // Load tables
            var tables = await _excelService.GetTableNamesAsync(EmployeeFilePath);
            AvailableTables.Clear();
            foreach (var table in tables)
            {
                AvailableTables.Add(table);
            }

            // Auto-select default table if exists
            var defaultTableName = _pathSettingsService.GetEmployeeTableName();
            if (AvailableTables.Contains(defaultTableName))
            {
                SelectedTable = defaultTableName;
            }
            else if (AvailableTables.Count > 0)
            {
                SelectedTable = AvailableTables[0];
            }

            CanCreateTable = AvailableTables.Count == 0;
            StatusMessage = AvailableTables.Count > 0 
                ? "File hợp lệ. Chọn table để tiếp tục." 
                : "File chưa có table. Click 'TẠO TABLE' để tạo mới.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading file info");
            StatusMessage = $"Lỗi: {ex.Message}";
            CanCreateTable = false;
            IsTableSelected = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedTableChanged(string value)
    {
        IsTableSelected = !string.IsNullOrWhiteSpace(value);

        if (IsTableSelected)
        {
            _ = LoadRecordCountAsync();
        }
    }

    private async Task LoadRecordCountAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EmployeeFilePath) || string.IsNullOrWhiteSpace(SelectedTable))
                return;

            RecordCount = await _excelService.GetRecordCountAsync(EmployeeFilePath, SelectedTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading record count");
            RecordCount = 0;
        }
    }

    private async Task CreateTableAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Đang tạo table...";

            // Ensure file exists
            if (!File.Exists(EmployeeFilePath))
            {
                var directory = Path.GetDirectoryName(EmployeeFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var tableName = _pathSettingsService.GetEmployeeTableName();
            var success = await _excelService.CreateEmployeeTableAsync(EmployeeFilePath, tableName);

            if (success)
            {
                StatusMessage = $"Đã tạo table '{tableName}' thành công";
                await _dialogService.ShowMessageAsync("Thành công", $"Đã tạo table '{tableName}' trong file Excel");
                
                // Reload tables
                await LoadFileInfoAsync();
            }
            else
            {
                StatusMessage = "Không thể tạo table";
                await _dialogService.ShowErrorAsync("Lỗi", "Không thể tạo table. Vui lòng kiểm tra lại file.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            StatusMessage = $"Lỗi: {ex.Message}";
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi tạo table: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExportAsync()
    {
        try
        {
            if (_testData == null || _testData.Count == 0)
            {
                await _dialogService.ShowWarningAsync("Cảnh báo", "Không có dữ liệu để xuất");
                return;
            }

            IsLoading = true;
            StatusMessage = "Đang xuất dữ liệu...";

            var success = await _excelService.ExportEmployeeDataAsync(EmployeeFilePath, SelectedTable, _testData);

            if (success)
            {
                await _dialogService.ShowMessageAsync("Thành công", 
                    $"Đã xuất {_testData.Count} nhân viên vào:\n{EmployeeFilePath}\nTable: {SelectedTable}");
                
                // Đóng dialog
                if (DialogWindow != null)
                {
                    DialogWindow.DialogResult = true;
                    DialogWindow.Close();
                }
            }
            else
            {
                await _dialogService.ShowErrorAsync("Lỗi", "Không thể xuất dữ liệu");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            await _dialogService.ShowErrorAsync("Lỗi", $"Lỗi khi xuất dữ liệu: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StatusMessage = string.Empty;
        }
    }

    private void Cancel()
    {
        if (DialogWindow != null)
        {
            DialogWindow.DialogResult = false;
            DialogWindow.Close();
        }
    }
}
