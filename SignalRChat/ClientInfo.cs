using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class ClientInfo
    {
        public string Name { get; set; }
        public int clientsMessagesGot = 0;
        public int messagesSend = 0;
        public TimeSpan LiveTime {
            get
            {
                return DateTime.Now - connectedTime;
            }
        }

        private DateTime connectedTime;

        public ClientInfo(string name, DateTime connectedTime)
        {
            Name = name;
            this.connectedTime = connectedTime;
        }
    }
}