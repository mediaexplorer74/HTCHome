using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherClockWidget
{
    /// <summary>
    /// Interaction logic for LocationItem.xaml
    /// </summary>
    public partial class LocationItem : UserControl
    {
        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (value == true)
                    Bg1.Opacity = 0.8;
                else
                    Bg1.Opacity = 0;
            }
        }

        private string _header = "";

        public string Header
        {
            get { return _header; }
            set
            {
                ItemTitleTextBlock.Text = value;
                _header = value;
            }
        }

        private int _order = 0;
        public int Order
        {
            get { return _order; }
            set
            {
                _order = value;
                TransformAnim.BeginTime = TimeSpan.FromMilliseconds(50 * value);
                OpacityAnim.BeginTime = TimeSpan.FromMilliseconds(50 * value);
            }
        }

        public LocationItem()
        {
            InitializeComponent();
        }
    }
}
