using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTCHome.Core;
using System.Windows.Controls;
using System.Windows;
using E = HTCHome.Core.Environment;
using L = HTCHome.Core.Logger;

namespace CalendarWidget
{
    public class Widget : IWidget
    {
        public static Window Parent;
        public static LocaleManager LocaleManager;
        private UserControl _widgetControl;

        public string GetWidgetName()
        {
            return "Calendar widget";
        }

        public System.Windows.Controls.UserControl Load()
        {
            LocaleManager = new HTCHome.Core.LocaleManager(E.Path + "\\Calendar\\Localization");
            LocaleManager.LoadLocale(E.Locale);
            _widgetControl = new Calendar();
            return _widgetControl;
        }

        public System.Windows.Controls.UserControl GetWidgetControl()
        {
            return _widgetControl;
        }

        public void Unload()
        {
            L.Log("Calendar: Saving settings");
            Properties.Settings.Default.Save();
            L.Log("Calendar: Saving calendar data");
        }

        public void SetParent(System.Windows.Window window)
        {
            Parent = window;
        }

        public System.Windows.Point GetWindowPosition()
        {
            return new Point(Properties.Settings.Default.Left, Properties.Settings.Default.Top);
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
            return WinAPI.CreateRoundRectRgn(0, 20, (int)(350 * Properties.Settings.Default.ScaleFactor * dpiX), (int)(430 * Properties.Settings.Default.ScaleFactor * dpiY), 5, 5);
        }

        public void SetWindowPosition(double left, double top)
        {
            Properties.Settings.Default.Left = left;
            Properties.Settings.Default.Top = top;
        }

        public string GetIcon()
        {
            return E.Path + "\\Calendar\\Resources\\icon.png";
        }


        public void SetScalefactor(double scale)
        {
            Properties.Settings.Default.ScaleFactor = scale;
            ((Calendar)_widgetControl).Scale.ScaleX = scale;
        }

        public double GetOpacity()
        {
            return Properties.Settings.Default.Opacity;
        }

        public void SetOpacity(double opacity)
        {
            Properties.Settings.Default.Opacity = opacity;
        }


        public double GetScalefactor()
        {
            return Properties.Settings.Default.ScaleFactor;
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
    }
}
