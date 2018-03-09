using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class DesktopExtensionsPage : Page
    {

        #region init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public DesktopExtensionsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            if (ApiInformation.IsTypePresent("Windows.UI.ApplicationSettings.ApplicationsSettingsContract"))
            {
                status.Log(LocalizableStrings.DESKTOP_EXTENSION_SETTINGS_FOUND);
                showSettings.IsEnabled = true;
            }
            else
            {
                status.Log(LocalizableStrings.DESKTOP_EXTENSION_SETTINGS_NOT_FOUND);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        #endregion


        private void showSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsPane.Show();
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }
    }
}
