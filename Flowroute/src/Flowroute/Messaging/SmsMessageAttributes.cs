using System;
using Newtonsoft.Json;

namespace Flowroute.Messaging
{
    public class SmsMessageAttributes
    {
        public string Body { get; set; }
        public string Direction { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("amount_nanodollars")]
        public long AmountInNanoDollars { get; set; }

        [JsonProperty("message_encoding")]
        public long MessageEncoding { get; set; }

        [JsonProperty("has_mms")]
        public bool HasMms { get; set; }
        public string To { get; set; }

        [JsonProperty("amount_display")]
        public string AmountDisplay { get; set; }

        [JsonProperty("callback_URL")]
        public string CallbackUrl { get; set; }

        [JsonProperty("message_type")]
        public string MessageType { get; set; }
    }
}