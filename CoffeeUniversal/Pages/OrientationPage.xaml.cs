using CoffeeUniversal.Helpers;
using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class OrientationPage : Page
    {
        private List<string> photos = new List<string>();
        private DisplayInformation displayInformation;
        private NavigationHelper navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        public OrientationPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            photos.Add("/Assets/Orientation/beans.jpg");
            photos.Add("/Assets/Orientation/espresso.jpg");
            photos.Add("/Assets/Orientation/latte.jpg");
            photos.Add("/Assets/Orientation/Gaggia.jpg");
            photos.Add("/Assets/Orientation/cappuccino.jpg");
            photos.Add("/Assets/Orientation/grinder.jpg");

            LandscapeList.ItemsSource = photos;
            PortraitList.ItemsSource = photos;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            displayInformation = DisplayInformation.GetForCurrentView();
            displayInformation.OrientationChanged += displayInformation_OrientationChanged;
        }

        private void displayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            switch (displayInformation.CurrentOrientation)
            {
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    LandscapeList.Visibility = Visibility.Visible;
                    PortraitList.Visibility = Visibility.Collapsed;
                    break;
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    LandscapeList.Visibility = Visibility.Collapsed;
                    PortraitList.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
