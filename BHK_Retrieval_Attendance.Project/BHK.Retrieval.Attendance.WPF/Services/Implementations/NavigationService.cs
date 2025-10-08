using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service implementation cho navigation giữa các views
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly ILogger<NavigationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<string> _navigationHistory;
        private Frame? _navigationFrame;

        public NavigationService(
            ILogger<NavigationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _navigationHistory = new Stack<string>();
        }

        /// <summary>
        /// Thiết lập Frame để sử dụng cho navigation
        /// </summary>
        public void SetNavigationFrame(Frame frame)
        {
            _navigationFrame = frame ?? throw new ArgumentNullException(nameof(frame));
            _logger.LogInformation("Navigation frame set successfully");
        }

        public event EventHandler<string>? Navigated;

        public bool CanGoBack => _navigationHistory.Count > 1;

        public void NavigateTo(string viewName)
        {
            try
            {
                _logger.LogInformation("Navigating to view: {ViewName}", viewName);

                if (_navigationFrame == null)
                {
                    _logger.LogWarning("Navigation frame is not set. Creating new window for view: {ViewName}", viewName);
                    NavigateToWindow(viewName);
                    return;
                }

                var page = CreatePage(viewName);
                if (page != null)
                {
                    _navigationFrame.Navigate(page);
                    _navigationHistory.Push(viewName);
                    Navigated?.Invoke(this, viewName);
                    _logger.LogInformation("Successfully navigated to: {ViewName}", viewName);
                }
                else
                {
                    _logger.LogError("Failed to create page for view: {ViewName}", viewName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during navigation to: {ViewName}", viewName);
                throw;
            }
        }

        public void NavigateTo(string viewName, object parameter)
        {
            try
            {
                _logger.LogInformation("Navigating to view with parameter: {ViewName}", viewName);

                if (_navigationFrame == null)
                {
                    _logger.LogWarning("Navigation frame is not set");
                    NavigateToWindow(viewName, parameter);
                    return;
                }

                var page = CreatePage(viewName, parameter);
                if (page != null)
                {
                    _navigationFrame.Navigate(page);
                    _navigationHistory.Push(viewName);
                    Navigated?.Invoke(this, viewName);
                    _logger.LogInformation("Successfully navigated to: {ViewName} with parameter", viewName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during navigation to: {ViewName}", viewName);
                throw;
            }
        }

        public async Task NavigateToAsync(string viewName)
        {
            await Task.Run(() => NavigateTo(viewName));
        }

        public async Task NavigateToAsync(string viewName, object parameter)
        {
            await Task.Run(() => NavigateTo(viewName, parameter));
        }

        public void GoBack()
        {
            try
            {
                if (CanGoBack && _navigationFrame != null && _navigationFrame.CanGoBack)
                {
                    _navigationHistory.Pop(); // Remove current
                    var previousView = _navigationHistory.Peek();
                    _navigationFrame.GoBack();
                    Navigated?.Invoke(this, previousView);
                    _logger.LogInformation("Navigated back to: {ViewName}", previousView);
                }
                else
                {
                    _logger.LogWarning("Cannot go back - no previous view in history");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during go back navigation");
                throw;
            }
        }

        public void ClearHistory()
        {
            _navigationHistory.Clear();
            _logger.LogInformation("Navigation history cleared");
        }

        /// <summary>
        /// Tạo Page instance từ view name
        /// </summary>
        private Page? CreatePage(string viewName, object? parameter = null)
        {
            try
            {
                // Map view names to actual page types
                var pageType = GetPageType(viewName);
                if (pageType == null)
                {
                    _logger.LogError("Page type not found for view: {ViewName}", viewName);
                    return null;
                }

                // Resolve page từ DI container
                var page = _serviceProvider.GetService(pageType) as Page;
                
                if (page == null)
                {
                    _logger.LogWarning("Page not registered in DI container, creating new instance: {ViewName}", viewName);
                    page = Activator.CreateInstance(pageType) as Page;
                }

                // Set DataContext nếu có parameter
                if (parameter != null && page != null)
                {
                    page.DataContext = parameter;
                }

                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating page for view: {ViewName}", viewName);
                return null;
            }
        }

        /// <summary>
        /// Get page type từ view name
        /// </summary>
        private Type? GetPageType(string viewName)
        {
            // Map view names to page types
            var mapping = new Dictionary<string, string>
            {
                { "DeviceConnection", "BHK.Retrieval.Attendance.WPF.Views.Pages.DeviceConnectionView" },
                { "Dashboard", "BHK.Retrieval.Attendance.WPF.Views.Pages.DashboardView" },
                { "AttendanceList", "BHK.Retrieval.Attendance.WPF.Views.Pages.AttendanceListView" },
                { "EmployeeList", "BHK.Retrieval.Attendance.WPF.Views.Pages.EmployeeListView" },
                { "Settings", "BHK.Retrieval.Attendance.WPF.Views.Pages.SettingsView" },
                { "HomePage", "BHK.Retrieval.Attendance.WPF.Views.Pages.HomePageView" }
            };

            if (mapping.TryGetValue(viewName, out var typeName))
            {
                return Type.GetType(typeName);
            }

            _logger.LogWarning("View name not found in mapping: {ViewName}", viewName);
            return null;
        }

        /// <summary>
        /// Navigate bằng cách mở window mới (fallback khi không có frame)
        /// </summary>
        private void NavigateToWindow(string viewName, object? parameter = null)
        {
            try
            {
                _logger.LogInformation("Creating new window for view: {ViewName}", viewName);

                // Tạo window tạm thời để hiển thị kết nối thành công
                var window = new Window
                {
                    Title = GetWindowTitle(viewName),
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = CreatePage(viewName, parameter)
                };

                window.Show();
                
                // Đóng window hiện tại (DeviceConnection)
                Application.Current.Windows[0]?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating window for view: {ViewName}", viewName);
            }
        }

        /// <summary>
        /// Lấy title cho window từ view name
        /// </summary>
        private string GetWindowTitle(string viewName)
        {
            return viewName switch
            {
                "Dashboard" => "BHK Attendance - Dashboard",
                "HomePage" => "BHK Attendance - Home",
                "AttendanceList" => "Attendance Records",
                "EmployeeList" => "Employee Management",
                "Settings" => "Settings",
                _ => "BHK Attendance System"
            };
        }
    }
}