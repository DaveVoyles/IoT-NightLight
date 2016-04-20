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
