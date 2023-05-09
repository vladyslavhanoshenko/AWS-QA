using AwsTestingSolution.ApiClients.Notification;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsFunctionalScenarios : AwsTestsBase
    {
        private NotificationApiClient _notificationApiClient = new NotificationApiClient();

        [Test]
        public void SubscribeEmailToSnsTopic()
        {
            var subscribeResponse = _notificationApiClient.SubscribeEmailToSnsTopic("sevam42970@carpetra.com");
            subscribeResponse.Should().BeEquivalentTo("Successfully subscribed.");

            var actualSubscriptions = _notificationApiClient.GetEmailSubscriptionsToSnsTopic();
            actualSubscriptions.Should().Contain(s => s.Endpoint.Equals("sevam42970@carpetra.com") && s.SubscriptionArn.Equals("PendingConfirmation"));
        }
    }
}
