using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using NUnit.Framework;


namespace AwsTestingSolution.EC2
{
    [TestFixture]
    public class EC2Scenarios
    {
        [Test]
        public void GetListOfInstances()
        {
            var profileName = "default";

            var config = new AmazonEC2Config
            {
                RegionEndpoint = RegionEndpoint.USEast1 // Replace with your desired region
            };

            var chain = new CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
            {
                using (var client = new AmazonEC2Client(awsCredentials, config))
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
                            // Add more properties as needed
                        }
                    }
                }
            }

        }
    }
}
