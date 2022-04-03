using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPlayerWidget.Domain;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Winamp
{
    public class Controller : IController
    {
        WACC.clsWACC winamp;
        bool isBinded;
        DispatcherTimer timer;
        string lastPath;
        WACC.clsWACC.cPlayback.Playback_State lastState;

        public void Initialize()
        {
            winamp = new WACC.clsWACC();
            isBinded = winamp.Bind();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            if (isBinded)
                timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (winamp.Playlist.GetItemPath(winamp.Playlist.Position) != lastPath && MediaChanged != null)
            {
                lastPath = winamp.Playlist.GetItemPath(winamp.Playlist.Position);
                MediaChanged(sender, e);
            }

            if (PlayStateChanged != null && winamp.Playback.PlaybackState != lastState && PlayStateChanged != null)
            {
                lastState = winamp.Playback.PlaybackState;
                PlayStateChanged(sender, e);
            }
        }

        public bool IsPlaying()
        {
            if (isBinded)
                return winamp.Playback.PlaybackState == WACC.clsWACC.cPlayback.Playback_State.Playing;
            else
                return false;
        }

        public bool IsMediaLoaded()
        {
            if (isBinded)
                return winamp.Playlist.Length > 0;
            else
                return false;
        }

        public void PlayPause()
        {
            if (isBinded)
                winamp.Playback.PauseUnpause();
        }

        public void Next()
        {
            if (isBinded)
                winamp.Playlist.JumpToNextTrack();
        }

        public void Previous()
        {
            if (isBinded)
                winamp.Playlist.JumpToPreviousTrack();
        }

        public string GetSongTitle()
        {
            if (isBinded)
                return winamp.Playlist.GetMetaData(winamp.Playlist.Position, "Title");
            else
                return "No Media";
        }

        public string GetSongArtist()
        {
            if (isBinded)
                return winamp.Playlist.GetMetaData(winamp.Playlist.Position, "Artist");
            else
                return "";
        }

        public string GetSongAlbum()
        {
            if (isBinded)
                return winamp.Playlist.GetMetaData(winamp.Playlist.Position, "Album");
            else
                return "";
        }

        public System.Windows.Media.ImageSource GetSongCover()
        {
            if (isBinded)
            {
                string file = winamp.Playlist.GetItemPath(winamp.Playlist.Position);
                // look for album art
                string[] artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "AlbumArt_*Large.jpg");
                if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "Folder.jpg");
                if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "cover.jpg");
                if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "AlbumArt*.jpg");
                if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "Album*.jpg");
                if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(file), "*.jpg");

                if (artPaths.Length > 0)
                {
                    return new BitmapImage(new Uri(artPaths[0]));
                }
            }
            return null;
        }

        public TimeSpan GetPosition()
        {
            if (isBinded)
               return TimeSpan.FromMilliseconds(winamp.Playback.TrackPosition);
            else
                return TimeSpan.Zero;
        }

        public TimeSpan GetDuration()
        {
            if (isBinded)
                return TimeSpan.FromSeconds(winamp.Playback.GetTrackLength());
            else
                return TimeSpan.Zero;
        }

        public void SetPosition(TimeSpan position)
        {
            winamp.Playback.TrackPosition = position.Milliseconds;
        }

        public void Shutdown()
        {
            //winamp.CloseWinamp();
        }

        public event EventHandler MediaChanged;


        public event EventHandler PlayStateChanged;
    }
}
