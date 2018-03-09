using CoffeeUniversal.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class IoTExtensionsPage : Page
    {
        public IoTExtensionsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

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


        private void gpioPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GpioPinsPage));
        }

        private void i2cBusPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(I2cBusPage));
        }

        private void spiBusPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SpiBusPage));
        }
    }
}
