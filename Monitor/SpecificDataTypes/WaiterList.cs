using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.SpecificDataTypes
{
    class WaitersList
    {
        private readonly List<Waiter> _waiters = new List<Waiter>();
        private ulong _lastClearAllTimestamp = 0;

        public Waiter GetFirstWaiterAndDeleteHim()
        {
            var waiter = _waiters.First();
            _waiters.RemoveAll(w => w.ProcessId.Equals(waiter.ProcessId));
            return waiter;
        }

        public void Add(Waiter w)
        {
            if (_lastClearAllTimestamp <= w.Timestamp)
            {
                _waiters.Add(w);
            }
        }

        public void RemoveWaiter(int processId, ulong timestamp)
        {
            _waiters.RemoveAll(e => e.ProcessId.Equals(processId) && e.Timestamp < timestamp);
        }

        public void RemoveAllWaiters(ulong timestamp)
        {
            _lastClearAllTimestamp = timestamp;
            _waiters.RemoveAll(e => e.Timestamp < timestamp);
        }
    }
}
