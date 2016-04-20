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


        /// <summary>
        /// Get btn name & nav to correct page
        /// </summary>
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
                case "Sound":
                    Globals.Nav_To_Main(sender, e);
                    break;
                case "Log":
                    Globals.Nav_To_Log(sender, e);
                    break;
            }
        }

    }
}
