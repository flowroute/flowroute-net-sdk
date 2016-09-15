using System.Collections.Generic;

namespace Flowroute.PhoneNumbers
{
    public class FlowroutePhoneNumberSearchCriteria
    {
        public int NPA { get; set; }
        public int NXX { get; set; }
        public string RateCenter { get; set; }
        public string State { get; set; }
        public string TelephoneNumber { get; set; }

        public List<string> GetQueryParameters()
        {
            var returnList = new List<string>();
            if (NPA > 0) returnList.Add($"npa={NPA}");
            if (NXX > 0) returnList.Add($"npa={NXX}");
            if (!string.IsNullOrWhiteSpace(RateCenter)) returnList.Add($"ratecenter={RateCenter}");
            if (!string.IsNullOrWhiteSpace(State)) returnList.Add($"state={State}");
            if (!string.IsNullOrWhiteSpace(TelephoneNumber)) returnList.Add($"tn={TelephoneNumber}");

            return returnList;
        }
    }
}