using AwsTestingSolution.ApiClients.CloudxImage;
using AwsTestingSolution.ApiClients.CloudxImage.Models;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.S3
{
    [TestFixture]
    public class S3FunctionalScenarios : AwsTestsBase
    {
        private CloudxImageUploadImageModel _uploadedImageModel;
        private CloudxImageApiClient _cloudxImageApiClient = new CloudxImageApiClient();
        private string _fileName = "UploadImageName" + DateTime.Now.Ticks;
        private string _fileDownloadPath = Directory.GetCurrentDirectory() + "/DowloadedFile" + DateTime.Now.Ticks;

        [SetUp]
        public void BeforeScenario()
        {
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
        }

        [Test]
        public void UploadFileToS3Bucket()
        {
            var actualUploadedImages = _cloudxImageApiClient.GetUploadedImages();
            actualUploadedImages.Should().Contain(image => image.ObjectKey.Contains(_fileName));
        }

        [Test]
        public void DowloadFileFromS3Bucket()
        {
            _cloudxImageApiClient.DownloadFileFromS3Bucket(_uploadedImageModel.Id, _fileDownloadPath);
            File.Exists(_fileDownloadPath).Should().BeTrue();
        }

        [Test]
        public void DeleteAnImageFromS3Bucket()
        {
            _cloudxImageApiClient.DeleteFileFromS3Bucket(_uploadedImageModel.Id);
            var actualUploadedImages = _cloudxImageApiClient.GetUploadedImages();
            actualUploadedImages.Should().NotContain(image => image.ObjectKey.Contains(_fileName));
        }
    }
}
