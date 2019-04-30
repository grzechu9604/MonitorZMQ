using Monitor.Communication.IDProviders;
using Monitor.Communication.Listeners;
using Monitor.Communication.Messages;
using Monitor.Communication.Senders;
using Monitor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitor.SpecificDataTypes;

namespace Monitor.Wrappers
{
    class MonitorWrapper
    {
        private readonly MonitorConfiguration _config;
        public readonly int ID;
        private readonly List<DistributedMonitor> Monitors = new List<DistributedMonitor>();

        public MonitorWrapper(MonitorConfiguration config)
        {
            _config = config;

            if (config.IsServer)
            {
                ID = 0;
                IDSetter.RunService(config.ServerAddress, config.Adresses.Count);
            }
            else
            {
                ID = IDGetter.GetId(config.ServerAddress);
            }

            Console.WriteLine(ID);
            Console.WriteLine(config.ListeningAddress);

            MessageListener.Instance.ListeningAddress = config.ListeningAddress;
            MessageListener.Instance.ListenerID = ID;
            MessageListener.Instance.StartListening();
            MessageSender.Instance.Adresses = config.Adresses;
            MessageSender.Instance.SenderID = ID;
        }

        public void CreateMonitor(int id)
        {
            if (Monitors.Any(m => m.ID.Equals(id)))
            {
                throw new InvalidOperationException("Monitor already exists!");
            }

            Monitors.Add(new DistributedMonitor(id));
        }

        public DistributedMonitor GetMonitor(int id)
        {
            return Monitors.FirstOrDefault(m => m.ID.Equals(id));
        }
    }
}
