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
using Google.GData.Client;
using Google.GData.Calendar;

namespace CalendarWidget
{
    /// <summary>
    /// Interaction logic for AgendaItem.xaml
    /// </summary>
    public partial class AgendaItem : UserControl
    {
        private DateTime d;

        //public AtomEntry entry;
        public CalendarEvent calendarEvent;
        public CalendarService service;

        public AgendaItem(DateTime d, int seed)
        {
            InitializeComponent();

            this.d = d;

            Random r = new Random(Environment.TickCount * seed);
            switch (r.Next(1, 4))
            {
                case 1:
                    Marker.Fill = (LinearGradientBrush)Resources["Green"];
                    break;
                case 2:
                    Marker.Fill = (LinearGradientBrush)Resources["Orange"];
                    break;
                case 3:
                    Marker.Fill = (LinearGradientBrush)Resources["Blue"];
                    break;
            }

            RemoveItem.Header = Widget.LocaleManager.GetString("Remove");
        }

        private void TitleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Title.Text = TitleTextBox.Text;
                TitleTextBox.Visibility = System.Windows.Visibility.Collapsed;
                DayConverter.dict[d].Title = Title.Text;
                /*DateTime startTime, endTime;

                if (DateTime.TryParse(StartTime.Text, out startTime))
                {
                    DayConverter.dict[d].StartTime = startTime;
                }

                if (DateTime.TryParse(EndTime.Text, out endTime))
                {
                    DayConverter.dict[d].EndTime = endTime;
                }*/

                if (service != null && calendarEvent != null && Properties.Settings.Default.Sync)
                {
                    DayConverter.dict[d].Url = CalendarHelper.UpdateEvent(service, calendarEvent.Url, Title.Text, calendarEvent.StartTime, calendarEvent.EndTime);
                    calendarEvent.Url = DayConverter.dict[d].Url;
                }
            }
            if (e.Key == Key.Escape)
                TitleTextBox.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                TitleTextBox.Text = Title.Text;
                TitleTextBox.Visibility = System.Windows.Visibility.Visible;
                TitleTextBox.Focus();
                TitleTextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            ((StackPanel)this.Parent).Children.Remove(this);
            DayConverter.dict.Remove(d);
            if (service != null && calendarEvent != null && Properties.Settings.Default.Sync)
            {
                CalendarHelper.RemoveEvent(service, calendarEvent.Url);
            }
        }

        private void StartTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                StartTimeTextBox.Text = StartTime.Text;
                StartTimeTextBox.Visibility = System.Windows.Visibility.Visible;
                StartTimeTextBox.Focus();
                StartTimeTextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void StartTimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DateTime startTime;
                if (DateTime.TryParse(d.ToShortDateString() + " " + StartTimeTextBox.Text, out startTime))
                {
                    StartTime.Text = startTime.ToShortTimeString();
                    StartTimeTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    DayConverter.dict[d].StartTime = startTime;
                    if (startTime.CompareTo(DayConverter.dict[d].EndTime) == 1)
                    {
                        DayConverter.dict[d].EndTime = startTime;
                        EndTime.Text = startTime.ToShortTimeString();
                    }
                    if (service != null && calendarEvent != null && Properties.Settings.Default.Sync)
                    {
                        DayConverter.dict[d].Url = CalendarHelper.UpdateEvent(service, calendarEvent.Url, calendarEvent.Title, startTime, calendarEvent.EndTime);
                        calendarEvent.Url = DayConverter.dict[d].Url;
                    }
                }
            }
            if (e.Key == Key.Escape)
                StartTimeTextBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void EndTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                EndTimeTextBox.Text = EndTime.Text;
                EndTimeTextBox.Visibility = System.Windows.Visibility.Visible;
                EndTimeTextBox.SelectAll();
                EndTimeTextBox.Focus();
                e.Handled = true;
            }
        }

        private void EndTimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DateTime endTime;
                if (DateTime.TryParse(d.ToShortDateString() + " " + EndTimeTextBox.Text, out endTime))
                {
                    EndTime.Text = endTime.ToShortTimeString();
                    EndTimeTextBox.Visibility = System.Windows.Visibility.Collapsed;
                    DayConverter.dict[d].EndTime = endTime;
                    if (endTime.CompareTo(DayConverter.dict[d].StartTime) == -1)
                    {
                        DayConverter.dict[d].StartTime = endTime;
                        StartTime.Text = endTime.ToShortTimeString();
                    }
                    if (service != null && calendarEvent != null && Properties.Settings.Default.Sync)
                    {
                        DayConverter.dict[d].Url = CalendarHelper.UpdateEvent(service, calendarEvent.Url, calendarEvent.Title, calendarEvent.StartTime, endTime);
                        calendarEvent.Url = DayConverter.dict[d].Url;
                    }
                }
            }

            if (e.Key == Key.Escape)
                EndTimeTextBox.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void StartTimeTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                e.Handled = true;
        }

        private void EndTimeTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                e.Handled = true;
        }

        private void TitleTextBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                e.Handled = true;
        }
    }
}
