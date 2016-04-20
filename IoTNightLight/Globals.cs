using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTNightLight
{
    public static class Globals
    {
        private static Frame rootFrame;

        static Globals()
        {

            rootFrame = Window.Current.Content as Frame;
        }

        public static void parseCommand(string msg)
        {
            switch (msg)
            {
                case "increase temp":
                    Debug.WriteLine("Increasing Temp");
                    //TODO: Create function to adjust GUI on client
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
                    rootFrame.Navigate((typeof(LogPage)), null);
                    break;
                case "nav to temp":
                    Debug.WriteLine("navigating to temp page");
                    rootFrame.Navigate((typeof(TempPage)), null);
                    break;
                case "nav to main":
                    Debug.WriteLine("navigating to main page");
                    rootFrame.Navigate((typeof(MainPage)), null);
                    break;
                case "nav to light":
                    Debug.WriteLine("navigating to light page");
                    rootFrame.Navigate((typeof(LightPage)), null);
                    break;
            }
        }

    }
}
