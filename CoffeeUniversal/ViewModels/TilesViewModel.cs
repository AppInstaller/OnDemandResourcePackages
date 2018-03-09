using CoffeeUniversal.Helpers;
using System.Collections.ObjectModel;

namespace CoffeeUniversal.ViewModels
{
	public class TilesViewModel
	{
		public ObservableCollection<TileViewModel> Items { get; private set; }

		public TilesViewModel()
		{
			Items = new ObservableCollection<TileViewModel>();
			Items.Add(new TileViewModel()
			{
				Photo = "/Assets/SecondaryTiles/espresso.jpg",
				Title = LocalizableStrings.TILE_TITLE_ESPRESSO,
				Details = LocalizableStrings.TILE_LOREM_IPSUM_BODY_1
			});
			Items.Add(new TileViewModel()
			{
				Photo = "/Assets/SecondaryTiles/latte.jpg",
				Title = LocalizableStrings.TILE_TITLE_LATTE,
				Details = LocalizableStrings.TILE_LOREM_IPSUM_BODY_2
			});
			Items.Add(new TileViewModel()
			{
				Photo = "/Assets/SecondaryTiles/cappuccino.jpg",
				Title = LocalizableStrings.TILE_TITLE_CAPPUCCINO,
				Details = LocalizableStrings.TILE_LOREM_IPSUM_BODY_3
			});
		}
	}
}
