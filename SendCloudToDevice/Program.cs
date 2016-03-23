using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Linq;

namespace SendCloudToDevice
{
    class Program
    {

        static ServiceClient serviceClient;
        private static string connString = "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";
        static string DeviceToReceiveMsg = "rasp";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connString);
            ReceiveFeedbackAsync();

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            SendCloudToDeviceMessageAsync().Wait();
            Console.ReadLine();
        }


        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            // In order to request feedback for the delivery of your cloud-to-device message, you have to specify a property
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(DeviceToReceiveMsg, commandMessage);
        }



        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }

    }
}
