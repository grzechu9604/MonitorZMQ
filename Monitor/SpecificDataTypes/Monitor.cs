using Monitor.Communication.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.SpecificDataTypes
{
    class DistributedMonitor
    {
        public readonly int ID;
        private readonly List<ConditionalVariable> ConditionalVariables = new List<ConditionalVariable>();

        public bool IsAcquired { get; private set; }
        public bool IsAcquiring { get; private set; }

        public DistributedMonitor(int id)
        {
            ID = id;
        }

        public void CreateConditionalVariable(int id)
        {
            if (ConditionalVariables.Any(cv => cv.ID.Equals(id)))
            {
                throw new InvalidOperationException("ConditionalVariabl with given ID already exists");
            }

            ConditionalVariables.Add(new ConditionalVariable(id, this));
        }

        public ConditionalVariable GetConditionalVariable(int id)
        {
            return ConditionalVariables.FirstOrDefault(cv => cv.ID.Equals(id));
        }

        public void Acquire()
        {
            lock (this)
            {
                IsAcquiring = true;
            }

            CommunicationHandler.Instance.SendAcquireMessage(this);

            lock (this)
            {
                IsAcquired = true;
                IsAcquiring = false;
            }
        }

        public void Release()
        {
            lock (this)
            {
                IsAcquired = false;
            }

            CommunicationHandler.Instance.SendReleaseMessage(this);
        }
    }
}
