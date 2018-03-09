using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class SecondaryTilePage : Page
	{

		#region Init

		private string itemTitle;
		private TileViewModel thisItem;
		private ApplicationDataContainer localSettings;
		private string isPinnedSettingName = "IsPinned";

		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
            Debug.WriteLine("SecondaryTilePage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
		}

		public SecondaryTilePage()
		{
            InitializeComponent();
			navigationHelper = new NavigationHelper(this);
			localSettings = ApplicationData.Current.LocalSettings;
			navigationHelper.LoadState += NavigationHelper_LoadState;
			navigationHelper.SaveState += NavigationHelper_SaveState;

            if (App.IsHardwareBackButtonAvailable)
            {
                backButton.Visibility = Visibility.Collapsed;
                pageTitle.Margin = new Thickness(-80, 0, 30, 40);
            }
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter != null && e.Parameter is string)
			{
				itemTitle = (string)e.Parameter;
				if (!string.IsNullOrEmpty(itemTitle))
				{
					try
					{
						TileViewModel pinnedItem = App.TilesViewModel.Items.First(x => x.Title == itemTitle);
						if (pinnedItem != null)
						{
							DataContext = thisItem = pinnedItem;
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("SecondaryTilePage.OnNavigatedTo: " +ex.ToString());
                    }
				}
			}

			navigationHelper.OnNavigatedTo(e);
		}

		#endregion

		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			object tmp = localSettings.Values[thisItem.Title + isPinnedSettingName];
			if (tmp != null)
			{
				thisItem.IsPinned = (bool)tmp;
			}
		}

		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			localSettings.Values[thisItem.Title + isPinnedSettingName] = thisItem.IsPinned;
		}

		private async void pinUnpinButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (SecondaryTile.Exists(thisItem.Title))
				{
					SecondaryTile secondaryTile = new SecondaryTile(thisItem.Title);
					FrameworkElement element = (FrameworkElement)sender;
					GeneralTransform transform = element.TransformToVisual(null);
					Point point = transform.TransformPoint(new Point());
					Rect rect = new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
					await secondaryTile.RequestDeleteForSelectionAsync(rect, Placement.Above);
					pinButton.Content = "pin";
					thisItem.IsPinned = false;
				}
				else
				{
					SecondaryTile secondaryTile = new SecondaryTile(
						thisItem.Title, thisItem.Title, thisItem.Title,
						new Uri(string.Format(CultureInfo.CurrentCulture, "ms-appx://{0}", thisItem.Photo)), TileSize.Square150x150);
					secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
					await secondaryTile.RequestCreateAsync();
					pinButton.Content = "unpin";
					thisItem.IsPinned = true;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("SecondaryTilePage.pinUnpinButton_Click: " +ex.ToString());
            }
		}
	}
}
