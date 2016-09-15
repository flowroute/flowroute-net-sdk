# Flowroute.DotNet

Current version: 0.1.0-beta

## Install via NuGet
```
Install-Package Flowroute -pre
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

#### ListAvailableNPAs(int limit = 10)
```csharp
```
#### RetrieveAvailableNPANetworkNumberingExchanges(int npa, int limit = 10, int page = 1)
```csharp
```
#### Search(FlowroutePhoneNumberSearchCriteria searchCriteria, int limit = 10, int page = 1)
```csharp
```
#### PurchasePhoneNumber(string phoneNumberToPurchase, string billingMethod = "METERED")
```csharp
```
#### ListTelephoneNumbers(int limit = 10, int page = 1, string pattern = "")
```csharp
```
#### ListTelephoneNumberDetails(string telephoneNumber)
```csharp
```
#### UpdateTelephoneNumberRoutes(string telephoneNumber, FlowrouteRoute primaryRoute, FlowrouteRoute secondaryRoute)
```csharp
```
#### ListInboundRoutes(int limit = 10, int page = 1)
```csharp
```
#### CreateInboundRoute(string routeName, InboundRouteType type, string value)
```csharp
```

## Support, Bug Fixes, Pull Requests
See an issue?  We accept pull requests!

## About
This project has been developed and maintained by [Kevin Griffin](https://twitter.com/1kevgriff).  Visit him at [http://kevgriffin.com](http://kevgriffin.com).
