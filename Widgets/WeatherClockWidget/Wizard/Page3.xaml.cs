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

namespace WeatherClockWidget.Wizard
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : UserControl
    {
        public Page3()
        {
            InitializeComponent();

            Title.Text = Widget.LocaleManager.GetString("Page3Title");
            String1.Text = Widget.LocaleManager.GetString("Page3String1");
            TaskbarIconCheckBox.Content = Widget.LocaleManager.GetString("Page3String2");

            NextButton.Content = Widget.LocaleManager.GetString("Next");

            if (Environment.OSVersion.Version.Major != 6 || Environment.OSVersion.Version.Minor != 1)
            {
                TaskbarIconCheckBox.IsChecked = false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TaskbarIconCheckBox.IsChecked = Properties.Settings.Default.ShowIconOnTaskbar;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowIconOnTaskbar = (bool)TaskbarIconCheckBox.IsChecked;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)Parent).Children.Add(new LastPage());
            ((Grid)Parent).Children.Remove(this);
        }
    }
}
