using Monitor.Communication.Messages;
using Monitor.Communication.Senders;
using Monitor.Configuration;
using Monitor.SpecificDataTypes;
using Monitor.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading;

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
            MonitorWrapper wrapper = MonitorWrapper.Instance;
            wrapper.ApplyConfig(config);
            wrapper.Start();
            wrapper.CreateMonitor(1);
            DistributedMonitor monitor = wrapper.GetMonitor(1);

            while(true)
            { 
            monitor.Acquire();

            Console.WriteLine($"Critical section {wrapper.ID}");
            Thread.Sleep(2000);
            Console.WriteLine($"Relese critical section {wrapper.ID}");

            monitor.Release();
            }
        }
    }
}
