using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ShareIt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareTarget : Page
    {
        public ShareTarget()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {            
            var args = e.Parameter as ShareTargetActivatedEventArgs;
            
            var shareOperation = args.ShareOperation;

            if (shareOperation.Data.Contains(StandardDataFormats.Uri))
                textboxLink.Text = (await shareOperation.Data.GetUriAsync()).ToString();            
        }

        private void CopyToClipBoard(object sender, RoutedEventArgs e)
        {
            var content = new DataPackage();
            content.SetUri(new Uri(textboxLink.Text));
            content.SetText(textboxLink.Text);

            Clipboard.SetContent(content);
        }
    }
}
