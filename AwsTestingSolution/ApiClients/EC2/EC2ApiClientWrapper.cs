
using Amazon.EC2;
using Amazon.EC2.Model;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.EC2
{
    public class EC2ApiClientWrapper : AwsApiClientBase
    {
        public EC2ApiClientWrapper() => GetAwsCredentials();
        private AmazonEC2Client EC2Client => new AmazonEC2Client(awsCredentials, AwsConfig.EC2Config);

        public DescribeVpcsResponse DescribeVpcs()
        {
            Task<DescribeVpcsResponse> describeVpscResponseTask = EC2Client.DescribeVpcsAsync();
            return describeVpscResponseTask.Result;
        }

        public DescribeSubnetsResponse DescribeSubnetsForVpc(params string[] vpcsIds)
        {
            Task<DescribeSubnetsResponse> describeSubnetsResponseTask = EC2Client.DescribeSubnetsAsync(new DescribeSubnetsRequest
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Name = "vpc-id",
                        Values = vpcsIds.ToList()
                    }
                }
            });
            return describeSubnetsResponseTask.Result;
        }

        public Vpc GetVpcByName(string vpcnName)
        {
            var vpscResponce = DescribeVpcs();
            return vpscResponce.Vpcs.Single(vpc => vpc.Tags.Any(tag => tag.Value.Equals(vpcnName)));
        }

        public IEnumerable<Instance> GetAllDeployedInstances()
        {
            var request = new DescribeInstancesRequest();
            var response = EC2Client.DescribeInstancesAsync(request);
            IEnumerable<Instance> actualInstancesDeployed = response.Result.Reservations.SelectMany(reservation => reservation.Instances);
            return actualInstancesDeployed;
        }

        public DescribeSecurityGroupsResponse GetSecurityGroupInfo(string securityGroupId)
        {
            var request = new DescribeSecurityGroupsRequest
            {
                GroupIds = new List<string> { securityGroupId }
            };
            var response = EC2Client.DescribeSecurityGroupsAsync(request);
            return response.Result;
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
