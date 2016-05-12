# Raspberry Pi Ghostbuster App

### Author(s): Dave Voyles | [@DaveVoyles](http://www.twitter.com/DaveVoyles)
### URL: [www.DaveVoyles.com](http://www.DaveVoyles.com)

Send messages from a Win10 console app to a Raspberry Pi, via Azure IoT Hub 
----------
### About
With this app, you'll be able to send messages from a console app to a Raspberry Pi and have it make changes to the application. 
The Pi reads in the string and parses the text and parameters, which it then converts into functions that the Pi can understand.

There are only three classes you need to be concerned with:
- Globals.cs (Rasp Pi -- IoT Night Light)
- MainPage.Xaml.cs (Rasp Pi -- Iot Night Light)
- Program.cs (Console App - SendCloudToDevice)

### Requirements
- Raspberry Pi
- Windows 10
- Visual Studio 2015

### Setup
-- [Install / configure Win 10 on the Pi](http://thinglabs.io/workshop/cs/nightlight/)
Follow the instructions for *Getting Started* and *Setting Up Your Raspberry Pi*

-- Deploy multiple applications from within Visual Studio
You want to deploy the *Send Cloud To Device* app as an x86 app and *IoTNightLight* as an ARM project

-- Make note of the IP address of the Raspberry Pi
Turn it on, and the IPV4 address is the one you are looking for. Should look something like 192.168.1.23

-- In the IoTNightLight project (NOT the solution) change the target device to:  *Remote Machine*
This is done by right-click on "properties" on the IoTNightLight (Universal Windows) project. Under the tab for "Debug", there is a 
an are marked "Target device." Change that to "Remote Machine" then add the IP address of your Raspberry Pi.


### Possible Commands
-- Tween
Accepts parameters for 
