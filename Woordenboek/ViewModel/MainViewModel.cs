using GalaSoft.MvvmLight;
using NotificationsExtensions.TileContent;
using Q42.WinRT.Data;
using System.Collections.Generic;
using Windows.UI.Notifications;
using Woordenboek.Services;

namespace Woordenboek.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public DataLoader DataLoader { get; set; }

        private string _query;

        public string Query
        {
            get { return _query; }
            set { _query = value;
            RaisePropertyChanged("Query");
            }
        }



        private SearchWord _searchWord;

        public SearchWord SearchWord
        {
            get { return _searchWord; }
            set { _searchWord = value;
            RaisePropertyChanged("SearchWord");
            }
        }

        private List<SearchWord> _historyList;

        public List<SearchWord> HistoryList
        {
            get { return _historyList; }
            set { _historyList = value;
            RaisePropertyChanged("HistoryList");
            }
        }

        private bool _right;

        public bool Right
        {
            get { return _right; }
            set { _right = value;
            RaisePropertyChanged("Right");
            }
        }
        


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            DataLoader = new DataLoader(true);
            LoadHistory();

        }

        private async void LoadHistory()
        {
            HistoryList = await HistoryService.GetHistory();

        }


        public async void SearchText(string query)
        {
            try
            {
                // create the square template and attach it to the wide template
                ITileSquarePeekImageAndText04 squareContent = TileContentFactory.CreateTileSquarePeekImageAndText04();
                squareContent.TextBodyWrap.Text = query;
                squareContent.Image.Src = "ms-appx:///Assets/Logo.png";
                squareContent.Branding = TileBranding.Name;

                // send the notification
                TileUpdateManager.CreateTileUpdaterForApplication().Update(squareContent.CreateNotification());
            }
            catch { }


            //Show loader:
            //LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //LoadingPanel2.Visibility = Windows.UI.Xaml.Visibility.Visible;

            var result = await DataLoader.LoadAsync(() => SearchService.SearchAsync(query));

            //TODO Hide loader
            //LoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //LoadingPanel2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


            if (result != null)
            {
                if (result.Results != null && result.Results.Count > 0)
                {
                    //ResultPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //ResultPanel2.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    //ResultGrid.DataContext = result;
                    //SnapGrid.DataContext = result;
                    SearchWord = result;

                    //ResultList.ItemsSource = result.Results;

                    var historyList = await HistoryService.GetHistory();
                    HistoryList = historyList;

                    //Scroller.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    Right = true;
                    //Scroller.ScrollToHorizontalOffset(double.MaxValue);
                }
                else
                {
                    //TODO: Show niet in woordenboek
                    SearchWord = null;

                    //NotAvailablePanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //NotAvailablePanel2.Visibility = Windows.UI.Xaml.Visibility.Visible;

                }
            }
            else
            {
                //TODO: Show error
                DataLoader.LoadingState = LoadingState.Error;
                //ErrorPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //ErrorPanel2.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
        }

        internal void DeleteHistory()
        {
            HistoryService.Clear();
            HistoryList = null;

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }
    }
}