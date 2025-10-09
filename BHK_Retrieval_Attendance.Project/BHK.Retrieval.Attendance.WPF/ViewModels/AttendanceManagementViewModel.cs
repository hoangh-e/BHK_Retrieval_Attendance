using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BHK.Retrieval.Attendance.Core.DTOs.Requests;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.Models;
using Microsoft.Extensions.Logging;

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
            ILogger<AttendanceManagementViewModel> logger)
        {
            _attendanceService = attendanceService;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _logger = logger;

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

            // Load initial data
            _ = LoadDataAsync();
        }

        #region Command Implementations

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                
                var filter = BuildFilterDto();
                var records = await _attendanceService.GetAttendanceRecordsAsync(filter);
                
                AttendanceRecords.Clear();
                foreach (var record in records)
                {
                    AttendanceRecords.Add(record);
                }
                
                TotalRecords = AttendanceRecords.Count;
                
                _logger.LogInformation($"Loaded {TotalRecords} attendance records");
            }
            catch (Exception ex)
            {
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
            // TODO: Mở dialog xuất file với dữ liệu hiện tại
            var exportConfig = new ExportConfigDto
            {
                FileType = ExportFileType.Excel,
                FileName = $"attendance_{DateTime.Now:yyyy-MM-dd}.xlsx",
                RecordCount = AttendanceRecords.Count,
                Data = AttendanceRecords.ToList()
            };

            // Mở dialog (sẽ implement sau)
            // var result = await _dialogService.ShowExportDialogAsync(exportConfig);
            
            _logger.LogInformation("Export dialog opened");
            await Task.CompletedTask;
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
                    filter.EndDate = DateTime.Now;
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
                PredefinedDateRange.Yesterday => DateTime.Today.AddDays(-1),
                PredefinedDateRange.Last3Days => DateTime.Today.AddDays(-2),   // 3 ngày: hôm nay + 2 ngày trước
                PredefinedDateRange.Last7Days => DateTime.Today.AddDays(-6),   // 7 ngày: hôm nay + 6 ngày trước
                PredefinedDateRange.Last30Days => DateTime.Today.AddDays(-29), // 30 ngày: hôm nay + 29 ngày trước
                PredefinedDateRange.CurrentWeek => GetMondayOfCurrentWeek(),
                PredefinedDateRange.CurrentMonth => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
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
