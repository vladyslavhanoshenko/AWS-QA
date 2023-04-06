using Amazon.EC2.Model;
using NUnit.Framework;
using AwsTestingSolution.Mappers;
using FluentAssertions;
using AwsTestingSolution.Storages;
using AwsTestingSolution.ApiClients.EC2;
using AwsTestingSolution.ApiClients.CloudxInfo;
using AwsTestingSolution.ApiClients.CloudxInfo.Models;
using Renci.SshNet;
using Newtonsoft.Json;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.EC2
{
    [TestFixture]
    public class EC2Scenarios
    {
        [Test]
        public void DeploymentValidation()
        {
            EC2ApiClientWrapper eC2ApiClientWrapper = new EC2ApiClientWrapper();
            IEnumerable<Instance> actualInstancesDeployed = eC2ApiClientWrapper.GetAllDeployedInstances();

            var actualMappedInstancesDeployed = actualInstancesDeployed.Select(instance => EC2Mapper.MapInstanceToEC2InstanceModel(instance)).ToList();
            actualMappedInstancesDeployed.ForEach(instance => instance.RootBlockDeviceSize = eC2ApiClientWrapper.GetRootDeviceVolume(instance.InstanceId));
            actualMappedInstancesDeployed.Should().BeEquivalentTo(EC2InstancesConfigurationStorage.ExpectedDeployedInstances);
        }

        [Test]
        public void VerifyPublicInstanceSecurityGroups()
        {
            EC2ApiClientWrapper eC2ApiClientWrapper = new EC2ApiClientWrapper();
            var actualSecurityGroupInfoResponse = eC2ApiClientWrapper.GetSecurityGroupInfo(EC2InstancesConfigurationStorage.PublicInstanceSecurityGroupId);

            var securityGroupInfo = actualSecurityGroupInfoResponse.SecurityGroups.Single();
            var outIpPermissions = securityGroupInfo.IpPermissionsEgress.Single();
            outIpPermissions.Ipv4Ranges.Should().OnlyContain(permission => permission.CidrIp.Equals("0.0.0.0/0") && permission.Description.Equals("Allow all outbound traffic by default"));

            var inIpPermissions = securityGroupInfo.IpPermissions;

            var port80Permission = inIpPermissions.Single(permission => permission.FromPort.Equals(80));
            port80Permission.Ipv4Ranges.Should().OnlyContain(range => range.CidrIp.Equals("0.0.0.0/0") && range.Description.Equals("HTTP from Internet"));

            var port22Permission = inIpPermissions.Single(permission => permission.FromPort.Equals(22));
            port22Permission.Ipv4Ranges.Should().OnlyContain(range => range.CidrIp.Equals("0.0.0.0/0") && range.Description.Equals("SSH from Internet"));
        }

        [Test]
        public void VerifyPrivateInstanceSecurityGroups()
        {
            EC2ApiClientWrapper eC2ApiClientWrapper = new EC2ApiClientWrapper();
            var actualSecurityGroupInfoResponse = eC2ApiClientWrapper.GetSecurityGroupInfo(EC2InstancesConfigurationStorage.PrivateInstanceSecurityGroupId);

            var securityGroupInfo = actualSecurityGroupInfoResponse.SecurityGroups.Single();
            var outIpPermissions = securityGroupInfo.IpPermissionsEgress.Single();
            outIpPermissions.Ipv4Ranges.Should().OnlyContain(permission => permission.CidrIp.Equals("0.0.0.0/0") && permission.Description.Equals("Allow all outbound traffic by default"));

            var inIpPermissions = securityGroupInfo.IpPermissions;

            var port80Permission = inIpPermissions.Single(permission => permission.FromPort.Equals(80));
            port80Permission.Ipv4Ranges.Should().BeNullOrEmpty();
            port80Permission.Ipv6Ranges.Should().BeNullOrEmpty();
            port80Permission.UserIdGroupPairs.Should().OnlyContain(usidgroup => usidgroup.GroupId.Equals(EC2InstancesConfigurationStorage.PublicInstanceSecurityGroupId) && usidgroup.Description.Equals("HTTP from Internet"));

            var port22Permission = inIpPermissions.Single(permission => permission.FromPort.Equals(22));
            port22Permission.Ipv4Ranges.Should().BeNullOrEmpty();
            port22Permission.Ipv6Ranges.Should().BeNullOrEmpty();
            port22Permission.UserIdGroupPairs.Should().OnlyContain(usidgroup => usidgroup.GroupId.Equals(EC2InstancesConfigurationStorage.PublicInstanceSecurityGroupId) && usidgroup.Description.Equals("SSH from Internet"));
        }

        [Test]
        public void PublicApplicationFunctionalValidation()
        {
            CloudxInfoApiClient CloudxInfoApiClient = new CloudxInfoApiClient();
            InstanceMetadataModel actualInstanceMetaData = CloudxInfoApiClient.GetIntanceMetaData("http://" + EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns + "/");
            actualInstanceMetaData.Should().BeEquivalentTo(EC2MetadataStorage.PublicInstanceExpectedMetadata);
        }

        [Test]
        public void PrivateApplicationFunctionalValidation()
        {
            var connectionInfo = new ConnectionInfo(EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns, EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

            SshCommand response = sshClient.RunCommand($"curl {EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns}");
            string curlResult = response.Result;

            var actualInstanceMetaData = JsonConvert.DeserializeObject<InstanceMetadataModel>(curlResult);
            actualInstanceMetaData.Should().BeEquivalentTo(EC2MetadataStorage.PrivateInstanceExpectedMetadata);
        }
    }
}
