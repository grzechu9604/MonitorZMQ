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

        private const int Timeout = 300;

        public void SendAcquireMessage(DistributedMonitor monitor)
        {
            bool succeded = false;
            var message = MessageFactory.CreateMessage(
                    LamportTimeProvider.Instance.IncrementAndReturn(), monitor.ID, -1, -1, MessageTypes.MonitorAcquire);
            MessageHandler.MyCurrentMessage = message;
            var alreadyAcceptedList = new List<int>();
            while (!succeded)
            {
                monitor.IsPassClearOrWait();
                succeded = MessageSender.Instance.BrodcastMessageWithResult(message, MessageTypes.Acknowledgement, alreadyAcceptedList);
            }
        }

        public void SendReleaseMessage(DistributedMonitor monitor)
        {
            var message = MessageFactory.CreateMessageWithValuesPropagation(
                LamportTimeProvider.Instance.IncrementAndReturn(), MessageTypes.MonitorRelease, monitor);
            MessageSender.Instance.BrodcastMessage(message);
        }

        public void SendWaitMessage(ConditionalVariable cv)
        {
            var message = MessageFactory.CreateMessage(
                LamportTimeProvider.Instance.IncrementAndReturn(), cv.Parent.ID, cv.ID, -1, MessageTypes.Wait);
            MessageSender.Instance.BrodcastMessage(message);
        }

        public void SendSignalMessage(ConditionalVariable cv)
        {
            try
            {
                var firstWaiter = cv.Waiters.GetFirstWaiterAndDeleteHim();
                var message = MessageFactory.CreateMessage(
                    LamportTimeProvider.Instance.IncrementAndReturn(), cv.Parent.ID, cv.ID, firstWaiter.ProcessId, MessageTypes.Signal);
                MessageSender.Instance.BrodcastMessage(message);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Nie ma kogo budzić!");
            }
        }

        public void SendSignalAllMessage(ConditionalVariable cv)
        {
            var message = MessageFactory.CreateMessage(
                LamportTimeProvider.Instance.IncrementAndReturn(), cv.Parent.ID, cv.ID, -1, MessageTypes.SignalAll);
            MessageSender.Instance.BrodcastMessage(message);
        }

    }
}
