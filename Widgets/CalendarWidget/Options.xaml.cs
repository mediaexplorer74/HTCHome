using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HTCHome.Core;
using System.Windows.Interop;
using E = HTCHome.Core.Environment;

namespace CalendarWidget
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private IntPtr handle;

        public Options()
        {
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;

            WinAPI.MARGINS margins = new WinAPI.MARGINS();
            margins.cyTopHeight = 24;

            HwndSource.FromHwnd(handle).CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            WinAPI.ExtendGlassFrame(handle, ref margins);

            GeneralTab.Header = Widget.LocaleManager.GetString("General");
            AboutTab.Header = Widget.LocaleManager.GetString("AboutWidget");
            SynchronizeCheckBox.Content = Widget.LocaleManager.GetString("Synchronize");
            UsernameTextBlock.Text = Widget.LocaleManager.GetString("Username");
            PassTextBlock.Text = Widget.LocaleManager.GetString("Password");

            Image1.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\icon.png"));

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            VersionString.Text = string.Format("Version {0}.{1} Build {2} R{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);

            SynchronizeCheckBox.IsChecked = Properties.Settings.Default.Sync;
            UsernameTextBox.Text = Properties.Settings.Default.Username;
            PassTextBox.Password = Properties.Settings.Default.Password;

            ApplyButton.IsEnabled = false;
        }

        private void SynchronizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void PassTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            ApplyButton.IsEnabled = false;
        }

        private void ApplySettings()
        {
            Properties.Settings.Default.Sync = (bool)SynchronizeCheckBox.IsChecked;
            Properties.Settings.Default.Username = UsernameTextBox.Text;
            Properties.Settings.Default.Password = PassTextBox.Password;

            Properties.Settings.Default.Save();
        }

        private void ContactStringMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }
    }
}
