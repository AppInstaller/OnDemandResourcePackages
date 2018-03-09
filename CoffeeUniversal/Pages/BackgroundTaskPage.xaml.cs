using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class BackgroundTaskPage : Page
    {

        #region Init

        private NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public BackgroundTaskPage()
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


		#region Register/Unregister Background Task

		// NOTE: Both the ToastsPage and the BackgroundTaskPage can register/unregister the same TZ background task.

		private async void timeZoneTask_Click(object sender, RoutedEventArgs e)
        {
            bool result = await BackgroundTaskHelper.RegisterTimeZoneChangeBackgroundTask(
                LocalizableStrings.BACKGROUND_TASK_TIMEZONE_NAME, LocalizableStrings.BACKGROUND_TASK_TIMEZONE_ENTRYPOINT);
            if (result)
            {
                status.Log(LocalizableStrings.BACKGROUND_TASK_REGISTRATION_SUCCESS);
            }
        }

        private async void appTriggerTask_Click(object sender, RoutedEventArgs e)
        {
            bool result = await BackgroundTaskHelper.RegisterApplicationTriggerBackgroundTask(
                LocalizableStrings.BACKGROUND_TASK_APPTRIGGER_NAME, LocalizableStrings.BACKGROUND_TASK_APPTRIGGER_ENTRYPOINT);
            if (result)
            {
                status.Log(LocalizableStrings.BACKGROUND_TASK_REGISTRATION_SUCCESS);
            }
        }

        private void unregisterTasks_Click(object sender, RoutedEventArgs e)
        {
            BackgroundTaskHelper.UnregisterBackgroundTasks();
            status.Log(LocalizableStrings.BACKGROUND_TASK_UNREGISTERED);
        }

        #endregion

    }
}
