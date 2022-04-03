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

namespace CalendarWidget
{
    /// <summary>
    /// Interaction logic for AgendaView.xaml
    /// </summary>
    public partial class AgendaView : UserControl
    {
        public delegate void AddNewItemEventHandler(DateTime d);
        public event AddNewItemEventHandler AddNewItem;

        public DateTime DateTime;

        public AgendaView()
        {
            InitializeComponent();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewItem(DateTime);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Widget.LocaleManager != null)
                AddItem.Header = Widget.LocaleManager.GetString("Add");
        }
    }
}
