using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Models;
using BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs;
using BHK.Retrieval.Attendance.WPF.Views.Dialogs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho trang Quản lý chấm công
    /// </summary>
    public partial class AttendanceManagementViewModel : ObservableObject
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AttendanceManagementViewModel> _logger;
        private readonly IServiceProvider _serviceProvider;

        #region Properties

        [ObservableProperty]
        private ObservableCollection<AttendanceDisplayDto> _attendanceRecords = new();

        [ObservableProperty]
        private AttendanceDisplayDto? _selectedRecord;

        [ObservableProperty]
        private bool _isLoading;

        // Filter Options
        [ObservableProperty]
        private FilterType _selectedFilterType = FilterType.PredefinedRange;

        // Option 1: Predefined Range
        [ObservableProperty]
        private ComboBoxItem<PredefinedDateRange>? _selectedPredefinedRangeItem;

        // Option 2: Single Date
        [ObservableProperty]
        private DateTime _singleDate = DateTime.Today;

        // Option 3: Date Range
        [ObservableProperty]
        private DateTime _startDate = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        // Time Filter
        [ObservableProperty]
        private ComboBoxItem<TimeFilter>? _selectedTimeFilterItem;

        [ObservableProperty]
        private int _totalRecords;

        [ObservableProperty]
        private long _loadTimeMs;

        // ComboBox Items Sources
        public List<ComboBoxItem<PredefinedDateRange>> PredefinedDateRanges { get; }
        public List<ComboBoxItem<TimeFilter>> TimeFilters { get; }

        #endregion

        #region Commands

        public IAsyncRelayCommand LoadDataCommand { get; }
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand OpenExportDialogCommand { get; }
        public IRelayCommand<string> ApplyFilterCommand { get; }

        #endregion

        public AttendanceManagementViewModel(
            IAttendanceService attendanceService,
            IDialogService dialogService,
            INotificationService notificationService,
            ILogger<AttendanceManagementViewModel> logger,
            IServiceProvider serviceProvider)
        {
            _attendanceService = attendanceService;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _logger = logger;
            _serviceProvider = serviceProvider;

            // Initialize ComboBox items
            PredefinedDateRanges = FilterItemsProvider.GetPredefinedDateRanges();
            TimeFilters = FilterItemsProvider.GetTimeFilters();
            
            // Set default selections
            SelectedPredefinedRangeItem = PredefinedDateRanges.First();
            SelectedTimeFilterItem = TimeFilters.First();

            // Initialize Commands
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            OpenExportDialogCommand = new AsyncRelayCommand(OpenExportDialogAsync);
            ApplyFilterCommand = new RelayCommand<string>(OnApplyFilter);

            // Không tự động load dữ liệu
            // User sẽ click nút "Lọc" để load dữ liệu theo điều kiện
        }

        #region Command Implementations

        public async Task LoadDataAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                IsLoading = true;
                LoadTimeMs = 0; // Reset load time
                
                var filter = BuildFilterDto();
                var records = await _attendanceService.GetAttendanceRecordsAsync(filter);
                
                AttendanceRecords.Clear();
                foreach (var record in records)
                {
                    AttendanceRecords.Add(record);
                }
                
                TotalRecords = AttendanceRecords.Count;
                
                stopwatch.Stop();
                LoadTimeMs = stopwatch.ElapsedMilliseconds;
                
                _logger.LogInformation($"Loaded {TotalRecords} attendance records in {LoadTimeMs}ms");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LoadTimeMs = 0;
                _logger.LogError(ex, "Error loading attendance records");
                await _notificationService.ShowErrorAsync("Lỗi", "Không thể tải dữ liệu chấm công");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshAsync()
        {
            // Reset về mặc định
            SelectedFilterType = FilterType.PredefinedRange;
            SelectedPredefinedRangeItem = PredefinedDateRanges.First();
            SelectedTimeFilterItem = TimeFilters.First();
            SingleDate = DateTime.Today;
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;

            await LoadDataAsync();
            
            await _notificationService.ShowSuccessAsync("Thành công", "Đã làm mới dữ liệu");
        }

        private async Task OpenExportDialogAsync()
        {
            try
            {
                // ✅ Kiểm tra có dữ liệu THẬT để xuất không
                if (AttendanceRecords == null || AttendanceRecords.Count == 0)
                {
                    await _notificationService.ShowWarningAsync("Cảnh báo", "Không có dữ liệu để xuất. Vui lòng tải dữ liệu trước.");
                    _logger.LogWarning("No attendance records to export");
                    return;
                }

                _logger.LogInformation($"Opening export dialog with {AttendanceRecords.Count} real attendance records");

                // ✅ Map dữ liệu THẬT sang AttendanceExportDto
                var exportData = AttendanceRecords.Select(x => new AttendanceExportDto
                {
                    ID = x.DIN,
                    Date = x.Date,
                    Time = x.Time,
                    Verify = x.Verify
                }).ToList();

                // ✅ Sử dụng dialog MỚI với dữ liệu THẬT
                var dialog = new ExportAttendanceDialog();
                var vm = _serviceProvider.GetRequiredService<ExportAttendanceDialogViewModel>();
                vm.SetData(exportData);  // ✅ Dữ liệu THẬT - không fallback
                vm.SetDialog(dialog);
                
                dialog.DataContext = vm;
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                
                _logger.LogInformation($"Export dialog opened with {exportData.Count} records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening export dialog");
                await _notificationService.ShowErrorAsync("Lỗi", $"Không thể mở dialog xuất file: {ex.Message}");
            }
        }

        private void OnApplyFilter(string? parameter)
        {
            // Apply filter khi user thay đổi options
            _ = LoadDataAsync();
        }

        #endregion

        #region Helper Methods

        private AttendanceFilterDto BuildFilterDto()
        {
            var filter = new AttendanceFilterDto
            {
                FilterType = SelectedFilterType,
                TimeFilter = SelectedTimeFilterItem?.Value ?? TimeFilter.All
            };

            switch (SelectedFilterType)
            {
                case FilterType.PredefinedRange:
                    var predefinedRange = SelectedPredefinedRangeItem?.Value ?? PredefinedDateRange.Today;
                    filter.PredefinedRange = predefinedRange;
                    filter.StartDate = GetStartDateFromPredefined(predefinedRange);
                    filter.EndDate = GetEndDateFromPredefined(predefinedRange);
                    break;

                case FilterType.SingleDate:
                    filter.SingleDate = SingleDate;
                    filter.StartDate = SingleDate.Date;
                    filter.EndDate = SingleDate.Date.AddDays(1).AddSeconds(-1);
                    break;

                case FilterType.DateRange:
                    filter.StartDate = StartDate.Date;
                    filter.EndDate = EndDate.Date.AddDays(1).AddSeconds(-1);
                    break;
            }

            return filter;
        }

        private DateTime GetStartDateFromPredefined(PredefinedDateRange range)
        {
            return range switch
            {
                PredefinedDateRange.Today => DateTime.Today,
                PredefinedDateRange.Yesterday => DateTime.Today.AddDays(-1), // Chỉ lấy hôm qua
                PredefinedDateRange.Last3Days => DateTime.Today.AddDays(-2),   // 3 ngày: hôm nay + 2 ngày trước
                PredefinedDateRange.Last7Days => DateTime.Today.AddDays(-6),   // 7 ngày: hôm nay + 6 ngày trước
                PredefinedDateRange.Last30Days => DateTime.Today.AddDays(-29), // 30 ngày: hôm nay + 29 ngày trước
                PredefinedDateRange.CurrentWeek => GetMondayOfCurrentWeek(),
                PredefinedDateRange.CurrentMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                _ => DateTime.Today
            };
        }

        private DateTime GetEndDateFromPredefined(PredefinedDateRange range)
        {
            return range switch
            {
                // Hôm qua: Chỉ lấy 1 ngày duy nhất (không bao gồm hôm nay)
                PredefinedDateRange.Yesterday => DateTime.Today.AddDays(-1),
                // Tất cả các lựa chọn khác: lấy đến hôm nay
                _ => DateTime.Today
            };
        }

        private DateTime GetMondayOfCurrentWeek()
        {
            var today = DateTime.Today;
            int daysToSubtract = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return today.AddDays(-daysToSubtract);
        }

        #endregion
    }
}
