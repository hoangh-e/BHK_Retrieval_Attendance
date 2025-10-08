namespace BHK.Retrieval.Attendance.Infrastructure.DeviceIntegration.Wrappers.Realand
{
    public interface IRealandDeviceWrapper
    {
        bool Connect(string ipAddress, int port);
        bool Disconnect();
        bool IsConnected { get; }
    }
}