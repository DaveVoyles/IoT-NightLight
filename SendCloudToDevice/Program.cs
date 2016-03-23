using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendCloudToDevice
{
    class Program
    {

        static ServiceClient serviceClient;
        static string connectionString   = "HostName=dv-iot-labs.azure-devices.net;DeviceId=rasp;SharedAccessKey=lw2etOoYw0ST+h801OmfbSW7uzunNz6KTfCokdI6eKg=";

        private static string connString =
            "HostName=dv-iot-labs.azure-devices.net;DeviceId=mySecondDevice;SharedAccessSignature=SharedAccessSignaturesr=dv-iot-labs.azure-devices.net%2fdevices%2fmySecondDevice&sig=jLafWuadTpQy%2fQR10OPSpJiQka2uj%2fZ3PccvzaUfngw%3d&se=1458754885";
        static string DeviceToReceiveMsg = "rasp";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connString);

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            SendCloudToDeviceMessageAsync().Wait();
            Console.ReadLine();
        }


        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await serviceClient.SendAsync(DeviceToReceiveMsg, commandMessage);
        }

    }
}
