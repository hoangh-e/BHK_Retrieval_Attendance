using System;
using System.Threading.Tasks;
using System.Windows;
using BHK.Retrieval.Attendance.WPF.Services.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace BHK.Retrieval.Attendance.WPF.Services.Implementations
{
    /// <summary>
    /// Service quản lý các dialog và message box
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly ILogger<DialogService> _logger;

        public DialogService(ILogger<DialogService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Hiển thị message thông thường
        /// </summary>
        public async Task ShowMessageAsync(string title, string message, string buttonText = "OK")
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    // TODO: Implement Material Design dialog
                    /*
                    var view = new MessageDialog
                    {
                        Title = title,
                        Message = message,
                        ButtonText = buttonText
                    };
                    await DialogHost.Show(view, "RootDialog");
                    */

                    // Tạm thời dùng MessageBox
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing message dialog");
            }
        }

        /// <summary>
        /// Hiển thị message lỗi
        /// </summary>
        public async Task ShowErrorAsync(string title, string message)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    // TODO: Implement Material Design error dialog
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing error dialog");
            }
        }

        /// <summary>
        /// Hiển thị message cảnh báo
        /// </summary>
        public async Task ShowWarningAsync(string title, string message)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    // TODO: Implement Material Design warning dialog
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing warning dialog");
            }
        }

        /// <summary>
        /// Hiển thị dialog xác nhận với Yes/No
        /// </summary>
        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            try
            {
                return await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // TODO: Implement Material Design confirmation dialog
                    var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    return result == MessageBoxResult.Yes;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing confirmation dialog");
                return false;
            }
        }

        /// <summary>
        /// Hiển thị dialog nhập liệu
        /// </summary>
        public async Task<string> ShowInputDialogAsync(string title, string message, string defaultValue = "")
        {
            try
            {
                return await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // TODO: Implement Material Design input dialog
                    /*
                    var view = new InputDialog
                    {
                        Title = title,
                        Message = message,
                        DefaultValue = defaultValue
                    };
                    var result = await DialogHost.Show(view, "RootDialog");
                    return result?.ToString() ?? string.Empty;
                    */

                    // Tạm thời return default
                    return defaultValue;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing input dialog");
                return string.Empty;
            }
        }

        /// <summary>
        /// Hiển thị notification toast
        /// </summary>
        public void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // TODO: Implement notification toast với Material Design Snackbar
                    /*
                    var messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
                    messageQueue.Enqueue(message);
                    */

                    _logger.LogInformation("Notification: {Title} - {Message} ({Type})", title, message, type);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing notification");
            }
        }
    }
}
