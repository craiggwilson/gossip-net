using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Support
{
    internal class ListMap<TKey, T> : IList<T>
    {
        private readonly Func<T, TKey> _keyProvider;
        private readonly List<T> _list;
        private readonly Dictionary<TKey, int> _map;

        public ListMap(Func<T, TKey> keyProvider)
        {
            _keyProvider = keyProvider;
            _list = new List<T>();
            _map = new Dictionary<TKey, int>();
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set 
            {
                var oldItem = _list[index];
                _list[index] = value;
                _map.Remove(_keyProvider(oldItem));
                _map[_keyProvider(value)] = index;
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            var key = _keyProvider(item);
            _map.Add(key, _list.Count);
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            _map.Clear();
        }

        public bool Contains(T item)
        {
            var key = _keyProvider(item);
            return _map.ContainsKey(key);
        }

        public bool ContainsKey(TKey key)
        {
            return _map.ContainsKey(key);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            var key = _keyProvider(item);
            int index;
            return _map.TryGetValue(key, out index)
                ? index
                : -1;
        }

        public void Insert(int index, T item)
        {
            var key = _keyProvider(item);

            var existingItem = _list[index];
            var existingKey = _keyProvider(existingItem);

            _map.Remove(existingKey);
            _map[key] = index;
            _list[index] = item;
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            var key = _keyProvider(item);
            _list.RemoveAt(index);
            _map.Remove(key);
        }

        public bool Remove(T item)
        {
            var key = _keyProvider(item);
            int index;
            if(!_map.TryGetValue(key, out index))
            {
                return false;
            }

            _map.Remove(key);
            _list.RemoveAt(index);
            return true;
        }

        public bool TryGetValue(TKey key, out T item)
        {
            int index;
            if(_map.TryGetValue(key, out index))
            {
                item = _list[index];
                return true;
            }

            item = default(T);
            return false;
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
