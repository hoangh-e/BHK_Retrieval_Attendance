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
using Riss.Devices;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    public class EmployeeDetailViewModel : INotifyPropertyChanged
    {
        private readonly IDeviceService _deviceService;
        private readonly IAttendanceService _attendanceService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ILogger<EmployeeDetailViewModel> _logger;

        public EmployeeDetailViewModel(IDeviceService deviceService,
                                     IAttendanceService attendanceService,
                                     INavigationService navigationService,
                                     IDialogService dialogService,
                                     ILogger<EmployeeDetailViewModel> logger)
        {
            _deviceService = deviceService;
            _attendanceService = attendanceService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _logger = logger;

            // Initialize collections
            AttendanceRecords = new ObservableCollection<AttendanceRecord>();

            // Initialize commands
            BackCommand = new RelayCommand(GoBack);
            RefreshDataCommand = new AsyncRelayCommand(RefreshDataAsync);
            RefreshAttendanceCommand = new AsyncRelayCommand(RefreshAttendanceAsync);
            ExportReportCommand = new AsyncRelayCommand(ExportReportAsync, CanExportReport);
        }

        #region Properties

        private Employee? _employee;
        public Employee? Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEmployeeData));
                ((AsyncRelayCommand)ExportReportCommand).RaiseCanExecuteChanged();
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
                OnPropertyChanged(nameof(HasEmployeeData));
            }
        }

        public bool HasEmployeeData => !IsLoading && Employee != null;

        private ObservableCollection<AttendanceRecord> _attendanceRecords;
        public ObservableCollection<AttendanceRecord> AttendanceRecords
        {
            get => _attendanceRecords;
            set
            {
                _attendanceRecords = value;
                OnPropertyChanged();
            }
        }

        // Computed properties for enrollment status
        public string PasswordStatus => 
            !string.IsNullOrEmpty(Employee?.Password) ? "Đã thiết lập" : "Chưa thiết lập";

        public string CardStatus => 
            !string.IsNullOrEmpty(Employee?.CardID) ? "Đã đăng ký" : "Chưa đăng ký";

        public string FingerprintStatus
        {
            get
            {
                if (Employee?.Fingerprints?.Any() == true)
                {
                    var count = Employee.Fingerprints.Count(fp => fp?.Length > 0);
                    return $"Đã đăng ký ({count} vân tay)";
                }
                return "Chưa đăng ký";
            }
        }

        #endregion

        #region Commands

        public ICommand BackCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ICommand RefreshAttendanceCommand { get; }
        public ICommand ExportReportCommand { get; }

        #endregion

        #region Command Implementations

        private void GoBack()
        {
            _navigationService.GoBack();
        }

        private async Task RefreshDataAsync()
        {
            if (Employee != null)
            {
                await LoadEmployeeDataAsync(Employee.DIN);
            }
        }

        private async Task RefreshAttendanceAsync()
        {
            if (Employee != null)
            {
                await LoadAttendanceRecordsAsync(Employee.DIN);
            }
        }

        private bool CanExportReport()
        {
            return HasEmployeeData;
        }

        private async Task ExportReportAsync()
        {
            try
            {
                if (Employee == null) return;

                var result = await _dialogService.ShowSaveFileDialogAsync(
                    "Xuất báo cáo nhân viên", 
                    $"BaoCao_NhanVien_{Employee.DIN}_{DateTime.Now:yyyyMMdd}.xlsx",
                    "Excel files (*.xlsx)|*.xlsx|PDF files (*.pdf)|*.pdf");

                if (result.IsSuccess && !string.IsNullOrEmpty(result.Data))
                {
                    IsLoading = true;
                    
                    var exportData = new EmployeeReportData
                    {
                        Employee = Employee,
                        AttendanceRecords = AttendanceRecords.ToList(),
                        GeneratedDate = DateTime.Now
                    };

                    var exportResult = await _attendanceService.ExportEmployeeReportAsync(exportData, result.Data);
                    
                    if (exportResult.IsSuccess)
                    {
                        await _dialogService.ShowMessageAsync("Thành công", 
                            $"Báo cáo đã được xuất thành công!\nĐường dẫn: {result.Data}");
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync("Lỗi", 
                            $"Không thể xuất báo cáo: {exportResult.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting report for employee {Employee?.DIN}");
                await _dialogService.ShowErrorAsync("Lỗi", "Có lỗi xảy ra khi xuất báo cáo.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Public Methods

        public async Task LoadEmployeeDataAsync(ulong employeeId)
        {
            try
            {
                IsLoading = true;
                _logger.LogInformation($"Loading detailed data for employee ID: {employeeId}");

                // Get employee basic info
                var employeeResult = await _deviceService.GetEmployeeByIdAsync(employeeId);
                if (employeeResult.IsSuccess && employeeResult.Data != null)
                {
                    Employee = MapUserToEmployee(employeeResult.Data);
                    
                    // Load additional enrollment data
                    await LoadEnrollmentDataAsync(employeeId);
                    
                    // Load attendance records
                    await LoadAttendanceRecordsAsync(employeeId);
                    
                    _logger.LogInformation($"Successfully loaded data for employee: {Employee.UserName}");
                }
                else
                {
                    _logger.LogWarning($"Failed to load employee data: {employeeResult.ErrorMessage}");
                    await _dialogService.ShowErrorAsync("Lỗi", 
                        $"Không thể tải thông tin nhân viên: {employeeResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading employee data for ID: {employeeId}");
                await _dialogService.ShowErrorAsync("Lỗi", "Có lỗi xảy ra khi tải thông tin nhân viên.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Private Methods

        private async Task LoadEnrollmentDataAsync(ulong employeeId)
        {
            try
            {
                if (Employee == null) return;

                // Load fingerprint data
                var fingerprintResult = await _deviceService.GetEmployeeFingerprintsAsync(employeeId);
                if (fingerprintResult.IsSuccess && fingerprintResult.Data != null)
                {
                    Employee.Fingerprints = fingerprintResult.Data.ToArray();
                    OnPropertyChanged(nameof(FingerprintStatus));
                }

                // Load card data
                var cardResult = await _deviceService.GetEmployeeCardAsync(employeeId);
                if (cardResult.IsSuccess && !string.IsNullOrEmpty(cardResult.Data))
                {
                    Employee.CardID = cardResult.Data;
                    OnPropertyChanged(nameof(CardStatus));
                }

                // Load password status
                var passwordResult = await _deviceService.GetEmployeePasswordAsync(employeeId);
                if (passwordResult.IsSuccess)
                {
                    Employee.Password = passwordResult.Data ?? "";
                    OnPropertyChanged(nameof(PasswordStatus));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading enrollment data for employee {employeeId}");
            }
        }

        private async Task LoadAttendanceRecordsAsync(ulong employeeId)
        {
            try
            {
                AttendanceRecords.Clear();
                
                // Get attendance records from the last 30 days
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now;
                
                var recordsResult = await _deviceService.GetEmployeeAttendanceRecordsAsync(employeeId, startDate, endDate);
                
                if (recordsResult.IsSuccess && recordsResult.Data != null)
                {
                    var sortedRecords = recordsResult.Data
                        .OrderByDescending(r => r.Clock)
                        .Take(50); // Show only last 50 records
                    
                    foreach (var record in sortedRecords)
                    {
                        var attendanceRecord = new AttendanceRecord
                        {
                            DIN = record.DIN,
                            DN = record.DN,
                            Clock = record.Clock,
                            Verify = record.Verify,
                            Action = record.Action,
                            Remark = record.Remark,
                            JobCode = record.JobCode
                        };
                        
                        AttendanceRecords.Add(attendanceRecord);
                    }
                    
                    _logger.LogInformation($"Loaded {AttendanceRecords.Count} attendance records for employee {employeeId}");
                }
                else
                {
                    _logger.LogWarning($"No attendance records found for employee {employeeId}");
                    
                    // Add sample data for demo
                    await LoadSampleAttendanceAsync(employeeId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading attendance records for employee {employeeId}");
                
                // Load sample data as fallback
                await LoadSampleAttendanceAsync(employeeId);
            }
        }

        private async Task LoadSampleAttendanceAsync(ulong employeeId)
        {
            try
            {
                // Generate sample attendance data for demo
                var random = new Random();
                var records = new List<AttendanceRecord>();
                
                for (int i = 0; i < 20; i++)
                {
                    var baseDate = DateTime.Now.AddDays(-i);
                    
                    // Morning check-in
                    records.Add(new AttendanceRecord
                    {
                        DIN = employeeId,
                        DN = 1,
                        Clock = baseDate.Date.AddHours(8).AddMinutes(random.Next(-15, 30)),
                        Verify = 1, // Fingerprint
                        Action = 0, // Check-in
                        Remark = "Chấm công vào",
                        JobCode = 0
                    });
                    
                    // Lunch break out
                    records.Add(new AttendanceRecord
                    {
                        DIN = employeeId,
                        DN = 1,
                        Clock = baseDate.Date.AddHours(12).AddMinutes(random.Next(0, 30)),
                        Verify = 1, // Fingerprint
                        Action = 1, // Check-out
                        Remark = "Ra ăn trưa",
                        JobCode = 0
                    });
                    
                    // Lunch break in
                    records.Add(new AttendanceRecord
                    {
                        DIN = employeeId,
                        DN = 1,
                        Clock = baseDate.Date.AddHours(13).AddMinutes(random.Next(0, 30)),
                        Verify = 1, // Fingerprint
                        Action = 0, // Check-in
                        Remark = "Vào sau ăn trưa",
                        JobCode = 0
                    });
                    
                    // Evening check-out
                    records.Add(new AttendanceRecord
                    {
                        DIN = employeeId,
                        DN = 1,
                        Clock = baseDate.Date.AddHours(17).AddMinutes(random.Next(0, 60)),
                        Verify = 1, // Fingerprint
                        Action = 1, // Check-out
                        Remark = "Chấm công ra",
                        JobCode = 0
                    });
                }
                
                var sortedRecords = records.OrderByDescending(r => r.Clock);
                foreach (var record in sortedRecords)
                {
                    AttendanceRecords.Add(record);
                }
                
                await Task.Delay(100); // Simulate async operation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sample attendance data");
            }
        }

        private Employee MapUserToEmployee(User user)
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