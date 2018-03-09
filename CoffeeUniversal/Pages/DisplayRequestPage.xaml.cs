using CoffeeUniversal.Helpers;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
    public sealed partial class DisplayRequestPage : Page
	{

		#region Init

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

		public DisplayRequestPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);
		}

		#endregion


		private void activate_Click(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			if (b != null)
			{
                DisplayRequestInfo info = App.ActivateDisplayRequest();
                switch (info.ActivationResult)
                {
                    case ActivationResult.Activated : 
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.DISPLAY_REQUEST_ACTIVATED, info.ActivationTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture)));
                        break;
                    case ActivationResult.PreviouslyActivated :
                        status.Log(string.Format(CultureInfo.CurrentCulture,
                            LocalizableStrings.DISPLAY_REQUEST_ALREADY_ACTIVATED, info.ActivationTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture)));
                        break;
                    default:
                        status.Log(LocalizableStrings.DISPLAY_REQUEST_NOT_ACTIVATED);
                        break;
                }
            }
		}

		private void release_Click(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			if (b != null)
			{
                DisplayRequestInfo info = App.ReleaseDisplayRequest();
                switch (info.ActivationResult)
                {
                    case ActivationResult.Released:
                        status.Log(LocalizableStrings.DISPLAY_REQUEST_RELEASED);
                        break;
                    default:
                        status.Log(LocalizableStrings.DISPLAY_REQUEST_NOT_ACTIVATED);
                        break;
                }
            }
        }
	}
}
