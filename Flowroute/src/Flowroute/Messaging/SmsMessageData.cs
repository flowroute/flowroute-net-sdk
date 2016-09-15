namespace Flowroute.Messaging
{
    public class SmsMessageData
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public SmsMessageAttributes Attributes { get; set; }
    }
}