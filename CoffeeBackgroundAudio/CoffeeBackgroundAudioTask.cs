using CoffeeUtilities;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;

namespace CoffeeBackgroundAudio
{
	enum ForegroundAppStatus
	{
		Active,
		Suspended,
		Unknown
	}

	public sealed class CoffeeBackgroundAudioTask : IBackgroundTask
	{

		#region fields & properties

		private SystemMediaTransportControls transportControl;

		private PlaylistManager playlistManager;
		private BackgroundTaskDeferral deferral;
		private ForegroundAppStatus foregroundAppState = ForegroundAppStatus.Unknown;
		private AutoResetEvent backgroundTaskStarted = new AutoResetEvent(false);
		private bool backgroundTaskRunning = false;

		private Playlist Playlist
		{
			get
			{
				if (null == playlistManager)
				{
					playlistManager = new PlaylistManager();
				}
				return playlistManager.Current;
			}
		}

		#endregion


		#region IBackgroundTask 

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			transportControl = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
			if (transportControl != null)
			{
				transportControl.ButtonPressed += transportControl_ButtonPressed;
				transportControl.PropertyChanged += transportControl_PropertyChanged;
				transportControl.IsEnabled = true;
				transportControl.IsPauseEnabled = true;
				transportControl.IsPlayEnabled = true;
				transportControl.IsNextEnabled = true;
				transportControl.IsPreviousEnabled = true;
			}

			taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
			taskInstance.Task.Completed += Taskcompleted;

			var value = ApplicationSettingsHelper.ReadResetSettingsValue(SharedStrings.BACKGROUND_AUDIO_APP_STATE);
			if (value == null)
			{
				foregroundAppState = ForegroundAppStatus.Unknown;
			}
			else
			{
				foregroundAppState = (ForegroundAppStatus)Enum.Parse(typeof(ForegroundAppStatus), value.ToString());
			}

			BackgroundMediaPlayer.Current.CurrentStateChanged += Current_CurrentStateChanged;
			Playlist.TrackChanged += playList_TrackChanged;
			BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;

