using System;
using System.Windows;
using Riss.Devices;

namespace BHK_Retrieval_Attendance.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Test SDK
            try
            {

                Device device = new Device
                {
                    DN = 1,
                    IpAddress = "192.168.10.249",
                    IpPort = 5500,
                    Password = "0",
                    Model = "ZDC2911",
                    ConnectionModel = 5,
                    CommunicationType = CommunicationType.Tcp, 
                    Baudrate = 9600                           
                };
                DeviceConnection conn = DeviceConnection.CreateConnection(ref device);
                int result = conn.Open();

                if (result > 0)
                {
                    MessageBox.Show("✅ Kết nối thành công với thiết bị!");
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("❌ Không kết nối được với thiết bị.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load package: {ex.Message}");
            }
        }
    }
}
