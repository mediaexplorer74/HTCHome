// Gallery

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace HTCHome.Gallery
{
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : Window
    {
        public Gallery()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //this.Topmost = false;
            foreach (var widget in App.widgets)
            {
                if (widget.IsLoaded)
                {
                    var item = new GalleryItem(widget)
                                   {
                                       Title = widget.WidgetName,
                                       //VerticalAlignment = VerticalAlignment,
                                       Margin = new Thickness(0, 0, 0, 40)
                                   };
                    item.VisualBrush.Visual = (Visual)widget.Content;
                    item.CloseButtonClick += ItemCloseButtonClick;
                    item.MouseLeftButtonDown += ItemMouseLeftButtonDown;
                    item.MaxWidth = 300;
                    item.CloseButton.ToolTip = App.LocaleManager.GetString("Close");

                    RunningWidgetsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(item, RunningWidgetsGrid.ColumnDefinitions.Count - 1);

                    RunningWidgetsGrid.Children.Add(item);
                }

                else
                {
                    var item = new GalleryItem(widget)
                    {
                        Title = widget.WidgetName,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 20)
                    };
                    item.VisualBrush.Visual = new Image { Source = new BitmapImage(new Uri(widget.WidgetIcon)) };
                    item.MouseLeftButtonDown += ItemMouseLeftButtonDown1;
                    item.MinHeight = 80;
                    item.CloseButton.Visibility = Visibility.Collapsed;

                    WidgetsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(item, WidgetsGrid.ColumnDefinitions.Count - 1);
                    WidgetsGrid.Children.Add(item);
                }
            }
        }

        void ItemMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((GalleryItem)sender).Widget.Activate();
            CloseGallery();
        }

        void ItemMouseLeftButtonDown1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = (GalleryItem)sender;

            WidgetsGrid.ColumnDefinitions.RemoveAt(WidgetsGrid.Children.IndexOf(item));
            WidgetsGrid.Children.Remove(item);

            int index = App.widgets.IndexOf(item.Widget);
            if (index > -1 && (!App.widgets[index].IsWidgetLoaded || !App.widgets[index].IsVisible))
            {
                var w = new Widget();
                w.Initalize(App.widgets[index].path);
                App.widgets[index] = w;
                App.widgets[index].Load();
            }

            item.VerticalAlignment = VerticalAlignment.Stretch;
            item.HorizontalAlignment = HorizontalAlignment.Stretch;
            item.Margin = new Thickness(0, 0, 0, 40);
            item.VisualBrush.Visual = (Visual)item.Widget.Content;
            item.MaxWidth = 300;
            item.CloseButton.Visibility = Visibility.Visible;
            item.CloseButton.ToolTip = App.LocaleManager.GetString("Close");

            RunningWidgetsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(item, RunningWidgetsGrid.ColumnDefinitions.Count - 1);

            RunningWidgetsGrid.Children.Add(item);

            item.MouseLeftButtonDown -= ItemMouseLeftButtonDown1;
            item.CloseButtonClick += ItemCloseButtonClick;
            item.MouseLeftButtonDown += ItemMouseLeftButtonDown;

            for (int i = 0; i < WidgetsGrid.Children.Count; i++)
            {
                Grid.SetColumn(WidgetsGrid.Children[i], i);
            }

            e.Handled = true;
        }

        void ItemCloseButtonClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var item = (GalleryItem)sender;

            RunningWidgetsGrid.ColumnDefinitions.RemoveAt(RunningWidgetsGrid.Children.IndexOf(item));
            RunningWidgetsGrid.Children.Remove(item);

            int index = App.widgets.IndexOf(item.Widget);
            if (index > -1 && App.widgets[index].IsWidgetLoaded)
            {
                App.widgets[index].IsWidgetLoaded = false;
                App.widgets[index].Unload();
                App.widgets[index].Close();
            }

            item.VerticalAlignment = VerticalAlignment.Bottom;
            item.HorizontalAlignment = HorizontalAlignment.Center;
            item.Margin = new Thickness(0, 0, 0, 20);
            item.VisualBrush.Visual = new Image { Source = new BitmapImage(new Uri(item.Widget.WidgetIcon)) };
            item.MinHeight = 80;
            item.CloseButton.Visibility = Visibility.Collapsed;
            WidgetsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(item, WidgetsGrid.ColumnDefinitions.Count - 1);
            WidgetsGrid.Children.Add(item);

            item.MouseLeftButtonDown += ItemMouseLeftButtonDown1;
            item.CloseButtonClick -= ItemCloseButtonClick;
            item.MouseLeftButtonDown -= ItemMouseLeftButtonDown;

            for (int i = 0; i < RunningWidgetsGrid.Children.Count; i++)
            {
                Grid.SetColumn(RunningWidgetsGrid.Children[i], i);
            }

            e.Handled = true;
        }

        private void CloseAnimCompleted(object sender, System.EventArgs e)
        {
            base.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source == this)
            {
                CloseGallery();
            }
        }

        // RnD - "new" or not "new"?
        public new void Close()
        {
            CloseGallery();
        }

        private void CloseGallery()
        {
            var s = (Storyboard)Resources["CloseAnim"];
            s.Begin();

            foreach (GalleryItem item in RunningWidgetsGrid.Children)
            {
                item.CloseButtonClick -= ItemCloseButtonClick;
                item.Unload();
            }

            foreach (GalleryItem item in WidgetsGrid.Children)
            {
                item.Unload();
            }
        }

        private void This_SourceInitialized(object sender, EventArgs e)
        {
            if (HTCHome.Properties.Settings.Default.EnableGlass)
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                
                HTCHome.Core.WinAPI.MakeGlassRegion
                (
                    ref handle,
                    Core.WinAPI.CreateRoundRectRgn
                    (
                        0, 0, (int) this.Width, (int) this.Height, 0, 0
                    )
                );
            }
        }
    }
}
