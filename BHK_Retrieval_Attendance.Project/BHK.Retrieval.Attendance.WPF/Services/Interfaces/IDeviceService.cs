using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.WPF.Services.Interfaces
{
    /// <summary>
    /// Interface cho Device Service - quản lý kết nối và giao tiếp với thiết bị
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Kết nối tới thiết bị qua TCP/IP
        /// </summary>
        Task<bool> ConnectTcpAsync(string ipAddress, int port, int deviceNumber, string password);

        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Kiểm tra kết nối tới thiết bị
        /// </summary>
        Task<bool> TestConnectionAsync(string ipAddress, int port);

        /// <summary>
        /// Lấy trạng thái hiện tại của thiết bị
        /// </summary>
        Task<string> GetDeviceStatusAsync();

        /// <summary>
        /// Kiểm tra xem thiết bị có đang kết nối không
        /// </summary>
        bool IsConnected { get; }
    }
}
