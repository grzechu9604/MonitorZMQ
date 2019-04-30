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

        public ConditionalVariable CreateConditionalVariableIfNotExists(int id)
        {
            var variable = GetConditionalVariable(id);
            if (variable != null)
            {
                return variable;
            }
            variable = new ConditionalVariable(id, this);
            ConditionalVariables.Add(variable);
            return variable;
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
