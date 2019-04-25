using Monitor.Communication.IDProviders;
using Monitor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Wrappers
{
    class MonitorWrapper
    {
        private readonly MonitorConfiguration _config;
        public readonly int ID;

        public MonitorWrapper(MonitorConfiguration config)
        {
            _config = config;

            if (config.IsServer)
            {
                ID = 0;
                IDSetter.RunService(config.ListeningAddress, config.Adresses.Count);
            }
            else
            {
                ID = IDGetter.GetId(config.ServerAddress);
            }
        }
    }
}
