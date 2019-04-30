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
        private MonitorConfiguration _config;
        public int ID { get; private set; }
        private readonly List<DistributedMonitor> Monitors = new List<DistributedMonitor>();

        #region Singleton
        public static MonitorWrapper Instance { get; } = new MonitorWrapper();
        private MonitorWrapper()
        {
        }
        #endregion

        public void ApplyConfig(MonitorConfiguration config)
        {
            if (_config != null)
            {
                throw new InvalidOperationException("You can apply config only once!");
            }

            _config = config;
        }

        public void Start()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("Apply config first!");
            }

            if (_config.IsServer)
            {
                ID = 0;
                IDSetter.RunService(_config.ServerAddress, _config.Adresses.Count);
            }
            else
            {
                ID = IDGetter.GetId(_config.ServerAddress);
            }

            Console.WriteLine(ID);
            Console.WriteLine(_config.ListeningAddress);

            MessageListener.Instance.ListeningAddress = _config.ListeningAddress;
            MessageListener.Instance.ListenerID = ID;
            MessageListener.Instance.StartListening();
            MessageSender.Instance.Adresses = _config.Adresses;
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
