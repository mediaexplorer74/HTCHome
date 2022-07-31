// Widget

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HTCHome.Core;
using System.Reflection;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace HTCHome
{
    /// <summary>
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class Widget : Window
    {
        private IntPtr handle;
        private IWidget widget;
        public string path;

        public string WidgetName
        {
            get;
            set;
        }

        public string WidgetIcon
        {
            get;
            set;
        }

        public bool IsWidgetLoaded
        {
            get;
            set;
        }

        public bool HasErrors
        {
            get;
            set;
        }

        public Widget()
        {
            InitializeComponent();
        }

        public void Initalize(string path)
        {
            this.path = path;

            Assembly assembly = Assembly.LoadFrom(path);

            Type widgetType = null;

            try
            {
                widgetType = assembly.GetTypes().FirstOrDefault(type => typeof(IWidget).IsAssignableFrom(type));
            }
            catch (Exception ex)
            {
                App.Log(ex.ToString());
            }

            if (widgetType == null)
            {
                App.Log(path + " is not a widget.");
                HasErrors = true;
                return;
            }

            widget = Activator.CreateInstance(widgetType) as IWidget;
            WidgetName = widget.GetWidgetName();
            WidgetIcon = widget.GetIcon();

            widget.UpdateAeroEvent += Widget_UpdateAero;

            CloseItem.Header = App.LocaleManager.GetString("Close");
            CloseHomeItem.Header = App.LocaleManager.GetString("CloseHome");

            GalleryItem.Header = App.LocaleManager.GetString("Widgets");
            HomeOptionsItem.Header = App.LocaleManager.GetString("HomeOptions");
            PinItem.Header = App.LocaleManager.GetString("Pin");
            TopMostItem.Header = App.LocaleManager.GetString("TopMost");
            SizeItem.Header = App.LocaleManager.GetString("Size");
            OpacityItem.Header = App.LocaleManager.GetString("Opacity");
        }

        void Widget_UpdateAero(object sender, EventArgs e)
        {
            if (HTCHome.Properties.Settings.Default.EnableGlass)
            {
                WinAPI.RemoveGlassRegion(ref handle);
                WinAPI.MakeGlassRegion(ref handle, widget.GetRegion());
            }
        }

        public void Load()
        {
            widget.SetParent(this);
            UserControl w = widget.Load();
            w.SizeChanged += new SizeChangedEventHandler(W_SizeChanged);
            this.Width = w.Width;
            this.Height = w.Height;

            Storyboard loadAnim = Resources["LoadAnim"] as Storyboard;
            //((DoubleAnimation)loadAnim.Children[0]).To = widget.GetWindowPosition().Y;
            //((DoubleAnimation)loadAnim.Children[0]).From = widget.GetWindowPosition().Y + 70;

            this.Left = widget.GetWindowPosition().X;
            this.Top = widget.GetWindowPosition().Y;
            if (this.Left == -100 || this.Top == -100)
            {
                this.Left = System.Windows.Forms.SystemInformation.WorkingArea.Width / 2 - w.ActualWidth / 2;
                this.Top = System.Windows.Forms.SystemInformation.WorkingArea.Height / 2 - w.ActualHeight / 2 - 30;
            }
            this.Show();

            SizeSlider.Value = widget.GetScalefactor() * 100;
            OpacitySlider.Value = widget.GetOpacity() * 100;

            this.Topmost = widget.GetTopMost();
            TopMostItem.IsChecked = this.Topmost;

            PinItem.IsChecked = widget.GetPin();

            WinAPI.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1); //наверное так делать не стоит, но зато теперь те, кто говорит "ОМГ он жрет столько памяти!!!11" могут успокоиться
            loadAnim.Begin(this);
        }

        void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            int index = AddWidgetPanel.Children.IndexOf((UIElement)sender);
            if (App.widgets[index].IsWidgetLoaded)
            {
                App.widgets[index].IsWidgetLoaded = false;
                App.widgets[index].Unload();
                App.widgets[index].Close();
            }
        }

        void Item_Checked(object sender, RoutedEventArgs e)
        {
            int index = AddWidgetPanel.Children.IndexOf((UIElement)sender);
            if (!App.widgets[index].IsWidgetLoaded || !App.widgets[index].IsVisible)
            {
                var w = new Widget();
                w.Initalize(App.widgets[index].path);
                App.widgets[index] = w;
                App.widgets[index].Load();
            }
        }

        void W_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = e.NewSize.Width;
            this.Height = e.NewSize.Height;
        }

        public void Unload()
        {
            widget.SetWindowPosition(this.Left, this.Top);
            widget.Unload();
            //IsWidgetLoaded = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !PinItem.IsChecked)
            {
                DragMove();
                widget.SetWindowPosition(this.Left, this.Top);
            }
        }

        private void CloseItem_Click(object sender, RoutedEventArgs e)
        {
            IsWidgetLoaded = false;
            Unload();
            this.Close();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;

            WinAPI.RemoveFromAeroPeek(handle);
            WinAPI.RemoveFromAltTab(handle);
            WinAPI.RemoveFromFlip3D(handle);

            MainGrid.Children.Add(widget.GetWidgetControl());
            IsWidgetLoaded = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsWidgetLoaded)
            {
                Unload();
            }

            int count = 0;
            foreach (Widget w in App.widgets)
            {
                if (w.IsLoaded)
                    count++;
            }
            if (count == 1)
                App.Current.Shutdown();
        }

        private void TopMostItem_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            widget.SetTopMost(true);
        }

        private void TopMostItem_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            widget.SetTopMost(false);
        }

        private void SizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            double scale = 1.0f - ((SizeItem.Items.IndexOf(sender)) / 10.0f);
            widget.SetScalefactor(scale);
            foreach (MenuItem item in SizeItem.Items)
            {
                if (sender != item)
                    item.IsChecked = false;
                else
                    item.IsChecked = true;
            }

            if (HTCHome.Properties.Settings.Default.EnableGlass)
            {
                WinAPI.RemoveGlassRegion(ref handle);
                WinAPI.MakeGlassRegion(ref handle, widget.GetRegion());
            }
        }

        private void PinItem_Checked(object sender, RoutedEventArgs e)
        {
            widget.SetPin(true);
        }

        private void PinItem_Unchecked(object sender, RoutedEventArgs e)
        {
            widget.SetPin(false);
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*SizeSlider.ToolTip = SizeSlider.Value.ToString();
            ((ToolTip)SizeSlider.ToolTip).IsOpen = true;*/
            if (widget != null)
            {
                widget.SetScalefactor(SizeSlider.Value / 100);
                if (HTCHome.Properties.Settings.Default.EnableGlass)
                {
                    WinAPI.RemoveGlassRegion(ref handle);
                    WinAPI.MakeGlassRegion(ref handle, widget.GetRegion());
                }
            }
        }

        private void HomeOptionsItem_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ShowOptions();
        }

        private void CloseHomeItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var files = from x in ((string[])e.Data.GetData(DataFormats.FileDrop, true))
                        where x.EndsWith(".hhskin") || x.EndsWith(".hhext")
                        select x;
            if (files != null)
            {
                foreach (string f in files)
                {
                    try
                    {
                        App.Unpack(App.Path, f);
                        if (f.EndsWith(".hhskin"))
                            MessageBox.Show(App.LocaleManager.GetString("SkinInstalled"), "", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                            MessageBox.Show(App.LocaleManager.GetString("ExtensionInstalled"), "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        if (f.EndsWith(".hhskin"))
                            MessageBox.Show(App.LocaleManager.GetString("SkinNotInstalledNoAccess"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                            MessageBox.Show(App.LocaleManager.GetString("ExtensionNotInstalledNoAccess"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                        App.Log("Can't install exntension " + f + "\n" + ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        if (f.EndsWith(".hhskin"))
                            MessageBox.Show(App.LocaleManager.GetString("SkinNotInstalled"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                            MessageBox.Show(App.LocaleManager.GetString("ExtensionNotInstalled"), "", MessageBoxButton.OK, MessageBoxImage.Error);
                        App.Log("Can't install exntension " + f + "\n" + ex.ToString());
                    }
                }
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (HTCHome.Properties.Settings.Default.EnableGlass)
                WinAPI.MakeGlassRegion(ref handle, widget.GetRegion());
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            AddWidgetPanel.Children.Clear();
            foreach (Widget widget1 in App.widgets)
            {
                var item = new ToggleButton();
                item.ToolTip = widget1.WidgetName;
                item.Margin = new Thickness(0, 0, 5, 0);
                Image icon = new Image();
                icon.Source = new BitmapImage(new Uri(widget1.WidgetIcon));
                icon.Width = 20;
                icon.Height = 20;
                item.Content = icon;
                if (widget1.IsLoaded)
                    item.IsChecked = true;
                item.Checked += Item_Checked;
                item.Unchecked += Item_Unchecked;
                AddWidgetPanel.Children.Add(item);
            }
        }

        private void GalleryItemClick(object sender, RoutedEventArgs e)
        {
            if (App.Gallery != null && App.Gallery.IsVisible)
            {
                App.Gallery.Close();
                return;
            }
            App.Gallery = new Gallery.Gallery()
                              {
                                  Left = 0,
                                  Top = 0,
                                  Width = SystemParameters.PrimaryScreenWidth,
                                  Height = SystemParameters.PrimaryScreenHeight
                              };
            App.Gallery.ShowDialog();
        }

        private void OpacitySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (widget != null)
            {
                widget.SetOpacity(OpacitySlider.Value / 100);
                MainGrid.Opacity = OpacitySlider.Value / 100;
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            var mouseEnterAnim = Resources["MouseEnter"] as Storyboard;
            mouseEnterAnim.Begin(MainGrid);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            var mouseLeaveAnim = Resources["MouseLeave"] as Storyboard;
            ((DoubleAnimation)mouseLeaveAnim.Children[0]).To = OpacitySlider.Value / 100;
            mouseLeaveAnim.Begin(MainGrid);
        }

        private void MouseEnter_Completed(object sender, EventArgs e)
        {
            MainGrid.Opacity = 1;
        }

        private void MouseLeave_Completed(object sender, EventArgs e)
        {
            var mouseLeaveAnim = Resources["MouseLeave"] as Storyboard;
            MainGrid.Opacity = (double)((DoubleAnimation)mouseLeaveAnim.Children[0]).To;
        }
    }
}
