using System.Collections.Generic;
using Flowroute.Messaging;
using Newtonsoft.Json;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteTelephoneNumberDetailResponse : FlowrouteBaseResponse
    {
        public List<FlowrouteRoute> Routes { get; set; }
        [JsonProperty("billing_method")]
        public string BillingMethod { get; set; }
        public string Detail { get; set; }
    }
}