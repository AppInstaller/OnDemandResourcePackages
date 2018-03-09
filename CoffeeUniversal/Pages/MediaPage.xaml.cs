using CoffeeUniversal.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;
using System;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using System.Globalization;

namespace CoffeeUniversal.Pages
{
	public sealed partial class MediaPage : Page
	{

		#region Init

		private NavigationHelper navigationHelper;

		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                string videoName = e.Parameter as string;
                if (!string.IsNullOrEmpty(videoName))
                {
                    switch (videoName)
                    {
                        case "espresso":
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
                                    new Uri("ms-appx:///Assets/Media/making-espresso_1280x720.wmv"));
                                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                                localVideo.MediaOpened += localVideo_MediaOpened;
                                localVideo.SetSource(stream, file.ContentType);
                            });
                            break;
                        case "windows":
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                Uri videoUri = new Uri("http://video.ch9.ms/ch9/4597/8db5a656-b173-4897-b2aa-e2075fb24597/windows10recap.mp4");
                                remoteVideo.MediaOpened += remoteVideo_MediaOpened;
                                remoteVideo.Source = videoUri;
                            });
                            break;
                        default:
                            status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.MEDIA_VIDEO_NOT_FOUND, videoName));
                            break;
                    }
                }
            }
        }

        private void localVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayLocalVideo();
        }

        private void remoteVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayRemoteVideo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		public MediaPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);
		}

		#endregion


        private void localPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayLocalVideo();
        }

        private void PlayLocalVideo()
        {
            localVideo.Play();
            localPause.IsEnabled = true;
            localPlay.IsEnabled = false;
            remotePlay.IsEnabled = false;
            remotePause.IsEnabled = false;
            status.Log(localPause, LocalizableStrings.MEDIA_LOCAL_PLAY_STARTED);
        }

        private void localPause_Click(object sender, RoutedEventArgs e)
        {
            localVideo.Pause();
            localPause.IsEnabled = false;
            localPlay.IsEnabled = true;
            remotePlay.IsEnabled = true;
            remotePause.IsEnabled = false;
            status.Log(localPlay, LocalizableStrings.MEDIA_LOCAL_PLAY_PAUSED);
        }

        private void remotePlay_Click(object sender, RoutedEventArgs e)
        {
            PlayRemoteVideo();
        }

        private void PlayRemoteVideo()
        {
            remoteVideo.Play();
            remotePause.IsEnabled = true;
            remotePlay.IsEnabled = false;
            localPause.IsEnabled = false;
            localPlay.IsEnabled = false;
            status.Log(remotePause, LocalizableStrings.MEDIA_REMOTE_PLAY_STARTED);
        }

        private void remotePause_Click(object sender, RoutedEventArgs e)
        {
            remoteVideo.Pause();
            remotePause.IsEnabled = false;
            remotePlay.IsEnabled = true;
            localPause.IsEnabled = false;
            localPlay.IsEnabled = true;
            status.Log(remotePlay, LocalizableStrings.MEDIA_REMOTE_PLAY_PAUSED);
        }

    }
}
