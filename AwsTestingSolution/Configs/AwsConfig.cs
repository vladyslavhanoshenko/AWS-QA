using Amazon;
using Amazon.EC2;
using Amazon.RDS;
using Amazon.S3;

namespace AwsTestingSolution.Configs
{
    public class AwsConfig
    {
        public const string ProfileName = "default";
        public static RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        public static AmazonEC2Config EC2Config = new AmazonEC2Config
        {
            RegionEndpoint = RegionEndpoint
        };

        public static AmazonS3Config S3Config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint
        };

        public static AmazonRDSConfig RdsConfig = new AmazonRDSConfig
        {
            RegionEndpoint = RegionEndpoint
        };
    }
}
