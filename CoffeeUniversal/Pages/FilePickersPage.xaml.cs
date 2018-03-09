using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using CoffeeUniversal.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
	public sealed partial class FilePickersPage : Page
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

		private ObservableCollection<ImageSource> images;

		public FilePickersPage()
		{
			images = new ObservableCollection<ImageSource>();
			InitializeComponent();
			imageList.ItemsSource = images;
			navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
            Debug.WriteLine("FilePickersPage_OnNavigatedFrom");
            images.Clear();
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		async private void pickMultipleFiles_Click(object sender, RoutedEventArgs e)
		{
			images.Clear();

			try
			{
				FileOpenPicker openPicker = new FileOpenPicker();
				openPicker.ViewMode = PickerViewMode.Thumbnail;
				openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
				openPicker.FileTypeFilter.Add(".jpg");
				openPicker.FileTypeFilter.Add(".jpeg");
				openPicker.FileTypeFilter.Add(".png");
				IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
				if (files.Count > 0)
				{
					foreach (StorageFile file in files)
					{
						status.Log(string.Format(CultureInfo.CurrentCulture,
							LocalizableStrings.FILE_PICKER_SELECTED_FILE, file.Name));

						IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
						BitmapImage bitmap = new BitmapImage();
						bitmap.DecodePixelHeight = 100;
						bitmap.DecodePixelWidth = 120;
						await bitmap.SetSourceAsync(stream);
						images.Add(bitmap);
					}
				}
				else
				{
					status.Log(string.Format(CultureInfo.CurrentCulture,
						LocalizableStrings.FILE_PICKER_NO_SELECTION));
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("FilePickersPage.pickMultipleFiles_Click: " +ex.ToString());
                status.Log(ex.Message);
			}
		}

		async private void pickSingleFile_Click(object sender, RoutedEventArgs e)
		{
			fileText.Text = string.Empty;

			try
			{
				FileOpenPicker openPicker = new FileOpenPicker();
				openPicker.ViewMode = PickerViewMode.List;
				openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				openPicker.FileTypeFilter.Add(".txt");
				StorageFile file = await openPicker.PickSingleFileAsync();
				if (file != null)
				{
					status.Log(string.Format(CultureInfo.CurrentCulture,
						LocalizableStrings.FILE_PICKER_SELECTED_FILE, file.Name));
					fileText.Text = await FileIO.ReadTextAsync(file);
				}
				else
				{
					status.Log(string.Format(CultureInfo.CurrentCulture,
						LocalizableStrings.FILE_PICKER_NO_SELECTION));
				}
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		async private void saveFile_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				FileSavePicker savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
				savePicker.SuggestedFileName = "New Document";
				StorageFile file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					CachedFileManager.DeferUpdates(file);
					await FileIO.WriteTextAsync(file, fileText.Text);
					FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);

					if (updateStatus == FileUpdateStatus.Complete)
					{
						status.Log(string.Format(CultureInfo.CurrentCulture,
							LocalizableStrings.FILE_PICKER_SAVE_SUCCESS, file.Name));
					}
					else
					{
						status.Log(string.Format(CultureInfo.CurrentCulture,
							LocalizableStrings.FILE_PICKER_SAVE_FAIL, file.Name));
					}
				}
				else
				{
					status.Log(string.Format(CultureInfo.CurrentCulture,
						LocalizableStrings.FILE_PICKER_NO_SELECTION));
				}
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}
	}
}
