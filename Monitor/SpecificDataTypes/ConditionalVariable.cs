using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.SpecificDataTypes
{
    class ConditionalVariable
    {
        public readonly int ID;
        private readonly DistributedMonitor Parent;

        public ConditionalVariable(int id, DistributedMonitor monitor)
        {
            ID = id;
            Parent = monitor;
        }

        public void Wait()
        {
            if (!Parent.IsAcquired)
            {
                throw new InvalidOperationException("Not owner exception");
            }

            Parent.Release();

            //TODO: Oczekiwanie na sygnał od innego procesu

            Parent.Acquire();
        }

        public void Signal()
        {

        }

        public void SignalAll()
        {

        }
    }
}
