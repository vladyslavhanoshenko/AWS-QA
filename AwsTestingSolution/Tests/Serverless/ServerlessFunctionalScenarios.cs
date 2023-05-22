using AwsTestingSolution.ApiClients.TempMail;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;
using AwsTestingSolution.ApiClients.ServerlessApp;
using AwsTestingSolution.ApiClients.ServerlessApp.Models;
using AwsTestingSolution.Tests.SnsSqs;

namespace AwsTestingSolution.Tests.Serverless
{
    [TestFixture]
    public class ServerlessFunctionalScenarios : SnsSqsScenariosBase
    {
        private ServerlessApiClient _serverlessApiClient = new ServerlessApiClient();
        private string _fileName = "UploadImageName" + DateTime.Now.Ticks;
        private string _fileDownloadPath = Directory.GetCurrentDirectory() + "/DowloadedFile" + DateTime.Now.Ticks + ".png";
        private ServerlessGuidGetImageModel? _uploadedImageModel;

        [OneTimeSetUp]
        public void BeforeFeature()
        {
            _uploadedImageModel = _serverlessApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            SubscribeResponse = _notificationApiClient.SubscribeEmailToSnsTopic(EmailAddress);
            var mail = TempMailRestServiceSteps.GetSubscriptionConfirmation(EmailAddress);
            ConfirmSubscriptionUi(TempMailRestServiceSteps.GetConfirmationUrl(mail.MailHtml));
        }

        [Test]
        public void NotificationShouldBeSentWhenImageUploaded()
        {
            _uploadedImageModel = _serverlessApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            actualEmail.MailText.Should().Contain("event_type: upload");
            actualEmail.MailText.Should().Contain(_fileName);
            actualEmail.MailText.Should().Contain("5074");
        }

        [Test]
        public void NotificationShouldBeSentWhenImageIsDeleted()
        {
            _serverlessApiClient.DeleteFileFromS3Bucket(_uploadedImageModel.Id);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            actualEmail.MailText.Should().Contain("event_type: delete");
            actualEmail.MailText.Should().Contain(_fileName);
            actualEmail.MailText.Should().Contain("5074");
        }

        [Test]
        public void UserCanDownloadFileUsingLinkFromTheNotification()
        {
            _uploadedImageModel = _serverlessApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            Regex regex = new Regex("(http|ftp|https):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])");
            string downloadLink = regex.Match(actualEmail.MailText).Value;
            _serverlessApiClient.DownloadFileGet(downloadLink, _fileDownloadPath);
            File.Exists(_fileDownloadPath).Should().BeTrue();
        }

        [Test]
        public void UnsubscribedUserSHouldNotSeeNotifications()
        {
            _notificationApiClient.UnsubscribeEmailFromSns(EmailAddress);
            _uploadedImageModel = _serverlessApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            bool isNotificationSent = TempMailRestServiceSteps.IsMessageWithSubjectPresent(EmailAddress, "AWS Notification Message");
            isNotificationSent.Should().BeFalse();
        }
    }
}
