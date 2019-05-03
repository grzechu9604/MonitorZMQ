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
        private readonly List<int> _passed = new List<int>();

        public bool IsAcquired { get; private set; } = false;
        public bool IsAcquiring { get; private set; } = false;

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
                CommunicationHandler.Instance.SendReleaseMessage(this);
            }
        }

        public Dictionary<int, object> GetConditionalVariablesValues()
        {
            return ConditionalVariables.ToDictionary(cv => cv.ID, cv => cv.Value);
        }

        public void Pass(int id)
        {
            _passed.Add(id);
        }

        public void DeleteFromPass(int id)
        {
            _passed.Remove(id);
            if (!_passed.Any())
            {
                System.Threading.Monitor.Pulse(this);
            }
        }

        public void IsPassClearOrWait()
        {
            lock(this)
            { 
                while (_passed.Any())
                {
                    System.Threading.Monitor.Wait(this);
                }
            }
        }

        public bool PassedContains(int id)
        {
            return _passed.Contains(id);
        }
    }
}
