using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

namespace CoffeeUniversal.Pages
{
	public sealed partial class SharePage : Page
    {
        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public SharePage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

		protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("SharePage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }

		private void shareText_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			ShareHelper.ShareText(LocalizableStrings.SHARE_TEXT_CONTENT);
		}

		private void shareUri_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			ShareHelper.ShareUri(new Uri("http://en.wikipedia.org/wiki/Coffee", UriKind.Absolute));
		}

		async private void shareFile_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			string installPath = Package.Current.InstalledLocation.Path;
			StorageFile file = await StorageFile.GetFileFromPathAsync(installPath + @"\Assets\Share\espresso_450x450.jpg");
			ShareHelper.ShareFiles(file);
		}
	}
}
