using System;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Azure.Devices.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTNightLight
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LightPage : Page
    {
        public LightPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += Page_Loaded;
        }

     /* NAVIGATION
      * ==========================================================*/

        private void Nav_To_Temp(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate((typeof(TempPage)), null);
        }

        private void Nav_To_Main(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate((typeof(MainPage)), null);
        }
        private void Nav_To_Light(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate((typeof(LightPage)), null);
        }

        /* GAUGE
         * ==========================================================*/
        public void Go()
        {
            Goto(int.Parse(MyTextBox.Text));
        }

        private void Goto(int percentage)
        {
            float oldMin = 0;
            float oldMax = 100;
            float newMin = -90;
            float newMax = 90;
            var oldRange = Math.Floor((oldMax - oldMin));
            var newRange = Math.Floor((newMax - newMin));
            var newValue = (((percentage - oldMin) * newRange) / oldRange) + newMin;

            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                To = newValue,
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new BounceEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Bounces = 1,
                    Bounciness = 5,
                },
            };
            Storyboard.SetTarget(animation, ArrowTransform);
            Storyboard.SetTargetProperty(animation, "Rotation");
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        /* PAGE SPECIFIC
         * ==========================================================*/
        private void IncreaseLight()
        {
            Goto(90);
        }

        private void DecreaseLight()
        {
            Goto(10);
        }



        /* PAGE LOAD / UNLOAD
         * ==========================================================*/
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Goto(70);
        }


        /* MESSAGING
       * ==========================================================*/
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

        //            switch (msg)
        //            {
        //                case "increase temp":
        //                    IncreaseLight();
        //                    break;
        //                case "decrease temp":
        //                    DecreaseLight();
        //                    break;
        //            }

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



    }
}
