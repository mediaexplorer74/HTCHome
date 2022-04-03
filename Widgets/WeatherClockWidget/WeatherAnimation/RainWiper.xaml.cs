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
    /// Interaction logic for RainWiper.xaml
    /// </summary>
    public partial class RainWiper : UserControl
    {
        public RainWiper()
        {
            InitializeComponent();
        }

        private int count;
        public void Initialize()
        {
            Wiper.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\rain_wiper.png")));
            Streaks.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\raindrop_streaks.png")));
            Canvas.SetLeft(this, 100);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard s = (Storyboard)Resources["MoveAnim"];
            s.Begin();

            Storyboard s1 = (Storyboard)Resources["StreaksAnim"];
            s1.Begin();
        }

        private void DoubleAnimation_CurrentStateInvalidated(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                Opacity = 1;
                Streaks.Opacity = 0.5;
            }
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            count++;
            if (count < 1) //2
            {
                Storyboard s = (Storyboard)Resources["MoveAnim"];
                s.Begin();

                Storyboard s1 = (Storyboard)Resources["StreaksAnim"];
                s1.Begin();
                Streaks.Opacity = 0.5;
            }
        }

        private void DoubleAnimation_Completed_1(object sender, EventArgs e)
        {
            Streaks.Opacity = 0;
        }
    }
}
