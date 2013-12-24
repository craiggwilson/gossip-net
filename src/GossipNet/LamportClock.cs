using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GossipNet
{
    public class LamportClock
    {
        private long _current;

        public LamportClock()
        {
            _current = 0;
        }

        public long Current
        {
            get { return Interlocked.Read(ref _current); }
        }

        public long Increment()
        {
            return Interlocked.Increment(ref _current);
        }

        public long Witness(long value)
        {
            var cur = Interlocked.Read(ref _current);
            if (value < cur) return cur;

            if(Interlocked.CompareExchange(ref _current, value + 1, cur) != cur)
            {
                return Witness(value);
            }

            return value + 1;
        }
    }
}