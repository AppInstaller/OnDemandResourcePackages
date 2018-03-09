using System.Globalization;
using CoffeeUniversal.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;
using System.Collections.Generic;
using Windows.System;
using System;

namespace CoffeeUniversal.Pages
{
    public sealed partial class ExtendedExecutionPage : Page
    {

        #region Init

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
            if (App.IsExtensionWorkRunning)
            {
                startWork.IsEnabled = false;
                stopWork.IsEnabled = true;
            }
            else
            {
                startWork.IsEnabled = true;
                stopWork.IsEnabled = false;
            }
        }

        public ExtendedExecutionPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

        #endregion


        #region Extended Execution

        private void requestExtension_Click(object sender, RoutedEventArgs e)
        {
            App.RequestExtension();
            status.Log(status, string.Format(CultureInfo.CurrentCulture,
                LocalizableStrings.EXTENDED_EXECUTION_TEXT, App.IsExtensionEnabled));
            UpdateButtons();
        }

        private void cancelExtension_Click(object sender, RoutedEventArgs e)
        {
            App.CancelExtension();
            status.Log(status, LocalizableStrings.EXTENDED_EXECUTION_CANCELLED);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            requestExtension.IsEnabled = !App.IsExtensionEnabled;
            cancelExtension.IsEnabled = App.IsExtensionEnabled;
        }

        private void startWork_Click(object sender, RoutedEventArgs e)
        {
            startWork.IsEnabled = false;
            stopWork.IsEnabled = true;
            stopWork.Focus(FocusState.Programmatic);
            App.StartWork();
            status.Log(LocalizableStrings.EXTENDED_EXECUTION_WORK_STARTED);
        }

        private void stopWork_Click(object sender, RoutedEventArgs e)
        {
            stopWork.IsEnabled = false;
            startWork.IsEnabled = true;
            startWork.Focus(FocusState.Programmatic);
            App.StopWork();
            status.Log(LocalizableStrings.EXTENDED_EXECUTION_WORK_STOPPED);
        }

        #endregion


        private const int MB = 1024 * 1024;
        private List<byte[]> consumedMemory;

        private void consumeMemory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (consumedMemory == null)
                {
                    consumedMemory = new List<byte[]>();
                }
                consumedMemory.Add(new byte[156 * MB]);
                ulong usage = MemoryManager.AppMemoryUsage;
                string memoryUsage = string.Format(CultureInfo.CurrentCulture, "usage={0}MB", Math.Ceiling((double)usage / MB));
                status.Log(memoryUsage);
            }
            catch (OutOfMemoryException)
            {
                status.Log("OOM");
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
        }

    }
}
