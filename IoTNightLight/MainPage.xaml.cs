using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Windows.Networking.Sockets;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTNightLight
{
    public sealed partial class MainPage : Page
    {
        /* IMPORTANT! Change this to either AdcDevice.MCP3002 or AdcDevice.MCP3208 depending on which ADC you have     */
        private AdcDevice ADC_DEVICE = AdcDevice.MCP3002;

        private enum AdcDevice
        {
            NONE,
            MCP3002,
            MCP3208
        };

        // used for http1 for DeviceClient.Create()
        private const string IOT_HUB_HOST_NAME = "HostName=dv-iot-labs.azure-devices.net;";
        //"HostName=dv-iot-labs.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=leERufTFaeRxPx7o9KN7w0Abc8Sl+Y6S11AkLo6iHFI=";

        // Use the device specific connection string here. Used for AMQPS and DeviceClient.CreateFromConnectionString()
        private const string IOT_HUB_CONN_STRING     = "HostName=dv-iot-labs.azure-devices.net;DeviceId=rasp;SharedAccessKey=lw2etOoYw0ST+h801OmfbSW7uzunNz6KTfCokdI6eKg=";
        // Use the name of your Azure IoT device here - this should be the same as the name in the connections string
        private const string IOT_HUB_DEVICE          = "rasp";
        // Provide a short description of the location of the device, such as 'Home Office' or 'Garage'
        private const string IOT_HUB_DEVICE_LOCATION = "home-office";
        private const string IOT_DEVICE_KEY          = "lw2etOoYw0ST+h801OmfbSW7uzunNz6KTfCokdI6eKg=";

        // Line 0 maps to physical pin 24 on the RPi2
        private const Int32 SPI_CHIP_SELECT_LINE = 0;
        private const string SPI_CONTROLLER_NAME = "SPI0";

        // 01101000 channel configuration data for the MCP3002
        private const byte MCP3002_CONFIG = 0x68;
        // 00000110 channel configuration data for the MCP3208 
        private const byte MCP3208_CONFIG = 0x06;

        private const int RED_LED_PIN    = 12;

        private SolidColorBrush redFill  = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayFill = new SolidColorBrush(Windows.UI.Colors.LightGray);

        private static DeviceClient deviceClient;
  
        private GpioPin redLedPin;
        private SpiDevice spiAdc;
        private int adcResolution;
        private int adcValue;

        private Timer readSensorTimer;
        private Timer sendMessageTimer;



        public MainPage()
        {
            this.InitializeComponent();

            // Register the Unloaded event to clean up on exit
            Unloaded += MainPage_Unloaded;

            // Initialize GPIO and SPI
            InitAllAsync();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (spiAdc != null)
            {
                spiAdc.Dispose();
            }

            if (redLedPin != null)
            {
                redLedPin.Dispose();
            }
        }


         private async Task InitAllAsync()
        {
            try
            {
                Task[] initTasks = { InitGpioAsync(), InitSpiAsync() };
                await Task.WhenAll(initTasks);
        
            }
            catch (Exception ex)
            {
                StatusText.Text = "EXCEPTION during InitAllAsync: " + ex.Message;
                return;
            }

            // Read sensors every 100ms and refresh the UI
            readSensorTimer = new Timer(this.SensorTimer_Tick, null, 0, 1500);

             try
             {
                // Instantiate the Azure device client
                deviceClient = DeviceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

                // UWP can't use AMQPS, so need to override and define Http1 transport
                //deviceClient = DeviceClient.Create(IOT_HUB_HOST_NAME, AuthenticationMethodFactory
                //    .CreateAuthenticationWithRegistrySymmetricKey(IOT_HUB_DEVICE, IOT_DEVICE_KEY), TransportType.Http1);
            }
            catch (Exception ex) 
             {
                 
                 Debug.Write("\n EXCEPTION:  " + ex.ToString() + "\n");
             }

            // Send messages to Azure IoT Hub every one-second
            sendMessageTimer = new Timer(this.MessageTimer_Tick, null, 0, 1500);

            StatusText.Text = "Status: Running";

            //ReceiveC2dAsync();
        }

        private void MessageTimer_Tick(object state)
        {
            SendMessageToIoTHubAsync(adcValue);
        }



        private void SensorTimer_Tick(object state)
        {
            ReadAdc();
            LightLed();
        }


        private async Task ReceiveC2dAsync()
        {

            MessagesIn.Text = ("\nReceiving cloud to device messages from service");
            //Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                try
                {
                    Message receivedMessage = await deviceClient.ReceiveAsync();
                    Debug.WriteLine("received Message: " + receivedMessage);
                    if (receivedMessage == null) continue;

                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                    //Console.ResetColor();
                    var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                    MessagesIn.Text = ("Received message: " + msg);

                    await deviceClient.CompleteAsync(receivedMessage);
                }
                catch (Exception ex)
                {
                    // UI updates must be invoked on the UI thread
                    var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MessagesIn.Text = "Sending message: " + ex.Message + "\n" + MessagesIn.Text;
                    });
                }
            }

            //try
            //{
            //    MessagesIn.Text = ("\nReceiving cloud to device messages from service");
            //    //Console.WriteLine("\nReceiving cloud to device messages from service");
            //    while (true)
            //    {
            //        Message receivedMessage = await deviceClient.ReceiveAsync();
            //        if (receivedMessage == null) continue;

            //        //Console.ForegroundColor = ConsoleColor.Yellow;
            //        //Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
            //        //Console.ResetColor();
            //        var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            //        MessagesIn.Text = ("Received message: " + msg);

            //        await deviceClient.CompleteAsync(receivedMessage);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // UI updates must be invoked on the UI thread
            //    var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        Debug.Write("Well this didn't work: " + ex.Message);
            //        MessagesIn.Text = "Sending message: " + ex.Message + "\n" + MessagesIn.Text;
            //    });
            //}

        }


        private async Task SendMessageToIoTHubAsync(int darkness)
        {
            try
            {
                var payload = "{\"deviceId\": \"" +
                    IOT_HUB_DEVICE +
                    "\", \"location\": \"" +
                    IOT_HUB_DEVICE_LOCATION +
                    "\", \"messurementValue\": " +
                    darkness +
                    ", \"messurementType\": \"darkness\", \"localTimestamp\": \"" +
                    DateTime.Now.ToLocalTime().ToString() +
                    "\"}";

                // UI updates must be invoked on the UI thread
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MessageLog.Text = "Sending message: " + payload + "\n" + MessageLog.Text;
                });

                var msg = new Message(Encoding.UTF8.GetBytes(payload));

                await deviceClient.SendEventAsync(msg);
            }
            catch (Exception ex)
            {
                // UI updates must be invoked on the UI thread
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MessageLog.Text = "Sending message: " + ex.Message + "\n" + MessageLog.Text;
                });
            }
        }




        private void ReadAdc()
        {
            // Create a buffer to hold the read data
            byte[] readBuffer  = new byte[3];
            byte[] writeBuffer = new byte[3] { 0x00, 0x00, 0x00 };

            switch (ADC_DEVICE)
            {
                case AdcDevice.MCP3002:
                    writeBuffer[0] = MCP3002_CONFIG;
                    break;
                case AdcDevice.MCP3208:
                    writeBuffer[0] = MCP3208_CONFIG;
                    break;
            }

            // Read data from the ADC
            spiAdc.TransferFullDuplex(writeBuffer, readBuffer);
            adcValue = convertToInt(readBuffer);

            // UI updates must be invoked on the UI thread
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textPlaceHolder.Text = adcValue.ToString();
                IndicatorBar.Width = Map(adcValue, 0, adcResolution - 1, 0, 300);
            });
        }


        private int convertToInt(byte[] data)
        {
            int result = 0;
            switch (ADC_DEVICE)
            {
                case AdcDevice.MCP3002:
                    result = data[0] & 0x03;
                    result <<= 8;
                    result += data[1];
                    break;
                case AdcDevice.MCP3208:
                    result = data[1] & 0x0F;
                    result <<= 8;
                    result += data[2];
                    break;

            }
            return result;
        }


        private double Map(int val, int inMin, int inMax, int outMin, int outMax)
        {
            return Math.Round((double)((val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin));
        }


        private async Task InitGpioAsync()
        {
            var gpio = await GpioController.GetDefaultAsync();

            if (gpio == null)
            {
                throw new Exception("There is no GPIO controller on this device.");
            }

            redLedPin = gpio.OpenPin(RED_LED_PIN);
            redLedPin.Write(GpioPinValue.High);
            redLedPin.SetDriveMode(GpioPinDriveMode.Output);
        }


        private void LightLed()
        {
            SolidColorBrush fillColor = grayFill;

            switch (ADC_DEVICE)
            {
                case AdcDevice.MCP3002:
                    adcResolution = 1024;
                    break;
                case AdcDevice.MCP3208:
                    adcResolution = 4096;
                    break;
            }


            if (adcValue > adcResolution * 0.5)
            {

                redLedPin.Write(GpioPinValue.High);
                fillColor = redFill;
            }
            else
            {
                redLedPin.Write(GpioPinValue.Low);
                fillColor = grayFill;
            }

            // UI updates must be invoked on the UI thread
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                IndicatorBar.Fill = fillColor;
            });
        }


        private async Task InitSpiAsync()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                // 3.2MHz is the rated speed of the MCP3002 at 5v (1.2MHz @ 2.7V)
                // 2.0MHz is the rated speed of the MCP3208 at 5v (1.0MHz @ 2.7V)
                settings.ClockFrequency = 800000; // Set the clock frequency at or slightly below the specified rate speed
                                                  // The ADC expects idle-low clock polarity so we use Mode0
                settings.Mode           = SpiMode.Mode0;
                // Get a selector string that will return all SPI controllers on the system
                string spiAqs           = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                // Find the SPI bus controller devices with our selector string 
                var deviceInfo          = await DeviceInformation.FindAllAsync(spiAqs);
                // Create an SpiDevice with our bus controller and SPI settings
                     spiAdc             = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("SPI initialization failed.", ex);
            }
        }

    }
}
