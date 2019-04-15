using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Util
{
    public class EventHandlers
    {
        public delegate void UpdateItem<T>(T newItem);
    }
}