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
using System.Xml;
using Argotic.Syndication;
using HtmlAgilityPack;
using E = HTCHome.Core.Environment;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Threading;
using HTCHome.Core;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Web;

namespace NewsWidget
{
    /// <summary>
    /// Interaction logic for News.xaml
    /// </summary>
    public partial class News : UserControl
    {
        private DispatcherTimer timer;

        private Options options;

        public NewsTimeline NewsTimeline;

        public News()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            Body.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\body.png"));
            Header.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\header.png"));
            Footer.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\footer.png"));
            NewsPaperIcon.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\newspaper.png"));
            RefreshIcon.Source = new BitmapImage(new Uri(E.Path + "\\News\\Resources\\refresh.png"));
        }

        public void Load()
        {
            Initialize();

            NewsTimeline = new NewsTimeline();
            NewsTimeline.RefreshFinished += new NewsWidget.NewsTimeline.RefreshFinishedDelegate(NewsTimelineRefreshFinished);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(Properties.Settings.Default.Interval);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();

            NewsTimeline.Refresh();

            var s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
            s.Begin();


            Scale.ScaleX = Properties.Settings.Default.ScaleFactor;

            Widget.Instance.Parent.ContextMenu.Items.Insert(0, new Separator());

            var optionsItem = new MenuItem();
            optionsItem.Header = Widget.Instance.LocaleManager.GetString("Options");
            optionsItem.Click += OptionsItemClick;
            Widget.Instance.Parent.ContextMenu.Items.Insert(0, optionsItem);

            var clearItem = new MenuItem();
            clearItem.Header = Widget.Instance.LocaleManager.GetString("Clear");
            clearItem.Click += ClearItemClick;
            Widget.Instance.Parent.ContextMenu.Items.Insert(0, clearItem);

            var refreshItem = new MenuItem();
            refreshItem.Header = Widget.Instance.LocaleManager.GetString("Refresh");
            refreshItem.Click += RefreshItemClick;
            Widget.Instance.Parent.ContextMenu.Items.Insert(0, refreshItem);

            options = new Options(this);

            NewsText.Text = Widget.Instance.LocaleManager.GetString("News");
            RefreshTime.Text = DateTime.Now.ToString(Widget.Instance.LocaleManager.GetString("ShortDateString"));
        }

        void RefreshItemClick(object sender, RoutedEventArgs e)
        {
            NewsTimeline.Refresh();
            RefreshIcon.Dispatcher.Invoke((Action)delegate
            {
                var s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
                s.Begin();
            }, null);
        }

        void NewsTimelineRefreshFinished(IEnumerable<RssItem> newItems)
        {
            if (newItems != null)
                foreach (var feed in newItems)
                {
                    NewsPanel.Dispatcher.Invoke((Action)delegate
                    {

                        var item = new NewsItem();
                        item.Title = feed.Title;
                        item.Text = StripTags(feed.Description);
                        if (feed.PublicationDate.ToLocalTime().Day == DateTime.Now.Day)
                            item.Footer = feed.PublicationDate.ToLocalTime().ToString(Widget.Instance.LocaleManager.GetString("ShortDateString")) + " from " + feed.Source.Title;
                        else
                            item.Footer = feed.PublicationDate.ToLocalTime().ToString(Widget.Instance.LocaleManager.GetString("DateString")) + " from " + feed.Source.Title;
                        item.ImageSource = GetImg(feed.Description);
                        item.Link = feed.Link.ToString();
                        NewsPanel.Children.Insert(0, item);
                        if (NewsPanel.Children.Count > Properties.Settings.Default.NewsCount)
                            NewsPanel.Children.RemoveAt(NewsPanel.Children.Count - 1);
                    },
                    null);
                }
            var s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
            s.Stop();
            RefreshTime.Dispatcher.Invoke((Action)delegate
            {
                RefreshTime.Text = DateTime.Now.ToString(Widget.Instance.LocaleManager.GetString("ShortDateString"));
            }, null);
        }

        //public void SourceRefreshFinished(Source sender, IEnumerable<System.ServiceModel.Syndication.SyndicationItem> newItems)
        //{
        //    if (newItems != null)
        //        foreach (var feed in newItems)
        //        {
        //            NewsPanel.Dispatcher.Invoke((Action)delegate
        //            {

        //                var item = new NewsItem();
        //                item.Title = feed.Title.Text;
        //                item.Text = StripTags(feed.Summary.Text);
        //                item.Footer = feed.PublishDate.ToLocalTime().ToString() + " from " + sender.Title;
        //                item.ImageSource = GetImg(feed.Summary.Text);
        //                NewsPanel.Children.Insert(0, item);
        //                if (NewsPanel.Children.Count >= Properties.Settings.Default.NewsCount)
        //                    NewsPanel.Children.RemoveAt(NewsPanel.Children.Count - 1);
        //            },
        //            null);
        //        }
        //    Storyboard s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
        //    s.Stop();
        //    RefreshTime.Dispatcher.Invoke((Action)delegate
        //                                               {
        //                                                   RefreshTime.Text = DateTime.Now.ToString(Widget.Instance.LocaleManager.GetString("ShortDateString"));
        //                                               }, null);
        //}

        void TimerTick(object sender, EventArgs e)
        {
            NewsTimeline.Refresh();
            RefreshIcon.Dispatcher.Invoke((Action)delegate
            {
                var s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
                s.Begin();
            }, null);
        }

        void ClearItemClick(object sender, RoutedEventArgs e)
        {
            NewsPanel.Children.Clear();
            NewsTimeline.Clear();
        }

        void OptionsItemClick(object sender, RoutedEventArgs e)
        {
            if (options.IsVisible)
            {
                options.Activate();
                return;
            }
            options = new Options(this);

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

        //public void GetNewsFinished(object sender, EventArgs e)
        //{
        //    var s = (Source)sender;
        //    foreach (var feed in s.Items)
        //    {
        //        NewsPanel.Dispatcher.Invoke((Action)delegate
        //                                                 {
        //                                                     if (NewsPanel.Children.Count >= Widget.Instance.Sett.NewsCount)
        //                                                         return;
        //                                                     var item = new NewsItem();
        //                                                     item.Title = feed.Title.Text;
        //                                                     item.Text = StripTags(feed.Summary.Text);
        //                                                     item.Footer = feed.PublishDate.ToLocalTime().ToString() + " from " + s.Title;
        //                                                     item.ImageSource = GetImg(feed.Summary.Text);
        //                                                     NewsPanel.Children.Add(item);

        //                                                 },
        //                                    null);

        //    }
        //}

        private static string StripTags(string input)
        {
            //var regHtml = new System.Text.RegularExpressions.Regex("<[^>]*>");
            string r = StripTagsCharArray(input); // regHtml.Replace(input, string.Empty);
            while (r.StartsWith("\n"))
                r = r.Replace("\n", string.Empty);
            while (r.StartsWith("\t"))
                r = r.Replace("\t", string.Empty);
            r = HttpUtility.HtmlDecode(r);
            return r;
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        private string GetImg(string s)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//img/@src");

            // Run only if there are links in the document.
            if (linkNodes != null)
            {
                foreach (HtmlNode linkNode in linkNodes)
                {
                    HtmlAttribute attrib = linkNode.Attributes["src"];
                    return attrib.Value;
                    // Do whatever else you need here
                }
            }
            return null;
        }

        private void StackPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewsTimeline.Refresh();
            RefreshIcon.Dispatcher.Invoke((Action)delegate
                                                {
                                                    Storyboard s = (Storyboard)RefreshIcon.Resources["RefreshAnim"];
                                                    s.Begin();
                                                }, null);

        }
    }
}
