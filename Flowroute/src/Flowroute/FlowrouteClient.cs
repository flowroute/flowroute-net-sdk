using System;
using System.Linq;
using System.Threading.Tasks;
using Flowroute.Messaging;
using Flowroute.PhoneNumbers;

namespace Flowroute
{
    public class FlowrouteClient
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        public MessagingClient Messaging { get; private set; }
        public PhoneNumbersClient PhoneNumbers { get; private set; }

        public FlowrouteClient(string accessKey, string _secretKey)
        {
            _accessKey = accessKey;
            this._secretKey = _secretKey;

            Messaging = new MessagingClient(accessKey, _secretKey);
            PhoneNumbers = new PhoneNumbersClient(accessKey, _secretKey);
        }
    }
}
