using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ShareIt
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : ShareIt.Common.LayoutAwarePage
    {
        private ResourceLoader resources;

        public MainPage()
        {
            this.InitializeComponent();
            RegisterForSharing();
            resources = new ResourceLoader();
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

        #region Share
        private void RegisterForSharing()
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareUri;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FromClipboard();
        }

        private async void FromClipboard()
        {
            var clipboardContent = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

            if (clipboardContent.AvailableFormats.Contains("Uri"))
            {
                textboxUri.Text = (await clipboardContent.GetUriAsync()).ToString();
            }
            else if (clipboardContent.AvailableFormats.Contains("Text"))
            {
                var text = await clipboardContent.GetTextAsync();
                if (text != null)
                {
                    if (text.Contains("\r")) text = text.Substring(text.IndexOf("\r"));

                    textboxUri.Text = text;
                }
            }

            textboxUri.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void ShareUri(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try {
                if (textboxUri.Text.Contains("//") == false)
                {
                    textboxUri.Text = "http://" + textboxUri.Text;
                }

                var uri = new Uri(textboxUri.Text);

                var request = args.Request;
                request.Data.SetUri(uri);
                request.Data.Properties.Title = uri.Host;                
            }
            catch (Exception) {
                ShowDialog(resources.GetString("ErrorMessageMalformedUri"));
            }
        }
        #endregion

        private async void ShowDialog(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }

        private void Preview(object sender, RoutedEventArgs e)
        {
            try {
                if (textboxUri.Text.Contains("//") == false)
                {
                    textboxUri.Text = "http://" + textboxUri.Text;
                }

                var uri = new Uri(textboxUri.Text);

                webViewPreview.Visibility = Windows.UI.Xaml.Visibility.Visible;
                webViewPreview.Navigate(uri);

                //WebViewStoryBoard.Begin();
            }
            catch (Exception)
            {
                ShowDialog(new ResourceLoader().GetString("ErrorMessageMalformedUri"));
            }            
        }

        private void Share(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void Paste(object sender, RoutedEventArgs e)
        {
            FromClipboard();
        }

        private void LoadCompleted(object sender, NavigationEventArgs e)
        {
            webViewProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            ShowDialog(resources.GetString("ErrorNavigating"));

            webViewProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

    }
}
