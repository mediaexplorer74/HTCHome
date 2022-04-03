using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using E = HTCHome.Core.Environment;
using System.Windows.Threading;
using System.IO;
using HTCHome.Core;
using System.Windows.Media.Animation;
using MediaPlayerWidget.Domain;

namespace MediaPlayerWidget
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        public IController controller;
        DispatcherTimer timer;
        private Options options;
        public List<IController> controllers;

        public MediaPlayer()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            if (!Directory.Exists(E.Path + "\\Music\\Skins\\" + Properties.Settings.Default.Skin))
                Properties.Settings.Default.Skin = "Sense";

            //Body.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("body.png")));

            //PrevBg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_btn_left.png")));
            //PlayBg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_btn_center.png")));
            //NextBg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_btn_right.png")));

            //PrevIcon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_icon_prev.png")));
            //PlayIcon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_icon_play.png")));
            //NextIcon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_icon_next.png")));

            //AlbumArtCover.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("album_art_cover.png")));
            //AlbumArt.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("NoartCover.png")));
            //AlbumArtShadow.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("music_album_shadow.png")));

            Skin.Source = new Uri(E.Path + "\\Music\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();

            options = new Options(this);

            MenuItem optionsItem = new MenuItem();
            optionsItem.Header = Widget.LocaleManager.GetString("Options");
            optionsItem.Click += optionsItem_Click;

            Widget.Parent.ContextMenu.Items.Insert(0, new Separator());
            Widget.Parent.ContextMenu.Items.Insert(0, optionsItem);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);

            controller = Controller.Load(E.ExtensionsPath + "\\Music\\" + Properties.Settings.Default.Controller + "\\" + Properties.Settings.Default.Controller + ".dll"); //new Wrappers.WindowsMediaPlayer.WmpControl();
            //mediaPlayer.CurrentMediaChanged += new RoutedPropertyChangedEventHandler<Wrappers.WindowsMediaPlayer.WmpMediaItem>(mediaPlayer_CurrentMediaChanged);
            //mediaPlayer.PlayStateChanged += new RoutedEventHandler(mediaPlayer_PlayStateChanged);

            InitController();
        }

        public void InitController()
        {
            controller.Initialize();
            controller.MediaChanged += controller_MediaChanged;
            controller.PlayStateChanged += controller_PlayStateChanged;

            if (controller.IsMediaLoaded())
            {
                SongTitle.Text = controller.GetSongTitle();
                SongTitle.ToolTip = SongTitle.Text;
                SongArtistTitle.Text = controller.GetSongArtist();
                SongAlbum.Text = controller.GetSongAlbum();
                AlbumArt.Source = controller.GetSongCover();
                if (AlbumArt.Source == null)
                    AlbumArt.Source = new BitmapImage(new Uri(E.Path + "\\Music\\Resources\\NoartCover.png"));

                SongTime.Text = "0:00";
                SongTimeLeft.Text = "-" + controller.GetDuration().ToString(@"%m\:ss");
                Progress.Maximum = controller.GetDuration().TotalMilliseconds;
                Progress.Value = controller.GetPosition().TotalMilliseconds;

                //timer.Start();
                //SongTime.Text = controller.GetPosition().ToString(@"%m\:ss");
                //SongTimeLeft.Text = "-" + (controller.GetDuration() - controller.GetPosition()).ToString(@"%m\:ss");
                //PlayButton.IsChecked = true;
            }

            if (controller.IsPlaying())
            {
                timer.Start();
                SongTime.Text = controller.GetPosition().ToString(@"%m\:ss");
                SongTimeLeft.Text = "-" + (controller.GetDuration() - controller.GetPosition()).ToString(@"%m\:ss");
                PlayButton.IsChecked = true;
            }
        }

        private void GetControllers()
        {
             //if (Directory.Exists(E.Path + "\\WeatherClock\\WeatherProviders"))
             //{
             //    controllers = new List<IController>();
             //    var files = from x in Directory.GetFiles(E.Path + "\\Music\\Controllers")
             //                where x.EndsWith(".dll")
             //                select x;
             //    foreach (var f in files)
             //    {
             //        var p = new IController();
             //        providers.Add(p);
             //        if (Widget.Sett.weatherProvider == p.Name)
             //        {
             //            currentProvider = p;
             //            p.Load();
             //        }
             //    }
             //}
        }

        void controller_PlayStateChanged(object sender, EventArgs e)
        {
            if (controller.IsPlaying())
            {
                if (!timer.IsEnabled)
                    timer.IsEnabled = true;
            }
            else
            {
                timer.Stop();
            }

        }

        void controller_MediaChanged(object sender, EventArgs e)
        {
            if (controller.IsMediaLoaded())
            {

                SongTitle.Text = controller.GetSongTitle();
                SongTitle.ToolTip = SongTitle.Text;
                SongArtistTitle.Text = controller.GetSongArtist();
                SongAlbum.Text = controller.GetSongAlbum();
                AlbumArt.Source = controller.GetSongCover();
                if (AlbumArt.Source == null)
                    AlbumArt.Source = new BitmapImage(new Uri(E.Path + "\\Music\\Resources\\NoartCover.png"));
                SongTime.Text = "0:00";
                SongTimeLeft.Text = "-" + controller.GetDuration().ToString(@"%m\:ss");
                Progress.Maximum = controller.GetDuration().TotalMilliseconds;
                Progress.Value = controller.GetPosition().TotalMilliseconds;
            }
        }

        void optionsItem_Click(object sender, RoutedEventArgs e)
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options(this);

            if (E.Locale == "he-IL" || E.Locale == "ar-SA")
            {
                options.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                options.FlowDirection = FlowDirection.LeftToRight;
            }

            options.ShowDialog();
        }

        void mediaPlayer_PlayStateChanged(object sender, RoutedEventArgs e)
        {

            /*if (mediaPlayer.IsPlaying)
            {
                if (!timer.IsEnabled)
                    timer.IsEnabled = true;
            }
            else
            {
                timer.Stop();
                if (!mediaPlayer.IsPaused && mediaPlayer.CurrentMedia != null)
                {
                    SongTime.Text = "0:00";
                    SongTimeLeft.Text = "-" + mediaPlayer.CurrentMedia.Duration.TimeSpan.ToString(@"%m\:ss");
                    Progress.Value = 0;
                }
            }*/
        }

        public void UpdateSettings()
        {
            Scale.ScaleX = Properties.Settings.Default.ScaleFactor;
            Scale.ScaleY = Scale.ScaleX;

            /*if (E.Locale != Widget.LocaleManager.LocaleCode)
                Widget.LocaleManager.LoadLocale(E.Locale);*/

            if (System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(Skin.Source.AbsolutePath)) != Properties.Settings.Default.Skin)
            {
                ReloadSkin();
            }
        }
        public void ReloadSkin()
        {
            Skin.Source = new Uri(E.Path + "\\Music\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");
            Widget.ResourceManager = new ResourceManager(E.Path + "\\Music", Properties.Settings.Default.Skin);

            Widget.Instance.UpdateAero(this);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (controller.IsPlaying())
            {
                TimeSpan position = controller.GetPosition();
                TimeSpan duration = controller.GetDuration();
                SongTime.Text = position.ToString(@"%m\:ss");
                SongTimeLeft.Text = "-" + (duration - position).ToString(@"%m\:ss");
                Progress.Value = position.TotalMilliseconds;
                /*SongTime.Text = mediaPlayer.Position.ToString(@"%m\:ss");
                SongTimeLeft.Text = "-" + (mediaPlayer.CurrentMedia.Duration.TimeSpan - mediaPlayer.Position).ToString(@"%m\:ss");
                Progress.Value = mediaPlayer.Position.TotalMilliseconds;*/
            }
        }

        /*void mediaPlayer_CurrentMediaChanged(object sender, RoutedPropertyChangedEventArgs<Wrappers.WindowsMediaPlayer.WmpMediaItem> e)
        {
            if (mediaPlayer.CurrentMedia != null)
            {
                SongTitle.Text = mediaPlayer.CurrentMedia.Title;
                SongTitle.ToolTip = mediaPlayer.CurrentMedia.Title;
                SongArtistTitle.Text = mediaPlayer.CurrentMedia.Artist;
                SongAlbum.Text = mediaPlayer.CurrentMedia.AlbumTitle;
                if (mediaPlayer.CurrentMedia.Picture != null)
                    AlbumArt.Source = mediaPlayer.CurrentMedia.Picture;
                else
                    AlbumArt.Source = new BitmapImage(new Uri(E.Path + "\\Music\\Resources\\NoartCover.png"));
                SongTime.Text = "0:00";
                SongTimeLeft.Text = "-" + mediaPlayer.CurrentMedia.Duration.TimeSpan.ToString(@"%m\:ss");
                Progress.Maximum = mediaPlayer.CurrentMedia.Duration.TimeSpan.TotalMilliseconds;
                Progress.Value = mediaPlayer.Position.TotalMilliseconds;
            }
        }*/

        private void Progress_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Progress.Value = 1 / (Progress.ActualWidth / e.GetPosition(Progress).X) * Progress.Maximum;
            controller.SetPosition(TimeSpan.FromMilliseconds(1 / (Progress.ActualWidth / e.GetPosition(Progress).X) * Progress.Maximum));
        }


        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            controller.Previous();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            controller.Next();
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                //mediaPlayer.Volume -= 5;
            }
            //else
            //mediaPlayer.Volume += 5;

            //VolumeBox.Text = "Volume: " + mediaPlayer.Volume.ToString() + "%";
            Storyboard s = Root.Resources["VolumeFadeIn"] as Storyboard;
            s.Begin();
        }

        private void VolumeFadeIn_Completed(object sender, EventArgs e)
        {
            Storyboard s = Root.Resources["VolumeFadeOut"] as Storyboard;
            s.Begin();
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (files.Length > 0)
            {
                //mediaPlayer.CurrentPlaylist.Clear();
                foreach (string f in files)
                {
                    //Wrappers.WindowsMediaPlayer.WmpMediaItem media = mediaPlayer.GetMediaItem(new Uri(f));
                    //if (media != null)
                    //{
                    //  mediaPlayer.CurrentPlaylist.Add(media);
                    //}
                }
                //if (mediaPlayer.CurrentPlaylist.Count > 0)
                // mediaPlayer.Play();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            controller.PlayPause();
        }
    }
}