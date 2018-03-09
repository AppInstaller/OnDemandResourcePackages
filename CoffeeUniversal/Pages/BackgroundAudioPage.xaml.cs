using CoffeeUniversal.Helpers;
using CoffeeUniversal.ViewModels;
using CoffeeUtilities;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class BackgroundAudioPage : Page
	{

        #region Fields & properties

        private const int RPC_S_SERVER_UNAVAILABLE = 1722;

        private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		private AutoResetEvent taskInitialized;

		private bool isTaskRunning = false;
		private bool IsTaskRunning
		{
			get
			{
				if (isTaskRunning)
				{
					return true;
				}

				object value = ApplicationSettingsHelper.ReadResetSettingsValue(SharedStrings.BACKGROUND_AUDIO_STATE);
				if (value == null)
				{
                    return false;
				}
				else
				{
					isTaskRunning = ((string)value).Equals(SharedStrings.BACKGROUND_AUDIO_RUNNING);
					return isTaskRunning;
				}
			}
		}

		private string CurrentTrack
		{
			get
			{
				object value = ApplicationSettingsHelper.ReadResetSettingsValue(SharedStrings.BACKGROUND_AUDIO_CURRENT_TRACK);
				if (value != null)
				{
					return (string)value;
				}
				else
				{
					return string.Empty;
				}
			}
		}

		#endregion


		#region Init

		public BackgroundAudioPage()
		{
			InitializeComponent();
			taskInitialized = new AutoResetEvent(false);
			navigationHelper = new NavigationHelper(this);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
			App.Current.Suspending += ForegroundApp_Suspending;
			App.Current.Resuming += ForegroundApp_Resuming;
			ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_APP_STATE, SharedStrings.FOREGROUND_APP_ACTIVE);

            AddMediaPlayerEventHandlers();

            try
            {
                UpdateState(BackgroundMediaPlayer.Current.CurrentState);
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    ResetBackgroundAudio();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (isTaskRunning)
            {
                RemoveMediaPlayerEventHandlers();
                ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_STATE, SharedStrings.BACKGROUND_AUDIO_RUNNING);
            }
            navigationHelper.OnNavigatedFrom(e);
        }

        async private void ResetBackgroundAudio()
        {
            BackgroundMediaPlayer.Shutdown();
            await Task.Delay(TimeSpan.FromSeconds(2));
            StartBackgroundAudioTask();
        }

        #endregion


        #region Foreground App Lifecycle

        private void ForegroundApp_Resuming(object sender, object e)
		{
			ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_APP_STATE, SharedStrings.FOREGROUND_APP_ACTIVE);

			// Verify if the task was running before
			if (IsTaskRunning)
			{
				//if yes, reconnect to media play handlers
				AddMediaPlayerEventHandlers();

				//send message to background task that app is resumed, so it can start sending notifications
				ValueSet message = new ValueSet();
				message.Add(SharedStrings.BACKGROUND_AUDIO_APP_RESUMED, DateTime.Now.ToString(CultureInfo.CurrentCulture));

                try
                {
                    BackgroundMediaPlayer.SendMessageToBackground(message);

                    if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                    {
                        playButton.Content = "| |";
                    }
                    else
                    {
                        playButton.Content = ">";
                    }
                    txtCurrentTrack.Text = CurrentTrack;
                }
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                    if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                    {
                        ResetBackgroundAudio();
                    }
                }
            }
            else
			{
				playButton.Content = ">";
				txtCurrentTrack.Text = "";
			}
		}

		// Send message to Background process that app is to be suspended
		private void ForegroundApp_Suspending(object sender, SuspendingEventArgs e)
		{
			SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
			ValueSet message = new ValueSet();
			message.Add(SharedStrings.BACKGROUND_AUDIO_APP_SUSPENDED, DateTime.Now.ToString(CultureInfo.CurrentCulture));
			BackgroundMediaPlayer.SendMessageToBackground(message);
			RemoveMediaPlayerEventHandlers();
			ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_APP_STATE, SharedStrings.FOREGROUND_APP_SUSPENDED);
			deferral.Complete();
		}

        #endregion


        #region Background MediaPlayer Event handlers

        async private void UpdateState(MediaPlayerState currentState)
        {
            switch (currentState)
            {
                case MediaPlayerState.Playing:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        playButton.Content = "| |";
                        prevButton.IsEnabled = true;
                        nextButton.IsEnabled = true;
                    });
                    break;
                case MediaPlayerState.Paused:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        playButton.Content = ">";
                    });
                    break;
            }
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
		{
            try
            {
                UpdateState(BackgroundMediaPlayer.Current.CurrentState);
            }
            catch (Exception ex)
            {
                status.Log(ex.Message);
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    ResetBackgroundAudio();
                }
            }
        }

        async private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
		{
			foreach (string key in e.Data.Keys)
			{
				switch (key)
				{
					case SharedStrings.BACKGROUND_AUDIO_TRACK_CHANGED:
						//When foreground app is active change track based on background message
						await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
						{
							txtCurrentTrack.Text = (string)e.Data[key];
						});
						break;
					case SharedStrings.BACKGROUND_AUDIO_STARTED:
						//Wait for Background Task to be initialized before starting playback
						taskInitialized.Set();
						break;
				}
			}
		}

		#endregion


		#region UI events

		private void prevButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ValueSet message = new ValueSet();
				message.Add(SharedStrings.BACKGROUND_AUDIO_SKIP_PREVIOUS, string.Empty);
				BackgroundMediaPlayer.SendMessageToBackground(message);

				// Prevent the user from repeatedly pressing the button and causing 
				// a backlog of button presses to be handled. This button is re-enabled 
				// in the TrackReady Playstate handler.
				prevButton.IsEnabled = false;
				status.Log(LocalizableStrings.BACKGROUND_AUDIO_SKIP_BACK);
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		// If the task is already running, it will just play/pause MediaPlayer Instance
		// Otherwise, initializes MediaPlayer Handlers and starts playback
		// track or to pause if we're already playing.
		private void playButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsTaskRunning)
			{
				try
				{
					if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
					{
						BackgroundMediaPlayer.Current.Pause();
						status.Log(LocalizableStrings.BACKGROUND_AUDIO_PAUSED);
					}
					else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
					{
						BackgroundMediaPlayer.Current.Play();
						status.Log(LocalizableStrings.BACKGROUND_AUDIO_STARTED);
					}
					else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
					{
						StartBackgroundAudioTask();
					}
				}
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                    if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                    {
                        ResetBackgroundAudio();
                    }
                }
            }
            else
			{
				StartBackgroundAudioTask();
			}
		}

		private void nextButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ValueSet message = new ValueSet();
				message.Add(SharedStrings.BACKGROUND_AUDIO_SKIP_NEXT, string.Empty);
				BackgroundMediaPlayer.SendMessageToBackground(message);

				// Prevent the user from repeatedly pressing the button and causing 
				// a backlog of button presses to be handled. This button is re-enabled 
				// in the TrackReady Playstate handler.
				nextButton.IsEnabled = false;
				status.Log(LocalizableStrings.BACKGROUND_AUDIO_SKIP_FORWARD);
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		#endregion


		#region Media Playback

		private void RemoveMediaPlayerEventHandlers()
		{
			try
			{
				BackgroundMediaPlayer.Current.CurrentStateChanged -= MediaPlayer_CurrentStateChanged;
				BackgroundMediaPlayer.MessageReceivedFromBackground -= BackgroundMediaPlayer_MessageReceivedFromBackground;
			}
            catch (Exception ex)
            {
                status.Log(ex.Message);
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    ResetBackgroundAudio();
                }
            }
        }

        private void AddMediaPlayerEventHandlers()
		{
			try
			{
				BackgroundMediaPlayer.Current.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
				BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
			}
            catch (Exception ex)
            {
                status.Log(ex.Message);
                if (ex.HResult == RPC_S_SERVER_UNAVAILABLE)
                {
                    ResetBackgroundAudio();
                }
            }
        }

        private void StartBackgroundAudioTask()
		{
			AddMediaPlayerEventHandlers();
			bool success = false;
            IAsyncAction taskInitResult = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				success = taskInitialized.WaitOne(2000);
				if (success)
				{
					ValueSet message = new ValueSet();
					message.Add(SharedStrings.BACKGROUND_AUDIO_START_PLAYBACK, "0");
					BackgroundMediaPlayer.SendMessageToBackground(message);
					status.Log(LocalizableStrings.BACKGROUND_AUDIO_TASK_INVOKED);
				}
				else
				{
					status.Log(SharedStrings.BACKGROUND_AUDIO_STARTUP_TIMEOUT);
				}
			});

			if (success)
			{
				taskInitResult.Completed = new AsyncActionCompletedHandler(taskInitResult_Completed);
			}
		}

		private void taskInitResult_Completed(IAsyncAction action, AsyncStatus asyncStatus)
		{
			if (asyncStatus == AsyncStatus.Completed)
			{
				status.Log(LocalizableStrings.BACKGROUND_AUDIO_TASK_INITIALIZED);
			}
			else if (asyncStatus == AsyncStatus.Error)
			{
				status.Log(string.Format(CultureInfo.CurrentCulture,
					LocalizableStrings.BACKGROUND_AUDIO_INIT_FAIL, action.ErrorCode.ToString()));
			}
		}

		#endregion

	}
}
