using System;
using System.Collections.Generic;

namespace Monitor.Configuration
{
    [Serializable]
    public class MonitorConfiguration
    {
        public MonitorConfiguration()
        {
            Adresses = new List<string>();
        }

        public List<string> Adresses { get; set; }
        public string ServerAddress { get; set; }
        public string ListeningAddress { get; set; }
        public bool IsServer { get; set; }
    }
}
