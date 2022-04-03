using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using HTCHome.Core;
using E = HTCHome.Core.Environment;

namespace PhotoWidget
{
    public class Widget : IWidget
    {
        public Window Parent;
        private WidgetControl _widgetControl;
        public LocaleManager LocaleManager;
        public ResourceManager ResourceManager;
        public static Widget Instance;

        public Widget()
        {
            Instance = this;
        }

        public string GetWidgetName()
        {
            return "Photo widget";
        }

        public UserControl Load()
        {
            LocaleManager = new HTCHome.Core.LocaleManager(E.Path + "\\Photo\\Localization");
            LocaleManager.LoadLocale(E.Locale);

            _widgetControl = new WidgetControl();
            _widgetControl.Load();

            return _widgetControl;
        }

        public UserControl GetWidgetControl()
        {
            return _widgetControl;
        }

        public void Unload()
        {
            //_widgetControl.Unload();
            Properties.Settings.Default.Save();
        }

        public void SetParent(Window window)
        {
            Parent = window;
        }

        public Point GetWindowPosition()
        {
            return new Point(Properties.Settings.Default.Left, Properties.Settings.Default.Top);
        }

        public IntPtr GetRegion()
        {
            return IntPtr.Zero;
        }

        public void SetWindowPosition(double left, double top)
        {
            Properties.Settings.Default.Left = left;
            Properties.Settings.Default.Top = top;
        }

        public string GetIcon()
        {
            return E.Path + "\\Photo\\Resources\\icon.png";
        }

        public double GetScalefactor()
        {
            return Properties.Settings.Default.ScaleFactor;
        }

        public void SetScalefactor(double scale)
        {
            Properties.Settings.Default.ScaleFactor = scale;
            _widgetControl.Scale.ScaleX = scale;
            _widgetControl.Scale.ScaleY = scale;
        }

        public double GetOpacity()
        {
            return Properties.Settings.Default.Opacity;
        }

        public void SetOpacity(double opacity)
        {
            Properties.Settings.Default.Opacity = opacity;
        }

        public bool GetTopMost()
        {
            return Properties.Settings.Default.TopMost;
        }

        public void SetTopMost(bool value)
        {
            Properties.Settings.Default.TopMost = value;
        }

        public bool GetPin()
        {
            return Properties.Settings.Default.Pinned;
        }

        public void SetPin(bool value)
        {
            Properties.Settings.Default.Pinned = value;
        }

        public void UpdateSettings(bool rescan)
        {
            _widgetControl.UpdateSettings(rescan);
        }

        public void UpdateAero(object sender)
        {
            UpdateAeroEvent(sender, EventArgs.Empty);
        }

        public event EventHandler UpdateAeroEvent;
    }
}
