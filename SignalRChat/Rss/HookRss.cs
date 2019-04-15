using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using SignalRChat.Util;
using static SignalRChat.Util.EventHandlers;

namespace SignalRChat.Rss
{
    public class HookRss : IHookRss
    {
        public readonly int newsBufferSize;
        public TimeSpan updateInterval = new TimeSpan(0, 0, 10);

        public event UpdateItem<News> NewsUpdated;

        private HashSet<string> keywords = new HashSet<string>();
        private IUniqueCircularBuffer<News> news;
        private Task updateNews;
        private CancellationTokenSource stopUpdating;

        public bool Subscribe(string keyword)
        {
            keyword = keyword.Trim().ToLower();
            return keywords.Add(keyword);
        }

        public bool Unsubscribe(string keyword)
        {
            return keywords.Remove(keyword);
        }

        public IEnumerable<SimpleNews> GetRelevant(string keyword)
        {
            List<SimpleNews> relevantNews = new List<SimpleNews>();
            var bufferedNews = news.Read();
            foreach (News news in bufferedNews)
            {
                if (news.Keywords.Contains(keyword))
                {
                    relevantNews.Add(news.SimpleNews);
                }
            }
            return relevantNews;
        }

        public HookRss(int newsBufferSize)
        {
            this.newsBufferSize = newsBufferSize;
            news = new ConcurrentUniqueCircularBuffer<News>(newsBufferSize);
            news.Updated += GotNews;
            stopUpdating = new CancellationTokenSource();
            updateNews = Task.Factory.StartNew(GetFeed, stopUpdating.Token);
        }

        public HookRss(int newsBufferSize, UpdateItem<News> updateHandler) : this(newsBufferSize)
        {
            NewsUpdated += updateHandler;
        }

        ~HookRss()
        {
            stopUpdating.Cancel();
        }

        private void GetFeed()
        {
            while (!stopUpdating.Token.IsCancellationRequested)
            {
                if (keywords.Count != 0)
                {
                    HookFeeds();
                }
                updateNews.Wait(updateInterval);
            }
        }

        private void HookFeeds()
        {
            string url = "https://news.google.com/news/rss/headlines/section/topic/WORLD?ned=us&hl=en";
            SyndicationFeed feed;
            using (XmlReader reader = XmlReader.Create(url))
            {
                feed = SyndicationFeed.Load(reader);
            }

            var item = feed.Items.GetEnumerator();
            for (int actualFeedCount = 0; item.MoveNext() && actualFeedCount < newsBufferSize; )
            {
                News newsItem = new News()
                {
                    Id = item.Current.Id,
                    Title = item.Current.Title.Text,
                    PubDate = item.Current.PublishDate.DateTime,
                    Link = item.Current.Links.FirstOrDefault().Uri.AbsoluteUri,
                    Source = item.Current.SourceFeed.Title.Text
                };

                if (IsActual(ref newsItem))
                {
                    news.Put(newsItem);
                    actualFeedCount++;
                }
            }
        }

        private bool IsActual(ref News news)
        {
            foreach (string keyword in this.keywords)
            {
                if (news.Title.ToLower().Contains(keyword))
                {
                    news.Keywords.Add(keyword);
                }
            }
            if (news.Keywords.Count == 0)
            {
                return false;
            }
            return true;
        }

        private void GotNews(News news)
        {
            NewsUpdated(news);
        }
    }
}