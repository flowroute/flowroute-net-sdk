using System.Collections.Generic;
using System.Net.Http;

namespace Flowroute.Messaging
{
    public class FlowrouteBaseResponse
    {
        public bool Success { get; set; }
        public HttpResponseMessage Raw { get; set; }
        public IEnumerable<FlowrouteError> Errors { get; set; }
        public string Error { get; set; }
    }
}