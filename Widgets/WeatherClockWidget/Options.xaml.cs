using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HTCHome.Core;
using System.Windows.Interop;
using System.Threading;
using WeatherClockWidget.Domain;
using E = HTCHome.Core.Environment;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

namespace WeatherClockWidget
{
    /// <summary>
    /// Interaction logic for options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private IntPtr handle;
        private WeatherClock widget;

        private List<CityLocation> results = new List<CityLocation>();

        private readonly List<string> skins = new List<string>();

        int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                //if (value == _selectedIndex)
                //    return;

                if (value > -1)
                {
                    ((LocationItem)SearchResults.Children[value]).Selected = true;
                    Properties.Settings.Default.LocationCode = results[value].City;//.Code;
                    widget.weatherTimer_Tick(null, EventArgs.Empty);

                }
                else
                {
                    //
                }

                if 
                (
                    _selectedIndex > -1
                    && 
                    _selectedIndex < SearchResults.Children.Count
                    )
                {
                    ((LocationItem)SearchResults.Children[_selectedIndex]).Selected = false;
                }
                
                _selectedIndex = value;
            }
        }


        public Options(WeatherClock w)
        {
            InitializeComponent();

            widget = w;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;

            WinAPI.MARGINS margins = new WinAPI.MARGINS();
            margins.cyTopHeight = 24;

            HwndSource.FromHwnd(handle).CompositionTarget.BackgroundColor 
                = Color.FromArgb(0, 0, 0, 0);

            WinAPI.ExtendGlassFrame(handle, ref margins);

            GeneralTab.Header = Widget.LocaleManager.GetString("General");
            LocationTab.Header = Widget.LocaleManager.GetString("Location");
            SkinTab.Header = Widget.LocaleManager.GetString("Skin");
            AboutTab.Header = Widget.LocaleManager.GetString("AboutWidget");

            TaskbarIconCheckBox.Content = Widget.LocaleManager.GetString("ShowTaskbarIcon");
            WeatherCheckBox.Content = Widget.LocaleManager.GetString("ShowWeather");
            WeatherAnimationCheckBox.Content = Widget.LocaleManager.GetString("EnableWeatherAnimation");
            ShowForecastCheckBox.Content = Widget.LocaleManager.GetString("ShowForecast");
            EnableSoundsCheckBox.Content = Widget.LocaleManager.GetString("EnableSounds");
            ProviderTextBlock.Text = Widget.LocaleManager.GetString("WeatherProvider");
            IntervalTextBlock.Text = Widget.LocaleManager.GetString("Interval");
            ShowTempInTextBlock.Text = Widget.LocaleManager.GetString("ShowTempIn");
            Fahrenheit.Content = Widget.LocaleManager.GetString("Fahrenheit");
            Celsius.Content = Widget.LocaleManager.GetString("Celsius");
            TimeModeTextBlock.Text = Widget.LocaleManager.GetString("TimeMode");
            TimeMode1.Content = Widget.LocaleManager.GetString("TimeMode1");
            TimeMode2.Content = Widget.LocaleManager.GetString("TimeMode2");
            WallpaperChangingCheckBox.Content = Widget.LocaleManager.GetString("EnableWallpaperChanging");
            WallpapersFolderTextBlock.Text = Widget.LocaleManager.GetString("WallpapersFolder");
            RestartTextBlock.Text = Widget.LocaleManager.GetString("RestartText");
            ProviderChangedTextBlock.Text = Widget.LocaleManager.GetString("ProviderChangedText");
            ChangeBgCheckBox.Content = Widget.LocaleManager.GetString("ChangeBg");

            ResetButton.Content = Widget.LocaleManager.GetString("Reset");
            OkButton.Content = Widget.LocaleManager.GetString("OK");
            CancelButton.Content = Widget.LocaleManager.GetString("Cancel");
            ApplyButton.Content = Widget.LocaleManager.GetString("Apply");

            CurrentLocationTextBlock.Text = Widget.LocaleManager.GetString("CurrentLocation");
            EnterLocationTextBlock.Text = Widget.LocaleManager.GetString("TypeLocationText");
            DetectLocationCheckBox.Content = Widget.LocaleManager.GetString("DetectLocation");

            TaskbarIconCheckBox.IsChecked = Properties.Settings.Default.ShowIconOnTaskbar;
            WeatherCheckBox.IsChecked = Properties.Settings.Default.EnableWeather;
            WeatherAnimationCheckBox.IsChecked = Properties.Settings.Default.EnableWeatherAnimation;
            ShowForecastCheckBox.IsChecked = Properties.Settings.Default.ShowForecast;
            IntervalComboBox.Text = Properties.Settings.Default.Interval.ToString();
            EnableSoundsCheckBox.IsChecked = Properties.Settings.Default.EnableSounds;
            WallpaperChangingCheckBox.IsChecked = Properties.Settings.Default.EnableWallpaperChanging;
            ChangeBgCheckBox.IsChecked = Properties.Settings.Default.ChangeBg;

            LocationText.Text = widget.City.Text;

            Image1.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("icon.png")));

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            VersionString.Text = string.Format("Version {0}.{1} Build {2} R{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);

            //SkinPreview.Source = new BitmapImage(new Uri(E.Path + "\\WeatherClock\\Skins\\" + Widget.Sett.skin + "\\Preview.png"));

            var dirs = from x in Directory.GetDirectories(E.Path + "\\WeatherClock\\Skins")
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

            XDocument skin = XDocument.Load(E.Path + "\\WeatherClock\\Skins\\" + Properties.Settings.Default.Skin + "\\Skin.xml");

            SkinsComboBox.Text = skin.Root.Element("Name").Value;

            if (Properties.Settings.Default.DegreeType == 1)
            {
                Fahrenheit.IsChecked = true;
            }
            else
            {
                Celsius.IsChecked = true;
            }

            if (Properties.Settings.Default.TimeMode == 0)
            {
                TimeMode1.IsChecked = true;
            }
            else
            {
                TimeMode2.IsChecked = true;
            }

            if (Properties.Settings.Default.EnableWallpaperChanging)
                WallpapersFolderPanel.IsEnabled = true;
            else
                WallpapersFolderPanel.IsEnabled = false;

            if (widget.providers.Count > 0)
            {
                foreach (WeatherProvider p in widget.providers)
                {
                    /*
                    if (p.Name == "Microsoft.WindowsAPICodePack")
                    {
                        // Skip this technical dll!
                    }
                    else if (p.Name == "Microsoft.WindowsAPICodePack.Shell")
                    {
                        // Skip this technical dll!
                    }
                    else if (p.Name == "HTCHome.Core")
                    {
                        // Skip this technical dll!
                    }
                    else
                    {
                    */
                        // Add the weather provider (use p.Name as Prov. Name)
                        ProviderComboBox.Items.Add(p.Name);
                    /*}*/
                }
            }

            ProviderComboBox.SelectedIndex = widget.providers.IndexOf(widget.currentProvider);

            WallpapersFolderTextBox.Text = Properties.Settings.Default.WallpapersFolder;
            ApplyButton.IsEnabled = false;
            ProviderChangedTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LocationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(LocationBox.Text))
            {
                SelectedIndex = -1;
                string s = LocationBox.Text;
                ThreadStart threadStarter = delegate
                {
                    GetLocation(s);
                };
                Thread thread = new Thread(threadStarter);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void GetLocation(string l)
        {
            results.Clear();

            SearchProgress.Dispatcher.Invoke((Action)delegate
            {
                SearchProgress.Visibility = System.Windows.Visibility.Visible;
            }, null);

            SearchResults.Dispatcher.Invoke((Action)delegate
            {
                SearchResults.Children.Clear();
            }, null);

            List<CityLocation> locations = widget.currentProvider.GetLocation(l);
            
            if (locations != null && locations.Count > 0)
            {
                foreach (CityLocation location in locations)
                {
                    SearchResults.Dispatcher.Invoke
                    (
                        (Action)delegate
                        {
                            LocationItem item = new LocationItem();
                            item.Header = location.City;
                            item.Order = SearchResults.Children.Count;
                            item.MouseLeftButtonDown +=
                                new MouseButtonEventHandler(
                                    item_MouseLeftButtonDown);
                            SearchResults.Children.Add(item);
                        }, 
                        null
                    );

                    results.Add(location);
                }
            }
            else
            {
                SearchResults.Dispatcher.Invoke
                (
                    (Action)delegate
                    {
                        LocationItem item = new LocationItem();
                        item.Header = Widget.LocaleManager.GetString("NoResults");
                        item.IsEnabled = false;
                        SearchResults.Children.Add(item);
                    }, 
                    null
                );
            }

            SearchProgress.Dispatcher.Invoke
            (
                (Action)delegate
                {
                    SearchProgress.Visibility = System.Windows.Visibility.Hidden;
                }, 
                null
            );
        }

        void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedIndex = SearchResults.Children.IndexOf((LocationItem)sender);

            if (Properties.Settings.Default.LastCities == null)
            {
                Properties.Settings.Default.LastCities = new StringCollection();
            }


            // RnD
            LocationText.Text = results[SelectedIndex].City;

            if (!Properties.Settings.Default.LastCities.Contains
                (results[SelectedIndex].City)// + "#" + results[SelectedIndex].Code)
                ) 
            {
                Properties.Settings.Default.LastCities.Insert
                (
                    0, results[SelectedIndex].City// + "#" + results[SelectedIndex].Code
                );

                if (Properties.Settings.Default.LastCities.Count > 10)
                {
                    Properties.Settings.Default.LastCities.RemoveAt
                    (
                        Properties.Settings.Default.LastCities.Count - 1
                    );
                }
            }
        }//

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void IntervalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyButton.IsEnabled)
            {
                // Apply settings
                ApplySettings();
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Apply settings
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();

            ApplyButton.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = -1;
            string s = LocationBox.Text;
            ThreadStart threadStarter = delegate
            {
                GetLocation(s);
            };
            Thread thread = new Thread(threadStarter);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }


        // ApplySettings
        private void ApplySettings()
        {
            if (Properties.Settings.Default.WeatherProvider != ProviderComboBox.Text && Properties.Settings.Default.LastCities!= null)
                Properties.Settings.Default.LastCities.Clear();

            Properties.Settings.Default.ShowIconOnTaskbar = (bool)TaskbarIconCheckBox.IsChecked;
            Properties.Settings.Default.EnableWeather = (bool)WeatherCheckBox.IsChecked;
            Properties.Settings.Default.EnableWeatherAnimation = (bool)WeatherAnimationCheckBox.IsChecked;
            Properties.Settings.Default.Interval = Convert.ToInt32(IntervalComboBox.Text);
            Properties.Settings.Default.EnableWallpaperChanging = (bool)WallpaperChangingCheckBox.IsChecked;
            Properties.Settings.Default.ShowForecast = (bool)ShowForecastCheckBox.IsChecked;
            Properties.Settings.Default.WeatherProvider = ProviderComboBox.Text;
            Properties.Settings.Default.WallpapersFolder = WallpapersFolderTextBox.Text;
            Properties.Settings.Default.DegreeType = (bool)Fahrenheit.IsChecked ? 1 : 0;
            Properties.Settings.Default.TimeMode = (bool)TimeMode2.IsChecked ? 1 : 0;
            Properties.Settings.Default.EnableSounds = (bool)EnableSoundsCheckBox.IsChecked;
            Properties.Settings.Default.Skin = skins[SkinsComboBox.SelectedIndex];
            Properties.Settings.Default.ChangeBg = (bool)ChangeBgCheckBox.IsChecked;

            if (TaskbarIconCheckBox.IsChecked == true)
            {
                Widget.Parent.ShowInTaskbar = true;
                if (!string.IsNullOrEmpty(widget.weatherReport.NowSky))
                {
                    Widget.Parent.Title = widget.weatherReport.NowSky;
                }

                try
                {
                    Widget.Parent.Icon = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", widget.weatherReport.NowSkyCode))));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Options - Widget.weatherReport.NowSkyCode Exception: " + ex.Message);
                }
            }
            else
                Widget.Parent.ShowInTaskbar = false;

            foreach (WeatherProvider p in widget.providers)
            {
                if (p.Name == Properties.Settings.Default.WeatherProvider)
                {
                    if (!p.IsLoaded)
                        p.Load();

                    widget.currentProvider = p;
                }
            }

            Properties.Settings.Default.Save();

            widget.UpdateSettings();

            ProviderChangedTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        
        }//ApplySettings


        // SkinsComboBox_SelectionChanged
        private void SkinsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XDocument doc = XDocument.Load(E.Path + "\\WeatherClock\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Skin.xml");
            AuthorTextBlock.Text = Widget.LocaleManager.GetString("Author") + " " + doc.Root.Element("Author").Value;
            VersionTextBlock.Text = Widget.LocaleManager.GetString("Version") + " " + doc.Root.Element("Version").Value;
            ContactsTextBlock.Text = Widget.LocaleManager.GetString("Contacts") + " " + doc.Root.Element("Contacts").Value;
            DescriptionTextBlock.Text = doc.Root.Element("Description").Value;

            if (File.Exists(E.Path + "\\WeatherClock\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Preview.png"))
                SkinPreview.Source = new BitmapImage(new Uri(E.Path + "\\WeatherClock\\Skins\\" + skins[SkinsComboBox.SelectedIndex] + "\\Preview.png"));
            WeatherClock.UseClockAnimation = Convert.ToBoolean(doc.Root.Element("UseClockAnimation").Value);

            ApplyButton.IsEnabled = true;
        }

        private void ContactString_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WinAPI.ShellExecute(IntPtr.Zero, "open", ContactString.Text, string.Empty, string.Empty, 0);
        }

        private void ProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProviderChangedTextBlock.Visibility = System.Windows.Visibility.Visible;
            ApplyButton.IsEnabled = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            this.Close();
            widget.ShowOptions();
        }
    }
}
