using AwsTestingSolution.ApiClients.Image;
using AwsTestingSolution.ApiClients.Image.Models;
using AwsTestingSolution.ApiClients.TempMail;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsFunctionalScenarios : SnsSqsScenariosBase
    {
        private ImageApiClient _cloudxImageApiClient = new ImageApiClient();
        private string _fileName = "UploadImageName" + DateTime.Now.Ticks;
        private string _fileDownloadPath = Directory.GetCurrentDirectory() + "/DowloadedFile" + DateTime.Now.Ticks + ".png";
        private ImageModel? _uploadedImageModel;

        [OneTimeSetUp]
        public void BeforeFeature()
        {
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            SubscribeResponse = _notificationApiClient.SubscribeEmailToSnsTopic(EmailAddress);
            var mail = TempMailRestServiceSteps.GetSubscriptionConfirmation(EmailAddress);
            ConfirmSubscriptionUi(TempMailRestServiceSteps.GetConfirmationUrl(mail.MailHtml));
        }

        [Test]
        public void NotificationShouldBeSentWhenImageUploaded()
        {
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            actualEmail.MailText.Should().Contain("event_type: upload");
            actualEmail.MailText.Should().Contain(_fileName);
            actualEmail.MailText.Should().Contain("5074");
        }

        [Test]
        public void NotificationShouldBeSentWhenImageIsDeleted()
        {
            _cloudxImageApiClient.DeleteFileFromS3Bucket(_uploadedImageModel.Id);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            actualEmail.MailText.Should().Contain("event_type: delete");
            actualEmail.MailText.Should().Contain(_fileName);
            actualEmail.MailText.Should().Contain("5074");
        }

        [Test]
        public void UserCanDownloadFileUsingLinkFromTheNotification()
        {
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            var actualEmail = TempMailRestServiceSteps.GetNotificationEmail(EmailAddress);
            Regex regex = new Regex("(http|ftp|https):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])");
            string downloadLink = regex.Match(actualEmail.MailText).Value;
            _cloudxImageApiClient.DownloadFileGet(downloadLink, _fileDownloadPath);
            File.Exists(_fileDownloadPath).Should().BeTrue();
        }

        [Test]
        public void UnsubscribedUserSHouldNotSeeNotifications()
        {
            _notificationApiClient.UnsubscribeEmailFromSns(EmailAddress);
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
            bool isNotificationSent = TempMailRestServiceSteps.IsMessageWithSubjectPresent(EmailAddress, "AWS Notification Message");
            isNotificationSent.Should().BeFalse();
        }
    }
}
