using Amazon.RDS.Model;
using AwsTestingSolution.ApiClients.CloudxImage;
using AwsTestingSolution.ApiClients.CloudxImage.Models;
using AwsTestingSolution.Configs;
using AwsTestingSolution.Storages;
using Dapper;
using FluentAssertions;
using MySqlConnector;
using NUnit.Framework;
using Renci.SshNet;

namespace AwsTestingSolution.Tests.RDS
{
    [TestFixture]
    public class RDSScenarios : AwsTestsBase
    {
        private CloudxImageApiClient _cloudxImageApiClient = new CloudxImageApiClient();
        private string _fileName = "UploadImageName" + DateTime.Now.Ticks;
        private CloudxImageUploadImageModel _uploadedImageModel;
        private MySqlConnection _connection;

        [OneTimeSetUp]
        public void BeforeFeature()
        {
            var connectionInfo = new ConnectionInfo(CloudximageDataStorage.AppInstancePublicIp, EC2InstancesDataStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesDataStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

            var forwardedPort = new ForwardedPortLocal(SshConfig.LocalIpAddress, CloudximageDataStorage.MySqlPort, CloudximageDataStorage.DataBaseHostName, CloudximageDataStorage.MySqlPort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            var connectionString = $"server=localhost;port={forwardedPort.BoundPort};database={CloudximageDataStorage.DataBaseName};uid={CloudximageDataStorage.DataBaseUserName};password={CloudximageDataStorage.DataBasePassword}";
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        [SetUp]
        public void BeforeScenario()
        {
            _uploadedImageModel = _cloudxImageApiClient.UploadImageToS3Bucket(CloudximageDataStorage.PathToImageToBeUploaded, _fileName);
        }

        [Test]
        [Category("Deployment")]
        public void DataBaseShouldBeAccessibleViaSshFromPublicInstance()
        {
            _connection.State.Should().Be(System.Data.ConnectionState.Open);
        }

        [Test]
        [Category("Deployment")]
        public void CheckRdsInstanceRequirements()
        {
            var actualInstancesDeployed = RdsApiClientWrapper.GetDbInstances();
            DBInstance actualDbInstance = actualInstancesDeployed.DBInstances.Single(i => i.DBName.Equals(CloudximageDataStorage.DataBaseName));
            actualDbInstance.DBInstanceClass.Should().BeEquivalentTo("db.t3.micro");
            actualDbInstance.MultiAZ.Should().BeFalse();
            actualDbInstance.AllocatedStorage.Should().Be(100);
            actualDbInstance.StorageType.Should().BeEquivalentTo("gp2");
            actualDbInstance.StorageEncrypted.Should().BeFalse();
            actualDbInstance.TagList.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
            actualDbInstance.Engine.Should().BeEquivalentTo("mysql");
            actualDbInstance.EngineVersion.Should().BeEquivalentTo("8.0.28");
        }

        [Test]
        [Category("Functional")]
        public void GetImageById()
        {
            CloudxImageGetModel actualImageMetaData = _cloudxImageApiClient.GetUploadedImageById(_uploadedImageModel.Id);
            actualImageMetaData.ObjectKey.Should().Contain(_fileName);
        }

        [Test]
        [Category("Functional")]
        public void CheckUploadedImageStoredInDatabase()
        {
            var queryResult = _connection.Query<CloudxImageGetModel>("select Id, object_key as ObjectKey from images").ToList();
            queryResult.Should().Contain(i => i.Id.Equals(_uploadedImageModel.Id) && i.ObjectKey.Contains(_fileName));
        }

        [Test]
        [Category("Functional")]
        public void CheckMetadataImageDeletedFromDatabase()
        {
            _cloudxImageApiClient.DeleteFileFromS3Bucket(_uploadedImageModel.Id);
            var queryResult = _connection.Query<CloudxImageGetModel>("select Id, object_key as ObjectKey from images").ToList();
            queryResult.Should().NotContain(i => i.Id.Equals(_uploadedImageModel) && i.ObjectKey.Equals(_uploadedImageModel.Id));
        }
    }
}
