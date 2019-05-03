using Monitor.Communication.Messages;
using Monitor.Communication.Technic;
using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Monitor.Communication.Senders
{
    class MessageSender
    {
        #region Singleton
        public static MessageSender Instance { get; } = new MessageSender();
        private MessageSender()
        {

        }
        #endregion

        public List<string> Adresses { get; set; }
        public int? SenderID { get; set; }

        public void BrodcastMessage(ControlMessage message)
        {
            if (Adresses == null || Adresses.Count == 0)
            {
                throw new InvalidOperationException("Set Addresses list first!");
            }

            if (!SenderID.HasValue)
            {
                throw new InvalidOperationException("Set SenderID first!");
            }
            
            Adresses.ForEach(address =>
            {
                using (ZSocket requester = new ZSocket(ZContextProvider.GlobalContext, ZSocketType.REQ))
                {
                    requester.Connect(address);
                    requester.Send(new ZFrame(BinarySerializer<ControlMessage>.ToByteArray(message)));

                    using (ZFrame reply = requester.ReceiveFrame())
                    {
                        ControlMessage rcvedMsg = BinarySerializer<ControlMessage>.ToObject(reply.Read());
                    }
                }
            });
        }

        public bool BrodcastMessageWithResult(ControlMessage message, MessageTypes wantedType)
        {
            bool succes = true;

            if (Adresses == null || Adresses.Count == 0)
            {
                throw new InvalidOperationException("Set Addresses list first!");
            }

            if (!SenderID.HasValue)
            {
                throw new InvalidOperationException("Set SenderID first!");
            }

            Adresses.ForEach(address =>
            {
                using (ZSocket requester = new ZSocket(ZContextProvider.GlobalContext, ZSocketType.REQ))
                {
                    requester.Connect(address);
                    requester.Send(new ZFrame(BinarySerializer<ControlMessage>.ToByteArray(message)));

                    using (ZFrame reply = requester.ReceiveFrame())
                    {
                        ControlMessage rcvedMsg = BinarySerializer<ControlMessage>.ToObject(reply.Read());
                        succes = succes && rcvedMsg.Type.Equals(wantedType);
                    }
                }
            });

            return succes;
        }
    }
}
