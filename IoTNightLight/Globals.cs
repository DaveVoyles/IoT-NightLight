using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace IoTNightLight
{
    public static class Globals
    {
        private static Frame rootFrame;
        private static MainPage mainPage;

        static Globals()
        {
            rootFrame = Window.Current.Content as Frame;

            mainPage = new MainPage();
        }

        /// <summary>
        /// Interpreets commands from console app and relays to the correct function
        /// </summary>
        /// <param name="msg">Command sent from console app</param>
        public static void parseCommand(string msg)
        {
            // TODO: Change this to a value from msg
            int numFromMsg = 10;
            switch (msg)
            {
                case "increase temp":
                    Debug.WriteLine("Increasing Temp");
                    //TODO: Create function to adjust GUI on client
                   mainPage.Goto(numFromMsg);
                    break;
                case "decrease temp":
                    Debug.WriteLine("Decreasing Temp");
                    //TODO: Create function to adjust GUI on client
                    break;
                case "increase light":
                    //TODO: Create function to adjust GUI on client
                    break;
                case "decrease light":
                    // TODO: Create a function on the client
                    break;
                case "nav to log":
                    Debug.WriteLine("navigating to log page");
                    rootFrame.Navigate((typeof (LogPage)), null);
                    break;
                case "nav to temp":
                    Debug.WriteLine("navigating to temp page");
                    rootFrame.Navigate((typeof (TempPage)), null);
                    break;
                case "nav to main":
                    Debug.WriteLine("navigating to main page");
                    rootFrame.Navigate((typeof (MainPage)), null);
                    break;
                case "nav to light":
                    Debug.WriteLine("navigating to light page");
                    rootFrame.Navigate((typeof (LightPage)), null);
                    break;
            }
        }


        /* NAVIGATION
         * ==========================================================*/

        public static void Nav_To_Temp(object sender, RoutedEventArgs e)
        {
            rootFrame.Navigate((typeof (TempPage)), null);
            Debug.WriteLine("Navigating to a new page");
        }

        public static void Nav_To_Main(object sender, RoutedEventArgs e)
        {
            rootFrame.Navigate((typeof (MainPage)), null);
            Debug.WriteLine("Navigating to a new page");
        }

        public static void Nav_To_Light(object sender, RoutedEventArgs e)
        {
            rootFrame.Navigate((typeof (LightPage)), null);
            Debug.WriteLine("Navigating to a new page");
        }

        public static void Nav_To_Log(object sender, RoutedEventArgs e)
        {
            rootFrame.Navigate((typeof (LogPage)), null);
            Debug.WriteLine("Navigating to a new page");
        }

    }
}
