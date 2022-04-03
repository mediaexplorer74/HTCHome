using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPlayerWidget.Domain;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AIMP
{
    public class Controller : IController
    {
        AIMP aimp;
        DispatcherTimer timer;
        string lastPath;
        AIMP.PlayingState lastState;

        public void Initialize()
        {
            aimp = new AIMP();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            if (aimp.IsRunning)
                timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            aimp.UpdateFileInfo();
            if (aimp.CurrentMedia.sFileName != lastPath && MediaChanged != null)
            {
                lastPath = aimp.CurrentMedia.sFileName;
                MediaChanged(sender, e);
            }

            aimp.UpdateState();
            if (PlayStateChanged != null && aimp.playingState != lastState && PlayStateChanged != null)
            {
                lastState = aimp.playingState;
                PlayStateChanged(sender, e);
            }
        }

        public bool IsPlaying()
        {
            return aimp.playingState == AIMP.PlayingState.Playing;
        }

        public bool IsMediaLoaded()
        {
            return true;
        }

        public void PlayPause()
        {
            aimp.PlayPause();
        }

        public void Next()
        {
            aimp.Next();
        }

        public void Previous()
        {
            aimp.Prev();
        }

        public string GetSongTitle()
        {
            return aimp.CurrentMedia.sTitle;
        }

        public string GetSongArtist()
        {
            return aimp.CurrentMedia.sArtist;
        }

        public string GetSongAlbum()
        {
            return aimp.CurrentMedia.sAlbum;
        }

        public System.Windows.Media.ImageSource GetSongCover()
        {
            if (!string.IsNullOrEmpty(aimp.CurrentMedia.sFileName))
            {
                string file = aimp.CurrentMedia.sFileName;
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
            return TimeSpan.FromSeconds(aimp.GetPosition());
        }

        public TimeSpan GetDuration()
        {
            return TimeSpan.FromMilliseconds(aimp.CurrentMedia.nDuration);
        }

        public void SetPosition(TimeSpan position)
        {
            
        }

        public void Shutdown()
        {
           
        }

        public event EventHandler MediaChanged;

        public event EventHandler PlayStateChanged;
    }
}
