using Woordenboek.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.ApplicationModel.Resources;
using Woordenboek.Helpers;
using Woordenboek.UserControls;
using Windows.ApplicationModel.Search;
using Windows.System.UserProfile;
using Windows.Globalization;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace Woordenboek
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        private bool _isSearchReg;
        private bool _isSettingsReg;
        private ResourceLoader _resourceLoader = new ResourceLoader("Resources");

        public static Frame RootFrame;


        // This is the SearchPane object
        private SearchPane searchPane;

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            //Set language
            DetectDutchLanguage();


            // Create a Frame to act navigation context and navigate to the first page
            RootFrame = new Frame();


            // Create a Frame to act as the navigation context and associate it with
            // a SuspensionManager key
            SuspensionManager.RegisterFrame(RootFrame, "AppFrame");

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                await SuspensionManager.RestoreAsync();
            }

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!RootFrame.Navigate(typeof(MainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = RootFrame;
            Window.Current.Activate();

            RegisterForSettings();

            RegisterForSearch();


        }

        private void RegisterForSettings()
        {
            if (!_isSettingsReg)
            {
                _isSettingsReg = true;

                try
                {
                    SettingsPane.GetForCurrentView().CommandsRequested -= App_CommandsRequested;
                }
                catch { }
                SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
            }
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (args.Request.ApplicationCommands.Count == 0)
            {
                // Add an About command
                var about = new SettingsCommand("about", _resourceLoader.GetString("AppBarAboutTitle"), (handler) =>
                {
                    var settings = new SettingsFlyout();
                    settings.ShowFlyout(new AboutUserControl());
                });
                args.Request.ApplicationCommands.Add(about);

                // Add an Privacy command
                var privacy = new SettingsCommand("privacy", "Privacy Policy", (handler) =>
                {
                    var settings = new SettingsFlyout();
                    settings.ShowFlyout(new PrivacyUserControl());
                });
                args.Request.ApplicationCommands.Add(privacy);


               
            }

        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }


        async protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            base.OnSearchActivated(args);

            EnsureMainPageAsync(args);
            //((MainPage)Window.Current.Content).Test(args.QueryText);

            RegisterForSettings();

            RegisterForSearch();

            //Set language
            DetectDutchLanguage();


            DoSearch(args.QueryText);
        }

        private void DoSearch(string queryText)
        {
            RootFrame.Navigate(typeof(MainPage), queryText);
        }

        private void EnsureMainPageAsync(IActivatedEventArgs args)
        {

            // If the window isn't already using Frame navigation, insert our own frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;
            if (frame == null)
            {
             

                // Create a Frame to act navigation context and navigate to the first page
                RootFrame = new Frame();

                RootFrame.Navigate(typeof(MainPage));

                // Place the frame in the current Window and ensure that it is active
                Window.Current.Content = RootFrame;
                Window.Current.Activate();
            }
            // Use navigation to display the results, packing both the query text and the previous
            // Window content into a single parameter object
            //frame.Navigate(typeof(MainPage));
            // The window must be activated in 15 seconds
            //Window.Current.Activate();

        }

        private void RegisterForSearch()
        {
            if (!_isSearchReg)
            {
                _isSearchReg = true;

                // Get Search Pane object
                this.searchPane = SearchPane.GetForCurrentView();
                searchPane.PlaceholderText = _resourceLoader.GetString("AppSearchPlaceHolder");

                // Register for Search Pane QuerySubmitted event
                try
                {
                    this.searchPane.QuerySubmitted -= searchPane_QuerySubmitted;
                }
                catch { }
                this.searchPane.QuerySubmitted += searchPane_QuerySubmitted;
                //this.searchPane.QueryChanged += searchPane_QueryChanged;

             
            }
        }

    
        async void searchPane_QuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
        {
            string queryText = args.QueryText;

            DoSearch(queryText);

        }

        private static void DetectDutchLanguage()
        {
            bool forceDutch = false;
            var userlanguages = GlobalizationPreferences.Languages;
            if (userlanguages.Contains("nl-NL"))
            {
                forceDutch = true;
            }

            GeographicRegion userRegion = new GeographicRegion();
            if (userRegion.CodeTwoLetter.ToLower() == "nl")
            {
                forceDutch = true;
            }

            if (forceDutch)
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "nl";
            else
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = string.Empty;
        }


    }
}
