using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Service interface cho thông báo
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Hiển thị thông báo thành công
        /// </summary>
        Task ShowSuccessAsync(string title, string message);

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        Task ShowErrorAsync(string title, string message);

        /// <summary>
        /// Hiển thị thông báo cảnh báo
        /// </summary>
        Task ShowWarningAsync(string title, string message);

        /// <summary>
        /// Hiển thị thông báo thông tin
        /// </summary>
        Task ShowInfoAsync(string title, string message);
    }
}
