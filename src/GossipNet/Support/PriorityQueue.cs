using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Support
{
    internal class PriorityQueue<TKey, T>
    {
        private readonly SortedDictionary<TKey, Queue<T>> _items;

        public PriorityQueue()
        {
            _items = new SortedDictionary<TKey, Queue<T>>();
        }

        public bool HasItems
        {
            get { return _items.Count > 0; }
        }

        public T Dequeue()
        {
            if(_items.Count == 0) throw new InvalidOperationException("No items available to dequeue.");

            var pair = _items.First();
            var q = pair.Value;

            var item = q.Dequeue();
            if (q.Count == 0)
            {
                _items.Remove(pair.Key);
            }

            return item;
        }

        public void Enqueue(TKey priority, T item)
        {
            Queue<T> q;
            if (!_items.TryGetValue(priority, out q))
            {
                _items[priority] = q = new Queue<T>();
            }

            q.Enqueue(item);
        }

        public T Peek()
        {
            if(_items.Count == 0) throw new InvalidOperationException("Cannot peek if not items exist.");

            return _items.First().Value.Peek();
        }
    }
}