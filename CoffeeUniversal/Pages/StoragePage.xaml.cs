using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class StoragePage : Page
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

        #endregion


        private ApplicationDataContainer localSettings;
        private const string ENTERPRISE_CONTAINER_NAME = "Managed.App.Settings";
        private const string ENTERPRISE_SETTINGS_WELCOME = "Welcome";
        private const string ENTERPRISE_SETTINGS_DESCRIPTION = "Description";

        public StoragePage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            localSettings = ApplicationData.Current.LocalSettings;
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
		}

		private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // Local storage.
            SerializableDictionary<string, string> storedData = await StorageHelper.ReadFromStorage<SerializableDictionary<string, string>>();
            if (storedData != null)
            {
				storageItem.Text = storedData.ContainsKey("StorageItem") ? storedData["StorageItem"] : string.Empty;
            }
            else
            {
				storageItem.Text = string.Empty;
            }

            if (localSettings != null)
            {
                // Local settings.
                object value;
				if (localSettings.Values.TryGetValue("SettingsItem", out value))
				{
					settingsItem.Text = value.ToString();
				}
				else
				{
					settingsItem.Text = string.Empty;
				}

                // Enterprise settings.
                ApplicationDataContainer enterpriseContainer;
                if (localSettings.Containers.TryGetValue(ENTERPRISE_CONTAINER_NAME, out enterpriseContainer))
                {
                    if (enterpriseContainer.Values.TryGetValue(ENTERPRISE_SETTINGS_WELCOME, out value))
                    {
                        enterpriseWelcome.Text = value.ToString();
                        status.Log(LocalizableStrings.ENTERPRISE_SETTINGS_WELCOME_FOUND);
                    }
                    else
                    {
                        enterpriseWelcome.Text = string.Empty;
                        status.Log(LocalizableStrings.ENTERPRISE_SETTINGS_WELCOME_NOT_FOUND);
                    }
                    if (enterpriseContainer.Values.TryGetValue(ENTERPRISE_SETTINGS_DESCRIPTION, out value))
                    {
                        enterpriseDescription.Text = value.ToString();
                        status.Log(LocalizableStrings.ENTERPRISE_SETTINGS_DESCRIPTION_FOUND);
                    }
                    else
                    {
                        enterpriseDescription.Text = string.Empty;
                        status.Log(LocalizableStrings.ENTERPRISE_SETTINGS_DESCRIPTION_NOT_FOUND);
                    }
                }
                else
                {
                    status.Log(pageTitleControl, LocalizableStrings.ENTERPRISE_SETTINGS_CONTAINER_NOT_FOUND);
                }
            }
        }

        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            SerializableDictionary<string, string> storedData = await StorageHelper.ReadFromStorage<SerializableDictionary<string, string>>();
            if (storedData == null)
            {
                storedData = new SerializableDictionary<string, string>();
            }
            storedData["StorageItem"] = storageItem.Text;
            StorageHelper.SaveToStorage(storedData);

            localSettings.Values["SettingsItem"] = settingsItem.Text;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
			storageItem.SelectAll();
            DataPackage package = new DataPackage();
            package.SetText(storageItem.SelectedText);

			try
			{
				Clipboard.SetContent(package);
				status.Log(string.Format(CultureInfo.CurrentCulture,
					LocalizableStrings.STORAGE_COPIED_TO_CLIPBOARD, storageItem.SelectedText));
			}
			catch (Exception ex)
			{
                status.Log(ex.Message);
				Debug.WriteLine("StoragePage.Copy_Click: " +ex.ToString());
            }
        }

    }
}
