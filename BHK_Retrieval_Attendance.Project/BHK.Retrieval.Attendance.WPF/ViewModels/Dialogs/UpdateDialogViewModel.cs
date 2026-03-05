using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Models;

namespace BHK.Retrieval.Attendance.WPF.ViewModels.Dialogs;

/// <summary>
/// ViewModel cho dialog thông báo cập nhật
/// </summary>
public partial class UpdateDialogViewModel : ObservableObject
{
    private readonly IUpdateService _updateService;
    private readonly ILogger<UpdateDialogViewModel> _logger;
    private Window? _dialog;

    [ObservableProperty]
    private string _newVersion = string.Empty;

    [ObservableProperty]
    private string _releaseNotes = string.Empty;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private int _downloadProgress;

    [ObservableProperty]
    private string _statusMessage = "Đã có phiên bản mới!";

    public UpdateInfo? UpdateInfo { get; private set; }
    public bool ShouldInstall { get; private set; }

    public UpdateDialogViewModel(
        IUpdateService updateService,
        ILogger<UpdateDialogViewModel> logger)
    {
        _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Set thông tin cập nhật
    /// </summary>
    public void SetUpdateInfo(UpdateInfo updateInfo)
    {
        UpdateInfo = updateInfo;
        NewVersion = updateInfo.Version;
        ReleaseNotes = updateInfo.Notes;
    }

    /// <summary>
    /// Set dialog window reference
    /// </summary>
    public void SetDialog(Window dialog)
    {
        _dialog = dialog;
    }

    /// <summary>
    /// Cập nhật ngay
    /// </summary>
    [RelayCommand]
    private async Task UpdateNow()
    {
        if (UpdateInfo == null)
            return;

        try
        {
            IsDownloading = true;
            StatusMessage = "Đang tải cập nhật...";

            var progress = new Progress<int>(percent =>
            {
                DownloadProgress = percent;
                StatusMessage = $"Đang tải... {percent}%";
            });

            var success = await _updateService.DownloadAndInstallUpdateAsync(UpdateInfo, progress);

            if (success)
            {
                StatusMessage = "Đang khởi động cài đặt...";
                ShouldInstall = true;
                
                // Đợi 1 giây rồi đóng app
                await Task.Delay(1000);
                _dialog?.Close();
                
                // Đóng ứng dụng để cài đặt
                Application.Current.Shutdown();
            }
            else
            {
                StatusMessage = "Lỗi khi tải cập nhật. Vui lòng thử lại sau.";
                IsDownloading = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading update");
            StatusMessage = $"Lỗi: {ex.Message}";
            IsDownloading = false;
        }
    }

    /// <summary>
    /// Cập nhật sau
    /// </summary>
    [RelayCommand]
    private void UpdateLater()
    {
        ShouldInstall = false;
        _dialog?.Close();
    }
}
