using Monitor.Communication.Messages;
using Monitor.Communication.Senders;
using Monitor.Communication.Technic;
using Monitor.SpecificDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private const int Timeout = 1000;

        public void SendAcquireMessage(DistributedMonitor monitor)
        {
            bool succeded = false;
            var message = MessageFactory.CreateMessage(
                    LamportTimeProvider.Instance.IncrementAndReturn(), monitor.ID, -1, MessageTypes.MonitorAcquire);
            MessageHandler.MyCurrentMessage = message;
            while (!succeded)
            {
                succeded = MessageSender.Instance.BrodcastMessageWithResult(message, MessageTypes.Acknowledgement);
                if (!succeded)
                {
                    Thread.Sleep(Timeout);
                }
            }

        }

        public void SendReleaseMessage(DistributedMonitor monitor)
        {
            var message = MessageFactory.CreateMessage(
                LamportTimeProvider.Instance.IncrementAndReturn(), monitor.ID, -1, MessageTypes.MonitorRelease);
            MessageSender.Instance.BrodcastMessage(message);
        }
    }
}
