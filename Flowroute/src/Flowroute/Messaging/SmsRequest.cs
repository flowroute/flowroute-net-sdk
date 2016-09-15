using Newtonsoft.Json;

namespace Flowroute.Messaging
{
    public class SmsRequest
    {
        [JsonProperty("to")]
        public string ToPhoneNumber { get; set; }
        [JsonProperty("from")]
        public string FromPhoneNumber { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}