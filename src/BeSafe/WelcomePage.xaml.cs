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
using SDKTemplate;
using System.Threading;


namespace SmsSendAndReceive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
        }


        private void WelcomeButton_click(object sender, RoutedEventArgs e)
        {
           
            // Check the validity of the formt of the number entered by the user, and pass it as an object to SendTexyMessage Page.
            string number;
            bool error = false;
            int i;
            number = WelcometextBox.Text;

            // Checking that the entered number contains only decimal numbers by comparing ASCII values
            for(i=0;i<number.Length;i++)
            {
                // Checking ASCII value violation
                if((int)number[i]> 57||(int)number[i]<48)
                {
                    error = true;
                }
            }
            // Length of number entered must be 10 and no error should be present in ASCII values
            if (number.Length != 10 || error == true)
            {
                InvalidNumberMessageTextBlock.Text = "You have not entered a valid mobile number! Please check and try again!";
            }
            // Else the entered number has a valid format, pass it as an argument to next page.
            else
            {
                // Concatenating the country code.
                number = CountryCodeTextBox.Text + number;
                Frame.Navigate(typeof(SendTextMessage), number);
            }

        }

        private void WelcometextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
