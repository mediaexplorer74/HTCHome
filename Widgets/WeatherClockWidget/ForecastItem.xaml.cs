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
using HTCHome.Core;

namespace WeatherClockWidget
{
    /// <summary>
    /// Interaction logic for ForecastItem.xaml
    /// </summary>
    public partial class ForecastItem : UserControl
    {
        public string Url { get; set; }

        public ForecastItem()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            Icon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\weather_1.png")));
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {

            Icon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", temp))));
            Storyboard s = (Storyboard)Icon.Resources["FlipAnim2"];
            s.Begin(this);
        }

        private int _order = 0;
        public int Order
        {
            get { return _order; }
            set
            {
                _order = value;
                Storyboard s = (Storyboard)Icon.Resources["FlipAnim1"];
                s.BeginTime = TimeSpan.FromMilliseconds(250 * value);
            }
        }

        public int temp;
        public void FlipWeather(int i)
        {
            if (i != temp)
            {
                temp = i;
                Storyboard s = (Storyboard)Icon.Resources["FlipAnim1"];
                s.Begin(this);
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1 && !string.IsNullOrEmpty(Url))
            {
                WinAPI.ShellExecute(IntPtr.Zero, "open", Url, "", "", 0);
            }
        }
    }
}
