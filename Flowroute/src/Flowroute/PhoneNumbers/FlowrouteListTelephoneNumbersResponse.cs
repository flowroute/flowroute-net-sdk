using System.Collections.Generic;
using Flowroute.Internal;
using Flowroute.Messaging;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteListTelephoneNumbersResponse : FlowrouteBaseResponse
    {
        public FlowrouteLinks Links { get; set; }   
        public Dictionary<string, FlowrouteTelephoneNumberDetails> TNS { get; set; }
    }
}