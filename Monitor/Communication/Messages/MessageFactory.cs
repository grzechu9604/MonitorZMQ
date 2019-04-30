using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using ZeroMQ;

namespace Monitor.Communication.Messages
{
    static class MessageFactory
    {
        public static ControlMessage CreateMessage(ulong timer, int monitorId, int data, MessageTypes type)
        {
            return new ControlMessage()
            {
                SenderId = -1,
                Timer = timer,
                MonitorId = monitorId,
                Data = data,
                Type = type
            };
        }

        public static ZFrame CreateMessageZFrame(ulong timer, int monitorId, int data, MessageTypes type)
        {
            ControlMessage message = CreateMessage(timer, monitorId, data, type);
            byte[] messageBytes = BinarySerializer<ControlMessage>.ToByteArray(message);
            return new ZFrame(messageBytes);
        }
    }
}
