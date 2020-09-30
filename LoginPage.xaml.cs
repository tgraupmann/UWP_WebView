using System;
using System.Threading.Tasks;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public class PageArgs
        {
            public string StartUrl = string.Empty;
            public string EndUrl = string.Empty;
            public delegate Task ContentCallback(string content);
            public ContentCallback OnComplete = null;
            public ContentCallback OnError = null;
        }

        private PageArgs _mPageArgs = null;


        public LoginPage()
        {
            this.InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += (s, e) =>
            {
                // Code run when the window is closing
                if (_mPageArgs != null)
                {
                    if (_mPageArgs.OnError != null)
                    {
                        _mPageArgs.OnError("User closed the window!");
                    }
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is PageArgs)
            {
                _mPageArgs = (PageArgs)e.Parameter;
                _mWebView.Language = "en-US";
                Windows.UI.Xaml.Media.TranslateTransform transform = new Windows.UI.Xaml.Media.TranslateTransform();
                transform.X = -200;
                _mWebView.RenderTransform = transform;
                _mWebView.Source = new Uri(_mPageArgs.StartUrl);
            }

        }

        private async void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (_mPageArgs != null)
            {
                string url = _mWebView.Source.ToString();
                if (url.StartsWith("https://www.twitch.tv/"))
                {
                    if (url.StartsWith("https://www.twitch.tv/login"))
                    {
                    }
                    else
                    {
                        //invoke callback and close window
                        if (_mPageArgs.OnComplete != null)
                        {
                            string html = await _mWebView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                            await _mPageArgs.OnComplete(html);
                        }
                        _mPageArgs = null;
                        Window.Current.Close();
                    }
                }
                else
                {
                    if (url.StartsWith(_mPageArgs.EndUrl))
                    {
                        if (_mPageArgs.OnComplete != null)
                        {
                            string html = await _mWebView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
                            await _mPageArgs.OnComplete(html);
                            _mPageArgs = null;
                            Window.Current.Close();
                        }
                    }
                }
            }
        }
    }
}
