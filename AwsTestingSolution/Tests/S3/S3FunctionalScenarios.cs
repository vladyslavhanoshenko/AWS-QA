using AwsTestingSolution.ApiClients.CloudxImage;
using AwsTestingSolution.ApiClients.CloudxImage.Models;
using AwsTestingSolution.Configs;
using AwsTestingSolution.Storages;
using Dapper;
using FluentAssertions;
using MySqlConnector;
using NUnit.Framework;
using Renci.SshNet;

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

        [Test]
        public void GetImageById()
        {
            CloudxImageGetModel actualImageMetaData = _cloudxImageApiClient.GetUploadedImageById(_uploadedImageModel.Id);
            actualImageMetaData.ObjectKey.Should().Contain(_fileName);
        }

        [Test]
        public void CheckUploadedImageStoredInDatabase()
        {
            var connectionInfo = new ConnectionInfo(CloudximageDataStorage.AppInstancePublicIp, EC2InstancesDataStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesDataStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

            var forwardedPort = new ForwardedPortLocal(SshConfig.LocalIpAddress, CloudximageDataStorage.MySqlPort, CloudximageDataStorage.DataBaseHostName, CloudximageDataStorage.MySqlPort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            var connectionString = $"server=localhost;port={forwardedPort.BoundPort};database={CloudximageDataStorage.DataBaseName};uid={CloudximageDataStorage.DataBaseUserName};password={CloudximageDataStorage.DataBasePassword}";
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var queryResult = connection.Query<CloudxImageGetModel>("select Id, object_key as ObjectKey from images").ToList();
            queryResult.Should().Contain(i => i.Id.Equals(_uploadedImageModel.Id) && i.ObjectKey.Contains(_fileName));
        }

        [Test]
        public void CheckMetadataImageDeletedFromDatabase()
        {
            _cloudxImageApiClient.DeleteFileFromS3Bucket(_uploadedImageModel.Id);

            var connectionInfo = new ConnectionInfo(CloudximageDataStorage.AppInstancePublicIp, EC2InstancesDataStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesDataStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

            var forwardedPort = new ForwardedPortLocal(SshConfig.LocalIpAddress, CloudximageDataStorage.MySqlPort, CloudximageDataStorage.DataBaseHostName, CloudximageDataStorage.MySqlPort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            var connectionString = $"server=localhost;port={forwardedPort.BoundPort};database={CloudximageDataStorage.DataBaseName};uid={CloudximageDataStorage.DataBaseUserName};password={CloudximageDataStorage.DataBasePassword}";
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            var queryResult = connection.Query<CloudxImageGetModel>("select Id, object_key as ObjectKey from images").ToList();
            queryResult.Should().NotContain(i => i.Id.Equals(_uploadedImageModel) && i.ObjectKey.Equals(_uploadedImageModel.Id));
        }
    }
}
