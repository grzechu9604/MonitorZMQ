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
            using (var context = new ZContext())
            using (var requester = new ZSocket(context, ZSocketType.REQ))
            {
                requester.Connect(serverAddress);
                var frame = MessageFactory.CreateMessageZFrame(-1, 0, -1, 0, MessageTypes.IDRequest);
                requester.Send(frame);

                using (ZFrame reply = requester.ReceiveFrame())
                {
                    var message = BinarySerializer<ControlMessage>.ToObject(reply.Read());
                    return message.Data;
                }
            }
        }
    }
}
