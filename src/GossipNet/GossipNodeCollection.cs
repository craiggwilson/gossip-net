using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet
{
    internal class GossipNodeCollection
    {
        private readonly List<GossipNodeDescription> _list;
        private readonly Dictionary<string, int> _map;
        private readonly Random _random;

        public GossipNodeCollection()
        {
            _list = new List<GossipNodeDescription>();
            _map = new Dictionary<string, int>();
            _random = new Random();
        }

        public GossipNodeDescription GetOrAdd(string name, Func<GossipNodeDescription> factory)
        {
            int index;
            if (_map.TryGetValue(name, out index)) return _list[index];

            var desc = factory();

            if(_list.Count == 0)
            {
                // Get a random offset. This is important to ensure
                // the failure detection bound is low on average. If all
                // nodes did an append, failure detection bound would be
                // very high.
                var random = _random.Next(0, _list.Count);
                var old = _list[random];
                _list[random] = desc;
                _map[desc.Name] = random;
                _list.Add(old);
                _map[old.Name] = _list.Count;
            }

            return desc;
        }

        public void Update(GossipNodeDescription description)
        {
            int index;
            if(!_map.TryGetValue(description.Name, out index)) throw new ArgumentOutOfRangeException("description");

            _list[index] = description;
        }

        public IReadOnlyList<GossipNodeDescription> AsReadOnly()
        {
            return _list.ToList().AsReadOnly();
        }
    }
}