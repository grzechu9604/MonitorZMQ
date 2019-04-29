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
                IDSetter.RunService(config.ServerAddress, config.Adresses.Count);
            }
            else
            {
                ID = IDGetter.GetId(config.ServerAddress);
            }

            Console.WriteLine(ID);
            Console.WriteLine(config.ListeningAddress);

            MessageListener.Instance.ListeningAddress = config.ListeningAddress;
            MessageListener.Instance.ListenerID = ID;
            MessageListener.Instance.StartListening();

            ControlMessage testMessage = MessageFactory.CreateMessage(0, 10, 10, 10, SpecificDataTypes.MessageTypes.TEST_REQ);

            MessageSender.Instance.Adresses = config.Adresses;
            MessageSender.Instance.SenderID = ID;
            MessageSender.Instance.BrodcastMessage(testMessage);
        }
    }
}
