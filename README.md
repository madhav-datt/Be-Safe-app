# Be-Safe-app
![Antidaephobiax Graphics](https://github.com/madhav-datt/Be-Safe-app/blob/master/src/BeSafe/Assets/SplashScreen.scale-150.png)

App written by **[Avikalp Srivastava](https://github.com/Avikalp7)** and **Madhav Datt** with mentorship from **Microsoft**.

Be-Safe is tool for *Traffic accident detection and notification with smartphones* and aims to reduce emergency response time in vehicle accidents.

Windows 10 app to send automatic *alert for help/rescue* when the user is in a car accident. Programmed using C# and XAML, with the Windows Phone Accelerometer API, Maps API and Messaging (SMS) API.

## Why use this App

A very large number of car accident fatalties happen because of long emergency services response times, especially if the accident happens in isolated areas or if the driver is injured/unconscious and not in a position to call for help. Based on 2007 traffic accident data, automatic traffic accident detection and notification systems could have saved *2,460 lives* (i.e., 6% of 41,000 fatalities) had they been in universal use. 

A major problem in using these systems everywhere is that they are infeasible or extremely expensive to install in existing vehicles and increase the initial cost of new vehicles. Our app uses smartphones to make up for this

## Using the App

Download the app on your Windows 10 Phone from [here](https://www.microsoft.com/en-in/store/apps/windows-phone). Run the app and enter an emergency mobile number (this could be of a friend, family or the emergency services) to start using it.

To use the background app, your Windows 10 Phone must have:
* An accelerometer sensor
* An active data connection
* The ability to send text messages

![Background App Interface](https://github.com/madhav-datt/Be-Safe-app/blob/master/src/BeSafe/Assets/app_interface.jpg)

## How it works

The app uses the phone's in-built sensors (accelerometer) and measures changes in its reading by polling the accelerometer at predefined intervals. It then compares change in acceleration of the phone to an emperically determined threshold to  automatically detect if the user was in a car accident.

The threshold change in acceleration is based on [research](http://www1.cse.wustl.edu/~schmidt/PDF/wreckwatch.pdf) and statistical data to minimize false positives. The app will not get triggered if the phone falls on the ground or is thrown at a wall.

If the threshold change in acceleration (set to around 4 times the value of [g](https://en.wikipedia.org/wiki/G-force)) is crossed, the app assumes that the user is in a car accident and sends out an emergency alert SMS/Text Message to the pre-entered emergency number with the users location.
