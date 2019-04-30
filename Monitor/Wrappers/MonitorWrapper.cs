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
        public int ID { get; private set; } = -1;
        private readonly List<DistributedMonitor> Monitors = new List<DistributedMonitor>();
        private readonly object _monitorsLock = new object();

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

        public DistributedMonitor CreateMonitorIfNotExists(int id)
        {
            System.Threading.Monitor.Enter(_monitorsLock);

            var monitor = GetMonitor(id);
            if (monitor != null)
            {
                return monitor;
            }

            monitor = new DistributedMonitor(id);

            Monitors.Add(monitor);

            System.Threading.Monitor.Exit(_monitorsLock);

            return monitor;
        }

        public void LockMonitors()
        {
            System.Threading.Monitor.Enter(_monitorsLock);
        }

        public void UnlockMonitors()
        {
            System.Threading.Monitor.Exit(_monitorsLock);
        }

        public DistributedMonitor GetMonitor(int id)
        {
            return Monitors.FirstOrDefault(m => m.ID.Equals(id));
        }
    }
}
