using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E = HTCHome.Core.Environment;
using HTCHome.Core;
using System.Windows;
using MediaPlayerWidget.Domain;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MediaPlayerWidget
{
    public class Widget : IWidget
    {
        public static Window Parent;
        private MediaPlayer _widgetControl;
        public static ResourceManager ResourceManager;
        public static LocaleManager LocaleManager;

        public static Widget Instance;

        public Widget()
        {
            Instance = this;
        }

        public string GetWidgetName()
        {
            return "Music widget";
        }

        public System.Windows.Controls.UserControl Load()
        {
            LocaleManager = new HTCHome.Core.LocaleManager(E.Path + "\\Music\\Localization");
            LocaleManager.LoadLocale(E.Locale);
            ResourceManager = new HTCHome.Core.ResourceManager(E.Path + "\\Music", Properties.Settings.Default.Skin);

            _widgetControl = new MediaPlayer();
            return _widgetControl;
        }

        public System.Windows.Controls.UserControl GetWidgetControl()
        {
            return _widgetControl;
        }

        public void Unload()
        {
            Properties.Settings.Default.Save();
            _widgetControl.controller.Shutdown();
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
            XDocument doc = XDocument.Load(Widget.ResourceManager.GetResourcePath("Skin.xml"));
            int left = Convert.ToInt32(doc.Descendants("Left").First().Value);
            int top = Convert.ToInt32(doc.Descendants("Top").First().Value);
            int right = Convert.ToInt32(doc.Descendants("Right").First().Value);
            int bottom = Convert.ToInt32(doc.Descendants("Bottom").First().Value);
            int radiusX = Convert.ToInt32(doc.Descendants("RadiusX").First().Value);
            int radiusY = Convert.ToInt32(doc.Descendants("RadiusY").First().Value);

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

                return WinAPI.CreateRoundRectRgn((int) (left*Properties.Settings.Default.ScaleFactor * dpiX),
                                                 (int) (top*Properties.Settings.Default.ScaleFactor * dpiY),
                                                 (int) (right*Properties.Settings.Default.ScaleFactor * dpiX),
                                                 (int) (bottom*Properties.Settings.Default.ScaleFactor * dpiY), radiusX,
                                                 radiusY);
            }
        }

        public void SetWindowPosition(double left, double top)
        {
            Properties.Settings.Default.Left = left;
            Properties.Settings.Default.Top = top;
        }

        public string GetIcon()
        {
            return E.Path + "\\Music\\Resources\\icon.png";
        }


        public void SetScalefactor(double scale)
        {
            Properties.Settings.Default.ScaleFactor = scale;
            _widgetControl.Scale.ScaleX = scale;
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

        public void UpdateAero(object sender)
        {
            UpdateAeroEvent(sender, EventArgs.Empty);
        }
    }
}
