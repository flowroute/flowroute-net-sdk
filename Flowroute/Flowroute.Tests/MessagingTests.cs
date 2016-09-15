using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flowroute.Messaging;
using NUnit.Framework;

namespace Flowroute.Tests
{
    [TestFixture]
    public class MessagingTests
    {
        const string AccessKey = "[FILL IN BLANK]";
        const string SecretKey = "[FILL IN BLANK]";

        const string KnownGoodPhoneNumber = "";

        private async Task<string> SendTestMessage(int id)
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var response =
                await client.Messaging.SendMessageAsync("17578675309", KnownGoodPhoneNumber, $"TestMessage x{id}");

            if (response.Success)
                return response.Data.Id;

            throw new Exception("Error sending test message");
        }

        [Test]
        public async Task SendMessageAsyncShouldReturnSuccessWhenSmsSent()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var request = new SmsRequest
            {
                ToPhoneNumber = "17578675309",
                FromPhoneNumber = "17575555555",
                Body = "TestMessage"
            };
            var response =
                await
                    client.Messaging.SendMessageAsync("17578675309", KnownGoodPhoneNumber, "Test SendMessageAsyncShouldReturnSuccessWhenSmsSent");

            Assert.AreEqual(true, response.Success);
        }

        [Test]
        public void SendMessageAsyncShouldThrowExceptionIfToPhoneNumberIsMissing()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var response =
                    await client.Messaging.SendMessageAsync("", KnownGoodPhoneNumber,
                            "Test SendMessageAsyncShouldThrowExceptionIfToPhoneNumberIsMissing");
            });
        }

        [Test]
        public void SendMessageAsyncShouldThrowExceptionIfFromPhoneNumberIsMissing()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var response =
                    await client.Messaging.SendMessageAsync("17578675309", "",
                            "Test SendMessageAsyncShouldThrowExceptionIfFromPhoneNumberIsMissing");
            });
        }

        [Test]
        public void SendMessageAsyncShouldThrowExceptionIfBodyIsMissing()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
                var response =
                    await client.Messaging.SendMessageAsync("17578675309", KnownGoodPhoneNumber,
                            "");
            });
        }

        [Test]
        public async Task SendMessageAsyncShouldReturn401IfBadUserName()
        {
            FlowrouteClient client = new FlowrouteClient("", SecretKey);
            var response =
                await
                    client.Messaging.SendMessageAsync("17578675309", KnownGoodPhoneNumber, "Test SendMessageAsyncShouldReturn401IfBadUserName");

            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Errors);
            Assert.AreEqual(response.Errors.First().Status, 401);
        }

        [Test]
        public async Task SendMessageAsyncShouldReturn401IfBadPassword()
        {
            FlowrouteClient client = new FlowrouteClient(AccessKey, "");
            var response =
                await
                    client.Messaging.SendMessageAsync("17578675309", KnownGoodPhoneNumber, "Test SendMessageAsyncShouldReturn401IfBadPassword");

            Assert.IsFalse(response.Success);
            Assert.IsNotEmpty(response.Errors);
            Assert.AreEqual(response.Errors.First().Status, 401);
        }

        [Test]
        public async Task ShouldReturnListOfMessages()
        {
            var start = DateTimeOffset.UtcNow.DateTime;

            // send 5 messages
            var ids = new List<string>();
            for (var x = 0; x < 5; x++)
            {
                var sendTestMessage = SendTestMessage(x);
                sendTestMessage.Wait();

                ids.Add(sendTestMessage.Result);
            }

            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var response = await client.Messaging.GetMessagesAsync(start, start.AddMinutes(1));

            Assert.IsTrue(response.Success);
            Assert.IsNotEmpty(response.Data);
            Assert.GreaterOrEqual(5, response.Data.Count());
        }

        [Test]
        public async Task ShouldReturnSingleMessage()
        {
            var messageId = await SendTestMessage(100);

            FlowrouteClient client = new FlowrouteClient(AccessKey, SecretKey);
            var response = await client.Messaging.GetMessageDetailsAsync(messageId);

            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Data.Id, messageId);
        }
    }
}
