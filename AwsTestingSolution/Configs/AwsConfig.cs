using Amazon;
using Amazon.EC2;
using Amazon.RDS;
using Amazon.S3;

namespace AwsTestingSolution.Configs
{
    public class AwsConfig
    {
        public const string ProfileName = "default";
        private static RegionEndpoint _regionEndpoint = RegionEndpoint.USEast1;

        public static AmazonEC2Config EC2Config = new AmazonEC2Config
        {
            RegionEndpoint = _regionEndpoint
        };

        public static AmazonS3Config S3Config = new AmazonS3Config
        {
            RegionEndpoint = _regionEndpoint
        };

        public static AmazonRDSConfig RdsConfig = new AmazonRDSConfig
        {
            RegionEndpoint = _regionEndpoint
        };
    }
}
