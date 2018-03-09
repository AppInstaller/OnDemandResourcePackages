using CoffeeUniversal.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class CrashPage : Page
    {

        #region init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public CrashPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
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


        private void crash_Click(object sender, RoutedEventArgs e)
        {
            throw new UnhandleableException("Deliberate unhandled exception");
        }
    }
}
