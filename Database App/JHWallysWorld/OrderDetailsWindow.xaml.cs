/*
* FILE:				OrderDetailsWindow.xaml.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/07/2016
* DESCRIPTION:
*	This file handles all events actioned from the front-end user interface of the popup window.
*	It initializes the interface and updates a public property (as "return" information) when the window closes.
*/


using System.Windows;

namespace JHWallysWorld
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private int returnCode;
        public OrderDetailsWindow(string message, string status)
        {
            InitializeComponent();
            messageTextBox.Text = message;
            returnCode = 0;
            if (status == "PEND")
            {
                cancelButton.IsEnabled = true;
            }
            else if(status == "PAID")
            {
                refundButton.IsEnabled = true;
            }

        }

        public int ReturnCode
        {
            get { return returnCode; }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            returnCode = 1;
            this.Close();
        }

        private void refundButton_Click(object sender, RoutedEventArgs e)
        {
            returnCode = 2;
            this.Close();
        }
    }
}
