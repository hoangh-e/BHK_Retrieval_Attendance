using BHK.Retrieval.Attendance.Core.Models;

namespace BHK.Retrieval.Attendance.Core.Interfaces;

/// <summary>
/// Service kiểm tra và tải cập nhật ứng dụng
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// Kiểm tra có phiên bản mới hay không
    /// </summary>
    /// <param name="currentVersion">Phiên bản hiện tại</param>
    /// <returns>Thông tin cập nhật nếu có phiên bản mới, null nếu không</returns>
    Task<UpdateInfo?> CheckForUpdateAsync(string currentVersion);

    /// <summary>
    /// Tải MSI và khởi động cài đặt
    /// </summary>
    /// <param name="updateInfo">Thông tin cập nhật</param>
    /// <param name="progress">Báo cáo tiến trình tải (0-100)</param>
    /// <returns>True nếu thành công</returns>
    Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo, IProgress<int>? progress = null);
}
