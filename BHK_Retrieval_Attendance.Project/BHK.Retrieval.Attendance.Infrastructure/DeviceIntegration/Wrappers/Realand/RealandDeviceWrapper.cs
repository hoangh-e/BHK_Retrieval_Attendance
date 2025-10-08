using Riss.Devices;

namespace BHK.Retrieval.Attendance.Infrastructure.DeviceIntegration.Wrappers.Realand
{
    public class RealandDeviceWrapper : IRealandDeviceWrapper
    {
        private Device? _device;
        private DeviceConnection? _connection;

        public bool Connect(string ipAddress, int port, int deviceNumber, string password)
        {
            try
            {
                _device = new Device
                {
                    DN = deviceNumber,
                    Password = password,
                    Model = "ZDC2911",
                    ConnectionModel = 5,
                    IpAddress = ipAddress,
                    IpPort = port,
                    CommunicationType = CommunicationType.Tcp
                };

                _connection = DeviceConnection.CreateConnection(ref _device);

                int result = _connection.Open();

                if (result > 0)
                {
                    // success
                    return true;
                }
                else
                {
                    // fail
                    _connection = null;
                    _device = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log exception
                _connection = null;
                _device = null;
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                // TODO: Implement actual disconnection logic
                // Call appropriate cleanup methods based on actual API
                
                _connection = null;
                _device = null;
                return true;
            }
            catch (Exception)
            {
                // Log error
                return false;
            }
        }

        public bool IsConnected => _connection is not null && _device is not null;
    }
}