using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using ZeroMQ;

namespace Monitor.Communication.Messages
{
    static class MessageFactory
    {
        public static ControlMessage CreateMessage(int senderId, ulong timer, int monitorId, int data, MessageTypes type)
        {
            return new ControlMessage()
            {
                SenderId = senderId,
                Timer = timer,
                MonitorId = monitorId,
                Data = data,
                Type = type
            };
        }

        public static ZFrame CreateMessageZFrame(int senderId, ulong timer, int monitorId, int data, MessageTypes type)
        {
            var message = CreateMessage(senderId, timer, monitorId, data, type);
            var messageBytes = BinarySerializer<ControlMessage>.ToByteArray(message);
            return new ZFrame(messageBytes);
        }
    }
}
