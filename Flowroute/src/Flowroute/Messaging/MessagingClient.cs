using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flowroute.Messaging
{
    public class MessagingClient
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        private const string UrlEndpoint = "https://api.flowroute.com/v2/messages";

        internal MessagingClient(string accessKey, string secretKey)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
        }


        private void SetAuthorization(HttpClient httpClient)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{_accessKey}:{_secretKey}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// This API endpoint is used to send a message from one valid phone number to another valid phone number.  
        /// All to and from phone numbers must use an E.164 format, which is an 11-digit 1NPANXXXXXX-formatted number in North America. 
        /// While you can send an SMS to any valid North American Numbering Plan Administration (NANPA) phone number; Flowroute cannot 
        /// guarantee delivery to any phone number not enabled for SMS. Additionally, you are only allowed to send an SMS from an 
        /// SMS-enabled phone number set up on the Direct Inward Dialing (DIDS > Manage) page.You cannot send an SMS from a phone 
        /// number not on your Flowroute account.
        /// 
        /// For more information: https://developer.flowroute.com/docs/messaging
        /// </summary>
        /// <param name="toPhoneNumber">Phone number of the message recipient, using an E.164 format, This must be in an E.164 format, 
        /// which is a North American, 11-digit 1NPANXXXXXX-formatted number.</param>
        /// <param name="fromPhoneNumber">Flowroute phone number to send the message from, using an E.164 format. On the receiver's phone,
        /// this is typically referred to as the Caller ID.</param>
        /// <param name="body">The content of the message to deliver. If a message is greater than 160 characters, and the carrier sends 
        /// header information with the message, the message with be reassembled on the receiver's phone as one message. If header 
        /// information is not sent by the carrier, the message will be received in multiple parts.</param>
        public async Task<FlowrouteSendMessageResponse> SendMessageAsync(string toPhoneNumber, string fromPhoneNumber, string body)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber)) throw new ArgumentOutOfRangeException(nameof(toPhoneNumber), "To phone number cannot be empty.");
            if (string.IsNullOrWhiteSpace(fromPhoneNumber)) throw new ArgumentOutOfRangeException(nameof(fromPhoneNumber), "From phone number cannot be empty.");
            if (string.IsNullOrWhiteSpace(body)) throw new ArgumentOutOfRangeException(nameof(body), "Body cannot be empty.");

            var smsRequest = new SmsRequest()
            {
                ToPhoneNumber = toPhoneNumber,
                FromPhoneNumber = fromPhoneNumber,
                Body = body
            };

            return await SendMessageAsync(smsRequest);
        }


        /// <summary>
        /// This API endpoint is used to send a message from one valid phone number to another valid phone number.  
        /// All to and from phone numbers must use an E.164 format, which is an 11-digit 1NPANXXXXXX-formatted number in North America. 
        /// While you can send an SMS to any valid North American Numbering Plan Administration (NANPA) phone number; Flowroute cannot 
        /// guarantee delivery to any phone number not enabled for SMS. Additionally, you are only allowed to send an SMS from an 
        /// SMS-enabled phone number set up on the Direct Inward Dialing (DIDS > Manage) page.You cannot send an SMS from a phone 
        /// number not on your Flowroute account.
        /// 
        /// For more information: https://developer.flowroute.com/docs/messaging
        /// </summary>
        /// <param name="request">An SMS request object.</param>
        public async Task<FlowrouteSendMessageResponse> SendMessageAsync(SmsRequest request)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SetAuthorization(httpClient);

                var json = JsonConvert.SerializeObject(request);

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, UrlEndpoint);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                FlowrouteSendMessageResponse responseInternal;
                using (var response = await httpClient.SendAsync(message))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    responseInternal = JsonConvert.DeserializeObject<FlowrouteSendMessageResponse>(responseString);

                    responseInternal.Raw = response;
                    responseInternal.Success = response.IsSuccessStatusCode;

                    if (!response.IsSuccessStatusCode)
                    {
                        responseInternal.Errors = responseInternal.Errors;
                    }
                }

                return responseInternal;
            }
        }

        /// <summary>
        /// The API endpoint allows you to search on a record identifier and return a Message Detail Record (MDR).
        /// 
        /// For more information: https://developer.flowroute.com/docs/look-up-mdr
        /// </summary>
        /// <param name="recordId">The unique messdate detail record identifier(MDR ID) of any message.When entering the MDR ID, the 
        /// number should include the mdr1- preface.</param>
        public async Task<FlowrouteGetMessageResponse> GetMessageDetailsAsync(string recordId)
        {
            if (string.IsNullOrWhiteSpace(recordId)) throw new ArgumentOutOfRangeException(nameof(recordId), "Record id cannot be empty.");
            using (HttpClient httpClient = new HttpClient())
            {
                SetAuthorization(httpClient);

                UriBuilder uriBuilder = new UriBuilder(UrlEndpoint);
                uriBuilder.Path = $"{uriBuilder.Path}/{recordId}";

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());

                using (var response = await httpClient.SendAsync(message))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseInternal = JsonConvert.DeserializeObject<FlowrouteGetMessageResponse>(responseString);

                    responseInternal.Success = response.IsSuccessStatusCode;
                    responseInternal.Raw = response;

                    if (!response.IsSuccessStatusCode)
                    {
                        responseInternal.Errors = responseInternal.Errors;
                    }

                    return responseInternal;
                }
            }
        }

        /// <summary>
        /// This API endpoint is used to retrieve a list of Message Detail Records (MDRs) within a specified date range. Date and Time is 
        /// based on Coordinated Universal Time (UTC).
        /// 
        /// For more information: https://developer.flowroute.com/docs/lookup-a-set-of-messages
        /// </summary>
        /// <param name="startDate">The beginning date and time, in UTC, on which to perform an MDR search.</param>
        /// <param name="endDate">Ending date and time, in UTC, on which to perform an MDR search.</param>
        /// <param name="limit">The number of MDRs to retrieve at one time. Use the spinner control to select the number, or enter a 
        /// number manually. You can set as high of a number as you want, but the number cannot be negative and must be greater than zero (0).</param>
        /// <param name="offset">The number of MDRs to skip when performing a query. Use the spinner control to select the number, or enter a 
        /// number manually. You can set this field to zero (0) or greater, but the number cannot be negative.</param>
        /// <returns></returns>
        public async Task<FlowrouteGetMessagesResponse> GetMessagesAsync(DateTimeOffset startDate, DateTimeOffset endDate, long limit = 100, long offset = 0)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero.");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be less than zero");

            using (HttpClient httpClient = new HttpClient())
            {
                SetAuthorization(httpClient);

                UriBuilder uriBuilder = new UriBuilder(UrlEndpoint);
                uriBuilder.Query =
                    $"limit={limit}&offset={offset}&start_date={startDate:o}&end_date={endDate:o}".Replace(":", "%3A")
                        .Replace("+", "%2B");

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());

                FlowrouteGetMessagesResponse responseInternal;
                using (var response = await httpClient.SendAsync(message))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    responseInternal = JsonConvert.DeserializeObject<FlowrouteGetMessagesResponse>(responseString);

                    responseInternal.Success = response.IsSuccessStatusCode;
                    responseInternal.Raw = response;

                    if (!response.IsSuccessStatusCode)
                    {
                        responseInternal.Errors = responseInternal.Errors;
                    }
                }

                return responseInternal;
            }
        }
    }
}
