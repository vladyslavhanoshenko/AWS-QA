using Amazon;
using Amazon.EC2;
using Amazon.S3;

namespace AwsTestingSolution.Configs
{
    public class AwsConfig
    {
        public const string ProfileName = "default";

        public static AmazonEC2Config EC2Config = new AmazonEC2Config
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };

        public static AmazonS3Config S3Config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };
    }
}
