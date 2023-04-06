using AwsTestingSolution.Models;

namespace AwsTestingSolution.Storages
{
    public class EC2DataStorage
    {
        public static EC2InstanceModel[] ExpectedDeployedInstances => new EC2InstanceModel[] { PublicInstanceExpectedData, PrivateInstanceExpectedData };

        public static EC2InstanceModel PublicInstanceExpectedData = new EC2InstanceModel
        {
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PublicInstance/Instance" },
            RootBlockDeviceSize = "8",
            InstanceOs = "Linux/UNIX",
            PublicDns = "ec2-54-159-168-192.compute-1.amazonaws.com",
            PublicIp = "54.159.168.192"
        };

        public static EC2InstanceModel PrivateInstanceExpectedData = new EC2InstanceModel
        {
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PrivateInstance/Instance" },
            RootBlockDeviceSize = "8",
            InstanceOs = "Linux/UNIX",
        };
    }
}
