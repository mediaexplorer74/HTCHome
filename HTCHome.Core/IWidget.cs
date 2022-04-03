using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace HTCHome.Core
{
    public interface IWidget
    {
        string GetWidgetName();
        UserControl Load();
        UserControl GetWidgetControl();
        void Unload();
        void SetParent(Window window);
        Point GetWindowPosition();
        IntPtr GetRegion();
        void SetWindowPosition(double left, double top);
        string GetIcon();
        double GetScalefactor();
        void SetScalefactor(double scale);
        double GetOpacity();
        void SetOpacity(double opacity);
        bool GetTopMost();
        void SetTopMost(bool value);
        bool GetPin();
        void SetPin(bool value);
        event EventHandler UpdateAeroEvent;
    }
}
