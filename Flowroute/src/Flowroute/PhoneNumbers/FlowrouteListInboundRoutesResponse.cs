using System.Collections.Generic;
using Flowroute.Messaging;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteListInboundRoutesResponse : FlowrouteBaseResponse
    {
        public Dictionary<string, FlowrouteRoute> Routes { get; set; }
    }
}