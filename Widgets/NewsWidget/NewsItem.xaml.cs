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
using System.Windows.Media.Animation;
using HTCHome.Core;
using System.Net;
using System.IO;
using System.Windows.Markup;
using E = HTCHome.Core.Environment;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace NewsWidget
{
    /// <summary>
    /// Interaction logic for NewsItem.xaml
    /// </summary>
    public partial class NewsItem : UserControl
    {
        public string Link;
        public string Source;

        public string Text
        {
            get { return ContentTextBlock.Text; }
            set
            {
                ContentTextBlock.Text = value;
            }
        }

        public string Title
        {
            get { return TitleTextBlock.Text; }
            set { TitleTextBlock.Text = value; }
        }

        public string Footer
        {
            get { return FooterTextBlock.Text; }
            set { FooterTextBlock.Text = value; }
        }

        public static void HyperlinkMouseDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ((Hyperlink)sender).NavigateUri.ToString(), "", "", 1);
        }

        private string _imageSource;
        public string ImageSource
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && value.StartsWith("http://") && (value.EndsWith(".jpg") || value.EndsWith(".png")))
                {
                    _imageSource = value;
                    WebClient w = new WebClient();
                    w.DownloadFileAsync(new Uri(value), Path.GetTempPath() + Path.GetFileName(value));
                    w.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(WDownloadFileCompleted);

                }
            }
        }

        void WDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string path = Path.GetTempPath() + Path.GetFileName(_imageSource);
            if (File.Exists(path))
            {
                var f = new FileInfo(path);
                if (f.Length > 0)
                {
                    try
                    {
                        IconImage.Source = new BitmapImage(new Uri(path));
                        IconImage.Visibility = System.Windows.Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        HTCHome.Core.Logger.Log("Can't set news item image source. Image " + path + "(original: " + _imageSource + ")" + ex.ToString());
                    }
                }
            }
        }

        public NewsItem()
        {
            InitializeComponent();
        }

        private void TitleTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                WinAPI.ShellExecute(IntPtr.Zero, "open", Link, "", "", 1);
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["LoadAnim"];
            s.Begin();
        }

    }
}
