using System.Windows;
using System.Windows.Controls;
using HTCHome.Core;
using E = HTCHome.Core.Environment;
using System;

namespace NewsWidget
{
    public class Widget : IWidget
    {
        public Window Parent;
        private News _widgetControl;
        public static Widget Instance;
        public LocaleManager LocaleManager;

        public Widget()
        {
            Instance = this;
        }

        public UserControl GetWidgetControl()
        {
            return _widgetControl;
        }

        public string GetWidgetName()
        {
            return "News widget";
        }

        public UserControl Load()
        {
            LocaleManager = new HTCHome.Core.LocaleManager(E.Path + "\\News\\Localization");
            LocaleManager.LoadLocale(E.Locale);
            _widgetControl = new News();
            _widgetControl.Load();
            return _widgetControl;
        }

        public void SetParent(Window window)
        {
            Parent = window;
        }

        public Point GetWindowPosition()
        {
            return new Point(Properties.Settings.Default.Left, Properties.Settings.Default.Top);
        }

        public void SetWindowPosition(double left, double top)
        {
            Properties.Settings.Default.Left = left;
            Properties.Settings.Default.Top = top;
        }

        public void Unload()
        {
            Properties.Settings.Default.Save();
        }


        public IntPtr GetRegion()
        {
            double dpiX = 0.0f;
            double dpiY = 0.0f;
            PresentationSource source = PresentationSource.FromVisual(_widgetControl);
            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }
            return WinAPI.CreateRoundRectRgn(0, (int)(20 * dpiY), (int)(320 * Properties.Settings.Default.ScaleFactor * dpiX), (int)(400 * Properties.Settings.Default.ScaleFactor * dpiY), 5, 5);
        }

        public double GetScalefactor()
        {
            return Properties.Settings.Default.ScaleFactor;
        }


        public string GetIcon()
        {
            return E.Path + "\\News\\Resources\\icon.png";
        }


        public void SetScalefactor(double scale)
        {
            Properties.Settings.Default.ScaleFactor = scale;
            ((News)_widgetControl).Scale.ScaleX = scale;
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

        public event EventHandler UpdateAeroEvent;


        public event EventHandler UpdateAero;
    }
}