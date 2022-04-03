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
    /// Interaction logic for Cloud.xaml
    /// </summary>
    public partial class Cloud : UserControl
    {
        public Cloud()
        {
            InitializeComponent();
        }

        private int count;

        public void Initialize(int seed)
        {
            Random r = new Random(Environment.TickCount * seed);
            Image.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\cloud_{0}.png", r.Next(1, 10)))));
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            //((DoubleAnimation)s.Children[0]).Duration = TimeSpan.FromMilliseconds(r.Next(3000, 7000));
            ((DoubleAnimation)s.Children[0]).Duration = TimeSpan.FromMilliseconds(r.Next(9000, 20000));
            Canvas.SetTop(this, r.Next(0, 100));
            //Scale.ScaleX = r.Next(5, 10) / 10;

            Storyboard fadeIn = (Storyboard)Resources["FadeIn"];
            fadeIn.Begin();

            Storyboard fadeOut = (Storyboard)Resources["FadeOut"];
            fadeOut.BeginTime = ((DoubleAnimation)s.Children[0]).Duration.TimeSpan - TimeSpan.FromMilliseconds(1000);
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            count++;
            if (count < 5)
            {
                Initialize(Environment.TickCount);
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
            Storyboard fadeOut = (Storyboard)Resources["FadeOut"];
            fadeOut.Begin();
        }

    }
}
