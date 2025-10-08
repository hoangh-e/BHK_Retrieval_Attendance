using Ookii.Dialogs.Wpf;
using System;
using System.Windows;

namespace BHK.Retrieval.Attendance.WPF.Utilities
{
    /// <summary>
    /// Helper class for displaying various types of dialogs
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Shows a success message dialog
        /// </summary>
        /// <param name="message">The success message to display</param>
        /// <param name="title">The dialog title (default: "Thành công")</param>
        public static void ShowSuccess(string message, string title = "Thành công")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Information,
                    ButtonStyle = TaskDialogButtonStyle.Standard
                };
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                dialog.ShowDialog();
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Shows an error message dialog
        /// </summary>
        /// <param name="message">The error message to display</param>
        /// <param name="details">Additional error details (optional)</param>
        /// <param name="title">The dialog title (default: "Lỗi")</param>
        public static void ShowError(string message, string details = "", string title = "Lỗi")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Error,
                    ButtonStyle = TaskDialogButtonStyle.Standard,
                    ExpandedInformation = details,
                    ExpandFooterArea = !string.IsNullOrEmpty(details)
                };
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                dialog.ShowDialog();
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                var fullMessage = string.IsNullOrEmpty(details) ? message : $"{message}\n\nChi tiết: {details}";
                MessageBox.Show(fullMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Shows a warning message dialog
        /// </summary>
        /// <param name="message">The warning message to display</param>
        /// <param name="details">Additional warning details (optional)</param>
        /// <param name="title">The dialog title (default: "Cảnh báo")</param>
        public static void ShowWarning(string message, string details = "", string title = "Cảnh báo")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Warning,
                    ButtonStyle = TaskDialogButtonStyle.Standard,
                    ExpandedInformation = details,
                    ExpandFooterArea = !string.IsNullOrEmpty(details)
                };
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                dialog.ShowDialog();
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                var fullMessage = string.IsNullOrEmpty(details) ? message : $"{message}\n\nChi tiết: {details}";
                MessageBox.Show(fullMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        /// <param name="message">The confirmation message to display</param>
        /// <param name="title">The dialog title (default: "Xác nhận")</param>
        /// <returns>True if user confirmed, false otherwise</returns>
        public static bool ShowConfirmation(string message, string title = "Xác nhận")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Warning,
                    ButtonStyle = TaskDialogButtonStyle.Standard
                };

                var yesButton = new TaskDialogButton(ButtonType.Yes);
                var noButton = new TaskDialogButton(ButtonType.No);
                dialog.Buttons.Add(yesButton);
                dialog.Buttons.Add(noButton);

                var result = dialog.ShowDialog();
                return result == yesButton;
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }
        }

        /// <summary>
        /// Shows a confirmation dialog with custom button text
        /// </summary>
        /// <param name="message">The confirmation message to display</param>
        /// <param name="yesText">Text for the positive button</param>
        /// <param name="noText">Text for the negative button</param>
        /// <param name="title">The dialog title (default: "Xác nhận")</param>
        /// <returns>True if user confirmed, false otherwise</returns>
        public static bool ShowConfirmation(string message, string yesText, string noText, string title = "Xác nhận")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Warning,
                    ButtonStyle = TaskDialogButtonStyle.CommandLinks
                };

                var yesButton = new TaskDialogButton { Text = yesText };
                var noButton = new TaskDialogButton { Text = noText };
                dialog.Buttons.Add(yesButton);
                dialog.Buttons.Add(noButton);

                var result = dialog.ShowDialog();
                return result == yesButton;
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }
        }

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        /// <param name="message">The information message to display</param>
        /// <param name="title">The dialog title (default: "Thông tin")</param>
        public static void ShowInformation(string message, string title = "Thông tin")
        {
            try
            {
                var dialog = new TaskDialog
                {
                    WindowTitle = title,
                    MainInstruction = title,
                    Content = message,
                    MainIcon = TaskDialogIcon.Information,
                    ButtonStyle = TaskDialogButtonStyle.Standard
                };
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                dialog.ShowDialog();
            }
            catch (Exception)
            {
                // Fallback to standard MessageBox if TaskDialog fails
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}