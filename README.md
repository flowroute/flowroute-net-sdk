# Flowroute.DotNet

Current version: 1.0.2 (Jan 4th, 2017)

## Install via NuGet
```
Install-Package Flowroute
```

## Obtaining your API access key and secret key
Go to `https://manage.flowroute.com/accounts/preferences/api/` and you will see your API credentials.

## How to Use

### Creating an instance of the Flowroute client
```csharp
string AccessKey = "accessKeyFromWebsite";
string SecretKey = "secretKeyFromWebsite";

FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
```
### Messaging
After you have created an instance of the Flowroute client, you will have access to the `MessagingClient`.  This client provides three methods:

#### SendMessageAsync(string toPhoneNumber, string fromPhoneNumber, string body)

```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var response = await client.Messaging.SendMessageAsync("17578675309", "17575555555", $"TestMessage");
```

#### SendMessageAsync(SmsRequest request)

```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var request = new SmsRequest
{
    ToPhoneNumber = "17578675309",
    FromPhoneNumber = "17575555555",
    Body = "TestMessage"
};

var response = await client.Messaging.SendMessageAsync(request);
```

#### GetMessageDetailsAsync(string recordId)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var response = await client.Messaging.GetMessageDetailsAsync(messageId);

```
### Phone Number Management

#### ListAvailableNPAsAsync(int limit = 10)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.ListAvailableNPAsAsync();
```
#### RetrieveAvailableNPANetworkNumberingExchangesAsync(int npa, int limit = 10, int page = 1)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.RetrieveAvailableNPANetworkNumberingExchangesAsync(757);
```
#### SearchAsync(FlowroutePhoneNumberSearchCriteria searchCriteria, int limit = 10, int page = 1)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { NPA = 757 });
var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { RateCenter = "SEATTLE", State = "WA" });
```
#### PurchasePhoneNumber(string phoneNumberToPurchase, string billingMethod = "METERED")
```csharp
var goodPhoneNumber = "17575555555";
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.PurchasePhoneNumberAsync(goodPhoneNumber);
```
#### ListTelephoneNumbers(int limit = 10, int page = 1, string pattern = "")
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.ListTelephoneNumbersAsync();
```
#### ListTelephoneNumberDetails(string telephoneNumber)
```csharp
var goodPhoneNumber = "17575555555";
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.ListTelephoneNumberDetailsAsync(goodPhoneNumber);
```
#### UpdateTelephoneNumberRoutes(string telephoneNumber, FlowrouteRoute primaryRoute, FlowrouteRoute secondaryRoute)
```csharp
var goodPhoneNumber = "17575555555";
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.UpdateTelephoneNumberRoutesAsync(goodPhoneNumber,
    new FlowrouteRoute() { Name = "Primary1" },
    new FlowrouteRoute() { Name = "Primary2" });
```
#### ListInboundRoutes(int limit = 10, int page = 1)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.ListInboundRoutesAsync();
```
#### CreateInboundRoute(string routeName, InboundRouteType type, string value)
```csharp
FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
var results = await client.PhoneNumbers.CreateInboundRouteAsync("TestRoute", InboundRouteType.HOST, "kevgriffin.com");
```

## Support, Bug Fixes, Pull Requests
See an issue? Please create an issue and we'll take a look! We accept pull requests!

## About
This project has been developed and maintained by [Kevin Griffin](https://twitter.com/1kevgriff).  Visit him at [http://kevgriffin.com](http://kevgriffin.com).
