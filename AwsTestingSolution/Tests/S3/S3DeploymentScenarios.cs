using AwsTestingSolution.Configs;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;
using Renci.SshNet;
using System.Net;

namespace AwsTestingSolution.Tests.S3
{
    [TestFixture]
    public class S3DeploymentScenarios : AwsTestsBase
    {
        [Test]
        public void VerifyCloudxImageApp()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(CloudximageDataStorage.AppInstancePublicDns + "/api/ui");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void CloudxImageAppShouldBeAccessibleViaSsh()
        {
            var connectionInfo = new ConnectionInfo(CloudximageDataStorage.AppInstancePublicIp, EC2InstancesDataStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesDataStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

            sshClient.IsConnected.Should().BeTrue();
        }

        [Test]
        public void CheckS3BucketRequirements()
        {
            var actualS3BucketsDeployed = S3ApiClientWrapper.GetS3Buckets();
            var bucketToBeDeployed = actualS3BucketsDeployed.Buckets.Single(b => b.BucketName.Contains("cloudximage-imagestorebucket"));

            var actualBucketInfo = S3ApiClientWrapper.GetS3BucketInfoByName(bucketToBeDeployed.BucketName);

            actualBucketInfo.IsEncryptionEnabled.Should().BeFalse();
            actualBucketInfo.IsVersioningEnabled.Should().BeFalse();
            actualBucketInfo.PublicAccess.Should().BeFalse();
            actualBucketInfo.Tags.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
        }
    }
}
