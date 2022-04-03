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

namespace WeatherClockWidget.Wizard
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : UserControl
    {
        public Page1()
        {
            InitializeComponent();
            Sun.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather/sun.png")));
            Cloud1.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather/cloud_3.png")));
            Cloud2.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather/cloud_9.png")));

            Title.Text = Widget.LocaleManager.GetString("Page1Title");
            String1.Text = Widget.LocaleManager.GetString("Page1String1");
            String2.Text = Widget.LocaleManager.GetString("Page1String2");

            SkipButton.Content = Widget.LocaleManager.GetString("Skip");
            NextButton.Content = Widget.LocaleManager.GetString("Next");
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            ((Window)(((Grid)this.Parent).Parent)).Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)Parent).Children.Add(new Page2());
            ((Grid)Parent).Children.Remove(this);
        }
    }
}
