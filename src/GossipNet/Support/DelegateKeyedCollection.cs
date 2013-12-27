using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Support
{
    internal class DelegateKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> _getKeyForItem;

        public DelegateKeyedCollection(Func<TItem, TKey> getKeyForItem)
        {
            Debug.Assert(getKeyForItem != null);

            _getKeyForItem = getKeyForItem;
        }

        public bool TryGetValue(TKey key, out TItem item)
        {
            if(Dictionary == null)
            {
                item = default(TItem);
                return false;
            }

            return Dictionary.TryGetValue(key, out item);
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return _getKeyForItem(item);
        }
    }
}