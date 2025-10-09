using System;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using BHK.Retrieval.Attendance.WPF.ViewModels;
using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// NavigationService - ContentControl pattern (Best practice)
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NavigationService> _logger;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public NavigationService(
            IServiceProvider serviceProvider,
            ILogger<NavigationService> logger,
            MainWindowViewModel mainWindowViewModel)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
        }

        public BaseViewModel CurrentViewModel => _mainWindowViewModel.CurrentPage as BaseViewModel;

        public event EventHandler<BaseViewModel> Navigated;

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            try
            {
                _logger.LogInformation("Navigating to {ViewModelType}", typeof(TViewModel).Name);

                // Resolve ViewModel từ DI container
                var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

                // Thay đổi CurrentPage → WPF tự động render View tương ứng
                _mainWindowViewModel.CurrentPage = viewModel;

                // Trigger event
                Navigated?.Invoke(this, viewModel);

                _logger.LogInformation("Navigation to {ViewModelType} successful", typeof(TViewModel).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Navigation to {ViewModelType} failed", typeof(TViewModel).Name);
                throw;
            }
        }
    }
}