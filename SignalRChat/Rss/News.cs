using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Rss
{
    public class News : SimpleNews
    {
        public SimpleNews SimpleNews
        {
            get
            {
                return new SimpleNews()
                {
                    Id = Id,
                    Title = Title,
                    Link = Link,
                    PubDate = PubDate,
                    Source = Source
                };
            }
        }

        public List<string> Keywords = new List<string>();

        public override bool Equals(object obj)
        {
            News target = (News)obj;
            return target.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
    }

    public class SimpleNews
    {
        public string Id;
        public string Title;
        public string Link;
        public DateTime PubDate;
        public string Source;
    }
}