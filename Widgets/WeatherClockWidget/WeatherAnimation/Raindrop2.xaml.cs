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
    /// Interaction logic for Raindrop2.xaml
    /// </summary>
    public partial class Raindrop2 : UserControl
    {
        public Raindrop2()
        {
            InitializeComponent();
        }

        private int seed;
        private int count;

        public void Initialize(int seed)
        {
            this.seed = seed;
            Random r = new Random(Environment.TickCount * seed);
            Image.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\layer_drop{0}.png", r.Next(1, 8)))));
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            ((DoubleAnimation)s.Children[0]).Duration = TimeSpan.FromMilliseconds(r.Next(6000, 7500));
            ((DoubleAnimation)s.Children[0]).BeginTime = TimeSpan.FromMilliseconds(r.Next(500, 2000) + seed * 1000);
            Canvas.SetLeft(this, r.Next(120, 380));
            Canvas.SetTop(this, r.Next(120, 200));
            ((DoubleAnimation)s.Children[0]).From = Canvas.GetTop(this);

            Storyboard f = (Storyboard)Resources["FadeIn"];
            ((DoubleAnimation)f.Children[0]).BeginTime = TimeSpan.FromMilliseconds(r.Next(1200, 3500) + seed * 1000);

        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {            
            Random r = new Random(Environment.TickCount * seed);

            Opacity = 0;
            count++;
            if (count < 2)
            {
                Initialize(seed);
                Storyboard s = (Storyboard)Resources["FadeIn"];
                s.Begin();
            }
            else
                if (this.Parent != null)
                {
                    ((Canvas)this.Parent).Children.Remove(this);
                }
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            Opacity = 0;
            Storyboard s = (Storyboard)Resources["FadeIn"];
            s.Begin();
        }

        private void DoubleAnimation_Completed_1(object sender, EventArgs e)
        {
            Opacity = 1;
            //Initialize(seed);
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();
        }
    }
}
