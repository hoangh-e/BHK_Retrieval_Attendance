using System;
using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Service interface cho navigation giữa các views
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate tới một view theo tên
        /// </summary>
        /// <param name="viewName">Tên của view cần navigate tới</param>
        void NavigateTo(string viewName);

        /// <summary>
        /// Navigate tới một view với parameters
        /// </summary>
        /// <param name="viewName">Tên của view</param>
        /// <param name="parameter">Parameters truyền vào</param>
        void NavigateTo(string viewName, object parameter);

        /// <summary>
        /// Navigate async
        /// </summary>
        Task NavigateToAsync(string viewName);

        /// <summary>
        /// Navigate async với parameters
        /// </summary>
        Task NavigateToAsync(string viewName, object parameter);

        /// <summary>
        /// Quay lại view trước đó
        /// </summary>
        void GoBack();

        /// <summary>
        /// Kiểm tra có thể go back hay không
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Clear navigation history
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Event khi navigation thay đổi
        /// </summary>
        event EventHandler<string>? Navigated;
    }
}