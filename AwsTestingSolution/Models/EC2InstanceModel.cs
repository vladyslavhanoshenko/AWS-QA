namespace AwsTestingSolution.Models
{
    public class EC2InstanceModel
    {
        public string InstanceId { get; set; }
        public string InstanceType { get; set; }
        public string[] InstanceTags { get; set; }
        public int RootBlockDeviceSize { get; set; }
        public string InstanceOs { get; set; }
        public string PublicIp { get; set; }
        public string PublicDns { get; set; }
    }
}
