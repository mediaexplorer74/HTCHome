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
    /// Interaction logic for Icicle.xaml
    /// </summary>
    public partial class Icicle : UserControl
    {
        public Icicle()
        {
            InitializeComponent();
        }
        public void Initialize(int n)
        {
            Random r = new Random(Environment.TickCount * n);
            Image.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\icicle{0}.png", n))));
            Storyboard s = (Storyboard)this.Resources["GrowIcicleAnim"];
            s.BeginTime = TimeSpan.FromMilliseconds(r.Next(1500, 2500));
            s.Begin();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                ((Canvas)this.Parent).Children.Remove(this);
            }
        }

        private void DoubleAnimation_Completed_1(object sender, EventArgs e)
        {
            Storyboard s = (Storyboard)this.Resources["FadeOut"];
            s.Begin();
        }
    }
}
