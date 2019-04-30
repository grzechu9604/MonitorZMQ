using Monitor.Communication.Messages;
using Monitor.Communication.Technic;
using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using ZeroMQ;

namespace Monitor.Communication.IDProviders
{
    public static class IDSetter
    {
        public static void RunService(string listeningAddress, int processesAmount)
        {
            using (ZSocket responser = new ZSocket(ZContextProvider.GlobalContext, ZSocketType.REP))
            {
                responser.Bind(listeningAddress);

                for (int i = 1; i < processesAmount + 1; i++)
                {
                    ZFrame frame = responser.ReceiveFrame();
                    ControlMessage message = BinarySerializer<ControlMessage>.ToObject(frame.Read());

                    int idToResponse = i;
                    MessageTypes responseType = MessageTypes.IDSet;

                    if (message.Type != MessageTypes.IDRequest)
                    {
                        idToResponse = -1;
                        responseType = MessageTypes.Reset;
                        i--;
                    }

                    ZFrame responseFrame = MessageFactory.CreateMessageZFrame(0, -1, -1, idToResponse, responseType);
                    responser.Send(responseFrame);
                }
            }
        }
    }
}
