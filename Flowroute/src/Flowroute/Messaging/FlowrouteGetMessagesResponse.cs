using System.Collections.Generic;
using Flowroute.Internal;

namespace Flowroute.Messaging
{
    public class FlowrouteGetMessagesResponse : FlowrouteBaseResponse
    {
        public IEnumerable<SmsMessageData> Data { get; set; }
        public FlowrouteLinks Links { get; set; }
    }
}