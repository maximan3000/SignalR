using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SignalRChat.Util.EventHandlers;

namespace SignalRChat.Rss
{
    interface IHookRss
    {
        bool Subscribe(string keyword);
        bool Unsubscribe(string keyword);
        IEnumerable<SimpleNews> GetRelevant(string keyword);

        event UpdateItem<News> NewsUpdated;
    }
}