			//Send information to foreground that background task has been started if app is active
			if (foregroundAppState != ForegroundAppStatus.Suspended)
			{
				ValueSet message = new ValueSet();
				message.Add(SharedStrings.BACKGROUND_AUDIO_STARTED, "");
				BackgroundMediaPlayer.SendMessageToForeground(message);
			}
			backgroundTaskStarted.Set();
			backgroundTaskRunning = true;

			ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_STATE, SharedStrings.BACKGROUND_AUDIO_RUNNING);
			deferral = taskInstance.GetDeferral();
		}

		private void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
		{
            if (deferral != null)
            {
                deferral.Complete();
                deferral = null;
            }
		}

		private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			try
			{
				ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_CURRENT_TRACK, Playlist.CurrentTrackName);
				ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_POSITION, BackgroundMediaPlayer.Current.Position.ToString());
				ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_STATE, SharedStrings.BACKGROUND_AUDIO_CANCELLED);
				ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_APP_STATE, Enum.GetName(typeof(ForegroundAppStatus), foregroundAppState));
				backgroundTaskRunning = false;

				if (transportControl != null)
				{
					transportControl.ButtonPressed -= transportControl_ButtonPressed;
					transportControl.PropertyChanged -= transportControl_PropertyChanged;
				}
				Playlist.TrackChanged -= playList_TrackChanged;

				playlistManager.ClearPlaylist();
				playlistManager = null;
				BackgroundMediaPlayer.Shutdown();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("CoffeeBackgroundAudioTask.OnCanceled: " +ex.ToString());
            }
            if (deferral != null)
            {
                deferral.Complete();
                deferral = null;
            }
        }
        #endregion


        #region SysteMediaTransportControls

        private void UpdateUVCOnNewTrack()
		{
			if (transportControl != null)
			{
				transportControl.PlaybackStatus = MediaPlaybackStatus.Playing;
				transportControl.DisplayUpdater.Type = MediaPlaybackType.Music;
				transportControl.DisplayUpdater.MusicProperties.Title = Playlist.CurrentTrackName;
				transportControl.DisplayUpdater.Update();
			}
		}

		private void transportControl_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
		{
			// If soundlevel turns to muted, app can choose to pause the music
		}

		private void transportControl_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
		{
			switch (args.Button)
			{
				case SystemMediaTransportControlsButton.Play:
					// If music is in paused state, for a period of more than 5 minutes, 
					//app will get task cancellation and it cannot run code. 
					//However, user can still play music by pressing play via UVC unless a new app comes in clears UVC.
					//When this happens, the task gets re-initialized and that is asynchronous and hence the wait
					if (!backgroundTaskRunning)
					{
						bool result = backgroundTaskStarted.WaitOne(2000);
						if (!result)
						{
							throw new Exception(SharedStrings.BACKGROUND_AUDIO_STARTUP_TIMEOUT);
						}
					}
					StartPlayback();
					break;
				case SystemMediaTransportControlsButton.Pause:
					try
					{
						BackgroundMediaPlayer.Current.Pause();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("CoffeeBackgroundAudioTask.transportControl_ButtonPressed: " +ex.ToString());
                    }
					break;
				case SystemMediaTransportControlsButton.Next:
					SkipToNext();
					break;
				case SystemMediaTransportControlsButton.Previous:
					SkipToPrevious();
					break;
			}
		}

		#endregion


		#region Playlist management

		private void StartPlayback()
		{
			try
			{
				if (Playlist.CurrentTrackName == string.Empty)
				{
					//If the task was cancelled we would have saved the current track and its position. We will try playback from there
					var currentTrackName = ApplicationSettingsHelper.ReadResetSettingsValue(SharedStrings.BACKGROUND_AUDIO_CURRENT_TRACK);
					var currentTrackPosition = ApplicationSettingsHelper.ReadResetSettingsValue(SharedStrings.BACKGROUND_AUDIO_POSITION);
					if (currentTrackName != null)
					{

						if (currentTrackPosition == null)
						{
							Playlist.StartTrackAt((string)currentTrackName);
						}
						else
						{
							Playlist.StartTrackAt((string)currentTrackName, TimeSpan.Parse((string)currentTrackPosition));
						}
					}
					else
					{
						Playlist.PlayAllTracks();
					}
				}
				else
				{
					BackgroundMediaPlayer.Current.Play();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("CoffeeBackgroundAudioTask.StartPlayback: " +ex.ToString());
            }
		}

		private void playList_TrackChanged(Playlist sender, object args)
		{
			UpdateUVCOnNewTrack();
			ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_CURRENT_TRACK, sender.CurrentTrackName);

			if (foregroundAppState == ForegroundAppStatus.Active)
			{
				ValueSet message = new ValueSet();
				message.Add(SharedStrings.BACKGROUND_AUDIO_TRACK_CHANGED, sender.CurrentTrackName);
				BackgroundMediaPlayer.SendMessageToForeground(message);
			}
		}

		private void SkipToPrevious()
		{
			if (transportControl != null)
			{
				transportControl.PlaybackStatus = MediaPlaybackStatus.Changing;
			}
			Playlist.SkipToPrevious();
		}

		private void SkipToNext()
		{
			if (transportControl != null)
			{
				transportControl.PlaybackStatus = MediaPlaybackStatus.Changing;
			}
			Playlist.SkipToNext();
		}

		#endregion


		#region Background Media Player Handlers

		private void Current_CurrentStateChanged(MediaPlayer sender, object args)
		{
			if (sender.CurrentState == MediaPlayerState.Playing)
			{
				if (transportControl != null)
				{
					transportControl.PlaybackStatus = MediaPlaybackStatus.Playing;
				}
			}
			else if (sender.CurrentState == MediaPlayerState.Paused)
			{
				if (transportControl != null)
				{
					transportControl.PlaybackStatus = MediaPlaybackStatus.Paused;
				}
			}
		}

		private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
		{
			foreach (string key in e.Data.Keys)
			{
				switch (key.ToLower())
				{
					case SharedStrings.BACKGROUND_AUDIO_APP_SUSPENDED:
						foregroundAppState = ForegroundAppStatus.Suspended;
						ApplicationSettingsHelper.SaveSettingsValue(SharedStrings.BACKGROUND_AUDIO_CURRENT_TRACK, Playlist.CurrentTrackName);
						break;
					case SharedStrings.BACKGROUND_AUDIO_APP_RESUMED:
						foregroundAppState = ForegroundAppStatus.Active;
						break;
					case SharedStrings.BACKGROUND_AUDIO_START_PLAYBACK:
						StartPlayback();
						break;
					case SharedStrings.BACKGROUND_AUDIO_SKIP_NEXT:
						SkipToNext();
						break;
					case SharedStrings.BACKGROUND_AUDIO_SKIP_PREVIOUS:
						SkipToPrevious();
						break;
				}
			}
		}
		#endregion

	}
}

