using AwsTestingSolution.ApiClients.TempMail.Models;
using AwsTestingSolution.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AwsTestingSolution.ApiClients.TempMail
{
    public class TempMailRestServiceSteps
    {
        public TempMailRestService TempMailRestService = new TempMailRestService();

        public EmailModel[] GetMailsWithWait(string md5Hash)
        {
            string response = TempMailRestService.GetEmails(md5Hash);
            while (response.Contains("There are no emails yet"))
            {
                response = TempMailRestService.GetEmails(md5Hash);
            }
            return JsonConvert.DeserializeObject<EmailModel[]>(response);
        }

        public EmailModel GetSubscriptionConfirmation(string email)
        {
            var actualMails = GetMailsWithWait(email.ToMd5Hash());
            return actualMails.Single(i => i.MailSubject.Equals("AWS Notification - Subscription Confirmation"));
        }

        public string GetConfirmationUrl(string messageBody)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(messageBody);

            var link = htmlDocument.DocumentNode.SelectSingleNode("//a[text()='Confirm subscription']").GetAttributes().Single(a => a.Name.Equals("href")).Value;
            return link;
        }
    }
}
