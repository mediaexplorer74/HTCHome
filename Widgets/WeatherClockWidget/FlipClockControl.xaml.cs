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

namespace WeatherClockWidget
{
    /// <summary>
    /// Interaction logic for FlipClockControl.xaml
    /// </summary>
    public partial class FlipClockControl : UserControl
    {
        public delegate void HalfFlipDelegate();
        public event HalfFlipDelegate HalfFlip;
        //private bool halfFlipFired;

        private bool showAmPm;

        public bool ShowAmPm
        {
            get { return showAmPm; }
            set
            {
                showAmPm = value;
                if (value == true)
                {
                    AmPm.Visibility = System.Windows.Visibility.Visible;
                    AmPmBack.Opacity = 1;
                }
                else
                {
                    AmPm.Visibility = System.Windows.Visibility.Hidden;
                    AmPmBack.Opacity = 0;
                }
            }
        }

        public FlipClockControl()
        {
            InitializeComponent();
        }

        //Loads all resources
        public void Initialize(int d)
        {
            Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("FlipAnim\\Background\\flip_bg.png")));
            TabUp.ImageSource = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("FlipAnim\\Background\\flip_clock_tab_up.png")));
            TabDown.ImageSource = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("FlipAnim\\Background\\flip_clock_tab_down.png")));

            string path = "FlipAnim\\Digits\\{0}.png";

            BgLeftDigitTop.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format(path, GetFirstDigit(d)))));
            BgRightDigitTop.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format(path, GetLastDigit(d)))));

            BgLeftDigitBottom.Source = BgLeftDigitTop.Source;
            BgRightDigitBottom.Source = BgRightDigitTop.Source;

            AmPm.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("\\am.png")));
            AmPmBack.ImageSource = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("\\am.png")));
        }

        public void Flip(int d, bool useAnimation)
        {
            Flip(d, -1, useAnimation);
        }
        
        public void Flip(int d, int timeMode, bool useAnimation)
        {
            //halfFlipFired = false;

            string path = "FlipAnim\\Digits\\{0}.png";

            BgLeftDigitTop.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format(path, GetFirstDigit(d)))));
            BgRightDigitTop.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format(path, GetLastDigit(d)))));

            if (timeMode != -1)
            {
                if (timeMode == 0)
                    AmPmBack.ImageSource = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("am.png")));
                else
                    AmPmBack.ImageSource = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("pm.png")));
            }

            if (useAnimation)
            {
                Storyboard s = (Storyboard)this.Resources["FlipAnim"];
                s.Begin();
            }
            else
            {
                FlipAnim_Completed(this, EventArgs.Empty);
                HalfFlip();
            }
        }

        private int GetFirstDigit(int n)
        {
            int result = n;
            if (result > 9)
            {
                return (int)(result / 10);
            }
            else
                return 0;
        }

        private int GetLastDigit(int n)
        {
            int result = n;
            if (result > 9)
            {
                return GetRemainder(result, 10);
            }
            else
                return result;
        }

        private int GetRemainder(int a, int b)
        {
            int result = (int)(a / b);
            return a - result * b;
        }

        private void FlipAnim_Completed(object sender, EventArgs e)
        {
            BgLeftDigitBottom.Source = BgLeftDigitTop.Source;
            BgRightDigitBottom.Source = BgRightDigitTop.Source;


            AmPm.Source = AmPmBack.ImageSource;
        }

        private void DummyAnim_Cmpleted(object sender, EventArgs e)
        {
            HalfFlip();
        }

    }
}
