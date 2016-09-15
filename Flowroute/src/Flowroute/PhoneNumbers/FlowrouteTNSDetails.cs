using System.Collections.Generic;
using Newtonsoft.Json;

namespace Flowroute.PhoneNumbers
{
    public class FlowrouteTNSDetails
    {
        [JsonProperty("initial_cost")]
        public decimal InitialCost { get; set; }
        [JsonProperty("monthly_cost")]
        public decimal MonthlyCost { get; set; }
        public  string State { get; set; }
        public string RateCenter { get; set; }
        [JsonProperty("billing_methods")]
        public List<string> BillingMethods { get; set; }
    }
}