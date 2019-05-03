using Monitor.Communication.Handlers;
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
        public WaitersList Waiters = new WaitersList();
        public readonly DistributedMonitor Parent;

        public object Value { get; set; }
        public ulong ValueTimestamp { set; get; }

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

            CommunicationHandler.Instance.SendWaitMessage(this);

            Parent.Release();

            StopCurrentThread();

            Parent.Acquire();
        }

        public void Signal()
        {
            CommunicationHandler.Instance.SendSignalMessage(this);
        }

        public void SignalAll()
        {
            CommunicationHandler.Instance.SendSignalAllMessage(this);
        }

        public void StopCurrentThread()
        {
            System.Threading.Monitor.Enter(this); // Just to execute wait
            System.Threading.Monitor.Wait(this);
            System.Threading.Monitor.Exit(this);
        }

        public void WakeThread()
        {
            System.Threading.Monitor.Enter(this); // Just to execute wait
            System.Threading.Monitor.Pulse(this);
            System.Threading.Monitor.Exit(this);
        }
    }
}
