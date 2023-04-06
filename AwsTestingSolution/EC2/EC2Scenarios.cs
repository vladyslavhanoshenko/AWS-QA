using Amazon.EC2.Model;
using NUnit.Framework;
using AwsTestingSolution.Mappers;
using FluentAssertions;
using AwsTestingSolution.Storages;
using AwsTestingSolution.ApiClients.EC2;
using AwsTestingSolution.ApiClients.CloudxInfo;
using AwsTestingSolution.ApiClients.CloudxInfo.Models;

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
        public void ApplicationFunctionalValidation()
        {
            CloudxInfoApiClient CloudxInfoApiClient = new CloudxInfoApiClient();
            InstanceMetadataModel actualInstanceMetaData = CloudxInfoApiClient.GetIntanceMetaData("http://" + EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns + "/");
            actualInstanceMetaData.Should().BeEquivalentTo(EC2MetadataStorage.PublicInstanceExpectedMetadata);
        }
    }
}
