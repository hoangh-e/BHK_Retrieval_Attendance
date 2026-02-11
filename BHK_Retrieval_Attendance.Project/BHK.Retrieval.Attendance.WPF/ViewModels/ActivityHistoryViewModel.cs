using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Models;
using BHK.Retrieval.Attendance.Core.DTOs;
using BHK.Retrieval.Attendance.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho màn hình Lịch sử hoạt động
    /// </summary>
    public class ActivityHistoryViewModel : BaseViewModel
    {
        private readonly IActivityHistoryService _activityHistoryService;
        private readonly ILogger<ActivityHistoryViewModel> _logger;
        private readonly ActivityHistorySettings _settings;

        private ObservableCollection<ActivityHistory> _activities;
        private ActivityHistory? _selectedActivity;
        private bool _isBusy;
        private string _statusMessage;

        // Filter properties
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private string _selectedDeviceIp;
        private ActivityType? _selectedActivityType;
        private ActivityStatus? _selectedStatus;
        private string _searchText;

        // Pagination properties
        private int _currentPage;
        private int _totalPages;
        private int _totalRecords;
        private int _pageSize;

        // Statistics properties
        private int _totalActivities;
        private double _successRate;
        private int _failedCount;
        private int _deviceCount;

        public ActivityHistoryViewModel(
            IActivityHistoryService activityHistoryService,
            ILogger<ActivityHistoryViewModel> logger,
            IOptions<ActivityHistorySettings> settings)
        {
            _activityHistoryService = activityHistoryService ?? throw new ArgumentNullException(nameof(activityHistoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            _activities = new ObservableCollection<ActivityHistory>();
            _statusMessage = "Sẵn sàng";
            _pageSize = _settings.PageSize;
            _currentPage = 0;
            _selectedDeviceIp = string.Empty;
            _searchText = string.Empty;

            // Initialize commands
            LoadActivitiesCommand = new RelayCommand(async _ => await LoadActivitiesAsync(), _ => !IsBusy);
            ApplyFiltersCommand = new RelayCommand(async _ => await ApplyFiltersAsync(), _ => !IsBusy);
            ClearFiltersCommand = new RelayCommand(async _ => await ClearFiltersAsync(), _ => !IsBusy);
            PreviousPageCommand = new RelayCommand(async _ => await PreviousPageAsync(), _ => CanGoPreviousPage());
            NextPageCommand = new RelayCommand(async _ => await NextPageAsync(), _ => CanGoNextPage());
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync(), _ => !IsBusy);
            DeleteSelectedCommand = new RelayCommand(async _ => await DeleteSelectedAsync(), _ => CanDeleteSelected());
            DeleteAllCommand = new RelayCommand(async _ => await DeleteAllAsync(), _ => !IsBusy);

            // Quick filter commands
            QuickFilterTodayCommand = new RelayCommand(async _ => await ApplyQuickFilterAsync(ActivityFilter.Today()), _ => !IsBusy);
            QuickFilterLast7DaysCommand = new RelayCommand(async _ => await ApplyQuickFilterAsync(ActivityFilter.Last7Days()), _ => !IsBusy);
            QuickFilterLast30DaysCommand = new RelayCommand(async _ => await ApplyQuickFilterAsync(ActivityFilter.Last30Days()), _ => !IsBusy);
            QuickFilterLast90DaysCommand = new RelayCommand(async _ => await ApplyQuickFilterAsync(ActivityFilter.Last90Days()), _ => !IsBusy);

            _logger.LogInformation("ActivityHistoryViewModel initialized with PageSize: {PageSize}", _pageSize);
        }

        #region Properties

        public ObservableCollection<ActivityHistory> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }

        public ActivityHistory? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                if (SetProperty(ref _selectedActivity, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // Filter Properties
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set => SetProperty(ref _dateFrom, value);
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set => SetProperty(ref _dateTo, value);
        }

        public string SelectedDeviceIp
        {
            get => _selectedDeviceIp;
            set => SetProperty(ref _selectedDeviceIp, value);
        }

        public ActivityType? SelectedActivityType
        {
            get => _selectedActivityType;
            set => SetProperty(ref _selectedActivityType, value);
        }

        public ActivityStatus? SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        // Pagination Properties
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnPropertyChanged(nameof(CurrentPageDisplay));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    OnPropertyChanged(nameof(PaginationInfo));
                }
            }
        }

        public int TotalRecords
        {
            get => _totalRecords;
            set
            {
                if (SetProperty(ref _totalRecords, value))
                {
                    OnPropertyChanged(nameof(PaginationInfo));
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public string CurrentPageDisplay => $"Trang {CurrentPage + 1}";
        public string PaginationInfo => $"Hiển thị {Activities.Count} / {TotalRecords} bản ghi (Trang {CurrentPage + 1}/{Math.Max(TotalPages, 1)})";

        // Statistics Properties
        public int TotalActivities
        {
            get => _totalActivities;
            set => SetProperty(ref _totalActivities, value);
        }

        public double SuccessRate
        {
            get => _successRate;
            set => SetProperty(ref _successRate, value);
        }

        public int FailedCount
        {
            get => _failedCount;
            set => SetProperty(ref _failedCount, value);
        }

        public int DeviceCount
        {
            get => _deviceCount;
            set => SetProperty(ref _deviceCount, value);
        }

        // Enum lists for ComboBox bindings
        public List<ActivityType> ActivityTypes => Enum.GetValues<ActivityType>().ToList();
        public List<ActivityStatus> ActivityStatuses => Enum.GetValues<ActivityStatus>().ToList();

        #endregion

        #region Commands

        public ICommand LoadActivitiesCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteSelectedCommand { get; }
        public ICommand DeleteAllCommand { get; }
        
        // Quick filter commands
        public ICommand QuickFilterTodayCommand { get; }
        public ICommand QuickFilterLast7DaysCommand { get; }
        public ICommand QuickFilterLast30DaysCommand { get; }
        public ICommand QuickFilterLast90DaysCommand { get; }

        #endregion

        #region Command Implementations

        public async Task LoadActivitiesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = "Đang tải dữ liệu...";
                _logger.LogInformation("Loading activities - Page: {Page}, PageSize: {PageSize}", CurrentPage, PageSize);

                var filter = BuildFilter();
                var (items, totalCount) = await _activityHistoryService.GetActivitiesAsync(filter);

                Activities.Clear();
                foreach (var item in items)
                {
                    Activities.Add(item);
                }

                TotalRecords = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

                // Load statistics
                await LoadStatisticsAsync(filter);

                StatusMessage = $"Đã tải {items.Count} bản ghi";
                _logger.LogInformation("✅ Loaded {Count} activities (Total: {Total})", items.Count, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activities");
                StatusMessage = "Lỗi khi tải dữ liệu";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadStatisticsAsync(ActivityFilter filter)
        {
            try
            {
                var stats = await _activityHistoryService.GetStatisticsAsync(filter);
                TotalActivities = stats.TotalActivities;
                SuccessRate = stats.SuccessRate;
                FailedCount = stats.FailedCount;
                DeviceCount = stats.DeviceCount;

                _logger.LogDebug("Statistics loaded - Total: {Total}, Success Rate: {Rate}%, Failed: {Failed}", 
                    TotalActivities, SuccessRate, FailedCount);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load statistics");
            }
        }

        private async Task ApplyFiltersAsync()
        {
            CurrentPage = 0; // Reset to first page
            await LoadActivitiesAsync();
        }

        private async Task ClearFiltersAsync()
        {
            DateFrom = null;
            DateTo = null;
            SelectedDeviceIp = string.Empty;
            SelectedActivityType = null;
            SelectedStatus = null;
            SearchText = string.Empty;
            CurrentPage = 0;

            await LoadActivitiesAsync();
        }

        private async Task ApplyQuickFilterAsync(ActivityFilter quickFilter)
        {
            DateFrom = quickFilter.FromDate;
            DateTo = quickFilter.ToDate;
            await ApplyFiltersAsync();
        }

        private bool CanGoPreviousPage() => !IsBusy && CurrentPage > 0;

        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
                await LoadActivitiesAsync();
            }
        }

        private bool CanGoNextPage() => !IsBusy && CurrentPage < TotalPages - 1;

        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages - 1)
            {
                CurrentPage++;
                await LoadActivitiesAsync();
            }
        }

        private async Task RefreshAsync()
        {
            await LoadActivitiesAsync();
        }

        private bool CanDeleteSelected() => !IsBusy;

        private async Task DeleteSelectedAsync()
        {
            try
            {
                // Get list of device IPs from statistics
                var stats = await _activityHistoryService.GetStatisticsAsync(BuildFilter());
                var deviceIps = stats?.DeviceActivityCounts?.Keys.ToList() ?? new List<string>();

                // Show delete dialog
                var dialog = new Views.Dialogs.DeleteActivitiesDialog(deviceIps)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                IsBusy = true;
                StatusMessage = "Đang xóa dữ liệu...";

                int deletedCount = 0;

                switch (dialog.SelectedOption)
                {
                    case Views.Dialogs.DeleteOption.ByDateRange:
                        if (dialog.FromDate.HasValue && dialog.ToDate.HasValue)
                        {
                            // Calculate days from ToDate to now
                            var daysSinceToDate = (int)(DateTime.Now - dialog.ToDate.Value).TotalDays;
                            if (daysSinceToDate < 0) daysSinceToDate = 0; // Don't delete future records
                            
                            deletedCount = await _activityHistoryService.ClearOldActivitiesAsync(daysSinceToDate);
                            _logger.LogInformation("Deleted {Count} activities older than {Days} days", 
                                deletedCount, daysSinceToDate);
                            StatusMessage = $"Đã xóa {deletedCount} bản ghi cũ hơn {daysSinceToDate} ngày";
                        }
                        break;

                    case Views.Dialogs.DeleteOption.ByDevice:
                        if (!string.IsNullOrWhiteSpace(dialog.SelectedDeviceIp))
                        {
                            deletedCount = await _activityHistoryService.ClearActivitiesByDeviceAsync(dialog.SelectedDeviceIp);
                            _logger.LogInformation("Deleted {Count} activities for device {DeviceIp}", 
                                deletedCount, dialog.SelectedDeviceIp);
                            StatusMessage = $"Đã xóa {deletedCount} bản ghi của thiết bị {dialog.SelectedDeviceIp}";
                        }
                        break;

                    case Views.Dialogs.DeleteOption.All:
                        deletedCount = await _activityHistoryService.ClearAllActivitiesAsync();
                        _logger.LogInformation("Deleted all {Count} activities", deletedCount);
                        StatusMessage = $"Đã xóa tất cả {deletedCount} bản ghi lịch sử";
                        break;
                }

                // Reload data
                await LoadActivitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activities");
                StatusMessage = $"Lỗi khi xóa dữ liệu: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteAllAsync()
        {
            try
            {
                // Show confirmation
                var result = System.Windows.MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa TẤT CẢ lịch sử hoạt động?\n\nThao tác này KHÔNG THỂ KHÔI PHỤC!",
                    "Xác nhận xóa tất cả",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    return;
                }

                IsBusy = true;
                StatusMessage = "Đang xóa tất cả dữ liệu...";

                int deletedCount = await _activityHistoryService.ClearAllActivitiesAsync();
                _logger.LogInformation("Deleted all {Count} activities", deletedCount);
                StatusMessage = $"Đã xóa tất cả {deletedCount} bản ghi lịch sử";

                // Reload data
                await LoadActivitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all activities");
                StatusMessage = $"Lỗi khi xóa dữ liệu: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Helper Methods

        private ActivityFilter BuildFilter()
        {
            var filter = new ActivityFilter
            {
                PageIndex = CurrentPage,
                PageSize = PageSize,
                FromDate = DateFrom,
                ToDate = DateTo,
                SearchText = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText
            };

            if (!string.IsNullOrWhiteSpace(SelectedDeviceIp))
            {
                filter.DeviceIp = SelectedDeviceIp;
            }

            if (SelectedActivityType.HasValue)
            {
                filter.ActivityType = SelectedActivityType.Value;
            }

            if (SelectedStatus.HasValue)
            {
                filter.Statuses = new List<ActivityStatus> { SelectedStatus.Value };
            }

            return filter;
        }

        #endregion

        #region RelayCommand

        /// <summary>
        /// RelayCommand implementation for ICommand
        /// </summary>
        private class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;

            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }
        }

        #endregion
    }
}
