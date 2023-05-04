using Amazon.RDS.Model;
using AwsTestingSolution.Configs;
using AwsTestingSolution.Storages;
using FluentAssertions;
using MySqlConnector;
using NUnit.Framework;
using Renci.SshNet;

namespace AwsTestingSolution.Tests.RDS
{
    [TestFixture]
    public class RDSDeploymentScenarios : AwsTestsBase
    {
        [Test]
        public void DataBaseShouldBeAccessibleViaSshFromPublicInstance()
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

            connection.State.Should().Be(System.Data.ConnectionState.Open);
        }

        [Test]
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
    }
}
