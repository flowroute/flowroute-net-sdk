using System;
using System.Threading.Tasks;
using Flowroute.PhoneNumbers;
using NUnit.Framework;

namespace Flowroute.Tests
{
    [TestFixture]
    public class PhoneNumbersTest
    {
        const bool PleaseSpendMoney = false;
        const string AccessKey = "[FILL IN BLANK]";
        const string SecretKey = "[FILL IN BLANK]";

        const string KnownGoodFlowrouteNumber = "";

        [Test]
        public async Task GetAvailableNPAsShouldReturnNPAs()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);

            var results = await client.PhoneNumbers.ListAvailableNPAsAsync();

            Assert.AreEqual(true, results.Success);
            Assert.IsNotEmpty(results.Npas);
        }

        [Test]
        public void GetAvailableNPAsShouldThrowExceptionIfLimitLessThanZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var results = await client.PhoneNumbers.ListAvailableNPAsAsync(-5);
            });
        }

        [Test]
        public void GetAvailableNPAsShouldThrowExceptionIfLimitGreaterThan200()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var results = await client.PhoneNumbers.ListAvailableNPAsAsync(201);
            });
        }

        [Test]
        public async Task RetrieveAvailableNPANetworkNumberingExchangesShouldReturnNXX()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.RetrieveAvailableNPANetworkNumberingExchangesAsync(757);

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.NPANXXS);

        }

        [Test]
        public async Task SearchShouldReturnResultsWithNoCriteria()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria());

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.TNS);
        }


        [Test]
        public async Task SearchShouldReturnResultsWithJustNpa()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { NPA = 757 });

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.TNS);
        }


        [Test]
        public void SearchShouldThrowExceptionWithJustRateCenter()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { RateCenter = "SEATTLE" });
            });
        }

        [Test]
        public async Task SearchShouldReturnResultsWithJustRateCenterAndState()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { RateCenter = "SEATTLE", State = "WA" });

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.TNS);
        }

        [Test]
        public async Task SearchShouldReturnResultsWithJustState()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.SearchAsync(new FlowroutePhoneNumberSearchCriteria() { State = "WA" });

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.TNS);
        }

        [Test]
        public async Task ShouldPurchasePhoneNumber()
        {
            if (PleaseSpendMoney)
            {
                var goodPhoneNumber = "17575555555";
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var results = await client.PhoneNumbers.PurchasePhoneNumberAsync(goodPhoneNumber);

                Assert.IsTrue(results.Success);
            }
            else
            {
                Assert.Inconclusive("You don't want to spend money, so the test did not run");
            }


        }

        [Test]
        public async Task PurchasePhoneNumberShouldErrorIfBadPhoneNumber()
        {
            var badPhoneNumber = "17575555555";
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.PurchasePhoneNumberAsync(badPhoneNumber);

            Assert.IsFalse(results.Success);
        }

        [Test]
        public async Task ShouldListTelephoneNumbers()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.ListTelephoneNumbersAsync();

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.TNS);
        }

        [Test]
        public async Task ShouldListTelephoneNumberDetails()
        {
            var goodPhoneNumber = "17575555555";
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.ListTelephoneNumberDetailsAsync(goodPhoneNumber);

            Assert.IsTrue(results.Success);
        }

        [Test]
        public async Task ShouldUpdateRoutesOfTelephoneNumber()
        {
            var goodPhoneNumber = "17575555555";
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.UpdateTelephoneNumberRoutesAsync(goodPhoneNumber,
                new FlowrouteRoute() { Name = "Primary1" },
                new FlowrouteRoute() { Name = "Primary2" });

            Assert.IsTrue(results.Success);

        }

        [Test]
        public async Task ShouldListInboundRoutes()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.ListInboundRoutesAsync();

            Assert.IsTrue(results.Success);
            Assert.IsNotEmpty(results.Routes);
        }

        [Test]
        public async Task ShouldCreateInboundRoute()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var results = await client.PhoneNumbers.CreateInboundRouteAsync("TestRoute", InboundRouteType.HOST, "kevgriffin.com");

            Assert.IsTrue(results.Success, results.Error);
        }
    }
}