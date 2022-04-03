using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using E = HTCHome.Core.Environment;
using Path = System.IO.Path;

namespace PhotoWidget
{
    /// <summary>
    /// Interaction logic for WidgetControl.xaml
    /// </summary>
    public partial class WidgetControl : UserControl
    {
        private List<string> pics;
        public Random r;
        private bool switching;
        private DispatcherTimer _timer;
        private Options options;
        private string _currentImage;

        public WidgetControl()
        {
            InitializeComponent();
        }

        public void Load()
        {
            var nextItem = new MenuItem { Header = Widget.Instance.LocaleManager.GetString("Next") };
            nextItem.Click += NextItemClick;

            var optionsItem = new MenuItem();
            optionsItem.Header = Widget.Instance.LocaleManager.GetString("Options");
            optionsItem.Click += OptionsItemClick;

            Widget.Instance.Parent.ContextMenu.Items.Insert(0, new Separator());
            Widget.Instance.Parent.ContextMenu.Items.Insert(0, optionsItem);
            Widget.Instance.Parent.ContextMenu.Items.Insert(0, nextItem);

            pics = new List<string>();
            Scan();
            Frame.FadeCompleted += new EventHandler(FrameFadeCompleted);
            BgFrame.FadeCompleted += new EventHandler(BgFrameFadeCompleted);

            r = new Random(Environment.TickCount);

            if (Properties.Settings.Default.Mode == 0)
            {
                BgImage.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/photo_frame_landscape.png", UriKind.Relative));
                BgImage.Width = 362;
                BgImage.Height = 250;
            }
            else
            {
                BgImage.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/photo_frame_portrait.png", UriKind.Relative));
                BgImage.Width = 250;
                BgImage.Height = 362;
            }

            if (pics.Count > 0)
            {
                var i = r.Next(0, pics.Count);
                if (File.Exists(pics[i]))
                {
                    BgFrame.Source = new BitmapImage(new Uri(pics[i]));
                    _lastImage = pics[i];
                }
                i = r.Next(0, pics.Count);
                if (File.Exists(pics[i]))
                {
                    Frame.Source = new BitmapImage(new Uri(pics[i]));
                    _currentImage = pics[i];
                }
            }
            else
            {
                if (Properties.Settings.Default.Mode == 0)
                {
                    BgFrame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_landscape.png", UriKind.Relative));
                    Frame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_landscape.png", UriKind.Relative));
                }
                else
                {
                    BgFrame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_portrait.png", UriKind.Relative));
                    Frame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_portrait.png", UriKind.Relative));
                }
            }

            options = new Options();

            if (Properties.Settings.Default.SwitchAutomatically)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMinutes(Properties.Settings.Default.Interval);
                _timer.Tick += new EventHandler(TimerTick);
                _timer.Start();
            }

            RenderOptions.SetBitmapScalingMode(BgFrame.FrameImage.Source, BitmapScalingMode.NearestNeighbor);
        }

        private string _lastImage;

        void BgFrameFadeCompleted(object sender, EventArgs e)
        {
            if (pics.Count > 0)
            {
                var i = r.Next(0, pics.Count);
                if (File.Exists(pics[i]))
                {
                    var bi = new BitmapImage(new Uri(pics[i]));
                    BgFrame.Source = bi;
                    bi.Freeze();
                    _currentImage = _lastImage;
                    _lastImage = pics[i];
                }
            }
            else
            {
                if (Properties.Settings.Default.Mode == 0)
                {
                    BgFrame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_landscape.png", UriKind.Relative));
                    BgFrame.Source.Freeze();
                }
                else
                {
                    BgFrame.Source =
                        new BitmapImage(new Uri("/Photo;component/Resources/preview_portrait.png", UriKind.Relative));
                    BgFrame.Source.Freeze();
                }
            }
        }

        public void UpdateSettings(bool rescan)
        {
            if (_timer != null)
                _timer.Stop();

            if (Properties.Settings.Default.SwitchAutomatically)
            {

                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMinutes(Properties.Settings.Default.Interval);
                _timer.Tick += new EventHandler(TimerTick);
                _timer.Start();
            }

            if (rescan)
                Scan();
        }

        private void Scan()
        {
            pics.Clear();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.PicsPath) && Directory.Exists(Properties.Settings.Default.PicsPath))
            {
                foreach (var f in Directory.GetFiles(Properties.Settings.Default.PicsPath, "*.jpg", SearchOption.AllDirectories))
                    pics.Add(f);

                foreach (var f in Directory.GetFiles(Properties.Settings.Default.PicsPath, "*.png", SearchOption.AllDirectories))
                    pics.Add(f);
            }
        }

        void TimerTick(object sender, EventArgs e)
        {
            SwitchPhotos();
        }

        void OptionsItemClick(object sender, RoutedEventArgs e)
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options();

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

        void FrameFadeCompleted(object sender, EventArgs e)
        {
            var bi = BgFrame.FrameImage.Source;
            Frame.Source = bi;
            bi.Freeze();
            switching = false;
        }

        void NextItemClick(object sender, RoutedEventArgs e)
        {
            SwitchPhotos();
        }

        private void SwitchPhotos()
        {
            switching = true;
            Frame.FadeOut();
            BgFrame.FadeIn();
        }

        private void PrepareThumbnail(string source)
        {

            string strFileName = source;
            string strThumbnail = "Thumb";//Path.GetFileNameWithoutExtension(source);
            byte[] baSource = File.ReadAllBytes(strFileName);
            using (Stream streamPhoto = new MemoryStream(baSource))
            {
                BitmapFrame bfPhoto = ReadBitmapFrame(streamPhoto);

                int nThumbnailSize = 200, nWidth, nHeight;
                if (bfPhoto.Width > bfPhoto.Height)
                {
                    nWidth = nThumbnailSize;
                    nHeight = (int)(bfPhoto.Height * nThumbnailSize / bfPhoto.Width);
                }
                else
                {
                    nHeight = nThumbnailSize;
                    nWidth = (int)(bfPhoto.Width * nThumbnailSize / bfPhoto.Height);
                }
                BitmapFrame bfResize = FastResize(bfPhoto, nWidth, nHeight);
                byte[] baResize = ToByteArray(bfResize);

                File.WriteAllBytes(E.Path + "\\Photo\\Thumbnail\\" + strThumbnail + ".png", baResize);
            }

        }

        private static BitmapFrame FastResize(BitmapFrame bfPhoto, int nWidth, int nHeight)
        {
            var tbBitmap = new TransformedBitmap(bfPhoto, new ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private static byte[] ToByteArray(BitmapFrame bfResize)
        {
            using (var msStream = new MemoryStream())
            {
                var pbdDecoder = new PngBitmapEncoder();
                pbdDecoder.Frames.Add(bfResize);
                pbdDecoder.Save(msStream);
                return msStream.ToArray();
            }
        }

        private static BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }


        private void UserControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && !switching)
                SwitchPhotos();
        }

        private void UserControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                HTCHome.Core.WinAPI.ShellExecute(IntPtr.Zero, "open", _currentImage, "", "", 0);
        }
    }
}
