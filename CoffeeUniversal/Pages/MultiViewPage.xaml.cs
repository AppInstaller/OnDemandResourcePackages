using CoffeeUniversal.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using CoffeeUniversal.Helpers;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class MultiViewPage : Page
	{

        #region standard init

        private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
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

        private int viewCount = 1;

        public MultiViewPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            viewList.ItemsSource = ((App)Application.Current).SecondaryViews;
        }

        async private void createView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set up the secondary view, but don't show it yet.
                ViewLifetimeController viewControl = null;
                await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                // This object is used to keep track of the views and important
                // details about the contents of those views across threads
                // In your app, you would probably want to track information
                // like the open document or page inside that window.
                viewControl = ViewLifetimeController.CreateForCurrentView();
                    viewControl.Title = string.Format(CultureInfo.CurrentCulture, 
                        LocalizableStrings.MULTIVIEW_DEFAULT_TITLE, viewCount++);

                    Frame frame = new Frame();
                    frame.Navigate(typeof(SecondaryViewPage), viewControl);
                    Window.Current.Content = frame;
                    ApplicationView.GetForCurrentView().Title = viewControl.Title;
                });

                if (viewControl != null)
                {
                    ((App)Application.Current).SecondaryViews.Add(viewControl);
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.MULTIVIEW_CREATE_VIEW_SUCCESS, viewControl.Title));
                }
                else
                {
                    status.Log(LocalizableStrings.MULTIVIEW_CREATE_VIEW_FAIL);
                }
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
                Debug.WriteLine("MultiViewPage.createView_Click: " +ex.ToString());
            }
        }

        async private void showView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prevent the view from closing while switching to it
                ViewLifetimeController selectedView = viewList.SelectedItem as ViewLifetimeController;
                selectedView.StartViewInUse();

                // Show the previously created secondary view, using the size
                // preferences the user specified. 
                bool isViewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    selectedView.Id, ViewSizePreference.Default,
                    ApplicationView.GetForCurrentView().Id, ViewSizePreference.Default);

                if (isViewShown)
                {
                    // If we succefully showed the view, make sure we update our internal state 
                    // to reflect that the view is no longer consolidated.
                    selectedView.Consolidated = false;
                    status.Log(string.Format(CultureInfo.CurrentCulture,
                        LocalizableStrings.MULTIVIEW_SHOW_VIEW_SUCCESS, selectedView.Title));
                }
                else
                {
                    // The window wasn't actually shown, so release the reference to it.
                    // This might trigger the window to be destroyed.
                    status.Log(LocalizableStrings.MULTIVIEW_SHOW_VIEW_FAIL);
                }

                // Signal that switching has completed and let the view close
                selectedView.StopViewInUse();
            }
            catch (InvalidOperationException ex)
            {
                // The view could be in the process of closing, and
                // this thread just hasn't updated. As part of being closed,
                // this thread will be informed to clean up its list of
                // views (see SecondaryViewPage.xaml.cs).
                Debug.WriteLine("MultiViewPage.showView_Click: " +ex.ToString());
                status.Log(ex.Message);
            }
        }

        private void viewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewList.SelectedIndex != -1)
            {
                showView.IsEnabled = true;
            }
            else
            {
                showView.IsEnabled = false;
            }
        }
    }
}
