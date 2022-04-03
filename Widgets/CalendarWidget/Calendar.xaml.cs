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
using E = HTCHome.Core.Environment;
using L = HTCHome.Core.Logger;
using System.Windows.Media.Animation;
using Google.GData.Calendar;
using System.Threading;
using Google.GData.Client;

namespace CalendarWidget
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        private Options options;
        private CalendarService service;

        public Calendar()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            Body.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\body.png"));
            Header.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\header.png"));
            SubHeader.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\subheader.png"));
            Footer.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\footer.png"));
            CalendarIcon.Source = new BitmapImage(new Uri(E.Path + "\\Calendar\\Resources\\calendar_icon.png"));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();

            HeaderTextBlock.Text = DateTime.Now.ToString("MMMM yyyy");
            CalendarIconDay.Text = DateTime.Now.Day.ToString();
            CalendarIconMonth.Text = DateTime.Now.ToString("MMM");

            Scale.ScaleX = Properties.Settings.Default.ScaleFactor;

            AgendaView.Date.MouseLeftButtonUp += new MouseButtonEventHandler(Header_MouseLeftButtonUp);
            AgendaView.AddNewItem += new CalendarWidget.AgendaView.AddNewItemEventHandler(AgendaView_AddNewItem);

            MenuItem optionsItem = new MenuItem();
            optionsItem.Header = Widget.LocaleManager.GetString("Options");
            optionsItem.Click += new RoutedEventHandler(optionsItem_Click);
            Widget.Parent.ContextMenu.Items.Insert(0, new Separator());
            Widget.Parent.ContextMenu.Items.Insert(0, optionsItem);

            CalendarControl.CalendarDisplayDateChanged += new CalendarWidget.CalendarControl.CalendarDisplayDateChangedEventHandler(CalendarControl_CalendarDisplayDateChanged);

            options = new Options();

            if (Properties.Settings.Default.Sync)
            {
                ThreadStart threadStarter = delegate
                {
                    L.Log("Calendar: Start Google calendar synchronization");
                    service = CalendarHelper.GetService("HTC Home 2 Calendar Widget", Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                    L.Log("Calendar: Connected to Google calendar");

                    IEnumerable<EventEntry> events = CalendarHelper.GetAllEvents(service, null);
                    L.Log("Calendar: Events readed");
                    if (events != null)
                    {
                        foreach (EventEntry ev in events)
                        {
                            /*var dates = from x in DayConverter.dict.Keys
                                        where ((x.Year == (ev.Times.First().StartTime.Date.Year) && (x.Month == (ev.Times.First().StartTime.Date.Month) && (x.Day == (ev.Times.First().StartTime.Date.Day)))))
                                        select x;
                            var count = dates.Count();*/
                            DateTime d = ev.Times.First().StartTime;
                            if (DayConverter.dict.ContainsKey(ev.Times.First().StartTime))
                            {
                                CalendarEvent calendarEvent = new CalendarEvent();
                                calendarEvent.Url = ev.EditUri.Content;
                                calendarEvent.Title = ev.Title.Text;
                                calendarEvent.StartTime = ev.Times.First().StartTime;
                                calendarEvent.EndTime = ev.Times.First().EndTime;
                                DayConverter.dict[ev.Times.First().StartTime] = calendarEvent;
                            }
                            else
                            {
                                CalendarEvent calendarEvent = new CalendarEvent();
                                calendarEvent.Url = ev.EditUri.Content;
                                calendarEvent.Title = ev.Title.Text;
                                calendarEvent.StartTime = ev.Times.First().StartTime;
                                calendarEvent.EndTime = ev.Times.First().EndTime;
                                DayConverter.dict.Add(ev.Times.First().StartTime, calendarEvent);
                            }

                            CalendarControl.Dispatcher.Invoke((Action)delegate
                            {
                                CalendarControl.Cal.DisplayDate = DateTime.Now.AddMonths(-1);
                                CalendarControl.Cal.DisplayDate = DateTime.Now;
                            }, null);
                        }
                    }
                };
                Thread thread = new Thread(threadStarter);
                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();
            }
        }

        void CalendarControl_CalendarDisplayDateChanged(DateTime date)
        {
            HeaderTextBlock.Text = date.ToString("MMMM yyyy");
            CalendarIconDay.Text = date.Day.ToString();
            CalendarIconMonth.Text = date.ToString("MMM");
        }

        void optionsItem_Click(object sender, RoutedEventArgs e)
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options();

            if (E.Locale == "he-IL" || E.Locale == "ar-SA")
            {
                options.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                options.FlowDirection = FlowDirection.LeftToRight;
            }

            options.ShowDialog();
        }

        void Header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Storyboard)Resources["Swap2"]).Begin();
        }

        private void CalendarControl_CalendarDayChanged(DateTime? date)
        {
            AgendaView.ItemsPanel.Children.Clear();
            AgendaView.DateTime = date.Value;
            CalendarEvent value;
            var dates = from x in DayConverter.dict.Keys
                        where ((x.Year == date.Value.Year) && (x.Month == date.Value.Month) && (x.Day == date.Value.Day))
                        select x;
            if (dates.Count() > 0)
            {
                foreach (var d in dates)
                {
                    if (DayConverter.dict.TryGetValue(d, out value))
                    {
                            AgendaItem item = new AgendaItem(d, AgendaView.ItemsPanel.Children.Count);
                            item.Title.Text = value.Title;
                            item.StartTime.Text = value.StartTime.ToShortTimeString();
                            item.EndTime.Text = value.EndTime.ToShortTimeString();
                            item.calendarEvent = value;
                            item.service = service;
                            AgendaView.ItemsPanel.Children.Add(item);
                    }
                }
            }

            ((Storyboard)Resources["Swap1"]).Begin();
            AgendaView.Date.Text = date.Value.ToLongDateString();
        }

        void AgendaView_AddNewItem(DateTime d)
        {
            CalendarEvent ev = new CalendarEvent();
            ev.Title = Widget.LocaleManager.GetString("EditTitleTooltip");
            ev.StartTime = d.AddHours(DateTime.Now.Hour);
            ev.EndTime = d.AddHours(DateTime.Now.Hour + 1);

            AgendaItem item = new AgendaItem(d.AddSeconds(AgendaView.ItemsPanel.Children.Count), AgendaView.ItemsPanel.Children.Count);
            item.Title.Text = Widget.LocaleManager.GetString("EditTitleTooltip");
            item.StartTime.Text = ev.StartTime.ToShortTimeString();
            item.EndTime.Text = ev.EndTime.ToShortTimeString();
            AgendaView.ItemsPanel.Children.Add(item);

            if (Properties.Settings.Default.Sync && service != null)
            {
                AtomEntry entry = CalendarHelper.AddEvent(service, ev.Title, "", "", d.AddHours(DateTime.Now.Hour), d.AddHours(DateTime.Now.Hour + 1));
                item.calendarEvent = ev;
                item.service = service;
                ev.Url = entry.EditUri.Content;
            }

            DayConverter.dict.Add(d.AddSeconds(AgendaView.ItemsPanel.Children.Count - 1), ev);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            CalendarControl.Cal.DisplayDate = DateTime.Now.AddMonths(-1);
            CalendarControl.Cal.DisplayDate = DateTime.Now;
        }
    }
}
