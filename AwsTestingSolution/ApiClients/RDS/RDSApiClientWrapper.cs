using Amazon.RDS;
using Amazon.RDS.Model;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.RDS
{
    public class RDSApiClientWrapper : AwsApiClientBase
    {
        public RDSApiClientWrapper() => GetAwsCredentials();
        private AmazonRDSClient RdsClient => new AmazonRDSClient(awsCredentials, AwsConfig.RdsConfig);

        public DescribeDBInstancesResponse GetDbInstances()
        {
            var request = new DescribeDBInstancesRequest();

            var response = RdsClient.DescribeDBInstancesAsync(request);
            return response.Result;
        }
    }
}
