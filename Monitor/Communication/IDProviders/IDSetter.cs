using Monitor.Communication.Messages;
using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using ZeroMQ;

namespace Monitor.Communication.IDProviders
{
    public static class IDSetter
    {
        public static void RunService(string listeningAddress, int processesAmount)
        {
            using (var context = new ZContext())
            {
                using (var responser = new ZSocket(context, ZSocketType.REP))
                {
                    responser.Bind(listeningAddress);

                    for (int i = 1; i < processesAmount + 1; i++)
                    {
                        var frame = responser.ReceiveFrame();
                        var message = BinarySerializer<ControlMessage>.ToObject(frame.Read());

                        int idToResponse = i;
                        MessageTypes responseType = MessageTypes.IDSet;

                        if (message.Type != MessageTypes.IDRequest)
                        {
                            idToResponse = -1;
                            responseType = MessageTypes.Reset;
                            i--;
                        }

                        var responseFrame = MessageFactory.CreateMessageZFrame(0, 0, -1, idToResponse, responseType);
                        responser.Send(responseFrame);
                    }
                }
            }
        }
    }
}
