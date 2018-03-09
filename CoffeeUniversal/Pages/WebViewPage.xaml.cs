using CoffeeUniversal.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class WebViewPage : Page
	{

		#region Standard init

		private NavigationHelper navigationHelper;

		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion


		#region SizeChanged

		private double horizontalBorder = 40;
		private double verticalBorder = 56;

		private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double heightExcludingRemoteBrowser =
				headerRow.ActualHeight + remoteLabel.ActualHeight + localLabel.ActualHeight +
                localWebView.ActualHeight + appToScript.ActualHeight +
				verticalBorder;
			double maximumHeight = ActualHeight - heightExcludingRemoteBrowser;
			if (maximumHeight > 0)
			{
				remoteBorder.Height = maximumHeight;
			}

			double maximumWidth = ActualWidth - horizontalBorder;
			if (maximumWidth > 0)
			{
				remoteBorder.Width = maximumWidth;
				localBorder.Width = maximumWidth;
			}
			UpdateLayout();
		}

		#endregion


		public WebViewPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);
            remoteWebView.Loaded += new RoutedEventHandler(remoteWebView_Loaded);
            localWebView.Loaded += new RoutedEventHandler(localWebView_Loaded);
		}

		private void remoteWebView_Loaded(object sender, RoutedEventArgs e)
		{
            remoteWebView.Navigate(new Uri("http://en.wikipedia.org/wiki/Coffee", UriKind.Absolute));
		}

		private void localWebView_Loaded(object sender, RoutedEventArgs e)
		{
            localWebView.Navigate(new Uri("ms-appx-web:///SimplePage.html", UriKind.RelativeOrAbsolute));
		}

		private async void InvokeScript_Click(object sender, RoutedEventArgs e)
		{
			await localWebView.InvokeScriptAsync("DataFromApp", new string[] { appToScript.Text });
		}

	}
}
