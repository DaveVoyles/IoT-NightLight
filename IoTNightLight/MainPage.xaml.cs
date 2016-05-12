using System;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using IoTNightLight.Msg;
using Microsoft.Azure.Devices.Client;

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

        // used for http1 for DeviceClient.Create() (also called iotHubUri)
        private const string IOT_HUB_HOST_NAME       = "dv-iot-labs.azure-devices.net";
        // Use the device specific connection string here. Used for AMQPS and DeviceClient.CreateFromConnectionString()
        private const string IOT_HUB_CONN_STRING     = "HostName=dv-iot-labs.azure-devices.net;DeviceId=minwinpc;SharedAccessKey=EVY5HecasruKNR0DRpxTpHYw1717v0C6TEkJ2QYeExI=";
        // Use the name of your Azure IoT device here - this should be the same as the name in the connections string
        private const string IOT_HUB_DEVICE          = "minwinpc";
        // Provide a short description of the location of the device, such as 'Home Office' or 'Garage'
        private const string IOT_HUB_DEVICE_LOCATION = "home-office";
        private const string IOT_DEVICE_KEY          = "EVY5HecasruKNR0DRpxTpHYw1717v0C6TEkJ2QYeExI=";

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

        // Placeholder values for tweening gauge
        private DispatcherTimer timer;
        private int numOfTicks;
        private int timerDelay;
        private int timerIn_MS;
        // Prevents the animation gauge from tweening back-and-forth
        private bool _canTween = true;


        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded             += Page_Loaded;
            Unloaded           += Page_Unloaded;

            //TODO: Uncomment this when hardware sensors are working
            // Initialize GPIO and SPI
            //InitAllAsync();

            // INIT DEVICE CLIENT (Either of these can be used to receive messages from IoT Hub)
            // ---------------------

            deviceClient = DeviceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

            // OR:
            //var messenger = new Messaging();
            //    messenger.MsgReceivedHandler += Messenger_MsgReceivedHandler;

            // ------------------------
            // RECEIVING MESSAGES
            listenForMessageFromDeviceTask();
            //receiveMsg();
        }


        /// <summary>
        /// Always listens for messages directly from console app
        /// </summary>
        private async Task listenForMessageFromDeviceTask()
        {
            while (true)
            {
                var msg =  await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
                if (msg == null) continue;

                Globals.ParseMsg(msg);
            }
        }


        /// <summary>
        /// Change title text from console app, rahter than navigating between XAML pages
        /// </summary>
        /// <param name="newName">What should we call this new "page"?</param>
        public void ChangeTitleText(string newName)
        {
            NameBlock.Text = newName;
        }


        /* NAVIGATION
         * ==========================================================*/
        private void navHandler_OnClick(object sender, RoutedEventArgs e)
        {
            var content = (sender as Button).Content.ToString();
            switch (content)
            {
                case "Temp":
                    Globals.Nav_To_Temp(sender, e);
                    break;
                case "Light":
                    Globals.Nav_To_Light(sender, e);
                    break;
                case "Moisture":
                    Globals.Nav_To_Main(sender, e);
                    break;
                case "Log":
                    Globals.Nav_To_Log(sender, e);
                    break;
            }
        }



        /* TWEENING
         * ==========================================================*/

        /// <summary>
        /// Tweens gauge on the page back-and-forth between two values
        /// </summary>
        /// <param name="startVal">Gauge starts here</param>
        /// <param name="finalVal">Final val</param>
        /// <param name="numOfTweens">How many times should it tween between both vals?</param>
        /// <param name="cycleTime">Delay between tweening</param>
        public async void TweenGauge(int startVal, int finalVal, int numOfTweens, int cycleTime)
        {
            var tweenIndex = 0;
            while (tweenIndex < numOfTweens)
            {
                if (!_canTween)
                {
                    Debug.WriteLine("Can't tween, returning");
                    return;
                }
                
                Debug.WriteLine("Can tween, so gonna do it");
                Goto(startVal); // Forward
                await Task.Delay(TimeSpan.FromSeconds(cycleTime));
                Goto(finalVal); // Back
                await Task.Delay(TimeSpan.FromSeconds(cycleTime));
                tweenIndex++; // One revolution complete
            }
            Debug.WriteLine("Exiting Tick");
        }

        public void PreventTweening()
        {
            _canTween = false;
            Debug.WriteLine("canTween: " + _canTween);
        }

        public void AllowTweening()
        {
            _canTween = true;
            Debug.WriteLine("canTween: " + _canTween);
        }

        /// <summary>
        /// TODO: Unused timer
        /// </summary>
        //public async void Timer_Tick(object sender, object e)
        //{
        //    numOfTicks = 3;
        //    while (numOfTicks <= 3)
        //    {
        //        Goto(100);
        //        await Task.Delay(TimeSpan.FromSeconds(5));
        //        Goto(90);
        //        numOfTicks++;
        //    }
        //}


        /* GAUGE
         * ==========================================================*/
        private void Go()
        {
            Goto(int.Parse(MyTextBox.Text));
        }

        public void Goto(int percentage)
        {
            float oldMin =   0;
            float oldMax = 100;
            float newMin = -90;
            float newMax =  90;

            var oldRange = Math.Floor((oldMax - oldMin));
            var newRange = Math.Floor((newMax - newMin));
            var newValue = (((percentage - oldMin) * newRange) / oldRange) + newMin;

            createStoryboard(newValue);
        }

        private void createStoryboard(double newValue)
        {
            var storyboard = new Storyboard();
            var animation  = new DoubleAnimation
            {
                To             = newValue,
                Duration       = TimeSpan.FromSeconds(.7),
                EasingFunction = new BounceEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Bounces    = 3,
                    Bounciness = 8,
                },
            };
            Storyboard.SetTarget(animation, ArrowTransform);
            Storyboard.SetTargetProperty(animation, "Rotation");
            storyboard.Children.Add(animation);
            storyboard.Begin();
            Debug.WriteLine("Beginning storyboard");
        }


        /* PAGE LOAD / UNLOAD
         * ==========================================================*/
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Goto(70);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (spiAdc != null)
            {
                //spiAdc.Dispose();
            }

            if (redLedPin != null)
            {
                redLedPin.Dispose();
            }
        }



        ///* MESSAGING
        // * ==========================================================*/
        //private async Task ReceiveC2dAsync()
        //{
        //    MessagesIn.Text = ("\nReceiving cloud to device messages from service");
        //    while (true)
        //    {
        //        try
        //        {
        //            Message receivedMessage = await deviceClient.ReceiveAsync();

        //            if (receivedMessage == null) { continue; }

        //            var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
        //            MessagesIn.Text = ("Received message: " + msg + " TIME: " + DateTime.Now.ToLocalTime().ToString());


        //            await deviceClient.CompleteAsync(receivedMessage);
        //        }
        //        catch (Exception ex)
        //        {
        //            // UI updates must be invoked on the UI thread
        //            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //            {
        //                MessagesIn.Text = "Sending message: " + ex.ToString() + "\n" + MessagesIn.Text;
        //            });
        //        }
        //    }
        //}


        //private async Task SendMessageToIoTHubAsync(int darkness)
        //{
        //    try
        //    {
        //        var payload = "{\"deviceId\": \"" +
        //            IOT_HUB_DEVICE +
        //            "\", \"location\": \"" +
        //            IOT_HUB_DEVICE_LOCATION +
        //            "\", \"messurementValue\": " +
        //            darkness +
        //            ", \"messurementType\": \"darkness\", \"localTimestamp\": \"" +
        //            DateTime.Now.ToLocalTime().ToString() +
        //            "\"}";

        //        // UI updates must be invoked on the UI thread
        //        var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //        {
        //            MessageLog.Text = "Sending message: " + payload + "\n" + MessageLog.Text;
        //        });

        //        var msg = new Message(Encoding.UTF8.GetBytes(payload));

        //        await deviceClient.SendEventAsync(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        // UI updates must be invoked on the UI thread
        //        var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //        {
        //            MessageLog.Text = "Sending message: " + ex.Message + "\n" + MessageLog.Text;
        //        });
        //    }
        //}




        /* HARDWARE
         * ==========================================================*/
        //private async Task InitAllAsync()
        //{
        //    try
        //    {
        //        Task[] initTasks = { InitGpioAsync(), InitSpiAsync() };
        //        await Task.WhenAll(initTasks);    
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusText.Text = "EXCEPTION during InitAllAsync: " + ex.Message;
        //        return;
        //    }

        //    // Read sensors every 100ms and refresh the UI
        //    readSensorTimer = new Timer(this.SensorTimer_Tick, null, 0, 1500);

        //     try
        //     {
        //        // Instantiate the Azure device client
        //        //deviceClient = DeviceClient.CreateFromConnectionString(IOT_HUB_CONN_STRING);

        //        // UWP can't use AMQPS, so need to override and define Http1 transport
        //        deviceClient = DeviceClient.Create(IOT_HUB_HOST_NAME, AuthenticationMethodFactory
        //            .CreateAuthenticationWithRegistrySymmetricKey(IOT_HUB_DEVICE, IOT_DEVICE_KEY), TransportType.Http1);
        //    }
        //    catch (Exception ex) 
        //     {             
        //         Debug.Write("\n EXCEPTION: device client  " + ex.ToString() + "\n");
        //     }

        //    // Send messages to Azure IoT Hub every 1.5 secs
        //    sendMessageTimer = new Timer(this.MessageTimer_Tick, null, 0, 1500);

        //    StatusText.Text = "STATUS: Running";
        //    await ReceiveC2dAsync();
        //}

        //private void MessageTimer_Tick(object state)
        //{
        //    SendMessageToIoTHubAsync(adcValue);
        //}

        private void SensorTimer_Tick(object state)
        {
            ReadAdc();
            LightLed();
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
                //textPlaceHolder.Text = adcValue.ToString();
                //IndicatorBar.Width = Map(adcValue, 0, adcResolution - 1, 0, 300);
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
                //IndicatorBar.Fill = fillColor;
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



        /* NOT IN USE
         * ==========================================================*/
        /// <summary>
        /// Raw text from message: e.Message  ex: Debug.WriteLine(e.Message);
        /// </summary>
        /// <param name="sender">Who sent the message</param>
        /// <param name="e">Contents of the message</param>
        private void Messenger_MsgReceivedHandler(object sender, IoTHubArgs e)
        {
            // TODO: Add a function
            Debug.WriteLine("MESSAGE RECEIVED to MainPage: " + e.Message);
        }


        /// <summary>
        /// Can be used to receive messages from IoT Hub
        /// TODO: Not currently used
        /// </summary>
        private async void receiveMsg()
        {

            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                var args = new IoTHubArgs(msg);
                Debug.WriteLine(args);
            }
        }

    }
}
