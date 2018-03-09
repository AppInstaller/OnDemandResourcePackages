using CoffeeUtilities;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace CoffeeBackgroundAudio
{
	public sealed class PlaylistManager
	{
		private static Playlist instance;

		public Playlist Current
		{
			get
			{
				if (instance == null)
				{
					instance = new Playlist();
				}
				return instance;
			}
		}

		public void ClearPlaylist()
		{
			instance = null;
		}
	}

	public sealed class Playlist
	{

		#region fields & properties

		private static string[] tracks =
		{
			"ms-appx:///Assets/Media/Ring01.wma",
			"ms-appx:///Assets/Media/Ring02.wma",
			"ms-appx:///Assets/Media/Ring03.wma"
		};

		private int CurrentTrackId = -1;
		private MediaPlayer mediaPlayer;
		private TimeSpan startPosition = TimeSpan.FromSeconds(0);
		private const string BACKGROUND_AUDIO_INVALID_TRACK = "Track Id is higher than total number of tracks.";

		internal Playlist()
		{
			mediaPlayer = BackgroundMediaPlayer.Current;
			mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
			mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
			mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
			mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
		}

		public string CurrentTrackName
		{
			get
			{
				if (CurrentTrackId == -1)
				{
					return string.Empty;
				}
                if (CurrentTrackId < tracks.Length)
                {
                    string fullUrl = tracks[CurrentTrackId];
                    return fullUrl.Split('/')[fullUrl.Split('/').Length - 1];
                }
                else
                {
                    //throw new ArgumentOutOfRangeException(BACKGROUND_AUDIO_INVALID_TRACK);
                    return string.Empty;
                }
			}
		}

		public event TypedEventHandler<Playlist, object> TrackChanged;

		#endregion


		#region MediaPlayer Handlers

		private void mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
		{
			if (sender.CurrentState == MediaPlayerState.Playing && startPosition != TimeSpan.FromSeconds(0))
			{
				// if the start position is other than 0, then set it now
				sender.Position = startPosition;
				sender.Volume = 1.0;
				startPosition = TimeSpan.FromSeconds(0);
				sender.PlaybackMediaMarkers.Clear();
			}
		}

		private void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
		{
			// wait for media to be ready
			sender.Play();
			Debug.WriteLine("New Track" + CurrentTrackName);
			TrackChanged.Invoke(this, CurrentTrackName);
		}

		private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
		{
			SkipToNext();
		}

		private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
		{
			Debug.WriteLine("Failed with error code " + args.ExtendedErrorCode.ToString());
		}

		#endregion


		#region Playlist command handlers

		private void StartTrackAt(int id)
		{
			string source = tracks[id];
			CurrentTrackId = id;
			mediaPlayer.AutoPlay = false;
            MediaSource mediaSource = MediaSource.CreateFromUri(new Uri(source));
            mediaPlayer.Source = mediaSource;
		}

		public void StartTrackAt(string TrackName)
		{
			for (int i = 0; i < tracks.Length; i++)
			{
				if (tracks[i].Contains(TrackName))
				{
					string source = tracks[i];
					CurrentTrackId = i;
					mediaPlayer.AutoPlay = false;
                    MediaSource mediaSource = MediaSource.CreateFromUri(new Uri(source));
                    mediaPlayer.Source = mediaSource;
                }
            }
		}

		public void StartTrackAt(string TrackName, TimeSpan position)
		{
			for (int i = 0; i < tracks.Length; i++)
			{
				if (tracks[i].Contains(TrackName))
				{
					CurrentTrackId = i;
					break;
				}
			}

			mediaPlayer.AutoPlay = false;

			// Set the start position, we set the position once the state changes to playing, 
			// it can be possible for a fraction of second, playback can start before we are 
			// able to seek to new start position
			mediaPlayer.Volume = 0;
			startPosition = position;
            MediaSource mediaSource = MediaSource.CreateFromUri(new Uri(tracks[CurrentTrackId]));
            mediaPlayer.Source = mediaSource;
        }

        public void PlayAllTracks()
		{
			StartTrackAt(0);
		}

		public void SkipToNext()
		{
			StartTrackAt((CurrentTrackId + 1) % tracks.Length);
		}

		public void SkipToPrevious()
		{
            if (CurrentTrackId < 1)
			{
                CurrentTrackId = 0;
				StartTrackAt(CurrentTrackId);
			}
			else
			{
				StartTrackAt(CurrentTrackId - 1);
			}
		}

		#endregion

	}
}
