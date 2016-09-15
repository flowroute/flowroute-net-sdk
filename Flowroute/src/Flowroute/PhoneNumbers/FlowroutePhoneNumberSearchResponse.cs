using System.Collections.Generic;
using Flowroute.Internal;
using Flowroute.Messaging;

namespace Flowroute.PhoneNumbers
{
    public class FlowroutePhoneNumberSearchResponse : FlowrouteBaseResponse
    {
        public FlowrouteLinks Links { get; set; }
        public Dictionary<string, FlowrouteTNSDetails> TNS { get; set; }
    }
}