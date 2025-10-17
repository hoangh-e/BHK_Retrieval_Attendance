using BHK.Retrieval.Attendance.WPF.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace BHK.Retrieval.Attendance.WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho AboutView - hiển thị thông tin về phần mềm
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        private readonly ILogger<AboutViewModel> _logger;

        public AboutViewModel(ILogger<AboutViewModel> logger)
        {
            _logger = logger;
            LoadSoftwareInfo();
        }

        public string SoftwareName { get; private set; }
        public string Version { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
        public string Developer { get; private set; }
        public string License { get; private set; }

        private void LoadSoftwareInfo()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName();

                SoftwareName = "BHK Attendance System";
                Version = $"Version {assemblyName.Version?.ToString() ?? "1.0.0"}";
                Copyright = $"© {DateTime.Now.Year} BHK. All rights reserved.";
                Description = "Hệ thống quản lý chấm công tích hợp với thiết bị vân tay/thẻ từ";
                Developer = "BHK Development Team";
                License = "Proprietary License";

                _logger.LogInformation("Software info loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading software info");
            }
        }
    }
}
