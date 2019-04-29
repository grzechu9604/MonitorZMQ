using Monitor.Communication.Messages;
using Monitor.Communication.Senders;
using Monitor.Configuration;
using Monitor.Wrappers;
using System;
using System.Collections.Generic;

namespace Monitor
{
    class Program
    {
        static readonly Dictionary<int, string> Configs = new Dictionary<int, string>
        {
            [1] = "config.xml",
            [2] = "config2.xml",
            [3] = "config3.xml",
            [4] = "config4.xml",
        };

        static string GetConfigPath()
        {
            short key = Convert.ToInt16(Console.ReadLine());
            return Configs[key];
        }

        static void Main(string[] args)
        {
            MonitorConfiguration config = ConfigurationReader.Read(GetConfigPath());
            MonitorWrapper wrapper = new MonitorWrapper(config);
        }
    }
}
