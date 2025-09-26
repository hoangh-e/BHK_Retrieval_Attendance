using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.WPF.Models;
using Riss.Devices;

namespace BHK.Retrieval.Attendance.WPF.Services
{
    public interface IDeviceService
    {
        bool IsConnected { get; }
        event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        event EventHandler<DeviceDataReceivedEventArgs>? DeviceDataReceived;

        Task<ServiceResult<bool>> ConnectAsync(DeviceConnectionConfig config);
        Task<ServiceResult<bool>> DisconnectAsync();
        Task<ServiceResult<bool>> TestConnectionAsync(DeviceConnectionConfig config);
        Task<ServiceResult<DeviceInfo>> GetDeviceInfoAsync();
        Task<ServiceResult<List<UserInfo>>> GetAllEmployeesAsync();
        Task<ServiceResult<List<UserInfo>>> SynchronizeEmployeeDataAsync();
    }

    public interface INavigationService
    {
        void NavigateTo(object view);
        void GoBack();
        void GoForward();
        bool CanGoBack { get; }
        bool CanGoForward { get; }
    }

    public interface IDialogService
    {
        Task<bool> ShowConfirmationAsync(string title, string message);
        Task ShowInformationAsync(string title, string message);
        Task ShowErrorAsync(string title, string message);
        Task<string?> ShowInputAsync(string title, string message);
    }

    public interface IUserService
    {
        Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<bool>> LogoutAsync();
        LoginResponse? CurrentUser { get; }
        bool IsLoggedIn { get; }
    }

    public interface IAttendanceService
    {
        Task<ServiceResult<List<object>>> GetAttendanceRecordsAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<ServiceResult<bool>> ExportAttendanceAsync(List<object> records, string filePath);
    }
}