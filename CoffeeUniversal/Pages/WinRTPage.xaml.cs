using CoffeeUniversal.Helpers;
using System.Globalization;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class WinRTPage : Page
    {

		#region Standard init

		private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public WinRTPage()
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
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private void getStringLength_Click(object sender, RoutedEventArgs e)
        {
			GetStringLength();
		}

		private void GetStringLength()
		{
			//CoffeeWinRT.StringHelper stringHelper = new CoffeeWinRT.StringHelper();
			//stringLength.Text = stringHelper.GetLength(inputString.Text).ToString(CultureInfo.CurrentCulture);
		}

		private void inputString_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			// Respond to Enter as equal to tapping the button.
			if (e.Key == VirtualKey.Enter)
			{
				e.Handled = true;
				GetStringLength();
			}
		}
	}
}
