using AwsTestingSolution.ApiClients.EC2.Models;

namespace AwsTestingSolution.Storages
{
    public class EC2InstancesConfigurationStorage
    {
        public static EC2InstanceModel[] ExpectedDeployedInstances => new EC2InstanceModel[] { PublicInstanceExpectedData, PrivateInstanceExpectedData };

        public static EC2InstanceModel PublicInstanceExpectedData = new EC2InstanceModel
        {
            InstanceId = "i-058f53810c4397292",
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PublicInstance/Instance" },
            RootBlockDeviceSize = 8,
            InstanceOs = "Linux/UNIX",
            PublicDns = "ec2-54-159-168-192.compute-1.amazonaws.com",
            PublicIp = "54.159.168.192"
        };

        public static EC2InstanceModel PrivateInstanceExpectedData = new EC2InstanceModel
        {
            InstanceId = "i-0765cdff5ff6f7070",
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PrivateInstance/Instance" },
            RootBlockDeviceSize = 8,
            InstanceOs = "Linux/UNIX",
        };
    }
}
