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

namespace ClockWidget
{
    public class Widget : IWidget
    {
        public static Window Parent;
        private WidgetControl _widgetControl;
        public static LocaleManager LocaleManager;
        public static ResourceManager ResourceManager;
        public static Widget Instance;

        public Widget()
        {
            Instance = this;
        }

        public string GetWidgetName()
        {
            return "Clock widget";
        }

        public UserControl Load()
        {
            //Sett = Settings.Read(E.ConfigDirectory + "\\ClockWidget.conf");

            LocaleManager = new HTCHome.Core.LocaleManager(E.Path + "\\Clock\\Localization");
            LocaleManager.LoadLocale(E.Locale);
            ResourceManager = new HTCHome.Core.ResourceManager(E.Path + "\\Clock", Properties.Settings.Default.Skin);

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
            //Sett.Write(E.ConfigDirectory + "\\ClockWidget.conf");
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
            string uri = Widget.ResourceManager.GetResourcePath("Skin.xml");
            if (string.IsNullOrEmpty(uri))
                return IntPtr.Zero;
            XDocument doc = XDocument.Load(Widget.ResourceManager.GetResourcePath("Skin.xml"));
            int left = Convert.ToInt32(doc.Descendants("Left").First().Value);
            int top = Convert.ToInt32(doc.Descendants("Top").First().Value);
            int right = Convert.ToInt32(doc.Descendants("Right").First().Value);
            int bottom = Convert.ToInt32(doc.Descendants("Bottom").First().Value);

            if (left == -1 || top == -1 || right == -1 || bottom == -1)
                return IntPtr.Zero;
            else
            {
                double dpiX = 0.0f;
                double dpiY = 0.0f;
                PresentationSource source = PresentationSource.FromVisual(_widgetControl);
                if (source != null)
                {
                    dpiX = source.CompositionTarget.TransformToDevice.M11;
                    dpiY = source.CompositionTarget.TransformToDevice.M22;
                }
                return WinAPI.CreateEllipticRgn((int)(left * Properties.Settings.Default.ScaleFactor * dpiX), (int)(top * Properties.Settings.Default.ScaleFactor * dpiY),
                                                (int)(right * Properties.Settings.Default.ScaleFactor * dpiX), (int)(bottom * Properties.Settings.Default.ScaleFactor * dpiY));
            }
        }

        public void SetWindowPosition(double left, double top)
        {
            Properties.Settings.Default.Left = left;
            Properties.Settings.Default.Top = top;
        }

        public string GetIcon()
        {
            return E.Path + "\\Clock\\Resources\\icon.png";
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

        public void UpdateSettings()
        {
            _widgetControl.UpdateSettings();
        }

        public void UpdateAero(object sender)
        {
            UpdateAeroEvent(sender, EventArgs.Empty);
        }

        public event EventHandler UpdateAeroEvent;
    }
}
