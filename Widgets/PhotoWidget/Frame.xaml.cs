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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoWidget
{
    /// <summary>
    /// Interaction logic for Frame.xaml
    /// </summary>
    public partial class Frame : UserControl
    {
        public event EventHandler FadeCompleted;

        public Frame()
        {
            InitializeComponent();

            if (Properties.Settings.Default.Mode == 1)
            {
                Width = 250;
                Height = 362;
                FrameBg.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/photo_frame_portrait.png", UriKind.Relative));
                FrameImage.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/preview_portrait.png", UriKind.Relative));
                FrameImage.Margin = new Thickness(24, 28, 24, 28);
            }
        }

        public ImageSource Source
        {
            get { return FrameImage.Source; }
            set { FrameImage.Source = value; }
        }

        public void FadeIn()
        {
            var s = (Storyboard)Resources["FadeIn"];
            s.Begin();
        }

        public void FadeOut()
        {
            var s = (Storyboard)Resources["FadeOut"];
            s.Begin();
        }

        public double Angle
        {
            get { return Rotation.Angle; }
            set { Rotation.Angle = value; }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (FadeCompleted != null)
                FadeCompleted(sender, e);
            var s = (Storyboard)Resources["FadeBack"];
            s.Begin();
        }

        private void Storyboard_Completed_1(object sender, EventArgs e)
        {
            if (FadeCompleted != null)
                FadeCompleted(sender, e);
        }

        public void SwitchMode()
        {
            if (Properties.Settings.Default.Mode == 1)
            {
                Width = 250;
                Height = 362;
                FrameBg.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/photo_frame_portrait.png", UriKind.Relative));
                FrameImage.Margin = new Thickness(24, 28, 24, 28);
            }
            else
            {
                Width = 362;
                Height = 250;
                FrameBg.Source =
                    new BitmapImage(new Uri("/Photo;component/Resources/photo_frame_portrait.png", UriKind.Relative));
                FrameImage.Margin = new Thickness(12, 14, 12, 14);
            }
        }
    }
}
