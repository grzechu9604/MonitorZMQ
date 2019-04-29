using System.IO;
using System.Xml.Serialization;

namespace Monitor.Configuration
{
    static class ConfigurationReader
    {
        public static MonitorConfiguration Read(string path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MonitorConfiguration));
            using (StreamReader reader = new StreamReader(path))
            {
                return ser.Deserialize(reader) as MonitorConfiguration;
            }
        }
    }
}
