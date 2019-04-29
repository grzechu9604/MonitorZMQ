using Monitor.Communication.Messages;
using Monitor.Communication.Technic;
using Monitor.Serialization;
using Monitor.SpecificDataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace Monitor.Communication.Listeners
{
    class MessageListener
    {
        #region Singleton

        public static MessageListener Instance { get; } = new MessageListener();
        private MessageListener()
        {

        }

        #endregion

        private Thread _listeningThread;

        private string _listeningAddress;
        public string ListeningAddress
        {
            get => _listeningAddress; set
            {
                if (_listeningThread != null && _listeningThread.IsAlive)
                {
                    _listeningThread.Abort();
                }
                _listeningAddress = value;
            }
        }

        public int? ListenerID { get; set; }


        public void Listen()
        {
            using (ZSocket responser = new ZSocket(ZContextProvider.GlobalContext, ZSocketType.REP))
            {
                responser.Bind(ListeningAddress);
                while (true)
                {
                    ZFrame frame = responser.ReceiveFrame();
                    ControlMessage message = BinarySerializer<ControlMessage>.ToObject(frame.Read());
                    Console.WriteLine($"ML {message.Type} odebrane od {message.SenderId}");

                    MessageTypes responseType = MessageTypes.TEST_RESP;
                    using (ZFrame responseFrame = MessageFactory.CreateMessageZFrame(ListenerID.Value, 0, -1, 0, responseType))
                    {
                        responser.Send(responseFrame);
                    }
                    Console.WriteLine($"ML {responseType} wysłane do {message.SenderId}");
                }
            }
        }

        public void StartListening()
        {
            if (string.IsNullOrWhiteSpace(_listeningAddress))
            {
                throw new InvalidOperationException("Set ListeningAddress first!");
            }

            if (!ListenerID.HasValue)
            {
                throw new InvalidOperationException("Set ListenerID first!");
            }

            _listeningThread = new Thread(new ThreadStart(Listen));
            _listeningThread.Start();
        }
    }
}
