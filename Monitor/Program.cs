using Monitor.Configuration;
using Monitor.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZeroMQ;

namespace Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationReader.Read("config.xml");
            var bytes = BinarySerializator<MonitorConfiguration>.ToByteArray(config);
            var config2 = BinarySerializator<MonitorConfiguration>.ToObject(bytes);
        }
    }
}
