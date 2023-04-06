using Amazon.EC2.Model;
using AwsTestingSolution.Models;

namespace AwsTestingSolution.Mappers
{
    public class EC2Mapper
    {
        public static EC2InstanceModel MapInstanceToEC2InstanceModel(Instance instance)
        {
            return new EC2InstanceModel
            {
                InstanceId = instance.InstanceId,
                InstanceType = instance.InstanceType,
                InstanceTags = instance.Tags.Where(tag => tag.Key.Equals("Name") || tag.Key.Equals("cloudx")).Select(i => i.Value).ToArray(),
                InstanceOs = instance.PlatformDetails,
                PublicIp = instance.PublicIpAddress,
                PublicDns = instance.PublicDnsName
            };

        }
    }
}
