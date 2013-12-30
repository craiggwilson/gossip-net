using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public class RandomKGossipMemberSelector : IGossipMemberSelector
    {
        private readonly int _k;
        private readonly Random _random;

        public RandomKGossipMemberSelector(int k)
        {
            Debug.Assert(k > 0);

            _k = k;
            _random = new Random();
        }

        public IEnumerable<SwimMember> Select(IReadOnlyList<SwimMember> members)
        {
            var list = new List<SwimMember>();
            for(int i = 0; i < _k && i < members.Count; i++)
            {
                var r = _random.Next(0, members.Count);
                list.Add(members[r]);
            }

            return list;
        }
    }
}