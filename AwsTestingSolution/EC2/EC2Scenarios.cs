using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using NUnit.Framework;
using AwsTestingSolution.Configs;
using AwsTestingSolution.Mappers;
using FluentAssertions;
using AwsTestingSolution.Storages;

namespace AwsTestingSolution.EC2
{
    [TestFixture]
    public class EC2Scenarios
    {
        [Test]
        public void GetListOfInstances()
        {
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            bool IsCredentialsReceived = chain.TryGetAWSCredentials(AwsConfig.ProfileName, out awsCredentials);
            if (!IsCredentialsReceived) throw new Exception("Credentials are not correct. Please check profile or credentials in AWS CLI");

            var client = new AmazonEC2Client(awsCredentials, AwsConfig.Config);
            var request = new DescribeInstancesRequest();
            var response = client.DescribeInstancesAsync(request);
            IEnumerable<Instance> actualInstancesDeployed = response.Result.Reservations.SelectMany(reservation => reservation.Instances);
            var actualMappedInstancesDeployed = actualInstancesDeployed.Select(instance => EC2Mapper.MapInstanceToEC2InstanceModel(instance)).ToList();
            actualMappedInstancesDeployed.ForEach(i => i.RootBlockDeviceSize = "8");

            foreach (Instance instance in actualInstancesDeployed)
            {
                var describeVolumesRequest = new DescribeVolumesRequest
                {
                    Filters = new List<Filter>
                    {
                        new Filter("attachment.instance-id", new List<string>{ instance.InstanceId }),
                        new Filter("attachment.device", new List<string>{ instance.RootDeviceName }) 
                    }
                };

                var describeVolumesResponse = client.DescribeVolumesAsync(describeVolumesRequest);

                var rootVolume = describeVolumesResponse.Result.Volumes.FirstOrDefault();
            }


            actualMappedInstancesDeployed.Should().BeEquivalentTo(EC2DataStorage.ExpectedDeployedInstances);


        }
    }
}
