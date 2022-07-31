// Options

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using HTCHome.Core;
using System.IO;
using Microsoft.Win32;
using System.Reflection;

namespace HTCHome
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private IntPtr handle;
        private List<string> localeCodes = new List<string>();

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

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString.Text = string.Format("Version {0}.{1} Build {2} R{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);

            TranslatedBy.Text = App.LocaleManager.GetString("LocaleAuthor");

            OkButton.Content = App.LocaleManager.GetString("OK");
            CancelButton.Content = App.LocaleManager.GetString("Cancel");
            ApplyButton.Content = App.LocaleManager.GetString("Apply");

            Title = App.LocaleManager.GetString("Options");
            AutostartCheckBox.Content = App.LocaleManager.GetString("Autostart");
            EnableGlassCheckBox.Content = App.LocaleManager.GetString("EnableGlass");
            CheckForUpdatesCheckBox.Content = App.LocaleManager.GetString("CheckForUpdates");
            LangTextBlock.Text = App.LocaleManager.GetString("Language");
            RestartText.Text = App.LocaleManager.GetString("RestartText");

            EnableGlassCheckBox.IsChecked = HTCHome.Properties.Settings.Default.EnableGlass;
            AutostartCheckBox.IsChecked = HTCHome.Properties.Settings.Default.Autostart;
            CheckForUpdatesCheckBox.IsChecked = HTCHome.Properties.Settings.Default.EnableUpdates;

            if (Directory.Exists(App.Path + "\\Localization"))
            {
                foreach (string f in Directory.GetFiles(App.Path + "\\Localization", "*.xaml"))
                {
                    string name = LocaleManager.GetLocaleName(f);
                    string code = LocaleManager.GetLocaleCode(f);
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(code))
                    {
                        LangComboBox.Items.Add(name);
                        localeCodes.Add(code);
                    }
                }
            }

            LangComboBox.SelectedIndex = localeCodes.IndexOf(Core.Environment.Locale);

            GeneralTab.Header = App.LocaleManager.GetString("General");
            AboutTab.Header = App.LocaleManager.GetString("About");
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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

        private void ContactString_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }

        private void ApplySettings()
        {

            HTCHome.Properties.Settings.Default.EnableGlass = (bool)EnableGlassCheckBox.IsChecked;
            HTCHome.Properties.Settings.Default.Autostart = (bool)AutostartCheckBox.IsChecked;
            HTCHome.Properties.Settings.Default.Locale = localeCodes[LangComboBox.SelectedIndex];
            HTCHome.Properties.Settings.Default.EnableUpdates = (bool)CheckForUpdatesCheckBox.IsChecked;
            HTCHome.Core.Environment.Locale = localeCodes[LangComboBox.SelectedIndex];
            App.LocaleManager.LoadLocale(localeCodes[LangComboBox.SelectedIndex]);

            if (AutostartCheckBox.IsChecked == true)
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree).OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Run", true))
                    {
                        key.SetValue("HTC Home", "\"" + Assembly.GetExecutingAssembly().Location + "\"", RegistryValueKind.String);
                        key.Close();
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Log(ex.ToString());
                }
            }
            else
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree).OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Run", true))
                    {
                        key.DeleteValue("HTC Home", false);
                        key.Close();
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Log(ex.ToString());
                }
            }

            HTCHome.Properties.Settings.Default.LoadedWidgets.Clear();

            foreach (Widget w in App.widgets)
            {
                if (w.IsWidgetLoaded)
                    HTCHome.Properties.Settings.Default.LoadedWidgets.Add(w.WidgetName);
            }

            HTCHome.Properties.Settings.Default.Save();
        }
    }
}
