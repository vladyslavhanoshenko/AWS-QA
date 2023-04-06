using AwsTestingSolution.ApiClients.CloudxInfo.Models;

namespace AwsTestingSolution.Storages
{
    public class EC2MetadataStorage
    {
        public static InstanceMetadataModel PublicInstanceExpectedMetadata = new InstanceMetadataModel
        {
            AvailabilityZone = "us-east-1a",
            PrivateIpv4 = "10.0.2.135",
            Region = "us-east-1"
        };
    }
}
