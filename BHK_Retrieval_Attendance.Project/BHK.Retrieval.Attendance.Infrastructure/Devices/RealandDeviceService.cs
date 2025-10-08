using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BHK.Retrieval.Attendance.Core.Interfaces.Services;
using Riss.Devices;

namespace BHK.Retrieval.Attendance.Infrastructure.Devices
{
    public class RealandDeviceService : IDeviceCommunicationService
    {
        private readonly DeviceCommEty _device = new();

        public async Task ConnectAsync(string ip, int port)
        {
            await Task.Run(() => _device.ConnectNet(ip, port));
        }

        public async Task<IEnumerable<string>> GetEmployeeListAsync()
        {
            // Giả sử SDK có hàm GetAllEmployee()
            return await Task.Run(() => _device.GetAllEmployee()
                .Select(e => e.EmpName)
                .ToList());
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() => _device.Disconnect());
        }
    }
}