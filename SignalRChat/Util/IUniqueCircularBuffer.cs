using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SignalRChat.Util.EventHandlers;

namespace SignalRChat.Util
{
    interface IUniqueCircularBuffer<T>
    {
        void Put(T item);
        IEnumerable<T> Read();

        event UpdateItem<T> Updated;
    }
}
