using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using CoffeeUniversal.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CoffeeUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionsPage : Page
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

        #endregion
        public ExtensionsPage()
        {
            this.InitializeComponent();

            navigationHelper = new NavigationHelper(this);
        }

        private void DesktopPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DesktopExtensionsPage));
        }

        private void MobilePage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MobileExtensionsPage));
        }

        private void XboxPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(XboxExtensionsPage));
        }

        private void IoTPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IoTExtensionsPage));
        }

        private void Navigation_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled & this.Frame.CanGoBack)
            {
                e.Handled = true;
                this.Frame.GoBack();
            }
        }
    }
}
