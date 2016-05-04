using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace IoTNightLight
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class TempPage : Page
    {
        public TempPage()
        {
            this.InitializeComponent();
        }



        /// <summary>
        /// Always listens for messages directly from console app
        /// </summary>
        private async Task listenForMessageFromDeviceTask()
        {
            while (true)
            {
                var msg = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
                if (msg == null) continue;

                Globals.parseMsg(msg);
            }
        }



        /// <summary>
        /// Get btn name & nav to correct page
        /// </summary>
        private void navHandler_OnClick(object sender, RoutedEventArgs e)
        {
            var content = (sender as Button).Content.ToString();
            switch (content)
            {
                case "Temp":
                    Globals.Nav_To_Temp (sender, e);
                    break;
                case "Light":
                    Globals.Nav_To_Light(sender, e);
                    break;
                case "Sound":
                    Globals.Nav_To_Main (sender, e);
                    break;
                case "Log":
                    Globals.Nav_To_Log  (sender, e);
                    break;
            }
        }



        /* GAUGE
         * ==========================================================*/
        private void Go()
        {
            Goto(int.Parse(MyTextBox.Text));
        }

        public void Goto(int percentage)
        {
            float oldMin = 0;
            float oldMax = 100;
            float newMin = -90;
            float newMax = 90;

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
                Duration       = TimeSpan.FromSeconds(3),
                EasingFunction = new BounceEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Bounces    = 4,
                    Bounciness = 8,
                },
            };
            Storyboard.SetTarget(animation, ArrowTransform);
            Storyboard.SetTargetProperty(animation, "Rotation");
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }


    }
}
