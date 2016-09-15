using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Flowroute.Messaging;
using Newtonsoft.Json;

namespace Flowroute.PhoneNumbers
{
    /// <summary>
    /// Client interface for accessing phone number management features of Flowroute.
    /// </summary>
    public class PhoneNumbersClient
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        private const string UrlAvailableTnsEndpoint = "https://api.flowroute.com/v1/available-tns";
        private const string UrlTnsEndpoint = "https://api.flowroute.com/v1/tns";
        private const string UrlRoutesEndpoint = "https://api.flowroute.com/v1/routes";

        internal PhoneNumbersClient(string accessKey, string secretKey)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
        }

        /// <summary>
        /// Encodes a string using HMAC-SHA1 to perform Flowroute v1 API authentication.
        /// </summary>
        /// <param name="input">String of content to encode.</param>
        /// <param name="key">Key to salt the encryption with.</param>
        /// <returns></returns>
        private string Encode(string input, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyBytes);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            var hash = myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + $"{e:x2}", s => s);

            return hash;
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <param name="dateTime">Date time.</param>
        /// <returns></returns>
        private string GetTimeStamp(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private async Task<T> GetResponse<T>(HttpRequestMessage message, object bodyParameters = null) where T : FlowrouteBaseResponse
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.SendAsync(message))
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var convertedObject = string.IsNullOrWhiteSpace(responseString)
                        ? new FlowrouteBaseResponse()
                        : JsonConvert.DeserializeObject<T>(responseString);

                    convertedObject.Success = response.IsSuccessStatusCode;
                    convertedObject.Raw = response;

                    return (T)convertedObject;
                }
            }
        }
        private HttpRequestMessage CreateRequestMessage(List<string> queryParameters, string url, HttpMethod httpMethod = null, object bodyParameters = null)
        {
            if (httpMethod == null) httpMethod = HttpMethod.Get;
            queryParameters.Sort();

            UriBuilder builder = new UriBuilder(url);
            builder.Query = string.Join("&", queryParameters);

            HttpRequestMessage message = new HttpRequestMessage(httpMethod, builder.ToString());

            var bodyString = bodyParameters != null ? JsonConvert.SerializeObject(bodyParameters) : string.Empty;

            var messageString = new PhoneNumbersMessageString
            {
                HttpMethod = httpMethod.Method,
                Timestamp = GetTimeStamp(DateTime.UtcNow),
                Body = bodyString,
                Canonical = $"{builder.Scheme}://{builder.Host}{builder.Path}\n{string.Join("&", queryParameters)}"
            };

            if (!string.IsNullOrWhiteSpace(bodyString))
            {
                message.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");
            }


            var messageStringAsString = messageString.ToString();
            var signature = Encode(messageStringAsString, _secretKey);

            message.Headers.Add("X-Timestamp", messageString.Timestamp);

            var authorizationString = $"{_accessKey}:{signature}";
            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(authorizationString));
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);

            return message;
        }

        /// <summary>
        /// This API endpoint returns a list of all Numbering Plan Areas (NPAs), which are area codes with purchasable telephone numbers.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/retrieve-available-npas
        /// </summary>
        /// <param name="limit">Number of items to retrieve. The maximum number is 200.</param>
        public async Task<FlowrouteNpaListResponse> ListAvailableNPAsAsync(int limit = 10)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero");
            if (limit > 200) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be greater than 200");


            var endpoint = $"{UrlAvailableTnsEndpoint}/npas/";
            var queryParameters = new List<string> { $"limit={limit}" };
            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowrouteNpaListResponse>(message);
        }

        /// <summary>
        /// This API endpoint returns a list of all Numbering Plan Area (NPAs) NXXs, which are area codes and exchanges 
        /// containing purchasable telephone numbers. All parameters that can be passed for the endpoint are optional; 
        /// if no parameters are specified, results are limited to the first ten items.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/retrieve-available-npas-nxxs
        /// </summary>
        /// <param name="npa">Restricts the results to the specified area code. For example, this might be 206.</param>
        /// <param name="limit">Limits the number of items to retrieve. A maximum of 200 items can be retrieved.</param>
        /// <param name="page">Displays the page set by the number in this field. For example, if 2 were entered, page 2 would display in the response.</param>
        public async Task<FlowrouteNpaNetworkNumberingExchangeResponse> RetrieveAvailableNPANetworkNumberingExchangesAsync(int npa, int limit = 10, int page = 1)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero");
            if (limit > 200) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be greater than 200");
            if (page < 0) page = 1;

            var endpoint = $"{UrlAvailableTnsEndpoint}/npanxxs/";
            var queryParameters = new List<string> { $"limit={limit}", $"page={page}", $"npa={npa}" };
            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowrouteNpaNetworkNumberingExchangeResponse>(message);
        }

        /// <summary>
        /// Search for phone numbers by a Numbering Plan Area (NPA), Numbering Plan Area and Exchange (NPA-NXX), 
        /// State, or rate center.
        ///
        /// For more information: https://developer.flowroute.com/v1.0/docs/search-for-purchasable-numbers
        /// </summary>
        /// <param name="searchCriteria">Search criteria for phone numbers</param>
        /// <param name="limit">Number of items to display. A maximum number of 200 can be returned.</param>
        /// <param name="page">Displays the specified page.</param>
        /// <returns></returns>
        public async Task<FlowroutePhoneNumberSearchResponse> SearchAsync(FlowroutePhoneNumberSearchCriteria searchCriteria, int limit = 10, int page = 1)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero");
            if (limit > 200) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be greater than 200");
            if (page < 0) page = 1;

            if (searchCriteria.NXX > 0 && searchCriteria.NPA == 0)
                throw new ArgumentOutOfRangeException(nameof(searchCriteria), "NPA is required if providing NXX");
            if (!string.IsNullOrWhiteSpace(searchCriteria.RateCenter) && string.IsNullOrWhiteSpace(searchCriteria.State))
                throw new ArgumentOutOfRangeException(nameof(searchCriteria), "RateCenter requires State");

            var endpoint = $"{UrlAvailableTnsEndpoint}/tns/";
            var queryParameters = searchCriteria.GetQueryParameters();
            queryParameters.Add($"limit={limit}");
            queryParameters.Add($"page={page}");

            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowroutePhoneNumberSearchResponse>(message);
        }

        /// <summary>
        /// Purchase a telephone number from available Flowroute inventory.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/purchase-phone-number
        /// </summary>
        /// <param name="phoneNumberToPurchase">Telephone number to purchase. The number must use an E.164 format, which is an 11-digit 1NPANXXXXXX-formatted number in North America.</param>
        /// <param name="billingMethod">Sets the billing method to use. This will always be METERED, which are unlimited concurrent calls, each billed per-minute used.</param>
        /// <returns></returns>
        public async Task<FlowrouteBaseResponse> PurchasePhoneNumberAsync(string phoneNumberToPurchase, string billingMethod = "METERED")
        {
            if (string.IsNullOrWhiteSpace(phoneNumberToPurchase))
                throw new ArgumentOutOfRangeException(nameof(phoneNumberToPurchase),
                    "Phone number to purchase is required.");
            if (string.IsNullOrWhiteSpace(billingMethod))
                throw new ArgumentOutOfRangeException(nameof(billingMethod),
                    "Billing method is required.");

            var endpoint = $"{UrlTnsEndpoint}/{phoneNumberToPurchase}";
            var queryParameters = new List<string>();
            var bodyParameters = new
            {
                billing_method = billingMethod
            };
            var message = CreateRequestMessage(queryParameters, endpoint, HttpMethod.Put, bodyParameters);

            return await GetResponse<FlowrouteBaseResponse>(message);
        }

        /// <summary>
        /// Returns list of all phone numbers currently on your Flowroute account.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/list-account-phone-numbers
        /// </summary>
        /// <param name="limit">Number of items to display. The maximum number is 200.</param>
        /// <param name="page">Sets the page to display.</param>
        /// <param name="pattern">Indicates a pattern of integers to match against. This field supports partial matches. For example, if you enter 
        /// 12066, all numbers that include 12066 are returned. There is no minimum number of integers on which you can search.</param>
        public async Task<FlowrouteListTelephoneNumbersResponse> ListTelephoneNumbersAsync(int limit = 10, int page = 1, string pattern = "")
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero");
            if (limit > 200) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be greater than 200");
            if (page < 0) page = 1;

            var endpoint = $"{UrlTnsEndpoint}/";
            var queryParameters = new List<string>();
            queryParameters.Add($"limit={limit}");
            queryParameters.Add($"page={page}");
            if (!string.IsNullOrWhiteSpace(pattern)) queryParameters.Add($"pattern={pattern}");

            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowrouteListTelephoneNumbersResponse>(message);
        }

        /// <summary>
        /// Get all of the information associated with a phone number, including billing method, primary route, and failover route.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/retrieve-phone-number-details
        /// </summary>
        /// <param name="telephoneNumber">Telephone number on which to search. Must be an E.164 11-digit formatted number.</param>
        public async Task<FlowrouteTelephoneNumberDetailResponse> ListTelephoneNumberDetailsAsync(string telephoneNumber)
        {
            if (string.IsNullOrWhiteSpace(telephoneNumber))
                throw new ArgumentOutOfRangeException(nameof(telephoneNumber),
                    "Phone number is required.");

            var endpoint = $"{UrlTnsEndpoint}/{telephoneNumber}";
            var queryParameters = new List<string>();

            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowrouteTelephoneNumberDetailResponse>(message);
        }

        /// <summary>
        /// The update method is used to update both the primary and failover route for a phone number, specified within an array. The 
        /// first route name within the array is assigned as the primary route; the second route listed in the array will be the failover 
        /// route in the event the first route is unavailable. Routes must first be created using the Create an Inbound Route endpoint.
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/update-phone-number-routes
        /// </summary>
        /// <param name="telephoneNumber">Telephone number for which to update the route. The phone number must be formatted as a valid E.164, 11-digit formatted North American phone number—for example, 12065555780.</param>
        /// <param name="primaryRoute">The primary route to which you want to point your phone number.</param>
        /// <param name="secondaryRoute">The secondary route to which you want to point your phone number.</param>
        /// <returns></returns>
        public async Task<FlowrouteBaseResponse> UpdateTelephoneNumberRoutesAsync(string telephoneNumber,
            FlowrouteRoute primaryRoute, FlowrouteRoute secondaryRoute)
        {
            if (string.IsNullOrWhiteSpace(telephoneNumber))
                throw new ArgumentOutOfRangeException(nameof(telephoneNumber),
                    "Phone number is required.");

            var endpoint = $"{UrlTnsEndpoint}/{telephoneNumber}";
            var queryParameters = new List<string>();
            var bodyParameters = new
            {
                routes = new List<FlowrouteRoute> { primaryRoute, secondaryRoute }
            };
            var message = CreateRequestMessage(queryParameters, endpoint, new HttpMethod("PATCH"), bodyParameters);

            return await GetResponse<FlowrouteBaseResponse>(message);
        }

        /// <summary>
        /// The Retrieve Routes endpoint allows you to return a list of your inbound routes. From the list, you can then select routes 
        /// to use as the primary and failover routes associated with a phone number, which is done using the Update Phone Number 
        /// Routes endpoint. 
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/retrieve-routes
        /// </summary>
        /// <param name="limit">Number of items to display. The maximum number is 200</param>
        /// <param name="page">Sets the page to display.</param>
        public async Task<FlowrouteListInboundRoutesResponse> ListInboundRoutesAsync(int limit = 10, int page = 1)
        {
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be less than zero");
            if (limit > 200) throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be greater than 200");
            if (page < 0) page = 1;

            var endpoint = $"{UrlRoutesEndpoint}/";
            var queryParameters = new List<string>();
            queryParameters.Add($"limit={limit}");
            queryParameters.Add($"page={page}");

            var message = CreateRequestMessage(queryParameters, endpoint);

            return await GetResponse<FlowrouteListInboundRoutesResponse>(message);
        }

        /// <summary>
        /// This API endpoint is used to create a new inbound route. Routes can then be associated with phone numbers using 
        /// the Update Phone Number Routes endpoint.  
        /// 
        /// For more information: https://developer.flowroute.com/v1.0/docs/create-inbound-route
        /// </summary>
        /// <param name="routeName">Name of the route to be created. Alphanumeric characters are supported.</param>
        /// <param name="type">The type of route to create. Must be HOST, PSTN, or URI. </param>
        /// <param name="value">Value of the route, dependent on the type</param>
        public async Task<FlowrouteBaseResponse> CreateInboundRouteAsync(string routeName, InboundRouteType type, string value)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                throw new ArgumentOutOfRangeException(nameof(routeName),
                    "Route name is required.");

            if (type.Value != "HOST" && type.Value != "PSTN" && type.Value != "URI")
                throw new ArgumentOutOfRangeException(nameof(type), "Type must be HOST, PSTN, or URI");

            if (type == null)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Value is required.");

            // TODO check value

            var endpoint = $"{UrlRoutesEndpoint}/{routeName}";
            var queryParameters = new List<string>();
            var bodyParameters = new
            {
                type = type.Value,
                value
            };
            var message = CreateRequestMessage(queryParameters, endpoint, HttpMethod.Put, bodyParameters);

            return await GetResponse<FlowrouteBaseResponse>(message);
        }
    }
}
