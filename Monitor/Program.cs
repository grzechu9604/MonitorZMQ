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
            wrapper.CreateMonitorIfNotExists(1);
            DistributedMonitor monitor = wrapper.GetMonitor(1);

            while(true)
            { 
                monitor.Acquire();
                Console.WriteLine($"Critical section {wrapper.ID}");
                Thread.Sleep(1000);
                var cv = monitor.CreateConditionalVariableIfNotExists(1);
                if (wrapper.ID == 0 || wrapper.ID == 1 || wrapper.ID == 2)
                {
                    Console.WriteLine($"Wait {wrapper.ID}");
                    cv.Wait();
                    Console.WriteLine($"After Wait {wrapper.ID}");
                    Thread.Sleep(1000);
                }
                else if (wrapper.ID == 23)
                {
                    Console.WriteLine($"Signal {wrapper.ID}");
                    cv.Signal();
                    Console.WriteLine($"After signal {wrapper.ID}");
                }
                else
                {
                    Console.WriteLine($"Signal all {wrapper.ID}");
                    cv.SignalAll();
                    Console.WriteLine($"After Signal all {wrapper.ID}");
                }
                Console.WriteLine($"Relese critical section {wrapper.ID}");

                monitor.Release();
            }
        }
    }
}
