using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class MobileExtensionsPage : Page
    {

        #region init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public MobileExtensionsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Power.Battery"))
            {
                status.Log(LocalizableStrings.MOBILE_EXTENSION_BATTERY_FOUND);
                getBatteryInfo.IsEnabled = true;
            }
            else
            {
                status.Log(LocalizableStrings.MOBILE_EXTENSION_BATTERY_NOT_FOUND);
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


        private void getBatteryInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Phone.Devices.Power.Battery battery = Windows.Phone.Devices.Power.Battery.GetDefault();
                int remainingCharge = battery.RemainingChargePercent;
                TimeSpan remainingTime = battery.RemainingDischargeTime;
                batteryInfo.Text = string.Format("{0}%, {1} mins", remainingCharge, remainingTime.Minutes);
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }

    }
}
