using CoffeeUniversal.Helpers;
using System;
using System.Globalization;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
    public sealed partial class AppServicePage : Page
	{

        #region Standard init

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

        public AppServicePage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
        }

        #endregion


        private const string APPSERVICE_PACKAGE_FAMILY_NAME = "77D655C9-63B3-49BE-889B-03C44227D430_7bbsmwx7amam4";
        private AppServiceConnection appServiceConnection;

        async private void callAppService_Click(object sender, RoutedEventArgs e)
        {
            long number = 0;
            if (long.TryParse(input.Text, out number))
            {
                try
                {
                    callAppService.IsEnabled = false;

                    if (appServiceConnection == null)
                    {
                        appServiceConnection = new AppServiceConnection();
                        appServiceConnection.PackageFamilyName = APPSERVICE_PACKAGE_FAMILY_NAME;
                        appServiceConnection.AppServiceName = "SomeArbitraryName";
                        AppServiceConnectionStatus result = await appServiceConnection.OpenAsync();
                        if (result != AppServiceConnectionStatus.Success)
                        {
                            appServiceConnection.Dispose();
                            appServiceConnection = null;
                            status.Log(LocalizableStrings.APPSERVICE_CREATE_FAIL);
                            closeAppService.IsEnabled = false;
                            return;
                        }
                        else
                        {
                            status.Log(LocalizableStrings.APPSERVICE_CREATE_SUCCESS);
                            closeAppService.IsEnabled = true;
                        }
                    }

                    ValueSet message = new ValueSet();
                    string requestText = input.Text;
                    message.Add("SQR", requestText);
                    AppServiceResponse response = await appServiceConnection.SendMessageAsync(message);
                    if (response.Status != AppServiceResponseStatus.Success)
                    {
                        status.Log(LocalizableStrings.APPSERVICE_SEND_FAIL);
                    }
                    else
                    {
                        string responseText = response.Message["Response"] as string;
                        status.Log(string.Format(
                            CultureInfo.CurrentCulture, LocalizableStrings.APPSERVICE_SEND_RESULT, requestText, responseText));
                        result.Text = responseText;
                    }
                }
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                }
                finally
                {
                    callAppService.IsEnabled = true;
                }
            }
            else
            {
                status.Log(LocalizableStrings.APPSERVICE_INVALID_INPUT);
            }
        }

        private void closeAppService_Click(object sender, RoutedEventArgs e)
        {
            if (appServiceConnection != null)
            {
                appServiceConnection.Dispose();
                appServiceConnection = null;
                closeAppService.IsEnabled = false;
                status.Log(LocalizableStrings.APPSERVICE_CLOSED);
            }
        }

    }
}
