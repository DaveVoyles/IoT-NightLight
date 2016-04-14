using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace IoTNightLight.Msg
{

    public class IoTHubArgs : EventArgs
    {
        public IoTHubArgs(string s)
        {
            msg = s;
        }
        private string msg;
        public string Message
        {
            get { return msg; }
        }
    }


    public class Messaging
    {

        // used for http1 for DeviceClient.Create() (also called iotHubUri)
        private const string IOT_HUB_HOST_NAME       = "dv-iot-labs.azure-devices.net";
        // Use the device specific connection string here. Used for AMQPS and DeviceClient.CreateFromConnectionString()
        private const string IOT_HUB_CONN_STRING     = "HostName=dv-iot-labs.azure-devices.net;DeviceId=rasp;SharedAccessKey=lw2etOoYw0ST+h801OmfbSW7uzunNz6KTfCokdI6eKg=";
        // Use the name of your Azure IoT device here - this should be the same as the name in the connections string
        private const string IOT_HUB_DEVICE          = "rasp";
        // Provide a short description of the location of the device, such as 'Home Office' or 'Garage'
        private const string IOT_HUB_DEVICE_LOCATION = "home-office";
        private const string IOT_DEVICE_KEY          = "lw2etOoYw0ST+h801OmfbSW7uzunNz6KTfCokdI6eKg=";

        private static DeviceClient deviceClient;

        public delegate void MsgReceived(object sender, IoTHubArgs a);
        // Anyone who subs to this, receives the message
        //TODO: Consider pre-processing based on msg type
        public event EventHandler<IoTHubArgs> MsgReceivedHandler;


        public Messaging()
        {
            // Init device client
            deviceClient = DeviceClient.Create(IOT_HUB_HOST_NAME, AuthenticationMethodFactory
                 .CreateAuthenticationWithRegistrySymmetricKey(IOT_HUB_DEVICE, IOT_DEVICE_KEY), TransportType.Http1);

            // Receive messages while app is active
            Task.Run( async () =>
            {
                while (true)
                {
                    Message receivedMessage = await deviceClient.ReceiveAsync();
                    var msg                 = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    var args                = new IoTHubArgs(msg);

      
                    EventHandler<IoTHubArgs> handler = MsgReceivedHandler;
                    if (handler != null)
                    {
                        handler(this, args);
                    }
                }
             });
        }


    }
}
