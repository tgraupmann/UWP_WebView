using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool _mPendingResult = false;

        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(600, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private async Task OnComplete(string content)
        {
            if (_mPendingResult)
            {
                _mPendingResult = false;

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        const int maxLength = 1024;
                        if (content.Length > maxLength)
                        {
                            _mTxtResult.Text = content.Substring(0, maxLength);
                        }
                        else
                        {
                            _mTxtResult.Text = content;
                        }
                    }
                    else
                    {
                        _mTxtResult.Text = "(Empty)";
                    }
                });
            }
        }

        private async Task OnError(string error)
        {
            _mPendingResult = false;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _mTxtResult.Text = error;
            });
        }

        private async void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            _mPendingResult = true;

            LoginPage.PageArgs pageArgs = new LoginPage.PageArgs();
            pageArgs.StartUrl = _mTxtUrl.Text;
            pageArgs.EndUrl = "https://www.twitch.com/";
            pageArgs.OnComplete = OnComplete;
            pageArgs.OnError = OnError;

            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(LoginPage), pageArgs);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mTxtUrl.Text = string.Format("https://example.com/OAuth2?guid={0}", Guid.NewGuid());
        }
    }
}
