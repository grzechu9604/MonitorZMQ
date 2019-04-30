using Monitor.Communication.Messages;
using Monitor.Communication.Senders;
using Monitor.Communication.Technic;
using Monitor.SpecificDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Communication.Handlers
{
    class CommunicationHandler
    {
        #region Singleton
        public static CommunicationHandler Instance { get; } = new CommunicationHandler();
        private CommunicationHandler()
        {

        }
        #endregion

        public void SendAcquireMessage(DistributedMonitor monitor)
        {
            var message = MessageFactory.CreateMessage(
                LamportTimeProvider.Instance.IncrementAndReturn(), monitor.ID, -1, MessageTypes.MonitorAcquire);
            MessageSender.Instance.BrodcastMessage(message);
        }

        public void SendReleaseMessage(DistributedMonitor monitor)
        {
            var message = MessageFactory.CreateMessage(
                LamportTimeProvider.Instance.IncrementAndReturn(), monitor.ID, -1, MessageTypes.MonitorRelease);
            MessageSender.Instance.BrodcastMessage(message);
        }
    }
}
