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

        public bool IsMessageWithSubjectPresent(string email, string subject)
        {
            var actualMessages = GetMailsWithWait(email.ToMd5Hash());
            return actualMessages.Any(m => m.MailSubject.Equals(subject));
        }

        public EmailModel WaitForMailWithSubject(string md5Hash, string subject)
        {
            EmailModel[] actualEmails = GetMailsWithWait(md5Hash);
            while(!actualEmails.Any(m => m.MailSubject.Equals(subject)))
            {
                actualEmails = GetMailsWithWait(md5Hash);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            return actualEmails.First(i => i.MailSubject.Equals(subject));
        }
        
        public EmailModel GetSubscriptionConfirmation(string email)
        {
            return WaitForMailWithSubject(email.ToMd5Hash(), "AWS Notification - Subscription Confirmation");
        }

        public EmailModel GetNotificationEmail(string email)
        {
            return WaitForMailWithSubject(email.ToMd5Hash(), "AWS Notification Message");
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
