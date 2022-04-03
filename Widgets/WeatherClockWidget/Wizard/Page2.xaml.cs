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
using System.IO;
using E = HTCHome.Core.Environment;
using System.Threading;
using WeatherClockWidget.Domain;

namespace WeatherClockWidget.Wizard
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : UserControl
    {

        private List<WeatherProvider> providers;
        private WeatherProvider currentProvider;

        private List<CityLocation> results = new List<CityLocation>();

        int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == _selectedIndex)
                    return;

                if (value > -1)
                {
                    ((LocationItem)SearchResults.Children[value]).Selected = true;
                    Properties.Settings.Default.LocationCode = results[value].Code;
                    NextButton.IsEnabled = true;
                }
                else
                {
                    //
                }

                if (_selectedIndex > -1 && _selectedIndex < SearchResults.Children.Count)
                    ((LocationItem)SearchResults.Children[_selectedIndex]).Selected = false;
                _selectedIndex = value;
            }
        }

        public Page2()
        {
            InitializeComponent();

            Title.Text = Widget.LocaleManager.GetString("Page2Title");
            String1.Text = Widget.LocaleManager.GetString("Page2String1");
            String2.Text = Widget.LocaleManager.GetString("Page2String2");

            NextButton.Content = Widget.LocaleManager.GetString("Next");
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetWeatherProviders();

            if (providers.Count > 0)
            {
                foreach (WeatherProvider p in providers)
                {
                    ProviderBox.Items.Add(p.Name);
                }
            }

            ProviderBox.SelectedIndex = providers.IndexOf(currentProvider);
        }

        private void GetWeatherProviders()
        {
            if (Directory.Exists(E.Path + "\\WeatherClock\\WeatherProviders"))
            {
                providers = new List<WeatherProvider>();
                var files = from x in Directory.GetFiles(E.Path + "\\WeatherClock\\WeatherProviders")
                            where x.EndsWith(".dll")
                            select x;
                foreach (var f in files)
                {
                    var p = new WeatherProvider(f);
                    providers.Add(p);
                    if (Properties.Settings.Default.WeatherProvider == p.Name)
                    {
                        currentProvider = p;
                        p.Load();
                    }
                }
            }
        }

        private void ProviderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (WeatherProvider p in providers)
            {
                if (p.Name == (string)ProviderBox.SelectedValue)
                {
                    if (!p.IsLoaded)
                        p.Load();
                    currentProvider = p;
                    Properties.Settings.Default.WeatherProvider = p.Name;
                }
            }
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

            List<CityLocation> locations = currentProvider.GetLocation(l);
            foreach (CityLocation location in locations)
            {
                SearchResults.Dispatcher.Invoke((Action)delegate
                {
                    LocationItem item = new LocationItem();
                    item.Header = location.City;
                    item.Order = SearchResults.Children.Count;
                    item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
                    SearchResults.Children.Add(item);
                }, null);
                results.Add(location);
            }

            SearchProgress.Dispatcher.Invoke((Action)delegate
            {
                SearchProgress.Visibility = System.Windows.Visibility.Hidden;
            }, null);
        }

        void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedIndex = SearchResults.Children.IndexOf((LocationItem)sender);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)Parent).Children.Add(new Page3());
            ((Grid)Parent).Children.Remove(this);
        }
    }
}
