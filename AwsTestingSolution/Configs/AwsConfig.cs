using Amazon;
using Amazon.EC2;

namespace AwsTestingSolution.Configs
{
    public class AwsConfig
    {
        public const string ProfileName = "default";

        public static AmazonEC2Config Config = new AmazonEC2Config
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };
    }
}
