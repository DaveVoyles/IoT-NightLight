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
    public sealed partial class LogPage : Page
    {
        public LogPage()
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
        private void Nav_To_Log(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate((typeof(LogPage)), null);
        }
    }
}
