using AwsTestingSolution.ApiClients.Notification;
using AwsTestingSolution.ApiClients.TempMail;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsApiScenarios : SnsSqsScenariosBase
    {
        private NotificationApiClient _notificationApiClient = new NotificationApiClient();
        private static TempMailRestServiceSteps TempMailRestServiceSteps = new TempMailRestServiceSteps();
        private string EmailAddress = CreateEmailAddress();
        private string? _subscribeResponse;

        [SetUp]
        public void BeforeScenario()
        {
            _subscribeResponse = _notificationApiClient.SubscribeEmailToSnsTopic(EmailAddress);
        }

        [Test]
        public void SubscribeEmailToSnsTopic()
        {
            _subscribeResponse.Should().BeEquivalentTo("Successfully subscribed.");

            var actualSubscriptions = _notificationApiClient.GetEmailSubscriptionsToSnsTopic();
            actualSubscriptions.Should().Contain(s => s.Endpoint.Equals(EmailAddress) && s.SubscriptionArn.Equals("PendingConfirmation"));
        }

        [Test]
        public void UserHasToConfirmSubscription()
        {
            var mail = TempMailRestServiceSteps.GetSubscriptionConfirmation(EmailAddress);
            var subscribtionId = ConfirmSubscriptionUi(TempMailRestServiceSteps.GetConfirmationUrl(mail.MailHtml));
            var actualSubscriptions = _notificationApiClient.GetEmailSubscriptionsToSnsTopic();
            actualSubscriptions.Should().Contain(s => s.Endpoint.Equals(EmailAddress) && s.SubscriptionArn.Equals(subscribtionId));
        }

        [Test]
        public void UserCanCancelSubscription()
        {
            var mail = TempMailRestServiceSteps.GetSubscriptionConfirmation(EmailAddress);
            ConfirmSubscriptionUi(TempMailRestServiceSteps.GetConfirmationUrl(mail.MailHtml));
            
            string actualUnsubscribeMessage = _notificationApiClient.UnsubscribeEmailFromSns(EmailAddress);
            actualUnsubscribeMessage.Should().Contain("Successfully unsubscribed.");

            var actualSubscriptions = _notificationApiClient.GetEmailSubscriptionsToSnsTopic();
            actualSubscriptions.Should().NotContain(s => s.Endpoint.Equals(EmailAddress));
        }
    }
}
