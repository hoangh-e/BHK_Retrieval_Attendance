namespace BHK.Retrieval.Attendance.Infrastructure.DeviceIntegration.Wrappers.Realand
{
    public interface IRealandDeviceWrapper
    {
        bool Connect(string ipAddress, int port, int deviceNumber, string password);
        bool Disconnect();
        bool IsConnected { get; }
    }
}