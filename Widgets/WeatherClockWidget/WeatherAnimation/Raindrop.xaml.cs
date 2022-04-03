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
using E = HTCHome.Core.Environment;

namespace WeatherClockWidget.WeatherAnimation
{
    /// <summary>
    /// Interaction logic for Raindrop.xaml
    /// </summary>
    public partial class Raindrop : UserControl
    {
        public Raindrop()
        {
            InitializeComponent();
        }

        private int count;
        private int seed;

        public void Initialize(int seed, int begintime)
        {
            this.seed = seed;
            Random r = new Random(Environment.TickCount * seed);
            Image.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\rain_fall.png", r.Next(1, 4)))));
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            ((DoubleAnimation)s.Children[0]).Duration = TimeSpan.FromMilliseconds(r.Next(900, 1400));
            ((DoubleAnimation)s.Children[0]).BeginTime = TimeSpan.FromMilliseconds(begintime);
            Canvas.SetLeft(this, r.Next(120, 380));
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            count++;
            if (count < 25)
            {
                Initialize(seed, 0);
                Storyboard s = (Storyboard)Resources["MoveAnim"];
                s.Begin();
            }
            else
                if (this.Parent != null)
                {
                    ((Canvas)this.Parent).Children.Remove(this);
                }
        }

        private void DoubleAnimation_Completed_1(object sender, EventArgs e)
        {

        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();
        }

        private void DoubleAnimation_CurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock c = (Clock)sender;
            if (c.CurrentState == ClockState.Active)
                this.Opacity = 0.7;
            if (c.CurrentState == ClockState.Stopped)
                this.Opacity = 0;
        }

    }
}
