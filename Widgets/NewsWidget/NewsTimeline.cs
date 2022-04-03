using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using Argotic.Syndication;

namespace NewsWidget
{
    /// <summary>
    /// Represents flow of news from all sources, sorted by time
    /// </summary>
    public class NewsTimeline
    {
        public List<Source> Sources;

        public List<RssItem> News { get; private set; }

        public event RefreshFinishedDelegate RefreshFinished;
        public delegate void RefreshFinishedDelegate(IEnumerable<RssItem> newItems);

        private int _sourcesUpdatedCount;
        private DateTime _lastFeedDate;

        public NewsTimeline()
        {
            Sources = new List<Source>();

            if (Properties.Settings.Default.Sources != null)
            {
                foreach (string s in Properties.Settings.Default.Sources)
                {
                    var source = new Source { Url = s };
                    source.RefreshFinished += SourceRefreshFinished;
                    Sources.Add(source);
                }

                News = new List<RssItem>();
            }
        }

        public void Clear()
        {
            _lastFeedDate = new DateTime();
            News.Clear();
            foreach (var source in Sources)
            {
                source.Clear();
            }
        }

        public void AddSource(string url)
        {
            var source = new Source {Url = url};
            source.RefreshFinished += SourceRefreshFinished;
            Sources.Add(source);
        }

        public void RemoveSource(string url)
        {
            foreach (var source in Sources)
            {
                if (source.Url == url)
                {
                    Sources.Remove(source);
                    break;
                }
            }
        }

        public void Refresh()
        {
            foreach (var source in Sources)
            {
                source.Refresh();
            }
        }

        void SourceRefreshFinished(Source sender, IEnumerable<RssItem> newItems)
        {
            if (newItems != null && newItems.Count() > 0)
            {
                //add all new items to timeline
                News.AddRange(newItems);

            }

            _sourcesUpdatedCount++;

            //check if all sources were updated
            if (_sourcesUpdatedCount == Sources.Count)
            {
                _sourcesUpdatedCount = 0;

                //sort by date
                News = News.OrderByDescending(x => x.PublicationDate).ToList();
                if (News.Count > Properties.Settings.Default.NewsCount)
                {
                    //remove some 
                    News.RemoveRange(Properties.Settings.Default.NewsCount, News.Count - Properties.Settings.Default.NewsCount);
                }
                //make list with new items that was added since last feed was published
                List<RssItem> items = (from x in News
                                                  where x.PublicationDate.CompareTo(_lastFeedDate) == 1
                                                  select x).Reverse().ToList();
                if (items.Count > 0)
                {
                    foreach (var syndicationItem in items)
                    {
                        if (syndicationItem.PublicationDate.CompareTo(_lastFeedDate) == 1)
                            _lastFeedDate = syndicationItem.PublicationDate;
                    }
                }
                RefreshFinished(items);
            }
        }

 }
}
