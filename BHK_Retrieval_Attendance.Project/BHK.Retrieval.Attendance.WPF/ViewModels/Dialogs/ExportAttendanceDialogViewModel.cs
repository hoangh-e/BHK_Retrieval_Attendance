using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel cho dialog xuất Attendance vào Excel
    /// ✅ TÁI SỬ DỤNG cho cả Test và Real data
    /// </summary>
    public partial class ExportAttendanceDialogViewModel : ObservableObject
    {
        private readonly IPathConfigurationService _pathConfig;
        private readonly IExcelTableService _excelService;
        private readonly ILogger<ExportAttendanceDialogViewModel> _logger;
        private List<AttendanceExportDto> _data;
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

        public ExportAttendanceDialogViewModel(
            IPathConfigurationService pathConfig,
            IExcelTableService excelService,
            ILogger<ExportAttendanceDialogViewModel> logger)
        {
            _pathConfig = pathConfig ?? throw new ArgumentNullException(nameof(pathConfig));
            _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _data = new List<AttendanceExportDto>();

            // Load initial values từ PathConfigurationService
            var folder = _pathConfig.GetAttendanceExportFolder();
            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            
            // Tạo tên file tự động trong folder
            var fileName = $"attendance_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx";
            FilePath = Path.Combine(folder, fileName);
            SelectedTable = _pathConfig.GetAttendanceTableName();

            _ = LoadFileInfoAsync();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set data để xuất - gọi từ bên ngoài (Test hoặc Real data)
        /// </summary>
        public void SetData(List<AttendanceExportDto> data)
        {
            _data = data ?? new List<AttendanceExportDto>();
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void BrowseFile()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Chọn thư mục để xuất file điểm danh",
                SelectedPath = FilePath
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Tạo tên file tự động trong folder đã chọn
                var fileName = $"attendance_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx";
                FilePath = Path.Combine(dialog.SelectedPath, fileName);
                _pathConfig.SaveAttendanceExportFolder(dialog.SelectedPath); // ✅ Lưu folder vào Settings
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
                StatusMessage = "Vui lòng nhập tên table";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Đang tạo table...";

                await _excelService.CreateAttendanceTableAsync(FilePath, SelectedTable);

                StatusMessage = $"Đã tạo table '{SelectedTable}' thành công";
                _logger.LogInformation($"Created table '{SelectedTable}' in {FilePath}");

                // Reload table list
                await LoadFileInfoAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi tạo table: {ex.Message}";
                _logger.LogError(ex, "Failed to create table");
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
                StatusMessage = "Vui lòng chọn table";
                return;
            }

            if (_data == null || !_data.Any())
            {
                StatusMessage = "Không có dữ liệu để xuất";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Đang xuất dữ liệu...";

                // ✅ Xuất dữ liệu (data được truyền từ bên ngoài - Test hoặc Real)
                await _excelService.ExportAttendanceDataAsync(FilePath, SelectedTable, _data);

                StatusMessage = $"Đã xuất {_data.Count} bản ghi thành công!";
                _logger.LogInformation($"Exported {_data.Count} records to {FilePath}");

                System.Windows.MessageBox.Show($"Đã xuất {_data.Count} bản ghi vào table '{SelectedTable}' thành công!",
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
                StatusMessage = $"Lỗi xuất dữ liệu: {ex.Message}";
                _logger.LogError(ex, "Failed to export data");
                
                System.Windows.MessageBox.Show($"Lỗi xuất dữ liệu: {ex.Message}",
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
                    // Nếu có tables, kiểm tra xem default table có tồn tại không
                    var defaultTable = _pathConfig.GetAttendanceTableName();
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
                    StatusMessage = $"Tìm thấy {tables.Count} table trong file";
                }
                else
                {
                    // Không có table nào, hiển thị nút "Tạo table"
                    CanCreateTable = true;
                    IsTableSelected = false;
                    StatusMessage = "File không có table nào. Vui lòng tạo table mới";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi kiểm tra file: {ex.Message}";
                _logger.LogError(ex, "Failed to load file info");
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
                return;
            }

            try
            {
                // Lưu table name vào settings
                _pathConfig.SaveAttendanceTableName(SelectedTable);

                // Get record count
                RecordCount = await _excelService.GetRecordCountAsync(FilePath, SelectedTable);
                IsTableSelected = true;
                CanCreateTable = false;
                StatusMessage = $"Table '{SelectedTable}' có {RecordCount} bản ghi";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi đọc table: {ex.Message}";
                _logger.LogError(ex, "Failed to update table info");
            }
        }

        partial void OnSelectedTableChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _ = UpdateTableInfoAsync();
            }
        }

        partial void OnFilePathChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _ = LoadFileInfoAsync();
            }
        }

        #endregion
    }
}
