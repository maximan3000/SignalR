using System.Collections.Generic;
using System.Linq;
using static SignalRChat.Util.EventHandlers;

namespace SignalRChat.Util
{
    public class ConcurrentUniqueCircularBuffer<T> : IUniqueCircularBuffer<T>
    {
        public event UpdateItem<T> Updated;

        private readonly LinkedList<T> _buffer;
        private int _maxItemCount;

        public ConcurrentUniqueCircularBuffer(int maxItemCount)
        {
            _maxItemCount = maxItemCount;
            _buffer = new LinkedList<T>();
        }

        public void Put(T item)
        {
            lock (_buffer)
            {
                if (_buffer.Contains(item))
                {
                    return;
                }
            }
            PutUnsafe(item);
        }

        public IEnumerable<T> Read()
        {
            lock (_buffer) { return _buffer.ToArray(); }
        }

        protected void PutUnsafe(T item)
        {
            lock (_buffer)
            {
                _buffer.AddFirst(item);
                if (_buffer.Count > _maxItemCount)
                {
                    _buffer.RemoveLast();
                }
            }
            Updated(item);
        }
    }
}