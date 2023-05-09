using HtmlAgilityPack;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace AwsTestingSolution.Helpers
{
    public class Pop3Helper
    {
        public List<Message> FetchAllMessages(string username, string password)
        {
            return FetchAllMessagesInInboxFolder("pop.gmx.com", 995, true, username, password);
        }

        public string GetConfirmationUrl(string username, string password)
        {
            var allMessages = FetchAllMessages(username, password);
            var sortedMessages = allMessages.Where(i => i.Headers.Subject.Equals("AWS Notification - Subscription Confirmation"));

            MessagePart message = sortedMessages.First().FindFirstHtmlVersion();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(message.GetBodyAsText());

            var link = htmlDocument.DocumentNode.SelectSingleNode("//a[contains(text(), 'Confirm subscription')]").Attributes["href"].Value;

            return link;
        }

        public List<Message> FetchAllMessagesInInboxFolder(string server, int port, bool useSsl, string username, string password)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(server, port, useSsl);

                client.Authenticate(username, password);

                int messageCount = client.GetMessageCount();

                List<Message> allMessages = new List<Message>(messageCount);

                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }

                return allMessages;
            }
        }
    }
}
