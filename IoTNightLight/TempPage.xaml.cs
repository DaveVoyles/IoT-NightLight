using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTNightLight
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TempPage : Page
    {
        public TempPage()
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


    }
}
