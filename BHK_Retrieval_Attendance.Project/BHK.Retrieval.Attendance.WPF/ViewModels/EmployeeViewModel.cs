using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.Core.DTOs.Responses;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho trang quản lý nhân viên
    /// </summary>
    public class EmployeeViewModel : BaseViewModel
    {
        #region Fields

        private readonly IDeviceService _deviceService;
        private readonly ILogger<EmployeeViewModel> _logger;

        private ObservableCollection<EmployeeDisplayModel> _employees = [];
        private EmployeeDetailModel? _selectedEmployee;
        private int _currentPage;
        private int _totalPages;
        private int _totalEmployees;
        private bool _isLoading;
        private string _searchKeyword = string.Empty;

        private const int PAGE_SIZE = 10;

        #endregion

        #region Properties

        /// <summary>
        /// Danh sách nhân viên hiển thị
        /// </summary>
        public ObservableCollection<EmployeeDisplayModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        /// <summary>
        /// Nhân viên được chọn để xem chi tiết
        /// </summary>
        public EmployeeDetailModel SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (SetProperty(ref _selectedEmployee, value))
                {
                    OnPropertyChanged(nameof(IsEmployeeSelected));
                }
            }
        }

        /// <summary>
        /// Trang hiện tại (bắt đầu từ 1)
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnPropertyChanged(nameof(CurrentPageDisplay));
                    OnPropertyChanged(nameof(CanGoPrevious));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        /// <summary>
        /// Tổng số nhân viên
        /// </summary>
        public int TotalEmployees
        {
            get => _totalEmployees;
            set
            {
                if (SetProperty(ref _totalEmployees, value))
                {
                    OnPropertyChanged(nameof(TotalEmployeesDisplay));
                }
            }
        }

        /// <summary>
        /// Đang tải dữ liệu
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetProperty(ref _searchKeyword, value);
        }

        /// <summary>
        /// Có nhân viên được chọn không
        /// </summary>
        public bool IsEmployeeSelected => SelectedEmployee != null;

        /// <summary>
        /// Có thể chuyển trang trước không
        /// </summary>
        public bool CanGoPrevious => CurrentPage > 1;

        /// <summary>
        /// Có thể chuyển trang sau không
        /// </summary>
        public bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// Hiển thị trang hiện tại
        /// </summary>
        public string CurrentPageDisplay => $"Trang {CurrentPage}/{TotalPages}";

        /// <summary>
        /// Hiển thị tổng số nhân viên
        /// </summary>
        public string TotalEmployeesDisplay => $"Tổng số: {TotalEmployees} nhân viên";

        #endregion

        #region Commands

        public ICommand LoadEmployeesCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand ViewEmployeeDetailCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSelectionCommand { get; }

        #endregion

        #region Constructor

        public EmployeeViewModel(
            IDeviceService deviceService,
            ILogger<EmployeeViewModel> logger)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Initialize collections
            Employees = new ObservableCollection<EmployeeDisplayModel>();
            CurrentPage = 1;
            TotalPages = 1;

            // Initialize commands
            LoadEmployeesCommand = new RelayCommand(async _ => await LoadEmployeesAsync());
            FirstPageCommand = new RelayCommand(_ => GoToFirstPage(), _ => CanGoPrevious);
            PreviousPageCommand = new RelayCommand(_ => GoToPreviousPage(), _ => CanGoPrevious);
            NextPageCommand = new RelayCommand(_ => GoToNextPage(), _ => CanGoNext);
            LastPageCommand = new RelayCommand(_ => GoToLastPage(), _ => CanGoNext);
            ViewEmployeeDetailCommand = new RelayCommand(async param => await ViewEmployeeDetailAsync(param));
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync());
            SearchCommand = new RelayCommand(async _ => await SearchEmployeesAsync());
            ClearSelectionCommand = new RelayCommand(_ => ClearSelection());

            _logger.LogInformation("EmployeeViewModel initialized");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tải danh sách nhân viên theo trang
        /// </summary>
        public async Task LoadEmployeesAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                _logger.LogInformation($"Loading employees page {CurrentPage}");

                // Lấy tổng số nhân viên từ thiết bị
                var allUsers = await _deviceService.GetAllUsersAsync();

                if (allUsers == null || !allUsers.Any())
                {
                    _logger.LogWarning("No employees found in device");
                    Employees.Clear();
                    TotalEmployees = 0;
                    TotalPages = 0;
                    return;
                }

                // Tính toán phân trang
                TotalEmployees = allUsers.Count;
                TotalPages = (int)Math.Ceiling(TotalEmployees / (double)PAGE_SIZE);

                // Đảm bảo CurrentPage hợp lệ
                if (CurrentPage > TotalPages)
                    CurrentPage = TotalPages;
                if (CurrentPage < 1)
                    CurrentPage = 1;

                // Lấy nhân viên cho trang hiện tại
                var pageUsers = allUsers
                    .Skip((CurrentPage - 1) * PAGE_SIZE)
                    .Take(PAGE_SIZE)
                    .ToList();

                // Cập nhật UI
                Employees.Clear();
                foreach (var user in pageUsers)
                {
                    Employees.Add(new EmployeeDisplayModel
                    {
                        DIN = user.DIN.ToString(),
                        UserName = user.UserName ?? "N/A",
                        IDNumber = user.IDNumber ?? "N/A",
                        Department = GetDepartmentName(user.DeptId),
                        Enable = user.Enable,
                        Privilege = GetPrivilegeName(user.Privilege),
                        // Nếu không có ngày tạo trong User, dùng ValidDate
                        CreatedDate = user.ValidDate != DateTime.MinValue 
                            ? user.ValidDate 
                            : DateTime.Now
                    });
                }

                _logger.LogInformation($"Loaded {Employees.Count} employees for page {CurrentPage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load employees");
                ShowErrorMessage("Không thể tải danh sách nhân viên", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Xem chi tiết nhân viên
        /// </summary>
        private async Task ViewEmployeeDetailAsync(object parameter)
        {
            if (parameter is not EmployeeDisplayModel employee)
                return;

            try
            {
                IsLoading = true;
                _logger.LogInformation($"Loading details for employee DIN: {employee.DIN}");

                // Lấy thông tin chi tiết từ thiết bị
                var userDetail = await _deviceService.GetUserByIdAsync(ulong.Parse(employee.DIN));

                if (userDetail == null)
                {
                    ShowWarningMessage("Không tìm thấy thông tin chi tiết nhân viên");
                    return;
                }

                // Tạo model chi tiết
                SelectedEmployee = new EmployeeDetailModel
                {
                    DIN = userDetail.DIN.ToString(),
                    UserName = userDetail.UserName ?? "N/A",
                    IDNumber = userDetail.IDNumber ?? "N/A",
                    DeptId = userDetail.DeptId ?? "N/A",
                    Department = GetDepartmentName(userDetail.DeptId),
                    Privilege = GetPrivilegeName(userDetail.Privilege),
                    PrivilegeValue = userDetail.Privilege,
                    Enable = userDetail.Enable,
                    Sex = userDetail.Sex.ToString(),
                    Birthday = userDetail.Birthday,
                    Comment = userDetail.Comment ?? "Không có ghi chú",
                    AccessControl = GetAccessControlText(userDetail.AccessControl),
                    ValidityPeriod = userDetail.ValidityPeriod,
                    ValidDate = userDetail.ValidDate,
                    InvalidDate = userDetail.InvalidDate,
                    UserGroup = userDetail.UserGroup,
                    AccessTimeZone = userDetail.AccessTimeZone,
                    AttType = userDetail.AttType,
                    
                    // Thông tin đăng ký
                    HasFingerprint = HasEnrollment(userDetail, 0), // FP0 = 0
                    HasPassword = HasEnrollment(userDetail, 10), // PWD = 10
                    HasCard = HasEnrollment(userDetail, 11), // Card = 11
                    FingerprintCount = CountFingerprints(userDetail)
                };

                _logger.LogInformation($"Loaded details for employee: {SelectedEmployee.UserName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load employee details for DIN: {employee.DIN}");
                ShowErrorMessage("Không thể tải thông tin chi tiết nhân viên", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Làm mới danh sách
        /// </summary>
        private async Task RefreshAsync()
        {
            CurrentPage = 1;
            SelectedEmployee = null;
            await LoadEmployeesAsync();
        }

        /// <summary>
        /// Tìm kiếm nhân viên
        /// </summary>
        private async Task SearchEmployeesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadEmployeesAsync();
                return;
            }

            try
            {
                IsLoading = true;
                _logger.LogInformation($"Searching employees with keyword: {SearchKeyword}");

                // Lấy tất cả nhân viên và lọc
                var allUsers = await _deviceService.GetAllUsersAsync();
                var filteredUsers = allUsers.Where(u =>
                    u.UserName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true ||
                    u.DIN.ToString().Contains(SearchKeyword) ||
                    u.IDNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                // Cập nhật phân trang
                TotalEmployees = filteredUsers.Count;
                TotalPages = (int)Math.Ceiling(TotalEmployees / (double)PAGE_SIZE);
                CurrentPage = 1;

                // Hiển thị kết quả trang đầu
                var pageUsers = filteredUsers.Take(PAGE_SIZE).ToList();
                
                Employees.Clear();
                foreach (var user in pageUsers)
                {
                    Employees.Add(new EmployeeDisplayModel
                    {
                        DIN = user.DIN.ToString(),
                        UserName = user.UserName ?? "N/A",
                        IDNumber = user.IDNumber ?? "N/A",
                        Department = GetDepartmentName(user.DeptId),
                        Enable = user.Enable,
                        Privilege = GetPrivilegeName(user.Privilege),
                        CreatedDate = user.ValidDate != DateTime.MinValue ? user.ValidDate : DateTime.Now
                    });
                }

                _logger.LogInformation($"Found {TotalEmployees} employees matching keyword: {SearchKeyword}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search employees");
                ShowErrorMessage("Không thể tìm kiếm nhân viên", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Xóa lựa chọn nhân viên
        /// </summary>
        private void ClearSelection()
        {
            SelectedEmployee = null;
        }

        #endregion

        #region Pagination Methods

        private void GoToFirstPage()
        {
            CurrentPage = 1;
            _ = LoadEmployeesAsync();
        }

        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                _ = LoadEmployeesAsync();
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                _ = LoadEmployeesAsync();
            }
        }

        private void GoToLastPage()
        {
            CurrentPage = TotalPages;
            _ = LoadEmployeesAsync();
        }

        #endregion

        #region Helper Methods

        private static string GetDepartmentName(string? deptId)
        {
            // TODO: Implement department mapping
            return string.IsNullOrEmpty(deptId) ? "Chưa có" : deptId;
        }

        private string GetPrivilegeName(int privilege)
        {
            return privilege switch
            {
                1 => "Người dùng",
                2 => "Người đăng ký",
                4 => "Truy vấn log",
                8 => "Quản lý",
                16 => "Khách (Reserve)",
                _ => "Không xác định"
            };
        }

        private string GetAccessControlText(int accessControl)
        {
            return accessControl switch
            {
                0 => "Tắt",
                1 => "Khóa 1",
                2 => "Khóa 2",
                3 => "Khóa 1 & 2",
                _ => "Không xác định"
            };
        }

        private static bool HasEnrollment(EmployeeDto user, int enrollType)
        {
            if (user.Enrollments == null || !user.Enrollments.Any())
                return false;

            return user.Enrollments.Any(e => e.EnrollType == enrollType);
        }

        private static int CountFingerprints(EmployeeDto user)
        {
            if (user.Enrollments == null)
                return 0;

            // FP0-FP9 = 0-9 in EnrollType
            return user.Enrollments.Count(e =>
                e.EnrollType >= 0 &&
                e.EnrollType <= 9);
        }

        private void ShowErrorMessage(string title, string message)
        {
            // TODO: Implement DialogService
            _logger.LogError($"{title}: {message}");
        }

        private void ShowWarningMessage(string message)
        {
            // TODO: Implement DialogService
            _logger.LogWarning(message);
        }

        #endregion

        #region RelayCommand Implementation

        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }
        }

        #endregion
    }

    #region Display Models

    /// <summary>
    /// Model hiển thị nhân viên trong danh sách
    /// </summary>
    public class EmployeeDisplayModel
    {
        public string DIN { get; set; }
        public string UserName { get; set; }
        public string IDNumber { get; set; }
        public string Department { get; set; }
        public bool Enable { get; set; }
        public string Privilege { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusText => Enable ? "Hoạt động" : "Vô hiệu hóa";
    }

    /// <summary>
    /// Model chi tiết nhân viên
    /// </summary>
    public class EmployeeDetailModel
    {
        // Thông tin cơ bản
        public string DIN { get; set; }
        public string UserName { get; set; }
        public string IDNumber { get; set; }
        public string DeptId { get; set; }
        public string Department { get; set; }
        public string Sex { get; set; }
        public DateTime Birthday { get; set; }
        public string Comment { get; set; }
        
        // Quyền và trạng thái
        public string Privilege { get; set; }
        public int PrivilegeValue { get; set; }
        public bool Enable { get; set; }
        public string StatusText => Enable ? "Hoạt động" : "Vô hiệu hóa";
        
        // Kiểm soát truy cập
        public string AccessControl { get; set; }
        public bool ValidityPeriod { get; set; }
        public DateTime ValidDate { get; set; }
        public DateTime InvalidDate { get; set; }
        public int UserGroup { get; set; }
        public int AccessTimeZone { get; set; }
        public int AttType { get; set; }
        
        // Thông tin đăng ký
        public bool HasFingerprint { get; set; }
        public bool HasPassword { get; set; }
        public bool HasCard { get; set; }
        public int FingerprintCount { get; set; }
        
        // Display properties
        public string BirthdayDisplay => Birthday != DateTime.MinValue ? Birthday.ToString("dd/MM/yyyy") : "Chưa cập nhật";
        public string ValidDateDisplay => ValidDate != DateTime.MinValue ? ValidDate.ToString("dd/MM/yyyy") : "Không giới hạn";
        public string InvalidDateDisplay => InvalidDate != DateTime.MinValue ? InvalidDate.ToString("dd/MM/yyyy") : "Không giới hạn";
        public string EnrollmentStatus => $"Vân tay: {FingerprintCount} | Mật khẩu: {(HasPassword ? "Có" : "Không")} | Thẻ: {(HasCard ? "Có" : "Không")}";
    }

    #endregion
}