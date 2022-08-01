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
using System.Windows.Threading;

using WeatherClockWidget.Domain;

using E = HTCHome.Core.Environment;
using System.IO;
using HTCHome.Core;
using System.Threading;
using WeatherClockWidget.WeatherAnimation;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.Diagnostics;

namespace WeatherClockWidget
{
    /// <summary>
    /// Interaction logic for WeatherClock.xaml
    /// </summary>
    public partial class WeatherClock : UserControl
    {
        private DispatcherTimer timer;
        private DispatcherTimer weatherTimer;
        private bool firstFlip;
        private int lastMinute, lastHour;

        public List<WeatherProvider> providers;
        public WeatherProvider currentProvider;
        public WeatherReport weatherReport;

        private Options options;

        public static bool UseClockAnimation = true;
        private WallpaperManager _wallpaperManager;

        public static MediaPlayer mediaPlayer;
        private MenuItem lastCitiesItem;

        public WeatherClock()
        {
            InitializeComponent();

            Initialize();
        }

        // Initialize
        private void Initialize()
        {
            if (!File.Exists(E.Path + "\\WeatherClock\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml"))
            {
                Properties.Settings.Default.Skin = "Sense";
                Widget.ResourceManager = new ResourceManager(E.Path + "\\WeatherClock", Properties.Settings.Default.Skin);
            }
            Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("base_default.png")));
            ForecastBg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("forecast_base_default.png")));

            FrostLeft.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\frost_left.png")));
            FrostRight.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\frost_right.png")));

            DateTime d = DateTime.Now.AddHours(-1).AddMinutes(-2);
            if (Properties.Settings.Default.TimeMode == 1)
            {
                int h = Convert.ToInt32(d.ToString("hh"));
                Hours.Initialize(h);
            }
            else
                Hours.Initialize(d.Hour);

            Minutes.Initialize(d.Minute);

            Skin.Source = new Uri(E.Path + "\\WeatherClock\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");
        
        }//Initialize

        public void ReloadSkin()
        {
            Skin.Source = new Uri(E.Path + "\\WeatherClock\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");
            Widget.ResourceManager = new ResourceManager(E.Path + "\\WeatherClock", Properties.Settings.Default.Skin);
            //switch bg
            if (Properties.Settings.Default.ChangeBg)
            {
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                                     weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\bg_20.png")))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png", 20))));
                }
                else
                    Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("base_default.png")));


                //switch forecast bg
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\forecast_bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format(
                                    "Backgrounds\\forecast_bg_{0}.png", weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\forecast_bg_20.png")))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(
                                    string.Format("Backgrounds\\forecast_bg_{0}.png", 20))));
                }
                else
                    ForecastBg.Source =
                        new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("forecast_base_default.png")));
            }
            else
            {
                Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("base_default.png")));
                ForecastBg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("forecast_base_default.png")));
            }

            //switch overlay
            if (!string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))));
            }
            else if (weatherReport.NowTemp <= -2 && !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Overlays\\o_20.png")))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", 20))));
            }
            else
                Overlay.Source = null;

            FrostLeft.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\frost_left.png")));
            FrostRight.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("Weather\\frost_right.png")));

            if (Properties.Settings.Default.TimeMode == 1)
            {
                int h = Convert.ToInt32(DateTime.Now.ToString("hh"));
                Hours.Initialize(h);
            }
            else
                Hours.Initialize(DateTime.Now.Hour);
            Minutes.Initialize(DateTime.Now.Minute);

            Widget.Instance.UpdateAero(this);
        
        }//ReloadSkin


        // Load
        public void Load()
        {
            weatherReport = WeatherReport.Read(E.Path + "\\WeatherClock\\Weather.data");

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);

            weatherTimer = new DispatcherTimer();
            weatherTimer.Interval = TimeSpan.FromMinutes(Properties.Settings.Default.Interval);
            weatherTimer.Tick += weatherTimer_Tick;

            Minutes.HalfFlip += Minutes_HalfFlip;
            Hours.HalfFlip += Hours_HalfFlip;

            Date.Text = DateTime.Now.ToString((string)Skin["MainDateFormat"]);

            City.Text = weatherReport.Location;
            Weather.Text = weatherReport.NowSky;
            Temperature.Text = weatherReport.NowTemp.ToString() + "°";

            if (Properties.Settings.Default.ShowIconOnTaskbar)
                Widget.Parent.ShowInTaskbar = true;

            if (Properties.Settings.Default.ChangeBg)
            {
                //switch bg
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                                     weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\bg_20.png")))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png", 20))));
                }
                else
                    Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("base_default.png")));

                //switch forecast bg
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\forecast_bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format(
                                    "Backgrounds\\forecast_bg_{0}.png", weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\forecast_bg_20.png")))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(
                                    string.Format("Backgrounds\\forecast_bg_{0}.png", 20))));
                }
                else
                    ForecastBg.Source =
                        new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("forecast_base_default.png")));
            }

            //switch overlay
            if (!string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))));
            }
            else if (weatherReport.NowTemp <= -2 && !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Overlays\\o_20.png")))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", 20))));
            }
            else
                Overlay.Source = null;

            WeatherIcon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", weatherReport.NowSkyCode))));
            if (Properties.Settings.Default.ShowIconOnTaskbar)
            {
                if (!string.IsNullOrEmpty(weatherReport.NowSky))
                    Widget.Parent.Title = weatherReport.NowSky;
                Widget.Parent.Icon = WeatherIcon.Source;
            }

            if (Properties.Settings.Default.ShowIconOnTaskbar && Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
            {
                System.Drawing.Icon oicon = DrawIcon(weatherReport.NowTemp); //System.Drawing.Icon.FromHandle(bitmap.GetHicon());
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetOverlayIcon(Widget.Parent, oicon, "test");
                //Widget.Parent.Icon = MakeIcon(15, 2);
            }

            for (int i = 0; i < ForecastPanel.Children.Count; i++)
            {
                var item = (ForecastItem)ForecastPanel.Children[i];
                item.Initialize();
                item.Day.Text = DateTime.Today.AddDays(i).ToString((string)Skin["ForecastDateFormat"]);
                if (weatherReport.Forecast != null && weatherReport.Forecast.Count == 5)
                {
                    item.Icon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", weatherReport.Forecast[i].SkyCode))));
                    item.TemperatureH.Text = weatherReport.Forecast[i].HighTemperature.ToString() + "°";
                    item.TemperatureL.Text = " " + weatherReport.Forecast[i].LowTemperature.ToString() + "°";
                    item.Url = weatherReport.Forecast[i].Url;
                }
            }

            FItem1.Day.Text = Widget.LocaleManager.GetString("Today");
            FItem2.Day.Text = Widget.LocaleManager.GetString("Tomorrow");


            ForecastGrid.Visibility = Properties.Settings.Default.ShowForecast ? Visibility.Visible : Visibility.Collapsed;

            GetWeatherProviders();

            var optionsItem = new MenuItem();
            optionsItem.Header = Widget.LocaleManager.GetString("Options");
            optionsItem.Click += optionsItem_Click;

            var refreshItem = new MenuItem();
            refreshItem.Header = Widget.LocaleManager.GetString("Refresh");
            refreshItem.Click += new RoutedEventHandler(refreshItem_Click);

            var showForecastItem = new MenuItem();
            showForecastItem.Header = Widget.LocaleManager.GetString("ShowForecast");
            showForecastItem.IsChecked = Properties.Settings.Default.ShowForecast;
            showForecastItem.IsCheckable = true;
            showForecastItem.Checked += new RoutedEventHandler(showForecastItem_Checked);
            showForecastItem.Unchecked += new RoutedEventHandler(showForecastItem_Unchecked);

            lastCitiesItem = new MenuItem();
            lastCitiesItem.Header = Widget.LocaleManager.GetString("LastResults");
            lastCitiesItem.IsEnabled = false;
            Widget.Parent.ContextMenu.Opened += new RoutedEventHandler(ContextMenu_Opened);

            Widget.Parent.ContextMenu.Items.Insert(0, new Separator());
            Widget.Parent.ContextMenu.Items.Insert(0, showForecastItem);
            Widget.Parent.ContextMenu.Items.Insert(0, optionsItem);
            Widget.Parent.ContextMenu.Items.Insert(0, lastCitiesItem);
            Widget.Parent.ContextMenu.Items.Insert(0, refreshItem);

            if (Properties.Settings.Default.Debug)
            {
                var demoItem = new MenuItem { Header = "Demo" };

                var rainDemo = new MenuItem { Header = "Rain" };
                rainDemo.Click += new RoutedEventHandler(rainDemo_Click);

                var snowDemo = new MenuItem { Header = "Snow" };
                snowDemo.Click += new RoutedEventHandler(snowDemo_Click);

                var cloudsDemo = new MenuItem { Header = "Clouds" };
                cloudsDemo.Click += new RoutedEventHandler(cloudsDemo_Click);

                var lightningDemo = new MenuItem { Header = "Lightning" };
                lightningDemo.Click += new RoutedEventHandler(lightningDemo_Click);

                var frostDemo = new MenuItem { Header = "Cold" };
                frostDemo.Click += new RoutedEventHandler(frostDemo_Click);

                demoItem.Items.Add(rainDemo);
                demoItem.Items.Add(snowDemo);
                demoItem.Items.Add(cloudsDemo);
                demoItem.Items.Add(lightningDemo);
                demoItem.Items.Add(frostDemo);

                Widget.Parent.ContextMenu.Items.Insert(0, demoItem);
            }

            Hours.ShowAmPm = Properties.Settings.Default.TimeMode == 1;

            Scale.ScaleX = Properties.Settings.Default.ScaleFactor;

            options = new Options(this);

            XDocument doc =
                XDocument.Load(E.Path + "\\WeatherClock\\Skins\\" + Properties.Settings.Default.Skin + "\\Skin.xml");
            WeatherClock.UseClockAnimation = Convert.ToBoolean(doc.Root.Element("UseClockAnimation").Value);

            if (UseClockAnimation)
            {
                FirstFlip();
            }
            else
            {
                lastMinute = 0;
                Minutes.Flip(DateTime.Now.Minute, Properties.Settings.Default.TimeMode, UseClockAnimation);
                timer.Start();
                weatherTimer.Start();
                weatherTimer_Tick(null, EventArgs.Empty);
            }

            _wallpaperManager = new WallpaperManager();
            if (string.IsNullOrEmpty(Properties.Settings.Default.WallpapersFolder))
            {
                Properties.Settings.Default.WallpapersFolder = E.Root + "\\Wallpapers";
            }

            _wallpaperManager.Scan(Properties.Settings.Default.WallpapersFolder);

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Volume = 1;
        }

        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            lastCitiesItem.Items.Clear();
            if (Properties.Settings.Default.LastCities != null && Properties.Settings.Default.LastCities.Count > 0)
            {
                lastCitiesItem.IsEnabled = true;
                lastCitiesItem.Header = Widget.LocaleManager.GetString("LastResults");
                foreach (string city in Properties.Settings.Default.LastCities)
                {
                    var subItem = new MenuItem();
                    string c = city.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    subItem.Header = c;
                    subItem.Click += new RoutedEventHandler(subItem_Click);
                    lastCitiesItem.Items.Add(subItem);
                }
            }
            else
            {
                lastCitiesItem.IsEnabled = false;
            }

        }//ContextMenu_Opened


        // subItem_Click
        void subItem_Click(object sender, RoutedEventArgs e)
        {
            int index = lastCitiesItem.Items.IndexOf(sender);
            if (index > -1)
            {
                string c = Properties.Settings.Default.LastCities[index].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries)[1];
                Properties.Settings.Default.LocationCode = c;
                refreshItem_Click(sender, e);
                //MessageBox.Show(Widget.Sett.LastCities[index].City);
            }
        }


        void showForecastItem_Unchecked(object sender, RoutedEventArgs e)
        {
            ForecastGrid.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.ShowForecast = false;
        }

        void showForecastItem_Checked(object sender, RoutedEventArgs e)
        {
            ForecastGrid.Visibility = Visibility.Visible;
            Properties.Settings.Default.ShowForecast = true;
        }

        private System.Drawing.Icon MakeIcon(int degrees, int skycode)
        {
            System.Drawing.Icon oIcon = null;
            try
            {
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", skycode)));
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)bm);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Font oFont = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                if (degrees < 10 && degrees > -10)
                {
                    g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 3, 2);
                }
                else
                {
                    g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 1, 2);
                }
                oIcon = System.Drawing.Icon.FromHandle(bm.GetHicon());
                oFont.Dispose();
                g.Dispose();
                bm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.InnerException.ToString());
            }

            return oIcon;

        }

        private System.Drawing.Icon DrawIcon(int degrees)
        {

            System.Drawing.Icon oIcon = null;
            try
            {
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(Widget.ResourceManager.GetResourcePath("Weather\\overlay_icon.png"));
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)bm);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Font oFont = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                switch (degrees.ToString().Length)
                {
                    case (1):
                        g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 8, 0);
                        break;
                    case (2):
                        g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 3, 0);
                        break;
                    case (3):
                        g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), -2, 0);
                        break;
                }
                /*if (degrees < 10 && degrees > -10)
                {
                    g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0);
                }
                else
                {
                    g.DrawString(degrees.ToString(), oFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), 3, 0);
                }*/
                oIcon = System.Drawing.Icon.FromHandle(bm.GetHicon());
                oFont.Dispose();
                g.Dispose();
                bm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return oIcon;

        }

        void frostDemo_Click(object sender, RoutedEventArgs e)
        {
            WeatherAnimationCanvas.Children.Clear();
            StartFrostAnimation();
        }

        void lightningDemo_Click(object sender, RoutedEventArgs e)
        {
            WeatherAnimationCanvas.Children.Clear();
            StartLightningAnimation();
            StartRainAnimation();
        }

        void cloudsDemo_Click(object sender, RoutedEventArgs e)
        {
            WeatherAnimationCanvas.Children.Clear();
            StartCloudAnimation();
        }

        void snowDemo_Click(object sender, RoutedEventArgs e)
        {
            WeatherAnimationCanvas.Children.Clear();
            StartSnowAnimation();
        }

        void rainDemo_Click(object sender, RoutedEventArgs e)
        {
            WeatherAnimationCanvas.Children.Clear();
            StartRainAnimation();
        }

        void refreshItem_Click(object sender, RoutedEventArgs e)
        {
            weatherTimer_Tick(null, EventArgs.Empty);
        }

        // GetWeatherProviders
        private void GetWeatherProviders()
        {
            if (Directory.Exists(E.ExtensionsPath + "\\Weather"))
            {
                providers = new List<WeatherProvider>();

                var files = from x in Directory.GetFiles(E.ExtensionsPath + "\\Weather")
                            where x.EndsWith(".dll")
                            select x;

                foreach (var f in files)
                {
                    WeatherProvider p = new WeatherProvider(f);

                    if (p.Name == "Microsoft.WindowsAPICodePack")
                    { 
                        // skip 
                    }
                    else if (p.Name == "Microsoft.WindowsAPICodePack.Shell")
                    {
                        // skip 
                    }
                    else if (p.Name == "HTCHome.Core")
                    {
                        // skip 
                    }
                    else
                    {
                        // add weather provider to list
                        providers.Add(p);

                        // check if default weather provider (w. p.)
                        if (Properties.Settings.Default.WeatherProvider == p.Name)
                        {
                            // set default w. p.
                            currentProvider = p;
                            p.Load();
                        }
                    }
                }
            }//if...

        }//GetWeatherProviders



        // UpdateSettings
        public void UpdateSettings()
        {
            Scale.ScaleX = Properties.Settings.Default.ScaleFactor;
            Scale.ScaleY = Scale.ScaleX;

            if (Convert.ToBoolean(Properties.Settings.Default.TimeMode) != Hours.ShowAmPm)
                FirstFlip();
            ForecastGrid.Visibility = Properties.Settings.Default.ShowForecast ? Visibility.Visible : Visibility.Collapsed;

            if (E.Locale != Widget.LocaleManager.LocaleCode)
            {
                Widget.LocaleManager.LoadLocale(E.Locale);
            }

            if (System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(Skin.Source.AbsolutePath)) != Properties.Settings.Default.Skin)
            {
                ReloadSkin();
            }

        }//UpdateSettings


        // timer_Tick
        private void timer_Tick(object sender, EventArgs e)
        {
            if (!firstFlip)
            {
                if (DateTime.Now.Minute != lastMinute)
                {
                    Minutes.Flip(DateTime.Now.Minute, UseClockAnimation);
                }

                lastMinute = DateTime.Now.Minute;
                Date.Text = DateTime.Now.ToString((string)Skin["MainDateFormat"]);
            }

            if (DateTime.Now.Hour != lastHour)
            {
                for (int i = 0; i < ForecastPanel.Children.Count; i++)
                {
                    ForecastItem item = (ForecastItem)ForecastPanel.Children[i];
                    item.Initialize();
                    item.Day.Text = DateTime.Today.AddDays(i).ToString((string)Skin["ForecastDateFormat"]);
                    if (weatherReport.Forecast != null && weatherReport.Forecast.Count == 5)
                    {
                        item.Icon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", weatherReport.Forecast[i].SkyCode))));
                        item.TemperatureH.Text = weatherReport.Forecast[i].HighTemperature.ToString() + "°";
                        item.TemperatureL.Text = " " + weatherReport.Forecast[i].LowTemperature.ToString() + "°";
                        item.Url = weatherReport.Forecast[i].Url;
                    }
                }

                FItem1.Day.Text = Widget.LocaleManager.GetString("Today");
                FItem2.Day.Text = Widget.LocaleManager.GetString("Tomorrow");
            }
        }

        private void FirstFlip()
        {
            if (UseClockAnimation)
            {
                firstFlip = true;
                ((Storyboard)Minutes.Resources["FlipAnim"]).BeginTime = TimeSpan.FromMilliseconds(400);
                Minutes.Flip(DateTime.Now.AddMinutes(-1).Minute, UseClockAnimation);
                ((Storyboard)Minutes.Resources["FlipAnim"]).BeginTime = TimeSpan.Zero;
            }
            else
            {
                Minutes_HalfFlip();
                Hours_HalfFlip();
            }
        }

        private void Minutes_HalfFlip()
        {
            if (DateTime.Now.Hour != lastHour || firstFlip)
            {
                if (Properties.Settings.Default.TimeMode == 1)
                {
                    int m;
                    if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 23)
                        m = 1;
                    else
                        m = 0;
                    int h = Convert.ToInt32(DateTime.Now.ToString("hh"));
                    if (h == 12 && m == 0)
                        h = 12;
                    Hours.Flip(h, m, UseClockAnimation);
                }
                else
                    Hours.Flip(DateTime.Now.Hour, UseClockAnimation);
            }
        }

        private void Hours_HalfFlip()
        {
            if (firstFlip)
            {
                Minutes.Flip(DateTime.Now.Minute, UseClockAnimation);
                firstFlip = false;

                timer.Start();
                weatherTimer.Start();
                weatherTimer_Tick(null, EventArgs.Empty);
            }

            lastHour = DateTime.Now.Hour;
            lastMinute = DateTime.Now.Minute;

            if (Convert.ToBoolean(Properties.Settings.Default.TimeMode) != Hours.ShowAmPm)
                Hours.ShowAmPm = Convert.ToBoolean(Properties.Settings.Default.TimeMode);
        }

        public void weatherTimer_Tick(object sender, EventArgs e)
        {
            ThreadStart threadStarter = RefreshWeather;
            var thread = new Thread(threadStarter);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void RefreshWeather()
        {
            try
            {
                GetWeatherReport();

                if (weatherReport != null)
                    WeatherPanel.Dispatcher.Invoke((Action)UpdateWeatherData, null);
                ForecastPanel.Dispatcher.Invoke((Action)UpdateForecastData, null);
            }
            catch (Exception ex)
            {
                WeatherPanel.Dispatcher.Invoke((Action)RefreshWeatherFail, null);
                HTCHome.Core.Logger.Log(ex.ToString());
            }
        }

        private void GetWeatherReport()
        {
            WeatherReport temp = null;
            if (Properties.Settings.Default.LocationCode != string.Empty)
            {
                try
                {
                    temp =
                        currentProvider.GetWeatherReport
                        (
                            E.Locale, 
                            Properties.Settings.Default.LocationCode,
                            Properties.Settings.Default.DegreeType
                        );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("currentProvider.GetWeatherReport (loc. code) Exception: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    temp = currentProvider.GetWeatherReport
                        (
                            E.Locale, 
                            string.Empty, 
                            Properties.Settings.Default.DegreeType
                        );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("currentProvider.GetWeatherReport (Empty loc. code) Exception: " + ex.Message);
                }
            }

            if (temp != null)
            {
                weatherReport = temp;
            }

        }

        private void UpdateWeatherData()
        {
            if (!string.IsNullOrEmpty(weatherReport.Location))
            {
                if (weatherReport.Location.Contains(","))
                    weatherReport.Location = weatherReport.Location.Substring(0, weatherReport.Location.IndexOf(","));

                City.Text = weatherReport.Location;

                FlipWeatherIcon();

                if (Properties.Settings.Default.ShowIconOnTaskbar && Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
                {
                    System.Drawing.Icon oicon = DrawIcon(weatherReport.NowTemp);
                    Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetOverlayIcon(Widget.Parent, oicon, "");
                }

                SetWeatherState(weatherReport.NowSkyCode);
            }

            Temperature.Text = weatherReport.NowTemp + "°";

            if (!string.IsNullOrEmpty(weatherReport.NowSky))
                Weather.Text = weatherReport.NowSky;
        }

        private void UpdateForecastData()
        {
            for (int i = 0; i < ForecastPanel.Children.Count; i++)
            {
                ForecastItem item = (ForecastItem)ForecastPanel.Children[i];
                if (weatherReport.Forecast != null && weatherReport.Forecast.Count == 5)
                {
                    item.FlipWeather(weatherReport.Forecast[i].SkyCode);
                    item.TemperatureH.Text = weatherReport.Forecast[i].HighTemperature.ToString() + "°";
                    item.TemperatureL.Text = " " + weatherReport.Forecast[i].LowTemperature.ToString() + "°";
                    item.Url = weatherReport.Forecast[i].Url;
                    item.ToolTip = weatherReport.Forecast[i].Text;
                }
            }
        }

        private void RefreshWeatherFail()
        {
            //MessageBox.Show("Fail");
        }

        private void FlipWeatherIcon()
        {
            WeatherIconBg.Source = WeatherIcon.Source;
            WeatherIconBg.Opacity = 1;
            WeatherIcon.Opacity = 0;
            WeatherIcon.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Weather\\weather_{0}.png", weatherReport.NowSkyCode))));
            if (Properties.Settings.Default.ChangeBg)
            {
                //switch bg
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png",
                                                                                     weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\bg_20.png")))
                {
                    Bg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\bg_{0}.png", 20))));
                }
                else
                    Bg.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("base_default.png")));

                //switch forecast bg
                if (
                    !string.IsNullOrEmpty(
                        Widget.ResourceManager.GetResourcePath(string.Format("Backgrounds\\forecast_bg_{0}.png",
                                                                             weatherReport.NowSkyCode))))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(string.Format(
                                    "Backgrounds\\forecast_bg_{0}.png", weatherReport.NowSkyCode))));
                }
                else if (weatherReport.NowTemp <= -2 &&
                         !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Backgrounds\\forecast_bg_20.png")))
                {
                    ForecastBg.Source =
                        new BitmapImage(
                            new Uri(
                                Widget.ResourceManager.GetResourcePath(
                                    string.Format("Backgrounds\\forecast_bg_{0}.png", 20))));
                }
                else
                    ForecastBg.Source =
                        new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("forecast_base_default.png")));
            }

            //switch overlay
            if (!string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", weatherReport.NowSkyCode))));
            }
            else if (weatherReport.NowTemp <= -2 && !string.IsNullOrEmpty(Widget.ResourceManager.GetResourcePath("Overlays\\o_20.png")))
            {
                Overlay.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath(string.Format("Overlays\\o_{0}.png", 20))));
            }
            else
                Overlay.Source = null;

            ((Storyboard)WeatherIconGrid.Resources["Flip"]).Begin();
            if (Properties.Settings.Default.ShowIconOnTaskbar)
            {
                if (!string.IsNullOrEmpty(weatherReport.NowSky))
                    Widget.Parent.Title = weatherReport.NowSky;
                Widget.Parent.Icon = WeatherIcon.Source;
            }
        }

        public void icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1 && !string.IsNullOrEmpty(weatherReport.Url))
                WinAPI.ShellExecute(IntPtr.Zero, "open", weatherReport.Url, "", "", 0);
        }

        void optionsItem_Click(object sender, RoutedEventArgs e)
        {
            ShowOptions();
        }

        public void ShowOptions()
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options(this);

            if (E.Locale == "he-IL" || E.Locale == "ar-SA")
            {
                options.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                options.FlowDirection = FlowDirection.LeftToRight;
            }

            try
            {
                options.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WeatherClock Options ShowDialog Exception: " + ex.Message);
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                /*IntPtr handle = WinAPI.GetDesktopWindow();
                WinAPI.SystemParametersInfo(WinAPI.SPI_SETDESKWALLPAPER, 0, string.Empty, WinAPI.SPIF_UPDATEINIFILE);*/
                WeatherAnimationCanvas.Children.Clear();
                //StartLightningAnimation();
                //StartRainAnimation();

                //StartCloudAnimation();
                //StartFrostAnimation();
                //StartSnowAnimation();
            }


            /*if (WeatherIconGrid.Children.Count > 0)
            {
                ((WeatherIcons.IWeatherIcon)WeatherIconGrid.Children[0]).Unload();
            }

            UserControl a = (UserControl)Application.LoadComponent(new Uri("/WeatherClock;component/WeatherIcons/Weather02.xaml", UriKind.Relative));
            WeatherIconGrid.Children.Add(a);*/
        }

        private void SetWeatherState(int weather)
        {
            WeatherAnimationCanvas.Children.Clear();

            switch (weather)
            {
                case 38:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartCloudAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Cloudy);
                    break;
                case 6:
                case 8:
                case 3:
                case 7:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartCloudAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Cloudy);
                    break;
                case 11:
                    //StartFogAnimation();
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Misty);
                    break;
                case 12:
                case 13:
                case 14:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartRainAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Rainy);
                    break;

                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartSnowAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Snowy);
                    break;

                case 32:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        //StartWindAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Windy);
                    break;
                case 18:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartRainAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Rainy);
                    break;

                case 15:
                case 16:
                case 17:
                    if (Properties.Settings.Default.EnableWeather && Properties.Settings.Default.EnableWeatherAnimation)
                    {
                        StartLightningAnimation();
                        StartRainAnimation();
                    }
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Stormy);
                    break;
                default:
                    if (Properties.Settings.Default.EnableWallpaperChanging)
                        _wallpaperManager.ChangeWallpaper(WallpaperManager.WeatherType.Sunny);
                    break;
            }

            if (weatherReport.NowTemp <= -2 && Properties.Settings.Default.EnableWeatherAnimation)
                StartFrostAnimation();
        }

        private void StartCloudAnimation()
        {
            for (int i = 0; i < 5; i++)
            {
                Cloud c = new Cloud();
                c.Initialize(i);
                WeatherAnimationCanvas.Children.Add(c);
            }
        }

        private void StartRainAnimation()
        {
            for (int i = 0; i < 30; i++)
            {
                Raindrop r = new Raindrop();
                r.Initialize(i, 2500 + i * 100);
                WeatherAnimationCanvas.Children.Add(r);
            }

            for (int i = 0; i < 4; i++)
            {
                Raindrop2 r2 = new Raindrop2();
                r2.Initialize(i);
                WeatherAnimationCanvas.Children.Add(r2);
            }

            /*if (Widget.Sett.useFullscreenAnimation)
            {
                for (int i = 0; i < 10; i++)
                {
                    FullscreenAnimation.Raindrop r = new FullscreenAnimation.Raindrop();
                    r.Initialize(i);
                    r.Show();
                }
            }*/

            if (Properties.Settings.Default.Top < 10)
            {
                RainWiper wiper = new RainWiper();
                wiper.Initialize();
                WeatherAnimationCanvas.Children.Add(wiper);
            }

            RainCloud c1 = new RainCloud();
            c1.Initialize(0);
            RainCloud c2 = new RainCloud();
            c2.Initialize(1);
            WeatherAnimationCanvas.Children.Add(c1);
            WeatherAnimationCanvas.Children.Add(c2);

            if (Properties.Settings.Default.EnableSounds)
                c1.PlayRainSound();
        }

        private void StartLightningAnimation()
        {
            Lightning l1 = new Lightning();
            l1.Initialize(1, 0, ref LightningBg1);
            Lightning l2 = new Lightning();
            l2.Initialize(2, 1, ref LightningBg2);
            WeatherAnimationCanvas.Children.Add(l1);
            WeatherAnimationCanvas.Children.Add(l2);

            if (Properties.Settings.Default.EnableSounds)
            {
                l1.PlayLightningSound();
                l2.PlayLightningSound();
            }
        }

        private void StartSnowAnimation()
        {

            for (int i = 0; i < 30; i++)
            {
                Snowflake s = new Snowflake();
                s.Initialize(i, 2500 + i * 100);
                WeatherAnimationCanvas.Children.Add(s);
            }

            RainCloud c1 = new RainCloud();
            c1.Initialize(0);
            RainCloud c2 = new RainCloud();
            c2.Initialize(1);
            WeatherAnimationCanvas.Children.Add(c1);
            WeatherAnimationCanvas.Children.Add(c2);

            if (Properties.Settings.Default.EnableSounds)
                c1.PlaySnowSound();
        }

        private void StartFrostAnimation()
        {
            Icicle i1 = new Icicle();
            i1.Initialize(3);

            i1.Style = (Style)Skin["Icicle1Style"];

            Icicle i2 = new Icicle();
            i2.Initialize(2);
            i2.Style = (Style)Skin["Icicle2Style"];

            Icicle i3 = new Icicle();
            i3.Initialize(1);
            i3.Style = (Style)Skin["Icicle3Style"];

            WeatherAnimationCanvas.Children.Add(i1);
            WeatherAnimationCanvas.Children.Add(i2);
            WeatherAnimationCanvas.Children.Add(i3);

            Storyboard s = (Storyboard)FrostBg.Resources["FadeIn"];
            s.Begin();

            if (Properties.Settings.Default.EnableSounds)
            {
                mediaPlayer.Open(new Uri(Widget.ResourceManager.GetResourcePath("Sounds\\Cold.wav")));
                mediaPlayer.Play();
            }
        }

        public void Unload()
        {
            if (weatherReport != null)
            {
                weatherReport.Write(E.Path + "\\WeatherClock\\Weather.data");
            }
        }

        private void FrostFadeInAnimation_Completed(object sender, EventArgs e)
        {
            Storyboard s = (Storyboard)FrostBg.Resources["FadeOut"];
            s.Begin();
        }
    }
}
