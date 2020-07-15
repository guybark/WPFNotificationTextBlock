using System.Windows;

namespace WPFNotificationTextBlock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RaiseEventButton_Click(object sender, RoutedEventArgs e)
        {
            // Some screen readers may always interrupt an existing announcement 
            // with a FocusChanged announcement. So make any focus changes before
            // raising the UIA Notification event.

            // For this demo app, move focus to another button.
            NextButton.Focus();

            RaiseEventButton.IsEnabled = false;

            // Set some visual status text.
            StatusTextBlock.Text = "All systems operating as required.";

            // Now raise a UIA Notification event. Note that if the text to be 
            // announced is the same as some text shown visually, then typically
            // a LiveRegionChanged event would be raised. The UIA Notification 
            // event is only being raised here to demonstrate how to raise the
            // event for situations where for soe reason that event is more 
            // helpful than a LiveRegionChanged event.
            StatusTextBlock.RaiseNotificationEvent(
                StatusTextBlock.Text,
                "Status update");
        }
    }
}
