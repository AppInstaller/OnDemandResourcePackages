using CoffeeUniversal.ViewModels;
using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
	public sealed partial class PublisherFolderPage : Page
	{

		#region Init

		private NavigationHelper navigationHelper;
		private const string folderName = "CoffeeFolder";
		private const string baseFileName = "TestFile";

		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		public PublisherFolderPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);

			StorageFolder sharedFolder = ApplicationData.Current.GetPublisherCacheFolder(folderName);
			PopulateFileList(sharedFolder);
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

		private async void createNewFile_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int fileCounter = 1;
				StorageFolder sharedFolder = ApplicationData.Current.GetPublisherCacheFolder(folderName);

				bool fileExists = await CheckFileExistsAsync(sharedFolder, baseFileName + fileCounter.ToString());
				while (fileExists)
				{
					fileCounter++;
					fileExists = await CheckFileExistsAsync(sharedFolder, baseFileName + fileCounter.ToString());
				}

				CreateFile(baseFileName + fileCounter.ToString());
			}
			catch (Exception ex)
			{
				status.Log(ex.ToString());
			}
		}

		private async void CreateFile(string fileName)
		{
			StorageFolder sharedFolder = ApplicationData.Current.GetPublisherCacheFolder(folderName);
			bool fileExists = await CheckFileExistsAsync(sharedFolder, fileName);
			if (fileExists)
			{
				StorageFile deleteFile = await sharedFolder.GetFileAsync(fileName);
				await deleteFile.DeleteAsync();
			}

			StorageFile file = await sharedFolder.CreateFileAsync(fileName);
			await FileIO.AppendTextAsync(file, DateTime.Now.ToString("HH:mm:ss.ffff"));
			string fileContent = await FileIO.ReadTextAsync(file);

			status.Log(string.Format(CultureInfo.CurrentCulture,
				LocalizableStrings.PUBLISHER_FOLDER_FILE_CREATED, fileName));
			status.Log(string.Format(CultureInfo.CurrentCulture,
				LocalizableStrings.PUBLISHER_FOLDER_FILE_CONTENTS, fileContent));

			PopulateFileList(sharedFolder);
		}

		public static async Task<bool> CheckFileExistsAsync(StorageFolder folder, string fileName)
		{
			try
			{
				await folder.GetFileAsync(fileName);
				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

		private async void deleteAllFiles_Click(object sender, RoutedEventArgs e)
		{
			StorageFolder sharedFolder = ApplicationData.Current.GetPublisherCacheFolder(folderName);
			await ApplicationData.Current.ClearPublisherCacheFolderAsync(folderName);
			status.Log(LocalizableStrings.PUBLISHER_FOLDER_FILES_DELETED);
			PopulateFileList(sharedFolder);
		}

		private async void PopulateFileList(StorageFolder folder)
		{
			try
			{
				files.Text = string.Empty;

				// BUG 1789765 GetFilesAsync throws when used on a shared folder.

				IReadOnlyList<StorageFile> filesInFolder = await folder.GetFilesAsync();
				StringBuilder builder = new StringBuilder();
				foreach (StorageFile file in filesInFolder)
				{
					builder.AppendLine(file.DisplayName);
				}
				files.Text = builder.ToString();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("PublisherFolderPage.PopulateFileList: " +ex.ToString());
                status.Log(ex.ToString());
			}
		}

	}
}
