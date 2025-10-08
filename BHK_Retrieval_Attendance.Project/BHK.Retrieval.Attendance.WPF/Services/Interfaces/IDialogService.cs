using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Interface cho Dialog Service - quản lý các dialog và message box
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Hiển thị message thông thường
        /// </summary>
        Task ShowMessageAsync(string title, string message, string buttonText = "OK");

        /// <summary>
        /// Hiển thị message lỗi
        /// </summary>
        Task ShowErrorAsync(string title, string message);

        /// <summary>
        /// Hiển thị message cảnh báo
        /// </summary>
        Task ShowWarningAsync(string title, string message);

        /// <summary>
        /// Hiển thị dialog xác nhận với Yes/No
        /// </summary>
        Task<bool> ShowConfirmationAsync(string title, string message);

        /// <summary>
        /// Hiển thị dialog nhập liệu
        /// </summary>
        Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "");

        /// <summary>
        /// Hiển thị notification toast
        /// </summary>
        void ShowNotification(string title, string message, NotificationType type = NotificationType.Information);
    }

    /// <summary>
    /// Loại notification
    /// </summary>
    public enum NotificationType
    {
        Information,
        Success,
        Warning,
        Error
    }
}
