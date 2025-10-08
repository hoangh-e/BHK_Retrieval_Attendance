using System.Collections.Generic;
using System.Threading.Tasks;

namespace BHK.Retrieval.Attendance.Core.Interfaces.Services
{
    public interface IDeviceCommunicationService
    {
        Task ConnectAsync(string ip, int port);
        Task<IEnumerable<string>> GetEmployeeListAsync();
        Task DisconnectAsync();
    }
}