using AwsTestingSolution.ApiClients.Notification;
using AwsTestingSolution.ApiClients.TempMail;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace AwsTestingSolution.Tests.SnsSqs
{
    public class SnsSqsScenariosBase : AwsTestsBase
    {
        protected NotificationApiClient _notificationApiClient = new NotificationApiClient();
        protected static TempMailRestServiceSteps TempMailRestServiceSteps = new TempMailRestServiceSteps();
        protected string EmailAddress = CreateEmailAddress();
        protected string? SubscribeResponse;

        protected string ConfirmSubscriptionUi(string confirmationUrl)
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(confirmationUrl);
            var subscribtionId = driver.FindElement(By.XPath("//code")).Text;
            driver.Quit();
            return subscribtionId;
        }

        protected static string CreateEmailAddress()
        {
            var emailWithoutDomain = "cloudx" + new Random().Next(0, 99999999) + "image";
            string domain = TempMailRestServiceSteps.TempMailRestService.GetDomainsList().First();
            return emailWithoutDomain + domain;
        }
    }
}
