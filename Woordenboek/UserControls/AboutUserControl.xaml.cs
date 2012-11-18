using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Woordenboek.UserControls
{
    public sealed partial class AboutUserControl : UserControl
    {
        public AboutUserControl()
        {
            this.InitializeComponent();
        }

        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
            SettingsPane.Show();
        }

        private async void MailMartenButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Create the URI to launch from a string.
            var uri = new Uri(@"mailto:marten@fabrique.nl?subject=Woordenboek Windows 8");

            // Launch the URI.
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void MailMichielButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Create the URI to launch from a string.
            var uri = new Uri(@"mailto:michiel@michielpost.nl?subject=Woordenboek Windows 8");

            // Launch the URI.
            bool success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
