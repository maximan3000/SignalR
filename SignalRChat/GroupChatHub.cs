using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.Rss;

namespace SignalRChat
{

    public class GroupChatHub : ChatHub
    {
        static IHookRss hookRss = new HookRss(10);
        static bool isNewsUpdateHandled = false;
        static ConcurrentDictionary<string, List<string>> clientGroups = 
            new ConcurrentDictionary<string, List<string>>();

        public GroupChatHub()
        {
            if (!isNewsUpdateHandled)
            {
                hookRss.NewsUpdated += NewsUpdated;
                isNewsUpdateHandled = true;
            }
        }

        public void Subscribe(string keyword)
        {
            keyword = keyword.Trim().ToLower();
            Groups.Add(Context.ConnectionId, keyword);
            hookRss.Subscribe(keyword);
            clientGroups.AddOrUpdate(
                Context.ConnectionId, 
                new List<string>() { keyword }, 
                (key, value) =>
                {
                    value.Add(keyword);
                    return value;
                });
            //TODO implement method getRelevantNews on Client
            var relevantNews = hookRss.GetRelevant(keyword);
            Clients.Client(Context.ConnectionId).getRelevantNews(relevantNews);
        }

        public void Unsubscribe(string keyword)
        {
            keyword = keyword.Trim().ToLower();
            Groups.Remove(Context.ConnectionId, keyword);
            hookRss.Unsubscribe(keyword);

            List<string> groups;
            bool result = false;
            clientGroups.TryGetValue(Context.ConnectionId, out groups);
            if (groups != null && groups.Count != 0)
            {
                result = groups.Remove(keyword);
            }
            Clients.Client(Context.ConnectionId).whisper(ServerName, $"You are not subscribed on {keyword}");
        }

        public void GroupCast(string group, string message)
        {
            ClientInfo clientInfo;
            clientsInfo.TryGetValue(Context.ConnectionId, out clientInfo);

            List<string> groups;
            clientGroups.TryGetValue(Context.ConnectionId, out groups);

            if (groups != null && groups.Contains(group))
            {
                //TODO implement method groupCast on Client
                Clients.Group(group).groupCast(clientInfo.Name, group, message);
            }
            else
            {
                Clients.Client(Context.ConnectionId).whisper(ServerName, $"You are not member of {group}");
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            List<string> groups;
            clientGroups.TryRemove(Context.ConnectionId, out groups);
            return base.OnDisconnected(stopCalled);
        }

        private void NewsUpdated(News news)
        {
            foreach (string keyword in news.Keywords)
            {
                //TODO implement method gotNews on Client
                string name = $"(group {keyword}) {news.Source}";
                Clients.Group(keyword).gotNews(name, news.SimpleNews);
            }
        }
    }
}