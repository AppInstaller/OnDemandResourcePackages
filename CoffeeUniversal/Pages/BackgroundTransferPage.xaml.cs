using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class BackgroundTransferPage : Page, IDisposable
	{

		#region Init

		private List<DownloadOperation> activeDownloads;
		private CancellationTokenSource cancelToken;

		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		public BackgroundTransferPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);

			cancelToken = new CancellationTokenSource();
		}

		public void Dispose()
		{
			if (cancelToken != null)
			{
				cancelToken.Dispose();
				cancelToken = null;
			}

			GC.SuppressFinalize(this);
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
			await DiscoverActiveDownloadsAsync();
		}

		private async Task DiscoverActiveDownloadsAsync()
		{
			activeDownloads = new List<DownloadOperation>();
			IReadOnlyList<DownloadOperation> downloads = null;
			try
			{
				downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
				return;
			}

			if (downloads.Count > 0)
			{
				List<Task> tasks = new List<Task>();
				foreach (DownloadOperation download in downloads)
				{
					status.Log(string.Format(CultureInfo.CurrentCulture,
						LocalizableStrings.BACKGROUND_TRANSFER_FOUND, download.Guid, download.Progress.Status));
					tasks.Add(HandleDownloadAsync(download, false));
				}

				await Task.WhenAll(tasks);
			}
		}

		#endregion


		#region Download

		private async void start_Click(object sender, RoutedEventArgs e)
		{
			Uri source;
			if (!Uri.TryCreate(sourceUrl.Text.Trim(), UriKind.Absolute, out source))
			{
				status.Log(LocalizableStrings.BACKGROUND_TRANSFER_INVALID_URI);
				return;
			}

			string destination = localFile.Text.Trim();
			if (string.IsNullOrWhiteSpace(destination))
			{
				status.Log(LocalizableStrings.BACKGROUND_TRANSFER_INVALID_FILENAME);
				return;
			}

			StorageFile destinationFile = null;
			try
			{
				destinationFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
					destination, CreationCollisionOption.GenerateUniqueName);
			}
            catch (Exception ex)
            {
                Debug.WriteLine("BackgroundTransferPage.start_Click: " + ex.ToString());
                status.Log(ex.Message);
                return;
            }

            try
            {
                BackgroundDownloader downloader = new BackgroundDownloader();
                if (downloader != null)
                {
                    DownloadOperation download = downloader.CreateDownload(source, destinationFile);
                    download.Priority = BackgroundTransferPriority.Default;
                    await HandleDownloadAsync(download, true);
                }
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
		}

		private void DownloadProgress(DownloadOperation download)
		{
			status.Log(string.Format(
				CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_PROGRESS, 
				download.Guid, download.Progress.Status));

			double percent = 100;
			if (download.Progress.TotalBytesToReceive > 0)
			{
				percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
			}

			status.Log(string.Format(
				CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_BYTES,
				download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent));
		}

		private async Task HandleDownloadAsync(DownloadOperation download, bool start)
		{
			try
			{
				status.Log(string.Format(
					CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_RUNNING, download.Guid));
				activeDownloads.Add(download);

				Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
				if (start)
				{
					await download.StartAsync().AsTask(cancelToken.Token, progressCallback);
				}
				else
				{
					await download.AttachAsync().AsTask(cancelToken.Token, progressCallback);
				}

				ResponseInformation response = download.GetResponseInformation();
				status.Log(string.Format(
					CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_COMPLETED,
					download.Guid, response.StatusCode));
			}
			catch (TaskCanceledException)
			{
				status.Log(string.Format(
					CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_CANCELLED, download.Guid));
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
                Debug.WriteLine(ex.ToString());
			}
			finally
			{
				activeDownloads.Remove(download);
			}
		}

		#endregion


		#region Pause/Resume/Cancel

		private void pause_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                pause.IsEnabled = false;
                if (activeDownloads.Count == 0)
                {
                    status.Log(LocalizableStrings.BACKGROUND_TRANSFER_NOT_FOUND);
                }
                else
                {
                    foreach (DownloadOperation download in activeDownloads)
                    {
                        if (download.Progress.Status == BackgroundTransferStatus.Running)
                        {
                            download.Pause();
                            status.Log(string.Format(
                                CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_PAUSED, download.Guid));
                        }
                        else
                        {
                            status.Log(string.Format(
                                CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_SKIPPED,
                                download.Guid, download.Progress.Status));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
            finally
            {
                pause.IsEnabled = true;
            }
		}

		private void resume_Click(object sender, RoutedEventArgs e)
		{
            try
            {
                resume.IsEnabled = false;
                if (activeDownloads.Count == 0)
                {
                    status.Log(LocalizableStrings.BACKGROUND_TRANSFER_NOT_FOUND);
                }
                else
                {
                    foreach (DownloadOperation download in activeDownloads)
                    {
                        if (download.Progress.Status == BackgroundTransferStatus.PausedByApplication)
                        {
                            download.Resume();
                            status.Log(string.Format(
                                CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_PAUSED, download.Guid));
                        }
                        else
                        {
                            status.Log(string.Format(
                                CultureInfo.CurrentCulture, LocalizableStrings.BACKGROUND_TRANSFER_SKIPPED,
                                download.Guid, download.Progress.Status));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
            }
            finally
            {
                resume.IsEnabled = true;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
		{
			status.Log(LocalizableStrings.BACKGROUND_TRANSFER_CANCEL);

			cancelToken.Cancel();
			cancelToken.Dispose();

			cancelToken = new CancellationTokenSource();
			activeDownloads = new List<DownloadOperation>();
		}


		#endregion

	}
}
