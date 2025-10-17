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
using BHK.Retrieval.Attendance.WPF.Models.Data;

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
        private string _exportFolder = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> _fileTypes = new() { "xlsx", "xls", "csv", "json" };

        [ObservableProperty]
        private string _selectedFileType = "xlsx";

        [ObservableProperty]
        private string _generatedFileName = string.Empty;

        [ObservableProperty]
        private string _tableName = string.Empty;

        [ObservableProperty]
        private int _attendanceCount;

        [ObservableProperty]
        private string _columnList = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        // ✅ Legacy properties để tương thích với code cũ
        public string FilePath => Path.Combine(ExportFolder, GeneratedFileName);
        public string FileName => GeneratedFileName;
        public string SelectedTable => TableName;

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

            // ✅ Load initial values từ PathConfigurationService
            var folder = _pathConfig.GetAttendanceExportFolder();
            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            
            ExportFolder = folder;
            TableName = _pathConfig.GetAttendanceTableName();
            SelectedFileType = "xlsx";
            
            // ✅ Thiết lập cột mặc định cho attendance
            ColumnList = "ID, Date, Time, Verify";
            
            UpdateGeneratedFileName();
            _ = LoadInitialDataAsync();
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
        private void BrowseFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Chọn thư mục để xuất file điểm danh",
                SelectedPath = ExportFolder
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExportFolder = dialog.SelectedPath;
                // Folder sẽ được lưu tự động trong OnExportFolderChanged
            }
        }

        // TODO: Cập nhật lại cho interface mới - sẽ tự tạo file và table khi Export
        /*
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
        */

        [RelayCommand]
        private async Task Export()
        {
            if (string.IsNullOrWhiteSpace(ExportFolder))
            {
                StatusMessage = "Vui lòng chọn thư mục xuất file";
                return;
            }

            if (string.IsNullOrWhiteSpace(TableName))
            {
                StatusMessage = "Vui lòng nhập tên table";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedFileType))
            {
                StatusMessage = "Vui lòng chọn định dạng file";
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

                // Tạo đường dẫn file đầy đủ
                var fullFilePath = Path.Combine(ExportFolder, GeneratedFileName);

                // ✅ Xuất dữ liệu theo định dạng đã chọn
                if (SelectedFileType == "xlsx" || SelectedFileType == "xls")
                {
                    await _excelService.ExportAttendanceDataAsync(fullFilePath, TableName, _data);
                }
                else
                {
                    // TODO: Thêm hỗ trợ JSON, CSV sau
                    StatusMessage = $"Định dạng {SelectedFileType} chưa được hỗ trợ";
                    return;
                }

                StatusMessage = $"Đã xuất {_data.Count} bản ghi thành công!";
                _logger.LogInformation($"Exported {_data.Count} records to {fullFilePath}");

                System.Windows.MessageBox.Show($"Đã xuất {_data.Count} bản ghi vào file '{GeneratedFileName}' thành công!\n\nĐường dẫn: {fullFilePath}",
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

        // TODO: Cập nhật lại method này cho interface mới
        /*
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
        */

        // TODO: Cập nhật lại method này cho interface mới
        /*
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
        */

        // Thêm các method mới cho interface cập nhật
        private void UpdateGeneratedFileName()
        {
            if (string.IsNullOrWhiteSpace(ExportFolder) || string.IsNullOrWhiteSpace(SelectedFileType))
            {
                GeneratedFileName = "Chưa chọn thư mục hoặc định dạng file";
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            // SelectedFileType hiện tại chỉ là string (xlsx, json, xls, csv)
            GeneratedFileName = $"attendance_{timestamp}.{SelectedFileType}";
        }

        private async Task LoadInitialDataAsync()
        {
            // Đọc thông tin từ settings
            ExportFolder = _pathConfig.GetAttendanceExportFolder();
            TableName = _pathConfig.GetAttendanceTableName();
            
            // Cập nhật số lượng bản ghi
            AttendanceCount = _data?.Count ?? 0;

            // Cập nhật thông tin cột (giả sử từ AttendanceExportDto)
            var columns = new List<string>
            {
                "EmployeeId", "EmployeeName", "Department", 
                "Date", "TimeIn", "TimeOut", "Status"
            };
            ColumnList = string.Join(", ", columns);

            // Cập nhật tên file
            UpdateGeneratedFileName();
        }

        // Property change handlers cho interface mới
        partial void OnSelectedFileTypeChanged(string value)
        {
            UpdateGeneratedFileName();
        }

        partial void OnExportFolderChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _pathConfig.SaveAttendanceExportFolder(value);
                UpdateGeneratedFileName();
            }
        }

        partial void OnTableNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _pathConfig.SaveAttendanceTableName(value);
            }
        }

        #endregion
    }
}
