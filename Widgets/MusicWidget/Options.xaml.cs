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
using System.Windows.Shapes;
using HTCHome.Core;
using System.Windows.Interop;
using System.Xml.Linq;
using E = HTCHome.Core.Environment;
using System.IO;
using MediaPlayerWidget.Domain;

namespace MediaPlayerWidget
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private IntPtr handle;
        private MediaPlayer widget;
        private List<string> skins = new List<string>();

        public Options(MediaPlayer p)
        {
            InitializeComponent();

            widget = p;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;

            WinAPI.MARGINS margins = new WinAPI.MARGINS();
            margins.cyTopHeight = 24;

            HwndSource.FromHwnd(handle).CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            WinAPI.ExtendGlassFrame(handle, ref margins);

            System.Reflection.Assembly _AsmObj = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName _CurrAsmName = _AsmObj.GetName();
            string _Major = _CurrAsmName.Version.Major.ToString();
            string _Minor = _CurrAsmName.Version.Minor.ToString();
            string _Build = _CurrAsmName.Version.Build.ToString();

            VersionString.Text = string.Format("Version {0}.{1} Build {2}", _Major, _Minor, _Build);

            //GeneralTab.Header = Widget.LocaleManager.GetString("General");
            //LocationTab.Header = Widget.LocaleManager.GetString("Location");
            GeneralTab.Header = Widget.LocaleManager.GetString("General");
            SkinTab.Header = Widget.LocaleManager.GetString("Skin");
            AboutTab.Header = Widget.LocaleManager.GetString("AboutWidget");

            ControllersText.Text = Widget.LocaleManager.GetString("Controller");

            OkButton.Content = Widget.LocaleManager.GetString("OK");
            CancelButton.Content = Widget.LocaleManager.GetString("Cancel");
            ApplyButton.Content = Widget.LocaleManager.GetString("Apply");

            var dirs = from x in Directory.GetDirectories(E.Path + "\\Music\\Skins")
                       where File.Exists(x + "\\Skin.xml")
                       select x;

            foreach (var d in dirs)
            {
                XDocument doc = XDocument.Load(d + "\\Skin.xml");

                ComboBoxItem item = new ComboBoxItem();
                item.Content = doc.Root.Element("Name").Value;
                SkinsComboBox.Items.Add(item);
                skins.Add(new DirectoryInfo(d).Name);
            }

            XDocument skin = XDocument.Load(E.Path + "\\Music\\Skins\\" + Properties.Settings.Default.Skin + "\\Skin.xml");

            SkinsComboBox.Text = skin.Root.Element("Name").Value;

            if (Directory.Exists(E.ExtensionsPath + "\\Music"))
            {
                var directories = from x in Directory.GetDirectories(E.ExtensionsPath + "\\Music")
                            select x;
                foreach (var d in directories)
                {
                    DirectoryInfo i = new DirectoryInfo(d);
                    if (File.Exists(E.ExtensionsPath + "\\Music\\" + i.Name + "\\" + i.Name + ".dll"))
                    {
                        ControllersBox.Items.Add(i.Name);
                    }
                }
            }

            ControllersBox.Text = Properties.Settings.Default.Controller;

            Image1.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("icon.png")));
            ApplyButton.IsEnabled = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyButton.IsEnabled)
            {
                ApplySettings();
            }

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
            Properties.Settings.Default.Skin = skins[SkinsComboBox.SelectedIndex];
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.Controller != ControllersBox.Text)
            {
                Properties.Settings.Default.Controller = ControllersBox.Text;
                widget.controller = Controller.Load(E.Path + "\\Music\\Controllers\\" + ControllersBox.Text + "\\" + ControllersBox.Text + ".dll");
                widget.InitController();
            }

            widget.UpdateSettings();
        }

        private void ContactString_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }

        private void SkinsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XDocument doc = XDocument.Load(E.Path + "\\Music\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Skin.xml");
            AuthorTextBlock.Text = Widget.LocaleManager.GetString("Author") + " " + doc.Root.Element("Author").Value;
            VersionTextBlock.Text = Widget.LocaleManager.GetString("Version") + " " + doc.Root.Element("Version").Value;
            ContactsTextBlock.Text = Widget.LocaleManager.GetString("Contacts") + " " + doc.Root.Element("Contacts").Value;
            DescriptionTextBlock.Text = doc.Root.Element("Description").Value;

            if (File.Exists(E.Path + "\\Music\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Preview.png"))
                SkinPreview.Source = new BitmapImage(new Uri(E.Path + "\\Music\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Preview.png"));
            else
                SkinPreview.Source = null;

            ApplyButton.IsEnabled = true;
        }

        private void ControllersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }
    }
}
