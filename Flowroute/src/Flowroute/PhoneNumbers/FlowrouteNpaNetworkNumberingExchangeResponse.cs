using System.Collections.Generic;
using Flowroute.Internal;
using Flowroute.Messaging;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteNpaNetworkNumberingExchangeResponse : FlowrouteBaseResponse
    {
        public FlowrouteLinks Links { get; set; }
        public Dictionary<string, FlowrouteNpaDetails> NPANXXS { get; set; }
    }
}