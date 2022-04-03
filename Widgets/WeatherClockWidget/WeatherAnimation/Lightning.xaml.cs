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
using System.Windows.Media.Animation;

namespace WeatherClockWidget.WeatherAnimation
{
    /// <summary>
    /// Interaction logic for Lightning.xaml
    /// </summary>
    public partial class Lightning : UserControl
    {
        public Lightning()
        {
            InitializeComponent();
        }

        private int seed;
        private int count;
        private int direction;

        Random r;

        Rectangle lightningBg;

        public void Initialize(int seed, int direction, ref Rectangle bg)
        {
            this.seed = seed;
            this.direction = direction;
            this.lightningBg = bg;

            r = new Random(Environment.TickCount * seed);
            Image.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\lightning_0{0}.png", r.Next(1, 4)))));

            if (direction == 0)
            {
                Canvas.SetLeft(this, 120);
                Canvas.SetTop(this, 55);
            }
            else
            {
                Canvas.SetLeft(this, 270);
                Canvas.SetTop(this, 85);
            }

            Storyboard s = (Storyboard)Resources["MoveAnim"];
            ((DoubleAnimationUsingKeyFrames)s.Children[0]).BeginTime = TimeSpan.FromMilliseconds(r.Next(3000, 5500) + seed * 1000);
            Storyboard s1 = (Storyboard)Resources["BgAnim"];
            ((DoubleAnimationUsingKeyFrames)s1.Children[0]).BeginTime = ((DoubleAnimationUsingKeyFrames)s.Children[0]).BeginTime;
            Storyboard.SetTarget(((DoubleAnimationUsingKeyFrames)s1.Children[0]), lightningBg);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();
            Storyboard s1 = ((Storyboard)Resources["BgAnim"]);
            s1.Begin();
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            count++;
            if (count < 3)
            {
                Initialize(seed, direction, ref lightningBg);
                Storyboard s = (Storyboard)Resources["MoveAnim"];
                ((DoubleAnimationUsingKeyFrames)s.Children[0]).BeginTime = TimeSpan.FromMilliseconds(r.Next(3000, 5500) + seed * 1500);
                s.Begin();
                Storyboard s1 = ((Storyboard)Resources["BgAnim"]);
                ((DoubleAnimationUsingKeyFrames)s1.Children[0]).BeginTime = ((DoubleAnimationUsingKeyFrames)s.Children[0]).BeginTime;
                s1.Begin();
            }
            else
                if (this.Parent != null)
                {
                    ((Canvas)this.Parent).Children.Remove(this);
                }
        }

        public void PlayLightningSound()
        {
            //WeatherClock.mediaPlayer.Open(new Uri(Widget.ResourceManager.GetResourcePath("Sounds\\Thunder.wav")));
        }

        private void DoubleAnimationUsingKeyFrames_CurrentStateInvalidated(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState == ClockState.Active)
                if (WeatherClock.mediaPlayer != null && WeatherClock.mediaPlayer.Source != null)
                {
                    WeatherClock.mediaPlayer.Open(new Uri(Widget.ResourceManager.GetResourcePath("Sounds\\Thunder.wav")));
                    WeatherClock.mediaPlayer.Position = TimeSpan.FromMilliseconds(0);
                    WeatherClock.mediaPlayer.Play();
                }
        }
    }
}
