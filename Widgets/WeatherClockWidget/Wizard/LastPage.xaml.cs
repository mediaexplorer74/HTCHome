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
using HTCHome.Core;

namespace WeatherClockWidget.Wizard
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class LastPage : UserControl
    {
        public LastPage()
        {
            InitializeComponent();

            Title.Text = Widget.LocaleManager.GetString("LastPageTitle");
            String1.Text = Widget.LocaleManager.GetString("LastPageString1");
            string s = Widget.LocaleManager.GetString("LastPageString2");
            String2_1.Text = s.Split(new string[] { "{0}" }, StringSplitOptions.RemoveEmptyEntries)[0];
            String2_2.Text = s.Split(new string[] { "{0}" }, StringSplitOptions.RemoveEmptyEntries)[1];
            String2_3.Text = Widget.LocaleManager.GetString("LastPageString3");

            Icon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather/weather_32.png")));

            CloseButton.Content = Widget.LocaleManager.GetString("Close");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((Window)(((Grid)this.Parent).Parent)).Close();
        }

        private void Link_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", Link.NavigateUri.ToString(), "", "", 0);
        }

        private void Link_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", Link.NavigateUri.ToString(), "", "", 0);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", "http://htchome.org", "", "", 0);
        }

    }
}
