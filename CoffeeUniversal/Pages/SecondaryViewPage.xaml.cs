using System;
using CoffeeUniversal.Helpers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class SecondaryViewPage : Page
    {
        private ViewLifetimeController thisViewControl;
        private int mainViewId;
        private CoreDispatcher mainDispatcher;

        public SecondaryViewPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            thisViewControl = (ViewLifetimeController) e.Parameter;
            mainViewId = ((App)Application.Current).MainViewId;
            mainDispatcher = ((App)Application.Current).MainDispatcher;
            thisViewControl.Released += ViewLifetimeControl_Released;
            title.Text = thisViewControl.Title;
            Window.Current.Activate();
        }

        private async void ViewLifetimeControl_Released(Object sender, EventArgs e)
        {
            ((ViewLifetimeController)sender).Released -= ViewLifetimeControl_Released;
            // The ViewLifetimeControl object is bound to UI elements on the main thread
            // So, the object must be removed from that thread.
            await mainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ((App)Application.Current).SecondaryViews.Remove(thisViewControl);
            });

            // The released event is fired on the thread of the window it pertains to.
            //
            // It's important to make sure no work is scheduled on this thread
            // after it starts to close (no data binding changes, no changes to
            // XAML, creating new objects in destructors, etc.) since
            // that will throw exceptions.
            Window.Current.Close();
        }

        async private void gotoMainPage_Click(object sender, RoutedEventArgs e)
        {
            // Switch to the main view without explicitly requesting
            // that this view be hidden.
            thisViewControl.StartViewInUse();
            await ApplicationViewSwitcher.SwitchAsync(mainViewId);
            thisViewControl.StopViewInUse();
        }

        async private void hideView_Click(object sender, RoutedEventArgs e)
        {
            // Switch to main and hide this view entirely from the user.
            thisViewControl.StartViewInUse();
            await ApplicationViewSwitcher.SwitchAsync(mainViewId,
                ApplicationView.GetForCurrentView().Id,
                ApplicationViewSwitchingOptions.ConsolidateViews);
            thisViewControl.StopViewInUse();
        }
    }
}
