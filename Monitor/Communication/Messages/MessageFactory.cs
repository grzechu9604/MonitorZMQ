using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using Monitor.Wrappers;
using ZeroMQ;

namespace Monitor.Communication.Messages
{
    static class MessageFactory
    {
        public static ControlMessage CreateMessage(ulong timer, int monitorId, int variableId, int signalDestId, MessageTypes type)
        {
            return new ControlMessage()
            {
                SenderId = MonitorWrapper.Instance.ID,
                Timer = timer,
                MonitorId = monitorId,
                ConditionalVariableId = variableId,
                SignalDestination = signalDestId,
                Type = type
            };
        }

        public static ControlMessage CreateMessageWithValuesPropagation(ulong timer, MessageTypes type, DistributedMonitor monitor)
        {
            return new ControlMessage()
            {
                SenderId = MonitorWrapper.Instance.ID,
                Timer = timer,
                MonitorId = monitor.ID,
                Type = type,
                ConditionalVariableValues = monitor.GetConditionalVariablesValues()
            };
        }

        public static ZFrame CreateMessageZFrame(ulong timer, int monitorId, int variableId, int signalDestId, MessageTypes type)
        {
            ControlMessage message = CreateMessage(timer, monitorId, variableId, signalDestId, type);
            return CreateMessageZFrame(message);
        }

        public static ZFrame CreateMessageZFrame(ControlMessage message)
        {
            byte[] messageBytes = BinarySerializer<ControlMessage>.ToByteArray(message);
            return new ZFrame(messageBytes);
        }
    }
}
