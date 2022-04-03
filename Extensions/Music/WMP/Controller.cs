using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wrappers.WindowsMediaPlayer;
using MediaPlayerWidget.Domain;

namespace WMP
{
    public class Controller : IController
    {
        WmpControl mediaPlayer;

        public void Initialize()
        {
            mediaPlayer = new WmpControl();
            mediaPlayer.CurrentMediaChanged += new System.Windows.RoutedPropertyChangedEventHandler<WmpMediaItem>(mediaPlayer_CurrentMediaChanged);
            mediaPlayer.PlayStateChanged += new System.Windows.RoutedEventHandler(mediaPlayer_PlayStateChanged);
        }

        void mediaPlayer_PlayStateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            PlayStateChanged(sender, e);
        }

        void mediaPlayer_CurrentMediaChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<WmpMediaItem> e)
        {
            if (MediaChanged != null)
                MediaChanged(sender, e);
        }

        public void PlayPause()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
            {
                if (mediaPlayer.IsPlaying)
                    mediaPlayer.Pause();
                else
                    mediaPlayer.Play();
            }
        }


        public bool IsPlaying()
        {
            if (mediaPlayer != null)
                return mediaPlayer.IsPlaying;
            else
                return false;
        }

        public void Next()
        {
            if (mediaPlayer != null)
                mediaPlayer.Next();
        }

        public void Previous()
        {
            if (mediaPlayer != null)
                mediaPlayer.Previous();
        }

        public string GetSongTitle()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
            {
                return mediaPlayer.CurrentMedia.Title;
            }
            else
                return "No Media";
        }

        public string GetSongArtist()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
            {
                return mediaPlayer.CurrentMedia.Artist;
            }
            else
                return "";
        }

        public string GetSongAlbum()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
            {
                return mediaPlayer.CurrentMedia.AlbumTitle;
            }
            else
                return "";
        }

        public System.Windows.Media.ImageSource GetSongCover()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
            {
                return mediaPlayer.CurrentMedia.Picture;
            }
            else
                return null;
        }


        public void Shutdown()
        {
            //mediaPlayer.ShutdownPlayerApplication();
        }

        public event EventHandler MediaChanged;


        public bool IsMediaLoaded()
        {

            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
                return true;
            else
                return false;
        }


        public TimeSpan GetPosition()
        {
            if (mediaPlayer != null)
                return mediaPlayer.Position;
            else
                return TimeSpan.Zero;
        }

        public TimeSpan GetDuration()
        {
            if (mediaPlayer != null && mediaPlayer.CurrentMedia != null)
                return mediaPlayer.CurrentMedia.Duration.TimeSpan;
            else
                return TimeSpan.Zero;
        }


        public void SetPosition(TimeSpan position)
        {
            mediaPlayer.Position = position;
        }


        public event EventHandler PlayStateChanged;
    }
}
