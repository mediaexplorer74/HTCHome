using System;
using System.Collections.Generic;
using System.IO;
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
using HTCHome.Core;
using E = HTCHome.Core.Environment;

namespace ClockWidget
{
    /// <summary>
    /// Interaction logic for WidgetControl.xaml
    /// </summary>
    public partial class WidgetControl : UserControl
    {
        private DispatcherTimer _timer;
        private Options options;

        public WidgetControl()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            if (!Directory.Exists(E.Path + "\\Clock\\Skins\\" + Properties.Settings.Default.Skin))
                Properties.Settings.Default.Skin = "Day Extra";
            ClockBase.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_base.png")));

            ClockHour.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_hour.png")));
            ClockMinute.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_minute.png")));
            ClockSecond.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_second.png")));
            ClockDot.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_center_dot.png")));

            Skin.Source = new Uri(E.Path + "\\Clock\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");
        }

        public void ReloadSkin()
        {
            Widget.ResourceManager = new ResourceManager(E.Path + "\\Clock", Properties.Settings.Default.Skin);
            Skin.Source = new Uri(E.Path + "\\Clock\\Skins\\" + Properties.Settings.Default.Skin + "\\Layout.xaml");

            ClockBase.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_base.png")));

            ClockHour.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_hour.png")));
            ClockMinute.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_minute.png")));
            ClockSecond.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_second.png")));
            ClockDot.Source = new BitmapImage(new Uri(Widget.ResourceManager.GetResourcePath("clock_center_dot.png")));

            Widget.Instance.UpdateAero(this);
        }

        public void Load()
        {
            Initialize();

            HourRotate.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5);
            MinuteRotate.Angle = DateTime.Now.Minute * 6;
            SecondRotate.Angle = DateTime.Now.Second * 6;

            Day.Text = DateTime.Now.ToString("ddd").ToUpper();
            Month.Text = DateTime.Now.ToString("MMM").ToUpper() + DateTime.Now.Day.ToString();
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 12)
                AmPm.Text = "AM";
            else
                AmPm.Text = "PM";

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Start();

            var optionsItem = new MenuItem();
            optionsItem.Header = Widget.LocaleManager.GetString("Options");
            optionsItem.Click += OptionsItemClick;

            Widget.Parent.ContextMenu.Items.Insert(0, new Separator());
            Widget.Parent.ContextMenu.Items.Insert(0, optionsItem);

            options = new Options();
        }

        void OptionsItemClick(object sender, RoutedEventArgs e)
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options();

            if (E.Locale == "he-IL" || E.Locale == "ar-SA")
            {
                options.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                options.FlowDirection = FlowDirection.LeftToRight;
            }

            options.ShowDialog();
        }

        void TimerTick(object sender, EventArgs e)
        {
            HourRotate.Angle = (DateTime.Now.Hour * 30) + (DateTime.Now.Minute * 0.5);
            MinuteRotate.Angle = DateTime.Now.Minute * 6;
            SecondRotate.Angle = DateTime.Now.Second * 6;

            Day.Text = DateTime.Now.ToString("ddd").ToUpper();
            Month.Text = DateTime.Now.ToString("MMM").ToUpper() + DateTime.Now.Day.ToString();
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 12)
                AmPm.Text = "AM";
            else
                AmPm.Text = "PM";
        }

        public void UpdateSettings()
        {
            if (System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(Skin.Source.AbsolutePath)) != Properties.Settings.Default.Skin)
            {
                ReloadSkin();
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Widget.Instance.UpdateAero(this);
        }
    }
}
