using System.Collections.Generic;
using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho Device Communication Service
    /// Clean Architecture - Core Interface, Infrastructure Implementation
    /// </summary>
    public interface IDeviceCommunicationService
    {
        /// <summary>
        /// Kết nối tới thiết bị
        /// </summary>
        /// <param name="ip">IP Address của thiết bị</param>
        /// <param name="port">Port của thiết bị</param>
        /// <param name="deviceNumber">Device Number (DN)</param>
        /// <param name="password">Password thiết bị</param>
        Task ConnectAsync(string ip, int port, int deviceNumber, string password);
        
        /// <summary>
        /// Lấy danh sách nhân viên từ thiết bị
        /// </summary>
        Task<IEnumerable<string>> GetEmployeeListAsync();
        
        /// <summary>
        /// Ngắt kết nối khỏi thiết bị
        /// </summary>
        Task DisconnectAsync();
    }
}