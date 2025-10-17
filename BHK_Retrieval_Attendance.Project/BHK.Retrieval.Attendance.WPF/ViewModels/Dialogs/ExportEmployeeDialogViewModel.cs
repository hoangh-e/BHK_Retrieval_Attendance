using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel cho dialog xuất Employee vào Excel
    /// ✅ TÁI SỬ DỤNG cho cả Test và Real data
    /// </summary>
    public partial class ExportEmployeeDialogViewModel : ObservableObject
    {
        private readonly IPathConfigurationService _pathConfig;
        private readonly IExcelTableService _excelService;
        private readonly ILogger<ExportEmployeeDialogViewModel> _logger;
        private List<EmployeeExportDto> _data;
        private Window? _dialog;

        #region Properties

        [ObservableProperty]
        private string _filePath = string.Empty;

        [ObservableProperty]
        private string _fileName = string.Empty;

        [ObservableProperty]
        private string _selectedTable = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _availableTables = new();

        [ObservableProperty]
        private int _recordCount;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _canCreateTable;

        [ObservableProperty]
        private bool _isTableSelected;

        [ObservableProperty]
        private bool _showCreateDefaultTable;

        [ObservableProperty]
        private string _tableValidationMessage = string.Empty;

        public bool DialogResult { get; private set; }
        
        /// <summary>
        /// Set dialog window reference - gọi từ bên ngoài
        /// </summary>
        public void SetDialog(Window dialog)
        {
            _dialog = dialog;
        }

        #endregion

        #region Constructor

        public ExportEmployeeDialogViewModel(
            IPathConfigurationService pathConfig,
            IExcelTableService excelService,
            ILogger<ExportEmployeeDialogViewModel> logger)
        {
            _pathConfig = pathConfig ?? throw new ArgumentNullException(nameof(pathConfig));
            _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _data = new List<EmployeeExportDto>();

            // Load initial values từ PathConfigurationService
            FilePath = _pathConfig.GetEmployeeDataFile();
            SelectedTable = _pathConfig.GetEmployeeTableName();

            _ = LoadFileInfoAsync();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set data để xuất - gọi từ bên ngoài (Test hoặc Real data)
        /// </summary>
        public void SetData(List<EmployeeExportDto> data)
        {
            _data = data ?? new List<EmployeeExportDto>();
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void BrowseFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn file Excel xuất danh sách nhân viên",
                Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                DefaultExt = ".xlsx",
                InitialDirectory = Path.GetDirectoryName(FilePath)
            };

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
                _pathConfig.SaveEmployeeDataFile(FilePath); // ✅ Lưu vào Settings
                _ = LoadFileInfoAsync();
            }
        }

        [RelayCommand]
        private async Task CreateTable()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                StatusMessage = "Vui lòng chọn file Excel trước";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedTable))
            {
                StatusMessage = "Vui lòng nhập tên Excel table";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Đang tạo Excel table...";

                await _excelService.CreateEmployeeTableAsync(FilePath, SelectedTable);

                StatusMessage = $"Đã tạo Excel table '{SelectedTable}' thành công với 10 cột employee";
                _logger.LogInformation($"Created Excel table '{SelectedTable}' in {FilePath}");

                // Reload table list
                await LoadFileInfoAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tạo Excel table: {ex.Message}";
                _logger.LogError(ex, "Failed to create Excel table");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Export()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                StatusMessage = "Vui lòng chọn file Excel";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedTable))
            {
                StatusMessage = "Vui lòng chọn Excel table";
                return;
            }

            if (_data == null || !_data.Any())
            {
                StatusMessage = "Không có dữ liệu employee để xuất";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = $"Đang xuất {_data.Count} records vào Excel table...";

                // ✅ Xuất dữ liệu vào Excel Table (không phải worksheet)
                await _excelService.ExportEmployeeDataAsync(FilePath, SelectedTable, _data);

                StatusMessage = $"Đã xuất {_data.Count} records vào Excel table thành công!";
                _logger.LogInformation($"Exported {_data.Count} records to Excel table '{SelectedTable}' in {FilePath}");

                MessageBox.Show($"Xuất thành công!\n\nĐã xuất {_data.Count} bản ghi employee vào Excel table '{SelectedTable}'\nFile: {Path.GetFileName(FilePath)}\n\nExcel table có filter và structured references sẵn sàng sử dụng!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                if (_dialog != null)
                {
                    _dialog.DialogResult = true;
                    _dialog.Close();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi xuất vào Excel table: {ex.Message}";
                _logger.LogError(ex, "Failed to export data to Excel table");
                
                MessageBox.Show($"Lỗi xuất dữ liệu: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
            if (_dialog != null)
            {
                _dialog.DialogResult = false;
                _dialog.Close();
            }
        }

        #endregion

        #region Helper Methods

        private async Task LoadFileInfoAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return;

            try
            {
                IsLoading = true;
                StatusMessage = "Đang kiểm tra file...";

                // Update file name
                FileName = Path.GetFileName(FilePath);

                // Validate file
                var isValid = await _excelService.ValidateExcelFileAsync(FilePath);
                if (!isValid)
                {
                    StatusMessage = "File Excel không hợp lệ hoặc không thể truy cập";
                    return;
                }

                // Get table names
                var tables = await _excelService.GetTableNamesAsync(FilePath);
                AvailableTables = new ObservableCollection<string>(tables);

                if (tables.Any())
                {
                    ShowCreateDefaultTable = false; // Hide create button when tables exist
                    
                    // Nếu có tables, kiểm tra xem default table có tồn tại không
                    var defaultTable = _pathConfig.GetEmployeeTableName();
                    if (tables.Contains(defaultTable, StringComparer.OrdinalIgnoreCase))
                    {
                        SelectedTable = defaultTable;
                        await UpdateTableInfoAsync();
                    }
                    else
                    {
                        // Chọn table đầu tiên
                        SelectedTable = tables.First();
                        await UpdateTableInfoAsync();
                    }

                    CanCreateTable = false;
                    StatusMessage = $"Tìm thấy {tables.Count} Excel table trong file";
                }
                else
                {
                    // Không có Excel table nào, hiển thị nút "Tạo default table"
                    ShowCreateDefaultTable = true;
                    CanCreateTable = true;
                    IsTableSelected = false;
                    TableValidationMessage = ""; // Clear any previous validation message
                    SelectedTable = ""; // Clear selected table
                    StatusMessage = "File không có Excel table nào. Cần tạo Excel table mới để xuất dữ liệu";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi kiểm tra file Excel: {ex.Message}";
                _logger.LogError(ex, "Failed to load Excel file info");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateTableInfoAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedTable))
            {
                IsTableSelected = false;
                RecordCount = 0;
                TableValidationMessage = ""; // Clear validation message when no table selected
                ShowCreateDefaultTable = AvailableTables?.Count == 0; // Show create button only when no tables exist
                return;
            }

            try
            {
                // ✅ Luôn lấy và hiển thị tên cột hiện tại
                var currentColumns = await _excelService.GetTableColumnsAsync(FilePath, SelectedTable);
                var currentColumnNames = currentColumns.Any() ? string.Join(", ", currentColumns) : "Không có cột nào";
                
                // Kiểm tra cột table có hợp lệ không
                var isValid = await _excelService.ValidateTableColumnsAsync(FilePath, SelectedTable, "Employee");
                
                if (!isValid)
                {
                    // Cột không hợp lệ - hiển thị thông báo và nút refactor
                    TableValidationMessage = $"Cấu trúc cột table không đúng định dạng Employee\nCột hiện tại: {currentColumnNames}";
                    IsTableSelected = false; // Không cho phép xuất
                    ShowCreateDefaultTable = false; // Hide create button when table exists but invalid
                }
                else
                {
                    // Cột hợp lệ - cho phép xuất
                    TableValidationMessage = $"Cấu trúc cột table hợp lệ\nCột hiện tại: {currentColumnNames}";
                    IsTableSelected = true;
                    ShowCreateDefaultTable = false; // Hide create button when valid table selected
                    
                    // Lưu table name vào settings
                    _pathConfig.SaveEmployeeTableName(SelectedTable);

                    // Get record count từ Excel Table
                    RecordCount = await _excelService.GetRecordCountAsync(FilePath, SelectedTable);
                    StatusMessage = $"Excel table '{SelectedTable}' có {RecordCount} records - Cấu trúc hợp lệ";
                }

                CanCreateTable = false;
            }
            catch (Exception ex)
            {
                TableValidationMessage = $"Lỗi kiểm tra table: {ex.Message}";
                IsTableSelected = false;
                _logger.LogError(ex, "Failed to update Excel table info");
            }
        }

        partial void OnSelectedTableChanged(string value)
        {
            // Always call UpdateTableInfoAsync to handle both selection and deselection
            _ = UpdateTableInfoAsync();
        }

        partial void OnFilePathChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _ = LoadFileInfoAsync();
            }
        }

        [RelayCommand]
        private async Task CreateDefaultTable()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                StatusMessage = "Vui lòng chọn file Excel trước";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Đang tạo Employee table mặc định...";

                // Lấy tên table mặc định từ settings
                var defaultTableName = _pathConfig.GetEmployeeTableName();
                
                await _excelService.CreateEmployeeTableAsync(FilePath, defaultTableName);

                StatusMessage = $"Đã tạo Employee table '{defaultTableName}' thành công với 10 cột tiêu chuẩn";
                _logger.LogInformation($"Created default Employee table '{defaultTableName}' in {FilePath}");

                // Reload table list
                await LoadFileInfoAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tạo Employee table mặc định: {ex.Message}";
                _logger.LogError(ex, "Failed to create default Employee table");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ShowRefactorDialog()
        {
            if (string.IsNullOrWhiteSpace(SelectedTable))
                return;

            try
            {
                // Lấy thông tin cột hiện tại và mong đợi
                var currentColumns = await _excelService.GetTableColumnsAsync(FilePath, SelectedTable);
                var expectedColumns = new List<string> { "ID", "Name", "IDNumber", "Department", "Sex", "Birthday", "Created", "Status", "Comment", "EnrollmentCount" };

                // Tạo và hiển thị dialog refactor
                var refactorDialog = new Views.Dialogs.RefactorColumnsDialog();
                var refactorViewModel = new RefactorColumnsDialogViewModel(_excelService, 
                    Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole())
                        .CreateLogger<RefactorColumnsDialogViewModel>());
                
                refactorViewModel.SetDialog(refactorDialog);
                refactorViewModel.SetData(FilePath, SelectedTable, "Employee", currentColumns, expectedColumns);
                
                refactorDialog.DataContext = refactorViewModel;
                refactorDialog.Owner = _dialog;

                var result = refactorDialog.ShowDialog();
                
                if (result == true)
                {
                    // Nếu refactor thành công, reload table info
                    await UpdateTableInfoAsync();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi hiển thị dialog refactor: {ex.Message}";
                _logger.LogError(ex, "Failed to show refactor dialog");
            }
        }

        #endregion
    }
}