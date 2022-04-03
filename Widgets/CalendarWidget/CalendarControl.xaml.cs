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
using System.Windows.Controls.Primitives;

namespace CalendarWidget
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl
    {
        public delegate void CalendarDayChangedEventHandler(DateTime? date);
        public event CalendarDayChangedEventHandler CalendarDayChanged;

        public delegate void CalendarDisplayDateChangedEventHandler(DateTime date);
        public event CalendarDisplayDateChangedEventHandler CalendarDisplayDateChanged;

        public CalendarControl()
        {
            InitializeComponent();
            Cal.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(Cal_SelectedDatesChanged);

        }

        void Cal_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cal.SelectedDate != null)
            {
                CalendarDayChanged(Cal.SelectedDate);
                Cal.SelectedDate = null;
            }
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Cal.DisplayDate = Cal.DisplayDate.AddMonths(-1);
            else
                Cal.DisplayDate = Cal.DisplayDate.AddMonths(1);
            CalendarDisplayDateChanged(Cal.DisplayDate);
        }
    }
}
