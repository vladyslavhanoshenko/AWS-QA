using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using NUnit.Framework;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.EC2
{
    [TestFixture]
    public class EC2Scenarios
    {
        [Test]
        public void GetListOfInstances()
        {
            var chain = new CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(AwsConfig.ProfileName, out AWSCredentials awsCredentials))
            {
                using (var client = new AmazonEC2Client(awsCredentials, AwsConfig.Config))
                {
                    var request = new DescribeInstancesRequest();

                    var response = client.DescribeInstancesAsync(request);

                    foreach (var reservation in response.Result.Reservations)
                    {
                        foreach (var instance in reservation.Instances)
                        {
                            Console.WriteLine($"Instance ID: {instance.InstanceId}");
                            Console.WriteLine($"Instance Type: {instance.InstanceType}");
                            Console.WriteLine($"Launch Time: {instance.LaunchTime}");
                            Console.WriteLine($"State: {instance.State.Name}");
                        }
                    }
                }
            }

        }
    }
}
