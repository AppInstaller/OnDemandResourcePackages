using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// TODO CameraPage: allow for orientation changes when determining preview and/or capture image rotation.

namespace CoffeeUniversal.Pages
{
    public sealed partial class CameraPage : Page
    {

        #region Init 

        private MediaCapture mediaCapture;
        private StorageFile photoFile;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        private NavigationHelper navigationHelper;
		private bool isPreviewing;
        private readonly DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private bool isRearCamera = true;

        public CameraPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
		}

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
			init.IsEnabled = true;
            preview.IsEnabled = false;
            capture.IsEnabled = false;
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (mediaCapture != null)
            {
				if (isPreviewing)
				{
					await mediaCapture.StopPreviewAsync();
					isPreviewing = false;
				}
				mediaCapture.Dispose();
                mediaCapture = null;
            }

            // Ensure we don't leak the Bitmap memory.
            captureImage.Source = null;
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private async void init_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                init.IsEnabled = false;
				mediaCapture = new MediaCapture();

                DeviceInformation cameraID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                    .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);
                if (cameraID != null)
                {
                    isRearCamera = true;
                }
                else
                {                     
                    isRearCamera = false;
                    cameraID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).FirstOrDefault();
                }
                if (cameraID == null)
                {
                    status.Log(LocalizableStrings.CAMERA_NOT_FOUND);
                    return;
                }

                await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    PhotoCaptureSource = PhotoCaptureSource.Photo,
                    AudioDeviceId = string.Empty,
                    VideoDeviceId = cameraID.Id
                });

				if (mediaCapture.MediaCaptureSettings.VideoDeviceId != string.Empty)
                {
                    preview.IsEnabled = true;
                    capture.IsEnabled = true;
                    mediaCapture.Failed += new MediaCaptureFailedEventHandler(mediaCapture_Failed);

					previewTransform.CenterX = previewElement.ActualWidth / 2;
					previewTransform.CenterY = previewElement.ActualHeight / 2;
                    captureTransform.CenterX = captureImage.ActualWidth / 2;
					captureTransform.CenterY = captureImage.ActualHeight / 2;
					captureTransform.ScaleX = captureImage.ActualHeight / captureImage.ActualWidth;
					captureTransform.ScaleY = 16.0 / 9.0; 

					status.Log(preview, LocalizableStrings.CAMERA_INITIALIZATION_SUCCESS);
				}
				else
                {
					status.Log(LocalizableStrings.CAMERA_INITIALIZATION_FAIL);
                    init.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                // HACK There's a bug in the camera exception where the message string is repeated.
                string message = ex.Message;
                string[] messageText = message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (messageText != null && messageText.Count() > 0 && string.Compare(messageText[0], messageText[1]) == 0)
                {
                    message = messageText[0];
                }
                status.Log(message);
                init.IsEnabled = true;
            }
        }

		private async void preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                previewElement.Source = mediaCapture;

                if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
                {
                    mediaCapture.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                }
                else if (!isRearCamera)
                {
                    mediaCapture.SetPreviewRotation(VideoRotation.Clockwise180Degrees);
                }

                await mediaCapture.StartPreviewAsync();
				isPreviewing = true;
                preview.IsEnabled = false;
				status.Log(LocalizableStrings.CAMERA_PREVIEW_SUCCESS);
            }
            catch (Exception ex)
            {
                previewElement.Source = null;
                preview.IsEnabled = true;
                status.Log(ex.Message);
            }
        }

		private async void capture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                capture.IsEnabled = false;

                if (displayInformation.NativeOrientation == DisplayOrientations.Portrait)
                {
                    captureTransform.Rotation = 90.0;
                }

                photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
					PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName);
				ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
				await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
                capture.IsEnabled = true;
				status.Log(capture, LocalizableStrings.CAMERA_CAPTURE_SUCCESS);

				IRandomAccessStream photoStream = await photoFile.OpenAsync(FileAccessMode.Read);
				BitmapImage bitmap = new BitmapImage();
				bitmap.SetSource(photoStream);
				captureImage.Source = bitmap;
			}
			catch (Exception ex)
            {
                status.Log(ex.Message);
            }
            finally
            {
                capture.IsEnabled = true;
            }
        }

		private void mediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            status.Log(currentFailure.Message);
        }
    }
}
