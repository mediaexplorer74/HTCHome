using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HTCHome.Core;
using System.Windows.Interop;
using System.Threading;
using E = HTCHome.Core.Environment;
using System.IO;
using System.Xml.Linq;

namespace PhotoWidget
{
    /// <summary>
    /// Interaction logic for options.xaml
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

            GeneralTab.Header = Widget.Instance.LocaleManager.GetString("General");
            AboutTab.Header = Widget.Instance.LocaleManager.GetString("AboutWidget");

            SwitchAutomaticallyCheckBox.Content = Widget.Instance.LocaleManager.GetString("SwitchAutomatically");
            IntervalTitle.Text = Widget.Instance.LocaleManager.GetString("Interval");
            PicsPathTitle.Text = Widget.Instance.LocaleManager.GetString("PicsDirectory");
            ChooseButton.Content = Widget.Instance.LocaleManager.GetString("Choose");
            UiModeTitle.Text = Widget.Instance.LocaleManager.GetString("UIMode");
            LandscapeItem.Content = Widget.Instance.LocaleManager.GetString("Landscape");
            PortraitItem.Content = Widget.Instance.LocaleManager.GetString("Portrait");

            RestartText.Text = Widget.Instance.LocaleManager.GetString("RestartText");

            OkButton.Content = Widget.Instance.LocaleManager.GetString("OK");
            CancelButton.Content = Widget.Instance.LocaleManager.GetString("Cancel");
            ApplyButton.Content = Widget.Instance.LocaleManager.GetString("Apply");

            Image1.Source = new BitmapImage(new Uri(E.Path + "\\Photo\\Resources\\icon.png"));

            System.Reflection.Assembly asmObj = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName currAsmName = asmObj.GetName();
            string major = currAsmName.Version.Major.ToString();
            string minor = currAsmName.Version.Minor.ToString();
            string build = currAsmName.Version.Build.ToString();

            VersionString.Text = string.Format("Version {0}.{1} Build {2}", major, minor, build);

            SwitchAutomaticallyCheckBox.IsChecked = Properties.Settings.Default.SwitchAutomatically;
            IntervalComboBox.Text = Properties.Settings.Default.Interval.ToString();
            PicsPathBox.Text = Properties.Settings.Default.PicsPath;

            UiModeComboBox.SelectedIndex = Properties.Settings.Default.Mode;

            ApplyButton.IsEnabled = false;
        }


        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            if (ApplyButton.IsEnabled)
            {
                ApplySettings();
            }

            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            ApplyButton.IsEnabled = false;
        }

        private void ApplySettings()
        {
            bool rescan = Properties.Settings.Default.PicsPath != PicsPathBox.Text;

            Properties.Settings.Default.Interval = Convert.ToInt32(IntervalComboBox.Text); ;
            Properties.Settings.Default.SwitchAutomatically = (bool)SwitchAutomaticallyCheckBox.IsChecked;
            Properties.Settings.Default.PicsPath = PicsPathBox.Text;
            Properties.Settings.Default.Mode = UiModeComboBox.SelectedIndex;

            Properties.Settings.Default.Save();
            Widget.Instance.UpdateSettings(rescan);
        }

        private void ContactStringMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }

        private void ChooseButtonClick(object sender, RoutedEventArgs e)
        {
            var d = new FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PicsPathBox.Text = d.SelectedPath;
            }
        }

        private void PicsPathBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void SwitchAutomaticallyCheckBoxClick(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }
    }
}
