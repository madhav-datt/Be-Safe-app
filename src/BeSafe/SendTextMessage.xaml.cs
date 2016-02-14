
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Devices.Sms;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using System.Threading.Tasks;
using Windows.UI.Core;


namespace SmsSendAndReceive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendTextMessage : Page
    {
        // Declaring Accelerommeter variables here
        private Accelerometer _accelerometer;
        private uint _desiredReportInterval;
        private DispatcherTimer _dispatcherTimer;
        public double acc_x, acc_y, acc_z;

        // The variable HighAccEvent is changed to true only if the accelerometer reading goes above the threshold.
        // The value change to true triggers the sending of the SMS to the emergency contact number.
        public bool HighAccEvent = false;

       
        private MainPage rootPage;
        private SmsDevice2 device;

        // The string Mnumber stores the emergency contact number provided by the user.
        // Until entered, it is set as "default".
        public string Mnumber = "default";

        public SendTextMessage()
        {
            this.InitializeComponent();

            rootPage = MainPage.Current;

            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                // This value will be used later to activate the sensor.
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;

                // Set up a DispatchTimer
                _dispatcherTimer = new DispatcherTimer();
                _dispatcherTimer.Tick += DisplayCurrentReading;
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)_desiredReportInterval);
            }
            else
            {
                 rootPage.NotifyUser("No accelerometer found. This app is not compatible with your device!", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the dispatcher callback.
        /// Used to diplay real time time accelerometer readings in the 3 textblocks on Page SendTextMessage.xaml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DisplayCurrentReading(object sender, object args)
        {
            RoutedEventArgs temp = null;
            AccelerometerReading reading = _accelerometer.GetCurrentReading();
            if (reading != null)
            {
                acc_x = reading.AccelerationX;
                acc_y = reading.AccelerationY;
                acc_z = reading.AccelerationZ;
                ScenarioOutput_X.Text = String.Format("{0,5:0.00}", reading.AccelerationX);
                ScenarioOutput_Y.Text = String.Format("{0,5:0.00}", reading.AccelerationY);
                ScenarioOutput_Z.Text = String.Format("{0,5:0.00}", reading.AccelerationZ);

                // Critical Step. Any component of acceleration crossing the precalculated limit of 39.2 will trigger 
                // the clicking of the StartApp button with the parameter HighAccEvent set as true.  
                if (acc_x > 39.2 || acc_y > 39.2 || acc_z > 39.2)
                {
                    HighAccEvent = true;
                    Send_Click(sender, temp);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
            // If the parameter is received from the Welcome Page (the mobile number) , then display a message to user.
            if (e.Parameter != null)
            {
                MessagetoUser.Text = "Your Emergency Contact numeber has been saved. We have you covered! Be Safe!";
                Mnumber = e.Parameter.ToString();
            }
            // Else if no parameter has been received, display message to user to enter the number
            else
            {
                MessagetoUser.Text = "Please enter your emergency contact number to get started!";
            }
        }

        // Clicking of the Start App button
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            // This event occurs when the user clicks the Start App button and is distinguished from the event where
            // this button click is triggered by a high acceleration event.
            if (HighAccEvent == false)
            {
                // If the device does have an accelerometer
                if (_accelerometer != null)
                {
                    // Set the report interval to enable the sensor for polling
                    _accelerometer.ReportInterval = _desiredReportInterval;
                    _dispatcherTimer.Start();
                }

                // No Accelerometer found. Notify the user.
                else
                {
                    rootPage.NotifyUser("No accelerometer found. This app is not compatible with your device.", NotifyType.ErrorMessage);
                }
            }


            // This sections corresponds to the the event in which the clicking of the start app button is triggered by HighAccEvent
            else
            {
                // Geolocation based on bing API
                Geolocator geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 500;
                try
                {
                    Geoposition geoposition = await geolocator.GetGeopositionAsync(
                         maximumAge: TimeSpan.FromMinutes(5),
                         timeout: TimeSpan.FromSeconds(10)
                        );

                    // If this is the first request, get the default SMS device. If this
                    // is the first SMS device access, the user will be prompted to grant
                    // access permission for this application.
                    if (device == null)
                    {
                        try
                        {
                            rootPage.NotifyUser("Getting default SMS device ...", NotifyType.StatusMessage);
                            device = SmsDevice2.GetDefault();
                        }
                        catch (Exception ex)
                        {
                            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                            return;
                        }

                    }

                    string msgStr = "";
                    if (device != null)
                    {
                        try
                        {
                            // Create a text message - set the entered destination number and message text.
                            SmsTextMessage2 msg = new SmsTextMessage2();

                            // If the user has not yet entered the number
                            if (Mnumber == null)
                            {
                                rootPage.NotifyUser("You have not yet entered your emergency contact number!", NotifyType.ErrorMessage);
                                return;
                            }

                            msg.To = Mnumber;
                            msg.Body = "Emergency alert! I have been in a possible accident at " + geoposition.Coordinate.Latitude.ToString("0.00") + ", " + geoposition.Coordinate.Longitude.ToString("0.00") + ". Please help.";

                            // Send the message asynchronously
                            rootPage.NotifyUser("Sending Message to emergency contact and rescue services.", NotifyType.StatusMessage);
                            SmsSendMessageResult result = await device.SendMessageAndGetResultAsync(msg);

                            if (result.IsSuccessful)
                            {
                                msgStr = "";
                                msgStr += "Message sent to: " + Mnumber + "(Predefined Emergency Number)" + System.Environment.NewLine;
                                msgStr += "Emergency alert! I have been in a possible accident at Map Coordinates:" + geoposition.Coordinate.Latitude.ToString("0.00") + ", " + geoposition.Coordinate.Longitude.ToString("0.00") + ". Please help." + System.Environment.NewLine;

                                IReadOnlyList<Int32> messageReferenceNumbers = result.MessageReferenceNumbers;
                                rootPage.NotifyUser(msgStr, NotifyType.StatusMessage);
                            }

                            else
                            {
                                msgStr = "";
                                msgStr += "ModemErrorCode: " + result.ModemErrorCode.ToString();
                                msgStr += "\nIsErrorTransient: " + result.IsErrorTransient.ToString();
                                if (result.ModemErrorCode == SmsModemErrorCode.MessagingNetworkError)
                                {
                                    msgStr += "\n\tNetworkCauseCode: " + result.NetworkCauseCode.ToString();

                                    if (result.CellularClass == CellularClass.Cdma)
                                    {
                                        msgStr += "\n\tTransportFailureCause: " + result.TransportFailureCause.ToString();
                                    }
                                    rootPage.NotifyUser(msgStr, NotifyType.ErrorMessage);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                        }
                    }
                    else
                    {
                        if (Mnumber != "default")
                            rootPage.NotifyUser("Could not connect to network. SMS could not be sent", NotifyType.ErrorMessage);
                        else
                            rootPage.NotifyUser("You have not yet entered your emergency contact number!", NotifyType.ErrorMessage);
                    }
                }
                catch (Exception ex)
                { // No action needed
                }
            }
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WelcomePage));
        }

    }

}
    
