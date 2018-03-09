using CoffeeDirectX;
using CoffeeUniversal.Pages;
using System.Collections.Generic;
using System.ComponentModel;

namespace CoffeeUniversal.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
	{

		#region Fields & Properties

		private bool isDataLoaded;
		public bool IsDataLoaded
		{
			get { return isDataLoaded; }
		}

		private List<MenuItem> menuItems = new List<MenuItem>();
		public List<MenuItem> MenuItems
		{
			get { return menuItems; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion


		#region Init

		internal void LoadData()
		{
			if (isDataLoaded)
			{
				return;
			}

			menuItems.Add(new MenuItem { Name = "appBar", Content = LocalizableStrings.MAINMENU_APPBAR, PageType = typeof(AppBarPage) });
            menuItems.Add(new MenuItem { Name = "appService", Content = LocalizableStrings.MAINMENU_APPSERVICE, PageType = typeof(AppServicePage) });
            menuItems.Add(new MenuItem { Name = "backgroundAudio", Content = LocalizableStrings.MAINMENU_BACKGROUND_AUDIO, PageType = typeof(BackgroundAudioPage) });
			menuItems.Add(new MenuItem { Name = "backgroundTask", Content = LocalizableStrings.MAINMENU_BACKGROUND_TASKS, PageType = typeof(BackgroundTaskPage) });
			menuItems.Add(new MenuItem { Name = "backgroundTransfer", Content = LocalizableStrings.MAINMENU_BACKGROUND_TRANSFER, PageType = typeof(BackgroundTransferPage) });
            menuItems.Add(new MenuItem { Name = "calendar", Content = LocalizableStrings.MAINMENU_CALENDAR, PageType = typeof(CalendarPage) });
            menuItems.Add(new MenuItem { Name = "camera", Content = LocalizableStrings.MAINMENU_CAMERA, PageType = typeof(CameraPage) });
            menuItems.Add(new MenuItem { Name = "contacts", Content = LocalizableStrings.MAINMENU_CONTACTS, PageType = typeof(ContactsPage) });
            menuItems.Add(new MenuItem { Name = "crash", Content = LocalizableStrings.MAINMENU_CRASH, PageType = typeof(CrashPage) });
            menuItems.Add(new MenuItem { Name = "directx", Content = LocalizableStrings.MAINMENU_DIRECTX, PageType = typeof(DirectXPage) });
            menuItems.Add(new MenuItem { Name = "displayRequests", Content = LocalizableStrings.MAINMENU_DISPLAY_REQUESTS, PageType = typeof(DisplayRequestPage) });
            menuItems.Add(new MenuItem { Name = "energy", Content = LocalizableStrings.MAINMENU_ENERGY, PageType = typeof(EnergyPage) });
            menuItems.Add(new MenuItem { Name = "extendedEx", Content = LocalizableStrings.MAINMENU_EXTENDED_EXECUTION, PageType = typeof(ExtendedExecutionPage) });
            menuItems.Add(new MenuItem { Name = "Extensions", Content = LocalizableStrings.MAINMENU_EXTENSIONS, PageType = typeof(ExtensionsPage) });
            menuItems.Add(new MenuItem { Name = "ink", Content = LocalizableStrings.MAINMENU_INK, PageType = typeof(InkPage) });
            menuItems.Add(new MenuItem { Name = "launchers", Content = LocalizableStrings.MAINMENU_LAUNCHERS, PageType = typeof(LaunchersPage) });
			menuItems.Add(new MenuItem { Name = "location", Content = LocalizableStrings.MAINMENU_LOCATION, PageType = typeof(MapLocationPage) });
            menuItems.Add(new MenuItem { Name = "mandatoryUpdates", Content = LocalizableStrings.MAINMENU_MANDATORY_UPDATES, PageType = typeof(MandatoryUpdatesPage) }); // Jasosal
            menuItems.Add(new MenuItem { Name = "media", Content = LocalizableStrings.MAINMENU_MEDIA, PageType = typeof(MediaPage) });
            menuItems.Add(new MenuItem { Name = "memory", Content = LocalizableStrings.MAINMENU_MEMORY, PageType = typeof(MemoryPage) });
            menuItems.Add(new MenuItem { Name = "multiview", Content = LocalizableStrings.MAINMENU_MULTIVIEW, PageType = typeof(MultiViewPage) });
            menuItems.Add(new MenuItem { Name = "optionalPackage", Content = LocalizableStrings.MAINMENU_OPTIONAL_PACKAGES, PageType = typeof(OptionalPackagesPage) }); //Jasosal
            menuItems.Add(new MenuItem { Name = "orientation", Content = LocalizableStrings.MAINMENU_ORIENTATION, PageType = typeof(OrientationPage) });
            menuItems.Add(new MenuItem { Name = "pickers", Content = LocalizableStrings.MAINMENU_FILE_PICKERS, PageType = typeof(FilePickersPage) });
            menuItems.Add(new MenuItem { Name = "publisherFolder", Content = LocalizableStrings.MAINMENU_PUBLISHER_FOLDER, PageType = typeof(PublisherFolderPage) });
            menuItems.Add(new MenuItem { Name = "resources", Content = LocalizableStrings.MAINMENU_RESOURCES, PageType = typeof(ResourcesPage) });
            menuItems.Add(new MenuItem { Name = "sensors", Content = LocalizableStrings.MAINMENU_SENSORS, PageType = typeof(SensorsPage) });
            menuItems.Add(new MenuItem { Name = "settings", Content = LocalizableStrings.MAINMENU_STORAGE, PageType = typeof(StoragePage) });
            menuItems.Add(new MenuItem { Name = "share", Content = LocalizableStrings.MAINMENU_SHARE, PageType = typeof(SharePage) });
            menuItems.Add(new MenuItem { Name = "speech", Content = LocalizableStrings.MAINMENU_SPEECH, PageType = typeof(SpeechPage) });
			menuItems.Add(new MenuItem { Name = "tiles", Content = LocalizableStrings.MAINMENU_TILES, PageType = typeof(TilesPage) });
			menuItems.Add(new MenuItem { Name = "toasts", Content = LocalizableStrings.MAINMENU_TOASTS, PageType = typeof(ToastsPage) });
			menuItems.Add(new MenuItem { Name = "webRequests", Content = LocalizableStrings.MAINMENU_WEB_REQUESTS, PageType = typeof(WebRequestsPage) });
            menuItems.Add(new MenuItem { Name = "webview", Content = LocalizableStrings.MAINMENU_WEBVIEW, PageType = typeof(WebViewPage) });
            menuItems.Add(new MenuItem { Name = "winRT", Content = LocalizableStrings.MAINMENU_WINRT, PageType = typeof(WinRTPage) });

            isDataLoaded = true;
		}

		#endregion

	}
}
