using Monitor.Communication.Messages;
using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using ZeroMQ;

namespace Monitor.Communication.IDProviders
{
    public static class IDGetter
    {
        public static int GetId(string serverAddress)
        {
            using (ZContext context = new ZContext())
            using (ZSocket requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(serverAddress);
                ZFrame frame = MessageFactory.CreateMessageZFrame(-1, 0, -1, 0, MessageTypes.IDRequest);
                requester.Send(frame);

                using (ZFrame reply = requester.ReceiveFrame())
                {
                    ControlMessage message = BinarySerializer<ControlMessage>.ToObject(reply.Read());
                    return message.Data;
                }
            }
        }
    }
}
