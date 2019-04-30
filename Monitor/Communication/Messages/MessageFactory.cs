using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using Monitor.Wrappers;
using ZeroMQ;

namespace Monitor.Communication.Messages
{
    static class MessageFactory
    {
        public static ControlMessage CreateMessage(ulong timer, int monitorId, int data, MessageTypes type)
        {
            return new ControlMessage()
            {
                SenderId = MonitorWrapper.Instance.ID,
                Timer = timer,
                MonitorId = monitorId,
                Data = data,
                Type = type
            };
        }

        public static ZFrame CreateMessageZFrame(ulong timer, int monitorId, int data, MessageTypes type)
        {
            ControlMessage message = CreateMessage(timer, monitorId, data, type);
            return CreateMessageZFrame(message);
        }

        public static ZFrame CreateMessageZFrame(ControlMessage message)
        {
            byte[] messageBytes = BinarySerializer<ControlMessage>.ToByteArray(message);
            return new ZFrame(messageBytes);
        }
    }
}
