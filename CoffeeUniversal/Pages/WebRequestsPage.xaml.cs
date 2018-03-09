using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class WebRequestsPage : Page
    {

		#region Init

		private const int MAX_IMAGE_COUNT = 32;
		private ObservableCollection<ImageSource> coffeeImages;

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
			// Forcibly clean up the image memory.
			coffeeImages.Clear();
			GC.Collect();
            navigationHelper.OnNavigatedFrom(e);
        }

        public WebRequestsPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);

            coffeeImages = new ObservableCollection<ImageSource>();
            imageList.ItemsSource = coffeeImages;
		}

		#endregion

		private void getImages_Click(object sender, RoutedEventArgs e)
        {
			try
			{
				GetImages();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("WebRequestsPage.getImages_Click: " +ex.ToString());
                status.Log(ex.Message);
            }
        }

		private async void GetImages()
		{
			coffeeImages.Clear();
			status.Log(LocalizableStrings.WEB_REQUEST_INIT);

			HttpClient httpClient = new HttpClient();
			string targetUri = "http://www.bing.com/images/search?q=italian+espresso";
			string response = await httpClient.GetStringAsync(targetUri);

			if (response != null)
			{
				// Extract each substring that starts with "http://" and ends with ".jpg".
				// and doesn't include a nested "http://" or "file:" or angle brackets.
				Regex regex = new Regex(@"http://(?!ts)(.+?)\.jpg");
				MatchCollection matches = regex.Matches(response);
				List<string> urls = new List<string>();

                foreach (Match match in matches)
				{
					string tmp = match.Value;
                    if (tmp.Contains("file:") || tmp.Contains("File:"))
                    {
                        continue;
                    }
                    if (tmp.Contains("<") || tmp.Contains(">"))
                    {
                        continue;
                    }
                    int lastHttp = tmp.LastIndexOf("http://");
					if (lastHttp > 0)
					{
						tmp = tmp.Substring(lastHttp);
					}
					if (!urls.Contains(tmp))
					{
						urls.Add(tmp);
					}
				}

				status.Log(LocalizableStrings.WEB_REQUEST_DATA);

				// Limit the number of images to fetch, so we don't hit memory problems on low-cost devices.
				for (int i = 0; i <= MAX_IMAGE_COUNT && i < urls.Count; i++)
				{
					string url = urls[i];
					try
					{
						Stream image = await httpClient.GetStreamAsync(url);

						if (image != null)
						{
							await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
							{
                                try
                                {
                                    MemoryStream memStream = new MemoryStream();
                                    await image.CopyToAsync(memStream);
                                    memStream.Position = 0;
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.DecodePixelHeight = 180;
                                    bitmap.DecodePixelWidth = 240;

                                    await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());

                                    // BUG 1995085 The version of the System.Runtime.WindowsRuntime used in the build tree doesn't include the
                                    // WindowsRuntimeStreamExtensions, where AsRandomAccessStream is defined.

                                    // UNDONE
                                    //InMemoryRandomAccessStream randomStream = new InMemoryRandomAccessStream();
                                    //memStream.CopyTo(randomStream.AsStream());
                                    //randomStream.Seek(0);
                                    //await bitmap.SetSourceAsync(randomStream);

                                    coffeeImages.Add(bitmap);
                                }
                                catch (Exception ex)
                                {
						            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "WebRequestsPage.GetImages ({0}) - {1}", url, ex));
                                }
							});
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine("WebRequestsPage.GetImages " +ex.ToString());
                    }
				}
			}
		}
	}
}
