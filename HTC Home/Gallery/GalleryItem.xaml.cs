using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace HTCHome.Gallery
{
    /// <summary>
    /// Interaction logic for GalleryItem.xaml
    /// </summary>
    public partial class GalleryItem : UserControl
    {
        public event MouseEventHandler CloseButtonClick;
        private Widget _widget;

        public GalleryItem(Widget widget)
        {
            InitializeComponent();
            _widget = widget;
        }

        public string Title
        {
            get { return WidgetName.Text; }
            set { WidgetName.Text = value; }
        }

        public Widget Widget { get { return _widget; }}

        private void ThisLoaded(object sender, RoutedEventArgs e)
        {
            var s = (Storyboard)Resources["LoadAnim"];
            s.Begin();
        }

        public void Unload()
        {
            var s = (Storyboard)Resources["UnloadAnim"];
            s.Begin();
        }

        private void UnloadAnimCompleted(object sender, EventArgs e)
        {
            VisualBrush.Visual = null;
        }

        private void CloseButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseButtonClick(this, e);
        }
    }
}
