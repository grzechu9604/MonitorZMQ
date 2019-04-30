using Monitor.Communication.Messages;
using Monitor.Communication.Technic;
using Monitor.SpecificDataTypes;
using Monitor.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Communication.Handlers
{
    class MessageHandler
    {
        #region Singleton
        public static MessageHandler Instance { get; } = new MessageHandler();
        private MessageHandler()
        {

        }
        #endregion

        public static object _currentMessageLock = new object();
        private static ControlMessage _currentMessage;

        public static ControlMessage MyCurrentMessage
        {
            get => _currentMessage;
            set
            {
                lock (_currentMessageLock)
                {
                    _currentMessage = value;
                }
            }
        }

        private readonly Dictionary<MessageTypes, Func<ControlMessage, ControlMessage>> handlers = new Dictionary<MessageTypes, Func<ControlMessage, ControlMessage>>()
        {
            [MessageTypes.MonitorAcquire] = HandleMonitorAcquire,
            [MessageTypes.MonitorRelease] = HandleMonitorRelease
        };

        private static ControlMessage HandleMonitorAcquire(ControlMessage message)
        {
            MessageTypes responseType;
            var wrapper = MonitorWrapper.Instance;
            wrapper.LockMonitors();
            var monitor = MonitorWrapper.Instance.GetMonitor(message.MonitorId);
            if (monitor == null) // let go if you don't have this monitor
            {
                responseType = MessageTypes.Acknowledgement;
            }
            else
            {
                lock (monitor)
                {
                    lock (_currentMessageLock)
                    {
                        if (monitor.IsAcquired
                            || (monitor.IsAcquiring && MyCurrentMessage != null && MyCurrentMessage.Timer < message.Timer)
                            || (monitor.IsAcquiring && MyCurrentMessage != null && MyCurrentMessage.Timer == message.Timer && wrapper.ID < message.SenderId))
                        {
                            responseType = MessageTypes.Negation;
                        }
                        else
                        {
                            responseType = MessageTypes.Acknowledgement;
                        }
                    }
                }
            }

            var responseMessage = MessageFactory.CreateMessage(LamportTimeProvider.Instance.IncrementAndReturn(), message.MonitorId, -1, responseType);

            wrapper.UnlockMonitors();

            return responseMessage;
        }

        private static ControlMessage HandleMonitorRelease(ControlMessage message)
        {
            return MessageFactory.CreateMessage(LamportTimeProvider.Instance.IncrementAndReturn(), message.MonitorId, -1, MessageTypes.Acknowledgement);
        }

        public ControlMessage HandleMessage(ControlMessage message)
        {
            ControlMessage returnMessage;
            try
            {
                LamportTimeProvider.Instance.IncrementAndReturnWithMin(message.Timer);
                returnMessage = handlers[message.Type].Invoke(message);
            }
            catch(KeyNotFoundException)
            {
                returnMessage = MessageFactory.CreateMessage(LamportTimeProvider.Instance.IncrementAndReturn(), -1, -1, MessageTypes.Reset);
            }

            return returnMessage;
        }
    }
}
