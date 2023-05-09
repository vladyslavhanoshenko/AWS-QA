using AwsTestingSolution.ApiClients.Notification;
using AwsTestingSolution.ApiClients.TempMail;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsApiScenarios : AwsTestsBase
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

        private string ConfirmSubscriptionUi(string confirmationUrl)
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(confirmationUrl);
            var subscribtionId = driver.FindElement(By.XPath("//code")).Text;
            driver.Quit();
            return subscribtionId;
        }

        private static string CreateEmailAddress()
        {
            var emailWithoutDomain = "cloudx" + new Random().Next(0, 99999999) + "image";
            string domain = TempMailRestServiceSteps.TempMailRestService.GetDomainsList().First();
            return emailWithoutDomain + domain;
        }
    }
}
