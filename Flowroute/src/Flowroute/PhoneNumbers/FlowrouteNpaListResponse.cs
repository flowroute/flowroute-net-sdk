using System.Collections.Generic;
using Flowroute.Internal;
using Flowroute.Messaging;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteNpaListResponse : FlowrouteBaseResponse
    {
        public FlowrouteLinks Links { get; set; }
        public Dictionary<string, FlowrouteNpaDetails> Npas { get; set; }
    }
}