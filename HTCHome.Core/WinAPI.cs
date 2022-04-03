// 24 sep 2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Forms;

namespace HTCHome.Core
{
    public static class WinAPI
    {
        public static UInt32 SPIF_UPDATEINIFILE = 0x1;

        public static UInt32 SPI_SETDESKWALLPAPER = 20;

        private const int ExcludedFromPeek = 12;

        private const int Flip3D = 8;

        private const int GWL_EXSTYLE = -20;

        private const int GWL_STYLE = -16;

        private const int WS_EX_APPWINDOW = 0x00040000;

        private const int WS_EX_TOOLWINDOW = 0x00000080;

        private const int WS_CHILDWINDOW = 0x40000000;
        private const uint WS_POPUP = 0x80000000;
        private const int WS_CLIPSIBLINGS = 0x4000000;

        private const int WS_CLIPCHILDREN = 0x2000000;

        private const int WS_MAXIMIZEBOX = 0x10000;

        private const int WM_WINDOWPOSCHANGING = 0x0046;

        private const int WM_SETFOCUS = 7;

        public const int WM_SETTINGCHANGE = 0x1A;

        public const int WmHotKey = 0x0312;

        public enum Flip3DPolicy
        {
            Default = 0,
            ExcludeBelow,
            ExcludeAbove
        }

        private enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };

        private struct BB_Struct //Blur Behind Structure
        {
            public BB_Flags flags;
            public bool enable;
            public IntPtr region;
            public bool transitionOnMaximized;
        }

        private enum BB_Flags : byte //Blur Behind Flags
        {
            DWM_BB_ENABLE = 1,
            DWM_BB_BLURREGION = 2,
            DWM_BB_TRANSITIONONMAXIMIZED = 4,
        };


        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        private static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref BB_Struct blurBehind);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int xradius, int yradius);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateEllipticRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(IntPtr hInstance, string path, int iconIndex);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, int minimumWorkingSetSize, int maximumWorkingSetSize);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static bool IsGlassAvailable() //Check if it is not a Windows Vista or it is a Windows Vista Home Basic
        {
            return (System.Environment.OSVersion.Version.Major >= 6 && System.Environment.OSVersion.Version.Build >= 5600) && File.Exists(System.Environment.SystemDirectory + @"\dwmapi.dll");
        }

        public static bool IsGlassEnabled()
        {
            bool result = false;
            DwmIsCompositionEnabled(out result);
            return result;
        }

        public static void RemoveFromAeroPeek(IntPtr hwnd)
        {
            if (IsGlassAvailable() && System.Environment.OSVersion.Version.Major == 6 &&
                System.Environment.OSVersion.Version.Minor == 1)
            {
                int attrValue = 1; // True
                DwmSetWindowAttribute(hwnd, 12, ref attrValue, sizeof(int));
            }
        }

        public static void RemoveFromAltTab(IntPtr hwnd)
        {
            var windowStyle = (uint)GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, windowStyle | WS_EX_TOOLWINDOW);
        }

        public static void ExtendGlassFrame(IntPtr hwnd, ref MARGINS margins)
        {
            if (IsGlassAvailable())
                DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        public static void RemoveFromFlip3D(IntPtr hwnd)
        {
            if (IsGlassAvailable())
            {
                var attrValue = (int)Flip3DPolicy.ExcludeBelow; // True
                DwmSetWindowAttribute(hwnd, Flip3D, ref attrValue, sizeof(int));
            }
        }

        public static void MakeGlassRegion(ref IntPtr handle, IntPtr rgn)
        {
            if (IsGlassAvailable() && rgn != IntPtr.Zero)
            {
                BB_Struct bb = new BB_Struct();
                bb.enable = true;
                bb.flags = BB_Flags.DWM_BB_ENABLE | BB_Flags.DWM_BB_BLURREGION;
                if (rgn != null)
                    bb.region = rgn;
                else
                    bb.region = IntPtr.Zero; //Region.GetHrgn(Graphics)
                DwmEnableBlurBehindWindow(handle, ref bb);
            }
        }

        public static void RemoveGlassRegion(ref IntPtr handle)
        {
            if (IsGlassAvailable())
            {
                BB_Struct bb = new BB_Struct();
                bb.enable = false;
                bb.flags = BB_Flags.DWM_BB_ENABLE | BB_Flags.DWM_BB_BLURREGION;
                bb.region = IntPtr.Zero; //Region.GetHrgn(Graphics)
                DwmEnableBlurBehindWindow(handle, ref bb);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 msg, UInt32 wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr window, int index, uint value);

        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd,
                                                 string lpOperation,
                                                 string lpFile,
                                                 string lpParameters,
                                                 string lpDirectory,
                                                 int nShowCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(HandleRef hWnd);
    }
}
