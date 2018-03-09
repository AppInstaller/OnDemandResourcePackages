using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using CoffeeUtilities;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class TilesPage : Page
    {

		#region Init

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
            Debug.WriteLine("TilesPage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
        }

		private TileViewModel espressoModel;
		private TileViewModel latteModel;
		private TileViewModel cappuccinoModel;

		public TilesPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

			espressoModel = App.TilesViewModel.Items.First(x => x.Title == LocalizableStrings.TILE_TITLE_ESPRESSO);
			espresso.DataContext = espressoModel;

			latteModel = App.TilesViewModel.Items.First(x => x.Title == LocalizableStrings.TILE_TITLE_LATTE);
			latte.DataContext = latteModel;

			cappuccinoModel = App.TilesViewModel.Items.First(x => x.Title == LocalizableStrings.TILE_TITLE_CAPPUCCINO);
			cappuccino.DataContext = cappuccinoModel;
		}

		#endregion


		private void UpdatePrimaryTile_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				// BUG Xbox 1911584 TileUpdateManager type not registered.
				TileHelper.UpdatePrimaryTile(TileUpdateType.Foreground);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("TilesPage.UpdatePrimaryTile_Click: " +ex.ToString());
            }
		}

		private void espresso_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(SecondaryTilePage), espressoModel.Title);
		}

		private void latte_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(SecondaryTilePage), latteModel.Title);
		}

		private void cappuccino_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(SecondaryTilePage), cappuccinoModel.Title);
		}
	}
}
