using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho MainWindow - quản lý navigation giữa các views
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ILogger<MainWindowViewModel> _logger;
        private BaseViewModel _currentViewModel;

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("MainWindowViewModel created");
        }

        /// <summary>
        /// ViewModel hiện tại đang được hiển thị
        /// Khi thay đổi property này, WPF tự động đổi View tương ứng
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (SetProperty(ref _currentViewModel, value))
                {
                    _logger.LogInformation("CurrentViewModel changed to: {ViewModelType}", 
                        value?.GetType().Name ?? "null");
                }
            }
        }
    }
}
