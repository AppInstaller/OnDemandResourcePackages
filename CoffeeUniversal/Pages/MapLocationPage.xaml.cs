using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class MapLocationPage : Page
    {
        // TODO This Map Service Token is for the CoffeeUniversal app in the store. Remove before releasing public documentation.
        private const string MAP_SERVICE_TOKEN = "kYb1vTNIsN0xgZqxU4Sb~DybsRATXlHTj_KuEKA_JAA~AnSIsSiv9LvN6koyKpx7N3ENBtUrHqDi1StS67RiJewjSHeORDO0j5KvRvOAilBC";
        private const string TARGET_LOCATION = "Redmond, WA";
		private Geopoint Seattle = new Geopoint(new BasicGeoposition() { Latitude = 47.608711, Longitude = -122.340567 });
        //private Geopoint Redmond = new Geopoint(new BasicGeoposition() { Latitude = 47.645392, Longitude = -122.141784 });


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
			map = null;
			navigationHelper.OnNavigatedFrom(e);
        }

        public MapLocationPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
		}

        private void mapLoaded(object sender, RoutedEventArgs e)
        {
            targetText.Text = TARGET_LOCATION;
            map.Center = Seattle;
            map.ZoomLevel = 11;
            map.MapServiceToken = MAP_SERVICE_TOKEN;

			try
			{
				MapService.ServiceToken = MAP_SERVICE_TOKEN;
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
            }
        }

        #endregion


        #region GetRoute

        private void getRoute_Click(object sender, RoutedEventArgs e)
        {
			GetRoute();
        }

		private void targetText_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter)
			{
				e.Handled = true;
				GetRoute();
			}
		}

		private async void GetRoute()
		{
			status.Log(LocalizableStrings.LOCATION_GETTING_ROUTE);

			try
			{
				Geolocator locator = new Geolocator();
				locator.DesiredAccuracy = PositionAccuracy.High;

				GeolocationAccessStatus locationAccess = await Geolocator.RequestAccessAsync();
				if (locationAccess != GeolocationAccessStatus.Allowed)
				{
					status.Log(LocalizableStrings.LOCATION_NOT_ALLOWED);
					return;
				}

				Geoposition currentPosition = await locator.GetGeopositionAsync();
				if (currentPosition == null)
				{
					status.Log(LocalizableStrings.LOCATION_CANNOT_GET_CURRENT_POSITION);
					return;
				}

				MapLocationFinderResult targetPosition = await MapLocationFinder.FindLocationsAsync(targetText.Text, null);
				if (targetPosition == null || targetPosition.Locations[0] == null)
				{
					status.Log(LocalizableStrings.LOCATION_CANNOT_GET_TARGET_POSITION);
					return;
				}

				Geopoint start = currentPosition.Coordinate.Point;
				Geopoint end = targetPosition.Locations[0].Point;
				MapRouteFinderResult route = await MapRouteFinder.GetDrivingRouteAsync(start, end);
				if (route != null)
				{
					if (route.Status != MapRouteFinderStatus.Success)
					{
						status.Log(string.Format(CultureInfo.CurrentCulture,
							LocalizableStrings.LOCATION_ROUTE_ERROR, route.Status.ToString()));
						return;
					}

					MapRouteView routeView = new MapRouteView(route.Route);
					map.Routes.Add(routeView);

					List<string> list = new List<string>();
					foreach (MapRouteLeg leg in route.Route.Legs)
					{
						for (int i = 0; i < leg.Maneuvers.Count; i++)
						{
							MapRouteManeuver maneuver = leg.Maneuvers[i];
							list.Add(string.Format("{0}. {1}", i + 1, maneuver.InstructionText));
						}
					}
					routeList.ItemsSource = list;
                    status.Log(LocalizableStrings.LOCATION_FOUND_ROUTE);
                }
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
            }
		}

        #endregion

    }
}
