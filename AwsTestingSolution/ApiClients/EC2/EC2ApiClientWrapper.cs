using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.EC2
{
    public class EC2ApiClientWrapper
    {
        private AWSCredentials awsCredentials;
        public EC2ApiClientWrapper() => GetAwsCredentials();
        private AmazonEC2Client EC2Client => new AmazonEC2Client(awsCredentials, AwsConfig.Config);

        private void GetAwsCredentials()
        {
            var chain = new CredentialProfileStoreChain();
            bool IsCredentialsReceived = chain.TryGetAWSCredentials(AwsConfig.ProfileName, out awsCredentials);
            if (!IsCredentialsReceived) throw new Exception("Credentials are not correct. Please check profile or credentials in AWS CLI");
        }

        public IEnumerable<Instance> GetAllDeployedInstances()
        {
            var request = new DescribeInstancesRequest();
            var response = EC2Client.DescribeInstancesAsync(request);
            IEnumerable<Instance> actualInstancesDeployed = response.Result.Reservations.SelectMany(reservation => reservation.Instances);
            return actualInstancesDeployed;
        }

        public int GetRootDeviceVolume(string instanceId, string deviceName = "/dev/xvda")
        {
            var describeVolumesRequest = new DescribeVolumesRequest
            {
                Filters = new List<Filter>
                    {
                        new Filter("attachment.instance-id", new List<string>{ instanceId }),
                        new Filter("attachment.device", new List<string>{ deviceName })
                    }
            };
            Task<DescribeVolumesResponse> describeVolumesResponse = EC2Client.DescribeVolumesAsync(describeVolumesRequest);
            Volume? rootVolume = describeVolumesResponse.Result.Volumes.FirstOrDefault();
            return rootVolume.Size;
        }
    }
}
