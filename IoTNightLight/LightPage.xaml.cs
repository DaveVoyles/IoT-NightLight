using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Goto(70);
        }




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


        private void IncreaseLight()
        {
        }

        private void DecreaseLight()
        {

        }

    }
}
