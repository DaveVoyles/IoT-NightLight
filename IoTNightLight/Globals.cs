using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoTNightLight
{
    public static class Globals
    {
        private static Frame     rootFrame;
        private static MainPage  mainPage;
        
        static Globals()
        {
            // Get current window and set it to MainPage
            rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null) mainPage  = (MainPage)rootFrame.Content;
        }


        /// <summary>
        /// Interprets commands from console app and relays to the correct function
        /// </summary>
        /// <param name="msg">Command sent from console app</param>
        public static void ParseMsg(string msg)
        {
            // Grab arguments for functions, but make sure it isn't empty
            List<int> listArgs = GetIntValList(msg);
            if (listArgs.Any())
            {
                foreach (var integer in listArgs)
                {
                    Debug.WriteLine(integer);
                }
            }
            // Split it on whitespace sequences.
            string[] firstWordInMsg = Regex.Split(msg, @"\s+");

            switch (firstWordInMsg[0])
            {
                // ---------------------------------------- TWEENING
                case "tween":
                    mainPage.AllowTweening();
                    mainPage.TweenGauge(listArgs[0], listArgs[1], listArgs[2], listArgs[3]);
                    Debug.WriteLine(firstWordInMsg + ": "  + listArgs[0] + " " + listArgs[1] + " " + listArgs[2] + " " + listArgs[3]);
                    break;
                case "stop":
                    mainPage.PreventTweening();
                    break;
                // ---------------------------------------- TEMPERATURE
                case "temp":
                    mainPage.ChangeTitleText("Temperature");
                    if (listArgs.Any()){ mainPage.Goto(listArgs[0]);}
                    Debug.WriteLine(firstWordInMsg + ": " + listArgs[0]);
                    break;
                // ---------------------------------------- LIGHT
                case "light":
                    mainPage.ChangeTitleText("Light");
                    if (listArgs.Any()) { mainPage.Goto(listArgs[0]); }
                    Debug.WriteLine(firstWordInMsg + ": " + listArgs[0]);
                    break;
                // ---------------------------------------- Moisture
                case "moisture":
                    mainPage.ChangeTitleText("Moisture");
                    if (listArgs.Any()) { mainPage.Goto(listArgs[0]); }
                    Debug.WriteLine(firstWordInMsg + ": " + listArgs[0]);
                    break;
                // ---------------------------------------- NAVIGATION
                case "log":
                    Debug.WriteLine("navigating to log page");
                    mainPage.ChangeTitleText("Log");
                    break;
            }
        }


        /* Parsing
         * ==========================================================*/

        /// <summary>
        /// Parses int from msg str which is sent to IoT device for increase / decrease temp, etc.
        /// </summary>
        /// <param name="msg">What do you want to the IoT device to do?</param>
        /// <returns>Integer used to change values in IoT device</returns>
        private static int GetIntVal(string msg)
        {
            int intInMsg     = 0;
            string[] numbers = Regex.Split(msg, @"\D");

            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    intInMsg = i;
                }
            }
            return intInMsg;
        }


        /// <summary>
        /// Parses the string from console message and grabs the Ints and stores as a list.
        /// </summary>
        /// <param name="msg">String sent from console app</param>
        /// <returns>List of integers that can be applied as parameters to functions</returns>
        private static List<int> GetIntValList(string msg)
        {
            string[] numbers = Regex.Split(msg, @"\D");

            return (from value in numbers where !string.IsNullOrEmpty(value) select int.Parse(value)).ToList();
        }


        /// <summary>
        /// Parse message sent from console app to IoT device
        /// </summary>
        /// <param name="msg">String sent from console app</param>
        /// <returns>Each word in the string, separately</returns>
        private static string stringValFromMsg(string msg)
        {
            string[] separators = new string[] { ",", ".", "!", "\'", " ", "\'s" };
            string text         = msg;
            string newMsg       = "";

            foreach (string word in text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                Debug.WriteLine(word);
            }
            return newMsg;
        }


        /* NAVIGATION
         * These change the title of the page and prevent it from tweening the old values from prev page
         * ==========================================================*/
        public static void Nav_To_Temp(object sender, RoutedEventArgs e)
        {
            mainPage.PreventTweening();
            mainPage.ChangeTitleText("Temperature");
        }

        public static void Nav_To_Main(object sender, RoutedEventArgs e)
        {
            mainPage.PreventTweening();
            mainPage.ChangeTitleText("Moisture");
        }

        public static void Nav_To_Light(object sender, RoutedEventArgs e)
        {
            mainPage.PreventTweening();
            mainPage.ChangeTitleText("Light");
        }

        public static void Nav_To_Log(object sender, RoutedEventArgs e)
        {
            mainPage.PreventTweening();
            mainPage.ChangeTitleText("Log");
        }

    }
}
