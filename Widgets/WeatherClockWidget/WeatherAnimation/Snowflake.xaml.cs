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
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace WeatherClockWidget.WeatherAnimation
{
    /// <summary>
    /// Interaction logic for Snowflake.xaml
    /// </summary>
    public partial class Snowflake : UserControl
    {
        private int count = 0;
        private int seed = 1;
        private double cX = 0;

        private double wind = 0;
        //private double gravity = 2500;

        private double speedX = 1.0;

        private double lifeTime; //used for changing wind and gravity

        private double scale = 1.0;

        Random rnd;

        public Snowflake()
        {
            InitializeComponent();
        }

        public void Initialize(int seed, int begintime)
        {
            this.seed = seed;

            rnd = new Random(Environment.TickCount * seed);
            Image.Source = new BitmapImage(new Uri((Widget.ResourceManager.GetResourcePath(string.Format("Weather\\snowflake{0}.png", rnd.Next(1,6))/*"Weather\\snow3.png"*/))));
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            ((DoubleAnimation)s.Children[0]).Duration = TimeSpan.FromMilliseconds(rnd.Next(3500, 4500));
            ((DoubleAnimation)s.Children[0]).BeginTime = TimeSpan.FromMilliseconds(begintime);
            //((DoubleAnimation)s.Children[1]).BeginTime = ((DoubleAnimation)s.Children[0]).Duration.TimeSpan - TimeSpan.FromMilliseconds(100);
            Canvas.SetLeft(this, rnd.Next(120, 380));

            scale = (25 + rnd.NextDouble() * 75) / 70;

            Scale.ScaleX = scale;
            Scale.ScaleY = scale;

            speedX = rnd.NextDouble() - rnd.NextDouble();
            //gravity = 0.5 * scale;

        }

        private void DoubleAnimation_CurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock c = (Clock)sender;
            if (c.CurrentState == ClockState.Active)
                this.Opacity = 1;
            if (c.CurrentState == ClockState.Stopped)
                this.Opacity = 0;
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            fadeRunning = false;
            count++;
            if (count < 8)
            {
                Initialize(seed++, 0);
                Storyboard s = (Storyboard)Resources["MoveAnim"];
                s.Begin();
            }
            else
                if (this.Parent != null)
                {
                    ((Canvas)this.Parent).Children.Remove(this);
                }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();
        }

        bool fadeRunning;
        private void DoubleAnimation_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            MoveSnowflake();

            if (((Clock)sender).CurrentProgress != null && ((Clock)sender).CurrentProgress.Value >= 0.7 && !fadeRunning)
            {
                Storyboard s = (Storyboard)Resources["FadeOut"];
                s.Begin();
                fadeRunning = true;
            }
        }

        void MoveSnowflake()
        {
            lifeTime += 0.001;
            cX = Canvas.GetLeft(this) + speedX + wind * scale;

            Canvas.SetLeft(this, cX);

            if (lifeTime > 0.1)
            {
                lifeTime = 0;

                double newX = rnd.NextDouble() - rnd.NextDouble();
                double chX = (newX - wind) / (50 + rnd.Next(50));
                wind += chX;
            }
        }

        private void DoubleAnimation_Completed_1(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }
    }
}
