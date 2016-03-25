using System;
using System.Text;
//using Microsoft.Azure.Devices; // Messaging conflicts with Devices.Client namespace

// New
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Azure.Devices;
using Message = Microsoft.Azure.Devices.Client.Message;


namespace SendCloudToDevice
{
    /* Sends message from app to IoT Hub, which gets picked up by Raspberry Pi */
    class Program
    {

        // New
        static DeviceClient deviceClient;

        private static string iotHubUri = "dv-iot-labs.azure-devices.net";
        private static string deviceKey = "viq10QVH2RUtvZ6Hpz7mtZzyHewPJmokhraftkMVYfk=";

        private const string IOT_HUB_CONN_STRING =
            "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";

        //static ServiceClient serviceClient;
        //private static string connString = "HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";
        static string DeviceToReceiveMsg = "rasp";
        private const string NAME_OF_DEVICE = "DeviceClient";
        private const string SharedAccessKey = "1q8HuqL0kPRZG9TAX62qBeCs88mOx1CAU7Jdsm5F9/E=";

        static void Main(string[] args)
        {

            // key must be base64 encoded

            // New
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, 
                new DeviceAuthenticationWithRegistrySymmetricKey(NAME_OF_DEVICE, SharedAccessKey));

            //SendDeviceToCloudMessagesAsync();
            sendMessageToDevice();
            Console.ReadLine();


            //Console.WriteLine("Send Cloud-to-Device message\n");
            //serviceClient = ServiceClient.CreateFromConnectionString(connString);
            //ReceiveFeedbackAsync();

            //Console.WriteLine("Press any key to send a C2D message.");
            //Console.ReadLine();
            //SendCloudToDeviceMessageAsync().Wait();
            //Console.ReadLine();
        }



        // New
        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = "myFirstDevice",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message       = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(1000);
            }
        }



        private static async void sendMessageToDevice()
        {
            try
            {
                var cloudToDeviceMessage = "";
                //cloudToDeviceMessage = DateTime.Now.ToLocalTime() + " - " + textBoxMessage.Text;
                cloudToDeviceMessage = DateTime.Now.ToLocalTime() + " - " + "hello!";

                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                var serviceMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(cloudToDeviceMessage));
                serviceMessage.Ack = DeliveryAcknowledgement.Full;
                serviceMessage.MessageId = Guid.NewGuid().ToString();
                await serviceClient.SendAsync(DeviceToReceiveMsg, serviceMessage);

                Console.WriteLine( cloudToDeviceMessage += $"Sent to Device ID: " + DeviceToReceiveMsg +cloudToDeviceMessage + "\n");

                await serviceClient.CloseAsync();

            }
            catch (Exception ex)
            {
               Console.WriteLine("EXCEPTION " + ex.ToString());
            }
        }









        //private async static Task SendCloudToDeviceMessageAsync()
        //{
        //    var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
        //    // In order to request feedback for the delivery of your cloud-to-device message, you have to specify a property
        //    commandMessage.Ack = DeliveryAcknowledgement.Full;

        //    await serviceClient.SendAsync(DeviceToReceiveMsg, commandMessage);
        //}



        //private async static void ReceiveFeedbackAsync()
        //{
        //    try
        //    {
        //        var feedbackReceiver = serviceClient.GetFeedbackReceiver();

        //        Console.WriteLine("\nReceiving c2d feedback from service");
        //        while (true)
        //        {
        //            var feedbackBatch = await feedbackReceiver.ReceiveAsync();
        //            if (feedbackBatch == null) continue;

        //            Console.ForegroundColor = ConsoleColor.Yellow;
        //            Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
        //            Console.ResetColor();

        //            await feedbackReceiver.CompleteAsync(feedbackBatch);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("EXCEPTION: " + ex.ToString());
        //        Console.ResetColor();
        //    }
        //}

    }
}
