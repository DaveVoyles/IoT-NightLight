using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace DeviceClientAmqpSample
{
    class Program
    {


        // String containing Hostname, Device Id & Device Key in one of the following formats:
        //  "HostName=<iothub_host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"
        //  "HostName=<iothub_host_name>;CredentialType=SharedAccessSignature;DeviceId=<device_id>;SharedAccessSignature=SharedAccessSignature sr=<iot_host>/devices/<device_id>&sig=<token>&se=<expiry_time>";
        private const string DeviceConnectionString =
            "HostName=dv-iot-labs.azure-devices.net;DeviceId=DeviceClientAmqpSampl;SharedAccessKey=KMjgssfmEjTVfceoaVBf9BUELcsx8LbtncSw64H8GBA=";

        private static int MESSAGE_COUNT = 5;

        static void Main(string[] args)
        {
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

                if (deviceClient == null)
                {
                    Console.WriteLine("Failed to create DeviceClient!");
                }
                else
                {
                    SendEvent(deviceClient).Wait();
                    ReceiveCommands(deviceClient).Wait();
                }

                Console.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            string dataBuffer;

            Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

            for (int count = 0; count < MESSAGE_COUNT; count++)
            {
                dataBuffer = Guid.NewGuid().ToString();
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                await deviceClient.SendEventAsync(eventMessage);
            }
        }

        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}