using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using BHK.Retrieval.Attendance.Core.Interfaces;
using BHK.Retrieval.Attendance.Core.Models;

namespace BHK.Retrieval.Attendance.Infrastructure.Services;

/// <summary>
/// Service kiểm tra và cập nhật ứng dụng qua MSI installer
/// </summary>
public class UpdateService : IUpdateService
{
    private readonly ILogger<UpdateService> _logger;
    private readonly HttpClient _httpClient;
    private const string UPDATE_URL = "https://hoangh-e.github.io/realand-app-update/version.json";

    public UpdateService(ILogger<UpdateService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Kiểm tra có phiên bản mới
    /// </summary>
    public async Task<UpdateInfo?> CheckForUpdateAsync(string currentVersion)
    {
        try
        {
            _logger.LogInformation("Checking for updates. Current version: {Version}", currentVersion);

            var response = await _httpClient.GetAsync(UPDATE_URL);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updateInfo == null)
            {
                _logger.LogWarning("Failed to parse update information");
                return null;
            }

            if (updateInfo.IsNewerThan(currentVersion))
            {
                _logger.LogInformation("New version available: {NewVersion}", updateInfo.Version);
                return updateInfo;
            }

            _logger.LogInformation("Application is up to date");
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Network error while checking for updates");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for updates");
            return null;
        }
    }

    /// <summary>
    /// Tải MSI và khởi động cài đặt
    /// </summary>
    public async Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo, IProgress<int>? progress = null)
    {
        try
        {
            _logger.LogInformation("Downloading update from {Url}", updateInfo.Url);

            // Tạo temp folder
            var tempFolder = Path.Combine(Path.GetTempPath(), "BHK_Attendance_Update");
            Directory.CreateDirectory(tempFolder);

            var msiPath = Path.Combine(tempFolder, "BHK_Realand_App.msi");

            // Download MSI với progress tracking
            using (var response = await _httpClient.GetAsync(updateInfo.Url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                var downloadedBytes = 0L;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(msiPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0 && progress != null)
                        {
                            var percentage = (int)((downloadedBytes * 100) / totalBytes);
                            progress.Report(percentage);
                        }
                    }
                }
            }

            _logger.LogInformation("Download completed. Starting installer...");

            // Khởi động MSI installer với msiexec
            // /i: install
            // /passive: hiển thị progress bar nhưng không tương tác
            var startInfo = new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = $"/i \"{msiPath}\" /passive",
                UseShellExecute = true,
                Verb = "runas" // Yêu cầu quyền admin nếu cần
            };

            Process.Start(startInfo);

            _logger.LogInformation("Installer started successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading or installing update");
            return false;
        }
    }
}
