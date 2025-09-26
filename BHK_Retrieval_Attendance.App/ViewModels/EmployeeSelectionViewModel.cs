using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.Models;
using BHK.Retrieval.Attendance.WPF.Services;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class DepartmentFilter : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class EmployeeSelectionViewModel : INotifyPropertyChanged
    {
        private readonly IDeviceService _deviceService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<EmployeeSelectionViewModel> _logger;

        public EmployeeSelectionViewModel(IDeviceService deviceService,
                                        INavigationService navigationService,
                                        IDialogService dialogService,
                                        IAttendanceService attendanceService,
                                        ILogger<EmployeeSelectionViewModel> logger)
        {
            _deviceService = deviceService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _attendanceService = attendanceService;
            _logger = logger;

            // Initialize collections
            AllEmployees = new ObservableCollection<Employee>();
            FilteredEmployees = new ObservableCollection<Employee>();
            DepartmentFilters = new ObservableCollection<DepartmentFilter>();

            // Initialize commands
            RefreshCommand = new AsyncRelayCommand(RefreshEmployeesAsync);
            SelectEmployeeCommand = new RelayCommand<Employee>(SelectEmployee);
            ViewDetailCommand = new RelayCommand<Employee>(ViewEmployeeDetail);
            SelectDepartmentFilterCommand = new RelayCommand<DepartmentFilter>(SelectDepartmentFilter);
            ExportCommand = new AsyncRelayCommand(ExportEmployeeListAsync, CanExport);

            // Set default grid columns
            GridColumns = 2;

            // Initialize department filters
            InitializeDepartmentFilters();

            // Load employees on startup
            _ = Task.Run(async () => await RefreshEmployeesAsync());
        }

        #region Properties

        private ObservableCollection<Employee> _allEmployees;
        public ObservableCollection<Employee> AllEmployees
        {
            get => _allEmployees;
            set
            {
                _allEmployees = value;
                OnPropertyChanged();
                UpdateFilteredEmployees();
            }
        }

        private ObservableCollection<Employee> _filteredEmployees;
        public ObservableCollection<Employee> FilteredEmployees
        {
            get => _filteredEmployees;
            set
            {
                _filteredEmployees = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredEmployeeCount));
                OnPropertyChanged(nameof(HasEmployees));
                OnPropertyChanged(nameof(HasNoEmployees));
                ((AsyncRelayCommand)ExportCommand).RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<DepartmentFilter> _departmentFilters;
        public ObservableCollection<DepartmentFilter> DepartmentFilters
        {
            get => _departmentFilters;
            set
            {
                _departmentFilters = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                UpdateFilteredEmployees();
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEmployees));
                OnPropertyChanged(nameof(HasNoEmployees));
                ((AsyncRelayCommand)ExportCommand).RaiseCanExecuteChanged();
            }
        }

        private DateTime _lastRefreshTime = DateTime.Now;
        public DateTime LastRefreshTime
        {
            get => _lastRefreshTime;
            set
            {
                _lastRefreshTime = value;
                OnPropertyChanged();
            }
        }

        private int _gridColumns = 2;
        public int GridColumns
        {
            get => _gridColumns;
            set
            {
                _gridColumns = value;
                OnPropertyChanged();
            }
        }

        // Computed properties
        public int TotalEmployeeCount => AllEmployees?.Count ?? 0;
        public int FilteredEmployeeCount => FilteredEmployees?.Count ?? 0;
        public bool HasEmployees => !IsLoading && FilteredEmployees?.Any() == true;
        public bool HasNoEmployees => !IsLoading && FilteredEmployees?.Any() != true;

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand SelectEmployeeCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand SelectDepartmentFilterCommand { get; }
        public ICommand ExportCommand { get; }

        #endregion

        #region Command Implementations

        private async Task RefreshEmployeesAsync()
        {
            try
            {
                IsLoading = true;
                _logger.LogInformation("Refreshing employee list");

                var result = await _deviceService.GetAllEmployeesAsync();
                
                if (result.IsSuccess && result.Data != null)
                {
                    AllEmployees.Clear();
                    
                    var employees = result.Data.Select(MapUserToEmployee).ToList();
                    foreach (var employee in employees)
                    {
                        AllEmployees.Add(employee);
                    }
                    
                    LastRefreshTime = DateTime.Now;
                    _logger.LogInformation($"Successfully loaded {AllEmployees.Count} employees");
                }
                else
                {
                    _logger.LogWarning($"Failed to load employees: {result.ErrorMessage}");
                    await _dialogService.ShowErrorAsync("Lỗi", 
                        $"Không thể tải danh sách nhân viên: {result.ErrorMessage}");
                    
                    // Load sample data for demo
                    await LoadSampleEmployeesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing employee list");
                await _dialogService.ShowErrorAsync("Lỗi", "Có lỗi xảy ra khi tải danh sách nhân viên.");
                
                // Load sample data as fallback
                await LoadSampleEmployeesAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SelectEmployee(Employee? employee)
        {
            if (employee != null)
            {
                _logger.LogInformation($"Employee selected: {employee.UserName} (ID: {employee.DIN})");
                
                // Navigate to employee detail view
                _navigationService.NavigateToEmployeeDetail(employee.DIN);
            }
        }

        private void ViewEmployeeDetail(Employee? employee)
        {
            if (employee != null)
            {
                _logger.LogInformation($"Viewing details for employee: {employee.UserName} (ID: {employee.DIN})");
                _navigationService.NavigateToEmployeeDetail(employee.DIN);
            }
        }

        private void SelectDepartmentFilter(DepartmentFilter? filter)
        {
            if (filter != null)
            {
                // Toggle selection
                filter.IsSelected = !filter.IsSelected;
                
                // If "Tất cả" is selected, deselect others
                if (filter.Id == -1 && filter.IsSelected)
                {
                    foreach (var f in DepartmentFilters.Where(f => f.Id != -1))
                    {
                        f.IsSelected = false;
                    }
                }
                // If any specific department is selected, deselect "Tất cả"
                else if (filter.Id != -1 && filter.IsSelected)
                {
                    var allFilter = DepartmentFilters.FirstOrDefault(f => f.Id == -1);
                    if (allFilter != null)
                    {
                        allFilter.IsSelected = false;
                    }
                }
                
                // If no specific department is selected, select "Tất cả"
                if (!DepartmentFilters.Where(f => f.Id != -1).Any(f => f.IsSelected))
                {
                    var allFilter = DepartmentFilters.FirstOrDefault(f => f.Id == -1);
                    if (allFilter != null)
                    {
                        allFilter.IsSelected = true;
                    }
                }
                
                UpdateFilteredEmployees();
            }
        }

        private bool CanExport()
        {
            return HasEmployees && !IsLoading;
        }

        private async Task ExportEmployeeListAsync()
        {
            try
            {
                var result = await _dialogService.ShowSaveFileDialogAsync(
                    "Xuất danh sách nhân viên", 
                    $"DanhSach_NhanVien_{DateTime.Now:yyyyMMdd}.xlsx",
                    "Excel files (*.xlsx)|*.xlsx|PDF files (*.pdf)|*.pdf");

                if (result.IsSuccess && !string.IsNullOrEmpty(result.Data))
                {
                    IsLoading = true;
                    
                    var exportResult = await _attendanceService.ExportEmployeeListAsync(
                        FilteredEmployees.ToList(), 
                        result.Data);
                    
                    if (exportResult.IsSuccess)
                    {
                        await _dialogService.ShowMessageAsync("Thành công", 
                            $"Danh sách nhân viên đã được xuất thành công!\nĐường dẫn: {result.Data}");
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync("Lỗi", 
                            $"Không thể xuất danh sách: {exportResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employee list");
                await _dialogService.ShowErrorAsync("Lỗi", "Có lỗi xảy ra khi xuất danh sách nhân viên.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeDepartmentFilters()
        {
            DepartmentFilters.Clear();
            
            // Add "All" filter
            DepartmentFilters.Add(new DepartmentFilter
            {
                Id = -1,
                Name = "Tất cả",
                IsSelected = true
            });
            
            // Add specific department filters
            var departments = new[]
            {
                new { Id = 1, Name = "Hành chính" },
                new { Id = 2, Name = "Kỹ thuật" },
                new { Id = 3, Name = "Kinh doanh" },
                new { Id = 4, Name = "Nhân sự" },
                new { Id = 5, Name = "Kế toán" }
            };
            
            foreach (var dept in departments)
            {
                var filter = new DepartmentFilter
                {
                    Id = dept.Id,
                    Name = dept.Name,
                    IsSelected = false
                };
                
                // Subscribe to property changes
                filter.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(DepartmentFilter.IsSelected))
                    {
                        UpdateFilteredEmployees();
                    }
                };
                
                DepartmentFilters.Add(filter);
            }
        }

        private void UpdateFilteredEmployees()
        {
            if (AllEmployees == null) return;

            var filtered = AllEmployees.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.ToLowerInvariant();
                filtered = filtered.Where(e => 
                    e.UserName.ToLowerInvariant().Contains(searchTerm) ||
                    e.DIN.ToString().Contains(searchTerm) ||
                    (e.IDNumber?.ToLowerInvariant().Contains(searchTerm) ?? false));
            }

            // Apply department filter
            var selectedDepartments = DepartmentFilters.Where(f => f.IsSelected && f.Id != -1).ToList();
            if (selectedDepartments.Any())
            {
                var selectedDepartmentIds = selectedDepartments.Select(f => f.Id).ToHashSet();
                filtered = filtered.Where(e => selectedDepartmentIds.Contains(e.Department));
            }

            // Update filtered collection
            FilteredEmployees.Clear();
            foreach (var employee in filtered.OrderBy(e => e.UserName))
            {
                FilteredEmployees.Add(employee);
            }
        }

        private async Task LoadSampleEmployeesAsync()
        {
            try
            {
                // Generate sample employee data for demo
                var sampleEmployees = new List<Employee>();
                var random = new Random();
                
                var sampleNames = new[]
                {
                    "Nguyễn Văn An", "Trần Thị Bình", "Lê Văn Cường", "Phạm Thị Dung",
                    "Hoàng Văn Em", "Đặng Thị Phương", "Võ Văn Giang", "Bùi Thị Hạnh",
                    "Đinh Văn Ích", "Ngô Thị Kim", "Lý Văn Long", "Vũ Thị Mai",
                    "Tô Văn Nam", "Đỗ Thị Oanh", "Hồ Văn Phúc", "Chu Thị Quỳnh"
                };
                
                for (int i = 0; i < sampleNames.Length; i++)
                {
                    var employee = new Employee
                    {
                        DIN = (ulong)(1000 + i),
                        UserName = sampleNames[i],
                        IDNumber = $"NV{1000 + i:D4}",
                        Sex = random.Next(2) == 0 ? "Nam" : "Nữ",
                        Enable = random.Next(10) > 1, // 90% enabled
                        Comment = random.Next(3) == 0 ? "Ghi chú mẫu" : "",
                        Department = random.Next(1, 6),
                        DepartmentName = GetDepartmentName(random.Next(1, 6)),
                        AttType = 0,
                        Privilege = random.Next(3),
                        AccessControl = 1,
                        AccessTimeZone = 1,
                        ValidDate = DateTime.Now.AddMonths(-6),
                        InvalidDate = DateTime.Now.AddYears(1),
                        UserGroup = 1,
                        Password = random.Next(2) == 0 ? "****" : "",
                        CardID = random.Next(2) == 0 ? $"CARD{1000 + i}" : "",
                        Fingerprints = new byte[random.Next(0, 5)][],
                        LastActivity = DateTime.Now.AddMinutes(-random.Next(0, 1440))
                    };
                    
                    sampleEmployees.Add(employee);
                }
                
                AllEmployees.Clear();
                foreach (var employee in sampleEmployees)
                {
                    AllEmployees.Add(employee);
                }
                
                LastRefreshTime = DateTime.Now;
                _logger.LogInformation($"Loaded {AllEmployees.Count} sample employees");
                
                await Task.Delay(100); // Simulate async operation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sample employees");
            }
        }

        private Employee MapUserToEmployee(Riss.Devices.User user)
        {
            return new Employee
            {
                DIN = user.DIN,
                UserName = user.UserName,
                IDNumber = user.IDNumber,
                Sex = user.Sex.ToString(),
                Enable = user.Enable,
                Comment = user.Comment,
                Department = user.Department,
                DepartmentName = GetDepartmentName(user.Department),
                AttType = user.AttType,
                Privilege = user.Privilege,
                AccessControl = user.AccessControl,
                AccessTimeZone = user.AccessTimeZone,
                ValidDate = user.ValidDate,
                InvalidDate = user.InvalidDate,
                UserGroup = user.UserGroup,
                Password = user.Enrolls?.FirstOrDefault()?.Password ?? "",
                CardID = user.Enrolls?.FirstOrDefault()?.CardID ?? "",
                Fingerprints = user.Enrolls?.Where(e => e.Fingerprint != null)
                                           .Select(e => e.Fingerprint)
                                           .ToArray() ?? new byte[0][],
                LastActivity = DateTime.Now.AddMinutes(-new Random().Next(0, 1440))
            };
        }

        private string GetDepartmentName(int departmentId)
        {
            return departmentId switch
            {
                1 => "Hành chính",
                2 => "Kỹ thuật",
                3 => "Kinh doanh",
                4 => "Nhân sự",
                5 => "Kế toán",
                _ => "Không xác định"
            };
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}