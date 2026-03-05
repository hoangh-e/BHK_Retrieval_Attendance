namespace BHK.Retrieval.Attendance.Core.Models;

/// <summary>
/// Thông tin phiên bản cập nhật
/// </summary>
public class UpdateInfo
{
    /// <summary>
    /// Phiên bản mới (ví dụ: "1.0.1")
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// URL download file MSI
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú phát hành (release notes)
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Kiểm tra phiên bản này có mới hơn phiên bản hiện tại không
    /// </summary>
    public bool IsNewerThan(string currentVersion)
    {
        try
        {
            var current = System.Version.Parse(currentVersion);
            var remote = System.Version.Parse(Version);
            return remote > current;
        }
        catch
        {
            return false;
        }
    }
}
