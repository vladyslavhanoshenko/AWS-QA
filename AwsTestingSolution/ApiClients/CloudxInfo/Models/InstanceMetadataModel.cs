using Newtonsoft.Json;

namespace AwsTestingSolution.ApiClients.CloudxInfo.Models
{
    public class InstanceMetadataModel
    {
        [JsonProperty(PropertyName = "availability_zone")]
        public string AvailabilityZone { get; set; }

        [JsonProperty(PropertyName = "private_ipv4")]
        public string PrivateIpv4 { get; set; }

        public string Region { get; set; }
    }
}
