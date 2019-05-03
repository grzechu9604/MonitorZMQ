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

        private readonly Dictionary<MessageTypes, Func<ControlMessage, ulong, ControlMessage>> handlers = new Dictionary<MessageTypes, Func<ControlMessage, ulong, ControlMessage>>()
        {
            [MessageTypes.MonitorAcquire] = HandleMonitorAcquire,
            [MessageTypes.MonitorRelease] = HandleMonitorRelease,
            [MessageTypes.Wait] = HandleWait,
            [MessageTypes.Signal] = HandleSignal,
            [MessageTypes.SignalAll] = HandleSignalAll
        };

        private static ControlMessage HandleWait(ControlMessage message, ulong timer)
        {
            var monitor = MonitorWrapper.Instance.CreateMonitorIfNotExists(message.MonitorId);
            var waiter = new Waiter()
            {
                Timestamp = message.Timer,
                ProcessId = message.SenderId
            };
            monitor.CreateConditionalVariableIfNotExists(message.ConditionalVariableId).Waiters.Add(waiter);
            return MessageFactory.CreateMessage(timer, message.MonitorId, message.ConditionalVariableId, -1, MessageTypes.Acknowledgement);
        }

        private static ControlMessage HandleSignal(ControlMessage message, ulong timer)
        {
            var monitor = MonitorWrapper.Instance.CreateMonitorIfNotExists(message.MonitorId);
            var variable = monitor.CreateConditionalVariableIfNotExists(message.ConditionalVariableId);
            variable.Waiters.RemoveWaiter(message.SignalDestination, message.Timer);

            if (message.SignalDestination.Equals(MonitorWrapper.Instance.ID))
            {
                variable.WakeThread();
            }

            return MessageFactory.CreateMessage(timer, message.MonitorId, message.ConditionalVariableId, -1, MessageTypes.Acknowledgement);
        }

        private static ControlMessage HandleSignalAll(ControlMessage message, ulong timer)
        {
            var monitor = MonitorWrapper.Instance.CreateMonitorIfNotExists(message.MonitorId);
            var variable = monitor.CreateConditionalVariableIfNotExists(message.ConditionalVariableId);
            variable.Waiters.RemoveAllWaiters(message.Timer);

            variable.WakeThread();

            return MessageFactory.CreateMessage(timer, message.MonitorId, message.ConditionalVariableId, -1, MessageTypes.Acknowledgement);
        }

        private static ControlMessage HandleMonitorAcquire(ControlMessage message, ulong timer)
        {
            MessageTypes responseType;
            MonitorWrapper.Instance.LockMonitors();
            var monitor = MonitorWrapper.Instance.CreateMonitorIfNotExists(message.MonitorId);
            lock (monitor)
            {
                lock (_currentMessageLock)
                {
                    if (monitor.PassedContains(message.SenderId))
                    {
                        // konsekwente przepuszczanie
                        //Console.WriteLine($"Ustępuję {message.SenderId}");
                        responseType = MessageTypes.Acknowledgement;
                    }
                    else if (monitor.IsAcquired
                        || (monitor.IsAcquiring && MyCurrentMessage != null && MyCurrentMessage.Timer < message.Timer)
                        || (monitor.IsAcquiring && MyCurrentMessage != null && MyCurrentMessage.Timer == message.Timer && MonitorWrapper.Instance.ID < message.SenderId))
                    {
                        //Console.WriteLine($"Nie ustępuję {message.SenderId}");
                        responseType = MessageTypes.Negation;
                    }
                    else
                    {
                        //Console.WriteLine($"Ustępuję {message.SenderId}");
                        responseType = MessageTypes.Acknowledgement;
                        monitor.Pass(message.SenderId);
                    }
                }
            }

            var responseMessage = MessageFactory.CreateMessage(timer, message.MonitorId, -1, -1, responseType);

            MonitorWrapper.Instance.UnlockMonitors();

            return responseMessage;
        }

        private static ControlMessage HandleMonitorRelease(ControlMessage message, ulong timer)
        {
            MonitorWrapper.Instance.LockMonitors();
            var monitor = MonitorWrapper.Instance.CreateMonitorIfNotExists(message.MonitorId);

            lock (monitor)
            {
                if (message.ConditionalVariableValues != null)
                {
                    message.ConditionalVariableValues.ToList().ForEach(e =>
                    {
                        var cv = monitor.CreateConditionalVariableIfNotExists(e.Key);
                        if (cv.ValueTimestamp <= message.Timer)
                        {
                            cv.Value = e.Value;
                            cv.ValueTimestamp = message.Timer;
                        }
                    });
                }

                monitor.DeleteFromPass(message.SenderId);
            }
            MonitorWrapper.Instance.UnlockMonitors();

            return MessageFactory.CreateMessage(timer, message.MonitorId, -1, -1, MessageTypes.Acknowledgement);
        }

        public ControlMessage HandleMessage(ControlMessage message)
        {
            var timer = LamportTimeProvider.Instance.IncrementAndReturnWithMin(message.Timer);
            ControlMessage returnMessage;
            try
            {
                returnMessage = handlers[message.Type].Invoke(message, timer);
            }
            catch (KeyNotFoundException)
            {
                returnMessage = MessageFactory.CreateMessage(timer, -1, -1, -1, MessageTypes.Reset);
            }

            return returnMessage;
        }
    }
}
