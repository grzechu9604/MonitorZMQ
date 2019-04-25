using Monitor.SpecificDataTypes;
using System;

namespace Monitor.Communication.Messages
{
    [Serializable]
    class ControlMessage
    {
        public int MonitorId { get; set; }
        public ulong Timer { get; set; }
        public int SenderId { get; set; }
        public int Data { get; set; }
        public MessageTypes Type { get; set; }
    }
}
