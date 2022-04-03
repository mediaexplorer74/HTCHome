using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MediaPlayerWidget.Domain
{
    public interface IController
    {
        void Initialize();
        bool IsPlaying();
        bool IsMediaLoaded();
        void PlayPause();
        void Next();
        void Previous();
        string GetSongTitle();
        string GetSongArtist();
        string GetSongAlbum();
        ImageSource GetSongCover();
        TimeSpan GetPosition();
        TimeSpan GetDuration();
        void SetPosition(TimeSpan position);
        void Shutdown();

        event EventHandler MediaChanged;
        event EventHandler PlayStateChanged;
    }
}
