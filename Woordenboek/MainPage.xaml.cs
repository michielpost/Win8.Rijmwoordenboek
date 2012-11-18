using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using NotificationsExtensions.TileContent;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Woordenboek.Services;
using Woordenboek.ViewModel;
using Windows.ApplicationModel.DataTransfer;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Woordenboek
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Woordenboek.Common.LayoutAwarePage
    {
        DispatcherTimer Timer { get; set; }
        double NewHorizontalOffset { get; set; }
        double HorizontalOffsetIncrement { get; set; }
        double CurrentHorizontalOffset { get; set; }
        ScrollViewer ScrollViewer { get; set; }
        double ViewportWidth { get; set; }
        double HorizontalOffset { get; set; }
        double Intervals { get; set; }

        private DataTransferManager dtm;

        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            ScrollViewer = Scroller;
            Intervals = 0.1 * 120; // number of timer Intervals
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1.0 / 120.0);
            Timer.Tick += Timer_Tick;

            this.Loaded += MainPage_Loaded;

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            
        }


        void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Back
                && InputTextBox.FocusState == Windows.UI.Xaml.FocusState.Unfocused
                && Scroller.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                Left();
                InputTextBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            }
           

        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        void Timer_Tick(object sender, object e)
        {
            CurrentHorizontalOffset += HorizontalOffsetIncrement;
            if (HorizontalOffsetIncrement > 0 && CurrentHorizontalOffset > NewHorizontalOffset ||
                HorizontalOffsetIncrement < 0 && NewHorizontalOffset > CurrentHorizontalOffset)
            {
                Timer.Stop();
            }
            else
            {
                ScrollViewer.ScrollToHorizontalOffset(CurrentHorizontalOffset);
            }
        }

        // This member function scrolls the ScrollViewer up ViewportWidth - 75
        // device-independent units so that there is overlap between one view and the next.
        public void Left()
        {
            // The user can change this between expansions/collapses.
            ViewportWidth = ScrollViewer.ViewportWidth;

            // The user can change this by moving the thumb control.
            // Equivalent to the data type Animation.From property.
            HorizontalOffset = ScrollViewer.HorizontalOffset;

            // Equivalent to the data type Animation.To property.
            NewHorizontalOffset = HorizontalOffset - ViewportWidth + 75;

            // We don't want to try to scroll out of the ScrollViewer.
            if (NewHorizontalOffset < 0)
            {
                NewHorizontalOffset = 0;
            }
            HorizontalOffsetIncrement = (NewHorizontalOffset - HorizontalOffset) / Intervals;
            if (HorizontalOffsetIncrement == 0.0)
            {
                return;
            }
            CurrentHorizontalOffset = HorizontalOffset;
            Timer.Start();
        }

        // This member function scrolls the ScrollViewer down ViewportWidth - 75
        // device-independent units so that there is overlap between one view and the next.
        public void Right()
        {
            // The user can change this between expansions/collapses.
            ViewportWidth = ScrollViewer.ViewportWidth;

            // The user can change this by moving the thumb control.
            // Equivalent to the data type Animation.From property.
            HorizontalOffset = ScrollViewer.HorizontalOffset;

            // Equivalent to the data type Animation.To property.
            NewHorizontalOffset = HorizontalOffset + ViewportWidth - 75;

            // We don't want to try to scroll out of the ScrollViewer.
            if (NewHorizontalOffset > ScrollViewer.ExtentWidth)
            {
                NewHorizontalOffset = ScrollViewer.ExtentWidth;
            }
            HorizontalOffsetIncrement = (NewHorizontalOffset - HorizontalOffset) / Intervals;
            if (HorizontalOffsetIncrement == 0.0)
            {
                return;
            }
            CurrentHorizontalOffset = HorizontalOffset;
            Timer.Start();
        }


       


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            dtm = DataTransferManager.GetForCurrentView();

            if (dtm != null)
            {
                dtm.DataRequested += dtm_DataRequested;
            }

            if (!string.IsNullOrEmpty((string)e.Parameter))
            {
                string search  = (string)e.Parameter;
                InputTextBox.Text = search;

                DoSearch(search);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            if (dtm != null)
            {
                dtm.DataRequested -= dtm_DataRequested;
            }

        }

        void dtm_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {

            if (ViewModel != null && ViewModel.SearchWord != null)
            {
                args.Request.Data.Properties.Title = string.Format(ViewModel.SearchWord.Word);

                args.Request.Data.SetText(ViewModel.SearchWord.GetAsText());
                args.Request.Data.SetHtmlFormat(ViewModel.SearchWord.GetAsHtml());

            }
            else
            {
                args.Request.FailWithDisplayText("Zoek naar een woord om de share functie te activeren.");
            }
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Right")
            {
                Right();
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

       

        private void InputTextBox_KeyDown_1(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;

                DoSearch(((TextBox)sender).Text);
            }

        }

        private async void DoSearch(string query)
        {

            if (string.IsNullOrEmpty(query))
                return;

            ViewModel.SearchText(query);

          

            GoToResultGrid();

          
        }

        private void GoToResultGrid()
        {

            SearchButton.Focus(Windows.UI.Xaml.FocusState.Programmatic);

            if (Scroller.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                SearchButton.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
            else
                InputTextBox2.Focus(Windows.UI.Xaml.FocusState.Keyboard);


            Right();
        }

        private void HistoryListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var result = (SearchWord)e.ClickedItem;

            if (result != null && result.Results != null)
            {
                ViewModel.SearchText(result.Word);

              
            }

        }

        private void Delete_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteHistory();
           

        }

        private void RootGrid_LayoutUpdated_1(object sender, object e)
        {
            if (RootGrid.ActualWidth > 0)
            {
                SearchGrid.Width = RootGrid.ActualWidth;
                ResultGrid.MinWidth = RootGrid.ActualWidth - 120;
            }

        }

        private void SearchButton_Click_1(object sender, RoutedEventArgs e)
        {
            DoSearch(InputTextBox.Text);
        }

        private void InputTextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            Left();
        }

       
    }
}
