using Monitor.Communication.Messages;
using Monitor.Serialization;
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

            message.SenderId = SenderID.Value;

            using (ZContext context = new ZContext())
            using (ZSocket requester = new ZSocket(context, ZSocketType.REQ))
            {
                Adresses.ForEach(address => { 
                    requester.Connect(address);
                    ZFrame frame = new ZFrame();
                    requester.Send(new ZFrame(BinarySerializer<ControlMessage>.ToByteArray(message)));
                    Console.WriteLine($"MS wysłane {message.Type} do {address}");

                    using (ZFrame reply = requester.ReceiveFrame())
                    {
                        ControlMessage rcvedMsg = BinarySerializer<ControlMessage>.ToObject(reply.Read());
                        Console.WriteLine($"MS {rcvedMsg.Type} odebrane od {rcvedMsg.SenderId}");
                    }
                });
            }
        }
    }
}
