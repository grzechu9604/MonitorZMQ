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
            var random = new Random();
            MonitorConfiguration config = ConfigurationReader.Read(GetConfigPath());
            MonitorWrapper wrapper = MonitorWrapper.Instance;
            wrapper.ApplyConfig(config);
            wrapper.Start();
            DistributedMonitor monitor = wrapper.CreateMonitorIfNotExists(1);

            // Przygotowanie zmiennej warunkowej
            monitor.Acquire();
            var cv = monitor.CreateConditionalVariableIfNotExists(1);
            if (cv.Value == null)
            {
                cv.Value = 0;
            }
            monitor.Release();

            if (wrapper.ID == 0 || wrapper.ID == 1 || wrapper.ID == 2)
            {
                while (true)
                {
                    //READERS
                    monitor.Acquire();
                    while (((int)cv.Value) < 0)
                    {
                        Console.WriteLine($"Wait {wrapper.ID} - pisarz w środku");
                        cv.Wait();
                    }
                    cv.Value = (int)cv.Value + 1;
                    monitor.Release();

                    Console.WriteLine($"Czytam {wrapper.ID}");
                    Thread.Sleep(random.Next(1000, 2000));
                    Console.WriteLine($"Skończyłem {wrapper.ID}");

                    monitor.Acquire();
                    cv.Value = (int)cv.Value - 1;
                    if ((int)cv.Value == 0)
                    {
                        cv.SignalAll();
                    }
                    monitor.Release();

                    Console.WriteLine($"Wyszedłem i czekam {wrapper.ID}");
                    Thread.Sleep(random.Next(3000, 5000));
                }
            }
            else
            {
                //WRITERS
                while (true)
                {
                    monitor.Acquire();
                    while (((int)cv.Value) != 0)
                    {
                        Console.WriteLine($"Wait {wrapper.ID} - ktoś w środku");
                        cv.Wait();
                    }
                    cv.Value = -1;
                    monitor.Release();

                    Console.WriteLine($"Piszę {wrapper.ID}");
                    Thread.Sleep(random.Next(1000, 3000));
                    Console.WriteLine($"Skończyłem {wrapper.ID}");

                    monitor.Acquire();
                    cv.Value = 0;
                    cv.SignalAll();
                    monitor.Release();

                    Console.WriteLine($"Wyszedłem i czekam {wrapper.ID}");
                    Thread.Sleep(random.Next(3000, 5000));
                }
            }
        }
    }
}
