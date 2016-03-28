using System;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices;
using System.Text;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SendCloudToDeviceUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

       
        private const string IOT_HUB_URI = "dv-iot-labs.azure-devices.net";
        private const string DEVICE_TO_RECEIVE_MSG = "rasp";
        private const string NAME_OF_DEVICE = "UWP_Dev_To-Cloud";
        private const string SHARED_ACCES_KEY = "ezJtd/oyEhBwdXKihJ2i3bxG/gVL234qz1O7YG6gmtU=";
        private const string IOT_HUB_CONN_STRING = "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";
    
        public MainPage()
        {
            this.InitializeComponent();
            SendDataToAzure();

            //deviceClient = DeviceClient.Create(IOT_HUB_URI, new DeviceAuthenticationWithRegistrySymmetricKey(NAME_OF_DEVICE, SHARED_ACCES_KEY));

        }



        private async Task SendDataToAzure()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING, TransportType.Http1);

            var text = "Hellow, Windows 10!";
            var msg = new Message(Encoding.UTF8.GetBytes(text));

            await deviceClient.SendEventAsync(msg);
        }



        private static async void sendMessageToDevice()
        {
            try
            {
                String cloudToDeviceMessage = "";
                cloudToDeviceMessage = DateTime.Now.ToLocalTime() + " - " + "hello!";

                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                var serviceMessage =
                    new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloudToDeviceMessage))
                    {
                        Ack = DeliveryAcknowledgement.Full,
                        MessageId = Guid.NewGuid().ToString()
                    };

                await serviceClient.SendAsync(DEVICE_TO_RECEIVE_MSG, serviceMessage);
                Console.WriteLine(cloudToDeviceMessage += $"Sent to Device ID: " + DEVICE_TO_RECEIVE_MSG + cloudToDeviceMessage + "\n");
                await serviceClient.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION. Unable to sendMessageToDevice(). " + ex.ToString());
            }
        }



    }
}
