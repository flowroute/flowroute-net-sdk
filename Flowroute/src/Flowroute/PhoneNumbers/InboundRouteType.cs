namespace Flowroute.PhoneNumbers
{
    public class InboundRouteType
    {
        private InboundRouteType(string host)
        {
            Value = host;
        }

        public static InboundRouteType HOST => new InboundRouteType("HOST");
        public static InboundRouteType PSTN => new InboundRouteType("PSTN");
        public static InboundRouteType URI => new InboundRouteType("URI");

        public string Value { get; private set; }
    }
}