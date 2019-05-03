using Monitor.SpecificDataTypes;
using System;
using System.Collections.Generic;

namespace Monitor.Communication.Messages
{
    [Serializable]
    class ControlMessage
    {
        public int MonitorId { get; set; }
        public ulong Timer { get; set; }
        public int SenderId { get; set; }
        public int ConditionalVariableId { get; set; }
        public int SignalDestination { get; set; }
        public MessageTypes Type { get; set; }
        public Dictionary<int, object> ConditionalVariableValues { get; set; }
    }
}
